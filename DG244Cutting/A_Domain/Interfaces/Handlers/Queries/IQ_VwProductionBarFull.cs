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
    /// Objectif : servir la question fermée « quelles barres ont été retenues par l'optimisation
    /// pour telle série de production, avec les seules caractéristiques que l'écran de
    /// consultation affiche ». Ces barres proviennent soit du stock de chutes issues de séries
    /// antérieures, soit du stock de barres neuves ; chacune porte, outre ses caractéristiques
    /// d'article et de profilé, cinq indicateurs d'état qui jalonnent son parcours - barre neuve
    /// ou chute, barre validée par l'opérateur, barre effectivement utilisée, barre en rupture de
    /// stock, barre refusée, ce dernier cas s'accompagnant d'un motif conservé sur
    /// l'enregistrement. La vue source expose quatre-vingt-deux colonnes ; le quatrième onglet de
    /// la Page11 en affiche seize. La lecture rapatrie dix-huit champs - les seize champs
    /// d'affichage, plus deux champs de service non affichés servant à l'identification des
    /// lignes, à la vérification de cohérence du lot reçu et à l'ordonnancement laissé à la
    /// charge de l'appelant.
    /// </para>
    /// <para>
    /// Portée du résultat : la lecture exposée n'écarte aucun enregistrement au motif qu'il
    /// serait marqué comme logiquement supprimé. Le refus d'une barre par l'opérateur marque
    /// précisément l'enregistrement de cette façon, et l'écran doit afficher ces barres refusées
    /// avec leur motif : elles font partie intégrante du résultat attendu. Une série dont
    /// l'optimisation n'a pas encore été lancée ne porte aucune barre ; une liste vide est alors
    /// un résultat nominal et non une erreur.
    /// </para>
    /// <para>
    /// Positionnement CQRS : le contrat est strictement côté lecture. Il ne déclare aucune
    /// signature d'écriture, aucune mutation, aucun point de validation transactionnelle. La
    /// question est au demeurant sans objet, la vue étant une source de lecture seule.
    /// </para>
    /// <para>
    /// Sous-cas de lecture spécialisée : la lecture déclarée relève du second sous-cas du critère
    /// de lecture spécialisée de §4.14.5 du 0230. Elle mobilise une API EF Core absente du contrat
    /// <c>IR_Generic&lt;T&gt;</c> - la projection SQL traduite côté base de données, soit un
    /// <c>Select</c> retournant un type <c>DTO_</c> par expression LINQ-to-Entities - et elle est
    /// donc servie par délégation au repository spécialisé <c>IR_VwProductionBarFull</c>
    /// (Patron 2 de §4.15.2), et non par les treize lectures du socle hérité. Aucune de ces treize
    /// lectures ne rend un type <c>DTO_</c> : toutes rendent l'entité paramétrée, une liste de
    /// cette entité, un booléen ou un entier.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Hériter du socle <see cref="IQ_Generic{T}"/> paramétré pour
    ///     <see cref="vw_ProductionBar_Full"/> afin d'exposer aux consommateurs les treize
    ///     lectures du socle en même temps que la lecture projetée propre à la vue.
    ///   </description></item>
    ///   <item><description>
    ///     Déclarer la lecture projetée par identifiant de série, avec sa signature de
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
    ///     Ne porte aucune règle métier, aucun calcul, aucun ordonnancement et aucune mise en
    ///     forme : le tri du tableau relève de l'appelant, qui l'applique en mémoire après
    ///     extraction.
    ///   </description></item>
    ///   <item><description>
    ///     N'écarte aucun enregistrement au motif qu'il serait marqué comme logiquement supprimé.
    ///     La qualification d'une barre refusée relève de l'affichage et non de l'exclusion.
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
        Task<List<DTO_VwProductionBarFull_P11>> HandleGetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        #endregion
    }
}