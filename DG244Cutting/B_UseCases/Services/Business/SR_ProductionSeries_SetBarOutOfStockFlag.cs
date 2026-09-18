using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable du recalcul de l'indicateur de rupture de stock d'une série de
    /// production, à partir de l'état courant de ses barres.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionSeries_SetBarOutOfStockFlag"/>, par le UseCase orchestrateur de la
    /// déclaration de rupture de stock d'une barre de production et de sa libération, à l'intérieur
    /// de la transaction ouverte par ce dernier. Il consomme directement <see cref="IQ_Generic{T}"/>
    /// pour la lecture de la série <see cref="ProductionSeries"/> et de ses barres
    /// <see cref="ProductionBar"/>, et <see cref="IC_Generic{T}"/> pour la mise à jour de la série.
    /// </para>
    /// <para>
    /// Objectif : une barre indisponible, neuve comme de chute, est déclarée en rupture de stock et
    /// mise de côté jusqu'à ce que la matière redevienne disponible. Le tableau de bord des séries
    /// signale les séries comportant au moins une barre en rupture en lisant l'indicateur
    /// <c>IsBarOutOfStock</c> de la série. Le service est le seul écrivain de cet indicateur : il ne
    /// reçoit jamais la valeur à poser et la déduit de l'état des barres, ce qui garantit la justesse
    /// du signalement quel que soit l'enchaînement des déclarations et des libérations.
    /// </para>
    /// <para>
    /// Règle de calcul : l'indicateur vaut <see langword="true"/> si et seulement si la série
    /// comporte au moins une barre en rupture de stock non supprimée logiquement, sans distinction
    /// entre barre neuve et barre de chute ; une barre refusée, donc supprimée logiquement, est
    /// sortie du circuit. Le recalcul est symétrique : libérer une barre ne remet la série à
    /// <see langword="false"/> que si aucune autre barre n'est encore en rupture, et déclarer une
    /// rupture sur une série qui en comptait déjà une ne change rien.
    /// </para>
    /// <para>
    /// Le recalcul est porté par un service, et non par un UseCase, afin d'être exécuté dans la
    /// transaction de l'appelant : un UseCase ouvre sa propre transaction et ne peut être invoqué à
    /// l'intérieur de celle d'un autre UseCase. L'appelant conserve ainsi un enregistrement unique de
    /// l'ensemble de ses modifications, barres et série comprises.
    /// </para>
    /// <para>
    /// Évaluation en mémoire : l'appelant modifie les barres dans le contexte partagé sans les
    /// enregistrer, et le contexte n'enregistre pas les modifications suivies avant d'exécuter une
    /// requête ; une requête filtrant sur l'état de rupture ou de suppression logique lirait donc
    /// l'état enregistré antérieur. Le service charge en lecture suivie les barres de la série,
    /// filtrées en base sur le seul identifiant de série, que l'appelant ne modifie pas ; la
    /// résolution d'identité du contexte restitue les instances déjà suivies avec leurs valeurs
    /// courantes, et l'état de rupture et de suppression logique est évalué en mémoire. Les barres de
    /// la série qui n'étaient pas encore suivies le deviennent, à l'état inchangé, sans écriture.
    /// </para>
    /// <para>
    /// Conditions d'usage et limites : l'appel intervient après modification des barres sur leurs
    /// instances suivies. Une barre ajoutée au contexte sans avoir été enregistrée n'est pas
    /// restituée par la lecture et n'est donc pas prise en compte ; une barre en cours de suppression
    /// physique serait encore restituée. Ces deux situations sont étrangères à la déclaration et à la
    /// libération d'une rupture, le refus d'une barre passant par sa suppression logique, évaluée en
    /// mémoire.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle de l'argument.</description></item>
    /// <item><description>Charger la série désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/> et vérifier son existence et son état.</description></item>
    /// <item><description>Charger les barres de la série en lecture suivie via <see cref="IQ_Generic{T}.HandleGetFilteredAsync"/> et évaluer leur état en mémoire.</description></item>
    /// <item><description>Positionner <c>IsBarOutOfStock</c> et déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>, uniquement si la valeur a changé.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne déclare ni ne libère la rupture d'une barre et ne modifie aucune barre.</description></item>
    /// <item><description>Ne modifie aucun autre champ de la série que <c>IsBarOutOfStock</c> ; <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>N'interroge jamais la base sur l'état de rupture ou de suppression logique des barres.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionSeries_SetBarOutOfStockFlag"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionSeries_SetBarOutOfStockFlag : IS_ProductionSeries_SetBarOutOfStockFlag
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
        /// Query Handler générique auquel est déléguée la lecture suivie de la série désignée.
        /// </summary>
        private readonly IQ_Generic<ProductionSeries> _querySeries;

        /// <summary>
        /// Query Handler générique auquel est déléguée la lecture suivie des barres de la série.
        /// </summary>
        private readonly IQ_Generic<ProductionBar> _queryBar;

        /// <summary>
        /// Command Handler générique auquel est déléguée la mise à jour de la série.
        /// </summary>
        private readonly IC_Generic<ProductionSeries> _commandSeries;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionSeries_SetBarOutOfStockFlag"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers les
        /// Query Handlers et le Command Handler ; la série et les barres lues sont ainsi les instances
        /// suivies par le contexte qui les enregistrera, modifications de l'appelant comprises.
        /// </para>
        /// </remarks>
        /// <param name="querySeries">Query Handler générique consommé pour la lecture suivie de la série. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="queryBar">Query Handler générique consommé pour la lecture suivie des barres de la série. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandSeries">Command Handler générique consommé pour la mise à jour de la série. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="querySeries"/>, <paramref name="queryBar"/>, <paramref name="commandSeries"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionSeries_SetBarOutOfStockFlag(
            IQ_Generic<ProductionSeries> querySeries,
            IQ_Generic<ProductionBar> queryBar,
            IC_Generic<ProductionSeries> commandSeries,
            IS_ExClassifier classifier)
        {
            _querySeries = querySeries ?? throw new ArgumentNullException(nameof(querySeries));
            _queryBar = queryBar ?? throw new ArgumentNullException(nameof(queryBar));
            _commandSeries = commandSeries ?? throw new ArgumentNullException(nameof(commandSeries));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Recalcule l'indicateur de rupture de stock de la série de production désignée à partir de
        /// l'état courant de ses barres, puis confie sa mise à jour au Command Handler générique s'il
        /// a changé.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, après modification des barres concernées sur leurs instances suivies. La série est
        /// lue avec suivi des changements et contrôlée avant toute lecture de ses barres, afin de
        /// rejeter une série invalide sans charger celles-ci. Les barres sont ensuite lues avec suivi
        /// des changements sur le seul critère de l'identifiant de série, et leur état de rupture et
        /// de suppression logique est évalué en mémoire. Un échec survenant avant la modification
        /// laisse la série intacte.
        /// </para>
        /// <para>
        /// Écriture conditionnelle : la série n'est modifiée et transmise au Command Handler générique
        /// que si la valeur recalculée diffère de la valeur courante, afin de ne produire ni date de
        /// mise à jour ni événement technique sans changement réel. La symétrie est préservée : les
        /// deux sens de variation sont écrits dès qu'ils se produisent, et un second appel sur un état
        /// inchangé ne laisse aucune trace. Lorsqu'elle est transmise, la série reste suivie dans le
        /// contexte partagé et n'est enregistrée qu'à la validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Objectif : maintenir exact le signalement des séries comportant au moins une barre en
        /// rupture de stock.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de série est strictement positif.</description></item>
        /// <item><description>Charger la série désignée en lecture suivie, puis vérifier dans l'ordre qu'elle a été trouvée et qu'elle n'est pas supprimée logiquement.</description></item>
        /// <item><description>Charger en lecture suivie les barres dont l'identifiant de série correspond, sans autre critère de filtrage en base.</description></item>
        /// <item><description>Déterminer en mémoire si au moins une de ces barres est en rupture de stock sans être supprimée logiquement.</description></item>
        /// <item><description>Si la valeur obtenue diffère de la valeur courante, positionner <c>IsBarOutOfStock</c> et transmettre au Command Handler générique l'instance chargée et modifiée ; à défaut, s'arrêter sans écriture.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ de la série, en particulier ni les indicateurs d'approvisionnement en barres neuves ou de chutes, ni les indicateurs de début et de fin de découpe, ni les champs d'audit.</description></item>
        /// <item><description>Ne modifie aucune des barres restituées.</description></item>
        /// <item><description>Ne restitue aucune indication de changement et n'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionSeries">Identifiant de la série de production dont l'indicateur de rupture de stock est recalculé. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé aux Query Handlers et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionSeries"/> n'est pas
        /// strictement positif (paramètre nommé et valeur reçue cités) ; avec le code <c>BU_ER_03</c>
        /// si la série désignée est introuvable (identifiant cité) ; avec le code <c>BU_ER_04</c> si la
        /// série désignée est supprimée logiquement (identifiant cité). Remonte également sans
        /// interception toute <see cref="Ex_Business"/> levée par les Query Handlers ou le Command
        /// Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la série ou de ses barres, ou la délégation de la mise à jour de la série, échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idProductionSeries,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // P1 - Identifiant de série strictement positif.
                if (idProductionSeries <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production (idProductionSeries) doit être strictement positif ; valeur reçue : {idProductionSeries}.");

                ct.ThrowIfCancellationRequested();

                // L1 - Lecture SUIVIE de la série : l'instance chargée est celle que le contexte
                // partagé enregistrera. Aucune variante AsNoTracking.
                ProductionSeries? series = await _querySeries.HandleGetByIdAsync(
                    callChain,
                    idProductionSeries,
                    ct);

                // C1 - La série désignée doit exister.
                if (series is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La série de production désignée est introuvable ; identifiant introuvable : {idProductionSeries}.");

                // C2 - La série désignée ne doit pas être supprimée logiquement.
                if (series.IsDeleted)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"La série de production {idProductionSeries} est supprimée logiquement : son indicateur de rupture de stock ne peut pas être recalculé.");

                // L2 - Lecture SUIVIE des barres, filtrée en base sur l'identifiant de série SEUL,
                // colonne non modifiée par l'appelant. La résolution d'identité restitue les
                // instances déjà suivies avec leurs valeurs courantes. Aucune variante AsNoTracking.
                List<ProductionBar> bars = await _queryBar.HandleGetFilteredAsync(
                    callChain,
                    b => b.IdProductionSeries == idProductionSeries,
                    ct);

                // Évaluation EN MÉMOIRE de l'état de rupture et de suppression logique, modifications
                // suivies non enregistrées comprises.
                bool hasOutOfStock = bars.Any(b => b.IsOutOfStock && !b.IsDeleted);

                // C3 - Valeur inchangée : aucune écriture, ni date de mise à jour ni événement.
                if (series.IsBarOutOfStock == hasOutOfStock)
                    return;

                // M - Écriture unique : seul IsBarOutOfStock est modifié.
                series.IsBarOutOfStock = hasOutOfStock;

                // D - Délégation de la même instance que celle chargée ; UpdatedAt et événement
                // Event Store relèvent du Command Handler générique.
                await _commandSeries.HandleUpdateAsync(callChain, series, ct);
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

        // A compléter

        #endregion
    }
}