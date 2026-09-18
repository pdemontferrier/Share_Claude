using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Settings;
using ScreenHelperLibrary;
using System.Windows.Forms;
using System.Windows;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_Window : IS_Window
    {
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

        public void UpdateWindowDimensions(MainWindow mainWindow)
        {
            if (mainWindow.WindowState == WindowState.Maximized)
            {
                // Utiliser l'écran sur lequel se trouve la fenêtre principale
                var screen = Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(mainWindow).Handle);

                // Récupérer les dimensions de la zone de travail de cet écran (hors barre des tâches)
                SE_Window.MainWindowWidth = (int)mainWindow.ActualWidth;
                SE_Window.MainWindowHeight = (int)mainWindow.ActualHeight;
                SE_Window.MainWindowTop = screen.WorkingArea.Top;
                SE_Window.MainWindowLeft = screen.WorkingArea.Left;

                if (SE_Window.MainWindowTop < 0)
                {
                    SE_Window.MainWindowTop = SE_Window.MainWindowTop + 500;
                }
            }
            else
            {
                SE_Window.MainWindowWidth = (int)mainWindow.ActualWidth;
                SE_Window.MainWindowHeight = (int)mainWindow.ActualHeight;
                SE_Window.MainWindowTop = mainWindow.Top;
                SE_Window.MainWindowLeft = mainWindow.Left;
            }

            var marginAjusted = (int)((SE_Window.MainWindowHeight - SE_Window.MainWindowMinHeight) / 13);
            if (marginAjusted > 0)
            {
                SE_Window.MainWindowMarginAjusted = marginAjusted;
            }
            else
            {
                SE_Window.MainWindowMarginAjusted = 0;
            }

            // Mettre à jour les dimensions de l'écran et le facteur de mise à l'échelle
            var handle = new System.Windows.Interop.WindowInteropHelper(mainWindow).Handle;
            ScreenHelper.UpdateScreenDimensions(handle);
        }

    }
}
