using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Contrat du service métier de lecture-projection des séries de production admissibles
    /// relevant du périmètre de découpe piloté, destiné au tableau de bord Page10.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de placement
    /// des contrats de la famille Services. Elle exprime un besoin de lecture-projection pure, sans
    /// exposer aux couches consommatrices la source de lecture retenue, la délégation au Query
    /// Handler générique ni la convention de propagation de la CallChain. L'implémentation
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_ProductionSeries_Get"/> réside en
    /// <c>B_UseCases/Services/Business</c>. Le contrat appartient au domaine <c>Business</c>.
    /// </para>
    /// <para>
    /// Objectif : restituer au composant consommateur l'ensemble des séries de production sur
    /// lesquelles l'opérateur est susceptible d'intervenir, sous une forme directement affichable -
    /// statut de classement, indicateur de retard, clé de tri semaine-jour - et déjà ordonnée, de
    /// sorte que le consommateur n'ait à porter ni règle de classement, ni règle de tri.
    /// </para>
    /// <para>
    /// Notion de série admissible : une série est admissible lorsqu'elle est importée, qu'elle porte
    /// ses deux dates de production, et qu'elle comporte au moins une pièce de découpe dans le
    /// périmètre piloté par la machine. Cette dernière restriction est garantie par la source de
    /// lecture <c>vw_ProductionSeries_Full</c>, dont le contenu est restreint aux seules séries
    /// satisfaisant cette condition ; l'énumération des gammes et catégories de pièces composant ce
    /// périmètre réside en base et n'est pas détenue par le code applicatif. Une série dépourvue de
    /// toute pièce de découpe n'est donc jamais restituée.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération de lecture-projection des séries de production admissibles.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// <item><description>Garantir au consommateur une liste plate, jamais nulle, déjà triée et qualifiée.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne mute aucun état et ne déclare aucune opération d'écriture : les mutations portées sur une série relèvent de contrats distincts.</description></item>
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'expose aucune frontière transactionnelle.</description></item>
    /// <item><description>Ne déclare ni pagination, ni filtrage complémentaire, ni regroupement par statut : la répartition en colonnes d'affichage relève du composant consommateur.</description></item>
    /// <item><description>N'expose aucun compteur d'avancement de découpe : le contenu de l'objet de transport restitué est stable.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DTO_ProductionSeriesItem"/>
    public interface IS_ProductionSeries_Get
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Retourne l'ensemble des séries de production admissibles, projetées en objets de
        /// transport qualifiés et triés.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée lors du chargement ou du rafraîchissement du tableau de bord des
        /// séries. L'opération est une lecture-projection pure : lecture filtrée sans suivi des
        /// changements, projection en mémoire vers objet de transport avec calcul des champs
        /// dérivés, tri, retour. Aucun état n'est muté, aucune écriture n'est émise.
        /// </para>
        /// <para>
        /// Objectif : restituer une liste plate déjà ordonnée selon trois critères successifs - date
        /// de fin de production, puis clé semaine-jour, puis numéro de série - dont chaque élément
        /// porte l'un des cinq statuts de classement réels ; la valeur sentinelle <c>NotValidated</c>
        /// n'est jamais restituée.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Recevoir la CallChain amont et le jeton d'annulation coopérative.</description></item>
        /// <item><description>Restituer les séries admissibles projetées en objets de transport qualifiés.</description></item>
        /// <item><description>Restituer la liste déjà triée selon les trois critères successifs.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne porte aucune validation d'argument : aucun identifiant n'est reçu et <paramref name="caller"/> n'est pas contrôlé.</description></item>
        /// <item><description>N'expose ni indicateur de résultat, ni code de retour : l'absence de matière est portée par une liste vide, l'échec par exception.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// CallChain amont transmise par l'appelant, enrichie localement par l'implémentation selon
        /// le format normatif de propagation. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Liste plate des séries de production admissibles, triée par date de fin de production,
        /// puis par clé semaine-jour en comparaison ordinale, puis par numéro de série. Retourne une
        /// liste vide si aucune série n'est admissible ; ne retourne jamais <see langword="null"/>.
        /// </returns>
        /// <exception cref="Ex_Business">Levée lorsqu'une erreur métier est détectée en aval lors de la lecture.</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors de l'accès aux données.</exception>
        /// <exception cref="OperationCanceledException">Levée lorsque l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task<List<DTO_ProductionSeriesItem>> GetProductionSeriesAsync(string caller, CancellationToken ct = default);

        #endregion
    }
}