using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de la réservation, au profit d'une série de production, d'une
    /// chute du stock retenue par le moteur d'optimisation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_CuttingScrapStock_Reserve"/>, par le UseCase orchestrateur de l'optimisation
    /// de barre, pour une barre de production issue d'une chute exclusivement. Il consomme
    /// directement <see cref="IQ_Generic{T}"/> pour la lecture préalable de la chute et
    /// <see cref="IC_Generic{T}"/> pour sa mise à jour, sur l'entité
    /// <see cref="CuttingScrapStock"/>.
    /// </para>
    /// <para>
    /// Objectif : l'application consomme en priorité les chutes réutilisables avant de mobiliser
    /// une barre neuve. Le stock de chutes étant partagé par toutes les séries de production, une
    /// chute retenue pour recevoir des découpes doit cesser d'être candidate aux optimisations
    /// suivantes ; sans cela, deux séries pourraient prétendre à la même matière physique. Le
    /// service inscrit l'identifiant de la série consommatrice dans le champ <c>ReservedFor</c> de
    /// la chute, champ que le prédicat de disponibilité exclut : une chute réservée n'est plus
    /// proposée à l'optimisation. Il délègue ensuite la mise à jour au Command Handler générique,
    /// sans exposer la logique de persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Prédicat de disponibilité : une chute est réservable si elle n'est pas supprimée
    /// logiquement, n'est pas en attente d'intégration et ne porte aucune réservation. Les trois
    /// contrôles d'état du service reprennent exactement ce prédicat ; leur violation signale que
    /// l'état de la chute a changé entre la lecture du stock et la réservation, ou que la chute
    /// désignée n'était pas candidate. Une réservation nulle ou vide est tenue pour libre ; une
    /// réservation composée de blancs ne l'est pas. Toute réservation existante est refusée, y
    /// compris celle qui porterait déjà la série appelante, car elle trahirait une double
    /// réservation anormale : aucune réservation n'est écrasée.
    /// </para>
    /// <para>
    /// La réservation n'est pas une consommation : la chute demeure en stock et son devenir suit
    /// celui de la barre qui la porte. La réservation conserve la trace de la série consommatrice
    /// et n'est libérée qu'à la clôture de la série.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier les préconditions structurelles des arguments.</description></item>
    /// <item><description>Charger la chute désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/>.</description></item>
    /// <item><description>Vérifier l'existence de la chute et la compatibilité de son état avec la réservation.</description></item>
    /// <item><description>Inscrire l'identifiant de la série consommatrice dans le champ <c>ReservedFor</c> de la chute.</description></item>
    /// <item><description>Déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne consomme pas la chute et ne libère aucune réservation : la suppression logique relève de la validation de la barre, la libération relève de la clôture de la série.</description></item>
    /// <item><description>Ne vérifie ni l'existence de la série ni la cohérence d'article : le choix de la chute est repris tel que déterminé par le moteur d'optimisation.</description></item>
    /// <item><description>Ne porte aucune logique de branchement sur l'origine de la barre : l'appelant ne l'invoque que pour une barre issue d'une chute.</description></item>
    /// <item><description>Ne modifie aucun autre champ que <c>ReservedFor</c> ; <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_CuttingScrapStock_Reserve"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_CuttingScrapStock_Reserve : IS_CuttingScrapStock_Reserve
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
        /// Command Handler générique auquel est déléguée la mise à jour de la chute réservée.
        /// </summary>
        private readonly IC_Generic<CuttingScrapStock> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_CuttingScrapStock_Reserve"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; la chute lue reste ainsi suivie par le contexte qui
        /// l'enregistrera.
        /// </para>
        /// </remarks>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture suivie de la chute. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la mise à jour de la chute. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="queryHandler"/>, <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_CuttingScrapStock_Reserve(
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
        /// Réserve la chute désignée au profit de la série de production désignée, en inscrivant
        /// l'identifiant de la série sur la chute, puis confie sa mise à jour au Command Handler
        /// générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, pour une barre de production issue d'une chute exclusivement. La chute est lue
        /// avec suivi des changements ; la même instance est contrôlée, modifiée puis transmise au
        /// Command Handler générique, qui positionne la date de mise à jour et inscrit un événement
        /// technique. L'enregistrement effectif n'intervient qu'à la validation de la transaction par
        /// l'appelant ; un échec survenant avant la modification laisse la chute intacte.
        /// </para>
        /// <para>
        /// Objectif : retirer des recherches de disponibilité la chute retenue par l'optimisation, en
        /// la rattachant à la série qui la consommera, afin qu'aucune autre série ne puisse y
        /// prétendre.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier, dans l'ordre : identifiant de chute strictement positif, identifiant de série strictement positif.</description></item>
        /// <item><description>Charger la chute désignée en lecture suivie.</description></item>
        /// <item><description>Vérifier que la chute a été trouvée.</description></item>
        /// <item><description>Vérifier l'état de la chute et rejeter en un échec unique l'ensemble des conditions violées, dans l'ordre : supprimée logiquement, en attente d'intégration, déjà réservée.</description></item>
        /// <item><description>Positionner <c>ReservedFor</c> à la représentation textuelle de l'identifiant de série, indépendante de la culture pour un entier strictement positif.</description></item>
        /// <item><description>Transmettre au Command Handler générique l'instance chargée et modifiée.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ : suppression logique, attente d'intégration, inventaire, longueur, code-barres, article, date de création et date de mise à jour restent en dehors de la réservation.</description></item>
        /// <item><description>N'écrase jamais une réservation existante.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne vérifie pas l'existence de la série.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idCuttingScrapStock">Identifiant de la chute retenue par le moteur d'optimisation. Doit être strictement positif.</param>
        /// <param name="idProductionSeries">Identifiant de la série de production consommatrice. Doit être strictement positif ; son existence n'est pas vérifiée.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idCuttingScrapStock"/> ou
        /// <paramref name="idProductionSeries"/> n'est pas strictement positif (paramètre nommé et
        /// valeur reçue citée) ; avec le code <c>BU_ER_03</c> si la chute désignée est introuvable
        /// (identifiant cité) ; avec le code <c>BU_ER_04</c>, en un échec unique, si la chute est
        /// supprimée logiquement, en attente d'intégration ou déjà réservée (chute, série visée et
        /// chaque condition violée citées, réservation existante comprise). Remonte également sans
        /// interception toute <see cref="Ex_Business"/> levée par le Query Handler ou le Command
        /// Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la chute ou la délégation de sa mise à jour échoue techniquement.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task ExecuteAsync(
            string caller,
            int idCuttingScrapStock,
            int idProductionSeries,
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

                // P2 - Identifiant de série strictement positif.
                if (idProductionSeries <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production (idProductionSeries) doit être strictement positif ; valeur reçue : {idProductionSeries}.");

                ct.ThrowIfCancellationRequested();

                // L - Lecture SUIVIE : l'instance chargée est celle que le contexte partagé
                // enregistrera. Aucune variante AsNoTracking.
                CuttingScrapStock? scrap = await _queryHandler.HandleGetByIdAsync(
                    callChain,
                    idCuttingScrapStock,
                    ct);

                // C1 - La chute désignée doit exister.
                if (scrap is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"La chute désignée est introuvable, signe d'une incohérence entre le calcul d'optimisation et l'état persistant ; identifiant introuvable : {idCuttingScrapStock}.");

                // C2 - État compatible avec la réservation (prédicat de disponibilité), toutes les
                // conditions violées étant citées en un échec unique.
                string? violation = DescribeStateViolation(scrap);
                if (violation is not null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"L'état de la chute {scrap.Id} ne permet pas sa réservation pour la série de production {idProductionSeries} : {violation}.");

                // M - Écriture unique : seul ReservedFor est modifié. Pour un entier strictement
                // positif, la représentation textuelle est indépendante de la culture.
                scrap.ReservedFor = idProductionSeries.ToString();

                // D - Délégation de la même instance que celle chargée ; UpdatedAt et événement
                // Event Store relèvent du Command Handler générique.
                await _commandHandler.HandleUpdateAsync(callChain, scrap, ct);
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
        /// Décrit les conditions d'état qui interdisent la réservation d'une chute, ou indique
        /// qu'aucune ne s'applique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée sur la chute chargée, avant toute modification. Les conditions
        /// contrôlées reprennent exactement le prédicat de disponibilité du stock : une chute est
        /// réservable si elle n'est pas supprimée logiquement, n'est pas en attente d'intégration et
        /// ne porte aucune réservation. Une réservation nulle ou vide est tenue pour libre ; une
        /// réservation composée de blancs ne l'est pas. Toutes les conditions violées sont citées
        /// ensemble, dans cet ordre, afin que l'échec renseigne complètement l'appelant.
        /// </para>
        /// </remarks>
        /// <param name="scrap">Chute chargée à contrôler. Ne doit pas être <see langword="null"/>.</param>
        /// <returns>
        /// Énumération des conditions violées, prête à être citée dans le message d'échec ;
        /// <see langword="null"/> si l'état de la chute permet sa réservation.
        /// </returns>
        private static string? DescribeStateViolation(CuttingScrapStock scrap)
        {
            List<string> conditions = new();

            if (scrap.IsDeleted)
                conditions.Add("supprimée logiquement");

            if (scrap.WaitForIntegration)
                conditions.Add("en attente d'intégration");

            if (!string.IsNullOrEmpty(scrap.ReservedFor))
                conditions.Add($"déjà réservée (réservation existante : « {scrap.ReservedFor} »)");

            return conditions.Count == 0
                ? null
                : string.Join(", ", conditions);
        }

        #endregion
    }
}