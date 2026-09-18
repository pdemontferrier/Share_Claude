using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// Contrat spécialisé du Query Handler dédié à la vue de base de données
    /// <see cref="vw_ProductionBar_Full"/>, qui expose la composition en barres retenues par
    /// l'optimisation pour les séries de production.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et étend le socle de lecture
    /// <see cref="IQ_Generic{T}"/> paramétré pour <see cref="vw_ProductionBar_Full"/>. Son
    /// implémentation concrète <c>QH_VwProductionBarFull</c> réside dans
    /// B_UseCases/Handlers/Queries/ et dérive de
    /// <c>QH_Generic&lt;vw_ProductionBar_Full&gt;</c>. Le contrat est consommé via injection,
    /// jamais par référence à son implémentation.
    /// </para>
    /// <para>
    /// Objectif : servir trois questions fermées portant sur les barres retenues par
    /// l'optimisation pour les séries de production. La première relève de la consultation :
    /// « quelles barres ont été retenues pour telle série de production, avec les seules
    /// caractéristiques que l'écran de consultation affiche ». Les deux autres relèvent de la
    /// validation des barres par l'opérateur conduite en Page20 : « quelle est la barre qui
    /// m'est présentée », afin de vérifier que la matière prise est la bonne avant de l'accepter,
    /// et « quelles barres bloquent la série pour rupture de stock », afin de voir ce qui empêche
    /// la progression et de libérer une barre dont la matière est revenue. Ces barres
    /// proviennent soit du stock de chutes issues de séries antérieures, soit du stock de barres
    /// neuves ; chacune porte, outre ses caractéristiques d'article et de profilé, cinq
    /// indicateurs d'état qui jalonnent son parcours - barre neuve ou chute, barre validée par
    /// l'opérateur, barre effectivement utilisée, barre en rupture de stock, barre refusée, ce
    /// dernier cas s'accompagnant d'un motif conservé sur l'enregistrement.
    /// </para>
    /// <para>
    /// La vue source expose quatre-vingt-deux colonnes ; le quatrième onglet de la Page11 en
    /// affiche seize. Les trois lectures partagent le même type de projection et rapatrient les
    /// mêmes dix-huit champs - les seize champs d'affichage, plus deux champs de service non
    /// affichés servant à l'identification des lignes, à la vérification de cohérence du lot
    /// reçu et, pour la lecture de consultation, à l'ordonnancement laissé à la charge de
    /// l'appelant. Ces dix-huit champs couvrent les caractéristiques que l'opérateur vérifie en
    /// Page20 : référence, désignation, couleur, longueur, origine neuve ou chute et nombre de
    /// découpes placées.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture Page11 : la lecture n'écarte aucun enregistrement au motif
    /// qu'il serait marqué comme logiquement supprimé. Le refus d'une barre par l'opérateur marque
    /// précisément l'enregistrement de cette façon, et l'écran doit afficher ces barres refusées
    /// avec leur motif : elles font partie intégrante du résultat attendu. Une série dont
    /// l'optimisation n'a pas encore été lancée ne porte aucune barre ; une liste vide est alors
    /// un résultat nominal et non une erreur.
    /// </para>
    /// <para>
    /// Portée du résultat - lectures Page20 : la lecture de la barre présentée rend la barre
    /// désignée quel que soit son état - validée, utilisée, en rupture de stock ou refusée - et
    /// au plus une barre. Son absence n'est pas une erreur à ce niveau : la barre ayant été
    /// désignée en amont, une valeur absente traduit une incohérence dont le traitement
    /// appartient à l'appelant. La lecture des barres en rupture ne retient que les barres de la
    /// série marquées en rupture de stock et non refusées, la validation n'étant pas un critère
    /// d'exclusion. Ce critère est strictement aligné sur celui du service
    /// <c>SR_ProductionSeries_SetBarOutOfStockFlag</c>, qui calcule l'indicateur de rupture de la
    /// série : une série signalée en attente présente toujours au moins une barre libérable, et
    /// une barre refusée, sortie du circuit, n'y figure pas. Son résultat est ordonné par ordre
    /// d'affichage de l'article puis par identifiant de barre, l'ordre étant constitutif de son
    /// contrat. Une liste vide y est un résultat nominal : série sans rupture, ou libération
    /// concurrente de la dernière barre en rupture.
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
    /// <c>IR_VwProductionBarFull</c> (Patron 2 de §4.15.2), et non par les treize lectures du socle
    /// hérité. Aucune de ces treize lectures ne rend un type <c>DTO_</c> : toutes rendent l'entité
    /// paramétrée, une liste de cette entité, un booléen ou un entier. La lecture des barres en
    /// rupture requiert en outre un tri sur deux colonnes, que le socle n'expose pas ; la lecture
    /// de la barre présentée ne peut davantage être servie par
    /// <c>HandleGetByIdAsNoTrackingAsync</c>, inopérante sur ce type sans clé (cf. avertissement
    /// de surface héritée ci-dessous).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Hériter du socle <see cref="IQ_Generic{T}"/> paramétré pour
    ///     <see cref="vw_ProductionBar_Full"/> afin d'exposer aux consommateurs les treize
    ///     lectures du socle en même temps que les trois lectures projetées propres à la vue.
    ///   </description></item>
    ///   <item><description>
    ///     Déclarer les trois lectures projetées - la lecture de consultation par identifiant de
    ///     série, la lecture de la barre présentée par identifiant de barre et la lecture des
    ///     barres en rupture par identifiant de série -, chacune avec sa signature de
    ///     traçabilité, sa frontière de retour <c>DTO_</c> de A_Domain et les retours signalables
    ///     qu'elle laisse remonter.
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
    ///     qui l'applique en mémoire après extraction. La lecture des barres en rupture de la
    ///     Page20 est en revanche ordonnée en aval, côté base de données, et rendue dans cet
    ///     ordre, qui est constitutif de son contrat ; la lecture de la barre présentée, qui rend
    ///     au plus une barre, n'appelle aucun ordonnancement.
    ///   </description></item>
    ///   <item><description>
    ///     Pour la lecture Page11, n'écarte aucun enregistrement au motif qu'il serait marqué
    ///     comme logiquement supprimé : la qualification d'une barre refusée relève de
    ///     l'affichage et non de l'exclusion. Pour la lecture de la barre présentée en Page20,
    ///     n'applique aucun filtrage d'état : la barre désignée est rendue quel que soit son
    ///     état, refus compris. Pour la lecture des barres en rupture en Page20, l'exclusion des
    ///     barres refusées fait partie du critère de sélection porté en aval ; le contrat n'y
    ///     ajoute rien.
    ///   </description></item>
    ///   <item><description>
    ///     N'expose aucune dépendance à EF Core ni à C_Infrastructure : la pureté contractuelle
    ///     de A_Domain est intégrale.
    ///   </description></item>
    /// </list>
    /// <para>
    /// AVERTISSEMENT DE SURFACE HÉRITÉE. La vue <see cref="vw_ProductionBar_Full"/> est déclarée
    /// sans clé dans le contexte EF Core (<c>HasNoKey</c>, <c>ToView</c>). L'héritage du socle de
    /// lecture expose donc au consommateur treize méthodes dont trois sont visibles mais échouent
    /// à l'exécution sur ce type. Elles ne doivent jamais être appelées :
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
    public interface IQ_VwProductionBarFull : IQ_Generic<vw_ProductionBar_Full>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Rend la liste des barres retenues par l'optimisation pour une série de production,
        /// réduites aux dix-huit champs utiles au quatrième onglet de la Page11.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// réduction de quatre-vingt-deux à dix-huit colonnes est appliquée sur la requête et
        /// traduite en clause <c>SELECT</c> côté serveur de base de données par le repository
        /// spécialisé délégué ; elle n'est jamais réalisée en mémoire.
        /// </para>
        /// <para>
        /// Objectif : offrir au consommateur un lot de lignes brut, qu'il lui appartient de trier
        /// et de mettre en forme. Aucun ordonnancement n'est appliqué : les trois critères de tri
        /// du tableau (<c>ARSortOrder</c>, <c>PBIsNewBar</c>, <c>PBId</c>) figurent parmi les
        /// champs projetés - les deux premiers au titre des champs d'affichage, le troisième au
        /// titre des champs de service - et sont mis à la disposition de l'appelant.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur l'indicateur de suppression logique : les barres
        /// refusées, marquées comme logiquement supprimées, font partie du résultat attendu et
        /// sont rendues avec leur motif de refus.
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
        /// Liste des barres projetées de la série demandée, dans l'ordre où la source les rend,
        /// soit un ordre indéterminé. Liste vide si la série ne comporte aucune barre - cas d'une
        /// série dont l'optimisation n'a pas encore été lancée : ce résultat est nominal et ne
        /// constitue pas une erreur. Ne retourne jamais <see langword="null"/>.
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
        Task<List<DTO_VwProductionBarFull>> HandleGetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        /// <summary>
        /// Rend la barre de production désignée par son identifiant, telle que l'écran de
        /// validation des barres de la Page20 la présente à l'opérateur, réduite aux dix-huit
        /// champs du type de projection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection et la réduction de quatre-vingt-deux à dix-huit colonnes sont appliquées sur
        /// la requête et traduites en SQL côté serveur de base de données par le repository
        /// spécialisé délégué ; elles ne sont jamais réalisées en mémoire.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « quelle est la barre qui m'est présentée ». La
        /// lecture sert l'onglet de détail de la barre présentée : l'opérateur y vérifie la
        /// matière - référence, désignation, couleur, longueur, origine neuve ou chute, nombre de
        /// découpes placées - avant d'accepter la barre.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur les indicateurs d'état : la barre désignée est rendue
        /// quel que soit son état, qu'elle soit validée, utilisée, en rupture de stock ou refusée.
        /// </para>
        /// <para>
        /// La lecture rend au plus une barre : l'identifiant demandé est la clé primaire de la
        /// table des barres de production qui pilote la vue, laquelle porte une ligne par barre.
        /// Aucun ordonnancement n'est appliqué.
        /// </para>
        /// <para>
        /// La barre est rendue sans transformation aucune : la référence produite en aval est
        /// retournée telle quelle, valeur absente comprise. Cette valeur absente n'est ni
        /// qualifiée ni contrôlée à ce niveau.
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
        /// porte cet identifiant. Ce retour absent n'est pas une erreur au niveau du Query Handler
        /// et ne lève aucune exception : la barre ayant été désignée en amont, il traduit une
        /// incohérence dont le traitement appartient à l'appelant.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="idProductionBar"/> est inférieur ou égal à zéro
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
        Task<DTO_VwProductionBarFull?> HandleGetByProductionBarIdForP20AsNoTrackingAsync(
            string caller,
            int idProductionBar,
            CancellationToken ct = default);

        /// <summary>
        /// Rend les barres d'une série de production mises en attente pour rupture de stock et non
        /// refusées, réduites aux dix-huit champs du type de projection, à destination de l'écran
        /// de validation des barres de la Page20.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection, l'ordonnancement et la réduction de quatre-vingt-deux à dix-huit colonnes
        /// sont appliqués sur la requête et traduits en SQL côté serveur de base de données par le
        /// repository spécialisé délégué ; ils ne sont jamais réalisés en mémoire.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « quelles barres bloquent la série pour rupture de
        /// stock ». La lecture sert l'onglet des barres en rupture de la série, accessible dans le
        /// seul régime où plus aucune découpe n'est optimisable mais où il en subsiste, toutes
        /// bloquées par une rupture de stock : l'opérateur y voit ce qui empêche la progression et
        /// peut libérer une barre dont la matière est revenue.
        /// </para>
        /// <para>
        /// Une barre est retenue lorsqu'elle appartient à la série demandée, qu'elle est marquée
        /// en rupture de stock (<c>PBIsOutOfStock</c>) et qu'elle n'est pas refusée
        /// (<c>PBIsDeleted</c>). La validation n'est pas un critère d'exclusion : une barre en
        /// rupture et validée est rendue. Ce critère est strictement aligné sur celui du service
        /// <c>SR_ProductionSeries_SetBarOutOfStockFlag</c>, qui calcule l'indicateur de rupture de
        /// la série : une série signalée en attente présente toujours au moins une barre
        /// libérable, et une barre refusée, sortie du circuit, n'y figure pas.
        /// </para>
        /// <para>
        /// Le résultat est ordonné par <c>ARSortOrder</c> croissant, puis par <c>PBId</c>
        /// croissant. L'ordre est constitutif du contrat et non décoratif : le départage par
        /// <c>PBId</c> garantit qu'une même situation d'atelier présente toujours les barres dans
        /// le même ordre. Il est établi en aval et n'est pas repris à ce niveau. Toutes les barres
        /// retenues sont rendues, sans limite de nombre.
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
        /// Liste des barres en rupture et non refusées de la série, projetées et ordonnées par
        /// <c>ARSortOrder</c> puis par <c>PBId</c> croissants. Ne retourne jamais
        /// <see langword="null"/>. Une liste vide est un résultat nominal et ne constitue pas une
        /// erreur : la série ne compte alors aucune barre en rupture, par exemple après la
        /// libération concurrente de la dernière d'entre elles.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="productionSeriesId"/> est inférieur ou égal à zéro
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
        Task<List<DTO_VwProductionBarFull>> HandleGetOutOfStockByProductionSeriesIdForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        #endregion
    }
}