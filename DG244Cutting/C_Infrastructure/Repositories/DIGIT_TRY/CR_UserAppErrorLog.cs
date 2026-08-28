using Microsoft.EntityFrameworkCore;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.C_Infrastructure.Persistence.DIGIT_TRY.Context;

namespace DG244Cutting.C_Infrastructure.Repositories.DIGIT_TRY
{
    /// <summary>
    /// Repository concret dédié à la persistance immédiate et autonome des enregistrements
    /// de log d'erreurs applicatifs dans la table <c>UserAppErrorLog</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans C_Infrastructure et implémente
    /// <see cref="IR_UserAppErrorLog"/>. Elle constitue une exception documentée et justifiée
    /// au modèle général des repositories de la solution, qui partagent un DbContext injecté
    /// et délèguent la persistance au UseCase orchestrateur.
    /// </para>
    /// <para>
    /// Justification de l'exception : un enregistrement de log d'erreur doit être persisté
    /// immédiatement et de manière inconditionnelle, indépendamment de toute transaction
    /// UseCase en cours. Il doit notamment survivre à un rollback transactionnel, ce qui
    /// est structurellement impossible si l'écriture est inscrite dans un DbContext partagé
    /// soumis à la transaction du UseCase. Ce repository crée donc son propre contexte
    /// de courte durée via <see cref="IDbContextFactory{TContext}"/>, persiste l'enregistrement,
    /// puis dispose le contexte — le tout dans une opération atomique et isolée.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Créer un DbContext indépendant via <see cref="IDbContextFactory{TContext}"/> pour chaque appel.</description></item>
    ///   <item><description>Inscrire l'entité <see cref="UserAppErrorLog"/> dans ce contexte isolé.</description></item>
    ///   <item><description>Appeler <c>SaveChangesAsync</c> de manière autonome pour commiter immédiatement.</description></item>
    ///   <item><description>Requalifier les exceptions EF Core en types applicatifs via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne participe à aucune transaction UseCase : le contexte créé ici est entièrement isolé.</description></item>
    ///   <item><description>Ne journalise pas, ne notifie pas : ces responsabilités remontent via le pipeline d'erreurs.</description></item>
    ///   <item><description>N'hérite pas de <c>CR_Generic</c>, dont le contrat est incompatible avec ce comportement.</description></item>
    /// </list>
    /// </remarks>
    public class CR_UserAppErrorLog : IR_UserAppErrorLog
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Factory EF Core permettant de créer un DbContext de courte durée, indépendant
        /// de toute transaction UseCase en cours. Chaque appel à <see cref="AddAndSaveAsync"/>
        /// crée et dispose sa propre instance via cette factory.
        /// </summary>
        private readonly IDbContextFactory<DigitTryDbContext> _contextFactory;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="CR_UserAppErrorLog"/> avec la factory EF Core
        /// et le classificateur d'exceptions.
        /// </summary>
        /// <remarks>
        /// L'usage de <see cref="IDbContextFactory{TContext}"/> est intentionnel et constitue
        /// une exception architecturale documentée. Contrairement aux repositories génériques
        /// qui reçoivent un DbContext partagé, ce repository doit créer son propre contexte
        /// isolé à chaque opération afin de garantir la persistance inconditionnelle des
        /// enregistrements de log, y compris en cas de rollback transactionnel externe.
        /// </remarks>
        /// <param name="contextFactory">
        /// Factory EF Core pour la création de contextes de courte durée. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        public CR_UserAppErrorLog(IDbContextFactory<DigitTryDbContext> contextFactory, IS_ExClassifier classifier)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion


        #region === Méthodes publiques ===

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
        public async Task AddAndSaveAsync(string caller, UserAppErrorLog entity, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(AddAndSaveAsync)}";

            try
            {
                if (entity is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "L'entité UserAppErrorLog fournie est nulle.");

                ct.ThrowIfCancellationRequested();

                // Contexte de courte durée, indépendant de toute transaction UseCase en cours.
                // L'instruction using garantit la disposition du contexte à l'issue de l'opération,
                // qu'elle réussisse ou échoue.
                await using var context = _contextFactory.CreateDbContext();
                await context.Set<UserAppErrorLog>().AddAsync(entity, ct);
                await context.SaveChangesAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion


        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}