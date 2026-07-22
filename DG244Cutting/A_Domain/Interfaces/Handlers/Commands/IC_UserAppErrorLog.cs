using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Commands
{
    /// <summary>
    /// Contrat du Command Handler dédié à la persistance immédiate et autonome des
    /// enregistrements de log d'erreurs applicatifs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans A_Domain, positionnée dans
    /// A_Domain/Interfaces/Handlers/Commands. Son implémentation concrète
    /// <c>CH_UserAppErrorLog</c> réside dans B_UseCases/Handlers/Commands.
    /// </para>
    /// <para>
    /// Objectif : garantir un contrat d'écriture autonome pour les logs d'erreurs
    /// applicatifs, dont la persistance est strictement indépendante de toute transaction
    /// UseCase en cours — contrairement aux Command Handlers génériques dont l'écriture
    /// s'inscrit dans la transaction de l'orchestrateur.
    /// </para>
    /// <para>
    /// Positionnement dans le pipeline : cette interface est appelée exclusivement par
    /// <c>SR_ErrorLogger</c>. Elle ne doit jamais être invoquée directement depuis un
    /// UseCase, un Service métier ou un ViewModel.
    /// </para>
    /// <para>
    /// Exception architecturale de persistance : contrairement à <see cref="IC_Generic{T}"/>,
    /// dont les méthodes participent à la transaction du UseCase orchestrateur, cette
    /// interface garantit une persistance immédiate et isolée, réalisée dans un DbContext
    /// de courte durée créé et disposé par l'implémentation. Un enregistrement de log doit
    /// survivre à tout rollback transactionnel externe, y compris celui de la transaction
    /// principale du UseCase ayant conduit à l'erreur.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Déclarer le seul contrat d'écriture autorisé pour les logs d'erreurs applicatifs,
    ///     avec persistance immédiate et autonome garantie.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne construit pas l'entité : cette responsabilité appartient à <c>SR_ErrorLogger</c>,
    ///     qui assemble l'entité <see cref="UserAppErrorLog"/> à partir du contexte applicatif
    ///     et de l'exception normalisée avant de déléguer à ce handler.
    ///   </description></item>
    ///   <item><description>
    ///     Ne journalise pas dans l'Event Store : les logs d'erreurs ne constituent pas des
    ///     événements métier au sens de <c>IC_UserAppEventStore</c>.
    ///   </description></item>
    /// </list>
    /// </remarks>
    public interface IC_UserAppErrorLog
    {
        /// <summary>
        /// Persiste immédiatement un enregistrement de log d'erreur dans un DbContext EF Core
        /// indépendant de toute transaction UseCase en cours.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée exclusivement depuis <c>SR_ErrorLogger</c>, après que l'entité
        /// <see cref="UserAppErrorLog"/> a été entièrement construite à partir du contexte
        /// applicatif et de l'exception normalisée.
        /// </para>
        /// <para>
        /// Objectif : persister le log d'erreur de manière atomique et isolée. La persistance
        /// est réalisée dans un DbContext de courte durée, commité avant le retour de la
        /// méthode, indépendamment de toute transaction externe ouverte dans le scope DI
        /// courant.
        /// </para>
        /// <para>
        /// Nommage délibéré : le suffixe <c>AndSave</c> signale explicitement que cette méthode
        /// gère elle-même la persistance (SaveChanges), contrairement aux méthodes
        /// <c>HandleAddAsync</c> de <see cref="IC_Generic{T}"/> qui délèguent le
        /// SaveChanges au UseCase orchestrateur via la transaction partagée.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour
        /// enrichissement et traçabilité.</param>
        /// <param name="entity">
        /// Entité <see cref="UserAppErrorLog"/> entièrement construite par <c>SR_ErrorLogger</c>,
        /// prête à être persistée. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière
        /// coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="entity"/> est <see langword="null"/> (code
        /// <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la persistance (code
        /// <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant
        /// l'exécution.
        /// </exception>
        Task HandleAddAndSaveAsync(string caller, UserAppErrorLog entity, CancellationToken ct = default);
    }
}