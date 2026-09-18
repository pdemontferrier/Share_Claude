
namespace LeitTemporImport.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service chargé d’exécuter les mises à jour post-import d’une série via une chaîne
    /// séquentielle de procédures stockées SQL Server.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé après l’import applicatif (Tempor → Tempor_Import) et le marquage applicatif
    /// de la série comme importée.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Consolider les données métier en base via une séquence ordonnée de procédures
    /// stockées. Chaque procédure est exécutée indépendamment : en cas d’erreur,
    /// celle-ci est journalisée avec le numéro de série et le détail technique,
    /// puis le traitement continue avec la procédure suivante.
    /// </para>
    /// <para>
    /// La procédure finale <c>spr_ProductionSeries_FinalizeImport</c> n’est exécutée
    /// que si toutes les procédures précédentes se sont terminées sans erreur.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases batch / console du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Exécuter les procédures stockées post-import dans un ordre strict.</description></item>
    /// <item><description>Journaliser toute erreur en incluant le numéro de série et le détail SQL.</description></item>
    /// <item><description>Poursuivre l’exécution malgré une erreur sur une étape intermédiaire.</description></item>
    /// <item><description>Exécuter la finalisation uniquement si aucune erreur n’a été détectée.</description></item>
    /// </list>
    /// </summary>
    public interface IS_PostImportSeriesUpdater
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Exécute la chaîne séquentielle de procédures stockées post-import pour une série donnée.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Appelé après l’import applicatif réussi (Tempor → Tempor_Import) et avant la finalisation
        /// logique de la série.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Consolider la base de données en exécutant les procédures techniques dans un ordre strict.
        /// Chaque procédure est exécutée indépendamment : en cas d’échec d’une étape, l’erreur est
        /// journalisée mais la chaîne continue afin de maximiser la consolidation.
        /// </para>
        /// <para>
        /// La procédure <c>spr_ProductionSeries_FinalizeImport</c> n’est exécutée que si toutes les
        /// étapes précédentes ont réussi. En cas d’échec d’au moins une étape, la finalisation
        /// est volontairement ignorée et un log de synthèse est produit.
        /// </para>
        /// <para>Comportement d’erreur</para>
        /// <list type="bullet">
        /// <item><description>
        /// Chaque échec est journalisé avec le numéro de série et le nom de la procédure concernée.
        /// </description></item>
        /// <item><description>
        /// Le service ne propage pas d’exception : il retourne <c>false</c> en cas d’erreur.
        /// </description></item>
        /// </list>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="serialNumberId">Identifiant métier de la série (IdSerialNumber).</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>
        /// <c>true</c> si toutes les procédures se sont exécutées avec succès et que la finalisation
        /// a été réalisée ; sinon <c>false</c>.
        /// </returns>
        /// </summary>
        Task<bool> ExecuteAsync(string caller, int serialNumberId, CancellationToken ct = default);

        #endregion
    }
}
