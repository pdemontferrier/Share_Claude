namespace BatchStockRelease.D_Presentation.Settings
{
    /// <summary>
    /// Définit les paramètres d’état de la fenêtre principale de l’application WPF.
    /// <para>
    /// Cette classe statique agit comme une zone mémoire partagée (cache global)
    /// pour conserver les dimensions, la position et les marges calculées
    /// de la fenêtre principale.  
    /// Elle est principalement utilisée par le service <see cref="D_Presentation.Services.SR_Window"/>.
    /// </para>
    /// </summary>
    public static class SE_Window
    {
        // ------------------------------------------------------------
        //      Dimensions et position de la fenêtre principale
        // ------------------------------------------------------------

        /// <summary>
        /// Largeur actuelle de la fenêtre principale, en pixels.
        /// </summary>
        public static int MainWindowWidth;

        /// <summary>
        /// Hauteur actuelle de la fenêtre principale, en pixels.
        /// </summary>
        public static int MainWindowHeight;

        /// <summary>
        /// Largeur minimale autorisée de la fenêtre principale.
        /// <para>
        /// Valeur par défaut : <c>1020</c> pixels.  
        /// Définie pour garantir la bonne lisibilité des éléments d’interface.
        /// </para>
        /// </summary>
        public static int MainWindowMinWidth = 1020;

        /// <summary>
        /// Hauteur minimale autorisée de la fenêtre principale.
        /// <para>
        /// Valeur par défaut : <c>680</c> pixels.
        /// </para>
        /// </summary>
        public static int MainWindowMinHeight = 680;

        /// <summary>
        /// Marge verticale ajustée utilisée pour adapter les espacements dynamiques
        /// en fonction de la hauteur de la fenêtre.
        /// <para>
        /// Calculée par <see cref="D_Presentation.Services.SR_Window.UpdateWindowDimensions"/>
        /// selon la formule :  
        /// <c>(MainWindowHeight - MainWindowMinHeight) / 13</c>.
        /// </para>
        /// </summary>
        public static int MainWindowMarginAjusted = 0;

        /// <summary>
        /// Position de la fenêtre principale sur l’axe vertical (Y).
        /// <para>
        /// Correspond au bord supérieur de la fenêtre à l’écran.
        /// </para>
        /// </summary>
        public static double MainWindowTop;

        /// <summary>
        /// Position de la fenêtre principale sur l’axe horizontal (X).
        /// <para>
        /// Correspond au bord gauche de la fenêtre à l’écran.
        /// </para>
        /// </summary>
        public static double MainWindowLeft;
    }
}