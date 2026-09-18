using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;
using BatchCutting_DG.B_UseCases.Handlers.Commands;
using BatchCutting_DG.B_UseCases.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Services.App;
using BatchCutting_DG.B_UseCases.Services.BusinessLogic;
using BatchCutting_DG.B_UseCases.Services.UserLogic;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.D_Presentation.Services;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;
using BatchCutting_DG.C_Infrastructure.Services.GestStock;
using BatchCutting_DG.C_Infrastructure.QueryDataProviders.GestStock;


namespace BatchCutting_DG.E_CompositionRoot.Services
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

            // B_UseCases

            // C_Infrastructure
            ConfigureDbContext(services);

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
            services.AddScoped<IC_ChutesMagasin, CH_ChutesMagasin>();
            services.AddScoped<IC_CommandeClient, CH_CommandeClient>();
            services.AddScoped<IC_CommandeClientAction, CH_CommandeClientAction>();
            services.AddScoped<IC_CommandeClientModification, CH_CommandeClientModification>();
            services.AddScoped<IC_CommandeClientStatut, CH_CommandeClientStatut>();
            services.AddScoped<IC_DecoupeBarre, CH_DecoupeBarre>();
            services.AddScoped<IC_DecoupeDetail, CH_DecoupeDetail>();
            services.AddScoped<IC_DecoupeLot, CH_DecoupeLot>();
            services.AddScoped<IC_UserAppEventStore, CH_UserAppEventStore>();
            services.AddScoped<IC_UserAppMessage, CH_UserAppMessage>();
            services.AddScoped<IC_UserSession, CH_UserSession>();
            services.AddScoped<IC_UserSessionCommand, CH_UserSessionCommand>();

            // Queries
            services.AddScoped<IQ_ArticleInterne, QH_ArticleInterne>();
            services.AddScoped<IQ_ChutesMagasin, QH_ChutesMagasin>();
            services.AddScoped<IQ_CommandeClient, QH_CommandeClient>();
            services.AddScoped<IQ_CommandeClientAction, QH_CommandeClientAction>();
            services.AddScoped<IQ_CommandeClientActionType, QH_CommandeClientActionType>();
            services.AddScoped<IQ_CommandeClientModification, QH_CommandeClientModification>();
            services.AddScoped<IQ_CommandeClientStatut, QH_CommandeClientStatut>();
            services.AddScoped<IQ_DecoupeBarre, QH_DecoupeBarre>();
            services.AddScoped<IQ_DecoupeBarreWithCut, QH_DecoupeBarreWithCut>();
            services.AddScoped<IQ_DecoupeDetail, QH_DecoupeDetail>();
            services.AddScoped<IQ_DecoupeDetailWithCut, QH_DecoupeDetailWithCut>();
            services.AddScoped<IQ_DecoupeLot, QH_DecoupeLot>();
            services.AddScoped<IQ_DecoupeLotWithCut, QH_DecoupeLotWithCut>();
            services.AddScoped<IQ_DecoupeMachine, QH_DecoupeMachine>();
            services.AddScoped<IQ_User, QH_User>();
            services.AddScoped<IQ_UserAppMessage, QH_UserAppMessage>();
            services.AddScoped<IQ_UserAppPageDroit, QH_UserAppPageDroit>();
            services.AddScoped<IQ_UserDroit, QH_UserDroit>();
            services.AddScoped<IQ_UserSession, QH_UserSession>();
            services.AddScoped<IQ_UserSessionCommand, QH_UserSessionCommand>();
            services.AddScoped<IQ_UserSessionDetails, QH_UserSessionDetails>();
            services.AddScoped<IQ_VieApplication, QH_VieApplication>();
            services.AddScoped<IQ_VieChuteMagasinReference, QH_VieChuteMagasinReference>();

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
            services.AddScoped<IR_DecoupeMachine, CR_DecoupeMachine>();
            services.AddScoped<IR_User, CR_User>();
            services.AddScoped<IR_UserAppMessage, CR_UserAppMessage>();
            services.AddScoped<IR_UserAppPageDroit, CR_UserAppPageDroit>();
            services.AddScoped<IR_UserDroit, CR_UserDroit>();
            services.AddScoped<IR_UserSessionCommand, CR_UserSessionCommand>();
            services.AddScoped<IR_UserSession, CR_UserSession>();
            services.AddScoped<IR_VieApplication, CR_VieApplication>();
            services.AddScoped<IR_VieChuteMagasinReference, CR_VieChuteMagasinReference>();

            // DP pour les DTO
            services.AddScoped<IR_DecoupeBarreWithCut, DP_DecoupeBarreWithCut>();
            services.AddScoped<IR_DecoupeDetailWithCut, DP_DecoupeDetailWithCut>();
            services.AddScoped<IR_DecoupeLotWithCut, DP_DecoupeLotWithCut>();
            services.AddScoped<IR_UserSessionDetails, DP_UserSessionDetail>();
        }

        private static void RegisterDomainInterfaceServicesApp(IServiceCollection services)
        {
            // Application Services
            services.AddSingleton<IS_Application, SR_Application>();
            services.AddSingleton<IS_ControlStyler, SR_ControlStyler>();
            services.AddSingleton<IS_Dictionary, SR_Dictionary>();
            services.AddSingleton<IS_Flag, SR_Flag>();
            services.AddSingleton<IS_Icons, SR_Icons>();
            services.AddSingleton<IS_Language, SR_Language>();
            services.AddSingleton<IS_Messages, SR_Messages>();
            services.AddSingleton<IS_Navigation, SR_Navigation>();
            services.AddSingleton<IS_Notification, SR_Notification>();
            services.AddSingleton<IS_OnStart, SR_OnStart>();
            services.AddSingleton<IS_ReferenceVue, SR_ReferenceVue>();
            services.AddSingleton<IS_Settings, SR_Settings>();
            services.AddSingleton<IS_Utilities, SR_Utilities>();
            services.AddSingleton<IS_Window, SR_Window>();
        }

        private static void RegisterDomainInterfaceServicesBusinessLogic(IServiceCollection services)
        {
            // BusinessLogic Services
            services.AddSingleton<IS_BarDropOptim, SR_BarDropOptim>();
            services.AddSingleton<IS_BarNewOptim, SR_BarNewOptim>();
            services.AddSingleton<IS_CommandeClientAction, SR_CommandeClientAction>();
            services.AddSingleton<IS_CuttingMachineIdentification, SR_DecoupeMachineIdentification>();
            services.AddSingleton<IS_DataBase, SR_DataBase>();
            services.AddSingleton<IS_Decoupe, SR_Decoupe>();
            services.AddSingleton<IS_LabelPrinter, SR_LabelPrinter>();
            services.AddSingleton<IS_SerialSender, SR_SerialSender>();

        }

        private static void RegisterDomainInterfaceServicesUserLogic(IServiceCollection services)
        {
            // UserLogic Services
            services.AddSingleton<IS_UserAuthentification, SR_UserAuthentification>();
            services.AddSingleton<IS_UserSession, SR_UserSession>();
            services.AddSingleton<IS_UserSessionsAdmin, SR_UserSessionsAdmin>();
            services.AddSingleton<IS_UserSettings, SR_UserAccess>();

        }

        #endregion


        #region B_UseCases

        #endregion


        #region C_Infrastructure

        private static void ConfigureDbContext(IServiceCollection services)
        {
            services.AddDbContext<GestStockContext>();
            services.AddSingleton<IDbContextFactory<GestStockContext>, GestStockContextFactory>();
        }
        #endregion


        #region D_Presentation

        private static void RegisterPresentationPagesViewModels(IServiceCollection services)
        {
            services.AddTransient<VM_Page00>();
            services.AddTransient<VM_Page10>();
            services.AddTransient<VM_Page20>();
            services.AddTransient<VM_Page21>();
            services.AddTransient<VM_Page22>();
            services.AddTransient<VM_Page30>();
            services.AddTransient<VM_Page31>();
            services.AddTransient<VM_Page40>();
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
            services.AddTransient<VM_MH_Page10>();
            services.AddTransient<VM_MH_Page20>();
            services.AddTransient<VM_MH_Page21>();
            services.AddTransient<VM_MH_Page22>();
            services.AddTransient<VM_MH_Page30>();
            services.AddTransient<VM_MH_Page31>();
            services.AddTransient<VM_MH_Page40>();
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