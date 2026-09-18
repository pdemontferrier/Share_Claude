using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic;
using BatchStockRelease.B_UseCases.Handlers.Commands;
using BatchStockRelease.B_UseCases.Handlers.Queries;
using BatchStockRelease.B_UseCases.Services.AppLogic;
using BatchStockRelease.B_UseCases.Services.BusinessLogic;
using BatchStockRelease.B_UseCases.Services.UserLogic;
using BatchStockRelease.B_UseCases.UseCases.AppLogic;
using BatchStockRelease.B_UseCases.UseCases.BusinessLogic;
using BatchStockRelease.B_UseCases.UseCases.UserLogic;
using BatchStockRelease.C_Infrastructure.DataProviders.QueriesGestStock;
using BatchStockRelease.C_Infrastructure.DataProviders.QueriesApp;
using BatchStockRelease.C_Infrastructure.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using BatchStockRelease.C_Infrastructure.Services;
using BatchStockRelease.D_Presentation.Services;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.ViewModels;
using BatchStockRelease.D_Presentation.ViewModels.Components.Banner;

// BatchStockRelease.E_Miscellaneous.CompositionRoot
namespace BatchStockRelease.E_Miscellaneous.CompositionRoot
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
            RegisterDomainInterfaceServicesBusinessLogic(services);
            RegisterDomainInterfaceServicesUserLogic(services);
            RegisterDomainInterfaceUseCases(services);

            // B_UseCases

            // C_Infrastructure
            RegisterInfrastructurePersistenceGestStock(services);

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
            services.AddScoped<IC_ArticleInterneConsommation, CH_ArticleInterneConsommation>();
            services.AddScoped<IC_ChutesArchive, CH_ChutesArchive>();
            services.AddScoped<IC_ChutesMagasin, CH_ChutesMagasin>();
            services.AddScoped<IC_CommandeClient, CH_CommandeClient>();
            services.AddScoped<IC_CommandeClientAction, CH_CommandeClientAction>();
            services.AddScoped<IC_CommandeClientModification, CH_CommandeClientModification>();
            services.AddScoped<IC_CommandeClientStatut, CH_CommandeClientStatut>();
            services.AddScoped<IC_DecoupeBarre, CH_DecoupeBarre>();
            services.AddScoped<IC_DecoupeDetail, CH_DecoupeDetail>();
            services.AddScoped<IC_DecoupeLot, CH_DecoupeLot>();
            services.AddScoped<IC_Stock, CH_Stock>();
            services.AddScoped<IC_StockModif, CH_StockModif>();
            services.AddScoped<IC_UserAppErrorLog, CH_UserAppErrorLog>();
            services.AddScoped<IC_UserAppEventStore, CH_UserAppEventStore>();
            services.AddScoped<IC_UserAppMessage, CH_UserAppMessage>();
            services.AddScoped<IC_UserSession, CH_UserSession>();
            services.AddScoped<IC_UserSessionCommand, CH_UserSessionCommand>();

            // Queries
            services.AddScoped<IQ_AppContext, DP_AppContext>();
            services.AddScoped<IQ_ArticleInterne, QH_ArticleInterne>();
            services.AddScoped<IQ_ArticleInterneConsommation, QH_ArticleInterneConsommation>();
            services.AddScoped<IQ_ChutesArchive, QH_ChutesArchive>();
            services.AddScoped<IQ_ChutesMagasin, QH_ChutesMagasin>();
            services.AddScoped<IQ_CommandeClient, QH_CommandeClient>();
            services.AddScoped<IQ_CommandeClientAction, QH_CommandeClientAction>();
            services.AddScoped<IQ_CommandeClientActionType, QH_CommandeClientActionType>();
            services.AddScoped<IQ_CommandeClientModification, QH_CommandeClientModification>();
            services.AddScoped<IQ_CommandeClientStatut, QH_CommandeClientStatut>();
            services.AddScoped<IQ_DecoupeBarre, QH_DecoupeBarre>();
            services.AddScoped<IQ_DecoupeBarreDetails, QH_DecoupeBarreDetails>();
            services.AddScoped<IQ_DecoupeBarreWithBarDropToRelease, QH_DecoupeBarreWithBarDropToRelease>();
            services.AddScoped<IQ_DecoupeBarreWithBarNewToRelease, QH_DecoupeBarreWithBarNewToRelease>();
            services.AddScoped<IQ_DecoupeDetail, QH_DecoupeDetail>();
            services.AddScoped<IQ_DecoupeLot, QH_DecoupeLot>();
            services.AddScoped<IQ_DecoupeLotWithBarDropToRelease, QH_DecoupeLotWithBarDropToRelease>();
            services.AddScoped<IQ_DecoupeLotWithBarNewToRelease, QH_DecoupeLotWithBarNewToRelease>();
            services.AddScoped<IQ_DecoupeMachine, QH_DecoupeMachine>();
            services.AddScoped<IQ_PickingEmplacement, QH_PickingEmplacement>();
            services.AddScoped<IQ_Stock, QH_Stock>();
            services.AddScoped<IQ_StockModif, QH_StockModif>();
            services.AddScoped<IQ_User, QH_User>();
            services.AddScoped<IQ_UserApplication, QH_UserApplication>();
            services.AddScoped<IQ_UserAppErrorLog, QH_UserAppErrorLog>();
            services.AddScoped<IQ_UserAppMessage, QH_UserAppMessage>();
            services.AddScoped<IQ_UserAppPageDroit, QH_UserAppPageDroit>();
            services.AddScoped<IQ_UserDroit, QH_UserDroit>();
            services.AddScoped<IQ_UserSession, QH_UserSession>();
            services.AddScoped<IQ_UserSessionCommand, QH_UserSessionCommand>();
            services.AddScoped<IQ_UserSessionDetails, QH_UserSessionDetails>();
            services.AddScoped<IQ_VieApplication, QH_VieApplication>();
            services.AddScoped<IQ_VieChuteMagasinReference, QH_VieChuteMagasinReference>();
            services.AddScoped<IQ_VieStockQuantiteEmplacement, QH_VieStockQuantiteEmplacement>();

        }

        private static void RegisterDomainInteraceRepositories(IServiceCollection services)
        {
            // CR pour les méthodes génériques
            services.AddScoped(typeof(IR_Generic<>), typeof(CR_Generic<>));

            // CR pour les méthodes spécifiques
            services.AddScoped<IR_ChutesMagasin, CR_ChutesMagasin>();
            services.AddScoped<IR_CommandesClient, CR_CommandeClient>();
            services.AddScoped<IR_DecoupeBarre, CR_DecoupeBarre>();
            services.AddScoped<IR_DecoupeDetail, CR_DecoupeDetail>();
            services.AddScoped<IR_DecoupeLot, CR_DecoupeLot>();
            services.AddScoped<IR_PickingEmplacement, CR_PickingEmplacement>();
            services.AddScoped<IR_User, CR_User>();
            services.AddScoped<IR_UserApplication, CR_UserApplication>();
            services.AddScoped<IR_UserAppMessage, CR_UserAppMessage>();
            services.AddScoped<IR_UserAppPageDroit, CR_UserAppPageDroit>();
            services.AddScoped<IR_UserDroit, CR_UserDroit>();
            services.AddScoped<IR_UserSessionCommand, CR_UserSessionCommand>();
            services.AddScoped<IR_UserSession, CR_UserSession>();
            services.AddScoped<IR_VieApplication, CR_VieApplication>();
            services.AddScoped<IR_VieChuteMagasinReference, CR_VieChuteMagasinReference>();
            services.AddScoped<IR_VieStockQuantiteEmplacement, CR_VieStockQuantiteEmplacement>();

            // DP pour DTO
            services.AddScoped<IR_DecoupeBarreDetails, DP_DecoupeBarreDetails>();
            services.AddScoped<IR_DecoupeBarreWithBarDropToRelease, DP_DecoupeBarreWithBarDropToRelease>();
            services.AddScoped<IR_DecoupeBarreWithBarNewToRelease, DP_DecoupeBarreWithBarNewToRelease>();
            services.AddScoped<IR_DecoupeLotWithBarDropToRelease, DP_DecoupeLotWithBarDropToRelease>();
            services.AddScoped<IR_DecoupeLotWithBarNewToRelease, DP_DecoupeLotWithBarNewToRelease>();
            services.AddScoped<IR_UserSessionDetails, DP_UserSessionDetail>();
        }

        private static void RegisterDomainInterfaceServicesApp(IServiceCollection services)
        {
            services.AddSingleton<INavigationState, NavigationState>();

            services.AddSingleton<I_DB_MonitoringController, SR_DB_Monitoring>();

            // Application Services
            services.AddSingleton<IS_ApplicationAvailability, SR_ApplicationAvailability>();
            services.AddSingleton<IS_ControlStyler, SR_ControlStyler>();
            services.AddSingleton<IS_DB_Monitoring, SR_DB_Monitoring>();
            services.AddSingleton<IS_Dictionary, SR_Dictionary>();
            services.AddSingleton<IS_FileLogger, SR_FileLogger>();
            services.AddSingleton<IS_Flags, SR_Flags>();
            services.AddSingleton<IS_Icons, SR_Icons>();
            services.AddSingleton<IS_Language, SR_Language>();
            services.AddSingleton<IS_LogAndNotify, SR_LogAndNotify>();
            services.AddSingleton<IS_Messages, SR_Messages>();
            services.AddSingleton<IS_Navigation, SR_Navigation>();
            services.AddSingleton<IS_Notification, SR_Notification>();
            services.AddSingleton<IS_Settings_App, SR_Settings_App>();
            services.AddSingleton<IS_Settings_Language, SR_Settings_Language>();
            services.AddSingleton<IS_Shutdown, SR_Shutdown>();
            services.AddSingleton<IS_StartupContextUpdater, SR_StartupContextUpdater>();
            services.AddSingleton<IS_StartupDatabaseConnectivity, SR_StartupDatabaseConnectivity>();
            services.AddSingleton<IS_Utilities, SR_Utilities>();
            services.AddSingleton<IS_Window, SR_Window>();
        }

        private static void RegisterDomainInterfaceServicesBusinessLogic(IServiceCollection services)
        {
            // BusinessLogic Services
            services.AddSingleton<IS_AIConsommation_Add, SR_AIConsommation_Add>();
            services.AddSingleton<IS_BatchDoc_AddDecoupeBarreToExcel, SR_BatchDoc_AddDecoupeBarreToExcel>();
            services.AddSingleton<IS_BatchDoc_AddDecoupeDetailToExcel, SR_BatchDoc_AddDecoupeDetailToExcel>();
            services.AddSingleton<IS_BatchDoc_AddEluFiles, SR_BatchDoc_AddEluFiles>();
            services.AddSingleton<IS_BatchDoc_SetDirectory, SR_BatchDoc_SetDirectory>();
            services.AddSingleton<IS_ChutesArchive_Add, SR_ChutesArchive_Add>();
            services.AddSingleton<IS_ChutesMagasin_Delete, SR_ChutesMagasin_Delete>();
            services.AddSingleton<IS_ChutesMagasin_UpdateIntegration, SR_ChutesMagasin_UpdateIntegration>();
            services.AddSingleton<IS_CommandeClient_UpdateStatut, SR_CommandeClient_UpdateStatut>();
            services.AddSingleton<IS_CommandeClientAction_Add, SR_CommandeClientAction_Add>();
            services.AddSingleton<IS_CommandeClientModification_Add, SR_CommandeClientModification_Add>();
            services.AddSingleton<IS_StoreProcedure, SR_StoreProcedure>();
            services.AddSingleton<IS_DecoupeBarre_AddNewBarre, SR_DecoupeBarre_AddNewBarre>();
            services.AddSingleton<IS_DecoupeBarre_ClearOutOfStockStatus, SR_DecoupeBarre_ClearOutOfStockStatus>();
            services.AddSingleton<IS_DecoupeBarre_FinalizeBarre, SR_DecoupeBarre_FinalizeBarre>();
            services.AddSingleton<IS_DecoupeBarre_OptimBarDrop, SR_DecoupeBarre_OptimBarDrop>();
            services.AddSingleton<IS_DecoupeBarre_OptimBarNew, SR_DecoupeBarre_OptimBarNew>();
            services.AddSingleton<IS_DecoupeBarre_ProcessAllocation, SR_DecoupeBarre_ProcessAllocation>();
            services.AddSingleton<IS_DecoupeBarre_UpdateApproDone, SR_DecoupeBarre_UpdateApproDone>();
            services.AddSingleton<IS_DecoupeBarre_UpdateApproInactif, SR_DecoupeBarre_UpdateApproInactif>();
            services.AddSingleton<IS_DecoupeBarre_UpdateApproStarted, SR_DecoupeBarre_UpdateApproStarted>();
            services.AddSingleton<IS_DecoupeBarre_UpdateAtForceOutOfStock, SR_DecoupeBarre_UpdateAtForceOutOfStock>();
            services.AddSingleton<IS_DecoupeBarre_UpdateChariot, SR_DecoupeBarre_UpdateChariot>();
            services.AddSingleton<IS_DecoupeBarre_UpdateOutOfStockStatus, SR_DecoupeBarre_UpdateOutOfStockStatus>();
            services.AddSingleton<IS_DecoupeBarre_UpdateStockReleaseState, SR_DecoupeBarre_UpdateStockReleaseState>();
            services.AddSingleton<IS_DecoupeDetail_AnnuleAllocation, SR_DecoupeDetail_AnnuleAllocation>();
            services.AddSingleton<IS_DecoupeDetail_MachineList, SR_DecoupeDetail_MachineList>();
            services.AddSingleton<IS_DecoupeDetail_UpdateAllocation, SR_DecoupeDetail_UpdateAllocation>();
            services.AddSingleton<IS_DecoupeDetail_UpdateIndice2, SR_DecoupeDetail_UpdateIndice2>();
            services.AddSingleton<IS_DecoupeLot_SetInfo, SR_DecoupeLot_SetInfo>();
            services.AddSingleton<IS_DecoupeLot_UpdateAtStockRelease, SR_DecoupeLot_UpdateAtStockRelease>();
            services.AddSingleton<IS_DecoupeLot_UpdateChariot, SR_DecoupeLot_UpdateChariot>();
            services.AddSingleton<IS_DecoupeLot_UpdateOptimBarNew, SR_DecoupeLot_UpdateOptimBarNew>();
            services.AddSingleton<IS_Settings_UseCase, SR_Settings_UseCase>();
            services.AddSingleton<IS_Stock_DeleteIfQuantityNull, SR_Stock_DeleteIfQuantityNull>();
            services.AddSingleton<IS_Stock_UpdateQuantity, SR_Stock_UpdateQuantity>();
            services.AddSingleton<IS_StockModif_Add, SR_StockModif_Add>();

        }

        private static void RegisterDomainInterfaceServicesUserLogic(IServiceCollection services)
        {
            // UserLogic Services
            services.AddSingleton<IS_Settings_User, SR_Settings_User>();
            services.AddSingleton<IS_User_CheckAppDeviceUser, SR_User_CheckAppDeviceUser>();
            services.AddSingleton<IS_User_InitializePageAccessRights, SR_User_InitializePageAccessRights>();
            services.AddSingleton<IS_User_IsLoginPasswordValid, SR_User_IsLoginPasswordValid>();
            services.AddSingleton<IS_User_UpdateContext, SR_User_UpdateContext>();
            services.AddSingleton<IS_UserApplication_CheckAccess, SR_UserApplication_CheckAccess>();
            services.AddSingleton<IS_UserSession_Close, SR_UserSession_Close>();
            services.AddSingleton<IS_UserSession_Open, SR_UserSession_Open>();
            services.AddSingleton<IS_UserSession_Integrity, SR_UserSession_Integrity>();
            services.AddSingleton<IS_UserSession_Admin, SR_UserSession_Admin>();

        }

        private static void RegisterDomainInterfaceUseCases(IServiceCollection services)
        {
            // Application UseCases AppLogic
            services.AddSingleton<IU_Application_OnStart, UC_Application_OnStart>();
            services.AddSingleton<IU_Page10Dispatch, UC_Page10Dispatch>();
            services.AddSingleton<IU_Page20Dispatch, UC_Page20Dispatch>();
            services.AddSingleton<IU_DB_Monitoring, UC_DB_Monitoring>();

            // Application UseCases BusinessLogic
            services.AddSingleton<IU_BarDropOptim, UC_BarDropOptim>();
            services.AddSingleton<IU_BarDropStockIntegration, UC_BarDropStockIntegration>();
            services.AddSingleton<IU_BarDropStockRelease, UC_BarDropStockRelease>();
            services.AddSingleton<IU_BarNewForceOutOfStock, UC_BarNewForceOutOfStock>();
            services.AddSingleton<IU_BarNewOptim, UC_BarNewOptim>();
            services.AddSingleton<IU_BarNewStockAllocation, UC_BarNewStockAllocation>();
            services.AddSingleton<IU_BarNewStockRelease, UC_BarNewStockRelease>();
            services.AddSingleton<IU_BarNewStockReturn, UC_BarNewStockReturn>();
            services.AddSingleton<IU_BatchDocumentation, UC_BatchDocumentation>();
            services.AddSingleton<IU_CommandeClientAction, UC_CommandeClientAction>();
            services.AddSingleton<IU_CommandeClientStatut, UC_CommandeClientStatut>();
            services.AddSingleton<IU_DecoupeLot_UpdateAtStockRelease, UC_DecoupeLot_UpdateAtStockRelease>();

            // Application UseCases UserLogic
            services.AddSingleton<IU_User_AccessApp, UC_User_AccessApp>();
            services.AddSingleton<IU_User_Authentification, UC_User_Authentification>();
            services.AddSingleton<IU_User_CloseApplication, UC_User_CloseApplication>();

        }

        #endregion


        #region B_UseCases

        #endregion


        #region C_Infrastructure

        private static void RegisterInfrastructurePersistenceGestStock(IServiceCollection services)
        {
            services.AddDbContext<GestStockContext>();
            services.AddSingleton<IDbContextFactory<GestStockContext>, GestStockContextFactory>();
        }

        #endregion


        #region D_Presentation

        private static void RegisterPresentationPagesViewModels(IServiceCollection services)
        {
            services.AddTransient<VM_MainWindow>();
            services.AddTransient<VM_Page00>();
            services.AddTransient<VM_Page10>();
            services.AddTransient<VM_Page20>();
            services.AddTransient<VM_Page21>();
            services.AddTransient<VM_Page22>();
            services.AddTransient<VM_Page23>();
            services.AddTransient<VM_Page30>();
            services.AddTransient<VM_Page31>();
            services.AddTransient<VM_Page32>();
            services.AddTransient<VM_Page33>();
            services.AddTransient<VM_Page40>();
            services.AddTransient<VM_Page41>();
            services.AddTransient<VM_Page50>();
            services.AddTransient<VM_Page60>();
            services.AddTransient<VM_Page70>();
            services.AddTransient<VM_Page90>();
            services.AddTransient<VM_Page91>();
            services.AddTransient<VM_Page96>();
            services.AddTransient<VM_Page97>();
            services.AddTransient<VM_Page98>();
            services.AddTransient<VM_Page99>();

        }

        private static void RegisterPresentationComponentsViewModels(IServiceCollection services)
        {
            services.AddTransient<VM_BA_MainWindow>();
            services.AddTransient<VM_MH_Page10>();
            services.AddTransient<VM_MH_Page20>();
            services.AddTransient<VM_MH_Page21>();
            services.AddTransient<VM_MH_Page22>();
            services.AddTransient<VM_MH_Page23>();
            services.AddTransient<VM_MH_Page30>();
            services.AddTransient<VM_MH_Page31>();
            services.AddTransient<VM_MH_Page32>();
            services.AddTransient<VM_MH_Page33>();
            services.AddTransient<VM_MH_Page40>();
            services.AddTransient<VM_MH_Page41>();
            services.AddTransient<VM_MH_Page50>();
            services.AddTransient<VM_MH_Page60>();
            services.AddTransient<VM_MH_Page70>();
            services.AddTransient<VM_MH_Page90>();
            services.AddTransient<VM_MH_Page91>();
            services.AddTransient<VM_MH_Page96>();
            services.AddTransient<VM_MH_Page97>();
            services.AddTransient<VM_MH_Page98>();
            services.AddTransient<VM_MH_Page99>();
            services.AddTransient<VM_MH_Reduce>();

        }
        #endregion

        private static void RegisterPresentationViews(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
        }

    }
}