using System.Collections.ObjectModel;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page23 — Page de forçage des barres neuves en rupture de stock.
    ///
    /// <para><b>Contexte :</b> Cette page intervient lorsque certaines barres
    /// neuves d’un lot ne peuvent pas être approvisionnées. Elle permet au
    /// magasinier de décider de forcer l’approvisionnement partiel ou total.</para>
    ///
    /// <para><b>Objectif :</b> Offrir deux actions possibles via le Menu Horizontal :</para>
    /// <list type="bullet">
    ///   <item><description><b>Forçage du lot :</b> Autorise uniquement la découpe
    ///   des barres déjà disponibles.</description></item>
    ///   <item><description><b>Forçage complet :</b> Force l’approvisionnement des barres
    ///   en rupture et les rend visibles aux postes de découpe.</description></item>
    /// </list>
    ///
    /// <para><b>Vue associée :</b> <c>Page23.xaml</c></para>
    ///
    /// <para><b>Spécificités techniques :</b> 
    /// - Affiche la liste des lots avec barres en rupture.  
    /// - Permet la sélection d’un lot pour en afficher les détails.  
    /// - Gère les exceptions classifiées via <see cref="Ex_Classifier"/>.</para>
    /// </summary>
    public class VM_Page23 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        private readonly IQ_DecoupeLotWithBarNewToRelease _qhDecoupeLotWithBarNewToRelease;
        private readonly IQ_DecoupeBarreDetails _qhDecoupeBarreDetails;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la page 23 avec les services nécessaires.
        /// </summary>
        /// <param name="qhDecoupeLotWithBarNewToRelease">Handler pour la récupération des lots en rupture de stock.</param>
        /// <param name="qhDecoupeBarreDetails">Handler pour la récupération des détails des barres de découpe.</param>
        /// <param name="settings">Service de gestion des paramètres métier.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue.</param>
        /// <param name="logAndNotify">Service de journalisation et notification des erreurs.</param>
        public VM_Page23(
            IQ_DecoupeLotWithBarNewToRelease qhDecoupeLotWithBarNewToRelease,
            IQ_DecoupeBarreDetails qhDecoupeBarreDetails,
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            _qhDecoupeLotWithBarNewToRelease = qhDecoupeLotWithBarNewToRelease;
            _qhDecoupeBarreDetails = qhDecoupeBarreDetails;
        }

        #endregion

        #region === Propriétés liées à la vue ===

        /// <summary>
        /// Liste des lots contenant des barres neuves en rupture de stock.
        /// </summary>
        private ObservableCollection<DTO_DecoupeLotWithBarNewToRelease> _batchOutOfStockList = new();
        public ObservableCollection<DTO_DecoupeLotWithBarNewToRelease> BatchOutOfStockList
        {
            get => _batchOutOfStockList;
            set => SetProperty(ref _batchOutOfStockList, value);
        }

        /// <summary>
        /// Lot sélectionné dans la liste.
        /// </summary>
        private DTO_DecoupeLotWithBarNewToRelease? _selectedBatch;
        public DTO_DecoupeLotWithBarNewToRelease? SelectedBatch
        {
            get => _selectedBatch;
            set
            {
                if (SetProperty(ref _selectedBatch, value) && value != null)
                {
                    _settingsUseCase.SetDecoupeLotId(value.Id);
                    _ = ExecuteSafeAsync(() => LoadOutOfStockListAsync(value.Id)); // fire & forget
                }
            }
        }

        /// <summary>
        /// Liste des barres neuves en rupture de stock associées au lot sélectionné.
        /// </summary>
        private ObservableCollection<DTO_DecoupeBarreDetails> _barNewOutOfStockList = new();
        public ObservableCollection<DTO_DecoupeBarreDetails> BarNewOutOfStockList
        {
            get => _barNewOutOfStockList;
            set => SetProperty(ref _barNewOutOfStockList, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge la liste des lots contenant des barres neuves en rupture.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    var batchList = await GetLotsOutOfStockAsync();
                    BatchOutOfStockList = new ObservableCollection<DTO_DecoupeLotWithBarNewToRelease>(batchList);

                    // Réinitialiser l'identifiant du lot sélectionné
                    _settingsUseCase.SetDecoupeLotId(0);
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
        /// Charge la liste des barres neuves en rupture pour le lot sélectionné.
        /// </summary>
        private async Task LoadOutOfStockListAsync(int lotId)
        {
            string callChain = BuildFirstCallChain(nameof(LoadOutOfStockListAsync));

            try
            {
                var groupedList = await GetGroupedOutOfStockBarsAsync(lotId);
                BarNewOutOfStockList = new ObservableCollection<DTO_DecoupeBarreDetails>(groupedList);
            }
            catch (Ex_Infrastructure)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Récupère la liste des lots comportant des barres en rupture.
        /// </summary>
        private async Task<List<DTO_DecoupeLotWithBarNewToRelease>> GetLotsOutOfStockAsync()
        {
            return await _qhDecoupeLotWithBarNewToRelease.HandleGetLotsOutOfStockAsync();
        }

        /// <summary>
        /// Récupère et groupe les barres neuves en rupture pour un lot donné.
        /// </summary>
        /// <param name="lotId">Identifiant du lot à traiter.</param>
        /// <returns>Liste regroupée des barres en rupture.</returns>
        private async Task<List<DTO_DecoupeBarreDetails>> GetGroupedOutOfStockBarsAsync(int lotId)
        {
            var decoupeBarreDetails = await _qhDecoupeBarreDetails.HandleGetAsync(lotId);

            return decoupeBarreDetails
                .Where(db => db.ApproOrigine == "neuf" && db.ApproRupture && !db.ApproSortieForce)
                .GroupBy(db => new { db.Reference, db.Designation, db.Couleur, db.LongueurBarre })
                .Select(g => new DTO_DecoupeBarreDetails
                {
                    Reference = g.Key.Reference,
                    Designation = g.Key.Designation,
                    Couleur = g.Key.Couleur,
                    LongueurBarre = g.Key.LongueurBarre,
                    QuantiteASortir = g.Count()
                })
                .OrderBy(db => db.Reference)
                .ToList();
        }

        #endregion
    }
}