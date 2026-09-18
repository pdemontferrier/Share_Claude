using CommonResources.Settings;
using CommonResources.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CommonResources.Views.Backgrounds
{
    public partial class CommonBackgroundPad : UserControl, INotifyPropertyChanged
    {
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
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

            // AppTitle
            Title = CR_CommonSettings.ApplicationTitle;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

