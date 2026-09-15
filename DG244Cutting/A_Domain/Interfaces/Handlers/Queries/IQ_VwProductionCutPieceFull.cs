using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// Contrat spécialisé du Query Handler dédié à la vue de base de données
    /// <see cref="vw_ProductionCutPiece_Full"/>, qui expose le détail des découpes composant les
    /// séries de production.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et étend le socle de lecture
    /// <see cref="IQ_Generic{T}"/> paramétré pour <see cref="vw_ProductionCutPiece_Full"/>. Son
    /// implémentation concrète <c>QH_VwProductionCutPieceFull</c> réside dans
    /// B_UseCases/Handlers/Queries/ et dérive de
    /// <c>QH_Generic&lt;vw_ProductionCutPiece_Full&gt;</c>. Le contrat est consommé via injection,
    /// jamais par référence à son implémentation.
    /// </para>
    /// <para>
    /// Objectif : servir trois questions fermées portant sur les découpes d'une série de
    /// production. La première relève de la consultation : « quelles découpes composent telle
    /// série de production, avec les seules caractéristiques que l'écran de consultation
    /// affiche ». Les deux autres relèvent de l'optimisation des barres conduite en Page20 :
    /// « que reste-t-il à faire dans la série », soit la prochaine découpe à traiter, tous
    /// articles confondus, et « que peut-on placer sur une barre de tel article », soit le
    /// vivier des découpes disponibles d'un article interne de la série. La découpe est l'unité
    /// élémentaire du travail d'atelier et l'objet même de l'application : chaque châssis d'une
    /// commande se décompose en ouvrants, eux-mêmes en pièces de profilé à découper. Chaque
    /// découpe porte l'identification du profilé dont elle est issue, sa géométrie de coupe -
    /// une longueur encadrée à gauche et à droite d'une inclinaison et d'un pivot -, les
    /// dimensions du profilé, et sa position sur la barre qui lui a été affectée. Quatre
    /// indicateurs d'état jalonnent son parcours : la barre nécessaire est-elle approvisionnée,
    /// est-elle en rupture de stock, la découpe a-t-elle été réalisée, a-t-elle été refusée. La
    /// vue source expose deux cent trente-trois colonnes ; chaque lecture n'en rapatrie que ce
    /// que sa destination exige.
    /// </para>
    /// <para>
    /// La lecture de consultation est destinée au cinquième onglet de la Page11, qui affiche
    /// seize colonnes. Elle rapatrie vingt champs - les seize champs d'affichage, plus quatre
    /// champs de service non affichés servant à l'identification des lignes, à la vérification
    /// de cohérence du lot reçu et à l'ordonnancement laissé à la charge de l'appelant. C'est la
    /// source la plus volumineuse du périmètre de la Page11, en colonnes comme en lignes
    /// attendues, chaque châssis d'une série produisant plusieurs découpes. Les deux lectures
    /// d'optimisation rapatrient treize champs : les données de découpe, les paramètres de coupe
    /// qui gouvernent la consommation de matière, les caractéristiques d'article qui déterminent
    /// la barre de repli et la qualification du reliquat, et les champs d'identification qui
    /// ordonnent la recherche de la prochaine référence et présentent le profilé à l'opérateur.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture Page11 : la lecture n'applique aucun filtrage au-delà de la
    /// série. Chez les découpes, à la différence des barres, le refus n'est pas porté par
    /// l'indicateur de suppression logique <c>PCPIsDeleted</c> mais par une colonne distincte et
    /// projetée, <c>PCPIsCutRefused</c> : le refus relève de l'affichage et non de l'exclusion.
    /// Une découpe non encore affectée à une barre - dite au vivier - porte une position et un
    /// identifiant de barre absents ; cet état est nominal et ne traduit aucune anomalie. Une
    /// série dont l'optimisation n'a pas encore été lancée, ou qui ne comporte aucune découpe,
    /// produit une liste vide, résultat nominal et non erreur.
    /// </para>
    /// <para>
    /// Portée du résultat - lectures Page20 : ces lectures ne retiennent que les découpes
    /// disponibles pour l'optimisation, c'est-à-dire ni coupées, ni engagées dans une
    /// optimisation provisoire ou définitive, ni supprimées, ni bloquées par une rupture de stock
    /// de leur barre. Le refus n'est pas, pour ces lectures, un critère d'exclusion : une découpe
    /// refusée et non coupée reste à réaliser et demeure dans le vivier. Leur résultat est
    /// ordonné, l'ordre étant constitutif de leur contrat. L'absence de découpe disponible y est
    /// un résultat nominal : une valeur absente pour la recherche de la prochaine découpe, une
    /// liste vide pour le vivier.
    /// </para>
    /// <para>
    /// Positionnement CQRS : le contrat est strictement côté lecture. Il ne déclare aucune
    /// signature d'écriture, aucune mutation, aucun point de validation transactionnelle. La
    /// question est au demeurant sans objet, la vue étant une source de lecture seule.
    /// </para>
    /// <para>
    /// Sous-cas de lecture spécialisée : les trois lectures déclarées relèvent du second sous-cas
    /// du critère de lecture spécialisée de §4.14.5 du 0230. Elles mobilisent une API EF Core
    /// absente du contrat <c>IR_Generic&lt;T&gt;</c> - la projection SQL traduite côté base de
    /// données, soit un <c>Select</c> retournant un type <c>DTO_</c> par expression
    /// LINQ-to-Entities - et elles sont donc servies par délégation au repository spécialisé
    /// <c>IR_VwProductionCutPieceFull</c> (Patron 2 de §4.15.2), et non par les treize lectures
    /// du socle hérité. Aucune de ces treize lectures ne rend un type <c>DTO_</c> : toutes rendent
    /// l'entité paramétrée, une liste de cette entité, un booléen ou un entier. La recherche de la
    /// prochaine découpe requiert en outre un tri sur deux colonnes, que le socle n'expose pas.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Hériter du socle <see cref="IQ_Generic{T}"/> paramétré pour
    ///     <see cref="vw_ProductionCutPiece_Full"/> afin d'exposer aux consommateurs les treize
    ///     lectures du socle en même temps que les trois lectures projetées propres à la vue.
    ///   </description></item>
    ///   <item><description>
    ///     Déclarer les trois lectures projetées - la lecture de consultation par identifiant de
    ///     série, la recherche de la prochaine découpe à réaliser et la lecture du vivier
    ///     d'optimisation -, chacune avec sa signature de traçabilité, sa frontière de retour
    ///     <c>DTO_</c> de A_Domain et les retours signalables qu'elle laisse remonter.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne redéclare aucune des treize lectures du socle <see cref="IQ_Generic{T}"/> : elles
    ///     sont héritées telles quelles et ne sont jamais masquées côté implémentation.
    ///   </description></item>
    ///   <item><description>
    ///     Ne porte aucune règle métier, aucun calcul et aucune mise en forme. Pour la lecture
    ///     Page11, aucun ordonnancement n'est appliqué : le tri du tableau relève de l'appelant,
    ///     qui l'applique en mémoire après extraction. Les lectures Page20 sont en revanche
    ///     ordonnées en aval, côté base de données, et rendues dans cet ordre, qui est
    ///     constitutif de leur contrat.
    ///   </description></item>
    ///   <item><description>
    ///     Pour la lecture Page11, n'écarte aucun enregistrement de la série, et n'applique en
    ///     particulier aucun filtrage sur l'indicateur de suppression logique
    ///     <c>PCPIsDeleted</c>. À la différence du contrat des barres, où le refus est
    ///     précisément porté par cet indicateur, le refus d'une découpe est porté par la colonne
    ///     distincte <c>PCPIsCutRefused</c>, projetée au même titre que les autres champs
    ///     d'affichage : la formulation retenue pour les barres n'est donc pas transposable ici.
    ///     Pour les lectures Page20, l'exclusion se limite à la série, à la disponibilité pour
    ///     l'optimisation - dont l'absence de suppression est l'un des critères - et, pour le
    ///     vivier, à l'article interne ; le refus n'y est pas davantage un critère d'exclusion.
    ///   </description></item>
    ///   <item><description>
    ///     N'expose aucune dépendance à EF Core ni à C_Infrastructure : la pureté contractuelle
    ///     de A_Domain est intégrale.
    ///   </description></item>
    /// </list>
    /// <para>
    /// AVERTISSEMENT DE SURFACE HÉRITÉE. La vue <see cref="vw_ProductionCutPiece_Full"/> est
    /// déclarée sans clé dans le contexte EF Core (<c>HasNoKey</c>, <c>ToView</c>). L'héritage du
    /// socle de lecture expose donc au consommateur treize méthodes dont trois sont visibles mais
    /// échouent à l'exécution sur ce type. Elles ne doivent jamais être appelées :
    /// </para>
    /// <list type="bullet">
    ///   <item><description>
    ///     <c>HandleGetByIdAsync</c> - délègue à <c>GetByIdAsync</c>, qui repose sur
    ///     <c>FindAsync</c>, non supporté sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>HandleGetByIdAsNoTrackingAsync</c> - délègue à <c>GetByIdAsNoTrackingAsync</c>, qui
    ///     résout l'identifiant par <c>EF.Property&lt;int&gt;(e, "Id")</c>, propriété inexistante
    ///     sur la vue.
    ///   </description></item>
    ///   <item><description>
    ///     <c>HandleGetAnyAsync(caller, int id)</c> - délègue à <c>GetAnyAsync</c>, qui repose sur
    ///     <c>FindAsync</c>, non supporté sans clé.
    ///   </description></item>
    /// </list>
    /// <para>
    /// Les dix lectures restantes sont opérantes sur ce type. Parmi elles, les lectures
    /// nominalement trackées - <c>HandleGetFirstOrDefaultAsync</c>, <c>HandleGetAllAsync</c> et
    /// <c>HandleGetFilteredAsync</c> - fonctionnent, mais rendent des instances non suivies :
    /// EF Core ne trace jamais un type sans clé.
    /// </para>
    /// <para>
    /// Cette conséquence est assumée. L'extension du socle est la forme canonique de la famille ;
    /// la contrainte du socle n'exige qu'un type référence, ce que la vue satisfait ; l'héritage
    /// est valide à la compilation. La restriction porte sur l'usage et non sur la structure, et
    /// elle est portée par le présent avertissement.
    /// </para>
    /// </remarks>
    public interface IQ_VwProductionCutPieceFull : IQ_Generic<vw_ProductionCutPiece_Full>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Rend la liste des découpes rattachées à une série de production, réduites aux vingt
        /// champs utiles au cinquième onglet de la Page11.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// réduction de deux cent trente-trois à vingt colonnes est appliquée sur la requête et
        /// traduite en clause <c>SELECT</c> côté serveur de base de données par le repository
        /// spécialisé délégué ; elle n'est jamais réalisée en mémoire.
        /// </para>
        /// <para>
        /// Objectif : offrir au consommateur un lot de lignes brut, qu'il lui appartient de trier
        /// et de mettre en forme. Aucun ordonnancement n'est appliqué : les quatre critères de tri
        /// du tableau (<c>ACMDescription</c>, <c>ARSortOrder</c>, <c>PCPIdProductionBar</c>,
        /// <c>PCPCutPositionInBar</c>) figurent tous parmi les champs projetés - les premier et
        /// quatrième au titre des champs d'affichage, les deuxième et troisième au titre des
        /// champs de service - et sont mis à la disposition de l'appelant.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur l'indicateur de suppression logique
        /// <c>PCPIsDeleted</c>, et aucun recours n'est fait à <c>IgnoreQueryFilters</c>. Chez les
        /// découpes, à la différence des barres, le refus n'est pas porté par cet indicateur mais
        /// par la colonne distincte <c>PCPIsCutRefused</c>, qui figure parmi les champs projetés :
        /// le refus relève de l'affichage et non de l'exclusion.
        /// </para>
        /// <para>
        /// Une découpe non encore affectée à une barre - dite au vivier - porte
        /// <c>PCPCutPositionInBar</c> et <c>PCPIdProductionBar</c> à l'état absent : état nominal,
        /// sans incidence sur la lecture.
        /// </para>
        /// <para>
        /// Le lot est rendu sans transformation aucune - ni tri, ni filtrage, ni recopie, ni
        /// projection complémentaire : la référence produite en aval est retournée telle quelle,
        /// liste vide comprise.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif. Il s'agit d'un identifiant fonctionnel
        /// étranger, hérité de la table d'origine de la série, et non de la clé de la vue : la vue
        /// n'en a pas.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des découpes projetées de la série demandée, dans l'ordre où la source les rend,
        /// soit un ordre indéterminé. Liste vide si la série ne comporte aucune découpe - cas
        /// d'une série dont l'optimisation n'a pas encore été lancée : ce résultat est nominal et
        /// ne constitue pas une erreur. Ne retourne jamais <see langword="null"/>.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="productionSeriesId"/> est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>). Un identifiant nul ou négatif traduit un contexte de sélection
        /// non renseigné, anomalie fonctionnelle qui doit remonter plutôt que produire une lecture
        /// vide.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<DTO_VwProductionCutPieceFull_P11>> HandleGetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        /// <summary>
        /// Désigne la prochaine découpe à réaliser dans une série de production, tous articles
        /// confondus, réduite aux treize champs utiles au moteur d'optimisation de la Page20.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection, l'ordonnancement, la réduction au premier enregistrement et la réduction de
        /// deux cent trente-trois à treize colonnes sont appliqués sur la requête et traduits en
        /// SQL côté serveur de base de données par le repository spécialisé délégué ; ils ne sont
        /// jamais réalisés en mémoire.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « que reste-t-il à faire dans la série ». La découpe
        /// retournée est la première des découpes disponibles de la série, dans l'ordre du couple
        /// référence et couleur <c>PCPReferenceColor</c> croissant, puis de la clé technique
        /// <c>PCPId</c> croissante. Son article interne détermine la matière sur laquelle porte
        /// l'optimisation et constitue la clé de compatibilité avec le stock de chutes.
        /// </para>
        /// <para>
        /// Une découpe est disponible lorsqu'elle n'est pas coupée, n'est engagée dans aucune
        /// optimisation, ni provisoire ni définitive, n'est pas supprimée et n'est pas bloquée par
        /// une rupture de stock de sa barre. Le refus d'une découpe n'est pas un critère
        /// d'exclusion : une découpe refusée et non coupée reste à réaliser.
        /// </para>
        /// <para>
        /// L'ordre est constitutif du contrat et non décoratif : il fixe l'ordre dans lequel
        /// l'atelier traite les références, et le départage par <c>PCPId</c> garantit qu'une même
        /// série présente toujours la même référence en premier. Il est établi en aval et n'est
        /// pas repris à ce niveau.
        /// </para>
        /// <para>
        /// L'article interne de la découpe retournée est renseigné en vertu d'un invariant de
        /// données garanti par l'import des séries ; il n'est pas contrôlé à ce niveau.
        /// </para>
        /// <para>
        /// La découpe est rendue sans transformation aucune : la référence produite en aval est
        /// retournée telle quelle, valeur absente comprise.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif. Il s'agit d'un identifiant fonctionnel
        /// étranger, hérité de la table d'origine de la série, et non de la clé de la vue : la vue
        /// n'en a pas.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// La prochaine découpe disponible de la série, projetée, ou <see langword="null"/> si la
        /// série ne compte plus aucune découpe optimisable. Ce retour absent est nominal et ne
        /// constitue pas une erreur : il signale au consommateur qu'aucune découpe ne reste à
        /// optimiser, situation qui appelle le contrôle de clôture de la série.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="productionSeriesId"/> est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>). Un identifiant nul ou négatif traduit un contexte de série non
        /// renseigné, anomalie qui doit remonter plutôt que produire une absence de résultat
        /// indiscernable de la fin de série.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<DTO_VwProductionCutPieceFull_P20?> HandleGetNextReferenceToCutForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        /// <summary>
        /// Rend l'ensemble des découpes disponibles pour un article interne d'une série de
        /// production, réduites aux treize champs utiles au moteur d'optimisation de la Page20.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection, l'ordonnancement et la réduction de deux cent trente-trois à treize colonnes
        /// sont appliqués sur la requête et traduits en SQL côté serveur de base de données par le
        /// repository spécialisé délégué ; ils ne sont jamais réalisés en mémoire.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « que peut-on placer sur une barre de cet article ».
        /// Le résultat constitue le vivier d'optimisation : la matière sur laquelle le moteur
        /// calcule le remplissage d'une barre - chute du stock ou barre neuve - de l'article
        /// interne demandé. Toutes les découpes disponibles de la série pour cet article sont
        /// rendues, sans limite de nombre.
        /// </para>
        /// <para>
        /// Une découpe est disponible lorsqu'elle n'est pas coupée, n'est engagée dans aucune
        /// optimisation, ni provisoire ni définitive, n'est pas supprimée et n'est pas bloquée par
        /// une rupture de stock de sa barre. Le refus d'une découpe n'est pas un critère
        /// d'exclusion : une découpe refusée et non coupée reste à réaliser.
        /// </para>
        /// <para>
        /// Le résultat est ordonné par clé technique <c>PCPId</c> croissante. Cet ordre fonde le
        /// déterminisme du moteur d'optimisation : une même situation d'atelier conduit toujours à
        /// la même barre proposée. Il est établi en aval et conservé intact à ce niveau.
        /// </para>
        /// <para>
        /// Le lot est rendu sans transformation aucune - ni tri, ni filtrage, ni recopie, ni
        /// projection complémentaire : la référence produite en aval est retournée telle quelle,
        /// liste vide comprise.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif. Il s'agit d'un identifiant fonctionnel
        /// étranger, hérité de la table d'origine de la série, et non de la clé de la vue : la vue
        /// n'en a pas.
        /// </param>
        /// <param name="idArticleInternal">
        /// Identifiant de l'article interne dont le vivier est demandé, correspondant à la colonne
        /// <c>PCPIdArticleInternal</c> de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des découpes disponibles de l'article interne dans la série, projetées et
        /// ordonnées par <c>PCPId</c> croissant. Ne retourne jamais <see langword="null"/>. Une
        /// liste vide est un résultat nominal et ne constitue pas une erreur.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée, dans cet ordre de contrôle, si <paramref name="productionSeriesId"/> est
        /// inférieur ou égal à zéro, puis si <paramref name="idArticleInternal"/> est inférieur ou
        /// égal à zéro (code <c>BU_ER_02</c> dans les deux cas). Lorsque les deux identifiants
        /// sont invalides, l'erreur porte sur la série.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<DTO_VwProductionCutPieceFull_P20>> HandleGetOptimizationPoolForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            int idArticleInternal,
            CancellationToken ct = default);

        #endregion
    }
}