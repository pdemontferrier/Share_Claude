
using BatchCutting_DG.D_Presentation.Settings;

namespace BatchCutting_DG.A_Domain.Interfaces.Services.App
{
    public interface IS_Navigation
    {
        Uri GetPage00_Source();
        Uri GetPage10_Source();
        Uri GetPage20_Source();
        Uri GetPage30_Source();
        Uri GetPage40_Source();
        Uri GetPage50_Source();
        Uri GetPage60_Source();
        Uri GetPage70_Source();
        Uri GetPage90_Source();
        Uri GetPage91_Source();
        Uri GetPage96_Source();
        Uri GetPage97_Source();
        Uri GetPage98_Source();
        Uri GetPage99_Source();


        Uri GetMH_Page10_Source();
        Uri GetMH_Page20_Source();
        Uri GetMH_Page30_Source();
        Uri GetMH_Page40_Source();
        Uri GetMH_Page50_Source();
        Uri GetMH_Page60_Source();
        Uri GetMH_Page70_Source();
        Uri GetMH_Page90_Source();
        Uri GetMH_Page91_Source();
        Uri GetMH_Page96_Source();
        Uri GetMH_Page97_Source();
        Uri GetMH_Page98_Source();
        Uri GetMH_Page99_Source();
        Uri GetMH_Reduce_Source();


        string GetPageActual();
        void SetPageActual(string pageName);
        Uri GetPageActual_Source();
        void SetPageActual_Source(Uri uri);
        Uri GetMHActual_Source();
        void SetMHActual_Source(Uri uri);


        void PushToNavigationHistory(string pageName);
        string? PopFromNavigationHistory();
        void ClearNavigationHistory();
        Stack<string> GetNavigationHistory();


        Uri? GetPageUri(string pageName);
        PageMapping? GetPageMappingByPageName(string pageName);
        Dictionary<string, PageMapping> GetAllPageMappings();


        void NavigateToNewPage(string pageName);
        void NavigateToPreviousPage();


        bool CanNavigate(string pageName);
        bool CanCreate(string pageName);
        bool CanRead(string pageName);
        bool CanUpdate(string pageName);
        bool CanDelete(string pageName);
        bool CanControl(string pageName);
        bool CanValidate(string pageName);
        bool CanSupervise(string pageName);
        bool CanMonitor(string pageName);
        bool CanAdmin(string pageName);


        void RefreshCurrentPage();
        void ExpendHorizontalMenu();
        void ReduceHorizontalMenu();

    }
}
