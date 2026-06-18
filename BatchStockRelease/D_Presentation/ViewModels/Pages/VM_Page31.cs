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
    /// ViewModel de la Page31 — Interface de sortie de stock des barres de chutes par référence.
    ///
    /// <para><b>Contexte :</b> Cette page guide le magasinier dans la sortie
    /// de stock des barres de chute, dans un ordre optimisé en fonction des
    /// zones de stockage, afin de réduire les déplacements. Les barres de chute
    /// sont sorties une à une.</para>
    ///
    /// <para><b>Objectif :</b> Afficher les barres à sortir, leur ordre de
    /// traitement et leur image de profil pour une identification rapide.</para>
    ///
    /// <para><b>Vue associée :</b> <c>Page31.xaml</c></para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>BarListView :</b> Liste des barres à sortir.</description></item>
    ///   <item><description><b>ImageProfil :</b> Représentation visuelle du profil de la barre.</description></item>
    ///   <item><description><b>Plus/Minus :</b> Ajustement de la quantité sortie.</description></item>
    /// </list>
    ///
    /// <para><b>Spécificités techniques :</b> 
    /// - Récupère les barres de chute à sortir pour le lot courant.  
    /// - Met à jour l’état d’approvisionnement dans la table <c>DecoupeBarre</c>.  
    /// - Gère les erreurs via <see cref="Ex_Classifier"/> et notifie via <see cref="IS_LogAndNotify"/>.</para>
    /// </summary>
    public class VM_Page31 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        private readonly IQ_AppContext _appContext;
        private readonly IQ_DecoupeBarreWithBarDropToRelease _qhDecoupeBarreDetails;
        private readonly IS_DecoupeBarre_UpdateApproStarted _decoupeBarre_UpdateApproStarted;
        private readonly IS_Utilities _vueSettings;

        #endregion

        #region === Commandes ===

        public ICommand IncrementQuantityCommand { get; }
        public ICommand DecrementQuantityCommand { get; }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la page 31 avec les services nécessaires.
        /// </summary>
        public VM_Page31(
            IQ_DecoupeBarreWithBarDropToRelease qhDecoupeBarreDetails,
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

            IncrementQuantityCommand = new UT_RelayCommandArg0(IncrementQuantity, () => Quantite == 0);
            DecrementQuantityCommand = new UT_RelayCommandArg0(DecrementQuantity, () => Quantite == 1);
        }

        #endregion

        #region === Propriétés liées à la vue ===

        private DTO_DecoupeBarreWithBarDropToRelease? _decoupeBarreDetails;
        /// <summary>
        /// Détails de la barre de chute actuellement sélectionnée.
        /// </summary>
        public DTO_DecoupeBarreWithBarDropToRelease? DecoupeBarreDetails
        {
            get => _decoupeBarreDetails;
            set
            {
                if (SetProperty(ref _decoupeBarreDetails, value) && value != null)
                {
                    _settingsUseCase.SetDecoupeBarreId(value.Id);
                    _settingsUseCase.SetDecoupeBarreIdStock(value.IdStock);
                }
            }
        }

        private int _quantite;
        /// <summary>
        /// Quantité de barres à sortir (initialement 1).
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

        private int _releaseNumber;
        /// <summary>
        /// Nombre total de barres à sortir pour le lot courant.
        /// </summary>
        public int ReleaseNumber
        {
            get => _releaseNumber;
            set => SetProperty(ref _releaseNumber, value);
        }

        private Uri? _referenceVueUri;
        /// <summary>
        /// URI de l’image du profil de la barre sélectionnée.
        /// </summary>
        public Uri? ReferenceVueUri
        {
            get => _referenceVueUri;
            set => SetProperty(ref _referenceVueUri, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge la première barre à sortir pour le lot courant,
        /// initialise la quantité et l’image du profil associé.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    var appCtx = _appContext.GetAppContext();

                    var decoupeBarreList = await GetDecoupeBarreListAsync();
                    if (!decoupeBarreList.Any())
                    {
                        _navigation.NavigateToNewPage("Page10");
                        return;
                    }

                    InitializeBarView(decoupeBarreList);
                    await UpdateApproStatusAsync(appCtx, callChain);
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
        /// Récupère la liste des barres de chute associées au lot courant.
        /// </summary>
        /// <returns>Liste des barres de chute à sortir.</returns>
        private async Task<List<DTO_DecoupeBarreWithBarDropToRelease>> GetDecoupeBarreListAsync()
        {
            int lotId = _settingsUseCase.GetDecoupeLotId();
            var decoupeBarreList = await _qhDecoupeBarreDetails.HandleGetAsync(lotId);
            return decoupeBarreList ?? new List<DTO_DecoupeBarreWithBarDropToRelease>();
        }

        /// <summary>
        /// Initialise l'affichage de la première barre de chute à sortir.
        /// </summary>
        /// <param name="decoupeBarreList">Liste complète des barres de chute récupérées.</param>
        private void InitializeBarView(List<DTO_DecoupeBarreWithBarDropToRelease> decoupeBarreList)
        {
            DecoupeBarreDetails = decoupeBarreList.First();
            ReleaseNumber = decoupeBarreList.Count();
            ReferenceVueUri = _vueSettings.GetVueUri(DecoupeBarreDetails.Reference);
            Quantite = 1;
        }

        /// <summary>
        /// Met à jour l’état d’approvisionnement de la première barre sélectionnée.
        /// </summary>
        /// <param name="appCtx">Contexte applicatif courant.</param>
        /// <param name="callChain">Chaîne d’appel pour la traçabilité.</param>
        private async Task UpdateApproStatusAsync(DTO_AppContext appCtx, string callChain)
        {
            if (DecoupeBarreDetails is not null)
            {
                await _decoupeBarre_UpdateApproStarted.ExecuteAsync(
                    DecoupeBarreDetails.Id,
                    appCtx,
                    callChain);
            }
        }

        /// <summary>
        /// Incrémente la quantité à sortir.
        /// <para>
        /// Dans le contexte de la sortie des barres de chutes, la quantité est limitée à une seule unité.
        /// Cette méthode est active uniquement lorsque la quantité actuelle est égale à 0.
        /// </para>
        /// </summary>
        private void IncrementQuantity()
        {
            if (Quantite == 0)
            {
                Quantite++;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Décrémente la quantité à sortir.
        /// <para>
        /// Si la quantité vaut 1, elle est remise à 0 afin d’annuler la sélection
        /// de la barre en cours de traitement.
        /// </para>
        /// </summary>
        private void DecrementQuantity()
        {
            if (Quantite == 1)
            {
                Quantite--;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion
    }
}