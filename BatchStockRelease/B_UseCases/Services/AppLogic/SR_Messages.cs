using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.B_UseCases.Settings.AppLogic;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// Service métier responsable de la gestion et de la supervision des messages applicatifs.
    /// <para>
    /// Il permet de récupérer, envoyer et marquer les messages comme lus, tout en assurant
    /// une surveillance continue de la présence de messages non lus pour l’utilisateur connecté.
    /// </para>
    /// </summary>
    public class SR_Messages : IS_Messages
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom interne du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IC_UserAppMessage _chUserAppMessage;
        private readonly IQ_UserAppMessage _qhUserAppMessage;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_Notification _notification;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_Messages"/>.
        /// </summary>
        public SR_Messages(
            IC_UserAppMessage chUserAppMessage,
            IQ_UserAppMessage qhUserAppMessage,
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser,
            IS_Notification notification)
        {
            _callee = GetType().Name;

            _chUserAppMessage = chUserAppMessage;
            _qhUserAppMessage = qhUserAppMessage;
            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
            _notification = notification;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Récupère la liste des messages reçus par l’utilisateur connecté.
        /// </summary>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        public async Task<List<UserAppMessage>> GetMessagesReceivedAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetMessagesReceivedAsync)}";

            try
            {
                // Tente d'obtenir la liste des messages reçus
                return await _qhUserAppMessage.HandleGetMessagesReceivedAsync();
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Récupère la liste des messages envoyés par l’utilisateur connecté.
        /// </summary>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        public async Task<List<UserAppMessage>> GetMessagesSentAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetMessagesSentAsync)}";

            try
            {
                // Tente d'obtenir la liste des messages envoyés
                return await _qhUserAppMessage.HandleGetMessagesSentAsync();
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Marque un message comme lu.
        /// </summary>
        /// <param name="messageId">Identifiant du message à mettre à jour.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        public async Task MarkMessageAsReadAsync(int messageId, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(MarkMessageAsReadAsync)}";

            try
            {
                // Mettre à jour IsRead à true pour un message spécifique
                await _chUserAppMessage.HandleMarkAsReadAsync(messageId, callChain);

                // Remplacer par :
                // le Service métier orchestre QH.HandleGetByIdAsync(messageId) → entity.IsRead = true → CH_Generic.HandleUpdateAsync(caller, entity, ct).
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Ajoute un nouveau message dans la table <c>user_app_message</c>.
        /// </summary>
        public async Task AddNewMessageAsync(int idAppRecepient, string subject, string content, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(AddNewMessageAsync)}";

            try
            {
                var newMessage = new UserAppMessage
                {
                    IdAppSender = _settingsApp.GetAppId(),
                    IdUserSender = _settingsUser.GetAppUserId(),
                    IdAppRecepient = idAppRecepient,
                    SentDate = _settingsApp.GetAppDateTime(),
                    Subject = subject,
                    Content = content,
                    IsRead = false
                };

                await _chUserAppMessage.HandleAddAsync(newMessage, callChain);
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Démarre la surveillance asynchrone des messages et des notifications.
        /// </summary>
        public async Task ListenForCommandsAsync(CancellationToken cancellationToken, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ListenForCommandsAsync)}";

            try
            {
                var checkTask = CheckMessagesLoopAsync(cancellationToken, callChain);
                var notifyTask = NotifyLoopAsync(cancellationToken, callChain);

                await Task.WhenAll(checkTask, notifyTask);
            }
            catch (TaskCanceledException)
            {
                // Arrêt propre, aucune action
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Boucle continue pour vérifier la présence de messages non lus.
        /// </summary>
        private async Task CheckMessagesLoopAsync(CancellationToken cancellationToken, string caller)
        {
            string callChain = $"{caller} > {nameof(CheckMessagesLoopAsync)}";

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    bool hasUnread = await _qhUserAppMessage.HandleGetAnyMessageNotReadAsync();
                    _settingsApp.SetHasUnreadMessages(hasUnread);

                    // Met également à jour SE_App pour la couche UI
                    SE_App.HasUnreadMessages = hasUnread;

                    await Task.Delay(TimeSpan.FromSeconds(_settingsApp.GetMessageCheckDelay()), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break; // fermeture propre
                }
                catch (Exception ex)
                {
                    // Classification centralisée des exceptions
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            }
        }

        /// <summary>
        /// Boucle continue affichant une notification périodique
        /// si des messages non lus sont détectés.
        /// </summary>
        private async Task NotifyLoopAsync(CancellationToken cancellationToken, string caller)
        {
            string callChain = $"{caller} > {nameof(NotifyLoopAsync)}";

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_settingsApp.GetMessageNotificationDelay()), cancellationToken);

                    // Affiche une notification uniquement s'il y a des messages non lus
                    if (await GetAnyMessageNotReadAsync(callChain))
                        await NotifyAsync(cancellationToken, callChain);
                }
                catch (TaskCanceledException)
                {
                    // Tâche annulée proprement
                    break;
                }
                catch (Exception ex)
                {
                    // Classification centralisée des exceptions
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            }
        }

        /// <summary>
        /// Vérifie si des messages non lus existent.
        /// </summary>
        private async Task<bool> GetAnyMessageNotReadAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(GetAnyMessageNotReadAsync)}";

            try
            {
                // Vérifie si un message non lu existe
                return await _qhUserAppMessage.HandleGetAnyMessageNotReadAsync();
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Affiche une notification visuelle pour signaler la présence de nouveaux messages.
        /// </summary>
        private async Task NotifyAsync(CancellationToken cancellationToken, string caller)
        {
            string callChain = $"{caller} > {nameof(NotifyAsync)}";

            try
            {
                // Afficher une fenêtre d'attente
                _notification.OpenDialogWindow("No_Ti_09", "No_Wa_11");

                // Attendre un interval de temps
                await Task.Delay(TimeSpan.FromSeconds(_settingsApp.GetShowDialogWindowDelay()), cancellationToken);

                // Fermer DialogWindow
                _notification.CloseDialogWindow();
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion
    }
}