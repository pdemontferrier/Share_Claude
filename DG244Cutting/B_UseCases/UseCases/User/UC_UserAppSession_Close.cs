using System.Data;
using Microsoft.EntityFrameworkCore;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
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
    /// <item><description>Après validation transactionnelle, ramener l'application à l'état déconnecté : réinitialiser le contexte utilisateur partagé (<see cref="ISE_User.Reset"/>) puis déclencher la navigation vers la page par défaut (<see cref="IU_Navigation.NavigateToDefaultAsync"/>), qui résout mécaniquement vers la page de connexion une fois l'identité utilisateur remise à zéro.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/>.</description></item>
    /// <item><description>Exposer un retour signalable booléen (<see langword="true"/> = succès, <see langword="false"/> = échec applicatif capté) destiné à un éventuel UseCase orchestrant amont, conformément à la clause de chaîne d'appel UseCase → UseCase de §4.14.2.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine de mutation : elle est déléguée au Service spécialisé.</description></item>
    /// <item><description>Ne réalise aucun accès EF Core métier ni aucune requalification d'exception : ces rôles relèvent des Services et Handlers.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository.</description></item>
    /// <item><description>Ne porte pas la logique de résolution de la page cible : la sélection de la page par défaut (et sa résolution vers la page de connexion en l'absence d'utilisateur identifié) est déléguée au UseCase de navigation consommé en sous-séquence.</description></item>
    /// </list>
    /// <para>Participation à la chaîne UseCase → UseCase normalisée (R-4.14.21) :</para>
    /// <list type="bullet">
    /// <item><description>Consommé en sous-séquence : le retour signalable <see cref="bool"/> est exploité par valeur par un UseCase orchestrant amont (fermeture applicative).</description></item>
    /// <item><description>Orchestrateur amont : consomme <see cref="IU_Navigation"/> en sous-séquence post-commit via <see cref="IU_Navigation.NavigateToDefaultAsync"/>. Le UseCase consommé expose un retour non signalable (<see cref="Task"/>) admis sous R-4.14.21 : il encapsule son propre traitement terminal des erreurs et n'a rien à signaler par valeur. L'appel est positionné hors de toute imbrication transactionnelle (post-<c>CommitAsync</c>), conformément à I-4.10.3.</description></item>
    /// </list>
    /// <para>
    /// Traçabilité de mode (dérogation actée) : le comportement de déconnexion d'état et
    /// de navigation post-fermeture a été introduit par un fil d'Extension qui n'a ajouté
    /// AUCUNE méthode publique au contrat <see cref="IU_UserAppSession_Close"/>. L'Extension
    /// a élargi le périmètre fonctionnel observable de la méthode publique existante
    /// <see cref="ExecuteAsync"/> (déconnexion d'état + navigation) et introduit deux
    /// dépendances au constructeur (<see cref="ISE_User"/>, <see cref="IU_Navigation"/>).
    /// Le contrat reste inchangé ; l'invariant d'unicité de la méthode publique du cas
    /// Entité est préservé par construction.
    /// </para>
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
        private readonly ISE_User _seUser;
        private readonly IU_Navigation _uNavigation;

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
        /// <param name="seUser">État utilisateur partagé, réinitialisé post-commit pour ramener l'application à l'état déconnecté.</param>
        /// <param name="uNavigation">UseCase de navigation consommé en sous-séquence post-commit pour rejoindre la page par défaut.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_UserAppSession_Close(
            DbContext dbContext,
            IQ_UserAppSession qhUserAppSession,
            IS_UserAppSession_Update sessionUpdateService,
            IS_AppContext appContext,
            IU_LogAndNotify logAndNotify,
            ISE_User seUser,
            IU_Navigation uNavigation)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _qhUserAppSession = qhUserAppSession ?? throw new ArgumentNullException(nameof(qhUserAppSession));
            _sessionUpdateService = sessionUpdateService ?? throw new ArgumentNullException(nameof(sessionUpdateService));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
            _uNavigation = uNavigation ?? throw new ArgumentNullException(nameof(uNavigation));
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
        /// terminal et signalée par <see langword="false"/>.
        /// </para>
        /// <para>
        /// Effets post-commit (inconditionnels, chemin de succès transactionnel) : une fois
        /// la transaction validée, l'application est ramenée à l'état déconnecté,
        /// indépendamment du constat de présence ou d'absence de session correspondante.
        /// Deux effets s'appliquent dans l'ordre, hors du bloc de présence de session :
        /// (a) réinitialisation du contexte utilisateur partagé via <see cref="ISE_User.Reset"/>,
        /// qui remet notamment l'identifiant utilisateur courant à zéro ; puis
        /// (b) navigation vers la page par défaut via <see cref="IU_Navigation.NavigateToDefaultAsync"/>.
        /// La réinitialisation précédant la navigation, la page par défaut résout
        /// mécaniquement vers la page de connexion (l'utilisateur n'étant plus identifié),
        /// effet attendu d'une déconnexion. Ces effets sont non transactionnels (état mémoire
        /// partagé et navigation UI) et n'interviennent qu'après persistance confirmée de la
        /// fermeture de session ; ils ne se produisent pas sur le chemin d'échec applicatif.
        /// </para>
        /// <para>
        /// Retour : <see langword="true"/> si la fermeture de session a abouti (transaction
        /// validée — soit après passage effectif à l'état déconnecté, soit après constat
        /// non bloquant d'absence de session correspondante) ; <see langword="false"/> si
        /// une exception applicative typée a été captée et traitée terminalement par
        /// <see cref="IU_LogAndNotify"/>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles d'entrée.</description></item>
        /// <item><description>Charger la session cible et contrôler son appartenance utilisateur.</description></item>
        /// <item><description>Déléguer la mise à jour à l'état déconnecté au Service spécialisé.</description></item>
        /// <item><description>Persister et valider la transaction, ou l'annuler en cas d'échec.</description></item>
        /// <item><description>Post-commit, réinitialiser le contexte utilisateur partagé puis déclencher la navigation vers la page par défaut.</description></item>
        /// <item><description>Signaler l'issue du scénario par un retour booléen.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="sessionId">Identifiant de la session à fermer. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// <see langword="true"/> si la fermeture de session a abouti (transaction validée) ;
        /// <see langword="false"/> si une exception applicative typée a été captée et traitée
        /// terminalement par <see cref="IU_LogAndNotify"/>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6.
        /// </exception>
        public async Task<bool> ExecuteAsync(string caller, int sessionId, CancellationToken ct = default)
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

                    // Effets post-commit inconditionnels (chemin de succès transactionnel).
                    // Non transactionnels (état mémoire partagé + navigation UI), appliqués
                    // hors du bloc de présence de session : la déconnexion doit avoir lieu
                    // que la session ait été effectivement mise à jour ou non.
                    // Reset() remet AppUserId à 0 AVANT que NavigateToDefaultAsync ne le lise,
                    // ce qui fait résoudre la navigation vers la page de connexion.
                    _seUser.Reset();
                    await _uNavigation.NavigateToDefaultAsync(callChain, ct);

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