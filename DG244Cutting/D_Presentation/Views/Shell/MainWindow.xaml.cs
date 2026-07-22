using System.Windows;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.ViewModels.Shell;
using DG244Cutting.D_Presentation.Views.Components.Banner;

namespace DG244Cutting.D_Presentation.Views.Shell
{
    /// <summary>
    /// Shell applicatif singulier de DG244Cutting, fenêtre principale hébergeant
    /// les frames de navigation <c>ActivePage</c> et <c>ActiveHorizontalMenu</c>
    /// pilotées par <see cref="IU_Navigation"/>.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant hors familles canoniques (UC, SR, CR, CH, QH,
    /// SE, RS, DTO, VM, MH, Page), instancié en portée Singleton par le conteneur
    /// d'injection de dépendances et résolu par <see cref="App"/> en Phase C de son
    /// constructeur via <c>ServiceProvider.GetRequiredService&lt;MainWindow&gt;()</c>.
    /// <c>Show()</c> est invoqué au Jalon 4a de la séquence de démarrage applicatif
    /// (§3.10), après validation positive de la séquence de démarrage par
    /// <see cref="IU_Application_OnStart"/>.</para>
    /// <para>Objectif : Constituer le shell visuel permanent de l'application,
    /// recevoir ses dépendances par injection, gérer le cycle de vie WPF minimal
    /// (chargement, redimensionnement, déplacement, changement d'état, fermeture),
    /// et déléguer la navigation initiale au UseCase orchestrateur
    /// <see cref="IU_Navigation"/> au moment du <see cref="FrameworkElement.Loaded"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Servir de shell visuel — la composition XAML est portée par
    ///   <c>MainWindow.xaml</c> et limitée au fond commun consommé depuis <c>Shared</c>
    ///   et aux deux frames <c>ActivePage</c> et <c>ActiveHorizontalMenu</c> dont les
    ///   <c>x:Name</c> sont consommés directement par <see cref="DG244Cutting.D_Presentation.Services.SR_Navigation"/>
    ///   (R-4.12.24).</item>
    ///   <item>Recevoir ses six dépendances par injection au constructeur et les
    ///   stocker en champs <c>private readonly</c>, avec gardes
    ///   <see cref="ArgumentNullException"/>.</item>
    ///   <item>Affecter <see cref="FrameworkElement.DataContext"/> à
    ///   <see cref="VM_MainWindow"/> pour alimenter le binding du <c>Title</c> et
    ///   tout binding futur du shell.</item>
    ///   <item>S'abonner aux cinq événements WPF du cycle de vie de la fenêtre
    ///   (<see cref="FrameworkElement.Loaded"/>, <see cref="FrameworkElement.SizeChanged"/>,
    ///   <see cref="Window.StateChanged"/>, <see cref="Window.LocationChanged"/>,
    ///   <see cref="Window.Closed"/>) et garantir qu'aucune exception ne remonte
    ///   au framework WPF par un filet de sécurité ultime au bord de chaque handler.</item>
    ///   <item>Appliquer au <see cref="FrameworkElement.Loaded"/> les dimensions
    ///   minimales lues sur <see cref="ISE_Window"/> et déléguer immédiatement la
    ///   navigation initiale à <see cref="IU_Navigation.NavigateToDefaultAsync"/>
    ///   (R-3.12.7 — le shell délègue la navigation aux UseCases).</item>
    ///   <item>Affecter au <c>BannerHost</c> du shell le composant
    ///   <see cref="Banner"/> injecté en constructeur, au moment du
    ///   <see cref="FrameworkElement.Loaded"/>, au titre de la mise en place de
    ///   l'ancrage visuel statique du shell préalable au déclenchement de la
    ///   navigation initiale.</item>
    ///   <item>Porter le <see cref="CancellationTokenSource"/> local au shell,
    ///   conformément à la doctrine de l'annulation coopérative (§4.6.5 — chaque
    ///   point d'entrée porte son propre CTS) et le libérer au
    ///   <see cref="Window.Closed"/>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Porter de logique applicative propre. Toute règle métier relève des
    ///   UseCases (<c>UC_*</c>) ; toute décision de navigation relève d'
    ///   <see cref="IU_Navigation"/> ; toute logique technique de mutation du
    ///   Setting de fenêtre relève d'<see cref="IS_Window"/>.</item>
    ///   <item>Consulter <see cref="DG244Cutting.A_Domain.Interfaces.Settings.User.ISE_User"/>
    ///   ou évaluer l'état de connexion utilisateur. La résolution de la cible de
    ///   navigation initiale (page de connexion ou page d'accueil) est intégralement
    ///   encapsulée par <see cref="IU_Navigation.NavigateToDefaultAsync"/>.</item>
    ///   <item>Porter de listener temps réel (administration, messages, monitoring
    ///   base de données). Ces écoutes feront l'objet de fils d'Extension ultérieurs.</item>
    ///   <item>Manipuler des contrôles XAML internes des pages applicatives. La
    ///   manipulation des frames est confinée à
    ///   <see cref="DG244Cutting.D_Presentation.Services.SR_Navigation"/>
    ///   (R-4.12.24 — interaction directe avec <see cref="MainWindow"/> réservée à
    ///   <c>SR_Navigation</c>).</item>
    ///   <item>Classifier ou traiter les exceptions applicatives typées
    ///   (<c>Ex_Business</c>, <c>Ex_Infrastructure</c>, <c>Ex_Unclassified</c>).
    ///   Le traitement terminal est délégué à <see cref="IU_LogAndNotify"/> ; la
    ///   classification relève des UseCases en amont (R-4.7.14 amendée).</item>
    /// </list>
    /// <para>Note sur le statut hors familles canoniques : <see cref="MainWindow"/>
    /// n'appartient à aucune des familles canoniques de l'écosystème et ne dispose
    /// donc d'aucun 0232 d'autorité documentaire. Les conventions documentaires et
    /// structurelles appliquées sont calquées sur celles de <see cref="App"/>
    /// (composant singulier hors familles déjà produit) ; la mécanique du filet de
    /// sécurité ultime des handlers WPF s'inspire de
    /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/> sans
    /// héritage formel.</para>
    /// <para>Exception architecturale propre 1 — Filet de sécurité ultime hors
    /// UseCase (EA-NN-FilSecuriteShell). <see cref="MainWindow"/> consomme directement
    /// <see cref="IU_LogAndNotify"/> au titre d'un filet de sécurité de dernier recours
    /// en frontière de WPF, au bord de chacun de ses handlers d'événements. Cette
    /// consommation est habituellement réservée aux UseCases (R-4.7.14) ; elle est
    /// admise nominativement pour <see cref="MainWindow"/> par parallèle avec les
    /// exceptions déjà documentées pour <c>VM_Page_Generic.ExecuteSafeAsync</c> (EA-01)
    /// et pour <see cref="App"/> (filet de sécurité d'amorçage), au titre de §4.7.4
    /// amendée et R-4.7.14 amendée. Périmètre strict : la présente exception se
    /// matérialise à cinq sites au sein du présent composant — un par handler WPF.
    /// La clé dictionnaire utilisée est <c>"No_EC_03"</c> (exception non classifiée
    /// localement) ; le paramètre <c>ct</c> est systématiquement
    /// <see cref="CancellationToken.None"/>, le <c>_cts.Token</c> local pouvant être
    /// en état imprévisible vis-à-vis du moment de défaillance.</para>
    /// <para>Exception architecturale propre 2 — Signature <c>async void</c> du
    /// handler <c>OnLoadedHandler</c> (EA-NN-AsyncVoidOnLoadedHandler). Le handler
    /// <c>OnLoadedHandler</c> adopte la signature <c>async void</c>, habituellement
    /// à proscrire en C# moderne. Cette exception repose sur trois justifications
    /// cumulatives : signature imposée par le contrat
    /// <see cref="RoutedEventHandler"/> du framework WPF, isolation par un
    /// <c>try/catch</c> ultime capturant toute exception (avec bloc
    /// <c>catch (OperationCanceledException)</c> distinct silencieux), et absence
    /// d'appelant nécessitant l'observation de la complétion (le framework WPF ne
    /// consomme pas le <see cref="System.Threading.Tasks.Task"/> qui aurait pu être
    /// retourné). Périmètre strict : la présente exception est cantonnée au seul
    /// handler <c>OnLoadedHandler</c>, le seul handler porteur d'une invocation
    /// asynchrone (<see cref="IU_Navigation.NavigateToDefaultAsync"/>) ; les quatre
    /// autres handlers du composant sont strictement synchrones.</para>
    /// </remarks>
    public partial class MainWindow : Window
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom dynamique de la classe courante, utilisé comme premier maillon de la
        /// <c>callChain</c> construite par chaque handler (§4.5 du 0230).
        /// </summary>
        private readonly string _callee;

        /// <summary>
        /// Source de jeton d'annulation coopérative locale au shell, conformément à
        /// la doctrine §4.6.5 (chaque point d'entrée porte son propre CTS).
        /// </summary>
        /// <remarks>
        /// <para>Le jeton issu de <see cref="CancellationTokenSource.Token"/> est
        /// propagé aux invocations asynchrones et synchrones des dépendances depuis
        /// chaque handler. La libération de cette ressource est portée par
        /// <see cref="OnClosedHandler"/>. Distinct du <c>CancellationTokenSource</c>
        /// du point d'entrée 2 porté par <see cref="App"/>, qui couvre la séquence
        /// d'amorçage applicative.</para>
        /// </remarks>
        private readonly CancellationTokenSource _cts;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// ViewModel singulier du shell, affecté à <see cref="FrameworkElement.DataContext"/>
        /// pour alimenter les bindings WPF du XAML (<c>Title</c>, et tout binding futur).
        /// </summary>
        private readonly VM_MainWindow _vmMainWindow;

        /// <summary>
        /// Composant visuel singulier de bannière principale du shell, affecté à
        /// <c>BannerHost.Content</c> au moment du <see cref="FrameworkElement.Loaded"/>
        /// du shell.
        /// </summary>
        private readonly Banner _banner;

        /// <summary>
        /// Setting transverse de présentation, source des dimensions minimales de la
        /// fenêtre principale (<see cref="ISE_Window.MainWindowMinWidth"/> et
        /// <see cref="ISE_Window.MainWindowMinHeight"/>), appliquées au <see cref="FrameworkElement.Loaded"/>.
        /// </summary>
        private readonly ISE_Window _seWindow;

        /// <summary>
        /// Service technique de présentation relayant les mutations dimensionnelles
        /// (<see cref="IS_Window.ApplySize"/>) et positionnelles
        /// (<see cref="IS_Window.ApplyPosition"/>) vers le Setting transverse.
        /// </summary>
        private readonly IS_Window _sWindow;

        /// <summary>
        /// UseCase orchestrateur de navigation WPF. Consommé au
        /// <see cref="FrameworkElement.Loaded"/> pour déclencher la navigation initiale
        /// via <see cref="IU_Navigation.NavigateToDefaultAsync"/>.
        /// </summary>
        private readonly IU_Navigation _uNavigation;

        /// <summary>
        /// Pipeline terminal de gestion d'erreurs applicatives. Consommé au bord de
        /// chaque handler WPF au titre du filet de sécurité ultime — voir
        /// « Exception architecturale propre 1 » dans la documentation de classe.
        /// </summary>
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du shell applicatif <see cref="MainWindow"/>
        /// avec ses six dépendances injectées par le conteneur DI.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur d'injection de
        /// dépendances lors de la résolution du Singleton, déclenchée par
        /// <see cref="App"/> en Phase C de son propre constructeur via
        /// <c>ServiceProvider.GetRequiredService&lt;MainWindow&gt;()</c>. L'instance
        /// est ensuite consommée par <see cref="App"/> au Jalon 4a de la séquence de
        /// démarrage applicative (§3.10) via l'invocation de <see cref="Window.Show"/>.</para>
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item>Résolution et stockage des six dépendances avec gardes
        ///   <see cref="ArgumentNullException"/>, dans l'ordre fonctionnel
        ///   ViewModel → composant visuel <see cref="Banner"/> → Setting → Service →
        ///   UseCase navigation → UseCase pipeline d'erreurs.</item>
        ///   <item>Initialisation des champs locaux : <c>_callee</c> à partir du
        ///   nom de type (cohérence avec la doctrine CallChain du projet, §4.5),
        ///   <c>_cts</c> par instanciation d'un nouveau
        ///   <see cref="CancellationTokenSource"/> local au shell (§4.6.5).</item>
        ///   <item>Invocation de <see cref="System.Windows.Markup.IComponentConnector.InitializeComponent"/>
        ///   pour la composition XAML — étape impérativement préalable à toute
        ///   affectation de <see cref="FrameworkElement.DataContext"/> ou abonnement
        ///   à un événement WPF du composant.</item>
        ///   <item>Affectation de <see cref="FrameworkElement.DataContext"/> à
        ///   <c>_vmMainWindow</c> pour activer le binding du <c>Title</c> et tout
        ///   binding futur du shell.</item>
        ///   <item>Abonnements aux cinq événements WPF du cycle de vie de la fenêtre :
        ///   <see cref="FrameworkElement.Loaded"/>, <see cref="FrameworkElement.SizeChanged"/>,
        ///   <see cref="Window.StateChanged"/>, <see cref="Window.LocationChanged"/>,
        ///   <see cref="Window.Closed"/>.</item>
        /// </list>
        /// </remarks>
        /// <param name="vmMainWindow">ViewModel singulier du shell, affecté à
        /// <see cref="FrameworkElement.DataContext"/>. Injecté en Singleton par le
        /// conteneur DI.</param>
        /// <param name="banner">Composant visuel singulier de bannière principale du
        /// shell, affecté à <c>BannerHost.Content</c> au <see cref="FrameworkElement.Loaded"/>
        /// du shell. Injecté en Singleton par le conteneur DI.</param>
        /// <param name="seWindow">Setting transverse de présentation, source des
        /// dimensions minimales appliquées au <see cref="FrameworkElement.Loaded"/>.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="sWindow">Service technique relayant les mutations
        /// dimensionnelles et positionnelles vers le Setting transverse. Injecté par
        /// le conteneur DI.</param>
        /// <param name="uNavigation">UseCase orchestrateur de la navigation initiale
        /// au <see cref="FrameworkElement.Loaded"/>. Injecté par le conteneur DI.</param>
        /// <param name="logAndNotify">Pipeline terminal de gestion d'erreurs —
        /// filet de sécurité ultime des handlers WPF. Injecté en Singleton par le
        /// conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si l'une des dépendances
        /// injectées est <see langword="null"/>.</exception>
        public MainWindow(
            VM_MainWindow vmMainWindow,
            Banner banner,
            ISE_Window seWindow,
            IS_Window sWindow,
            IU_Navigation uNavigation,
            IU_LogAndNotify logAndNotify)
        {
            _vmMainWindow = vmMainWindow ?? throw new ArgumentNullException(nameof(vmMainWindow));
            _banner = banner ?? throw new ArgumentNullException(nameof(banner));
            _seWindow = seWindow ?? throw new ArgumentNullException(nameof(seWindow));
            _sWindow = sWindow ?? throw new ArgumentNullException(nameof(sWindow));
            _uNavigation = uNavigation ?? throw new ArgumentNullException(nameof(uNavigation));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));

            _callee = GetType().Name;
            _cts = new CancellationTokenSource();

            InitializeComponent();

            DataContext = _vmMainWindow;

            Loaded += OnLoadedHandler;
            SizeChanged += OnSizeChangedHandler;
            StateChanged += OnStateChangedHandler;
            LocationChanged += OnLocationChangedHandler;
            Closed += OnClosedHandler;
        }

        #endregion

        #region === Méthodes publiques ===

        // Aucune méthode publique exposée par MainWindow hors handlers framework.

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler d'amorçage du shell invoqué par le runtime WPF en réponse à
        /// l'événement <see cref="FrameworkElement.Loaded"/> de la fenêtre principale —
        /// applique les dimensions minimales puis délègue la navigation initiale à
        /// <see cref="IU_Navigation.NavigateToDefaultAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF lors du premier
        /// rendu visuel de la fenêtre, après le <c>Show()</c> exécuté par
        /// <see cref="App"/> au Jalon 4a. Premier handler de cycle de vie WPF
        /// déclenché sur le shell.</para>
        /// <para>Objectif : Construire la <c>callChain</c> locale, appliquer les
        /// dimensions minimales lues sur <see cref="ISE_Window"/> aux propriétés WPF
        /// natives <see cref="FrameworkElement.MinWidth"/> et
        /// <see cref="FrameworkElement.MinHeight"/>, affecter au <c>BannerHost</c> du
        /// shell le composant <see cref="Banner"/> injecté en constructeur, puis
        /// déléguer la navigation initiale au UseCase orchestrateur en lui propageant
        /// le jeton d'annulation coopérative local <c>_cts.Token</c>.</para>
        /// <para>Ordre des invocations : application des minimas, puis affectation
        /// du composant <see cref="Banner"/> au <c>BannerHost</c>, puis déclenchement
        /// de la navigation initiale, conformément à la posture « scaffold first »
        /// unifiée — les minimas et l'ancrage statique de la bannière s'inscrivent
        /// dans le même bloc de mise en place du shell, qui précède le déclenchement
        /// de la navigation initiale conditionnant le rendu des pages applicatives
        /// chargées par <see cref="IU_Navigation.NavigateToDefaultAsync"/>.</para>
        /// <para>Filet de sécurité ultime : Le bloc <c>try/catch</c> englobant
        /// garantit qu'aucune exception ne remonte au framework WPF — voir
        /// « Exception architecturale propre 1 » dans la documentation de classe.
        /// Un bloc <c>catch (OperationCanceledException)</c> distinct silencieux
        /// précède le <c>catch (Exception)</c> global : une annulation observée à
        /// ce stade procède d'un signal amont légitime de fermeture porté par
        /// <see cref="App"/> via son propre <see cref="CancellationTokenSource"/>,
        /// et ne doit pas être journalisée. Le <c>catch (Exception)</c> global
        /// délègue à <see cref="IU_LogAndNotify"/> avec la clé canonique
        /// <c>"No_EC_03"</c> (exception non classifiée localement) et
        /// <c>notify: true</c> — la défaillance au premier rendu utilisateur justifie
        /// une notification opérateur.</para>
        /// <para>CancellationToken du filet de sécurité : Le paramètre <c>ct</c>
        /// passé à <see cref="IU_LogAndNotify.ExecuteAsync"/> est
        /// <see cref="CancellationToken.None"/> et non <c>_cts.Token</c> — le
        /// <c>_cts.Token</c> peut être en état imprévisible vis-à-vis du moment de
        /// défaillance, et <see cref="CancellationToken.None"/> garantit que la
        /// journalisation ne sera pas interrompue par un signal d'annulation
        /// concomitant. Posture parallèle à <c>App.OnResolveAssembly</c>.</para>
        /// <para>Signature <c>async void</c> : Imposée par le contrat du
        /// <see cref="RoutedEventHandler"/> du framework WPF — voir « Exception
        /// architecturale propre 2 » dans la documentation de classe.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance courante de
        /// <see cref="MainWindow"/>).</param>
        /// <param name="e">Arguments de l'événement <see cref="FrameworkElement.Loaded"/>
        /// — non consommés par le handler.</param>
        private async void OnLoadedHandler(object sender, RoutedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnLoadedHandler)}";

            try
            {
                MinWidth = _seWindow.MainWindowMinWidth;
                MinHeight = _seWindow.MainWindowMinHeight;

                BannerHost.Content = _banner;

                await _uNavigation.NavigateToDefaultAsync(callChain, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Annulation amont légitime (App pilote la fermeture via son propre CTS).
                // Aucune journalisation ni notification — Q-CB-4 (A).
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: true,
                    ct: CancellationToken.None);
            }
        }

        /// <summary>
        /// Handler synchrone invoqué par le runtime WPF en réponse à l'événement
        /// <see cref="FrameworkElement.SizeChanged"/> de la fenêtre principale —
        /// relaie la nouvelle taille mesurée au Setting transverse via
        /// <see cref="IS_Window.ApplySize"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF à chaque
        /// redimensionnement effectif de la fenêtre principale — événement à haute
        /// fréquence pendant un drag utilisateur (plusieurs déclenchements par
        /// seconde). Les dimensions reçues sont passées en pixels entiers
        /// (conversion explicite <c>(int)</c>) au format attendu par
        /// <see cref="ISE_Window.MainWindowWidth"/> et
        /// <see cref="ISE_Window.MainWindowHeight"/>.</para>
        /// <para>Objectif : Construire la <c>callChain</c> locale et déléguer la
        /// nouvelle taille au service technique <see cref="IS_Window"/>, qui
        /// calcule la marge ajustée et écrit atomiquement les trois propriétés
        /// liées via <see cref="ISE_Window.UpdateWindowDimensions"/>. La propagation
        /// INPC vers les ViewModels consommateurs est portée par le Setting.</para>
        /// <para>Filet de sécurité ultime : Le bloc <c>try/catch</c> englobant
        /// garantit qu'aucune exception ne remonte au framework WPF — voir
        /// « Exception architecturale propre 1 » dans la documentation de classe.
        /// La délégation à <see cref="IU_LogAndNotify"/> est faite en
        /// fire-and-forget (handler synchrone, le retour de
        /// <c>ExecuteAsync</c> est volontairement ignoré via la décharge <c>_</c>).
        /// <c>notify: false</c> — l'événement étant à haute fréquence, une
        /// défaillance répétée submergerait inutilement l'opérateur de notifications.
        /// Aucun bloc <c>catch (OperationCanceledException)</c> distinct : ce handler
        /// invoque une opération atomique d'<see cref="IS_Window"/>, qui n'est pas
        /// censée s'annuler en cours d'exécution ; une éventuelle
        /// <see cref="OperationCanceledException"/> sera donc journalisée comme une
        /// défaillance non classifiée.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance courante de
        /// <see cref="MainWindow"/>).</param>
        /// <param name="e">Arguments de l'événement portant les anciennes et nouvelles
        /// dimensions ; seule <see cref="SizeChangedEventArgs.NewSize"/> est consommée
        /// par le handler.</param>
        private void OnSizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnSizeChangedHandler)}";

            try
            {
                _sWindow.ApplySize(
                    callChain,
                    (int)e.NewSize.Width,
                    (int)e.NewSize.Height,
                    _cts.Token);
            }
            catch (Exception ex)
            {
                _ = _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: false,
                    ct: CancellationToken.None);
            }
        }

        /// <summary>
        /// Handler synchrone invoqué par le runtime WPF en réponse à l'événement
        /// <see cref="Window.StateChanged"/> de la fenêtre principale — déclencheur
        /// composé invoquant successivement <see cref="IS_Window.ApplySize"/> et
        /// <see cref="IS_Window.ApplyPosition"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF à chaque transition
        /// d'état de la fenêtre principale (passage entre <see cref="WindowState.Normal"/>,
        /// <see cref="WindowState.Maximized"/>, <see cref="WindowState.Minimized"/>).
        /// Une telle transition altère simultanément les dimensions et la position
        /// effectives de la fenêtre, mais ne déclenche pas systématiquement les
        /// événements <see cref="FrameworkElement.SizeChanged"/> et
        /// <see cref="Window.LocationChanged"/> dans tous les scénarios — d'où la
        /// nécessité d'un déclencheur composé.</para>
        /// <para>Objectif : Resynchroniser le Setting transverse avec l'état effectif
        /// de la fenêtre après transition. Les dimensions sont lues depuis
        /// <see cref="FrameworkElement.ActualWidth"/> et
        /// <see cref="FrameworkElement.ActualHeight"/> (et non depuis
        /// <see cref="Window.Width"/>/<see cref="Window.Height"/>, qui ne reflètent
        /// pas les dimensions réelles en mode <see cref="WindowState.Maximized"/>).</para>
        /// <para>Filet de sécurité ultime : Identique à
        /// <see cref="OnSizeChangedHandler"/> — fire-and-forget,
        /// <c>notify: false</c>, clé canonique <c>"No_EC_03"</c>,
        /// <see cref="CancellationToken.None"/>.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance courante de
        /// <see cref="MainWindow"/>).</param>
        /// <param name="e">Arguments de l'événement <see cref="Window.StateChanged"/>
        /// — non consommés par le handler ; l'état courant est lu directement
        /// depuis les propriétés WPF de la fenêtre.</param>
        private void OnStateChangedHandler(object? sender, EventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnStateChangedHandler)}";

            try
            {
                _sWindow.ApplySize(
                    callChain,
                    (int)ActualWidth,
                    (int)ActualHeight,
                    _cts.Token);

                _sWindow.ApplyPosition(
                    callChain,
                    Top,
                    Left,
                    _cts.Token);
            }
            catch (Exception ex)
            {
                _ = _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: false,
                    ct: CancellationToken.None);
            }
        }

        /// <summary>
        /// Handler synchrone invoqué par le runtime WPF en réponse à l'événement
        /// <see cref="Window.LocationChanged"/> de la fenêtre principale — relaie
        /// la nouvelle position au Setting transverse via
        /// <see cref="IS_Window.ApplyPosition"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF à chaque
        /// déplacement effectif de la fenêtre principale — événement à fréquence
        /// modérée pendant un drag utilisateur. La position est lue directement
        /// depuis <see cref="Window.Top"/> et <see cref="Window.Left"/>, exprimées
        /// en pixels au format <see cref="double"/> attendu par
        /// <see cref="ISE_Window.MainWindowTop"/> et <see cref="ISE_Window.MainWindowLeft"/>.</para>
        /// <para>Objectif : Construire la <c>callChain</c> locale et déléguer la
        /// nouvelle position au service technique <see cref="IS_Window"/>, qui
        /// écrit atomiquement les deux propriétés liées via
        /// <see cref="ISE_Window.UpdateWindowPosition"/>.</para>
        /// <para>Filet de sécurité ultime : Identique à
        /// <see cref="OnSizeChangedHandler"/> — fire-and-forget,
        /// <c>notify: false</c>, clé canonique <c>"No_EC_03"</c>,
        /// <see cref="CancellationToken.None"/>.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance courante de
        /// <see cref="MainWindow"/>).</param>
        /// <param name="e">Arguments de l'événement <see cref="Window.LocationChanged"/>
        /// — non consommés par le handler ; la position courante est lue directement
        /// depuis les propriétés WPF de la fenêtre.</param>
        private void OnLocationChangedHandler(object? sender, EventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnLocationChangedHandler)}";

            try
            {
                _sWindow.ApplyPosition(
                    callChain,
                    Top,
                    Left,
                    _cts.Token);
            }
            catch (Exception ex)
            {
                _ = _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: false,
                    ct: CancellationToken.None);
            }
        }

        /// <summary>
        /// Handler synchrone invoqué par le runtime WPF en réponse à l'événement
        /// <see cref="Window.Closed"/> de la fenêtre principale — annule et libère
        /// le <see cref="CancellationTokenSource"/> local au shell.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF au terme du cycle
        /// de vie de la fenêtre principale, postérieurement à
        /// <see cref="Window.Closing"/> (non traité par le shell) et préalablement
        /// à <see cref="System.Windows.Application.Exit"/> (traité par
        /// <see cref="App"/>). Dernier handler de cycle de vie WPF déclenché sur
        /// le shell.</para>
        /// <para>Objectif : Signaler l'annulation aux opérations asynchrones
        /// éventuellement encore en cours et libérer les ressources du
        /// <see cref="CancellationTokenSource"/> local, conformément à la doctrine
        /// d'annulation coopérative §4.6.5 (chaque point d'entrée porte son propre
        /// CTS et en assure la libération).</para>
        /// <para>Filet de sécurité silencieux : Le bloc <c>try/catch</c> englobant
        /// capte toute défaillance éventuelle des opérations
        /// <see cref="CancellationTokenSource.Cancel()"/> ou
        /// <see cref="CancellationTokenSource.Dispose()"/> sans journalisation ni
        /// notification — le shell se ferme, aucune notification opérateur n'est
        /// utile à ce stade, et toute remontée à WPF serait sans effet le composant
        /// étant en fin de vie. La doctrine du filet de sécurité ultime est ici
        /// respectée dans son intention (aucune exception ne remonte au framework)
        /// sans recours au pipeline complet <see cref="IU_LogAndNotify"/>.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance courante de
        /// <see cref="MainWindow"/>).</param>
        /// <param name="e">Arguments de l'événement <see cref="Window.Closed"/> —
        /// non consommés par le handler.</param>
        private void OnClosedHandler(object? sender, EventArgs e)
        {
            try
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            catch
            {
                // Try/catch silencieux : le shell se ferme, aucune notification utile (Q-CB-1 (A)).
            }
        }

        #endregion
    }
}