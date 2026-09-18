using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable du retrait définitif du stock, par suppression logique, de la
    /// chute dont est issue une barre de production sortie du circuit de production.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_CuttingScrapStock_Withdraw"/>, par les UseCases orchestrateurs de la
    /// validation et du refus d'une barre de production, pour une barre issue d'une chute
    /// exclusivement. Il consomme directement <see cref="IQ_Generic{T}"/> pour la lecture préalable
    /// de la chute et <see cref="IC_Generic{T}"/> pour sa suppression logique, sur l'entité
    /// <see cref="CuttingScrapStock"/>.
    /// </para>
    /// <para>
    /// Objectif : le stock de chutes doit refléter ce qui se trouve physiquement dans les
    /// emplacements de rangement de l'atelier. Une chute portée par une barre qui sort du circuit
    /// de production est consommée ou écartée et ne doit plus jamais être proposée à
    /// l'optimisation. Le service délègue sa suppression logique au Command Handler générique ; le
    /// prédicat de disponibilité excluant toute chute supprimée logiquement, la chute quitte les
    /// recherches du stock. Le service n'expose aucune logique de persistance et n'assume aucune
    /// responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Le retrait du stock se décline en consommation par la coupe, consommation sans production et
    /// écart pour défaut ou absence. Ces trois situations convergent exactement sur la chute, et le
    /// service les traite à l'identique sans savoir laquelle l'invoque : la distinction relève du
    /// UseCase appelant, qui traite la barre et ses découpes et en porte la traçabilité.
    /// </para>
    /// <para>
    /// Rôle de la lecture préalable : la suppression logique générique rend la main sans erreur
    /// lorsque l'entité est absente et n'examine pas son état. Seule la lecture préalable permet
    /// donc de rejeter une chute introuvable ou déjà retirée. Cette lecture est suivie : la
    /// relecture opérée par la suppression logique générique retrouve la même instance dans le
    /// contexte de données partagé, sans requête supplémentaire. La réservation de la chute est
    /// conservée : elle porte la trace de la série de production au profit de laquelle la chute a
    /// été retenue.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle de l'argument.</description></item>
    /// <item><description>Charger la chute désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/>.</description></item>
    /// <item><description>Vérifier l'existence de la chute et qu'elle n'est pas déjà supprimée logiquement.</description></item>
    /// <item><description>Déléguer la suppression logique au Command Handler générique via <see cref="IC_Generic{T}.HandleSoftDeleteAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>N'affecte lui-même aucun champ de la chute : l'indicateur de suppression logique et la date de mise à jour relèvent exclusivement du Command Handler générique, et la mise à jour générique n'est pas invoquée.</description></item>
    /// <item><description>Ne distingue pas les situations de retrait du stock et ne connaît pas la barre de production ; la cohérence entre la chute et la barre relève de l'appelant.</description></item>
    /// <item><description>Ne contrôle ni la réservation, dont l'absence après une reprise sur incident ne doit pas empêcher le retrait, ni l'attente d'intégration, état de gestion du stock étranger au retrait.</description></item>
    /// <item><description>Ne libère pas la réservation : la libération relève de la clôture de la série et ne porte que sur les chutes non supprimées logiquement.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_CuttingScrapStock_Withdraw"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_CuttingScrapStock_Withdraw : IS_CuttingScrapStock_Withdraw
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du type concret, utilisé comme segment propre dans les CallChains construites
        /// par le service.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Query Handler générique auquel est déléguée la lecture suivie de la chute désignée.
        /// </summary>
        private readonly IQ_Generic<CuttingScrapStock> _queryHandler;

        /// <summary>
        /// Command Handler générique auquel est déléguée la suppression logique de la chute retirée.
        /// </summary>
        private readonly IC_Generic<CuttingScrapStock> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_CuttingScrapStock_Withdraw"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; la chute lue est ainsi l'instance que le Command
        /// Handler retrouve et que le contexte enregistrera.
        /// </para>
        /// </remarks>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture suivie de la chute. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la suppression logique de la chute. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="queryHandler"/>, <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_CuttingScrapStock_Withdraw(
            IQ_Generic<CuttingScrapStock> queryHandler,
            IC_Generic<CuttingScrapStock> commandHandler,
            IS_ExClassifier classifier)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Retire du stock, par suppression logique, la chute désignée, dont est issue une barre de
        /// production sortie du circuit de production, en confiant cette suppression logique au
        /// Command Handler générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, lors de la validation ou du refus d'une barre de production issue d'une chute.
        /// La chute est lue avec suivi des changements pour contrôler son existence et son état ;
        /// sa suppression logique est ensuite déléguée par identifiant au Command Handler générique,
        /// qui retrouve la même instance dans le contexte partagé, positionne l'indicateur de
        /// suppression logique et la date de mise à jour et inscrit un événement technique.
        /// L'enregistrement effectif n'intervient qu'à la validation de la transaction par
        /// l'appelant.
        /// </para>
        /// <para>
        /// Transactionnalité : le service ne modifie aucune donnée avant la délégation ; un échec
        /// survenant avant celle-ci laisse la chute intacte.
        /// </para>
        /// <para>
        /// Objectif : faire sortir définitivement du stock une chute consommée ou écartée, afin
        /// qu'elle ne soit plus jamais proposée à l'optimisation.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de chute est strictement positif.</description></item>
        /// <item><description>Charger la chute désignée en lecture suivie.</description></item>
        /// <item><description>Vérifier que la chute a été trouvée.</description></item>
        /// <item><description>Vérifier que la chute n'est pas déjà supprimée logiquement et, dans le cas contraire, citer sa réservation existante afin d'orienter le diagnostic vers la série concernée.</description></item>
        /// <item><description>Déléguer la suppression logique de la chute au Command Handler générique, par son identifiant.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucune propriété de l'instance chargée : la réservation, l'attente d'intégration et l'indicateur de suppression logique sont laissés en l'état à la délégation.</description></item>
        /// <item><description>N'invoque pas la mise à jour générique et n'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// <item><description>Ne contrôle ni la réservation, ni l'attente d'intégration, ni la cohérence de la chute avec la barre appelante.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idCuttingScrapStock">Identifiant de la chute dont est issue la barre de production sortie du circuit. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idCuttingScrapStock"/> n'est pas
        /// strictement positif (paramètre nommé et valeur reçue citée) ; avec le code
        /// <c>BU_ER_03</c> si la chute désignée est introuvable (identifiant cité) ; avec le code
        /// <c>BU_ER_04</c> si la chute est déjà supprimée logiquement (chute et réservation
        /// existante citées). Remonte également sans interception toute <see cref="Ex_Business"/>
        /// levée par le Query Handler ou le Command Handler, dont les contrôles structurels de la
        /// suppression logique générique.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la chute ou la délégation de sa suppression logique échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idCuttingScrapStock,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // P1 - Identifiant de chute strictement positif.
                if (idCuttingScrapStock <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de chute (idCuttingScrapStock) doit être strictement positif ; valeur reçue : {idCuttingScrapStock}.");

                ct.ThrowIfCancellationRequested();

                // L - Lecture SUIVIE : la suppression logique générique retrouvera cette instance
                // par l'identity map du contexte partagé. Aucune variante AsNoTracking.
                CuttingScrapStock? scrap = await _queryHandler.HandleGetByIdAsync(
                    callChain,
                    idCuttingScrapStock,
                    ct);

                // C1 - La chute désignée doit exister : la suppression logique générique ne
                // signale pas une entité absente.
                if (scrap is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La chute désignée est introuvable ; identifiant introuvable : {idCuttingScrapStock}.");

                // C2 - La chute ne doit pas être déjà retirée : un retrait répété trahit une erreur
                // d'orchestration. La réservation est citée pour orienter le diagnostic.
                if (scrap.IsDeleted)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"La chute {scrap.Id} est déjà retirée du stock (supprimée logiquement) ; réservation existante : {DescribeReservation(scrap.ReservedFor)}.");

                // D - Délégation de la suppression logique par identifiant ; IsDeleted, UpdatedAt et
                // événement Event Store relèvent exclusivement du Command Handler générique. Aucun
                // champ de la chute n'est affecté par le service.
                await _commandHandler.HandleSoftDeleteAsync(callChain, idCuttingScrapStock, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Produit la représentation, destinée au message d'échec, de la réservation portée par une
        /// chute.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée lors du rejet d'une chute déjà retirée du stock. Une réservation nulle
        /// ou vide est signalée comme absente ; toute autre valeur est citée telle quelle, entre
        /// guillemets, afin que le diagnostic puisse être orienté vers la série concernée.
        /// </para>
        /// </remarks>
        /// <param name="reservedFor">Valeur du champ de réservation de la chute ; peut être <see langword="null"/>.</param>
        /// <returns>Libellé de la réservation, prêt à être cité dans le message d'échec.</returns>
        private static string DescribeReservation(string? reservedFor)
        {
            return string.IsNullOrEmpty(reservedFor)
                ? "aucune"
                : $"« {reservedFor} »";
        }

        #endregion
    }
}