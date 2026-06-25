using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.D_Presentation.ViewModels.Pages;
using DG244Cutting.D_Presentation.Views.Generic;

namespace DG244Cutting.D_Presentation.Views.Pages
{
    /// <summary>
    /// Vue WPF de la page de sélection de la langue <c>Page02</c> de
    /// l'application DG244Cutting, affichant un titre et une grille
    /// verticale de six boutons illustrés (drapeau du pays + nom
    /// endonyme de la langue + RadioButton mutuellement exclusif via
    /// <c>GroupName="LanguageSelection"</c>), pour la sélection de la
    /// langue d'affichage de l'application parmi français, anglais
    /// britannique, allemand, espagnol, italien et portugais.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille Page de la couche
    /// <c>D_Presentation</c>, dérivé de
    /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/>.
    /// La page est instanciée par le framework WPF de navigation via
    /// <c>Activator.CreateInstance</c> (<c>SR_Navigation.NavigateToPage</c>),
    /// hors conteneur DI. Le constructeur sans paramètre est imposé par
    /// cette contrainte et résout son <c>DataContext</c>
    /// <see cref="VM_Page02"/> via
    /// <c>App.ServiceProvider.GetRequiredService</c> au titre de l'EA-02
    /// Service Locator héritée de <c>Page_Generic</c> (§4.15.7 et
    /// §4.15.10 du 0230). La page est accessible à tout utilisateur
    /// connecté et présente une commande utilisateur unique de
    /// changement de langue exposée par
    /// <see cref="VM_Page02.ChangeLanguageCommand"/>, bindée sur les
    /// six boutons de langue. La sortie de la page s'effectue
    /// exclusivement via les boutons transverses du menu horizontal
    /// (Home, Previous) portés par le couple <c>VM_MH02</c> /
    /// <c>MH02</c> ; à l'inverse du couple legacy
    /// <c>BatchStockRelease.Page91</c> qui redirigeait vers
    /// <c>Page10</c> à la fin du changement de langue, la
    /// <see cref="Page02"/> reste affichée après application — la
    /// cascade INPC sur <see cref="DG244Cutting.A_Domain.Interfaces.Settings.App.ISE_App.AppCultureCode"/>
    /// propage automatiquement le rafraîchissement des libellés
    /// multilingues à tous les ViewModels actifs sans nécessité de
    /// navigation forcée.</para>
    ///
    /// <para>Objectif : Constituer la vue WPF permanente de la page
    /// de sélection de la langue, résoudre <see cref="VM_Page02"/>
    /// via le ServiceProvider, l'affecter à
    /// <see cref="System.Windows.FrameworkElement.DataContext"/> pour
    /// activer les bindings WPF des 7 propriétés observables de
    /// libellés multilingues (<c>Label_P02_00</c> à
    /// <c>Label_P02_06</c>), des 6 propriétés observables de
    /// sélection (<c>IsLanguage{n}Selected</c>), des 6 propriétés
    /// auto-property d'URI de drapeau (<c>Flag{n}Source</c>) et de
    /// la commande <see cref="VM_Page02.ChangeLanguageCommand"/>,
    /// appliquer au chargement la stylisation des contrôles XAML
    /// nommés (<c>Grid</c> extérieur <c>PageGrid</c> stylé directement
    /// puis utilisé comme racine du balayage logique du dispatcher
    /// par convention de nommage
    /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.ApplyStylesToTextBlocks(System.Windows.DependencyObject)"/>,
    /// et six triplets <c>Language{n}Button</c> +
    /// <c>Language{n}Image</c> + <c>Language{n}RadioButton</c>
    /// stylés un-à-un via <c>StyleLanguageButton</c>), et déclencher au
    /// <see cref="System.Windows.FrameworkElement.Loaded"/>
    /// l'initialisation asynchrone de l'état de sélection visuelle
    /// par invocation du hook <c>LoadAsync</c> de
    /// <see cref="VM_Page02"/> (override du hook canonique déclaré
    /// au socle
    /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>,
    /// §4.15.6 du 0230).</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Servir de vue WPF — la composition XAML
    ///   est portée par <c>Page02.xaml</c> et se compose d'un
    ///   <c>Grid</c> extérieur nommé <c>PageGrid</c> à 3 colonnes
    ///   (<c>*</c>, <c>Auto</c>, <c>*</c>) et 4 lignes (<c>*</c>,
    ///   <c>50</c>, <c>Auto</c>, <c>*</c>) ; un <c>TextBlock</c>
    ///   nommé <c>PageTitleMain</c> en ligne 1 colonne 1 portant le
    ///   titre principal de la page bindé sur
    ///   <see cref="VM_Page02.Label_P02_00"/> ; un sous-Grid en
    ///   ligne 2 colonne 1 à 6 lignes de <c>50</c> × 1 colonne de
    ///   <c>355</c> contenant 6 <c>Button</c>s nommés
    ///   <c>Language{1..6}Button</c> portant chacun un <c>StackPanel</c>
    ///   horizontal avec un <c>Image</c> nommé
    ///   <c>Language{n}Image</c> (<c>Source</c> bindée sur
    ///   <c>Flag{n}Source</c>), un <c>RadioButton</c> nommé
    ///   <c>Language{n}RadioButton</c> (<c>IsChecked</c> bindée sur
    ///   <c>IsLanguage{n}Selected</c> en sélection mutuellement
    ///   exclusive par <c>GroupName="LanguageSelection"</c>) et un
    ///   <c>TextBlock</c> nommé <c>Language{n}Data</c> (<c>Text</c>
    ///   bindée sur <c>Label_P02_0{n}</c>). Aucun <c>FlowDocument</c>,
    ///   aucun <c>UT_BindingProxy</c> n'est nécessaire — la dérogation
    ///   au pattern <c>UT_BindingProxy</c> n'étant requise que pour
    ///   les descendants d'un <c>FlowDocument</c> qui ne participent
    ///   pas à l'héritage standard du <c>DataContext</c> WPF
    ///   (cf. §3.2.2 du 0232-Page-VM).</description></item>
    ///   <item><description>Résoudre <see cref="VM_Page02"/> via
    ///   <c>App.ServiceProvider.GetRequiredService</c> dans le
    ///   constructeur sans paramètre, conformément au contrat de la
    ///   convention de plateforme <c>App.ServiceProvider</c>
    ///   (§4.15.11 du 0230) et à l'EA-02 Service Locator étendue aux
    ///   dérivés directs de <c>Page_Generic</c> pour la résolution
    ///   de leur ViewModel (§4.15.7 du 0230).</description></item>
    ///   <item><description>Affecter
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/> à
    ///   l'instance de <see cref="VM_Page02"/> pour alimenter les
    ///   bindings WPF des 7 libellés multilingues, des 6 booléens
    ///   de sélection, des 6 URIs de drapeau et de la commande
    ///   <see cref="VM_Page02.ChangeLanguageCommand"/>.</description></item>
    ///   <item><description>Appliquer la stylisation initiale des
    ///   contrôles XAML via le service <c>IS_ControlStyler</c>
    ///   hérité de <c>Page_Generic</c> (champ
    ///   <see cref="Page_Generic._controlStyler"/>) par surcharge de
    ///   <see cref="Page_Generic.ApplyLayout(string)"/>, invoquée par
    ///   le filet de sécurité ultime du handler
    ///   <c>OnLoadedHandler</c> de <c>Page_Generic</c>
    ///   préalablement à
    ///   <see cref="Page_Generic.OnResized(string)"/> puis à
    ///   <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>.
    ///   La stylisation procède en deux temps : (1) le <c>PageGrid</c>
    ///   est résolu une fois par le helper
    ///   <see cref="Page_Generic.Find{T}(string)"/> hérité puis stylé
    ///   directement par <c>StylePage</c> et utilisé comme racine du
    ///   balayage logique du dispatcher
    ///   <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.ApplyStylesToTextBlocks(System.Windows.DependencyObject)"/>
    ///   qui applique automatiquement le style adéquat à chaque
    ///   <see cref="TextBlock"/> descendant selon la convention de
    ///   nommage (préfixe <c>PageTitle</c> pour
    ///   <c>PageTitleMain</c> → <c>StyleTextBlockPageTitle</c> ;
    ///   suffixe <c>Data</c> pour les six <c>Language{n}Data</c> →
    ///   <c>StyleTextBlockData</c>) ; (2) les 18 contrôles des six
    ///   triplets <c>Language{n}Button</c> + <c>Language{n}Image</c>
    ///   + <c>Language{n}RadioButton</c> sont résolus par le helper
    ///   <see cref="Page_Generic.Find{T}(string)"/> sous forme
    ///   conjointe à triple garde et stylés en un seul appel par
    ///   triplet via <c>StyleLanguageButton</c> avec la largeur
    ///   fixée par la constante privée
    ///   <see cref="LanguageButtonWidth"/>. Le helper
    ///   <see cref="Page_Generic.Find{T}(string)"/> assortit la
    ///   résolution typée <c>FindName(name) as T</c> d'une trace de
    ///   diagnostic émise en cas d'absence (filet contre les ruptures
    ///   silencieuses de contrat XAML).</description></item>
    ///   <item><description>Déclencher au
    ///   <see cref="System.Windows.FrameworkElement.Loaded"/>
    ///   l'initialisation asynchrone de l'état de sélection
    ///   visuelle des six <c>RadioButton</c> à partir de la
    ///   composante pays de
    ///   <see cref="DG244Cutting.A_Domain.Interfaces.Settings.App.ISE_App.AppCultureCode"/>,
    ///   par surcharge de
    ///   <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
    ///   et invocation de
    ///   <c>VM_Page02.LoadAsync(callChain, ct)</c>, hook canonique
    ///   formellement déclaré au socle
    ///   <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>
    ///   (§4.15.6 du 0230). La <c>CallChain</c> reçue du handler
    ///   <c>OnLoadedHandler</c> est propagée telle quelle au hook
    ///   <c>LoadAsync</c>, qui la consomme pour construire ses
    ///   propres <c>CallChain</c>s internes via
    ///   <c>VM_Generic.BuildFirstCallChain</c> hérité.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune logique métier (R-4.12.1,
    ///   I-4.12.1 du 0231) — le code-behind est strictement borné
    ///   au câblage Vue/ViewModel (constructeur, override des points
    ///   d'extension de <see cref="Page_Generic"/>) et à la
    ///   mécanique de plateforme (stylisation invariante au
    ///   <c>Loaded</c>, invocation de <c>_viewModel.LoadAsync</c>
    ///   depuis <see cref="OnLoadedAsync"/>). En particulier, aucun
    ///   chargement d'images en code-behind ni en XAML
    ///   (I-4.11.10 du 0231) : les six drapeaux sont alimentés par
    ///   les bindings WPF sur les propriétés
    ///   <see cref="VM_Page02.Flag1Source"/> à
    ///   <see cref="VM_Page02.Flag6Source"/> du ViewModel, elles-mêmes
    ///   résolues une fois au constructeur du ViewModel via
    ///   <c>ISE_Flag.GetFlagUriOrDefault</c> sur les six codes pays
    ///   ISO 3166-1 alpha-2 (« FR », « GB », « DE », « ES », « IT »,
    ///   « PT »), au contraire du couple legacy
    ///   <c>BatchStockRelease.Page91</c> qui chargeait les drapeaux
    ///   en code-behind via <c>new BitmapImage(...)</c>.</description></item>
    ///   <item><description>Aucun chargement de libellés
    ///   multilingues depuis le code-behind ou le XAML
    ///   (I-4.11.10 du 0231) ; les libellés sont chargés
    ///   exclusivement par le ViewModel via la mécanique multilingue
    ///   héritée de <c>VM_Generic</c>. Les six
    ///   <c>CommandParameter</c> littéraux des six boutons de
    ///   langue (« FR », « GB », « DE », « ES », « IT », « PT »)
    ///   ne sont pas des chaînes UI au sens d'I-4.11.1 mais des
    ///   paramètres applicatifs (codes pays ISO 3166-1 alpha-2)
    ///   transmis à
    ///   <see cref="VM_Page02.ChangeLanguageCommand"/> ; ils
    ///   relèvent légitimement du contrat de la commande et non du
    ///   texte UI.</description></item>
    ///   <item><description>Aucune chaîne UI en dur dans le XAML
    ///   (I-4.11.1 du 0231) ; aucun <c>DynamicResource</c> ni
    ///   <c>StaticResource</c> pour les textes UI (I-4.11.10 du
    ///   0231) : tout texte affiché est issu d'un binding sur une
    ///   propriété observable du ViewModel.</description></item>
    ///   <item><description>Aucune injection ni résolution directe
    ///   d'un contrat <c>IU_</c> ou <c>IQ_</c> dans le code-behind
    ///   (I-4.10.10 du 0231) ; la consommation de UseCases et de
    ///   Query Handlers est intégralement portée par le ViewModel
    ///   via <c>IS_UseCaseInvoker</c> (EA-11).</description></item>
    ///   <item><description>Aucun ajustement dimensionnel par
    ///   override d'<see cref="Page_Generic.OnResized(string)"/> :
    ///   la composition XAML est intégralement responsive par les
    ///   mécanismes WPF standards de <c>Grid</c> (étoiles <c>*</c>
    ///   et <c>Auto</c> sur les définitions de colonnes et de
    ///   lignes), aucun contrôle ne porte de hauteur calculée par
    ///   le code-behind. L'implémentation par défaut de
    ///   <see cref="Page_Generic.OnResized(string)"/> (corps vide)
    ///   suffit au cas d'usage de
    ///   <see cref="Page02"/>.</description></item>
    ///   <item><description>Aucune libération de ressource au
    ///   démontage par override d'<c>OnUnloadedAsync</c> : la page
    ///   ne détient aucune ressource asynchrone propre,
    ///   l'implémentation par défaut de
    ///   <see cref="Page_Generic"/> suffit.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales : Aucune
    /// exception architecturale propre n'est portée par
    /// <see cref="Page02"/>. La résolution de
    /// <see cref="VM_Page02"/> via
    /// <c>App.ServiceProvider.GetRequiredService</c> relève de
    /// l'EA-02 Service Locator héritée de
    /// <see cref="Page_Generic"/> (§4.15.7 et §4.15.11 du 0230),
    /// étendue aux dérivés directs de <c>Page_Generic</c> pour la
    /// résolution de leur ViewModel ; cette exception est héritée
    /// et non re-déclarée à ce niveau.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq
    /// régions standard (§4.4.2) complétée par une extension
    /// (R-4.4.10 du 0231) au titre des overrides protégés de
    /// <see cref="ApplyLayout"/> et <see cref="OnLoadedAsync"/>,
    /// soit six régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   constante privée <see cref="LanguageButtonWidth"/> portant
    ///   la largeur en unités WPF des six boutons de langue (valeur
    ///   <c>300</c>), transmise au quatrième paramètre <c>width</c>
    ///   du contrat <c>StyleLanguageButton</c> par les six
    ///   invocations de
    ///   <see cref="ApplyLayout"/>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   champ <see cref="_viewModel"/> stockant l'instance
    ///   Singleton de <see cref="VM_Page02"/> résolue au
    ///   constructeur via
    ///   <c>App.ServiceProvider.GetRequiredService</c>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur sans paramètre <c>public</c> imposé par le
    ///   framework WPF de navigation, résolvant
    ///   <see cref="VM_Page02"/> et l'affectant à
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   présente mais vide, marqueur
    ///   <c>// A compléter</c>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c>
    ///   (extension §4.4.3) : deux overrides —
    ///   <see cref="ApplyLayout"/> pour la stylisation initiale des
    ///   contrôles XAML nommés par <c>IS_ControlStyler</c>
    ///   (<c>StylePage</c> + dispatcher
    ///   <c>ApplyStylesToTextBlocks</c> sur le <c>PageGrid</c>, puis
    ///   six invocations un-à-un de
    ///   <c>StyleLanguageButton</c> sur les six triplets de boutons
    ///   de langue),
    ///   <see cref="OnLoadedAsync"/> pour l'invocation du hook
    ///   <c>LoadAsync</c> du ViewModel au montage de la page
    ///   (ancrage canonique invariant 19 de §2.1 du 0232-Page-VM).
    ///   Aucun override d'<see cref="Page_Generic.OnResized(string)"/>
    ///   ni d'<c>OnUnloadedAsync</c> : la page ne porte aucun
    ///   ajustement dimensionnel calculé et ne détient aucune
    ///   ressource asynchrone à libérer au démontage.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   présente mais vide, marqueur
    ///   <c>// A compléter</c>.</description></item>
    /// </list>
    /// </remarks>
    public partial class Page02 : Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Largeur en unités WPF des six boutons de langue, transmise
        /// au quatrième paramètre <c>width</c> de
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleLanguageButton(System.Windows.Controls.Button, System.Windows.Controls.Image, System.Windows.Controls.RadioButton, double?)"/>
        /// par les six invocations de <see cref="ApplyLayout"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constante de mise en forme propre à la
        /// présente vue, extraite en propriété privée pour éliminer
        /// la valeur magique dupliquée six fois dans le corps de
        /// <see cref="ApplyLayout"/>. La largeur est passée par
        /// boxing implicite <c>double</c> → <c>double?</c> au contrat
        /// de <c>StyleLanguageButton</c>.</para>
        /// <para>Note sur la valeur : <c>300</c> unités WPF (et non
        /// <c>350</c> du legacy <c>BatchStockRelease.Page91.xaml.cs</c>
        /// qui appelait <c>_controlStyler.StyleLanguageButton(...,
        /// 350)</c>). La réduction de la largeur traduit un choix
        /// d'ergonomie spécifique à DG244Cutting et n'a aucune
        /// incidence sur la sémantique applicative — la composition
        /// XAML reste responsive par les mécanismes WPF standards de
        /// <see cref="System.Windows.Controls.Grid"/> (étoiles <c>*</c>
        /// et <c>Auto</c> sur les définitions de colonnes du Grid
        /// extérieur <c>PageGrid</c>).</para>
        /// </remarks>
        private const double LanguageButtonWidth = 300;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Instance Singleton du ViewModel associé à la présente vue,
        /// résolue au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> et affectée à
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// pour alimenter les bindings WPF des 20 membres exposés par
        /// <see cref="VM_Page02"/> (7 libellés multilingues
        /// <c>Label_P02_0n</c>, 6 booléens
        /// <c>IsLanguage{n}Selected</c>, 6 URIs
        /// <c>Flag{n}Source</c>, 1 commande
        /// <c>ChangeLanguageCommand</c>) déclarés par
        /// <c>Page02.xaml</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Champ stocké en lecture seule pour
        /// exposer le type concret <see cref="VM_Page02"/> au
        /// code-behind, distinct du
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// typé en <see cref="object"/>. Consommé par
        /// <see cref="OnLoadedAsync"/> pour l'invocation du hook
        /// <c>LoadAsync</c> au montage de la page, et nulle part
        /// ailleurs — la page n'invoque aucune commande du ViewModel
        /// depuis le code-behind, conformément à la séparation MVVM
        /// stricte. La commande
        /// <see cref="VM_Page02.ChangeLanguageCommand"/> est invoquée
        /// exclusivement par WPF via le binding
        /// <c>Button.Command</c> des six boutons de langue.</para>
        /// </remarks>
        private readonly VM_Page02 _viewModel;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="Page02"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur sans paramètre imposé par
        /// le framework WPF de navigation, qui instancie la page via
        /// <c>Activator.CreateInstance</c> au sein de
        /// <c>SR_Navigation.NavigateToPage</c>. La résolution des
        /// dépendances ne peut donc se faire par injection paramétrée
        /// et s'effectue par le canal légitime
        /// <c>App.ServiceProvider.GetRequiredService</c>, au titre de
        /// la convention de plateforme documentée en §4.15.11 du 0230
        /// et de l'EA-02 Service Locator étendue aux dérivés directs
        /// de <c>Page_Generic</c> pour la résolution de leur
        /// ViewModel.</para>
        ///
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Résolution de <see cref="VM_Page02"/>
        ///   via <c>App.ServiceProvider.GetRequiredService</c> et
        ///   stockage dans le champ <see cref="_viewModel"/>. La
        ///   méthode <c>GetRequiredService</c> est utilisée (et non
        ///   <c>GetService</c>), conformément à la règle 2 de §4.15.11
        ///   du 0230 : toute dépendance non résolue doit faire échouer
        ///   l'instanciation immédiatement par exception explicite
        ///   plutôt que de produire une
        ///   <see cref="NullReferenceException"/> ultérieure.</description></item>
        ///   <item><description>Invocation de <c>InitializeComponent</c>
        ///   pour la composition XAML — étape impérativement
        ///   préalable à toute affectation de
        ///   <see cref="System.Windows.FrameworkElement.DataContext"/>.</description></item>
        ///   <item><description>Affectation de
        ///   <see cref="System.Windows.FrameworkElement.DataContext"/>
        ///   à <see cref="_viewModel"/> pour activer les bindings WPF
        ///   des 20 membres exposés par
        ///   <see cref="VM_Page02"/>.</description></item>
        /// </list>
        ///
        /// <para>Filet de sécurité : Aucune invocation susceptible
        /// de lever une exception terminale n'est portée par le
        /// constructeur au-delà de la résolution du ViewModel. Une
        /// défaillance de <c>GetRequiredService</c> traduirait une
        /// erreur de configuration du conteneur DI (par exemple
        /// <c>VM_Page02</c> non enregistré, ou défaillance d'une
        /// dépendance résolue en cascade par le ViewModel) et doit
        /// faire échouer l'instanciation immédiatement. Le filet de
        /// sécurité ultime au bord des handlers WPF est porté par
        /// <c>Page_Generic</c> et couvre les éventuelles défaillances
        /// survenant au chargement, au déchargement et au
        /// redimensionnement de la page.</para>
        /// </remarks>
        public Page02()
        {
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page02>();

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
        /// (<c>Grid</c> extérieur <c>PageGrid</c> stylé directement
        /// puis utilisé comme racine du balayage logique du dispatcher
        /// par convention de nommage
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.ApplyStylesToTextBlocks(System.Windows.DependencyObject)"/>,
        /// et six triplets <c>Language{n}Button</c> +
        /// <c>Language{n}Image</c> + <c>Language{n}RadioButton</c>
        /// stylés un-à-un via
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleLanguageButton(System.Windows.Controls.Button, System.Windows.Controls.Image, System.Windows.Controls.RadioButton, double?)"/>)
        /// via le service <c>IS_ControlStyler</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <c>Page_Generic</c> à
        /// l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de la
        /// page, préalablement à
        /// <see cref="Page_Generic.OnResized(string)"/> (corps par
        /// défaut vide) puis à
        /// <see cref="OnLoadedAsync(string, System.Threading.CancellationToken)"/>.
        /// Le caractère synchrone est imposé par la signature du
        /// point d'extension de <c>Page_Generic</c> (§4.15.7 du
        /// 0230). La <paramref name="callChain"/> reçue est
        /// construite par le handler <c>OnLoadedHandler</c> sous la
        /// forme
        /// <c>Page02 &gt; OnLoadedHandler &gt; ApplyLayout</c>,
        /// conformément au patron méthode publique de §4.5.1 du
        /// 0230.</para>
        ///
        /// <para>Objectif : Appliquer la stylisation visuelle de
        /// l'ensemble des contrôles XAML nommés de la page selon
        /// l'ordre logique de l'arbre visuel :
        /// <c>PageGrid</c> stylé directement par
        /// <c>StylePage</c> puis utilisé comme racine du balayage
        /// logique du dispatcher
        /// <c>ApplyStylesToTextBlocks</c> qui applique les styles
        /// adéquats à tous les <see cref="TextBlock"/> descendants
        /// selon la convention de nommage (préfixe <c>PageTitle</c>
        /// pour <c>PageTitleMain</c> →
        /// <c>StyleTextBlockPageTitle</c> ; suffixe <c>Data</c> pour
        /// les six <c>Language{n}Data</c> →
        /// <c>StyleTextBlockData</c>), puis six triplets
        /// <c>Language{n}Button</c> + <c>Language{n}Image</c> +
        /// <c>Language{n}RadioButton</c> stylisés en un seul appel
        /// par triplet via
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleLanguageButton(System.Windows.Controls.Button, System.Windows.Controls.Image, System.Windows.Controls.RadioButton, double?)"/>
        /// avec la largeur fixée par la constante privée
        /// <see cref="LanguageButtonWidth"/>. Le <c>Grid</c>
        /// extérieur <c>PageGrid</c> porte un <c>x:Name</c> dans
        /// <c>Page02.xaml</c> et est résolu par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> hérité ; les
        /// 18 contrôles des six triplets sont également résolus par
        /// <see cref="Page_Generic.Find{T}(string)"/> chacun pour
        /// passation au contrat de
        /// <c>StyleLanguageButton</c>.</para>
        ///
        /// <para>Dispatcher
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.ApplyStylesToTextBlocks(System.Windows.DependencyObject)"/> :
        /// La méthode parcourt récursivement le LogicalTree à partir
        /// du <see cref="DependencyObject"/> fourni
        /// (<c>PageGrid</c> ici) et applique automatiquement à chaque
        /// <see cref="TextBlock"/> rencontré le style adéquat selon
        /// son <c>x:Name</c> :
        /// <list type="bullet">
        ///   <item><description>Nom commençant par <c>PageTitle</c>
        ///   (insensible à la casse) → <c>StyleTextBlockPageTitle</c>
        ///   (cas du <c>PageTitleMain</c> de la présente page).</description></item>
        ///   <item><description>Nom se terminant par <c>Title</c> →
        ///   <c>StyleTextBlockTitle</c> (non employé ici).</description></item>
        ///   <item><description>Nom se terminant par <c>Data</c> →
        ///   <c>StyleTextBlockData</c> (cas des six
        ///   <c>Language{n}Data</c> de la présente page).</description></item>
        ///   <item><description>Nom se terminant par <c>Input</c> →
        ///   <c>StyleTextBlockInput</c> (non employé ici).</description></item>
        ///   <item><description>Autre cas → non stylé.</description></item>
        /// </list>
        /// Cette factorisation introduit une dépendance implicite
        /// entre le nommage des contrôles dans
        /// <c>Page02.xaml</c> et le comportement du code-behind :
        /// toute modification du nom d'un <c>TextBlock</c> casserait
        /// silencieusement la stylisation. Le contrat de
        /// <c>ApplyStylesToTextBlocks</c> n'étant pas documenté à ce
        /// jour dans le 0232-Page-VM, cette dépendance fait l'objet
        /// d'une remarque écosystémique à consigner en clôture du
        /// fil.</para>
        ///
        /// <para>Résolution typée par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> : Chaque
        /// contrôle XAML stylisable est résolu par le helper hérité,
        /// qui combine <c>FindName(name) as T</c> avec une trace
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// en cas d'absence ou de cast invalide. Le retour <c>T?</c>
        /// du helper est consommé via une garde <c>is</c> qui
        /// conditionne l'invocation du service
        /// <c>IS_ControlStyler</c> (paramètres non-nullable) au
        /// succès de la résolution, selon la forme dépliée un-à-un
        /// <c>if (Find&lt;T&gt;(name) is T x) _controlStyler.X(x)</c>
        /// prescrite par le patron normatif « Patron de surcharge -
        /// ApplyLayout » de §4.15.7 du 0230 et par la règle
        /// R-4.15.25 du 0231, qui constituent l'ancrage doctrinal
        /// de la garde et proscrivent explicitement l'opérateur
        /// null-forgiving (<c>!</c>) pour franchir ce pont.</para>
        ///
        /// <para>Bloc <c>StyleLanguageButton</c> par triplet : Le
        /// contrat
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleLanguageButton(System.Windows.Controls.Button, System.Windows.Controls.Image, System.Windows.Controls.RadioButton, double?)"/>
        /// expose trois paramètres non-nullable (le <c>Button</c>,
        /// l'<c>Image</c> du drapeau et le <c>RadioButton</c>) et un
        /// quatrième paramètre <c>double?</c> de largeur optionnelle,
        /// alimenté ici par la constante privée
        /// <see cref="LanguageButtonWidth"/> (boxing implicite
        /// <c>double</c> → <c>double?</c>). Le patron normatif de
        /// garde par contrôle (R-4.15.25 du 0231) impose la
        /// résolution typée de chaque contrôle suivie d'une garde
        /// <c>is</c>. Pour conserver l'invocation unique (les trois
        /// paramètres devant être passés ensemble dans une seule
        /// invocation du service), le patron est appliqué sous la
        /// forme conjointe à triple garde
        /// <c>if (Find&lt;Button&gt;(buttonName) is Button button
        /// &amp;&amp; Find&lt;Image&gt;(imageName) is Image image
        /// &amp;&amp; Find&lt;RadioButton&gt;(radioName) is RadioButton radio)
        /// _controlStyler.StyleLanguageButton(button, image, radio, LanguageButtonWidth);</c>.
        /// Si l'un quelconque des trois contrôles d'un triplet est
        /// absent du XAML (cas marginal d'erreur structurelle), la
        /// stylisation du triplet entier est neutralisée et trois
        /// traces de diagnostic
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// sont émises par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> sur les
        /// contrôles manquants, sans propagation d'exception au
        /// framework WPF.</para>
        ///
        /// <para>Appel à <c>base.ApplyLayout(callChain)</c> en
        /// première instruction : L'implémentation par défaut de
        /// <see cref="Page_Generic.ApplyLayout"/> ne porte aucun
        /// traitement. L'appel est néanmoins conservé en geste de
        /// robustesse vis-à-vis de toute évolution future du socle,
        /// conformément à la convention d'override standard et au
        /// patron normatif présenté en §4.15.7 du 0230.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. La
        /// continuation gracieuse est assurée au niveau du corps :
        /// en cas d'absence ou de cast invalide d'un contrôle XAML,
        /// la garde <c>is</c> n'engage pas l'invocation du service
        /// <c>IS_ControlStyler</c>, et la trace de diagnostic émise
        /// par <see cref="Page_Generic.Find{T}(string)"/> via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// assure la détectabilité en environnement de développement.
        /// Toute exception qui parviendrait néanmoins à être levée
        /// par <c>IS_ControlStyler</c> ou par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> serait capturée
        /// par le filet de sécurité ultime de
        /// <c>Page_Generic.OnLoadedHandler</c> (try/catch englobant
        /// le handler), qui trace l'exception via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// sans la propager au framework WPF, conformément à §4.15.7
        /// du 0230.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page02 &gt; OnLoadedHandler &gt; ApplyLayout</c>.</param>
        protected override void ApplyLayout(string callChain)
        {
            base.ApplyLayout(callChain);

            if (Find<Grid>("PageGrid") is Grid pageGrid)
            {
                _controlStyler.StylePage(pageGrid);
                _controlStyler.ApplyStylesToTextBlocks(pageGrid);
            }

            if (Find<Button>("Language1Button") is Button language1Button
                && Find<Image>("Language1Image") is Image language1Image
                && Find<RadioButton>("Language1RadioButton") is RadioButton language1RadioButton)
                _controlStyler.StyleLanguageButton(language1Button, language1Image, language1RadioButton, LanguageButtonWidth);

            if (Find<Button>("Language2Button") is Button language2Button
                && Find<Image>("Language2Image") is Image language2Image
                && Find<RadioButton>("Language2RadioButton") is RadioButton language2RadioButton)
                _controlStyler.StyleLanguageButton(language2Button, language2Image, language2RadioButton, LanguageButtonWidth);

            if (Find<Button>("Language3Button") is Button language3Button
                && Find<Image>("Language3Image") is Image language3Image
                && Find<RadioButton>("Language3RadioButton") is RadioButton language3RadioButton)
                _controlStyler.StyleLanguageButton(language3Button, language3Image, language3RadioButton, LanguageButtonWidth);

            if (Find<Button>("Language4Button") is Button language4Button
                && Find<Image>("Language4Image") is Image language4Image
                && Find<RadioButton>("Language4RadioButton") is RadioButton language4RadioButton)
                _controlStyler.StyleLanguageButton(language4Button, language4Image, language4RadioButton, LanguageButtonWidth);

            if (Find<Button>("Language5Button") is Button language5Button
                && Find<Image>("Language5Image") is Image language5Image
                && Find<RadioButton>("Language5RadioButton") is RadioButton language5RadioButton)
                _controlStyler.StyleLanguageButton(language5Button, language5Image, language5RadioButton, LanguageButtonWidth);

            if (Find<Button>("Language6Button") is Button language6Button
                && Find<Image>("Language6Image") is Image language6Image
                && Find<RadioButton>("Language6RadioButton") is RadioButton language6RadioButton)
                _controlStyler.StyleLanguageButton(language6Button, language6Image, language6RadioButton, LanguageButtonWidth);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
        /// pour déclencher l'initialisation asynchrone de l'état de
        /// sélection visuelle des six <c>RadioButton</c> à partir de
        /// la composante pays de
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Settings.App.ISE_App.AppCultureCode"/>,
        /// par invocation du hook <c>LoadAsync</c> de
        /// <see cref="VM_Page02"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <c>Page_Generic</c> à
        /// l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de la
        /// page, postérieurement à l'application de la stylisation
        /// par <see cref="ApplyLayout(string)"/> et à l'ajustement
        /// dimensionnel initial par
        /// <see cref="Page_Generic.OnResized(string)"/> (corps par
        /// défaut vide, non override par <see cref="Page02"/>). Le
        /// caractère asynchrone (<see cref="Task"/>) est imposé par
        /// la signature du point d'extension de
        /// <c>Page_Generic</c> (§4.15.7 du 0230).</para>
        ///
        /// <para>Objectif : Déclencher l'initialisation de l'état de
        /// sélection visuelle des six propriétés observables
        /// <c>IsLanguage{n}Selected</c> par invocation du hook
        /// canonique <c>LoadAsync</c> de <see cref="VM_Page02"/>
        /// (override du hook formellement déclaré au socle
        /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>,
        /// §4.15.6 du 0230). Cet override consomme la composante
        /// pays de
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Settings.App.ISE_App.AppCultureCode"/>
        /// via
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Settings.Presentation.ISE_Language.ExtractCountryCodeFromCulture(string)"/>
        /// pour identifier la langue active, puis met à jour les six
        /// booléens via le helper privé du ViewModel
        /// <c>ApplyLanguageSelection</c>. Les 7 propriétés
        /// observables <c>Label_P02_0n</c> et les 6 propriétés
        /// <c>Flag{n}Source</c> ont déjà été alimentées au
        /// constructeur du ViewModel (libellés via l'orchestration
        /// <c>InitializeLabels</c> de <c>VM_Generic</c> ; URIs via
        /// <c>ISE_Flag.GetFlagUriOrDefault</c>) et ne nécessitent
        /// pas de chargement asynchrone au
        /// <c>Loaded</c>.</para>
        ///
        /// <para>Patron de surcharge normatif (§4.15.7 du 0230) :
        /// Le corps comprend exactement deux instructions, l'appel à
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
        /// internes via <c>VM_Generic.BuildFirstCallChain</c> hérité
        /// (§4.15.5 du 0230). Le
        /// <see cref="System.Threading.CancellationToken"/> est
        /// propagé symétriquement à <c>base</c> puis au hook
        /// <c>LoadAsync</c>, autorisant le ViewModel à participer à
        /// l'annulation coopérative.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. Toute
        /// exception métier levée par
        /// <c>VM_Page02.LoadAsync(callChain, ct)</c> est capturée
        /// par le filet de sécurité métier
        /// <c>VM_Generic.ExecuteSafeAsync</c> porté par le ViewModel
        /// (§4.15.5 du 0230) ; toute exception non gérée ultime
        /// serait capturée par le filet de sécurité ultime de
        /// <c>Page_Generic.OnLoadedHandler</c>, qui trace l'exception
        /// via <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// sans la propager au framework WPF, conformément à §4.15.7
        /// du 0230 et à l'EA <c>async void</c> des handlers WPF.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page02 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>,
        /// propagée telle quelle à <c>base</c> et au hook
        /// <c>VM_Page02.LoadAsync</c>.</param>
        /// <param name="ct">Token d'annulation coopérative propagé
        /// par le handler appelant (valeur par défaut <c>default</c>
        /// côté <c>Page_Generic</c> qui ne construit pas de
        /// <c>CancellationTokenSource</c> propre), retransmis
        /// symétriquement à <c>base</c> et au hook
        /// <c>VM_Page02.LoadAsync</c>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone de
        /// l'initialisation de l'état de sélection visuelle des six
        /// <c>RadioButton</c>.</returns>
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