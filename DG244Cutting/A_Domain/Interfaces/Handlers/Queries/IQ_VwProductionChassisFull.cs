using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// Contrat spécialisé du Query Handler dédié à la vue de base de données
    /// <see cref="vw_ProductionChassis_Full"/>, qui expose la composition physique des séries
    /// de production.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et étend le socle de lecture
    /// <see cref="IQ_Generic{T}"/> paramétré pour <see cref="vw_ProductionChassis_Full"/>. Son
    /// implémentation concrète <c>QH_VwProductionChassisFull</c> réside dans
    /// B_UseCases/Handlers/Queries/ et dérive de
    /// <c>QH_Generic&lt;vw_ProductionChassis_Full&gt;</c>. Le contrat est consommé via injection,
    /// jamais par référence à son implémentation.
    /// </para>
    /// <para>
    /// Objectif : exposer à la couche des cas d'usage la lecture projetée des châssis d'une série
    /// de production, réduits aux seize champs utiles au troisième onglet de la Page11. La vue
    /// source expose soixante-seize colonnes ; l'écran en affiche onze. La lecture rapatrie seize
    /// champs - les onze champs d'affichage, plus cinq champs de service non affichés servant à
    /// l'identification des lignes, à la vérification de cohérence du lot reçu et à
    /// l'ordonnancement laissé à la charge de l'appelant.
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
    /// donc servie par délégation au repository spécialisé
    /// <c>IR_VwProductionChassisFull</c> (Patron 2 de §4.15.2), et non par les treize lectures du
    /// socle hérité. Aucune de ces treize lectures ne rend un type <c>DTO_</c> : toutes rendent
    /// l'entité paramétrée, une liste de cette entité, un booléen ou un entier.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Hériter du socle <see cref="IQ_Generic{T}"/> paramétré pour
    ///     <see cref="vw_ProductionChassis_Full"/> afin d'exposer aux consommateurs les treize
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
    ///     forme : le tri du tableau relève de l'appelant, qui l'applique après extraction.
    ///   </description></item>
    ///   <item><description>
    ///     N'expose aucune dépendance à EF Core ni à C_Infrastructure : la pureté contractuelle
    ///     de A_Domain est intégrale.
    ///   </description></item>
    /// </list>
    /// <para>
    /// AVERTISSEMENT DE SURFACE HÉRITÉE. La vue <see cref="vw_ProductionChassis_Full"/> est
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
    public interface IQ_VwProductionChassisFull : IQ_Generic<vw_ProductionChassis_Full>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Rend la liste des châssis rattachés à une série de production, réduits aux seize champs
        /// utiles au troisième onglet de la Page11.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// réduction de soixante-seize à seize colonnes est appliquée sur la requête et traduite
        /// en clause <c>SELECT</c> côté serveur de base de données par le repository spécialisé
        /// délégué ; elle n'est jamais réalisée en mémoire.
        /// </para>
        /// <para>
        /// Objectif : offrir au service métier consommateur un lot de lignes brut, qu'il lui
        /// appartient de trier et de mettre en forme. Aucun ordonnancement n'est appliqué : les
        /// trois critères de tri du tableau (<c>COIdOrder</c>, <c>COPartialSeriesIndex</c>,
        /// <c>PCOrderPosition</c>) figurent parmi les champs de service projetés et sont mis à la
        /// disposition de l'appelant.
        /// </para>
        /// <para>
        /// Le lot est rendu sans transformation aucune - ni tri, ni filtrage, ni recopie, ni
        /// projection complémentaire : la référence produite en aval est retournée telle quelle.
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
        /// Liste des châssis projetés de la série demandée, dans l'ordre où la source les rend.
        /// Liste vide si la série ne comporte aucun châssis : ce résultat est nominal et ne
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
        Task<List<DTO_VwProductionChassisFull_P11>> HandleGetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);

        #endregion
    }
}