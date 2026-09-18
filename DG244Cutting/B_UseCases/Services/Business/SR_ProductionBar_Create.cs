using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;

namespace DG244Cutting.B_UseCases.Services.Business
{
    /// <summary>
    /// Service métier responsable de la création d'une barre de production provisoire à
    /// partir du résultat réussi d'une optimisation de découpe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (<c>B_UseCases</c>) et réside
    /// en <c>B_UseCases/Services/Business</c>. Il est résolu par injection de dépendances et ne
    /// doit jamais être instancié directement. Il est consommé par le UseCase orchestrateur de
    /// l'optimisation de barre via son interface <see cref="IS_ProductionBar_Create"/>, et
    /// consomme directement <see cref="IC_Generic{T}"/> pour l'entité
    /// <see cref="ProductionBar"/>.
    /// </para>
    /// <para>
    /// Objectif : transformer la décision du moteur d'optimisation (matière à mobiliser et
    /// découpes à y réaliser) en une barre de production provisoire que l'opérateur consulte,
    /// puis valide, refuse ou déclare en rupture de stock. Le service est le point unique de
    /// correspondance entre le vocabulaire du résultat de calcul
    /// (<see cref="DTO_CuttingOptimizationResult"/>) et celui du modèle de données ; il
    /// construit l'entité puis en délègue la mutation au Command Handler générique, sans
    /// exposer la logique de persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>
    /// La barre créée porte le marqueur de placement provisoire (<c>IsOptimizedTemp</c>) :
    /// elle n'est ni validée ni scellée, et son plan de coupe peut encore être défait par un
    /// refus ou recomposé par une validation avec défauts. Ce marqueur n'est jamais retiré au
    /// cours de la vie de la barre ; ce sont les états de validation, d'utilisation, de
    /// rupture de stock ou de suppression logique qui la font ensuite progresser, puis sortir
    /// du circuit.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Vérifier les préconditions structurelles des arguments et la cohérence du résultat d'optimisation reçu.</description></item>
    /// <item><description>Construire l'entité <see cref="ProductionBar"/> provisoire à partir du résultat, de la série et de l'article interne.</description></item>
    /// <item><description>Déléguer l'écriture au Command Handler générique via <see cref="IC_Generic{T}.HandleAddAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à l'appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction et n'appelle jamais <c>SaveChangesAsync</c> : ces rôles appartiennent au UseCase orchestrateur.</description></item>
    /// <item><description>Ne porte aucune règle de calcul : le choix des découpes, la longueur physique du résidu et sa qualification sont repris tels que déterminés par le moteur d'optimisation.</description></item>
    /// <item><description>Ne rattache pas les découpes, ne réserve pas la chute source et ne positionne aucun indicateur d'approvisionnement de la série.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> : toute valeur nécessaire lui est fournie par argument depuis le UseCase.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Command Handler.</description></item>
    /// <item><description>Ne journalise ni ne notifie, et n'inscrit aucune action de cycle de vie : la traçabilité métier de la barre commence à sa validation.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_ProductionBar_Create"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_ProductionBar_Create : IS_ProductionBar_Create
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
        /// Command Handler générique auquel est déléguée l'écriture de la barre de production.
        /// </summary>
        private readonly IC_Generic<ProductionBar> _commandHandler;

        /// <summary>
        /// Service de requalification des exceptions non prévues en exceptions typées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ProductionBar_Create"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instance résolue par le conteneur d'injection de dépendances dans la
        /// portée de l'invocation, afin de partager le contexte de données du UseCase
        /// orchestrateur à travers le Command Handler.
        /// </para>
        /// </remarks>
        /// <param name="commandHandler">Command Handler générique consommé pour l'écriture de la barre de production. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées. Ne doit pas être <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="commandHandler"/> ou <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_ProductionBar_Create(
            IC_Generic<ProductionBar> commandHandler,
            IS_ExClassifier classifier)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Construit la barre de production provisoire correspondant à un résultat
        /// d'optimisation réussi, pour une série et un article interne donnés, puis la
        /// confie au Command Handler générique sans la persister.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de l'optimisation de barre, à
        /// l'intérieur de la transaction qu'il a ouverte. Le Command Handler générique
        /// positionne la date de création, inscrit l'entité dans le suivi du contexte partagé
        /// et enregistre l'événement associé ; l'enregistrement effectif n'intervient qu'à la
        /// validation de la transaction par l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier, dans l'ordre : présence du résultat, validité des identifiants de série et d'article, issue de succès, présence puis non-vacuité de la liste des découpes, longueur de barre strictement positive, cohérence entre l'origine du contenant et sa chute source.</description></item>
        /// <item><description>Reporter sur la barre la série, l'article interne, l'origine du contenant, la chute source, la longueur de barre, le nombre de découpes (cardinal de la liste reçue) et le résidu physique, toujours renseigné y compris lorsqu'il vaut zéro.</description></item>
        /// <item><description>Recopier sans transformation la qualification du résidu : <see langword="true"/> signifie chute réutilisable, <see langword="false"/> signifie déchet.</description></item>
        /// <item><description>Marquer la barre comme placement provisoire, seul indicateur d'état positionné.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique et retourner l'instance qui lui a été transmise.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne contrôle ni ne recalcule la longueur ni la qualification du résidu.</description></item>
        /// <item><description>Laisse à leur valeur par défaut les défauts, le reliquat final validé, l'emplacement et le code-barre de chute, la valorisation du reste, le motif de refus, les autres états de cycle de vie et les champs d'audit.</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> et ne relit pas l'entité après écriture.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="result">
        /// Résultat du moteur d'optimisation de découpe décrivant la barre à matérialiser. Ne
        /// doit pas être <see langword="null"/> ; son issue doit être un succès et sa liste
        /// d'identifiants de découpes doit être renseignée et non vide.
        /// </param>
        /// <param name="idProductionSeries">Identifiant de la série de production de rattachement. Doit être strictement positif.</param>
        /// <param name="idArticleInternal">Identifiant de l'article interne dont la barre est constituée. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé au Command Handler. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Instance de <see cref="ProductionBar"/> transmise au Command Handler, jamais
        /// <see langword="null"/>, dont l'identifiant vaut <c>0</c> tant que l'appelant n'a pas
        /// déclenché l'enregistrement.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="result"/> ou sa liste
        /// d'identifiants de découpes est <see langword="null"/> ; avec le code <c>BU_ER_02</c>
        /// si <paramref name="idProductionSeries"/> ou <paramref name="idArticleInternal"/> n'est
        /// pas strictement positif, si la liste des découpes est vide ou si la longueur de barre
        /// n'est pas strictement positive ; avec le code <c>BU_ER_03</c> si l'issue du résultat
        /// n'est pas un succès, si une barre neuve porte une chute source, ou si une barre de
        /// chute ne porte pas de chute source d'identifiant strictement positif. Remonte
        /// également sans interception toute <see cref="Ex_Business"/> levée par le Command Handler.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si l'écriture échoue lors de la délégation au Command Handler.</exception>
        /// <exception cref="Ex_Unclassified">Levée si une exception non prévue est interceptée et ne relève d'aucune catégorie typée après requalification par <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        public async Task<ProductionBar> ExecuteAsync(
            string caller,
            DTO_CuttingOptimizationResult result,
            int idProductionSeries,
            int idArticleInternal,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // P1 - Résultat d'optimisation obligatoire.
                if (result is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le résultat d'optimisation (result) est obligatoire pour la création d'une barre de production.");

                // P2 - Identifiant de série strictement positif.
                if (idProductionSeries <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production (idProductionSeries) doit être strictement positif ; valeur reçue : {idProductionSeries}.");

                // P3 - Identifiant d'article interne strictement positif.
                if (idArticleInternal <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'article interne (idArticleInternal) doit être strictement positif ; valeur reçue : {idArticleInternal}.");

                // P4 - Seule une issue de succès décrit une barre à matérialiser.
                if (result.Outcome != En_CuttingOptimizationOutcome.Success)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"L'issue du résultat d'optimisation (result.Outcome) doit être Success ; valeur reçue : {result.Outcome}.");

                // P5 - Liste des découpes obligatoire.
                if (result.PieceIds is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "La liste des identifiants de découpes (result.PieceIds) est obligatoire.");

                // P6 - Un contenant retenu porte toujours au moins une découpe.
                if (result.PieceIds.Count == 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        "La liste des identifiants de découpes (result.PieceIds) doit contenir au moins un élément.");

                // P7 - Longueur de barre strictement positive.
                if (result.BarLength <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"La longueur de barre (result.BarLength) doit être strictement positive ; valeur reçue : {result.BarLength}.");

                // P8 - Une barre neuve ne porte pas de chute source.
                if (result.IsNewBar && result.IdSourceScrap is not null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Une barre neuve (result.IsNewBar = true) ne doit pas porter de chute source (result.IdSourceScrap) ; valeur reçue : {result.IdSourceScrap}.");

                // P9 - Une barre de chute porte une chute source valide.
                if (!result.IsNewBar && result.IdSourceScrap is null or <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Une barre de chute (result.IsNewBar = false) doit porter une chute source (result.IdSourceScrap) d'identifiant strictement positif ; valeur reçue : {result.IdSourceScrap?.ToString() ?? "null"}.");

                ct.ThrowIfCancellationRequested();

                // Tous les champs non listés conservent leur valeur par défaut ;
                // CreatedAt est positionné par le Command Handler générique.
                var bar = new ProductionBar
                {
                    IdProductionSeries = idProductionSeries,
                    IdArticleInternal = idArticleInternal,
                    IsNewBar = result.IsNewBar,
                    IdSourceScrap = result.IdSourceScrap,
                    BarLength = result.BarLength,
                    CutPieceCount = result.PieceIds.Count,
                    ResidueLength = result.ResidueLength,

                    // Polarité : true = chute réutilisable, false = déchet. Recopie sans transformation.
                    ResidueIsScrap = result.ResidueIsScrap,

                    // Placement provisoire : seul indicateur d'état positionné à la création.
                    IsOptimizedTemp = true
                };

                await _commandHandler.HandleAddAsync(callChain, bar, ct);

                return bar;
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