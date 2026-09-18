using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic;
using BatchStockRelease.C_Infrastructure.Services;

namespace BatchStockRelease.B_UseCases.UseCases.AppLogic
{
    /// <summary>
    /// <b>UseCase : Démarrage et supervision du monitoring base de données.</b>
    /// <para>
    /// Ce UseCase orchestre le lancement du service <see cref="SR_DB_Monitoring"/> et
    /// gère les événements de perte et restauration de connexion via <see cref="BatchStockRelease.B_UseCases.Settings.AppLogic.SE_App"/>.
    /// Il inclut également la gestion visuelle des modales d’attente (via <see cref="IS_Notification"/>)
    /// et la fermeture contrôlée de l’application en cas d’échec de reconnexion.
    /// </para>
    /// </summary>
    public class UC_DB_Monitoring : IU_DB_Monitoring
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom interne du UseCase pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_DB_Monitoring _dbMonitoring;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Shutdown _shutdown;
        private readonly IS_Notification _notification;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du UseCase <see cref="UC_DB_Monitoring"/>.
        /// </summary>
        public UC_DB_Monitoring(
            IS_DB_Monitoring dbMonitoring,
            IS_Settings_App settingsApp,
            IS_LogAndNotify logAndNotify,
            IS_Shutdown shutdown,
            IS_Notification notification)
        {
            _callee = GetType().Name;
            _dbMonitoring = dbMonitoring;
            _settingsApp = settingsApp;
            _logAndNotify = logAndNotify;
            _shutdown = shutdown;
            _notification = notification;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Lance le monitoring de la base de données et initialise la gestion des événements
        /// de perte et restauration de connexion.
        /// </summary>
        /// <param name="caller">Nom du composant appelant pour la traçabilité.</param>
        public async Task ExecuteAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Démarrage du monitoring
                await _dbMonitoring.ExecuteAsync(callChain);

                // Abonnement aux événements globaux de SE_App
                _settingsApp.ConnectionLost += async (s, e) => await HandleDisconnectionAsync(callChain);
                _settingsApp.ConnectionRestored += async (s, e) => await HandleReconnectionAsync(callChain);
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Gère la perte de connexion à la base de données.
        /// Lance successivement deux cycles d’observation pour tenter une reconnexion.
        /// </summary>
        private async Task HandleDisconnectionAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(HandleDisconnectionAsync)}";

            try
            {
                // --- Ouverture modale d’attente ---
                if (!_settingsApp.GetIsDialogOpen())
                {
                    _settingsApp.SetIsDialogOpen(true);
                    _notification.OpenDialogWindow("No_Ti_05", "No_Wa_15");
                }

                // --- Cycle 1 : rapide ---
                bool cycle1Ok = await RunObservationCycleAsync(
                    _settingsApp.GetDatabaseCheckFirstLoop(),
                    _settingsApp.GetDatabaseCheckFirstLoop(),
                    callChain);

                if (cycle1Ok)
                {
                    await HandleReconnectionAsync(callChain);
                    return;
                }

                // --- Cycle 2 : lent ---
                bool cycle2Ok = await RunObservationCycleAsync(
                    _settingsApp.GetDatabaseCheckSecondLoop(),
                    _settingsApp.GetDatabaseCheckSecondLoop(),
                    callChain);

                if (cycle2Ok)
                {
                    await HandleReconnectionAsync(callChain);
                    return;
                }

                // --- Échec complet ---
                await _logAndNotify.ExecuteAsync("No_EC_19", new Ex_Infrastructure(callChain, "DBMS_01", "No_Wa_16"), false);

                _notification.CloseDialogWindow();
                _settingsApp.SetIsDialogOpen(false);

                await _shutdown.ShutdownWithDelayAsync(5, "No_Ti_05", "No_Wa_16", callChain);
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Gère la restauration de la connexion à la base de données.
        /// Ferme la modale d’attente et notifie l’utilisateur.
        /// </summary>
        private async Task HandleReconnectionAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(HandleReconnectionAsync)}";

            try
            {
                if (_settingsApp.GetIsDialogOpen())
                {
                    _notification.CloseDialogWindow();
                    _settingsApp.SetIsDialogOpen(false);
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Exécute un cycle de reconnexion comprenant :
        /// 1. Une phase d’attente passive.
        /// 2. Une phase d’observation active (tests consécutifs).
        /// </summary>
        private async Task<bool> RunObservationCycleAsync(int waitDelay, int observeDelay, string caller)
        {
            string callChain = $"{caller} > {nameof(RunObservationCycleAsync)}";

            await Task.Delay(TimeSpan.FromSeconds(waitDelay));

            DateTime start = DateTime.Now;
            int requiredStableChecks = 2;
            int stableCount = 0;

            while ((DateTime.Now - start).TotalSeconds < observeDelay)
            {
                if (_settingsApp.GetIsConnected())
                {
                    stableCount++;
                    if (stableCount >= requiredStableChecks)
                        return true;
                }
                else
                {
                    stableCount = 0;
                }

                await Task.Delay(TimeSpan.FromSeconds(_settingsApp.GetDatabaseCheckInterval()));
            }

            return false;
        }

        #endregion
    }
}