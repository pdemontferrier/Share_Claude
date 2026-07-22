using CommonResources.Settings;
using CommonResources.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CommonResources.Views.Backgrounds
{
    public partial class CommonBackground : UserControl
    {
        public CommonBackground()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Appliquer les paramètres globaux
            ControlStyler.StyleWindow(Background_1, Background_2, TitleBorder, LogoImage, TitleContent, null);
            // Image
            LogoImage.Source = new BitmapImage(CR_CommonSettings.LogoSource);
            // AppTitle
            TitleContent.Text = CR_CommonSettings.ApplicationTitle;
        }

    }
}

