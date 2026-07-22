using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.D_Presentation.ViewModels.Pages;
using DG244Cutting.D_Presentation.Views.Generic;

namespace DG244Cutting.D_Presentation.Views.Pages
{
    /// <summary>
    /// Vue WPF de la page Messagerie applicative <c>Page03</c> de
    /// l'application DG244Cutting, affichant dans un
    /// <see cref="TabControl"/> à trois onglets les deux boîtes de
    /// la messagerie applicative interne (Reçus, Envoyés) et une
    /// fenêtre de consultation détaillée du message sélectionné.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille Page de la couche
    /// <c>D_Presentation</c>, dérivé de
    /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/>.
    /// La page est instanciée par le framework WPF de navigation via
    /// <c>Activator.CreateInstance</c>
    /// (<c>SR_Navigation.NavigateToPage</c>), hors conteneur DI. Le
    /// constructeur sans paramètre est imposé par cette contrainte
    /// et résout son <c>DataContext</c> <see cref="VM_Page03"/> via
    /// <c>App.ServiceProvider.GetRequiredService</c> au titre de
    /// l'EA-02 Service Locator héritée de <c>Page_Generic</c>
    /// (§4.15.7 et §4.15.10 du 0230). La page est accessible à tout
    /// utilisateur connecté en lecture seule et n'expose aucune
    /// commande utilisateur de composition de message ; la sortie
    /// s'effectue exclusivement via les boutons transverses du menu
    /// horizontal portés par le couple
    /// <c>VM_MH_Generic</c> / <c>MH_Generic</c>.</para>
    ///
    /// <para>Objectif : Tenir le rôle structurant et minimaliste
    /// attendu d'un code-behind WPF dans le respect de la
    /// séparation MVVM stricte (I-4.12.1 du 0231) :</para>
    /// <list type="bullet">
    ///   <item><description>Résoudre le ViewModel
    ///   <see cref="VM_Page03"/> via le ServiceProvider au
    ///   constructeur et l'affecter au <c>DataContext</c> de la
    ///   vue.</description></item>
    ///   <item><description>Redéfinir le point d'extension
    ///   synchrone <see cref="Page_Generic.ApplyLayout"/> pour
    ///   appliquer la stylisation centralisée aux contrôles XAML
    ///   nominaux via <see cref="Page_Generic._controlStyler"/>
    ///   hérité, avec résolution typée par
    ///   <see cref="Page_Generic.Find{T}"/> et garde <c>is</c>
    ///   conditionnant chaque invocation
    ///   (R-4.15.25 du 0231).</description></item>
    ///   <item><description>Redéfinir le point d'extension
    ///   asynchrone <see cref="Page_Generic.OnLoadedAsync"/> pour
    ///   déléguer le chargement post-montage des deux collections
    ///   observables au hook canonique
    ///   <see cref="VM_Page03.LoadAsync"/>.</description></item>
    ///   <item><description>Redéfinir le point d'extension
    ///   synchrone <see cref="Page_Generic.OnResized"/> pour
    ///   ajuster dynamiquement la hauteur du
    ///   <see cref="TabControl"/> et des deux
    ///   <see cref="ScrollViewer"/> en fonction de la hauteur
    ///   courante de la fenêtre principale.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne charge aucun libellé : la mécanique
    ///   multilingue est portée par
    ///   <see cref="VM_Page03.LoadLabels"/> via les onze
    ///   propriétés observables exposées par le ViewModel.
    ///   Conformité I-4.11.10 du 0231.</description></item>
    ///   <item><description>Ne décide ni ne déclenche aucune
    ///   navigation, conformément à I-4.12.1 et R-4.12.2 du 0231.
    ///   Aucune commande de sortie de page ; les sorties transitent
    ///   par les commandes transverses du menu horizontal
    ///   <c>MH03</c>.</description></item>
    ///   <item><description>N'injecte ni ne résout aucun contrat
    ///   <c>IU_*</c> ni <c>IQ_*</c>, conformément à I-4.10.10 du
    ///   0231. Les invocations des Use Cases et Query Handlers
    ///   sont portées exclusivement par
    ///   <see cref="VM_Page03"/>.</description></item>
    ///   <item><description>N'expose aucun handler d'événement WPF
    ///   propre (aucun <c>SelectionChanged</c>, aucun
    ///   <c>Click</c>, etc.). Conformément à l'arbitrage Q9=A du
    ///   fil de fabrique du présent prompt : séparation MVVM
    ///   stricte, aucune logique applicative ni de coordination
    ///   d'UI dans le code-behind. La coordination des trois
    ///   propriétés de sélection et de la sélection automatique
    ///   de l'onglet Détail est portée par les setters des
    ///   propriétés observables de
    ///   <see cref="VM_Page03"/>.</description></item>
    ///   <item><description>Ne redéfinit pas
    ///   <see cref="Page_Generic.OnUnloadedAsync"/> : la page ne
    ///   tient aucune ressource asynchrone propre à libérer
    ///   (pas de polling, pas d'abonnement métier propre, pas de
    ///   <c>CancellationTokenSource</c> local). L'abonnement INPC
    ///   à <c>ISE_App.AppCultureCode</c> porté par
    ///   <c>VM_Generic</c> est libéré naturellement par la
    ///   portée Singleton à l'arrêt de l'application (P4-bis,
    ///   §4.10.10 du 0230).</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales héritées :</para>
    /// <list type="bullet">
    ///   <item><description>EA-02 — Service Locator : héritée de
    ///   <see cref="Page_Generic"/>, étendue nominativement à la
    ///   résolution du ViewModel <see cref="VM_Page03"/> dans le
    ///   constructeur via
    ///   <c>App.ServiceProvider.GetRequiredService</c>, conformément
    ///   à §4.15.7 et §4.15.10 du 0230.</description></item>
    /// </list>
    ///
    /// <para>Structure des régions : La classe applique la
    /// structure normative à six régions standard (§4.4.2 du
    /// 0230), à parité avec <c>Page01</c>, <c>Page98</c> et
    /// <c>Page99</c> :</para>
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   constantes <see cref="HeaderWidth"/> et
    ///   <see cref="DetailTitleWidth"/>, dimensions locales
    ///   consommées par <see cref="ApplyLayout"/>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   champ <c>_viewModel</c>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur sans paramètre.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   présente mais vide, marqueur
    ///   <c>// A compléter</c>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> :
    ///   overrides de <see cref="Page_Generic.ApplyLayout"/>,
    ///   <see cref="Page_Generic.OnLoadedAsync"/> et
    ///   <see cref="Page_Generic.OnResized"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   présente mais vide, marqueur
    ///   <c>// A compléter</c>.</description></item>
    /// </list>
    /// </remarks>
    public partial class Page03 : Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Largeur en pixels device-independent appliquée au
        /// <see cref="TextBlock"/> de header de chacun des trois
        /// <see cref="TabItem"/> du <see cref="TabControl"/>
        /// central par
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleTabItem"/>
        /// dans <see cref="ApplyLayout"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constante locale au code-behind,
        /// consommée à trois sites symétriques dans la cascade
        /// de stylisation d'<see cref="ApplyLayout"/> (Onglets
        /// Reçus, Envoyés et Détail). La valeur 150 reproduit
        /// strictement celle utilisée par <c>Page01</c> pour
        /// ses deux TabItems, garantissant la cohérence
        /// visuelle inter-page des onglets de l'application.</para>
        /// </remarks>
        private const double HeaderWidth = 200;

        /// <summary>
        /// Largeur en pixels device-independent appliquée à chacun
        /// des trois <see cref="TextBlock"/> de titre du sous-Grid
        /// de l'onglet Détail (<c>SubjectTitle</c>,
        /// <c>DateTitle</c>, <c>ContentTitle</c>) par
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleTextBlockTitle"/>
        /// dans <see cref="ApplyLayout"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constante locale au code-behind,
        /// consommée à trois sites symétriques dans la cascade
        /// de stylisation d'<see cref="ApplyLayout"/>. La valeur
        /// 200 est alignée sur la première
        /// <c>ColumnDefinition Width="200"</c> du <c>MessageGrid</c>
        /// de l'onglet Détail du XAML, garantissant la
        /// cohérence visuelle entre la largeur du contrôle stylé
        /// et la largeur de la colonne hôte.</para>
        /// </remarks>
        private const double DetailTitleWidth = 200;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// ViewModel associé à la vue, résolu via le ServiceProvider
        /// au constructeur et affecté au <c>DataContext</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton résolu via
        /// <c>App.ServiceProvider.GetRequiredService</c> au titre de
        /// l'EA-02 Service Locator héritée de
        /// <see cref="Page_Generic"/>. La résolution est nominative
        /// (<see cref="VM_Page03"/>) et limitée au présent
        /// dérivé.</para>
        /// <para>Visibilité : <c>private</c>. La propriété
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// affectée au constructeur est la voie canonique de
        /// consommation des propriétés observables et des
        /// collections du ViewModel par les bindings XAML. Le champ
        /// <see cref="_viewModel"/> est en outre consommé par
        /// l'override <see cref="OnLoadedAsync"/> qui invoque
        /// <see cref="VM_Page03.LoadAsync"/>.</para>
        /// </remarks>
        private readonly VM_Page03 _viewModel;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="Page03"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur sans paramètre imposé par
        /// le framework WPF de navigation (instanciation par
        /// <c>Activator.CreateInstance</c> au sein de
        /// <c>SR_Navigation.NavigateToPage</c>). La chaîne
        /// d'héritage remonte à <see cref="Page_Generic"/> dont le
        /// constructeur résout les deux dépendances transverses
        /// <see cref="Page_Generic._controlStyler"/> et
        /// <see cref="Page_Generic._window"/>, initialise le champ
        /// <c>_callee</c> et abonne les trois handlers privés aux
        /// événements WPF <c>Loaded</c>, <c>Unloaded</c> et
        /// <c>SizeChanged</c>.</para>
        ///
        /// <para>Séquence d'initialisation en trois temps stricte
        /// (§4.15.7 du 0230, parité <c>Page01</c>, <c>Page98</c>,
        /// <c>Page99</c>) :</para>
        /// <list type="number">
        ///   <item><description>Résolution du ViewModel
        ///   <see cref="VM_Page03"/> via
        ///   <c>App.ServiceProvider.GetRequiredService</c> au titre
        ///   de l'EA-02 Service Locator héritée. La méthode
        ///   <c>GetRequiredService</c> est utilisée (et non
        ///   <c>GetService</c>) conformément à la convention de
        ///   plateforme <c>App.ServiceProvider</c> : toute
        ///   défaillance de résolution traduit une erreur de
        ///   configuration du conteneur DI et doit faire échouer
        ///   l'instanciation immédiatement par exception
        ///   explicite.</description></item>
        ///   <item><description>Appel à
        ///   <c>InitializeComponent()</c> généré par le compilateur
        ///   WPF, qui charge l'arbre XAML et matérialise les
        ///   contrôles nommés exposés à
        ///   <see cref="Page_Generic.Find{T}"/>.</description></item>
        ///   <item><description>Affectation du <c>DataContext</c> à
        ///   <see cref="_viewModel"/>, autorisant la résolution
        ///   immédiate des bindings WPF par la couche de
        ///   présentation. L'ordre <c>_viewModel</c> →
        ///   <c>InitializeComponent</c> → <c>DataContext</c> est
        ///   imposé par §4.15.7 du 0230 : la résolution du
        ///   ViewModel doit précéder
        ///   <c>InitializeComponent</c> pour permettre l'affectation
        ///   immédiate du <c>DataContext</c> sans pause WPF
        ///   intermédiaire au cours du parsing XAML, et
        ///   l'affectation de <c>DataContext</c> doit suivre
        ///   <c>InitializeComponent</c> pour s'appliquer à l'arbre
        ///   XAML déjà matérialisé.</description></item>
        /// </list>
        ///
        /// <para>Filet de sécurité : Aucune invocation susceptible
        /// de lever une exception terminale n'est portée par le
        /// constructeur au-delà de la résolution
        /// <c>GetRequiredService</c> et de l'éventuelle exception
        /// XAML levée par <c>InitializeComponent</c>. Une
        /// défaillance de <c>GetRequiredService</c> traduit une
        /// erreur de configuration du conteneur DI et doit faire
        /// échouer l'instanciation immédiatement. Le filet de
        /// sécurité ultime au bord des handlers WPF est porté par
        /// <see cref="Page_Generic"/> et couvre les éventuelles
        /// défaillances survenant au chargement, au déchargement et
        /// au redimensionnement de la page.</para>
        /// </remarks>
        public Page03()
        {
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page03>();

            InitializeComponent();

            DataContext = _viewModel;
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension synchrone
        /// <see cref="Page_Generic.ApplyLayout(string)"/> pour
        /// appliquer la stylisation centralisée aux contrôles
        /// XAML nominaux via <see cref="Page_Generic._controlStyler"/>
        /// hérité.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <see cref="Page_Generic"/> à
        /// l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de
        /// la page, avant <see cref="OnResized(string)"/> et
        /// <see cref="OnLoadedAsync(string, System.Threading.CancellationToken)"/>.
        /// Conformément à §4.15.7 du 0230, l'invocation à ce
        /// moment garantit que les contrôles sont stylés avant tout
        /// ajustement dimensionnel et avant tout chargement de
        /// données.</para>
        ///
        /// <para>Objectif : Appliquer la stylisation centralisée
        /// par <see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler"/>
        /// aux contrôles XAML de <c>Page03</c> selon leur rôle dans
        /// la composition visuelle de la page :</para>
        /// <list type="bullet">
        ///   <item><description>Grid racine <c>PageGrid</c> et
        ///   <c>TabControl</c> central
        ///   <c>MainTabControl</c>.</description></item>
        ///   <item><description>Trois <see cref="TabItem"/> avec
        ///   leurs <see cref="TextBlock"/> de header (largeur
        ///   <c>150</c> à parité <c>Page01</c>) — onglets
        ///   « Messages reçus », « Messages envoyés » et
        ///   « Détail ».</description></item>
        ///   <item><description>Pour chacun des deux onglets
        ///   ListView (Reçus et Envoyés) : Border d'en-tête de
        ///   colonnes (<see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleBorderHeader"/>),
        ///   ScrollViewer variadique avec trois TextBlock d'en-tête
        ///   de colonnes (<see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleScrollViewer"/>),
        ///   et ListView (<see cref="DG244Cutting.A_Domain.Interfaces.Services.Presentation.IS_ControlStyler.StyleListView"/>).</description></item>
        ///   <item><description>Onglet Détail : Border conteneur
        ///   <c>MessageBorder</c>, trois TextBlock de titre
        ///   (<c>SubjectTitle</c>, <c>DateTitle</c>,
        ///   <c>ContentTitle</c>, largeur <c>200</c> à parité
        ///   colonne XAML) et trois TextBlock de données
        ///   (<c>SubjectData</c>, <c>DateData</c>,
        ///   <c>ContentData</c>).</description></item>
        /// </list>
        ///
        /// <para>Patron de surcharge normatif (§4.15.7 du 0230) :
        /// <c>base.ApplyLayout(callChain)</c> en première
        /// instruction (conservé en geste de robustesse vis-à-vis
        /// de toute évolution future du socle, bien que
        /// l'implémentation par défaut ne porte aucun traitement),
        /// suivi de la cascade d'invocations conditionnelles de
        /// <see cref="Page_Generic._controlStyler"/> hérité. La
        /// résolution typée par
        /// <see cref="Page_Generic.Find{T}(string)"/> hérité, avec
        /// garde <c>is</c> conditionnant chaque invocation, est la
        /// voie canonique imposée par R-4.15.25 du 0231 : aucune
        /// invocation de stylisation n'est portée sans résolution
        /// préalable typée et conditionnelle du contrôle XAML
        /// cible.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. La
        /// continuation gracieuse est assurée par la garde
        /// <c>is</c> par contrôle : en cas d'absence ou de cast
        /// invalide d'un contrôle XAML, la garde <c>is</c>
        /// n'engage pas l'invocation du service
        /// <c>IS_ControlStyler</c>, et la trace de diagnostic
        /// émise par
        /// <see cref="Page_Generic.Find{T}(string)"/> via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// assure la détectabilité en environnement de
        /// développement. Toute exception qui parviendrait
        /// néanmoins à être levée par <c>IS_ControlStyler</c> ou
        /// par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> serait
        /// capturée par le filet de sécurité ultime de
        /// <c>Page_Generic.OnLoadedHandler</c> (try/catch englobant
        /// le handler), qui trace l'exception via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// sans la propager au framework WPF, conformément à
        /// §4.15.7 du 0230.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page03 &gt; OnLoadedHandler &gt; ApplyLayout</c>.</param>
        protected override void ApplyLayout(string callChain)
        {
            base.ApplyLayout(callChain);

            if (Find<Grid>("PageGrid") is Grid pageGrid) _controlStyler.StylePage(pageGrid);
            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl) _controlStyler.StyleTabControl(mainTabControl);

            // Onglet Messages reçus
            if (Find<TabItem>("MessagesReceivedTabItem") is TabItem messagesReceivedTabItem
                && Find<TextBlock>("MessagesReceivedTabHeader") is TextBlock messagesReceivedTabHeader)
                _controlStyler.StyleTabItem(messagesReceivedTabItem, messagesReceivedTabHeader, HeaderWidth);
            if (Find<Border>("MessagesReceivedHeaderBorder") is Border messagesReceivedHeaderBorder) _controlStyler.StyleBorderHeader(messagesReceivedHeaderBorder);

            if (Find<ScrollViewer>("MessagesReceivedScrollViewer") is ScrollViewer messagesReceivedScrollViewer)
            {
                Border? headerBorderForReceived = Find<Border>("MessagesReceivedHeaderBorder");
                TextBlock? rH01 = Find<TextBlock>("MessagesReceivedHeader01");
                TextBlock? rH02 = Find<TextBlock>("MessagesReceivedHeader02");
                TextBlock? rH03 = Find<TextBlock>("MessagesReceivedHeader03");

                _controlStyler.StyleScrollViewer(
                    messagesReceivedScrollViewer,
                    null,
                    headerBorderForReceived,
                    rH01, rH02, rH03);
            }

            if (Find<ListView>("MessagesReceivedListView") is ListView messagesReceivedListView) _controlStyler.StyleListView(messagesReceivedListView);

            // Onglet Messages envoyés (symétrique)
            if (Find<TabItem>("MessagesSentTabItem") is TabItem messagesSentTabItem
                && Find<TextBlock>("MessagesSentTabHeader") is TextBlock messagesSentTabHeader)
                _controlStyler.StyleTabItem(messagesSentTabItem, messagesSentTabHeader, HeaderWidth);
            if (Find<Border>("MessagesSentHeaderBorder") is Border messagesSentHeaderBorder) _controlStyler.StyleBorderHeader(messagesSentHeaderBorder);

            if (Find<ScrollViewer>("MessagesSentScrollViewer") is ScrollViewer messagesSentScrollViewer)
            {
                Border? headerBorderForSent = Find<Border>("MessagesSentHeaderBorder");
                TextBlock? sH01 = Find<TextBlock>("MessagesSentHeader01");
                TextBlock? sH02 = Find<TextBlock>("MessagesSentHeader02");
                TextBlock? sH03 = Find<TextBlock>("MessagesSentHeader03");

                _controlStyler.StyleScrollViewer(
                    messagesSentScrollViewer,
                    null,
                    headerBorderForSent,
                    sH01, sH02, sH03);
            }

            if (Find<ListView>("MessagesSentListView") is ListView messagesSentListView) _controlStyler.StyleListView(messagesSentListView);

            // Onglet Détail
            if (Find<TabItem>("MessageTabItem") is TabItem messageTabItem
                && Find<TextBlock>("MessageTabHeader") is TextBlock messageTabHeader)
                _controlStyler.StyleTabItem(messageTabItem, messageTabHeader, HeaderWidth);
            if (Find<Border>("MessageBorder") is Border messageBorder) _controlStyler.StyleBorder(messageBorder);

            if (Find<TextBlock>("SubjectTitle") is TextBlock subjectTitle) _controlStyler.StyleTextBlockTitle(subjectTitle, DetailTitleWidth);
            if (Find<TextBlock>("DateTitle") is TextBlock dateTitle) _controlStyler.StyleTextBlockTitle(dateTitle, DetailTitleWidth);
            if (Find<TextBlock>("ContentTitle") is TextBlock contentTitle) _controlStyler.StyleTextBlockTitle(contentTitle, DetailTitleWidth);

            if (Find<TextBlock>("SubjectData") is TextBlock subjectData) _controlStyler.StyleTextBlockData(subjectData);
            if (Find<TextBlock>("DateData") is TextBlock dateData) _controlStyler.StyleTextBlockData(dateData);
            if (Find<TextBlock>("ContentData") is TextBlock contentData) _controlStyler.StyleTextBlockData(contentData);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnLoadedAsync(string, System.Threading.CancellationToken)"/>
        /// pour déclencher le chargement asynchrone des deux
        /// collections observables
        /// <see cref="VM_Page03.MessagesReceived"/> et
        /// <see cref="VM_Page03.MessagesSent"/> par invocation du
        /// hook <see cref="VM_Page03.LoadAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <see cref="Page_Generic"/> à
        /// l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de
        /// la page, postérieurement à l'application de la
        /// stylisation par <see cref="ApplyLayout(string)"/> et à
        /// l'ajustement dimensionnel initial par
        /// <see cref="OnResized(string)"/>. Le caractère
        /// asynchrone (<see cref="System.Threading.Tasks.Task"/>)
        /// est imposé par la signature du point d'extension de
        /// <c>Page_Generic</c> et permet la propagation
        /// coopérative de l'annulation au hook ViewModel.</para>
        ///
        /// <para>Objectif : Déclencher le chargement post-montage
        /// des deux collections observables par invocation directe
        /// du hook canonique <see cref="VM_Page03.LoadAsync"/>,
        /// déclaré <c>public virtual</c> au socle
        /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>
        /// conformément à §4.15.6 du 0230 et redéfini en
        /// <see cref="VM_Page03"/>.</para>
        ///
        /// <para>Patron de surcharge normatif (§4.15.7 du
        /// 0230) :</para>
        /// <list type="number">
        ///   <item><description><c>await base.OnLoadedAsync(callChain, ct)</c>
        ///   en première instruction, conservé en geste de
        ///   robustesse vis-à-vis de toute évolution future du
        ///   socle, bien que l'implémentation par défaut retourne
        ///   <see cref="System.Threading.Tasks.Task.CompletedTask"/>.</description></item>
        ///   <item><description><c>await _viewModel.LoadAsync(callChain, ct)</c>
        ///   en seconde instruction, propagation symétrique de la
        ///   CallChain et du jeton d'annulation au hook ViewModel
        ///   sans transformation intermédiaire.</description></item>
        /// </list>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. Les
        /// défaillances métier (<c>Ex_Business</c>) et
        /// infrastructure (<c>Ex_Infrastructure</c>) éventuellement
        /// levées par les invocations des Query Handlers en aval
        /// de <see cref="VM_Page03.LoadAsync"/> sont absorbées par
        /// le filet <c>VM_Generic.ExecuteSafeAsync</c> hérité par
        /// le ViewModel selon le pipeline canonique à cinq
        /// captures (§4.7.3 du 0230). Toute exception qui
        /// parviendrait néanmoins à être levée hors filet du
        /// ViewModel serait capturée par le filet de sécurité
        /// ultime de <c>Page_Generic.OnLoadedHandler</c>.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page03 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>,
        /// propagée telle quelle au hook ViewModel.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé
        /// par le handler appelant (valeur par défaut
        /// <c>default</c> côté <c>Page_Generic</c> qui ne
        /// construit pas de <c>CancellationTokenSource</c>
        /// propre), retransmis symétriquement à <c>base</c> et au
        /// hook <see cref="VM_Page03.LoadAsync"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone
        /// du chargement des deux collections
        /// observables.</returns>
        protected override async Task OnLoadedAsync(
            string callChain,
            CancellationToken ct = default)
        {
            await base.OnLoadedAsync(callChain, ct);

            await _viewModel.LoadAsync(callChain, ct);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnResized(string)"/> pour
        /// ajuster dynamiquement la hauteur du
        /// <see cref="TabControl"/> <c>MainTabControl</c> et des
        /// deux <see cref="ScrollViewer"/>
        /// <c>MessagesReceivedScrollViewer</c> et
        /// <c>MessagesSentScrollViewer</c> en fonction de la
        /// hauteur courante de la fenêtre principale.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par les handlers
        /// <c>OnLoadedHandler</c> (au montage initial,
        /// immédiatement après <see cref="ApplyLayout(string)"/>
        /// et avant
        /// <see cref="OnLoadedAsync(string, System.Threading.CancellationToken)"/>)
        /// et <c>OnSizeChangedHandler</c> (à chaque
        /// redimensionnement ultérieur de la fenêtre principale)
        /// de <see cref="Page_Generic"/>, conformément à §4.15.7
        /// du 0230. Le caractère synchrone est imposé par la
        /// signature du point d'extension de
        /// <c>Page_Generic</c> ; l'événement
        /// <see cref="System.Windows.FrameworkElement.SizeChanged"/>
        /// étant déclenché à haute fréquence pendant un
        /// redimensionnement utilisateur, le traitement doit
        /// rester rapide et purement local à la vue.</para>
        ///
        /// <para>Objectif : Ajuster la hauteur du
        /// <see cref="TabControl"/> <c>MainTabControl</c> à
        /// <c>MainWindowHeight - 220</c> et la hauteur des deux
        /// <see cref="ScrollViewer"/> à
        /// <c>tabControlHeight - 93</c>. Les constantes
        /// <c>220</c> et <c>93</c> reproduisent strictement les
        /// valeurs du legacy et représentent les marges réservées
        /// à l'environnement de la page (bandeau de fenêtre, menu
        /// horizontal, marges visuelles) et au bandeau d'en-têtes
        /// de colonnes des ListViews
        /// respectivement.</para>
        ///
        /// <para>Patron de surcharge normatif (§4.15.7 du 0230) :
        /// <c>base.OnResized(callChain)</c> en première
        /// instruction (conservé en geste de robustesse vis-à-vis
        /// de toute évolution future du socle, bien que
        /// l'implémentation par défaut ne porte aucun
        /// traitement), suivi du calcul dimensionnel local et de
        /// l'affectation conditionnelle des propriétés
        /// <see cref="System.Windows.FrameworkElement.Height"/>
        /// sur les trois contrôles concernés. La consommation de
        /// <c>_window.MainWindowHeight</c> (propriété <c>int</c>
        /// exposée par
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Settings.Presentation.ISE_Window"/>)
        /// via le champ <see cref="Page_Generic._window"/> hérité
        /// est la voie canonique d'accès à la dimension de la
        /// fenêtre principale documentée en §4.15.7 du 0230 ; la
        /// conversion implicite <c>int</c> → <c>double</c> à
        /// l'affectation des variables locales est licite C# et
        /// n'appelle aucun cast explicite.</para>
        ///
        /// <para>Résolution typée par le helper
        /// <see cref="Page_Generic.Find{T}(string)"/> : Chaque
        /// contrôle dont la hauteur est ajustée est résolu par le
        /// helper hérité, avec garde <c>is</c> conditionnant
        /// l'affectation, selon le patron normatif identique à
        /// celui d'<see cref="ApplyLayout"/>.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. La
        /// continuation gracieuse est assurée par la garde
        /// <c>is</c> par contrôle ; toute exception qui
        /// parviendrait à être levée serait capturée par le filet
        /// de sécurité ultime de
        /// <c>Page_Generic.OnLoadedHandler</c> ou
        /// <c>OnSizeChangedHandler</c>.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> (au montage initial)
        /// ou <c>OnSizeChangedHandler</c> (à chaque
        /// redimensionnement ultérieur) sous la forme
        /// <c>Page03 &gt; {handler} &gt; OnResized</c>.</param>
        protected override void OnResized(string callChain)
        {
            base.OnResized(callChain);

            double tabControlHeight = _window.MainWindowHeight - 220;
            double scrollViewerHeight = tabControlHeight - 93;

            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl)
                mainTabControl.Height = tabControlHeight;
            if (Find<ScrollViewer>("MessagesReceivedScrollViewer") is ScrollViewer messagesReceivedScrollViewer)
                messagesReceivedScrollViewer.Height = scrollViewerHeight;
            if (Find<ScrollViewer>("MessagesSentScrollViewer") is ScrollViewer messagesSentScrollViewer)
                messagesSentScrollViewer.Height = scrollViewerHeight;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}