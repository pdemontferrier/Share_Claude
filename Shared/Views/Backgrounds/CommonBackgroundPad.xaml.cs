using Shared.Settings;
using Shared.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Shared.Views.Backgrounds
{
    public partial class CommonBackgroundPad : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(CommonBackgroundPad),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public CommonBackgroundPad()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Appliquer les paramètres globaux
            ControlStyler.StyleWindowPad(Background_1, TitleBorder, LogoImage, TitleContent, null);

            // Image
            LogoImage.Source = new BitmapImage(CR_CommonSettings.LogoSource);
        }
    }
}
