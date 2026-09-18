using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic;
using BatchStockRelease.E_Miscellaneous.CompositionRoot;

namespace BatchStockRelease
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Point d’entrée principal de l’application WPF.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Cette classe initialise le conteneur de dépendances, résout les services essentiels
    /// et orchestre la séquence de démarrage via le UseCase <see cref="IU_Application_OnStart"/>.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Garantir que l’environnement applicatif et les dépendances sont prêts avant
    /// le lancement de la fenêtre principale.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Configurer le conteneur d’injection de dépendances (DI).</description></item>
    /// <item><description>Traiter les arguments passés à l’application.</description></item>
    /// <item><description>Exécuter le UseCase <see cref="IU_Application_OnStart"/>.</description></item>
    /// <item><description>Lancer ou fermer l’application selon le résultat du démarrage.</description></item>
    /// </list>
    /// </summary>
    public partial class App : Application
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe pour la traçabilité.
        /// </summary>
        private readonly string _callee;
        #endregion

        #region === Dépendances privées ===

        private readonly ServiceProvider serviceProvider;
        public static IServiceProvider _serviceProvider { get; private set; } = null!;
        private readonly IU_Application_OnStart _ucOnStart;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise l’application et configure les services nécessaires.
        /// </summary>
        public App()
        {
            _callee = GetType().Name;

            // Ajouter les DLL des ressources communes
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

            // Configuration du conteneur DI
            serviceProvider = SR_ConteneurDI.ConfigureServices();
            _serviceProvider = serviceProvider;

            // Résolution des services nécessaires au démarrage
            _ucOnStart = serviceProvider.GetRequiredService<IU_Application_OnStart>();
            _settingsApp = serviceProvider.GetRequiredService<IS_Settings_App>();
            _settingsUser = serviceProvider.GetRequiredService<IS_Settings_User>();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Point d’entrée principal du cycle de vie de l’application.
        /// </summary>
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(Application_Startup)}";

            // Étape 1️ : lecture des arguments
            ProcessStartupArguments(e.Args, callChain);

            // Étape 2️ : exécution du processus de lancement applicatif
            await LaunchApplicationAsync(CultureInfo.CurrentCulture.Name, callChain);
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Analyse et applique les paramètres reçus en ligne de commande
        /// (par exemple : identifiant utilisateur).
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Initialiser le contexte utilisateur avant l’exécution du UseCase de démarrage.
        /// </para>
        /// </summary>
        /// <param name="args">Tableau d’arguments passés à l’application.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        private void ProcessStartupArguments(string[] args, string caller)
        {
            string callChain = $"{caller} > {nameof(ProcessStartupArguments)}";

            if (args == null || args.Length == 0)
                return;

            foreach (var arg in args)
            {
                if (arg.StartsWith("iduser="))
                {
                    if (int.TryParse(arg.Substring("iduser=".Length), out int idUser))
                    {
                        _settingsUser.SetAppUserId(idUser);
                    }
                }
            }
        }

        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Exécute le processus complet de démarrage applicatif via le UseCase <see cref="IU_Application_OnStart"/>.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Vérifier l’environnement, exécuter les vérifications de démarrage,
        /// et lancer ou fermer l’application selon le résultat.
        /// </para>
        /// </summary>
        /// <param name="cultureCode">Code de culture (ex. "fr-FR").</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        private async Task LaunchApplicationAsync(string cultureCode, string caller)
        {
            string callChain = $"{caller} > {nameof(LaunchApplicationAsync)}";

            bool start = await _ucOnStart.ExecuteAsync(cultureCode, callChain);

            if (start)
            {
                var mainWindow = App._serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Résout dynamiquement les dépendances manquantes des bibliothèques communes.
        /// </summary>
        private Assembly? OnResolveAssembly(object? sender, ResolveEventArgs args)
        {
            string assemblyPath = Path.Combine(
                _settingsApp.GetCommonRessourcesPath().LocalPath,
                new AssemblyName(args.Name).Name + ".dll");

            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath)!;
            }
            return null;
        }

        #endregion
    }
}