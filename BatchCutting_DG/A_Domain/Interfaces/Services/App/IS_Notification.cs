using System.Windows;

namespace BatchCutting_DG.A_Domain.Interfaces.Services.App
{
    public interface IS_Notification
    {
        void Information(string messageKey, string? additionalInfo = null);
        void Stop(string messageKey, string? additionalInfo = null);
        void Error(string messageKey, string? additionalInfo = null);
        void Question(string messageKey, string? additionalInfo = null);
        void Warning(string messageKey, string? additionalInfo = null);
        void NotValid(string messageKey, string? additionalInfo = null);
        void Confirmation(string messageKey, string? additionalInfo = null);
        void Success(string messageKey, string? additionalInfo = null);
        void ImportantInformation(string messageKey, string? additionalInfo = null);
        MessageBoxResult ConfirmationReturn(string messageKey, string? additionalInfo = null);
        void OpenDialogWindow(string title, string content);
        void CloseDialogWindow();
    }
}
