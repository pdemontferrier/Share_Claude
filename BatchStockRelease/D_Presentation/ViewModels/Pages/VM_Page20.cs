using System.Collections.ObjectModel;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page20 — Page d’affectation d’un chariot à un lot à approvisionner.
    ///
    /// <para><b>Contexte :</b> Cette page est affichée lorsqu’un lot sélectionné
    /// dans la Page10 n’a pas encore de chariot attribué. Elle constitue une
    /// étape intermédiaire du processus BatchStockRelease avant la sortie de stock.</para>
    ///
    /// <para><b>Objectif :</b> Permettre à l’utilisateur de sélectionner un
    /// chariot disponible et de l’affecter au lot courant.</para>
    ///
    /// <para><b>Vue associée :</b> <c>Page20.xaml</c></para>
    ///
    /// <list type="bullet">
    ///   <item><description>Affiche la liste des chariots disponibles.</description></item>
    ///   <item><description>Permet la sélection d’un chariot et son association au lot en cours.</description></item>
    ///   <item><description>Gère les interactions via les services partagés de <see cref="VM_Page_Generic"/>.</description></item>
    /// </list>
    /// </summary>
    public class VM_Page20 : VM_Page_Generic
    {
        #region === Dépendances privées ===
        private readonly IQ_PickingEmplacement _qhPickingEmplacement;
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel de la page 20 avec les services nécessaires.
        /// </summary>
        /// <param name="qhPickingEmplacement">Handler de récupération des emplacements de picking (chariots).</param>
        /// <param name="settings">Service de gestion des paramètres métier.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue.</param>
        /// <param name="logAndNotify">Service de journalisation et notification des erreurs.</param>
        public VM_Page20(
            IQ_PickingEmplacement qhPickingEmplacement,
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            _qhPickingEmplacement = qhPickingEmplacement;
        }
        #endregion

        #region === Propriétés liées à la vue ===
        /// <summary>
        /// Liste observable des chariots disponibles.
        /// </summary>
        private ObservableCollection<PickingEmplacement>? _chariots;
        public ObservableCollection<PickingEmplacement>? Chariots
        {
            get => _chariots;
            set => SetProperty(ref _chariots, value);
        }

        /// <summary>
        /// Chariot actuellement sélectionné.
        /// </summary>
        private PickingEmplacement? _selectedChariot;
        public PickingEmplacement? SelectedChariot
        {
            get => _selectedChariot;
            set
            {
                if (SetProperty(ref _selectedChariot, value) && value != null)
                {
                    _settingsUseCase.SetDecoupeBarreIdChariot(value.Id);
                    _settingsUseCase.SetDecoupeBarreChariotDesignation(value.Nom);
                }
            }
        }
        #endregion

        #region === Méthodes publiques ===
        /// <summary>
        /// Charge la liste des chariots disponibles pour l’affectation.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l’opération de chargement.</returns>
        public async Task LoadDataAsync()
        {
            var callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                await LoadChariotsAsync(callChain);
            });
        }
        #endregion

        #region === Méthodes privées ===
        /// <summary>
        /// Charge la liste des chariots disponibles pour l’affectation.
        /// </summary>
        /// <param name="caller">CallChain de la méthode appellante.</param>
        private async Task LoadChariotsAsync(string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {nameof(LoadChariotsAsync)}";

            try
            {
                // === Réinitialiser le chariot sélectionné du lot courant ===
                _settingsUseCase.SetDecoupeBarreChariotDesignation(string.Empty);

                // === Charger la liste des PickingEmplacement commençant par 'Chariot' ===
                var chariots = await _qhPickingEmplacement.HandleGetChariotListAsync();

                // === Ajouter un enregistrement vide en haut de la liste ===
                var emptyChariot = new PickingEmplacement
                {
                    Id = 0,
                    Nom = string.Empty
                };
                chariots.Insert(0, emptyChariot);

                // === Mise à jour de la propriété observable ===
                Chariots = new ObservableCollection<PickingEmplacement>(chariots);
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
        #endregion
    }
}