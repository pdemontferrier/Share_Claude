using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Components.Banner;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;


namespace BatchStockRelease.D_Presentation.Views.Components.Banner
{
    /// <summary>
    /// <b>Composant : Bannière principale de la fenêtre d’application.</b>
    /// </summary>
    /// <para>
    /// Ce composant visuel constitue la partie supérieure fixe de l’interface principale.  
    /// Il regroupe les actions globales accessibles à tout moment par l’utilisateur :  
    /// changement de langue, affichage du profil utilisateur, accès aux messages,
    /// affichage des informations applicatives et fermeture de l’application.
    /// </para>
    /// 
    /// <b>Contexte :</b>
    /// <para>
    /// Intégré dynamiquement dans <c>MainWindow</c> après initialisation du conteneur
    /// de dépendances (<c>App._serviceProvider</c>), ce composant repose sur son propre
    /// ViewModel (<c>VM_BA_MainWindow</c>) et applique les styles d’interface définis
    /// par le service <c>IS_ControlStyler</c>.
    /// </para>
    /// 
    /// <b>Objectif :</b>
    /// <para>
    /// Offrir un bandeau ergonomique, homogène et réactif sur toutes les pages de
    /// l’application, garantissant une cohérence visuelle et une navigation fluide.
    /// </para>
    /// 
    /// <b>Utilisateurs cibles :</b>
    /// <para>
    /// Destiné à l’ensemble des utilisateurs de l’application, qu’ils soient magasiniers,
    /// opérateurs ou responsables de production.  
    /// Accessible sur tablette via la fenêtre principale (<c>MainWindow</c>).
    /// </para>
    /// 
    /// <b>ViewModel associé :</b>
    /// <para><c>VM_BA_MainWindow</c> — Gère la logique d’interaction (commandes, navigation,
    /// mise à jour des icônes et informations dynamiques).</para>
    /// 
    /// <b>Services injectés :</b>
    /// <list type="bullet">
    ///   <item><description><c>IS_ControlStyler</c> — applique les styles et icônes du bandeau.</description></item>
    ///   <item><description><c>VM_BA_MainWindow</c> — contexte de données et logique de navigation.</description></item>
    /// </list>
    /// 
    /// <b>Comportement :</b>
    /// <list type="number">
    ///   <item><description>Au chargement (<c>OnLoaded</c>), applique les styles du bandeau via <c>ApplyLayout()</c>.</description></item>
    ///   <item><description>Initialise le ViewModel pour mettre à jour les données dynamiques (langue, utilisateur, messages).</description></item>
    ///   <item><description>Met à jour automatiquement les icônes et états selon le contexte utilisateur.</description></item>
    /// </list>
    /// 
    /// <b>Remarques techniques :</b>
    /// <para>
    /// Ce composant est enregistré dans le conteneur DI de l’application et chargé dynamiquement
    /// afin d’éviter toute erreur de création prématurée du ViewModel avant initialisation du DI.
    /// </para>

    public partial class BA_MainWindow : UserControl
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe appellée pour la traçabilité des logs et opérations.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly VM_BA_MainWindow _viewModel;

        private readonly IS_ControlStyler _controlStyler;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le composant et résout les dépendances via DI.
        /// </summary>
        public BA_MainWindow()
        {
            InitializeComponent();

            _callee = GetType().Name;

            _viewModel = App._serviceProvider.GetRequiredService<VM_BA_MainWindow>();
            DataContext = _viewModel;
            _controlStyler = App._serviceProvider.GetRequiredService<IS_ControlStyler>();

            Loaded += OnLoaded;

        }
        #endregion

        #region === Méthodes publiques ===
        // A compléter
        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Événement de chargement de la fenêtre.
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnLoaded)}";

            ApplyLayout();

            // Initialiser le viewModel
            await _viewModel.InitializeAsync(callChain);
        }


        private void ApplyLayout()
        {
            // Drapeau
            _controlStyler.StyleAppLanguageButton(LanguageButton, LanguageIcon);

            // Utilisateur
            _controlStyler.StyleAppUserButton(UserFullNameButton, UserFullName);

            // AppInfo
            _controlStyler.StyleAppInfoButton(AppInfo, AppInfoSign);

            // Messages
            _controlStyler.StyleAppMessageButton(MessageButton, MessageButtonIcon, MessageNotReadButtonIcon);

            // AppClose
            _controlStyler.StyleAppCloseButton(AppCloseButton, AppCloseButtonIcon);
        }

        #endregion

    }
}