using System.Windows;
using System.Diagnostics;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.D_Presentation.Services
{
    /// <summary>
    /// <b>Service de fermeture applicative centralisée.</b>
    /// <para>
    /// Ce service gère tous les scénarios de fermeture de l’application :
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Fermeture immédiate (ForceShutdown).</description></item>
    ///   <item><description>Fermeture standard (Shutdown).</description></item>
    ///   <item><description>Fermeture différée avec message d’avertissement (ShutdownWithDelayAsync).</description></item>
    /// </list>
    /// <para>
    /// Tous les appels au thread UI utilisent <c>Dispatcher.BeginInvoke</c> afin d’éviter
    /// tout blocage du thread appelant, même si le service est déclenché depuis un thread d’arrière-plan.
    /// </para>
    /// </summary>
    public class SR_Shutdown : IS_Shutdown
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom interne du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_Settings_User _settingsUser;
        private readonly IS_Notification _notification;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service de fermeture applicative.
        /// </summary>
        /// <param name="settingsUser">Service de gestion des paramètres utilisateurs.</param>
        /// <param name="notification">Service d'affichage des notifications visuelles.</param>
        public SR_Shutdown(
            IS_Settings_User settingsUser,
            IS_Notification notification)
        {
            _callee = GetType().Name;
            _settingsUser = settingsUser;
            _notification = notification;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Ferme immédiatement l’application après affichage éventuel d’un message d’avertissement.
        /// </summary>
        /// <param name="warningText">Texte d’avertissement à afficher avant fermeture. Peut être nul.</param>
        /// <param name="caller">Chaîne d’appel utilisée pour la traçabilité (<c>CallChain</c>).</param>
        /// <remarks>
        /// Cette méthode marque l’état utilisateur comme "fermeture forcée"
        /// afin de signaler aux autres composants (ex. journalisation, session) qu’une fermeture exceptionnelle est en cours.
        /// </remarks>
        public void ForceShutdown(string caller, string? warningText = null)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ForceShutdown)}";

            try
            {
                _settingsUser.SetForceClose(true);

                // Affiche un message warning si demandé, sans bloquer
                if (!string.IsNullOrWhiteSpace(warningText))
                {
                    Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            _notification.Warning(warningText);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[{callChain}] Warning display error: {ex.Message}");
                        }
                    }));
                }

                // Ferme la fenêtre principale
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        Application.Current.MainWindow?.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[{callChain}] ForceShutdown Close error: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Ferme normalement l’application sans avertissement.
        /// </summary>
        /// <remarks>
        /// Cette méthode est utilisée lors d’une fermeture standard (ex. clic sur le bouton "Quitter").
        /// </remarks>
        /// <param name="caller">Chaîne d’appel utilisée pour la traçabilité (<c>CallChain</c>).</param>
        public void Shutdown(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Shutdown)}";

            try
            {
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        Application.Current.MainWindow?.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[{callChain}] Shutdown Close error: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Ferme l’application après un court délai, en affichant un message d’avertissement.
        /// </summary>
        /// <param name="delaySeconds">Durée (en secondes) avant fermeture.</param>
        /// <param name="titleKey">Clé du titre du message dans le dictionnaire multilingue.</param>
        /// <param name="contentKey">Clé du message de contenu dans le dictionnaire multilingue.</param>
        /// <param name="caller">Chaîne d’appel utilisée pour la traçabilité (<c>CallChain</c>).</param>
        public async Task ShutdownWithDelayAsync(int delaySeconds, string titleKey, string contentKey, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ShutdownWithDelayAsync)}";

            try
            {
                _settingsUser.SetForceClose(true);

                // Ouvre la boîte de dialogue dans le thread UI sans bloquer
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        _notification.OpenDialogWindow(titleKey, contentKey);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[{callChain}] OpenDialogWindow error: {ex.Message}");
                    }
                }));

                // Attendre le délai de fermeture
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

                // Fermer la boîte de dialogue et l'application dans le thread UI sans bloquer
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        _notification.CloseDialogWindow();
                        Application.Current.MainWindow?.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[{callChain}] CloseDialogWindow error: {ex.Message}");
                    }
                }));
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion
    }
}