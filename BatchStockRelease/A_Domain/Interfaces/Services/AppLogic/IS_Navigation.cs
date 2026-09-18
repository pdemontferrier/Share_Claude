
using BatchStockRelease.D_Presentation.Settings;

namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    public interface IS_Navigation
    {
        Uri? GetPageSource(string pageName);
        Uri? GetMenuSource(string pageName);
        PageMapping? GetPageMappingByPageName(string pageName);
        Dictionary<string, PageMapping> GetAllPageMappings();


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
