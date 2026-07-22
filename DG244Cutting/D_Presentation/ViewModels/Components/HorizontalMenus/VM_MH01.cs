using System.Windows.Input;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Components.HorizontalMenus
{
    /// <summary>
    /// ViewModel du menu horizontal associé à la
    /// <see cref="DG244Cutting.D_Presentation.ViewModels.Pages.VM_Page01"/>
    /// de l'application DG244Cutting, exposant à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Components.HorizontalMenus.MH01"/>
    /// les quatre commandes transverses standards héritées du socle
    /// <see cref="VM_MH_Generic"/>, augmentées d'une commande de
    /// navigation propre <see cref="AdminCommand"/> ouvrant la page
    /// d'administration des utilisateurs, et d'un libellé multilingue
    /// propre <see cref="Label_MH_Admin"/> alimentant le bouton
    /// correspondant.
    /// </summary>
    /// <remarks>
    /// <para>Contexte :</para>
    ///
    /// <para>ViewModel du menu horizontal de la page utilisateur
    /// (Page01). Il complète le socle transverse par un unique point
    /// d'accès applicatif supplémentaire : l'ouverture de la page
    /// d'administration des utilisateurs (Page04).</para>
    ///
    /// <para>Objectif :</para>
    ///
    /// <para>Offrir à l'opérateur disposant des droits requis un
    /// bouton « Admin » déclenchant une navigation applicative vers
    /// Page04, de même nature que les commandes transverses de
    /// navigation du socle (Home, Previous), et non un traitement
    /// métier. Le bouton n'est visible que lorsque l'utilisateur
    /// courant dispose à la fois du droit d'administration sur sa page
    /// utilisateur et de l'accès effectif à la page cible ; cette
    /// visibilité conditionnée est portée par la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Components.HorizontalMenus.MH01"/>.</para>
    ///
    /// <para>Responsabilités :</para>
    ///
    /// <para>Exposer la commande de navigation
    /// <see cref="AdminCommand"/> et le libellé observable
    /// <see cref="Label_MH_Admin"/>, en sus des membres transverses
    /// hérités. Déléguer l'ouverture de Page04 au UseCase de
    /// navigation <see cref="IU_Navigation"/> (EA-05), dans le filet
    /// de sécurité hérité <see cref="VM_Generic.ExecuteSafeAsync"/> et
    /// sous garde d'anti-réentrance
    /// <see cref="VM_MH_Generic.IsProcessing"/>. Alimenter le libellé
    /// propre par surcharge nominative de
    /// <see cref="LoadLabels"/>.</para>
    ///
    /// <para>Non-responsabilités :</para>
    ///
    /// <para>N'effectue aucun traitement métier et ne consomme aucun
    /// UseCase métier via <c>IS_UseCaseInvoker</c> (EA-11 non
    /// mobilisée) : la commande <see cref="AdminCommand"/> est à
    /// parité de nature avec les cinq commandes transverses du socle.
    /// Ne résout ni n'évalue les droits d'accès, délégués à
    /// <see cref="IU_Navigation"/> et à la vue.</para>
    ///
    /// <para>Note sur les exceptions architecturales :</para>
    ///
    /// <para>Mobilise l'EA-05 (consommation de
    /// <see cref="IU_Navigation"/> par un ViewModel via le filet de
    /// sécurité), déjà portée par le socle
    /// <see cref="VM_MH_Generic"/> pour ses cinq commandes
    /// transverses. Le champ <c>_navigation</c> du socle étant
    /// <c>private</c>, le présent dérivé retient sa propre référence à
    /// la même instance Singleton — reçue par son constructeur et
    /// aujourd'hui transmise à <c>base(...)</c> — dans un champ privé
    /// propre, pour la rendre accessible au handler
    /// <see cref="AdminCommand"/>. L'EA-11 n'est pas mobilisée.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2 du 0230), augmentée de l'extension §4.4.3
    /// <c>=== Propriétés publiques ===</c> qui porte les deux membres
    /// exposés propres <see cref="AdminCommand"/> et
    /// <see cref="Label_MH_Admin"/>. La région Méthodes protégées est
    /// absente conformément à R-4.4.10 du 0231 (la classe n'expose
    /// aucune méthode <c>protected</c> propre : la surcharge
    /// <see cref="LoadLabels"/> est <c>protected override</c> et non
    /// une méthode <c>protected</c> propre). L'extension
    /// <c>=== Événements / Délégués / Indexeurs ===</c> n'est pas
    /// présente. Soit six régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   porte le champ backing <c>_label_mh_admin</c> du libellé
    ///   propre.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   porte la référence propre <c>_navigation</c>
    ///   (<see cref="IU_Navigation"/>, EA-05), rétention de la même
    ///   instance Singleton transmise à <c>base(...)</c>, rendue
    ///   accessible au handler propre.</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c>
    ///   (extension §4.4.3) : porte la commande de navigation
    ///   <see cref="AdminCommand"/> et le libellé observable
    ///   <see cref="Label_MH_Admin"/>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur <c>public</c> à quatre paramètres (signature
    ///   inchangée), délégation à <see cref="VM_MH_Generic"/> via
    ///   <c>base(...)</c>, puis rétention de <c>_navigation</c>,
    ///   composition d'<see cref="AdminCommand"/>, et invocation
    ///   d'<see cref="VM_Generic.InitializeLabels"/> en dernière
    ///   instruction (R-4.11.8 du 0231).</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   porte la surcharge nominative
    ///   <see cref="LoadLabels"/> qui alimente le libellé propre après
    ///   appel de <c>base.LoadLabels(callChain)</c> en première
    ///   instruction. Aucun override de
    ///   <see cref="VM_MH_Generic.LoadAsync"/>, le composant n'ayant
    ///   pas de donnée métier à charger.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   porte le handler <see cref="ExecuteAdminAsync"/> de la
    ///   commande de navigation propre.</description></item>
    /// </list>
    /// </remarks>
    public class VM_MH01 : VM_MH_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Champ de stockage du libellé multilingue propre
        /// <see cref="Label_MH_Admin"/>, initialisé à la chaîne vide
        /// et alimenté par la surcharge <see cref="LoadLabels"/> via
        /// la résolution de la clé <c>MH_Ti_18</c>. Les mutations
        /// passent par <c>SetProperty</c> hérité de
        /// <see cref="VM_Generic"/> pour émettre la notification
        /// <c>PropertyChanged</c>.
        /// </summary>
        private string _label_mh_admin = string.Empty;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Référence propre au UseCase de navigation
        /// <see cref="IU_Navigation"/> (EA-05), rétention de la même
        /// instance Singleton reçue par le constructeur et transmise à
        /// <c>base(...)</c>. Le champ homonyme du socle
        /// <see cref="VM_MH_Generic"/> étant <c>private</c>, cette
        /// rétention rend la dépendance accessible au handler propre
        /// <see cref="ExecuteAdminAsync"/>. Consommée exclusivement au
        /// travers du filet de sécurité hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>.
        /// </summary>
        private readonly IU_Navigation _navigation;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Commande de navigation applicative ouvrant la page
        /// d'administration des utilisateurs (Page04).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Commande à parité de nature avec les cinq
        /// commandes transverses du socle
        /// <see cref="VM_MH_Generic"/> — elle déclenche une navigation
        /// et non un traitement métier. Instance de
        /// <see cref="UT_RelayCommandArg0Async"/> composée au
        /// constructeur, câblée sur le handler privé
        /// <see cref="ExecuteAdminAsync"/> avec une garde
        /// <c>CanExecute</c> fixée à <c>!IsProcessing</c>
        /// (anti-réentrance identique aux commandes du socle).</para>
        /// <para>Sortie : ouverture de Page04 déléguée à
        /// <see cref="IU_Navigation.NavigateToPageAsync"/>. Aucun cas
        /// d'échec métier propre : la résolution d'accès et le
        /// traitement terminal des erreurs sont délégués au UseCase de
        /// navigation et au filet
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>.</para>
        /// </remarks>
        public ICommand AdminCommand { get; }

        /// <summary>
        /// Libellé multilingue propre du bouton « Admin », bindé sur
        /// le <c>TextBlock</c> du bouton <c>MH_Admin</c> de la vue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Libellé observable alimenté par la clé
        /// multilingue <c>MH_Ti_18</c> au travers de la surcharge
        /// <see cref="LoadLabels"/>. Le setter est <c>private</c> :
        /// la propriété n'est mutée qu'en interne, via
        /// <c>SetProperty</c> hérité de <see cref="VM_Generic"/> pour
        /// émettre la notification <c>PropertyChanged</c>.</para>
        /// </remarks>
        public string Label_MH_Admin
        {
            get => _label_mh_admin;
            private set => SetProperty(ref _label_mh_admin, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_MH01"/>.
        /// </summary>
        /// <remarks>
        /// <para>Séquence d'initialisation :</para>
        ///
        /// <para>Délègue d'abord à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c> (validation des quatre dépendances,
        /// composition des cinq commandes transverses). Retient
        /// ensuite la référence de navigation dans le champ propre
        /// <c>_navigation</c>, compose la commande de navigation propre
        /// <see cref="AdminCommand"/> (garde <c>!IsProcessing</c>),
        /// puis invoque <see cref="VM_Generic.InitializeLabels"/> en
        /// DERNIÈRE instruction (R-4.11.8 du 0231) afin de déclencher
        /// l'alimentation des libellés — les quatre transverses via
        /// <c>base.LoadLabels</c> et le libellé propre
        /// <see cref="Label_MH_Admin"/> via la surcharge
        /// <see cref="LoadLabels"/>.</para>
        ///
        /// <para>Filet de sécurité :</para>
        ///
        /// <para>La validation non-nulle des quatre paramètres est
        /// portée par la chaîne <c>base(...)</c> ; à l'entrée du corps
        /// du présent constructeur, <paramref name="navigation"/> est
        /// donc nécessairement non <see langword="null"/>, ce qui rend
        /// la rétention directe sûre.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c>. Injecté en Singleton par le conteneur
        /// DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à
        /// <see cref="VM_MH_Generic"/> via <c>base(...)</c>.
        /// Mobilisé uniquement par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, non utilisé
        /// directement par le présent ViewModel. Injecté en
        /// Singleton par le conteneur DI au titre de l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_MH_Generic"/> via
        /// <c>base(...)</c> pour l'alimentation de la mécanique
        /// multilingue factorisée par <see cref="VM_Generic"/>. Le
        /// présent dérivé ne stocke pas cette dépendance ni n'y
        /// accède directement, conformément à I-4.11.11 du 0231.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="navigation">UseCase de navigation, transmis
        /// à <see cref="VM_MH_Generic"/> via <c>base(...)</c> et
        /// retenu par le présent dérivé dans son champ propre
        /// <c>_navigation</c> (même instance Singleton). Consommé par
        /// les cinq handlers hérités du socle et par le handler propre
        /// <see cref="ExecuteAdminAsync"/> au titre de l'EA-05.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée par la
        /// chaîne <c>base(...)</c> si l'un des quatre paramètres est
        /// <see langword="null"/>.</exception>
        public VM_MH01(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IU_Navigation navigation)
            : base(dictionary, logAndNotify, app, navigation)
        {
            _navigation = navigation;

            AdminCommand = new UT_RelayCommandArg0Async(
                ExecuteAdminAsync, () => !IsProcessing);

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Alimente les libellés multilingues du menu horizontal :
        /// les quatre libellés transverses du socle puis le libellé
        /// propre <see cref="Label_MH_Admin"/>.
        /// </summary>
        /// <param name="callChain">CallChain courante propagée par la
        /// mécanique multilingue héritée
        /// (<see cref="VM_Generic.InitializeLabels"/> au premier appel,
        /// puis à chaque changement de langue), transmise telle quelle
        /// à <c>base.LoadLabels</c> et à
        /// <see cref="IS_Dictionary.GetText"/>.</param>
        /// <remarks>
        /// <para>Contexte : Surcharge nominative de
        /// <see cref="VM_MH_Generic.LoadLabels"/>. L'appel à
        /// <c>base.LoadLabels(callChain)</c> est IMPÉRATIVEMENT la
        /// première instruction du corps, afin de préserver
        /// l'alimentation des quatre libellés transverses du socle
        /// (<c>MH_Ti_01</c> à <c>MH_Ti_04</c>) : leur omission
        /// constituerait une non-conformité au contrat de la mécanique
        /// multilingue de la famille MH. Le libellé propre est ensuite
        /// résolu depuis la clé <c>MH_Ti_18</c>.</para>
        /// </remarks>
        protected override void LoadLabels(string callChain)
        {
            base.LoadLabels(callChain);

            Label_MH_Admin = _dictionary.GetText(callChain, "MH_Ti_18");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler de la commande <see cref="AdminCommand"/> :
        /// déclenche la navigation vers la page d'administration des
        /// utilisateurs (Page04), sur le patron strictement identique
        /// aux cinq handlers transverses du socle.
        /// </summary>
        /// <returns>Tâche asynchrone représentant l'exécution de la
        /// navigation.</returns>
        /// <remarks>
        /// <para>Contexte : Encadre l'invocation de
        /// <see cref="IU_Navigation.NavigateToPageAsync"/> par le
        /// pattern <c>BeginProcessing</c> / <c>try</c> / <c>finally</c>
        /// / <c>EndProcessing</c> (garantissant la remise à
        /// <see langword="false"/> de
        /// <see cref="VM_MH_Generic.IsProcessing"/>) et par le filet
        /// de sécurité hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// alimenté par une CallChain initiale construite via
        /// <c>BuildFirstCallChain</c>. Le traitement terminal des
        /// erreurs est intégralement délégué au filet ; aucun cas
        /// d'échec métier propre n'est traité ici.</para>
        /// </remarks>
        private async Task ExecuteAdminAsync()
        {
            BeginProcessing();
            try
            {
                string callChain = BuildFirstCallChain();
                await ExecuteSafeAsync(callChain, async () =>
                {
                    await _navigation.NavigateToPageAsync(callChain, "Page04");
                });
            }
            finally
            {
                EndProcessing();
            }
        }

        #endregion
    }
}