using System.Data;
using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.Business;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.B_UseCases.UseCases.Business
{
    /// <summary>
    /// UseCase de refus d'une barre de production par l'opérateur : annulation transactionnelle
    /// du placement provisoire posé par l'optimisation, mise à l'écart motivée de la barre,
    /// libération de ses découpes, retrait définitif de la chute source pour une barre de chute,
    /// et inscription du refus au journal métier.
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
    /// Objectif : une barre présentée à l'opérateur est provisoire ; lorsqu'il la refuse, rien de
    /// ce que l'optimisation avait posé ne doit subsister. Le UseCase contrôle la présence du
    /// motif, lit la barre pour en connaître l'origine, puis délègue chaque écriture au Service
    /// métier qui en porte la responsabilité : mise à l'écart de la barre avec son motif,
    /// libération de ses découpes qui retournent au vivier en conservant la trace du refus,
    /// retrait définitif de la chute source lorsque la barre en est issue, inscription du refus
    /// au journal métier avec le motif pour commentaire. Ces écritures sont validées ou annulées
    /// ensemble.
    /// </para>
    /// <para>
    /// Un seul enregistrement est nécessaire : toutes les écritures portent sur des entités
    /// existantes identifiées à l'avance ou sur une entrée de journal dont aucune écriture
    /// ultérieure ne dépend ; aucun identifiant attribué par la base n'est attendu en cours de
    /// scénario.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire et propager la CallChain en début de scénario, conformément à §4.5.</description></item>
    /// <item><description>Ouvrir, valider ou annuler la transaction d'écriture, sous stratégie d'exécution, et rendre chaque tentative de la stratégie indépendante de l'état laissé par une tentative annulée.</description></item>
    /// <item><description>Valider les préconditions structurelles du scénario : identifiant de barre, présence du motif, existence de la barre, cohérence entre l'origine de la barre et sa chute source.</description></item>
    /// <item><description>Déterminer, à partir de l'origine de la barre, si la chute source doit être retirée du stock.</description></item>
    /// <item><description>Déléguer la mise à l'écart de la barre, la libération de ses découpes, le retrait de la chute source et l'inscription au journal métier, dans cet ordre.</description></item>
    /// <item><description>Transmettre le motif sans aucune altération à la mise à l'écart de la barre et au journal métier.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs typées à <c>IU_LogAndNotify</c> et restituer à la présentation un retour interprétable.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'implémente aucune règle métier propre et n'écrit jamais directement en base : chaque écriture est déléguée à son Service métier.</description></item>
    /// <item><description>Ne duplique aucun contrôle d'état porté par les Services métier (état de la barre, état des découpes, état de la chute, longueur du motif, validité du contexte applicatif) ; seule la présence du motif est contrôlée avant toute écriture, parce qu'elle conditionne le geste même du refus.</description></item>
    /// <item><description>Ne contrôle pas la famille du motif, dont la pertinence relève de la liste présentée.</description></item>
    /// <item><description>N'agit pas sur le contexte de sélection et ne pose aucun indicateur d'approvisionnement de la série.</description></item>
    /// <item><description>Ne libère pas la chute source et ne modifie pas sa réservation : elle est retirée définitivement du stock.</description></item>
    /// <item><description>Ne modifie pas le nombre de découpes réalisées sur la barre, aucune coupe n'ayant eu lieu.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository (I-4.14.4 amendée, I-4.14.6) et ne requalifie aucune exception.</description></item>
    /// <item><description>N'expose aucun type technique de persistance par son contrat.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_BarRefusal"/>
    public class UC_BarRefusal : IU_BarRefusal
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IQ_Generic<ProductionBar> _queryBar;
        private readonly IS_ProductionBar_Reject _barReject;
        private readonly IS_ProductionCutPiece_DetachAndRefuse _pieceDetachRefuse;
        private readonly IS_CuttingScrapStock_Withdraw _scrapWithdraw;
        private readonly IS_LifecycleAction_Add _lifecycleAdd;
        private readonly IS_AppContext _appContext;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_BarRefusal"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la stratégie d'exécution, de la transaction et de l'enregistrement du scénario.</param>
        /// <param name="queryBar">Query Handler générique des barres de production, pour la lecture de l'origine de la barre refusée.</param>
        /// <param name="barReject">Service métier de mise à l'écart motivée de la barre de production.</param>
        /// <param name="pieceDetachRefuse">Service métier de libération sur refus des découpes rattachées à la barre.</param>
        /// <param name="scrapWithdraw">Service métier de retrait définitif d'une chute du stock.</param>
        /// <param name="lifecycleAdd">Service métier d'inscription d'une entrée au journal métier.</param>
        /// <param name="appContext">Service d'accès au contexte applicatif, requis par l'inscription au journal métier.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_BarRefusal(
            DbContext dbContext,
            IQ_Generic<ProductionBar> queryBar,
            IS_ProductionBar_Reject barReject,
            IS_ProductionCutPiece_DetachAndRefuse pieceDetachRefuse,
            IS_CuttingScrapStock_Withdraw scrapWithdraw,
            IS_LifecycleAction_Add lifecycleAdd,
            IS_AppContext appContext,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _queryBar = queryBar ?? throw new ArgumentNullException(nameof(queryBar));
            _barReject = barReject ?? throw new ArgumentNullException(nameof(barReject));
            _pieceDetachRefuse = pieceDetachRefuse ?? throw new ArgumentNullException(nameof(pieceDetachRefuse));
            _scrapWithdraw = scrapWithdraw ?? throw new ArgumentNullException(nameof(scrapWithdraw));
            _lifecycleAdd = lifecycleAdd ?? throw new ArgumentNullException(nameof(lifecycleAdd));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Annule intégralement, en une transaction unique, le placement provisoire de la barre de
        /// production refusée par l'opérateur, et inscrit le refus au journal métier avec son
        /// motif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Séquence, intégralement contenue dans le délégué de la stratégie d'exécution et dans
        /// une transaction en isolation <c>ReadCommitted</c> : contrôle de l'identifiant de barre ;
        /// contrôle de la présence du motif ; obtention du contexte applicatif ; lecture non suivie
        /// de la barre, dont l'absence est rejetée ; pour une barre de chute, contrôle de sa chute
        /// source ; mise à l'écart de la barre avec son motif ; libération de ses découpes ;
        /// retrait de la chute source si la barre en est issue ; inscription du refus au journal
        /// métier ; enregistrement unique et validation de la transaction.
        /// </para>
        /// <para>
        /// La lecture de la barre est non suivie : elle ne sert qu'au branchement sur l'origine de
        /// la barre, le Service de mise à l'écart effectuant sa propre lecture suivie. Le motif est
        /// transmis tel que reçu, sans rognage ni normalisation.
        /// </para>
        /// <para>
        /// Chaque exception typée est annulée puis confiée à <c>IU_LogAndNotify</c> (clés
        /// <c>No_EC_01</c>, <c>No_EC_02</c>, <c>No_EC_03</c>). L'annulation coopérative est
        /// propagée sans journalisation ; la transaction est alors annulée par la libération du
        /// bloc <c>await using</c>.
        /// </para>
        /// <para>
        /// Comportement au rejeu : une défaillance transitoire survenant lors de l'enregistrement
        /// n'est pas typée ; elle sort du délégué et la stratégie d'exécution le rejoue en entier,
        /// sous une nouvelle transaction. Le contexte partagé conserve toutefois l'état suivi de la
        /// tentative annulée : barre marquée supprimée, découpes détachées, chute retirée et entrée
        /// de journal ajoutée, en mémoire seulement. Sans neutralisation, la lecture suivie du
        /// Service de mise à l'écart retrouverait la barre déjà marquée supprimée et la rejetterait
        /// à tort. Chaque tentative postérieure à la première commence donc par vider le suivi des
        /// changements ; elle repart ainsi d'un état équivalent à celui de la première tentative,
        /// l'état de la base ayant été rétabli par l'annulation de la transaction. La première
        /// tentative n'altère aucun état suivi antérieur à l'invocation.
        /// </para>
        /// <para>
        /// Limite : si la validation de la transaction aboutit côté serveur alors que son
        /// acquittement est perdu, le rejeu relit une barre déjà refusée ; le Service de mise à
        /// l'écart la rejette et le retour vaut <see langword="false"/>, alors que le refus est
        /// persisté. Aucune donnée n'est corrompue, et la barre n'est plus proposée.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production refusée. Doit être strictement positif ; contrôlé dans le bloc transactionnel.</param>
        /// <param name="rejectionReason">Motif du refus. Ne doit être ni nul, ni vide, ni composé uniquement d'espaces ; contrôlé dans le bloc transactionnel et transmis sans altération. Sa longueur est contrôlée par le Service de mise à l'écart.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, vaut : <see langword="true"/>
        /// lorsque le refus est effectué, après validation de la transaction ;
        /// <see langword="false"/> sur échec applicatif typé, après annulation de la transaction et
        /// délégation à <c>IU_LogAndNotify</c>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/> (§4.6).
        /// </exception>
        public async Task<bool> ExecuteAsync(
            string caller,
            int idProductionBar,
            string rejectionReason,
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
            return await strategy.ExecuteAsync<bool>(async () =>
            {
                await using var transaction = await _dbContext.Database
                    .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

                // Neutralisation de l'état suivi laissé par une tentative annulée : sur un
                // rejeu, la barre marquée supprimée en mémoire serait retrouvée par l'identity
                // map et rejetée à tort. La première tentative n'est pas concernée.
                if (attemptCount++ > 0)
                    _dbContext.ChangeTracker.Clear();

                try
                {
                    // Étape 1 — Précondition structurelle sur la barre, dans le try afin
                    // d'être captée par le patron canonique.
                    if (idProductionBar <= 0)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_02,
                            $"Identifiant de barre de production invalide ({idProductionBar}) : une valeur strictement positive est attendue.");

                    // Étape 2 — Présence du motif, contrôlée avant toute écriture : elle
                    // conditionne le geste même du refus.
                    if (string.IsNullOrWhiteSpace(rejectionReason))
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_01,
                            $"Motif de refus absent pour la barre de production {idProductionBar} : un motif non vide est obligatoire.");

                    // Étape 3 — Contexte applicatif, requis par l'inscription au journal métier.
                    DTO_AppContext appContext = _appContext.GetAppContext();

                    // Étape 4 — Lecture non suivie de la barre, pour le seul branchement sur
                    // son origine ; le Service de mise à l'écart effectue sa propre lecture
                    // suivie.
                    ProductionBar? bar = await _queryBar.HandleGetByIdAsNoTrackingAsync(
                        callChain, idProductionBar, ct);

                    if (bar is null)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_03,
                            $"Barre de production introuvable (Id = {idProductionBar}).");

                    // Étape 5 — Chute source d'une barre de chute, par filtrage par motif :
                    // aucun accès .Value, dont l'échec non typé échapperait au patron.
                    int? idScrap = null;
                    if (!bar.IsNewBar)
                    {
                        if (bar.IdSourceScrap is not int sourceScrap || sourceScrap <= 0)
                            throw new Ex_Business(
                                callChain,
                                Ex_Business.ErrorCodes.BU_ER_04,
                                $"Chute source absente ou invalide ({bar.IdSourceScrap?.ToString() ?? "null"}) sur la barre de chute {idProductionBar} : données persistées incohérentes.");

                        idScrap = sourceScrap;
                    }

                    // Étape 6 — Mise à l'écart de la barre, motif transmis sans altération.
                    await _barReject.ExecuteAsync(callChain, idProductionBar, rejectionReason, ct);

                    // Étape 7 — Libération sur refus des découpes rattachées à la barre.
                    await _pieceDetachRefuse.ExecuteAsync(callChain, idProductionBar, ct);

                    // Étape 8 — Retrait définitif de la chute source, sur barre de chute
                    // uniquement ; sa réservation n'est pas modifiée.
                    if (idScrap is int scrapId)
                        await _scrapWithdraw.ExecuteAsync(callChain, scrapId, ct);

                    // Étape 9 — Inscription du refus au journal métier, le motif servant de
                    // commentaire et de seul discriminant de la nature d'action.
                    await _lifecycleAdd.ExecuteAsync(
                        callChain,
                        appContext,
                        En_LifecycleActionSource.ProductionBar,
                        En_LifecycleActionType.BarRefused,
                        idProductionBar,
                        rejectionReason,
                        ct);

                    // Étape 10 — Enregistrement unique, puis validation de la transaction
                    // (§4.10.4).
                    await _dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    // Issue nominale restituée à la présentation (R-4.14.22).
                    return true;
                }
                catch (Ex_Business ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
                    return false;
                }
                catch (Ex_Infrastructure ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
                    return false;
                }
                catch (Ex_Unclassified ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
                    return false;
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

        // A compléter

        #endregion
    }
}