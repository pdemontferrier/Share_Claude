namespace BatchStockRelease.B_UseCases.Settings.AppLogic
{
    public static class SE_Dictionary
    {
        // Langue actuellement utilisé par l'application
        public static string AppCultureCode = string.Empty;

        // Dictionaries sources
        private static Uri dic_en = new("B_UseCases/Ressources/Languages/RE_Language.en.xaml", UriKind.Relative);
        private static Uri dic_fr = new("B_UseCases/Ressources/Languages/RE_Language.fr.xaml", UriKind.Relative);
        private static Uri dic_de = new("B_UseCases/Ressources/Languages/RE_Language.de.xaml", UriKind.Relative);
        private static Uri dic_es = new("B_UseCases/Ressources/Languages/RE_Language.es.xaml", UriKind.Relative);
        private static Uri dic_it = new("B_UseCases/Ressources/Languages/RE_Language.it.xaml", UriKind.Relative);
        private static Uri dic_pt = new("B_UseCases/Ressources/Languages/RE_Language.pt.xaml", UriKind.Relative);

        // Dictionaries
        public static Uri Dic_en { get => dic_en; set => dic_en = value; }
        public static Uri Dic_fr { get => dic_fr; set => dic_fr = value; }
        public static Uri Dic_de { get => dic_de; set => dic_de = value; }
        public static Uri Dic_es { get => dic_es; set => dic_es = value; }
        public static Uri Dic_it { get => dic_it; set => dic_it = value; }
        public static Uri Dic_pt { get => dic_pt; set => dic_pt = value; }
    }
}