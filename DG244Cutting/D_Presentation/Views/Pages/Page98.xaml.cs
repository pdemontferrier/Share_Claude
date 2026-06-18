using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.D_Presentation.ViewModels.Pages;
using DG244Cutting.D_Presentation.Views.Generic;

namespace DG244Cutting.D_Presentation.Views.Pages
{
    /// <summary>
    /// Vue WPF de la page de présentation générale <c>Page98</c> de
    /// l'application DG244Cutting, affichant dans un <c>FlowDocument</c>
    /// structuré 55 libellés multilingues de présentation
    /// (<c>Label_P98_01</c> à <c>Label_P98_55</c>) et le numéro de
    /// version courant de l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille Page de la couche
    /// <c>D_Presentation</c>, dérivé de
    /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/>.
    /// La page est instanciée par le framework WPF de navigation via
    /// <c>Activator.CreateInstance</c> (<c>SR_Navigation.NavigateToPage</c>),
    /// hors conteneur DI. Le constructeur sans paramètre est imposé par
    /// cette contrainte et résout son <c>DataContext</c>
    /// <see cref="VM_Page98"/> via
    /// <c>App.ServiceProvider.GetRequiredService</c> au titre de l'EA-02
    /// Service Locator héritée de <c>Page_Generic</c> (§4.15.7 et
    /// §4.15.10 du 0230). La page est consultée librement par tout
    /// utilisateur de l'application et présente le contexte fonctionnel,
    /// l'architecture logicielle et les principes de développement de
    /// l'application, ainsi que son numéro de version courant. La sortie
    /// s'effectue exclusivement via les boutons transverses du menu
    /// horizontal (Home, Previous) portés par le couple
    /// <c>VM_MH_Generic</c> / <c>MH_Generic</c>.</para>
    /// <para>Objectif : Constituer la vue WPF permanente de la page de
    /// présentation, résoudre <see cref="VM_Page98"/> via le
    /// ServiceProvider, l'affecter à
    /// <see cref="System.Windows.FrameworkElement.DataContext"/> pour
    /// activer les bindings WPF des 55 propriétés observables
    /// <c>Label_P98_01</c> à <c>Label_P98_55</c> et de la propriété
    /// observable <see cref="VM_Page98.VersionNumber"/>, appliquer au
    /// chargement la stylisation du <c>Grid</c> central et du
    /// <c>FlowDocument</c> via <c>IS_ControlStyler</c>, et déclencher
    /// au <see cref="System.Windows.FrameworkElement.Loaded"/> le
    /// chargement asynchrone de la propriété
    /// <see cref="VM_Page98.VersionNumber"/> par invocation du hook
    /// <c>LoadAsync</c> de <see cref="VM_Page98"/> (override du hook
    /// canonique déclaré au socle
    /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>,
    /// §4.15.6 du 0230).</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Servir de vue WPF — la composition XAML est
    ///   portée par <c>Page98.xaml</c> et se compose d'un <c>Grid</c>
    ///   central nommé <c>PageGrid</c> contenant un
    ///   <c>FlowDocumentScrollViewer</c> dont le <c>FlowDocument</c>
    ///   est nommé <c>AppInfoDoc</c>. Les Run du <c>FlowDocument</c>
    ///   indirectent leur binding via la ressource
    ///   <c>UT_BindingProxy</c> déclarée en <c>Page.Resources</c>, en
    ///   raison de la non-participation des descendants d'un
    ///   <c>FlowDocument</c> à l'héritage standard du <c>DataContext</c>
    ///   WPF.</description></item>
    ///   <item><description>Résoudre <see cref="VM_Page98"/> via
    ///   <c>App.ServiceProvider.GetRequiredService</c> dans le
    ///   constructeur sans paramètre, conformément au contrat de la
    ///   convention de plateforme <c>App.ServiceProvider</c> (§4.15.11
    ///   du 0230) et à l'EA-02 Service Locator étendue aux dérivés
    ///   directs de <c>Page_Generic</c> pour la résolution de leur
    ///   ViewModel (§4.15.7 du 0230).</description></item>
    ///   <item><description>Affecter
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/> à
    ///   l'instance de <see cref="VM_Page98"/> pour alimenter les
    ///   bindings WPF des 55 libellés multilingues et de la propriété
    ///   <see cref="VM_Page98.VersionNumber"/>, indirectés par la
    ///   ressource <c>UT_BindingProxy</c> pour les Run du
    ///   <c>FlowDocument</c>.</description></item>
    ///   <item><description>Appliquer la stylisation initiale du
    ///   <c>Grid</c> nommé <c>PageGrid</c> et du <c>FlowDocument</c>
    ///   nommé <c>AppInfoDoc</c> par surcharge de
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
    ///   <item><description>Déclencher au
    ///   <see cref="System.Windows.FrameworkElement.Loaded"/> le
    ///   chargement asynchrone de la propriété
    ///   <see cref="VM_Page98.VersionNumber"/> par surcharge de
    ///   <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
    ///   et invocation de
    ///   <c>VM_Page98.LoadAsync(callChain, ct)</c>, hook canonique
    ///   formellement déclaré au socle
    ///   <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>
    ///   (§4.15.6 du 0230). La <c>CallChain</c> reçue du handler
    ///   <c>OnLoadedHandler</c> est propagée telle quelle au hook
    ///   <c>LoadAsync</c>, qui la consomme pour construire ses
    ///   <c>CallChain</c>s internes via
    ///   <c>BuildFirstCallChain</c> hérité de <c>VM_Generic</c>
    ///   (§4.15.5 du 0230). Le
    ///   <see cref="System.Threading.CancellationToken"/> est propagé
    ///   symétriquement.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Porter de la logique métier propre — la
    ///   page est un relais visuel pur entre <see cref="VM_Page98"/> et
    ///   le rendu WPF. Aucune décision de navigation, aucune commande
    ///   utilisateur, aucune règle métier ne sont portées par le
    ///   présent code-behind.</description></item>
    ///   <item><description>Invoquer directement un UseCase ou un
    ///   QueryHandler — la consommation de
    ///   <c>IU_GetApplicationVersion</c> est intégralement portée par
    ///   <see cref="VM_Page98"/> via le service
    ///   <c>IS_UseCaseInvoker</c> (EA-11) au titre du mode d'invocation
    ///   normatif depuis <c>D_Presentation</c> (§4.10.10 du 0230).
    ///   Aucune injection ni résolution directe de contrat
    ///   <c>IU_*</c> ou <c>IQ_*</c> n'apparaît dans le présent
    ///   code-behind ; une telle apparition constituerait une
    ///   non-conformité à I-4.10.10 du 0231.</description></item>
    ///   <item><description>Charger les libellés — le chargement est
    ///   intégralement porté par <see cref="VM_Page98"/> via son
    ///   override protected <c>LoadLabels</c>, lui-même orchestré par
    ///   <c>VM_Generic.InitializeLabels</c> (§4.15.5 du 0230) au
    ///   constructeur du ViewModel et par le handler interne
    ///   d'abonnement INPC à <c>ISE_App.AppCultureCode</c> porté par
    ///   <c>VM_Generic</c>. Toute tentative de chargement de libellé
    ///   depuis le présent code-behind constituerait une non-conformité
    ///   à I-4.11.10 du 0231.</description></item>
    ///   <item><description>Surcharger <c>OnUnloadedAsync</c> ou
    ///   <c>OnResized</c> — la page ne porte aucune ressource
    ///   asynchrone à libérer et n'a pas d'ajustement dynamique aux
    ///   dimensions à effectuer. Les implémentations par défaut de
    ///   <c>Page_Generic</c> suffisent. Pour mémoire, le socle invoque
    ///   désormais <c>OnResized</c> une première fois au montage
    ///   initial, immédiatement après <c>ApplyLayout</c> et avant
    ///   <c>OnLoadedAsync</c>, conformément à la rubrique « Changement
    ///   comportemental introduit par la refonte » de §4.15.7 du
    ///   0230 ; cette invocation au montage est sans incidence pour
    ///   <see cref="Page98"/> qui ne surcharge pas
    ///   <c>OnResized</c>.</description></item>
    ///   <item><description>Traiter les exceptions applicatives — le
    ///   filet de sécurité métier <c>ExecuteSafeAsync</c> est porté
    ///   par <c>VM_Generic</c> (§4.15.5 du 0230) et utilisé par
    ///   l'override <c>VM_Page98.LoadAsync</c> ; le filet de sécurité
    ///   ultime au bord des handlers WPF est porté par
    ///   <c>Page_Generic</c>. Aucun filet local n'est requis au niveau
    ///   du présent code-behind.</description></item>
    /// </list>
    /// <para>Statut de premier exemple canonique de second rang :</para>
    /// <para>La présente vue constitue le premier exemple canonique de
    /// second rang de la famille Page, en complémentarité avec
    /// <c>Page99.xaml.cs</c> qui est le premier exemple canonique de
    /// premier rang. Là où <c>Page99</c> illustre le cas minimal d'une
    /// vue de page purement statique — surcharge de
    /// <see cref="Page_Generic.ApplyLayout(string)"/> exclusivement sur
    /// deux contrôles XAML simples (un <c>Grid</c> et un
    /// <c>TextBlock</c>), sans surcharge de
    /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>,
    /// de
    /// <see cref="Page_Generic.OnUnloadedAsync(string, System.Threading.CancellationToken)"/>
    /// ni de <see cref="Page_Generic.OnResized(string)"/> —,
    /// <see cref="Page98"/> illustre le cas riche d'une vue de page
    /// combinant la stylisation au <c>Loaded</c> de deux contrôles XAML
    /// structurants (un <c>Grid</c> et un <c>FlowDocument</c>) par
    /// surcharge de <see cref="Page_Generic.ApplyLayout(string)"/>, et
    /// le déclenchement du chargement asynchrone de données métier via
    /// le hook <c>LoadAsync</c> du ViewModel par surcharge de
    /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>.
    /// La composition XAML elle-même illustre par ailleurs un cas non
    /// trivial — pattern <c>UT_BindingProxy</c> pour les descendants
    /// d'un <c>FlowDocument</c> qui ne participent pas à l'héritage
    /// standard du <c>DataContext</c> WPF — qui n'apparaît pas dans
    /// <c>Page99</c>. Les deux composants forment ainsi un couple
    /// canonique complémentaire qui couvre la totalité du spectre
    /// d'usage du socle <c>Page_Generic</c> et de ses points
    /// d'extension, et constituent ensemble la matière première de la
    /// future §5.12 du 0230 et du futur 0232_VI_VM, à inscrire dans un
    /// fil de maintenance documentaire ultérieur. Le statut d'étalon
    /// doctrinal couvre la structure des régions, l'héritage strict à
    /// <see cref="Page_Generic"/>, le constructeur en résolution
    /// Service Locator du ViewModel, la signature canonique des deux
    /// overrides <see cref="ApplyLayout(string)"/> et
    /// <see cref="OnLoadedAsync(string, System.Threading.CancellationToken)"/>,
    /// la consommation du patron normatif <c>Find&lt;T&gt; + garde is</c>
    /// et la propagation symétrique de la <c>CallChain</c> et du
    /// <see cref="System.Threading.CancellationToken"/> au hook
    /// <c>LoadAsync</c> du ViewModel.</para>
    /// <para>Note sur les exceptions architecturales : Aucune exception
    /// architecturale propre n'est portée par <see cref="Page98"/>.
    /// L'usage d'<c>App.ServiceProvider.GetRequiredService</c> dans le
    /// constructeur sans paramètre, exclusivement pour la résolution du
    /// ViewModel correspondant, relève de l'EA-02 Service Locator déjà
    /// documentée pour <c>Page_Generic</c> (§4.15.7 du 0230) et étendue
    /// nominativement aux dérivés directs au titre de la couverture
    /// définie en §4.15.7 du 0230 et confirmée par la règle 1 alinéa 2
    /// de la convention de plateforme <c>App.ServiceProvider</c>
    /// (§4.15.11 du 0230). Cette EA est héritée et non re-déclarée au
    /// niveau du présent code-behind. Pour mémoire, l'override
    /// <c>LoadAsync</c> de <see cref="VM_Page98"/> consomme
    /// <c>IS_UseCaseInvoker</c> au titre d'EA-11 et de §4.10.10 du
    /// 0230 (mode d'invocation normatif des UseCases depuis
    /// <c>D_Presentation</c>) ; cette mécanique est entièrement portée
    /// par le ViewModel et invisible au présent code-behind.</para>
    /// </remarks>
    public partial class Page98 : Page_Generic
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
        /// pour alimenter les bindings WPF déclarés par
        /// <c>Page98.xaml</c>, indirectés par la ressource
        /// <c>UT_BindingProxy</c> pour les Run du <c>FlowDocument</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Champ stocké en lecture seule pour exposer
        /// le type concret <see cref="VM_Page98"/> au code-behind,
        /// distinct du
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// typé en <see cref="object"/>. L'usage local se limite à
        /// l'affectation du <c>DataContext</c> dans le constructeur et
        /// à l'invocation de <c>LoadAsync</c> dans la surcharge de
        /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
        /// (hook canonique déclaré au socle
        /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>,
        /// §4.15.6 du 0230), conformément à la séparation MVVM stricte :
        /// aucune autre méthode ni commande du ViewModel n'est invoquée
        /// depuis le code-behind.</para>
        /// </remarks>
        private readonly VM_Page98 _viewModel;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="Page98"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur sans paramètre imposé par le
        /// framework WPF de navigation, qui instancie la page via
        /// <c>Activator.CreateInstance</c> au sein de
        /// <c>SR_Navigation.NavigateToPage</c>. La résolution des
        /// dépendances ne peut donc se faire par injection paramétrée
        /// et s'effectue par le canal légitime
        /// <c>App.ServiceProvider.GetRequiredService</c>, au titre de la
        /// convention de plateforme documentée en §4.15.11 du 0230 et
        /// de l'EA-02 Service Locator étendue aux dérivés directs de
        /// <c>Page_Generic</c> pour la résolution de leur ViewModel.</para>
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Résolution de <see cref="VM_Page98"/>
        ///   via <c>App.ServiceProvider.GetRequiredService</c> et
        ///   stockage dans le champ <see cref="_viewModel"/>. La méthode
        ///   <c>GetRequiredService</c> est utilisée (et non
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
        ///   des 55 propriétés observables <c>Label_P98_01</c> à
        ///   <c>Label_P98_55</c> et de la propriété
        ///   <see cref="VM_Page98.VersionNumber"/>, indirectés par la
        ///   ressource <c>UT_BindingProxy</c> pour les Run du
        ///   <c>FlowDocument</c>.</description></item>
        /// </list>
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le
        /// constructeur au-delà de la résolution du ViewModel. Une
        /// défaillance de <c>GetRequiredService</c> traduirait une
        /// erreur de configuration du conteneur DI et doit faire échouer
        /// l'instanciation immédiatement. Le filet de sécurité ultime
        /// au bord des handlers WPF est porté par <c>Page_Generic</c>
        /// et couvre les éventuelles défaillances survenant au
        /// chargement, au déchargement et au redimensionnement de la
        /// page.</para>
        /// </remarks>
        public Page98()
        {
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page98>();

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
        /// <see cref="Page_Generic.ApplyLayout(string)"/> pour appliquer
        /// la stylisation initiale du <c>Grid</c> central et du
        /// <c>FlowDocument</c> de la page.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <c>Page_Generic</c> à l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de la
        /// page, préalablement à
        /// <see cref="Page_Generic.OnResized(string)"/> et à
        /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>.
        /// Le caractère synchrone est imposé par la signature du point
        /// d'extension de <c>Page_Generic</c> (§4.15.7 du 0230). La
        /// <paramref name="callChain"/> reçue est construite par le
        /// handler <c>OnLoadedHandler</c> sous la forme
        /// <c>Page98 &gt; OnLoadedHandler &gt; ApplyLayout</c>,
        /// conformément au patron méthode publique de §4.5.1 du
        /// 0230.</para>
        /// <para>Objectif : Appliquer la stylisation visuelle des deux
        /// contrôles XAML stylisables de la page via le service
        /// <c>IS_ControlStyler</c> hérité de <c>Page_Generic</c> (champ
        /// <see cref="Page_Generic._controlStyler"/>) :</para>
        /// <list type="bullet">
        ///   <item><description><c>if (Find&lt;Grid&gt;("PageGrid") is Grid pageGrid) _controlStyler.StylePage(pageGrid)</c>
        ///   applique la stylisation standard du conteneur de page
        ///   (fond, marges, alignements) lorsque le <c>Grid</c> nommé
        ///   <c>PageGrid</c> est effectivement résolu dans l'arbre
        ///   XAML ; en cas d'absence ou de cast invalide, la
        ///   stylisation est silencieusement ignorée et la trace de
        ///   diagnostic émise par
        ///   <see cref="Page_Generic.Find{T}(string)"/> assure la
        ///   détectabilité en environnement de
        ///   développement.</description></item>
        ///   <item><description><c>if (Find&lt;FlowDocument&gt;("AppInfoDoc") is FlowDocument appInfoDoc) _controlStyler.StyleAppInfoDoc(appInfoDoc)</c>
        ///   applique la stylisation standard du <c>FlowDocument</c> de
        ///   présentation (police, taille, espacement, marges,
        ///   complément au <c>Foreground</c> blanc porté en dur sur
        ///   l'élément) lorsque le <c>FlowDocument</c> nommé
        ///   <c>AppInfoDoc</c> est effectivement résolu dans l'arbre
        ///   XAML ; en cas d'absence ou de cast invalide, la
        ///   stylisation est silencieusement ignorée et la trace de
        ///   diagnostic émise par
        ///   <see cref="Page_Generic.Find{T}(string)"/> assure la
        ///   détectabilité en environnement de
        ///   développement.</description></item>
        /// </list>
        /// <para>Résolution typée par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> : Les deux
        /// contrôles XAML stylisables sont résolus par le helper
        /// hérité, qui combine <c>FindName(name) as T</c> avec une
        /// trace <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// en cas d'absence ou de cast invalide. Le retour <c>T?</c>
        /// du helper est consommé via une garde <c>is</c> qui
        /// conditionne l'invocation du service <c>IS_ControlStyler</c>
        /// (paramètres non-nullable) au succès de la résolution, selon
        /// la forme dépliée un-à-un
        /// <c>if (Find&lt;T&gt;(name) is T x) _controlStyler.X(x)</c>
        /// prescrite par le patron normatif « Patron de surcharge -
        /// ApplyLayout » de §4.15.7 du 0230 et par la règle R-4.15.25
        /// du 0231, qui constituent l'ancrage doctrinal de la garde et
        /// proscrivent explicitement l'opérateur null-forgiving
        /// (<c>!</c>) pour franchir ce pont. Cette indirection
        /// substitue l'accès direct aux champs nommés générés par
        /// <c>InitializeComponent</c> au profit du patron normatif
        /// susvisé, qui ajoute un filet contre les ruptures
        /// silencieuses de contrat XAML (renommage d'un <c>x:Name</c>
        /// côté XAML sans propagation au code-behind).</para>
        /// <para>Appel à <c>base.ApplyLayout(callChain)</c> en première
        /// instruction : L'implémentation par défaut de
        /// <see cref="Page_Generic.ApplyLayout(string)"/> ne porte
        /// aucun traitement. L'appel est néanmoins conservé en geste
        /// de robustesse vis-à-vis de toute évolution future du socle,
        /// conformément à la convention d'override standard énoncée en
        /// §3.14.4 du 0230 et au patron normatif présenté en §4.15.7
        /// du 0230. La <paramref name="callChain"/> est propagée
        /// telle quelle à <c>base</c> mais n'a pas vocation à être
        /// enrichie ou retransmise au sein du corps : la stylisation
        /// est une opération locale à la vue, sans délégation aval à
        /// un service nécessitant un <c>caller</c>.</para>
        /// <para>Remarques : Cette méthode ne porte aucune interaction
        /// avec <see cref="_viewModel"/> ; la séparation entre
        /// stylisation visuelle (synchrone, locale à la vue) et
        /// chargement de données (asynchrone, porté par le ViewModel)
        /// est stricte, conformément à la séparation sémantique entre
        /// <see cref="ApplyLayout(string)"/> et
        /// <see cref="OnLoadedAsync(string, System.Threading.CancellationToken)"/>
        /// posée par §4.15.7 du 0230.</para>
        /// <para>Filet de sécurité : Aucun try/catch local. La
        /// continuation gracieuse est assurée au niveau du corps : en
        /// cas d'absence ou de cast invalide d'un contrôle XAML, la
        /// garde <c>is</c> n'engage pas l'invocation du service
        /// <c>IS_ControlStyler</c> sur le contrôle concerné, la
        /// stylisation des contrôles suivants n'est pas interrompue,
        /// et la trace de diagnostic émise par
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
        /// du 0230 et à l'EA <c>async void</c> des handlers WPF.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page98 &gt; OnLoadedHandler &gt; ApplyLayout</c>.</param>
        protected override void ApplyLayout(string callChain)
        {
            base.ApplyLayout(callChain);

            if (Find<Grid>("PageGrid") is Grid pageGrid) _controlStyler.StylePage(pageGrid);
            if (Find<FlowDocument>("AppInfoDoc") is FlowDocument appInfoDoc) _controlStyler.StyleAppInfoDoc(appInfoDoc);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
        /// pour déclencher le chargement asynchrone de la propriété
        /// observable <see cref="VM_Page98.VersionNumber"/> par
        /// invocation du hook <c>LoadAsync</c> de
        /// <see cref="VM_Page98"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <c>Page_Generic</c> à l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de la
        /// page, postérieurement à l'application de la stylisation par
        /// <see cref="ApplyLayout(string)"/> et à l'ajustement
        /// dimensionnel initial par
        /// <see cref="Page_Generic.OnResized(string)"/>. Le caractère
        /// asynchrone (<c>Task</c>) est imposé par la signature du
        /// point d'extension de <c>Page_Generic</c> (§4.15.7 du
        /// 0230).</para>
        /// <para>Objectif : Déclencher l'alimentation de la propriété
        /// observable <see cref="VM_Page98.VersionNumber"/> par
        /// invocation du hook canonique <c>LoadAsync</c> de
        /// <see cref="VM_Page98"/> (override du hook formellement
        /// déclaré au socle
        /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>,
        /// §4.15.6 du 0230). Cet override invoque lui-même le UseCase
        /// <c>IU_GetApplicationVersion</c> via le service
        /// <c>IS_UseCaseInvoker</c> (EA-11) au titre du mode
        /// d'invocation normatif des UseCases depuis
        /// <c>D_Presentation</c> (§4.10.10 du 0230). Les 55 propriétés
        /// observables <c>Label_P98_NN</c> ont déjà été alimentées au
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
        /// l'invocation <c>_viewModel.LoadAsync(callChain, ct)</c>.
        /// La <paramref name="callChain"/> reçue est propagée telle
        /// quelle au hook <c>LoadAsync</c> du ViewModel, qui la
        /// consommera pour construire ses propres <c>CallChain</c>s
        /// internes via
        /// <c>VM_Generic.BuildFirstCallChain</c> hérité (§4.15.5 du
        /// 0230). Le <see cref="System.Threading.CancellationToken"/>
        /// est propagé symétriquement à <c>base</c> puis au hook
        /// <c>LoadAsync</c>, autorisant le ViewModel à participer à
        /// l'annulation coopérative jusqu'au UseCase.</para>
        /// <para>Filet de sécurité : Aucun try/catch local. Toute
        /// exception métier levée par
        /// <c>VM_Page98.LoadAsync(callChain, ct)</c> est capturée par
        /// le filet de sécurité métier <c>VM_Generic.ExecuteSafeAsync</c>
        /// porté par le ViewModel (§4.15.5 du 0230) ; toute exception
        /// non gérée ultime serait capturée par le filet de sécurité
        /// ultime de <c>Page_Generic.OnLoadedHandler</c>, qui trace
        /// l'exception via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// sans la propager au framework WPF, conformément à §4.15.7
        /// du 0230 et à l'EA <c>async void</c> des handlers WPF.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page98 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>,
        /// propagée telle quelle à <c>base</c> et au hook
        /// <c>VM_Page98.LoadAsync</c>.</param>
        /// <param name="ct">Token d'annulation coopérative propagé par
        /// le handler appelant (valeur par défaut <c>default</c> côté
        /// <c>Page_Generic</c> qui ne construit pas de
        /// <c>CancellationTokenSource</c> propre), retransmis
        /// symétriquement à <c>base</c> et au hook
        /// <c>VM_Page98.LoadAsync</c>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// chargement de la propriété
        /// <see cref="VM_Page98.VersionNumber"/>.</returns>
        protected override async Task OnLoadedAsync(
            string callChain,
            CancellationToken ct = default)
        {
            await base.OnLoadedAsync(callChain, ct);

            await _viewModel.LoadAsync(callChain, ct);
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}