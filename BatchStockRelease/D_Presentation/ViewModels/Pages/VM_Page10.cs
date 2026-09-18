using System.Collections.ObjectModel;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page10 — Page d’accueil des lots à approvisionner.
    ///
    /// <para><b>Contexte :</b> Cette page constitue le point de départ du processus
    /// d’approvisionnement. Elle affiche deux listes distinctes de lots de découpe :
    /// ceux approvisionnés via les <b>barres de chutes</b> et ceux approvisionnés via
    /// les <b>barres neuves</b>.</para>
    ///
    /// <para><b>Objectif :</b> Permettre au magasinier de sélectionner un lot pour
    /// initier la phase d’approvisionnement correspondante, tout en assurant la
    /// robustesse et la traçabilité des opérations.</para>
    ///
    /// <para><b>Vue associée :</b> <c>Page10.xaml</c></para>
    ///
    /// <list type="bullet">
    ///   <item><description>Chargement des lots à approvisionner (BarDrop et BarNew).</description></item>
    ///   <item><description>Navigation dynamique selon le type de lot sélectionné.</description></item>
    ///   <item><description>Gestion centralisée des exceptions et de la traçabilité via <see cref="VM_Page_Generic"/>.</description></item>
    /// </list>
    /// </summary>
    public class VM_Page10 : VM_Page_Generic
    {
        #region === Dépendances privées ===
        private readonly IQ_DecoupeLotWithBarDropToRelease _qhBarDrop;
        private readonly IQ_DecoupeLotWithBarNewToRelease _qhBarNew;
        private readonly IU_Page10Dispatch _ucPage10Dispatch;
        #endregion

        #region === Propriétés privées ===
        /// <summary>
        /// Verrou anti-exécution concurrente lors de la sélection d’un lot.
        /// </summary>
        private readonly SemaphoreSlim _selectionLock = new(1, 1);

        /// <summary>
        /// Indique qu’une action de sélection est en cours (navigation, etc.).
        /// </summary>
        private bool _isSelectionInProgress;
        #endregion

        #region === Propriétés publiques ===
        /// <summary>
        /// Liste des lots à approvisionner à partir des barres de chute.
        /// </summary>
        private ObservableCollection<DTO_DecoupeLotWithBarDropToRelease> _barDropBatchList = new();
        public ObservableCollection<DTO_DecoupeLotWithBarDropToRelease> BarDropBatchList
        {
            get => _barDropBatchList;
            private set => SetProperty(ref _barDropBatchList, value);
        }

        /// <summary>
        /// Lot sélectionné dans la liste des chutes.
        /// </summary>
        private DTO_DecoupeLotWithBarDropToRelease? _selectedBarDropBatch;
        public DTO_DecoupeLotWithBarDropToRelease? SelectedBarDropBatch
        {
            get => _selectedBarDropBatch;
            set
            {
                if (SetProperty(ref _selectedBarDropBatch, value) && value is not null)
                    _ = HandleSelectionAsync(value.Id, value.ApproIdChariot, isBarDrop: true);
            }
        }

        /// <summary>
        /// Liste des lots à approvisionner à partir des barres neuves.
        /// </summary>
        private ObservableCollection<DTO_DecoupeLotWithBarNewToRelease> _barNewBatchList = new();
        public ObservableCollection<DTO_DecoupeLotWithBarNewToRelease> BarNewBatchList
        {
            get => _barNewBatchList;
            private set => SetProperty(ref _barNewBatchList, value);
        }

        /// <summary>
        /// Lot sélectionné dans la liste des barres neuves.
        /// </summary>
        private DTO_DecoupeLotWithBarNewToRelease? _selectedBarNewBatch;
        public DTO_DecoupeLotWithBarNewToRelease? SelectedBarNewBatch
        {
            get => _selectedBarNewBatch;
            set
            {
                if (SetProperty(ref _selectedBarNewBatch, value) && value is not null)
                    _ = HandleSelectionAsync(value.Id, value.ApproIdChariot, isBarDrop: false);
            }
        }
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel de la page avec les services nécessaires.
        /// </summary>
        /// <param name="qhBarDrop">Handler de récupération des lots de chutes à libérer.</param>
        /// <param name="qhBarNew">Handler de récupération des lots de barres neuves à libérer.</param>
        /// <param name="ucPage10Dispatch">UseCase responsable de la navigation métier depuis la page 10.</param>
        /// <param name="settings">Service de gestion des paramètres métier.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue de l’application.</param>
        /// <param name="logAndNotify">Service de journalisation et notification des erreurs.</param>
        public VM_Page10(
            IQ_DecoupeLotWithBarDropToRelease qhBarDrop,
            IQ_DecoupeLotWithBarNewToRelease qhBarNew,
            IU_Page10Dispatch ucPage10Dispatch,
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            _qhBarDrop = qhBarDrop;
            _qhBarNew = qhBarNew;
            _ucPage10Dispatch = ucPage10Dispatch;
        }
        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge les listes de lots de découpe à approvisionner.
        /// </summary>
        public async Task LoadDataAsync()
        {
            await ExecuteSafeAsync(async () =>
            {
                var callChain = BuildFirstCallChain(nameof(LoadDataAsync));
                await ExecuteLoadingAsync(callChain);
            });
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Gère la sélection d’un lot (chute ou neuf) et déclenche la navigation appropriée.
        /// </summary>
        /// <param name="lotId">Identifiant du lot sélectionné.</param>
        /// <param name="chariotId">Identifiant du chariot associé.</param>
        /// <param name="isBarDrop">Indique s’il s’agit d’un lot de type chute (<c>true</c>) ou neuf (<c>false</c>).</param>
        private async Task HandleSelectionAsync(int lotId, int chariotId, bool isBarDrop)
        {
            // Protection anti-concurrence : ignore si une sélection est déjà en cours
            if (_isSelectionInProgress)
                return;

            await _selectionLock.WaitAsync();
            try
            {
                if (_isSelectionInProgress)
                    return;

                _isSelectionInProgress = true;

                await ExecuteSafeAsync(async () =>
                {
                    // CallChain racine (méthode "entry" déclenchée par interaction UI)
                    string caller = BuildFirstCallChain(nameof(HandleSelectionAsync));
                    await ExecuteActionAsync(lotId, chariotId, isBarDrop, caller);
                });
            }
            finally
            {
                _isSelectionInProgress = false;
                _selectionLock.Release();
            }
        }

        /// <summary>
        /// Exécute le chargement des deux listes de lots (chutes et barres neuves).
        /// </summary>
        /// <param name="caller">Chaîne de traçabilité de l’appelant.</param>
        private async Task ExecuteLoadingAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(ExecuteLoadingAsync)}";

            // Chargement des lots "BarDrop"
            var dropLots = await _qhBarDrop.HandleGetLotsToReleaseAsync();
            BarDropBatchList = new ObservableCollection<DTO_DecoupeLotWithBarDropToRelease>(dropLots);

            // Chargement des lots "BarNew"
            var newLots = await _qhBarNew.HandleGetLotsToReleaseAsync();
            BarNewBatchList = new ObservableCollection<DTO_DecoupeLotWithBarNewToRelease>(newLots);
        }

        /// <summary>
        /// Exécute la navigation vers la page suivante selon le type de lot sélectionné.
        /// </summary>
        /// <param name="lotId">Identifiant du lot sélectionné.</param>
        /// <param name="chariotId">Identifiant du chariot associé.</param>
        /// <param name="isBarDrop">Type de lot (chute/neuf).</param>
        /// <param name="caller">Chaîne de traçabilité de l’appelant.</param>
        private async Task ExecuteActionAsync(int lotId, int chariotId, bool isBarDrop, string caller)
        {
            string callChain = $"{caller} > {nameof(ExecuteActionAsync)}";

            string nextPage = isBarDrop
                ? await _ucPage10Dispatch.ExecuteBarDropAsync(lotId, chariotId, callChain)
                : await _ucPage10Dispatch.ExecuteBarNewAsync(lotId, chariotId, callChain);

            _navigation.NavigateToNewPage(nextPage);
        }

        #endregion
    }
}