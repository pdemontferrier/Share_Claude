using System.Data;
using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.Business;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.B_UseCases.UseCases.Business
{
    /// <summary>
    /// UseCase de validation de la barre de production présentée à l'opérateur : engagement de
    /// la matière et scellement transactionnel du plan de coupe, conservé à l'identique ou
    /// recomposé autour des zones défectueuses signalées, ou mise à l'écart de la barre lorsque
    /// ces défauts la rendent improductive pour la série.
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
    /// Objectif : la barre présentée a été préparée sous forme provisoire, avec un plan de coupe
    /// provisoire et, pour une barre de chute, la réservation de la chute source. Sa validation
    /// par l'opérateur engage la matière et fige le plan. Le UseCase n'implémente aucune règle
    /// métier propre : il ordonnance les opérations, en délègue chaque écriture au Service métier
    /// qui en porte la responsabilité et confie la recomposition du plan au moteur
    /// d'optimisation.
    /// </para>
    /// <para>
    /// Trois issues abouties sont possibles. Sans défaut, le plan provisoire devient définitif à
    /// l'identique. Avec défauts, le plan provisoire est entièrement défait puis recomposé en
    /// contournant les zones perdues ; si au moins une découpe est placée, la barre est scellée
    /// avec son nouveau plan ; si aucune découpe n'est plaçable dans aucun segment sain, la barre
    /// est écartée pour défauts et les découpes restent disponibles pour une barre suivante.
    /// Cette dernière issue est régulière : sa transaction est validée comme celle des deux
    /// autres. Pour une barre de chute, la chute source est retirée du stock sur chacune des
    /// trois issues. L'indicateur d'approvisionnement de la série n'est posé que sur une barre
    /// scellée.
    /// </para>
    /// <para>
    /// Le traitement avec défauts comporte deux enregistrements dans la même transaction : la
    /// lecture du vivier est une projection sur l'état persistant, qui ignore les découpes encore
    /// marquées comme placées provisoirement ; les découpes libérées ne sont donc visibles par la
    /// recomposition qu'après un enregistrement intermédiaire suivant leur détachement.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire et propager la CallChain en début de scénario, conformément à §4.5.</description></item>
    /// <item><description>Ouvrir, valider ou annuler la transaction d'écriture, sous stratégie d'exécution, et rendre chaque tentative de la stratégie indépendante de l'état laissé par une tentative annulée.</description></item>
    /// <item><description>Valider les préconditions structurelles du scénario : identifiant de barre, existence de la barre, complétude de la première zone sur le traitement avec défauts, chute source d'une barre de chute, exploitabilité de l'issue de recomposition.</description></item>
    /// <item><description>Ordonnancer strictement le traitement avec défauts : inscription des défauts, détachement, enregistrement intermédiaire, lecture du vivier, recomposition, puis scellement ou mise à l'écart.</description></item>
    /// <item><description>Déléguer chaque écriture : validation, scellement, recomposition inscrite ou mise à l'écart de la barre ; scellement, détachement ou rattachement des découpes ; retrait de la chute source ; indicateur d'approvisionnement ; action de cycle de vie.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs typées à <c>IU_LogAndNotify</c> et restituer à la présentation une issue interprétable.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne contrôle ni l'état de la barre, ni la complétude, l'ordre, le non-chevauchement ou le bornage des zones défectueuses : ces contrôles sont portés par le Service de validation de la barre.</description></item>
    /// <item><description>Ne contrôle pas la correspondance entre le résultat de recomposition et la barre : ce contrôle est porté par le Service d'inscription du plan recomposé.</description></item>
    /// <item><description>N'implémente aucune règle de calcul et ne réordonne jamais la liste des découpes retenues : le rang de chaque découpe vaut position de coupe.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository (I-4.14.4 amendée, I-4.14.6) et ne requalifie aucune exception.</description></item>
    /// <item><description>N'émet aucun message d'information à l'opérateur sur une barre écartée pour défauts : ce message relève du ViewModel consommateur.</description></item>
    /// <item><description>Ne qualifie pas le reliquat de la barre et ne restitue aucun reste au stock.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_BarValidation"/>
    public class UC_BarValidation : IU_BarValidation
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        /// <summary>
        /// Motif stable, non traduit, d'une barre écartée pour défauts : inscrit comme motif de
        /// mise à l'écart sur la barre et repris en commentaire de l'action de cycle de vie.
        /// </summary>
        private const string DefectsNoPlaceableCutReason = "DEFECTS_NO_PLACEABLE_CUT";

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IS_AppContext _appContext;
        private readonly IQ_Generic<ProductionBar> _queryBar;
        private readonly IQ_VwProductionCutPieceFull _queryCutPiece;
        private readonly IS_CuttingOptimizer _optimizer;
        private readonly IS_ProductionBar_Validate _barValidate;
        private readonly IS_ProductionBar_UpdateOptimization _barUpdateOptimization;
        private readonly IS_ProductionBar_Reject _barReject;
        private readonly IS_ProductionCutPiece_SealToBar _pieceSeal;
        private readonly IS_ProductionCutPiece_DetachFromBar _pieceDetach;
        private readonly IS_ProductionCutPiece_AssignAndSealToBar _pieceAssignAndSeal;
        private readonly IS_CuttingScrapStock_Withdraw _scrapWithdraw;
        private readonly IS_ProductionSeries_SetSupplyFlag _seriesSupplyFlag;
        private readonly IS_LifecycleAction_Add _lifecycleAction;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_BarValidation"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la stratégie d'exécution, de la transaction et des enregistrements du scénario.</param>
        /// <param name="appContext">Service de contexte applicatif, source de l'utilisateur et de l'application courants inscrits au journal du cycle de vie.</param>
        /// <param name="queryBar">Query Handler générique des barres de production, pour la lecture sans suivi des attributs invariants de la barre présentée.</param>
        /// <param name="queryCutPiece">Query Handler des découpes de série, pour la lecture du vivier de recomposition.</param>
        /// <param name="optimizer">Moteur d'optimisation de découpe, calcul pur et synchrone de la recomposition d'une barre à défauts.</param>
        /// <param name="barValidate">Service métier de validation de la barre, avec ou sans inscription des zones défectueuses.</param>
        /// <param name="barUpdateOptimization">Service métier d'inscription du plan recomposé et de scellement de la barre à défauts.</param>
        /// <param name="barReject">Service métier de mise à l'écart de la barre.</param>
        /// <param name="pieceSeal">Service métier de scellement à l'identique des découpes rattachées provisoirement.</param>
        /// <param name="pieceDetach">Service métier de détachement des découpes rattachées provisoirement.</param>
        /// <param name="pieceAssignAndSeal">Service métier de rattachement ordonné et de scellement des découpes retenues par la recomposition.</param>
        /// <param name="scrapWithdraw">Service métier de retrait de la chute source du stock.</param>
        /// <param name="seriesSupplyFlag">Service métier de pose de l'indicateur d'approvisionnement de la série.</param>
        /// <param name="lifecycleAction">Service métier d'inscription d'une action au journal du cycle de vie.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_BarValidation(
            DbContext dbContext,
            IS_AppContext appContext,
            IQ_Generic<ProductionBar> queryBar,
            IQ_VwProductionCutPieceFull queryCutPiece,
            IS_CuttingOptimizer optimizer,
            IS_ProductionBar_Validate barValidate,
            IS_ProductionBar_UpdateOptimization barUpdateOptimization,
            IS_ProductionBar_Reject barReject,
            IS_ProductionCutPiece_SealToBar pieceSeal,
            IS_ProductionCutPiece_DetachFromBar pieceDetach,
            IS_ProductionCutPiece_AssignAndSealToBar pieceAssignAndSeal,
            IS_CuttingScrapStock_Withdraw scrapWithdraw,
            IS_ProductionSeries_SetSupplyFlag seriesSupplyFlag,
            IS_LifecycleAction_Add lifecycleAction,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _queryBar = queryBar ?? throw new ArgumentNullException(nameof(queryBar));
            _queryCutPiece = queryCutPiece ?? throw new ArgumentNullException(nameof(queryCutPiece));
            _optimizer = optimizer ?? throw new ArgumentNullException(nameof(optimizer));
            _barValidate = barValidate ?? throw new ArgumentNullException(nameof(barValidate));
            _barUpdateOptimization = barUpdateOptimization ?? throw new ArgumentNullException(nameof(barUpdateOptimization));
            _barReject = barReject ?? throw new ArgumentNullException(nameof(barReject));
            _pieceSeal = pieceSeal ?? throw new ArgumentNullException(nameof(pieceSeal));
            _pieceDetach = pieceDetach ?? throw new ArgumentNullException(nameof(pieceDetach));
            _pieceAssignAndSeal = pieceAssignAndSeal ?? throw new ArgumentNullException(nameof(pieceAssignAndSeal));
            _scrapWithdraw = scrapWithdraw ?? throw new ArgumentNullException(nameof(scrapWithdraw));
            _seriesSupplyFlag = seriesSupplyFlag ?? throw new ArgumentNullException(nameof(seriesSupplyFlag));
            _lifecycleAction = lifecycleAction ?? throw new ArgumentNullException(nameof(lifecycleAction));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Valide la barre de production présentée et scelle son plan de coupe, en le recomposant
        /// autour des zones défectueuses signalées, ou écarte la barre lorsque ces défauts la
        /// rendent improductive pour la série.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Séquence commune, intégralement contenue dans le délégué de la stratégie d'exécution et
        /// dans une transaction en isolation <c>ReadCommitted</c> : contrôle de l'identifiant de
        /// barre ; lecture du contexte applicatif ; lecture sans suivi de la barre, dont seuls les
        /// attributs invariants sont exploités ; validation de la barre, avec inscription des
        /// zones défectueuses le cas échéant. Une première borne de début de zone renseignée
        /// désigne ensuite le traitement avec défauts.
        /// </para>
        /// <para>
        /// Sans défaut : scellement à l'identique des découpes rattachées, puis clôture scellée.
        /// Avec défauts : contrôle de la borne de fin de la première zone ; détachement des
        /// découpes ; enregistrement intermédiaire ; lecture du vivier de la série et de l'article
        /// de la barre, transmis tel quel et dans l'ordre fourni ; recomposition synchrone ;
        /// lecture exhaustive de son issue. Sur un succès, rattachement et scellement des découpes
        /// retenues dans l'ordre calculé, inscription du plan recomposé sur la barre, puis clôture
        /// scellée. Si aucun segment sain n'accueille de découpe, mise à l'écart de la barre avec
        /// le motif <c>DEFECTS_NO_PLACEABLE_CUT</c>, retrait de la chute source pour une barre de
        /// chute, inscription de l'action <c>BarRefused</c> commentée par ce même motif,
        /// enregistrement final et validation de la transaction. Toute autre issue est rejetée.
        /// </para>
        /// <para>
        /// Clôture scellée : retrait de la chute source pour une barre de chute ; pose de
        /// l'indicateur d'approvisionnement de la série, sans écriture s'il est déjà posé ;
        /// inscription de l'action <c>BarValidated</c> ou <c>BarWithDefectsValidated</c> sans
        /// commentaire ; enregistrement final et validation de la transaction.
        /// </para>
        /// <para>
        /// Enregistrements : un sur le traitement sans défaut, deux sur chacune des deux issues
        /// avec défauts, tous dans la même transaction. L'annulation explicite est réservée au
        /// traitement des exceptions typées, chacune étant annulée puis confiée à
        /// <c>IU_LogAndNotify</c> (clés <c>No_EC_01</c>, <c>No_EC_02</c>, <c>No_EC_03</c>).
        /// L'annulation coopérative est propagée sans journalisation ; la transaction est alors
        /// annulée par la libération du bloc <c>await using</c>.
        /// </para>
        /// <para>
        /// Comportement au rejeu : une défaillance transitoire survenant lors d'un enregistrement
        /// n'est pas typée ; elle sort du délégué et la stratégie d'exécution le rejoue en entier,
        /// sous une nouvelle transaction. Le contexte partagé conserve toutefois l'état suivi de la
        /// tentative annulée : barre, découpes, chute, série et action de cycle de vie modifiées
        /// ou ajoutées en mémoire. Sans neutralisation, un rejeu réécrirait ces changements dès son
        /// premier enregistrement. Chaque tentative postérieure à la première commence donc par
        /// vider le suivi des changements ; elle repart ainsi d'un état équivalent à celui de la
        /// première tentative, l'état de la base ayant été rétabli par l'annulation de la
        /// transaction. La première tentative n'altère aucun état suivi antérieur à l'invocation.
        /// </para>
        /// <para>
        /// Limite : si la validation de la transaction aboutit côté serveur alors que son
        /// acquittement est perdu, le rejeu repart de l'état persisté et trouve la barre déjà
        /// validée ; le Service de validation la rejette et l'issue restituée vaut
        /// <see cref="En_BarValidationOutcome.Failed"/> alors que la validation est effective.
        /// Aucune donnée n'est corrompue.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production présentée. Doit être strictement positif ; contrôlé dans le bloc transactionnel.</param>
        /// <param name="defectStart1">Début de la première zone défectueuse, en millimètres depuis la tête de la barre ; renseigné, il désigne le traitement avec défauts.</param>
        /// <param name="defectEnd1">Fin de la première zone défectueuse, en millimètres depuis la tête de la barre ; requise sur le traitement avec défauts.</param>
        /// <param name="defectStart2">Début de la seconde zone défectueuse, en millimètres depuis la tête de la barre. Facultatif.</param>
        /// <param name="defectEnd2">Fin de la seconde zone défectueuse, en millimètres depuis la tête de la barre ; requise lorsque <paramref name="defectStart2"/> est renseigné.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, vaut :
        /// <see cref="En_BarValidationOutcome.BarSealed"/> lorsque la barre est validée et scellée,
        /// avec ou sans défauts, après validation de la transaction ;
        /// <see cref="En_BarValidationOutcome.BarUnusable"/> lorsque la barre est écartée pour
        /// défauts, après validation de la transaction ;
        /// <see cref="En_BarValidationOutcome.Failed"/> sur échec applicatif typé, après
        /// annulation de la transaction et délégation à <c>IU_LogAndNotify</c>. La valeur
        /// <see cref="En_BarValidationOutcome.Undetermined"/> n'est jamais retournée.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/> (§4.6).
        /// </exception>
        public async Task<En_BarValidationOutcome> ExecuteAsync(
            string caller,
            int idProductionBar,
            int? defectStart1,
            int? defectEnd1,
            int? defectStart2,
            int? defectEnd2,
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
            return await strategy.ExecuteAsync<En_BarValidationOutcome>(async () =>
            {
                // C1 — Ouverture de la transaction.
                await using var transaction = await _dbContext.Database
                    .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

                // C2 — Neutralisation de l'état suivi laissé par une tentative annulée. La
                // première tentative n'est pas concernée.
                if (attemptCount++ > 0)
                    _dbContext.ChangeTracker.Clear();

                try
                {
                    // C3 — Précondition structurelle sur la barre, dans le try afin d'être
                    // captée par le patron canonique.
                    if (idProductionBar <= 0)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_02,
                            $"Identifiant de barre de production invalide ({idProductionBar}) : une valeur strictement positive est attendue.");

                    // C4 — Contexte applicatif, lu une fois par tentative.
                    DTO_AppContext ctx = _appContext.GetAppContext();

                    // C5 — Lecture sans suivi : seuls des attributs invariants de la barre sont
                    // exploités ; les Services relisent la barre en suivi.
                    ProductionBar? bar = await _queryBar.HandleGetByIdAsNoTrackingAsync(
                        callChain, idProductionBar, ct);

                    if (bar is null)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_03,
                            $"Barre de production introuvable (Id = {idProductionBar}).");

                    // C6 — Validation de la barre ; contrôle d'état et de cohérence des bornes
                    // porté par le Service.
                    await _barValidate.ExecuteAsync(
                        callChain, idProductionBar, defectStart1, defectEnd1, defectStart2, defectEnd2, ct);

                    // C7 — Discriminant du traitement, aligné sur celui du Service de validation.
                    En_LifecycleActionType lifecycleType;

                    if (defectStart1 is int start1)
                    {
                        // D1 — Garde défensive, inatteignable après la validation ; aucun accès
                        // .Value, dont l'échec non typé échapperait au patron.
                        if (defectEnd1 is not int end1)
                            throw new Ex_Business(
                                callChain,
                                Ex_Business.ErrorCodes.BU_ER_03,
                                $"Borne de fin de la première zone défectueuse absente sur la barre de production {idProductionBar} (début = {start1} mm).");

                        // D2 — Le plan provisoire est entièrement défait.
                        await _pieceDetach.ExecuteAsync(callChain, idProductionBar, ct);

                        // D3 — Enregistrement intermédiaire : le vivier est lu sur l'état
                        // persistant et ignore les découpes encore placées provisoirement.
                        await _dbContext.SaveChangesAsync(ct);

                        // D4 — Vivier de la série et de l'article, transmis tel quel : l'ordre
                        // fourni par la lecture fonde le déterminisme du moteur.
                        List<DTO_VwProductionCutPieceFull_P20> pool =
                            await _queryCutPiece.HandleGetOptimizationPoolForP20AsNoTrackingAsync(
                                callChain, bar.IdProductionSeries, bar.IdArticleInternal, ct);

                        // D5 — Calcul pur et synchrone ; l'annulation est honorée par l'appel
                        // asynchrone suivant.
                        DTO_CuttingOptimizationResult result = _optimizer.OptimizeWithDefects(
                            callChain,
                            pool,
                            bar.BarLength,
                            bar.IsNewBar,
                            bar.IdSourceScrap,
                            start1,
                            end1,
                            defectStart2,
                            defectEnd2);

                        // D6 — Lecture exhaustive de l'issue.
                        switch (result.Outcome)
                        {
                            case En_CuttingOptimizationOutcome.Success:
                                break;

                            case En_CuttingOptimizationOutcome.NoUsableSegment:
                                // Issue dégradée régulière : la barre est écartée pour défauts
                                // et la transaction est validée.
                                await RejectBarForDefectsAsync(callChain, ctx, bar, ct);

                                // R4 — Enregistrement final, puis validation (§4.10.4).
                                await _dbContext.SaveChangesAsync(ct);
                                await transaction.CommitAsync(ct);

                                // Issue restituée à la présentation (R-4.14.22).
                                return En_BarValidationOutcome.BarUnusable;

                            default:
                                // DataAnomaly, EmptyPool, Undetermined et toute valeur
                                // inconnue : issue inexploitable.
                                throw BuildUnexploitableOutcomeException(callChain, result.Outcome);
                        }

                        // D7 — Rattachement et scellement, liste transmise sans
                        // réordonnancement : le rang vaut position de coupe.
                        await _pieceAssignAndSeal.ExecuteAsync(
                            callChain, result.PieceIds, idProductionBar, ct);

                        // D8 — Inscription du plan recomposé et scellement de la barre ;
                        // correspondance entre résultat et barre contrôlée par le Service.
                        await _barUpdateOptimization.ExecuteAsync(
                            callChain, idProductionBar, result, ct);

                        // D9 — Type d'action de la clôture scellée.
                        lifecycleType = En_LifecycleActionType.BarWithDefectsValidated;
                    }
                    else
                    {
                        // S1 — Le plan provisoire devient définitif à l'identique.
                        await _pieceSeal.ExecuteAsync(callChain, idProductionBar, ct);

                        // S2 — Type d'action de la clôture scellée.
                        lifecycleType = En_LifecycleActionType.BarValidated;
                    }

                    // F1 — Retrait de la chute source, sur barre de chute uniquement.
                    await WithdrawSourceScrapIfDropBarAsync(callChain, bar, ct);

                    // F2 — Indicateur d'approvisionnement ; le Service sort sans écrire s'il est
                    // déjà posé.
                    await _seriesSupplyFlag.ExecuteAsync(
                        callChain, bar.IdProductionSeries, bar.IsNewBar, ct);

                    // F3 — Action de cycle de vie, sans commentaire.
                    await _lifecycleAction.ExecuteAsync(
                        callChain,
                        ctx,
                        En_LifecycleActionSource.ProductionBar,
                        lifecycleType,
                        idProductionBar,
                        null,
                        ct);

                    // F4 — Enregistrement final, puis validation de la transaction (§4.10.4).
                    await _dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    // Issue restituée à la présentation (R-4.14.22).
                    return En_BarValidationOutcome.BarSealed;
                }
                catch (Ex_Business ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
                    return En_BarValidationOutcome.Failed;
                }
                catch (Ex_Infrastructure ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
                    return En_BarValidationOutcome.Failed;
                }
                catch (Ex_Unclassified ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
                    return En_BarValidationOutcome.Failed;
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
        /// Écarte pour défauts la barre dont aucun segment sain n'accueille de découpe : mise à
        /// l'écart motivée, retrait de la chute source pour une barre de chute et inscription de
        /// l'action de cycle de vie correspondante, sans enregistrer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Les découpes ont été détachées et enregistrées avant la recomposition : elles restent
        /// disponibles pour une barre suivante. Aucun indicateur d'approvisionnement n'est posé et
        /// aucun reste n'est restitué. L'action inscrite est <c>BarRefused</c>, dont le
        /// commentaire <c>DEFECTS_NO_PLACEABLE_CUT</c> distingue la barre écartée pour défauts
        /// d'un refus de l'opérateur. L'enregistrement et la validation de la transaction restent
        /// à la charge de l'appelant.
        /// </para>
        /// </remarks>
        /// <param name="callChain">CallChain construite par la méthode publique appelante.</param>
        /// <param name="ctx">Contexte applicatif lu par la tentative courante.</param>
        /// <param name="bar">Barre de production lue sans suivi, dont seuls les attributs invariants sont exploités.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé aux appels asynchrones.</param>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
        private async Task RejectBarForDefectsAsync(
            string callChain,
            DTO_AppContext ctx,
            ProductionBar bar,
            CancellationToken ct)
        {
            // R1 — Mise à l'écart motivée.
            await _barReject.ExecuteAsync(callChain, bar.Id, DefectsNoPlaceableCutReason, ct);

            // R2 — Retrait de la chute source, sur barre de chute uniquement.
            await WithdrawSourceScrapIfDropBarAsync(callChain, bar, ct);

            // R3 — Action de cycle de vie commentée par le motif.
            await _lifecycleAction.ExecuteAsync(
                callChain,
                ctx,
                En_LifecycleActionSource.ProductionBar,
                En_LifecycleActionType.BarRefused,
                bar.Id,
                DefectsNoPlaceableCutReason,
                ct);
        }

        /// <summary>
        /// Retire du stock la chute source d'une barre de chute ; sans effet pour une barre
        /// neuve.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La chute source est celle référencée par la barre lue, sur chacune des issues abouties.
        /// Une barre de chute sans chute source traduit une incohérence d'état et interrompt le
        /// scénario.
        /// </para>
        /// </remarks>
        /// <param name="callChain">CallChain construite par la méthode publique appelante.</param>
        /// <param name="bar">Barre de production lue sans suivi.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé à l'appel asynchrone.</param>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
        /// <exception cref="Ex_Business">
        /// Code <c>BU_ER_04</c> : barre de chute ne référençant aucune chute source.
        /// </exception>
        private async Task WithdrawSourceScrapIfDropBarAsync(
            string callChain,
            ProductionBar bar,
            CancellationToken ct)
        {
            if (bar.IsNewBar)
                return;

            if (bar.IdSourceScrap is not int idScrap)
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_04,
                    $"La barre de chute {bar.Id} ne référence aucune chute source : son retrait du stock est impossible.");

            await _scrapWithdraw.ExecuteAsync(callChain, idScrap, ct);
        }

        /// <summary>
        /// Construit l'exception métier signalant une issue de recomposition inexploitable pour
        /// la validation d'une barre à défauts, avec un message différencié selon l'issue reçue.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Seules les issues de succès et de segment inexploitable sont exploitables par la
        /// validation d'une barre à défauts. Une anomalie de données traduit des entrées
        /// incohérentes ; un vivier vide contredit le détachement préalable d'un plan provisoire
        /// non vide ; la sentinelle non valorisée et toute valeur inconnue de l'énumération sont
        /// rejetées par garde défensive.
        /// </para>
        /// </remarks>
        /// <param name="callChain">CallChain construite par la méthode publique appelante.</param>
        /// <param name="outcome">Issue rendue par le moteur d'optimisation, autre qu'un succès ou un segment inexploitable.</param>
        /// <returns>L'exception <see cref="Ex_Business"/> au code <c>BU_ER_04</c>, à lever par l'appelant.</returns>
        private static Ex_Business BuildUnexploitableOutcomeException(
            string callChain,
            En_CuttingOptimizationOutcome outcome)
        {
            string message = outcome switch
            {
                En_CuttingOptimizationOutcome.DataAnomaly =>
                    $"Recomposition inexploitable (Outcome = {outcome}) : les données d'entrée du moteur d'optimisation sont incohérentes.",
                En_CuttingOptimizationOutcome.EmptyPool =>
                    $"Recomposition inexploitable (Outcome = {outcome}) : le vivier est vide alors que le plan provisoire de la barre vient d'être défait.",
                En_CuttingOptimizationOutcome.Undetermined =>
                    $"Recomposition inexploitable (Outcome = {outcome}) : issue non valorisée par le moteur d'optimisation.",
                _ =>
                    $"Recomposition inexploitable (Outcome = {outcome}) : issue inconnue du moteur d'optimisation."
            };

            return new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_04, message);
        }

        #endregion
    }
}