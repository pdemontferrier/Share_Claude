using DG244Cutting.A_Domain.Interfaces.Handlers.Commands;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
//using DG244Cutting.A_Domain.Interfaces.Services.Business;
using DG244Cutting.A_Domain.Interfaces.Services.Infrastructure;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
//using DG244Cutting.A_Domain.Interfaces.Settings.Business;
using DG244Cutting.A_Domain.Interfaces.Settings.Infrastructure;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
//using DG244Cutting.A_Domain.Interfaces.UseCases.Business;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
//using DG244Cutting.B_UseCases.DataProviders.App;
//using DG244Cutting.B_UseCases.DataProviders.Business;
//using DG244Cutting.B_UseCases.DataProviders.User;
using DG244Cutting.B_UseCases.Handlers.Commands;
using DG244Cutting.B_UseCases.Handlers.Generic;
using DG244Cutting.B_UseCases.Handlers.Queries;
using DG244Cutting.B_UseCases.Services.App;
//using DG244Cutting.B_UseCases.Services.Presentation;

//using DG244Cutting.B_UseCases.Services.Business;
using DG244Cutting.B_UseCases.Services.User;
using DG244Cutting.B_UseCases.Settings.App;
//using DG244Cutting.B_UseCases.Settings.Business;
using DG244Cutting.B_UseCases.Settings.User;
using DG244Cutting.B_UseCases.UseCases.App;
//using DG244Cutting.B_UseCases.UseCases.Business;
using DG244Cutting.B_UseCases.UseCases.User;
using DG244Cutting.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using DG244Cutting.C_Infrastructure.Repositories.DIGIT_TRY;
using DG244Cutting.C_Infrastructure.Repositories.Generic;
using DG244Cutting.C_Infrastructure.Services;
using DG244Cutting.C_Infrastructure.Settings;
using DG244Cutting.D_Presentation.Services;
using DG244Cutting.D_Presentation.Settings;
using DG244Cutting.D_Presentation.ViewModels.Components.Banner;
using DG244Cutting.D_Presentation.ViewModels.Components.HorizontalMenus;
using DG244Cutting.D_Presentation.ViewModels.Pages;
using DG244Cutting.D_Presentation.ViewModels.Shell;
using DG244Cutting.D_Presentation.Views.Components.Banner;
using DG244Cutting.D_Presentation.Views.Shell;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DG244Cutting.E_Miscellaneous.CompositionRoot
{
    /// <summary>
    /// Composition Root de la solution : composant statique câblant le graphe d'injection
    /// de dépendances de toutes les couches.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cas particulier EA-10 « Composition Root hors modèle d'injection paramétrée » (§2.8
    /// note [d] augmentée, §5.9.4 du 0230, §17.4 du 0231). Classe statique sans interface
    /// contractuelle <c>IS_ConteneurDI</c> : l'assembleur du système d'injection ne peut
    /// lui-même figurer dans ce système sans circularité.
    /// </para>
    /// <para>
    /// Doctrine des portées DI (P4-bis, §4.10.10) : la portée est déterminée par la
    /// dépendance au DbContext partagé scoped, et non par la famille ni la catégorie de
    /// domaine. Un composant qui touche le DbContext partagé (directement, ou transitivement
    /// via un IQ_/IC_/IR_ ou un Service qui le touche) est Scoped ; un composant sans
    /// dépendance scoped est Singleton. Cas notables : UC_Navigation (transverse, état de
    /// session, sans DbContext) et le pipeline d'erreur autonome (UC_LogAndNotify,
    /// SR_ErrorLogger, CH_UserAppErrorLog, CR_UserAppErrorLog — EA-09 via factory) sont
    /// Singleton ; les UseCases transactionnels et ceux qui lisent via un IQ_ sont Scoped
    /// quelle que soit leur catégorie.
    /// </para>
    /// </remarks>
    public static class SR_ConteneurDI
    {
        /// <summary>
        /// Construit et retourne le <see cref="ServiceProvider"/> de la solution (jalon 1, §3.10).
        /// </summary>
        /// <remarks>
        /// <c>ValidateScopes = true</c> en DEBUG : toute résolution d'un service Scoped depuis
        /// le scope racine, ou toute captive dependency (Singleton capturant un Scoped), lève
        /// immédiatement au démarrage.
        /// </remarks>
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // A_Domain
            RDI_Handlers(services);
            RDI_Repositories(services);
            RDI_Services(services);
            RDI_Settings(services);
            RDI_UseCases(services);

            // C_Infrastructure
            RDI_PersistenceGestStock(services);

            // D_Presentation
            RDI_ViewModels(services);
            RDI_Views(services);

            // Lancement du service.
#if DEBUG
            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true
            });
#else
            return services.BuildServiceProvider();
#endif
        }



        #region A_Domain
        private static void RDI_Handlers(IServiceCollection services)
        {
            // Portée déterminée par la dépendance au DbContext partagé (P4-bis, §4.10.10).

            // Commands génériques — touchent le DbContext partagé via le Repository -> Scoped.
            services.AddScoped(typeof(IC_Generic<>), typeof(CH_Generic<>));

            // Command du log d'erreur — pipeline autonome EA-09 (factory, hors DbContext partagé) -> Singleton.
            // (Permet à SR_ErrorLogger, Singleton, de l'injecter sans captive dependency.)
            services.AddSingleton<IC_UserAppErrorLog, CH_UserAppErrorLog>();

            // Command de l'Event Store — écrit dans le DbContext partagé (solidarité transactionnelle) -> Scoped.
            services.AddScoped<IC_UserAppEventStore, CH_UserAppEventStore>();

            // Queries génériques — lecture via le DbContext partagé -> Scoped.
            services.AddScoped(typeof(IQ_Generic<>), typeof(QH_Generic<>));

            // Queries spécialisées — lecture via le DbContext partagé -> Scoped.
            services.AddScoped<IQ_AppList, QH_AppList>();
            services.AddScoped<IQ_UserApp, QH_UserApp>();
            services.AddScoped<IQ_UserAppAccess, QH_UserAppAccess>();
            services.AddScoped<IQ_UserAppMessage, QH_UserAppMessage>();
            services.AddScoped<IQ_UserAppPageRight, QH_UserAppPageRight>();
            services.AddScoped<IQ_UserAppSession, QH_UserAppSession>();

        }

        private static void RDI_Repositories(IServiceCollection services)
        {
            // CR générique — DbContext partagé -> Scoped.
            services.AddScoped(typeof(IR_Generic<>), typeof(CR_Generic<>));

            // CR du log d'erreur — EA-09, factory autonome (hors DbContext partagé) -> Singleton.
            // Sa dépendance est IDbContextFactory (Singleton) ; aucune captive dependency.
            services.AddSingleton<IR_UserAppErrorLog, CR_UserAppErrorLog>();

        }

        private static void RDI_Services(IServiceCollection services)
        {
            // Portée déterminée par la dépendance (P4-bis) : Singleton si aucune dépendance
            // scoped ; Scoped si le service touche le DbContext partagé (typiquement via un IQ_/IC_).

            // App (techniques transverses, sans dépendance scoped) -> Singleton.
            services.AddSingleton<IS_AppContext, SR_AppContext>();
            services.AddSingleton<IS_ErrorLogger, SR_ErrorLogger>();   // injecte IC_UserAppErrorLog (Singleton) : OK.
            services.AddSingleton<IS_ExClassifier, SR_ExClassifier>(); // aucune dépendance.

            // Business


            // Infrastructure
            services.AddSingleton<ISet<string>>(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "dbo.spr_ProductionSeries_SyncFromSource",
                "dbo.spr_ArticleReference_InsertFromSource",
                "dbo.spr_ColorRalFinish_InsertFromSource",
                "dbo.spr_ArticleInternal_InsertFromSource",
                "dbo.spr_SpatialPosition_InsertFromSource",
                "dbo.spr_CustomerOrder_InsertFromSource",
                "dbo.spr_ProductionChassis_InsertFromSource",
                "dbo.spr_ProductionFrameSash_InsertFromSource",
                "dbo.spr_ProductionCutPiece_InsertFromSource",
                "dbo.spr_ProductionSeries_FinalizeImport"
            });

            services.AddSingleton<IS_StoredProcedure, SR_StoredProcedure>(); // factory (hors DbContext partagé).
            services.AddSingleton<IS_DigitTryDb_TestConnection, SR_DigitTryDb_TestConnection>();

            // Presentation (techniques transverses, sans dépendance scoped) -> Singleton.
            services.AddSingleton<IS_ControlStyler, SR_ControlStyler>();
            services.AddSingleton<IS_Dictionary, SR_Dictionary>();
            services.AddSingleton<IS_Navigation, SR_Navigation>();
            services.AddSingleton<IS_Notification, SR_Notification>();
            services.AddSingleton<IS_Shutdown, SR_Shutdown>();
            // SR créateur de scope par invocation (EA-11, §4.10.10). Singleton stateless.
            services.AddSingleton<IS_UseCaseInvoker, SR_UseCaseInvoker>();
            services.AddSingleton<IS_Window, SR_Window>();


            // User
            // CheckAppAccess / CheckDeviceUser injectent un IQ_ (lecture via DbContext partagé) -> Scoped (P4-bis).
            services.AddScoped<IS_User_CheckAppAccess, SR_User_CheckAppAccess>();
            services.AddScoped<IS_User_CheckDeviceUser, SR_User_CheckDeviceUser>();

            // DeviceContext - sans dépendance scoped (ISE_User Singleton, IS_ExClassifier Singleton) -> Singleton (P4-bis, §4.10.10).
            services.AddSingleton<IS_UserDeviceContext, SR_UserDeviceContext>();

            // Services métier de session : consomment un IC_ (mutation via DbContext partagé) -> Scoped.
            services.AddScoped<IS_UserAppMessage_Add, SR_UserAppMessage_Add>();
            services.AddScoped<IU_UserAppMessage_CheckUnread, UC_UserAppMessage_CheckUnread>();
            services.AddScoped<IS_UserAppMessage_MarkAsRead, SR_UserAppMessage_MarkAsRead>();
            services.AddScoped<IS_UserAppSession_Create, SR_UserAppSession_Create>();
            services.AddScoped<IS_UserAppSession_DeleteExtra, SR_UserAppSession_DeleteExtra>();
            services.AddScoped<IS_UserAppSession_Update, SR_UserAppSession_Update>();

        }

        private static void RDI_Settings(IServiceCollection services)
        {
            // Famille Settings : Singleton (conteneur d'état applicatif partagé).

            // Application
            services.AddSingleton<ISE_App, SE_App>();

            // Business


            // Infrastructure
            services.AddSingleton<ISE_Database, SE_Database>();

            // Presentation
            services.AddSingleton<ISE_BarProfilSection, SE_BarProfilSection>();
            services.AddSingleton<ISE_Flag, SE_Flag>();
            services.AddSingleton<ISE_Language, SE_Language>();
            services.AddSingleton<ISE_Navigation, SE_Navigation>();
            services.AddSingleton<ISE_Window, SE_Window>();

            // User
            services.AddSingleton<ISE_User, SE_User>();

        }

        private static void RDI_UseCases(IServiceCollection services)
        {
            // Portée déterminée par la dépendance au DbContext partagé (P4-bis, §4.10.10).

            // Application
            // UC_Application_OnStart : orchestrateur de la séquence de démarrage applicatif
            //   (§3.10 du 0230). Non transactionnel par construction, mais consomme trois
            //   Query Handlers Scoped (IQ_UserApp, IQ_UserAppSession, IQ_AppList) qui touchent
            //   le DbContext partagé -> Scoped (P4-bis, §4.10.10 ; R-4.10.14).
            services.AddScoped<IU_Application_OnStart, UC_Application_OnStart>();
            services.AddScoped<IU_CloseApplication, UC_CloseApplication>();
            // UC_GetApplicationVersion : lecture transverse du numéro de version applicatif
            //   par accès direct à Assembly.GetExecutingAssembly (EA propre tracée au remarks
            //   de classe), sans dépendance Scoped ni accès au DbContext partagé -> Singleton
            //   (P4-bis, §4.10.10).
            services.AddSingleton<IU_GetApplicationVersion, UC_GetApplicationVersion>();
            // UC_Language_Apply : orchestrateur du changement de langue de l'application
            //   (chargement du dictionnaire XAML, persistance du code culture, synchronisation
            //   du drapeau et des cibles CultureInfo .NET) — dépendances Singleton uniquement
            //   (ISE_App, ISE_Language, ISE_Flag, IU_LogAndNotify), non transactionnel par
            //   construction -> Singleton (P4-bis, §4.10.10).
            services.AddSingleton<IU_Language_Apply, UC_Language_Apply>();
            // UC_LogAndNotify : pipeline d'erreur autonome (IS_AppContext, IS_ErrorLogger,
            //   IS_Notification, IS_Dictionary — tous Singleton, hors DbContext partagé) -> Singleton.
            services.AddSingleton<IU_LogAndNotify, UC_LogAndNotify>();
            // UC_Navigation : orchestrateur transverse détenant l'état de session (historique de
            //   navigation), sans dépendance au DbContext partagé -> Singleton.
            services.AddSingleton<IU_Navigation, UC_Navigation>();

            // Business


            // User
            services.AddScoped<IU_UserAppMessage_Add, UC_UserAppMessage_Add>();
            services.AddScoped<IU_UserAppMessage_MarkAsRead, UC_UserAppMessage_MarkAsRead>();
            // UC_UserAppPageRight_Apply : lit via IQ_ (DbContext partagé) -> Scoped.
            services.AddScoped<IU_UserAppPageRight_Apply, UC_UserAppPageRight_Apply>();
            // UC_UserAppSession_Close / _Open : injectent le DbContext partagé + transaction -> Scoped.
            services.AddScoped<IU_UserAppSession_Close, UC_UserAppSession_Close>();
            services.AddScoped<IU_UserAppSession_Open, UC_UserAppSession_Open>();
            // UC_UserIdentify : injecte CheckDeviceUser/CheckAppAccess (Scoped, via IQ_) -> Scoped.
            services.AddScoped<IU_UserIdentify, UC_UserIdentify>();

        }

        #endregion


        #region B_UseCases

        #endregion


        #region C_Infrastructure

        private static void RDI_PersistenceGestStock(IServiceCollection services)
        {
            // Câblage triple du DbContext (§4.8.5 du 0230). Doctrine faisant autorité en §4.8.5.

            // (1) DbContext partagé Scoped — chaîne (1) d'écriture stricte UC -> SR -> CH -> CR.
            //     optionsLifetime: ServiceLifetime.Singleton — TEST EXPLORATOIRE piste α.
            //     Les DbContextOptions<TContext> sont immuables après construction ; les promouvoir
            //     en Singleton aligne leur portée sur celle de la factory du Pattern 3 et résout
            //     la captive dependency IDbContextFactory (Singleton) -> DbContextOptions (Scoped).
            //     Le DbContext lui-même reste Scoped (contextLifetime non spécifié = défaut Scoped).
            services.AddDbContext<DigitTryDbContext>((sp, options) =>
            {
                var databaseSettings = sp.GetRequiredService<ISE_Database>();

                options.UseSqlServer(
                    databaseSettings.BuildConnectionString(),
                    sqlOptions =>
                    {
                        // EnableRetryOnFailure : voir §4.8.5 / §4.10.1 (ExecutionStrategy exigée
                        // pour tout UseCase ouvrant une transaction explicite).
                        sqlOptions.EnableRetryOnFailure();
                    });
            }, optionsLifetime: ServiceLifetime.Singleton);

            // (2) Résolution du type abstrait DbContext pour CR_Generic<T> et CR_[Entité] standards.
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<DigitTryDbContext>());

            // (3) Factory pour cas hors transaction UseCase : CR_UserAppErrorLog (EA-09) et SR_StoredProcedure.
            services.AddDbContextFactory<DigitTryDbContext>((sp, options) =>
            {
                var databaseSettings = sp.GetRequiredService<ISE_Database>();

                options.UseSqlServer(
                    databaseSettings.BuildConnectionString(),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure();
                    });
            });

        }

        #endregion


        #region D_Presentation

        private static void RDI_ViewModels(IServiceCollection services)
        {
            // Components
            //   Banner
            services.AddSingleton<VM_Banner>();

            //   MenuHorizontal
            //   Famille VM_MH : aucune dépendance scoped (IS_Dictionary,
            //   IU_LogAndNotify, IU_Navigation, ISE_App tous Singleton),
            //   aucune captive dependency -> Singleton (P4-bis, §4.10.10
            //   du 0230, R-4.10.14 du 0231). Validation par
            //   ValidateScopes=true en build DEBUG.
            services.AddSingleton<VM_MH_Reduce>();
            services.AddSingleton<VM_MH01>();
            services.AddSingleton<VM_MH02>();
            services.AddSingleton<VM_MH03>();
            services.AddSingleton<VM_MH10>();
            services.AddSingleton<VM_MH20>();
            services.AddSingleton<VM_MH30>();
            services.AddSingleton<VM_MH40>();
            services.AddSingleton<VM_MH50>();
            services.AddSingleton<VM_MH60>();
            services.AddSingleton<VM_MH70>();
            services.AddSingleton<VM_MH80>();
            services.AddSingleton<VM_MH90>();
            services.AddSingleton<VM_MH98>();
            services.AddSingleton<VM_MH99>();

            // Generic

            // Pages
            services.AddSingleton<VM_Page01>();
            services.AddSingleton<VM_Page02>();
            services.AddSingleton<VM_Page03>();
            services.AddSingleton<VM_Page10>();
            services.AddSingleton<VM_Page20>();
            services.AddSingleton<VM_Page30>();
            services.AddSingleton<VM_Page40>();
            services.AddSingleton<VM_Page50>();
            services.AddSingleton<VM_Page60>();
            services.AddSingleton<VM_Page70>();
            services.AddSingleton<VM_Page80>();
            services.AddSingleton<VM_Page90>();
            services.AddSingleton<VM_Page98>();
            services.AddSingleton<VM_Page99>();

            // Shell
            services.AddSingleton<VM_MainWindow>();
        }

        private static void RDI_Views(IServiceCollection services)
        {
            // Components
            services.AddSingleton<Banner>();

            // Generic
            // Instanciée par WPF Navigation via Activator.CreateInstance, hors conteneur DI

            // Pages
            // Instanciée par WPF Navigation via Activator.CreateInstance, hors conteneur DI

            // Shell
            services.AddSingleton<MainWindow>();

        }
        #endregion

    }
}