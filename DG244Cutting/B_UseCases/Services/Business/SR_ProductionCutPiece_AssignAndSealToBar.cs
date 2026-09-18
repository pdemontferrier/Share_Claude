using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable du rattachement définitif, à une barre de production validée
    /// avec défauts, des découpes du plan de coupe recomposé par le moteur d'optimisation, dans
    /// l'ordre de coupe calculé.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionCutPiece_AssignAndSealToBar"/>, par le UseCase orchestrateur de la
    /// validation de barre (<c>UC_BarValidation</c>), sur le seul chemin de validation avec défauts
    /// et lorsque la recomposition a placé au moins une découpe. Il consomme directement
    /// <see cref="IQ_Generic{T}"/> pour la lecture préalable des découpes et
    /// <see cref="IC_Generic{T}"/> pour leur mise à jour, sur l'entité
    /// <see cref="ProductionCutPiece"/>.
    /// </para>
    /// <para>
    /// Objectif : les zones défectueuses déclarées à la validation d'une barre rendent irréalisable
    /// le plan de coupe établi auparavant ; l'application le recompose sur la matière réellement
    /// exploitable. La barre étant déjà acceptée, le plan recomposé est définitif. Le service
    /// inscrit ce plan sur les découpes elles-mêmes : chacune se rattache à la barre, reçoit son
    /// rang dans le plan de coupe et est scellée en une seule écriture, sans jamais être marquée
    /// comme provisoirement placée. Il réunit ainsi les deux gestes que le parcours nominal sépare
    /// entre <see cref="IS_ProductionCutPiece_AssignToBar"/> (placement provisoire) et le service
    /// homologue de scellement (<c>IS_ProductionCutPiece_SealToBar</c>), évitant l'enregistrement
    /// intermédiaire et le double événement par découpe qu'imposerait leur enchaînement. Il marque
    /// les découpes puis en délègue la mise à jour au Command Handler générique, sans exposer la
    /// logique de persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Invariant d'ordre : la position de coupe d'une découpe est son rang dans la liste
    /// d'identifiants reçue, compté à partir de 1. Aucune autre donnée ne transporte cette position ;
    /// la liste n'est jamais triée ni réordonnée, y compris à titre interne. C'est le point le plus
    /// fragile du composant : un réordonnancement casserait le plan de coupe sans produire aucune
    /// erreur visible. La liste chargée ne revenant pas nécessairement dans l'ordre demandé,
    /// l'appariement entre identifiant et découpe se fait par identifiant, jamais par rang dans la
    /// liste chargée.
    /// </para>
    /// <para>
    /// Le placement est définitif : chaque découpe est marquée comme placée et scellée, son
    /// marqueur de placement provisoire restant levé, et elle est marquée comme portée par une
    /// barre approvisionnée. Une découpe précédemment refusée sur une autre barre retrouve par ce
    /// rattachement un statut neutre. Le service n'est pas réentrant : une découpe déjà rattachée à
    /// une barre, y compris à la barre désignée, est rejetée, le plan provisoire devant avoir été
    /// défait au préalable. Le traitement est intégral : toutes les découpes sont marquées ou
    /// aucune ; tout échec interrompt le traitement avant marquage, et la transaction de l'appelant
    /// est annulée.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier les préconditions structurelles des arguments.</description></item>
    /// <item><description>Charger les découpes désignées en une lecture unique suivie via <see cref="IQ_Generic{T}.HandleGetFilteredAsync"/>.</description></item>
    /// <item><description>Vérifier l'existence de chaque découpe et la compatibilité de son état avec le rattachement.</description></item>
    /// <item><description>Positionner sur chaque découpe la barre de rattachement, la position de coupe, le marqueur de placement définitif, la levée du marqueur de placement provisoire, la levée du marqueur de refus et le marqueur d'approvisionnement de la barre.</description></item>
    /// <item><description>Déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateRangeAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne lit ni ne vérifie la barre de production : l'intégrité référentielle entre la découpe et la barre est signalée par la persistance déclenchée par l'appelant.</description></item>
    /// <item><description>Ne valide pas la barre, ne détache pas les découpes du plan provisoire et n'inscrit pas le plan recomposé sur la barre : ces actions sont orchestrées par le UseCase.</description></item>
    /// <item><description>Ne choisit pas les découpes et ne calcule pas leur ordre : l'un et l'autre sont repris tels que déterminés par le moteur d'optimisation.</description></item>
    /// <item><description>Ne positionne pas les champs d'audit : <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> : toute valeur nécessaire lui est fournie par argument depuis le UseCase.</description></item>
    /// <item><description>N'appelle jamais directement un Repository ni un autre Service : l'accès aux données passe par le Query Handler et le Command Handler.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionCutPiece_AssignAndSealToBar"/>
    /// <seealso cref="IS_ProductionCutPiece_AssignToBar"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    /// <seealso cref="DTO_CuttingOptimizationResult.PieceIds"/>
    public class SR_ProductionCutPiece_AssignAndSealToBar : IS_ProductionCutPiece_AssignAndSealToBar
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
        /// Query Handler générique auquel est déléguée la lecture suivie des découpes désignées.
        /// </summary>
        private readonly IQ_Generic<ProductionCutPiece> _queryHandler;

        /// <summary>
        /// Command Handler générique auquel est déléguée la mise à jour des découpes marquées.
        /// </summary>
        private readonly IC_Generic<ProductionCutPiece> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionCutPiece_AssignAndSealToBar"/> avec ses dépendances.
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
        public SR_ProductionCutPiece_AssignAndSealToBar(
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

        /// <summary>
        /// Rattache à une barre de production validée avec défauts les découpes du plan recomposé,
        /// en leur attribuant leur position de coupe selon leur rang dans la liste reçue et en les
        /// scellant directement, puis confie leur mise à jour au Command Handler générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, après la validation de la barre avec ses défauts, le détachement du plan
        /// provisoire et la recomposition du plan. Les découpes sont lues en une requête unique avec
        /// suivi des changements ; le Command Handler générique positionne ensuite la date de mise à
        /// jour et inscrit un événement par découpe. L'enregistrement effectif n'intervient qu'à la
        /// validation de la transaction par l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier, dans l'ordre : présence de la liste, non-vacuité de la liste, identifiant de barre strictement positif, identifiants de découpes strictement positifs, absence de doublon.</description></item>
        /// <item><description>Charger les découpes désignées en une lecture unique suivie.</description></item>
        /// <item><description>Vérifier que toutes les découpes désignées ont été trouvées.</description></item>
        /// <item><description>Vérifier l'état de chaque découpe et rejeter en un échec unique l'ensemble des découpes déjà rattachées à une barre quelle qu'elle soit, déjà réalisées ou supprimées logiquement, citées dans l'ordre de la liste reçue.</description></item>
        /// <item><description>Parcourir la liste reçue dans son ordre et, pour chaque identifiant de rang <c>i</c> compté à partir de 0, retrouver la découpe par son identifiant puis positionner la barre de rattachement, la position de coupe <c>i + 1</c>, le marqueur de placement définitif à <see langword="true"/>, le marqueur de placement provisoire à <see langword="false"/>, le marqueur de refus à <see langword="false"/> et le marqueur d'approvisionnement de la barre à <see langword="true"/>.</description></item>
        /// <item><description>Transmettre au Command Handler générique les instances chargées et marquées.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ : la réalisation, les dates de début et de fin de coupe, la rupture d'approvisionnement, la suppression logique, la date de création et la date de mise à jour restent en dehors du marquage.</description></item>
        /// <item><description>Ne positionne jamais le marqueur de placement provisoire à <see langword="true"/>, même transitoirement.</description></item>
        /// <item><description>Ne trie ni ne réordonne la liste reçue et n'apparie jamais par rang dans la liste chargée.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne lit pas la barre.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="pieceIds">
        /// Identifiants des découpes retenues par la recomposition, dans l'ordre de coupe ; le rang de
        /// chaque identifiant, compté à partir de 1, devient la position de coupe. Ne doit pas être
        /// <see langword="null"/> ni vide ; chaque identifiant doit être strictement positif et
        /// n'apparaître qu'une seule fois.
        /// </param>
        /// <param name="idProductionBar">Identifiant de la barre de production validée avec défauts. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="pieceIds"/> est
        /// <see langword="null"/> ; avec le code <c>BU_ER_02</c> si <paramref name="pieceIds"/> est
        /// vide, si <paramref name="idProductionBar"/> n'est pas strictement positif (valeur reçue
        /// citée) ou si des identifiants de découpes ne sont pas strictement positifs (valeurs
        /// fautives citées) ; avec le code <c>BU_ER_03</c> si des identifiants figurent plusieurs
        /// fois dans la liste (identifiants dupliqués cités), ou si des découpes désignées sont
        /// introuvables (identifiants cités) ; avec le code <c>BU_ER_04</c>, en un échec unique, si
        /// des découpes sont déjà rattachées à une barre quelle qu'elle soit, déjà réalisées ou
        /// supprimées logiquement (chaque découpe et chaque condition violée citées). Remonte
        /// également sans interception toute <see cref="Ex_Business"/> levée par le Query Handler
        /// ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture des découpes ou la délégation de leur mise à jour échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            IReadOnlyList<int> pieceIds,
            int idProductionBar,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // P1 - Liste des découpes obligatoire.
                if (pieceIds is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "La liste des identifiants de découpes (pieceIds) est obligatoire pour le rattachement définitif à une barre de production.");

                // P2 - Au moins une découpe à rattacher.
                if (pieceIds.Count == 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        "La liste des identifiants de découpes (pieceIds) doit contenir au moins un élément.");

                // P3 - Identifiant de barre strictement positif.
                if (idProductionBar <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de barre de production (idProductionBar) doit être strictement positif ; valeur reçue : {idProductionBar}.");

                // P4 - Identifiants de découpes strictement positifs.
                List<int> invalidIds = pieceIds.Where(id => id <= 0).Distinct().ToList();
                if (invalidIds.Count > 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"Les identifiants de découpes (pieceIds) doivent être strictement positifs ; valeurs fautives : {string.Join(", ", invalidIds)}.");

                // P5 - Aucun doublon : une même découpe ne peut occuper deux positions de coupe.
                List<int> duplicateIds = pieceIds
                    .GroupBy(id => id)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToList();
                if (duplicateIds.Count > 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Les identifiants de découpes (pieceIds) ne doivent figurer qu'une seule fois, une même découpe ne pouvant occuper deux positions de coupe ; identifiants dupliqués : {string.Join(", ", duplicateIds)}.");

                ct.ThrowIfCancellationRequested();

                // L - Lecture SUIVIE et UNIQUE : les instances chargées sont celles que le contexte
                // partagé enregistrera. Ni variante AsNoTracking, ni lectures unitaires.
                List<ProductionCutPiece> pieces = await _queryHandler.HandleGetFilteredAsync(
                    callChain,
                    p => pieceIds.Contains(p.Id),
                    ct);

                // C1 - Toutes les découpes désignées doivent exister.
                if (pieces.Count != pieceIds.Count)
                {
                    HashSet<int> foundIds = pieces.Select(p => p.Id).ToHashSet();
                    List<int> missingIds = pieceIds.Where(id => !foundIds.Contains(id)).ToList();

                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Des découpes désignées sont introuvables, signe d'une incohérence entre la recomposition du plan de coupe et l'état persistant ; identifiants introuvables : {string.Join(", ", missingIds)}.");
                }

                // Index d'appariement par identifiant : la liste chargée ne revient pas
                // nécessairement dans l'ordre demandé. Aucun appariement par rang.
                Dictionary<int, ProductionCutPiece> piecesById = pieces.ToDictionary(p => p.Id);

                // C2 - État compatible avec le rattachement définitif, contrôlé sur chaque découpe ;
                // les découpes fautives sont citées dans l'ordre de la liste reçue, en un échec unique.
                List<string> stateViolations = new();
                foreach (int pieceId in pieceIds)
                {
                    string? violation = DescribeStateViolation(piecesById[pieceId]);
                    if (violation is not null)
                        stateViolations.Add(violation);
                }

                if (stateViolations.Count > 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"L'état des découpes suivantes ne permet pas leur rattachement définitif à la barre de production {idProductionBar}, le plan de coupe provisoire aurait dû être défait au préalable : {string.Join(" ; ", stateViolations)}.");

                // M - Marquage dans l'ordre EXACT de pieceIds : la position de coupe est le rang
                // de l'identifiant, compté à partir de 1. Ni tri, ni réordonnancement.
                // IsOptimizedTemp n'est jamais positionné à true, même transitoirement.
                for (int i = 0; i < pieceIds.Count; i++)
                {
                    ProductionCutPiece piece = piecesById[pieceIds[i]];

                    piece.IdProductionBar = idProductionBar;
                    piece.CutPositionInBar = i + 1;
                    piece.IsOptimized = true;
                    piece.IsOptimizedTemp = false;
                    piece.IsCutRefused = false;
                    piece.IsBarSupplied = true;
                }

                // D - Délégation des mêmes instances que celles chargées, en une seule opération ;
                // UpdatedAt et événements Event Store relèvent du Command Handler générique.
                await _commandHandler.HandleUpdateRangeAsync(callChain, pieces, ct);
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
        /// Décrit les conditions d'état qui interdisent le rattachement définitif d'une découpe à une
        /// barre de production, ou indique qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée pour chaque découpe chargée, avant tout marquage. Une découpe est
        /// rattachable si elle n'est rattachée à aucune barre, y compris la barre désignée, n'a pas
        /// été réalisée et n'est pas supprimée logiquement ; toutes les conditions violées sont
        /// citées ensemble afin que l'échec agrégé renseigne complètement l'appelant.
        /// </para>
        /// </remarks>
        /// <param name="piece">Découpe chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Description de la découpe et des conditions violées, prête à être citée dans le message
        /// d'échec ; <see langword="null"/> si l'état de la découpe permet son rattachement.
        /// </returns>
        private static string? DescribeStateViolation(ProductionCutPiece piece)
        {
            List<string> conditions = new();

            if (piece.IdProductionBar is not null)
                conditions.Add($"déjà rattachée à la barre de production {piece.IdProductionBar}");

            if (piece.IsCut)
                conditions.Add("déjà réalisée");

            if (piece.IsDeleted)
                conditions.Add("supprimée logiquement");

            return conditions.Count == 0
                ? null
                : $"découpe {piece.Id} ({string.Join(", ", conditions)})";
        }

        #endregion
    }
}