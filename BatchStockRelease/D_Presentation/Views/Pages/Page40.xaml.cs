using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    public partial class Page40 : Page_Generic
    {
        private readonly VM_Page40 _viewModel;

        public Page40()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page40>();
            this.DataContext = _viewModel;
        }
        /// <summary>
        /// Charge les données du ViewModel lors du chargement de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Appel de la logique générique

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Applique les styles à la page et à ses composants.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Appel de la logique générique

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // PageTitle
            _controlStyler.StyleTextBlockPageTitle(PageTitleMain);

        }
    }
}