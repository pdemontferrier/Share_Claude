using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de la clôture des découpes d'une série de production, qui marque
    /// la série comme ayant achevé l'ensemble de ses découpes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside en
    /// <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé, via son interface
    /// <see cref="IS_ProductionSeries_CompleteCutting"/>, par le UseCase orchestrateur de la
    /// clôture d'une série de production, à l'intérieur de la transaction ouverte par ce dernier.
    /// Il consomme directement <see cref="IQ_Generic{T}"/> pour la lecture de la série
    /// <see cref="ProductionSeries"/>, et <see cref="IC_Generic{T}"/> pour sa mise à jour.
    /// </para>
    /// <para>
    /// Objectif : le parcours de découpe traite une série barre après barre jusqu'à ce qu'il ne
    /// reste plus rien à couper. Le service inscrit ce point d'arrivée en posant l'indicateur
    /// <c>IsCuttingCompleted</c> de la série. La série est alors classée parmi les séries terminées
    /// au tableau de bord, s'ouvre en consultation seule plutôt qu'en production, et n'est plus
    /// reprise par le parcours de découpe.
    /// </para>
    /// <para>
    /// Répartition des responsabilités : le service n'établit pas la condition de clôture. Le
    /// constat qu'aucune découpe de la série ne reste à couper est établi par l'appelant avant
    /// l'appel ; le service écrit sur la série sans interroger ni ses découpes ni ses barres. La
    /// journalisation de la clôture dans le cycle de vie de la série, la libération des chutes
    /// réservées pour elle et l'enregistrement de l'ensemble relèvent de l'appelant, dans la même
    /// transaction. Les autres indicateurs d'avancement de la série, notamment ceux
    /// d'approvisionnement et de rupture de stock, sont écrits par d'autres services et ne sont
    /// jamais touchés ici.
    /// </para>
    /// <para>
    /// Caractère définitif : le service ne pose jamais que la valeur <see langword="true"/> et
    /// n'offre aucun retour arrière. Il rejette la clôture d'une série déjà clôturée plutôt que de
    /// s'arrêter sans écriture : un second appel traduit une erreur d'orchestration et conduirait
    /// l'appelant à journaliser une seconde fois la même clôture.
    /// </para>
    /// <para>
    /// La clôture est portée par un service, et non par un UseCase, afin d'être exécutée dans la
    /// transaction de l'appelant : un UseCase ouvre sa propre transaction et ne peut être invoqué à
    /// l'intérieur de celle d'un autre UseCase. L'appelant conserve ainsi un enregistrement unique
    /// de l'ensemble de ses modifications, série comprise.
    /// </para>
    /// <para>
    /// Conditions d'usage : l'appel intervient à l'intérieur de la transaction ouverte par
    /// l'appelant, après que celui-ci a établi l'absence de découpe restante. Ces deux conditions
    /// ne sont pas contrôlées par le service.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la précondition structurelle de l'argument.</description></item>
    /// <item><description>Charger la série désignée en lecture suivie via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/> et vérifier son existence, son état de suppression logique et l'absence de clôture antérieure.</description></item>
    /// <item><description>Positionner <c>IsCuttingCompleted</c> à <see langword="true"/> et déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>N'établit pas la condition de clôture et ne lit ni les découpes ni les barres de la série.</description></item>
    /// <item><description>Ne contrôle pas l'engagement préalable de la série : ni <c>IsCuttingStarted</c>, ni <c>IsDropBarSupplied</c>, ni <c>IsNewBarSupplied</c> ne sont testés.</description></item>
    /// <item><description>Ne modifie aucun autre champ de la série que <c>IsCuttingCompleted</c> ; <c>UpdatedAt</c> est positionné par le Command Handler générique.</description></item>
    /// <item><description>Ne libère aucune chute réservée pour la série.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> et n'appelle jamais directement un Repository.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionSeries_CompleteCutting"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionSeries_CompleteCutting : IS_ProductionSeries_CompleteCutting
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
        /// Initialise une nouvelle instance de <see cref="SR_ProductionSeries_CompleteCutting"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la portée de
        /// l'invocation, afin de partager le contexte de données du UseCase orchestrateur à travers le
        /// Query Handler et le Command Handler ; la série lue est ainsi l'instance suivie par le
        /// contexte qui l'enregistrera.
        /// </para>
        /// </remarks>
        /// <param name="querySeries">Query Handler générique consommé pour la lecture suivie de la série. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="commandSeries">Command Handler générique consommé pour la mise à jour de la série. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="querySeries"/>, <paramref name="commandSeries"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionSeries_CompleteCutting(
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
        /// Marque comme achevées les découpes de la série de production désignée en posant son
        /// indicateur de clôture, puis confie sa mise à jour au Command Handler générique, sans
        /// persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par un UseCase orchestrateur, à l'intérieur de la transaction qu'il a
        /// ouverte, après qu'il a établi qu'aucune découpe de la série ne reste à couper. La série est
        /// lue avec suivi des changements et contrôlée avant toute modification. Un échec survenant
        /// avant la modification laisse la série intacte.
        /// </para>
        /// <para>
        /// Écriture inconditionnelle : une fois les contrôles passés, la série est toujours modifiée
        /// et transmise au Command Handler générique. Le rejet d'une série déjà clôturée garantit que
        /// cette écriture correspond à un changement réel. La série transmise reste suivie dans le
        /// contexte partagé et n'est enregistrée qu'à la validation de la transaction par l'appelant.
        /// </para>
        /// <para>
        /// Ordre des contrôles : la suppression logique est évaluée avant la clôture antérieure ; une
        /// série à la fois supprimée logiquement et clôturée est rejetée au motif de sa suppression.
        /// </para>
        /// <para>
        /// Objectif : inscrire sur la série le point d'arrivée de son parcours de découpe.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier que l'identifiant de série est strictement positif.</description></item>
        /// <item><description>Charger la série désignée en lecture suivie, puis vérifier dans l'ordre qu'elle a été trouvée, qu'elle n'est pas supprimée logiquement et que ses découpes ne sont pas déjà marquées comme achevées.</description></item>
        /// <item><description>Positionner <c>IsCuttingCompleted</c> à <see langword="true"/> et transmettre au Command Handler générique l'instance chargée et modifiée.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne modifie aucun autre champ de la série, en particulier ni les indicateurs d'approvisionnement, d'optimisation, de validation, de rupture de stock ou de début de découpe, ni les dates de planification, ni la description et le numéro de série, ni les champs d'origine et d'audit.</description></item>
        /// <item><description>Ne vérifie ni l'absence de découpe restante ni l'engagement préalable de la série.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionSeries">Identifiant de la série de production dont les découpes sont marquées comme achevées. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Query Handler et au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_02</c> si <paramref name="idProductionSeries"/> n'est pas
        /// strictement positif (paramètre nommé et valeur reçue cités) ; avec le code <c>BU_ER_03</c>
        /// si la série désignée est introuvable (identifiant cité) ; avec le code <c>BU_ER_04</c> si la
        /// série désignée est supprimée logiquement ou si ses découpes sont déjà marquées comme
        /// achevées (identifiant et motif cités). Remonte également sans interception toute
        /// <see cref="Ex_Business"/> levée par le Query Handler ou le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si la lecture de la série, ou la délégation de sa mise à jour, échoue techniquement.</exception>
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
                        $"La série de production {idProductionSeries} est supprimée logiquement : ses découpes ne peuvent pas être marquées comme achevées.");

                // C3 - La série désignée ne doit pas être déjà clôturée : un second appel est une
                // erreur d'orchestration, rejetée plutôt qu'ignorée.
                if (series.IsCuttingCompleted)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        $"Les découpes de la série de production {idProductionSeries} sont déjà marquées comme achevées : la clôture ne peut pas être inscrite une seconde fois.");

                // M - Écriture unique et définitive : seul IsCuttingCompleted est modifié.
                series.IsCuttingCompleted = true;

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