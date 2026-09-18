using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier responsable de la vérification de la connectivité entre l’application
    /// et la base de données principale. Il constitue la première étape de validation
    /// au démarrage du système.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé par le <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/> lors du processus
    /// d’initialisation de l’application. Il permet de s’assurer que la base de données
    /// est disponible et opérationnelle avant de poursuivre les vérifications suivantes.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Confirmer la disponibilité de la base de données et détecter tout problème
    /// de connectivité ou d’infrastructure avant l’ouverture de la fenêtre principale.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Établir une connexion à la base de données via le handler <see cref="IQ_User"/>.</description></item>
    /// <item><description>Exécuter une requête de test simple pour valider la communication.</description></item>
    /// <item><description>Retourner <c>true</c> si la base répond correctement, sinon <c>false</c>.</description></item>
    /// <item><description>Journaliser les erreurs de connectivité via <see cref="IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_StartupDatabaseConnectivity : IS_StartupDatabaseConnectivity
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_User _qhUser;
        private readonly IS_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_StartupDatabaseConnectivity"/>.
        /// </summary>
        /// <param name="qhUser">Handler de requêtes utilisateur utilisé pour tester la connectivité.</param>
        /// <param name="logAndNotify">Service centralisé de journalisation et notification.</param>
        public SR_StartupDatabaseConnectivity(IQ_User qhUser, IS_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;

            _qhUser = qhUser;
            _logAndNotify = logAndNotify;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Teste la connectivité de la base de données en effectuant une requête minimale.
        /// </para>
        ///
        /// <b>Contexte :</b>
        /// <para>
        /// Méthode appelée au lancement de l’application pour valider la disponibilité
        /// de la base avant le chargement du contexte utilisateur et des vues principales.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Vérifier que la base de données est accessible et répond correctement aux requêtes.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Construire une chaîne de traçabilité complète (CallChain).</description></item>
        /// <item><description>Exécuter une requête simple via le handler <see cref="IQ_User"/>.</description></item>
        /// <item><description>Retourner <c>true</c> si la base répond, sinon journaliser l’erreur et retourner <c>false</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (CallChain).</param>
        /// <returns><c>true</c> si la connexion réussit, sinon <c>false</c>.</returns>
        public async Task<bool> TestConnectionAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(TestConnectionAsync)}";

            try
            {
                var user = await _qhUser.HandleGetFirstOrDefaultAsync();
                return user != null;
            }
            catch (Exception ex)
            {
                // Ici on considère que toute exception est une erreur infra (DB down, timeout réseau, etc.).
                // On NE log PAS au niveau du service.
                // On ne notifie PAS l'utilisateur ici.
                // On laisse le UseCase décider quoi faire de cette info.
                System.Diagnostics.Debug.WriteLine($"[{callChain}] TestConnectionAsync infrastructure error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}