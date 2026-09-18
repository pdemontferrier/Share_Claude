using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable du blocage et du déblocage de l'ensemble des découpes
    /// rattachées à une barre de production, selon que la matière de cette barre est en rupture
    /// de stock ou redevenue disponible.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionCutPiece_SetBarOutOfStock"/>, par le UseCase orchestrateur de la
    /// rupture de stock de barre, <c>UC_ProductionBar_SetOutOfStock</c>, après le basculement de la
    /// barre et avant le recalcul de l'indicateur de rupture de la série. Il consomme directement
    /// <see cref="IQ_Generic{T}"/> pour la lecture préalable des découpes et
    /// <see cref="IC_Generic{T}"/> pour leur mise à jour, sur l'entité
    /// <see cref="ProductionCutPiece"/>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier approvisionne chaque barre juste avant de la couper. Lorsque la
    /// matière d'une barre neuve est absente de l'atelier, la barre est déclarée en rupture de
    /// stock et mise en attente, sans être supprimée ni écartée, jusqu'à sa libération lorsque la
    /// matière redevient disponible. Le service porte le versant « découpes » de cette mise en
    /// attente, dans les deux sens : la déclaration pose le marqueur de blocage sur les découpes
    /// rattachées à la barre, la libération le retire. Il marque les découpes puis en délègue la
    /// mise à jour au Command Handler générique, sans exposer la logique de persistance ni assumer
    /// de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Le marqueur de blocage compte parmi les critères qui excluent une découpe du vivier des
    /// pièces à optimiser ; il maintient cette exclusion indépendamment du marqueur de placement
    /// provisoire, et constitue un motif de refus du scellement du plan de coupe. Le traitement se
    /// définit par ce qu'il préserve : les découpes restent rattachées à la barre avec leur
    /// position de coupe, et leur marqueur de placement provisoire n'est pas modifié. Le plan de
    /// coupe demeure intact, simplement gelé, ce qui permet de retrouver les découpes de la barre
    /// et de les restaurer sans recalcul au retour de la matière, à la différence du refus ou du
    /// détachement, qui défont le plan.
    /// </para>
    /// <para>
    /// Sélection par rattachement : les découpes traitées sont toutes celles qui portent
    /// l'identifiant de la barre, y compris les découpes supprimées logiquement, qui restent ainsi
    /// visibles pour être rejetées. Le traitement est en bloc : tous les contrôles précèdent tout
    /// marquage, et tout échec de contrôle laisse l'ensemble des découpes intact. Les deux sens
    /// partagent l'intégralité de leurs contrôles ; seules les découpes dont le marqueur diffère de
    /// la valeur demandée sont écrites, et l'absence de telles découpes constitue un aboutissement
    /// nominal sans écriture.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle portant sur l'identifiant de barre.</description></item>
    /// <item><description>Charger les découpes rattachées à la barre en une lecture unique suivie via <see cref="IQ_Generic{T}.HandleGetFilteredAsync"/>.</description></item>
    /// <item><description>Vérifier la présence d'au moins une découpe rattachée et l'absence de découpe réalisée ou supprimée logiquement.</description></item>
    /// <item><description>Écrire la valeur demandée dans le marqueur de blocage des seules découpes dont le marqueur en diffère.</description></item>
    /// <item><description>Déléguer la mise à jour des découpes modifiées au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateRangeAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne contrôle ni l'existence ni l'état de la barre, ni l'état courant du marqueur de blocage des découpes : le contrôle d'état relève du service de rupture de stock de la barre, appelé auparavant dans la même transaction.</description></item>
    /// <item><description>Ne défait pas le plan de coupe et ne modifie aucun autre champ que le marqueur de blocage : <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>Ne recalcule pas l'indicateur de rupture de stock de la série : ce rôle relève d'un service dédié appelé par le UseCase.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c>, n'appelle aucun autre service métier et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : la déclaration ou la libération est tracée une seule fois, sur la barre, par le UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionCutPiece_SetBarOutOfStock"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionCutPiece_SetBarOutOfStock : IS_ProductionCutPiece_SetBarOutOfStock
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
        /// Query Handler générique auquel est déléguée la lecture suivie des découpes rattachées
        /// à la barre.
        /// </summary>
        private readonly IQ_Generic<ProductionCutPiece> _queryHandler;

        /// <summary>
        /// Command Handler générique auquel est déléguée la mise à jour des découpes bloquées ou
        /// débloquées.
        /// </summary>
        private readonly IC_Generic<ProductionCutPiece> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionCutPiece_SetBarOutOfStock"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; les découpes lues restent ainsi suivies par le
        /// contexte qui les enregistrera.
        /// </para>
        /// </remarks>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture suivie des découpes. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la mise à jour des découpes. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="queryHandler"/>, <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionCutPiece_SetBarOutOfStock(
            IQ_Generic<ProductionCutPiece> queryHandler,
            IC_Generic<ProductionCutPiece> commandHandler,
            IS_ExClassifier classifier)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        // --- Groupe 1 : Blocage des découpes d'une barre de production en rupture de stock ---

        /// <summary>
        /// Pose ou retire le marqueur de blocage sur les découpes rattachées à la barre de
        /// production désignée, sans toucher au plan de coupe, puis confie la mise à jour des
        /// découpes modifiées au Command Handler générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de la rupture de stock de barre, à
        /// l'intérieur de la transaction qu'il a ouverte, après le basculement de la barre. Les
        /// découpes sont lues en une requête unique avec suivi des changements, filtrée sur le seul
        /// rattachement à la barre : les découpes supprimées logiquement restent ainsi visibles et
        /// sont rejetées par le contrôle d'état. Le Command Handler générique positionne ensuite la
        /// date de mise à jour et inscrit un événement par découpe transmise. L'enregistrement
        /// effectif n'intervient qu'à la validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : bloquer les découpes d'une barre dont la matière est absente de l'atelier
        /// (déclaration), ou les débloquer lorsque la matière est redevenue disponible
        /// (libération). La valeur reçue est écrite telle quelle ; les deux sens partagent
        /// l'intégralité de leurs contrôles.
        /// </para>
        /// <para>
        /// Le contrôle d'état porte sur toutes les découpes chargées, y compris celles dont le
        /// marqueur porte déjà la valeur demandée. La sélection des découpes à écrire est arrêtée
        /// avant tout marquage ; lorsqu'elle est vide, la méthode rend la main sans écriture, sans
        /// date de mise à jour et sans événement.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de barre est strictement positif, avant toute lecture.</description></item>
        /// <item><description>Charger en une lecture unique suivie toutes les découpes rattachées à la barre.</description></item>
        /// <item><description>Vérifier qu'au moins une découpe est rattachée : une barre présentée porte nécessairement un plan de coupe, et une liste vide signale un appel hors séquence.</description></item>
        /// <item><description>Vérifier l'état de chaque découpe et rejeter en un échec unique l'ensemble des découpes déjà réalisées ou supprimées logiquement, en citant la barre, le sens de l'opération et, pour chaque découpe, toutes ses conditions violées, les découpes étant triées par position de coupe croissante, positions absentes en dernier, puis par identifiant.</description></item>
        /// <item><description>Sélectionner les découpes dont le marqueur de blocage diffère de la valeur demandée et y écrire cette valeur.</description></item>
        /// <item><description>Transmettre au Command Handler générique, en un appel unique, les instances chargées et modifiées.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ : le rattachement, la position de coupe, les marqueurs de placement provisoire et définitif, d'approvisionnement, de refus, de réalisation et de suppression logique, les horodatages de coupe, la date de création et la date de mise à jour restent en dehors du marquage.</description></item>
        /// <item><description>Ne marque aucune découpe avant que tous les contrôles soient passés.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne contrôle ni l'état de la barre ni l'état courant du marqueur des découpes.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production dont les découpes rattachées sont à bloquer ou à débloquer. Doit être strictement positif.</param>
        /// <param name="isBarOutOfStock">Valeur à écrire dans le marqueur de blocage des découpes : <see langword="true"/> pour la déclaration de rupture de stock, <see langword="false"/> pour sa libération.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif (valeur reçue citée) ; avec le code <c>BU_ER_03</c> si aucune découpe
        /// n'est rattachée à la barre désignée (barre citée) ; avec le code <c>BU_ER_04</c>, en un
        /// échec unique, si des découpes rattachées sont déjà réalisées ou supprimées logiquement
        /// (barre, sens de l'opération, chaque découpe et chaque condition violée cités). Remonte
        /// également sans interception toute <see cref="Ex_Business"/> levée par le Query Handler
        /// ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture des découpes ou la délégation de leur mise à jour échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idProductionBar,
            bool isBarOutOfStock,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // P - Identifiant de barre strictement positif.
                if (idProductionBar <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de barre de production (idProductionBar) doit être strictement positif ; valeur reçue : {idProductionBar}.");

                ct.ThrowIfCancellationRequested();

                // L - Lecture SUIVIE et UNIQUE, filtrée sur le seul rattachement : les découpes
                // supprimées logiquement doivent rester visibles pour être rejetées en C2.
                // Ni variante AsNoTracking, ni lectures unitaires.
                List<ProductionCutPiece> pieces = await _queryHandler.HandleGetFilteredAsync(
                    callChain,
                    p => p.IdProductionBar == idProductionBar,
                    ct);

                // C1 - Une barre présentée porte nécessairement un plan de coupe.
                if (pieces.Count == 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Aucune découpe n'est rattachée à la barre de production {idProductionBar}, alors qu'une barre présentée porte nécessairement un plan de coupe ; appel hors séquence (identifiant de barre sans plan de coupe ou découpes détachées).");

                // C2 - Aucune découpe réalisée ni supprimée logiquement, contrôle porté sur TOUTES
                // les découpes chargées, indépendamment de la valeur courante de leur marqueur ;
                // les découpes fautives sont citées dans l'ordre de coupe, en un échec unique.
                List<string> stateViolations = pieces
                    .OrderBy(p => p.CutPositionInBar is null)
                    .ThenBy(p => p.CutPositionInBar)
                    .ThenBy(p => p.Id)
                    .Select(DescribeStateViolation)
                    .OfType<string>()
                    .ToList();

                if (stateViolations.Count > 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"L'état des découpes suivantes ne permet pas la {DescribeOperation(isBarOutOfStock)} de rupture de stock de la barre de production {idProductionBar} : {string.Join(" ; ", stateViolations)}.");

                // F - Sélection des découpes à écrire, MATÉRIALISÉE avant tout marquage : une
                // évaluation différée après marquage serait faussée.
                List<ProductionCutPiece> piecesToUpdate = pieces
                    .Where(p => p.IsBarOutOfStock != isBarOutOfStock)
                    .ToList();

                // C3 - Aucune découpe à modifier : aucune écriture, ni date de mise à jour ni événement.
                if (piecesToUpdate.Count == 0)
                    return;

                // M - Écriture unique : seul IsBarOutOfStock est modifié. Le plan de coupe
                // (rattachement, position de coupe) et IsOptimizedTemp restent intacts.
                foreach (ProductionCutPiece piece in piecesToUpdate)
                    piece.IsBarOutOfStock = isBarOutOfStock;

                // D - Délégation des mêmes instances que celles chargées, en une seule opération ;
                // UpdatedAt et événements Event Store relèvent du Command Handler générique.
                await _commandHandler.HandleUpdateRangeAsync(callChain, piecesToUpdate, ct);
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
        /// Décrit les conditions d'état qui interdisent le blocage ou le déblocage d'une découpe,
        /// ou indique qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée pour chaque découpe chargée, avant toute sélection et tout marquage.
        /// Une découpe peut être bloquée ou débloquée si elle n'est ni réalisée ni supprimée
        /// logiquement ; ces conditions s'appliquent identiquement aux deux sens de l'opération et
        /// ne tiennent pas compte de la valeur courante du marqueur de blocage. Toutes les
        /// conditions violées sont citées ensemble, dans cet ordre, afin que l'échec agrégé
        /// renseigne complètement l'appelant.
        /// </para>
        /// </remarks>
        /// <param name="piece">Découpe chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Description de la découpe et des conditions violées, prête à être citée dans le message
        /// d'échec ; <see langword="null"/> si l'état de la découpe permet l'opération.
        /// </returns>
        private static string? DescribeStateViolation(ProductionCutPiece piece)
        {
            List<string> conditions = new();

            if (piece.IsCut)
                conditions.Add("déjà réalisée");

            if (piece.IsDeleted)
                conditions.Add("supprimée logiquement");

            return conditions.Count == 0
                ? null
                : $"découpe {piece.Id} ({string.Join(", ", conditions)})";
        }

        /// <summary>
        /// Restitue la désignation du sens de l'opération, destinée aux messages d'échec.
        /// </summary>
        /// <param name="isBarOutOfStock">Valeur demandée pour le marqueur de blocage des découpes.</param>
        /// <returns><c>déclaration</c> si <paramref name="isBarOutOfStock"/> vaut <see langword="true"/> ; <c>libération</c> sinon.</returns>
        private static string DescribeOperation(bool isBarOutOfStock)
        {
            return isBarOutOfStock
                ? "déclaration"
                : "libération";
        }

        #endregion
    }
}