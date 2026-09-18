namespace BatchCutting_DG.D_Presentation.Settings
{
    public static class SE_Navigation
    {

        // Pages Sources
        public static Uri Page00_Source = new Uri("D_Presentation/Views/Pages/Page00.xaml", UriKind.Relative);
        public static Uri Page10_Source = new Uri("D_Presentation/Views/Pages/Page10.xaml", UriKind.Relative);
        public static Uri Page20_Source = new Uri("D_Presentation/Views/Pages/Page20.xaml", UriKind.Relative);
        public static Uri Page21_Source = new Uri("D_Presentation/Views/Pages/Page21.xaml", UriKind.Relative);
        public static Uri Page22_Source = new Uri("D_Presentation/Views/Pages/Page22.xaml", UriKind.Relative);
        public static Uri Page30_Source = new Uri("D_Presentation/Views/Pages/Page30.xaml", UriKind.Relative);
        public static Uri Page31_Source = new Uri("D_Presentation/Views/Pages/Page31.xaml", UriKind.Relative);
        public static Uri Page40_Source = new Uri("D_Presentation/Views/Pages/Page40.xaml", UriKind.Relative);
        public static Uri Page50_Source = new Uri("D_Presentation/Views/Pages/Page50.xaml", UriKind.Relative);
        public static Uri Page60_Source = new Uri("D_Presentation/Views/Pages/Page60.xaml", UriKind.Relative);
        public static Uri Page70_Source = new Uri("D_Presentation/Views/Pages/Page70.xaml", UriKind.Relative);
        public static Uri Page90_Source = new Uri("D_Presentation/Views/Pages/Page90.xaml", UriKind.Relative);
        public static Uri Page91_Source = new Uri("D_Presentation/Views/Pages/Page91.xaml", UriKind.Relative);
        public static Uri Page96_Source = new Uri("D_Presentation/Views/Pages/Page96.xaml", UriKind.Relative);
        public static Uri Page97_Source = new Uri("D_Presentation/Views/Pages/Page97.xaml", UriKind.Relative);
        public static Uri Page98_Source = new Uri("D_Presentation/Views/Pages/Page98.xaml", UriKind.Relative);
        public static Uri Page99_Source = new Uri("D_Presentation/Views/Pages/Page99.xaml", UriKind.Relative);
        public static Uri PageUser_Source = new Uri("D_Presentation/Views/Pages/PageUser.xaml", UriKind.Relative);

        // Menu Horizontal Sources
        public static Uri MH_Page10_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page10.xaml", UriKind.Relative);
        public static Uri MH_Page20_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page20.xaml", UriKind.Relative);
        public static Uri MH_Page21_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page21.xaml", UriKind.Relative);
        public static Uri MH_Page22_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page22.xaml", UriKind.Relative);
        public static Uri MH_Page30_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page30.xaml", UriKind.Relative);
        public static Uri MH_Page31_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page31.xaml", UriKind.Relative);
        public static Uri MH_Page40_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page40.xaml", UriKind.Relative);
        public static Uri MH_Page50_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page50.xaml", UriKind.Relative);
        public static Uri MH_Page60_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page60.xaml", UriKind.Relative);
        public static Uri MH_Page70_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page70.xaml", UriKind.Relative);
        public static Uri MH_Page90_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page90.xaml", UriKind.Relative);
        public static Uri MH_Page91_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page91.xaml", UriKind.Relative);
        public static Uri MH_Page96_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page96.xaml", UriKind.Relative);
        public static Uri MH_Page97_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page97.xaml", UriKind.Relative);
        public static Uri MH_Page98_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page98.xaml", UriKind.Relative);
        public static Uri MH_Page99_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Page99.xaml", UriKind.Relative);
        public static Uri MH_Reduce_Source = new Uri("D_Presentation/Views/Components/MenuHorizontal/MH_Reduce.xaml", UriKind.Relative);

        // Mapping des sources des composants d'une page
        public static readonly Dictionary<string, PageMapping> PageMappings = new()
        {
            { "Page00", new PageMapping("Page00", Page00_Source, MH_Reduce_Source, "MV0") },
            { "Page10", new PageMapping("Page10", Page10_Source, MH_Page10_Source, "MV1") },
            { "Page20", new PageMapping("Page20", Page20_Source, MH_Page20_Source, "MV2") },
            { "Page21", new PageMapping("Page21", Page21_Source, MH_Page21_Source, "MV2") },
            { "Page22", new PageMapping("Page22", Page22_Source, MH_Page22_Source, "MV2") },
            { "Page30", new PageMapping("Page30", Page30_Source, MH_Page30_Source, "MV3") },
            { "Page31", new PageMapping("Page31", Page31_Source, MH_Page31_Source, "MV3") },
            { "Page40", new PageMapping("Page40", Page40_Source, MH_Page40_Source, "MV4") },
            { "Page50", new PageMapping("Page50", Page50_Source, MH_Page50_Source, "MV5") },
            { "Page60", new PageMapping("Page60", Page60_Source, MH_Page60_Source, "MV6") },
            { "Page70", new PageMapping("Page70", Page70_Source, MH_Page70_Source, "MV7") },
            { "Page90", new PageMapping("Page90", Page90_Source, MH_Page90_Source, "MV9") },
            { "Page91", new PageMapping("Page91", Page91_Source, MH_Page91_Source, "MV9") },
            { "Page96", new PageMapping("Page96", Page96_Source, MH_Page96_Source, "MV9") },
            { "Page97", new PageMapping("Page97", Page97_Source, MH_Page97_Source, "MV9") },
            { "Page98", new PageMapping("Page98", Page98_Source, MH_Page98_Source, "MV9") },
            { "Page99", new PageMapping("Page99", Page99_Source, MH_Page99_Source, "MV9") }
        };

        // Navigation
        public static string PageActual = "Page10";
        public static Uri PageActual_Source = Page10_Source;
        public static Uri MHActual_Source = MH_Page10_Source;

        // Historique de navigation
        public static Stack<string> PageNameNavigationHistory { get; } = new Stack<string>();
    }

    public class PageMapping
    {
        public string PageName { get; set; }
        public Uri PageUri { get; set; }
        public Uri MenuUri { get; set; }
        public string MVName { get; set; }

        public PageMapping(string pageName, Uri pageUri, Uri menuUri, string mVName)
        {
            PageName = pageName;
            PageUri = pageUri;
            MenuUri = menuUri;
            MVName = mVName;
        }
    }
}
