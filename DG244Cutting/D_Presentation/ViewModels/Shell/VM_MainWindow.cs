using System.ComponentModel;
using System.Runtime.CompilerServices;
using DG244Cutting.A_Domain.Interfaces.Settings.App;

namespace DG244Cutting.D_Presentation.ViewModels.Shell
{
    /// <summary>
    /// ViewModel singulier du shell applicatif <c>MainWindow</c> de DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant hors familles canoniques (UC, SR, CR, CH, QH,
    /// SE, RS, DTO, VM-Page, MH, Page), instancié en portée Singleton par le conteneur
    /// d'injection de dépendances et consommé par <c>MainWindow</c> via affectation
    /// directe à son <c>DataContext</c>. À l'instar de <see cref="App"/>, son statut
    /// singulier est défendu en l'absence de 0232 d'autorité documentaire ; les
    /// conventions documentaires et structurelles sont calquées sur celles de
    /// <see cref="App"/>.</para>
    /// <para>Objectif : Exposer à la View les propriétés observables strictement
    /// nécessaires au binding du shell — au présent stade, le titre applicatif via
    /// <see cref="ApplicationTitle"/>, alimenté par <see cref="ISE_App.ApplicationTitle"/>
    /// et relayé en INPC. Le ViewModel se comporte comme un relais INPC d'un Setting
    /// vers la View, et ne porte aucune logique d'orchestration applicative.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer <see cref="ApplicationTitle"/> bindable en lecture, alimentée
    ///   depuis <see cref="ISE_App.ApplicationTitle"/>.</item>
    ///   <item>Implémenter <see cref="INotifyPropertyChanged"/> et propager à la View
    ///   les changements de <see cref="ISE_App.ApplicationTitle"/> par abonnement à
    ///   <see cref="INotifyPropertyChanged.PropertyChanged"/> du Setting source.</item>
    ///   <item>Servir de point d'extension pour d'éventuelles propriétés bindables
    ///   futures du shell — ces ajouts feront l'objet de fils d'Extension ultérieurs.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Aucun pilotage de séquence applicative : le démarrage est intégralement
    ///   porté par <see cref="App"/> et <c>UC_Application_OnStart</c> en amont du
    ///   <c>Show()</c> de <c>MainWindow</c>.</item>
    ///   <item>Aucun listener temps réel (admin, messages, monitoring DB) ; ces écoutes
    ///   feront l'objet de fils ultérieurs séparés (UseCases dédiés).</item>
    ///   <item>Aucune logique de navigation ; la navigation initiale au <c>Loaded</c>
    ///   de <c>MainWindow</c> est déléguée à <c>IU_Navigation.NavigateToDefaultAsync</c>,
    ///   invoquée par le code-behind de <c>MainWindow</c>.</item>
    ///   <item>Aucune logique métier ni accès aux données.</item>
    /// </list>
    /// <para>Note sur le statut hors familles canoniques : VM_MainWindow
    /// n'appartient à aucune des familles canoniques de l'écosystème et ne dispose
    /// donc d'aucun 0232 d'autorité documentaire. Les conventions documentaires et
    /// structurelles appliquées sont calquées sur celles de <see cref="App"/>
    /// (composant singulier hors familles déjà produit). La mécanique INPC s'inspire
    /// de <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>
    /// sans héritage formel ; le helper <see cref="SetField{T}"/> est aligné sur
    /// l'étalon documentaire du projet <c>SE_App.SetField</c>.</para>
    /// </remarks>
    public class VM_MainWindow : INotifyPropertyChanged
    {
        #region === Propriétés privées ===

        private readonly string _callee;
        private string _applicationTitle;

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_App _seApp;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient le titre principal de l'application, destiné au binding du
        /// shell applicatif.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par <c>MainWindow</c>
        /// (binding sur sa propriété <c>Title</c>) et par <c>CommonBackgroundPad</c>
        /// (binding sur sa <c>DependencyProperty Title</c>) au sein du shell.</para>
        /// <para>Objectif : Refléter en temps réel la valeur de
        /// <see cref="ISE_App.ApplicationTitle"/>. La doctrine INPC retenue est un
        /// backing field local synchronisé : le ViewModel s'abonne à
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> du Setting source et
        /// met à jour le champ local via <see cref="SetField{T}"/>, garantissant la
        /// propagation canonique vers la View. L'accesseur en écriture est privé :
        /// la valeur ne peut être modifiée qu'à travers le handler interne d'abonnement.</para>
        /// </remarks>
        public string ApplicationTitle
        {
            get => _applicationTitle;
            private set => SetField(ref _applicationTitle, value);
        }

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        /// <summary>
        /// Émis lorsqu'une propriété observable du ViewModel change de valeur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Mécanisme INPC standard exploité par les bindings
        /// WPF de <c>MainWindow</c> et de ses sous-composants.</para>
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_MainWindow"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur d'injection
        /// de dépendances lors de la résolution du Singleton, au démarrage applicatif
        /// et préalablement à l'instanciation de <c>MainWindow</c>.</para>
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item>Résolution et stockage de la dépendance <see cref="ISE_App"/> avec
        ///   garde <see cref="ArgumentNullException"/>.</item>
        ///   <item>Initialisation du champ <c>_callee</c> à partir du nom de type
        ///   (cohérence avec la doctrine CallChain du projet, §4.5).</item>
        ///   <item>Initialisation du backing field <c>_applicationTitle</c> à la valeur
        ///   courante de <see cref="ISE_App.ApplicationTitle"/>, garantissant un état
        ///   cohérent dès l'instanciation, indépendamment de l'ordre relatif d'exécution
        ///   par rapport à <c>UC_Application_OnStart</c>.</item>
        ///   <item>Abonnement à <see cref="INotifyPropertyChanged.PropertyChanged"/>
        ///   du Setting source, afin de relayer toute évolution ultérieure de
        ///   <see cref="ISE_App.ApplicationTitle"/> vers la View.</item>
        /// </list>
        /// <para>Filet de sécurité : Aucune invocation asynchrone ni opération
        /// susceptible de lever une exception terminale n'est portée par le
        /// constructeur ; l'abonnement à
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> n'engage pas de risque
        /// exploitable par <c>try/catch</c>. Aucune dépendance vers
        /// <c>IU_LogAndNotify</c> n'est donc requise dans le périmètre du présent fil.</para>
        /// </remarks>
        /// <param name="seApp">Setting Singleton centralisant l'état applicatif global,
        /// injecté par le conteneur DI. Source de la valeur de
        /// <see cref="ApplicationTitle"/> et émetteur des notifications INPC consommées
        /// par le ViewModel.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="seApp"/> est
        /// <see langword="null"/>.</exception>
        public VM_MainWindow(ISE_App seApp)
        {
            _seApp = seApp ?? throw new ArgumentNullException(nameof(seApp));

            _callee = GetType().Name;
            _applicationTitle = _seApp.ApplicationTitle;

            _seApp.PropertyChanged += OnSeAppPropertyChanged;
        }

        #endregion

        #region === Méthodes publiques ===

        // Aucune méthode publique exposée par VM_MainWindow dans le périmètre du présent fil.

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Gestionnaire d'abonnement à <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// du Setting <see cref="ISE_App"/>, relayant les changements pertinents vers
        /// les propriétés bindables du ViewModel.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Invoqué automatiquement par <see cref="ISE_App"/>
        /// lors de toute mutation d'une de ses propriétés observables. Filtre les
        /// notifications pour ne propager que celles couvertes par les propriétés
        /// exposées par le ViewModel.</para>
        /// <para>Objectif : Synchroniser le backing field local
        /// <c>_applicationTitle</c> avec la valeur courante de
        /// <see cref="ISE_App.ApplicationTitle"/> et déclencher la notification INPC
        /// correspondante via <see cref="SetField{T}"/>, garantissant la mise à jour
        /// des bindings WPF consommateurs.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance Singleton de
        /// <see cref="ISE_App"/>).</param>
        /// <param name="e">Arguments de l'événement portant le nom de la propriété
        /// modifiée.</param>
        private void OnSeAppPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISE_App.ApplicationTitle))
            {
                ApplicationTitle = _seApp.ApplicationTitle;
            }
        }

        /// <summary>
        /// Met à jour un champ de stockage et déclenche la notification
        /// <see cref="PropertyChanged"/> si la valeur a changé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Helper INPC canonique du ViewModel, utilisé par les
        /// setters privés des propriétés bindables et indirectement par les handlers
        /// d'abonnement qui passent par ces setters.</para>
        /// <para>Objectif : Centraliser la triade comparaison de valeur /
        /// écriture du champ support / émission de la notification INPC, et informer
        /// l'appelant du changement effectif via le retour booléen, exploitable pour
        /// des effets de bord conditionnels.</para>
        /// <para>Étalon documentaire : Signature et comportement alignés sur
        /// <c>SE_App.SetField</c>, étalon documentaire du projet pour le helper INPC
        /// canonique.</para>
        /// </remarks>
        /// <typeparam name="T">Type de la valeur stockée.</typeparam>
        /// <param name="field">Référence au champ de stockage privé.</param>
        /// <param name="value">Nouvelle valeur à appliquer.</param>
        /// <param name="propertyName">Nom de la propriété appelante, résolu
        /// automatiquement par <see cref="CallerMemberNameAttribute"/>.</param>
        /// <returns><see langword="true"/> si la valeur a effectivement changé,
        /// <see langword="false"/> sinon.</returns>
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
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