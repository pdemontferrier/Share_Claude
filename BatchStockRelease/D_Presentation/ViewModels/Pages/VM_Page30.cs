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
    /// ViewModel de la Page30 — Page d'affichage des liste de barres pour un lot de fabrication.
    ///
    /// <para><b>Contexte :</b> Cette page fait partie du processus global BatchStockRelease,
    /// utilisé par les magasiniers pour préparer et valider les approvisionnements en barres.</para>
    ///
    /// <para><b>Objectif :</b> Permettre de visualiser et de valider les barres
    /// issues de trois sources distinctes :</para>
    /// <list type="bullet">
    ///   <item><description><b>BarDrop :</b> Barres de chutes disponibles et attribuées au lot.</description></item>
    ///   <item><description><b>BarNew :</b> Barres neuves calculées et à sortir du stock.</description></item>
    ///   <item><description><b>OutOfStock :</b> Barres en rupture nécessitant une action manuelle.</description></item>
    /// </list>
    ///
    /// <para><b>Vue associée :</b> <c>Page30.xaml</c></para>
    ///
    /// <para><b>Spécificités techniques :</b> 
    /// - Récupère les détails de découpe pour le lot courant.  
    /// - Regroupe les données selon leur origine.  
    /// - Gère les erreurs via <see cref="Ex_Classifier"/> et notifie via <see cref="IS_LogAndNotify"/>.</para>
    /// </summary>
    public class VM_Page30 : VM_Page_Generic
    {
        #region === Dépendances privées ===
        private readonly IQ_DecoupeBarreDetails _qhDecoupeBarreDetails;
        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la page 30 avec les services nécessaires.
        /// </summary>
        /// <param name="qhDecoupeBarreDetails">Handler de récupération des détails des barres de découpe.</param>
        /// <param name="settings">Service de gestion des paramètres métier.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue.</param>
        /// <param name="logAndNotify">Service de journalisation et notification des erreurs.</param>
        public VM_Page30(
            IQ_DecoupeBarreDetails qhDecoupeBarreDetails,
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            _qhDecoupeBarreDetails = qhDecoupeBarreDetails;
        }

        #endregion

        #region === Propriétés liées à la vue ===

        private string _batchDesignation = string.Empty;
        /// <summary>
        /// Désignation du lot de fabrication courant.
        /// </summary>
        public string BatchDesignation
        {
            get => _batchDesignation;
            set => SetProperty(ref _batchDesignation, value);
        }

        /// <summary>
        /// Liste des barres issues de chutes.
        /// </summary>
        private ObservableCollection<DTO_DecoupeBarreDetails> _barDropList = new();
        public ObservableCollection<DTO_DecoupeBarreDetails> BarDropList
        {
            get => _barDropList;
            set => SetProperty(ref _barDropList, value);
        }

        /// <summary>
        /// Liste regroupée des barres neuves à sortir.
        /// </summary>
        private ObservableCollection<DTO_DecoupeBarreDetails> _barNewList = new();
        public ObservableCollection<DTO_DecoupeBarreDetails> BarNewList
        {
            get => _barNewList;
            set => SetProperty(ref _barNewList, value);
        }

        /// <summary>
        /// Liste regroupée des barres neuves en rupture de stock.
        /// </summary>
        private ObservableCollection<DTO_DecoupeBarreDetails> _outOfStockList = new();
        public ObservableCollection<DTO_DecoupeBarreDetails> OutOfStockList
        {
            get => _outOfStockList;
            set => SetProperty(ref _outOfStockList, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge les listes des barres de stock pour le lot courant.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    BatchDesignation = _settingsUseCase.GetDecoupeLotDesignation() ?? string.Empty;
                    int lotId = _settingsUseCase.GetDecoupeLotId();

                    var decoupeBarreDetails = await _qhDecoupeBarreDetails.HandleGetAsync(lotId);

                    BarDropList = new ObservableCollection<DTO_DecoupeBarreDetails>(
                        GetBarDropList(decoupeBarreDetails));

                    BarNewList = new ObservableCollection<DTO_DecoupeBarreDetails>(
                        GetBarNewList(decoupeBarreDetails));

                    OutOfStockList = new ObservableCollection<DTO_DecoupeBarreDetails>(
                        GetOutOfStockList(decoupeBarreDetails));
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
        /// Retourne la liste des barres de chutes attribuées au lot.
        /// </summary>
        private IEnumerable<DTO_DecoupeBarreDetails> GetBarDropList(IEnumerable<DTO_DecoupeBarreDetails> list)
        {
            return list
                .Where(db => db.ApproOrigine == "chute" && db.ApproRupture == false)
                .OrderByDescending(db => db.ApproSortieFaite)
                .ThenBy(db => db.ApproEmplacementDesignation)
                .ThenBy(db => db.Reference);
        }

        /// <summary>
        /// Retourne la liste regroupée des barres neuves à sortir.
        /// </summary>
        private IEnumerable<DTO_DecoupeBarreDetails> GetBarNewList(IEnumerable<DTO_DecoupeBarreDetails> list)
        {
            return list
                .Where(db => db.ApproOrigine == "neuf" && db.ApproRupture == false)
                .GroupBy(db => new
                {
                    db.Reference,
                    db.Designation,
                    db.Couleur,
                    db.LongueurBarre,
                    db.ApproAdressePriorite,
                    db.ApproAdresseDesignation,
                    db.ApproConteneur,
                    db.ApproSortieFaite
                })
                .Select(g => new DTO_DecoupeBarreDetails
                {
                    Reference = g.Key.Reference,
                    Designation = g.Key.Designation,
                    Couleur = g.Key.Couleur,
                    LongueurBarre = g.Key.LongueurBarre,
                    ApproAdressePriorite = g.Key.ApproAdressePriorite,
                    ApproAdresseDesignation = g.Key.ApproAdresseDesignation,
                    ApproConteneur = g.Key.ApproConteneur,
                    ApproSortieFaite = g.Key.ApproSortieFaite,
                    QuantiteASortir = g.Count()
                })
                .OrderBy(db => db.ApproAdressePriorite)
                .ThenBy(db => db.ApproAdresseDesignation)
                .ThenBy(db => db.Reference)
                .ThenByDescending(db => db.ApproSortieFaite);
        }

        /// <summary>
        /// Retourne la liste regroupée des barres neuves en rupture de stock.
        /// </summary>
        private IEnumerable<DTO_DecoupeBarreDetails> GetOutOfStockList(IEnumerable<DTO_DecoupeBarreDetails> list)
        {
            return list
                .Where(db => db.ApproOrigine == "neuf" && db.ApproRupture == true)
                .GroupBy(db => new
                {
                    db.Reference,
                    db.Designation,
                    db.Couleur,
                    db.LongueurBarre
                })
                .Select(g => new DTO_DecoupeBarreDetails
                {
                    Reference = g.Key.Reference,
                    Designation = g.Key.Designation,
                    Couleur = g.Key.Couleur,
                    LongueurBarre = g.Key.LongueurBarre,
                    QuantiteASortir = g.Count()
                })
                .OrderBy(db => db.Reference);
        }

        #endregion
    }
}