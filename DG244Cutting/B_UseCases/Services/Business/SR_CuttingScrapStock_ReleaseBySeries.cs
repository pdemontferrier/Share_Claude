using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de la libération, à la clôture d'une série de production, de
    /// l'ensemble des chutes du stock que cette série avait réservées sans les consommer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_CuttingScrapStock_ReleaseBySeries"/>, par le UseCase orchestrateur de clôture
    /// de la série de production, après le marquage de la série achevée et à l'intérieur de la
    /// transaction de ce UseCase. Il consomme directement <see cref="IQ_Generic{T}"/> pour la
    /// lecture suivie des chutes réservées et <see cref="IC_Generic{T}"/> pour leur mise à jour,
    /// sur l'entité <see cref="CuttingScrapStock"/>.
    /// </para>
    /// <para>
    /// Objectif : l'application consomme en priorité les chutes réutilisables avant de mobiliser
    /// une barre neuve. Le stock de chutes étant partagé par toutes les séries de production, une
    /// chute retenue par une optimisation est réservée au profit de la série consommatrice : elle
    /// demeure physiquement en stock mais n'est plus proposée à l'optimisation. Une chute réservée
    /// que la série n'a finalement pas consommée resterait ainsi invisible au stock indéfiniment.
    /// La clôture de la série étant le seul moment où il est certain que la série ne consommera
    /// plus rien, le service rend à cet instant, en une seule opération, toutes ces chutes au
    /// stock disponible, afin qu'elles redeviennent candidates aux optimisations des séries
    /// suivantes. Il délègue la mise à jour au Command Handler générique, sans exposer la logique
    /// de persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// Symétrie avec la réservation : la réservation inscrit dans le champ <c>ReservedFor</c> la
    /// représentation textuelle de l'identifiant de la série consommatrice. La libération
    /// sélectionne les chutes par égalité stricte avec cette même représentation, produite par la
    /// même conversion ; une réservation d'un autre format, composée de blancs notamment, n'est
    /// pas sélectionnée. Seules les chutes non supprimées logiquement sont concernées, y compris
    /// celles en attente d'intégration : les chutes consommées ou écartées conservent leur
    /// réservation, seule trace de la série qui les a mobilisées.
    /// </para>
    /// <para>
    /// L'information de réservation est tenue pour transitoire : la libération efface, sur chaque
    /// chute rendue au stock, l'indication de la série qui l'avait retenue, et cette perte est
    /// assumée.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle de l'argument.</description></item>
    /// <item><description>Charger en lecture suivie, via <see cref="IQ_Generic{T}.HandleGetFilteredAsync"/>, les chutes non supprimées réservées par la série.</description></item>
    /// <item><description>Remettre à <see langword="null"/> le champ <c>ReservedFor</c> de chaque chute chargée.</description></item>
    /// <item><description>Déléguer en un appel unique la mise à jour des chutes libérées au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateRangeAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne consomme aucune chute et n'en supprime aucune logiquement : la libération retourne les chutes au stock, elle ne les en fait jamais sortir.</description></item>
    /// <item><description>Ne vérifie ni l'existence ni l'état de la série.</description></item>
    /// <item><description>Ne modifie aucun autre champ que <c>ReservedFor</c> ; <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : la clôture est journalisée une seule fois, sur la série.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_CuttingScrapStock_ReleaseBySeries"/>
    /// <seealso cref="IS_CuttingScrapStock_Reserve"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_CuttingScrapStock_ReleaseBySeries : IS_CuttingScrapStock_ReleaseBySeries
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
        /// Query Handler générique auquel est déléguée la lecture suivie des chutes réservées par
        /// la série.
        /// </summary>
        private readonly IQ_Generic<CuttingScrapStock> _queryHandler;

        /// <summary>
        /// Command Handler générique auquel est déléguée la mise à jour des chutes libérées.
        /// </summary>
        private readonly IC_Generic<CuttingScrapStock> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_CuttingScrapStock_ReleaseBySeries"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; les chutes lues restent ainsi suivies par le contexte
        /// qui les enregistrera.
        /// </para>
        /// </remarks>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture suivie des chutes. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la mise à jour des chutes. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="queryHandler"/>, <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_CuttingScrapStock_ReleaseBySeries(
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
        /// Rend au stock disponible, en une seule opération, toutes les chutes que la série de
        /// production désignée avait réservées sans les consommer, puis confie leur mise à jour au
        /// Command Handler générique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de clôture de la série, à l'intérieur de
        /// la transaction qu'il a ouverte et après le marquage de la série achevée. Les chutes
        /// concernées sont lues avec suivi des changements ; la liste chargée est modifiée puis
        /// transmise telle quelle au Command Handler générique, qui positionne la date de mise à
        /// jour et inscrit un événement technique par chute. L'enregistrement effectif n'intervient
        /// qu'à la validation de la transaction par l'appelant ; un échec survenant avant la
        /// délégation laisse les chutes intactes du point de vue persistant.
        /// </para>
        /// <para>
        /// Objectif : réintégrer dans les recherches de disponibilité les chutes physiquement
        /// présentes en atelier que la série close ne consommera plus.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de série est strictement positif.</description></item>
        /// <item><description>Calculer la référence de réservation par la même conversion textuelle que celle appliquée à la réservation, hors de l'expression de filtrage afin qu'elle soit transmise comme paramètre à la requête.</description></item>
        /// <item><description>Charger en lecture suivie les chutes dont la réservation est égale à cette référence et qui ne sont pas supprimées logiquement, sans autre critère.</description></item>
        /// <item><description>Terminer sans écriture, sans date de mise à jour ni événement, lorsque aucune chute n'est sélectionnée.</description></item>
        /// <item><description>Remettre à <see langword="null"/> le champ <c>ReservedFor</c> de chaque chute chargée.</description></item>
        /// <item><description>Transmettre au Command Handler générique, en un appel unique, la liste chargée elle-même.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ : suppression logique, attente d'intégration, inventaire, longueur, code-barres, article, emplacement, date de création et date de mise à jour restent en dehors de la libération.</description></item>
        /// <item><description>Ne contrôle ni l'état des chutes chargées, garanti par le filtre de lecture, ni l'existence de la série.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne restitue pas le nombre de chutes libérées.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionSeries">Identifiant de la série de production close. Doit être strictement positif ; son existence et son état ne sont pas vérifiés.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionSeries"/> n'est pas
        /// strictement positif (paramètre nommé et valeur reçue cités). L'absence de chute réservée
        /// par la série n'est pas un échec. Remonte également sans interception toute
        /// <see cref="Ex_Business"/> levée par le Query Handler ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture des chutes ou la délégation de leur mise à jour échoue techniquement.</exception>
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

                // Référence de réservation produite par la conversion strictement identique à celle
                // de la réservation, calculée HORS du prédicat pour être capturée et paramétrée.
                string reference = idProductionSeries.ToString();

                // L - Lecture SUIVIE et UNIQUE : les instances chargées sont celles que le contexte
                // partagé enregistrera. Le filtre de suppression logique est porté explicitement ;
                // aucun autre critère ne s'ajoute. Ni variante AsNoTracking, ni lecture unitaire.
                List<CuttingScrapStock> scraps = await _queryHandler.HandleGetFilteredAsync(
                    callChain,
                    s => s.ReservedFor == reference && !s.IsDeleted,
                    ct);

                // C - Sélection vide : cas nominal, aucune écriture, ni date de mise à jour ni événement.
                if (scraps.Count == 0)
                    return;

                // M - Écriture unique : seul ReservedFor est modifié, sur chaque chute chargée.
                foreach (CuttingScrapStock scrap in scraps)
                    scrap.ReservedFor = null;

                // D - Délégation de la liste chargée elle-même, en un appel unique ; UpdatedAt et
                // événements Event Store relèvent du Command Handler générique.
                await _commandHandler.HandleUpdateRangeAsync(callChain, scraps, ct);
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