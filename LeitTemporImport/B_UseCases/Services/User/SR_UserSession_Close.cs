using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Services.User;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Services.User
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service User de fermeture de session applicative (<see cref="UserAppSession"/>).
    /// Orchestration : charger la session cible, vérifier la cohérence utilisateur, puis
    /// marquer la session comme déconnectée en mettant à jour les informations associées.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé lors de l’arrêt du programme console afin de clôturer proprement la session utilisateur
    /// ouverte au démarrage (IsConnected = false + DisconnectionDate).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Garantir une fermeture cohérente et traçable de la session courante, en réutilisant la mécanique
    /// de CommandHandlers/QueryHandlers déjà en place (avec journalisation Event Store via le CH).
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Pipeline de fermeture console / UseCases User.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Valider les paramètres d’entrée.</description></item>
    /// <item><description>Charger la session par Id.</description></item>
    /// <item><description>Vérifier la cohérence <c>IdUser</c> (sécurité fonctionnelle).</description></item>
    /// <item><description>Mettre à jour la session en état déconnecté.</description></item>
    /// <item><description>Reclassifier les exceptions via <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_UserSession_Close : IS_UserSession_Close
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserSession _qhUserSession;
        private readonly IC_UserSession _chUserSession;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service <see cref="SR_UserSession_Close"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires au chargement et à la mise à jour des sessions.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les dépendances injectées.</description></item>
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="userSessionCommand">CommandHandler de <see cref="UserAppSession"/>.</param>
        /// <param name="userSessionQuery">QueryHandler de <see cref="UserAppSession"/>.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est null.</exception>
        public SR_UserSession_Close(
            IC_UserSession userSessionCommand,
            IQ_UserSession userSessionQuery)
        {
            _callee = GetType().Name;
            _chUserSession = userSessionCommand ?? throw new ArgumentNullException(nameof(userSessionCommand));
            _qhUserSession = userSessionQuery ?? throw new ArgumentNullException(nameof(userSessionQuery));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Ferme la session utilisateur spécifiée (déconnexion).</para>
        /// <para>Contexte</para>
        /// <para>
        /// Appelé lors de la fermeture du programme console. La session est identifiée par son Id.
        /// </para>
        /// <para>Objectif</para>
        /// <para>Marquer la session comme déconnectée et renseigner la date de déconnexion via le CommandHandler.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Construire la callChain.</description></item>
        /// <item><description>Valider <paramref name="userId"/> et <paramref name="sessionId"/>.</description></item>
        /// <item><description>Charger la session cible.</description></item>
        /// <item><description>Vérifier qu’elle appartient à l’utilisateur courant.</description></item>
        /// <item><description>Appliquer la mise à jour (IsConnected=false).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="userId">Identifiant utilisateur.</param>
        /// <param name="sessionId">Identifiant de session à fermer.</param>
        public async Task ExecuteAsync(string caller, int userId, int sessionId)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (userId <= 0) return;
                if (sessionId <= 0) return;

                UserAppSession? existingSession = await _qhUserSession.HandleGetByIdAsync(callChain,sessionId);

                if (existingSession is null)
                    return;

                // Sécurité fonctionnelle : on ne ferme pas une session d’un autre utilisateur
                if (existingSession.IdUser != userId)
                    throw new Ex_Business($"UserAppSession mismatch: sessionId={sessionId} does not belong to userId={userId}.");

                await _chUserSession.HandleUpdateUserSessionAsync(existingSession, false, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}