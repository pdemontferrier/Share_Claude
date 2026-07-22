using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page99 — Page d’avertissement pour accès non autorisé.
    ///
    /// <para><b>Contexte :</b> Cette page s’affiche lorsqu’un utilisateur
    /// tente d’accéder à une page pour laquelle il ne possède pas
    /// les droits nécessaires. Elle fait partie du mécanisme de
    /// double sécurité du système de navigation.</para>
    ///
    /// <para><b>Objectif :</b> Informer l’utilisateur que l’accès
    /// est refusé tout en maintenant la stabilité de l’application.
    /// Cette page peut aussi être affichée si un jeton de session
    /// ou un droit a expiré.</para>
    ///
    /// <para><b>Vue associée :</b> <c>Page99.xaml</c></para>
    ///
    /// <para><b>Spécificités techniques :</b>
    /// - Utilise les services <see cref="IS_Navigation"/> et <see cref="IS_Dictionary"/> pour afficher un message multilingue.  
    /// - Le message d’erreur est défini via le dictionnaire de ressources (clé : <c>No_Ac_99</c>).  
    /// - La méthode <see cref="LoadDataAsync"/> suit le schéma standard <c>ExecuteSafeAsync</c>.  
    /// - Gère les erreurs via <see cref="Ex_Classifier"/> et <see cref="IS_LogAndNotify"/>.</para>
    /// </summary>
    public class VM_Page99 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Commandes ===

        // A compléter

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la Page99 avec les services nécessaires.
        /// </summary>
        /// <param name="settings">Service de gestion des paramètres applicatifs.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue pour la traduction des libellés.</param>
        /// <param name="logAndNotify">Service de gestion centralisée des erreurs et notifications.</param>
        public VM_Page99(
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

        private string _unauthorizedMessage = string.Empty;

        /// <summary>
        /// Message d’avertissement affiché à l’utilisateur.
        /// </summary>
        public string UnauthorizedMessage
        {
            get => _unauthorizedMessage;
            private set => SetProperty(ref _unauthorizedMessage, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge et prépare le message d’avertissement affiché à l’utilisateur.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    // Chargement du message depuis le dictionnaire multilingue
                    UnauthorizedMessage = _dictionary.GetText("P99_01");
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

        // A compléter

        #endregion
    }
}