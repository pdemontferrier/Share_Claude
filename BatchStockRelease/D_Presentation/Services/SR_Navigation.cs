using System.Windows;
using BatchStockRelease.A_Domain.App.Entities;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.D_Presentation.Settings;

namespace BatchStockRelease.D_Presentation.Services
{
    /// <summary>
    /// Interface d’état de navigation, injectée pour découpler l’UI du runtime state
    /// et permettre les tests unitaires (mock de l’état).
    /// </summary>
    public interface INavigationState
    {
        /// <summary>Nom logique de la page courante (ex. "Page10").</summary>
        string CurrentPage { get; set; }

        /// <summary>URI de la page courante.</summary>
        Uri CurrentPageUri { get; set; }

        /// <summary>URI du menu horizontal courant.</summary>
        Uri CurrentMenuUri { get; set; }

        /// <summary>Historique des pages (pile LIFO).</summary>
        Stack<string> History { get; }
    }

    /// <summary>
    /// Implémentation par défaut de l’état de navigation.
    /// </summary>
    public sealed class NavigationState : INavigationState
    {
        public string CurrentPage { get; set; } = SE_Navigation.DefaultPageName;
        public Uri CurrentPageUri { get; set; } = SE_Navigation.DefaultPageUri;
        public Uri CurrentMenuUri { get; set; } = SE_Navigation.DefaultMenuUri;
        public Stack<string> History { get; } = new();
    }

    /// <summary>
    /// Service de navigation centralisé.
    /// <para>
    /// - Orchestration de la navigation entre pages (droits, historique)  
    /// - Synchronisation du menu horizontal  
    /// - État découplé via <see cref="INavigationState"/>
    /// </para>
    /// </summary>
    public class SR_Navigation : IS_Navigation
    {
        private readonly INavigationState _state;
        private readonly IS_Notification _notification;
        private readonly IS_Settings_User _settingsUser;

        /// <summary>Nom du service pour la traçabilité.</summary>
        protected string _callee { get; }

        public SR_Navigation(
            INavigationState state,
            IS_Notification notification,
            IS_Settings_User settingsUser)
        {
            _callee = GetType().Name;

            _state = state;
            _notification = notification;
            _settingsUser = settingsUser;
        }

        #region Accès génériques aux sources (réduisent la duplication)

        /// <summary>
        /// Retourne l’URI de la page par son nom logique (ex. "Page10").
        /// </summary>
        public Uri? GetPageSource(string pageName)
            => SE_Navigation.PageMappings.TryGetValue(pageName, out var m) ? m.PageUri : null;

        /// <summary>
        /// Retourne l’URI du menu horizontal associé à la page (ex. "Page10").
        /// </summary>
        public Uri? GetMenuSource(string pageName)
            => SE_Navigation.PageMappings.TryGetValue(pageName, out var m) ? m.MHUri : null;

        /// <summary>
        /// Retourne le mapping complet d’une page (ou <c>null</c> si inconnue).
        /// </summary>
        public PageMapping? GetPageMappingByPageName(string pageName)
            => SE_Navigation.PageMappings.TryGetValue(pageName, out var m) ? m : null;

        /// <summary>Copie défensive de tous les mappings.</summary>
        public Dictionary<string, PageMapping> GetAllPageMappings()
            => new(SE_Navigation.PageMappings);

        #endregion


        #region État courant (via INavigationState)

        public string GetPageActual() => _state.CurrentPage;
        public void SetPageActual(string pageName) => _state.CurrentPage = pageName;

        public Uri GetPageActual_Source() => _state.CurrentPageUri;
        public void SetPageActual_Source(Uri uri) => _state.CurrentPageUri = uri;

        public Uri GetMHActual_Source() => _state.CurrentMenuUri;
        public void SetMHActual_Source(Uri uri) => _state.CurrentMenuUri = uri;

        #endregion

        #region Historique

        /// <summary>Ajoute une page à l’historique.</summary>
        public void PushToNavigationHistory(string pageName) => _state.History.Push(pageName);

        /// <summary>Récupère la dernière page visitée (ou null si vide).</summary>
        public string? PopFromNavigationHistory()
            => _state.History.Count > 0 ? _state.History.Pop() : null;

        /// <summary>Efface l’historique.</summary>
        public void ClearNavigationHistory() => _state.History.Clear();

        /// <summary>Retourne une copie de l’historique.</summary>
        public Stack<string> GetNavigationHistory() => new(_state.History);

        #endregion

        #region Navigation principale

        /// <summary>
        /// Navigue vers une nouvelle page après validation des droits d’accès.
        /// </summary>
        /// <param name="pageName">Nom logique (ex. "Page10").</param>
        public void NavigateToNewPage(string pageName)
        {
            string callChain = $"{_callee} > {nameof(NavigateToNewPage)}";

            try
            {
                if (!TryGetMapping(pageName, out var mapping))
                {
                    _notification.Error("No_Er_09", $"Page not registered : {pageName}.");
                    return;
                }

                // Empile la page courante avant de naviguer
                PushToNavigationHistory(GetPageActual());

                // Si la page est la page de connexion, on ignore les vérifications de droits
                if (pageName != "Page00")
                {
                    // Vérifie le droit d'accès
                    if (!CanNavigate(pageName))
                    {
                        HandleUnauthorizedAccess(pageName);
                        return;
                    }
                }

                PerformNavigation(pageName, mapping!.Value);
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_20", $"{pageName} : {ex.Message} - {callChain}");
            }
        }

        /// <summary>
        /// Navigation vers la page précédente si disponible.
        /// </summary>
        public void NavigateToPreviousPage()
        {
            var previous = PopFromNavigationHistory();
            if (!string.IsNullOrWhiteSpace(previous))
            {
                NavigateToNewPage(previous);
            }
            else
            {
                _notification.Warning("No_Wa_10");
            }
        }

        private bool TryGetMapping(string pageName, out PageMapping? mapping)
        {
            mapping = GetPageMappingByPageName(pageName);
            return mapping != null;
        }

        private void HandleUnauthorizedAccess(string attemptedPage)
        {
            _notification.Warning("No_Wa_04", $"Accès refusé à {attemptedPage}. Redirection vers Page99.");
            var fallback = GetPageMappingByPageName("Page99");
            if (fallback is not null)
                PerformNavigation("Page99", fallback.Value);
        }

        private void PerformNavigation(string pageName, PageMapping mapping)
        {
            SetPageActual(pageName);
            SetPageActual_Source(mapping.PageUri);
            SetMHActual_Source(mapping.MHUri);
            NavigateToUri(mapping.PageUri);
        }

        #endregion

        #region Gestion des droits

        /// <summary>
        /// Vérifie de manière générique un droit sur une page donnée.
        /// </summary>
        private bool HasRight(string pageName, Func<PageRights, bool> predicate)
        {
            var rights = _settingsUser.GetPageRights(pageName);
            return rights is not null && predicate(rights);
        }

        public bool CanNavigate(string pageName) => HasRight(pageName, r => r.CanAccess);
        public bool CanCreate(string pageName) => HasRight(pageName, r => r.CanCreate);
        public bool CanRead(string pageName) => HasRight(pageName, r => r.CanRead);
        public bool CanUpdate(string pageName) => HasRight(pageName, r => r.CanUpdate);
        public bool CanDelete(string pageName) => HasRight(pageName, r => r.CanDelete);
        public bool CanControl(string pageName) => HasRight(pageName, r => r.CanControl);
        public bool CanValidate(string pageName) => HasRight(pageName, r => r.CanValidate);
        public bool CanSupervise(string pageName) => HasRight(pageName, r => r.CanSupervise);
        public bool CanMonitor(string pageName) => HasRight(pageName, r => r.CanMonitor);
        public bool CanAdmin(string pageName) => HasRight(pageName, r => r.CanAdmin);

        #endregion

        #region Intégration WPF

        public void RefreshCurrentPage()
        {
            ExecuteOnMainWindow(mainWindow =>
            {
                var current = mainWindow.ActivePage.Content;
                if (current is not null)
                {
                    var t = current.GetType();
                    mainWindow.ActivePage.Navigate(Activator.CreateInstance(t));
                }
            });
        }

        public void ExpendHorizontalMenu() => NavigateHorizontalMenu(GetMHActual_Source());
        public void ReduceHorizontalMenu() => NavigateHorizontalMenu(SE_Navigation.MH_Reduce_Source);

        private void NavigateToUri(Uri uri)
        {
            ExecuteOnMainWindow(mainWindow =>
            {
                mainWindow.ActivePage.Navigate(uri);
                NavigateHorizontalMenu(SE_Navigation.MH_Reduce_Source);
            });
        }

        private void NavigateHorizontalMenu(Uri menuUri)
            => ExecuteOnMainWindow(mainWindow => mainWindow.ActiveHorizontalMenu.Navigate(menuUri));

        private void ExecuteOnMainWindow(Action<MainWindow> action)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null) return;

            if (dispatcher.CheckAccess())
            {
                // Déjà sur le thread UI
                if (Application.Current.MainWindow is MainWindow mainWindow)
                    action(mainWindow);
            }
            else
            {
                // Appelé depuis un thread de fond
                dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                        action(mainWindow);
                }));
            }
        }

        #endregion
    }
}