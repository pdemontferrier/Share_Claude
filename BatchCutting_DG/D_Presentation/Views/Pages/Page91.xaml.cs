using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;
using System.Windows.Media.Imaging;

namespace BatchCutting_DG.D_Presentation.Views.Pages
{
    public partial class Page91 : Page
    {
        private readonly VM_Page91 _viewModel;
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Flag _flag;

        public Page91()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page91>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _flag = App.ServiceProvider.GetRequiredService<IS_Flag>();
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

            // Buttons
            _controlStyler.StyleLanguageButton(Language1Button, Language1Image, Language1RadioButton, 350);
            _controlStyler.StyleLanguageButton(Language2Button, Language2Image, Language2RadioButton, 350);
            _controlStyler.StyleLanguageButton(Language3Button, Language3Image, Language3RadioButton, 350);
            _controlStyler.StyleLanguageButton(Language4Button, Language4Image, Language4RadioButton, 350);
            _controlStyler.StyleLanguageButton(Language5Button, Language5Image, Language5RadioButton, 350);
            _controlStyler.StyleLanguageButton(Language6Button, Language6Image, Language6RadioButton, 350);

            // Afficher les images de langues
            Language1Image.Source = new BitmapImage(_flag.GetFlagUri("FR"));
            Language2Image.Source = new BitmapImage(_flag.GetFlagUri("EN"));
            Language3Image.Source = new BitmapImage(_flag.GetFlagUri("DE"));
            Language4Image.Source = new BitmapImage(_flag.GetFlagUri("ES"));
            Language5Image.Source = new BitmapImage(_flag.GetFlagUri("IT"));
            Language6Image.Source = new BitmapImage(_flag.GetFlagUri("PT"));

        }
    }
}