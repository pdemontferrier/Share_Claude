using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;


namespace Shared.Utilities
{
    public static class Hyperlink
    {
        public static void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                if (e.Uri != null && System.IO.Directory.Exists(e.Uri.LocalPath)) // Vérifie la validité du chemin
                {
                    var startInfo = new ProcessStartInfo
                    {
                        Arguments = e.Uri.LocalPath,
                        FileName = "explorer.exe"
                    };
                    Process.Start(startInfo);
                }
                else
                {
                    MessageBox.Show("Le chemin spécifié est invalide ou inexistant.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                e.Handled = true;
            }
        }
    }
}
