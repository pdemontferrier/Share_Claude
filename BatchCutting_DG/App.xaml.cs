using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.E_CompositionRoot.Services;
using System.Globalization;

namespace BatchCutting_DG
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        private readonly IS_Language _language;
        private readonly IS_OnStart _onStart;
        private readonly IS_Settings _settings;

        public App()
        {
            // Ajouter les DLL des ressources communes
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

            // Configuration du conteneur DI
            _serviceProvider = SR_ConteneurDI.ConfigureServices();
            ServiceProvider = _serviceProvider;
            _language = _serviceProvider.GetRequiredService<IS_Language>();
            _onStart = _serviceProvider.GetRequiredService<IS_OnStart>();
            _settings = _serviceProvider.GetRequiredService<IS_Settings>();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // Lire les arguments de ligne de commande
            string[] args = e.Args;
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg.StartsWith("iduser="))
                    {
                        if (int.TryParse(arg.Substring("iduser=".Length), out int idUser))
                        {
                            _settings.SetAppUserID(idUser);
                        }
                    }
                }
            }

            // Configuration de la langue
            string cultureCode = CultureInfo.CurrentCulture.Name;
            _language.Execute(cultureCode);

            // Tester le UserService
            bool start = await _onStart.ExecuteAsync();
            if (start)
            {
                // Lancer la fenêtre principale
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                // Fermer l'application
                Application.Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // _serviceProvider.Dispose();
            base.OnExit(e);
        }

        private Assembly? OnResolveAssembly(object? sender, ResolveEventArgs args)
        {
            string assemblyPath = Path.Combine(_settings.GetCommonRessourcesPath().LocalPath, new AssemblyName(args.Name).Name + ".dll");

            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath)!;
            }
            return null;
        }
    }
}
