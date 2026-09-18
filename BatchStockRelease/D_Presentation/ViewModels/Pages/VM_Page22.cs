using System.Windows;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page22 — Page d’intégration en stock des barres de chutes générées à la découpe.
    ///
    /// <para><b>Contexte :</b> Cette page intervient en fin de cycle de découpe.
    /// Elle permet à l’opérateur de scanner les barres de chutes pour les réintégrer
    /// dans le stock et les rendre à nouveau disponibles pour de futurs lots.</para>
    ///
    /// <para><b>Objectif :</b> Identifier et marquer comme “en stock” les chutes valides
    /// scannées via leur QR Code, en mettant à jour les tables de gestion de stock.</para>
    ///
    /// <para><b>Vue associée :</b> <c>Page22.xaml</c></para>
    ///
    /// <list type="bullet">
    ///   <item><description>Analyse le QR code scanné et vérifie sa validité.</description></item>
    ///   <item><description>Appelle le service métier <see cref="IU_BarDropStockIntegration"/> pour mise à jour du stock.</description></item>
    ///   <item><description>Met à jour dynamiquement la visibilité des sections de la vue selon le résultat.</description></item>
    ///   <item><description>Gère les erreurs via <see cref="Ex_Classifier"/> et notifie via <see cref="IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public class VM_Page22 : VM_Page_Generic
    {
        #region === Dépendances privées ===
        private readonly IU_BarDropStockIntegration _barDropStockIntegration;
        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la page 22 avec les services nécessaires.
        /// </summary>
        /// <param name="barDropStockIntegration">UseCase métier d’intégration en stock des barres de chutes.</param>
        /// <param name="settings">Service de gestion des paramètres métier.</param>
        /// <param name="navigation">Service de navigation entre les pages.</param>
        /// <param name="dictionary">Service multilingue.</param>
        /// <param name="logAndNotify">Service de journalisation et notification des erreurs.</param>
        public VM_Page22(
            IU_BarDropStockIntegration barDropStockIntegration,
            IS_Settings_UseCase settings,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settings, navigation, dictionary, logAndNotify)
        {
            _barDropStockIntegration = barDropStockIntegration;
        }

        #endregion

        #region === Propriétés liées à la vue ===

        private Visibility _notFoundVisibility = Visibility.Collapsed;
        public Visibility NotFoundVisibility
        {
            get => _notFoundVisibility;
            set => SetProperty(ref _notFoundVisibility, value);
        }

        private Visibility _doneVisibility = Visibility.Collapsed;
        public Visibility DoneVisibility
        {
            get => _doneVisibility;
            set => SetProperty(ref _doneVisibility, value);
        }

        private Visibility _notDoneVisibility = Visibility.Collapsed;
        public Visibility NotDoneVisibility
        {
            get => _notDoneVisibility;
            set => SetProperty(ref _notDoneVisibility, value);
        }

        private Visibility _commentVisibility = Visibility.Collapsed;
        public Visibility CommentVisibility
        {
            get => _commentVisibility;
            set => SetProperty(ref _commentVisibility, value);
        }

        private string? _qrCode;
        public string? QrCode
        {
            get => _qrCode;
            set => SetProperty(ref _qrCode, value);
        }

        private string? _commentaire;
        public string? Commentaire
        {
            get => _commentaire;
            set => SetProperty(ref _commentaire, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Initialise les données de la page (placeholder pour une future extension).
        /// </summary>
        public async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Traite le QR code scanné, exécute la logique d’intégration en stock
        /// et met à jour l’interface utilisateur selon le résultat obtenu.
        /// </summary>
        /// <param name="qrCode">Chaîne représentant le QR code scanné.</param>
        /// <returns>Tâche asynchrone gérant la séquence complète du traitement.</returns>
        public async Task ProcessQrCodeAsync(string qrCode)
        {
            string callChain = BuildFirstCallChain(nameof(ProcessQrCodeAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    ResetVisibility();
                    QrCode = qrCode;
                    Commentaire = string.Empty;

                    if (string.IsNullOrWhiteSpace(qrCode))
                        return;

                    int result = await _barDropStockIntegration.ExecuteAsync(qrCode, callChain);
                    ApplyResult(result);
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
        /// Réinitialise l’état visuel de la page avant un nouveau traitement.
        /// </summary>
        private void ResetVisibility()
        {
            NotFoundVisibility = Visibility.Collapsed;
            DoneVisibility = Visibility.Collapsed;
            NotDoneVisibility = Visibility.Collapsed;
            CommentVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Applique le résultat du traitement selon le code retour.
        /// </summary>
        /// <param name="result">Code retour du service métier.</param>
        private void ApplyResult(int result)
        {
            switch (result)
            {
                case 0:
                    NotFoundVisibility = Visibility.Visible;
                    Commentaire = _dictionary.GetText("P22_03");
                    break;
                case 1:
                    DoneVisibility = Visibility.Visible;
                    Commentaire = _dictionary.GetText("P22_04");
                    break;
                case 2:
                    NotDoneVisibility = Visibility.Visible;
                    Commentaire = _dictionary.GetText("P22_05");
                    break;
                default:
                    NotFoundVisibility = Visibility.Visible;
                    Commentaire = _dictionary.GetText("P22_06");
                    break;
            }

            CommentVisibility = Visibility.Visible;
        }

        #endregion
    }
}