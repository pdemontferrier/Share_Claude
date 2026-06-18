using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier chargé de mettre à jour le contexte utilisateur en mémoire
    /// après l’authentification ou la vérification des droits d’accès.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé après l’étape de vérification d’accès utilisateur
    /// afin de synchroniser les informations contextuelles de l’utilisateur courant
    /// (identifiant, nom complet, droits d’accès) avec la configuration locale.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Mettre à jour les informations utilisateur dans le contexte applicatif
    /// afin qu’elles soient disponibles pour les autres services et UseCases.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Construire la chaîne de traçabilité complète (<c>callChain</c>).</description></item>
    /// <item><description>Récupérer le nom complet de l’utilisateur via le handler <see cref="IQ_User"/>.</description></item>
    /// <item><description>Mettre à jour le contexte utilisateur avec son identifiant, son nom complet et son autorisation d’accès.</description></item>
    /// <item><description>Lancer une exception classifiée en cas d’erreur d’accès base de données ou de logique métier.</description></item>
    /// </list>
    ///
    /// <b>Exceptions :</b>
    /// <list type="bullet">
    /// <item><description><see cref="Ex_Infrastructure"/> : en cas d’erreur de communication avec la base de données.</description></item>
    /// <item><description><see cref="Ex_Business"/> : en cas d’incohérence dans les données utilisateur.</description></item>
    /// <item><description><see cref="Exception"/> : toute autre erreur non classifiée, reclassée par <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_User_UpdateContext : IS_User_UpdateContext
    {
        #region === Propriétés privées ===
        private readonly string _callee;
        #endregion

        #region === Dépendances privées ===
        private readonly IS_Settings_User _settingsUser;
        private readonly IQ_User _qhUser;
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_User_UpdateContext"/>.
        /// </summary>
        /// <param name="settingsUser">Service de gestion du contexte utilisateur.</param>
        /// <param name="qhUser">Handler de requêtes pour les informations utilisateur.</param>
        public SR_User_UpdateContext(IS_Settings_User settingsUser, IQ_User qhUser)
        {
            _callee = GetType().Name;
            _settingsUser = settingsUser;
            _qhUser = qhUser;
        }
        #endregion

        #region === Méthodes publiques ===
        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Met à jour le contexte utilisateur après l’authentification ou la validation d’accès.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Synchroniser les informations locales avec les données de la base utilisateur.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Récupérer le nom complet de l’utilisateur via <see cref="IQ_User"/>.</description></item>
        /// <item><description>Mettre à jour les propriétés du service <see cref="IS_Settings_User"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        /// <param name="access">Statut d’accès de l’utilisateur (autorisé ou non).</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        /// <returns>Tâche asynchrone représentant la mise à jour du contexte utilisateur.</returns>
        /// <exception cref="Ex_Business">Levée en cas de données incohérentes pour l’utilisateur.</exception>
        /// <exception cref="Ex_Infrastructure">Levée en cas d’erreur de communication avec la base.</exception>
        public async Task ExecuteAsync(int userId, bool access, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1️ : Récupérer le nom complet de l'utilisateur depuis la base
                string? userFullName = await _qhUser.HandleGetUserFullNameAsync(userId);

                if (string.IsNullOrWhiteSpace(userFullName))
                    throw new Ex_Business(callChain, "USRCTX_01", "No_Er_Bu_10");

                // Étape 2 : Mettre à jour le contexte utilisateur
                _settingsUser.SetCanUserAccessApp(access);
                _settingsUser.SetAppUserId(userId);
                _settingsUser.SetAppUserFullName(userFullName);
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