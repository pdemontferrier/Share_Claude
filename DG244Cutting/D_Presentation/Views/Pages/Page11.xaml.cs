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
    /// série (premier onglet) et quatre onglets d'accueil destinés aux
    /// tableaux des commandes clients, des châssis, des barres optimisées
    /// et des découpes.
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
    ///   activer les dix-neuf bindings déclarés par
    ///   <c>Page11.xaml</c>.</description></item>
    ///   <item><description>Appliquer au <c>Loaded</c> la stylisation
    ///   invariante des quatorze contrôles XAML nommés stylisables via le
    ///   service <c>IS_ControlStyler</c> hérité de
    ///   <see cref="Page_Generic"/>.</description></item>
    ///   <item><description>Ajuster au <c>Loaded</c> puis à chaque
    ///   <c>SizeChanged</c> la hauteur du <c>TabControl</c> à la hauteur
    ///   de fenêtre courante lue sur <c>ISE_Window</c>.</description></item>
    ///   <item><description>Amorcer au <c>Loaded</c> le chargement
    ///   asynchrone des sept caractéristiques de la fiche de synthèse par
    ///   invocation de <see cref="VM_Page11.LoadAsync"/>, au titre de
    ///   l'ancrage canonique <c>OnLoadedAsync</c> →
    ///   <c>LoadAsync</c>.</description></item>
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
    ///   I-4.11.10 du 0231 : les douze libellés sont chargés exclusivement
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
    /// L'EA-03 (« Handlers WPF async void ») et l'EA-04
    /// (« Page_Generic concrète non-abstract ») sont portées par le socle
    /// et non re-déclarées à ce niveau. Aucune exception architecturale
    /// propre n'est portée par <see cref="Page11"/>.</para>
    ///
    /// <para>Absence délibérée de stylisation des trois cases à cocher :
    /// Les trois <c>CheckBox</c> de la fiche de synthèse
    /// (<c>CuttingStartedCheckBox</c>, <c>CuttingCompletedCheckBox</c>,
    /// <c>BarOutOfStockCheckBox</c>) ne sont ni résolues ni stylisées par
    /// <see cref="ApplyLayout"/> : le contrat <c>IS_ControlStyler</c>
    /// n'expose aucune méthode dédiée à ce type de contrôle. Elles
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
    ///   <item><description><c>=== Propriétés privées ===</c> : constante
    ///   <see cref="HeaderWidth"/>, largeur uniforme des cinq en-têtes
    ///   d'onglets.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> : champ
    ///   <see cref="_viewModel"/> stockant l'instance Singleton de
    ///   <see cref="VM_Page11"/> résolue au constructeur via
    ///   <c>App.ServiceProvider.GetRequiredService</c>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   sans paramètre <c>public</c> imposé par le framework WPF de
    ///   navigation, résolvant <see cref="VM_Page11"/> et l'affectant à
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>. La page n'expose
    ///   aucun membre public au-delà de son
    ///   constructeur.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> : overrides
    ///   <see cref="ApplyLayout"/>, <see cref="OnLoadedAsync"/> et
    ///   <see cref="OnResized"/>. Aucun override d'<c>OnUnloadedAsync</c>,
    ///   l'implémentation par défaut de <see cref="Page_Generic"/>
    ///   suffisant.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>.</description></item>
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
        /// typé en <see cref="object"/>. Son unique usage local au-delà
        /// de l'affectation du <c>DataContext</c> est l'invocation de
        /// <see cref="VM_Page11.LoadAsync"/> depuis
        /// <see cref="OnLoadedAsync"/>, au titre de l'ancrage canonique
        /// entre les deux socles génériques de la famille — seule
        /// invocation d'un membre du ViewModel admise depuis une vue,
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
        ///   <see cref="_viewModel"/> pour activer les dix-neuf bindings
        ///   déclarés par <c>Page11.xaml</c> : douze libellés
        ///   multilingues et sept caractéristiques de la
        ///   série.</description></item>
        /// </list>
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
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.ApplyLayout"/> pour appliquer la
        /// stylisation invariante des quatorze contrôles XAML nommés
        /// stylisables de la page : la <c>Grid</c> de page, le
        /// <c>TabControl</c> principal, les cinq onglets et leurs
        /// en-têtes, le <c>Border</c> de la fiche de synthèse, les sept
        /// intitulés et les quatre <c>TextBlock</c> de donnée.
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
        /// </list>
        ///
        /// <para>Absence délibérée de stylisation des trois cases à
        /// cocher : Les trois <c>CheckBox</c> de la colonne 1 de la fiche
        /// (<c>CuttingStartedCheckBox</c>, <c>CuttingCompletedCheckBox</c>,
        /// <c>BarOutOfStockCheckBox</c>) ne sont ni résolues ni stylisées.
        /// Le contrat <c>IS_ControlStyler</c> n'expose aucune méthode
        /// dédiée à ce type de contrôle ; les trois cases conservent le
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
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnLoadedAsync"/> pour amorcer le
        /// chargement asynchrone des sept caractéristiques de la fiche de
        /// synthèse par invocation de
        /// <see cref="VM_Page11.LoadAsync"/>.
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
        /// <c>TabControl</c> principal à la hauteur de fenêtre courante.
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
        /// <para>Constante unique : À la différence de <c>Page01</c> et
        /// <c>Page03</c> qui calculent en outre une hauteur de
        /// <c>ScrollViewer</c> dérivée, la présente page ne porte qu'une
        /// seule grandeur ajustée — l'onglet 1 ne comportant aucun
        /// <c>ScrollViewer</c>, sa fiche de synthèse tenant intégralement
        /// dans la hauteur disponible.</para>
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

            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl)
                mainTabControl.Height = tabControlHeight;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}