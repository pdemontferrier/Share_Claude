using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier chargé d’initialiser les droits d’accès aux pages pour l’utilisateur courant.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Appelé après l’authentification ou la mise à jour du contexte utilisateur,
    /// ce service synchronise les droits d’accès aux différentes pages applicatives
    /// en combinant les valeurs par défaut et celles stockées dans la base de données.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Garantir que les droits de l’utilisateur sur les pages de l’application sont correctement initialisés
    /// avant toute interaction avec l’interface.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Initialiser toutes les pages avec des droits par défaut (<c>false</c>).</description></item>
    /// <item><description>Lire les droits spécifiques depuis la base de données via <see cref="IQ_UserAppPageDroit"/>.</description></item>
    /// <item><description>Mettre à jour le contexte utilisateur via <see cref="IS_Settings_User.SetPageAccessRights"/>.</description></item>
    /// <item><description>Lancer une exception classifiée en cas d’erreur d’infrastructure.</description></item>
    /// </list>
    ///
    /// <b>Exceptions :</b>
    /// <list type="bullet">
    /// <item><description><see cref="Ex_Infrastructure"/> : levée en cas de problème de connexion ou d’accès base de données.</description></item>
    /// <item><description><see cref="Exception"/> : toute autre erreur non classifiée, reclassée par <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_User_InitializePageAccessRights : IS_User_InitializePageAccessRights
    {
        #region === Propriétés privées ===
        private readonly string _callee;
        #endregion

        #region === Dépendances privées ===
        private readonly IS_Settings_User _settingsUser;
        private readonly IQ_UserAppPageDroit _qhUserAppPageDroit;
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_User_InitializePageAccessRights"/>.
        /// </summary>
        /// <param name="settingsUser">Service de gestion du contexte utilisateur.</param>
        /// <param name="qhUserAppPageDroit">Handler de requêtes pour les droits d’accès aux pages utilisateur.</param>
        public SR_User_InitializePageAccessRights(
            IS_Settings_User settingsUser,
            IQ_UserAppPageDroit qhUserAppPageDroit)
        {
            _callee = GetType().Name;

            _settingsUser = settingsUser;
            _qhUserAppPageDroit = qhUserAppPageDroit;
        }
        #endregion

        #region === Méthodes publiques ===
        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Initialise les droits d’accès de l’utilisateur sur les pages applicatives.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Appliquer des droits par défaut, puis mettre à jour les autorisations spécifiques
        /// depuis la base de données pour l’utilisateur et l’application en cours.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Initialiser les accès par défaut de toutes les pages à <c>false</c>.</description></item>
        /// <item><description>Charger les droits utilisateurs enregistrés dans la base.</description></item>
        /// <item><description>Mettre à jour les droits du contexte utilisateur.</description></item>
        /// </list>
        /// </summary>
        /// <param name="userId">Identifiant de l’utilisateur à vérifier.</param>
        /// <param name="appId">Identifiant de l’application cible.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        public async Task ExecuteAsync(int userId, int appId, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1️ : Initialiser les accès par défaut (toutes les pages à false)
                _settingsUser.InitializeUserDefaultAccesses();

                // Étape 2 : Récupérer les droits depuis la base
                var accessiblePages = await _qhUserAppPageDroit.HandleGetByUserIdAppIdAsync( userId, appId);

                // Étape 3 : Appliquer les droits si disponibles
                if (accessiblePages != null && accessiblePages.Any())
                {
                    _settingsUser.SetPageAccessRights(accessiblePages);
                }
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
