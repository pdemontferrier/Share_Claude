
namespace BatchStockRelease.D_Presentation.Settings
{
    /// <summary>
    /// Configuration centralisée de la navigation (URIs et mappings).
    /// <para>
    /// Cette classe est <b>stateless</b> : elle ne contient <b>aucun état d’exécution</b>
    /// (page courante, historique, etc.). Elle expose uniquement les URIs et le
    /// dictionnaire des mappings, générés automatiquement à partir de conventions.
    /// </para>
    /// </summary>
    public static class SE_Navigation
    {
        /// <summary>Chemin des pages (pattern).</summary>
        private const string PagePathPattern = "D_Presentation/Views/Pages/Page{0}.xaml";

        /// <summary>Chemin des menus horizontaux (pattern).</summary>
        private const string MenuPathPattern = "D_Presentation/Views/Components/MenuHorizontal/MH_Page{0}.xaml";

        /// <summary>URI du menu réduit (commun).</summary>
        public static readonly Uri MH_Reduce_Source = new Uri(
            "D_Presentation/Views/Components/MenuHorizontal/MH_Reduce.xaml",
            UriKind.Relative);

        /// <summary>
        /// Mapping complet des pages de l’application.
        /// <para>
        /// La génération est automatique à partir de conventions :  
        /// - fichiers <c>PageNN.xaml</c> pour les vues  
        /// - fichiers <c>MH_PageNN.xaml</c> pour les menus horizontaux  
        /// - <c>MVName</c> dérivé du premier chiffre (<c>MV1</c>, <c>MV2</c>, …)
        /// </para>
        /// </summary>
        public static readonly Dictionary<string, PageMapping> PageMappings;

        /// <summary>Nom logique de la page par défaut.</summary>
        public const string DefaultPageName = "Page10";

        /// <summary>URI de la page par défaut.</summary>
        public static Uri DefaultPageUri => PageMappings[DefaultPageName].PageUri;

        /// <summary>URI du menu horizontal par défaut.</summary>
        public static Uri DefaultMenuUri => PageMappings[DefaultPageName].MHUri;

        static SE_Navigation()
        {
            // Déclarer la liste des pages connues pour l’application
            var pages = new[]
            {
                "00",
                "10",
                "20","21","22","23",
                "30","31","32","33",
                "40","41",
                "50",
                "60",
                "70",
                "90","91","96","97","98","99"
            };

            // Génération automatique du dictionnaire
            PageMappings = pages.ToDictionary(
                p => $"Page{p}",
                p =>
                {
                    var pageName = $"Page{p}";
                    var pageUri = new Uri(string.Format(PagePathPattern, p), UriKind.Relative);
                    var mhUri = new Uri(string.Format(MenuPathPattern, p), UriKind.Relative);
                    var mvName = $"MV{(p.Length > 0 ? p[0] : '0')}";

                    return new PageMapping(pageName, pageUri, mhUri, mvName);
                });

            // Exceptions / ajustements : Page00 → menu réduit par design
            if (PageMappings.TryGetValue("Page00", out var m00))
                PageMappings["Page00"] = m00 with { MHUri = MH_Reduce_Source };
        }
    }

    /// <summary>
    /// Description d’une page : nom logique, URIs et MV associé.
    /// </summary>
    /// <param name="PageName">Nom logique (ex. "Page10").</param>
    /// <param name="PageUri">URI de la vue XAML.</param>
    /// <param name="MHUri">URI du menu horizontal correspondant.</param>
    /// <param name="MVName">Nom du ViewModel/région logique (ex. "MV1").</param>
    public readonly record struct PageMapping(
        string PageName,
        Uri PageUri,
        Uri MHUri,
        string MVName
    );
}