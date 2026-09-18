using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service métier chargé de vérifier si un nom d’utilisateur Windows correspond à un utilisateur
    /// applicatif connu (table <c>UserApp</c> / <c>User</c> selon ton modèle) et de retourner son identifiant.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé au démarrage de l’application console afin d’identifier l’utilisateur courant avant
    /// d’autoriser le lancement des traitements d’import.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Identifier l’utilisateur à partir du login Windows et initialiser le contexte applicatif
    /// (IdUser) via <c>IS_Settings_User</c>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application (ex : <c>UC_UserIdentify</c>).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Valider le paramètre <c>deviceUser</c>.</description></item>
    /// <item><description>Rechercher l’utilisateur en base via <c>IQ_UserApp</c>.</description></item>
    /// <item><description>Initialiser <c>IS_Settings_User</c> avec l’IdUser si trouvé.</description></item>
    /// <item><description>Retourner l’IdUser, sinon 0 si non trouvé.</description></item>
    /// </list>
    /// </summary>
    public class SR_User_CheckDeviceUser : IS_User_CheckDeviceUser
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserApp _qhUserApp;
        private readonly ISE_User _seUser;
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service de vérification de l’utilisateur applicatif.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases/Business.</para>
        /// <para>Objectif</para>
        /// <para>Valider les dépendances et initialiser la traçabilité.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider les dépendances non nulles.</description></item>
        /// </list>
        /// </summary>
        /// <param name="qhUserApp">QueryHandler pour la recherche utilisateur par login Windows.</param>
        /// <param name="seUser">Paramètres/contexte utilisateur applicatif.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est nulle.</exception>
        public SR_User_CheckDeviceUser(
            IQ_UserApp qhUserApp,
            ISE_User seUser,
            IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _qhUserApp = qhUserApp ?? throw new ArgumentNullException(nameof(qhUserApp));
            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Vérifie si le nom d’utilisateur Windows fourni correspond à un utilisateur enregistré.
        /// Si oui, retourne son identifiant et initialise le contexte utilisateur.
        /// </para>
        /// <para>Contexte</para>
        /// <para>Appelé au démarrage par le UseCase <c>UC_UserIdentify</c>.</para>
        /// <para>Objectif</para>
        /// <para>Retourner l’IdUser si reconnu, sinon 0.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre.</description></item>
        /// <item><description>Lire l’utilisateur en base via QueryHandler.</description></item>
        /// <item><description>Mettre à jour <c>IS_Settings_User</c> si trouvé.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="deviceUser">Login Windows (poste) à vérifier.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>IdUser si trouvé, sinon 0.</returns>
        /// <exception cref="Exception">Reclassifiée via <c>Ex_Classifier</c>.</exception>
        /// </summary>
        public async Task<int> ExecuteAsync(string caller, string deviceUser, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(deviceUser))
                    return 0;

                deviceUser = deviceUser.Trim();
                deviceUser = deviceUser.Replace("\0", string.Empty);
                var idx = deviceUser.LastIndexOf('\\');
                if (idx >= 0)
                    deviceUser = deviceUser[(idx + 1)..];

                var user = await _qhUserApp.HandleGetByWindowsLoginAsync(callChain, deviceUser, ct);

                if (user is null)
                    return 0;

                _seUser.AppUserId = user.Id;
                return user.Id;
            }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}