using System.Diagnostics;
using System.Windows;
using CommonResources.Views.Components;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.D_Presentation.Services
{
    /// <summary>
    /// <b>SR_Notification</b>
    /// <para>
    /// Service applicatif responsable de l’affichage des messages et notifications à l’utilisateur.  
    /// Il encapsule les appels aux boîtes de dialogue WPF (<see cref="MessageBox"/>) et aux fenêtres personnalisées (<see cref="DialogWindow"/>)
    /// tout en appliquant la gestion centralisée du multilingue via <see cref="IS_Dictionary"/>.
    /// </para>
    /// <para>
    /// Ce service permet d’assurer la cohérence visuelle et comportementale des messages de l’application :
    /// <list type="bullet">
    /// <item>Information, avertissement, erreur, confirmation, succès, etc.</item>
    /// <item>Ouverture/fermeture de fenêtre de dialogue non bloquante (<see cref="DialogWindow"/>)</item>
    /// <item>Gestion du focus et de la désactivation temporaire de la fenêtre principale</item>
    /// </list>
    /// </para>
    /// </summary>
    public class SR_Notification : IS_Notification
    {
        #region === Propriétés privées ===

        private DialogWindow? _dialogWindow;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_Dictionary _dictionary;
        private readonly IS_Settings_App _settingsApp;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_Notification"/>.
        /// </summary>
        /// <param name="dictionary">Service de dictionnaire multilingue.</param>
        /// <param name="settingsApp">Service d’accès aux paramètres d’application (notamment <see cref="DialogWindow"/>).</param>
        public SR_Notification(
            IS_Dictionary dictionary,                               
            IS_Settings_App settingsApp)
        {
            _dictionary = dictionary;
            _settingsApp = settingsApp;
        }

        #endregion

        #region === Messages standards ===

        /// <summary>Affiche un message d’information standard.</summary>
        public void Information(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_01"), MessageBoxButton.OK, MessageBoxImage.Information);

        /// <summary>Affiche un message de type Stop (erreur bloquante).</summary>
        public void Stop(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_02"), MessageBoxButton.OK, MessageBoxImage.Stop);

        /// <summary>Affiche un message d’erreur critique.</summary>
        public void Error(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_03"), MessageBoxButton.OK, MessageBoxImage.Error);

        /// <summary>Affiche une question à l’utilisateur, avec boutons Oui/Non.</summary>
        public void Question(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_04"), MessageBoxButton.YesNo, MessageBoxImage.Question);

        /// <summary>Affiche un message d’avertissement.</summary>
        public void Warning(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_05"), MessageBoxButton.OK, MessageBoxImage.Warning);

        /// <summary>Affiche un message de type “valeur non valide”.</summary>
        public void NotValid(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_06"), MessageBoxButton.OK, MessageBoxImage.Warning);

        /// <summary>Affiche un message de confirmation (Oui/Non).</summary>
        public void Confirmation(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_07"), MessageBoxButton.YesNo, MessageBoxImage.Question);

        /// <summary>Affiche un message de succès (opération réussie).</summary>
        public void Success(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_08"), MessageBoxButton.OK, MessageBoxImage.Exclamation);

        /// <summary>Affiche un message d’information importante.</summary>
        public void ImportantInformation(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_09"), MessageBoxButton.OK, MessageBoxImage.Warning);

        #endregion

        #region === Messages avec retour utilisateur ===

        /// <summary>
        /// Affiche une boîte de confirmation et retourne la réponse de l’utilisateur.
        /// </summary>
        /// <param name="messageKey">Clé du message à afficher (localisée via dictionnaire).</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel.</param>
        /// <returns>Résultat de la boîte de dialogue (<see cref="MessageBoxResult.Yes"/> ou <see cref="MessageBoxResult.No"/>).</returns>
        public MessageBoxResult ConfirmationReturn(string messageKey, string? additionalInfo = null)
        {
            string fullMessage = additionalInfo == null
                ? _dictionary.GetText(messageKey)
                : $"{_dictionary.GetText(messageKey)} {additionalInfo}";

            return Application.Current.Dispatcher.Invoke(() =>
                MessageBox.Show(fullMessage,
                                _dictionary.GetText("No_Ti_07"),
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question));
        }

        #endregion

        #region === DialogWindow (fenêtre non bloquante) ===

        /// <summary>
        /// Ouvre une fenêtre de notification non bloquante centrée sur la fenêtre principale.  
        /// Si une fenêtre est déjà ouverte, l’appel est ignoré.
        /// </summary>
        /// <param name="title">Clé du titre localisée via le dictionnaire.</param>
        /// <param name="content">Clé du contenu localisée via le dictionnaire.</param>
        public void OpenDialogWindow(string title, string content)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    // Ne pas ouvrir deux fois
                    if (_dialogWindow != null && _dialogWindow.IsVisible)
                        return;

                    _dialogWindow = new DialogWindow
                    {
                        Owner = Application.Current.MainWindow,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    // Définir contenu dynamique
                    _settingsApp.SetDW_Title(_dictionary.GetText(title));
                    _settingsApp.SetDW_Content(_dictionary.GetText(content));

                    // Désactiver la fenêtre principale
                    if (Application.Current.MainWindow != null)
                        Application.Current.MainWindow.IsEnabled = false;

                    _dialogWindow.Show();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SR_Notification] Erreur ouverture DialogWindow : {ex.Message}");
                }
            }));
        }

        /// <summary>
        /// Ferme la fenêtre de notification si elle est ouverte, et réactive la fenêtre principale.
        /// </summary>
        public void CloseDialogWindow()
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (_dialogWindow != null && _dialogWindow.IsVisible)
                    {
                        _dialogWindow.Close();
                        _dialogWindow = null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SR_Notification] Erreur fermeture DialogWindow : {ex.Message}");
                }
                finally
                {
                    if (Application.Current.MainWindow != null)
                        Application.Current.MainWindow.IsEnabled = true;
                }
            }));
        }

        #endregion

        #region === Méthode interne ===

        /// <summary>
        /// Affiche un message générique avec le titre, le contenu et l’icône spécifiés.
        /// </summary>
        /// <param name="messageKey">Message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel.</param>
        /// <param name="title">Titre de la boîte de dialogue.</param>
        /// <param name="button">Type de boutons affichés.</param>
        /// <param name="icon">Icône du message (Information, Avertissement, etc.).</param>
        private void ShowMessage(string messageKey, string? additionalInfo, string title, MessageBoxButton button, MessageBoxImage icon)
        {
            string fullMessage = additionalInfo == null ? messageKey : $"{messageKey} {additionalInfo}";
            Application.Current.Dispatcher.Invoke(() => MessageBox.Show(fullMessage, title, button, icon));
        }

        #endregion
    }
}