using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.D_Presentation.Views.Generic
{
    /// <summary>
    /// MH_Page_Generic — Classe de base générique pour toutes les menus horizontaux 
    /// et fichier code-behind de type 'MH_Page' de l'application.
    ///
    /// <para><b>Contexte :</b> Composant de navigation horizontal affiché
    /// en haut de chaque page de l’application BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Centraliser la gestion :
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Du style et de la disposition de la grille.</description></item>
    ///   <item><description>Des boutons standards : Menu, Refresh, Previous, Home.</description></item>
    ///   <item><description>Des règles de navigation et de sécurité communes.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisation :</b> Les classes dérivées (ex : MH_Page20/>)
    /// ajoutent leurs boutons spécifiques et redéfinissent les méthodes
    /// <see cref="ApplyNavigationRules"/> et <see cref="ApplySecurityRules"/> si nécessaire.</para>
    ///
    /// <para><b>Spécificités techniques :</b> Gère automatiquement le redimensionnement
    /// et l’application des styles via <see cref="IS_ControlStyler"/>.</para>
    /// </summary>
    public class MH_Page_Generic : UserControl
    {
        protected readonly IS_ControlStyler _controlStyler;
        protected readonly IS_Icons _icons;
        protected readonly IS_Window _window;
        protected readonly IS_Navigation _navigation;

        protected MH_Page_Generic()
        {
            _controlStyler = App._serviceProvider.GetRequiredService<IS_ControlStyler>();
            _icons = App._serviceProvider.GetRequiredService<IS_Icons>();
            _window = App._serviceProvider.GetRequiredService<IS_Window>();
            _navigation = App._serviceProvider.GetRequiredService<IS_Navigation>();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyCommonStyles();
            ApplyNavigationRules();
            ApplySecurityRules();

            if (Window.GetWindow(this) is MainWindow mainWindow)
                mainWindow.SizeChanged += (_, _) => ApplyCommonStyles();
        }

        /// <summary>
        /// Applique les styles à la grille principale et aux boutons communs.
        /// </summary>
        private void ApplyCommonStyles()
        {
            var grid = FindName("MH_Grid") as Grid;
            var col1 = FindName("MH_Grid_C1") as ColumnDefinition;
            var col2 = FindName("MH_Grid_C2") as ColumnDefinition;
            var border = FindName("MH_Border") as Border;

            if (grid is not null && col1 is not null && col2 is not null && border is not null)
                _controlStyler.StyleHorizontalMenuGrid(grid, col1, col2, border, _window.GetMainWindowWidth());

            StyleCommonButtons();
        }

        /// <summary>
        /// Applique le style et les icônes aux boutons de base du menu horizontal.
        /// </summary>
        private void StyleCommonButtons()
        {
            var mapping = new (string Name, Uri Icon)[]
            {
                ("MH_Menu", _icons.GetMH_Menu_Source()),
                ("MH_Refresh", _icons.GetMH_Refresh_Source()),
                ("MH_Previous", _icons.GetMH_Previous_Source()),
                ("MH_Home", _icons.GetMH_Home_Source())
            };

            foreach (var (name, icon) in mapping)
            {
                if (FindButton(name) is Button btn)
                    _controlStyler.StyleButton(btn, icon);
            }
        }

        /// <summary>
        /// Règles de navigation personnalisables.
        /// Par défaut : affiche MH_Home si navigation vers Page10 possible.
        /// </summary>
        protected virtual void ApplyNavigationRules()
        {
            if (FindButton("MH_Home") is Button homeButton)
            {
                homeButton.Visibility = _navigation.CanNavigate("Page10")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Règles de sécurité personnalisables.
        /// Par défaut : aucune restriction.
        /// </summary>
        protected virtual void ApplySecurityRules()
        {
            // Logique à exécuter pour l'application des règles de sécurité, si nécessaire
        }

        /// <summary>
        /// Recherche un bouton par nom et le retourne s’il existe.
        /// </summary>
        /// <param name="name">Nom du bouton dans le XAML.</param>
        /// <returns>Instance du bouton si trouvé, sinon null.</returns>
        protected Button? FindButton(string name) => FindName(name) as Button;
    }
}