using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier chargé de vérifier si le nom d’utilisateur Windows fourni
    /// correspond à un enregistrement existant dans la table <c>User</c>.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé au démarrage de l’application ou lors de l’authentification
    /// automatique, afin d’identifier un utilisateur à partir de son nom Windows
    /// (<c>LoginWindows</c>) et de retourner son identifiant unique si trouvé.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Identifier l’utilisateur correspondant au poste courant en se basant
    /// sur le login Windows et retourner son identifiant s’il existe.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Recevoir le nom d’utilisateur Windows en paramètre.</description></item>
    /// <item><description>Rechercher une correspondance dans la table <c>User</c>.</description></item>
    /// <item><description>Retourner l’identifiant utilisateur si trouvé, sinon 0.</description></item>
    /// <item><description>Lancer une exception classifiée en cas d’erreur d’infrastructure.</description></item>
    /// </list>
    ///
    /// <b>Exceptions :</b>
    /// <list type="bullet">
    /// <item><description><see cref="Ex_Infrastructure"/> : en cas d’erreur de communication avec la base de données.</description></item>
    /// <item><description><see cref="Exception"/> : toute autre erreur non classifiée, reclassée par <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_User_CheckAppDeviceUser : IS_User_CheckAppDeviceUser
    {
        #region === Propriétés privées ===
        private readonly string _callee;
        #endregion

        #region === Dépendances privées ===
        private readonly IQ_User _qhUser;
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_User_CheckAppDeviceUser"/>.
        /// </summary>
        /// <param name="qhUser">Handler de requêtes pour la table utilisateur.</param>
        public SR_User_CheckAppDeviceUser(IQ_User qhUser)
        {
            _callee = GetType().Name;
            _qhUser = qhUser;
        }
        #endregion

        #region === Méthodes publiques ===
        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Vérifie si le nom d’utilisateur Windows fourni correspond à un utilisateur enregistré.
        /// Si oui, retourne son identifiant utilisateur.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Identifier l’utilisateur à partir de son login Windows et retourner son ID,
        /// ou 0 si aucune correspondance n’est trouvée.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Lire la valeur du login Windows reçu en argument.</description></item>
        /// <item><description>Rechercher l’utilisateur correspondant dans la base.</description></item>
        /// <item><description>Retourner l’identifiant utilisateur s’il est trouvé.</description></item>
        /// </list>
        /// </summary>
        /// <param name="deviceUser">Nom d’utilisateur Windows du poste courant.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        /// <returns>
        /// Identifiant de l’utilisateur si trouvé, sinon <c>0</c>.
        /// </returns>
        public async Task<int> ExecuteAsync(string deviceUser, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // 1️ Vérification de la validité du paramètre
                if (string.IsNullOrWhiteSpace(deviceUser))
                    return 0;

                // 2️ Rechercher la correspondance dans la table User
                var user = await _qhUser.HandleGetByLoginWindowsAsync(deviceUser);

                // 3 Retourner l’ID s’il existe, sinon 0
                return user?.Id ?? 0;
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
        #endregion

        #region === Méthodes privées ===
        // A compléter : extensions futures (authentification multi-domaine, alias, SSO, etc.)
        #endregion
    }
}
