using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de la mise à l'écart définitive, hors du circuit de production,
    /// d'une barre de production désignée, avec conservation du motif de sa mise à l'écart.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionBar_Reject"/>, par les UseCases orchestrateurs du traitement d'une
    /// barre de production : lors du refus d'une barre avant son acceptation, avec un motif choisi
    /// par l'opérateur, et lors de l'issue en barre-déchet d'une barre acceptée sur laquelle aucune
    /// pièce n'a pu être placée, avec un motif fourni par le UseCase. Il consomme directement
    /// <see cref="IQ_Generic{T}"/> pour la lecture préalable de la barre et
    /// <see cref="IC_Generic{T}"/> pour sa suppression logique, sur l'entité
    /// <see cref="ProductionBar"/>.
    /// </para>
    /// <para>
    /// Objectif : l'approvisionnement de l'atelier s'effectue à la demande, et une barre désignée
    /// peut se révéler inutilisable. Qu'elle soit refusée ou déclarée en déchet, la barre doit
    /// sortir du circuit de production de manière définitive et conserver la raison de sa mise à
    /// l'écart. Le service inscrit le motif sur la barre, puis délègue sa suppression logique au
    /// Command Handler générique ; les recherches du circuit de production excluant les barres
    /// supprimées logiquement, la barre n'est plus jamais présentée à l'opérateur. Le service
    /// n'expose aucune logique de persistance et n'assume aucune responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Conditions d'état : une barre peut être écartée si elle n'est pas déjà supprimée
    /// logiquement, n'est pas épuisée, n'est pas en rupture de stock et n'est pas scellée dans le
    /// plan. Le placement définitif dans le plan marque le point de non-retour. L'acceptation
    /// physique de la barre n'est pas une condition : elle est absente sur un refus et acquise sur
    /// une barre-déchet. Le placement provisoire n'en est pas une non plus : il demeure positionné
    /// pendant toute la vie de la barre et n'est ni lu ni modifié.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier les préconditions structurelles des arguments.</description></item>
    /// <item><description>Charger la barre désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/>.</description></item>
    /// <item><description>Vérifier l'existence de la barre et la compatibilité de son état avec sa mise à l'écart.</description></item>
    /// <item><description>Inscrire le motif de mise à l'écart dans le champ <c>RejectionReason</c> de la barre.</description></item>
    /// <item><description>Déléguer la suppression logique au Command Handler générique via <see cref="IC_Generic{T}.HandleSoftDeleteAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne positionne jamais lui-même l'indicateur de suppression logique ni la date de mise à jour : ils relèvent du Command Handler générique.</description></item>
    /// <item><description>Ne modifie aucun autre champ que <c>RejectionReason</c> : les indicateurs de placement provisoire, d'acceptation, d'épuisement, de rupture et de placement définitif, le nombre de découpes, les bornes de défauts, la longueur et la qualification du reliquat ainsi que les autres champs de reliquat, le code-barres et l'emplacement de la chute générée et la date de création restent intacts.</description></item>
    /// <item><description>Ne distingue pas le refus de la barre-déchet et ne contrôle pas le motif au regard d'un référentiel de motifs.</description></item>
    /// <item><description>Ne traite pas les découpes affectées à la barre, ne statue pas sur le devenir de la chute dont la barre est éventuellement issue et n'inscrit aucune action de cycle de vie : ces traitements relèvent d'autres services invoqués par le UseCase.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionBar_Reject"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionBar_Reject : IS_ProductionBar_Reject
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Longueur maximale admise pour le motif de mise à l'écart.
        /// </summary>
        /// <remarks>
        /// Reflète la colonne <c>ProductionBar.RejectionReason</c>, de type <c>nvarchar(500)</c> en
        /// base DIGIT_TRY. La limite se mesure par <see cref="string.Length"/>, dont les unités
        /// UTF-16 coïncident avec le décompte de la colonne.
        /// </remarks>
        private const int RejectionReasonMaxLength = 500;

        /// <summary>
        /// Nom du type concret, utilisé comme segment propre dans les CallChains construites
        /// par le service.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Query Handler générique auquel est déléguée la lecture suivie de la barre désignée.
        /// </summary>
        private readonly IQ_Generic<ProductionBar> _queryHandler;

        /// <summary>
        /// Command Handler générique auquel est déléguée la suppression logique de la barre écartée.
        /// </summary>
        private readonly IC_Generic<ProductionBar> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionBar_Reject"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; la barre lue et modifiée est ainsi l'instance que le
        /// Command Handler retrouve et que le contexte enregistrera.
        /// </para>
        /// </remarks>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture suivie de la barre. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la suppression logique de la barre. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="queryHandler"/>, <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionBar_Reject(
            IQ_Generic<ProductionBar> queryHandler,
            IC_Generic<ProductionBar> commandHandler,
            IS_ExClassifier classifier)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Écarte définitivement du circuit de production la barre désignée, en inscrivant le motif
        /// reçu sur la barre, puis confie sa suppression logique au Command Handler générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, lors du refus d'une barre avant son acceptation ou lors de l'issue en
        /// barre-déchet d'une barre acceptée. La barre est lue avec suivi des changements ; le motif
        /// est inscrit sur cette instance avant la délégation, afin que l'événement technique inscrit
        /// par le Command Handler le porte. Le Command Handler retrouve la même instance dans le
        /// contexte partagé, positionne l'indicateur de suppression logique et la date de mise à jour
        /// et inscrit l'événement. L'enregistrement effectif n'intervient qu'à la validation de la
        /// transaction par l'appelant.
        /// </para>
        /// <para>
        /// Transactionnalité : un échec survenant avant l'inscription du motif laisse la barre
        /// intacte. Un échec survenant après cette inscription laisse une modification en mémoire
        /// sur l'instance suivie ; l'appelant ne doit alors pas enregistrer le contexte et annule la
        /// transaction.
        /// </para>
        /// <para>
        /// Objectif : retirer la barre du circuit de production de manière définitive, en
        /// conservant la raison de sa mise à l'écart.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier, dans l'ordre : identifiant de barre strictement positif, motif non nul, non vide et non composé uniquement d'espaces, longueur du motif au plus égale à la longueur autorisée.</description></item>
        /// <item><description>Charger la barre désignée en lecture suivie.</description></item>
        /// <item><description>Vérifier que la barre a été trouvée.</description></item>
        /// <item><description>Vérifier l'état de la barre et rejeter en un échec unique l'ensemble des conditions violées, dans l'ordre : déjà refusée ou supprimée logiquement, épuisée, en rupture de stock, scellée dans le plan.</description></item>
        /// <item><description>Positionner <c>RejectionReason</c> au motif reçu, sans suppression d'espaces ni autre transformation.</description></item>
        /// <item><description>Déléguer la suppression logique de la barre au Command Handler générique, par son identifiant.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne contrôle ni ne modifie l'acceptation physique ni le placement provisoire de la barre.</description></item>
        /// <item><description>Ne positionne ni l'indicateur de suppression logique ni la date de mise à jour, et n'invoque pas la mise à jour générique.</description></item>
        /// <item><description>Ne modifie aucun autre champ que le motif et n'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production à écarter. Doit être strictement positif.</param>
        /// <param name="rejectionReason">Motif de mise à l'écart. Ne doit être ni <see langword="null"/>, ni vide, ni composé uniquement d'espaces ; sa longueur ne doit pas excéder 500 caractères. Il est inscrit tel que reçu.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="rejectionReason"/> est
        /// <see langword="null"/>, vide ou composé uniquement d'espaces (paramètre nommé) ; avec le
        /// code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas strictement positif
        /// (paramètre nommé et valeur reçue citée) ou si <paramref name="rejectionReason"/> excède la
        /// longueur autorisée (paramètre nommé, longueur reçue et limite citées) ; avec le code
        /// <c>BU_ER_03</c> si la barre désignée est introuvable (identifiant cité) ; avec le code
        /// <c>BU_ER_04</c>, en un échec unique, si la barre est déjà supprimée logiquement, épuisée,
        /// en rupture de stock ou scellée dans le plan (barre et chaque condition violée citées).
        /// Remonte également sans interception toute <see cref="Ex_Business"/> levée par le Query
        /// Handler ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la barre ou la délégation de sa suppression logique échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idProductionBar,
            string rejectionReason,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // P1 - Identifiant de barre strictement positif.
                if (idProductionBar <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de barre de production (idProductionBar) doit être strictement positif ; valeur reçue : {idProductionBar}.");

                // P2 - Motif obligatoire : ni nul, ni vide, ni composé uniquement d'espaces.
                if (string.IsNullOrWhiteSpace(rejectionReason))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le motif de mise à l'écart (rejectionReason) est obligatoire et ne peut être nul, vide ou composé uniquement d'espaces.");

                // P3 - Longueur du motif bornée par la colonne de destination.
                if (rejectionReason.Length > RejectionReasonMaxLength)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"Le motif de mise à l'écart (rejectionReason) excède la longueur autorisée ; longueur reçue : {rejectionReason.Length}, limite : {RejectionReasonMaxLength}.");

                ct.ThrowIfCancellationRequested();

                // L - Lecture SUIVIE : l'instance chargée est celle que le Command Handler retrouvera
                // par l'identity map du contexte partagé. Aucune variante AsNoTracking.
                ProductionBar? bar = await _queryHandler.HandleGetByIdAsync(
                    callChain,
                    idProductionBar,
                    ct);

                // C1 - La barre désignée doit exister.
                if (bar is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La barre de production désignée est introuvable ; identifiant introuvable : {idProductionBar}.");

                // C2 - État compatible avec la mise à l'écart, toutes les conditions violées étant
                // citées en un échec unique.
                string? violation = DescribeStateViolation(bar);
                if (violation is not null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"L'état de la barre de production {bar.Id} ne permet pas sa mise à l'écart : {violation}.");

                // M - Écriture métier unique, posée avant la délégation afin que l'événement Event
                // Store inscrit par le Command Handler porte le motif. Aucune transformation.
                bar.RejectionReason = rejectionReason;

                // D - Délégation de la suppression logique ; IsDeleted, UpdatedAt et événement
                // Event Store relèvent exclusivement du Command Handler générique.
                await _commandHandler.HandleSoftDeleteAsync(callChain, idProductionBar, ct);
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

        /// <summary>
        /// Décrit les conditions d'état qui interdisent la mise à l'écart d'une barre de production,
        /// ou indique qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée sur la barre chargée, avant toute modification. Une barre peut être
        /// écartée si elle n'est pas déjà supprimée logiquement, n'est pas épuisée, n'est pas en
        /// rupture de stock et n'est pas scellée dans le plan. L'acceptation physique et le placement
        /// provisoire ne sont pas contrôlés. Toutes les conditions violées sont citées ensemble, dans
        /// cet ordre, afin que l'échec renseigne complètement l'appelant.
        /// </para>
        /// </remarks>
        /// <param name="bar">Barre chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Énumération des conditions violées, prête à être citée dans le message d'échec ;
        /// <see langword="null"/> si l'état de la barre permet sa mise à l'écart.
        /// </returns>
        private static string? DescribeStateViolation(ProductionBar bar)
        {
            List<string> conditions = new();

            if (bar.IsDeleted)
                conditions.Add("déjà refusée ou supprimée logiquement");

            if (bar.IsUsed)
                conditions.Add("épuisée");

            if (bar.IsOutOfStock)
                conditions.Add("en rupture de stock");

            if (bar.IsOptimized)
                conditions.Add("scellée dans le plan");

            return conditions.Count == 0
                ? null
                : string.Join(", ", conditions);
        }

        #endregion
    }
}