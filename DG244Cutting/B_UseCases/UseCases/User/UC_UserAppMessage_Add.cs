using System.Data;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// UseCase d'ajout d'un message applicatif à destination d'une application identifiée
    /// par son <c>AppList.Id</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : maillon 2 de la chaîne (1) d'écriture stricte
    /// VM → UC → SR → CH → CR posée en §4.14.9 et indexée par R-4.14.19. Seul composant de
    /// la chaîne autorisé à ouvrir, valider et annuler la transaction (R-4.10.1) et à
    /// appeler <c>SaveChangesAsync</c> (§4.10.4). Encapsulation dans
    /// <c>CreateExecutionStrategy().ExecuteAsync</c> exigée par <c>EnableRetryOnFailure</c>
    /// activé au câblage de <c>DigitTryDbContext</c> dans <c>SR_ConteneurDI</c>
    /// (R-4.10.2 amendée). Lecture du contexte applicatif courant via
    /// <c>IS_AppContext.GetAppContext()</c> à l'intérieur du délégué de la stratégie
    /// d'exécution, pour rejouabilité atomique en cas de réexécution.
    /// </para>
    /// <para>
    /// Objectif : orchestrer le scénario unitaire d'ajout d'un enregistrement
    /// <c>UserAppMessage</c> de manière transactionnelle et traçable, en déléguant la
    /// logique métier fine au Service spécialisé <c>IS_UserAppMessage_Add</c> et le
    /// traitement terminal des erreurs au pipeline <c>IU_LogAndNotify</c>.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire et propager la CallChain en début de scénario, conformément à §4.5.</description></item>
    /// <item><description>Ouvrir, valider ou annuler la transaction d'écriture, sous stratégie d'exécution (R-4.10.2 amendée).</description></item>
    /// <item><description>Résoudre le contexte applicatif courant via <c>IS_AppContext</c> à l'intérieur du délégué d'exécution.</description></item>
    /// <item><description>Déléguer l'action métier unitaire au Service <c>IS_UserAppMessage_Add</c>.</description></item>
    /// <item><description>Persister la mutation et l'enregistrement Event Store solidairement via <c>SaveChangesAsync</c>.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <c>IU_LogAndNotify</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine : elle est déléguée au Service spécialisé.</description></item>
    /// <item><description>Ne réalise aucun accès EF Core métier ni aucune requalification d'exception : ces rôles relèvent du Service et du Command Handler en aval.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository (I-4.14.4 amendée, I-4.14.6).</description></item>
    /// <item><description>Ne positionne pas les champs d'audit (CreatedAt, UpdatedAt, IsDeleted) : centralisé dans <c>CH_Generic&lt;T&gt;</c> (R-4.15.7).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_UserAppMessage_Add"/>
    public class UC_UserAppMessage_Add : IU_UserAppMessage_Add
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IS_UserAppMessage_Add _addService;
        private readonly IS_AppContext _appContext;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_UserAppMessage_Add"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la transaction du scénario.</param>
        /// <param name="addService">Service métier d'ajout d'un message applicatif.</param>
        /// <param name="appContext">Service fournissant le contexte applicatif courant.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_UserAppMessage_Add(
            DbContext dbContext,
            IS_UserAppMessage_Add addService,
            IS_AppContext appContext,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _addService = addService ?? throw new ArgumentNullException(nameof(addService));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Orchestre l'ajout d'un nouvel enregistrement <c>UserAppMessage</c> à destination
        /// de l'application désignée par <paramref name="idApplicationRecipient"/>, avec
        /// le sujet et le contenu fournis.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté typiquement par un ViewModel de page de composition de
        /// message applicatif. Le UseCase ouvre la transaction sous stratégie d'exécution,
        /// lit le contexte applicatif courant via <c>IS_AppContext.GetAppContext()</c> à
        /// l'intérieur du délégué, délègue la mutation au Service métier
        /// <c>IS_UserAppMessage_Add</c> en lui transmettant le <c>DTO_AppContext</c>
        /// obtenu et les trois arguments métier, persiste via <c>SaveChangesAsync</c> et
        /// valide la transaction. Toute exception applicative typée est traitée
        /// terminalement par <c>IU_LogAndNotify</c> et n'est pas propagée à l'appelant.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idApplicationRecipient">Identifiant de l'application destinataire du message. Doit être strictement positif.</param>
        /// <param name="subject">Sujet du message. Ne doit être ni <see langword="null"/>, ni vide, ni constitué exclusivement d'espaces.</param>
        /// <param name="content">Contenu textuel du message. Peut être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du scénario.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6.
        /// </exception>
        public async Task ExecuteAsync(
            string caller,
            int idApplicationRecipient,
            string subject,
            string? content,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // P6 - encapsulation dans la stratégie d'exécution exigée par EnableRetryOnFailure
            // (§4.10.1 du 0230, sous-bloc « Encapsulation dans une stratégie d'exécution » ;
            //  R-4.10.2 amendée du 0231). Sans elle, BeginTransactionAsync lève
            //  InvalidOperationException : l'execution strategy ne supporte pas les
            //  transactions initiées par l'utilisateur hors de ce délégué.
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database
                    .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

                try
                {
                    // Lecture du contexte applicatif courant à l'intérieur du délégué,
                    // pour rejouabilité atomique en cas de réexécution par la stratégie
                    // d'exécution (cohérence avec l'étalon doctrinal UC_UserAppSession_Open).
                    DTO_AppContext appContext = _appContext.GetAppContext();

                    // Délégation au Service métier (chaîne (1) maillon 3 :
                    // VM → UC → SR → CH → CR). Aucune précondition métier dupliquée ici :
                    // les préconditions structurelles sont portées par le Service en aval.
                    await _addService.ExecuteAsync(
                        callChain,
                        appContext,
                        idApplicationRecipient,
                        subject,
                        content,
                        ct);

                    // Persistance unifiée de la mutation et de l'enregistrement Event Store
                    // (R-4.10.1, R-4.10.4 ; SaveChangesAsync exclusif au UseCase).
                    await _dbContext.SaveChangesAsync(ct);

                    // Validation de la transaction (R-4.10.4).
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
                    // Annulation coopérative : la transaction est annulée par le Dispose
                    // du bloc await using interne au délégué de la stratégie d'exécution.
                    // Aucune journalisation ni notification (l'annulation n'est pas une
                    // erreur). Aucune réexécution par la stratégie d'exécution
                    // (OperationCanceledException n'est pas une défaillance transitoire).
                    throw;
                }
            });
        }

        #endregion

        #region === Méthodes privées ===

        // Aucune méthode privée requise pour le scénario courant.

        #endregion
    }
}