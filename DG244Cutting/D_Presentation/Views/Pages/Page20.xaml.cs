using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.D_Presentation.ViewModels.Pages;
using DG244Cutting.D_Presentation.Views.Generic;

namespace DG244Cutting.D_Presentation.Views.Pages
{
    /// <summary>
    /// Vue WPF de la page de validation de barre <c>Page20</c> : présente
    /// sous son titre un <c>TabControl</c> à quatre onglets — fiche de la
    /// barre désignée et choix du motif de refus, saisie de deux zones
    /// défectueuses, plan de coupe de la barre, barres en rupture de stock
    /// de la série.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille Page de la couche
    /// <c>D_Presentation</c>, vue concrète associée au ViewModel
    /// <see cref="VM_Page20"/> par identifiant commun <c>20</c>. La page
    /// est le cœur du parcours de production : l'opérateur y prend la barre
    /// que l'application lui désigne, en constate l'état et décide de son
    /// sort par les boutons du menu horizontal <c>MH20</c>. La seule action
    /// portée par la page elle-même est la libération d'une barre en
    /// rupture, par la case à cocher de l'onglet « Ruptures », liée à une
    /// commande du ViewModel.</para>
    ///
    /// <para>Objectif : Assurer le câblage Vue/ViewModel et la mécanique
    /// de plateforme WPF de la page :</para>
    /// <list type="bullet">
    ///   <item><description>Résoudre <see cref="VM_Page20"/> au
    ///   constructeur et l'affecter au
    ///   <see cref="System.Windows.FrameworkElement.DataContext"/> pour
    ///   activer les bindings déclarés par
    ///   <c>Page20.xaml</c>.</description></item>
    ///   <item><description>Appliquer au montage la stylisation invariante
    ///   du titre, du <c>TabControl</c>, des onglets, des fiches, des
    ///   saisies et des deux tableaux (<see cref="ApplyLayout"/>).</description></item>
    ///   <item><description>Ajuster, au montage puis à chaque
    ///   redimensionnement, la hauteur du <c>TabControl</c> et des deux
    ///   <c>ScrollViewer</c> à la hauteur de la fenêtre principale
    ///   (<see cref="OnResized"/>).</description></item>
    ///   <item><description>Amorcer la séquence d'entrée du ViewModel au
    ///   montage de la page (<see cref="OnLoadedAsync"/>).</description></item>
    /// </list>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Câblage Vue/ViewModel au
    ///   constructeur.</description></item>
    ///   <item><description>Stylisation invariante et ajustement
    ///   dimensionnel, strictement séparés.</description></item>
    ///   <item><description>Invocation unique de
    ///   <see cref="VM_Page20.LoadAsync"/> au montage.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte aucune logique métier, aucune
    ///   transformation de données, aucun contrôle de saisie ni aucune
    ///   décision de navigation : ces responsabilités relèvent du ViewModel
    ///   et des UseCases qu'il invoque.</description></item>
    ///   <item><description>Ne charge aucun libellé multilingue : les
    ///   textes sont exposés par le ViewModel.</description></item>
    ///   <item><description>Ne s'abonne à aucun événement de contrôle : le
    ///   régime d'affichage, l'onglet sélectionné et la libération d'une
    ///   barre transitent par des bindings.</description></item>
    ///   <item><description>Ne stylise pas l'image de section ni les cases à
    ///   cocher : <c>IS_ControlStyler</c> n'expose aucune méthode pour ces
    ///   types de contrôle.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales : La page consomme
    /// EA-02 étendue, en résolvant son ViewModel par
    /// <c>App.ServiceProvider</c> au constructeur, et hérite de
    /// <see cref="Page_Generic"/> EA-02 (résolution de
    /// <c>IS_ControlStyler</c> et <c>ISE_Window</c>), EA-03 (handlers
    /// asynchrones à filet ultime) et EA-04 (classe de base
    /// concrète).</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2) complétée par une extension (§4.4.3) : au titre
    /// de R-4.4.10 du 0231 l'extension Méthodes protégées pour les
    /// overrides des points d'extension. Soit six régions au
    /// total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> : constantes
    ///   de largeur de stylisation et de réserves
    ///   dimensionnelles.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> : champ
    ///   <see cref="_viewModel"/> stockant l'instance Singleton de
    ///   <see cref="VM_Page20"/> résolue au constructeur via
    ///   <c>App.ServiceProvider.GetRequiredService</c>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   sans paramètre <c>public</c> imposé par le framework WPF de
    ///   navigation, résolvant <see cref="VM_Page20"/> et l'affectant
    ///   à <see cref="System.Windows.FrameworkElement.DataContext"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> :
    ///   overrides de <see cref="ApplyLayout"/>,
    ///   <see cref="OnLoadedAsync"/> et <see cref="OnResized"/>. Aucun
    ///   override de <c>OnUnloadedAsync</c>, la page ne détenant aucune
    ///   ressource à libérer.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : présente
    ///   mais vide, marqueur <c>// A compléter</c>.</description></item>
    /// </list>
    /// </remarks>
    public partial class Page20 : Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Largeur de stylisation des en-têtes d'onglets.
        /// </summary>
        private const double TabHeaderWidth = 150;

        /// <summary>
        /// Largeur de stylisation des intitulés des fiches et du sélecteur de
        /// motif de refus, égale à celle de la première colonne des fiches.
        /// </summary>
        private const double FormColumnWidth = 300;

        /// <summary>
        /// Réserve de hauteur soustraite à la hauteur de la fenêtre principale
        /// pour les bandeaux transverses et le menu horizontal.
        /// </summary>
        private const double WindowHeightReserve = 220;

        /// <summary>
        /// Hauteur de la ligne de titre de la page, déclarée dans
        /// <c>Page20.xaml</c>.
        /// </summary>
        private const double TitleRowHeight = 40;

        /// <summary>
        /// Réserve de hauteur soustraite à celle du <c>TabControl</c> pour le
        /// bandeau d'en-têtes de colonnes et les marges internes d'un onglet à
        /// tableau.
        /// </summary>
        private const double TableTabHeightReserve = 93;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Instance Singleton du ViewModel associé à la présente vue,
        /// résolue au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> et affectée à
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// pour alimenter les bindings WPF déclarés par
        /// <c>Page20.xaml</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Champ stocké en lecture seule pour exposer
        /// le type concret <see cref="VM_Page20"/> au code-behind,
        /// distinct du
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// typé en <see cref="object"/>. Outre l'affectation du
        /// <c>DataContext</c> dans le constructeur, son unique usage
        /// local est l'invocation de
        /// <see cref="VM_Page20.LoadAsync"/> depuis
        /// <see cref="OnLoadedAsync"/> ; la page n'invoque aucune autre
        /// méthode ni commande du ViewModel depuis le code-behind,
        /// conformément à la séparation MVVM stricte.</para>
        /// </remarks>
        private readonly VM_Page20 _viewModel;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="Page20"/>.
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
        ///   <item><description>Résolution de <see cref="VM_Page20"/>
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
        ///   déclarés par <c>Page20.xaml</c>.</description></item>
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
        public Page20()
        {
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page20>();

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
        /// stylisation initiale du <c>Grid</c> central, du
        /// <c>TextBlock</c> de titre de la page, du <c>TabControl</c> et
        /// du contenu de ses quatre onglets.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <c>Page_Generic</c> à l'événement
        /// <see cref="System.Windows.FrameworkElement.Loaded"/> de la
        /// page, préalablement à
        /// <see cref="OnResized"/> et <see cref="OnLoadedAsync"/>. Le
        /// caractère synchrone est imposé par la
        /// signature du point d'extension de <c>Page_Generic</c>
        /// (§4.15.7 du 0230). La <paramref name="callChain"/> reçue est
        /// construite par le handler <c>OnLoadedHandler</c> sous la
        /// forme <c>Page20 &gt; OnLoadedHandler &gt; ApplyLayout</c>,
        /// conformément au patron méthode publique de §4.5.1 du
        /// 0230.</para>
        /// <para>Objectif : Appliquer la stylisation visuelle des
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
        ///   <item><description><c>if (Find&lt;TextBlock&gt;("PageTitleMain") is TextBlock pageTitle) _controlStyler.StyleTextBlockPageTitle(pageTitle)</c>
        ///   applique la stylisation standard d'un titre de page
        ///   (police, taille, couleur, alignement) lorsque le
        ///   <c>TextBlock</c> nommé <c>PageTitleMain</c> est
        ///   effectivement résolu dans l'arbre XAML ; en cas d'absence
        ///   ou de cast invalide, la stylisation est silencieusement
        ///   ignorée et la trace de diagnostic émise par
        ///   <see cref="Page_Generic.Find{T}(string)"/> assure la
        ///   détectabilité en environnement de développement. Les
        ///   paramètres optionnels <c>text</c> et <c>width</c> de
        ///   <c>StyleTextBlockPageTitle</c> ne sont pas fournis : le
        ///   texte est alimenté par le binding sur
        ///   <see cref="VM_Page20.PageName"/> et la largeur conserve
        ///   la valeur par défaut du contrôle.</description></item>
        /// </list>
        /// <para>Extension au contenu des onglets : la stylisation
        /// s'applique ensuite, selon le même patron, au <c>TabControl</c>
        /// (<c>StyleTabControl</c>), aux quatre onglets et à leurs en-têtes
        /// (<c>StyleTabItem</c>), aux <c>Border</c> des fiches des onglets
        /// 1 et 2 et de l'image de section (<c>StyleBorder</c>), aux onze
        /// intitulés (<c>StyleTextBlockTitle</c>) et aux onze données
        /// (<c>StyleTextBlockData</c>) de la fiche de barre — numéro et
        /// désignation de la série, origine et emplacement d'origine de la
        /// barre, puis référence, désignation, couleur, catégorie, longueur,
        /// nombre de découpes et longueur du reste —, au sélecteur de
        /// motif (<c>StyleComboBox</c>), aux intitulés et aux quatre zones
        /// de saisie des défauts (<c>StyleTextBlockTitle</c>,
        /// <c>StyleTextBoxInput</c>), puis, pour chacun des deux tableaux,
        /// au <c>Border</c> d'en-têtes (<c>StyleBorderHeader</c>), au
        /// <c>ScrollViewer</c> associé à ses douze en-têtes
        /// (<c>StyleScrollViewer</c>) et à la <c>ListView</c>
        /// (<c>StyleListView</c>). Les contrôles d'un couple onglet /
        /// en-tête, et ceux d'un bloc <c>ScrollViewer</c>, sont résolus
        /// ensemble, l'invocation étant conditionnée à la résolution du
        /// contrôle principal. L'image de section et les cases à cocher ne
        /// sont pas stylisées, faute de méthode dédiée.</para>
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
        /// <see cref="Page_Generic.ApplyLayout"/> ne porte aucun
        /// traitement. L'appel est néanmoins conservé en geste de
        /// robustesse vis-à-vis de toute évolution future du socle,
        /// conformément à la convention d'override standard et au
        /// patron normatif présenté en §4.15.7 du 0230.</para>
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
        /// du 0230. Ce filet ultime n'intervient plus que comme
        /// rempart contre les défaillances inattendues du framework
        /// WPF, et non comme mécanisme de rattrapage des résolutions
        /// XAML manquantes — celles-ci étant intégralement absorbées
        /// en amont par la mécanique de garde <c>is</c>.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page20 &gt; OnLoadedHandler &gt; ApplyLayout</c>.</param>
        protected override void ApplyLayout(string callChain)
        {
            base.ApplyLayout(callChain);

            if (Find<Grid>("PageGrid") is Grid pageGrid) _controlStyler.StylePage(pageGrid);
            if (Find<TextBlock>("PageTitleMain") is TextBlock pageTitle) _controlStyler.StyleTextBlockPageTitle(pageTitle);

            // Contrôle d'onglets et quatre onglets — garde is composée sur le couple TabItem + TextBlock d'en-tête
            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl) _controlStyler.StyleTabControl(mainTabControl);
            if (Find<TabItem>("BarTabItem") is TabItem barTabItem
                && Find<TextBlock>("BarTabHeader") is TextBlock barTabHeader)
                _controlStyler.StyleTabItem(barTabItem, barTabHeader, TabHeaderWidth);
            if (Find<TabItem>("DefectsTabItem") is TabItem defectsTabItem
                && Find<TextBlock>("DefectsTabHeader") is TextBlock defectsTabHeader)
                _controlStyler.StyleTabItem(defectsTabItem, defectsTabHeader, TabHeaderWidth);
            if (Find<TabItem>("CutPlanTabItem") is TabItem cutPlanTabItem
                && Find<TextBlock>("CutPlanTabHeader") is TextBlock cutPlanTabHeader)
                _controlStyler.StyleTabItem(cutPlanTabItem, cutPlanTabHeader, TabHeaderWidth);
            if (Find<TabItem>("OutOfStockTabItem") is TabItem outOfStockTabItem
                && Find<TextBlock>("OutOfStockTabHeader") is TextBlock outOfStockTabHeader)
                _controlStyler.StyleTabItem(outOfStockTabItem, outOfStockTabHeader, TabHeaderWidth);

            // Onglet 1 — fiche de la barre
            if (Find<Border>("BarDetailsBorder") is Border barDetailsBorder) _controlStyler.StyleBorder(barDetailsBorder);
            if (Find<TextBlock>("SerialNumberTitle") is TextBlock serialNumberTitle) _controlStyler.StyleTextBlockTitle(serialNumberTitle, FormColumnWidth);
            if (Find<TextBlock>("SeriesDescriptionTitle") is TextBlock seriesDescriptionTitle) _controlStyler.StyleTextBlockTitle(seriesDescriptionTitle, FormColumnWidth);
            if (Find<TextBlock>("OriginTitle") is TextBlock originTitle) _controlStyler.StyleTextBlockTitle(originTitle, FormColumnWidth);
            if (Find<TextBlock>("SourceLocationTitle") is TextBlock sourceLocationTitle) _controlStyler.StyleTextBlockTitle(sourceLocationTitle, FormColumnWidth);
            if (Find<TextBlock>("ReferenceTitle") is TextBlock referenceTitle) _controlStyler.StyleTextBlockTitle(referenceTitle, FormColumnWidth);
            if (Find<TextBlock>("DesignationTitle") is TextBlock designationTitle) _controlStyler.StyleTextBlockTitle(designationTitle, FormColumnWidth);
            if (Find<TextBlock>("ColorTitle") is TextBlock colorTitle) _controlStyler.StyleTextBlockTitle(colorTitle, FormColumnWidth);
            if (Find<TextBlock>("CategoryTitle") is TextBlock categoryTitle) _controlStyler.StyleTextBlockTitle(categoryTitle, FormColumnWidth);
            if (Find<TextBlock>("BarLengthTitle") is TextBlock barLengthTitle) _controlStyler.StyleTextBlockTitle(barLengthTitle, FormColumnWidth);
            if (Find<TextBlock>("CutPieceCountTitle") is TextBlock cutPieceCountTitle) _controlStyler.StyleTextBlockTitle(cutPieceCountTitle, FormColumnWidth);
            if (Find<TextBlock>("ResidueLengthTitle") is TextBlock residueLengthTitle) _controlStyler.StyleTextBlockTitle(residueLengthTitle, FormColumnWidth);
            if (Find<TextBlock>("RejectionReasonTitle") is TextBlock rejectionReasonTitle) _controlStyler.StyleTextBlockTitle(rejectionReasonTitle, FormColumnWidth);
            if (Find<TextBlock>("SerialNumberData") is TextBlock serialNumberData) _controlStyler.StyleTextBlockData(serialNumberData);
            if (Find<TextBlock>("SeriesDescriptionData") is TextBlock seriesDescriptionData) _controlStyler.StyleTextBlockData(seriesDescriptionData);
            if (Find<TextBlock>("OriginData") is TextBlock originData) _controlStyler.StyleTextBlockData(originData);
            if (Find<TextBlock>("SourceLocationData") is TextBlock sourceLocationData) _controlStyler.StyleTextBlockData(sourceLocationData);
            if (Find<TextBlock>("ReferenceData") is TextBlock referenceData) _controlStyler.StyleTextBlockData(referenceData);
            if (Find<TextBlock>("DesignationData") is TextBlock designationData) _controlStyler.StyleTextBlockData(designationData);
            if (Find<TextBlock>("ColorData") is TextBlock colorData) _controlStyler.StyleTextBlockData(colorData);
            if (Find<TextBlock>("CategoryData") is TextBlock categoryData) _controlStyler.StyleTextBlockData(categoryData);
            if (Find<TextBlock>("BarLengthData") is TextBlock barLengthData) _controlStyler.StyleTextBlockData(barLengthData);
            if (Find<TextBlock>("CutPieceCountData") is TextBlock cutPieceCountData) _controlStyler.StyleTextBlockData(cutPieceCountData);
            if (Find<TextBlock>("ResidueLengthData") is TextBlock residueLengthData) _controlStyler.StyleTextBlockData(residueLengthData);
            if (Find<ComboBox>("RejectionReasonComboBox") is ComboBox rejectionReasonComboBox) _controlStyler.StyleComboBox(rejectionReasonComboBox, FormColumnWidth);
            if (Find<Border>("ProfilSectionBorder") is Border profilSectionBorder) _controlStyler.StyleBorder(profilSectionBorder);

            // Onglet 2 — saisie des zones défectueuses
            if (Find<Border>("DefectsFormBorder") is Border defectsFormBorder) _controlStyler.StyleBorder(defectsFormBorder);
            if (Find<TextBlock>("DefectStart1Title") is TextBlock defectStart1Title) _controlStyler.StyleTextBlockTitle(defectStart1Title, FormColumnWidth);
            if (Find<TextBlock>("DefectEnd1Title") is TextBlock defectEnd1Title) _controlStyler.StyleTextBlockTitle(defectEnd1Title, FormColumnWidth);
            if (Find<TextBlock>("DefectStart2Title") is TextBlock defectStart2Title) _controlStyler.StyleTextBlockTitle(defectStart2Title, FormColumnWidth);
            if (Find<TextBlock>("DefectEnd2Title") is TextBlock defectEnd2Title) _controlStyler.StyleTextBlockTitle(defectEnd2Title, FormColumnWidth);
            if (Find<TextBox>("DefectStart1Input") is TextBox defectStart1Input) _controlStyler.StyleTextBoxInput(defectStart1Input);
            if (Find<TextBox>("DefectEnd1Input") is TextBox defectEnd1Input) _controlStyler.StyleTextBoxInput(defectEnd1Input);
            if (Find<TextBox>("DefectStart2Input") is TextBox defectStart2Input) _controlStyler.StyleTextBoxInput(defectStart2Input);
            if (Find<TextBox>("DefectEnd2Input") is TextBox defectEnd2Input) _controlStyler.StyleTextBoxInput(defectEnd2Input);

            // Onglet 3 — tableau des découpes — Border d'en-têtes
            if (Find<Border>("CutPlanHeaderBorder") is Border cutPlanHeaderBorder) _controlStyler.StyleBorderHeader(cutPlanHeaderBorder);
            // Bloc StyleScrollViewer variadique : résolution typée des douze en-têtes en variables
            // locales optionnelles, invocation unique conditionnée à la résolution du ScrollViewer.
            if (Find<ScrollViewer>("CutPlanScrollViewer") is ScrollViewer cutPlanScrollViewer)
            {
                Border? cutPlanHeaderBorderForScrollViewer = Find<Border>("CutPlanHeaderBorder");
                TextBlock? c01 = Find<TextBlock>("CutPlanHeader01");
                TextBlock? c02 = Find<TextBlock>("CutPlanHeader02");
                TextBlock? c03 = Find<TextBlock>("CutPlanHeader03");
                TextBlock? c04 = Find<TextBlock>("CutPlanHeader04");
                TextBlock? c05 = Find<TextBlock>("CutPlanHeader05");
                TextBlock? c06 = Find<TextBlock>("CutPlanHeader06");
                TextBlock? c07 = Find<TextBlock>("CutPlanHeader07");
                TextBlock? c08 = Find<TextBlock>("CutPlanHeader08");
                TextBlock? c09 = Find<TextBlock>("CutPlanHeader09");
                TextBlock? c10 = Find<TextBlock>("CutPlanHeader10");
                TextBlock? c11 = Find<TextBlock>("CutPlanHeader11");
                TextBlock? c12 = Find<TextBlock>("CutPlanHeader12");
                _controlStyler.StyleScrollViewer(
                    cutPlanScrollViewer,
                    null,
                    cutPlanHeaderBorderForScrollViewer,
                    c01, c02, c03, c04, c05, c06,
                    c07, c08, c09, c10, c11, c12);
            }
            if (Find<ListView>("CutPlanListView") is ListView cutPlanListView) _controlStyler.StyleListView(cutPlanListView);

            // Onglet 4 — tableau des barres en rupture — Border d'en-têtes
            if (Find<Border>("OutOfStockHeaderBorder") is Border outOfStockHeaderBorder) _controlStyler.StyleBorderHeader(outOfStockHeaderBorder);
            // Bloc StyleScrollViewer variadique : résolution typée des douze en-têtes en variables
            // locales optionnelles, invocation unique conditionnée à la résolution du ScrollViewer.
            if (Find<ScrollViewer>("OutOfStockScrollViewer") is ScrollViewer outOfStockScrollViewer)
            {
                Border? outOfStockHeaderBorderForScrollViewer = Find<Border>("OutOfStockHeaderBorder");
                TextBlock? o01 = Find<TextBlock>("OutOfStockHeader01");
                TextBlock? o02 = Find<TextBlock>("OutOfStockHeader02");
                TextBlock? o03 = Find<TextBlock>("OutOfStockHeader03");
                TextBlock? o04 = Find<TextBlock>("OutOfStockHeader04");
                TextBlock? o05 = Find<TextBlock>("OutOfStockHeader05");
                TextBlock? o06 = Find<TextBlock>("OutOfStockHeader06");
                TextBlock? o07 = Find<TextBlock>("OutOfStockHeader07");
                TextBlock? o08 = Find<TextBlock>("OutOfStockHeader08");
                TextBlock? o09 = Find<TextBlock>("OutOfStockHeader09");
                TextBlock? o10 = Find<TextBlock>("OutOfStockHeader10");
                TextBlock? o11 = Find<TextBlock>("OutOfStockHeader11");
                TextBlock? o12 = Find<TextBlock>("OutOfStockHeader12");
                _controlStyler.StyleScrollViewer(
                    outOfStockScrollViewer,
                    null,
                    outOfStockHeaderBorderForScrollViewer,
                    o01, o02, o03, o04, o05, o06,
                    o07, o08, o09, o10, o11, o12);
            }
            if (Find<ListView>("OutOfStockListView") is ListView outOfStockListView) _controlStyler.StyleListView(outOfStockListView);
        }

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="Page_Generic.OnLoadedAsync"/> pour amorcer la séquence
        /// d'entrée de la page par invocation de
        /// <see cref="VM_Page20.LoadAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le handler privé
        /// <c>OnLoadedHandler</c> de <see cref="Page_Generic"/> en
        /// troisième et dernière position de la séquence de montage
        /// <c>ApplyLayout</c> → <c>OnResized</c> → <c>OnLoadedAsync</c>,
        /// une fois la stylisation invariante et l'ajustement dimensionnel
        /// appliqués. La page étant reconstruite à chaque navigation et à
        /// chaque rafraîchissement, la séquence d'entrée est rejouée à
        /// chaque affichage. La <paramref name="callChain"/> reçue est
        /// construite par le handler sous la forme
        /// <c>Page20 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>.</para>
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
        /// <see cref="VM_Page20.LoadAsync"/> (EA-01) ; le filet ultime de
        /// <c>Page_Generic.OnLoadedHandler</c> n'intervient qu'en rempart
        /// contre les défaillances inattendues du framework
        /// WPF.</para>
        /// </remarks>
        /// <param name="callChain">CallChain transmise par
        /// <c>Page_Generic.OnLoadedHandler</c> sous la forme
        /// <c>Page20 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>, propagée
        /// telle quelle au hook du ViewModel.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé tel
        /// quel au hook du ViewModel. Valeur par défaut :
        /// <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// séquence d'entrée déléguée au ViewModel.</returns>
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
        /// <c>TabControl</c> principal à la hauteur de fenêtre courante, et
        /// celles des <c>ScrollViewer</c> des onglets « Découpes » et
        /// « Ruptures » par dérivation de la précédente.
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
        /// <c>Page20 &gt; {handler} &gt; OnResized</c>.</para>
        ///
        /// <para>Objectif : Porter l'ajustement dimensionnel dynamique,
        /// strictement disjoint de la stylisation invariante
        /// d'<see cref="ApplyLayout"/>. Sans hauteur explicite, un
        /// <c>ScrollViewer</c> placé dans un <c>StackPanel</c> reçoit une
        /// hauteur non bornée et ne défile pas ; l'ajustement rend donc
        /// les deux tableaux parcourables quelle que soit leur longueur.
        /// Le calcul est idempotent : il ne dépend que de la hauteur
        /// courante de la fenêtre.</para>
        ///
        /// <para>Grandeurs ajustées : La hauteur du <c>TabControl</c> est
        /// la hauteur de fenêtre courante, lue sur
        /// <c>ISE_Window.MainWindowHeight</c> (champ
        /// <see cref="Page_Generic._window"/> hérité), diminuée de la
        /// réserve des bandeaux transverses (<see cref="WindowHeightReserve"/>)
        /// et de la ligne de titre de la page
        /// (<see cref="TitleRowHeight"/>). La hauteur des <c>ScrollViewer</c>
        /// <c>CutPlanScrollViewer</c> et <c>OutOfStockScrollViewer</c> en
        /// est dérivée par soustraction de la réserve d'en-têtes
        /// (<see cref="TableTabHeightReserve"/>) ; elle est calculée une
        /// fois et consommée deux fois, les deux onglets partageant la même
        /// géométrie verticale. Les onglets « Barre » et « Défauts » ne
        /// portent aucun <c>ScrollViewer</c>.</para>
        ///
        /// <para>Appel à <c>base.OnResized(callChain)</c> en première
        /// instruction : L'implémentation par défaut de
        /// <see cref="Page_Generic.OnResized"/> ne porte aucun traitement.
        /// L'appel est conservé en geste de robustesse vis-à-vis de toute
        /// évolution future du socle.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. En cas
        /// d'absence ou de cast invalide d'un contrôle, la garde
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
        /// <c>Page20 &gt; {handler} &gt; OnResized</c>.</param>
        protected override void OnResized(string callChain)
        {
            base.OnResized(callChain);

            double tabControlHeight = _window.MainWindowHeight - WindowHeightReserve - TitleRowHeight;
            double scrollViewerHeight = tabControlHeight - TableTabHeightReserve;

            if (Find<TabControl>("MainTabControl") is TabControl mainTabControl)
                mainTabControl.Height = tabControlHeight;
            if (Find<ScrollViewer>("CutPlanScrollViewer") is ScrollViewer cutPlanScrollViewer)
                cutPlanScrollViewer.Height = scrollViewerHeight;
            if (Find<ScrollViewer>("OutOfStockScrollViewer") is ScrollViewer outOfStockScrollViewer)
                outOfStockScrollViewer.Height = scrollViewerHeight;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}