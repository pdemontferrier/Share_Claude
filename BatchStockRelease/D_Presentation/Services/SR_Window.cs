using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.Settings;
using ScreenHelperLibrary;
using System.Windows;
using System.Windows.Forms;

namespace BatchStockRelease.D_Presentation.Services
{
    /// <summary>
    /// Service responsable de la gestion des dimensions et de la position
    /// de la fenêtre principale de l’application WPF.
    /// <para>
    /// Ce service synchronise les valeurs runtime avec les paramètres
    /// statiques définis dans <see cref="SE_Window"/> et tient compte
    /// du facteur d’échelle et de l’écran actif.
    /// </para>
    /// </summary>
    public class SR_Window : IS_Window
    {
        #region Accesseurs standard vers SE_Window
        public int GetMainWindowWidth() => SE_Window.MainWindowWidth;
        public void SetMainWindowWidth(int value) => SE_Window.MainWindowWidth = value;

        public int GetMainWindowHeight() => SE_Window.MainWindowHeight;
        public void SetMainWindowHeight(int value) => SE_Window.MainWindowHeight = value;

        public int GetMainWindowMinWidth() => SE_Window.MainWindowMinWidth;
        public int GetMainWindowMinHeight() => SE_Window.MainWindowMinHeight;

        public int GetMainWindowMarginAjusted() => SE_Window.MainWindowMarginAjusted;
        public void SetMainWindowMarginAjusted(int value) => SE_Window.MainWindowMarginAjusted = value;

        public double GetMainWindowTop() => SE_Window.MainWindowTop;
        public void SetMainWindowTop(double value) => SE_Window.MainWindowTop = value;

        public double GetMainWindowLeft() => SE_Window.MainWindowLeft;
        public void SetMainWindowLeft(double value) => SE_Window.MainWindowLeft = value;
        #endregion

        /// <summary>
        /// Met à jour les dimensions, la position et le facteur de mise à l’échelle
        /// de la fenêtre principale.
        /// </summary>
        /// <param name="mainWindow">Référence à la fenêtre principale active.</param>
        /// <remarks>
        /// Cette méthode :
        /// <list type="bullet">
        /// <item><description>Synchronise la taille et la position de la fenêtre avec <see cref="SE_Window"/>.</description></item>
        /// <item><description>Détermine l’écran actif et ajuste la position si la fenêtre est maximisée.</description></item>
        /// <item><description>Recalcule la marge verticale d’ajustement en fonction de la hauteur courante.</description></item>
        /// <item><description>Met à jour les informations système via <see cref="ScreenHelper.UpdateScreenDimensions"/>.</description></item>
        /// </list>
        /// </remarks>
        public void UpdateWindowDimensions(MainWindow mainWindow)
        {
            if (mainWindow == null)
                return;

            // Déterminer l’écran actif
            var handle = new System.Windows.Interop.WindowInteropHelper(mainWindow).Handle;
            var screen = Screen.FromHandle(handle);

            // Actualiser les valeurs de base selon l’état de la fenêtre
            if (mainWindow.WindowState == WindowState.Maximized)
            {
                UpdateFromMaximized(mainWindow, screen);
            }
            else
            {
                UpdateFromNormal(mainWindow);
            }

            // Calculer la marge verticale ajustée selon la hauteur
            UpdateMarginAdjusted();

            // Mettre à jour les informations de l’écran et le facteur de mise à l’échelle
            ScreenHelper.UpdateScreenDimensions(handle);
        }

        #region Méthodes privées utilitaires

        /// <summary>
        /// Met à jour les dimensions et la position lorsque la fenêtre est maximisée.
        /// </summary>
        private static void UpdateFromMaximized(Window mainWindow, Screen screen)
        {
            SE_Window.MainWindowWidth = (int)mainWindow.ActualWidth;
            SE_Window.MainWindowHeight = (int)mainWindow.ActualHeight;
            SE_Window.MainWindowTop = screen.WorkingArea.Top;
            SE_Window.MainWindowLeft = screen.WorkingArea.Left;

            // Correction de sécurité (certaines configurations retournent des positions négatives)
            if (SE_Window.MainWindowTop < 0)
                SE_Window.MainWindowTop = 0;
        }

        /// <summary>
        /// Met à jour les dimensions et la position lorsque la fenêtre est en mode normal.
        /// </summary>
        private static void UpdateFromNormal(Window mainWindow)
        {
            SE_Window.MainWindowWidth = (int)mainWindow.ActualWidth;
            SE_Window.MainWindowHeight = (int)mainWindow.ActualHeight;
            SE_Window.MainWindowTop = mainWindow.Top;
            SE_Window.MainWindowLeft = mainWindow.Left;
        }

        /// <summary>
        /// Calcule la marge verticale ajustée en fonction de la hauteur actuelle de la fenêtre.
        /// </summary>
        private static void UpdateMarginAdjusted()
        {
            var height = SE_Window.MainWindowHeight;
            var minHeight = SE_Window.MainWindowMinHeight;

            if (minHeight <= 0) // Sécurité : éviter une division par zéro
            {
                SE_Window.MainWindowMarginAjusted = 0;
                return;
            }

            // Formule empirique : 1/13 de la différence entre la hauteur réelle et la hauteur minimale
            var adjustedMargin = (int)((height - minHeight) / 13.0);
            SE_Window.MainWindowMarginAjusted = adjustedMargin > 0 ? adjustedMargin : 0;
        }

        #endregion
    }
}