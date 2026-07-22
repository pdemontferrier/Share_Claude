using System.Reflection;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page98 — Page de présentation générale de l’application.
    ///
    /// <para><b>Contexte :</b> Cette page présente les informations générales de
    /// l’application, son numéro de version actuellement déployé, son objectif, 
    /// et son architecture logicielle</para>
    ///
    /// <para><b>Objectif :</b> Fournir aux utilisateurs une vue d’ensemble sur :</para>
    /// <list type="bullet">
    ///   <item><description><b>La version :</b> Numéro de build affiché dynamiquement via le ViewModel.</description></item>
    ///   <item><description><b>Le contexte :</b> Digitalisation de l’atelier de fabrication.</description></item>
    ///   <item><description><b>L’objectif :</b> Automatiser les flux de stock et d’approvisionnement.</description></item>
    ///   <item><description><b>L’architecture :</b> Application WPF .NET 8 basée sur Clean Architecture et MVVM.</description></item>
    /// </list>
    ///
    /// <para><b>Vue associée :</b> <c>Page98.xaml</c></para>
    ///
    /// <para><b>Spécificités techniques :</b>
    /// - Le numéro de version est extrait dynamiquement depuis l’assembly courant.  
    /// - La méthode <see cref="LoadDataAsync"/> suit le schéma standard <c>ExecuteSafeAsync</c>.  
    /// - Gère les erreurs via <see cref="Ex_Classifier"/> et <see cref="IS_LogAndNotify"/>.</para>
    /// </summary>
    public class VM_Page98 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Commandes ===

        // A compléter

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la Page98 avec les services nécessaires.
        /// </summary>
        /// <param name="settings">Service de gestion des paramètres applicatifs.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue pour la traduction des libellés.</param>
        /// <param name="logAndNotify">Service centralisé de gestion des erreurs et notifications.</param>
        public VM_Page98(
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            // A compléter
        }

        #endregion

        #region === Propriétés liées à la vue ===

        private string _versionNumber = string.Empty;

        /// <summary>
        /// Numéro de version actuellement déployé de l’application BatchStockRelease.
        /// </summary>
        public string VersionNumber
        {
            get => _versionNumber;
            private set => SetProperty(ref _versionNumber, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Initialise les données de la page, notamment le numéro de version.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    VersionNumber = GetVersionNumber(callChain);
                    await Task.CompletedTask;
                }
                catch (Ex_Infrastructure)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            });
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Récupère dynamiquement le numéro de version à partir de l’assembly en cours d’exécution.
        /// </summary>
        /// <returns>
        /// Une chaîne représentant la version actuelle du build (ex. : "1.0.8423.25562"),
        /// ou "Version inconnue" si la lecture échoue.
        /// </returns>
        private string GetVersionNumber(string caller)
        {
            string callChain = $"{caller} > {nameof(GetVersionNumber)}";

            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return version != null ? version.ToString() : _dictionary.GetText("No_In_09");
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion
    }
}