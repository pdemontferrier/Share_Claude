using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;
using System.Windows.Media;

namespace BatchCutting_DG.D_Presentation.Views.Pages
{
    public partial class Page30 : Page
    {
        private readonly VM_Page30 _viewModel;
        private readonly IS_ControlStyler _controlStyler;

        public Page30()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page30>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            this.DataContext = _viewModel;
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.SizeChanged += OnSizeChanged;
        }

        // Méthodes relatives à la page
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Appliquer les styles
            StyleControls();

            // Charger les données du View Model
            await _viewModel.LoadDataAsync();

            // Adapter les styles en fonction des valeures chargées
            ReStyleControls();
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

            // Appliquer les styles génériques aux TextBlocks
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);
        }

        private void ReStyleControls()
        {
            if (_viewModel.DecoupeDetailWithCut is not null && _viewModel.DecoupeDetailWithCut.IndiceDecoupe >= 2)
            {
                var customRed = (Brush)new BrushConverter().ConvertFromString("#FF3E3E")!;
                IndiceCoupeData.Foreground = customRed;
                ComentData.Foreground = customRed;
            }
        }

    }
}