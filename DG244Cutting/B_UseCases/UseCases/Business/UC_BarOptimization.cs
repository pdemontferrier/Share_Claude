using System.Data;
using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.Business;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.Business;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.B_UseCases.UseCases.Business
{
    /// <summary>
    /// UseCase de préparation de la prochaine barre de production d'une série : résolution de la
    /// matière à traiter, délégation du calcul d'optimisation, puis matérialisation
    /// transactionnelle de la barre provisoire, de son plan de coupe et de la réservation de la
    /// chute source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : maillon 2 de la chaîne (1) d'écriture stricte VM → UC → SR → CH → CR (§4.14.9,
    /// R-4.14.19), consommé en chaîne directe par <c>VM_Page20</c>. Seul composant de la chaîne
    /// autorisé à ouvrir, valider et annuler la transaction (R-4.10.1) et à appeler
    /// <c>SaveChangesAsync</c>. Le bloc transactionnel est encapsulé dans
    /// <c>CreateExecutionStrategy().ExecuteAsync</c>, exigé par la réexécution sur échec activée
    /// au câblage du contexte de persistance (§4.10.1).
    /// </para>
    /// <para>
    /// Objectif : l'atelier est approvisionné à la demande et une seule barre est préparée par
    /// invocation. Le UseCase résout la prochaine combinaison référence / couleur de la série et
    /// l'article interne qui la porte, lit le vivier des découpes de cet article et les chutes
    /// disponibles, confie le choix du contenant et de son contenu au moteur d'optimisation, puis
    /// matérialise le résultat en déléguant chaque écriture au Service métier qui en porte la
    /// responsabilité. La création de la barre, le rattachement des découpes et la réservation
    /// de la chute source sont validés ou annulés ensemble.
    /// </para>
    /// <para>
    /// Deux enregistrements sont nécessaires : la découpe ne référence sa barre que par
    /// identifiant, sans navigation ; l'identifiant de la barre créée n'est donc disponible
    /// qu'après un premier enregistrement intermédiaire, que suit un enregistrement final
    /// couvrant le rattachement et la réservation. Les deux enregistrements appartiennent à la
    /// même transaction.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire et propager la CallChain en début de scénario, conformément à §4.5.</description></item>
    /// <item><description>Ouvrir, valider ou annuler la transaction d'écriture, sous stratégie d'exécution, et rendre chaque tentative de la stratégie indépendante de l'état laissé par une tentative annulée.</description></item>
    /// <item><description>Valider les préconditions structurelles du scénario : identifiant de série, article interne de la prochaine découpe, exploitabilité de l'issue d'optimisation.</description></item>
    /// <item><description>Transmettre au moteur d'optimisation le vivier dans l'ordre fourni par la lecture et les seules chutes disponibles de l'article traité.</description></item>
    /// <item><description>Déléguer la création de la barre, le rattachement ordonné des découpes et, pour une barre de chute uniquement, la réservation de la chute source.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs typées à <c>IU_LogAndNotify</c> et restituer à la présentation un retour interprétable.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'implémente aucune règle de calcul : placement, choix de la chute et qualification du résidu relèvent du moteur d'optimisation.</description></item>
    /// <item><description>Ne contrôle pas la cohérence interne du résultat d'optimisation (liste non vide, longueur positive, accord entre origine et chute source) : ce contrôle est porté par le Service de création de la barre.</description></item>
    /// <item><description>Ne réordonne jamais la liste des découpes retenues : le rang de chaque découpe vaut position de coupe.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository (I-4.14.4 amendée, I-4.14.6) et ne requalifie aucune exception.</description></item>
    /// <item><description>Ne positionne pas le contexte de sélection de la barre, n'inscrit aucune action de cycle de vie et ne pose aucun indicateur d'approvisionnement de la série.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_BarOptimization"/>
    public class UC_BarOptimization : IU_BarOptimization
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IQ_VwProductionCutPieceFull _queryCutPiece;
        private readonly IQ_Generic<vw_CuttingScrapStock_Full> _queryScrapStock;
        private readonly IS_CuttingOptimizer _optimizer;
        private readonly IS_ProductionBar_Create _barCreate;
        private readonly IS_ProductionCutPiece_AssignToBar _pieceAssign;
        private readonly IS_CuttingScrapStock_Reserve _scrapReserve;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_BarOptimization"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la stratégie d'exécution, de la transaction et des enregistrements du scénario.</param>
        /// <param name="queryCutPiece">Query Handler des découpes de série : recherche de la prochaine découpe à réaliser et lecture du vivier d'optimisation.</param>
        /// <param name="queryScrapStock">Query Handler générique du stock de chutes, pour la lecture des chutes disponibles de l'article traité.</param>
        /// <param name="optimizer">Moteur d'optimisation de découpe, calcul pur et synchrone.</param>
        /// <param name="barCreate">Service métier de création de la barre de production provisoire.</param>
        /// <param name="pieceAssign">Service métier de rattachement ordonné des découpes à la barre.</param>
        /// <param name="scrapReserve">Service métier de réservation de la chute source au profit de la série.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_BarOptimization(
            DbContext dbContext,
            IQ_VwProductionCutPieceFull queryCutPiece,
            IQ_Generic<vw_CuttingScrapStock_Full> queryScrapStock,
            IS_CuttingOptimizer optimizer,
            IS_ProductionBar_Create barCreate,
            IS_ProductionCutPiece_AssignToBar pieceAssign,
            IS_CuttingScrapStock_Reserve scrapReserve,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _queryCutPiece = queryCutPiece ?? throw new ArgumentNullException(nameof(queryCutPiece));
            _queryScrapStock = queryScrapStock ?? throw new ArgumentNullException(nameof(queryScrapStock));
            _optimizer = optimizer ?? throw new ArgumentNullException(nameof(optimizer));
            _barCreate = barCreate ?? throw new ArgumentNullException(nameof(barCreate));
            _pieceAssign = pieceAssign ?? throw new ArgumentNullException(nameof(pieceAssign));
            _scrapReserve = scrapReserve ?? throw new ArgumentNullException(nameof(scrapReserve));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Prépare la prochaine barre de production de la série désignée et la matérialise en
        /// base avec son plan de coupe provisoire et, pour une barre de chute, la réservation de
        /// la chute source.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Séquence, intégralement contenue dans le délégué de la stratégie d'exécution et dans
        /// une transaction en isolation <c>ReadCommitted</c> : contrôle de l'identifiant de série ;
        /// recherche de la prochaine découpe à réaliser, dont l'absence clôt le scénario sans
        /// écriture ; contrôle de son article interne ; lecture du vivier de l'article, transmis
        /// tel quel et dans l'ordre fourni, puis des chutes disponibles de l'article, c'est-à-dire
        /// non supprimées, non en attente d'intégration et dont la réservation est nulle ou vide ;
        /// appel synchrone du moteur d'optimisation ; lecture de l'issue, un vivier vide clôturant
        /// le scénario sans écriture et toute issue autre qu'un succès étant rejetée ; création de
        /// la barre ; enregistrement intermédiaire rendant son identifiant disponible ; rattachement
        /// des découpes dans l'ordre calculé ; réservation de la chute source si la barre en est
        /// issue ; enregistrement final et validation de la transaction.
        /// </para>
        /// <para>
        /// Les sorties sans écriture sont clôturées par la validation d'une transaction vide ;
        /// l'annulation explicite est réservée au traitement des exceptions typées, chacune étant
        /// annulée puis confiée à <c>IU_LogAndNotify</c> (clés <c>No_EC_01</c>, <c>No_EC_02</c>,
        /// <c>No_EC_03</c>). L'annulation coopérative est propagée sans journalisation ; la
        /// transaction est alors annulée par la libération du bloc <c>await using</c>.
        /// </para>
        /// <para>
        /// Comportement au rejeu : une défaillance transitoire survenant lors d'un enregistrement
        /// n'est pas typée ; elle sort du délégué et la stratégie d'exécution le rejoue en entier,
        /// sous une nouvelle transaction. Le contexte partagé conserve toutefois l'état suivi de la
        /// tentative annulée : barre encore à insérer si l'échec touche l'enregistrement
        /// intermédiaire, découpes et chute déjà modifiées en mémoire s'il touche l'enregistrement
        /// final. Sans neutralisation, un rejeu insérerait une seconde barre et validerait une
        /// barre sans découpe, ou écrirait des rattachements vers une barre annulée. Chaque
        /// tentative postérieure à la première commence donc par vider le suivi des changements ;
        /// elle repart ainsi d'un état équivalent à celui de la première tentative, l'état de la
        /// base ayant été rétabli par l'annulation de la transaction. La première tentative
        /// n'altère aucun état suivi antérieur à l'invocation.
        /// </para>
        /// <para>
        /// Limite : si la validation de la transaction aboutit côté serveur alors que son
        /// acquittement est perdu, le rejeu repart de l'état persisté ; la barre déjà validée est
        /// conservée et la tentative prépare la barre suivante de la série, dont l'identifiant est
        /// seul restitué. Aucune donnée n'est corrompue, mais deux barres provisoires résultent
        /// alors d'une même invocation.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionSeries">Identifiant de la série de production sélectionnée. Doit être strictement positif ; contrôlé dans le bloc transactionnel.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, vaut : l'identifiant strictement
        /// positif de la barre provisoire créée, après validation de la transaction ; zéro lorsque
        /// la série ne compte plus aucune découpe optimisable, sans écriture ; <see langword="null"/>
        /// sur échec applicatif typé, après annulation de la transaction et délégation à
        /// <c>IU_LogAndNotify</c>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/> (§4.6).
        /// </exception>
        public async Task<int?> ExecuteAsync(
            string caller,
            int idProductionSeries,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // Compteur de tentatives du délégué, capturé par la fermeture : la stratégie
            // d'exécution peut rejouer le délégué en entier sur défaillance transitoire.
            int attemptCount = 0;

            // ===================================================================
            // Bloc transactionnel — DANS le délégué de la stratégie d'exécution.
            // Encapsulation exigée par EnableRetryOnFailure (§4.10.1) : sans elle,
            // BeginTransactionAsync lève InvalidOperationException.
            // ===================================================================
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync<int?>(async () =>
            {
                await using var transaction = await _dbContext.Database
                    .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

                // Neutralisation de l'état suivi laissé par une tentative annulée : sur un
                // rejeu, les entités ajoutées ou modifiées par la tentative précédente seraient
                // réécrites par le premier enregistrement. La première tentative n'est pas
                // concernée.
                if (attemptCount++ > 0)
                    _dbContext.ChangeTracker.Clear();

                try
                {
                    // Étape 1 — Précondition structurelle sur la série, dans le try afin
                    // d'être captée par le patron canonique.
                    if (idProductionSeries <= 0)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_02,
                            $"Identifiant de série de production invalide ({idProductionSeries}) : une valeur strictement positive est attendue.");

                    // Étape 2 — Prochaine découpe à réaliser, tous articles confondus.
                    DTO_VwProductionCutPieceFull_P20? next =
                        await _queryCutPiece.HandleGetNextReferenceToCutForP20AsNoTrackingAsync(
                            callChain, idProductionSeries, ct);

                    // Aucune découpe optimisable : sortie nominale sans écriture.
                    if (next is null)
                    {
                        await transaction.CommitAsync(ct);
                        return 0;
                    }

                    // Étape 3 — Article interne de la prochaine découpe, par filtrage par
                    // motif : aucun accès .Value, dont l'échec non typé échapperait au patron.
                    if (next.PCPIdArticleInternal is not int idArticle || idArticle <= 0)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_04,
                            $"Article interne absent ou invalide ({next.PCPIdArticleInternal?.ToString() ?? "null"}) sur la prochaine découpe à réaliser (PCPId = {next.PCPId}).");

                    // Étape 4 — Vivier de l'article, transmis tel quel : l'ordre par PCPId
                    // fourni par la lecture fonde le déterminisme du moteur.
                    List<DTO_VwProductionCutPieceFull_P20> pool =
                        await _queryCutPiece.HandleGetOptimizationPoolForP20AsNoTrackingAsync(
                            callChain, idProductionSeries, idArticle, ct);

                    // Étape 5 — Chutes disponibles de l'article. La vue étant sans clé, seule
                    // une lecture opérante du socle est employée ; l'exclusion des chutes
                    // supprimées est explicite, et une réservation vide vaut absence de
                    // réservation.
                    List<vw_CuttingScrapStock_Full> stock =
                        await _queryScrapStock.HandleGetFilteredAsNoTrackingAsync(
                            callChain,
                            s => s.CSSIdArticleInternal == idArticle
                                && !s.CSSIsDeleted
                                && !s.CSSWaitForIntegration
                                && (s.CSSReservedFor == null || s.CSSReservedFor == ""),
                            ct);

                    // Étape 6 — Calcul pur et synchrone ; l'annulation est honorée par
                    // l'appel asynchrone suivant.
                    DTO_CuttingOptimizationResult result =
                        _optimizer.Optimize(callChain, pool, stock);

                    // Étape 7 — Lecture exhaustive de l'issue.
                    switch (result.Outcome)
                    {
                        case En_CuttingOptimizationOutcome.Success:
                            break;

                        case En_CuttingOptimizationOutcome.EmptyPool:
                            // Vivier vide : sortie nominale sans écriture.
                            await transaction.CommitAsync(ct);
                            return 0;

                        default:
                            // DataAnomaly, NoUsableSegment, Undetermined et toute valeur
                            // inconnue : issue inexploitable.
                            throw BuildUnexploitableOutcomeException(callChain, result.Outcome);
                    }

                    // Étape 8 — Création de la barre provisoire ; l'instance est conservée
                    // pour lire son identifiant après l'enregistrement intermédiaire.
                    ProductionBar bar = await _barCreate.ExecuteAsync(
                        callChain, result, idProductionSeries, idArticle, ct);

                    // Étape 9 — Enregistrement intermédiaire : la découpe ne référence sa
                    // barre que par identifiant, attribué par la base.
                    await _dbContext.SaveChangesAsync(ct);

                    // Étape 10 — Rattachement des découpes, liste transmise sans
                    // réordonnancement : le rang vaut position de coupe.
                    await _pieceAssign.ExecuteAsync(callChain, result.PieceIds, bar.Id, ct);

                    // Étape 11 — Réservation de la chute source, sur barre de chute
                    // uniquement ; l'accord entre origine et chute source est garanti par le
                    // Service de création de la barre.
                    if (result.IdSourceScrap is int idScrap)
                        await _scrapReserve.ExecuteAsync(callChain, idScrap, idProductionSeries, ct);

                    // Étape 12 — Enregistrement final (rattachement et réservation), puis
                    // validation de la transaction (§4.10.4).
                    await _dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    // Issue nominale restituée à la présentation (R-4.14.22).
                    return bar.Id;
                }
                catch (Ex_Business ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
                    return null;
                }
                catch (Ex_Infrastructure ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
                    return null;
                }
                catch (Ex_Unclassified ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
                    return null;
                }
                catch (OperationCanceledException)
                {
                    // Annulation coopérative : la transaction est annulée par le Dispose
                    // du await using interne au délégué. Aucune journalisation ni
                    // notification ; aucune réexécution par la stratégie d'exécution.
                    throw;
                }
            });
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Construit l'exception métier signalant une issue d'optimisation inexploitable pour la
        /// préparation d'une barre, avec un message différencié selon l'issue reçue.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Seules les issues de succès et de vivier vide sont exploitables par la préparation
        /// d'une barre. Une anomalie de données traduit des entrées incohérentes ; l'issue de
        /// segment inexploitable est propre à la recomposition d'une barre à défauts et étrangère
        /// à ce scénario ; la sentinelle non valorisée et toute valeur inconnue de l'énumération
        /// sont rejetées par garde défensive.
        /// </para>
        /// </remarks>
        /// <param name="callChain">CallChain construite par la méthode publique appelante.</param>
        /// <param name="outcome">Issue rendue par le moteur d'optimisation, autre qu'un succès ou un vivier vide.</param>
        /// <returns>L'exception <see cref="Ex_Business"/> au code <c>BU_ER_04</c>, à lever par l'appelant.</returns>
        private static Ex_Business BuildUnexploitableOutcomeException(
            string callChain,
            En_CuttingOptimizationOutcome outcome)
        {
            string message = outcome switch
            {
                En_CuttingOptimizationOutcome.DataAnomaly =>
                    $"Optimisation inexploitable (Outcome = {outcome}) : les données d'entrée du moteur d'optimisation sont incohérentes.",
                En_CuttingOptimizationOutcome.NoUsableSegment =>
                    $"Optimisation inexploitable (Outcome = {outcome}) : issue propre à la recomposition d'une barre à défauts, étrangère à la préparation d'une barre.",
                En_CuttingOptimizationOutcome.Undetermined =>
                    $"Optimisation inexploitable (Outcome = {outcome}) : issue non valorisée par le moteur d'optimisation.",
                _ =>
                    $"Optimisation inexploitable (Outcome = {outcome}) : issue inconnue du moteur d'optimisation."
            };

            return new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_04, message);
        }

        #endregion
    }
}