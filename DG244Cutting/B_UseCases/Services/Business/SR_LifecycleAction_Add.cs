using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de l'inscription, au journal métier
    /// <see cref="LifecycleAction"/>, d'une entrée décrivant une action structurante du
    /// parcours de production, située dans le contexte applicatif courant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside
    /// en <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne
    /// doit jamais être instancié directement. Il est consommé par les UseCases orchestrateurs
    /// du parcours de découpe via son interface <see cref="IS_LifecycleAction_Add"/>, et
    /// consomme directement <see cref="IC_Generic{T}"/> pour l'entité
    /// <see cref="LifecycleAction"/>.
    /// </para>
    /// <para>
    /// Objectif : rendre retraçables les décisions qui jalonnent le parcours d'une série —
    /// quelle action, sur quelle entité, par qui et quand. Le journal métier enregistre des
    /// événements du cycle de vie des produits et se distingue du journal technique des
    /// incidents d'exécution. Le service en est le point d'écriture unique au sein de
    /// l'application : la table, partagée dans la base de production, peut être alimentée par
    /// d'autres applications, mais aucun autre composant de l'application n'y écrit.
    /// </para>
    /// <para>
    /// Répartition des rôles : l'appelant décrit l'événement et transmet le contexte
    /// applicatif en un objet unique (<see cref="DTO_AppContext"/>) ; le service situe
    /// l'événement en portant seul la correspondance entre ce contexte et les champs de
    /// contexte de l'entrée, de sorte que cette correspondance n'est jamais reproduite chez les
    /// appelants. Il assume également, pour <see cref="En_LifecycleActionSource"/> et
    /// <see cref="En_LifecycleActionType"/>, le rejet des valeurs non déclarées et la
    /// conversion explicite vers les identifiants <see langword="short"/> de l'entrée.
    /// </para>
    /// <para>
    /// Cohérence source / type non contrôlée : la correspondance admissible entre une source et
    /// une nature d'action n'est pas portée par le modèle de données ; l'inscrire ici en ferait
    /// une seconde source de vérité, à maintenir à chaque évolution des deux référentiels. Le
    /// risque assumé est qu'un couple incohérent soit inscrit sans être signalé ; les couples
    /// étant écrits en littéraux au point d'appel, leur cohérence se vérifie à la lecture du
    /// code appelant.
    /// </para>
    /// <para>
    /// Deux horodatages distincts : <see cref="LifecycleAction.ActionTimestamp"/> est
    /// l'horodatage métier de l'action, issu du contexte applicatif ;
    /// <see cref="LifecycleAction.CreatedAt"/> est l'horodatage technique de l'insertion,
    /// positionné par le Command Handler générique.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier les préconditions structurelles du contexte, des valeurs d'énumération, de l'identifiant de l'entité et du commentaire.</description></item>
    /// <item><description>Construire l'entrée <see cref="LifecycleAction"/> à partir de la description de l'événement et du contexte applicatif.</description></item>
    /// <item><description>Déléguer l'écriture au Command Handler générique via <see cref="IC_Generic{T}.HandleAddAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à l'appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : l'entrée partage le sort de la transaction métier du UseCase appelant.</description></item>
    /// <item><description>Ne modifie ni ne supprime jamais une entrée existante : le journal est alimenté en insertion seule.</description></item>
    /// <item><description>Ne lit pas le contexte applicatif et n'injecte aucune interface <c>ISE_</c> : le contexte lui est fourni par argument depuis le UseCase.</description></item>
    /// <item><description>Ne contrôle ni la cohérence du couple source / type, ni l'existence de l'entité désignée.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Command Handler.</description></item>
    /// <item><description>Ne journalise ni ne notifie : ces rôles relèvent du pipeline terminal du UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_LifecycleAction_Add"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_LifecycleAction_Add : IS_LifecycleAction_Add
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Longueur maximale admise pour le commentaire, alignée sur la capacité de la colonne
        /// <c>Comments</c> du journal métier.
        /// </summary>
        private const int CommentsMaxLength = 500;

        /// <summary>
        /// Nom du type concret, utilisé comme segment propre dans les CallChains construites
        /// par le service.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Command Handler générique auquel est déléguée l'écriture de l'entrée de journal.
        /// </summary>
        private readonly IC_Generic<LifecycleAction> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_LifecycleAction_Add"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la
        /// portée de l'invocation, afin de partager le contexte de données du UseCase
        /// orchestrateur à travers le Command Handler.
        /// </para>
        /// </remarks>
        /// <param name="commandHandler">Command Handler générique consommé pour l'écriture de l'entrée de journal. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_LifecycleAction_Add(
            IC_Generic<LifecycleAction> commandHandler,
            IS_ExClassifier classifier)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Construit l'entrée de journal métier décrivant une action du cycle de vie sur une
        /// entité désignée, la situe dans le contexte applicatif fourni, puis la confie au
        /// Command Handler générique sans la persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur du parcours de production, à
        /// l'intérieur de la transaction qu'il a ouverte. Le Command Handler générique
        /// positionne la date de création, inscrit l'entrée dans le suivi du contexte partagé
        /// et enregistre l'événement associé ; l'enregistrement effectif n'intervient qu'à la
        /// validation de la transaction par l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier, dans l'ordre : présence du contexte applicatif, identifiants d'application et d'utilisateur strictement positifs, source et nature d'action déclarées, identifiant de l'entité strictement positif.</description></item>
        /// <item><description>Normaliser le commentaire (valeur <see langword="null"/>, vide ou blanche ramenée à <see langword="null"/> ; toute autre valeur conservée telle quelle), puis rejeter un commentaire normalisé excédant 500 caractères plutôt que de le tronquer.</description></item>
        /// <item><description>Reporter sur l'entrée la source et la nature d'action converties en <see langword="short"/>, l'identifiant de l'entité, le commentaire normalisé, l'application, l'utilisateur, l'horodatage métier et les informations de poste.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne contrôle ni la cohérence du couple source / type, ni la longueur des informations de poste.</description></item>
        /// <item><description>Laisse à leur valeur par défaut l'identifiant, la date de mise à jour et l'indicateur de suppression logique ; la date de création est positionnée par le Command Handler.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne relit pas l'entrée après écriture.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="appContext">
        /// Contexte applicatif courant fournissant l'application, l'utilisateur, l'horodatage
        /// métier et les informations de poste. Ne doit pas être <see langword="null"/> ; ses
        /// identifiants d'application et d'utilisateur doivent être strictement positifs.
        /// </param>
        /// <param name="source">Entité métier sur laquelle porte l'action. Doit être une valeur déclarée de l'énumération.</param>
        /// <param name="type">Nature de l'action du cycle de vie. Doit être une valeur déclarée de l'énumération.</param>
        /// <param name="idSource">
        /// Identifiant technique de l'entité concernée dans la table désignée par
        /// <paramref name="source"/>. Doit être strictement positif : la colonne portant une
        /// valeur par défaut en base, une valeur nulle serait omise à l'insertion et remplacée
        /// silencieusement par cette valeur par défaut.
        /// </param>
        /// <param name="comments">
        /// Commentaire libre facultatif. Après normalisation, sa longueur ne doit pas excéder
        /// 500 caractères. Par défaut <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant l'opération asynchrone, sans valeur de retour.</returns>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="appContext"/> est
        /// <see langword="null"/> ; avec le code <c>BU_ER_02</c> si l'identifiant d'application
        /// ou d'utilisateur du contexte n'est pas strictement positif, si
        /// <paramref name="source"/> ou <paramref name="type"/> n'est pas une valeur déclarée, si
        /// <paramref name="idSource"/> n'est pas strictement positif, ou si le commentaire
        /// normalisé excède 500 caractères. Remonte également sans interception toute
        /// <see cref="Ex_Business"/> levée par le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si l'écriture échoue lors de la délégation au Command Handler.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            DTO_AppContext appContext,
            En_LifecycleActionSource source,
            En_LifecycleActionType type,
            int idSource,
            string? comments = null,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // P1 - Contexte applicatif obligatoire.
                if (appContext is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le contexte applicatif (appContext) est obligatoire pour l'inscription d'une action de cycle de vie.");

                // P2 - Identifiant d'application strictement positif.
                if (appContext.AppId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'application du contexte (appContext.AppId) doit être strictement positif ; valeur reçue : {appContext.AppId}.");

                // P3 - Identifiant d'utilisateur strictement positif.
                if (appContext.AppUserId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'utilisateur du contexte (appContext.AppUserId) doit être strictement positif ; valeur reçue : {appContext.AppUserId}.");

                // P4 - Source déclarée (l'énumération n'a pas de sentinelle : 0 n'est pas un membre).
                if (!Enum.IsDefined(source))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"La source de l'action (source) doit être une valeur déclarée de En_LifecycleActionSource ; valeur reçue : {(short)source}.");

                // P5 - Nature d'action déclarée (l'énumération n'a pas de sentinelle : 0 n'est pas un membre).
                if (!Enum.IsDefined(type))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"La nature de l'action (type) doit être une valeur déclarée de En_LifecycleActionType ; valeur reçue : {(short)type}.");

                // P6 - Identifiant de l'entité strictement positif : la colonne IdSource porte une
                // valeur par défaut en base, qu'EF Core substituerait silencieusement à une valeur 0.
                if (idSource <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de l'entité concernée (idSource) doit être strictement positif ; valeur reçue : {idSource}.");

                // N1 - Normalisation : null, vide ou blanc devient null ; sinon conservé tel quel, sans rognage.
                string? normalizedComments = string.IsNullOrWhiteSpace(comments) ? null : comments;

                // P7 - Rejet plutôt que troncature, afin que le motif ne soit jamais altéré.
                if (normalizedComments is not null && normalizedComments.Length > CommentsMaxLength)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"Le commentaire (comments) ne doit pas excéder {CommentsMaxLength} caractères ; longueur reçue : {normalizedComments.Length}.");

                ct.ThrowIfCancellationRequested();

                // Id, UpdatedAt et IsDeleted conservent leur valeur par défaut ;
                // CreatedAt est positionné par le Command Handler générique.
                var action = new LifecycleAction
                {
                    IdLifecycleActionSource = (short)source,
                    IdLifecycleActionType = (short)type,
                    IdSource = idSource,
                    Comments = normalizedComments,
                    IdApplication = appContext.AppId,
                    IdUser = appContext.AppUserId,

                    // Horodatage métier de l'action, distinct de l'horodatage technique CreatedAt.
                    ActionTimestamp = appContext.AppDateTime,

                    DeviceUser = appContext.AppDeviceUser,
                    DeviceId = appContext.AppDeviceId,
                    DeviceIp = appContext.AppDeviceIP
                };

                await _commandHandler.HandleAddAsync(callChain, action, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}