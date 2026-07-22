using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// UseCase orchestrateur de l'ouverture (ou réactivation) d'une session applicative utilisateur.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/UseCases/User</c>. Il est résolu par injection de dépendances et
    /// constitue le point unique d'orchestration du scénario d'ouverture de session. Il
    /// pilote la transaction d'écriture, délègue les traitements unitaires aux Services
    /// spécialisés (création, mise à jour, suppression des sessions supplémentaires),
    /// consomme le Query Handler pour la lecture, propage l'identifiant de session retenu
    /// dans le contexte utilisateur, et porte le traitement terminal des erreurs.
    /// </para>
    /// <para>
    /// Objectif : garantir qu'une unique session active existe pour le couple
    /// (utilisateur, application) et que son identifiant soit disponible dans le contexte
    /// utilisateur global, de manière transactionnelle et traçable.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ouvrir la transaction d'écriture et la valider ou l'annuler selon l'issue.</description></item>
    /// <item><description>Rechercher les sessions existantes et déterminer la session principale via le Query Handler.</description></item>
    /// <item><description>Déléguer la création, la mise à jour et la suppression des sessions aux Services spécialisés.</description></item>
    /// <item><description>Propager l'identifiant de session retenu dans <see cref="ISE_User"/>.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/>.</description></item>
    /// <item><description>Exposer un retour signalable booléen (<see langword="true"/> = succès, <see langword="false"/> = échec applicatif capté) destiné à un éventuel UseCase orchestrant amont, conformément à la clause de chaîne d'appel UseCase → UseCase de §4.14.2.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine : elle est déléguée aux Services spécialisés.</description></item>
    /// <item><description>Ne réalise aucun accès EF Core métier ni aucune requalification d'exception : ces rôles relèvent des Services et Handlers.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_UserAppSession_Open"/>
    public class UC_UserAppSession_Open : IU_UserAppSession_Open
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IQ_UserAppSession _qhUserAppSession;
        private readonly IS_UserAppSession_Create _sessionCreateService;
        private readonly IS_UserAppSession_Update _sessionUpdateService;
        private readonly IS_UserAppSession_DeleteExtra _sessionDeleteExtraService;
        private readonly IS_AppContext _appContext;
        private readonly ISE_User _seUser;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_UserAppSession_Open"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la transaction du scénario.</param>
        /// <param name="qhUserAppSession">Query Handler de lecture des sessions utilisateur.</param>
        /// <param name="sessionCreateService">Service de création d'une session connectée.</param>
        /// <param name="sessionUpdateService">Service de mise à jour d'une session existante.</param>
        /// <param name="sessionDeleteExtraService">Service de suppression des sessions supplémentaires.</param>
        /// <param name="appContext">Service fournissant le contexte applicatif courant.</param>
        /// <param name="seUser">Setting utilisateur courant, cible de la propagation de l'identifiant de session.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_UserAppSession_Open(
            DbContext dbContext,
            IQ_UserAppSession qhUserAppSession,
            IS_UserAppSession_Create sessionCreateService,
            IS_UserAppSession_Update sessionUpdateService,
            IS_UserAppSession_DeleteExtra sessionDeleteExtraService,
            IS_AppContext appContext,
            ISE_User seUser,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _qhUserAppSession = qhUserAppSession ?? throw new ArgumentNullException(nameof(qhUserAppSession));
            _sessionCreateService = sessionCreateService ?? throw new ArgumentNullException(nameof(sessionCreateService));
            _sessionUpdateService = sessionUpdateService ?? throw new ArgumentNullException(nameof(sessionUpdateService));
            _sessionDeleteExtraService = sessionDeleteExtraService ?? throw new ArgumentNullException(nameof(sessionDeleteExtraService));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Ouvre ou réactive la session applicative de l'utilisateur courant pour
        /// l'application courante, en garantissant l'unicité de la session active.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté au démarrage du programme console. L'identité de
        /// l'utilisateur et de l'application est lue depuis le contexte applicatif
        /// courant. Ouvre la transaction, orchestre la préparation de l'état des sessions
        /// (création ou activation + suppression des doublons) via les Services
        /// spécialisés, propage l'identifiant de session retenu dans le contexte
        /// utilisateur, persiste et valide la transaction. Toute exception typée est
        /// traitée par le pipeline terminal et signalée par <see langword="false"/>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles issues du contexte applicatif.</description></item>
        /// <item><description>Préparer l'état des sessions via les Services spécialisés.</description></item>
        /// <item><description>Propager l'identifiant de session retenu dans <see cref="ISE_User"/>.</description></item>
        /// <item><description>Persister et valider la transaction, ou l'annuler en cas d'échec.</description></item>
        /// <item><description>Signaler l'issue du scénario par un retour booléen.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// <see langword="true"/> si l'ouverture de session a abouti (transaction validée et
        /// identifiant de session propagé dans <see cref="ISE_User"/>) ; <see langword="false"/>
        /// si une exception applicative typée a été captée et traitée terminalement par
        /// <see cref="IU_LogAndNotify"/>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6.
        /// </exception>
        public async Task<bool> ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // P6 — encapsulation dans la stratégie d'exécution exigée par EnableRetryOnFailure
            // (§4.10.1 du 0230, sous-bloc « Encapsulation dans une stratégie d'exécution » ;
            //  R-4.10.2 amendée du 0231). Sans elle, BeginTransactionAsync lève
            //  InvalidOperationException : l'execution strategy ne supporte pas les
            //  transactions initiées par l'utilisateur hors de ce délégué.
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database
                    .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

                try
                {
                    DTO_AppContext appCtx = _appContext.GetAppContext();

                    if (appCtx.AppUserId <= 0)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_02,
                            $"L'identifiant utilisateur fourni pour l'ouverture de session est invalide : {appCtx.AppUserId}. Doit être strictement positif.");

                    if (appCtx.AppId <= 0)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_02,
                            $"L'identifiant application fourni pour l'ouverture de session est invalide : {appCtx.AppId}. Doit être strictement positif.");

                    List<UserAppSession> existingSessions =
                        await _qhUserAppSession.HandleGetByUserIdAppIdAsync(callChain, appCtx.AppUserId, appCtx.AppId, ct);

                    int retainedSessionId;

                    if (existingSessions.Count == 0)
                    {
                        await _sessionCreateService.ExecuteAsync(callChain, appCtx, ct);

                        await _dbContext.SaveChangesAsync(ct);

                        retainedSessionId =
                            await _qhUserAppSession.HandleGetSessionIdAsync(callChain, appCtx.AppUserId, appCtx.AppId, ct);

                        if (retainedSessionId <= 0)
                            throw new Ex_Business(
                                callChain,
                                Ex_Business.ErrorCodes.BU_ER_04,
                                $"Aucune session disponible après création pour l'utilisateur {appCtx.AppUserId} et l'application {appCtx.AppId}.");
                    }
                    else
                    {
                        retainedSessionId =
                            await _qhUserAppSession.HandleGetSessionIdAsync(callChain, appCtx.AppUserId, appCtx.AppId, ct);

                        if (retainedSessionId <= 0)
                            throw new Ex_Business(
                                callChain,
                                Ex_Business.ErrorCodes.BU_ER_04,
                                $"Aucune session principale déterminée pour l'utilisateur {appCtx.AppUserId} et l'application {appCtx.AppId}.");

                        UserAppSession mainSession =
                            existingSessions.First(session => session.Id == retainedSessionId);

                        await _sessionUpdateService.ExecuteAsync(callChain, mainSession, true, appCtx, ct);

                        List<UserAppSession> additionalSessions = existingSessions
                            .Where(session => session.Id != retainedSessionId)
                            .ToList();

                        if (additionalSessions.Count > 0)
                            await _sessionDeleteExtraService.ExecuteAsync(callChain, additionalSessions, ct);
                    }

                    _seUser.AppSessionId = retainedSessionId;

                    await _dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    return true;
                }
                catch (Ex_Business ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
                    return false;
                }
                catch (Ex_Infrastructure ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
                    return false;
                }
                catch (Ex_Unclassified ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
                    return false;
                }
                catch (OperationCanceledException)
                {
                    // Annulation coopérative : rollback implicite par le Dispose du bloc await using.
                    // Ne pas appeler RollbackAsync ici — il serait redondant.
                    // Ne pas appeler IU_LogAndNotify — l'annulation n'est pas une erreur.
                    // Ne pas retourner de booléen — l'annulation est propagée à l'appelant
                    // selon le mécanisme normatif de §4.6, distinct de la signalisation
                    // d'échec applicatif entre UseCases orchestrants.
                    // Cf. section 4.6 pour la mécanique d'annulation coopérative.
                    throw;
                }
            });
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}