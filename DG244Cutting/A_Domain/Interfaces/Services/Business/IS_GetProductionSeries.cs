using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;

namespace DG244Cutting.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// Description :
    /// <para>
    /// Contrat du service métier de lecture-projection des séries de production
    /// admissibles, destiné à alimenter le tableau de bord Page10.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette interface est définie dans la couche A_Domain afin de constituer le
    /// contrat fonctionnel d'un service de lecture-projection pure, sans dépendre
    /// d'une implémentation technique particulière. Elle est consommée par le
    /// ViewModel <c>VM_Page10</c> (Singleton), qui invoque le service via
    /// <c>IS_UseCaseInvoker</c> afin de franchir la frontière Singleton vers Scoped
    /// (EA-11). L'implémentation <c>SR_GetProductionSeries</c> réside en
    /// B_UseCases/Services/Business/.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Fournir l'ensemble des séries de production admissibles, projetées en objets
    /// de transport affichables et qualifiés (statut de classement, indicateur de
    /// retard, clé de tri semaine-jour), sous forme d'une liste plate déjà triée.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// ViewModel <c>VM_Page10</c> du tableau de bord, via médiation
    /// <c>IS_UseCaseInvoker</c>.
    /// </para>
    ///
    /// Tâches / Actions :
    /// <list type="bullet">
    /// <item><description>Définir le contrat de lecture-projection des séries de production admissibles.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c>.</description></item>
    /// <item><description>Retourner une liste plate triée de <c>DTO_ProductionSeriesItem</c>, exploitable par le ViewModel appelant.</description></item>
    /// </list>
    /// </summary>
    public interface IS_GetProductionSeries
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>
        /// Retourne l'ensemble des séries de production admissibles, projetées en
        /// objets de transport affichables, qualifiées et triées.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette méthode est appelée depuis le ViewModel du tableau de bord Page10
        /// lors du chargement ou du rafraîchissement de la vue. Elle représente une
        /// opération de lecture-projection pure : elle lit les entités admissibles
        /// via un Query Handler, les projette en DTO en calculant les champs dérivés,
        /// trie le résultat, et le retourne. Elle ne mute aucun état et n'écrit rien.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner une liste plate de <c>DTO_ProductionSeriesItem</c> déjà triée
        /// (par date de fin de production, puis clé semaine-jour, puis numéro de
        /// série), chaque élément portant un statut de classement réel, jamais la
        /// sentinelle <c>NotValidated</c>.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Recevoir la CallChain amont.</description></item>
        /// <item><description>Lire les séries admissibles et les projeter en objets de transport qualifiés.</description></item>
        /// <item><description>Retourner la liste plate triée des séries admissibles.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont transmise par l'appelant.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Une liste plate, jamais nulle (vide si aucune série n'est admissible), de
        /// <c>DTO_ProductionSeriesItem</c> triée selon les trois critères successifs
        /// date de fin de production, clé semaine-jour, numéro de série.
        /// </returns>
        /// <exception cref="Ex_Business">Levée si une erreur métier est détectée en aval lors de la lecture.</exception>
        /// <exception cref="Ex_Infrastructure">Levée si une erreur technique survient lors de l'accès aux données.</exception>
        Task<List<DTO_ProductionSeriesItem>> GetProductionSeriesAsync(string caller, CancellationToken ct = default);

        #endregion
    }
}