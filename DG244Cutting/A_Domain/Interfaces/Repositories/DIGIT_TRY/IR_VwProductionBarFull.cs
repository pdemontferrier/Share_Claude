using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY
{
    /// <summary>
    /// Contrat spécialisé du repository dédié à la vue de base de données
    /// <see cref="vw_ProductionBar_Full"/>, qui expose les barres retenues par l'optimisation pour
    /// les séries de production, sous la forme de trois lectures projetées sur un type de
    /// transport commun.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et étend le contrat générique
    /// <see cref="IR_Generic{T}"/> paramétré pour <see cref="vw_ProductionBar_Full"/>. Son
    /// implémentation concrète <c>CR_VwProductionBarFull</c> réside dans
    /// C_Infrastructure/Repositories/DIGIT_TRY/ et dérive de
    /// <c>CR_Generic&lt;vw_ProductionBar_Full&gt;</c>.
    /// </para>
    /// <para>
    /// Objectif : servir trois questions fermées portant sur les barres retenues par
    /// l'optimisation. La lecture par série répond à « quelles barres ont été retenues pour telle
    /// série de production » : elle sert le suivi d'une série dont la production est achevée, où
    /// se lit ce qui a été produit, refusé ou bloqué. La lecture par barre répond à « quelle est
    /// la barre désignée par tel identifiant » : elle sert le poste de décision, où l'opérateur
    /// compare la matière proposée à celle qu'il a sous les yeux avant de l'accepter ou de
    /// l'écarter. La lecture des barres en rupture répond à « quelles barres de la série sont
    /// mises en attente pour rupture de stock » : elle sert la libération d'une barre dont la
    /// matière est revenue, dans le seul régime où plus rien n'est optimisable. Ces barres
    /// proviennent soit du stock de chutes issues de séries antérieures, soit du stock de barres
    /// neuves ; chacune porte, outre ses caractéristiques d'article et de profilé, cinq
    /// indicateurs d'état qui jalonnent son parcours - barre neuve ou chute, barre validée par
    /// l'opérateur, barre effectivement utilisée, barre en rupture de stock, barre refusée, ce
    /// dernier cas s'accompagnant d'un motif conservé sur l'enregistrement. La vue source expose
    /// quatre-vingt-dix-sept colonnes.
    /// </para>
    /// <para>
    /// Les trois lectures rapatrient les mêmes vingt et un champs, sous le même type de
    /// projection : dix-neuf champs d'affichage et deux champs de service non affichés, ces
    /// derniers servant à l'identification des lignes, à la vérification de cohérence du lot reçu
    /// et à l'ordonnancement. Les deux natures ne forment pas deux blocs contigus : l'ordre retenu
    /// est celui du type projeté, où <c>CSLScrapLocationSource</c> s'intercale parmi les champs
    /// d'affichage et où <c>PSIdSerialNumber</c> et <c>PSDescription</c> s'intercalent entre les
    /// deux champs de service. Le type est commun aux trois lectures : trois de ses champs ne
    /// servent nominalement que la fiche de la barre désignée, mais transitent néanmoins par les
    /// deux lectures de liste, un type partiellement alimenté ne permettant pas de distinguer un
    /// champ non projeté d'un champ nul en base.
    /// </para>
    /// <para>
    /// Deux identifiants de série sont projetés et ne sont jamais interchangeables.
    /// <c>PSId</c> est la clé technique de la série et le paramètre de deux des trois lectures ;
    /// <c>PSIdSerialNumber</c> est le numéro métier sous lequel l'atelier désigne la série, sur
    /// les documents comme dans les échanges entre postes, et aucune lecture ne se paramètre par
    /// lui. <c>PSDescription</c> complète ce numéro du libellé de chantier ou de commande que le
    /// numéro seul ne porte pas. <c>CSLScrapLocationSource</c>, pour sa part, porte l'emplacement
    /// réel d'où la matière est à prendre : renseigné sur une barre de chute, absent sur une barre
    /// neuve, la vue ne joignant alors aucune chute.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture par série : à la différence des autres lectures du projet,
    /// cette lecture ne filtre pas les enregistrements marqués comme logiquement supprimés. Le
    /// refus d'une barre par l'opérateur marque précisément l'enregistrement de cette façon, et
    /// les barres refusées font partie intégrante du résultat attendu : elles sont rendues avec
    /// leur motif. Une série dont l'optimisation n'a pas encore été lancée ne porte aucune barre ;
    /// une liste vide est alors un résultat nominal et non une erreur.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture par barre et lecture des barres en rupture : la lecture par
    /// barre ne filtre aucun indicateur d'état ; la barre désignée est rendue quel que soit son
    /// état, qu'elle soit validée, utilisée, en rupture de stock ou refusée. L'absence de ligne
    /// pour l'identifiant fourni produit une valeur absente, qui n'est pas une erreur au niveau du
    /// repository : la barre ayant été désignée en amont, ce retour traduit une incohérence dont
    /// le traitement appartient à l'appelant. La lecture des barres en rupture retient les barres
    /// marquées en rupture de stock et exclut les barres refusées, par alignement strict sur le
    /// critère du service qui calcule l'indicateur de rupture de la série : une barre refusée est
    /// sortie du circuit, et sa rupture éventuelle n'a plus de portée. La liste rendue coïncide
    /// ainsi exactement avec les barres qui maintiennent la série en attente ; la validation n'y
    /// est pas un critère d'exclusion. Cette lecture ordonne son résultat côté base de données,
    /// l'ordre faisant partie de son contrat, et une liste vide y est un résultat nominal.
    /// </para>
    /// <para>
    /// Justification du Patron 2 : cette interface n'existe que parce que les trois lectures de
    /// son périmètre nécessitent une API EF Core non disponible dans <see cref="IR_Generic{T}"/> -
    /// en l'occurrence la projection SQL traduite côté base de données, soit un <c>Select</c>
    /// retournant un type <c>DTO_</c> par expression LINQ-to-Entities. Aucune des dix-huit
    /// méthodes du contrat générique ne rend un type <c>DTO_</c> : toutes rendent l'entité
    /// paramétrée, une liste de cette entité, un booléen ou un entier. Servir ces besoins par
    /// consommation directe du contrat générique imposerait de matérialiser les
    /// quatre-vingt-dix-sept colonnes puis d'en écarter soixante-seize en mémoire, ce qui
    /// relèverait du Cas 2 (transformation LINQ-to-Objects portée par un Query Handler) et ferait
    /// perdre la réduction côté base qui est la finalité même du composant. La lecture des barres
    /// en rupture requiert en outre un tri sur deux colonnes, que le socle n'expose pas.
    /// Conformément au sous-bloc « Critère de création d'un Repository spécialisé CR_[Entité] » de
    /// §4.14.6 du 0230, la projection SQL traduite côté base est nominativement rangée au Cas 3,
    /// seul cas justifiant la création d'un repository spécialisé.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Hériter du contrat <see cref="IR_Generic{T}"/> paramétré pour
    ///     <see cref="vw_ProductionBar_Full"/> afin d'exposer aux consommateurs les dix-huit
    ///     méthodes du socle générique en même temps que les lectures projetées propres à la vue.
    ///   </description></item>
    ///   <item><description>
    ///     Déclarer les trois lectures projetées qui mobilisent la projection SQL absente du
    ///     contrat générique - lecture par série, lecture par barre et lecture des barres en
    ///     rupture -, toutes réduites au type projeté. La réduction est dans tous les cas
    ///     traduite en clause <c>SELECT</c> côté serveur de base de données.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne redéclare aucune des dix-huit méthodes du contrat <see cref="IR_Generic{T}"/> :
    ///     leur implémentation par défaut portée par
    ///     <c>CR_Generic&lt;vw_ProductionBar_Full&gt;</c> est finale, conformément à R-4.15.3
    ///     et I-4.15.1 du 0231.
    ///   </description></item>
    ///   <item><description>
    ///     Ne porte aucune règle métier et aucun calcul. La lecture par série n'applique aucun
    ///     ordonnancement : le tri du tableau relève de l'appelant, qui l'applique en mémoire après
    ///     extraction. La lecture des barres en rupture ordonne en revanche son résultat côté base
    ///     de données, l'ordre étant constitutif de son contrat. La question de l'ordonnancement
    ///     est sans objet pour la lecture par barre, qui rend au plus une barre.
    ///   </description></item>
    ///   <item><description>
    ///     N'expose aucune opération d'écriture. La vue étant une source de lecture seule, toute
    ///     signature de persistance serait sans objet ; le contrat n'en déclare aucune, et le
    ///     socle n'en expose aucune qui soit opérante sur ce type (cf. avertissement ci-dessous).
    ///   </description></item>
    ///   <item><description>
    ///     Pour la lecture par série et pour la lecture par barre, n'écarte aucun
    ///     enregistrement au motif qu'il serait marqué comme logiquement supprimé. La
    ///     qualification d'une barre refusée relève alors de l'affichage et non de l'exclusion :
    ///     l'indicateur de refus et son motif sont projetés au même titre que les autres champs.
    ///     La lecture des barres en rupture écarte en revanche les barres refusées, par
    ///     alignement sur le critère de rupture de la série.
    ///   </description></item>
    /// </list>
    /// <para>
    /// AVERTISSEMENT DE SURFACE HÉRITÉE. La vue <see cref="vw_ProductionBar_Full"/> est
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
    public interface IR_VwProductionBarFull : IR_Generic<vw_ProductionBar_Full>
    {
        /// <summary>
        /// Rend la liste des barres retenues par l'optimisation pour une série de production,
        /// réduites au type projeté, la réduction étant appliquée sur la requête et traduite
        /// en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La projection est appliquée sur la requête et non après matérialisation : seules
        /// vingt et une des quatre-vingt-dix-sept colonnes de la vue transitent depuis le serveur
        /// de base de données. C'est la raison d'être de la méthode et la justification du
        /// Patron 2.
        /// </para>
        /// <para>
        /// Aucun ordonnancement n'est appliqué. Les trois critères de tri du tableau
        /// (<c>ARSortOrder</c>, <c>PBIsNewBar</c>, <c>PBId</c>) figurent parmi les champs projetés
        /// - les deux premiers au titre des champs d'affichage, le troisième au titre des champs
        /// de service - et sont mis à la disposition de l'appelant, qui trie en mémoire après
        /// extraction.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur l'indicateur de suppression logique, et aucun recours
        /// n'est fait à <c>IgnoreQueryFilters</c> : aucun filtre global n'est configuré sur le
        /// contexte de données, et la vue ne filtre pas l'indicateur de suppression de sa table
        /// pilote. Les barres refusées, marquées comme logiquement supprimées, font partie du
        /// résultat attendu et sont rendues avec leur motif de refus.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif. Il s'agit d'un identifiant fonctionnel
        /// étranger, hérité de la table d'origine de la série, et non de la clé de la vue : la vue
        /// n'en a pas. À distinguer de <c>PSIdSerialNumber</c>, numéro métier de la série, qui est
        /// projeté mais n'est jamais un paramètre de sélection.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des barres projetées de la série demandée, jamais <see langword="null"/>, dans un
        /// ordre indéterminé. Liste vide si la série ne comporte aucune barre - cas d'une série
        /// dont l'optimisation n'a pas encore été lancée : ce résultat est nominal et ne constitue
        /// pas une erreur.
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
        Task<List<DTO_VwProductionBarFull>> GetByProductionSeriesIdAsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        /// <summary>
        /// Rend la barre désignée par son identifiant, réduite au type projeté, la sélection et la
        /// réduction étant traduites en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La lecture sert le poste de décision de l'opérateur : il y vérifie la matière -
        /// référence, désignation, couleur, longueur, origine neuve ou chute, emplacement d'où la
        /// prendre, nombre de découpes placées - avant d'accepter la barre. Le type rendu est
        /// <see cref="DTO_VwProductionBarFull"/>.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur les indicateurs d'état : la barre désignée est rendue
        /// quel que soit son état, qu'elle soit validée, utilisée, en rupture de stock ou refusée.
        /// Aucun recours n'est fait à <c>IgnoreQueryFilters</c> : aucun filtre global n'est
        /// configuré sur le contexte de données.
        /// </para>
        /// <para>
        /// La lecture rend au plus une barre : <c>PBId</c> est la clé primaire de la table des
        /// barres qui pilote la vue, laquelle porte une ligne par barre. Aucun tri n'est appliqué.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à vingt et une colonnes sur
        /// quatre-vingt-dix-sept. <c>GetFirstOrDefaultAsNoTrackingAsync</c> avec prédicat servirait
        /// la sélection, mais matérialise l'entité complète ; <c>GetByIdAsNoTrackingAsync</c> est
        /// inopérante sur ce type sans clé (cf. avertissement de surface héritée).
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre, correspondant à la colonne <c>PBId</c> de la vue, clé primaire
        /// de la table des barres de production. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// La barre désignée, projetée, ou <see langword="null"/> si aucune ligne de la vue ne
        /// porte cet identifiant. Ce retour absent n'est pas une erreur au niveau du repository et
        /// ne lève aucune exception : la barre ayant été désignée en amont, il traduit une
        /// incohérence dont le traitement appartient à l'appelant.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant de barre fourni est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>). Un identifiant nul ou négatif traduit une barre non désignée,
        /// anomalie qui doit remonter plutôt que produire une absence de résultat indiscernable
        /// d'une barre introuvable.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<DTO_VwProductionBarFull?> GetByProductionBarIdAsNoTrackingAsync(
            string caller,
            int idProductionBar,
            CancellationToken ct = default);

        /// <summary>
        /// Rend les barres d'une série de production mises en attente pour rupture de stock et non
        /// refusées, réduites au type projeté, la sélection, le tri et la réduction étant traduits
        /// en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La lecture sert la libération des barres bloquées : elle donne à voir ce qui maintient
        /// la série en attente et permet de libérer une barre dont la matière est revenue. Le type
        /// rendu est <see cref="DTO_VwProductionBarFull"/>.
        /// </para>
        /// <para>
        /// Une barre est retenue lorsqu'elle appartient à la série demandée, qu'elle est marquée
        /// en rupture de stock (<c>PBIsOutOfStock</c>) et qu'elle n'est pas refusée
        /// (<c>PBIsDeleted</c>). Ce critère est strictement aligné sur celui du service
        /// <c>SR_ProductionSeries_SetBarOutOfStockFlag</c>, qui calcule l'indicateur de rupture de
        /// la série : la liste rendue coïncide exactement avec les barres qui maintiennent la série
        /// en attente, de sorte qu'une série signalée en attente présente toujours au moins une
        /// barre libérable. Une barre refusée est sortie du circuit, et sa rupture éventuelle n'a
        /// plus de portée. La validation n'est pas un critère d'exclusion : une barre en rupture et
        /// validée est rendue. Les indicateurs de rupture et de refus restent projetés tels quels.
        /// </para>
        /// <para>
        /// Le résultat est ordonné par <c>ARSortOrder</c> croissant, puis par <c>PBId</c>
        /// croissant. L'ordre est constitutif du contrat : le départage par <c>PBId</c> garantit
        /// qu'une même situation d'atelier présente toujours les barres dans le même ordre. Toutes
        /// les barres retenues sont rendues, sans limite de nombre.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à vingt et une colonnes sur
        /// quatre-vingt-dix-sept ; le tri porte en outre sur deux colonnes, ce que le socle
        /// n'expose pas. <c>GetFilteredAsNoTrackingAsync</c> servirait la sélection, mais ne trie
        /// pas et matérialise l'entité complète ; <c>GetPagedAsNoTrackingAsync</c> ne trie que sur
        /// une colonne, impose une fenêtre bornée et matérialise elle aussi l'entité complète.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif. À distinguer de <c>PSIdSerialNumber</c>,
        /// numéro métier de la série, qui est projeté mais n'est jamais un paramètre de sélection.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des barres en rupture et non refusées de la série, projetées et ordonnées, jamais
        /// <see langword="null"/>. Une liste vide est un résultat nominal et ne constitue pas une
        /// erreur : la série ne compte alors aucune barre en rupture, par exemple après la
        /// libération concurrente de la dernière d'entre elles.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant de série fourni est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>). Un identifiant nul ou négatif traduit un contexte de série non
        /// renseigné, anomalie qui doit remonter plutôt que produire une liste vide indiscernable
        /// d'une série sans rupture.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<DTO_VwProductionBarFull>> GetOutOfStockByProductionSeriesIdAsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);
    }
}