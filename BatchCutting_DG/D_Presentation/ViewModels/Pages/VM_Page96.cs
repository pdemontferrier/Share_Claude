using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page96 : VM_Page_Generic
    {
        private readonly IS_Notification _notification;
        private readonly IS_Navigation _navigation;
        private readonly IS_Messages _userMessages;

        public ICommand RefreshPageCommand { get; }

        // Collection des messages à afficher
        public ObservableCollection<UserAppMessage> MessagesReceived { get; set; } = new ObservableCollection<UserAppMessage>();
        public ObservableCollection<UserAppMessage> MessagesSent { get; set; } = new ObservableCollection<UserAppMessage>();

        private UserAppMessage? _selectedReceivedMessage;
        public UserAppMessage? SelectedReceivedMessage
        {
            get => _selectedReceivedMessage;
            set
            {
                _selectedReceivedMessage = value;
                OnPropertyChanged();
                if (_selectedReceivedMessage != null)
                {
                    SelectedMessage = _selectedReceivedMessage;
                    if (SelectedMessage != null && !SelectedMessage.IsRead)
                    {
                        MarkMessageAsRead(SelectedMessage.Id);
                    }
                }
            }
        }
        private UserAppMessage? _selectedSentMessage;
        public UserAppMessage? SelectedSentMessage
        {
            get => _selectedSentMessage;
            set
            {
                _selectedSentMessage = value;
                OnPropertyChanged();
                if (_selectedSentMessage != null)
                {
                    SelectedMessage = _selectedSentMessage;
                }
            }
        }

        private UserAppMessage? _selectedMessage;
        public UserAppMessage? SelectedMessage
        {
            get => _selectedMessage;
            set
            {
                _selectedMessage = value;
                OnPropertyChanged();
                SelectTabItemMessage = _selectedMessage != null;
            }
        }

        private bool _selectTabItemMessage;
        public bool SelectTabItemMessage
        {
            get => _selectTabItemMessage;
            set
            {
                _selectTabItemMessage = value;
                OnPropertyChanged();
            }
        }

        public VM_Page96(IS_Notification notification, IS_Navigation navigation, IS_Messages userMessages)
        {
            _notification = notification;
            _navigation = navigation;
            _userMessages = userMessages;

            LoadData();
            RefreshPageCommand = new UT_RelayCommandArg0(RefreshPage);
        }

        private async void LoadData()
        {
            await LoadMessagesReceivedAsync();
            await LoadMessagesSentAsync();
        }

        private async Task LoadMessagesReceivedAsync()
        {
            MessagesReceived.Clear();
            var messages = await _userMessages.GetMessagesReceivedAsync();
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    MessagesReceived.Add(message);
                }
            }
        }

        private async Task LoadMessagesSentAsync()
        {
            MessagesSent.Clear();
            var messages = await _userMessages.GetMessagesSentAsync();
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    MessagesSent.Add(message);
                }
            }
        }

        private async void MarkMessageAsRead(int messageId)
        {
            await _userMessages.MarkMessageAsReadAsync(messageId);
        }

        private void RefreshPage()
        {
            _navigation.RefreshCurrentPage();
        }
    }
}