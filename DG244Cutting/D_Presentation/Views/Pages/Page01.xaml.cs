using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.D_Presentation.ViewModels.Pages;
using DG244Cutting.D_Presentation.Views.Generic;

namespace DG244Cutting.D_Presentation.Views.Pages
{
    /// <summary>
    /// Vue WPF de la page Utilisateur <c>Page01</c> de l'application
    /// DG244Cutting, affichant dans un <c>TabControl</c> à deux onglets
    /// les informations d'identification de l'utilisateur applicatif et
    /// du poste courant (Onglet 1) et la liste des droits d'accès
    /// granulaires de l'utilisateur courant aux pages de l'application
    /// courante (Onglet 2).
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille Page de la couche
    /// <c>D_Presentation</c>, dérivé de
    /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/>.
    /// La page est instanciée par le framework WPF de navigation via
    /// <c>Activator.CreateInstance</c> (<c>SR_Navigation.NavigateToPage</c>),
    /// hors conteneur DI. Le constructeur sans paramètre est imposé par
    /// cette contrainte et résout son <c>DataContext</c>
    /// <see cref="VM_Page01"/> via
    /// <c>App.ServiceProvider.GetRequiredService</c> au titre de l'EA-02
    /// Service Locator héritée de <c>Page_Generic</c> (§4.15.7 et
    /// §4.15.10 du 0230). La page est accessible à tout utilisateur
    /// connecté en lecture seule et n'expose aucune commande
    /// utilisateur ; la sortie s'effectue exclusivement via les boutons
    /// transverses du menu horizontal (Home, Previous) portés par le
    /// couple <c>VM_MH_Generic</c> / <c>MH_Generic</c>.</para>
    /// <para>Objectif : Constituer la vue WPF permanente de la page
    /// Utilisateur, résoudre <see cref="VM_Page01"/> via le
    /// ServiceProvider, l'affecter à
    /// <see cref="System.Windows.FrameworkElement.DataContext"/> pour
    /// activer les bindings WPF des 18 propriétés observables de
    /// libellés multilingues (<c>Label_P01_NN</c>), des 5 propriétés
    /// observables data (<see cref="VM_Page01.UserId"/>,
    /// <see cref="VM_Page01.UserFullName"/>,
    /// <see cref="VM_Page01.DeviceUser"/>, <see cref="VM_Page01.DeviceId"/>,
    /// <see cref="VM_Page01.DeviceIP"/>) et de la propriété observable
    /// de collection <see cref="VM_Page01.PagesUserRights"/>, appliquer
    /// au chargement la stylisation des contrôles XAML via
    /// <c>IS_ControlStyler</c>, déclencher à chaque
    /// <see cref="System.Windows.FrameworkElement.SizeChanged"/>
    /// l'ajustement dimensionnel du <c>TabControl</c> et du
    /// <c>ScrollViewer</c> portant la <c>ListView</c> des droits, et
    /// déclencher au <see cref="System.Windows.FrameworkElement.Loaded"/>
    /// le chargement asynchrone des cinq données d'identification et de
    /// la collection des droits d'accès par invocation du hook
    /// <c>LoadAsync</c> de <see cref="VM_Page01"/> (override du hook
    /// canonique déclaré au socle
    /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>,
    /// §4.15.6 du 0230).</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Servir de vue WPF — la composition XAML est
    ///   portée par <c>Page01.xaml</c> et se compose d'un <c>Grid</c>
    ///   central nommé <c>PageGrid</c> contenant comme unique enfant
    ///   direct un <c>TabControl</c> nommé <c>MainTabControl</c> avec
    ///   deux <c>TabItem</c> (<c>UserDetailsTabItem</c>,
    ///   <c>UserAccessTabItem</c>). Le second TabItem porte un
    ///   <c>ScrollViewer</c> nommé <c>UserAccessScrollViewer</c>
    ///   englobant une <c>ListView</c> nommée
    ///   <c>UserAccessListView</c>, dont l'<c>ItemTemplate</c> consomme
    ///   par binding les propriétés de l'entité
    ///   <c>UserAppPageRight</c>. Les descendants d'un <c>TabControl</c>
    ///   et d'une <c>ListView</c> participent à l'héritage standard du
    ///   <c>DataContext</c> WPF : aucun <c>UT_BindingProxy</c> n'est
    ///   nécessaire ici (la dérogation au pattern
    ///   <c>UT_BindingProxy</c> n'étant requise que pour les
    ///   descendants d'un <c>FlowDocument</c>, cf. §3.2.2 du
    ///   0232-Page-VM).</description></item>
    ///   <item><description>Résoudre <see cref="VM_Page01"/> via
    ///   <c>App.ServiceProvider.GetRequiredService</c> dans le
    ///   constructeur sans paramètre, conformément au contrat de la
    ///   convention de plateforme <c>App.ServiceProvider</c> (§4.15.11
    ///   du 0230) et à l'EA-02 Service Locator étendue aux dérivés
    ///   directs de <c>Page_Generic</c> pour la résolution de leur
    ///   ViewModel (§4.15.7 du 0230).</description></item>
    ///   <item><description>Affecter
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/> à
    ///   l'instance de <see cref="VM_Page01"/> pour alimenter les
    ///   bindings WPF des 18 libellés multilingues, des 5 propriétés
    ///   data et de la collection <c>PagesUserRights</c>.</description></item>
    ///   <item><description>Appliquer la stylisation initiale des
    ///   contrôles XAML nommés via le service <c>IS_ControlStyler</c>
    ///   hérité de <c>Page_Generic</c> (champ
    ///   <see cref="Page_Generic._controlStyler"/>) par surcharge de
    ///   <see cref="Page_Generic.ApplyLayout(string)"/>, invoquée par
    ///   le filet de sécurité ultime du handler <c>OnLoadedHandler</c>
    ///   de <c>Page_Generic</c> préalablement à
    ///   <see cref="Page_Generic.OnResized(string)"/> puis à
    ///   <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>.
    ///   Les contrôles à styliser sont résolus par le helper
    ///   <see cref="Page_Generic.Find{T}(string)"/> hérité, qui
    ///   assortit la résolution typée <c>FindName(name) as T</c> d'une
    ///   trace de diagnostic émise en cas d'absence (filet contre les
    ///   ruptures silencieuses de contrat XAML).</description></item>
    ///   <item><description>Ajuster dynamiquement la hauteur du
    ///   <c>TabControl</c> et du <c>ScrollViewer</c> portant la
    ///   <c>ListView</c> des droits d'accès, par surcharge de
    ///   <see cref="Page_Generic.OnResized(string)"/>. L'override est
    ///   invoqué une première fois par <c>OnLoadedHandler</c>
    ///   immédiatement après <see cref="ApplyLayout(string)"/> au
    ///   montage initial de la page, puis à chaque événement
    ///   <see cref="System.Windows.FrameworkElement.SizeChanged"/> de
    ///   la fenêtre principale, conformément à §4.15.7 du 0230. Le
    ///   calcul dimensionnel consomme la propriété
    ///   <c>ISE_Window.MainWindowHeight</c> via le champ
    ///   <see cref="Page_Generic._window"/> hérité, à parité avec le
    ///   composant legacy <c>BatchStockRelease.Page90</c>.</description></item>
    ///   <item><description>Déclencher au
    ///   <see cref="System.Windows.FrameworkElement.Loaded"/> le
    ///   chargement asynchrone des cinq propriétés data et de la
    ///   collection <c>PagesUserRights</c> par surcharge de
    ///   <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
    ///   et invocation de
    ///   <c>VM_Page01.LoadAsync(callChain, ct)</c>, hook canonique
    ///   formellement déclaré au socle
    ///   <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>
    ///   (§4.15.6 du 0230). La <c>CallChain</c> reçue du handler
    ///   <c>OnLoadedHandler</c> est propagée telle quelle au hook
    ///   <c>LoadAsync</c>, qui la consomme pour construire ses propres
    ///   <c>CallChain</c>s internes via
    ///   <c>VM_Generic.BuildFirstCallChain</c> hérité.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune logique métier (R-4.12.1, I-4.12.1
    ///   du 0231) — le code-behind est strictement borné au câblage
    ///   Vue/ViewModel (constructeur, override des points d'extension
    ///   de <see cref="Page_Generic"/>) et à la mécanique de plateforme
    ///   (stylisation invariante, ajustement dimensionnel, invocation
    ///   de <c>_viewModel.LoadAsync</c> depuis
    ///   <see cref="OnLoadedAsync"/>).</description></item>
    ///   <item><description>Aucun chargement de libellés multilingues
    ///   depuis le code-behind ou le XAML (I-4.11.10 du 0231) ; les
    ///   libellés sont chargés exclusivement par le ViewModel via la
    ///   mécanique multilingue héritée de
    ///   <c>VM_Generic</c>.</description></item>
    ///   <item><description>Aucune chaîne UI en dur dans le XAML
    ///   (I-4.11.1 du 0231) ; aucun <c>DynamicResource</c> ni
    ///   <c>StaticResource</c> pour les textes UI (I-4.11.10 du 0231) :
    ///   tout texte affiché est issu d'un binding sur une propriété
    ///   observable du ViewModel.</description></item>
    ///   <item><description>Aucune injection ni résolution directe d'un
    ///   contrat <c>IU_</c> ou <c>IQ_</c> dans le code-behind
    ///   (I-4.10.10 du 0231) ; la consommation de UseCases et de Query
    ///   Handlers est intégralement portée par le ViewModel via
    ///   <c>IS_UseCaseInvoker</c> (EA-11).</description></item>
    /// </list>
    /// <para>Note sur les exceptions architecturales : Aucune exception
    /// architecturale propre n'est portée par <see cref="Page01"/>.
    /// La résolution de <see cref="VM_Page01"/> via
    /// <c>App.ServiceProvider.GetRequiredService</c> relève de l'EA-02
    /// Service Locator héritée de <see cref="Page_Generic"/> (§4.15.7
    /// et §4.15.11 du 0230), étendue aux dérivés directs de
    /// <c>Page_Generic</c> pour la résolution de leur ViewModel ; cette
    /// exception est héritée et non re-déclarée à ce niveau.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2) complétée par une extension (R-4.4.10 du 0231)
    /// au titre des overrides protégés de <see cref="ApplyLayout"/>,
    /// <see cref="OnLoadedAsync"/> et <see cref="OnResized"/>, soit six
    /// régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> : champ
    ///   <see cref="_viewModel"/> stockant l'instance Singleton de
    ///   <see cref="VM_Page01"/> résolue au constructeur via
    ///   <c>App.ServiceProvider.GetRequiredService</c>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   sans paramètre <c>public</c> imposé par le framework WPF de
    ///   navigation, résolvant <see cref="VM_Page01"/> et l'affectant à
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c>
    ///   (extension §4.4.3) : trois overrides — <see cref="ApplyLayout"/>
    ///   pour la stylisation initiale des contrôles XAML par
    ///   <c>IS_ControlStyler</c>, <see cref="OnLoadedAsync"/> pour
    ///   l'invocation du hook <c>LoadAsync</c> du ViewModel au montage
    ///   de la page (ancrage canonique), <see cref="OnResized"/> pour
    ///   l'ajustement dimensionnel du <c>TabControl</c> et du
    ///   <c>ScrollViewer</c> au montage initial et à chaque
    ///   redimensionnement ultérieur de la fenêtre principale. Aucun
    ///   override d'<c>OnUnloadedAsync</c> : la page ne détient aucune
    ///   ressource asynchrone à libérer au démontage, l'implémentation
    ///   par défaut de <see cref="Page_Generic"/> suffit.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>.</description></item>
    /// </list>
    /// </remarks>
    public partial class Page01 : Page_Generic
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Instance Singleton du ViewModel associé à la présente vue,
        /// résolue au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> et affectée à
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// pour alimenter les bindings WPF des 24 propriétés
        /// observables exposées par <see cref="VM_Page01"/> et déclarés
        /// par <c>Page01.xaml</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Champ stocké en lecture seule pour exposer
        /// le type concret <see cref="VM_Page01"/> au code-behind,
        /// distinct du
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// typé en <see cref="object"/>. Consommé par
        /// <see cref="OnLoadedAsync"/> pour l'invocation du hook
        /// <c>LoadAsync</c> au montage de la page, et nulle part
        /// ailleurs — la page n'invoque aucune commande du ViewModel
        /// depuis le code-behind, conformément à la séparation MVVM
        /// stricte.</para>
        /// </remarks>
        private readonly VM_Page01 _viewModel;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="Page01"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur sans paramètre imposé par le
        /// framework WPF de navigation, qui instancie la page via
        /// <c>Activator.CreateInstance</c> au sein de
        /// <c>SR_Navigation.NavigateToPage</c>. La résolution des
        /// dépendances ne peut donc se faire par injection paramétrée
        /// et s'effectue par le canal légitime
        /// <c>App.ServiceProvider.GetRequiredService</c>, au titre de
        /// la convention de plateforme documentée en §4.15.11 du 0230
        /// et de l'EA-02 Service Locator étendue aux dérivés directs
        /// de <c>Page_Generic</c> pour la résolution de leur
        /// ViewModel.</para>
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Résolution de <see cref="VM_Page01"/>
        ///   via <c>App.ServiceProvider.GetRequiredService</c> et
        ///   stockage dans le champ <see cref="_viewModel"/>. La
        ///   méthode <c>GetRequiredService</c> est utilisée (et non
        ///   <c>GetService</c>), conformément à la règle 2 de §4.15.11
        ///   du 0230 : toute dépendance non résolue doit faire échouer
        ///   l'instanciation immédiatement par exception explicite
        ///   plutôt que de produire une
        ///   <see cref="NullReferenceException"/> ultérieure.</description></item>
        ///   <item><description>Invocation de <c>InitializeComponent</c>
        ///   pour la composition XAML — étape impérativement préalable
        ///   à toute affectation de
        ///   <see cref="System.Windows.FrameworkElement.DataContext"/>.</description></item>
        ///   <item><description>Affectation de
        ///   <see cref="System.Windows.FrameworkElement.DataContext"/>
        ///   à <see cref="_viewModel"/> pour activer les bindings WPF
        ///   des 24 propriétés observables exposées par
        ///   <see cref="VM_Page01"/>.</description></item>
        /// </list>
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le
        /// constructeur au-delà de la résolution du ViewModel. Une
        /// défaillance de <c>GetRequiredService</c> traduirait une
        /// erreur de configuration du conteneur DI et doit faire
        /// échouer l'instanciation immédiatement. Le filet de sécurité
        /// ultime au bord des handlers WPF est porté par
        /// <c>Page_Generic</c> et couvre les éventuelles défaillances
        /// survenant au chargement, au déchargement et au
        /// redimensionnement de la page.</para>
        /// </remarks>
        public Page01()
        {
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page01>();

            InitializeComponent();

            DataContext = _viewModel;
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.ApplyLayout"/> pour appliquer la
        /// stylisation initiale des contrôles XAML nommés de la page
        /// (<c>Grid</c> central, <c>TabControl</c>, deux <c>TabItem</c>,
        /// deux <c>TextBlock</c> Header de TabItem, <c>Border</c> de
        /// l'Onglet 1, dix <c>TextBlock</c> titres et data de l'Onglet 1,
        /// <c>Border</c> d'en-têtes de l'Onglet 2, <c>ScrollViewer</c>
        /// avec ses onze en-têtes variadiques, <c>ListView</c>) via le
        /// service <c>IS_ControlStyler</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <c>Page_Generic</c> à l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de la
        /// page, préalablement à <see cref="OnResized(string)"/> puis à
        /// <see cref="OnLoadedAsync(string, System.Threading.CancellationToken)"/>.
        /// Le caractère synchrone est imposé par la signature du point
        /// d'extension de <c>Page_Generic</c> (§4.15.7 du 0230). La
        /// <paramref name="callChain"/> reçue est construite par le
        /// handler <c>OnLoadedHandler</c> sous la forme
        /// <c>Page01 &gt; OnLoadedHandler &gt; ApplyLayout</c>,
        /// conformément au patron méthode publique de §4.5.1 du
        /// 0230.</para>
        /// <para>Objectif : Appliquer la stylisation visuelle de
        /// l'ensemble des contrôles XAML nommés de la page selon
        /// l'ordre logique de l'arbre visuel :
        /// <c>PageGrid</c> → <c>MainTabControl</c> → deux
        /// <c>TabItem</c> et leurs Headers → contenu de
        /// <c>UserDetailsTabItem</c> (Border + 10 TextBlocks) → contenu
        /// de <c>UserAccessTabItem</c> (Border d'en-têtes, puis
        /// <c>ScrollViewer</c> avec ses onze en-têtes variadiques, puis
        /// <c>ListView</c>).</para>
        /// <para>Résolution typée par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> : Chaque contrôle
        /// XAML stylisable est résolu par le helper hérité, qui combine
        /// <c>FindName(name) as T</c> avec une trace
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/> en
        /// cas d'absence ou de cast invalide. Le retour <c>T?</c> du
        /// helper est consommé via une garde <c>is</c> qui conditionne
        /// l'invocation du service <c>IS_ControlStyler</c> (paramètres
        /// non-nullable) au succès de la résolution, selon la forme
        /// dépliée un-à-un
        /// <c>if (Find&lt;T&gt;(name) is T x) _controlStyler.X(x)</c>
        /// prescrite par le patron normatif « Patron de surcharge -
        /// ApplyLayout » de §4.15.7 du 0230 et par la règle R-4.15.25
        /// du 0231, qui constituent l'ancrage doctrinal de la garde et
        /// proscrivent explicitement l'opérateur null-forgiving
        /// (<c>!</c>) pour franchir ce pont.</para>
        /// <para>Bloc <c>StyleScrollViewer</c> variadique : Le contrat
        /// <c>IS_ControlStyler.StyleScrollViewer</c> expose un premier
        /// paramètre non-nullable (le <c>ScrollViewer</c> lui-même)
        /// suivi de douze paramètres nullables optionnels (un
        /// <c>TextBlock</c> de titre, un <c>Border</c> de bandeau,
        /// onze <c>TextBlock</c> d'en-têtes). Le bloc dédié conditionne
        /// l'invocation à la résolution du <c>ScrollViewer</c> (garde
        /// <c>is</c>) ; la résolution du <c>Border</c> et des onze
        /// en-têtes est portée dans des variables locales typées
        /// <c>Border?</c> et <c>TextBlock?</c> directement passées en
        /// argument — le contrat acceptant le <c>null</c> sur ces
        /// paramètres optionnels, la garde <c>is</c> par paramètre
        /// n'est pas requise et l'invocation reste unique. Le
        /// <c>UserAccessHeaderBorder</c>, par ailleurs stylisé en
        /// dehors de ce bloc par <c>StyleBorderHeader</c>, est résolu
        /// une seconde fois ici dans la portée locale dédiée — le
        /// helper <see cref="Page_Generic.Find{T}(string)"/>
        /// (idempotent et de coût négligeable) absorbe sans cérémonie
        /// cette double résolution, qui préserve la lisibilité et
        /// l'autonomie du bloc.</para>
        /// <para>Appel à <c>base.ApplyLayout(callChain)</c> en première
        /// instruction : L'implémentation par défaut de
        /// <see cref="Page_Generic.ApplyLayout"/> ne porte aucun
        /// traitement. L'appel est néanmoins conservé en geste de
        /// robustesse vis-à-vis de toute évolution future du socle,
        /// conformément à la convention d'override standard et au
        /// patron normatif présenté en §4.15.7 du 0230.</para>
        /// <para>Filet de sécurité : Aucun try/catch local. La
        /// continuation gracieuse est assurée au niveau du corps : en
        /// cas d'absence ou de cast invalide d'un contrôle XAML, la
        /// garde <c>is</c> n'engage pas l'invocation du service
        /// <c>IS_ControlStyler</c>, et la trace de diagnostic émise par
        /// <see cref="Page_Generic.Find{T}(string)"/> via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// assure la détectabilité en environnement de développement.
        /// Toute exception qui parviendrait néanmoins à être levée par
        /// <c>IS_ControlStyler</c> ou par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> serait capturée
        /// par le filet de sécurité ultime de
        /// <c>Page_Generic.OnLoadedHandler</c> (try/catch englobant le
        /// handler), qui trace l'exception via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// sans la propager au framework WPF, conformément à §4.15.7
        /// du 0230.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page01 &gt; OnLoadedHandler &gt; ApplyLayout</c>.</param>
        protected override void ApplyLayout(string callChain)
        {
            base.ApplyLayout(callChain);

            if (Find<Grid>("PageGrid") is Grid pageGrid) _controlStyler.StylePage(pageGrid);
            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl) _controlStyler.StyleTabControl(mainTabControl);
            if (Find<TabItem>("UserDetailsTabItem") is TabItem userDetailsTabItem
                     && Find<TextBlock>("UserDetailsTabHeader") is TextBlock userDetailsTabHeader)
                _controlStyler.StyleTabItem(userDetailsTabItem, userDetailsTabHeader, 150);
            if (Find<TabItem>("UserAccessTabItem") is TabItem userAccessTabItem
                && Find<TextBlock>("UserAccessTabHeader") is TextBlock userAccessTabHeader)
                _controlStyler.StyleTabItem(userAccessTabItem, userAccessTabHeader, 150);
            if (Find<Border>("UserDetailsBorder") is Border userDetailsBorder) _controlStyler.StyleBorder(userDetailsBorder);

            if (Find<TextBlock>("IDTitle") is TextBlock idTitle) _controlStyler.StyleTextBlockTitle(idTitle, 300);
            if (Find<TextBlock>("UserFullNameTitle") is TextBlock userFullNameTitle) _controlStyler.StyleTextBlockTitle(userFullNameTitle, 300);
            if (Find<TextBlock>("DeviceUserTitle") is TextBlock deviceUserTitle) _controlStyler.StyleTextBlockTitle(deviceUserTitle, 300);
            if (Find<TextBlock>("DeviceIdTitle") is TextBlock deviceIdTitle) _controlStyler.StyleTextBlockTitle(deviceIdTitle, 300);
            if (Find<TextBlock>("DeviceIPTitle") is TextBlock deviceIPTitle) _controlStyler.StyleTextBlockTitle(deviceIPTitle, 300);

            if (Find<TextBlock>("IDData") is TextBlock idData) _controlStyler.StyleTextBlockData(idData);
            if (Find<TextBlock>("UserFullNameData") is TextBlock userFullNameData) _controlStyler.StyleTextBlockData(userFullNameData);
            if (Find<TextBlock>("DeviceUserData") is TextBlock deviceUserData) _controlStyler.StyleTextBlockData(deviceUserData);
            if (Find<TextBlock>("DeviceIdData") is TextBlock deviceIdData) _controlStyler.StyleTextBlockData(deviceIdData);
            if (Find<TextBlock>("DeviceIPData") is TextBlock deviceIPData) _controlStyler.StyleTextBlockData(deviceIPData);

            if (Find<Border>("UserAccessHeaderBorder") is Border userAccessHeaderBorder) _controlStyler.StyleBorderHeader(userAccessHeaderBorder);

            // Bloc StyleScrollViewer variadique : résolution typée des onze en-têtes en variables locales,
            // invocation unique conditionnée à la résolution du ScrollViewer.
            if (Find<ScrollViewer>("UserAccessScrollViewer") is ScrollViewer userAccessScrollViewer)
            {
                Border? headerBorderForScrollViewer = Find<Border>("UserAccessHeaderBorder");
                TextBlock? h01 = Find<TextBlock>("UserAccessHeader01");
                TextBlock? h02 = Find<TextBlock>("UserAccessHeader02");
                TextBlock? h03 = Find<TextBlock>("UserAccessHeader03");
                TextBlock? h04 = Find<TextBlock>("UserAccessHeader04");
                TextBlock? h05 = Find<TextBlock>("UserAccessHeader05");
                TextBlock? h06 = Find<TextBlock>("UserAccessHeader06");
                TextBlock? h07 = Find<TextBlock>("UserAccessHeader07");
                TextBlock? h08 = Find<TextBlock>("UserAccessHeader08");
                TextBlock? h09 = Find<TextBlock>("UserAccessHeader09");
                TextBlock? h10 = Find<TextBlock>("UserAccessHeader10");
                TextBlock? h11 = Find<TextBlock>("UserAccessHeader11");

                _controlStyler.StyleScrollViewer(
                    userAccessScrollViewer,
                    null,
                    headerBorderForScrollViewer,
                    h01, h02, h03, h04, h05, h06, h07, h08, h09, h10, h11);
            }

            if (Find<ListView>("UserAccessListView") is ListView userAccessListView) _controlStyler.StyleListView(userAccessListView);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
        /// pour déclencher le chargement asynchrone des cinq propriétés
        /// data d'identification et de la collection
        /// <see cref="VM_Page01.PagesUserRights"/> par invocation du
        /// hook <c>LoadAsync</c> de <see cref="VM_Page01"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <c>Page_Generic</c> à l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de la
        /// page, postérieurement à l'application de la stylisation par
        /// <see cref="ApplyLayout(string)"/> et à l'ajustement
        /// dimensionnel initial par <see cref="OnResized(string)"/>.
        /// Le caractère asynchrone (<c>Task</c>) est imposé par la
        /// signature du point d'extension de <c>Page_Generic</c>
        /// (§4.15.7 du 0230).</para>
        /// <para>Objectif : Déclencher l'alimentation des cinq
        /// propriétés observables data
        /// (<see cref="VM_Page01.UserId"/>,
        /// <see cref="VM_Page01.UserFullName"/>,
        /// <see cref="VM_Page01.DeviceUser"/>,
        /// <see cref="VM_Page01.DeviceId"/>,
        /// <see cref="VM_Page01.DeviceIP"/>) et de la collection
        /// observable <see cref="VM_Page01.PagesUserRights"/> par
        /// invocation du hook canonique <c>LoadAsync</c> de
        /// <see cref="VM_Page01"/> (override du hook formellement
        /// déclaré au socle
        /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>,
        /// §4.15.6 du 0230). Cet override invoque lui-même
        /// <c>IS_AppContext.GetAppContext</c> pour le snapshot atomique
        /// du contexte applicatif et utilisateur, puis le Query Handler
        /// générique <c>IQ_Generic&lt;UserAppPageRight&gt;</c> via le
        /// service <c>IS_UseCaseInvoker</c> (EA-11) au titre du mode
        /// d'invocation normatif des UseCases et Query Handlers depuis
        /// <c>D_Presentation</c> (§4.10.10 du 0230). Les 18 propriétés
        /// observables <c>Label_P01_NN</c> ont déjà été alimentées au
        /// constructeur du ViewModel via l'orchestration
        /// <c>InitializeLabels</c> de <c>VM_Generic</c> (§4.15.5 du
        /// 0230) et ne nécessitent pas de chargement asynchrone au
        /// <c>Loaded</c>.</para>
        /// <para>Patron de surcharge normatif (§4.15.7 du 0230) : Le
        /// corps comprend exactement deux instructions, l'appel à
        /// <c>base.OnLoadedAsync(callChain, ct)</c> en première
        /// instruction (conservé en geste de robustesse vis-à-vis de
        /// toute évolution future du socle conformément à §3.14.4 du
        /// 0230, bien que l'implémentation par défaut de
        /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
        /// retourne <see cref="Task.CompletedTask"/>) suivi de
        /// l'invocation <c>_viewModel.LoadAsync(callChain, ct)</c>. La
        /// <paramref name="callChain"/> reçue est propagée telle quelle
        /// au hook <c>LoadAsync</c> du ViewModel, qui la consommera
        /// pour construire ses propres <c>CallChain</c>s internes via
        /// <c>VM_Generic.BuildFirstCallChain</c> hérité (§4.15.5 du
        /// 0230). Le <see cref="System.Threading.CancellationToken"/>
        /// est propagé symétriquement à <c>base</c> puis au hook
        /// <c>LoadAsync</c>, autorisant le ViewModel à participer à
        /// l'annulation coopérative jusqu'au Query Handler.</para>
        /// <para>Filet de sécurité : Aucun try/catch local. Toute
        /// exception métier levée par
        /// <c>VM_Page01.LoadAsync(callChain, ct)</c> est capturée par
        /// le filet de sécurité métier
        /// <c>VM_Generic.ExecuteSafeAsync</c> porté par le ViewModel
        /// (§4.15.5 du 0230) ; toute exception non gérée ultime serait
        /// capturée par le filet de sécurité ultime de
        /// <c>Page_Generic.OnLoadedHandler</c>, qui trace l'exception
        /// via <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// sans la propager au framework WPF, conformément à §4.15.7
        /// du 0230 et à l'EA <c>async void</c> des handlers WPF.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page01 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>,
        /// propagée telle quelle à <c>base</c> et au hook
        /// <c>VM_Page01.LoadAsync</c>.</param>
        /// <param name="ct">Token d'annulation coopérative propagé par
        /// le handler appelant (valeur par défaut <c>default</c> côté
        /// <c>Page_Generic</c> qui ne construit pas de
        /// <c>CancellationTokenSource</c> propre), retransmis
        /// symétriquement à <c>base</c> et au hook
        /// <c>VM_Page01.LoadAsync</c>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// chargement des cinq propriétés data et de la collection
        /// <see cref="VM_Page01.PagesUserRights"/>.</returns>
        protected override async Task OnLoadedAsync(
            string callChain,
            CancellationToken ct = default)
        {
            await base.OnLoadedAsync(callChain, ct);

            await _viewModel.LoadAsync(callChain, ct);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnResized(string)"/> pour ajuster
        /// dynamiquement la hauteur du <c>TabControl</c>
        /// <c>MainTabControl</c> et du <c>ScrollViewer</c>
        /// <c>UserAccessScrollViewer</c> en fonction de la hauteur
        /// courante de la fenêtre principale.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par les handlers
        /// <c>OnLoadedHandler</c> (au montage initial, immédiatement
        /// après <see cref="ApplyLayout(string)"/> et avant
        /// <see cref="OnLoadedAsync(string, System.Threading.CancellationToken)"/>)
        /// et <c>OnSizeChangedHandler</c> (à chaque redimensionnement
        /// ultérieur de la fenêtre principale) de <c>Page_Generic</c>,
        /// conformément à §4.15.7 du 0230. Le caractère synchrone est
        /// imposé par la signature du point d'extension de
        /// <c>Page_Generic</c> ; l'événement
        /// <see cref="System.Windows.FrameworkElement.SizeChanged"/>
        /// étant déclenché à haute fréquence pendant un
        /// redimensionnement utilisateur, le traitement doit rester
        /// rapide et purement local à la vue.</para>
        /// <para>Objectif : Ajuster la hauteur du <c>TabControl</c>
        /// <c>MainTabControl</c> à <c>MainWindowHeight - 220</c> et
        /// la hauteur du <c>ScrollViewer</c>
        /// <c>UserAccessScrollViewer</c> à
        /// <c>tabControlHeight - 93</c>, à parité avec le composant
        /// legacy <c>BatchStockRelease.Page90</c>. Les constantes
        /// <c>220</c> et <c>93</c> reproduisent strictement les
        /// valeurs du legacy et représentent les marges réservées à
        /// l'environnement de la page (bandeau de fenêtre, menu
        /// horizontal, marges visuelles) et au bandeau d'en-têtes de
        /// colonnes de l'Onglet 2 respectivement.</para>
        /// <para>Patron de surcharge normatif (§4.15.7 du 0230) :
        /// <c>base.OnResized(callChain)</c> en première instruction
        /// (conservé en geste de robustesse vis-à-vis de toute
        /// évolution future du socle, bien que l'implémentation par
        /// défaut ne porte aucun traitement), suivi du calcul
        /// dimensionnel local et de l'affectation conditionnelle des
        /// propriétés <c>Height</c> sur les deux contrôles concernés.
        /// La consommation de <c>_window.MainWindowHeight</c>
        /// (propriété <c>int</c> exposée par
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Settings.Presentation.ISE_Window"/>)
        /// via le champ <see cref="Page_Generic._window"/> hérité est
        /// la voie canonique d'accès à la dimension de la fenêtre
        /// principale documentée en §4.15.7 du 0230 ; la conversion
        /// implicite <c>int</c> → <c>double</c> à l'affectation des
        /// variables locales est licite C# et n'appelle aucun cast
        /// explicite.</para>
        /// <para>Résolution typée par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> : Chaque contrôle
        /// dont la hauteur est ajustée est résolu par le helper hérité,
        /// avec garde <c>is</c> conditionnant l'affectation, selon le
        /// patron normatif identique à celui d'<see cref="ApplyLayout"/>.</para>
        /// <para>Filet de sécurité : Aucun try/catch local. La
        /// continuation gracieuse est assurée par la garde <c>is</c>
        /// par contrôle ; toute exception qui parviendrait à être
        /// levée (par exemple <see cref="OverflowException"/> ou
        /// <see cref="ArgumentException"/> sur l'affectation d'une
        /// <c>Height</c> aberrante) serait capturée par le filet de
        /// sécurité ultime de <c>Page_Generic.OnLoadedHandler</c> ou
        /// <c>OnSizeChangedHandler</c>.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> (au montage initial)
        /// ou <c>OnSizeChangedHandler</c> (à chaque redimensionnement
        /// ultérieur) sous la forme
        /// <c>Page01 &gt; {handler} &gt; OnResized</c>.</param>
        protected override void OnResized(string callChain)
        {
            base.OnResized(callChain);

            double tabControlHeight = _window.MainWindowHeight - 220;
            double scrollViewerHeight = tabControlHeight - 93;

            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl)
                mainTabControl.Height = tabControlHeight;
            if (Find<ScrollViewer>("UserAccessScrollViewer") is ScrollViewer userAccessScrollViewer)
                userAccessScrollViewer.Height = scrollViewerHeight;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}