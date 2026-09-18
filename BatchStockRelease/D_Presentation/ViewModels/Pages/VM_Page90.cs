using System.Collections.ObjectModel;
using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.App.Entities;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page90 — Page d’affichage des informations relatives à l’utilisateur connecté.
    ///
    /// <para><b>Contexte :</b> Cette page affiche les informations personnelles
    /// de l’utilisateur ainsi que ses droits d’accès aux différentes pages
    /// de l’application BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Fournir une vue centralisée sur les données
    /// de l’utilisateur connecté :</para>
    /// <list type="bullet">
    ///   <item><description><b>Onglet 1 :</b> Informations personnelles (nom, rôle, adresse e-mail, date de dernière connexion).</description></item>
    ///   <item><description><b>Onglet 2 :</b> Droits d’accès (lecture, écriture, suppression) pour chaque page de l’application.</description></item>
    /// </list>
    ///
    /// <para><b>Vue associée :</b> <c>Page90.xaml</c></para>
    ///
    /// <para><b>Spécificités techniques :</b>
    /// - Récupère les informations de l’utilisateur courant via <see cref="IQ_User"/>.  
    /// - Charge les droits d’accès applicatifs à partir de <see cref="IQ_UserAppPageDroit"/>.  
    /// - Affiche les données contextuelles du poste utilisateur issues de <see cref="IQ_AppContext"/>.  
    /// - Gère les erreurs via <see cref="Ex_Classifier"/> et la journalisation via <see cref="IS_LogAndNotify"/>.</para>
    /// </summary>
    public class VM_Page90 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        private readonly IQ_User _qhUser;
        private readonly IQ_UserAppPageDroit _qhUserPageRights;
        private readonly IQ_AppContext _qhAppContext;
        private readonly DTO_AppContext _appCtx;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la Page90 avec les services nécessaires.
        /// </summary>
        public VM_Page90(
            IQ_User qhUser,
            IQ_UserAppPageDroit qhUserPageRights,
            IQ_AppContext qhAppContext,
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            _qhUser = qhUser;
            _qhUserPageRights = qhUserPageRights;
            _qhAppContext = qhAppContext;

            _appCtx = _qhAppContext.GetAppContext();
        }

        #endregion

        #region === Propriétés liées à la vue ===

        /// <summary>
        /// Liste des droits d’accès de l’utilisateur par page.
        /// </summary>
        public ObservableCollection<UserAppPageDroit> PagesUserRightsList { get; private set; }
            = new ObservableCollection<UserAppPageDroit>();

        /// <summary>
        /// Liste complète des droits d’accès regroupés par nom de page.
        /// </summary>
        public ObservableCollection<KeyValuePair<string, PageRights>> AllPagesUserRightsList { get; private set; }
            = new ObservableCollection<KeyValuePair<string, PageRights>>();

        private User _currentUser = new();
        /// <summary>
        /// Informations personnelles de l’utilisateur connecté.
        /// </summary>
        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        private string? _deviceID = string.Empty;
        /// <summary>
        /// Identifiant du poste sur lequel l’utilisateur est connecté.
        /// </summary>
        public string? DeviceID
        {
            get => _deviceID;
            set => SetProperty(ref _deviceID, value);
        }

        private string? _deviceIP = string.Empty;
        /// <summary>
        /// Adresse IP du poste utilisateur.
        /// </summary>
        public string? DeviceIP
        {
            get => _deviceIP;
            set => SetProperty(ref _deviceIP, value);
        }

        private string? _deviceUser = string.Empty;
        /// <summary>
        /// Nom de l’utilisateur Windows ou du terminal connecté.
        /// </summary>
        public string? DeviceUser
        {
            get => _deviceUser;
            set => SetProperty(ref _deviceUser, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge les informations de l’utilisateur connecté ainsi que ses droits d’accès.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    await LoadUserDataAsync(callChain);
                    await LoadUserAccessRightsAsync(callChain);
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
        /// Charge les informations personnelles et techniques de l’utilisateur connecté.
        /// </summary>
        private async Task LoadUserDataAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadUserDataAsync)}";

            try
            {
                var user = await _qhUser.HandleGetByIdAsync(_appCtx.AppUserId);

                CurrentUser = user ?? new User
                {
                    Id = 0,
                    Nom = "Non trouvé",
                    Prenom = "Non trouvé",
                    Login = "Non trouvé",
                    Initial = "N/A",
                    TelPro = "N/A",
                    TelFixePro = "N/A",
                    MailPro = "N/A"
                };

                DeviceUser = _appCtx.AppDeviceUser;
                DeviceID = _appCtx.AppDeviceId;
                DeviceIP = _appCtx.AppDeviceIP;
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Charge la liste des droits d’accès de l’utilisateur pour l’application courante.
        /// </summary>
        private async Task LoadUserAccessRightsAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadUserAccessRightsAsync)}";

            try
            {
                var userId = _appCtx.AppUserId;
                var appId = _appCtx.AppId;

                var userRights = await _qhUserPageRights.HandleGetByUserIdAppIdAsync(userId, appId);

                PagesUserRightsList.Clear();
                foreach (var droit in userRights)
                    PagesUserRightsList.Add(droit);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion
    }
}