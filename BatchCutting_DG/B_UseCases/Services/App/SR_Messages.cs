using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;

namespace BatchCutting_DG.B_UseCases.Services.App
{
    public class SR_Messages : IS_Messages
    {
        private readonly IC_UserAppMessage _chUserAppMessage;
        private readonly IQ_UserAppMessage _qhUserAppMessage;
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;

        public event Action<bool>? UnreadMessagesStatusChanged;

        public SR_Messages(IC_UserAppMessage chUserAppMessage, IQ_UserAppMessage qhUserAppMessage,
                               IS_Application application, IS_Settings settings, IS_Notification notification)
        {
            _chUserAppMessage = chUserAppMessage;
            _qhUserAppMessage = qhUserAppMessage;
            _settings = settings;
            _notification = notification;
        }


        // Obtenir la liste des messages reçu
        public async Task<List<UserAppMessage>> GetMessagesReceivedAsync()
        {
            try
            {
                // Tente d'obtenir la liste des messages reçus
                return await _qhUserAppMessage.HandleGetMessagesReceivedAsync();
            }
            catch (Exception ex)
            {
                // Gère l'exception et notifie l'utilisateur
                _notification.Error("No_EC_15", ex.Message);

                // Retourne une liste vide
                return new List<UserAppMessage>(); 
            }
        }


        // Obtenir la liste des messages envoyés
        public async Task<List<UserAppMessage>> GetMessagesSentAsync()
        {
            try
            {
                // Tente d'obtenir la liste des messages envoyés
                return await _qhUserAppMessage.HandleGetMessagesSentAsync();
            }
            catch (Exception ex)
            {
                // Gère l'exception et notifie l'utilisateur
                _notification.Error("No_EC_15", ex.Message);

                // Retourne une liste vide
                return new List<UserAppMessage>();
            }
        }


        // Mettre à jour IsRead à true pour un message spécifique
        public async Task MarkMessageAsReadAsync(int messageId)
        {
            try
            {
                await _chUserAppMessage.HandleMarkAsReadAsync(messageId, GetType().Name, nameof(MarkMessageAsReadAsync));
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_06", ex.Message);
            }
        }


        // Ajouter un nouveau message
        public async Task AddNewMessageAsync(int idAppRecepient, string subject, string content)
        {
            try
            {
                var newMessage = new UserAppMessage
                {
                    IdAppSender = _settings.GetAppID(),
                    IdUserSender = _settings.GetAppUserID(),
                    IdAppRecepient = idAppRecepient,
                    SentDate = _settings.GetAppDateTime(),
                    Subject = subject,
                    Content = content,
                    IsRead = false
                };

                await _chUserAppMessage.HandleAddAsync(newMessage, GetType().Name, nameof(AddNewMessageAsync));
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_03", ex.Message);
            }
        }


        // Commande d'écoute attaché à MainWindow
        public async Task ListenForCommandsAsync(CancellationToken cancellationToken)
        {
            // Exécuter deux tâches en parallèle
            var checkMessagesTask = CheckMessagesLoopAsync(cancellationToken);
            var notifyTask = NotifyLoopAsync(cancellationToken);

            await Task.WhenAll(checkMessagesTask, notifyTask);
        }


        // Boucle pour vérifier les messages non lus
        private async Task CheckMessagesLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_settings.GetMessageCheckDelay() * 1000, cancellationToken);

                    bool hasUnreadMessages = await GetAnyMessageNotReadAsync();

                    // Notifie l'état des messages non lus
                    UnreadMessagesStatusChanged?.Invoke(hasUnreadMessages);
                }
                catch (TaskCanceledException)
                {
                    // Tâche annulée proprement
                    break;
                }
                catch (Exception ex)
                {
                    _notification.Error("No_EC_10", ex.Message);
                }
            }
        }


        // Boucle pour afficher une notification
        private async Task NotifyLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_settings.GetMessageNotificationDelay() * 1000, cancellationToken);

                    // Affiche une notification uniquement s'il y a des messages non lus
                    if (await GetAnyMessageNotReadAsync())
                    {
                        await NotifyAsync(cancellationToken);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Tâche annulée proprement
                    break;
                }
                catch (Exception ex)
                {
                    _notification.Error("No_EC_11", ex.Message);
                }
            }
        }


        // Retourne true si un message IsRead = false
        private async Task<bool> GetAnyMessageNotReadAsync()
        {
            try
            {
                // Vérifie si un message non lu existe
                return await _qhUserAppMessage.HandleGetAnyMessageNotReadAsync();
            }
            catch (Exception ex)
            {
                // Gère l'exception et notifie l'utilisateur
                _notification.Error("No_EC_15", ex.Message);

                // Retourne false par défaut en cas d'erreur
                return false;
            }
        }


        private async Task NotifyAsync(CancellationToken cancellationToken)
        {
            // Afficher une fenêtre d'attente
            _notification.OpenDialogWindow("No_Ti_05", "No_Wa_11");

            // Attendre un interval de temps
            await Task.Delay(_settings.GetShowDialogWindowDelay() * 1000, cancellationToken);

            // Fermer DialogWindow
            _notification.CloseDialogWindow();
        }
    }
}