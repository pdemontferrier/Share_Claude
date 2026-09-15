using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY
{
    /// <summary>
    /// Contrat spécialisé du repository dédié à la vue de base de données
    /// <see cref="vw_ProductionCutPiece_Full"/>, qui expose le détail des découpes composant
    /// les séries de production, à destination de l'écran de consultation d'une série et du
    /// moteur d'optimisation des barres.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et étend le contrat générique
    /// <see cref="IR_Generic{T}"/> paramétré pour <see cref="vw_ProductionCutPiece_Full"/>. Son
    /// implémentation concrète <c>CR_VwProductionCutPieceFull</c> réside dans
    /// C_Infrastructure/Repositories/DIGIT_TRY/ et dérive de
    /// <c>CR_Generic&lt;vw_ProductionCutPiece_Full&gt;</c>.
    /// </para>
    /// <para>
    /// Objectif : servir trois questions fermées portant sur les découpes d'une série de
    /// production. La première relève de la consultation : « quelles découpes composent telle
    /// série, avec les seules caractéristiques que l'écran affiche ». Les deux autres relèvent de
    /// l'optimisation des barres : « quelle est la prochaine découpe à traiter dans la série » et
    /// « quelles découpes restent à placer pour tel article interne de la série ». La découpe est
    /// l'unité élémentaire du travail d'atelier et l'objet même de l'application : chaque châssis
    /// d'une commande se décompose en ouvrants, eux-mêmes en pièces de profilé à découper. Chaque
    /// découpe porte l'identification du profilé dont elle est issue, sa géométrie de coupe - une
    /// longueur encadrée à gauche et à droite d'une inclinaison et d'un pivot -, les dimensions
    /// du profilé, et sa position sur la barre qui lui a été affectée. Quatre indicateurs d'état
    /// jalonnent son parcours : la barre nécessaire est-elle approvisionnée, est-elle en rupture
    /// de stock, la découpe a-t-elle été réalisée, a-t-elle été refusée. La vue source expose deux
    /// cent trente-trois colonnes ; chaque lecture n'en rapatrie que ce que sa destination exige.
    /// </para>
    /// <para>
    /// La lecture destinée au cinquième onglet de la Page11 rapatrie vingt champs - les seize
    /// champs d'affichage, plus quatre champs de service non affichés servant à l'identification
    /// des lignes, à la vérification de cohérence du lot reçu et à l'ordonnancement laissé à la
    /// charge de l'appelant. Les deux lectures destinées au moteur d'optimisation de la Page20
    /// rapatrient treize champs : les données de découpe, les paramètres de coupe qui gouvernent
    /// la consommation de matière, les caractéristiques d'article qui déterminent la barre de
    /// repli et la qualification du reliquat, et les champs d'identification qui ordonnent la
    /// recherche de la prochaine référence et présentent le profilé à l'opérateur.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture Page11 : cette lecture n'applique aucun filtrage au-delà de
    /// la série. Chez les découpes, à la différence des barres, le refus n'est pas porté par
    /// l'indicateur de suppression logique mais par une colonne distincte et projetée,
    /// <c>PCPIsCutRefused</c> : le refus relève de l'affichage et non de l'exclusion. Une découpe
    /// non encore affectée à une barre - dite au vivier - porte une position et un identifiant de
    /// barre absents ; cet état est nominal et ne traduit aucune anomalie. Une série dont
    /// l'optimisation n'a pas encore été lancée, ou qui ne comporte aucune découpe, produit une
    /// liste vide, résultat nominal et non erreur.
    /// </para>
    /// <para>
    /// Portée du résultat - lectures Page20 : ces lectures ne retiennent que les découpes
    /// disponibles pour l'optimisation, c'est-à-dire ni coupées, ni engagées dans une
    /// optimisation provisoire ou définitive, ni supprimées, ni bloquées par une rupture de stock
    /// de leur barre. Une découpe en rupture reste à réaliser, mais demeure hors du vivier tant
    /// que la matière n'est pas libérée. Le refus n'est pas non plus, pour ces lectures, un
    /// critère d'exclusion : une découpe refusée et non coupée reste à réaliser et demeure dans
    /// le vivier. Ces lectures ordonnent leur résultat côté base de données, le tri faisant partie
    /// de leur contrat. L'absence de découpe disponible y est un résultat nominal : une valeur
    /// absente pour la recherche de la prochaine découpe, une liste vide pour le vivier.
    /// </para>
    /// <para>
    /// Justification du Patron 2 : cette interface n'existe que parce que les trois lectures de
    /// son périmètre nécessitent une API EF Core non disponible dans <see cref="IR_Generic{T}"/> -
    /// en l'occurrence la projection SQL traduite côté base de données, soit un <c>Select</c>
    /// retournant un type <c>DTO_</c> par expression LINQ-to-Entities. Aucune des dix-huit
    /// méthodes du contrat générique ne rend un type <c>DTO_</c> : toutes rendent l'entité
    /// paramétrée, une liste de cette entité, un booléen ou un entier. Servir ces besoins par
    /// consommation directe du contrat générique imposerait de matérialiser les deux cent
    /// trente-trois colonnes puis d'en écarter en mémoire deux cent treize pour la lecture Page11
    /// et deux cent vingt pour les lectures Page20, ce qui relèverait du Cas 2 (transformation
    /// LINQ-to-Objects portée par un Query Handler) et ferait perdre la réduction côté base qui
    /// est la finalité même du composant. La recherche de la prochaine découpe requiert en outre
    /// un tri sur deux colonnes, que le socle n'expose pas. Conformément au sous-bloc « Critère de
    /// création d'un Repository spécialisé CR_[Entité] » de §4.14.6 du 0230, la projection SQL
    /// traduite côté base est nominativement rangée au Cas 3, seul cas justifiant la création d'un
    /// repository spécialisé.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Hériter du contrat <see cref="IR_Generic{T}"/> paramétré pour
    ///     <see cref="vw_ProductionCutPiece_Full"/> afin d'exposer aux consommateurs les dix-huit
    ///     méthodes du socle générique en même temps que la lecture projetée propre à la vue.
    ///   </description></item>
    ///   <item><description>
    ///     Déclarer les trois lectures projetées qui mobilisent la projection SQL absente du
    ///     contrat générique : la lecture de consultation de la Page11, réduite à vingt colonnes,
    ///     et les deux lectures du moteur d'optimisation de la Page20 - recherche de la prochaine
    ///     découpe à traiter et lecture du vivier d'un article interne -, réduites à treize
    ///     colonnes. La réduction est dans tous les cas traduite en clause <c>SELECT</c> côté
    ///     serveur de base de données.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne redéclare aucune des dix-huit méthodes du contrat <see cref="IR_Generic{T}"/> :
    ///     leur implémentation par défaut portée par
    ///     <c>CR_Generic&lt;vw_ProductionCutPiece_Full&gt;</c> est finale, conformément à R-4.15.3
    ///     et I-4.15.1 du 0231.
    ///   </description></item>
    ///   <item><description>
    ///     Ne porte aucune règle métier et aucun calcul. La lecture Page11 n'applique aucun
    ///     ordonnancement : le tri du tableau relève de l'appelant, qui l'applique en mémoire après
    ///     extraction. Les lectures Page20 ordonnent en revanche leur résultat côté base de
    ///     données, l'ordre étant constitutif de leur contrat.
    ///   </description></item>
    ///   <item><description>
    ///     N'expose aucune opération d'écriture. La vue étant une source de lecture seule, toute
    ///     signature de persistance serait sans objet ; le contrat n'en déclare aucune, et le
    ///     socle n'en expose aucune qui soit opérante sur ce type (cf. avertissement ci-dessous).
    ///   </description></item>
    ///   <item><description>
    ///     Pour la lecture Page11, n'écarte aucun enregistrement de la série et n'applique en
    ///     particulier aucun filtrage sur l'indicateur de suppression logique
    ///     <c>PCPIsDeleted</c>. À la différence du repository des barres, où le refus est
    ///     précisément porté par cet indicateur, le refus d'une découpe est porté par la colonne
    ///     distincte <c>PCPIsCutRefused</c>, projetée au même titre que les autres champs
    ///     d'affichage. Le refus n'est pas davantage un critère d'exclusion pour les lectures
    ///     Page20, dont le filtrage se limite à la série, à la disponibilité pour l'optimisation
    ///     et, pour le vivier, à l'article interne.
    ///   </description></item>
    /// </list>
    /// <para>
    /// AVERTISSEMENT DE SURFACE HÉRITÉE. La vue <see cref="vw_ProductionCutPiece_Full"/> est
    /// déclarée sans clé dans le contexte EF Core (<c>HasNoKey</c>, <c>ToView</c>). L'héritage du
    /// contrat générique expose donc au consommateur dix-huit méthodes dont huit sont visibles
    /// mais échouent à l'exécution sur ce type. Elles ne doivent jamais être appelées :
    /// </para>
    /// <list type="bullet">
    ///   <item><description>
    ///     <c>AddAsync</c> - suivi impossible sur un type sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>GetByIdAsync</c> - <c>FindAsync</c> n'est pas supporté sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>GetByIdAsNoTrackingAsync</c> - résolution par <c>EF.Property&lt;int&gt;(e, "Id")</c>,
    ///     propriété inexistante sur la vue.
    ///   </description></item>
    ///   <item><description>
    ///     <c>GetAnyAsync</c> - <c>FindAsync</c> n'est pas supporté sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>UpdateAsync</c> - suivi impossible sur un type sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>UpdateRangeAsync</c> - suivi impossible sur un type sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>DeleteAsync</c> - <c>FindAsync</c>, puis suppression trackée.
    ///   </description></item>
    ///   <item><description>
    ///     <c>SoftDeleteAsync</c> - <c>FindAsync</c>, puis mise à jour trackée.
    ///   </description></item>
    /// </list>
    /// <para>
    /// Les dix méthodes restantes sont opérantes sur ce type. Parmi elles, les trois lectures
    /// nominalement trackées - <c>GetFirstOrDefaultAsync</c>, <c>GetAllAsync</c> et
    /// <c>GetFilteredAsync</c> - fonctionnent, mais rendent des instances non suivies : EF Core
    /// ne trace jamais un type sans clé.
    /// </para>
    /// <para>
    /// Cette conséquence est assumée. L'extension du contrat générique est la forme canonique de
    /// la famille (Patron 2 « Extension par dérivation », §4.15.2 du 0230) ; la contrainte du
    /// socle n'exige qu'un type référence, ce que la vue satisfait ; l'héritage est valide à la
    /// compilation. La restriction porte sur l'usage et non sur la structure, et elle est portée
    /// par le présent avertissement.
    /// </para>
    /// </remarks>
    public interface IR_VwProductionCutPieceFull : IR_Generic<vw_ProductionCutPiece_Full>
    {
        /// <summary>
        /// Rend la liste des découpes rattachées à une série de production, réduites aux vingt
        /// champs utiles au cinquième onglet de la Page11, la réduction étant appliquée sur la
        /// requête et traduite en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La projection est appliquée sur la requête et non après matérialisation : seules vingt
        /// des deux cent trente-trois colonnes de la vue transitent depuis le serveur de base de
        /// données. C'est la raison d'être de la méthode et la justification du Patron 2. Le
        /// rapport de réduction y est le plus marqué du périmètre de la Page11, et le nombre de
        /// lignes le plus élevé, chaque châssis d'une série produisant plusieurs découpes.
        /// </para>
        /// <para>
        /// Aucun ordonnancement n'est appliqué. Les quatre critères de tri du tableau
        /// (<c>ACMDescription</c>, <c>ARSortOrder</c>, <c>PCPIdProductionBar</c>,
        /// <c>PCPCutPositionInBar</c>) figurent tous parmi les champs projetés - les premier et
        /// quatrième au titre des champs d'affichage, les deuxième et troisième au titre des
        /// champs de service - et sont mis à la disposition de l'appelant, qui trie en mémoire
        /// après extraction.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur l'indicateur de suppression logique
        /// <c>PCPIsDeleted</c>, et aucun recours n'est fait à <c>IgnoreQueryFilters</c> : aucun
        /// filtre global n'est configuré sur le contexte de données. Chez les découpes, à la
        /// différence du repository des barres, le refus n'est pas porté par l'indicateur de
        /// suppression logique mais par la colonne distincte <c>PCPIsCutRefused</c>, qui figure
        /// parmi les champs projetés.
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
        /// Liste des découpes projetées de la série demandée, jamais <see langword="null"/>, dans
        /// un ordre indéterminé. Liste vide si la série ne comporte aucune découpe - cas d'une
        /// série dont l'optimisation n'a pas encore été lancée, ou d'une série sans découpe : ce
        /// résultat est nominal et ne constitue pas une erreur.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant de série fourni est inférieur ou égal à zéro
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
        Task<List<DTO_VwProductionCutPieceFull_P11>> GetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        /// <summary>
        /// Désigne la prochaine découpe à réaliser dans une série de production, tous articles
        /// confondus, réduite aux treize champs utiles au moteur d'optimisation de la Page20, la
        /// sélection, le tri et la réduction étant traduits en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La découpe retournée est la première des découpes disponibles de la série, dans
        /// l'ordre du couple référence et couleur <c>PCPReferenceColor</c> croissant, puis de la
        /// clé technique <c>PCPId</c> croissante. Son article interne détermine la matière sur
        /// laquelle porte l'optimisation et constitue la clé de compatibilité avec le stock de
        /// chutes.
        /// </para>
        /// <para>
        /// Une découpe est disponible lorsqu'elle n'est pas coupée, n'est engagée dans aucune
        /// optimisation, ni provisoire ni définitive, n'est pas supprimée et n'est pas bloquée par
        /// une rupture de stock de sa barre. Le refus d'une découpe n'est pas un critère
        /// d'exclusion : une découpe refusée et non coupée reste à réaliser.
        /// </para>
        /// <para>
        /// Le tri est constitutif du contrat et non décoratif : il fixe l'ordre dans lequel
        /// l'atelier traite les références. Le départage par <c>PCPId</c> garantit qu'une même
        /// série présente toujours la même référence en premier.
        /// </para>
        /// <para>
        /// L'article interne de la découpe retournée est renseigné, en vertu d'un invariant de
        /// données garanti par l'import des séries ; la requête ne le contrôle pas.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à treize colonnes sur deux cent
        /// trente-trois ; le tri porte en outre sur deux colonnes, ce que le socle n'expose pas.
        /// <c>GetFirstOrDefaultAsNoTrackingAsync</c> avec prédicat ne trie pas et matérialise
        /// l'entité complète ; <c>GetPagedAsNoTrackingAsync</c>, appelée pour une fenêtre d'un
        /// seul enregistrement, ne trie que sur une colonne et matérialise elle aussi l'entité
        /// complète.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// La prochaine découpe disponible de la série, projetée, ou <see langword="null"/> si la
        /// série ne compte plus aucune découpe optimisable. Ce retour absent est nominal et ne
        /// constitue pas une erreur.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant de série fourni est inférieur ou égal à zéro
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
        Task<DTO_VwProductionCutPieceFull_P20?> GetNextReferenceToCutForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        /// <summary>
        /// Rend l'ensemble des découpes restant à réaliser pour un article interne d'une série de
        /// production, réduites aux treize champs utiles au moteur d'optimisation de la Page20, la
        /// sélection, le tri et la réduction étant traduits en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Le résultat constitue le vivier d'optimisation : la matière sur laquelle le moteur
        /// calcule le remplissage d'une barre de l'article interne demandé. Toutes les découpes
        /// disponibles de la série pour cet article sont rendues, sans limite de nombre.
        /// </para>
        /// <para>
        /// Une découpe est disponible lorsqu'elle n'est pas coupée, n'est engagée dans aucune
        /// optimisation, ni provisoire ni définitive, n'est pas supprimée et n'est pas bloquée par
        /// une rupture de stock de sa barre. Le refus d'une découpe n'est pas un critère
        /// d'exclusion : une découpe refusée et non coupée reste à réaliser.
        /// </para>
        /// <para>
        /// Le résultat est ordonné par clé technique <c>PCPId</c> croissante. Cet ordre porte le
        /// déterminisme du moteur d'optimisation : il départage les combinaisons de remplissage
        /// équivalentes, de sorte qu'une même situation d'atelier conduit toujours à la même
        /// barre proposée.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à treize colonnes sur deux cent
        /// trente-trois. <c>GetFilteredAsNoTrackingAsync</c> servirait la sélection, mais ne trie
        /// pas et matérialise l'entité complète.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="idArticleInternal">
        /// Identifiant de l'article interne dont le vivier est demandé, correspondant à la
        /// colonne <c>PCPIdArticleInternal</c> de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des découpes disponibles de l'article interne dans la série, projetées et
        /// ordonnées, jamais <see langword="null"/>. Une liste vide est un résultat nominal et ne
        /// constitue pas une erreur.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée, dans cet ordre de contrôle, si l'identifiant de série fourni est inférieur ou
        /// égal à zéro, puis si l'identifiant d'article interne fourni est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c> dans les deux cas).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<DTO_VwProductionCutPieceFull_P20>> GetOptimizationPoolForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            int idArticleInternal,
            CancellationToken ct = default);
    }
}