using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// <b>Service de diagnostic ou de vérification</b> chargé de vérifier la disponibilité de l’application dans le système.
    /// Il interroge la table ou la vue de suivi applicatif afin de déterminer
    /// si l’application est actuellement ouverte à l’utilisation ou verrouillée
    /// pour maintenance, mise à jour ou autre raison.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est utilisé au démarrage de l’application par le 
    /// <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/> pour s’assurer que l’accès
    /// à l’application est autorisé avant d’initialiser les vues principales.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Garantir que l’application n’est pas en maintenance ou désactivée.
    /// Empêcher l’ouverture d’une session utilisateur si l’application
    /// est temporairement inaccessible.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Interroger la table de surveillance applicative (<c>vie_application</c>).</description></item>
    /// <item><description>Déterminer si l’application est marquée comme accessible.</description></item>
    /// <item><description>Retourner <c>true</c> si l’accès est autorisé, sinon <c>false</c>.</description></item>
    /// <item><description>Journaliser les erreurs de connexion ou d’infrastructure via <see cref="IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_ApplicationAvailability : IS_ApplicationAvailability
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_VieApplication _qhVieApplication;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_ApplicationAvailability"/>.
        /// </summary>
        /// <param name="qhVieApplication">Handler de requêtes pour la vue de disponibilité applicative.</param>
        public SR_ApplicationAvailability(IQ_VieApplication qhVieApplication)
        {
            _callee = GetType().Name;

            _qhVieApplication = qhVieApplication;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Vérifie si l’application est actuellement accessible aux utilisateurs.
        /// </para>
        ///
        /// <b>Contexte :</b>
        /// <para>
        /// Cette méthode est appelée lors du démarrage de l’application pour éviter
        /// d’ouvrir une session lorsque l’application est verrouillée ou en maintenance.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Empêcher tout accès non autorisé à l’application en cas d’inaccessibilité temporaire.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Construire la chaîne de traçabilité complète (<c>callChain</c>).</description></item>
        /// <item><description>Interroger la vue <c>vie_application</c> pour déterminer le statut de l’application.</description></item>
        /// <item><description>Retourner <c>true</c> si l’application est accessible, sinon journaliser l’erreur et retourner <c>false</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="appId">Identifiant unique de l’application à vérifier.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (CallChain).</param>
        /// <returns><c>true</c> si l’application est accessible, sinon <c>false</c>.</returns>
        public async Task<bool> IsAppAccessibleAsync(int appId, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(IsAppAccessibleAsync)}";

            try
            {
                bool result = await _qhVieApplication.HandleGetApplicationAccessibilityAsync(appId);
                return result;
            }
            catch (Exception ex)
            {
                // Si une erreur survient, il s’agit d’un problème d’infrastructure (base indisponible)
                throw new Ex_Infrastructure(callChain, "IAA_01", "No_Er_In_07", ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}