using CommonResources.Settings;
using CommonResources.Utilities;
using System.Windows;
using System.Windows.Media.Imaging;
using System;
using System.Runtime.InteropServices;


namespace CommonResources.Views.Components
{
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();

            //this.Height = CR_CommonSettings.DW_Height;
            //this.Width = CR_CommonSettings.DW_Width;
            this.Loaded += OnLoaded;
        }

        #region === Suppression du bouton Fermer (X) ===

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint SC_CLOSE = 0xF060;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Récupère le handle natif de la fenêtre
            var handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            // Supprime le bouton “Fermer” du menu système
            IntPtr hMenu = GetSystemMenu(handle, false);
            DeleteMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
        }

        #endregion

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Appliquer les paramètres globaux
            ControlStyler.StyleWindow(Background_1, null, TitleBorder, LogoImage, TitleContent, MainContent);

            // Image
            LogoImage.Source = new BitmapImage(CR_CommonSettings.DW_Icon);

            // AppTitle
            TitleContent.Text = CR_CommonSettings.DW_Title;
            TitleContent.Margin = new Thickness(0);
            TitleContent.VerticalAlignment = VerticalAlignment.Center;
            TitleContent.HorizontalAlignment = HorizontalAlignment.Center;

            // MainContent
            MainContent.Text = CR_CommonSettings.DW_Content;
            MainContent.Margin = new Thickness(50, 100, 50, 0);

            // Maximiser la fenêtre
            WindowState = WindowState.Maximized;
        }

    }
}
