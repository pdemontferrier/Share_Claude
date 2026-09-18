using System.Windows;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Settings;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_Navigation : IS_Navigation
    {
        private readonly IS_Notification _notification;
        private readonly IS_Settings _settings;

        public SR_Navigation(IS_Notification notification, IS_Settings settings)
        {
            _notification = notification;
            _settings = settings;
        }

        public Uri GetPage00_Source() => SE_Navigation.Page00_Source;
        public Uri GetPage10_Source() => SE_Navigation.Page10_Source;
        public Uri GetPage20_Source() => SE_Navigation.Page20_Source;
        public Uri GetPage21_Source() => SE_Navigation.Page21_Source;
        public Uri GetPage30_Source() => SE_Navigation.Page30_Source;
        public Uri GetPage31_Source() => SE_Navigation.Page31_Source;
        public Uri GetPage40_Source() => SE_Navigation.Page40_Source;
        public Uri GetPage50_Source() => SE_Navigation.Page50_Source;
        public Uri GetPage60_Source() => SE_Navigation.Page60_Source;
        public Uri GetPage70_Source() => SE_Navigation.Page70_Source;
        public Uri GetPage90_Source() => SE_Navigation.Page90_Source;
        public Uri GetPage91_Source() => SE_Navigation.Page91_Source;
        public Uri GetPage96_Source() => SE_Navigation.Page96_Source;
        public Uri GetPage97_Source() => SE_Navigation.Page97_Source;
        public Uri GetPage98_Source() => SE_Navigation.Page98_Source;
        public Uri GetPage99_Source() => SE_Navigation.Page99_Source;


        public Uri GetMH_Page10_Source() => SE_Navigation.MH_Page10_Source;
        public Uri GetMH_Page20_Source() => SE_Navigation.MH_Page20_Source;
        public Uri GetMH_Page21_Source() => SE_Navigation.MH_Page21_Source;
        public Uri GetMH_Page30_Source() => SE_Navigation.MH_Page30_Source;
        public Uri GetMH_Page31_Source() => SE_Navigation.MH_Page31_Source;
        public Uri GetMH_Page40_Source() => SE_Navigation.MH_Page40_Source;
        public Uri GetMH_Page50_Source() => SE_Navigation.MH_Page50_Source;
        public Uri GetMH_Page60_Source() => SE_Navigation.MH_Page60_Source;
        public Uri GetMH_Page70_Source() => SE_Navigation.MH_Page70_Source;
        public Uri GetMH_Page90_Source() => SE_Navigation.MH_Page90_Source;
        public Uri GetMH_Page91_Source() => SE_Navigation.MH_Page91_Source;
        public Uri GetMH_Page96_Source() => SE_Navigation.MH_Page96_Source;
        public Uri GetMH_Page97_Source() => SE_Navigation.MH_Page97_Source;
        public Uri GetMH_Page98_Source() => SE_Navigation.MH_Page98_Source;
        public Uri GetMH_Page99_Source() => SE_Navigation.MH_Page99_Source;
        public Uri GetMH_Reduce_Source() => SE_Navigation.MH_Reduce_Source;


        public string GetPageActual() => SE_Navigation.PageActual;
        public void SetPageActual(string pageName) => SE_Navigation.PageActual = pageName;
        public Uri GetPageActual_Source() => SE_Navigation.PageActual_Source;
        public void SetPageActual_Source(Uri uri) => SE_Navigation.PageActual_Source = uri;
        public Uri GetMHActual_Source() => SE_Navigation.MHActual_Source;
        public void SetMHActual_Source(Uri uri) => SE_Navigation.MHActual_Source = uri;

        // Ajoute une page dans l'historique
        public void PushToNavigationHistory(string pageName)
        {
            SE_Navigation.PageNameNavigationHistory.Push(pageName);
        }

        // Récupère la dernière page visitée
        public string? PopFromNavigationHistory()
        {
            return SE_Navigation.PageNameNavigationHistory.Count > 0
                ? SE_Navigation.PageNameNavigationHistory.Pop()
                : null;
        }

        // Efface l'historique de navigation
        public void ClearNavigationHistory()
        {
            SE_Navigation.PageNameNavigationHistory.Clear();
        }

        // Retourne une copie de l'historique de navigation
        public Stack<string> GetNavigationHistory()
        {
            return new Stack<string>(SE_Navigation.PageNameNavigationHistory);
        }

        // Résout un nom de page en URI
        public Uri? GetPageUri(string pageName)
        {
            return SE_Navigation.PageMappings.TryGetValue(pageName, out var mapping)
                ? mapping.PageUri
                : null;
        }

        // Obtient le mapping d'une page en fonction de son nom de page
        public PageMapping? GetPageMappingByPageName(string pageName)
        {
            return SE_Navigation.PageMappings.TryGetValue(pageName, out var mapping) ? mapping : null;
        }

        // Retourne tous les mappings
        public Dictionary<string, PageMapping> GetAllPageMappings()
        {
            return new Dictionary<string, PageMapping>(SE_Navigation.PageMappings);
        }



        public void NavigateToNewPage(string pageName)
        {
            var mapping = GetPageMappingByPageName(pageName);
            if (mapping == null)
            {
                _notification.Error("No_EC_20", $"Page : {pageName}.");
                return;
            }

            PushToNavigationHistory(GetPageActual());

            if (!CanNavigate(pageName))
            {
                pageName = "Page99";
                mapping = GetPageMappingByPageName(pageName);
                NavigateToUri(GetPage99_Source());
                return;
            }

            SetPageActual(pageName);
            SetPageActual_Source(mapping.PageUri);
            SetMHActual_Source(mapping.MenuUri);
            NavigateToUri(mapping.PageUri);
        }

        public void NavigateToPreviousPage()
        {
            var previousPageName = PopFromNavigationHistory();
            if (!string.IsNullOrEmpty(previousPageName))
            {
                NavigateToNewPage(previousPageName);
            }
            else
            {
                _notification.Warning("No_Wa_10");
            }
        }


        public bool CanNavigate(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanAccess)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanCreate(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanCreate)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanRead(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanRead)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanUpdate(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanUpdate)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanDelete(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanDelete)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanControl(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanControl)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanValidate(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanValidate)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanSupervise(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanSupervise)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanMonitor(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanMonitor)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }

        public bool CanAdmin(string pageName)
        {
            // Récupérer les droits d'accès de la page
            var pageAccessRights = _settings.GetPageRights(pageName);

            // Vérifier si la propriété CanAccess est true
            if (pageAccessRights != null && pageAccessRights.CanAdmin)
            {
                return true;
            }

            // Retourne false si la page n'est pas accessible ou si les droits sont absents
            return false;
        }


        public void RefreshCurrentPage()
        {
            ExecuteOnMainWindow(mainWindow =>
            {
                var currentPage = mainWindow.ActivePage.Content;
                if (currentPage != null)
                {
                    var currentPageType = currentPage.GetType();
                    mainWindow.ActivePage.Navigate(Activator.CreateInstance(currentPageType));
                }
            });
        }

        public void ExpendHorizontalMenu()
        {
            NavigateHorizontalMenu(GetMHActual_Source());
        }

        public void ReduceHorizontalMenu()
        {
            NavigateHorizontalMenu(GetMH_Reduce_Source());
        }

        private void NavigateToUri(Uri uri)
        {
            ExecuteOnMainWindow(mainWindow =>
            {
                mainWindow.ActivePage.Navigate(uri);
                NavigateHorizontalMenu(GetMH_Reduce_Source());
            });
        }

        private void NavigateHorizontalMenu(Uri menuUri)
        {
            ExecuteOnMainWindow(mainWindow => mainWindow.ActiveHorizontalMenu.Navigate(menuUri));
        }

        private void ExecuteOnMainWindow(Action<MainWindow> action)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                action(mainWindow);
            }
        }
    }
}