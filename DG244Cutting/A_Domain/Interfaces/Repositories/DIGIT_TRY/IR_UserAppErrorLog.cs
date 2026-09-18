using DG244Cutting.A_Domain.Entities.DIGIT_TRY;

namespace DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY
{
    /// <summary>
    /// Contrat spécialisé du repository dédié à la persistance des enregistrements de log d'erreurs applicatifs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et constitue le contrat d'accès
    /// aux données pour la table <c>UserAppErrorLog</c>. Son implémentation concrète
    /// <c>CR_UserAppErrorLog</c> réside dans C_Infrastructure/Repositories/DG244Cutting/.
    /// </para>
    /// <para>
    /// Exception architecturale documentée : contrairement à <see cref="Generic.IR_Generic{T}"/>,
    /// dont le contrat suppose un DbContext partagé sans persistance autonome, cette interface
    /// impose une persistance immédiate et indépendante de toute transaction UseCase en cours.
    /// Cette exception est justifiée par la nature transversale du log d'erreurs : un enregistrement
    /// de log doit survivre à tout rollback transactionnel, y compris celui de la transaction
    /// au sein de laquelle l'erreur s'est produite. Son implémentation utilise
    /// <c>IDbContextFactory</c> et appelle <c>SaveChangesAsync</c> de manière autonome.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Déclarer le seul contrat d'écriture autorisé sur la table <c>UserAppErrorLog</c> :
    ///     une insertion immédiate et atomique, indépendante de toute transaction externe.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     N'expose aucune opération de lecture, de mise à jour ou de suppression :
    ///     la table <c>UserAppErrorLog</c> est en écriture seule depuis ce contrat.
    ///     Les consultations éventuelles relèvent d'un Query Handler dédié (<c>IQ_UserAppErrorLog</c>).
    ///   </description></item>
    ///   <item><description>
    ///     Ne partage pas le DbContext du UseCase appelant : toute notion de transaction
    ///     externe est intentionnellement absente de ce contrat.
    ///   </description></item>
    /// </list>
    /// </remarks>
    public interface IR_UserAppErrorLog
    {
        /// <summary>
        /// Insère un enregistrement de log d'erreur et le persiste immédiatement dans un contexte
        /// EF Core indépendant de toute transaction UseCase en cours.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Comportement garanti : l'enregistrement est commité atomiquement dans son propre
        /// contexte de courte durée, créé et disposé au sein de cette méthode. Il n'est pas
        /// affecté par un éventuel rollback transactionnel provenant du UseCase appelant.
        /// </para>
        /// <para>
        /// Nommage délibéré : le suffixe <c>AndSave</c> signale explicitement que cette méthode
        /// persiste de manière autonome, contrairement aux méthodes <c>AddAsync</c> de
        /// <see cref="Generic.IR_Generic{T}"/> qui délèguent la persistance au UseCase.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour enrichissement et traçabilité.</param>
        /// <param name="entity">
        /// Entité <see cref="UserAppErrorLog"/> entièrement construite par <c>SR_ErrorLogger</c>,
        /// prête à être persistée. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée si l'entité fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la création du contexte,
        /// de l'insertion ou de la persistance (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task AddAndSaveAsync(string caller, UserAppErrorLog entity, CancellationToken ct = default);
    }
}