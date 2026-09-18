using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// UseCase orchestrateur du marquage comme lu d'un message applicatif existant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/UseCases/User</c>. Il est résolu par injection de dépendances et
    /// constitue le maillon 2 de la chaîne (1) d'écriture stricte
    /// VM → UC → SR → CH → CR (§4.14.9). Il est consommé en chaîne (1) directe par un
    /// ViewModel (typiquement un VM de page de consultation des messages applicatifs),
    /// sans sous-séquence amont prévue (signature <see cref="Task"/> simple, sans retour
    /// signalable). Il pilote la transaction d'écriture, délègue l'action métier unitaire
    /// au Service spécialisé <see cref="IS_UserAppMessage_MarkAsRead"/>, et porte le
    /// traitement terminal des erreurs par délégation exclusive à
    /// <see cref="IU_LogAndNotify"/>.
    /// </para>
    /// <para>
    /// Objectif : garantir que le marquage comme lu d'un message applicatif identifié
    /// soit appliqué sous une transaction unique, persisté de manière unifiée avec
    /// l'enregistrement Event Store solidaire, et que toute défaillance applicative soit
    /// traitée terminalement sans propagation à l'appelant.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ouvrir la transaction d'écriture et la valider ou l'annuler selon l'issue (§4.10.1, §4.10.4).</description></item>
    /// <item><description>Encapsuler le bloc transactionnel dans <c>CreateExecutionStrategy().ExecuteAsync</c> en présence d'<c>EnableRetryOnFailure</c> activé sur la connexion DigitTry (P6, R-4.10.2 amendée).</description></item>
    /// <item><description>Déléguer l'action métier unitaire au Service <see cref="IS_UserAppMessage_MarkAsRead"/>.</description></item>
    /// <item><description>Persister la mutation et l'enregistrement Event Store via un appel unique à <c>SaveChangesAsync</c> (R-4.10.4).</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/> dans les trois <c>catch</c> typés (R-4.7.14, I-4.7.6).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la logique métier fine (chargement de l'entité, positionnement de <c>IsRead</c>, délégation au Command Handler) : ces actions sont portées par le Service en aval (I-4.14.4 amendée).</description></item>
    /// <item><description>Ne valide pas la précondition structurelle sur <c>messageId</c> : cette validation est documentée comme responsabilité du Service en aval (code <c>BU_ER_02</c>).</description></item>
    /// <item><description>Ne réalise aucun accès EF Core métier ni aucune requalification d'exception : ces rôles relèvent des Services et Handlers en aval (I-4.14.4 amendée).</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler (I-4.14.4 amendée) ni un Repository.</description></item>
    /// <item><description>Ne positionne aucun champ d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) : ce rôle est centralisé dans le Command Handler générique (R-4.15.7).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_UserAppMessage_MarkAsRead"/>
    public class UC_UserAppMessage_MarkAsRead : IU_UserAppMessage_MarkAsRead
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IS_UserAppMessage_MarkAsRead _markAsReadService;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_UserAppMessage_MarkAsRead"/>
        /// avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">
        /// Contexte de persistance partagé, support de la transaction du scénario.
        /// </param>
        /// <param name="markAsReadService">
        /// Service spécialisé portant l'action métier unitaire de marquage comme lu :
        /// chargement de l'entité ciblée via <c>IQ_Generic&lt;UserAppMessage&gt;</c>,
        /// positionnement de <c>IsRead</c> à <see langword="true"/>, délégation de la
        /// mise à jour à <c>IC_Generic&lt;UserAppMessage&gt;</c>.
        /// </param>
        /// <param name="logAndNotify">
        /// Pipeline terminal de journalisation et de notification des erreurs.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'un des paramètres est <see langword="null"/>.
        /// </exception>
        public UC_UserAppMessage_MarkAsRead(
            DbContext dbContext,
            IS_UserAppMessage_MarkAsRead markAsReadService,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _markAsReadService = markAsReadService ?? throw new ArgumentNullException(nameof(markAsReadService));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Orchestre le marquage comme lu par son destinataire du message applicatif
        /// identifié par son entier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : maillon 2 de la chaîne (1) d'écriture stricte
        /// VM → UC → SR → CH → CR (§4.14.9). Seul composant de la chaîne autorisé à
        /// ouvrir, valider et annuler la transaction (R-4.10.1) et à appeler
        /// <c>SaveChangesAsync</c> (R-4.10.4). L'encapsulation du bloc transactionnel
        /// dans <c>CreateExecutionStrategy().ExecuteAsync</c> est exigée par
        /// <c>EnableRetryOnFailure</c> activé sur la connexion DigitTry (P6, §4.10.1
        /// sous-bloc « Encapsulation dans une stratégie d'exécution » ; R-4.10.2
        /// amendée). Sans elle, <c>BeginTransactionAsync</c> lèverait au runtime
        /// <see cref="InvalidOperationException"/> : la stratégie d'exécution ne
        /// supporte pas les transactions initiées par l'utilisateur hors du délégué.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire la CallChain au format normatif et la propager au Service consommé en aval (R-4.5.5, R-4.5.7).</description></item>
        /// <item><description>Ouvrir la transaction en isolation <see cref="IsolationLevel.ReadCommitted"/>.</description></item>
        /// <item><description>Déléguer l'action métier unitaire au Service <see cref="IS_UserAppMessage_MarkAsRead"/>.</description></item>
        /// <item><description>Persister via <c>SaveChangesAsync</c> et valider via <c>CommitAsync</c>.</description></item>
        /// <item><description>Annuler la transaction et déléguer à <see cref="IU_LogAndNotify"/> en cas d'exception applicative typée.</description></item>
        /// <item><description>Propager <see cref="OperationCanceledException"/> à l'appelant sans journalisation ni notification (§4.6).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="messageId">
        /// Identifiant du message applicatif à marquer comme lu. La validation
        /// structurelle (positivité stricte) est portée par le Service en aval
        /// (code <c>BU_ER_02</c>).
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative. Par défaut <see langword="default"/>.
        /// </param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée,
        /// conformément à §4.6. Les exceptions applicatives typées
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles sont captées
        /// terminalement par les trois <c>catch</c> typés et traitées par
        /// <see cref="IU_LogAndNotify"/>.
        /// </exception>
        public async Task ExecuteAsync(string caller, int messageId, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // P6 — encapsulation dans la stratégie d'exécution exigée par EnableRetryOnFailure
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
                    // Délégation au Service métier — chaîne (1) maillon 3
                    // (VM → UC → SR → CH → CR).
                    await _markAsReadService.ExecuteAsync(callChain, messageId, ct);

                    // Persistance unifiée — appel unique à SaveChangesAsync par le
                    // UseCase orchestrateur (R-4.10.4).
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
                    // Annulation coopérative : rollback implicite par le Dispose du bloc
                    // await using. Ne pas appeler RollbackAsync ici — il serait redondant.
                    // Ne pas appeler IU_LogAndNotify — l'annulation n'est pas une erreur.
                    // Aucune réexécution par la stratégie d'exécution :
                    // OperationCanceledException n'est pas une défaillance transitoire.
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