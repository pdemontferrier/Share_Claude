using System.Diagnostics;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.D_Presentation.ViewModels.Pages;
using DG244Cutting.D_Presentation.Views.Generic;

namespace DG244Cutting.D_Presentation.Views.Pages
{
    /// <summary>
    /// Vue WPF de la page de consultation détaillée d'une série de
    /// production <c>Page11</c> de l'application DG244Cutting, présentant
    /// dans un <c>TabControl</c> à cinq onglets la fiche de synthèse de la
    /// série (premier onglet), le tableau des commandes clients qui
    /// composent la série (deuxième onglet), le tableau des châssis qui la
    /// composent physiquement (troisième onglet), le tableau des barres
    /// retenues par l'optimisation pour la découpe (quatrième onglet) et
    /// le tableau des découpes à réaliser pour la série (cinquième
    /// onglet).
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille Page de la couche
    /// <c>D_Presentation</c>, vue concrète associée au ViewModel
    /// <see cref="VM_Page11"/> par identifiant commun <c>11</c>. La page
    /// est atteinte par un opérateur d'atelier de découpe depuis le
    /// tableau de bord des séries de production <c>Page10</c>, à
    /// l'ouverture d'une série. Elle est strictement en lecture : elle
    /// n'expose aucune commande utilisateur et n'attend aucune saisie ;
    /// les trois cases à cocher d'état d'avancement sont désactivées. La
    /// sortie s'effectue exclusivement par les boutons transverses du
    /// menu horizontal <c>MH11</c>, hors périmètre de la présente
    /// vue.</para>
    ///
    /// <para>Objectif : Assurer le câblage Vue/ViewModel et la mécanique
    /// de plateforme WPF de la page :</para>
    /// <list type="bullet">
    ///   <item><description>Résoudre <see cref="VM_Page11"/> au
    ///   constructeur et l'affecter au
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/> pour
    ///   activer les cent cinquante-et-un bindings déclarés par
    ///   <c>Page11.xaml</c>.</description></item>
    ///   <item><description>Appliquer au <c>Loaded</c> la stylisation
    ///   invariante des quatre-vingt-trois contrôles XAML nommés stylisables via
    ///   le service <c>IS_ControlStyler</c> hérité de
    ///   <see cref="Page_Generic"/>.</description></item>
    ///   <item><description>Ajuster au <c>Loaded</c> puis à chaque
    ///   <c>SizeChanged</c> la hauteur du <c>TabControl</c> à la hauteur
    ///   de fenêtre courante lue sur <c>ISE_Window</c>, et celles des
    ///   <c>ScrollViewer</c> des deuxième, troisième, quatrième et
    ///   cinquième onglets
    ///   par dérivation de la précédente.</description></item>
    ///   <item><description>Amorcer au <c>Loaded</c> le chargement
    ///   asynchrone des sept caractéristiques de la fiche de synthèse et
    ///   du tableau des commandes clients de la série par invocation de
    ///   <see cref="VM_Page11.LoadAsync"/>, au titre de l'ancrage
    ///   canonique <c>OnLoadedAsync</c> →
    ///   <c>LoadAsync</c>.</description></item>
    ///   <item><description>Amorcer à l'activation du troisième onglet le
    ///   chargement asynchrone du tableau des châssis par invocation de
    ///   <see cref="VM_Page11.LoadChassisAsync"/>, à l'activation du
    ///   quatrième onglet celui du tableau des barres par invocation de
    ///   <see cref="VM_Page11.LoadBarsAsync"/>, et à l'activation du
    ///   cinquième onglet celui du tableau des découpes par invocation de
    ///   <see cref="VM_Page11.LoadCutPiecesAsync"/>, au moyen d'un unique
    ///   handler
    ///   d'événement propre branché au constructeur sur le
    ///   <c>SelectionChanged</c> du <c>TabControl</c>
    ///   principal, dont l'aiguillage compte trois
    ///   destinations.</description></item>
    /// </list>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Résolution du ViewModel par le canal
    ///   <c>App.ServiceProvider.GetRequiredService</c> et affectation du
    ///   <c>DataContext</c>, selon la séquence d'initialisation en trois
    ///   temps imposée par §4.15.11 du 0230.</description></item>
    ///   <item><description>Redéfinition des trois points d'extension
    ///   <see cref="ApplyLayout"/>, <see cref="OnLoadedAsync"/> et
    ///   <see cref="OnResized"/> exposés par
    ///   <see cref="Page_Generic"/>.</description></item>
    ///   <item><description>Branchement au constructeur, puis service, du
    ///   handler d'événement propre <c>OnTabSelectionChanged</c> sur le
    ///   <c>SelectionChanged</c> du <c>TabControl</c> principal, seul
    ///   dispositif de plateforme propre à la présente vue au-delà des
    ///   points d'extension du socle.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune logique métier, règle de gestion,
    ///   transformation de données ni logique d'orchestration dans le
    ///   code-behind, conformément à I-4.12.1 du 0231. Le formatage
    ///   d'affichage des deux dates relève d'un convertisseur déclaré en
    ///   ressource de page, non du code-behind.</description></item>
    ///   <item><description>Aucun chargement de libellé multilingue
    ///   depuis la présente vue ni depuis son XAML, conformément à
    ///   I-4.11.10 du 0231 : les quarante-huit libellés sont chargés
    ///   exclusivement
    ///   par l'override de <c>LoadLabels</c> de
    ///   <see cref="VM_Page11"/>.</description></item>
    ///   <item><description>Aucune injection ni résolution directe d'un
    ///   contrat <c>IU_</c> ou <c>IQ_</c>, conformément à I-4.10.10 du
    ///   0231 : la consommation du Query Handler est intégralement portée
    ///   par le ViewModel via <c>IS_UseCaseInvoker</c>.</description></item>
    ///   <item><description>Aucun style WPF défini en XAML : la
    ///   stylisation visuelle est intégralement appliquée au
    ///   <c>Loaded</c> par <see cref="ApplyLayout"/> via
    ///   <c>IS_ControlStyler</c>.</description></item>
    ///   <item><description>Aucune libération asynchrone de ressources au
    ///   démontage : la page ne détient ni timer, ni abonnement externe,
    ///   ni ressource non managée propre ;
    ///   <c>OnUnloadedAsync</c> n'est pas redéfinie, l'implémentation par
    ///   défaut de <see cref="Page_Generic"/> suffisant.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales : La résolution de
    /// <see cref="VM_Page11"/> par
    /// <c>App.ServiceProvider.GetRequiredService</c> relève de l'EA-02
    /// « Service Locator via App.ServiceProvider dans Page_Generic »,
    /// étendue aux dérivés directs de <see cref="Page_Generic"/> pour la
    /// seule résolution de leur ViewModel (§4.15.11 du 0230). Cette
    /// dérogation procède de la contrainte technique du framework WPF de
    /// navigation, qui instancie les pages par
    /// <c>Activator.CreateInstance</c> au sein de
    /// <c>SR_Navigation.NavigateToPage</c> (R-4.12.23 du 0231) — signature
    /// sans paramètre incompatible avec l'injection paramétrée nominale.
    /// L'EA-04 (« <c>Page_Generic</c> concrète non-<c>abstract</c> ») est
    /// portée par le socle et non re-déclarée à ce niveau.</para>
    ///
    /// <para>Extension du périmètre d'EA-03 au handler d'événement propre :
    /// L'EA-03 (« Handlers WPF <c>async void</c> ») est portée par le socle
    /// <see cref="Page_Generic"/>, dont les trois handlers privés
    /// <c>OnLoadedHandler</c>, <c>OnUnloadedHandler</c> et
    /// <c>OnSizeChangedHandler</c> constituent le périmètre nominal. Le
    /// handler d'événement propre <c>OnTabSelectionChanged</c> du présent
    /// dérivé porte à son tour une signature <c>async void</c>, imposée par
    /// la signature de <see cref="System.Windows.Controls.SelectionChangedEventHandler"/>
    /// et par la nécessité d'attendre l'appel asynchrone au ViewModel. Ce
    /// cas se situe hors du périmètre d'EA-03 tel que rédigé à ce jour. Il
    /// est admis par arbitrage explicite du développeur, sous la condition
    /// stricte du filet ultime calqué sur le patron du socle : le corps
    /// utile est intégralement encapsulé dans un <c>try/catch</c> qui trace
    /// par <see cref="System.Diagnostics.Debug.WriteLine(string)"/> sans
    /// propager. Cette condition est structurellement nécessaire et non
    /// décorative — <c>ExecuteSafeAsync</c> relance silencieusement
    /// <see cref="OperationCanceledException"/>, et depuis un
    /// <c>async void</c> non protégé cette relance atteindrait
    /// <see cref="System.Windows.Application.DispatcherUnhandledException"/>,
    /// le filet ultime de <c>Page_Generic.OnLoadedHandler</c> ne couvrant
    /// pas ce chemin d'exécution. L'amendement de §4.15.7 du 0230 et de
    /// l'entrée EA-03 du 0231 aux fins d'étendre nominalement ce périmètre
    /// relève d'un fil de maintenance normative distinct.</para>
    ///
    /// <para>Absence délibérée de stylisation des cases à cocher :
    /// Les trois <c>CheckBox</c> de la fiche de synthèse
    /// (<c>CuttingStartedCheckBox</c>, <c>CuttingCompletedCheckBox</c>,
    /// <c>BarOutOfStockCheckBox</c>) ne sont ni résolues ni stylisées par
    /// <see cref="ApplyLayout"/> : le contrat <c>IS_ControlStyler</c>
    /// n'expose aucune méthode dédiée à ce type de contrôle. Il en va de
    /// même des cinq <c>CheckBox</c> du gabarit d'éléments du tableau des
    /// barres du quatrième onglet et des quatre <c>CheckBox</c> du gabarit
    /// d'éléments du tableau des découpes du cinquième, qui sont au
    /// surplus instanciées par le
    /// <c>DataTemplate</c> et donc hors de portée de la résolution par nom.
    /// Les douze cases
    /// conservent le rendu par défaut du framework WPF. Cette absence est
    /// délibérée et documentée ; aucune extension du contrat n'est
    /// produite.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2) complétée par une extension (R-4.4.10 du 0231) :
    /// l'extension <c>=== Méthodes protégées ===</c> au titre des trois
    /// overrides de points d'extension. Soit six régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> : quatre
    ///   constantes — <see cref="HeaderWidth"/>, largeur uniforme des cinq
    ///   en-têtes d'onglets, <see cref="FramesTabIndex"/>, indice du
    ///   troisième onglet dans le <c>TabControl</c>,
    ///   <see cref="BarsTabIndex"/>, indice du quatrième onglet, et
    ///   <see cref="CuttingsTabIndex"/>, indice du cinquième
    ///   onglet.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> : champ
    ///   <see cref="_viewModel"/> stockant l'instance Singleton de
    ///   <see cref="VM_Page11"/> résolue au constructeur via
    ///   <c>App.ServiceProvider.GetRequiredService</c>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   sans paramètre <c>public</c> imposé par le framework WPF de
    ///   navigation, résolvant <see cref="VM_Page11"/>, l'affectant à
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/> puis
    ///   branchant le handler d'événement propre sur le
    ///   <c>SelectionChanged</c> du <c>TabControl</c> principal.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>. La page n'expose
    ///   aucun membre public au-delà de son
    ///   constructeur.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> : overrides
    ///   <see cref="ApplyLayout"/>, <see cref="OnLoadedAsync"/> et
    ///   <see cref="OnResized"/>. Aucun override d'<c>OnUnloadedAsync</c>,
    ///   l'implémentation par défaut de <see cref="Page_Generic"/>
    ///   suffisant.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : handler
    ///   d'événement propre <c>OnTabSelectionChanged</c>, seul membre de
    ///   la région ; le marqueur <c>// A compléter</c> est en
    ///   conséquence retiré.</description></item>
    /// </list>
    /// </remarks>
    public partial class Page11 : Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Largeur uniforme, en unités indépendantes du périphérique,
        /// appliquée aux cinq en-têtes d'onglets du <c>TabControl</c> par
        /// <see cref="ApplyLayout"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constante consommée en troisième argument des
        /// cinq invocations de
        /// <c>IS_ControlStyler.StyleTabItem(TabItem, TextBlock, double)</c>,
        /// dont le paramètre de largeur n'est pas optionnel. La
        /// centralisation en constante privée garantit l'uniformité
        /// visuelle des cinq en-têtes et rend le réglage modifiable en un
        /// point unique. La valeur reprend celle appliquée aux en-têtes
        /// d'onglets de <c>Page01</c>.</para>
        /// </remarks>
        private const double HeaderWidth = 150;

        /// <summary>
        /// Indice, dans la collection d'onglets du <c>TabControl</c>
        /// <c>MainTabControl</c>, de l'onglet des châssis dont
        /// l'activation déclenche le chargement du tableau.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constante consommée par la branche
        /// correspondante de l'aiguillage de
        /// <see cref="OnTabSelectionChanged"/>, qui compare l'indice
        /// sélectionné aux indices des onglets à chargement différé et
        /// sort immédiatement lorsqu'aucun ne correspond. La valeur
        /// <c>2</c> correspond à la position de
        /// <c>FramesTabItem</c> dans l'ordre de déclaration du XAML
        /// (<c>SeriesTabItem</c> 0, <c>OrdersTabItem</c> 1,
        /// <c>FramesTabItem</c> 2, <c>BarsTabItem</c> 3,
        /// <c>CuttingsTabItem</c> 4). La centralisation en constante
        /// privée rend le réglage modifiable en un point unique et évite
        /// le littéral numérique nu dans le corps du handler, à parité de
        /// forme avec <see cref="HeaderWidth"/>.</para>
        /// </remarks>
        private const int FramesTabIndex = 2;

        /// <summary>
        /// Indice, dans la collection d'onglets du <c>TabControl</c>
        /// <c>MainTabControl</c>, de l'onglet des barres dont
        /// l'activation déclenche le chargement du tableau.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constante consommée par la branche
        /// correspondante de l'aiguillage de
        /// <see cref="OnTabSelectionChanged"/>, à parité stricte de forme
        /// et d'usage avec <see cref="FramesTabIndex"/>. La valeur
        /// <c>3</c> correspond à la position de <c>BarsTabItem</c> dans
        /// l'ordre de déclaration du XAML (<c>SeriesTabItem</c> 0,
        /// <c>OrdersTabItem</c> 1, <c>FramesTabItem</c> 2,
        /// <c>BarsTabItem</c> 3, <c>CuttingsTabItem</c> 4). La
        /// centralisation en constante privée rend le réglage modifiable
        /// en un point unique et évite le littéral numérique nu dans le
        /// corps du handler.</para>
        /// </remarks>
        private const int BarsTabIndex = 3;

        /// <summary>
        /// Indice, dans la collection d'onglets du <c>TabControl</c>
        /// <c>MainTabControl</c>, de l'onglet des découpes dont
        /// l'activation déclenche le chargement du tableau.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constante consommée par la branche
        /// correspondante de l'aiguillage de
        /// <see cref="OnTabSelectionChanged"/>, à parité stricte de forme
        /// et d'usage avec <see cref="FramesTabIndex"/> et
        /// <see cref="BarsTabIndex"/>. La valeur <c>4</c> correspond à la
        /// position de <c>CuttingsTabItem</c> dans l'ordre de déclaration
        /// du XAML (<c>SeriesTabItem</c> 0, <c>OrdersTabItem</c> 1,
        /// <c>FramesTabItem</c> 2, <c>BarsTabItem</c> 3,
        /// <c>CuttingsTabItem</c> 4). La centralisation en constante
        /// privée rend le réglage modifiable en un point unique et évite
        /// le littéral numérique nu dans le corps du handler.</para>
        /// </remarks>
        private const int CuttingsTabIndex = 4;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Instance Singleton du ViewModel associé à la présente vue,
        /// résolue au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> et affectée à
        /// <see cref="System.Windows.FrameworkElement.DataContext"/> pour
        /// alimenter les bindings WPF déclarés par <c>Page11.xaml</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Champ stocké en lecture seule pour exposer le
        /// type concret <see cref="VM_Page11"/> au code-behind, distinct
        /// du <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// typé en <see cref="object"/>. Ses usages locaux au-delà
        /// de l'affectation du <c>DataContext</c> sont l'invocation de
        /// <see cref="VM_Page11.LoadAsync"/> depuis
        /// <see cref="OnLoadedAsync"/>, au titre de l'ancrage canonique
        /// entre les deux socles génériques de la famille, et celles de
        /// <see cref="VM_Page11.LoadChassisAsync"/>, de
        /// <see cref="VM_Page11.LoadBarsAsync"/> et de
        /// <see cref="VM_Page11.LoadCutPiecesAsync"/> depuis
        /// <see cref="OnTabSelectionChanged"/> — seules
        /// invocations de membres du ViewModel admises depuis une vue,
        /// conformément à la séparation MVVM stricte.</para>
        /// </remarks>
        private readonly VM_Page11 _viewModel;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="Page11"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur sans paramètre imposé par le
        /// framework WPF de navigation, qui instancie la page via
        /// <c>Activator.CreateInstance</c> au sein de
        /// <c>SR_Navigation.NavigateToPage</c> (R-4.12.23 du 0231). La
        /// résolution des dépendances ne peut donc se faire par injection
        /// paramétrée et s'effectue par le canal légitime
        /// <c>App.ServiceProvider.GetRequiredService</c>, au titre de la
        /// convention de plateforme documentée en §4.15.11 du 0230 et de
        /// l'EA-02 Service Locator étendue aux dérivés directs de
        /// <see cref="Page_Generic"/> pour la résolution de leur
        /// ViewModel.</para>
        ///
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Résolution de <see cref="VM_Page11"/> via
        ///   <c>App.ServiceProvider.GetRequiredService</c> et stockage
        ///   dans le champ <see cref="_viewModel"/>. La méthode
        ///   <c>GetRequiredService</c> est utilisée (et non
        ///   <c>GetService</c>), conformément à la règle 2 de §4.15.11 du
        ///   0230 : toute dépendance non résolue doit faire échouer
        ///   l'instanciation immédiatement par exception explicite plutôt
        ///   que de produire une <see cref="NullReferenceException"/>
        ///   ultérieure.</description></item>
        ///   <item><description>Invocation de <c>InitializeComponent</c>
        ///   pour la composition XAML — étape impérativement préalable à
        ///   toute affectation de
        ///   <see cref="System.Windows.FrameworkElement.DataContext"/>.</description></item>
        ///   <item><description>Affectation de
        ///   <see cref="System.Windows.FrameworkElement.DataContext"/> à
        ///   <see cref="_viewModel"/> pour activer les cent
        ///   cinquante-et-un
        ///   bindings déclarés par <c>Page11.xaml</c> : cinquante-neuf
        ///   liaisons de libellé
        ///   multilingue, sept caractéristiques de la série et les quatre
        ///   collections portées par le
        ///   <c>DataContext</c> de page — soit soixante-dix — plus
        ///   quatre bindings de données
        ///   de commande portés par l'élément courant du gabarit
        ///   d'éléments de la <c>ListView</c> du deuxième onglet, onze
        ///   bindings de données de châssis portés par celui du troisième
        ///   onglet, trente-huit bindings portés par celui du quatrième
        ///   onglet, dont seize de contenu et vingt-deux de marquage
        ///   visuel, et vingt-huit bindings portés par celui du cinquième
        ///   onglet, dont douze de contenu texte, douze de marquage et
        ///   quatre de case à cocher — soit quatre-vingt-un
        ///   au total.</description></item>
        ///   <item><description>Branchement du handler d'événement propre
        ///   <see cref="OnTabSelectionChanged"/> sur l'événement
        ///   <c>SelectionChanged</c> du <c>TabControl</c>
        ///   <c>MainTabControl</c>, strictement après l'affectation du
        ///   <c>DataContext</c>.</description></item>
        /// </list>
        ///
        /// <para>Canal de résolution du <c>TabControl</c> : Le patron
        /// <see cref="Page_Generic.Find{T}(string)"/> assorti d'une garde
        /// <c>is</c> est retenu, par uniformité avec
        /// <see cref="ApplyLayout"/> et <see cref="OnResized"/> et parce
        /// que R-4.15.25 du 0231 proscrit de franchir le pont de
        /// résolution sans garde. L'accès direct au champ généré par le
        /// compilateur XAML n'est pas retenu : il court-circuiterait ce
        /// patron et la trace de diagnostic qui l'accompagne.</para>
        ///
        /// <para>Absence de désabonnement : Aucun désabonnement n'est
        /// posé, à parité avec la doctrine du socle sur le cycle de vie de
        /// navigation WPF documentée en §4.15.7 du 0230. La page et son
        /// <c>TabControl</c> ont une durée de vie commune ; l'abonnement
        /// disparaît avec l'instance.</para>
        ///
        /// <para>Le constructeur de <see cref="Page_Generic"/> est invoqué
        /// implicitement en amont : il résout <c>IS_ControlStyler</c> et
        /// <c>ISE_Window</c> par le même canal, initialise le champ
        /// <c>_callee</c> et branche les trois handlers privés sur les
        /// événements <c>Loaded</c>, <c>Unloaded</c> et
        /// <c>SizeChanged</c>.</para>
        ///
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le constructeur
        /// au-delà de la résolution du ViewModel. Une défaillance de
        /// <c>GetRequiredService</c> traduirait une erreur de
        /// configuration du conteneur DI et doit faire échouer
        /// l'instanciation immédiatement. Le filet de sécurité ultime au
        /// bord des handlers WPF est porté par
        /// <see cref="Page_Generic"/> et couvre les éventuelles
        /// défaillances survenant au chargement, au déchargement et au
        /// redimensionnement de la page.</para>
        /// </remarks>
        public Page11()
        {
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page11>();

            InitializeComponent();

            DataContext = _viewModel;

            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl)
                mainTabControl.SelectionChanged += OnTabSelectionChanged;
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.ApplyLayout"/> pour appliquer la
        /// stylisation invariante des quatre-vingt-trois contrôles XAML nommés
        /// stylisables de la page : la <c>Grid</c> de page, le
        /// <c>TabControl</c> principal, les cinq onglets et leurs
        /// en-têtes, le <c>Border</c> de la fiche de synthèse, les sept
        /// intitulés et les quatre <c>TextBlock</c> de donnée du premier
        /// onglet, le <c>Border</c> d'en-têtes, le <c>ScrollViewer</c>,
        /// les quatre <c>TextBlock</c> d'en-tête et la <c>ListView</c> du
        /// deuxième onglet, le <c>Border</c> d'en-têtes, le
        /// <c>ScrollViewer</c>, les onze <c>TextBlock</c> d'en-tête et la
        /// <c>ListView</c> du troisième onglet, le <c>Border</c>
        /// d'en-têtes, le <c>ScrollViewer</c>, les seize <c>TextBlock</c>
        /// d'en-tête et la <c>ListView</c> du quatrième onglet, puis le
        /// <c>Border</c> d'en-têtes, le <c>ScrollViewer</c>, les seize
        /// <c>TextBlock</c> d'en-tête et la <c>ListView</c> du cinquième
        /// onglet.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <see cref="Page_Generic"/> à
        /// l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de la
        /// page, en première position de la séquence
        /// <c>ApplyLayout</c> → <c>OnResized</c> → <c>OnLoadedAsync</c>.
        /// Le caractère synchrone est imposé par la signature du point
        /// d'extension de <see cref="Page_Generic"/> (§4.15.7 du 0230).
        /// La <paramref name="callChain"/> reçue est construite par le
        /// handler sous la forme
        /// <c>Page11 &gt; OnLoadedHandler &gt; ApplyLayout</c>.</para>
        ///
        /// <para>Objectif : Appliquer la stylisation visuelle via le
        /// service <c>IS_ControlStyler</c> hérité de
        /// <see cref="Page_Generic"/> (champ
        /// <see cref="Page_Generic._controlStyler"/>), selon la séquence
        /// suivante, ordonnée sur la déclaration du XAML :</para>
        /// <list type="number">
        ///   <item><description><c>StylePage</c> sur la <c>Grid</c> de
        ///   page <c>PageGrid</c> (fond, marges,
        ///   alignements).</description></item>
        ///   <item><description><c>StyleTabControl</c> sur le
        ///   <c>TabControl</c> <c>MainTabControl</c>.</description></item>
        ///   <item><description><c>StyleTabItem</c> sur les cinq couples
        ///   <c>TabItem</c> + <c>TextBlock</c> d'en-tête, à la largeur
        ///   uniforme <see cref="HeaderWidth"/>. La garde <c>is</c> est
        ///   composée sur le couple : la méthode du contrat exigeant
        ///   conjointement les deux contrôles en paramètres
        ///   non-nullable, la stylisation d'un onglet n'est engagée que
        ///   si l'onglet et son en-tête sont tous deux
        ///   résolus.</description></item>
        ///   <item><description><c>StyleBorder</c> sur le <c>Border</c>
        ///   de la fiche de synthèse
        ///   <c>SeriesDetailsBorder</c>.</description></item>
        ///   <item><description><c>StyleTextBlockTitle</c> à la largeur
        ///   <c>300</c> sur les sept <c>TextBlock</c> d'intitulé de la
        ///   colonne 0 de la fiche, en cohérence avec la première
        ///   <c>ColumnDefinition</c> de la <c>Grid</c> de
        ///   fiche.</description></item>
        ///   <item><description><c>StyleTextBlockData</c> sans largeur
        ///   imposée sur les quatre <c>TextBlock</c> de donnée de la
        ///   colonne 1 de la fiche, dont la largeur est portée par la
        ///   seconde <c>ColumnDefinition</c>
        ///   étoilée.</description></item>
        ///   <item><description><c>StyleBorderHeader</c> sur le
        ///   <c>Border</c> d'en-têtes du tableau des commandes
        ///   <c>OrdersHeaderBorder</c>, du deuxième onglet.</description></item>
        ///   <item><description><c>StyleScrollViewer</c> sur le
        ///   <c>ScrollViewer</c> <c>OrdersScrollViewer</c>, en invocation
        ///   variadique unique portant en outre le <c>Border</c>
        ///   d'en-têtes et les quatre <c>TextBlock</c> d'en-tête
        ///   <c>OrdersHeader01</c> à
        ///   <c>OrdersHeader04</c>.</description></item>
        ///   <item><description><c>StyleListView</c> sur la
        ///   <c>ListView</c> <c>OrdersListView</c>, qui porte le rendu
        ///   des quatre <c>TextBlock</c> non nommés de son gabarit
        ///   d'éléments.</description></item>
        ///   <item><description><c>StyleBorderHeader</c> sur le
        ///   <c>Border</c> d'en-têtes du tableau des châssis
        ///   <c>FramesHeaderBorder</c>, du troisième onglet.</description></item>
        ///   <item><description><c>StyleScrollViewer</c> sur le
        ///   <c>ScrollViewer</c> <c>FramesScrollViewer</c>, en invocation
        ///   variadique unique portant en outre le <c>Border</c>
        ///   d'en-têtes et les onze <c>TextBlock</c> d'en-tête
        ///   <c>FramesHeader01</c> à
        ///   <c>FramesHeader11</c>.</description></item>
        ///   <item><description><c>StyleListView</c> sur la
        ///   <c>ListView</c> <c>FramesListView</c>, qui porte le rendu
        ///   des onze <c>TextBlock</c> non nommés de son gabarit
        ///   d'éléments.</description></item>
        ///   <item><description><c>StyleBorderHeader</c> sur le
        ///   <c>Border</c> d'en-têtes du tableau des barres
        ///   <c>BarsHeaderBorder</c>, du quatrième onglet.</description></item>
        ///   <item><description><c>StyleScrollViewer</c> sur le
        ///   <c>ScrollViewer</c> <c>BarsScrollViewer</c>, en invocation
        ///   variadique unique portant en outre le <c>Border</c>
        ///   d'en-têtes et les seize <c>TextBlock</c> d'en-tête
        ///   <c>BarsHeader01</c> à
        ///   <c>BarsHeader16</c>.</description></item>
        ///   <item><description><c>StyleListView</c> sur la
        ///   <c>ListView</c> <c>BarsListView</c>, qui porte le rendu
        ///   des onze <c>TextBlock</c> et cinq <c>CheckBox</c> non nommés
        ///   de son gabarit d'éléments.</description></item>
        ///   <item><description><c>StyleBorderHeader</c> sur le
        ///   <c>Border</c> d'en-têtes du tableau des découpes
        ///   <c>CuttingsHeaderBorder</c>, du cinquième onglet.</description></item>
        ///   <item><description><c>StyleScrollViewer</c> sur le
        ///   <c>ScrollViewer</c> <c>CuttingsScrollViewer</c>, en
        ///   invocation variadique unique portant en outre le
        ///   <c>Border</c> d'en-têtes et les seize <c>TextBlock</c>
        ///   d'en-tête <c>CuttingsHeader01</c> à
        ///   <c>CuttingsHeader16</c>.</description></item>
        ///   <item><description><c>StyleListView</c> sur la
        ///   <c>ListView</c> <c>CuttingsListView</c>, qui porte le rendu
        ///   des douze <c>TextBlock</c> et quatre <c>CheckBox</c> non
        ///   nommés de son gabarit d'éléments.</description></item>
        /// </list>
        ///
        /// <para>Blocs <c>StyleScrollViewer</c> variadiques : Le contrat
        /// <c>IS_ControlStyler.StyleScrollViewer</c> expose un premier
        /// paramètre non-nullable — le <c>ScrollViewer</c> lui-même —
        /// suivi de vingt-deux paramètres nullables optionnels : un
        /// <c>TextBlock</c> de titre, un <c>Border</c> de bandeau et vingt
        /// <c>TextBlock</c> d'en-têtes — plafond de vingt en-têtes que les
        /// seize colonnes du quatrième onglet comme celles du cinquième
        /// n'atteignent pas, aucune
        /// extension du contrat n'étant donc requise. Chaque bloc dédié conditionne
        /// l'invocation à la seule résolution du <c>ScrollViewer</c>
        /// (garde <c>is</c>) ; la résolution du <c>Border</c> et des
        /// en-têtes est portée dans des variables locales typées
        /// <c>Border?</c> et <c>TextBlock?</c> passées directement en
        /// argument — le contrat acceptant le <c>null</c> sur ces
        /// paramètres, la garde <c>is</c> par paramètre n'est pas requise
        /// et l'invocation reste unique. Le paramètre de titre est passé
        /// à <see langword="null"/> dans les quatre blocs, aucun des
        /// quatre
        /// tableaux ne portant de titre
        /// propre ; les paramètres d'en-tête non consommés restent à leur
        /// valeur par défaut, le tableau des commandes ne comptant que
        /// quatre colonnes, celui des châssis onze, celui des barres
        /// seize et celui des découpes seize. Les
        /// <c>Border</c> d'en-têtes, par ailleurs stylisés hors de ces
        /// blocs par <c>StyleBorderHeader</c>, y sont résolus une seconde
        /// fois dans la portée locale dédiée — le helper
        /// <see cref="Page_Generic.Find{T}(string)"/>, idempotent et de
        /// coût négligeable, absorbe sans cérémonie cette double
        /// résolution, qui préserve la lisibilité et l'autonomie de
        /// chaque bloc.</para>
        ///
        /// <para>Absence délibérée de stylisation individuelle des
        /// <c>TextBlock</c> d'en-tête : Aucun des quarante-sept
        /// <c>TextBlock</c>
        /// d'en-tête des quatre tableaux ne reçoit de
        /// <c>StyleTextBlockHeader</c> individuel ; leur stylisation est
        /// intégralement portée par les blocs variadiques ci-dessus. Le
        /// traitement est uniforme entre les quatre onglets.</para>
        ///
        /// <para>Absence délibérée de stylisation des cinq <c>Grid</c>
        /// internes : Les <c>Grid</c> <c>SeriesDetailsGrid</c>,
        /// <c>OrdersGrid</c>, <c>FramesGrid</c>, <c>BarsGrid</c> et
        /// <c>CuttingsGrid</c> sont
        /// nommés sans être
        /// résolus ni stylisés.
        /// Le contrat <c>IS_ControlStyler</c> n'expose aucune méthode de
        /// stylisation de <c>Grid</c> interne — <c>StylePage</c> ne
        /// s'adresse qu'à la <c>Grid</c> racine de page. L'omission est
        /// délibérée et documentée, non un oubli.</para>
        ///
        /// <para>Absence délibérée de stylisation des cases à
        /// cocher : Les trois <c>CheckBox</c> de la colonne 1 de la fiche
        /// (<c>CuttingStartedCheckBox</c>, <c>CuttingCompletedCheckBox</c>,
        /// <c>BarOutOfStockCheckBox</c>) ne sont ni résolues ni stylisées.
        /// Le contrat <c>IS_ControlStyler</c> n'expose aucune méthode
        /// dédiée à ce type de contrôle. Il en va de même des cinq
        /// <c>CheckBox</c> du gabarit d'éléments du quatrième onglet et
        /// des quatre <c>CheckBox</c> du gabarit d'éléments du cinquième,
        /// non
        /// nommées et instanciées par le <c>DataTemplate</c>, donc hors de
        /// portée de <see cref="Page_Generic.Find{T}(string)"/> ; leur
        /// rendu relève de <c>StyleListView</c> appliqué à la
        /// <c>ListView</c>. Les douze cases conservent le
        /// rendu par défaut du framework WPF. L'omission est délibérée et
        /// documentée, non un oubli ; aucune extension du contrat n'est
        /// produite.</para>
        ///
        /// <para>Résolution typée par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> : Chaque contrôle
        /// est résolu par le helper hérité, qui combine
        /// <c>FindName(name) as T</c> avec une trace
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/> en cas
        /// d'absence ou de cast invalide. Le retour <c>T?</c> est consommé
        /// par une garde <c>is</c> qui conditionne l'invocation du service
        /// (paramètres non-nullable) au succès de la résolution, selon la
        /// forme dépliée un-à-un
        /// <c>if (Find&lt;T&gt;(name) is T x) _controlStyler.X(x)</c>
        /// prescrite par le patron normatif « Patron de surcharge —
        /// ApplyLayout » de §4.15.7 du 0230 et par R-4.15.25 du 0231, qui
        /// proscrivent explicitement l'opérateur null-forgiving
        /// (<c>!</c>) pour franchir ce pont.</para>
        ///
        /// <para>Appel à <c>base.ApplyLayout(callChain)</c> en première
        /// instruction : L'implémentation par défaut de
        /// <see cref="Page_Generic.ApplyLayout"/> ne porte aucun
        /// traitement. L'appel est néanmoins conservé en geste de
        /// robustesse vis-à-vis de toute évolution future du socle,
        /// conformément à la convention d'override standard et au patron
        /// normatif de §4.15.7 du 0230.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. La
        /// continuation gracieuse est assurée au niveau du corps : en cas
        /// d'absence ou de cast invalide d'un contrôle, la garde
        /// <c>is</c> n'engage pas l'invocation du service sur le contrôle
        /// concerné, la stylisation des contrôles suivants n'est pas
        /// interrompue, et la trace de diagnostic émise par
        /// <see cref="Page_Generic.Find{T}(string)"/> assure la
        /// détectabilité en environnement de développement. Toute
        /// exception qui parviendrait néanmoins à être levée serait
        /// capturée par le filet de sécurité ultime de
        /// <c>Page_Generic.OnLoadedHandler</c>, qui la trace sans la
        /// propager au framework WPF.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page11 &gt; OnLoadedHandler &gt; ApplyLayout</c>.</param>
        protected override void ApplyLayout(string callChain)
        {
            base.ApplyLayout(callChain);

            if (Find<Grid>("PageGrid") is Grid pageGrid) _controlStyler.StylePage(pageGrid);
            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl) _controlStyler.StyleTabControl(mainTabControl);

            // Cinq onglets — garde is composée sur le couple TabItem + TextBlock d'en-tête
            if (Find<TabItem>("SeriesTabItem") is TabItem seriesTabItem
                && Find<TextBlock>("SeriesTabHeader") is TextBlock seriesTabHeader)
                _controlStyler.StyleTabItem(seriesTabItem, seriesTabHeader, HeaderWidth);
            if (Find<TabItem>("OrdersTabItem") is TabItem ordersTabItem
                && Find<TextBlock>("OrdersTabHeader") is TextBlock ordersTabHeader)
                _controlStyler.StyleTabItem(ordersTabItem, ordersTabHeader, HeaderWidth);
            if (Find<TabItem>("FramesTabItem") is TabItem framesTabItem
                && Find<TextBlock>("FramesTabHeader") is TextBlock framesTabHeader)
                _controlStyler.StyleTabItem(framesTabItem, framesTabHeader, HeaderWidth);
            if (Find<TabItem>("BarsTabItem") is TabItem barsTabItem
                && Find<TextBlock>("BarsTabHeader") is TextBlock barsTabHeader)
                _controlStyler.StyleTabItem(barsTabItem, barsTabHeader, HeaderWidth);
            if (Find<TabItem>("CuttingsTabItem") is TabItem cuttingsTabItem
                && Find<TextBlock>("CuttingsTabHeader") is TextBlock cuttingsTabHeader)
                _controlStyler.StyleTabItem(cuttingsTabItem, cuttingsTabHeader, HeaderWidth);

            // Fiche de synthèse de l'onglet 1
            if (Find<Border>("SeriesDetailsBorder") is Border seriesDetailsBorder) _controlStyler.StyleBorder(seriesDetailsBorder);

            // Colonne 0 — sept intitulés
            if (Find<TextBlock>("SerialNumberTitle") is TextBlock serialNumberTitle) _controlStyler.StyleTextBlockTitle(serialNumberTitle, 300);
            if (Find<TextBlock>("DescriptionTitle") is TextBlock descriptionTitle) _controlStyler.StyleTextBlockTitle(descriptionTitle, 300);
            if (Find<TextBlock>("ProductionStartDateTitle") is TextBlock productionStartDateTitle) _controlStyler.StyleTextBlockTitle(productionStartDateTitle, 300);
            if (Find<TextBlock>("ProductionEndDateTitle") is TextBlock productionEndDateTitle) _controlStyler.StyleTextBlockTitle(productionEndDateTitle, 300);
            if (Find<TextBlock>("CuttingStartedTitle") is TextBlock cuttingStartedTitle) _controlStyler.StyleTextBlockTitle(cuttingStartedTitle, 300);
            if (Find<TextBlock>("CuttingCompletedTitle") is TextBlock cuttingCompletedTitle) _controlStyler.StyleTextBlockTitle(cuttingCompletedTitle, 300);
            if (Find<TextBlock>("BarOutOfStockTitle") is TextBlock barOutOfStockTitle) _controlStyler.StyleTextBlockTitle(barOutOfStockTitle, 300);

            // Colonne 1 — quatre TextBlock de donnée. Les trois CheckBox ne sont pas
            // stylisées : IS_ControlStyler n'expose aucune méthode dédiée à ce type
            // de contrôle (absence délibérée, cf. documentation XML ci-dessus).
            if (Find<TextBlock>("SerialNumberData") is TextBlock serialNumberData) _controlStyler.StyleTextBlockData(serialNumberData);
            if (Find<TextBlock>("DescriptionData") is TextBlock descriptionData) _controlStyler.StyleTextBlockData(descriptionData);
            if (Find<TextBlock>("ProductionStartDateData") is TextBlock productionStartDateData) _controlStyler.StyleTextBlockData(productionStartDateData);
            if (Find<TextBlock>("ProductionEndDateData") is TextBlock productionEndDateData) _controlStyler.StyleTextBlockData(productionEndDateData);

            // Tableau des commandes de l'onglet 2 — Border d'en-têtes
            if (Find<Border>("OrdersHeaderBorder") is Border ordersHeaderBorder) _controlStyler.StyleBorderHeader(ordersHeaderBorder);

            // Bloc StyleScrollViewer variadique : résolution typée des quatre en-têtes en variables
            // locales optionnelles, invocation unique conditionnée à la résolution du ScrollViewer.
            if (Find<ScrollViewer>("OrdersScrollViewer") is ScrollViewer ordersScrollViewer)
            {
                Border? headerBorderForScrollViewer = Find<Border>("OrdersHeaderBorder");
                TextBlock? h01 = Find<TextBlock>("OrdersHeader01");
                TextBlock? h02 = Find<TextBlock>("OrdersHeader02");
                TextBlock? h03 = Find<TextBlock>("OrdersHeader03");
                TextBlock? h04 = Find<TextBlock>("OrdersHeader04");

                _controlStyler.StyleScrollViewer(
                    ordersScrollViewer,
                    null,
                    headerBorderForScrollViewer,
                    h01, h02, h03, h04);
            }

            if (Find<ListView>("OrdersListView") is ListView ordersListView) _controlStyler.StyleListView(ordersListView);

            // Tableau des châssis de l'onglet 3 — Border d'en-têtes
            if (Find<Border>("FramesHeaderBorder") is Border framesHeaderBorder) _controlStyler.StyleBorderHeader(framesHeaderBorder);

            // Bloc StyleScrollViewer variadique : résolution typée des onze en-têtes en variables
            // locales optionnelles, invocation unique conditionnée à la résolution du ScrollViewer.
            if (Find<ScrollViewer>("FramesScrollViewer") is ScrollViewer framesScrollViewer)
            {
                Border? framesHeaderBorderForScrollViewer = Find<Border>("FramesHeaderBorder");
                TextBlock? f01 = Find<TextBlock>("FramesHeader01");
                TextBlock? f02 = Find<TextBlock>("FramesHeader02");
                TextBlock? f03 = Find<TextBlock>("FramesHeader03");
                TextBlock? f04 = Find<TextBlock>("FramesHeader04");
                TextBlock? f05 = Find<TextBlock>("FramesHeader05");
                TextBlock? f06 = Find<TextBlock>("FramesHeader06");
                TextBlock? f07 = Find<TextBlock>("FramesHeader07");
                TextBlock? f08 = Find<TextBlock>("FramesHeader08");
                TextBlock? f09 = Find<TextBlock>("FramesHeader09");
                TextBlock? f10 = Find<TextBlock>("FramesHeader10");
                TextBlock? f11 = Find<TextBlock>("FramesHeader11");

                _controlStyler.StyleScrollViewer(
                    framesScrollViewer,
                    null,
                    framesHeaderBorderForScrollViewer,
                    f01, f02, f03, f04, f05, f06, f07, f08, f09, f10, f11);
            }

            if (Find<ListView>("FramesListView") is ListView framesListView) _controlStyler.StyleListView(framesListView);

            // Tableau des barres de l'onglet 4 — Border d'en-têtes
            if (Find<Border>("BarsHeaderBorder") is Border barsHeaderBorder) _controlStyler.StyleBorderHeader(barsHeaderBorder);

            // Bloc StyleScrollViewer variadique : résolution typée des seize en-têtes en variables
            // locales optionnelles, invocation unique conditionnée à la résolution du ScrollViewer.
            if (Find<ScrollViewer>("BarsScrollViewer") is ScrollViewer barsScrollViewer)
            {
                Border? barsHeaderBorderForScrollViewer = Find<Border>("BarsHeaderBorder");
                TextBlock? b01 = Find<TextBlock>("BarsHeader01");
                TextBlock? b02 = Find<TextBlock>("BarsHeader02");
                TextBlock? b03 = Find<TextBlock>("BarsHeader03");
                TextBlock? b04 = Find<TextBlock>("BarsHeader04");
                TextBlock? b05 = Find<TextBlock>("BarsHeader05");
                TextBlock? b06 = Find<TextBlock>("BarsHeader06");
                TextBlock? b07 = Find<TextBlock>("BarsHeader07");
                TextBlock? b08 = Find<TextBlock>("BarsHeader08");
                TextBlock? b09 = Find<TextBlock>("BarsHeader09");
                TextBlock? b10 = Find<TextBlock>("BarsHeader10");
                TextBlock? b11 = Find<TextBlock>("BarsHeader11");
                TextBlock? b12 = Find<TextBlock>("BarsHeader12");
                TextBlock? b13 = Find<TextBlock>("BarsHeader13");
                TextBlock? b14 = Find<TextBlock>("BarsHeader14");
                TextBlock? b15 = Find<TextBlock>("BarsHeader15");
                TextBlock? b16 = Find<TextBlock>("BarsHeader16");

                _controlStyler.StyleScrollViewer(
                    barsScrollViewer,
                    null,
                    barsHeaderBorderForScrollViewer,
                    b01, b02, b03, b04, b05, b06, b07, b08,
                    b09, b10, b11, b12, b13, b14, b15, b16);
            }

            if (Find<ListView>("BarsListView") is ListView barsListView) _controlStyler.StyleListView(barsListView);

            // Tableau des découpes de l'onglet 5 — Border d'en-têtes
            if (Find<Border>("CuttingsHeaderBorder") is Border cuttingsHeaderBorder) _controlStyler.StyleBorderHeader(cuttingsHeaderBorder);

            // Bloc StyleScrollViewer variadique : résolution typée des seize en-têtes en variables
            // locales optionnelles, invocation unique conditionnée à la résolution du ScrollViewer.
            if (Find<ScrollViewer>("CuttingsScrollViewer") is ScrollViewer cuttingsScrollViewer)
            {
                Border? cuttingsHeaderBorderForScrollViewer = Find<Border>("CuttingsHeaderBorder");
                TextBlock? c01 = Find<TextBlock>("CuttingsHeader01");
                TextBlock? c02 = Find<TextBlock>("CuttingsHeader02");
                TextBlock? c03 = Find<TextBlock>("CuttingsHeader03");
                TextBlock? c04 = Find<TextBlock>("CuttingsHeader04");
                TextBlock? c05 = Find<TextBlock>("CuttingsHeader05");
                TextBlock? c06 = Find<TextBlock>("CuttingsHeader06");
                TextBlock? c07 = Find<TextBlock>("CuttingsHeader07");
                TextBlock? c08 = Find<TextBlock>("CuttingsHeader08");
                TextBlock? c09 = Find<TextBlock>("CuttingsHeader09");
                TextBlock? c10 = Find<TextBlock>("CuttingsHeader10");
                TextBlock? c11 = Find<TextBlock>("CuttingsHeader11");
                TextBlock? c12 = Find<TextBlock>("CuttingsHeader12");
                TextBlock? c13 = Find<TextBlock>("CuttingsHeader13");
                TextBlock? c14 = Find<TextBlock>("CuttingsHeader14");
                TextBlock? c15 = Find<TextBlock>("CuttingsHeader15");
                TextBlock? c16 = Find<TextBlock>("CuttingsHeader16");

                _controlStyler.StyleScrollViewer(
                    cuttingsScrollViewer,
                    null,
                    cuttingsHeaderBorderForScrollViewer,
                    c01, c02, c03, c04, c05, c06, c07, c08,
                    c09, c10, c11, c12, c13, c14, c15, c16);
            }

            if (Find<ListView>("CuttingsListView") is ListView cuttingsListView) _controlStyler.StyleListView(cuttingsListView);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnLoadedAsync"/> pour amorcer le
        /// chargement asynchrone des sept caractéristiques de la fiche de
        /// synthèse et du tableau des commandes clients de la série par
        /// invocation de <see cref="VM_Page11.LoadAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <see cref="Page_Generic"/> en
        /// troisième et dernière position de la séquence de montage
        /// <c>ApplyLayout</c> → <c>OnResized</c> → <c>OnLoadedAsync</c>,
        /// une fois la stylisation invariante et l'ajustement dimensionnel
        /// appliqués. La <paramref name="callChain"/> reçue est construite
        /// par le handler sous la forme
        /// <c>Page11 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>.</para>
        ///
        /// <para>Objectif : Matérialiser l'ancrage canonique
        /// <c>Page_Generic.OnLoadedAsync</c> →
        /// <c>VM_Page_Generic.LoadAsync</c> (§4.15.6 et §4.15.7 du 0230),
        /// articulation centrale du couple générique de la famille. Le
        /// corps comporte exactement deux instructions : l'appel à la base
        /// puis l'invocation du hook du ViewModel. La CallChain et le
        /// <see cref="System.Threading.CancellationToken"/> sont propagés
        /// symétriquement, sans réinitialisation locale : le ViewModel
        /// reconstruit lui-même sa CallChain interne via
        /// <c>BuildFirstCallChain</c>, conformément au patron de surcharge
        /// de §4.15.6.</para>
        ///
        /// <para>Appel à <c>base.OnLoadedAsync(callChain, ct)</c> en première
        /// instruction : L'implémentation par défaut de
        /// <see cref="Page_Generic.OnLoadedAsync"/> retourne
        /// <c>Task.CompletedTask</c> et ne porte aucun traitement.
        /// L'appel est conservé en geste de robustesse vis-à-vis de toute
        /// évolution future du socle.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. Le traitement
        /// terminal des erreurs de chargement est intégralement porté par
        /// le filet <c>ExecuteSafeAsync</c> interne à
        /// <see cref="VM_Page11.LoadAsync"/> (EA-01) ; le filet ultime de
        /// <c>Page_Generic.OnLoadedHandler</c> n'intervient qu'en rempart
        /// contre les défaillances inattendues du framework
        /// WPF.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page11 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>, propagée
        /// telle quelle au hook du ViewModel.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé tel
        /// quel au hook du ViewModel. Valeur par défaut :
        /// <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// chargement délégué au ViewModel.</returns>
        protected override async Task OnLoadedAsync(
            string callChain,
            CancellationToken ct = default)
        {
            await base.OnLoadedAsync(callChain, ct);

            await _viewModel.LoadAsync(callChain, ct);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnResized"/> pour ajuster la hauteur du
        /// <c>TabControl</c> principal à la hauteur de fenêtre courante,
        /// et celles des <c>ScrollViewer</c> des deuxième, troisième,
        /// quatrième et cinquième onglets par dérivation de la
        /// précédente.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <see cref="Page_Generic"/> en
        /// deuxième position de la séquence de montage, puis par
        /// <c>OnSizeChangedHandler</c> à chaque redimensionnement
        /// ultérieur de la page. Le caractère synchrone est imposé par la
        /// signature du point d'extension (§4.15.7 du 0230). La
        /// <paramref name="callChain"/> reçue est construite par le handler
        /// concerné sous la forme
        /// <c>Page11 &gt; {handler} &gt; OnResized</c>.</para>
        ///
        /// <para>Objectif : Porter l'ajustement dimensionnel dynamique,
        /// strictement disjoint de la stylisation invariante d'
        /// <see cref="ApplyLayout"/> conformément à la séparation
        /// sémantique constitutive du contrat de
        /// <see cref="Page_Generic"/>. La hauteur du <c>TabControl</c> est
        /// calculée par soustraction d'une réserve fixe de
        /// <c>220</c> unités à la hauteur de fenêtre courante lue sur
        /// <c>ISE_Window.MainWindowHeight</c> (champ
        /// <see cref="Page_Generic._window"/> hérité). Cette réserve
        /// couvre les bandeaux transverses de la fenêtre principale et le
        /// menu horizontal ; sa valeur reprend le précédent uniforme des
        /// pages <c>Page01</c> et <c>Page03</c>.</para>
        ///
        /// <para>Cinq grandeurs ajustées : Les hauteurs des
        /// <c>ScrollViewer</c> <c>OrdersScrollViewer</c> du deuxième
        /// onglet, <c>FramesScrollViewer</c> du troisième,
        /// <c>BarsScrollViewer</c> du quatrième et
        /// <c>CuttingsScrollViewer</c> du cinquième sont dérivées
        /// de celle du <c>TabControl</c> par
        /// soustraction d'une réserve de <c>93</c> unités, couvrant la
        /// hauteur du bandeau d'en-têtes de colonnes et les marges
        /// internes de l'onglet. Cette réserve reprend celle de l'étalon
        /// <c>Page01</c> et vaut pour les quatre onglets sans recalcul,
        /// leur
        /// géométrie verticale étant identique — même <c>Grid</c> à deux
        /// lignes, même <c>StackPanel</c>, même <c>Border</c> d'en-têtes.
        /// La valeur est calculée une fois et consommée quatre fois. Le
        /// premier onglet ne porte aucun
        /// <c>ScrollViewer</c>, sa fiche de synthèse tenant intégralement
        /// dans la hauteur disponible. Aucune autre grandeur n'est
        /// ajustée.</para>
        ///
        /// <para>Appel à <c>base.OnResized(callChain)</c> en première
        /// instruction : L'implémentation par défaut de
        /// <see cref="Page_Generic.OnResized"/> ne porte aucun traitement.
        /// L'appel est conservé en geste de robustesse vis-à-vis de toute
        /// évolution future du socle.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. En cas
        /// d'absence ou de cast invalide du <c>TabControl</c>, la garde
        /// <c>is</c> n'engage pas l'affectation et la trace de diagnostic
        /// émise par <see cref="Page_Generic.Find{T}(string)"/> assure la
        /// détectabilité. Toute exception qui parviendrait néanmoins à
        /// être levée serait capturée par le filet ultime du handler
        /// appelant de <see cref="Page_Generic"/>.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> (au montage initial) ou
        /// <c>OnSizeChangedHandler</c> (à chaque redimensionnement
        /// ultérieur) sous la forme
        /// <c>Page11 &gt; {handler} &gt; OnResized</c>.</param>
        protected override void OnResized(string callChain)
        {
            base.OnResized(callChain);

            double tabControlHeight = _window.MainWindowHeight - 220;
            double scrollViewerHeight = tabControlHeight - 93;

            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl)
                mainTabControl.Height = tabControlHeight;
            if (Find<ScrollViewer>("OrdersScrollViewer") is ScrollViewer ordersScrollViewer)
                ordersScrollViewer.Height = scrollViewerHeight;
            if (Find<ScrollViewer>("FramesScrollViewer") is ScrollViewer framesScrollViewer)
                framesScrollViewer.Height = scrollViewerHeight;
            if (Find<ScrollViewer>("BarsScrollViewer") is ScrollViewer barsScrollViewer)
                barsScrollViewer.Height = scrollViewerHeight;
            if (Find<ScrollViewer>("CuttingsScrollViewer") is ScrollViewer cuttingsScrollViewer)
                cuttingsScrollViewer.Height = scrollViewerHeight;
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler d'événement propre branché au constructeur sur
        /// l'événement <c>SelectionChanged</c> du <c>TabControl</c>
        /// <c>MainTabControl</c>, déclenchant le chargement du tableau des
        /// châssis lorsque le troisième onglet devient l'onglet actif,
        /// celui du tableau des barres lorsque c'est le quatrième, et
        /// celui du tableau des découpes lorsque c'est le cinquième.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Handler propre au présent dérivé, distinct des
        /// trois handlers privés du socle
        /// <see cref="Page_Generic"/>. Il matérialise le choix de charger
        /// les tableaux des châssis, des barres et des découpes à
        /// l'activation de leur
        /// onglet respectif plutôt
        /// qu'au montage de la page — la vue source des châssis étant large
        /// de soixante-seize colonnes, le tableau des barres n'ayant de
        /// contenu qu'une fois l'optimisation lancée, et celui des
        /// découpes étant le plus fourni en lignes de la page. Le
        /// chargement est
        /// intégralement rejoué
        /// à chaque activation ultérieure : aucun indicateur ne mémorise
        /// qu'une lecture a déjà eu lieu et aucune garde de réentrance
        /// n'est posée, le rechargement systématique étant la
        /// règle.</para>
        ///
        /// <para>Signature <c>async void</c> : Elle est imposée par la
        /// signature de
        /// <see cref="System.Windows.Controls.SelectionChangedEventHandler"/>
        /// et par la nécessité d'attendre l'appel asynchrone au ViewModel.
        /// Elle se situe hors du périmètre d'EA-03 tel que rédigé, admise
        /// par arbitrage explicite du développeur sous condition du filet
        /// ultime décrit ci-après (cf. la note de niveau classe).</para>
        ///
        /// <para>Première garde — origine de l'événement :
        /// <c>SelectionChanged</c> est un événement routé qui remonte
        /// depuis les contrôles enfants. Dans le modèle WPF,
        /// <paramref name="sender"/> désigne l'élément auquel le handler
        /// est attaché, soit <c>MainTabControl</c> dans tous les cas — y
        /// compris lorsque l'événement a été levé par la <c>ListView</c>
        /// du deuxième onglet au clic sur une ligne. Une garde écrite sur
        /// <paramref name="sender"/> serait donc structurellement
        /// inopérante. <see cref="System.Windows.RoutedEventArgs.Source"/>
        /// porte en revanche l'élément logique ayant réellement levé
        /// l'événement : le <c>TabControl</c> pour son propre changement
        /// de sélection, la <c>ListView</c> pour le sien.
        /// <see cref="System.Windows.RoutedEventArgs.OriginalSource"/>
        /// n'est pas retenu, susceptible de désigner un sous-élément du
        /// template visuel et donc fragile aux évolutions de stylisation.
        /// Cette garde est le seul dispositif préservant le comportement
        /// existant de la <c>ListView</c> cliquable du deuxième onglet.
        /// Le motif <c>is not TabControl tabControl</c> désigne au passage
        /// l'instance résolue, consommée par la seconde garde.</para>
        ///
        /// <para>Aiguillage sur l'onglet devenu actif : L'indice
        /// sélectionné est comparé aux indices des onglets à chargement
        /// différé — <see cref="FramesTabIndex"/> pour les châssis,
        /// <see cref="BarsTabIndex"/> pour les barres,
        /// <see cref="CuttingsTabIndex"/> pour les découpes — et la
        /// branche par
        /// défaut sort immédiatement lorsqu'aucun ne correspond. Seuls les
        /// deux
        /// premiers onglets sont
        /// chargés à l'ouverture de la page par l'ancrage canonique
        /// <c>OnLoadedAsync</c> → <c>LoadAsync</c> et n'ont rien à faire
        /// ici. La branche par défaut
        /// neutralise du même coup le déclenchement
        /// parasite qui accompagne l'initialisation du conteneur
        /// d'onglets.</para>
        ///
        /// <para>Note de conformité — élargissement de l'aiguillage : Le
        /// <c>switch</c>, qui comptait deux destinations, en compte
        /// désormais trois. Cet élargissement est le vecteur
        /// structurellement nécessaire de l'ajout d'une troisième
        /// destination, et non une correction du code existant : la
        /// clause de préservation du mode Extension (§5.2.3 du
        /// 0232-Page-VM) vise les capacités énumérées — propriétés
        /// observables, overrides, bindings XAML, dépendances injectées —
        /// et non un aiguillage privé dont l'élargissement est le vecteur
        /// même de l'extension. Cette lecture a été posée et documentée
        /// par le fil qui a livré le quatrième onglet ; elle est
        /// reconduite ici sans être rouverte. Le comportement des branches
        /// des châssis et des barres, ainsi que la garde sur l'origine de
        /// l'événement routé, sont
        /// invariants.</para>
        ///
        /// <para>Filet de sécurité : Le corps utile est encapsulé dans un
        /// <c>try/catch</c> ultime qui trace par
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/> sans
        /// propager, à parité de forme avec les trois handlers privés du
        /// socle. Ce filet est structurellement nécessaire et non
        /// décoratif : <c>ExecuteSafeAsync</c> relance silencieusement
        /// <see cref="OperationCanceledException"/> conformément à §4.7.3
        /// du 0230, et depuis un <c>async void</c> non protégé cette
        /// relance atteindrait
        /// <see cref="System.Windows.Application.DispatcherUnhandledException"/>
        /// ; le filet ultime de <c>Page_Generic.OnLoadedHandler</c> ne
        /// couvre pas ce chemin d'exécution. Le nom de classe du préfixe
        /// de trace est porté par le littéral <c>nameof(Page11)</c>, le
        /// champ <c>_callee</c> de <see cref="Page_Generic"/> étant
        /// <c>private</c> et non accessible depuis le dérivé. Le handler
        /// ne porte en revanche aucun traitement applicatif typé des
        /// erreurs : celui-ci est porté par <c>ExecuteSafeAsync</c> côté
        /// ViewModel, sur un plan distinct du flot d'exécution. Le filet
        /// ultime local relève exclusivement de la mesure défensive de
        /// plateforme.</para>
        ///
        /// <para>Absence de CallChain : Aucune CallChain n'est construite
        /// ni propagée. Le handler n'en reçoit aucune du socle, et ni
        /// <see cref="VM_Page11.LoadChassisAsync"/>, ni
        /// <see cref="VM_Page11.LoadBarsAsync"/>, ni
        /// <see cref="VM_Page11.LoadCutPiecesAsync"/> n'en exposent en
        /// paramètre : le ViewModel reconstruit la sienne localement via
        /// <c>BuildFirstCallChain</c>, conformément au patron de surcharge
        /// de §4.15.6 du 0230. Le jeton d'annulation n'est pas davantage
        /// passé : les trois méthodes exposent une valeur par défaut
        /// <c>default</c> qui s'applique.</para>
        /// </remarks>
        /// <param name="sender">Élément auquel le handler est attaché,
        /// soit le <c>TabControl</c> <c>MainTabControl</c> dans tous les
        /// cas. Non consommé : l'origine réelle de l'événement est lue sur
        /// <paramref name="e"/>.</param>
        /// <param name="e">Arguments de l'événement routé, dont la
        /// propriété <see cref="System.Windows.RoutedEventArgs.Source"/>
        /// porte l'élément logique ayant réellement levé
        /// l'événement.</param>
        private async void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is not TabControl tabControl) return;

            try
            {
                switch (tabControl.SelectedIndex)
                {
                    case FramesTabIndex: await _viewModel.LoadChassisAsync(); break;
                    case BarsTabIndex: await _viewModel.LoadBarsAsync(); break;
                    case CuttingsTabIndex: await _viewModel.LoadCutPiecesAsync(); break;
                    default: return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[{nameof(Page11)}.{nameof(OnTabSelectionChanged)}] " +
                    $"Exception non gérée capturée par le filet de sécurité ultime : " +
                    $"{ex.GetType().Name} — {ex.Message}");
            }
        }

        #endregion
    }
}