using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page96 — Page de messagerie interne entre utilisateurs et applications.
    ///
    /// <para><b>Contexte :</b> Cette page centralise l’affichage des messages reçus
    /// et envoyés par l’utilisateur au sein de l'application.</para>
    ///
    /// <para><b>Objectif :</b> Offrir un espace de consultation et de lecture
    /// des messages, structuré en trois onglets :</para>
    /// <list type="bullet">
    ///   <item><description><b>Messages reçus :</b> Liste des messages entrants, triés par date.</description></item>
    ///   <item><description><b>Messages envoyés :</b> Liste des messages sortants.</description></item>
    ///   <item><description><b>Détail du message :</b> Contenu complet du message sélectionné.</description></item>
    /// </list>
    ///
    /// <para><b>Vue associée :</b> <c>Page96.xaml</c></para>
    ///
    /// <para><b>Spécificités techniques :</b>
    /// - Utilise le service <see cref="IS_Messages"/> pour la récupération et la mise à jour des messages.  
    /// - Implémente la lecture automatique d’un message lorsqu’il est sélectionné.  
    /// - Gère les erreurs via <see cref="Ex_Classifier"/> et <see cref="IS_LogAndNotify"/>.</para>
    /// </summary>
    public class VM_Page96 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        private readonly IS_Messages _userMessages;

        #endregion

        #region === Commandes ===

        /// <summary>Commande permettant de rafraîchir la page manuellement.</summary>
        public ICommand RefreshPageCommand { get; }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la Page96 avec les services nécessaires.
        /// </summary>
        /// <param name="userMessages">Service de gestion des messages utilisateurs.</param>
        /// <param name="settings">Service de gestion des paramètres applicatifs.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue.</param>
        /// <param name="logAndNotify">Service de journalisation et notification des erreurs.</param>
        public VM_Page96(
            IS_Messages userMessages,
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            _userMessages = userMessages;
            RefreshPageCommand = new UT_RelayCommandArg0(RefreshPage);
        }

        #endregion

        #region === Propriétés liées à la vue ===

        /// <summary>Liste des messages reçus par l’utilisateur.</summary>
        public ObservableCollection<UserAppMessage> MessagesReceived { get; } = new();

        /// <summary>Liste des messages envoyés par l’utilisateur.</summary>
        public ObservableCollection<UserAppMessage> MessagesSent { get; } = new();

        private UserAppMessage? _selectedReceivedMessage;
        /// <summary>
        /// Message sélectionné dans la liste des messages reçus.
        /// Si le message n’était pas encore lu, il est marqué comme lu automatiquement.
        /// </summary>
        public UserAppMessage? SelectedReceivedMessage
        {
            get => _selectedReceivedMessage;
            set
            {
                if (SetProperty(ref _selectedReceivedMessage, value) && value != null)
                {
                    SelectedMessage = value;
                    if (!value.IsRead)
                        _ = MarkMessageAsReadAsync(value.Id);
                }
            }
        }

        private UserAppMessage? _selectedSentMessage;
        /// <summary>Message sélectionné dans la liste des messages envoyés.</summary>
        public UserAppMessage? SelectedSentMessage
        {
            get => _selectedSentMessage;
            set
            {
                if (SetProperty(ref _selectedSentMessage, value) && value != null)
                    SelectedMessage = value;
            }
        }

        private UserAppMessage? _selectedMessage;
        /// <summary>Message actuellement affiché dans l’onglet “Détail du message”.</summary>
        public UserAppMessage? SelectedMessage
        {
            get => _selectedMessage;
            set
            {
                if (SetProperty(ref _selectedMessage, value))
                    SelectTabItemMessage = value != null;
            }
        }

        private bool _selectTabItemMessage;
        /// <summary>Définit si l’onglet “Détail du message” doit être activé.</summary>
        public bool SelectTabItemMessage
        {
            get => _selectTabItemMessage;
            set => SetProperty(ref _selectTabItemMessage, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge la liste des messages reçus et envoyés par l’utilisateur.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    await LoadMessagesReceivedAsync(callChain);
                    await LoadMessagesSentAsync(callChain);
                }
                catch (Ex_Infrastructure)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            });
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Charge les messages reçus depuis le service et met à jour la collection observable.
        /// </summary>
        private async Task LoadMessagesReceivedAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadMessagesReceivedAsync)}";

            try
            {
                MessagesReceived.Clear();
                var messages = await _userMessages.GetMessagesReceivedAsync(callChain);

                if (messages != null)
                {
                    foreach (var message in messages)
                        MessagesReceived.Add(message);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Charge les messages envoyés depuis le service et met à jour la collection observable.
        /// </summary>
        private async Task LoadMessagesSentAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadMessagesSentAsync)}";

            try
            {
                MessagesSent.Clear();
                var messages = await _userMessages.GetMessagesSentAsync(callChain);

                if (messages != null)
                {
                    foreach (var message in messages)
                        MessagesSent.Add(message);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Marque le message spécifié comme lu.
        /// </summary>
        /// <param name="messageId">Identifiant du message à mettre à jour.</param>
        private async Task MarkMessageAsReadAsync(int messageId)
        {
            string callChain = BuildFirstCallChain(nameof(MarkMessageAsReadAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    await _userMessages.MarkMessageAsReadAsync(messageId, callChain);
                }
                catch (Ex_Infrastructure)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            });
        }

        /// <summary>
        /// Rafraîchit la page en rechargeant les données à partir du service de navigation.
        /// </summary>
        private void RefreshPage()
        {
            _navigation.RefreshCurrentPage();
        }

        #endregion
    }
}
