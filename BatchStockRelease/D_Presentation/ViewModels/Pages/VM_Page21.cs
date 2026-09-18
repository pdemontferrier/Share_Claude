using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page21 — Page de retour en stock des barres neuves non consommées.
    ///
    /// <para><b>Contexte :</b> Cette page permet de remettre en stock les barres
    /// neuves non consommées après la découpe, en sélectionnant la référence, la couleur
    /// et le conteneur de stockage proposé.</para>
    ///
    /// <para><b>Objectif :</b> Guider l’utilisateur dans le processus de réintégration
    /// des barres non utilisées dans le stock, selon leur type et leur emplacement.</para>
    ///
    /// <para><b>Vue associée :</b> <c>Page21.xaml</c></para>
    ///
    /// <list type="bullet">
    ///   <item><description>Affiche dynamiquement les références, couleurs et conteneurs disponibles.</description></item>
    ///   <item><description>Permet la sélection d’un enregistrement précis pour réintégration.</description></item>
    ///   <item><description>Gère les exceptions via le mécanisme <see cref="Ex_Classifier"/> et la notification unifiée <see cref="IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public class VM_Page21 : VM_Page_Generic
    {
        #region === Dépendances privées ===
        private readonly IQ_VieStockQuantiteEmplacement _qhStockQuantiteEmplacement;
        #endregion

        #region === Commandes ===
        public ICommand IncrementQuantityCommand { get; }
        public ICommand DecrementQuantityCommand { get; }
        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la page 21 avec les services nécessaires.
        /// </summary>
        /// <param name="qhStockQuantiteEmplacement">Handler de lecture des données de stock par référence/couleur/conteneur.</param>
        /// <param name="settings">Service de gestion des paramètres métier.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue.</param>
        /// <param name="logAndNotify">Service de journalisation et notification des erreurs.</param>
        public VM_Page21(
            IQ_VieStockQuantiteEmplacement qhStockQuantiteEmplacement,
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            _qhStockQuantiteEmplacement = qhStockQuantiteEmplacement;

            References = new ObservableCollection<string>();
            Couleurs = new ObservableCollection<string>();
            Conteneurs = new ObservableCollection<string>();
            StockRecord = null;
            _reference = _couleur = _conteneur = string.Empty;
            Quantite = 1;

            IncrementQuantityCommand = new UT_RelayCommandArg0(IncrementQuantity);
            DecrementQuantityCommand = new UT_RelayCommandArg0(DecrementQuantity, () => Quantite > 1);
        }

        #endregion

        #region === Propriétés liées à la vue ===
        private ObservableCollection<string> _references = new();
        public ObservableCollection<string> References
        {
            get => _references;
            set => SetProperty(ref _references, value);
        }

        private string _reference;
        public string Reference
        {
            get => _reference;
            set
            {
                if (SetProperty(ref _reference, value))
                    _ = ExecuteSafeAsync(() => LoadDataCouleurAsync());
            }
        }

        private ObservableCollection<string> _couleurs = new();
        public ObservableCollection<string> Couleurs
        {
            get => _couleurs;
            set => SetProperty(ref _couleurs, value);
        }

        private string _couleur;
        public string Couleur
        {
            get => _couleur;
            set
            {
                if (SetProperty(ref _couleur, value))
                    _ = ExecuteSafeAsync(() => LoadDataConteneurAsync());
            }
        }

        private ObservableCollection<string> _conteneurs = new();
        public ObservableCollection<string> Conteneurs
        {
            get => _conteneurs;
            set => SetProperty(ref _conteneurs, value);
        }

        private string _conteneur;
        public string Conteneur
        {
            get => _conteneur;
            set
            {
                if (SetProperty(ref _conteneur, value))
                    _ = ExecuteSafeAsync(() => LoadDataStockAsync());
            }
        }

        private int _quantite;
        public int Quantite
        {
            get => _quantite;
            set
            {
                if (SetProperty(ref _quantite, value))
                    _settingsUseCase.SetDecoupeBarreStockQuantite(value);
            }
        }

        private VieStockQuantiteEmplacement? _stockRecord;
        public VieStockQuantiteEmplacement? StockRecord
        {
            get => _stockRecord;
            set
            {
                if (SetProperty(ref _stockRecord, value))
                    _settingsUseCase.SetStockQuantiteEmplacement(value);
            }
        }
        #endregion

        #region === Méthodes publiques ===
        /// <summary>
        /// Charge la liste initiale des références disponibles.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    StockRecord = null;
                    var references = await GetFilteredReferencesAsync();
                    References = new ObservableCollection<string>(references!);
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
        /// Charge les couleurs associées à la référence sélectionnée.
        /// </summary>
        public async Task LoadDataCouleurAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataCouleurAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(Reference))
                    {
                        Couleurs = new ObservableCollection<string>();
                        return;
                    }

                    StockRecord = null;
                    var couleurs = await GetFilteredCouleursAsync(Reference);
                    Couleurs = new ObservableCollection<string>(couleurs!);
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
        /// Charge les conteneurs disponibles pour la combinaison référence / couleur.
        /// </summary>
        public async Task LoadDataConteneurAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataConteneurAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(Reference) || string.IsNullOrEmpty(Couleur))
                    {
                        Conteneurs = new ObservableCollection<string>();
                        return;
                    }

                    StockRecord = null;
                    var conteneurs = await GetFilteredConteneursAsync(Reference, Couleur);
                    Conteneurs = new ObservableCollection<string>(conteneurs!);
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
        /// Charge l’enregistrement de stock correspondant à la sélection complète.
        /// </summary>
        public async Task LoadDataStockAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataStockAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    var list = await _qhStockQuantiteEmplacement.HandleGetAllAsNoTrackingAsync();

                    var stockRecord = list.FirstOrDefault(vsqe =>
                        vsqe.Reference == Reference &&
                        vsqe.Couleur == Couleur &&
                        vsqe.ConteneurDesignation == Conteneur);

                    StockRecord = stockRecord;
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
        /// Récupère et filtre les références valides à partir de la vue
        /// <see cref="VieStockQuantiteEmplacement"/>.
        /// </summary>
        /// <returns>
        /// Une liste triée et distincte des références valides
        /// (catégorie 1, 2 ou 3 uniquement).
        /// </returns>
        private async Task<List<string?>> GetFilteredReferencesAsync()
        {
            var list = await _qhStockQuantiteEmplacement.HandleGetAllAsNoTrackingAsync();

            return list
                .Where(vsqe => vsqe.Categorie1 is 1 or 2 or 3)
                .Select(vsqe => vsqe.Reference)
                .Distinct()
                .OrderBy(r => r)
                .ToList();
        }

        /// <summary>
        /// Récupère et filtre la liste des couleurs disponibles pour une référence donnée.
        /// </summary>
        /// <param name="reference">Référence sélectionnée.</param>
        /// <returns>Liste triée et distincte des couleurs disponibles.</returns>
        private async Task<List<string?>> GetFilteredCouleursAsync(string reference)
        {
            var list = await _qhStockQuantiteEmplacement.HandleGetAllAsNoTrackingAsync();

            return list
                .Where(vsqe => vsqe.Reference == reference)
                .Select(vsqe => vsqe.Couleur)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        /// <summary>
        /// Récupère et filtre la liste des conteneurs disponibles pour une combinaison référence / couleur.
        /// </summary>
        /// <param name="reference">Référence sélectionnée.</param>
        /// <param name="couleur">Couleur sélectionnée.</param>
        /// <returns>Liste triée et distincte des conteneurs valides.</returns>
        private async Task<List<string?>> GetFilteredConteneursAsync(string reference, string couleur)
        {
            var list = await _qhStockQuantiteEmplacement.HandleGetAllAsNoTrackingAsync();

            return list
                .Where(vsqe =>
                    vsqe.Reference == reference &&
                    vsqe.Couleur == couleur &&
                    vsqe.ConteneurDesignation != "K0000")
                .Select(vsqe => vsqe.ConteneurDesignation)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        private void IncrementQuantity()
        {
            Quantite += 1;
            CommandManager.InvalidateRequerySuggested();
        }

        private void DecrementQuantity()
        {
            if (Quantite > 1)
            {
                Quantite -= 1;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion
    }
}