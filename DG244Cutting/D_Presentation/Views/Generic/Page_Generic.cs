using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Views.Generic
{
    /// <summary>
    /// Classe de base commune à toutes les pages WPF de l'application
    /// DG244Cutting, socle de la famille Page de la couche
    /// <c>D_Presentation</c>.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Socle de présentation qui orchestre le cycle de
    /// vie WPF d'une page (chargement, déchargement, redimensionnement)
    /// et le traduit en points d'extension asynchrones ou synchrones
    /// selon la nature du traitement attendu. Réside dans
    /// <c>D_Presentation/Views/Generic</c>. La classe est instanciable
    /// directement par le framework WPF de navigation (constructeur
    /// <c>public</c>, classe non <c>abstract</c>) lorsqu'aucune
    /// spécialisation n'est requise, conformément à R-4.15.1 règle 7 et
    /// à §4.15.6 du 0230.</para>
    /// <para>Objectif : Fournir un comportement uniforme pour toutes
    /// les vues de page de l'application, en isolant les dérivés des
    /// contraintes d'abonnement aux événements WPF, en propageant une
    /// CallChain canonique aux points d'extension conformément à §4.5,
    /// et en garantissant qu'aucune exception non gérée ne remonte au
    /// <see cref="System.Windows.Application.DispatcherUnhandledException"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>S'abonner aux événements WPF
    ///   <see cref="FrameworkElement.Loaded"/>,
    ///   <see cref="FrameworkElement.Unloaded"/> et
    ///   <see cref="FrameworkElement.SizeChanged"/> au constructeur,
    ///   et orchestrer leur traduction en points d'extension
    ///   <c>protected</c> exploitables par les dérivés.</description></item>
    ///   <item><description>Exposer les quatre points d'extension
    ///   <see cref="ApplyLayout"/> (synchrone),
    ///   <see cref="OnLoadedAsync"/> (asynchrone),
    ///   <see cref="OnUnloadedAsync"/> (asynchrone) et
    ///   <see cref="OnResized"/> (synchrone), tous redéfinissables
    ///   par les dérivés selon leurs besoins propres.</description></item>
    ///   <item><description>Construire et propager une CallChain
    ///   canonique aux points d'extension sous la forme
    ///   <c>{_callee} &gt; {handler} &gt; {extensionPoint}</c>,
    ///   conformément au patron méthode publique de §4.5.1.</description></item>
    ///   <item><description>Propager un
    ///   <see cref="System.Threading.CancellationToken"/> optionnel aux
    ///   points d'extension asynchrones, permettant aux dérivés de
    ///   participer à une annulation coopérative sans imposer la
    ///   présence d'un <c>CancellationTokenSource</c> côté socle.</description></item>
    ///   <item><description>Exposer le helper protégé
    ///   <see cref="Find{T}"/> pour la résolution typée d'éléments XAML
    ///   nommés, assortie d'une trace de diagnostic lorsqu'un élément
    ///   attendu est absent (filet de sécurité contre les ruptures
    ///   silencieuses de contrat XAML).</description></item>
    ///   <item><description>Garantir, par un filet de sécurité ultime au
    ///   bord des trois handlers privés, qu'aucune exception ne remonte
    ///   au framework WPF, même si un dérivé a omis d'encapsuler ses
    ///   appels asynchrones dans le filet du ViewModel
    ///   (<c>VM_Page_Generic.ExecuteSafeAsync</c>, hérité de
    ///   <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Generic"/>).</description></item>
    ///   <item><description>Mettre à disposition des dérivés les
    ///   services transverses <see cref="IS_ControlStyler"/> (stylisation
    ///   centralisée) et <see cref="ISE_Window"/> (accès à la fenêtre
    ///   principale), stockés en champs <c>protected</c> exploitables
    ///   directement dans les surcharges des points d'extension.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne charge aucun libellé : la traduction
    ///   relève des ViewModels via leurs bindings, conformément à §4.11
    ///   et à I-4.11.10. Toute tentative de chargement de libellé depuis
    ///   un code-behind dérivant de <see cref="Page_Generic"/>
    ///   constituerait une non-conformité.</description></item>
    ///   <item><description>Ne porte aucune logique métier : celle-ci
    ///   relève des UseCases appelés par les ViewModels.</description></item>
    ///   <item><description>Ne traite pas les exceptions métier : ce
    ///   rôle appartient au filet de sécurité
    ///   <c>VM_Page_Generic.ExecuteSafeAsync</c>. Le filet ultime des
    ///   handlers WPF de la présente classe n'est qu'un dernier rempart
    ///   contre les défaillances inattendues du framework, et ses
    ///   captures sont tracées via
    ///   <see cref="System.Diagnostics.Debug.WriteLine(string)"/> sans
    ///   journalisation applicative.</description></item>
    ///   <item><description>N'expose pas de découpage
    ///   <c>ApplyNavigationRules</c> / <c>ApplySecurityRules</c> : ces
    ///   deux points d'extension sont propres à <c>MH_Generic</c>, dont
    ///   la standardisation des quatre boutons transverses du menu
    ///   horizontal justifie le découpage en méthodes synchrones
    ///   distinctes. Côté pages, dont les contrôles sont propres à
    ///   chaque dérivé, un tel découpage serait artificiel : la totalité
    ///   de la stylisation et du conditionnement visuel d'une page passe
    ///   par <see cref="ApplyLayout"/>.</description></item>
    ///   <item><description>Ne désabonne pas les handlers
    ///   <see cref="FrameworkElement.Loaded"/>,
    ///   <see cref="FrameworkElement.Unloaded"/> et
    ///   <see cref="FrameworkElement.SizeChanged"/> : ce choix est
    ///   délibéré. Le framework de navigation WPF instancie une nouvelle
    ///   page à chaque navigation et ne réutilise jamais une instance
    ///   antérieure ; le garbage collector libère naturellement la page
    ///   au démontage. Tout désabonnement explicite serait techniquement
    ///   inutile et compliquerait inutilement le code.</description></item>
    /// </list>
    /// <para>Changement comportemental introduit par la refonte :</para>
    /// <para>À compter de la présente refonte, <see cref="OnResized"/>
    /// est invoqué une première fois au montage par
    /// <see cref="OnLoadedHandler"/>, immédiatement après
    /// <see cref="ApplyLayout"/> et avant <see cref="OnLoadedAsync"/>.
    /// Auparavant, <see cref="OnResized"/> (alors nommée
    /// <c>OnPageResized</c>) n'était déclenchée qu'à compter du premier
    /// événement <see cref="FrameworkElement.SizeChanged"/> ultérieur au
    /// montage. Les dérivés qui surchargent <see cref="OnResized"/>
    /// doivent en tenir compte : leur override sera invoqué au montage
    /// initial puis à chaque redimensionnement ultérieur. Cette
    /// modification garantit que la mise en page dépendante des
    /// dimensions de la fenêtre principale est appliquée dès le montage,
    /// sans dépendre de la survenue d'un événement de redimensionnement
    /// pour devenir effective. Le contrat de séparation sémantique
    /// reste préservé : <see cref="ApplyLayout"/> porte la stylisation
    /// invariante (indépendante des dimensions de la fenêtre) et n'est
    /// jamais invoqué au redimensionnement, tandis que
    /// <see cref="OnResized"/> porte l'ajustement dimensionnel.</para>
    /// <para>Exception architecturale documentée — Service Locator
    /// (EA-2) :</para>
    /// <para>Cette classe résout ses dépendances via
    /// <c>App.ServiceProvider.GetRequiredService</c>, pattern
    /// habituellement proscrit (Service Locator). Cette exception est
    /// délibérée et limitée à la couche vue : une <see cref="Page"/> WPF
    /// est instanciée par le framework de navigation XAML
    /// (<c>SR_Navigation.NavigateToPage</c> via
    /// <c>Activator.CreateInstance</c>), qui n'autorise pas d'injection
    /// par constructeur paramétré. L'usage du Service Locator est
    /// strictement cantonné aux dérivés directs de
    /// <see cref="Page_Generic"/>, étendu nominativement à la résolution
    /// de leur ViewModel associé, et ne doit jamais se propager aux
    /// ViewModels, UseCases, Services ni Repositories. Cf. §4.15.6 et
    /// §4.15.10 du 0230 pour la formalisation complète de cette
    /// exception.</para>
    /// <para>Structure des régions : La classe applique la structure
    /// normative à six régions, conformément à §4.4.2 (cinq régions
    /// standard) et à §4.4.3 (région supplémentaire Méthodes protégées,
    /// obligatoirement présente au titre de R-4.4.10 car la classe
    /// expose au moins une méthode <c>protected</c>). Les deux autres
    /// extensions de §4.4.3 (Propriétés publiques, Événements / Délégués
    /// / Indexeurs) ne s'appliquent pas : la classe n'expose ni
    /// propriété publique propre, ni événement applicatif propre. La
    /// région Méthodes publiques est obligatoirement présente mais vide
    /// avec le marqueur <c>// A compléter</c>, aucune méthode publique
    /// propre n'étant exposée (les méthodes publiques héritées
    /// proviennent intégralement de <see cref="Page"/> et de
    /// <see cref="FrameworkElement"/>).</para>
    /// </remarks>
    public class Page_Generic : Page
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
        /// par R-4.4.7 (dépendances → <c>_callee</c> → initialisations
        /// locales). La valeur reflète le type concret du dérivé (par
        /// exemple <c>Page99</c> ou <c>Page20</c>), non le type
        /// déclarant <see cref="Page_Generic"/> — ce qui garantit la
        /// pertinence du nom dans les CallChains et les traces de
        /// diagnostic, indépendamment de la classe dans laquelle le
        /// champ est physiquement déclaré.</para>
        /// <para>Format des CallChains construites : chaque CallChain
        /// produite par cette classe suit le patron méthode publique de
        /// §4.5.1, sous la forme
        /// <c>{_callee} &gt; {handler} &gt; {extensionPoint}</c>. Par
        /// exemple, pour un dérivé <c>Page99</c>, la CallChain transmise
        /// à <see cref="ApplyLayout"/> sera
        /// <c>Page99 &gt; OnLoadedHandler &gt; ApplyLayout</c>, et celle
        /// transmise à <see cref="OnLoadedAsync"/> sera
        /// <c>Page99 &gt; OnLoadedHandler &gt; OnLoadedAsync</c>.</para>
        /// </remarks>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service de stylisation centralisée des contrôles WPF, exposé
        /// en <c>protected</c> aux dérivés pour leurs surcharges de
        /// <see cref="ApplyLayout"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton résolu au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> au titre de
        /// l'exception architecturale Service Locator (EA-2) propre à la
        /// couche vue. La visibilité <c>protected</c> est justifiée par
        /// un usage légitime documenté : les dérivés appellent
        /// directement les méthodes de stylisation
        /// (<c>StylePage</c>, <c>StyleTextBlockPageTitle</c>,
        /// <c>StyleListView</c>, etc.) dans leur override de
        /// <see cref="ApplyLayout"/>, conformément au principe posé par
        /// la nuance « visibilité conditionnée à un usage légitime
        /// documenté » de §4.15.</para>
        /// <para>Non-usage : Cette dépendance n'est pas consommée
        /// directement par <see cref="Page_Generic"/> elle-même ; sa
        /// résolution au constructeur est effectuée pour la mettre à
        /// disposition des dérivés, qui en sont les consommateurs
        /// exclusifs.</para>
        /// </remarks>
        protected readonly IS_ControlStyler _controlStyler;

        /// <summary>
        /// Setting Singleton d'accès à la fenêtre principale de
        /// l'application, exposé en <c>protected</c> aux dérivés pour
        /// les ajustements dynamiques liés aux dimensions de la fenêtre
        /// (hauteur de <c>ScrollViewer</c>, taille de panneaux, etc.).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton résolu au constructeur via
        /// <c>App.ServiceProvider.GetRequiredService</c> au titre de
        /// l'EA-2 Service Locator. La visibilité <c>protected</c> est
        /// justifiée par un usage légitime documenté : les dérivés
        /// consomment typiquement <c>_window.MainWindowWidth</c> et
        /// <c>_window.MainWindowHeight</c> dans leur override de
        /// <see cref="ApplyLayout"/> ou de <see cref="OnResized"/>
        /// pour ajuster dynamiquement la mise en page.</para>
        /// </remarks>
        protected readonly ISE_Window _window;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="Page_Generic"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur sans paramètre, modificateur
        /// <c>public</c>, conformément à R-4.15.1 règle 7 (instanciation
        /// directe attendue par le framework WPF de navigation lorsque
        /// la classe est utilisée sans spécialisation). Les dépendances
        /// sont résolues via le ServiceProvider statique de
        /// <c>App</c> conformément à l'exception architecturale Service
        /// Locator documentée en remarque de classe (EA-2).</para>
        /// <para>Séquence d'initialisation (R-4.4.7) :</para>
        /// <list type="number">
        ///   <item><description>Résolution et affectation des deux
        ///   dépendances <see cref="_controlStyler"/> et
        ///   <see cref="_window"/> via
        ///   <c>App.ServiceProvider.GetRequiredService</c>. La méthode
        ///   <c>GetRequiredService</c> est utilisée (et non
        ///   <c>GetService</c>), conformément à la convention de
        ///   plateforme <c>App.ServiceProvider</c> : toute dépendance
        ///   non résolue doit faire échouer l'instanciation immédiatement
        ///   par exception explicite.</description></item>
        ///   <item><description>Initialisation du champ
        ///   <see cref="_callee"/> via <c>GetType().Name</c>, après
        ///   l'affectation des dépendances.</description></item>
        ///   <item><description>Abonnement aux trois événements WPF
        ///   <see cref="FrameworkElement.Loaded"/>,
        ///   <see cref="FrameworkElement.Unloaded"/> et
        ///   <see cref="FrameworkElement.SizeChanged"/> — initialisations
        ///   locales au sens de R-4.4.7. Les handlers privés
        ///   <see cref="OnLoadedHandler"/>,
        ///   <see cref="OnUnloadedHandler"/> et
        ///   <see cref="OnSizeChangedHandler"/> constituent le filet de
        ///   sécurité ultime qui isole le framework WPF de toute
        ///   exception non gérée éventuellement levée par les dérivés.</description></item>
        /// </list>
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le
        /// constructeur au-delà de la résolution des dépendances. Une
        /// défaillance de <c>GetRequiredService</c> traduirait une
        /// erreur de configuration du conteneur DI et doit faire échouer
        /// l'instanciation immédiatement.</para>
        /// </remarks>
        public Page_Generic()
        {
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _window = App.ServiceProvider.GetRequiredService<ISE_Window>();

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
        /// Point d'extension synchrone invoqué pour appliquer la mise
        /// en page de la page lors du chargement.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinie par les dérivés pour
        /// appliquer la stylisation initiale des contrôles XAML (Page,
        /// ScrollViewers, ListViews, TextBlocks, etc.) via le service
        /// <see cref="IS_ControlStyler"/>. Invoquée par le handler
        /// <see cref="OnLoadedHandler"/> à l'événement
        /// <see cref="FrameworkElement.Loaded"/>, avant
        /// <see cref="OnResized"/> et <see cref="OnLoadedAsync"/>, afin
        /// que les contrôles soient stylés avant tout ajustement
        /// dimensionnel et avant tout chargement de données.</para>
        /// <para>Caractère synchrone : Cette méthode est volontairement
        /// synchrone. La stylisation est une opération purement locale
        /// à la vue, sans accès aux ressources externes ni aux
        /// UseCases.</para>
        /// <para>Séparation sémantique avec <see cref="OnResized"/> :
        /// <see cref="ApplyLayout"/> porte la stylisation invariante
        /// (indépendante des dimensions de la fenêtre) et n'est jamais
        /// invoqué au redimensionnement ; <see cref="OnResized"/> porte
        /// l'ajustement dimensionnel et est invoqué au montage et à
        /// chaque redimensionnement ultérieur.</para>
        /// <para>Implémentation par défaut : Aucun traitement. Les
        /// dérivés qui n'ont aucune stylisation propre à appliquer
        /// n'ont pas besoin de redéfinir cette méthode.</para>
        /// <para>Patron de surcharge normatif :</para>
        /// <code>
        /// protected override void ApplyLayout(string callChain)
        /// {
        ///     base.ApplyLayout(callChain);
        ///     if (Find&lt;Grid&gt;("PageGrid") is Grid pageGrid) _controlStyler.StylePage(pageGrid);
        ///     if (Find&lt;TextBlock&gt;("PageTitleMain") is TextBlock pageTitle) _controlStyler.StyleTextBlockPageTitle(pageTitle);
        /// }
        /// </code>
        /// </remarks>
        /// <param name="callChain">CallChain construite par
        /// <see cref="OnLoadedHandler"/> sous la forme
        /// <c>{_callee} &gt; OnLoadedHandler &gt; ApplyLayout</c>, à
        /// utiliser telle quelle si le dérivé délègue à un service
        /// nécessitant un <c>caller</c>, ou à enrichir selon le patron
        /// de §4.5.1 si le dérivé invoque ensuite des méthodes propres.</param>
        protected virtual void ApplyLayout(string callChain)
        {
            // Aucun traitement par défaut.
        }

        /// <summary>
        /// Point d'extension asynchrone invoqué au chargement de la
        /// page, après application de la mise en page et de l'ajustement
        /// dimensionnel initial.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinie par les dérivés pour
        /// déclencher le chargement asynchrone des données du
        /// ViewModel associé via <c>_viewModel.LoadAsync</c>,
        /// hook formellement déclaré au socle
        /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic.LoadAsync"/>.
        /// L'invocation a lieu après l'application de la mise en page
        /// (<see cref="ApplyLayout"/>) et de l'ajustement dimensionnel
        /// initial (<see cref="OnResized"/>), de sorte que les contrôles
        /// soient stylés et correctement dimensionnés avant l'affichage
        /// des données.</para>
        /// <para>Implémentation par défaut : Retourne
        /// <see cref="Task.CompletedTask"/>. Les dérivés qui n'ont
        /// aucun chargement asynchrone à effectuer n'ont pas besoin de
        /// redéfinir cette méthode (cas typique des pages statiques
        /// telles que <c>Page99</c>).</para>
        /// <para>Patron de surcharge normatif :</para>
        /// <code>
        /// protected override async Task OnLoadedAsync(
        ///     string callChain,
        ///     CancellationToken ct = default)
        /// {
        ///     await base.OnLoadedAsync(callChain, ct);
        ///     await _viewModel.LoadAsync(callChain, ct);
        /// }
        /// </code>
        /// <para>Ancrage doctrinal : Le hook <c>LoadAsync</c> est
        /// désormais formellement déclaré au socle
        /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic"/>
        /// (cf.
        /// <see cref="DG244Cutting.D_Presentation.ViewModels.Generic.VM_Page_Generic.LoadAsync"/>),
        /// uniformisant le patron d'override pour tous les VM_Page
        /// concrets de l'application. Le nom retenu est <c>LoadAsync</c>
        /// (et non <c>LoadDataAsync</c>) pour sobriété sémantique (une
        /// page peut charger d'autres choses que des données métier
        /// stricto sensu) et pour parité nominale avec le hook
        /// équivalent introduit ultérieurement dans
        /// <c>VM_MH_Generic</c>.</para>
        /// <para>Gestion d'erreur : Toute exception levée par les
        /// dérivés est capturée par le filet de sécurité ultime du
        /// handler <see cref="OnLoadedHandler"/> et tracée via
        /// <see cref="System.Diagnostics.Debug.WriteLine(string)"/>.
        /// Les exceptions métier doivent être traitées en amont par le
        /// filet de sécurité du ViewModel
        /// (<c>VM_Page_Generic.ExecuteSafeAsync</c>) ; ce filet de
        /// second niveau n'est qu'un ultime rempart contre les
        /// défaillances inattendues du framework.</para>
        /// </remarks>
        /// <param name="callChain">CallChain construite par
        /// <see cref="OnLoadedHandler"/> sous la forme
        /// <c>{_callee} &gt; OnLoadedHandler &gt; OnLoadedAsync</c>.</param>
        /// <param name="ct">Token d'annulation coopérative propagé aux
        /// opérations asynchrones invoquées par le dérivé. Valeur par
        /// défaut <c>default</c> (équivalent à
        /// <see cref="System.Threading.CancellationToken.None"/>) :
        /// <see cref="Page_Generic"/> ne construit pas de
        /// <c>CancellationTokenSource</c> propre. Les dérivés qui
        /// souhaitent introduire une mécanique d'annulation propre
        /// peuvent en construire un et l'utiliser dans leur override.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// chargement.</returns>
        protected virtual Task OnLoadedAsync(
            string callChain,
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Point d'extension asynchrone invoqué au déchargement de la
        /// page.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinie par les dérivés pour
        /// libérer les ressources asynchrones (souscriptions, timers,
        /// sauvegardes différées) au moment où la page quitte la zone
        /// de navigation.</para>
        /// <para>Implémentation par défaut : Retourne
        /// <see cref="Task.CompletedTask"/>.</para>
        /// <para>Gestion d'erreur : Identique à
        /// <see cref="OnLoadedAsync"/> — toute exception est
        /// capturée par le filet de sécurité ultime du handler
        /// <see cref="OnUnloadedHandler"/> et tracée silencieusement en
        /// environnement de développement.</para>
        /// </remarks>
        /// <param name="callChain">CallChain construite par
        /// <see cref="OnUnloadedHandler"/> sous la forme
        /// <c>{_callee} &gt; OnUnloadedHandler &gt; OnUnloadedAsync</c>.</param>
        /// <param name="ct">Token d'annulation coopérative. Cf.
        /// remarques de <see cref="OnLoadedAsync"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// libération des ressources.</returns>
        protected virtual Task OnUnloadedAsync(
            string callChain,
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Point d'extension synchrone invoqué au montage initial de
        /// la page (immédiatement après <see cref="ApplyLayout"/>) et
        /// à chaque redimensionnement ultérieur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinie par les dérivés pour
        /// ajuster dynamiquement les dimensions des contrôles internes
        /// (hauteur de <c>ScrollViewer</c>, taille de panneaux) en
        /// réaction au redimensionnement de la fenêtre principale, et
        /// pour appliquer ces mêmes ajustements une première fois au
        /// montage initial de la page.</para>
        /// <para>Invocation au montage initial : À compter de la
        /// présente refonte, <see cref="OnResized"/> est invoqué une
        /// première fois par <see cref="OnLoadedHandler"/>,
        /// immédiatement après <see cref="ApplyLayout"/> et avant
        /// <see cref="OnLoadedAsync"/>. Les dérivés qui surchargent
        /// <see cref="OnResized"/> doivent donc savoir que leur override
        /// sera invoqué au montage initial puis à chaque
        /// redimensionnement ultérieur. Voir la rubrique « Changement
        /// comportemental introduit par la refonte » du bloc remarques
        /// de <see cref="Page_Generic"/> pour la doctrine
        /// associée.</para>
        /// <para>Caractère synchrone : Cette méthode est volontairement
        /// synchrone et ne propage pas de <c>CancellationToken</c>.
        /// L'événement <see cref="FrameworkElement.SizeChanged"/> est
        /// déclenché à haute fréquence pendant un redimensionnement
        /// utilisateur (plusieurs fois par seconde) ; un traitement
        /// asynchrone introduirait un risque d'accumulation de tâches
        /// concurrentes et de désynchronisation de l'affichage. Le
        /// traitement attendu ici est purement local à la vue et doit
        /// rester rapide.</para>
        /// <para>Implémentation par défaut : Aucun traitement.</para>
        /// </remarks>
        /// <param name="callChain">CallChain construite par le handler
        /// appelant (<see cref="OnLoadedHandler"/> au montage initial,
        /// <see cref="OnSizeChangedHandler"/> à chaque redimensionnement
        /// ultérieur) sous la forme
        /// <c>{_callee} &gt; {handler} &gt; OnResized</c>.</param>
        protected virtual void OnResized(string callChain)
        {
            // Aucun traitement par défaut.
        }

        /// <summary>
        /// Helper protégé de résolution typée d'éléments XAML nommés,
        /// assorti d'une trace de diagnostic en cas d'absence.
        /// </summary>
        /// <typeparam name="T">Type attendu de l'élément XAML recherché
        /// (par exemple <see cref="Grid"/>, <see cref="TextBlock"/>,
        /// <see cref="ScrollViewer"/>). Contrainte sur <c>class</c>
        /// pour permettre le retour de <c>null</c>.</typeparam>
        /// <param name="name">Valeur de l'attribut <c>x:Name</c> de
        /// l'élément XAML recherché.</param>
        /// <returns>L'élément XAML résolu et casté en <typeparamref name="T"/>,
        /// ou <c>null</c> si aucun élément n'est trouvé ou si le cast
        /// échoue.</returns>
        /// <remarks>
        /// <para>Contexte : Helper destiné à factoriser la combinaison
        /// récurrente <c>FindName(name) as T</c> dans les overrides de
        /// <see cref="ApplyLayout"/>, qui répètent fréquemment ce
        /// patron pour résoudre les contrôles à styliser.</para>
        /// <para>Trace de diagnostic : Lorsque la résolution échoue
        /// (élément absent ou cast invalide), le helper émet une trace
        /// via <see cref="System.Diagnostics.Debug.WriteLine(string)"/>
        /// identifiant le nom recherché, le type attendu et le dérivé
        /// appelant via <see cref="_callee"/>. Cette trace est un filet
        /// de sécurité contre les ruptures silencieuses de contrat XAML
        /// (renommage d'un <c>x:Name</c> côté XAML sans propagation au
        /// code-behind). Elle est observable exclusivement en
        /// environnement de développement.</para>
        /// <para>Usage typique :</para>
        /// <code>
        /// protected override void ApplyLayout(string callChain)
        /// {
        ///     base.ApplyLayout(callChain);
        ///     if (Find&lt;Grid&gt;("PageGrid") is Grid pageGrid) _controlStyler.StylePage(pageGrid);
        ///     if (Find&lt;TextBlock&gt;("PageTitleMain") is TextBlock pageTitle) _controlStyler.StyleTextBlockPageTitle(pageTitle);
        /// }
        /// </code>
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

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler interne de l'événement
        /// <see cref="FrameworkElement.Loaded"/>. Construit les
        /// CallChains canoniques, applique la mise en page, déclenche
        /// l'ajustement dimensionnel initial puis le point d'extension
        /// asynchrone <see cref="OnLoadedAsync"/>, en encapsulant
        /// l'ensemble dans un filet de sécurité ultime.
        /// </summary>
        /// <remarks>
        /// <para>Contraintes WPF : La signature <c>async void</c> est
        /// imposée par l'abonnement à un
        /// <see cref="RoutedEventHandler"/>. Cette signature —
        /// habituellement à proscrire — est ici acceptable parce qu'elle
        /// est confinée à un handler local, isolée par un try/catch
        /// ultime, et qu'aucun appelant n'a besoin d'observer la fin de
        /// l'opération.</para>
        /// <para>Ordre canonique d'invocation des trois points
        /// d'extension :</para>
        /// <list type="number">
        ///   <item><description><see cref="ApplyLayout"/> — stylisation
        ///   invariante au montage.</description></item>
        ///   <item><description><see cref="OnResized"/> — ajustement
        ///   dimensionnel initial, invoqué dès le montage pour
        ///   garantir que la mise en page dépendante des dimensions de
        ///   la fenêtre principale est appliquée sans attendre la
        ///   survenue d'un premier événement
        ///   <see cref="FrameworkElement.SizeChanged"/>. Cette invocation
        ///   est introduite par la présente refonte ; voir la rubrique
        ///   « Changement comportemental introduit par la refonte » du
        ///   bloc remarques de <see cref="Page_Generic"/>.</description></item>
        ///   <item><description><see cref="OnLoadedAsync"/> — chargement
        ///   asynchrone post-montage, sous le filet de sécurité
        ///   ultime.</description></item>
        /// </list>
        /// <para>Construction des CallChains : Trois CallChains
        /// distinctes sont construites localement, l'une pour
        /// <see cref="ApplyLayout"/>, une pour <see cref="OnResized"/>
        /// et la dernière pour <see cref="OnLoadedAsync"/>, conformément
        /// au patron méthode publique de §4.5.1 (chaque CallChain
        /// transmise à une méthode se termine par le nom de cette
        /// méthode). La variable locale <c>callChain</c> est déclarée
        /// une fois en tête de handler et réaffectée entre chaque
        /// invocation.</para>
        /// <para>Filet de sécurité : Le bloc try/catch ultime ne peut
        /// pas être contourné. Il garantit qu'aucune exception ne
        /// remonte au
        /// <see cref="System.Windows.Application.DispatcherUnhandledException"/>
        /// de WPF, même si un dérivé a omis d'encapsuler ses appels
        /// asynchrones dans le filet de sécurité du ViewModel.</para>
        /// </remarks>
        private async void OnLoadedHandler(object sender, RoutedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnLoadedHandler)} > {nameof(ApplyLayout)}";

            try
            {
                ApplyLayout(callChain);

                callChain = $"{_callee} > {nameof(OnLoadedHandler)} > {nameof(OnResized)}";
                OnResized(callChain);

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
        /// <see cref="FrameworkElement.Unloaded"/>. Construit la CallChain
        /// canonique et déclenche le point d'extension asynchrone
        /// <see cref="OnUnloadedAsync"/> dans un filet de sécurité
        /// ultime.
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
        /// <see cref="FrameworkElement.SizeChanged"/>. Construit la
        /// CallChain canonique et déclenche le point d'extension
        /// synchrone <see cref="OnResized"/> dans un filet de
        /// sécurité ultime.
        /// </summary>
        /// <remarks>
        /// <para>Caractère synchrone : La signature respecte celle d'un
        /// handler classique (<see cref="SizeChangedEventHandler"/>)
        /// sans introduire d'asynchronisme inutile, conformément au
        /// caractère synchrone du point d'extension
        /// <see cref="OnResized"/> documenté ci-dessus.</para>
        /// <para>Non-invocation de <see cref="ApplyLayout"/> au
        /// redimensionnement : <see cref="ApplyLayout"/> porte la
        /// stylisation invariante et n'est jamais invoqué au
        /// redimensionnement, conformément à la séparation sémantique
        /// stricte entre stylisation invariante et ajustement
        /// dimensionnel.</para>
        /// </remarks>
        private void OnSizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnSizeChangedHandler)} > {nameof(OnResized)}";

            try
            {
                OnResized(callChain);
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