using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;

namespace BatchCutting_DG.D_Presentation.Views.Pages
{
    public partial class Page00 : Page
    {
        private readonly VM_Page00 _viewModel;
        private readonly IS_ControlStyler _controlStyler;

        public Page00()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page00>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            this.DataContext = _viewModel;
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.SizeChanged += OnSizeChanged;
        }

        // Méthodes relatives à la page
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Appliquer les styles
            StyleControls();

            LoginInput.Focus();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Logique à exécuter lors du déchargement de la page, si nécessaire
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Logique à exécuter lors du changement de taille de la page, si nécessaire
        }

        // Méthodes relatives aux composants de la page
        private void StyleControls()
        {
            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // TextBlocks
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // Style Page00 controls
            _controlStyler.StyleBorder(IdentificationBorder);
            _controlStyler.StylePageOOControls(LoginBorder, PasswordBorder, PasswordInput, LoginButton, LoginButtonText);

            IdentificationData.HorizontalAlignment = HorizontalAlignment.Center;
        }
        private async void On_Valider_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.UserAuthenticationAsync(LoginInput.Text, PasswordInput.Password);
            PasswordInput.Password = string.Empty;
            LoginInput.Focus();
        }
    }
}
