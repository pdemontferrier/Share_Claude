using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier responsable de la mise à jour du contexte applicatif et utilisateur
    /// au moment du démarrage de l’application.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé depuis le <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/> après la
    /// validation des préconditions techniques (connectivité, session, disponibilité applicative).
    /// Il agit comme un point d’initialisation visuel et contextuel, garantissant que
    /// l’interface et les informations utilisateur sont correctement affichées dès l’ouverture
    /// de l’application.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Mettre à jour le titre de l’application et le nom complet de l’utilisateur connecté
    /// afin d’assurer la cohérence entre le contexte système et l’affichage utilisateur.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Définir le titre principal de l’application à partir du dictionnaire multilingue.</description></item>
    /// <item><description>Vérifier si un utilisateur est identifié dans le contexte courant.</description></item>
    /// <item><description>Récupérer le nom complet de l’utilisateur à partir du service de requêtes <see cref="IQ_User"/>.</description></item>
    /// <item><description>Mettre à jour le paramètre <see cref="IS_Settings_User"/> avec le nom complet.</description></item>
    /// </list>
    /// </summary>
    public class SR_StartupContextUpdater : IS_StartupContextUpdater
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_Dictionary _dictionary;
        private readonly IQ_User _qhUser;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_StartupContextUpdater"/>.
        /// </summary>
        /// <param name="settingsApp">Service de gestion des paramètres applicatifs.</param>
        /// <param name="settingsUser">Service de gestion des paramètres utilisateur.</param>
        /// <param name="dictionary">Service d’accès au dictionnaire multilingue.</param>
        /// <param name="qhUser">Handler de requêtes pour la récupération des informations utilisateur.</param>
        public SR_StartupContextUpdater(
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser,
            IS_Dictionary dictionary,
            IQ_User qhUser)
        {
            _callee = GetType().Name;

            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
            _dictionary = dictionary;
            _qhUser = qhUser;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Met à jour le titre de l’application et, si un utilisateur est connecté,
        /// recharge son nom complet depuis la base de données.
        /// </para>
        ///
        /// <b>Contexte :</b>
        /// <para>
        /// Cette méthode est appelée à la fin du démarrage de l’application, après la
        /// vérification de l’environnement. Elle garantit la synchronisation entre
        /// les informations de configuration et l’affichage utilisateur.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Rafraîchir le titre et le contexte utilisateur pour garantir que les
        /// informations affichées à l’écran reflètent fidèlement l’état du système.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Appliquer le titre principal défini dans le dictionnaire.</description></item>
        /// <item><description>Vérifier la présence d’un identifiant utilisateur.</description></item>
        /// <item><description>Recharger le nom complet depuis la table <c>User</c>.</description></item>
        /// <item><description>Mettre à jour la propriété <see cref="IS_Settings_User.SetAppUserFullName"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">Chaîne d’appel utilisée pour la traçabilité (CallChain).</param>
        public async Task ExecuteAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            _settingsApp.SetApplicationTitle(_dictionary.GetText("App_Ti_00"));

            if (_settingsUser.GetAppUserId() > 0)
            {
                string? fullName = await _qhUser.HandleGetUserFullNameAsync(_settingsUser.GetAppUserId());
                if (!string.IsNullOrEmpty(fullName))
                    _settingsUser.SetAppUserFullName(fullName);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}