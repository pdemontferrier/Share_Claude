using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable du détachement, sur le chemin de validation d'une barre de
    /// production avec défauts, de l'ensemble des découpes rattachées à cette barre, qui
    /// redeviennent matière à optimiser sans être marquées comme refusées.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionCutPiece_DetachFromBar"/>, par le UseCase orchestrateur de la
    /// validation de barre, sur le chemin avec défauts, après la validation de la barre et avant
    /// la recomposition de son plan de coupe. Il consomme directement <see cref="IQ_Generic{T}"/>
    /// pour la lecture préalable des découpes et <see cref="IC_Generic{T}"/> pour leur mise à
    /// jour, sur l'entité <see cref="ProductionCutPiece"/>.
    /// </para>
    /// <para>
    /// Objectif : défaire le plan de coupe provisoire d'une barre que des zones défectueuses ont
    /// rendu irréalisable, afin que le moteur d'optimisation puisse le recomposer sur la matière
    /// réellement exploitable. Chaque découpe rattachée perd son rattachement et sa position de
    /// coupe, quitte l'état de placement provisoire et rejoint le vivier des pièces à optimiser.
    /// Le service marque les découpes puis en délègue la mise à jour au Command Handler générique,
    /// sans exposer la logique de persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Absence de trace de refus : le marqueur de refus des découpes n'est jamais modifié, rien
    /// n'étant arrivé aux pièces, qui figuraient seulement sur un plan devenu caduc. Le service se
    /// distingue en cela du détachement consécutif au refus d'une barre
    /// (<c>SR_ProductionCutPiece_DetachAndRefuse</c>), qui libère les mêmes champs et marque en
    /// outre chaque découpe comme refusée ; les deux services ne diffèrent que par ce marqueur et
    /// ne sont pas interchangeables.
    /// </para>
    /// <para>
    /// Sélection par rattachement : les découpes détachées sont toutes celles qui portent
    /// l'identifiant de la barre, et non une liste fournie par l'appelant ; aucun reliquat du plan
    /// caduc ne peut ainsi subsister. Le traitement est intégral : tous les contrôles précèdent
    /// tout marquage, toutes les découpes sont détachées ou aucune, et tout échec survenant
    /// pendant la délégation est neutralisé par l'annulation de la transaction de l'appelant.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle portant sur l'identifiant de barre.</description></item>
    /// <item><description>Charger les découpes rattachées à la barre en une lecture unique suivie via <see cref="IQ_Generic{T}.HandleGetFilteredAsync"/>.</description></item>
    /// <item><description>Vérifier la présence d'au moins une découpe rattachée et la compatibilité de l'état de chacune avec le détachement.</description></item>
    /// <item><description>Lever sur chaque découpe le marqueur de placement provisoire, supprimer son rattachement à la barre et effacer sa position de coupe.</description></item>
    /// <item><description>Déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateRangeAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur, qui enregistre le détachement avant la recomposition, le moteur d'optimisation lisant le vivier sur l'état persistant.</description></item>
    /// <item><description>Ne vérifie ni l'existence ni l'état de la barre : sa validation, préalable au détachement dans la même transaction, relève d'un autre service.</description></item>
    /// <item><description>Ne marque aucune découpe comme refusée et ne modifie aucun autre champ que les trois champs du plan provisoire : <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>Ne recompose pas le plan de coupe et ne rattache aucune découpe.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> : toute valeur nécessaire lui est fournie par argument depuis le UseCase.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Query Handler et le Command Handler.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionCutPiece_DetachFromBar"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionCutPiece_DetachFromBar : IS_ProductionCutPiece_DetachFromBar
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
        /// Command Handler générique auquel est déléguée la mise à jour des découpes détachées.
        /// </summary>
        private readonly IC_Generic<ProductionCutPiece> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionCutPiece_DetachFromBar"/> avec ses dépendances.
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
        public SR_ProductionCutPiece_DetachFromBar(
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

        // --- Groupe 1 : Détachement des découpes d'une barre de production ---

        /// <summary>
        /// Défait le plan de coupe provisoire de la barre de production désignée en libérant
        /// toutes ses découpes rattachées vers le vivier des pièces à optimiser, sans les marquer
        /// comme refusées, puis confie leur mise à jour au Command Handler générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de la validation de barre, sur le chemin
        /// avec défauts, à l'intérieur de la transaction qu'il a ouverte, après la validation de la
        /// barre et avant la recomposition du plan. Les découpes sont lues en une requête unique
        /// avec suivi des changements, filtrée sur le seul rattachement à la barre : les découpes
        /// supprimées logiquement restent ainsi visibles et sont rejetées par le contrôle d'état.
        /// Le Command Handler générique positionne ensuite la date de mise à jour et inscrit un
        /// événement par découpe. L'enregistrement effectif relève de l'appelant.
        /// </para>
        /// <para>
        /// L'opération n'est pas idempotente : un second appel sur la même barre échoue, aucune
        /// découpe n'y étant plus rattachée. Une barre sans découpe rattachée signale de même un
        /// appel hors séquence.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de barre est strictement positif, avant toute lecture.</description></item>
        /// <item><description>Charger en une lecture unique suivie toutes les découpes rattachées à la barre.</description></item>
        /// <item><description>Vérifier qu'au moins une découpe est rattachée : une écriture de groupe sur un ensemble vide serait silencieuse.</description></item>
        /// <item><description>Vérifier l'état de chaque découpe et rejeter en un échec unique l'ensemble des découpes déjà scellées, déjà réalisées ou supprimées logiquement, triées par identifiant croissant.</description></item>
        /// <item><description>Positionner sur chaque découpe le marqueur de placement provisoire à <see langword="false"/>, la barre de rattachement à <see langword="null"/> et la position de coupe à <see langword="null"/>.</description></item>
        /// <item><description>Transmettre au Command Handler générique, en un appel unique, les instances chargées et marquées.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ : les marqueurs de refus, de placement définitif, d'approvisionnement de la barre, de réalisation, de rupture de stock et de suppression logique, les horodatages de coupe, la date de création et la date de mise à jour restent en dehors du marquage.</description></item>
        /// <item><description>Ne marque aucune découpe avant que tous les contrôles soient passés.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie ni l'existence ni l'état de la barre.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production dont le plan de coupe provisoire est à défaire ; seul critère de sélection des découpes. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif (valeur reçue citée) ; avec le code <c>BU_ER_03</c> si aucune découpe
        /// n'est rattachée à la barre désignée (barre citée) ; avec le code <c>BU_ER_04</c>, en un
        /// échec unique, si des découpes rattachées ne sont pas détachables (barre, chaque découpe et
        /// chaque condition violée citées). Remonte également sans interception toute
        /// <see cref="Ex_Business"/> levée par le Query Handler ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture des découpes ou la délégation de leur mise à jour échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idProductionBar,
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

                // L - Lecture SUIVIE et UNIQUE, filtrée sur le seul rattachement : les découpes
                // supprimées logiquement doivent rester visibles pour être rejetées en C2.
                // Ni variante AsNoTracking, ni lectures unitaires.
                List<ProductionCutPiece> pieces = await _queryHandler.HandleGetFilteredAsync(
                    callChain,
                    p => p.IdProductionBar == idProductionBar,
                    ct);

                // C1 - Au moins une découpe rattachée : une écriture de groupe sur un ensemble
                // vide serait silencieuse, ce contrôle est le seul à détecter ce cas.
                if (pieces.Count == 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Aucune découpe n'est rattachée à la barre de production {idProductionBar}, alors que son plan de coupe provisoire doit être défait ; appel hors séquence (détachement déjà effectué ou barre sans plan de coupe).");

                // C2 - État compatible avec le détachement, contrôlé sur chaque découpe ; les
                // découpes fautives sont citées par identifiant croissant, en un échec unique.
                List<string> stateViolations = pieces
                    .OrderBy(p => p.Id)
                    .Select(DescribeStateViolation)
                    .OfType<string>()
                    .ToList();

                if (stateViolations.Count > 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"L'état des découpes suivantes ne permet pas leur détachement de la barre de production {idProductionBar} : {string.Join(" ; ", stateViolations)}.");

                // M - Marquage, après succès de tous les contrôles : chaque découpe quitte le
                // plan provisoire et redevient matière à optimiser.
                foreach (ProductionCutPiece piece in pieces)
                {
                    piece.IsOptimizedTemp = false;
                    piece.IdProductionBar = null;
                    piece.CutPositionInBar = null;
                    // IsCutRefused : NON MODIFIÉ - le détachement ne laisse aucune trace de refus.
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
        /// Décrit les conditions d'état qui interdisent le détachement d'une découpe de sa barre de
        /// production, ou indique qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée pour chaque découpe chargée, avant tout marquage. Une découpe est
        /// détachable si elle n'est pas encore scellée, n'a pas été réalisée et n'est pas supprimée
        /// logiquement ; toutes les conditions violées sont citées ensemble, dans cet ordre, afin
        /// que l'échec agrégé renseigne complètement l'appelant.
        /// </para>
        /// </remarks>
        /// <param name="piece">Découpe chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Description de la découpe et des conditions violées, prête à être citée dans le message
        /// d'échec ; <see langword="null"/> si l'état de la découpe permet son détachement.
        /// </returns>
        private static string? DescribeStateViolation(ProductionCutPiece piece)
        {
            List<string> conditions = new();

            if (piece.IsOptimized)
                conditions.Add("déjà scellée");

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