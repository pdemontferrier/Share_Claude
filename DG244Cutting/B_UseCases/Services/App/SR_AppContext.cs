using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;

namespace DG244Cutting.B_UseCases.Services.App
{
    /// <summary>
    /// Service de fourniture du contexte applicatif courant.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : implémentation de <see cref="IS_AppContext"/> résidant en
    /// B_UseCases/Services/App. Consomme par injection les contrats de Settings
    /// <see cref="ISE_App"/> et <see cref="ISE_User"/>, conformément au régime d'accès
    /// aux Settings des services hors B_UseCases/Services/Business.</para>
    /// <para>Objectif : agréger les valeurs de contexte applicatif et utilisateur
    /// courantes dans un <c>DTO_AppContext</c>.</para>
    /// <para>Non-responsabilités : aucune logique métier ; aucun accès aux données ;
    /// aucune mutation des Settings injectés.</para>
    /// </remarks>
    public class SR_AppContext : IS_AppContext
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_App _settingsApp;
        private readonly ISE_User _settingsUser;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service de contexte applicatif.
        /// </summary>
        /// <param name="settingsApp">Contrat des paramètres applicatifs courants.</param>
        /// <param name="settingsUser">Contrat du contexte utilisateur courant.</param>
        public SR_AppContext(
            ISE_App settingsApp,
            ISE_User settingsUser)
        {
            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Retourne un instantané du contexte applicatif et utilisateur courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : projection en lecture seule des valeurs courantes portées
        /// par les Settings injectés.</para>
        /// <para>Objectif : fournir un objet de transport unique sans comportement.</para>
        /// <para>Dénomination : la méthode publique porte le préfixe <c>Get</c> en
        /// dérogation explicite au préfixe <c>Execute</c> posé par défaut par R-4.2.12
        /// du 0231, dérogation typologiquement bornée au cas Concept admise au titre
        /// de la double dérogation (i) de l'item SR20 du 0232-SR (§4.3.2). La sémantique
        /// justifiant la dénomination retenue est la fourniture d'un instantané du
        /// contexte applicatif et utilisateur courant par agrégation en lecture seule
        /// des Settings consommés, dont l'expression naturelle en verbe d'action en
        /// anglais à l'impératif est <c>Get</c> (récupérer / fournir un instantané)
        /// plutôt que <c>Execute</c> (exécuter une action métier ou applicative unitaire
        /// portée par un Service cas Entité). Cette trace nominative satisfait l'exigence
        /// de documentation imposée par I-4.2.6 du 0231.</para>
        /// </remarks>
        /// <returns>Un <c>DTO_AppContext</c> renseigné à partir des Settings courants.</returns>
        public DTO_AppContext GetAppContext()
        {
            return new DTO_AppContext
            {
                AppId = _settingsApp.AppId,
                AppDate = _settingsApp.AppDate,
                AppDateTime = _settingsApp.AppDateTime,
                AppUserId = _settingsUser.AppUserId,
                AppUserFullName = _settingsUser.AppUserFullName,
                AppDeviceUser = _settingsUser.AppDeviceUser,
                AppDeviceId = _settingsUser.AppDeviceId,
                AppDeviceIP = _settingsUser.AppDeviceIP,
            };
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}