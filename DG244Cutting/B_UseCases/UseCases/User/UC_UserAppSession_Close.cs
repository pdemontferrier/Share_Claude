using System.Data;
using Microsoft.EntityFrameworkCore;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// UseCase orchestrateur de la fermeture (déconnexion) d'une session applicative utilisateur.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/UseCases/User</c>. Il est résolu par injection de dépendances et
    /// constitue le point unique d'orchestration du scénario de fermeture de session. Il
    /// pilote la transaction d'écriture, consomme le Query Handler pour le chargement de
    /// cohérence, porte le contrôle de sécurité fonctionnelle (appartenance utilisateur),
    /// délègue la mise à jour à l'état déconnecté au Service spécialisé, et porte le
    /// traitement terminal des erreurs. Il est le pendant symétrique de
    /// <see cref="IU_UserAppSession_Open"/>.
    /// </para>
    /// <para>
    /// Objectif : garantir une fermeture cohérente et traçable de la session courante, de
    /// manière transactionnelle, en réutilisant les Services unitaires déjà en place.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ouvrir la transaction d'écriture et la valider ou l'annuler selon l'issue.</description></item>
    /// <item><description>Charger la session cible via le Query Handler et contrôler son appartenance utilisateur.</description></item>
    /// <item><description>Déléguer la mise à jour à l'état déconnecté au Service spécialisé.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine de mutation : elle est déléguée au Service spécialisé.</description></item>
    /// <item><description>Ne réalise aucun accès EF Core métier ni aucune requalification d'exception : ces rôles relèvent des Services et Handlers.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_UserAppSession_Close"/>
    public class UC_UserAppSession_Close : IU_UserAppSession_Close
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IQ_UserAppSession _qhUserAppSession;
        private readonly IS_UserAppSession_Update _sessionUpdateService;
        private readonly IS_AppContext _appContext;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_UserAppSession_Close"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la transaction du scénario.</param>
        /// <param name="qhUserAppSession">Query Handler de lecture des sessions utilisateur.</param>
        /// <param name="sessionUpdateService">Service de mise à jour d'une session existante.</param>
        /// <param name="appContext">Service fournissant le contexte applicatif courant.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_UserAppSession_Close(
            DbContext dbContext,
            IQ_UserAppSession qhUserAppSession,
            IS_UserAppSession_Update sessionUpdateService,
            IS_AppContext appContext,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _qhUserAppSession = qhUserAppSession ?? throw new ArgumentNullException(nameof(qhUserAppSession));
            _sessionUpdateService = sessionUpdateService ?? throw new ArgumentNullException(nameof(sessionUpdateService));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Ferme la session applicative identifiée de l'utilisateur courant en la passant
        /// à l'état déconnecté, après contrôle d'appartenance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté à la fermeture du programme console. L'identité de
        /// l'utilisateur courant est lue depuis le contexte applicatif courant. Ouvre la
        /// transaction, charge la session via le Query Handler, contrôle son appartenance
        /// à l'utilisateur courant, délègue la mise à jour à l'état déconnecté au Service
        /// spécialisé, persiste et valide la transaction. L'absence de session
        /// correspondante est un cas non bloquant (retour sans mutation après validation
        /// d'une transaction vide). Toute exception typée est traitée par le pipeline
        /// terminal.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles d'entrée.</description></item>
        /// <item><description>Charger la session cible et contrôler son appartenance utilisateur.</description></item>
        /// <item><description>Déléguer la mise à jour à l'état déconnecté au Service spécialisé.</description></item>
        /// <item><description>Persister et valider la transaction, ou l'annuler en cas d'échec.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="sessionId">Identifiant de la session à fermer. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant utilisateur lu du contexte applicatif courant ou
        /// <paramref name="sessionId"/> n'est pas strictement positif, ou si la session
        /// chargée n'appartient pas à l'utilisateur courant.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture ou l'écriture en base échoue.</exception>
        public async Task ExecuteAsync(string caller, int sessionId, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            await using var transaction = await _dbContext.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            try
            {
                if (sessionId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de session fourni pour la fermeture est invalide : {sessionId}. Doit être strictement positif.");

                UserAppSession? existingSession =
                    await _qhUserAppSession.HandleGetByIdAsync(callChain, sessionId, ct);

                DTO_AppContext appCtx = _appContext.GetAppContext();

                if (appCtx.AppUserId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant utilisateur fourni pour la fermeture de session est invalide : {appCtx.AppUserId}. Doit être strictement positif.");

                if (existingSession is not null)
                {
                    // Sécurité fonctionnelle : on ne ferme pas une session d'un autre
                    // utilisateur. Ce contrôle est une décision d'orchestration, portée
                    // par le UseCase qui dispose de la vision globale du scénario.
                    if (existingSession.IdUser != appCtx.AppUserId)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_04,
                            $"Incohérence de session : la session {sessionId} n'appartient pas à l'utilisateur {appCtx.AppUserId}.");

                    await _sessionUpdateService.ExecuteAsync(callChain, existingSession, false, appCtx, ct);
                }

                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch (Ex_Business ex)
            {
                await transaction.RollbackAsync(ct);
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
            }
            catch (Ex_Infrastructure ex)
            {
                await transaction.RollbackAsync(ct);
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
            }
            catch (Ex_Unclassified ex)
            {
                await transaction.RollbackAsync(ct);
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
            }
            catch (OperationCanceledException)
            {
                // Annulation coopérative : rollback implicite par le Dispose du bloc await using.
                // Ne pas appeler RollbackAsync ici — il serait redondant.
                // Ne pas appeler IU_LogAndNotify — l'annulation n'est pas une erreur.
                // Cf. section 4.6 pour la mécanique d'annulation coopérative.
                throw;
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}