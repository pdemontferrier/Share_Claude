using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier chargé de vérifier l’intégrité des sessions utilisateur actives.
    /// Il permet de détecter si un même utilisateur tente d’ouvrir une session
    /// sur un autre poste alors qu’une session est déjà active.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé au démarrage de l’application depuis le <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/>
    /// afin de garantir la cohérence et l’unicité de la session utilisateur dans le système.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Identifier toute session utilisateur active sur un poste différent
    /// et empêcher la création d’une session concurrente.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Récupérer les sessions actives pour l’utilisateur et l’application courante.</description></item>
    /// <item><description>Comparer l’identifiant du poste avec celui de la session enregistrée.</description></item>
    /// <item><description>Retourner <c>true</c> si une autre session active est détectée.</description></item>
    /// <item><description>Journaliser toute erreur de vérification via <see cref="IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_UserSession_Integrity : IS_UserSession_Integrity
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserSession _qhUserSession;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_UserSession_Integrity"/>.
        /// </summary>
        /// <param name="qhUserSession">Handler de requêtes pour la lecture des sessions utilisateur.</param>
        public SR_UserSession_Integrity(IQ_UserSession qhUserSession)
        {
            _callee = GetType().Name;

            _qhUserSession = qhUserSession;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Vérifie si une session utilisateur est déjà active sur un autre poste
        /// et lève une exception classifiée en cas d’erreur d’infrastructure.
        /// </para>
        ///
        /// <b>Contexte :</b>
        /// <para>
        /// Cette méthode est appelée lors du démarrage de l’application, depuis le
        /// <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/>, afin de garantir qu’un utilisateur
        /// ne dispose que d’une seule session active dans le système.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Identifier un éventuel conflit de session (même utilisateur connecté sur un autre poste)
        /// et signaler toute erreur technique (ex. base de données inaccessible) via une exception
        /// classifiée pour traitement par le UseCase orchestrateur.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Construire la chaîne de traçabilité complète (<c>callChain</c>).</description></item>
        /// <item><description>Lire la liste des sessions actives pour l’utilisateur et l’application spécifiés.</description></item>
        /// <item><description>Comparer les identifiants de poste pour détecter d’éventuelles connexions multiples.</description></item>
        /// <item><description>Retourner <c>true</c> si une session concurrente est détectée, sinon <c>false</c>.</description></item>
        /// <item><description>Lancer une exception <see cref="Ex_Infrastructure"/> si une erreur technique survient.</description></item>
        /// </list>
        ///
        /// <b>Exceptions :</b>
        /// <list type="bullet">
        /// <item><description><see cref="Ex_Infrastructure"/> : Levée si la connexion à la base ou la lecture des sessions échoue.</description></item>
        /// </list>
        /// </summary>
        /// <param name="appId">Identifiant de l’application à vérifier.</param>
        /// <param name="userId">Identifiant de l’utilisateur concerné.</param>
        /// <param name="appDeviceId">Identifiant unique du poste local (device).</param>
        /// <param name="caller">Chaîne d’appel utilisée pour la traçabilité (<c>CallChain</c>).</param>
        /// <returns>
        /// <c>true</c> si une session active est détectée sur un autre poste, sinon <c>false</c>.
        /// </returns>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une erreur d’infrastructure empêche la vérification (ex. base de données indisponible).
        /// </exception>
        public async Task<bool> HasActiveSessionOnAnotherDeviceAsync(int appId, int userId, string appDeviceId, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HasActiveSessionOnAnotherDeviceAsync)}";

            try
            {
                var sessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(userId, appId);

                return sessions.Any(s => s.Connected && s.DeviceId != appDeviceId);
            }
            catch (Exception ex)
            {
                // Si une erreur survient, il s’agit d’un problème d’infrastructure (ex. base de données inaccessible)
                throw new Ex_Infrastructure(callChain, "HASOAD_01", "No_Er_In_07", ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}