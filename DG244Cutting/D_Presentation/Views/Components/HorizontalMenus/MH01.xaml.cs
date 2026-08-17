using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.D_Presentation.Settings;
using DG244Cutting.D_Presentation.ViewModels.Components.HorizontalMenus;
using DG244Cutting.D_Presentation.Views.Generic;

namespace DG244Cutting.D_Presentation.Views.Components.HorizontalMenus
{
    /// <summary>
    /// Vue WPF du menu horizontal <c>MH01</c> de l'application
    /// DG244Cutting, associé à la
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page01"/>,
    /// exposant quatre boutons transverses standards bindés
    /// respectivement sur <c>ReduceCommand</c>, <c>HomeCommand</c>,
    /// <c>PreviousCommand</c> et <c>RefreshCommand</c> du socle
    /// <see cref="VM_MH_Generic"/>, augmentés d'un bouton de
    /// navigation contextuelle <c>MH_Admin</c> conduisant à la page
    /// d'administration des utilisateurs.
    /// </summary>
    /// <remarks>
    /// <para>Contexte :</para>
    ///
    /// <para>Menu horizontal de la page utilisateur (Page01), page
    /// d'accueil de l'opérateur authentifié. L'administration des
    /// comptes utilisateurs est portée par une page distincte
    /// (Page04), dont l'accès n'est ouvert qu'à une fraction des
    /// opérateurs. La présente vue porte le bouton qui ouvre ce
    /// point d'entrée.</para>
    ///
    /// <para>Objectif :</para>
    ///
    /// <para>Matérialiser dans le menu horizontal de la page
    /// utilisateur l'accès à la page d'administration des
    /// utilisateurs. Le bouton est stylisé au montage et sa
    /// visibilité est conditionnée à la conjonction du droit d'accès
    /// de l'utilisateur courant à la page cible et de son droit
    /// d'administration sur la page hôte.</para>
    ///
    /// <para>Responsabilités :</para>
    ///
    /// <list type="bullet">
    ///   <item><description>Servir de vue WPF — la composition XAML
    ///   est portée par <c>MH01.xaml</c> et se conforme au contrat
    ///   XAML attendu par <c>MH_Generic</c> (§4.15.9 du 0230) :
    ///   <see cref="System.Windows.Controls.Grid"/> nommé
    ///   <c>MH_Grid</c> contenant deux
    ///   <see cref="System.Windows.Controls.ColumnDefinition"/>
    ///   <c>MH_Grid_C1</c> et <c>MH_Grid_C2</c>, un
    ///   <see cref="System.Windows.Controls.Border"/> latéral
    ///   <c>MH_Border</c>, et les quatre boutons transverses
    ///   <c>MH_Menu</c>, <c>MH_Home</c>, <c>MH_Previous</c>,
    ///   <c>MH_Refresh</c> bindés respectivement sur
    ///   <see cref="VM_MH_Generic.ReduceCommand"/>,
    ///   <see cref="VM_MH_Generic.HomeCommand"/>,
    ///   <see cref="VM_MH_Generic.PreviousCommand"/> et
    ///   <see cref="VM_MH_Generic.RefreshCommand"/>. Le bouton
    ///   <c>MH_Menu</c> conserve son nommage XAML prescrit par le
    ///   contrat du socle <c>MH_Generic</c> mais est câblé sur
    ///   <see cref="VM_MH_Generic.ReduceCommand"/> et non sur
    ///   <see cref="VM_MH_Generic.MenuCommand"/>, car MH01 est
    ///   affiché lorsque le menu horizontal est en état déployé :
    ///   l'action accessible à l'opérateur est de le réduire, pas de
    ///   le déployer.</description></item>
    ///   <item><description>Exposer le bouton de navigation
    ///   contextuelle <c>MH_Admin</c>, bindé sur <c>AdminCommand</c>
    ///   du <see cref="VM_MH01"/> associé, avec son icône
    ///   <c>MH_Admin_Icon</c> et son libellé
    ///   <c>MH_Admin_Text</c>.</description></item>
    ///   <item><description>Styliser ce bouton au montage par
    ///   l'override propre d'<see cref="ApplyLayout"/>, en
    ///   complément de la stylisation des quatre boutons transverses
    ///   portée par le socle.</description></item>
    ///   <item><description>Conditionner la visibilité de ce bouton
    ///   au droit d'accès de l'utilisateur courant à la page cible,
    ///   par l'override propre
    ///   d'<see cref="ApplyNavigationRules"/>, en complément du
    ///   conditionnement de <c>MH_Home</c> et <c>MH_Previous</c>
    ///   porté par le socle.</description></item>
    ///   <item><description>Restreindre cette visibilité au droit
    ///   applicatif granulaire d'administration sur la page hôte,
    ///   par l'override propre
    ///   d'<see cref="ApplySecurityRules"/>, qui compose son
    ///   prédicat avec celui mémorisé par l'override
    ///   précédent.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    ///
    /// <list type="bullet">
    ///   <item><description>Ne décide ni ne déclenche aucune
    ///   navigation : la présente vue consulte
    ///   <see cref="IU_Navigation"/> en lecture seule, au travers des
    ///   seuls prédicats <c>CanNavigate</c> et <c>CanAdmin</c>.
    ///   L'invocation de l'opération de navigation relève
    ///   exclusivement de la commande portée par le
    ///   <see cref="VM_MH01"/> associé.</description></item>
    ///   <item><description>Ne porte aucune logique métier, aucune
    ///   règle de gestion, aucune transformation de données ni
    ///   aucune invocation de service applicatif. Le code-behind est
    ///   borné au câblage Vue/ViewModel et à la mécanique de
    ///   plateforme (I-4.12.1).</description></item>
    ///   <item><description>Ne charge aucun libellé multilingue : le
    ///   texte du bouton provient exclusivement du binding sur la
    ///   propriété observable du ViewModel (I-4.11.10).</description></item>
    ///   <item><description>N'override ni <c>OnResized</c>, ni
    ///   <c>OnLoadedAsync</c>, ni <c>OnUnloadedAsync</c> : le menu
    ///   n'a ni ajustement dimensionnel propre, ni chargement
    ///   asynchrone post-montage — <see cref="VM_MH01"/> n'override
    ///   pas <c>LoadAsync</c> —, ni ressource à
    ///   libérer.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales :</para>
    ///
    /// <para>La résolution du ViewModel par
    /// <c>App.ServiceProvider.GetRequiredService</c> au constructeur
    /// sans paramètre s'opère au titre de l'EA-06, étendue aux
    /// dérivés directs de <c>MH_Generic</c> pour cette seule
    /// finalité — le framework WPF instanciant les composants de
    /// navigation sans injection paramétrée possible. Les trois
    /// dépendances <see cref="IS_ControlStyler"/>, <c>ISE_Window</c>
    /// et <see cref="IU_Navigation"/> sont résolues par EA-06 au
    /// socle et exposées en champs <c>protected</c> ; le présent
    /// code-behind n'en résout aucune. La consultation des prédicats
    /// <see cref="IU_Navigation.CanNavigate"/> et
    /// <see cref="IU_Navigation.CanAdmin"/> dans les deux overrides
    /// de règles relève du périmètre de lecture seule assigné au
    /// rang Vue par l'EA-05, admis par R-4.12.19 du 0231 car il
    /// concerne l'état d'affichage d'une commande et non la décision
    /// de naviguer.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq
    /// régions standard (§4.4.2 du 0230) augmentée d'une extension
    /// §4.4.3 : <c>=== Méthodes protégées ===</c>, présente au titre
    /// de R-4.4.10 du 0231 car la classe expose des méthodes
    /// <c>protected</c> — trois overrides de points d'extension du
    /// socle —, et insérée entre la région Méthodes publiques et la
    /// région Méthodes privées. Les extensions
    /// <c>=== Propriétés publiques ===</c> et <c>=== Événements /
    /// Délégués / Indexeurs ===</c> ne sont pas présentes : aucune
    /// propriété publique propre ni aucun événement propre n'est
    /// exposé par le présent code-behind. Soit six régions au
    /// total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   champ de mémorisation
    ///   <c>_canNavigateToAdminPage</c> du prédicat de navigation
    ///   évalué par <see cref="ApplyNavigationRules"/> et consommé
    ///   par <see cref="ApplySecurityRules"/>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   champ <c>_viewModel</c>, instance Singleton du ViewModel
    ///   associé.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur <c>public</c> sans paramètre, en trois
    ///   instructions ordonnées — résolution du ViewModel,
    ///   <c>InitializeComponent()</c>, affectation du
    ///   <c>DataContext</c>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> :
    ///   overrides propres d'<see cref="ApplyLayout"/>,
    ///   d'<see cref="ApplyNavigationRules"/> et
    ///   d'<see cref="ApplySecurityRules"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   présente mais vide, marqueur <c>// A compléter</c>.</description></item>
    /// </list>
    /// </remarks>
    public partial class MH01 : MH_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Mémorise le résultat du prédicat de navigation
        /// <see cref="IU_Navigation.CanNavigate"/> évalué sur la page
        /// cible <c>Page04</c> par
        /// <see cref="ApplyNavigationRules"/>, en vue de sa
        /// consommation par <see cref="ApplySecurityRules"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Le helper
        /// <see cref="MH_Generic.SetButtonVisibility"/> est un
        /// affecteur simple — une seconde invocation sur le même
        /// bouton écrase la décision de la première. La visibilité de
        /// <c>MH_Admin</c> résultant de la conjonction de deux
        /// prédicats de natures distinctes, évalués par deux points
        /// d'extension distincts au titre de §4.13.4.2 du 0230, le
        /// présent champ porte le report du premier vers le
        /// second.</para>
        /// <para>Cycle de vie : Affecté à chaque invocation
        /// d'<see cref="ApplyNavigationRules"/> par les handlers du
        /// socle, soit à l'événement <c>Loaded</c> puis à chaque
        /// <c>SizeChanged</c>. Valeur initiale
        /// <see langword="false"/>, cohérente avec l'état
        /// <c>Visibility="Collapsed"</c> déclaré au XAML.</para>
        /// </remarks>
        private bool _canNavigateToAdminPage;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Instance Singleton du ViewModel associé à la présente
        /// vue, résolue au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> et
        /// affectée à
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>
        /// pour alimenter les bindings WPF déclarés par
        /// <c>MH01.xaml</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instance unique partagée à l'échelle de
        /// l'application, enregistrée en portée Singleton au titre du
        /// principe P4-bis (§4.10.10 du 0230), ses quatre dépendances
        /// de constructeur étant elles-mêmes Singleton. La résolution
        /// par <c>App.ServiceProvider</c> est imposée par le
        /// constructeur sans paramètre du composant, contrainte par
        /// le framework WPF de navigation, et s'opère au titre de
        /// l'EA-06 étendue aux dérivés directs de
        /// <c>MH_Generic</c>.</para>
        /// <para>Consommation : Le champ n'est lu que pour
        /// l'affectation du <c>DataContext</c> au constructeur.
        /// Aucune méthode du présent code-behind ne l'invoque : le
        /// menu n'a aucun chargement asynchrone post-montage à
        /// déclencher, l'override d'<c>OnLoadedAsync</c> étant
        /// absent.</para>
        /// </remarks>
        private readonly VM_MH01 _viewModel;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="MH01"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte :</para>
        ///
        /// <para>Constructeur <c>public</c> sans paramètre, contraint
        /// par le framework WPF de navigation qui instancie les
        /// composants par réflexion au sein des opérations
        /// d'<see cref="IU_Navigation"/> — aucune injection
        /// paramétrée n'est possible (R-4.12.23 du 0231).</para>
        ///
        /// <para>Séquence d'initialisation :</para>
        ///
        /// <list type="number">
        ///   <item><description>Résolution du ViewModel associé par
        ///   <c>App.ServiceProvider.GetRequiredService</c> au titre
        ///   de l'EA-06.</description></item>
        ///   <item><description><c>InitializeComponent()</c>,
        ///   construisant l'arbre XAML.</description></item>
        ///   <item><description>Affectation du <c>DataContext</c>,
        ///   activant les bindings des quatre commandes transverses,
        ///   de la commande de navigation contextuelle et des
        ///   libellés associés.</description></item>
        /// </list>
        ///
        /// <para>L'ordre <c>InitializeComponent</c> puis
        /// <c>DataContext</c> est impératif (§4.15.11 du 0230). Les
        /// points d'extension <see cref="ApplyLayout"/>,
        /// <see cref="ApplyNavigationRules"/> et
        /// <see cref="ApplySecurityRules"/> ne sont pas invoqués
        /// ici : ils le sont ultérieurement par les handlers de
        /// chargement du socle <see cref="MH_Generic"/>, à
        /// l'événement <c>Loaded</c>.</para>
        ///
        /// <para>Filet de sécurité :</para>
        ///
        /// <para>La résolution du ViewModel via
        /// <c>GetRequiredService</c> lève une exception si le service
        /// n'est pas enregistré, garantissant l'échec explicite en
        /// cas de mauvaise configuration du conteneur DI plutôt
        /// qu'une défaillance différée de binding.</para>
        /// </remarks>
        public MH01()
        {
            _viewModel = App.ServiceProvider.GetRequiredService<VM_MH01>();

            InitializeComponent();

            DataContext = _viewModel;
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Applique la stylisation invariante du menu horizontal :
        /// délègue au socle pour les quatre boutons transverses, puis
        /// stylise le bouton de navigation contextuelle
        /// <c>MH_Admin</c>.
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// de chargement du socle et propagée à
        /// <c>base.ApplyLayout</c>.</param>
        /// <remarks>
        /// <para>Contexte : Override du point d'extension synchrone
        /// <see cref="MH_Generic.ApplyLayout"/>, invoqué à
        /// l'événement <c>Loaded</c>. L'appel à
        /// <c>base.ApplyLayout(callChain)</c> est IMPÉRATIVEMENT la
        /// première instruction, afin de préserver la stylisation des
        /// quatre boutons transverses portée par le socle.</para>
        /// <para>Stylisation propre : Le bouton, son icône et son
        /// libellé sont résolus par le patron <c>Find&lt;T&gt;</c>
        /// sous garde <c>is</c> groupée, sans opérateur
        /// null-forgiving (R-4.15.25 du 0231), puis délégués à
        /// <see cref="IS_ControlStyler.StyleHorizontalMenuButton"/>
        /// avec l'icône <c>RS_Icons.MH_Admin_Source</c>. Le paramètre
        /// textuel optionnel de la méthode de stylisation n'est pas
        /// fourni : le contenu du libellé est alimenté par binding
        /// sur le ViewModel au titre de la mécanique
        /// multilingue.</para>
        /// <para>Résolution partielle : Si l'un des trois éléments
        /// XAML est absent, la garde court-circuite la stylisation du
        /// bouton sans lever ni journaliser. Une trace de diagnostic
        /// est émise par <c>Find&lt;T&gt;</c> pour chaque élément
        /// manquant, et le chargement du menu se poursuit sans
        /// interruption.</para>
        /// </remarks>
        protected override void ApplyLayout(string callChain)
        {
            base.ApplyLayout(callChain);

            if (Find<Button>("MH_Admin") is Button button
                && Find<Image>("MH_Admin_Icon") is Image icon
                && Find<TextBlock>("MH_Admin_Text") is TextBlock textBlock)
            {
                _controlStyler.StyleHorizontalMenuButton(
                    button, icon, textBlock, RS_Icons.MH_Admin_Source);
            }
        }

        /// <summary>
        /// Applique les règles de navigation du menu horizontal :
        /// délègue au socle pour les boutons transverses, puis
        /// conditionne la visibilité du bouton <c>MH_Admin</c> au
        /// droit d'accès de l'utilisateur courant à la page cible.
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// du socle et propagée à
        /// <c>base.ApplyNavigationRules</c>.</param>
        /// <remarks>
        /// <para>Contexte : Override du point d'extension synchrone
        /// <see cref="MH_Generic.ApplyNavigationRules"/>. L'appel à
        /// <c>base.ApplyNavigationRules(callChain)</c> est
        /// IMPÉRATIVEMENT la première instruction, l'implémentation
        /// par défaut du socle portant le conditionnement de
        /// <c>MH_Previous</c> sur
        /// <see cref="IU_Navigation.CanNavigateBack"/> et de
        /// <c>MH_Home</c> sur
        /// <see cref="IU_Navigation.CanNavigateToDefault"/>
        /// (R-4.12.19 du 0231). Son omission constituerait une
        /// régression directe sur ces deux boutons.</para>
        /// <para>Caractère impératif de l'override : La présente vue
        /// exposant un bouton de navigation contextuelle, l'override
        /// cesse d'être facultatif — il porte le conditionnement de
        /// visibilité sur <see cref="IU_Navigation.CanNavigate"/>
        /// exigé par R-4.13.14 du 0231, dont l'omission constituerait
        /// une non-conformité à I-4.13.14.</para>
        /// <para>Choix du point d'extension : Le conditionnement
        /// fondé sur <see cref="IU_Navigation.CanNavigate"/> est
        /// porté ici et non par <see cref="ApplySecurityRules"/>, la
        /// répartition étant gouvernée par la nature du prédicat
        /// consulté et non par celle du facteur (§4.13.4.2 du
        /// 0230) : <c>CanNavigate</c> est l'un des trois prédicats de
        /// navigation, à l'exclusion des prédicats de droits
        /// granulaires.</para>
        /// <para>Composition de la visibilité : Le résultat du
        /// prédicat est mémorisé dans
        /// <c>_canNavigateToAdminPage</c> avant d'être appliqué au
        /// bouton, afin d'être composé par conjonction avec le droit
        /// granulaire évalué par <see cref="ApplySecurityRules"/>. Le
        /// socle invoque les deux points d'extension dans cet ordre —
        /// <see cref="ApplyNavigationRules"/> puis
        /// <see cref="ApplySecurityRules"/> —, tant à l'événement
        /// <c>Loaded</c> qu'à chaque <c>SizeChanged</c> ; l'ordre
        /// d'affectation en dépend nominativement, le helper
        /// <see cref="MH_Generic.SetButtonVisibility"/> étant un
        /// affecteur simple dont la seconde invocation écrase la
        /// première.</para>
        /// <para>Périmètre de consommation : Seul le prédicat de
        /// lecture est consulté. Aucune opération d'écriture
        /// d'<see cref="IU_Navigation"/> n'est invoquée depuis le
        /// présent code-behind, conformément au périmètre de lecture
        /// seule assigné au rang Vue par l'EA-05 ; l'écriture relève
        /// exclusivement du rang ViewModel.</para>
        /// <para>Fondement ergonomique : Lorsque le droit d'accès
        /// fait défaut, le bouton demeure masqué et l'action est
        /// rendue inaccessible en amont, plutôt que d'échouer à
        /// l'exécution (§4.13.6.4 du 0230). Le nom de page consulté
        /// est strictement identique à celui invoqué par
        /// <c>AdminCommand</c> du <see cref="VM_MH01"/>
        /// associé.</para>
        /// </remarks>
        protected override void ApplyNavigationRules(string callChain)
        {
            base.ApplyNavigationRules(callChain);

            _canNavigateToAdminPage = _navigation.CanNavigate("Page04");

            SetButtonVisibility("MH_Admin", _canNavigateToAdminPage);
        }

        /// <summary>
        /// Applique les règles de sécurité du menu horizontal :
        /// restreint la visibilité du bouton <c>MH_Admin</c> au droit
        /// applicatif granulaire d'administration de l'utilisateur
        /// courant sur la page hôte.
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// du socle et propagée à
        /// <c>base.ApplySecurityRules</c>.</param>
        /// <remarks>
        /// <para>Contexte : Override du point d'extension synchrone
        /// <see cref="MH_Generic.ApplySecurityRules"/>. L'appel à
        /// <c>base.ApplySecurityRules(callChain)</c> est placé en
        /// première instruction ; l'implémentation par défaut du
        /// socle ne porte à ce jour aucun traitement, l'appel étant
        /// un geste de robustesse vis-à-vis de toute évolution
        /// future.</para>
        /// <para>Choix du point d'extension : Seul le prédicat de
        /// droit applicatif granulaire
        /// <see cref="IU_Navigation.CanAdmin"/> est évalué ici, la
        /// méthode porteuse de l'évaluation étant déterminée par la
        /// nature du prédicat consulté (§4.13.4.2 du 0230). Le
        /// prédicat de navigation n'est PAS réévalué : il est
        /// consommé depuis le champ de mémorisation
        /// <c>_canNavigateToAdminPage</c> affecté par
        /// <see cref="ApplyNavigationRules"/>.</para>
        /// <para>Dépendance à l'ordre d'invocation : La présente
        /// composition suppose que le socle invoque
        /// <see cref="ApplyNavigationRules"/> avant
        /// <see cref="ApplySecurityRules"/>. Cet ordre est celui
        /// qu'implémentent les deux handlers de <c>MH_Generic</c>,
        /// à l'événement <c>Loaded</c> comme à chaque
        /// <c>SizeChanged</c>, et celui que pose §4.13.4.2 du
        /// 0230.</para>
        /// <para>Visibilité résultante : Le bouton <c>MH_Admin</c>
        /// est visible si et seulement si l'utilisateur courant
        /// dispose à la fois du droit d'accès à <c>Page04</c> et du
        /// droit d'administration sur <c>Page01</c>.</para>
        /// <para>Résolution partielle : Si le bouton est
        /// introuvable, aucune action n'est effectuée ; une trace de
        /// diagnostic est émise par <c>Find&lt;T&gt;</c> depuis
        /// <see cref="MH_Generic.SetButtonVisibility"/>.</para>
        /// </remarks>
        protected override void ApplySecurityRules(string callChain)
        {
            base.ApplySecurityRules(callChain);

            SetButtonVisibility(
                "MH_Admin",
                _canNavigateToAdminPage && _navigation.CanAdmin("Page01"));
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}