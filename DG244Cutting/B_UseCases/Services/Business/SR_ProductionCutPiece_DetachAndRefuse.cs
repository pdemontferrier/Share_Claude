using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de la libération, sur une barre de production refusée par
    /// l'opérateur, de l'ensemble des découpes qui y étaient provisoirement placées, avec
    /// inscription sur chacune de la trace du refus de coupe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionCutPiece_DetachAndRefuse"/>, par le UseCase orchestrateur du refus
    /// de barre, après la mise à l'écart de la barre. Il consomme directement
    /// <see cref="IQ_Generic{T}"/> pour la lecture préalable des découpes et
    /// <see cref="IC_Generic{T}"/> pour leur mise à jour, sur l'entité
    /// <see cref="ProductionCutPiece"/>.
    /// </para>
    /// <para>
    /// Objectif : défaire, côté découpes, tout ce que l'optimisation avait posé sur une barre que
    /// l'opérateur a trouvée inutilisable et refusée. Les découpes perdent leur rattachement et leur
    /// position de coupe et retournent au vivier des pièces à optimiser, d'où une optimisation
    /// ultérieure pourra les reprendre sur une autre barre. Le service marque les découpes puis en
    /// délègue la mise à jour au Command Handler générique, sans exposer la logique de persistance
    /// ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Trace du refus : chaque découpe libérée reçoit le marqueur de refus de coupe, qui conserve la
    /// trace du fait que la matière qui lui était destinée a été écartée. Ce marqueur ne bloque pas
    /// la reprise : il n'exclut pas la découpe du vivier, et le rattachement provisoire à une
    /// nouvelle barre le lève. Le marqueur de placement définitif n'est jamais posé : les découpes,
    /// jamais scellées, redeviennent matière à optimiser, alors que ce marqueur les ferait sortir
    /// durablement du vivier.
    /// </para>
    /// <para>
    /// Sélection par rattachement : les découpes à libérer sont toutes celles qui portent
    /// l'identifiant de la barre, et non une liste fournie par l'appelant ; aucune pièce du plan ne
    /// peut ainsi rester attachée à une barre refusée. Une barre dont le plan est scellé n'est plus
    /// refusable, de sorte que toute découpe déjà scellée est rejetée. Le service est le symétrique
    /// du scellement : il défait le placement provisoire posé par le rattachement, là où le
    /// scellement le consacre.
    /// </para>
    /// <para>
    /// La libération opérée ne diffère de celle du service jumeau
    /// <c>IS_ProductionCutPiece_DetachFromBar</c>, qui sert le chemin de validation avec défauts où
    /// le plan de coupe est simplement refait, que par la pose du marqueur de refus de coupe.
    /// L'emploi de l'un pour l'autre ne produirait aucune erreur visible, mais fausserait l'analyse
    /// des refus de coupe. Le traitement est intégral : tous les contrôles précèdent tout marquage,
    /// toutes les découpes sont libérées ou aucune, et tout échec survenant pendant la délégation
    /// est neutralisé par l'annulation de la transaction de l'appelant.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle portant sur l'identifiant de barre.</description></item>
    /// <item><description>Charger les découpes rattachées à la barre en une lecture unique suivie via <see cref="IQ_Generic{T}.HandleGetFilteredAsync"/>.</description></item>
    /// <item><description>Vérifier la présence d'au moins une découpe rattachée et la compatibilité de l'état de chacune avec la libération.</description></item>
    /// <item><description>Positionner sur chaque découpe le marqueur de refus de coupe, lever le marqueur de placement provisoire, et effacer le rattachement à la barre et la position de coupe.</description></item>
    /// <item><description>Déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateRangeAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne vérifie ni l'existence ni l'état de la barre : sa mise à l'écart, préalable à la libération dans la même transaction, relève d'un autre service.</description></item>
    /// <item><description>Ne positionne pas les champs d'audit : <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> : toute valeur nécessaire lui est fournie par argument depuis le UseCase.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Query Handler et le Command Handler.</description></item>
    /// <item><description>Ne journalise ni ne notifie.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie : le refus est tracé une seule fois, sur la barre, par le UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionCutPiece_DetachAndRefuse"/>
    /// <seealso cref="IS_ProductionCutPiece_AssignToBar"/>
    /// <seealso cref="IS_ProductionCutPiece_SealToBar"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionCutPiece_DetachAndRefuse : IS_ProductionCutPiece_DetachAndRefuse
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
        /// Command Handler générique auquel est déléguée la mise à jour des découpes libérées.
        /// </summary>
        private readonly IC_Generic<ProductionCutPiece> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionCutPiece_DetachAndRefuse"/> avec ses dépendances.
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
        public SR_ProductionCutPiece_DetachAndRefuse(
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

        // --- Groupe 1 : Libération sur refus des découpes d'une barre de production ---

        /// <summary>
        /// Libère toutes les découpes provisoirement placées sur la barre de production refusée
        /// désignée, en inscrivant sur chacune la trace du refus de coupe, puis confie leur mise à
        /// jour au Command Handler générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur du refus de barre, à l'intérieur de la
        /// transaction qu'il a ouverte et après la mise à l'écart de la barre. Les découpes sont
        /// lues en une requête unique avec suivi des changements, filtrée sur le seul rattachement à
        /// la barre : les découpes supprimées logiquement restent ainsi visibles et sont rejetées
        /// par le contrôle d'état. Le Command Handler générique positionne ensuite la date de mise à
        /// jour et inscrit un événement par découpe. L'enregistrement effectif n'intervient qu'à la
        /// validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// L'opération n'est pas idempotente : un second appel sur la même barre échoue, aucune
        /// découpe n'y étant plus rattachée.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de barre est strictement positif, avant toute lecture.</description></item>
        /// <item><description>Charger en une lecture unique suivie toutes les découpes rattachées à la barre.</description></item>
        /// <item><description>Vérifier qu'au moins une découpe est rattachée : une barre présentée à l'opérateur porte nécessairement un plan de coupe.</description></item>
        /// <item><description>Vérifier l'état de chaque découpe et rejeter en un échec unique l'ensemble des découpes non placées provisoirement, déjà scellées, déjà réalisées, supprimées logiquement, en rupture de stock de barre ou dépourvues de position de coupe, triées par position de coupe croissante, positions absentes en dernier, puis par identifiant.</description></item>
        /// <item><description>Positionner sur chaque découpe, dans cet ordre, le marqueur de refus de coupe à <see langword="true"/>, le marqueur de placement provisoire à <see langword="false"/>, le rattachement à la barre et la position de coupe à <see langword="null"/>.</description></item>
        /// <item><description>Transmettre au Command Handler générique, en un appel unique, les instances chargées et marquées.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ : le marqueur de placement définitif, les marqueurs d'approvisionnement de la barre, de rupture de stock, de réalisation et de suppression logique, les horodatages de coupe, la date de création et la date de mise à jour restent en dehors du marquage.</description></item>
        /// <item><description>Ne marque aucune découpe avant que tous les contrôles soient passés.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie ni l'existence ni l'état de la barre.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production refusée dont les découpes rattachées sont à libérer. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionBar"/> n'est pas
        /// strictement positif (valeur reçue citée) ; avec le code <c>BU_ER_03</c> si aucune découpe
        /// n'est rattachée à la barre désignée (barre citée) ; avec le code <c>BU_ER_04</c>, en un
        /// échec unique, si des découpes rattachées ne sont pas libérables (barre, chaque découpe et
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

                // C1 - Une barre présentée à l'opérateur porte nécessairement un plan de coupe.
                if (pieces.Count == 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Aucune découpe n'est rattachée à la barre de production {idProductionBar}, alors qu'une barre présentée à l'opérateur porte nécessairement un plan de coupe ; appel hors séquence (identifiant de barre sans plan de coupe ou découpes déjà libérées).");

                // C2 - État compatible avec la libération, contrôlé sur chaque découpe AVANT toute
                // mutation ; les découpes fautives sont citées dans l'ordre de coupe, en un échec
                // unique.
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
                        $"L'état des découpes suivantes ne permet pas leur libération sur refus de la barre de production {idProductionBar} : {string.Join(" ; ", stateViolations)}.");

                // M - Marquage, après succès de tous les contrôles. IsOptimized n'est jamais
                // touché : les découpes retournent au vivier des pièces à optimiser.
                foreach (ProductionCutPiece piece in pieces)
                {
                    piece.IsCutRefused = true;
                    piece.IsOptimizedTemp = false;
                    piece.IdProductionBar = null;
                    piece.CutPositionInBar = null;
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
        /// Décrit les conditions d'état qui interdisent la libération d'une découpe sur refus de sa
        /// barre, ou indique qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée pour chaque découpe chargée, avant tout marquage. Une découpe est
        /// libérable si elle est placée provisoirement, non encore scellée, non réalisée, non
        /// supprimée logiquement, non en rupture de stock de barre et pourvue d'une position de
        /// coupe ; toutes les conditions violées sont citées ensemble, dans cet ordre, afin que
        /// l'échec agrégé renseigne complètement l'appelant. Une découpe déjà scellée signale une
        /// barre dont le plan de coupe est définitif et qui n'est donc plus refusable.
        /// </para>
        /// </remarks>
        /// <param name="piece">Découpe chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Description de la découpe et des conditions violées, prête à être citée dans le message
        /// d'échec ; <see langword="null"/> si l'état de la découpe permet sa libération.
        /// </returns>
        private static string? DescribeStateViolation(ProductionCutPiece piece)
        {
            List<string> conditions = new();

            if (!piece.IsOptimizedTemp)
                conditions.Add("non placée provisoirement");

            if (piece.IsOptimized)
                conditions.Add("déjà scellée");

            if (piece.IsCut)
                conditions.Add("déjà réalisée");

            if (piece.IsDeleted)
                conditions.Add("supprimée logiquement");

            if (piece.IsBarOutOfStock)
                conditions.Add("en rupture de stock de barre");

            if (piece.CutPositionInBar is null)
                conditions.Add("sans position de coupe");

            return conditions.Count == 0
                ? null
                : $"découpe {piece.Id} ({string.Join(", ", conditions)})";
        }

        #endregion
    }
}