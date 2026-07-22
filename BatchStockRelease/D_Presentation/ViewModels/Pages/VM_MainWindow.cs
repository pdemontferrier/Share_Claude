using BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic;

namespace BatchStockRelease.D_Presentation.ViewModels
{
    /// <summary>
    /// <b>ViewModel principal associé à <c>MainWindow</c>.</b>
    /// </summary>
    /// <para>
    /// Ce ViewModel agit comme le coordinateur global de la fenêtre principale.
    /// Il orchestre les opérations d’initialisation UI et les interactions utilisateurs
    /// essentielles, en déléguant la logique métier aux UseCases injectés.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Instancié et injecté dans <c>MainWindow</c> via le conteneur DI, ce ViewModel assure
    /// la cohérence de l’état de l’interface et gère l’authentification initiale de
    /// l’utilisateur via le UseCase <c>IU_User_Authentification</c>.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Centraliser la logique de présentation de la fenêtre principale tout en respectant
    /// le découplage entre la vue (XAML) et la logique applicative.
    /// </para>
    ///
    /// <b>Services injectés :</b>
    /// <list type="bullet">
    ///   <item><description><c>IU_User_Authentification</c> — exécute l’authentification automatique au lancement.</description></item>
    /// </list>
    ///
    /// <b>Méthodes principales :</b>
    /// <list type="number">
    ///   <item><description><c>InitializeAsync(string caller)</c> : exécute le UseCase d’authentification et initialise le contexte utilisateur.</description></item>
    /// </list>
    ///
    /// <b>Remarques techniques :</b>
    /// <para>
    /// Cette classe est conçue pour être extensible : de nouvelles propriétés bindables ou
    /// commandes pourront être ajoutées au fur et à mesure des évolutions du projet.
    /// </para>

    public class VM_MainWindow
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe appellée pour la traçabilité des logs et opérations.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances ===

        private readonly IU_User_Authentification _userAuthentification;

        #endregion

        #region === Propriétés bindables ===

        // A compléter

        #endregion

        #region === Commandes ===

        // A compléter

        #endregion

        #region === Constructeur ===

        public VM_MainWindow(IU_User_Authentification userAuthentification)
        {
            _callee = GetType().Name;

            _userAuthentification = userAuthentification;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Initialise le ViewModel : authentification automatique.
        /// </summary>
        public async Task InitializeAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(InitializeAsync)}";

            await _userAuthentification.ExecuteAsync(callChain);

        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion

    }
}