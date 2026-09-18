using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonResources.Settings;
using CommonResources.Utilities;

namespace CommonResources.Views.Components
{
    public partial class MenuHorizontalReduce : UserControl
    {
        public event RoutedEventHandler? ExpandClicked;

        public MenuHorizontalReduce()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // MHR
            MHR.Width = CR_CommonSettings.MHR_Width;
            MHR.Height = CR_CommonSettings.MHR_Height;
            MHR.Margin = new Thickness(0,CR_CommonSettings.MHR_MarginTop,CR_CommonSettings.MHR_Margin,0);
            MHR.BorderBrush = new SolidColorBrush(CR_CommonSettings.BorderColor);
            MHR.BorderThickness = new Thickness(CR_CommonSettings.BorderThickness);
            MHR.CornerRadius = new CornerRadius(CR_CommonSettings.BackgroundCornerRadius);

            // MHR_Menu
            MHR_Menu.Width = CR_CommonSettings.Button_MinWidth;
            MHR_Menu.Height = CR_CommonSettings.Button_MinHeight;
            MHR_Menu.HorizontalAlignment = HorizontalAlignment.Center;
            MHR_Menu.VerticalAlignment = VerticalAlignment.Center;
            MHR_Menu.Margin = new Thickness(CR_CommonSettings.MHR_Margin, 0, CR_CommonSettings.MHR_Margin, 0);

            // MHR_Menu_Icon
            MHR_Menu_Icon.Width = CR_CommonSettings.Icon_Size;
            MHR_Menu_Icon.Height = CR_CommonSettings.Icon_Size;
            MHR_Menu_Icon.VerticalAlignment = VerticalAlignment.Top;
            MHR_Menu_Icon.HorizontalAlignment = HorizontalAlignment.Center;
            MHR_Menu_Icon.Source = new BitmapImage(CR_CommonSettings.MHR_Button_Icon_Source);

            // MHR_Menu_Text
            MHR_Menu_Text.Text = CR_CommonSettings.MHR_Button_Text;
            MHR_Menu_Text.FontSize = CR_CommonSettings.TextSize_3;
            MHR_Menu_Text.Foreground = new SolidColorBrush(CR_CommonSettings.TextColor_2);
            ButtonMouseEventHandler.AttachMouseEvents(MHR_Menu, MHR_Menu_Text);

            // Evènements
            MHR_Menu.Click += MHR_Menu_Click;
        }

        private void MHR_Menu_Click(object? sender, RoutedEventArgs e)
        {
            OnExpandClicked(e);
        }

        private void OnExpandClicked(RoutedEventArgs e)
        {
            ExpandClicked?.Invoke(this, e);
        }
    }
}
