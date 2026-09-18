using System.Windows.Media;
using System.IO;

namespace CommonResources.Settings
{
    public static class CR_CommonSettings
    {
        // Titre de l'application
        public static string ApplicationTitle = "Titre de l'application";

        // Main Window dimensions
        public static int W_MinimumWidth = 1280;
        public static int W_MinimumHeight = 720 + 35;
        public static double W_Ratio = 1.44;

        // Global
        public static Color BorderColor = (Color)ColorConverter.ConvertFromString("#0472B9");
        public static int BorderThickness = 1;
        public static FontFamily ApplicationFont = new FontFamily(new Uri("pack://application:,,,/CommonResources;component/"), "./Fonts/#Suisse Int'l");
        public static FontFamily ApplicationFontBold = new FontFamily(new Uri("pack://application:,,,/CommonResources;component/"), "./Fonts/#Suisse Int'l Bold");

        // Background effects
        public static int BackgroundCornerRadius = 20;
        public static int BackgroundMargin = 0;
        public static int BackgroundPadding = 0;
        public static double BackgroundOpacity = 0.9;

        // Background_0
        public static Color BackgroundColor_0 = (Color)ColorConverter.ConvertFromString("#FFFFFF");

        // Background_1
        public static Color BackgroundColor_1 = (Color)ColorConverter.ConvertFromString("#29293D"); //Base XD : #33334C  - Utilisé : #29293D

        // Background_2
        public static Color BackgroundColor_2 = BackgroundColor_1;
        public static int BackgroundWidth_2 = 250;

        // Title
        public static Color TitleColor = BackgroundColor_1;
        public static int TitleHeight = 70;
        public static int TitleMarginTop = 40;

        // Logo
        public static Uri LogoSource = new Uri("/CommonResources;component/Logo/logo.png", UriKind.Relative);
        public static int LogoWidth = 160;
        public static int LogoHeight = 60;
        public static int LogoLeftMargin = 45;

        // Text
        public static Color TextColor_1 = (Color)ColorConverter.ConvertFromString("#FFFFFF"); // Withe
        public static Color TextColor_2 = (Color)ColorConverter.ConvertFromString("#BFBFBF"); // Gray
        public static Color TextColor_3 = (Color)ColorConverter.ConvertFromString("#0c9bea"); // Blue
        public static Color TextColor_Red = (Color)ColorConverter.ConvertFromString("#FF3E3E"); // Red
        public static Color TextColor_Green = (Color)ColorConverter.ConvertFromString("#59C64A"); // Green
        public static Color TextColor_Orange = (Color)ColorConverter.ConvertFromString("#ffb245"); // Orange
        public static int TextSize_1 = 20;
        public static int TextSize_2 = 15;
        public static int TextSize_3 = 12;
        public static int TextSize_4 = 10;

        // Icon
        public static int Icon_Size = 25;

        // Menu Horizontal Reduce
        public static int MHR_Width = 100;
        public static int MHR_Height = 61;
        public static int MHR_Margin = 10;
        public static int MHR_MarginTop = 44;

        // Button
        public static int Button_MinWidth = 80;
        public static int Button_MinHeight = 50;
        public static double Button_Opacity = 0.8;
        public static Uri MHR_Button_Icon_Source = CR_IconsSettings.IconMenu;
        // MHR Button Text
        public static string MHR_Button_Text = "Menu";

        // Page
        public static int PageMarginTop = TitleMarginTop + TitleHeight + 50;
        public static int PageMarginLeft = BackgroundWidth_2 + 20 + 15;
        public static int PageMarginRight = 20;
        public static int PageMarginBottom = 20;

        // Menu Vertical
        public static int MV_MarginTop = 150;

        // Menu Vertical Buttons
        public static int MV_ButtonHeight = 40;
        public static int MV_ButtonWidth = BackgroundWidth_2;
        public static int MV_ButtonMinWidth = 190;
        public static int MV_ButtonMargin = 15;
        public static int MV_ButtonIconSize = Icon_Size;

        // Dialog window
        public static string DW_Title = "A déterminer";
        public static string DW_Content = "A déterminer";
        public static int DW_Width = 500;
        public static int DW_Height = 350;
        public static Uri DW_Icon = CR_IconsSettings.IconWarningTriangleOrange;

        // Dossier Réseau
        public static readonly string DirectoryPathServer1 = @"\\gunder07\chronos\"; // Z:
        public static readonly string DirectoryPathServer2 = @"\\mertzwi3\veranda\"; // L:

        // Dossier GED Projet
        public static readonly string DirectoryPathGed1 = @"App GestStock\data\";
        public static readonly string DirectoryPathGed2 = @"cmd_client\";
        public static readonly string DirectoryPathGed3 = @"\Liste de debit\";
        public static readonly string DirectoryPathProjectModel = @"models\Modele_Projet\Modele_Liste_Debit.xlsm";

        // Dossier GED Lot
        public static readonly string DirectoryPathBatchModel = Path.Combine(DirectoryPathServer1, DirectoryPathGed1, @"models\Modele_Lot\Modele_Liste_Debit.xlsm");
        public static readonly string DirectoryPathLotsDeFabrication = @"LOTS_DE_FABRICATION\01_DECOUPE\";
        public static readonly string DirectoryPathElumatec = @"dossier fab\Machine elumatec\";
        public static readonly string DirectoryPathElumatec1 = @"Elumatec1\";
        public static readonly string DirectoryPathElumatec2 = @"Elumatec2\";
        public static readonly string DirectoryPathElumatec3 = @"Elumatec3\";

    }
}