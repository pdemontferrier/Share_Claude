using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du moteur d'optimisation de découpe, calcul pur déterminant la matière à
    /// mobiliser et les découpes à y placer pour une référence de profilé.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par injection de dépendances par les UseCases de préparation et de
    /// validation des barres de production, qui l'invoquent à l'intérieur de leur
    /// transaction, entre leurs lectures et leurs écritures. L'implémentation
    /// <c>SR_CuttingOptimizer</c> réside en <c>B_UseCases/Services/Business</c>.
    /// L'approvisionnement de l'atelier étant réalisé à la demande, barre par barre, le
    /// moteur ne détermine jamais plus d'un contenant par invocation.
    /// </para>
    /// <para>
    /// Objectif : minimiser la matière non réutilisable (déchet) et faire tourner en
    /// priorité le stock de chutes, une barre neuve n'étant mobilisée que si aucune chute
    /// ne peut recevoir la moindre découpe ; recomposer en outre le plan de coupe d'une
    /// barre déjà validée dont une ou deux zones sont défectueuses.
    /// </para>
    /// <para>
    /// Modèle de consommation : les calculs sont conduits en millimètres entiers ; ce qui
    /// consomme de la matière (longueur de découpe, coupe de propreté, trait de scie) est
    /// arrondi à l'entier supérieur, ce qui est disponible (longueur de barre neuve) à
    /// l'entier inférieur. Une coupe de propreté est retranchée une seule fois, en tête
    /// de tout contenant. Le résidu est une chute réutilisable si et seulement si
    /// l'article gère les chutes et si le résidu diminué d'une coupe de propreté dépasse
    /// strictement la longueur minimale de chute ; il est déchet dans tous les autres cas.
    /// La perte croît de zéro jusqu'à ce seuil puis retombe à zéro : remplir au maximum
    /// n'est donc pas l'objectif, et une découpe écartée pour qualifier le résidu en
    /// chute n'est pas perdue, puisqu'elle demeure dans le vivier des optimisations
    /// suivantes.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le choix du meilleur contenant parmi les chutes disponibles, à défaut une barre neuve, et son garnissage.</description></item>
    /// <item><description>Déclarer la recomposition du plan de coupe d'une barre imposée présentant une ou deux zones défectueuses.</description></item>
    /// <item><description>Exprimer les issues métier par le discriminant <c>En_CuttingOptimizationOutcome</c> du résultat, jamais par exception.</description></item>
    /// <item><description>Garantir le déterminisme et l'absence d'effet de bord : des entrées identiques produisent un résultat identique, et les entrées ne sont jamais modifiées.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne lit ni n'écrit aucune donnée, ne notifie pas l'opérateur et ne conserve aucun état entre deux invocations.</description></item>
    /// <item><description>Ne filtre pas le stock de chutes ni ne trie le vivier : ces garanties incombent à l'appelant.</description></item>
    /// <item><description>Ne crée ni ne met à jour la barre de production : l'exploitation du résultat relève du UseCase appelant.</description></item>
    /// <item><description>Ne vérifie aucune cohérence de données au-delà des paramètres de coupe et des longueurs indispensables au calcul.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DTO_CuttingOptimizationResult"/>
    public interface IS_CuttingOptimizer
    {
        // --- Groupe 1 : Optimisation d'une barre ---

        /// <summary>
        /// Détermine le meilleur contenant pour le vivier de découpes reçu, en privilégiant
        /// les chutes du stock, et le garnit des découpes qui en optimisent le rendement.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase de préparation d'une barre lorsqu'aucune barre
        /// n'est en cours pour le couple série de production / article interne traité.
        /// </para>
        /// <para>
        /// Objectif : chaque chute est garnie en recherchant la consommation la plus forte
        /// qui laisse un résidu qualifié de chute, à défaut la consommation la plus forte.
        /// Parmi les chutes recevant au moins une découpe, la retenue est, dans l'ordre :
        /// celle dont le résidu est une chute plutôt qu'un déchet ; parmi les résidus
        /// déchet, celle dont le résidu est le plus court ; parmi les résidus chute, la
        /// chute la plus courte ; puis la plus ancienne en stock ; puis celle de plus petit
        /// identifiant. Une barre neuve n'est évaluée que si aucune chute ne reçoit de
        /// découpe.
        /// </para>
        /// <para>
        /// Les paramètres de coupe (coupe de propreté, trait de scie), la longueur
        /// minimale de chute, la gestion des chutes et la longueur de barre neuve sont lus
        /// sur la première découpe du vivier, ces valeurs étant invariantes pour un article
        /// interne. Le résultat porte l'issue
        /// <c>DataAnomaly</c> lorsqu'une valeur indispensable est inexploitable : coupe de
        /// propreté ou trait de scie absent ou négatif (la valeur zéro étant admise) ;
        /// longueur d'une découpe absente ou non strictement positive après arrondi ;
        /// longueur minimale de chute absente ou négative, contrôlée uniquement si
        /// l'article gère les chutes ; longueur de barre neuve absente ou non strictement
        /// positive après arrondi, contrôlée uniquement lorsque la barre neuve est évaluée.
        /// La même issue est retournée si aucune découpe ne tient sur une barre neuve.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Retourner l'issue <c>EmptyPool</c> sur un vivier vide, situation nominale.</description></item>
        /// <item><description>Évaluer toutes les chutes reçues, puis, à défaut de chute garnie, la barre neuve de l'article.</description></item>
        /// <item><description>Qualifier le résidu du contenant retenu en chute réutilisable ou en déchet.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne vérifie pas que le vivier est trié par identifiant croissant ni qu'il relève d'un article interne unique : l'appelant le garantit, et le déterminisme en dépend.</description></item>
        /// <item><description>Ne vérifie pas que les chutes reçues sont disponibles et compatibles avec l'article traité : l'appelant les sélectionne.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="pool">
        /// Vivier des découpes restant à réaliser pour un couple série de production /
        /// article interne, trié par identifiant de découpe croissant. Ne doit pas être
        /// <see langword="null"/> ; une liste vide est admise et produit l'issue
        /// <c>EmptyPool</c>. N'est jamais modifié.
        /// </param>
        /// <param name="scrapStock">
        /// Chutes disponibles de l'article interne traité, candidates à la découpe, dans
        /// un ordre quelconque. Ne doit pas être <see langword="null"/> ; une liste vide
        /// est admise et conduit directement à l'évaluation d'une barre neuve. N'est jamais
        /// modifié.
        /// </param>
        /// <returns>
        /// Le résultat de l'optimisation, jamais <see langword="null"/>. Sur l'issue
        /// <c>Success</c> : origine du contenant (chute ou barre neuve), identifiant de la
        /// chute source (<see langword="null"/> pour une barre neuve), longueur du contenant
        /// en millimètres, identifiants des découpes retenues, longueur physique du résidu
        /// et sa qualification (<see langword="true"/> = chute réutilisable,
        /// <see langword="false"/> = déchet). Les identifiants de découpes sont ordonnés par
        /// longueur arrondie décroissante puis par identifiant croissant ; le rang de chaque
        /// identifiant, compté à partir de 1, vaut position de coupe dans la barre, et la
        /// liste ne doit pas être réordonnée par le consommateur. Sur les issues
        /// <c>EmptyPool</c> et <c>DataAnomaly</c>, le contenant et le contenu sont sans objet.
        /// </returns>
        /// <exception cref="Ex_Business">Levée, au code <c>BU_ER_01</c>, lorsque <paramref name="pool"/> ou <paramref name="scrapStock"/> est <see langword="null"/>.</exception>
        /// <exception cref="Ex_Unclassified">Levée lorsqu'une défaillance imprévue du calcul est requalifiée par le classifieur d'exceptions.</exception>
        DTO_CuttingOptimizationResult Optimize(
            string caller,
            IReadOnlyList<DTO_VwProductionCutPieceFull_P20> pool,
            IReadOnlyList<vw_CuttingScrapStock_Full> scrapStock);

        // --- Groupe 2 : Recomposition d'une barre à défauts ---

        /// <summary>
        /// Recompose le plan de coupe d'une barre déjà validée dont une ou deux zones sont
        /// défectueuses, en garnissant un unique segment sain de la barre imposée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase de validation d'une barre lorsque l'opérateur
        /// déclare des défauts. Le contenant est imposé : aucun choix entre chutes et barre
        /// neuve n'est opéré.
        /// </para>
        /// <para>
        /// Objectif : les zones défectueuses découpent la barre en segments sains, repérés
        /// depuis l'extrémité de tête physique de la barre ; le dernier est le segment
        /// terminal, les autres sont intermédiaires. Chaque segment est un contenant
        /// distinct, amputé de sa propre coupe de propreté. Les segments sont parcourus de
        /// la tête vers le terminal et seul le premier segment recevant au moins une
        /// découpe est garni : un segment intermédiaire l'est au maximum de sa capacité, son
        /// reste étant de toute façon perdu ; le segment terminal l'est selon la même règle
        /// que le moteur principal. Le résidu conservé est toujours le segment terminal :
        /// son reste après garnissage s'il est le segment retenu, sa longueur physique
        /// entière sinon. Les restes des segments intermédiaires sont du déchet non
        /// restitué.
        /// </para>
        /// <para>
        /// Les paramètres de coupe, la longueur minimale de chute et la gestion des chutes
        /// sont lus et contrôlés comme pour <see cref="Optimize"/>, à l'exception de la
        /// longueur de barre neuve, étrangère à ce calcul ; une valeur inexploitable produit
        /// l'issue <c>DataAnomaly</c>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Contrôler la cohérence structurelle de la barre et des zones défectueuses déclarées.</description></item>
        /// <item><description>Retourner l'issue <c>NoUsableSegment</c> lorsqu'aucun segment ne reçoit de découpe.</description></item>
        /// <item><description>Recopier dans le résultat l'origine, la chute source et la longueur de la barre reçue.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne répartit pas les découpes sur plusieurs segments : un seul segment est garni par invocation.</description></item>
        /// <item><description>Ne décide pas du traitement d'un refus partiel : l'interprétation de l'issue relève du UseCase appelant.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="pool">
        /// Vivier des découpes restant à réaliser pour le couple série de production /
        /// article interne de la barre, trié par identifiant de découpe croissant. Ne doit
        /// pas être <see langword="null"/> ; une liste vide produit l'issue
        /// <c>EmptyPool</c>. N'est jamais modifié.
        /// </param>
        /// <param name="barLength">Longueur physique de la barre imposée, en millimètres. Doit être strictement positive.</param>
        /// <param name="isNewBar">Origine de la barre imposée : <see langword="true"/> pour une barre neuve, <see langword="false"/> pour une chute du stock.</param>
        /// <param name="idSourceScrap">Identifiant de la chute source ; <see langword="null"/> si et seulement si <paramref name="isNewBar"/> vaut <see langword="true"/>.</param>
        /// <param name="defectStart1">Début de la première zone défectueuse, en millimètres depuis l'extrémité de tête physique de la barre, compris entre 0 et <paramref name="barLength"/>.</param>
        /// <param name="defectEnd1">Fin de la première zone défectueuse, en millimètres depuis l'extrémité de tête, comprise entre 0 et <paramref name="barLength"/> et strictement supérieure à <paramref name="defectStart1"/>.</param>
        /// <param name="defectStart2">Début de la seconde zone défectueuse, ou <see langword="null"/> en l'absence de seconde zone ; lorsqu'il est renseigné, compris entre 0 et <paramref name="barLength"/> et supérieur ou égal à <paramref name="defectEnd1"/>.</param>
        /// <param name="defectEnd2">Fin de la seconde zone défectueuse, renseignée si et seulement si <paramref name="defectStart2"/> l'est ; comprise entre 0 et <paramref name="barLength"/> et strictement supérieure à <paramref name="defectStart2"/>.</param>
        /// <returns>
        /// Le résultat de la recomposition, jamais <see langword="null"/>. Sur l'issue
        /// <c>Success</c> : origine, chute source et longueur recopiées des entrées,
        /// identifiants des découpes retenues, longueur physique du segment terminal
        /// conservé et sa qualification (<see langword="true"/> = chute réutilisable,
        /// <see langword="false"/> = déchet). Les identifiants de découpes sont ordonnés par
        /// longueur arrondie décroissante puis par identifiant croissant ; le rang de chaque
        /// identifiant, compté à partir de 1, vaut position de coupe dans la barre, et la
        /// liste ne doit pas être réordonnée par le consommateur. Sur les issues
        /// <c>EmptyPool</c>, <c>DataAnomaly</c> et <c>NoUsableSegment</c>, le contenant et
        /// le contenu sont sans objet.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée au code <c>BU_ER_01</c> lorsque <paramref name="pool"/> est
        /// <see langword="null"/> ; au code <c>BU_ER_02</c> lorsque
        /// <paramref name="barLength"/> n'est pas strictement positive ou qu'une borne de
        /// zone renseignée est négative ou supérieure à <paramref name="barLength"/> ; au code
        /// <c>BU_ER_03</c> lorsque les bornes d'une zone sont désordonnées, qu'une seule
        /// borne de la seconde zone est renseignée, que la seconde zone chevauche ou précède
        /// la première, ou que <paramref name="idSourceScrap"/> est incohérent avec
        /// <paramref name="isNewBar"/>.
        /// </exception>
        /// <exception cref="Ex_Unclassified">Levée lorsqu'une défaillance imprévue du calcul est requalifiée par le classifieur d'exceptions.</exception>
        DTO_CuttingOptimizationResult OptimizeWithDefects(
            string caller,
            IReadOnlyList<DTO_VwProductionCutPieceFull_P20> pool,
            int barLength,
            bool isNewBar,
            int? idSourceScrap,
            int defectStart1,
            int defectEnd1,
            int? defectStart2,
            int? defectEnd2);
    }
}