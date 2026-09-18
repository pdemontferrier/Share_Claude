using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.D_Presentation.Views.Generic
{
    public class Page_Generic : Page
    {
        protected readonly IS_ControlStyler _controlStyler;
        protected readonly IS_Window _window;
        protected readonly IS_Dictionary _dictionary;

        /// <summary>
        /// Classe de base générique pour toutes les fichier code-behind de type 'Page' de l'application.
        /// <para>Fournit une gestion unifiée du cycle de vie des pages (chargement, déchargement, redimensionnement),
        /// et assure l’injection des services d’infrastructure nécessaires à la mise en page et à la traduction.</para>
        /// </summary>
        public Page_Generic()
        {
            _controlStyler = App._serviceProvider.GetRequiredService<IS_ControlStyler>();
            _window = App._serviceProvider.GetRequiredService<IS_Window>();
            _dictionary = App._serviceProvider.GetRequiredService<IS_Dictionary>();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;
        }

        // Gestion des événements du cycle de vie de la page
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyLayout();
            OnPageLoaded();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            OnPageUnloaded();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnPageResized();
        }


        // ==============================
        // Méthodes virtuelles à surcharger
        // ==============================
        protected virtual void OnPageLoaded() 
        {
            // Logique à exécuter lors du chargement de la page, si nécessaire
        }

        protected virtual void OnPageUnloaded() 
        {
            // Logique à exécuter lors du déchargement de la page, si nécessaire
        }
        protected virtual void OnPageResized() 
        {
            // Logique à exécuter lors du changement de taille de la page, si nécessaire
        }
        protected virtual void ApplyLayout()
        {
            // Logique à exécuter pour la mise en page, si nécessaire
        }
    }
}