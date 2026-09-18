using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de l'inscription, sur une série de production, de son premier
    /// approvisionnement en barres neuves ou en barres de chute.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionSeries_SetSupplyFlag"/>, par le UseCase orchestrateur de la validation
    /// d'une barre de production, à chaque validation et à l'intérieur de la transaction ouverte par
    /// ce dernier. Il consomme directement <see cref="IQ_Generic{T}"/> pour la lecture de la série
    /// <see cref="ProductionSeries"/> et <see cref="IC_Generic{T}"/> pour sa mise à jour.
    /// </para>
    /// <para>
    /// Objectif : l'atelier est approvisionné à la demande et une série est considérée comme en cours
    /// dès son premier engagement de matière, que constitue l'acceptation d'une barre par l'opérateur.
    /// Le service inscrit ce fait sur la série en posant l'indicateur d'approvisionnement
    /// correspondant à l'origine de la barre acceptée : <c>IsNewBarSupplied</c> pour une barre neuve,
    /// <c>IsDropBarSupplied</c> pour une barre de chute prélevée au stock. Le tableau de bord des
    /// séries lit ces indicateurs pour faire apparaître la série comme en cours.
    /// </para>
    /// <para>
    /// Règles d'inscription : un seul indicateur est visé par appel et il n'est écrit que s'il passe
    /// de <see langword="false"/> à <see langword="true"/>. Dès la deuxième barre d'une même origine,
    /// l'indicateur est déjà posé et l'appel s'achève sans écriture, sans date de mise à jour ni
    /// événement technique superflus. Les deux indicateurs sont indépendants et cumulatifs, et aucun
    /// n'est jamais remis à <see langword="false"/> : l'approvisionnement est un fait acquis, qu'un
    /// refus ultérieur de barre ne défait pas.
    /// </para>
    /// <para>
    /// L'inscription est portée par un service, et non par un UseCase, afin d'être exécutée dans la
    /// transaction de l'appelant : un UseCase ouvre sa propre transaction et ne peut être invoqué à
    /// l'intérieur de celle d'un autre UseCase. L'appelant conserve ainsi un enregistrement unique de
    /// l'ensemble de ses modifications, barre et série comprises.
    /// </para>
    /// <para>
    /// Évaluation sur l'état courant : la série est lue avec suivi des changements ; si elle est déjà
    /// suivie par le contexte partagé, la résolution d'identité restitue cette instance avec ses
    /// valeurs courantes, et l'état de l'indicateur visé est évalué sur ces valeurs, modifications non
    /// enregistrées comprises. L'origine de la barre est reçue de l'appelant : le service ne lit ni ne
    /// modifie aucune barre de production.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle de l'argument identifiant.</description></item>
    /// <item><description>Charger la série désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/> et vérifier son existence et son état.</description></item>
    /// <item><description>Poser l'indicateur d'approvisionnement correspondant à l'origine reçue et déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>, uniquement s'il n'était pas déjà posé.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne détermine pas l'origine de la barre et ne lit ni ne modifie aucune barre de production.</description></item>
    /// <item><description>Ne modifie aucun autre champ de la série que l'indicateur visé ; <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>N'écrit jamais <see langword="false"/> sur un indicateur d'approvisionnement.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionSeries_SetSupplyFlag"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionSeries_SetSupplyFlag : IS_ProductionSeries_SetSupplyFlag
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
        /// Initialise une nouvelle instance de <see cref="SR_ProductionSeries_SetSupplyFlag"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; la série lue est ainsi l'instance suivie par le
        /// contexte qui l'enregistrera, modifications de l'appelant comprises.
        /// </para>
        /// </remarks>
        /// <param name="querySeries">Query Handler générique consommé pour la lecture suivie de la série. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandSeries">Command Handler générique consommé pour la mise à jour de la série. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="querySeries"/>, <paramref name="commandSeries"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionSeries_SetSupplyFlag(
            IQ_Generic<ProductionSeries> querySeries,
            IC_Generic<ProductionSeries> commandSeries,
            IS_ExClassifier classifier)
        {
            _querySeries = querySeries ?? throw new ArgumentNullException(nameof(querySeries));
            _commandSeries = commandSeries ?? throw new ArgumentNullException(nameof(commandSeries));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Inscrit sur la série de production désignée son premier approvisionnement, en barres
        /// neuves ou en barres de chute, puis confie sa mise à jour au Command Handler générique si
        /// l'indicateur correspondant n'était pas déjà posé.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur à chaque validation de barre, à l'intérieur
        /// de la transaction qu'il a ouverte. La série est lue avec suivi des changements, puis
        /// contrôlée dans l'ordre : existence, absence de suppression logique, absence de clôture. Ces
        /// contrôles s'exécutent à chaque appel, y compris lorsque l'indicateur visé est déjà posé :
        /// une série supprimée ou clôturée est rejetée même si aucune écriture ne devait suivre. Un
        /// échec survenant avant la modification laisse la série intacte.
        /// </para>
        /// <para>
        /// Écriture conditionnelle : l'indicateur visé est <c>IsNewBarSupplied</c> si
        /// <paramref name="isNewBar"/> vaut <see langword="true"/>, <c>IsDropBarSupplied</c> sinon.
        /// S'il est déjà posé sur l'état courant de la série, la méthode s'achève sans écriture ni
        /// exception, afin de ne produire ni date de mise à jour ni événement technique sans
        /// changement réel. Dans le cas contraire, il est positionné à <see langword="true"/> et
        /// l'instance chargée est transmise au Command Handler générique ; elle reste suivie dans le
        /// contexte partagé et n'est enregistrée qu'à la validation de la transaction par l'appelant.
        /// Aucune branche n'écrit <see langword="false"/>.
        /// </para>
        /// <para>
        /// Objectif : faire apparaître la série comme en cours dès son premier engagement de matière,
        /// quelle que soit l'origine de la barre engagée.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de série est strictement positif.</description></item>
        /// <item><description>Charger la série désignée en lecture suivie, puis vérifier dans l'ordre qu'elle a été trouvée, qu'elle n'est pas supprimée logiquement et qu'elle n'est pas clôturée.</description></item>
        /// <item><description>Sélectionner l'indicateur visé selon l'origine reçue et, s'il est déjà posé, s'arrêter sans écriture.</description></item>
        /// <item><description>À défaut, positionner l'indicateur visé à <see langword="true"/> et transmettre au Command Handler générique l'instance chargée et modifiée.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ de la série, en particulier ni l'autre indicateur d'approvisionnement, ni les indicateurs de début de découpe, de rupture de stock et de fin de découpe, ni les dates de production, ni les champs d'audit.</description></item>
        /// <item><description>Ne restitue aucune indication de changement et n'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionSeries">Identifiant de la série de production dont le premier approvisionnement est inscrit. Doit être strictement positif.</param>
        /// <param name="isNewBar">Origine de la barre acceptée : <see langword="true"/> pour une barre neuve, <see langword="false"/> pour une barre de chute.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionSeries"/> n'est pas
        /// strictement positif (paramètre nommé et valeur reçue cités) ; avec le code <c>BU_ER_03</c>
        /// si la série désignée est introuvable (identifiant cité) ; avec le code <c>BU_ER_04</c> si la
        /// série désignée est supprimée logiquement ou clôturée (identifiant cité). Remonte également
        /// sans interception toute <see cref="Ex_Business"/> levée par le Query Handler ou le Command
        /// Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la série ou la délégation de sa mise à jour échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idProductionSeries,
            bool isNewBar,
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
                        $"La série de production {idProductionSeries} est supprimée logiquement : son approvisionnement ne peut pas être inscrit sur une série supprimée.");

                // C3 - La série désignée ne doit pas être clôturée.
                if (series.IsCuttingCompleted)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"La série de production {idProductionSeries} est clôturée : une série clôturée ne peut plus être approvisionnée.");

                // C4 - Indicateur visé déjà posé sur l'état courant : aucune écriture, ni date de
                // mise à jour ni événement.
                bool isAlreadySupplied = isNewBar
                    ? series.IsNewBarSupplied
                    : series.IsDropBarSupplied;

                if (isAlreadySupplied)
                    return;

                // M - Écriture unique : seul l'indicateur visé est modifié, et uniquement à true.
                if (isNewBar)
                    series.IsNewBarSupplied = true;
                else
                    series.IsDropBarSupplied = true;

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