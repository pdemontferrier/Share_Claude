using System.Windows.Input;
using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page32 — Page de sortie de stock des barres neuves.
    ///
    /// <para><b>Contexte :</b> Cette page permet de préparer la sortie des barres
    /// neuves à approvisionner pour un lot donné. L’ordre de traitement suit
    /// la logique de déplacement optimisée dans les zones de stockage.</para>
    ///
    /// <para><b>Objectif :</b> Afficher les barres neuves à sortir, leur profil,
    /// et permettre l’ajustement visuel des quantités avant validation.</para>
    ///
    /// <para><b>Vue associée :</b> <c>Page32.xaml</c></para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>BarNewListView :</b> Liste des barres neuves à sortir.</description></item>
    ///   <item><description><b>ImageProfil :</b> Représentation visuelle du profil de chaque barre.</description></item>
    ///   <item><description><b>Plus/Minus :</b> Ajustement de la quantité sortie.</description></item>
    /// </list>
    ///
    /// <para><b>Spécificités techniques :</b>
    /// - Récupère les barres à approvisionner pour le lot courant.  
    /// - Met à jour les informations d’approvisionnement dans la table <c>DecoupeBarre</c>.  
    /// - Gère les exceptions via <see cref="Ex_Classifier"/> et la notification via <see cref="IS_LogAndNotify"/>.</para>
    /// </summary>
    public class VM_Page32 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        private readonly IQ_DecoupeBarreWithBarNewToRelease _qhDecoupeBarreDetails;
        private readonly IQ_AppContext _appContext;
        private readonly IS_DecoupeBarre_UpdateApproStarted _decoupeBarre_UpdateApproStarted;
        private readonly IS_Utilities _vueSettings;

        #endregion

        #region === Commandes ===

        public ICommand IncrementQuantityCommand { get; }
        public ICommand DecrementQuantityCommand { get; }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la page 32 avec les services nécessaires.
        /// </summary>
        public VM_Page32(
            IQ_DecoupeBarreWithBarNewToRelease qhDecoupeBarreDetails,
            IQ_AppContext appContext,
            IS_DecoupeBarre_UpdateApproStarted decoupeBarre_UpdateApproStarted,
            IS_Utilities vueSettings,
            IS_Settings_UseCase settingsUseCase,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settingsUseCase, navigation, dictionary, logAndNotify)
        {
            _qhDecoupeBarreDetails = qhDecoupeBarreDetails;
            _appContext = appContext;
            _decoupeBarre_UpdateApproStarted = decoupeBarre_UpdateApproStarted;
            _vueSettings = vueSettings;

            IncrementQuantityCommand = new UT_RelayCommandArg0(IncrementQuantity);
            DecrementQuantityCommand = new UT_RelayCommandArg0(DecrementQuantity, () => Quantite > 0);
        }

        #endregion

        #region === Propriétés liées à la vue ===

        private DTO_DecoupeBarreWithBarNewToRelease? _decoupeBarreDetails;
        /// <summary>
        /// Détails de la première barre neuve à sortir.
        /// </summary>
        public DTO_DecoupeBarreWithBarNewToRelease? DecoupeBarreDetails
        {
            get => _decoupeBarreDetails;
            set
            {
                if (SetProperty(ref _decoupeBarreDetails, value) && value != null)
                    _settingsUseCase.SetDecoupeBarreIdStock(value.IdStock);
            }
        }

        private int _quantite;
        /// <summary>
        /// Quantité à sortir pour la barre sélectionnée.
        /// </summary>
        public int Quantite
        {
            get => _quantite;
            set
            {
                if (SetProperty(ref _quantite, value))
                    _settingsUseCase.SetDecoupeBarreStockQuantite(value);
            }
        }

        private int _quantiteMax;
        /// <summary>
        /// Quantité maximale de barres disponibles à sortir pour la référence courante.
        /// </summary>
        public int QuantiteMax
        {
            get => _quantiteMax;
            set => SetProperty(ref _quantiteMax, value);
        }

        private int _releaseNumber;
        /// <summary>
        /// Nombre total de barres à approvisionner pour le lot.
        /// </summary>
        public int ReleaseNumber
        {
            get => _releaseNumber;
            set => SetProperty(ref _releaseNumber, value);
        }

        private Uri? _referenceVueUri;
        /// <summary>
        /// Image du profil de la barre sélectionnée.
        /// </summary>
        public Uri? ReferenceVueUri
        {
            get => _referenceVueUri;
            set => SetProperty(ref _referenceVueUri, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge les barres neuves à approvisionner pour le lot sélectionné.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    var appCtx = _appContext.GetAppContext();

                    var barNewList = await GetBarNewListAsync();
                    if (!barNewList.Any())
                    {
                        _navigation.NavigateToNewPage("Page10");
                        return;
                    }

                    var barNewListToRelease = await InitializeBarViewAsync(barNewList);
                    if (!barNewListToRelease.Any())
                    {
                        _navigation.NavigateToNewPage("Page10");
                        return;
                    }

                    await UpdateApproStatusForBarsAsync(barNewListToRelease, appCtx, callChain);
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
        /// Récupère la liste complète des barres neuves à sortir pour le lot courant.
        /// </summary>
        /// <returns>Liste des barres neuves à approvisionner.</returns>
        private async Task<List<DTO_DecoupeBarreWithBarNewToRelease>> GetBarNewListAsync()
        {
            int lotId = _settingsUseCase.GetDecoupeLotId();
            var barNewList = await _qhDecoupeBarreDetails.HandleGetAsync(lotId);
            return barNewList ?? new List<DTO_DecoupeBarreWithBarNewToRelease>();
        }

        /// <summary>
        /// Initialise la première barre à afficher et prépare la liste des barres à traiter.
        /// </summary>
        /// <param name="barNewList">Liste complète des barres neuves récupérées.</param>
        /// <returns>Liste filtrée des barres à libérer (même IdStock que la première).</returns>
        private Task<List<DTO_DecoupeBarreWithBarNewToRelease>> InitializeBarViewAsync(
            List<DTO_DecoupeBarreWithBarNewToRelease> barNewList)
        {
            // Sélection et initialisation de la première barre
            DecoupeBarreDetails = barNewList.First();
            ReleaseNumber = barNewList.Count();
            ReferenceVueUri = _vueSettings.GetVueUri(DecoupeBarreDetails.Reference);

            // Filtrer les barres appartenant au même IdStock
            var barNewListToRelease = barNewList
                .Where(x => x.IdStock == DecoupeBarreDetails.IdStock)
                .ToList();

            // Mise à jour des quantités à afficher
            if (barNewListToRelease.Any())
            {
                QuantiteMax = barNewListToRelease.Count;
                Quantite = QuantiteMax;
            }

            // Retour synchrone compatible avec await
            return Task.FromResult(barNewListToRelease);
        }


        /// <summary>
        /// Met à jour l’état d’approvisionnement pour chaque barre neuve de la liste.
        /// </summary>
        /// <param name="barNewListToRelease">Liste des barres à marquer comme “approvisionnement démarré”.</param>
        /// <param name="appCtx">Contexte applicatif courant.</param>
        /// <param name="callChain">Chaîne de traçabilité de l'appelant.</param>
        private async Task UpdateApproStatusForBarsAsync(
            List<DTO_DecoupeBarreWithBarNewToRelease> barNewListToRelease,
            DTO_AppContext appCtx,
            string callChain)
        {
            foreach (var barNew in barNewListToRelease)
            {
                _settingsUseCase.SetDecoupeBarreId(barNew.Id);
                await _decoupeBarre_UpdateApproStarted.ExecuteAsync(barNew.Id, appCtx, callChain);
            }
        }


        private void IncrementQuantity()
        {
            if (Quantite < QuantiteMax)
            {
                Quantite++;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void DecrementQuantity()
        {
            if (Quantite > 0)
            {
                Quantite--;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion
    }
}



/*
using System.Windows.Input;
using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// VM_Page32 — ViewModel pour la page de sortie des barres neuves.
    /// 
    /// <para><b>Contexte :</b> Cette page affiche les barres neuves à sortir du stock
    /// dans un ordre optimisé, suivant la disposition des zones de stockage.
    /// Elle permet à l’opérateur de valider la sortie des barres préparées.</para>
    /// 
    /// <para><b>Objectif :</b> Fournir les données nécessaires à l’interface
    /// pour permettre la visualisation et la validation des barres à sortir.</para>
    /// 
    /// <list type="bullet">
    ///   <item><description>Afficher les barres à approvisionner par référence.</description></item>
    ///   <item><description>Présenter l’image du profil pour aider à l’identification.</description></item>
    ///   <item><description>Gérer les actions utilisateur (rafraîchissement, validation).</description></item>
    /// </list>
    /// 
    /// <para><b>Vue associée :</b> <see cref="BatchStockRelease.D_Presentation.Views.Pages.Page32"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="VM_Page_Generic"/> pour bénéficier
    /// des services partagés (navigation, paramètres, dictionnaire multilingue).</para>
    /// </summary>
    public class VM_Page32 : VM_Page_Generic
    {
        private readonly string _callee;
        private DTO_DecoupeBarreWithBarNewToRelease? _decoupeBarreDetails;
        private int _quantite;
        private int _quantiteMax;
        private int _releaseNumber;
        private Uri? _referenceVueUri;
        private readonly IQ_DecoupeBarreWithBarNewToRelease _qhDecoupeBarreDetails;
        private readonly IQ_AppContext _appContextProvider;
        private readonly IS_DecoupeBarre_UpdateApproStarted _decoupeBarre_UpdateApproStarted;
        private readonly IS_ReferenceVue _vueSettings;

        public ICommand IncrementQuantityCommand { get; }
        public ICommand DecrementQuantityCommand { get; }

        /// <summary>
        /// Initialise le ViewModel de la page 32 avec les dépendances requises.
        /// </summary>
        public VM_Page32(
            IQ_DecoupeBarreWithBarNewToRelease qhDecoupeBarreDetails,
            IQ_AppContext appContextProvider,
            IS_DecoupeBarre_UpdateApproStarted decoupeBarre_UpdateApproStarted,
            IS_ReferenceVue vueSettings,
            IS_Settings_UseCase settingsUseCase,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settingsUseCase, navigation, dictionary, logAndNotify)
        {
            _callee = GetType().Name;

            _qhDecoupeBarreDetails = qhDecoupeBarreDetails;
            _appContextProvider = appContextProvider;
            _decoupeBarre_UpdateApproStarted = decoupeBarre_UpdateApproStarted;
            _vueSettings = vueSettings;

            IncrementQuantityCommand = new UT_RelayCommandArg0(IncrementQuantity, () => Quantite == 0);
            DecrementQuantityCommand = new UT_RelayCommandArg0(DecrementQuantity, () => Quantite > 0);
        }

        #region === Propriétés liées à la vue ===

        /// <summary>
        /// Liste des barres neuves à approvisionner.
        /// </summary>
        public DTO_DecoupeBarreWithBarNewToRelease? DecoupeBarreDetails
        {
            get => _decoupeBarreDetails;
            set
            {
                _decoupeBarreDetails = value;
                _settingsUseCase.SetDecoupeBarreIdStock(value.IdStock);
                OnPropertyChanged();
            }
        }

        public int Quantite
        {
            get => _quantite;
            set
            {
                _quantite = value;
                _settingsUseCase.SetDecoupeBarreStockQuantite(value);
                OnPropertyChanged();
            }
        }

        public int QuantiteMax
        {
            get => _quantiteMax;
            set
            {
                _quantiteMax = value;
                OnPropertyChanged();
            }
        }

        public int ReleaseNumber
        {
            get => _releaseNumber;
            set
            {
                _releaseNumber = value;
                OnPropertyChanged();
            }
        }

        public Uri? ReferenceVueUri
        {
            get => _referenceVueUri;
            set
            {
                _referenceVueUri = value;
                OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// Charge les barres neuves à approvisionner pour le lot sélectionné.
        /// </summary>
        public async Task LoadDataAsync()
        {
            // Conctruire la callChain
            string callChain = $"{_callee} > {nameof(LoadDataAsync)}";

            // Eléments de paramétrage
            DTO_AppContext appCtx = _appContextProvider.GetAppContext();

            // Charger les données de BarNewList
            var BarNewList = await _qhDecoupeBarreDetails.HandleGetAsync(_settingsUseCase.GetDecoupeLotId());

            // Tester si decoupeBarreDetails == null
            if (BarNewList == null || !BarNewList.Any())
            {
                _navigation.NavigateToNewPage("Page10");
                return;
            }

            // Mettre à jour DecoupeBarreDetails
            DecoupeBarreDetails = BarNewList.FirstOrDefault();

            // Mettre à jour ReleaseNumber
            ReleaseNumber = BarNewList.Count();

            // Mettre à jour l'image de coupe de la référence
            ReferenceVueUri = _vueSettings.GetVueUri(DecoupeBarreDetails.Reference);

            // Identifier tout les enregistrements de decoupeBarreDetails pour DecoupeBarreDetails.IdStock
            var BarNewListToRelease = BarNewList.Where(x => x.IdStock == DecoupeBarreDetails?.IdStock).ToList(); ;

            if (!BarNewListToRelease.Any())
            {
                _navigation.NavigateToNewPage("Page10");
                return;
            }

            // Mettre à jour les quantitées
            QuantiteMax = BarNewListToRelease.Count();
            Quantite = QuantiteMax;

            // Pour chaque enregistrement mettre à jour:
            foreach(var BarNew in BarNewListToRelease)
            {
                _settingsUseCase.SetDecoupeBarreId(BarNew.Id);

                // Mettre à jour la date de début d'approvissionnement dans la table DecoupeDarre
                await _decoupeBarre_UpdateApproStarted.ExecuteAsync(BarNew.Id, appCtx, callChain);
            }
        }

        private void IncrementQuantity()
        {
            if (Quantite < _quantiteMax)
            {
                Quantite += 1;
                UpdateCommandStates();
            }
        }

        private void DecrementQuantity()
        {
            if (Quantite > 0)
            {
                Quantite -= 1;
                UpdateCommandStates();
            }
        }

        private void UpdateCommandStates()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}*/