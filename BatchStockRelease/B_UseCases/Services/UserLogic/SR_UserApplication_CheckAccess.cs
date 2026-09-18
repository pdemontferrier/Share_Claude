using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier chargé de vérifier les droits d’accès d’un utilisateur à une application donnée.
    /// Il constitue la première brique du processus d’authentification et de contrôle des accès.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé depuis le <see cref="BatchStockRelease.B_UseCases.UseCases.UserLogic.UC_User_AccessApp"/> afin de déterminer
    /// si un utilisateur identifié dispose des autorisations nécessaires pour accéder
    /// à l’application en cours d’exécution.  
    /// Les résultats sont enregistrés dans le contexte utilisateur via <see cref="IS_Settings_User"/>.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Garantir que l’utilisateur possède à la fois l’accès à l’application
    /// et les droits d’exécution de l’action demandée.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Récupérer les droits d’accès utilisateur pour l’application courante.</description></item>
    /// <item><description>Vérifier les autorisations spécifiques liées à l’action demandée.</description></item>
    /// <item><description>Combiner les deux vérifications pour déterminer si l’accès est autorisé.</description></item>
    /// <item><description>Mettre à jour le contexte utilisateur avec le résultat de l’autorisation.</description></item>
    /// <item><description>Lancer une exception classifiée en cas d’erreur d’infrastructure ou de logique métier.</description></item>
    /// </list>
    ///
    /// <b>Exceptions :</b>
    /// <list type="bullet">
    /// <item><description><see cref="Ex_Infrastructure"/> : levée en cas d’erreur de communication avec la base de données.</description></item>
    /// <item><description><see cref="Ex_Business"/> : levée en cas de violation d’une règle d’accès.</description></item>
    /// <item><description><see cref="Exception"/> : toute autre erreur non classifiée, reclassée par <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_UserApplication_CheckAccess : IS_UserApplication_CheckAccess
    {
        #region === Propriétés privées ===
        /// <summary>
        /// Nom interne du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;
        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserApplication _qhUserApplication;
        private readonly IQ_UserDroit _qhUserDroit;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_UserApplication_CheckAccess"/>.
        /// </summary>
        /// <param name="qhUserApplication">Handler de requêtes pour les accès utilisateur aux applications.</param>
        /// <param name="qhUserDroit">Handler de requêtes pour les droits utilisateurs par action.</param>
        public SR_UserApplication_CheckAccess(
            IQ_UserApplication qhUserApplication,
            IQ_UserDroit qhUserDroit)
        {
            _callee = GetType().Name;

            _qhUserApplication = qhUserApplication;
            _qhUserDroit = qhUserDroit;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Vérifie si un utilisateur dispose des droits nécessaires pour accéder à une application
        /// et exécuter l’action spécifiée.  
        /// Met à jour le contexte utilisateur avec le résultat du contrôle.
        /// </para>
        ///
        /// <b>Contexte :</b>
        /// <para>
        /// Appelée depuis un UseCase d’authentification ou de démarrage, cette méthode
        /// constitue la première étape de validation des droits d’accès utilisateur.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Retourner <c>true</c> si l’utilisateur est autorisé, sinon <c>false</c>,
        /// et lancer une exception classifiée si une erreur survient pendant la vérification.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Construire la chaîne de traçabilité complète (<c>callChain</c>).</description></item>
        /// <item><description>Vérifier si l’utilisateur est autorisé à accéder à l’application.</description></item>
        /// <item><description>Vérifier si l’utilisateur a le droit d’exécuter l’action spécifiée.</description></item>
        /// <item><description>Combiner les résultats pour déterminer l’accès global.</description></item>
        /// <item><description>Mettre à jour le contexte utilisateur via <see cref="IS_Settings_User"/>.</description></item>
        /// </list>
        ///
        /// <b>Exceptions :</b>
        /// <list type="bullet">
        /// <item><description><see cref="Ex_Infrastructure"/> : en cas d’erreur d’accès base de données.</description></item>
        /// <item><description><see cref="Ex_Business"/> : en cas de violation d’une règle de droit utilisateur.</description></item>
        /// </list>
        /// </summary>
        /// <param name="userId">Identifiant de l’utilisateur à vérifier.</param>
        /// <param name="appId">Identifiant de l’application cible.</param>
        /// <param name="actionId">Identifiant de l’action ou du module à exécuter.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        /// <returns><c>true</c> si l’utilisateur dispose des droits nécessaires, sinon <c>false</c>.</returns>
        /// <exception cref="Ex_Business">Levée si une règle d’accès est violée.</exception>
        /// <exception cref="Ex_Infrastructure">Levée si une erreur d’accès base de données survient.</exception>
        public async Task<bool> ExecuteAsync(int userId, int appId, int actionId, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1️ : Vérifier si l'utilisateur a accès à l'application
                bool app = await _qhUserApplication.HandleHasUserAccessAppAsync(appId, userId);

                // Étape 2 : Vérifier si l'utilisateur a le droit d'effectuer l'action
                bool droit = await _qhUserDroit.HandleGetUserActionAsync(userId, actionId);

                // Étape 3 : Déterminer si l'utilisateur peut accéder à l'application
                bool access = app && droit;

                return access;
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}