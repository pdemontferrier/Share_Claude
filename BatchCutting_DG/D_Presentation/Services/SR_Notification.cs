using System.Windows;
using CommonResources.Settings;
using CommonResources.Views.Components;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Settings;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_Notification : IS_Notification
    {
        private readonly IS_Dictionary _dictionary;

        public SR_Notification(IS_Dictionary dictionary)
        {
            _dictionary = dictionary;
        }

        private void ShowMessage(string messageKey, string? additionalInfo, string title, MessageBoxButton button, MessageBoxImage icon)
        {
            string fullMessage = additionalInfo == null ? messageKey : $"{messageKey} {additionalInfo}";
            Application.Current.Dispatcher.Invoke(() => MessageBox.Show(fullMessage, title, button, icon));
        }

        public void Information(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_01"), MessageBoxButton.OK, MessageBoxImage.Information);

        public void Stop(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_02"), MessageBoxButton.OK, MessageBoxImage.Stop);

        public void Error(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_03"), MessageBoxButton.OK, MessageBoxImage.Error);

        public void Question(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_04"), MessageBoxButton.YesNo, MessageBoxImage.Question);

        public void Warning(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_05"), MessageBoxButton.OK, MessageBoxImage.Warning);

        public void NotValid(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_06"), MessageBoxButton.OK, MessageBoxImage.Warning);

        public void Confirmation(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_07"), MessageBoxButton.YesNo, MessageBoxImage.Question);

        public void Success(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_08"), MessageBoxButton.OK, MessageBoxImage.Exclamation);

        public void ImportantInformation(string messageKey, string? additionalInfo = null) =>
            ShowMessage(_dictionary.GetText(messageKey), additionalInfo, _dictionary.GetText("No_Ti_09"), MessageBoxButton.OK, MessageBoxImage.Warning);

        public MessageBoxResult ConfirmationReturn(string messageKey, string? additionalInfo = null)
        {
            string fullMessage = additionalInfo == null ? _dictionary.GetText(messageKey) : $"{_dictionary.GetText(messageKey)} {additionalInfo}";
            return Application.Current.Dispatcher.Invoke(() => MessageBox.Show(fullMessage, _dictionary.GetText("No_Ti_07"), MessageBoxButton.YesNo, MessageBoxImage.Question));
        }

        private DialogWindow? _dialogWindow;

        public void OpenDialogWindow(string title, string content)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _dialogWindow = new DialogWindow
                {
                    Owner = Application.Current.MainWindow // Définit la fenêtre principale comme propriétaire
                };

                CR_CommonSettings.DW_Title = _dictionary.GetText(title);
                CR_CommonSettings.DW_Content = _dictionary.GetText(content);

                // Positionner la fenêtre de dialogue
                _dialogWindow.Left = (SE_Window.MainWindowWidth - CR_CommonSettings.DW_Width) / 2 + SE_Window.MainWindowLeft;
                _dialogWindow.Top = (SE_Window.MainWindowHeight - CR_CommonSettings.DW_Height) / 2 + SE_Window.MainWindowTop;
                _dialogWindow.Show();
            });
        }

        public void CloseDialogWindow()
        {
            Application.Current.Dispatcher.Invoke(() => _dialogWindow.Close());
        }
    }
}