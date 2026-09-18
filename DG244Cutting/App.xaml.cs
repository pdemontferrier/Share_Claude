using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.Views.Shell;
using DG244Cutting.E_Miscellaneous.CompositionRoot;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace DG244Cutting
{
    /// <summary>
    /// Point d'amorçage WPF de l'application DG244Cutting, site d'initialisation
    /// de la convention de plateforme <c>App.ServiceProvider</c> (§4.15.10) et
    /// orchestrateur de la séquence de démarrage applicatif à quatre jalons.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant hors familles canoniques (UC, SR, CR, CH, QH,
    /// SE, RS, DTO, VM, MH, Page), instancié par le runtime WPF au lancement de
    /// l'application. Tête du processus DG244Cutting, point d'entrée du cycle de
    /// vie <see cref="Application"/>.</para>
    /// <para>Objectif : Orchestrer de manière ordonnée la séquence de démarrage
    /// applicatif à quatre jalons décrite en §3.10 du référentiel : Jalon 1 —
    /// construction du conteneur d'injection de dépendances ; Jalon 2 — traitement
    /// des arguments de ligne de commande ; Jalon 3 — exécution de la séquence
    /// de validation applicative portée par <see cref="IU_Application_OnStart"/>,
    /// invoquée par médiation de <see cref="IS_UseCaseInvoker"/> conformément à
    /// §4.10.10 ; Jalon 4 — arbitrage entre ouverture de la fenêtre principale et
    /// fermeture de l'application. Porter la convention de plateforme §4.15.10 en
    /// publiant la propriété statique publique <see cref="ServiceProvider"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Construire le conteneur d'injection de dépendances via
    ///   <c>SR_ConteneurDI.ConfigureServices()</c>.</item>
    ///   <item>Publier le conteneur via la propriété statique publique
    ///   <see cref="ServiceProvider"/> conformément à la convention §4.15.10.</item>
    ///   <item>S'abonner aux événements d'application <c>Startup</c> et <c>Exit</c>
    ///   ainsi qu'à l'événement <c>AssemblyResolve</c> du domaine d'application.</item>
    ///   <item>Invoquer le UseCase orchestrateur <see cref="IU_Application_OnStart"/>
    ///   au Jalon 3 par médiation de <see cref="IS_UseCaseInvoker"/>, conformément
    ///   à §4.10.10. Cette médiation matérialise la doctrine P4-bis : <see cref="App"/>
    ///   est de fait Singleton (instance unique du runtime WPF) tandis que
    ///   <see cref="IU_Application_OnStart"/> est Scoped (UseCase consommateur du
    ///   DbContext partagé) ; l'invocation transite par <see cref="IS_UseCaseInvoker"/>
    ///   qui crée un <c>IServiceScope</c> distinct à chaque appel, prévenant ainsi
    ///   la captive dependency Singleton → Scoped.</item>
    ///   <item>Porter le <see cref="CancellationTokenSource"/> du point d'entrée 2
    ///   du jeton d'annulation coopérative (§4.6.5).</item>
    ///   <item>Exposer le filet de sécurité d'amorçage à deux sites
    ///   (<c>Application_Startup</c> et <c>OnResolveAssembly</c>) au titre de
    ///   §4.7.4 amendée.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Porter de logique métier. Toute règle métier relève des UseCases
    ///   (<c>UC_*</c>) et des Services applicatifs ; <see cref="App"/> n'en porte
    ///   aucune.</item>
    ///   <item>Classifier ou traiter les exceptions applicatives typées
    ///   (<c>Ex_Business</c>, <c>Ex_Infrastructure</c>, <c>Ex_Unclassified</c>).
    ///   Le traitement terminal est délégué à <see cref="IU_LogAndNotify"/> ; la
    ///   classification relève des UseCases en amont.</item>
    ///   <item>Décider de la navigation. La navigation est portée par les UseCases
    ///   conformément à §4.11.2.</item>
    ///   <item>Charger les libellés ou gérer la traduction. Le chargement de la
    ///   langue applicative est délégué à <see cref="IU_Application_OnStart"/> au
    ///   Jalon 3.</item>
    ///   <item>Porter une exception architecturale propre au titre de la médiation
    ///   par <see cref="IS_UseCaseInvoker"/>. La consommation médiatisée du UseCase
    ///   orchestrateur de démarrage est une application standard du pattern §4.10.10
    ///   (dont la description faisant autorité de l'EA-11 est portée par
    ///   <see cref="IS_UseCaseInvoker"/> / <c>SR_UseCaseInvoker</c>), motivée par
    ///   la doctrine P4-bis applicable à tout composant Singleton consommateur
    ///   d'un Scoped. Elle ne déroge à aucune règle générale du référentiel et
    ///   n'introduit donc aucune EA propre.</item>
    /// </list>
    /// <para>Dérogation au modèle à cinq régions : <see cref="App"/> bénéficie
    /// d'une extension à six régions par admission nominative (§4.4.3 amendée et
    /// R-4.4.8 amendée). Une région supplémentaire <c>=== Propriétés publiques ===</c>
    /// est positionnée entre <c>=== Dépendances privées ===</c> et
    /// <c>=== Constructeur ===</c>, par analogie avec l'extension déjà admise pour
    /// les familles VM_* et SE_*. La consommation par <see cref="App"/> de sa propre
    /// propriété <see cref="ServiceProvider"/> depuis son propre constructeur est
    /// par ailleurs hors du périmètre d'application de la convention §4.15.10
    /// (§4.15.10 amendée).</para>
    /// <para>Exception architecturale propre 1 — Filet de sécurité d'amorçage
    /// (EA-NN-FilSecuriteAmorcage). <see cref="App"/> consomme directement
    /// <see cref="IU_LogAndNotify"/> au titre d'un filet de sécurité de dernier
    /// recours en frontière de WPF. Cette consommation est habituellement réservée
    /// aux UseCases (R-4.7.14) ; elle est admise nominativement pour
    /// <see cref="App"/> par parallèle avec l'exception déjà documentée pour
    /// <c>VM_Page_Generic.ExecuteSafeAsync</c> (EA-01), au titre de §4.7.4 amendée
    /// et R-4.7.14 amendée. Périmètre strict : le filet de sécurité d'amorçage se
    /// matérialise à deux sites au sein du présent composant —
    /// <c>Application_Startup</c> (filet englobant la séquence applicative) et
    /// <c>OnResolveAssembly</c> (filet local du handler runtime, §3.10.9 amendée).
    /// Les clés dictionnaire utilisées sont celles canoniques de la chaîne de
    /// gestion d'erreur (<c>"No_EC_03"</c> pour les exceptions non typées) sans
    /// spécialisation.</para>
    /// <para>Exception architecturale propre 2 — Signature <c>async void</c> du
    /// handler <c>Application_Startup</c> (EA-NN-AsyncVoidApplicationStartup).
    /// Le handler <c>Application_Startup</c> adopte la signature <c>async void</c>,
    /// habituellement à proscrire en C# moderne. Cette exception repose sur trois
    /// justifications cumulatives : signature imposée par le contrat
    /// <see cref="System.Windows.StartupEventHandler"/> du framework WPF, isolation
    /// par un <c>try/catch</c> ultime capturant toute exception (avec bloc
    /// <c>catch (OperationCanceledException)</c> distinct produisant un arrêt propre
    /// par <see cref="Application.Shutdown"/>), et absence d'appelant nécessitant
    /// l'observation de la complétion (le framework WPF ne consomme pas le
    /// <see cref="System.Threading.Tasks.Task"/> qui aurait pu être retourné).
    /// Périmètre strict : la présente exception est cantonnée au seul handler
    /// <c>Application_Startup</c> et ne s'étend ni à <c>Application_Exit</c>
    /// (synchrone), ni à <c>OnResolveAssembly</c> (synchrone), ni à toute autre
    /// méthode du présent composant.</para>
    /// </remarks>
    public partial class App : Application
    {
        #region === Propriétés privées ===

        private readonly string _callee;
        private readonly CancellationTokenSource _cts;

        #endregion

        #region === Dépendances privées ===

        private readonly IU_LogAndNotify _logAndNotify;
        private readonly IS_UseCaseInvoker _useCaseInvoker;
        private readonly ISE_App _seApp;
        private readonly ISE_User _seUser;

        #endregion

        #region === Propriétés publiques ===

        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du composant <see cref="App"/> au lancement
        /// du runtime WPF.
        /// </summary>
        /// <remarks>
        /// <para>Phase B — Initialisation du conteneur DI : construction du conteneur
        /// d'injection de dépendances par appel à
        /// <c>SR_ConteneurDI.ConfigureServices()</c> et publication immédiate via la
        /// propriété statique publique <see cref="ServiceProvider"/> conformément à
        /// la convention §4.15.10.</para>
        /// <para>Phase C — Résolution des dépendances internes : résolution des quatre
        /// dépendances consommées par le composant (<see cref="IU_LogAndNotify"/>,
        /// <see cref="IS_UseCaseInvoker"/>, <see cref="ISE_App"/>,
        /// <see cref="ISE_User"/>) par appel direct à
        /// <c>ServiceProvider.GetRequiredService&lt;T&gt;()</c>, dans l'ordre
        /// fonctionnel services techniques → settings. L'absence d'une
        /// dépendance directe à <see cref="IU_Application_OnStart"/> au constructeur
        /// est intentionnelle : le UseCase orchestrateur de démarrage est consommé
        /// au Jalon 3 par médiation de <see cref="IS_UseCaseInvoker"/>, conformément
        /// à §4.10.10 et à la doctrine P4-bis (cf. responsabilités de la classe).</para>
        /// <para>Phase D — Initialisations locales : initialisation du champ
        /// <c>_callee</c> à partir du nom de type et instanciation du
        /// <see cref="CancellationTokenSource"/> du point d'entrée 2 du jeton
        /// d'annulation coopérative (§4.6.5).</para>
        /// <para>Phase A — Abonnements aux événements du domaine d'application :
        /// abonnement à <see cref="AppDomain.AssemblyResolve"/> du domaine courant.
        /// Les abonnements aux événements <c>Startup</c> et <c>Exit</c> du composant
        /// <see cref="Application"/> sont déclarés dans le markup <c>App.xaml</c>.</para>
        /// </remarks>
        public App()
        {
            // Phase B — Initialisation du conteneur DI
            ServiceProvider = SR_ConteneurDI.ConfigureServices();

            // Phase C — Résolution des dépendances internes (ordre fonctionnel β1)
            _logAndNotify = ServiceProvider.GetRequiredService<IU_LogAndNotify>();
            _useCaseInvoker = ServiceProvider.GetRequiredService<IS_UseCaseInvoker>();
            _seApp = ServiceProvider.GetRequiredService<ISE_App>();
            _seUser = ServiceProvider.GetRequiredService<ISE_User>();

            // Phase D — Initialisations locales
            _callee = GetType().Name;
            _cts = new CancellationTokenSource();

            // Phase A — Abonnements aux événements du domaine d'application
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
        }

        #endregion

        #region === Méthodes publiques ===

        // Aucune méthode publique exposée par App hors handlers framework.

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler d'amorçage du runtime WPF orchestrant la séquence de démarrage
        /// applicatif et l'arbitrage final entre ouverture de la fenêtre principale
        /// et fermeture de l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF en réponse à
        /// l'événement <see cref="Application.Startup"/>, conformément à l'abonnement
        /// déclaré dans le markup <c>App.xaml</c>. Premier code applicatif exécuté
        /// après le constructeur de <see cref="App"/>.</para>
        /// <para>Objectif : Construire la <c>callChain</c> du point d'entrée, traiter
        /// les arguments de ligne de commande (Jalon 2), exécuter la séquence de
        /// validation applicative portée par <see cref="IU_Application_OnStart"/>
        /// par médiation de <see cref="IS_UseCaseInvoker"/> conformément à §4.10.10
        /// (Jalon 3), et arbitrer le verdict de démarrage (Jalon 4) entre ouverture
        /// de <see cref="MainWindow"/> et <see cref="Application.Shutdown"/>.</para>
        /// <para>Filet de sécurité d'amorçage : Le présent handler porte le filet
        /// de sécurité de dernier recours en frontière de WPF — voir « Exception
        /// architecturale propre 1 » dans la documentation de classe (§4.7.4 amendée).
        /// Toute exception non typée est captée terminalement et déléguée à
        /// <see cref="IU_LogAndNotify"/> avec clé canonique <c>"No_EC_03"</c> ; un
        /// fallback natif <see cref="MessageBox"/> de second niveau couvre l'éventuelle
        /// défaillance du pipeline standard (§3.10.4). L'application se ferme dans
        /// tous les cas par <see cref="Application.Shutdown"/>.</para>
        /// <para>Annulation coopérative : Le handler propage le jeton <c>_cts.Token</c>
        /// du <see cref="CancellationTokenSource"/> du composant à
        /// <c>LaunchApplicationAsync</c>. Une <see cref="OperationCanceledException"/>
        /// observée localement produit un arrêt propre par
        /// <see cref="Application.Shutdown"/> sans classification, sans log ni
        /// notification (§4.6.6 amendée).</para>
        /// <para>Signature <c>async void</c> : imposée par le contrat du
        /// <see cref="System.Windows.StartupEventHandler"/> du framework WPF — voir
        /// « Exception architecturale propre 2 » dans la documentation de classe.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (le composant <see cref="App"/>
        /// lui-même).</param>
        /// <param name="e">Arguments de démarrage WPF, dont le tableau d'arguments
        /// de ligne de commande.</param>
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(Application_Startup)}";

            string cultureCode = CultureInfo.CurrentCulture.Name;

            try
            {
                ProcessStartupArguments(callChain, e.Args);

                bool started = await LaunchApplicationAsync(callChain, cultureCode, _cts.Token);

                if (started)
                {
                    // Résolution différée : la grappe de présentation
                    // (MainWindow -> Banner -> VM_Banner et son polling) n'est construite
                    // qu'une fois le démarrage validé, jamais en cas de refus.
                    var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                }
                else
                {
                    Current.Shutdown();
                }
            }
            catch (OperationCanceledException)
            {
                Current.Shutdown();
            }
            catch (Exception ex)
            {
                try
                {
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: _cts.Token);
                }
                catch
                {
                    MessageBox.Show(
                        ex.Message,
                        "Application startup error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

                Current.Shutdown();
            }
        }

        /// <summary>
        /// Handler de fin de vie du runtime WPF annulant et libérant le
        /// <see cref="CancellationTokenSource"/> du composant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF en réponse à
        /// l'événement <see cref="Application.Exit"/>, conformément à l'abonnement
        /// déclaré dans le markup <c>App.xaml</c>. Dernier code applicatif exécuté
        /// avant l'arrêt du processus.</para>
        /// <para>Objectif : Signaler l'annulation aux opérations asynchrones
        /// éventuellement encore en cours et libérer les ressources du
        /// <see cref="CancellationTokenSource"/> du point d'entrée 2 du jeton
        /// d'annulation coopérative (§4.6.5 amendée).</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (le composant <see cref="App"/>
        /// lui-même).</param>
        /// <param name="e">Arguments de fin de vie WPF.</param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        /// <summary>
        /// Handler de résolution dynamique d'assembly invoqué par le runtime CLR
        /// lorsque l'environnement échoue à localiser une assembly référencée par
        /// l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime CLR en réponse à
        /// l'événement <see cref="AppDomain.AssemblyResolve"/>, conformément à
        /// l'abonnement déclaré en Phase A du constructeur de <see cref="App"/>.
        /// Exécution hors séquence applicative, sans <c>caller</c> reçu en paramètre.</para>
        /// <para>Objectif : Tenter de localiser l'assembly demandée dans le
        /// répertoire des ressources communes désigné par
        /// <see cref="ISE_App.SharedProjectPath"/> et la charger si elle est
        /// présente, ou retourner <see langword="null"/> sinon, conformément au
        /// contrat de l'événement <see cref="AppDomain.AssemblyResolve"/>.</para>
        /// <para>Filet de sécurité local : Toute exception non maîtrisée survenant
        /// pendant la résolution est captée terminalement et journalisée en
        /// <c>fire-and-forget</c> via <see cref="IU_LogAndNotify"/> avec clé
        /// canonique <c>"No_EC_03"</c>, sans notification opérateur. Ce filet
        /// local matérialise le mécanisme de dernier recours pour les phases les
        /// plus précoces (§3.10.9 amendée) à un second site du composant,
        /// conformément au régime des handlers runtime portés par
        /// <see cref="App"/> au titre du filet de sécurité d'amorçage — voir
        /// « Exception architecturale propre 1 » dans la documentation de classe
        /// (§4.7.4 amendée).</para>
        /// <para>CallChain locale : Le handler construit sa <c>callChain</c>
        /// localement selon la forme <c>$"{_callee} > {nameof(OnResolveAssembly)}"</c>
        /// au titre du quatrième point d'entrée canonique de la <c>callChain</c>
        /// (callChain d'origine handler runtime, §4.5 amendée).</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (le <see cref="AppDomain"/>
        /// courant).</param>
        /// <param name="args">Arguments de résolution d'assembly, dont
        /// <see cref="ResolveEventArgs.Name"/> portant le nom qualifié de l'assembly
        /// demandée.</param>
        /// <returns>L'assembly chargée depuis le répertoire des ressources communes
        /// si elle y est trouvée ; sinon <see langword="null"/>, laissant le runtime
        /// poursuivre sa propre chaîne de résolution ou échouer définitivement selon
        /// le contrat de l'événement.</returns>
        private Assembly? OnResolveAssembly(object? sender, ResolveEventArgs args)
        {
            try
            {
                string assemblyPath = Path.Combine(
                    _seApp.SharedProjectPath.LocalPath,
                    new AssemblyName(args.Name).Name + ".dll");

                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }

                return null;
            }
            catch (Exception ex)
            {
                // Filet de sécurité local — §3.10.9 amendée. Prolongation de l'Exception
                // architecturale propre 1 à un second site du composant : le handler est
                // invoqué par le runtime CLR hors séquence applicative et doit toujours
                // retourner sans propager.
                //
                // notify: false (R-4.7.17) — la résolution d'assembly s'exécute hors
                // contexte applicatif standard ; une notification opérateur serait
                // techniquement incertaine (chaîne UI possiblement non initialisée) et
                // fonctionnellement inappropriée à ce stade.
                //
                // ct: CancellationToken.None — le jeton _cts.Token du composant peut
                // être en état imprévisible vis-à-vis du moment d'invocation par le
                // runtime CLR ; CancellationToken.None garantit que la journalisation
                // ne sera pas interrompue par un signal d'annulation concomitant.
                //
                // Fire-and-forget (R-3.6.2, §4.7.4 amendée) — aucun blocage du handler
                // runtime synchrone, principe best-effort de la chaîne de gestion
                // d'erreur.
                string callChain = $"{_callee} > {nameof(OnResolveAssembly)}";
                _ = _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: false,
                    ct: CancellationToken.None);

                return null;
            }
        }

        /// <summary>
        /// Analyse le tableau d'arguments de ligne de commande reçu au démarrage
        /// et initialise les settings concernés.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode privée appelée par <c>Application_Startup</c>
        /// avant l'exécution du Jalon 3 de la séquence de démarrage. Constitue le
        /// Jalon 2 de la séquence à quatre jalons (§3.10).</para>
        /// <para>Objectif : Traiter les arguments reconnus par <see cref="App"/>
        /// (à ce jour : <c>iduser=&lt;entier&gt;</c>, pré-renseignant
        /// <see cref="ISE_User.AppUserId"/>) et ignorer silencieusement les
        /// arguments non reconnus, conformément au principe de robustesse en
        /// frontière de runtime.</para>
        /// <para>Comportement en l'absence d'arguments : Retour immédiat sans
        /// effet si <paramref name="args"/> est <see langword="null"/> ou vide.
        /// Ce comportement correspond au cas nominal d'un démarrage sans paramètre
        /// de ligne de commande.</para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel amont reçue de
        /// <c>Application_Startup</c>.</param>
        /// <param name="args">Tableau d'arguments de ligne de commande à analyser,
        /// tel que transmis par <see cref="StartupEventArgs.Args"/>.</param>
        private void ProcessStartupArguments(string caller, string[] args)
        {
            string callChain = $"{caller} > {nameof(ProcessStartupArguments)}";

            if (args == null || args.Length == 0)
                return;

            foreach (string arg in args)
            {
                if (arg.StartsWith("iduser=", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(arg.Substring("iduser=".Length), out int userId))
                    {
                        _seUser.AppUserId = userId;
                    }
                }
            }
        }

        /// <summary>
        /// Délègue la séquence de validation applicative au UseCase orchestrateur
        /// <see cref="IU_Application_OnStart"/> par médiation de
        /// <see cref="IS_UseCaseInvoker"/> et propage son verdict booléen à
        /// l'appelant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode privée appelée par <c>Application_Startup</c>
        /// après traitement des arguments de ligne de commande (Jalon 2 achevé).
        /// Sa finalité est strictement orchestrale : elle ne porte aucune règle
        /// de validation et signale terminalement le verdict de démarrage au
        /// handler d'amorçage.</para>
        /// <para>Objectif : Exécuter le Jalon 3 de la séquence de démarrage
        /// applicatif (§3.10) en invoquant
        /// <see cref="IU_Application_OnStart.ExecuteAsync(string, string, CancellationToken)"/>
        /// par l'intermédiaire de
        /// <see cref="IS_UseCaseInvoker.InvokeAsync{TUseCase, TResult}(System.Func{TUseCase, CancellationToken, System.Threading.Tasks.Task{TResult}}, CancellationToken)"/>,
        /// avec le code culture courant et le jeton d'annulation coopérative
        /// transmis depuis le point d'entrée 2.</para>
        /// <para>Médiation <see cref="IS_UseCaseInvoker"/> : Conformément à §4.10.10,
        /// l'invocation du UseCase orchestrateur transite par
        /// <see cref="IS_UseCaseInvoker"/>, qui crée à chaque appel un
        /// <c>IServiceScope</c> distinct, y résout <see cref="IU_Application_OnStart"/>
        /// (composant Scoped), exécute le délégué fourni, puis dispose le scope à
        /// la sortie. Cette médiation est l'application standard du pattern §4.10.10
        /// au cas d'<see cref="App"/> et matérialise la doctrine P4-bis : la résolution
        /// directe du UseCase au constructeur de <see cref="App"/> — composant de
        /// fait Singleton — aurait capturé une instance Scoped résolue depuis le
        /// scope racine du conteneur, neutralisant la mécanique de séparation des
        /// contextes par invocation décrite en §3.8 et §4.10.10. La présente
        /// médiation ne constitue pas une exception architecturale propre : elle
        /// n'introduit aucune dérogation aux règles générales du référentiel
        /// (cf. Non-responsabilités dans la documentation de classe).</para>
        /// <para>Annulation coopérative : Le paramètre <paramref name="ct"/> est
        /// propagé sans requalification au délégué d'invocation, puis au UseCase
        /// orchestrateur via le <c>innerCt</c> du contrat
        /// <see cref="IS_UseCaseInvoker"/>. Une <see cref="OperationCanceledException"/>
        /// est remontée à l'appelant sans capture locale (R-4.6.13).</para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel amont reçue de
        /// <c>Application_Startup</c>.</param>
        /// <param name="cultureCode">Code culture courant au format BCP 47
        /// (ex. <c>"fr-FR"</c>).</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé sans
        /// requalification au UseCase orchestrateur via
        /// <see cref="IS_UseCaseInvoker"/>.</param>
        /// <returns><see langword="true"/> si la séquence de validation applicative
        /// est validée dans son intégralité (Jalon 4a — ouverture de
        /// <see cref="MainWindow"/>) ; <see langword="false"/> si l'une des étapes
        /// aboutit à un refus de démarrage (Jalon 4b —
        /// <see cref="Application.Shutdown"/>).</returns>
        private async Task<bool> LaunchApplicationAsync(
            string caller,
            string cultureCode,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {nameof(LaunchApplicationAsync)}";

            return await _useCaseInvoker.InvokeAsync<IU_Application_OnStart, bool>(
                (useCase, innerCt) => useCase.ExecuteAsync(callChain, cultureCode, innerCt),
                ct);
        }

        #endregion
    }
}