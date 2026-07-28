using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.Services.Business;
using LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure;

namespace LeitTemporImport.B_UseCases.Services.Business
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
    public class SR_PostImportSeriesUpdater : IS_PostImportSeriesUpdater
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_StoredProcedure _storedProcedure;
        private readonly IS_ErrorLogger _errorLog;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service de mise à jour post-import.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires à l’exécution des procédures et à la journalisation.</para>
        /// </summary>
        public SR_PostImportSeriesUpdater(
            IS_StoredProcedure storedProcedure,
            IS_ErrorLogger errorLog)
        {
            _callee = GetType().Name;

            _storedProcedure = storedProcedure ?? throw new ArgumentNullException(nameof(storedProcedure));
            _errorLog = errorLog ?? throw new ArgumentNullException(nameof(errorLog));
        }

        #endregion

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
        public async Task<bool> ExecuteAsync(string caller, int serialNumberId, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            var failedProcedures = new List<string>();

            try
            {
                ct.ThrowIfCancellationRequested();

                if (serialNumberId <= 0)
                    throw new ArgumentOutOfRangeException(nameof(serialNumberId), "serialNumberId must be > 0.");

                IReadOnlyList<string> procedures = new[]
                {
                    "dbo.spr_ArticleReference_InsertFromSource",
                    "dbo.spr_ColorRalFinish_InsertFromSource",
                    "dbo.spr_ArticleInternal_InsertFromSource",
                    "dbo.spr_SpatialPosition_InsertFromSource",
                    "dbo.spr_CustomerOrder_InsertFromSource",
                    "dbo.spr_ProductionChassis_InsertFromSource",
                    "dbo.spr_ProductionFrameSash_InsertFromSource",
                    "dbo.spr_ProductionCutPiece_InsertFromSource"
                };

                foreach (var proc in procedures)
                {
                    ct.ThrowIfCancellationRequested();

                    bool ok = await ExecuteStepAsync(callChain, proc, serialNumberId, ct);
                    if (!ok) failedProcedures.Add(proc);
                }

                // Finalize UNIQUEMENT si tout est OK
                if (failedProcedures.Count == 0)
                {
                    bool finalizeOk = await ExecuteStepArg1IntAsync(
                        callChain,
                        "dbo.spr_ProductionSeries_FinalizeImport",
                        serialNumberId,
                        serialNumberId,
                        ct);

                    if (!finalizeOk)
                    {
                        // Finalize en échec => on log et on retourne false
                        await _errorLog.ExecuteAsync(
                            $"{callChain} > SerieNr={serialNumberId} > Finalize failed",
                            new Ex_Infrastructure("Post-import finalize failed. Series not marked as finalized."),
                            ct);

                        return false;
                    }

                    return true;
                }

                // Il y a eu des erreurs : on log une synthèse et on ne finalize pas
                string failed = string.Join(", ", failedProcedures);

                await _errorLog.ExecuteAsync(
                    $"{callChain} > SerieNr={serialNumberId} > Post-import incomplete. Finalize skipped. Failed=[{failed}]",
                    new Ex_Infrastructure("Post-import procedures failed; finalize skipped."),
                    ct);

                return false;
            }
            catch (Exception ex)
            {
                await _errorLog.ExecuteAsync($"{callChain} > SerieNr={serialNumberId}", ex, ct);
                return false;
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée (sans paramètre) et gère l’arrêt de chaîne en cas d’erreur.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée pour chaîner les procédures post-import.</para>
        /// <para>Objectif</para>
        /// <para>Encapsuler l’exécution d’une étape et journaliser en cas d’échec.</para>
        /// </summary>
        private async Task<bool> ExecuteStepArg1IntAsync(string caller, string procedureName, int arg1, int serialNumberId, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteStepArg1IntAsync)}";

            try
            {
                await _storedProcedure.ExecuteArg1IntAsync(callChain, procedureName, arg1, ct);
                return true;
            }
            catch (Exception ex)
            {
                await _errorLog.ExecuteAsync(
                    $"{callChain} > SerieNr={serialNumberId} > Proc='{procedureName}' > Arg1={arg1}",
                    ex,
                    ct);

                return false;
            }
        }

        private async Task<bool> ExecuteStepAsync(string caller, string procedureName, int serialNumberId, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteStepAsync)}";

            try
            {
                await _storedProcedure.ExecuteAsync(callChain, procedureName, ct);
                return true;
            }
            catch (Exception ex)
            {
                await _errorLog.ExecuteAsync(
                    $"{callChain} > SerieNr={serialNumberId} > Proc='{procedureName}'",
                    ex,
                    ct);

                return false;
            }
        }

        #endregion
    }
}
