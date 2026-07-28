using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.Services.Business;
using LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure;
using LeitTemporImport.A_Domain.Interfaces.Services.User;
using LeitTemporImport.A_Domain.Interfaces.Settings.App;
using LeitTemporImport.A_Domain.Interfaces.Settings.Business;
using LeitTemporImport.A_Domain.Interfaces.Settings.Infrastructure;
using LeitTemporImport.A_Domain.Interfaces.Settings.User;
using LeitTemporImport.A_Domain.Interfaces.UseCases.Business;
using LeitTemporImport.A_Domain.Interfaces.UseCases.User;
using LeitTemporImport.B_UseCases.Handlers.Commands;
using LeitTemporImport.B_UseCases.Handlers.Queries;
using LeitTemporImport.B_UseCases.Services.App;
using LeitTemporImport.B_UseCases.Services.Business;
using LeitTemporImport.B_UseCases.Services.User;
using LeitTemporImport.B_UseCases.Settings.Business;
using LeitTemporImport.B_UseCases.UseCases.Business;
using LeitTemporImport.B_UseCases.UseCases.User;
using LeitTemporImport.C_Infrastructure.DataProviders.QueriesApp;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;
using LeitTemporImport.C_Infrastructure.Repositories.DIGIT_TRY;
using LeitTemporImport.C_Infrastructure.Repositories.Generic;
using LeitTemporImport.C_Infrastructure.Services;
using LeitTemporImport.C_Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// BatchStockRelease.E_Miscellaneous.CompositionRoot
namespace LeitTemporImport.E_Miscellaneous.CompositionRoot
{
    public static class SR_ConteneurDI
    {
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // A_Domain
            RegisterDomainInterfaceHandlers(services);
            RegisterDomainInteraceRepositories(services);
            RegisterDomainInterfaceServicesApp(services);
            RegisterDomainInterfaceServicesBusiness(services);
            RegisterDomainInterfaceServicesUser(services);
            RegisterDomainInterfaceUseCases(services);

            // B_UseCases

            // C_Infrastructure
            RegisterInfrastructurePersistenceGestStock(services);
            RegisterDomainInterfaceSettings(services);
            RegisterDomainInterfaceServicesInfrastructure(services);

            // D_Presentation
            RegisterPresentationPagesViewModels(services);
            RegisterPresentationComponentsViewModels(services);

            // MainWindow
            RegisterPresentationViews(services);

            // Lancement du service
            return services.BuildServiceProvider();
        }



        #region A_Domain
        private static void RegisterDomainInterfaceHandlers(IServiceCollection services)
        {
            // Commands
            services.AddTransient<IC_LifecycleAction, CH_LifecycleAction>();
            services.AddTransient<IC_ProductionSeries, CH_ProductionSeries>();
            services.AddTransient<IC_TemporImport, CH_TemporImport>();
            services.AddTransient<IC_UserAppErrorLog, CH_UserAppErrorLog>();
            services.AddTransient<IC_UserSession, CH_UserSession>();

            // Queries
            services.AddTransient<IQ_AppContext, DP_AppContext>();
            services.AddTransient<IQ_LifecycleAction, QH_LifecycleAction>();
            services.AddTransient<IQ_ProductionSeries, QH_ProductionSeries>();
            services.AddTransient<IQ_TemporImport, QH_TemporImport>();
            services.AddTransient<IQ_UserApp, QH_UserApp>();
            services.AddTransient<IQ_UserAppErrorLog, QH_UserAppErrorLog>();
            services.AddTransient<IQ_UserSession, QH_UserSession>();

        }

        private static void RegisterDomainInteraceRepositories(IServiceCollection services)
        {
            // CR pour les méthodes génériques
            services.AddTransient(typeof(IR_Generic<>), typeof(CR_Generic<>));

            // CR pour les méthodes spécifiques
            services.AddTransient<IR_Generic<LifecycleAction>, CR_LifecycleAction>();
            services.AddTransient<IR_ProductionSeries, CR_ProductionSeries>();
            services.AddTransient<IR_UserApp, CR_UserApp>();
            services.AddTransient<IR_UserSession, CR_UserSession>();

            // DP pour DTO
        }

        private static void RegisterDomainInterfaceServicesApp(IServiceCollection services)
        {
            // Application Services
            services.AddSingleton<IS_ErrorLogger, SR_ErrorLogger>();
            services.AddSingleton<IS_FileScanner, SR_FileScanner>();
            services.AddSingleton<ISE_App, SR_Settings_App>();
        }

        private static void RegisterDomainInterfaceServicesBusiness(IServiceCollection services)
        {
            // Business Services
            services.AddSingleton<IS_LifecycleActionAdd, SR_LifecycleActionAdd>();
            services.AddSingleton<IS_PostImportSeriesUpdater, SR_PostImportSeriesUpdater>();
            services.AddSingleton<IS_ProductionSeriesReader, SR_ProductionSeriesReader>();
            services.AddSingleton<IS_TemporImportFileImporter, SR_TemporImportFileImporter>();
            services.AddSingleton<IS_TemporRowTransformer, SR_TemporRowTransformer>();

        }

        private static void RegisterDomainInterfaceServicesInfrastructure(IServiceCollection services)
        {
            // Procédures stockées (en premier, avant les services)
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

            // Infrastructure Services
            services.AddSingleton<IS_FileDelete, SR_FileDelete>();
            services.AddSingleton<IS_FileMoveToFailed, SR_FileMoveToFailed>();
            services.AddSingleton<IS_MdbReader, SR_MdbReader>();
            services.AddSingleton<IS_StoredProcedure, SR_StoredProcedure>();

        }

        private static void RegisterDomainInterfaceServicesUser(IServiceCollection services)
        {
            // User Service
            services.AddSingleton<ISE_User, SR_Settings_User>();
            services.AddSingleton<IS_User_CheckAppDeviceUser, SR_User_CheckAppDeviceUser>();
            services.AddSingleton<IS_UserSession_Open, SR_UserSession_Open>();
            services.AddSingleton<IS_UserSession_Close, SR_UserSession_Close>();
            
        }

        private static void RegisterDomainInterfaceUseCases(IServiceCollection services)
        {
            // Application UseCases App

            // Application UseCases Business
            services.AddSingleton<IU_TemporImport, UC_TemporImport>();
            services.AddSingleton<IU_TemporImport_ProcessFile, UC_TemporImport_ProcessFile>();

            // Application UseCases User
            services.AddSingleton<IU_UserIdentify, UC_UserIdentify>();

        }

        #endregion


        #region B_UseCases

        #endregion


        #region C_Infrastructure

        private static void RegisterInfrastructurePersistenceGestStock(IServiceCollection services)
        {
            //services.AddDbContext<DigitTryDbContext>();

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

        private static void RegisterDomainInterfaceSettings(IServiceCollection services)
        {
            // Settings
            services.AddSingleton<ISE_Database, SE_Database>();
            services.AddSingleton<ISE_Business, SE_Business>();
        }

        #endregion


        #region D_Presentation

        private static void RegisterPresentationPagesViewModels(IServiceCollection services)
        {

        }

        private static void RegisterPresentationComponentsViewModels(IServiceCollection services)
        {

        }
        #endregion

        private static void RegisterPresentationViews(IServiceCollection services)
        {

        }

    }
}