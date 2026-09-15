using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Moteur d'optimisation de découpe : service métier de calcul pur déterminant la
    /// matière à mobiliser et les découpes à y placer pour une référence de profilé.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : implémentation de <see cref="IS_CuttingOptimizer"/> résidant en
    /// <c>B_UseCases/Services/Business</c>. Le service est consommé par les UseCases de
    /// préparation et de validation des barres de production, à l'intérieur de leur
    /// transaction. Il reçoit toutes ses données par arguments, n'injecte aucun Setting et
    /// ne consomme aucun Handler : sa seule dépendance est le classifieur d'exceptions
    /// <see cref="IS_ExClassifier"/>. Sans dépendance scoped, il est enregistré en
    /// Singleton.
    /// </para>
    /// <para>
    /// Objectif : minimiser la matière non réutilisable et faire tourner en priorité le
    /// stock de chutes, en déterminant un unique contenant par invocation. Le garnissage
    /// d'un contenant repose sur une table des consommations atteignables en millimètres
    /// entiers, chaque découpe consommant sa longueur arrondie à l'entier supérieur
    /// augmentée d'un trait de scie ; la consommation retenue est la plus forte qui laisse
    /// un résidu qualifié de chute, à défaut la plus forte, sans jamais retenir une
    /// consommation nulle.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Lire et contrôler les paramètres de coupe figés sur la première découpe du vivier.</description></item>
    /// <item><description>Garnir chaque contenant évalué et qualifier son résidu en chute réutilisable ou en déchet.</description></item>
    /// <item><description>Départager les chutes candidates selon un ordre lexicographique total, garant du déterminisme.</description></item>
    /// <item><description>Découper une barre à défauts en segments sains et garnir le premier segment exploitable.</description></item>
    /// <item><description>Restituer les découpes retenues dans l'ordre de coupe et exprimer les issues métier par le discriminant du résultat.</description></item>
    /// <item><description>Requalifier toute exception imprévue via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne lit ni n'écrit aucune donnée, ne pilote aucune transaction, ne journalise pas et ne notifie pas l'opérateur.</description></item>
    /// <item><description>Ne conserve aucun état d'instance lié à une invocation : les tables de travail sont locales à chaque appel et les entrées ne sont jamais modifiées.</description></item>
    /// <item><description>Ne filtre pas le stock de chutes ni ne trie le vivier : ces garanties incombent à l'appelant.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_CuttingOptimizer"/>
    /// <seealso cref="DTO_CuttingOptimizationResult"/>
    public class SR_CuttingOptimizer : IS_CuttingOptimizer
    {
        #region === Propriétés privées ===

        /// <summary>Nom réel de la classe, segment local des CallChains construites par le service.</summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>Classifieur terminal des exceptions imprévues.</summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_CuttingOptimizer"/> avec le
        /// classifieur d'exceptions.
        /// </summary>
        /// <param name="classifier">Service de requalification des exceptions imprévues. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_CuttingOptimizer(IS_ExClassifier classifier)
        {
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Détermine le meilleur contenant pour le vivier de découpes reçu, en privilégiant
        /// les chutes du stock, et le garnit des découpes qui en optimisent le rendement.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase de préparation d'une barre. Après contrôle des
        /// préconditions structurelles, le vivier vide est traité comme issue nominale, puis
        /// les paramètres figés sont lus et contrôlés. Chaque chute est garnie en mode
        /// standard ; les chutes garnies sont départagées par
        /// <see cref="IsBetterScrapCandidate"/>. À défaut de chute garnie, la longueur de
        /// barre neuve est contrôlée puis la barre neuve est garnie en mode standard ; une
        /// barre neuve non garnie traduit une anomalie de données.
        /// </para>
        /// <para>
        /// Objectif : les tables de travail sont allouées une seule fois pour l'invocation,
        /// dimensionnées sur la plus grande capacité de chute, réinitialisées à chaque
        /// contenant et étendues si la barre neuve l'exige.
        /// </para>
        /// <para>
        /// Ordre de restitution : les identifiants de découpes sont ordonnés par longueur
        /// arrondie décroissante puis par identifiant croissant ; le rang de chaque
        /// identifiant, compté à partir de 1, vaut position de coupe dans la barre. Cet
        /// ordre, également porté par le résultat, ne doit pas être modifié en aval sous
        /// peine de fausser le plan de coupe.
        /// </para>
        /// <para>
        /// Dénomination : la méthode publique porte le verbe <c>Optimize</c> en dérogation
        /// explicite au préfixe <c>Execute</c> posé par défaut par R-4.2.12 du 0231, et
        /// coexiste avec <see cref="OptimizeWithDefects"/> en dérogation à l'unicité de la
        /// méthode publique ; cette double dérogation est typologiquement bornée au cas
        /// Concept (item SR20 du 0232-SR). La sémantique du concept porté, l'optimisation
        /// de l'imbrication des découpes dans un contenant, s'exprime naturellement par le
        /// verbe d'action anglais à l'impératif <c>Optimize</c>, plus précis qu'<c>Execute</c>
        /// pour distinguer cette opération de la recomposition d'une barre à défauts. Cette
        /// trace nominative satisfait l'exigence de I-4.2.6 du 0231.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="pool">Vivier des découpes à optimiser, trié par identifiant croissant. Ne doit pas être <see langword="null"/> ; vide admis. N'est jamais modifié.</param>
        /// <param name="scrapStock">Chutes disponibles de l'article traité. Ne doit pas être <see langword="null"/> ; vide admis. N'est jamais modifié.</param>
        /// <returns>
        /// Le résultat de l'optimisation, jamais <see langword="null"/> : issue
        /// <see cref="En_CuttingOptimizationOutcome.Success"/> avec contenant, contenu
        /// ordonné et qualification du résidu (<see langword="true"/> = chute réutilisable,
        /// <see langword="false"/> = déchet) ; ou issue
        /// <see cref="En_CuttingOptimizationOutcome.EmptyPool"/> ou
        /// <see cref="En_CuttingOptimizationOutcome.DataAnomaly"/> sans contenant ni contenu.
        /// </returns>
        /// <exception cref="Ex_Business">Levée, au code <c>BU_ER_01</c>, lorsque <paramref name="pool"/> ou <paramref name="scrapStock"/> est <see langword="null"/>.</exception>
        /// <exception cref="Ex_Unclassified">Levée lorsqu'une défaillance imprévue du calcul est requalifiée par le classifieur d'exceptions.</exception>
        public DTO_CuttingOptimizationResult Optimize(
            string caller,
            IReadOnlyList<DTO_VwProductionCutPieceFull_P20> pool,
            IReadOnlyList<vw_CuttingScrapStock_Full> scrapStock)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Optimize)}";

            try
            {
                if (pool is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le vivier des découpes à optimiser est nul.");

                if (scrapStock is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le stock de chutes candidates est nul.");

                if (pool.Count == 0)
                    return BuildOutcome(En_CuttingOptimizationOutcome.EmptyPool);

                if (!TryReadFrozenParameters(
                        pool,
                        out int finishingCut,
                        out int sawCut,
                        out bool manageScraps,
                        out decimal scrapThreshold,
                        out int[] pieceLengths))
                    return BuildOutcome(En_CuttingOptimizationOutcome.DataAnomaly);

                int maxCapacity = 0;
                foreach (vw_CuttingScrapStock_Full scrap in scrapStock)
                {
                    maxCapacity = Math.Max(maxCapacity, ComputeCapacity(scrap.CSSLengthMm, finishingCut, sawCut));
                }

                bool[] reachable = new bool[maxCapacity + 1];
                int[] origin = new int[maxCapacity + 1];

                vw_CuttingScrapStock_Full? bestScrap = null;
                List<int> bestContent = [];
                int bestResidue = 0;
                bool bestQualified = false;

                foreach (vw_CuttingScrapStock_Full scrap in scrapStock)
                {
                    List<int> content = FillContainer(
                        scrap.CSSLengthMm,
                        pieceLengths,
                        finishingCut,
                        sawCut,
                        manageScraps,
                        scrapThreshold,
                        false,
                        ref reachable,
                        ref origin,
                        out int residue,
                        out bool qualified);

                    if (content.Count == 0)
                        continue;

                    if (bestScrap is null
                        || IsBetterScrapCandidate(scrap, residue, qualified, bestScrap, bestResidue, bestQualified))
                    {
                        bestScrap = scrap;
                        bestContent = content;
                        bestResidue = residue;
                        bestQualified = qualified;
                    }
                }

                if (bestScrap is not null)
                {
                    return BuildSuccess(
                        false,
                        bestScrap.CSSId,
                        bestScrap.CSSLengthMm,
                        OrderPieceIds(bestContent, pieceLengths, pool),
                        bestResidue,
                        bestQualified);
                }

                if (!TryReadNewBarLength(pool[0], out int newBarLength))
                    return BuildOutcome(En_CuttingOptimizationOutcome.DataAnomaly);

                List<int> newBarContent = FillContainer(
                    newBarLength,
                    pieceLengths,
                    finishingCut,
                    sawCut,
                    manageScraps,
                    scrapThreshold,
                    false,
                    ref reachable,
                    ref origin,
                    out int newBarResidue,
                    out bool newBarQualified);

                if (newBarContent.Count == 0)
                    return BuildOutcome(En_CuttingOptimizationOutcome.DataAnomaly);

                return BuildSuccess(
                    true,
                    null,
                    newBarLength,
                    OrderPieceIds(newBarContent, pieceLengths, pool),
                    newBarResidue,
                    newBarQualified);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Recompose le plan de coupe d'une barre déjà validée dont une ou deux zones sont
        /// défectueuses, en garnissant un unique segment sain de la barre imposée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase de validation d'une barre à défauts. Après
        /// contrôle des préconditions structurelles, le vivier vide est traité comme issue
        /// nominale, puis les paramètres figés sont lus et contrôlés sans la longueur de
        /// barre neuve. La barre est découpée en segments sains par
        /// <see cref="BuildSegmentLengths"/>, parcourus de la tête vers le terminal : un
        /// segment intermédiaire est garni en consommation maximale, le segment terminal en
        /// mode standard, et le premier segment garni est retenu.
        /// </para>
        /// <para>
        /// Objectif : le résidu restitué est toujours le segment terminal ; s'il n'est pas
        /// le segment retenu, sa longueur physique entière est restituée et qualifiée selon
        /// la règle commune. Les tables de travail sont allouées une seule fois pour
        /// l'invocation, dimensionnées sur le plus grand segment.
        /// </para>
        /// <para>
        /// Ordre de restitution : les identifiants de découpes sont ordonnés par longueur
        /// arrondie décroissante puis par identifiant croissant ; le rang de chaque
        /// identifiant, compté à partir de 1, vaut position de coupe dans la barre. Cet
        /// ordre, également porté par le résultat, ne doit pas être modifié en aval sous
        /// peine de fausser le plan de coupe.
        /// </para>
        /// <para>
        /// Dénomination : la méthode publique porte le verbe <c>OptimizeWithDefects</c> en
        /// dérogation explicite au préfixe <c>Execute</c> posé par défaut par R-4.2.12 du
        /// 0231, et coexiste avec <see cref="Optimize"/> en dérogation à l'unicité de la
        /// méthode publique ; cette double dérogation est typologiquement bornée au cas
        /// Concept (item SR20 du 0232-SR). La sémantique du concept porté, l'optimisation de
        /// l'imbrication des découpes dans les parties saines d'une barre défectueuse,
        /// s'exprime par le verbe d'action anglais à l'impératif <c>Optimize</c> qualifié de
        /// son complément <c>WithDefects</c>, qui la distingue de l'optimisation d'un
        /// contenant sain. Cette trace nominative satisfait l'exigence de I-4.2.6 du 0231.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="pool">Vivier des découpes à placer, trié par identifiant croissant. Ne doit pas être <see langword="null"/> ; vide admis. N'est jamais modifié.</param>
        /// <param name="barLength">Longueur physique de la barre imposée, en millimètres. Doit être strictement positive.</param>
        /// <param name="isNewBar">Origine de la barre imposée : <see langword="true"/> pour une barre neuve, <see langword="false"/> pour une chute.</param>
        /// <param name="idSourceScrap">Identifiant de la chute source ; <see langword="null"/> si et seulement si <paramref name="isNewBar"/> vaut <see langword="true"/>.</param>
        /// <param name="defectStart1">Début de la première zone défectueuse, en millimètres depuis l'extrémité de tête, compris entre 0 et <paramref name="barLength"/>.</param>
        /// <param name="defectEnd1">Fin de la première zone défectueuse, comprise entre 0 et <paramref name="barLength"/>, strictement supérieure à <paramref name="defectStart1"/>.</param>
        /// <param name="defectStart2">Début de la seconde zone défectueuse, ou <see langword="null"/> ; renseigné, compris entre <paramref name="defectEnd1"/> et <paramref name="barLength"/>.</param>
        /// <param name="defectEnd2">Fin de la seconde zone défectueuse, renseignée si et seulement si <paramref name="defectStart2"/> l'est ; comprise entre 0 et <paramref name="barLength"/>, strictement supérieure à <paramref name="defectStart2"/>.</param>
        /// <returns>
        /// Le résultat de la recomposition, jamais <see langword="null"/> : issue
        /// <see cref="En_CuttingOptimizationOutcome.Success"/> avec contenant recopié,
        /// contenu ordonné et qualification du segment terminal (<see langword="true"/> =
        /// chute réutilisable, <see langword="false"/> = déchet) ; ou issue
        /// <see cref="En_CuttingOptimizationOutcome.EmptyPool"/>,
        /// <see cref="En_CuttingOptimizationOutcome.DataAnomaly"/> ou
        /// <see cref="En_CuttingOptimizationOutcome.NoUsableSegment"/> sans contenant ni
        /// contenu.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée au code <c>BU_ER_01</c> si <paramref name="pool"/> est
        /// <see langword="null"/> ; au code <c>BU_ER_02</c> si <paramref name="barLength"/>
        /// n'est pas strictement positive ou si une borne renseignée sort de l'intervalle
        /// [0 ; <paramref name="barLength"/>] ; au code <c>BU_ER_03</c> si les zones sont
        /// désordonnées, incomplètes ou chevauchantes, ou si
        /// <paramref name="idSourceScrap"/> est incohérent avec <paramref name="isNewBar"/>.
        /// </exception>
        /// <exception cref="Ex_Unclassified">Levée lorsqu'une défaillance imprévue du calcul est requalifiée par le classifieur d'exceptions.</exception>
        public DTO_CuttingOptimizationResult OptimizeWithDefects(
            string caller,
            IReadOnlyList<DTO_VwProductionCutPieceFull_P20> pool,
            int barLength,
            bool isNewBar,
            int? idSourceScrap,
            int defectStart1,
            int defectEnd1,
            int? defectStart2,
            int? defectEnd2)
        {
            string callChain = $"{caller} > {_callee} > {nameof(OptimizeWithDefects)}";

            try
            {
                if (pool is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le vivier des découpes à placer sur la barre à défauts est nul.");

                if (barLength <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"La longueur de la barre à défauts ({barLength} mm) n'est pas strictement positive.");

                if (!IsBoundWithinBar(defectStart1, barLength)
                    || !IsBoundWithinBar(defectEnd1, barLength)
                    || (defectStart2.HasValue && !IsBoundWithinBar(defectStart2.Value, barLength))
                    || (defectEnd2.HasValue && !IsBoundWithinBar(defectEnd2.Value, barLength)))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"Une borne de zone défectueuse sort de l'intervalle [0 ; {barLength}] mm "
                        + $"(zone 1 : {defectStart1}-{defectEnd1} ; zone 2 : {defectStart2?.ToString() ?? "-"}-{defectEnd2?.ToString() ?? "-"}).");

                if (defectStart1 >= defectEnd1)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Les bornes de la première zone défectueuse sont désordonnées ({defectStart1} >= {defectEnd1} mm).");

                if (defectStart2.HasValue != defectEnd2.HasValue)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        "La seconde zone défectueuse est incomplète : une seule de ses deux bornes est renseignée.");

                if (defectStart2.HasValue
                    && (defectStart2.Value >= defectEnd2!.Value || defectStart2.Value < defectEnd1))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La seconde zone défectueuse ({defectStart2.Value}-{defectEnd2.Value} mm) est désordonnée "
                        + $"ou chevauche la première ({defectStart1}-{defectEnd1} mm).");

                if (isNewBar == idSourceScrap.HasValue)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"L'identifiant de chute source ({idSourceScrap?.ToString() ?? "null"}) est incohérent avec "
                        + $"l'origine déclarée de la barre (barre neuve : {isNewBar}).");

                if (pool.Count == 0)
                    return BuildOutcome(En_CuttingOptimizationOutcome.EmptyPool);

                if (!TryReadFrozenParameters(
                        pool,
                        out int finishingCut,
                        out int sawCut,
                        out bool manageScraps,
                        out decimal scrapThreshold,
                        out int[] pieceLengths))
                    return BuildOutcome(En_CuttingOptimizationOutcome.DataAnomaly);

                int[] segmentLengths = BuildSegmentLengths(barLength, defectStart1, defectEnd1, defectStart2, defectEnd2);
                int terminalIndex = segmentLengths.Length - 1;

                int maxCapacity = 0;
                foreach (int segmentLength in segmentLengths)
                {
                    maxCapacity = Math.Max(maxCapacity, ComputeCapacity(segmentLength, finishingCut, sawCut));
                }

                bool[] reachable = new bool[maxCapacity + 1];
                int[] origin = new int[maxCapacity + 1];

                for (int segmentIndex = 0; segmentIndex <= terminalIndex; segmentIndex++)
                {
                    bool isTerminal = segmentIndex == terminalIndex;

                    List<int> content = FillContainer(
                        segmentLengths[segmentIndex],
                        pieceLengths,
                        finishingCut,
                        sawCut,
                        manageScraps,
                        scrapThreshold,
                        !isTerminal,
                        ref reachable,
                        ref origin,
                        out int residue,
                        out bool qualified);

                    if (content.Count == 0)
                        continue;

                    int keptResidue;
                    bool keptQualified;

                    if (isTerminal)
                    {
                        keptResidue = residue;
                        keptQualified = qualified;
                    }
                    else
                    {
                        keptResidue = segmentLengths[terminalIndex];
                        keptQualified = IsQualifiedScrap(keptResidue, finishingCut, manageScraps, scrapThreshold);
                    }

                    return BuildSuccess(
                        isNewBar,
                        idSourceScrap,
                        barLength,
                        OrderPieceIds(content, pieceLengths, pool),
                        keptResidue,
                        keptQualified);
                }

                return BuildOutcome(En_CuttingOptimizationOutcome.NoUsableSegment);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Lit et contrôle les paramètres figés de l'invocation et les longueurs arrondies
        /// de toutes les découpes du vivier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par les deux méthodes publiques après le test de vivier vide.
        /// Coupe de propreté, trait de scie, longueur minimale de chute et gestion des
        /// chutes sont invariants pour un article interne et lus sur la première découpe ;
        /// la longueur de chaque découpe est arrondie à l'entier supérieur.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Rejeter une coupe de propreté ou un trait de scie absent ou négatif, la valeur zéro étant admise.</description></item>
        /// <item><description>Rejeter une longueur de découpe absente ou non strictement positive après arrondi.</description></item>
        /// <item><description>Rejeter une longueur minimale de chute absente ou négative, uniquement si l'article gère les chutes ; une gestion des chutes absente vaut absence de gestion.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne contrôle pas la longueur de barre neuve, lue par <see cref="TryReadNewBarLength"/> seulement si la barre neuve est évaluée.</description></item>
        /// <item><description>Ne vérifie pas l'homogénéité des paramètres entre les découpes du vivier.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="pool">Vivier non vide des découpes.</param>
        /// <param name="finishingCut">Longueur de la coupe de propreté, en millimètres ; sans signification si la méthode retourne <see langword="false"/>.</param>
        /// <param name="sawCut">Épaisseur du trait de scie, en millimètres ; sans signification si la méthode retourne <see langword="false"/>.</param>
        /// <param name="manageScraps">Indique si l'article gère les chutes.</param>
        /// <param name="scrapThreshold">Longueur minimale de chute, en millimètres ; vaut zéro, sans être utilisée, si l'article ne gère pas les chutes.</param>
        /// <param name="pieceLengths">Longueurs arrondies des découpes, dans l'ordre du vivier ; sans signification si la méthode retourne <see langword="false"/>.</param>
        /// <returns>
        /// <see langword="true"/> si toutes les valeurs contrôlées sont exploitables ;
        /// <see langword="false"/> si l'une d'elles est inexploitable, ce qui traduit une
        /// anomalie de données.
        /// </returns>
        /// <exception cref="OverflowException">Levée si une longueur de découpe arrondie excède la capacité d'un entier.</exception>
        private static bool TryReadFrozenParameters(
            IReadOnlyList<DTO_VwProductionCutPieceFull_P20> pool,
            out int finishingCut,
            out int sawCut,
            out bool manageScraps,
            out decimal scrapThreshold,
            out int[] pieceLengths)
        {
            DTO_VwProductionCutPieceFull_P20 first = pool[0];

            finishingCut = first.PCPFinishingCutLength ?? 0;
            sawCut = first.PCPSawCutLength ?? 0;
            manageScraps = first.AIManageScraps == true;
            scrapThreshold = 0m;
            pieceLengths = new int[pool.Count];

            if (first.PCPFinishingCutLength is null or < 0)
                return false;

            if (first.PCPSawCutLength is null or < 0)
                return false;

            for (int index = 0; index < pool.Count; index++)
            {
                decimal? cutDimension = pool[index].PCPCutDimension;
                if (cutDimension is null)
                    return false;

                decimal roundedLength = Math.Ceiling(cutDimension.Value);
                if (roundedLength <= 0m)
                    return false;

                pieceLengths[index] = (int)roundedLength;
            }

            if (manageScraps)
            {
                if (first.ARMinScrapLength is null or < 0m)
                    return false;

                scrapThreshold = first.ARMinScrapLength.Value;
            }

            return true;
        }

        /// <summary>
        /// Lit et contrôle la longueur de barre neuve de l'article, arrondie à l'entier
        /// inférieur.
        /// </summary>
        /// <param name="first">Première découpe du vivier, porteuse des caractéristiques de l'article.</param>
        /// <param name="newBarLength">Longueur de barre neuve en millimètres ; sans signification si la méthode retourne <see langword="false"/>.</param>
        /// <returns>
        /// <see langword="true"/> si la longueur est renseignée et strictement positive
        /// après arrondi ; <see langword="false"/> sinon, ce qui traduit une anomalie de
        /// données.
        /// </returns>
        /// <exception cref="OverflowException">Levée si la longueur arrondie excède la capacité d'un entier.</exception>
        private static bool TryReadNewBarLength(DTO_VwProductionCutPieceFull_P20 first, out int newBarLength)
        {
            newBarLength = 0;

            if (first.AIStandardBarLengthMm is null)
                return false;

            double roundedLength = Math.Floor(first.AIStandardBarLengthMm.Value);
            if (!(roundedLength > 0d))
                return false;

            newBarLength = checked((int)roundedLength);
            return true;
        }

        /// <summary>
        /// Calcule la capacité de la table des consommations d'un contenant : longueur
        /// utile, après coupe de propreté, augmentée d'un trait de scie.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : chaque découpe consommant sa longueur augmentée d'un trait de scie, la
        /// capacité intègre un trait supplémentaire afin que la dernière découpe n'en
        /// consomme pas lorsqu'elle achève exactement la longueur utile.
        /// </para>
        /// </remarks>
        /// <param name="containerLength">Longueur physique du contenant, en millimètres.</param>
        /// <param name="finishingCut">Longueur de la coupe de propreté, en millimètres, positive ou nulle.</param>
        /// <param name="sawCut">Épaisseur du trait de scie, en millimètres, positive ou nulle.</param>
        /// <returns>La capacité en millimètres, ou zéro si la longueur utile est nulle ou négative.</returns>
        /// <exception cref="OverflowException">Levée si la capacité excède la capacité d'un entier.</exception>
        private static int ComputeCapacity(int containerLength, int finishingCut, int sawCut)
        {
            int usableLength = checked(containerLength - finishingCut);
            return usableLength <= 0 ? 0 : checked(usableLength + sawCut);
        }

        /// <summary>
        /// Garnit un contenant des découpes du vivier selon le mode de cible demandé et
        /// qualifie le résidu obtenu.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : cœur de calcul commun aux deux méthodes publiques. Les découpes sont
        /// parcourues dans l'ordre du vivier ; pour chacune, la table des consommations
        /// atteignables est mise à jour par capacités décroissantes, l'origine d'une
        /// consommation n'étant enregistrée qu'à sa première atteinte, ce qui garantit une
        /// reconstruction déterministe par des découpes distinctes. Le résidu physique
        /// d'une consommation vaut la capacité diminuée de cette consommation et d'un trait
        /// de scie, borné à zéro.
        /// </para>
        /// <para>
        /// Objectif : en mode standard, la consommation retenue est la plus forte qui laisse
        /// un résidu qualifié de chute, à défaut la plus forte ; en mode de consommation
        /// maximale, la plus forte. La cible est toujours choisie parmi les consommations
        /// strictement positives : un contenant dont le reste à vide serait qualifié reçoit
        /// ainsi des découpes même lorsque toutes les combinaisons tombent sous le seuil.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Étendre les tables de travail si la capacité du contenant l'exige, puis les réinitialiser sur la capacité utile.</description></item>
        /// <item><description>Reconstruire le contenu par retour arrière sur la table des origines.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>N'ordonne pas le contenu pour la restitution : ce rôle revient à <see cref="OrderPieceIds"/>.</description></item>
        /// <item><description>Ne modifie aucune entrée autre que les tables de travail de l'invocation.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="containerLength">Longueur physique du contenant, en millimètres.</param>
        /// <param name="pieceLengths">Longueurs arrondies des découpes, strictement positives, dans l'ordre du vivier.</param>
        /// <param name="finishingCut">Longueur de la coupe de propreté, en millimètres, positive ou nulle.</param>
        /// <param name="sawCut">Épaisseur du trait de scie, en millimètres, positive ou nulle.</param>
        /// <param name="manageScraps">Indique si l'article gère les chutes.</param>
        /// <param name="scrapThreshold">Longueur minimale de chute, en millimètres.</param>
        /// <param name="maximizeConsumption"><see langword="true"/> pour le mode de consommation maximale ; <see langword="false"/> pour le mode standard.</param>
        /// <param name="reachable">Table de travail des consommations atteignables, locale à l'invocation, étendue au besoin.</param>
        /// <param name="origin">Table de travail des découpes d'origine, locale à l'invocation, étendue au besoin.</param>
        /// <param name="residue">Longueur physique du résidu en millimètres ; zéro si le contenu est vide.</param>
        /// <param name="residueIsScrap"><see langword="true"/> si le résidu est une chute réutilisable ; <see langword="false"/> s'il est un déchet ou si le contenu est vide.</param>
        /// <returns>
        /// Les index, dans le vivier, des découpes retenues ; liste vide si aucune découpe
        /// ne tient dans le contenant, jamais <see langword="null"/>.
        /// </returns>
        /// <exception cref="OverflowException">Levée si une capacité ou une consommation excède la capacité d'un entier.</exception>
        private static List<int> FillContainer(
            int containerLength,
            int[] pieceLengths,
            int finishingCut,
            int sawCut,
            bool manageScraps,
            decimal scrapThreshold,
            bool maximizeConsumption,
            ref bool[] reachable,
            ref int[] origin,
            out int residue,
            out bool residueIsScrap)
        {
            residue = 0;
            residueIsScrap = false;

            int capacity = ComputeCapacity(containerLength, finishingCut, sawCut);
            if (capacity <= 0)
                return [];

            if (reachable.Length < capacity + 1)
            {
                reachable = new bool[capacity + 1];
                origin = new int[capacity + 1];
            }

            Array.Clear(reachable, 0, capacity + 1);
            Array.Clear(origin, 0, capacity + 1);
            reachable[0] = true;

            for (int pieceIndex = 0; pieceIndex < pieceLengths.Length; pieceIndex++)
            {
                int weight = checked(pieceLengths[pieceIndex] + sawCut);

                for (int consumption = capacity; consumption >= weight; consumption--)
                {
                    if (reachable[consumption - weight] && !reachable[consumption])
                    {
                        reachable[consumption] = true;
                        origin[consumption] = pieceIndex;
                    }
                }
            }

            int target = 0;
            int highestConsumption = 0;

            for (int consumption = capacity; consumption >= 1; consumption--)
            {
                if (!reachable[consumption])
                    continue;

                if (highestConsumption == 0)
                    highestConsumption = consumption;

                if (maximizeConsumption)
                    break;

                int candidateResidue = Math.Max(0, capacity - consumption - sawCut);
                if (IsQualifiedScrap(candidateResidue, finishingCut, manageScraps, scrapThreshold))
                {
                    target = consumption;
                    break;
                }
            }

            if (target == 0)
                target = highestConsumption;

            if (target == 0)
                return [];

            List<int> content = [];
            int remaining = target;
            while (remaining > 0)
            {
                int pieceIndex = origin[remaining];
                content.Add(pieceIndex);
                remaining -= pieceLengths[pieceIndex] + sawCut;
            }

            residue = Math.Max(0, capacity - target - sawCut);
            residueIsScrap = IsQualifiedScrap(residue, finishingCut, manageScraps, scrapThreshold);
            return content;
        }

        /// <summary>
        /// Détermine si un résidu physique est une chute réutilisable ou un déchet.
        /// </summary>
        /// <param name="residue">Longueur physique du résidu, en millimètres.</param>
        /// <param name="finishingCut">Longueur de la coupe de propreté, en millimètres, que la réutilisation du résidu consommera.</param>
        /// <param name="manageScraps">Indique si l'article gère les chutes.</param>
        /// <param name="scrapThreshold">Longueur minimale de chute, en millimètres.</param>
        /// <returns>
        /// <see langword="true"/> si l'article gère les chutes et si le résidu diminué d'une
        /// coupe de propreté dépasse strictement la longueur minimale de chute (chute
        /// réutilisable) ; <see langword="false"/> sinon (déchet).
        /// </returns>
        private static bool IsQualifiedScrap(int residue, int finishingCut, bool manageScraps, decimal scrapThreshold)
        {
            return manageScraps && (decimal)((long)residue - finishingCut) > scrapThreshold;
        }

        /// <summary>
        /// Détermine si une chute garnie candidate l'emporte sur la meilleure chute retenue
        /// jusqu'alors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : ordre lexicographique total appliqué par <see cref="Optimize"/> aux
        /// chutes recevant au moins une découpe. Les critères sont, dans l'ordre : résidu
        /// chute avant résidu déchet ; entre deux résidus déchet, résidu le plus court ;
        /// entre deux résidus chute, chute la plus courte ; date d'entrée en stock la plus
        /// ancienne ; identifiant le plus petit. Le dernier critère étant unique, l'ordre
        /// est strict et garantit le déterminisme indépendamment de l'ordre du stock reçu.
        /// </para>
        /// </remarks>
        /// <param name="candidate">Chute candidate.</param>
        /// <param name="candidateResidue">Résidu physique de la chute candidate, en millimètres.</param>
        /// <param name="candidateQualified">Qualification du résidu de la chute candidate.</param>
        /// <param name="best">Meilleure chute retenue jusqu'alors.</param>
        /// <param name="bestResidue">Résidu physique de la meilleure chute, en millimètres.</param>
        /// <param name="bestQualified">Qualification du résidu de la meilleure chute.</param>
        /// <returns>
        /// <see langword="true"/> si la candidate précède strictement la meilleure chute
        /// dans l'ordre de préférence ; <see langword="false"/> sinon.
        /// </returns>
        private static bool IsBetterScrapCandidate(
            vw_CuttingScrapStock_Full candidate,
            int candidateResidue,
            bool candidateQualified,
            vw_CuttingScrapStock_Full best,
            int bestResidue,
            bool bestQualified)
        {
            if (candidateQualified != bestQualified)
                return candidateQualified;

            if (!candidateQualified && candidateResidue != bestResidue)
                return candidateResidue < bestResidue;

            if (candidateQualified && candidate.CSSLengthMm != best.CSSLengthMm)
                return candidate.CSSLengthMm < best.CSSLengthMm;

            if (candidate.CSSEntryDate != best.CSSEntryDate)
                return candidate.CSSEntryDate < best.CSSEntryDate;

            return candidate.CSSId < best.CSSId;
        }

        /// <summary>
        /// Calcule les longueurs des segments sains d'une barre à défauts, de l'extrémité de
        /// tête vers le segment terminal.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par <see cref="OptimizeWithDefects"/> après contrôle des
        /// bornes. Les segments sont [0 ; début zone 1], puis, si une seconde zone existe,
        /// [fin zone 1 ; début zone 2] et [fin zone 2 ; longueur], sinon
        /// [fin zone 1 ; longueur]. La longueur d'un segment peut être nulle.
        /// </para>
        /// </remarks>
        /// <param name="barLength">Longueur physique de la barre, en millimètres.</param>
        /// <param name="defectStart1">Début de la première zone défectueuse, en millimètres.</param>
        /// <param name="defectEnd1">Fin de la première zone défectueuse, en millimètres.</param>
        /// <param name="defectStart2">Début de la seconde zone défectueuse, ou <see langword="null"/>.</param>
        /// <param name="defectEnd2">Fin de la seconde zone défectueuse, ou <see langword="null"/>.</param>
        /// <returns>
        /// Les longueurs des segments en millimètres, deux ou trois éléments ; le dernier
        /// élément est le segment terminal.
        /// </returns>
        private static int[] BuildSegmentLengths(
            int barLength,
            int defectStart1,
            int defectEnd1,
            int? defectStart2,
            int? defectEnd2)
        {
            if (defectStart2.HasValue && defectEnd2.HasValue)
            {
                return
                [
                    defectStart1,
                    defectStart2.Value - defectEnd1,
                    barLength - defectEnd2.Value
                ];
            }

            return
            [
                defectStart1,
                barLength - defectEnd1
            ];
        }

        /// <summary>
        /// Indique si une borne de zone défectueuse est comprise dans la longueur de la
        /// barre, bornes incluses.
        /// </summary>
        /// <param name="bound">Borne à contrôler, en millimètres depuis l'extrémité de tête.</param>
        /// <param name="barLength">Longueur physique de la barre, en millimètres.</param>
        /// <returns><see langword="true"/> si la borne est comprise entre 0 et <paramref name="barLength"/> ; <see langword="false"/> sinon.</returns>
        private static bool IsBoundWithinBar(int bound, int barLength)
        {
            return bound >= 0 && bound <= barLength;
        }

        /// <summary>
        /// Ordonne les découpes retenues dans l'ordre de coupe et en restitue les
        /// identifiants.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : l'ordre de coupe place les découpes par longueur arrondie
        /// décroissante, puis par identifiant croissant ; le rang d'un identifiant, compté
        /// à partir de 1, vaut position de coupe dans la barre.
        /// </para>
        /// </remarks>
        /// <param name="content">Index, dans le vivier, des découpes retenues.</param>
        /// <param name="pieceLengths">Longueurs arrondies des découpes, dans l'ordre du vivier.</param>
        /// <param name="pool">Vivier des découpes.</param>
        /// <returns>Les identifiants des découpes retenues dans l'ordre de coupe, jamais <see langword="null"/>.</returns>
        private static List<int> OrderPieceIds(
            List<int> content,
            int[] pieceLengths,
            IReadOnlyList<DTO_VwProductionCutPieceFull_P20> pool)
        {
            return content
                .OrderByDescending(index => pieceLengths[index])
                .ThenBy(index => pool[index].PCPId)
                .Select(index => pool[index].PCPId)
                .ToList();
        }

        /// <summary>
        /// Construit un résultat sans contenant ni contenu portant l'issue fournie.
        /// </summary>
        /// <param name="outcome">Issue autre qu'un succès.</param>
        /// <returns>Un résultat dont seule l'issue est significative, les autres propriétés portant leurs valeurs par défaut.</returns>
        private static DTO_CuttingOptimizationResult BuildOutcome(En_CuttingOptimizationOutcome outcome)
        {
            return new DTO_CuttingOptimizationResult { Outcome = outcome };
        }

        /// <summary>
        /// Construit un résultat de succès à partir du contenant retenu, du contenu ordonné
        /// et de la qualification du résidu.
        /// </summary>
        /// <param name="isNewBar">Origine du contenant : <see langword="true"/> pour une barre neuve.</param>
        /// <param name="idSourceScrap">Identifiant de la chute source, <see langword="null"/> pour une barre neuve.</param>
        /// <param name="barLength">Longueur du contenant, en millimètres.</param>
        /// <param name="pieceIds">Identifiants des découpes dans l'ordre de coupe, au moins un élément.</param>
        /// <param name="residueLength">Longueur physique du résidu, en millimètres.</param>
        /// <param name="residueIsScrap">Qualification du résidu : <see langword="true"/> pour une chute réutilisable.</param>
        /// <returns>Un résultat portant l'issue <see cref="En_CuttingOptimizationOutcome.Success"/>.</returns>
        private static DTO_CuttingOptimizationResult BuildSuccess(
            bool isNewBar,
            int? idSourceScrap,
            int barLength,
            List<int> pieceIds,
            int residueLength,
            bool residueIsScrap)
        {
            return new DTO_CuttingOptimizationResult
            {
                Outcome = En_CuttingOptimizationOutcome.Success,
                IsNewBar = isNewBar,
                IdSourceScrap = idSourceScrap,
                BarLength = barLength,
                PieceIds = pieceIds,
                ResidueLength = residueLength,
                ResidueIsScrap = residueIsScrap
            };
        }

        #endregion
    }
}