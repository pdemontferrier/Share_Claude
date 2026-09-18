using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DG244Cutting.D_Presentation.Views.Generic
{
    /// <summary>
    /// Classe de base commune à tous les UserControls de menu horizontal
    /// de l'application DG244Cutting, socle de la famille MH de la
    /// couche <c>D_Presentation</c>.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Socle de présentation qui orchestre le cycle de
    /// vie WPF d'un menu horizontal (chargement, déchargement,
    /// redimensionnement) et le traduit en points d'extension
    /// asynchrones ou synchrones selon la nature du traitement attendu.
    /// Réside dans <c>D_Presentation/Views/Generic</c>. La classe est
    /// marquée <c>abstract</c> conformément à §4.15.9 du 0230 :
    /// l'instanciation directe est interdite (constructeur
    /// <c>protected</c>), seule la dérivation est attendue. Les menus
    /// horizontaux concrets de l'application, nommés selon le schéma
    /// <c>MH01</c> à <c>MH99</c>, héritent de cette classe.</para>
    /// <para>Objectif : Fournir un comportement uniforme à tous les
    /// menus horizontaux, en factorisant la stylisation des boutons
    /// transverses communs (Menu, Home, Previous, Refresh) et la
    /// stylisation dimensionnelle du Grid principal, en orchestrant
    /// l'application des règles de visibilité fondées sur les droits
    /// d'accès via <see cref="IU_Navigation"/>, et en garantissant
    /// qu'aucune exception non gérée ne remonte au
    /// <see cref="System.Windows.Application.DispatcherUnhandledException"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>S'abonner aux événements WPF
    ///   <see cref="FrameworkElement.Loaded"/>,
    ///   <see cref="FrameworkElement.Unloaded"/> et
    ///   <see cref="FrameworkElement.SizeChanged"/> au constructeur, et
    ///   orchestrer leur traduction en six points d'extension
    ///   <c>protected</c> exploitables par les dérivés.</description></item>
    ///   <item><description>Exposer les quatre points d'extension
    ///   synchrones <see cref="ApplyLayout"/>,
    ///   <see cref="ApplyNavigationRules"/>,
    ///   <see cref="ApplySecurityRules"/> et <see cref="OnResized"/>,
    ///   invoqués en cascade au chargement et — pour <see cref="OnResized"/>,
    ///   <see cref="ApplyNavigationRules"/> et <see cref="ApplySecurityRules"/>
    ///   — à chaque redimensionnement. La séparation sémantique entre
    ///   <see cref="ApplyLayout"/> (stylisation invariante des quatre
    ///   boutons transverses) et <see cref="OnResized"/> (stylisation
    ///   dimensionnelle du Grid, des ColumnDefinition et du Border)
    ///   garantit que la stylisation invariante n'est pas réappliquée
    ///   inutilement à chaque redimensionnement.</description></item>
    ///   <item><description>Exposer les deux points d'extension
    ///   asynchrones <see cref="OnLoadedAsync"/> et
    ///   <see cref="OnUnloadedAsync"/> pour les chargements de données
    ///   propres aux menus horizontaux (cas marginal mais prévu par
    ///   symétrie avec <c>Page_Generic</c>).</description></item>
    ///   <item><description>Construire et propager une CallChain
    ///   canonique aux points d'extension sous la forme
    ///   <c>{_callee} &gt; {handler} &gt; {extensionPoint}</c>,
    ///   conformément au patron méthode publique de §4.5.1.</description></item>
    ///   <item><description>Propager un
    ///   <see cref="System.Threading.CancellationToken"/> optionnel aux
    ///   points d'extension asynchrones, conformément à §4.15.9 du
    ///   0230.</description></item>
    ///   <item><description>Exposer les helpers protégés
    ///   <see cref="Find{T}"/> (résolution typée d'éléments XAML
    ///   nommés avec trace de diagnostic) et
    ///   <see cref="SetButtonVisibility"/> (factorisation du patron
    ///   <c>if (Find&lt;Button&gt;(name) is Button btn) btn.Visibility = ...</c>
    ///   récurrent dans <see cref="ApplyNavigationRules"/> et
    ///   <see cref="ApplySecurityRules"/>).</description></item>
    ///   <item><description>Garantir, par un filet de sécurité ultime
    ///   au bord des trois handlers privés, qu'aucune exception ne
    ///   remonte au framework WPF.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne décide ni ne déclenche aucune
    ///   navigation : la décision de navigation appartient
    ///   exclusivement à <c>UC_Navigation</c>, conformément à
    ///   R-4.12.2. Le code-behind du menu horizontal consulte
    ///   <see cref="IU_Navigation.CanNavigate"/> pour déterminer la
    ///   visibilité des boutons, usage acceptable au titre de R-4.12.19
    ///   car il concerne l'état d'affichage d'une commande, non la
    ///   décision de naviguer.</description></item>
    ///   <item><description>Ne charge aucun libellé : la traduction
    ///   relève des ViewModels via leurs bindings, conformément à §4.11
    ///   et à I-4.11.10.</description></item>
    ///   <item><description>Ne désabonne pas les handlers
    ///   <see cref="FrameworkElement.Loaded"/>,
    ///   <see cref="FrameworkElement.Unloaded"/> et
    ///   <see cref="FrameworkElement.SizeChanged"/> : choix délibéré,
    ///   identique à <c>Page_Generic</c>. Le framework de navigation
    ///   WPF instancie un nouveau menu horizontal à chaque navigation
    ///   et ne réutilise jamais une instance antérieure ; le garbage
    ///   collector libère naturellement le menu au démontage.</description></item>
    /// </list>
    /// <para>Exception architecturale documentée — Service Locator
    /// (EA-06) :</para>
    /// <para>Cette classe résout ses dépendances via
    /// <c>App.ServiceProvider.GetRequiredService</c>, pattern
    /// habituellement proscrit (Service Locator). Cette exception est
    /// délibérée et limitée à la couche vue : un
    /// <see cref="UserControl"/> WPF chargé dynamiquement par le
    /// framework de navigation (via <c>ContentControl.Content</c>) ne
    /// peut pas recevoir d'injection par constructeur paramétré.
    /// L'usage du Service Locator est strictement cantonné aux
    /// dérivés directs de <see cref="MH_Generic"/> et ne doit
    /// jamais se propager aux ViewModels, UseCases, Services ni
    /// Repositories. Cf. §4.15.9 et §4.15.10 du 0230 pour la
    /// formalisation complète de cette exception.</para>
    /// <para>Contrat XAML attendu des dérivés : Les dérivés concrets
    /// doivent exposer dans leur XAML un ensemble normalisé
    /// d'éléments nommés, recherchés par le code de la présente
    /// classe via <see cref="Find{T}"/>. Les noms attendus sont :
    /// <c>MH_Grid</c> (élément <see cref="Grid"/> conteneur),
    /// <c>MH_Grid_C1</c> et <c>MH_Grid_C2</c> (deux
    /// <see cref="ColumnDefinition"/>), <c>MH_Border</c>
    /// (<see cref="Border"/> latéral), <c>MH_Menu</c>,
    /// <c>MH_Home</c>, <c>MH_Previous</c>, <c>MH_Refresh</c> (quatre
    /// <see cref="Button"/> transverses). Tout élément manquant est
    /// signalé par une trace de diagnostic via
    /// <see cref="Find{T}"/>, sans interruption du chargement
    /// (filet de sécurité contre les ruptures silencieuses de
    /// contrat XAML, conformément à §4.15.9 du 0230).</para>
    /// <para>Changements comportementaux introduits par la refonte :</para>
    /// <list type="number">
    ///   <item><description>Renommage de la classe de
    ///   <c>MH_Page_Generic</c> en <c>MH_Generic</c>. Le mot
    ///   <c>Page</c> est retiré du nom : un menu horizontal n'est pas
    ///   un composant d'une page WPF, c'est un UserControl transverse
    ///   côté Vue. Le mot <c>Page</c> retrouve dans le nommage du
    ///   projet sa portée stricte de composant WPF
    ///   <see cref="System.Windows.Controls.Page"/>. Le renommage
    ///   restaure la parité nominale avec le ViewModel typé
    ///   <c>VM_MH_Generic</c>, par symétrie avec le couple
    ///   <c>(Page_Generic, VM_Page_Generic)</c>.</description></item>
    ///   <item><description>Renommages des deux points d'extension
    ///   asynchrones : <c>OnMenuLoadedAsync</c> est renommé en
    ///   <see cref="OnLoadedAsync"/>, et <c>OnMenuUnloadedAsync</c>
    ///   en <see cref="OnUnloadedAsync"/>. Le mot <c>Menu</c>
    ///   disparaît des noms de méthodes, par symétrie nominale avec
    ///   <c>Page_Generic</c> : le nom de la classe (<c>MH_Generic</c>)
    ///   suffit à porter la spécialisation. Ces renommages emportent
    ///   les mises à jour correspondantes des CallChains construites
    ///   dans les handlers privés.</description></item>
    ///   <item><description>Introduction du nouveau point d'extension
    ///   <see cref="OnResized"/>, synchrone, dont l'implémentation par
    ///   défaut est non vide et porte la stylisation dimensionnelle
    ///   du Grid principal, des deux <see cref="ColumnDefinition"/>
    ///   et du <see cref="Border"/> latéral. Sa signature est
    ///   strictement identique à celle de <c>Page_Generic.OnResized</c>.
    ///   À compter de la présente refonte, <see cref="OnResized"/>
    ///   est invoqué une première fois par
    ///   <see cref="OnLoadedHandler"/>, immédiatement après
    ///   <see cref="ApplyLayout"/>, puis à chaque redimensionnement
    ///   ultérieur par <see cref="OnSizeChangedHandler"/>. Les
    ///   dérivés qui surchargent <see cref="OnResized"/> doivent
    ///   donc savoir que leur override sera invoqué au montage initial
    ///   puis à chaque redimensionnement.</description></item>
    ///   <item><description>Séparation sémantique d'<see cref="ApplyLayout"/> :
    ///   l'implémentation par défaut de <see cref="ApplyLayout"/>
    ///   portait jusqu'à la présente refonte deux catégories de
    ///   traitement (stylisation invariante des quatre boutons
    ///   transverses et stylisation dimensionnelle du Grid, des
    ///   ColumnDefinition et du Border). À compter de la présente
    ///   refonte, <see cref="ApplyLayout"/> ne porte plus que la
    ///   stylisation invariante des boutons transverses ; la
    ///   stylisation dimensionnelle a migré dans
    ///   <see cref="OnResized"/>. En conséquence,
    ///   <see cref="ApplyLayout"/> est retiré de l'enchaînement de
    ///   <see cref="OnSizeChangedHandler"/> : la stylisation
    ///   invariante n'est jamais réappliquée au redimensionnement.
    ///   Implication breaking pour les dérivés concrets actuels qui
    ///   surchargeaient <see cref="ApplyLayout"/> en y mêlant des
    ///   ajustements dimensionnels : ces dérivés devront redistribuer
    ///   leur override entre <see cref="ApplyLayout"/> (invariant) et
    ///   <see cref="OnResized"/> (dimensionnel), dans des fils
    ///   ultérieurs dédiés par dérivé concret.</description></item>
    /// </list>
    /// <para>Statut canonique : La présente classe constitue le
    /// cinquième et dernier exemple canonique d'une séquence de cinq
    /// fils de refactoring du socle de présentation. Elle est
    /// destinée à servir de matière première au futur 0232 de la
    /// famille UserControls de menu horizontal, dont la rédaction
    /// relève d'un fil de maintenance documentaire ultérieur.</para>
    /// <para>Structure des régions : La classe applique la structure
    /// normative à six régions, conformément à §4.4.2 (cinq régions
    /// standard) et à §4.4.3 (extension Méthodes protégées
    /// obligatoirement présente au titre de R-4.4.10, car la classe
    /// expose au moins une méthode <c>protected</c>). Les deux autres
    /// extensions de §4.4.3 (Propriétés publiques, Événements /
    /// Délégués / Indexeurs) ne s'appliquent pas. La région Méthodes
    /// publiques est obligatoirement présente mais vide avec le
    /// marqueur <c>// A compléter</c>.</para>
    /// </remarks>
    public abstract class MH_Generic : UserControl
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom dynamique de la classe courante, utilisé dans la
        /// construction des CallChains transmises aux points d'extension
        /// et dans les traces de diagnostic émises au bord des handlers
        /// WPF par le filet de sécurité ultime.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Initialisé via <c>GetType().Name</c> dans le
        /// constructeur, après l'affectation des dépendances injectées,
        /// conformément à l'ordre canonique d'initialisation prescrit
        /// par R-4.4.7. La valeur reflète le type concret du dérivé
        /// (typiquement <c>MH01</c> à <c>MH99</c>).</para>
        /// </remarks>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service de stylisation centralisée des contrôles WPF, exposé
        /// en <c>protected</c> aux dérivés pour leurs surcharges de
        /// <see cref="ApplyLayout"/> et de <see cref="OnResized"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton résolu au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> au titre de
        /// l'EA-06 Service Locator. La visibilité <c>protected</c> est
        /// justifiée par un usage légitime documenté : les dérivés
        /// peuvent étendre la stylisation invariante par défaut
        /// appliquée par <see cref="ApplyLayout"/> et la stylisation
        /// dimensionnelle par défaut appliquée par
        /// <see cref="OnResized"/>.</para>
        /// </remarks>
        protected readonly IS_ControlStyler _controlStyler;

        /// <summary>
        /// Setting Singleton d'accès à la fenêtre principale de
        /// l'application, exposé en <c>protected</c> aux dérivés pour
        /// les ajustements dynamiques liés aux dimensions de la fenêtre.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton résolu au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> au titre de
        /// l'EA-06. La visibilité <c>protected</c> est justifiée par un
        /// usage légitime documenté : les dérivés consomment
        /// <c>_window.MainWindowWidth</c> dans
        /// <see cref="OnResized"/> pour ajuster les dimensions des
        /// colonnes du menu horizontal en réaction au redimensionnement
        /// de la fenêtre principale.</para>
        /// </remarks>
        protected readonly ISE_Window _window;

        /// <summary>
        /// UseCase de navigation, exposé en <c>protected</c> aux
        /// dérivés pour la consultation des droits d'accès aux pages
        /// (<see cref="IU_Navigation.CanNavigate"/>) dans
        /// <see cref="ApplyNavigationRules"/> et
        /// <see cref="ApplySecurityRules"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton résolu au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> au titre de
        /// l'EA-06. La visibilité <c>protected</c> est justifiée par un
        /// usage légitime documenté : les dérivés interrogent
        /// directement <see cref="IU_Navigation"/> dans
        /// <see cref="ApplyNavigationRules"/> pour conditionner la
        /// visibilité des boutons transverses aux droits d'accès de
        /// l'utilisateur courant, conformément à R-4.12.19.</para>
        /// <para>Usage non décisionnel : Cette dépendance est consommée
        /// pour évaluer la visibilité d'un bouton, non pour déclencher
        /// une navigation. La décision de navigation reste exclusivement
        /// portée par <c>UC_Navigation</c>, conformément à R-4.12.2.</para>
        /// </remarks>
        protected readonly IU_Navigation _navigation;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de
        /// <see cref="MH_Generic"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur sans paramètre, modificateur
        /// <c>protected</c> conformément à §4.15.9 du 0230 : la
        /// classe est <c>abstract</c> et n'est jamais instanciée
        /// directement. Le constructeur n'est invoqué que par les
        /// constructeurs des menus horizontaux dérivés via
        /// <c>base()</c>. Les dépendances sont résolues via le
        /// ServiceProvider statique de <c>App</c> conformément à
        /// l'EA-06 Service Locator.</para>
        /// <para>Séquence d'initialisation (R-4.4.7) :</para>
        /// <list type="number">
        ///   <item><description>Résolution et affectation des trois
        ///   dépendances <see cref="_controlStyler"/>,
        ///   <see cref="_window"/> et <see cref="_navigation"/>.</description></item>
        ///   <item><description>Initialisation du champ
        ///   <see cref="_callee"/> via <c>GetType().Name</c>.</description></item>
        ///   <item><description>Abonnement aux trois événements WPF
        ///   <see cref="FrameworkElement.Loaded"/>,
        ///   <see cref="FrameworkElement.Unloaded"/> et
        ///   <see cref="FrameworkElement.SizeChanged"/>.</description></item>
        /// </list>
        /// </remarks>
        protected MH_Generic()
        {
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _window = App.ServiceProvider.GetRequiredService<ISE_Window>();
            _navigation = App.ServiceProvider.GetRequiredService<IU_Navigation>();

            _callee = GetType().Name;

            Loaded += OnLoadedHandler;
            Unloaded += OnUnloadedHandler;
            SizeChanged += OnSizeChangedHandler;
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Point d'extension synchrone invoqué pour appliquer la
        /// stylisation invariante du menu horizontal lors du chargement.
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// invoquant, sous la forme
        /// <c>{_callee} &gt; {handler} &gt; ApplyLayout</c>.</param>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="OnLoadedHandler"/> à l'événement
        /// <see cref="FrameworkElement.Loaded"/>. L'implémentation par
        /// défaut applique la stylisation invariante des quatre boutons
        /// transverses communs (Menu, Home, Previous, Refresh) via
        /// <see cref="IS_ControlStyler"/> et les icônes du référentiel
        /// statique <c>RS_Icons</c>. Aucune dépendance aux dimensions
        /// de la fenêtre principale n'est portée par cette méthode :
        /// la stylisation dimensionnelle relève exclusivement de
        /// <see cref="OnResized"/>.</para>
        /// <para>Séparation sémantique avec <see cref="OnResized"/> :
        /// <see cref="ApplyLayout"/> porte la stylisation invariante,
        /// appliquée une seule fois au montage initial et jamais
        /// réinvoquée au redimensionnement ; <see cref="OnResized"/>
        /// porte la stylisation dimensionnelle, appliquée au montage
        /// initial et à chaque redimensionnement de la fenêtre
        /// principale. Cette séparation préserve la performance lors
        /// des redimensionnements à haute fréquence en évitant de
        /// réappliquer inutilement la stylisation des boutons.</para>
        /// <para>Implémentation par défaut : Stylise les quatre boutons
        /// transverses standards via
        /// <see cref="IS_ControlStyler.StyleHorizontalMenuButton"/>.
        /// Un override doit appeler <c>base.ApplyLayout(callChain)</c>
        /// avant toute stylisation invariante propre au dérivé.</para>
        /// </remarks>
        protected virtual void ApplyLayout(string callChain)
        {
            StyleCommonButtons();
        }

        /// <summary>
        /// Point d'extension synchrone invoqué pour appliquer les
        /// règles de visibilité des boutons transverses fondées sur les
        /// droits de navigation de l'utilisateur courant.
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// invoquant, sous la forme
        /// <c>{_callee} &gt; {handler} &gt; ApplyNavigationRules</c>.</param>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="OnLoadedHandler"/> et par
        /// <see cref="OnSizeChangedHandler"/>, après
        /// <see cref="OnResized"/>. Conformément à §3.11 du 0230,
        /// la visibilité des boutons transverses est évaluée au
        /// chargement (présent point d'extension), distincte de
        /// l'activation dynamique évaluée par le <c>CanExecute</c>
        /// des commandes côté ViewModel.</para>
        /// <para>Implémentation par défaut : Conditionne la
        /// visibilité de deux boutons transverses : le bouton
        /// <c>MH_Previous</c> au prédicat
        /// <see cref="IU_Navigation.CanNavigateBack"/> (si
        /// l'historique de navigation est vide, le bouton est
        /// masqué) et le bouton <c>MH_Home</c> au prédicat
        /// <see cref="IU_Navigation.CanNavigateToDefault"/> (si
        /// l'utilisateur courant n'a pas le droit d'accès à la
        /// page d'accueil de l'application, le bouton est masqué).
        /// Le prédicat <see cref="IU_Navigation.CanNavigateToDefault"/>
        /// est défini nominativement à cet usage par
        /// <see cref="IU_Navigation"/> : il encapsule la lecture
        /// de la page par défaut depuis
        /// <c>ISE_Navigation.DefaultPageName</c> et l'évaluation
        /// du droit <c>CanAccess</c> sur cette page, sans codage
        /// en dur du nom logique de la page d'accueil dans le
        /// présent code-behind. Cette généralisation au socle
        /// résulte du principe que l'accès à la page d'accueil
        /// doit être contrôlé uniformément dans tous les menus
        /// horizontaux de l'application, plutôt que dupliqué par
        /// surcharge dans chaque dérivé concret (cf. §4.15.9 du
        /// 0230). Les boutons <c>MH_Menu</c> et <c>MH_Refresh</c>
        /// restent visibles inconditionnellement par défaut ; les
        /// dérivés peuvent surcharger cette logique selon leur
        /// contexte. Un override doit appeler
        /// <c>base.ApplyNavigationRules(callChain)</c> avant
        /// toute règle propre au dérivé.</para>
        /// <para>Conformité R-4.12.19 : La consultation directe
        /// de <see cref="IU_Navigation.CanNavigateBack"/>, de
        /// <see cref="IU_Navigation.CanNavigateToDefault"/> et de
        /// <see cref="IU_Navigation.CanNavigate"/> par le
        /// code-behind du menu horizontal est acceptable, car
        /// elle concerne l'état d'affichage des commandes, non la
        /// décision de naviguer.</para>
        /// </remarks>
        protected virtual void ApplyNavigationRules(string callChain)
        {
            SetButtonVisibility("MH_Previous", _navigation.CanNavigateBack);
            SetButtonVisibility("MH_Home", _navigation.CanNavigateToDefault());
        }

        /// <summary>
        /// Point d'extension synchrone invoqué pour appliquer les
        /// règles de visibilité des boutons transverses fondées sur les
        /// droits applicatifs spécifiques (au-delà de la navigation).
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// invoquant, sous la forme
        /// <c>{_callee} &gt; {handler} &gt; ApplySecurityRules</c>.</param>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="OnLoadedHandler"/> et par
        /// <see cref="OnSizeChangedHandler"/>, après
        /// <see cref="ApplyNavigationRules"/>. Permet aux dérivés de
        /// conditionner la visibilité de boutons supplémentaires (par
        /// exemple, un bouton de paramétrage réservé aux administrateurs)
        /// aux droits applicatifs de l'utilisateur courant.</para>
        /// <para>Implémentation par défaut : Aucun traitement. Les
        /// menus horizontaux qui n'exposent que les quatre boutons
        /// transverses standards n'ont pas besoin de redéfinir cette
        /// méthode.</para>
        /// </remarks>
        protected virtual void ApplySecurityRules(string callChain)
        {
            // Aucun traitement par défaut.
        }

        /// <summary>
        /// Point d'extension asynchrone invoqué au chargement du menu
        /// horizontal, après application des quatre méthodes synchrones
        /// (<see cref="ApplyLayout"/>, <see cref="OnResized"/>,
        /// <see cref="ApplyNavigationRules"/>,
        /// <see cref="ApplySecurityRules"/>).
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// <see cref="OnLoadedHandler"/> sous la forme
        /// <c>{_callee} &gt; OnLoadedHandler &gt; OnLoadedAsync</c>.</param>
        /// <param name="ct">Token d'annulation coopérative. La valeur
        /// par défaut <see cref="CancellationToken.None"/> est admise :
        /// le socle n'instancie pas de
        /// <see cref="CancellationTokenSource"/> propre, l'annulation
        /// est laissée à la discrétion des dérivés qui en ont besoin,
        /// conformément à §4.15.9 du 0230.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// chargement.</returns>
        /// <remarks>
        /// <para>Implémentation par défaut : Retourne
        /// <see cref="Task.CompletedTask"/>. Les menus horizontaux
        /// statiques (sans chargement asynchrone de données) n'ont pas
        /// besoin de redéfinir cette méthode.</para>
        /// </remarks>
        protected virtual Task OnLoadedAsync(
            string callChain,
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Point d'extension asynchrone invoqué au déchargement du
        /// menu horizontal.
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// <see cref="OnUnloadedHandler"/> sous la forme
        /// <c>{_callee} &gt; OnUnloadedHandler &gt; OnUnloadedAsync</c>.</param>
        /// <param name="ct">Token d'annulation coopérative. Cf.
        /// remarques de <see cref="OnLoadedAsync"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// libération des ressources.</returns>
        /// <remarks>
        /// <para>Implémentation par défaut : Retourne
        /// <see cref="Task.CompletedTask"/>.</para>
        /// </remarks>
        protected virtual Task OnUnloadedAsync(
            string callChain,
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Point d'extension synchrone invoqué au montage initial du
        /// menu horizontal (immédiatement après
        /// <see cref="ApplyLayout"/>) et à chaque redimensionnement
        /// ultérieur de la fenêtre principale.
        /// </summary>
        /// <param name="callChain">CallChain construite par le handler
        /// appelant (<see cref="OnLoadedHandler"/> au montage initial,
        /// <see cref="OnSizeChangedHandler"/> à chaque redimensionnement
        /// ultérieur) sous la forme
        /// <c>{_callee} &gt; {handler} &gt; OnResized</c>.</param>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinissable par les dérivés
        /// pour ajuster dynamiquement les dimensions des contrôles
        /// internes du menu horizontal en réaction au redimensionnement
        /// de la fenêtre principale, et pour appliquer ces mêmes
        /// ajustements une première fois au montage initial. Sa
        /// signature est strictement symétrique de celle de
        /// <c>Page_Generic.OnResized</c>.</para>
        /// <para>Invocation au montage initial : À compter de la
        /// présente refonte, <see cref="OnResized"/> est invoqué une
        /// première fois par <see cref="OnLoadedHandler"/>,
        /// immédiatement après <see cref="ApplyLayout"/> et avant
        /// <see cref="ApplyNavigationRules"/>, puis à chaque
        /// redimensionnement ultérieur par
        /// <see cref="OnSizeChangedHandler"/>. Les dérivés qui
        /// surchargent <see cref="OnResized"/> doivent donc savoir
        /// que leur override sera invoqué au montage initial puis à
        /// chaque redimensionnement ultérieur. Voir la rubrique
        /// « Changements comportementaux introduits par la refonte »
        /// du bloc remarques de <see cref="MH_Generic"/> pour la
        /// doctrine associée.</para>
        /// <para>Caractère synchrone : Cette méthode est volontairement
        /// synchrone et ne propage pas de <c>CancellationToken</c>.
        /// L'événement <see cref="FrameworkElement.SizeChanged"/> est
        /// déclenché à haute fréquence pendant un redimensionnement
        /// utilisateur (plusieurs fois par seconde) ; un traitement
        /// asynchrone introduirait un risque d'accumulation de tâches
        /// concurrentes et de désynchronisation de l'affichage. Le
        /// traitement attendu ici est purement local à la vue et doit
        /// rester rapide.</para>
        /// <para>Implémentation par défaut : Stylise le
        /// <see cref="Grid"/> principal <c>MH_Grid</c>, ses deux
        /// <see cref="ColumnDefinition"/> <c>MH_Grid_C1</c> et
        /// <c>MH_Grid_C2</c>, et le <see cref="Border"/> latéral
        /// <c>MH_Border</c> via
        /// <see cref="IS_ControlStyler.StyleHorizontalMenuGrid"/>, en
        /// passant la largeur courante <c>_window.MainWindowWidth</c>
        /// pour le calcul des dimensions des colonnes. Si l'un des
        /// quatre éléments XAML est absent, la stylisation est
        /// silencieusement ignorée et une trace de diagnostic est
        /// émise par <see cref="Find{T}"/>. Un override doit appeler
        /// <c>base.OnResized(callChain)</c> avant tout ajustement
        /// dimensionnel propre au dérivé.</para>
        /// </remarks>
        protected virtual void OnResized(string callChain)
        {
            if (Find<Grid>("MH_Grid") is Grid grid
                && Find<ColumnDefinition>("MH_Grid_C1") is ColumnDefinition column1
                && Find<ColumnDefinition>("MH_Grid_C2") is ColumnDefinition column2
                && Find<Border>("MH_Border") is Border border)
            {
                _controlStyler.StyleHorizontalMenuGrid(
                    grid, column1, column2, border, _window.MainWindowWidth);
            }
        }

        /// <summary>
        /// Helper protégé de résolution typée d'éléments XAML nommés,
        /// assorti d'une trace de diagnostic en cas d'absence.
        /// </summary>
        /// <typeparam name="T">Type attendu de l'élément XAML recherché
        /// (par exemple <see cref="Grid"/>, <see cref="Button"/>,
        /// <see cref="ColumnDefinition"/>). Contrainte sur
        /// <c>class</c>.</typeparam>
        /// <param name="name">Valeur de l'attribut <c>x:Name</c> de
        /// l'élément XAML recherché.</param>
        /// <returns>L'élément XAML résolu et casté en
        /// <typeparamref name="T"/>, ou <c>null</c> si aucun élément
        /// n'est trouvé ou si le cast échoue.</returns>
        /// <remarks>
        /// <para>Contexte : Helper destiné à factoriser la combinaison
        /// récurrente <c>FindName(name) as T</c> dans
        /// <see cref="OnResized"/> et dans
        /// <see cref="SetButtonVisibility"/>. Émet une trace de
        /// diagnostic via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// lorsque l'élément attendu est absent ou que le cast échoue.
        /// Cette trace est un filet de sécurité contre les ruptures
        /// silencieuses de contrat XAML (renommage d'un <c>x:Name</c>
        /// côté XAML sans propagation au code-behind), conformément à
        /// §4.15.9 du 0230.</para>
        /// </remarks>
        protected T? Find<T>(string name) where T : class
        {
            var element = FindName(name) as T;
            if (element is null)
            {
                Debug.WriteLine(
                    $"[{_callee}.{nameof(Find)}] Élément '{name}' de type " +
                    $"{typeof(T).Name} attendu mais introuvable dans l'arbre XAML.");
            }
            return element;
        }

        /// <summary>
        /// Helper protégé d'affectation de visibilité à un
        /// <see cref="Button"/> nommé, factorisant le patron
        /// <c>if (Find&lt;Button&gt;(name) is Button btn) btn.Visibility = visible ? Visible : Collapsed;</c>.
        /// </summary>
        /// <param name="name">Valeur de l'attribut <c>x:Name</c> du
        /// bouton.</param>
        /// <param name="visible"><c>true</c> pour rendre le bouton
        /// visible (<see cref="Visibility.Visible"/>), <c>false</c>
        /// pour le masquer (<see cref="Visibility.Collapsed"/>).</param>
        /// <remarks>
        /// <para>Contexte : Helper destiné à factoriser le patron
        /// récurrent de conditionnement de la visibilité des boutons
        /// transverses dans <see cref="ApplyNavigationRules"/> et
        /// <see cref="ApplySecurityRules"/>. La résolution du bouton
        /// passe par <see cref="Find{T}"/>, ce qui garantit l'émission
        /// d'une trace de diagnostic si le bouton attendu est absent
        /// du XAML.</para>
        /// <para>Si le bouton est introuvable, aucune action n'est
        /// effectuée (l'absence est uniquement tracée par
        /// <see cref="Find{T}"/>) ; la stylisation du menu se poursuit
        /// sans interruption.</para>
        /// </remarks>
        protected void SetButtonVisibility(string name, bool visible)
        {
            if (Find<Button>(name) is Button button)
            {
                button.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Applique la stylisation invariante des quatre boutons transverses
        /// communs à tous les menus horizontaux (Menu, Home, Previous,
        /// Refresh) via la méthode
        /// <see cref="IS_ControlStyler.StyleHorizontalMenuButton"/> et les
        /// URI d'icônes du référentiel statique <c>RS_Icons</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode privée invoquée par
        /// <see cref="ApplyLayout"/>. Pour chaque bouton transverse, la
        /// stylisation consomme trois éléments XAML résolus via
        /// <see cref="Find{T}"/> : le <see cref="Button"/> lui-même
        /// (<c>MH_Xxx</c>), son icône <see cref="System.Windows.Controls.Image"/>
        /// (<c>MH_Xxx_Icon</c>) et son libellé <see cref="TextBlock"/>
        /// (<c>MH_Xxx_Text</c>). L'URI iconographique est résolue depuis
        /// <c>RS_Icons</c>, référentiel statique d'accès direct conformément
        /// à §3.13 du 0230. Le paramètre optionnel <c>text</c> de
        /// <see cref="IS_ControlStyler.StyleHorizontalMenuButton"/> n'est
        /// pas fourni : le contenu textuel des boutons est alimenté par
        /// binding sur le ViewModel pour la traduction multilingue,
        /// conformément à §4.11.6.</para>
        /// <para>Trace de diagnostic : Toute absence d'un des trois
        /// éléments XAML pour un bouton donné est signalée par une trace
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/> émise
        /// par <see cref="Find{T}"/>, sans interruption du chargement.</para>
        /// </remarks>
        private void StyleCommonButtons()
        {
            StyleCommonButton("MH_Menu", RS_Icons.MH_Menu_Source);
            StyleCommonButton("MH_Home", RS_Icons.MH_Home_Source);
            StyleCommonButton("MH_Previous", RS_Icons.MH_Previous_Source);
            StyleCommonButton("MH_Refresh", RS_Icons.MH_Refresh_Source);
        }

        /// <summary>
        /// Stylise un bouton transverse standard du menu horizontal en
        /// résolvant ses trois sous-éléments XAML (le <see cref="Button"/>
        /// lui-même, son <see cref="Image"/> et son <see cref="TextBlock"/>)
        /// et en déléguant à
        /// <see cref="IS_ControlStyler.StyleHorizontalMenuButton"/>.
        /// </summary>
        /// <param name="buttonName">Nom du bouton à styliser (par exemple
        /// <c>"MH_Menu"</c>). Les noms des sous-éléments sont dérivés par
        /// concaténation : <c>{buttonName}_Icon</c> et
        /// <c>{buttonName}_Text</c>.</param>
        /// <param name="iconUri">URI iconographique à appliquer, résolue
        /// depuis le référentiel statique <c>RS_Icons</c>.</param>
        /// <remarks>
        /// <para>Contexte : Helper privé invoqué par
        /// <see cref="StyleCommonButtons"/> pour factoriser la combinaison
        /// récurrente <c>Find&lt;Button&gt; + Find&lt;Image&gt; + Find&lt;TextBlock&gt;
        /// + StyleHorizontalMenuButton</c>, avec garde
        /// défensive sur la nullité des trois résolutions.</para>
        /// <para>Filet de sécurité : Si l'un des trois éléments XAML est
        /// absent, la stylisation du bouton concerné est silencieusement
        /// ignorée. Une trace de diagnostic est émise par
        /// <see cref="Find{T}"/> pour chaque élément manquant.</para>
        /// </remarks>
        private void StyleCommonButton(string buttonName, Uri iconUri)
        {
            if (Find<Button>(buttonName) is Button button
                && Find<Image>($"{buttonName}_Icon") is Image icon
                && Find<TextBlock>($"{buttonName}_Text") is TextBlock textBlock)
            {
                _controlStyler.StyleHorizontalMenuButton(
                    button, icon, textBlock, iconUri);
            }
        }

        /// <summary>
        /// Handler interne de l'événement
        /// <see cref="FrameworkElement.Loaded"/>. Construit les
        /// CallChains canoniques, applique la stylisation invariante,
        /// la stylisation dimensionnelle initiale, les règles de
        /// navigation et de sécurité, puis déclenche le point
        /// d'extension asynchrone <see cref="OnLoadedAsync"/>, dans
        /// un filet de sécurité ultime.
        /// </summary>
        /// <remarks>
        /// <para>Ordre d'invocation (§4.15.9 du 0230) :</para>
        /// <list type="number">
        ///   <item><description><see cref="ApplyLayout"/> — stylisation
        ///   invariante des quatre boutons transverses.</description></item>
        ///   <item><description><see cref="OnResized"/> — stylisation
        ///   dimensionnelle initiale du Grid principal et de ses
        ///   colonnes.</description></item>
        ///   <item><description><see cref="ApplyNavigationRules"/> —
        ///   conditionnement de la visibilité des boutons aux droits
        ///   de navigation.</description></item>
        ///   <item><description><see cref="ApplySecurityRules"/> —
        ///   conditionnement de la visibilité des boutons aux droits
        ///   applicatifs.</description></item>
        ///   <item><description><see cref="OnLoadedAsync"/> —
        ///   chargement asynchrone post-montage.</description></item>
        /// </list>
        /// <para>Construction des CallChains : Une CallChain distincte
        /// est construite pour chacune des cinq invocations, par
        /// réaffectation successive d'une variable locale. La première
        /// CallChain est déclarée hors du bloc <c>try</c> par symétrie
        /// stricte avec le pattern de <c>Page_Generic.OnLoadedHandler</c>.</para>
        /// </remarks>
        private async void OnLoadedHandler(object sender, RoutedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnLoadedHandler)} > {nameof(ApplyLayout)}";

            try
            {
                ApplyLayout(callChain);

                callChain = $"{_callee} > {nameof(OnLoadedHandler)} > {nameof(OnResized)}";
                OnResized(callChain);

                callChain = $"{_callee} > {nameof(OnLoadedHandler)} > {nameof(ApplyNavigationRules)}";
                ApplyNavigationRules(callChain);

                callChain = $"{_callee} > {nameof(OnLoadedHandler)} > {nameof(ApplySecurityRules)}";
                ApplySecurityRules(callChain);

                callChain = $"{_callee} > {nameof(OnLoadedHandler)} > {nameof(OnLoadedAsync)}";
                await OnLoadedAsync(callChain);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[{_callee}.{nameof(OnLoadedHandler)}] " +
                    $"Exception non gérée capturée par le filet de sécurité ultime : " +
                    $"{ex.GetType().Name} — {ex.Message}");
            }
        }

        /// <summary>
        /// Handler interne de l'événement
        /// <see cref="FrameworkElement.Unloaded"/>. Construit la
        /// CallChain canonique et déclenche le point d'extension
        /// asynchrone <see cref="OnUnloadedAsync"/> dans un filet
        /// de sécurité ultime.
        /// </summary>
        /// <remarks>
        /// <para>Symétrie avec <see cref="OnLoadedHandler"/> : Mêmes
        /// contraintes WPF (<c>async void</c>) et même stratégie de
        /// filet de sécurité ultime.</para>
        /// </remarks>
        private async void OnUnloadedHandler(object sender, RoutedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnUnloadedHandler)} > {nameof(OnUnloadedAsync)}";

            try
            {
                await OnUnloadedAsync(callChain);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[{_callee}.{nameof(OnUnloadedHandler)}] " +
                    $"Exception non gérée capturée par le filet de sécurité ultime : " +
                    $"{ex.GetType().Name} — {ex.Message}");
            }
        }

        /// <summary>
        /// Handler interne de l'événement
        /// <see cref="FrameworkElement.SizeChanged"/>. Construit les
        /// CallChains canoniques et réinvoque la stylisation
        /// dimensionnelle (<see cref="OnResized"/>), les règles de
        /// navigation (<see cref="ApplyNavigationRules"/>) et les
        /// règles de sécurité (<see cref="ApplySecurityRules"/>) dans
        /// un filet de sécurité ultime.
        /// </summary>
        /// <remarks>
        /// <para>Ordre d'invocation (§4.15.9 du 0230) :</para>
        /// <list type="number">
        ///   <item><description><see cref="OnResized"/> — réajustement
        ///   dimensionnel.</description></item>
        ///   <item><description><see cref="ApplyNavigationRules"/> —
        ///   réévaluation des droits de navigation.</description></item>
        ///   <item><description><see cref="ApplySecurityRules"/> —
        ///   réévaluation des droits applicatifs.</description></item>
        /// </list>
        /// <para>Non-invocation de <see cref="ApplyLayout"/> au
        /// redimensionnement : <see cref="ApplyLayout"/> porte la
        /// stylisation invariante et n'est jamais invoqué au
        /// redimensionnement, conformément à la séparation sémantique
        /// stricte entre stylisation invariante et ajustement
        /// dimensionnel introduite par la présente refonte. La
        /// stylisation invariante des quatre boutons transverses,
        /// appliquée une seule fois au montage initial par
        /// <see cref="OnLoadedHandler"/>, n'a pas à être réappliquée
        /// au redimensionnement.</para>
        /// </remarks>
        private void OnSizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnSizeChangedHandler)} > {nameof(OnResized)}";

            try
            {
                OnResized(callChain);

                callChain = $"{_callee} > {nameof(OnSizeChangedHandler)} > {nameof(ApplyNavigationRules)}";
                ApplyNavigationRules(callChain);

                callChain = $"{_callee} > {nameof(OnSizeChangedHandler)} > {nameof(ApplySecurityRules)}";
                ApplySecurityRules(callChain);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[{_callee}.{nameof(OnSizeChangedHandler)}] " +
                    $"Exception non gérée capturée par le filet de sécurité ultime : " +
                    $"{ex.GetType().Name} — {ex.Message}");
            }
        }

        #endregion
    }
}