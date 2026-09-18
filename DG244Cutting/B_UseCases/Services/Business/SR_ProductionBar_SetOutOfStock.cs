using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de la déclaration et de la libération de la rupture de stock
    /// d'une barre de production neuve.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionBar_SetOutOfStock"/>, par le UseCase orchestrateur de la rupture de
    /// stock de barre, UC_ProductionBar_SetOutOfStock. Il consomme directement
    /// <see cref="IQ_Generic{T}"/> pour la lecture préalable de la barre et
    /// <see cref="IC_Generic{T}"/> pour sa mise à jour, sur l'entité <see cref="ProductionBar"/>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier approvisionne chaque barre juste avant de la couper. Lorsque la barre
    /// neuve désignée par l'application est absente de l'atelier, faute de livraison du
    /// fournisseur, l'opérateur déclare sa rupture de stock. À la différence du refus, qui écarte
    /// une matière défectueuse ou introuvable, la rupture met la barre en attente : la matière
    /// n'est pas condamnée, elle est seulement indisponible. La rupture ne concerne que les barres
    /// neuves ; une chute introuvable relève du refus. Le service inscrit ce basculement sur la
    /// barre et délègue ensuite la mise à jour au Command Handler générique, sans exposer la
    /// logique de persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Le basculement s'opère dans les deux sens, la valeur reçue étant écrite telle quelle dans
    /// l'indicateur de rupture de stock. La déclaration pose l'indicateur : la barre cesse d'être
    /// considérée comme en cours de traitement, tandis que les découpes qu'elle porte lui restent
    /// rattachées afin de pouvoir être restaurées d'un bloc. La libération retire l'indicateur
    /// lorsque la matière est redevenue disponible : le rattachement ayant été conservé, les
    /// découpes retrouvent aussitôt leur place dans le plan de coupe et la barre redevient
    /// éligible aux recherches. Les deux sens partagent l'ensemble de leurs contrôles, à
    /// l'exception de celui qui porte sur l'état courant de l'indicateur. Une barre en rupture
    /// conserve son plan de coupe intact, prêt à reprendre : aucun autre indicateur d'état n'est
    /// modifié, et le marqueur de placement provisoire <c>IsOptimizedTemp</c>, positionné pour
    /// toute la vie de la barre, n'est ni lu ni écrit.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle de l'identifiant de barre.</description></item>
    /// <item><description>Charger la barre désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/>.</description></item>
    /// <item><description>Vérifier l'existence de la barre et la compatibilité de son état avec le sens demandé.</description></item>
    /// <item><description>Écrire la valeur reçue dans l'indicateur de rupture de stock.</description></item>
    /// <item><description>Déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne modifie aucun autre champ que l'indicateur de rupture de stock ; <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>Ne marque ni ne libère les découpes rattachées à la barre et ne recalcule pas l'indicateur de rupture de la série : ces traitements relèvent de services dédiés appelés par le UseCase.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c>, n'appelle aucun autre service métier et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : cette inscription relève d'un service dédié appelé par le UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionBar_SetOutOfStock"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionBar_SetOutOfStock : IS_ProductionBar_SetOutOfStock
    {
        #region === Propriétés privées ===

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
        /// Command Handler générique auquel est déléguée la mise à jour de la barre basculée.
        /// </summary>
        private readonly IC_Generic<ProductionBar> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionBar_SetOutOfStock"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; la barre lue reste ainsi suivie par le contexte qui
        /// l'enregistrera, et une seconde lecture par le UseCase restitue la même instance.
        /// </para>
        /// </remarks>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture suivie de la barre. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la mise à jour de la barre. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="queryHandler"/>, <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionBar_SetOutOfStock(
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
        /// Pose ou retire l'indicateur de rupture de stock de la barre de production neuve
        /// désignée, puis confie sa mise à jour au Command Handler générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte. La barre est lue avec suivi des changements ; la même instance est contrôlée,
        /// modifiée puis transmise au Command Handler générique, qui positionne la date de mise à
        /// jour et inscrit un événement technique. L'enregistrement effectif n'intervient qu'à la
        /// validation de la transaction par l'appelant ; un échec survenant avant la modification
        /// laisse la barre intacte.
        /// </para>
        /// <para>
        /// Objectif : mettre en attente une barre neuve dont la matière est absente de l'atelier
        /// (déclaration), ou la rendre de nouveau éligible lorsque la matière est redevenue
        /// disponible (libération). La valeur reçue est écrite telle quelle ; elle détermine le
        /// seul contrôle propre à chaque sens, celui de l'état courant de l'indicateur.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de barre est strictement positif.</description></item>
        /// <item><description>Charger la barre désignée en lecture suivie, puis vérifier qu'elle a été trouvée.</description></item>
        /// <item><description>Vérifier l'état de la barre et rejeter en un échec unique l'ensemble des conditions violées, dans l'ordre : déjà validée, épuisée, déjà en rupture de stock lors d'une déclaration ou non en rupture de stock lors d'une libération, refusée ou supprimée logiquement, le motif de refus étant cité lorsqu'il est renseigné, et issue d'une chute.</description></item>
        /// <item><description>Écrire la valeur reçue dans <c>IsOutOfStock</c>.</description></item>
        /// <item><description>Transmettre au Command Handler générique l'instance chargée et modifiée.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne lit ni n'écrit <c>IsOptimizedTemp</c> et ne contrôle pas <c>IsOptimized</c>.</description></item>
        /// <item><description>Ne modifie ni <c>IsValidated</c> et <c>IsOptimized</c>, ni le nombre de découpes et les champs de reliquat, ni <c>IsUsed</c> et <c>IsDeleted</c>, ni le motif de refus, ni les bornes de zones défectueuses, ni la chute source, les rattachements, la longueur, l'origine et la date de création de la barre.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production neuve concernée. Doit être strictement positif.</param>
        /// <param name="isOutOfStock">Valeur à écrire dans l'indicateur de rupture de stock : <see langword="true"/> pour la déclaration, <see langword="false"/> pour la libération.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif (paramètre nommé et valeur reçue cités) ; avec le code
        /// <c>BU_ER_03</c> si la barre désignée est introuvable (identifiant cité) ; avec le code
        /// <c>BU_ER_04</c>, en un échec unique, si la barre est déjà validée, épuisée, déjà en
        /// rupture de stock lors d'une déclaration ou non en rupture de stock lors d'une
        /// libération, refusée ou supprimée logiquement, ou issue d'une chute (barre, sens de
        /// l'opération et chaque condition violée cités). Remonte également sans interception toute
        /// <see cref="Ex_Business"/> levée par le Query Handler ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la barre ou la délégation de sa mise à jour échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idProductionBar,
            bool isOutOfStock,
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

                ct.ThrowIfCancellationRequested();

                // L - Lecture SUIVIE : l'instance chargée est celle que le contexte partagé
                // enregistrera. Aucune variante AsNoTracking.
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

                // C2 - État compatible avec le sens demandé, toutes les conditions violées étant
                // citées en un échec unique.
                string? violation = DescribeStateViolation(bar, isOutOfStock);
                if (violation is not null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"L'état de la barre de production {bar.Id} ne permet pas sa {DescribeOperation(isOutOfStock)} de rupture de stock : {violation}.");

                // M - Écriture unique : seul IsOutOfStock est modifié. Le plan de coupe est
                // préservé et IsOptimizedTemp n'est jamais touché.
                bar.IsOutOfStock = isOutOfStock;

                // D - Délégation de la même instance que celle chargée ; UpdatedAt et événement
                // Event Store relèvent du Command Handler générique.
                await _commandHandler.HandleUpdateAsync(callChain, bar, ct);
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
        /// Décrit les conditions d'état qui interdisent le basculement demandé de la rupture de
        /// stock d'une barre de production, ou indique qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée sur la barre chargée, avant toute modification. Une barre peut
        /// basculer si elle n'est ni déjà validée, ni épuisée, si son indicateur de rupture diffère
        /// de la valeur demandée, si elle n'est ni refusée ni supprimée logiquement et si elle est
        /// neuve. Seule la condition portant sur l'état courant de l'indicateur dépend du sens de
        /// l'opération ; les autres s'appliquent aux deux sens. Toutes les conditions violées sont
        /// citées ensemble, dans cet ordre, afin que l'échec renseigne complètement l'appelant ; le
        /// motif de refus est cité lorsqu'il est renseigné. Ni l'indicateur de scellement ni le
        /// marqueur de placement provisoire ne sont contrôlés.
        /// </para>
        /// </remarks>
        /// <param name="bar">Barre chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="isOutOfStock">Valeur demandée pour l'indicateur de rupture de stock : <see langword="true"/> pour la déclaration, <see langword="false"/> pour la libération.</param>
        /// <returns>
        /// Énumération des conditions violées, prête à être citée dans le message d'échec ;
        /// <see langword="null"/> si l'état de la barre permet le basculement demandé.
        /// </returns>
        private static string? DescribeStateViolation(ProductionBar bar, bool isOutOfStock)
        {
            List<string> conditions = new();

            if (bar.IsValidated)
                conditions.Add("déjà validée");

            if (bar.IsUsed)
                conditions.Add("épuisée");

            if (bar.IsOutOfStock == isOutOfStock)
                conditions.Add(isOutOfStock
                    ? "déjà en rupture de stock"
                    : "n'est pas en rupture de stock");

            if (bar.IsDeleted)
                conditions.Add(string.IsNullOrEmpty(bar.RejectionReason)
                    ? "refusée ou supprimée logiquement"
                    : $"refusée ou supprimée logiquement (motif : « {bar.RejectionReason} »)");

            if (!bar.IsNewBar)
                conditions.Add("issue d'une chute");

            return conditions.Count == 0
                ? null
                : string.Join(", ", conditions);
        }

        /// <summary>
        /// Restitue la désignation du sens de l'opération, destinée aux messages d'échec.
        /// </summary>
        /// <param name="isOutOfStock">Valeur demandée pour l'indicateur de rupture de stock.</param>
        /// <returns><c>déclaration</c> si <paramref name="isOutOfStock"/> vaut <see langword="true"/> ; <c>libération</c> sinon.</returns>
        private static string DescribeOperation(bool isOutOfStock)
        {
            return isOutOfStock
                ? "déclaration"
                : "libération";
        }

        #endregion
    }
}