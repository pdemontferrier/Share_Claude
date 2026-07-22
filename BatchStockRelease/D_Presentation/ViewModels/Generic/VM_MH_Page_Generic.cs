using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;

namespace BatchStockRelease.D_Presentation.ViewModels.Generic
{
    /// <summary>
    /// <para><b>Classe de base des ViewModels des menus horizontaux (VM_MH_PageXX).</b></para>
    /// <para>Elle définit les commandes génériques accessibles depuis toutes les pages :</para>
    /// <list type="bullet">
    /// <item><description>Réduction du menu horizontal</description></item>
    /// <item><description>Navigation vers la page précédente</description></item>
    /// <item><description>Rafraîchissement de la page actuelle</description></item>
    /// <item><description>Navigation vers la page d'accueil (Page10)</description></item>
    /// </list>
    /// <para>Cette classe implémente également <see cref="INotifyPropertyChanged"/> pour assurer
    /// le binding dynamique des propriétés dans l’interface utilisateur.</para>
    /// </summary>
    public abstract class VM_MH_Page_Generic : INotifyPropertyChanged
    {
        #region === Propriétés privés ===
        /// <summary>
        /// Nom du ViewModel appelé, utilisé pour la traçabilité et les logs.
        /// </summary>
        private readonly string _callee;

        /// <summary>
        /// Propriété permettant de capter l'état du traitement.
        /// </summary>
        private bool _isProcessing;

        /// <summary>
        /// Service de navigation permettant la gestion centralisée de la navigation entre les pages.
        /// </summary>
        protected readonly IS_Navigation _navigation;
        #endregion

        #region === Propriétés publiques ===
        /// <summary>Indique si un traitement est en cours afin d’éviter les exécutions concurrentes.</summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        /// <summary>Commande pour réduire le menu horizontal.</summary>
        public ICommand MenuCommand { get; }

        /// <summary>Commande pour rafraîchir la page actuelle.</summary>
        public ICommand RefreshCommand { get; }

        /// <summary>Commande pour revenir à la page précédente.</summary>
        public ICommand PreviousCommand { get; }

        /// <summary>Commande pour revenir à la page d'accueil (Page10).</summary>
        public ICommand HomeCommand { get; }
        #endregion

        /// <summary>
        /// Initialise les services de navigation et les commandes communes à tous les menus horizontaux.
        /// </summary>
        protected VM_MH_Page_Generic()
        {
            _callee = GetType().Name;

            _navigation = App._serviceProvider.GetRequiredService<IS_Navigation>();

            MenuCommand = new UT_RelayCommandArg0(ReduceHorizontalMenu);
            RefreshCommand = new UT_RelayCommandArg0(RefreshCurrentPage);
            PreviousCommand = new UT_RelayCommandArg0(NavigateToPreviousPage);
            HomeCommand = new UT_RelayCommandArg0(NavigateToPage10);
        }

        #region === Méthodes de mise à jour de l'état de traitement ===
        /// <summary>
        /// Construit la première partie d'une callChain pour la traçabilité et les logs.
        /// </summary>
        protected string BuildFirstCallChain([CallerMemberName] string caller = "")
            => $"{_callee} > {caller}";

        /// <summary>
        /// Active l’état de traitement en cours et modifie le curseur de la souris.
        /// </summary>
        protected virtual void BeginProcessing()
        {
            // Mettre à jour le staut du process
            IsProcessing = true;

            // Changer l'apparence de la souris
            Mouse.OverrideCursor = Cursors.Wait;
        }

        /// <summary>
        /// Réinitialise l’état de traitement et restaure le curseur par défaut.
        /// </summary>
        protected virtual void EndProcessing()
        {
            // Changer l'apparence de la souris
            Mouse.OverrideCursor = null;

            // Mettre à jour le staut du process
            IsProcessing = false;
        }
        #endregion

        #region === Méthodes d’exécution des commandes de base ===
        /// <summary>Réduit ou masque le menu horizontal.</summary>
        private void ReduceHorizontalMenu()
        {
            _navigation.ReduceHorizontalMenu();
        }

        /// <summary>Navigate directement vers la page d'accueil (Page10).</summary>
        private void NavigateToPage10()
        {
            _navigation.NavigateToNewPage("Page10");
        }

        /// <summary>Rafraîchit la page actuellement affichée.</summary>
        private void RefreshCurrentPage()
        {
            _navigation.RefreshCurrentPage();
        }

        /// <summary>Navigate vers la page précédente si disponible.</summary>
        private void NavigateToPreviousPage()
        {
            _navigation.NavigateToPreviousPage();
        }
        #endregion

        #region === Implémentation INotifyPropertyChanged ===
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Déclenche la notification de changement de propriété pour mettre à jour la vue.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Met à jour la valeur d’une propriété et déclenche <see cref="PropertyChanged"/>.
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
        #endregion
    }
}