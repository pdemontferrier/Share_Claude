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
    /// UseCase de positionnement de l'état de rupture de stock d'une barre de production neuve :
    /// orchestration transactionnelle du marquage de la barre, de ses découpes, du recalcul de
    /// l'indicateur de série et, pour une mise en attente, de l'inscription au journal métier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : maillon 2 de la chaîne (1) d'écriture stricte VM → UC → SR → CH → CR (§4.14.9,
    /// R-4.14.19), consommé en chaîne directe par la présentation via <c>IS_UseCaseInvoker</c>.
    /// Seul composant de la chaîne autorisé à ouvrir, valider et annuler la transaction
    /// (R-4.10.1) et à appeler <c>SaveChangesAsync</c>. Le bloc transactionnel est encapsulé dans
    /// <c>CreateExecutionStrategy().ExecuteAsync</c>, exigé par la réexécution sur échec activée
    /// au câblage du contexte de persistance (§4.10.1).
    /// </para>
    /// <para>
    /// Objectif : mettre en attente une barre dont la matière est absente de l'atelier, ou lever
    /// cette attente, sans rien défaire de ce que l'optimisation a posé. Les deux sens écrivent les
    /// mêmes champs avec des valeurs opposées et enchaînent les mêmes Services dans le même ordre ;
    /// seule la mise en attente est inscrite au journal métier, la libération constituant un retour
    /// à l'état normal. Toutes les écritures sont validées ou annulées ensemble, en un
    /// enregistrement unique.
    /// </para>
    /// <para>
    /// L'indicateur de rupture de la série est recalculé par le Service qui en est l'écrivain
    /// unique, appelé dans la transaction du UseCase, sans seconde transaction (I-4.10.3). Aucun
    /// enregistrement intermédiaire n'est nécessaire : ce Service lit les barres de la série en
    /// mode suivi, filtrées sur l'identifiant de série, colonne que la séquence ne modifie pas, et
    /// évalue leur état en mémoire ; la résolution d'identité lui restitue l'instance de la barre
    /// déjà modifiée dans le contexte partagé.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire et propager la CallChain en début de scénario, conformément à §4.5.</description></item>
    /// <item><description>Ouvrir, valider ou annuler la transaction d'écriture, sous stratégie d'exécution, et rendre chaque tentative de la stratégie indépendante de l'état laissé par une tentative annulée.</description></item>
    /// <item><description>Valider les préconditions structurelles du scénario : identifiant de barre strictement positif et existence de la barre, dont l'identifiant de série est exploité.</description></item>
    /// <item><description>Déléguer, dans cet ordre, le marquage de la barre, le marquage de ses découpes, le recalcul de l'indicateur de série et, pour une mise en attente uniquement, l'inscription au journal métier.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs typées à <c>IU_LogAndNotify</c> et restituer à la présentation un retour interprétable.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne duplique aucun contrôle d'état de la barre, des découpes ou de la série : ces contrôles sont portés par les Services métier en aval.</description></item>
    /// <item><description>Ne modifie jamais l'instance de barre qu'il lit : sa lecture est non suivie et ne sert qu'à établir l'existence de la barre et son identifiant de série.</description></item>
    /// <item><description>N'altère pas le plan de coupe et n'agit sur aucune chute du stock.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository (I-4.14.4 amendée, I-4.14.6) et ne requalifie aucune exception.</description></item>
    /// <item><description>Ne modifie pas le contexte de sélection de la barre et ne décide pas de la suite du parcours.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_ProductionBar_SetOutOfStock"/>
    public class UC_ProductionBar_SetOutOfStock : IU_ProductionBar_SetOutOfStock
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IS_AppContext _appContext;
        private readonly IQ_Generic<ProductionBar> _queryBar;
        private readonly IS_ProductionBar_SetOutOfStock _barSetOutOfStock;
        private readonly IS_ProductionCutPiece_SetBarOutOfStock _pieceSetOutOfStock;
        private readonly IS_ProductionSeries_SetBarOutOfStockFlag _seriesFlag;
        private readonly IS_LifecycleAction_Add _lifecycleAdd;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_ProductionBar_SetOutOfStock"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la stratégie d'exécution, de la transaction et de l'enregistrement du scénario.</param>
        /// <param name="appContext">Service d'accès au contexte applicatif courant, transmis au journal métier lors d'une mise en attente.</param>
        /// <param name="queryBar">Query Handler générique des barres de production, pour la lecture non suivie de la barre visée.</param>
        /// <param name="barSetOutOfStock">Service métier de positionnement de l'état de rupture de la barre.</param>
        /// <param name="pieceSetOutOfStock">Service métier de répercussion de l'état de rupture sur les découpes rattachées à la barre.</param>
        /// <param name="seriesFlag">Service métier de recalcul de l'indicateur de rupture de la série.</param>
        /// <param name="lifecycleAdd">Service métier d'inscription d'une entrée au journal métier.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_ProductionBar_SetOutOfStock(
            DbContext dbContext,
            IS_AppContext appContext,
            IQ_Generic<ProductionBar> queryBar,
            IS_ProductionBar_SetOutOfStock barSetOutOfStock,
            IS_ProductionCutPiece_SetBarOutOfStock pieceSetOutOfStock,
            IS_ProductionSeries_SetBarOutOfStockFlag seriesFlag,
            IS_LifecycleAction_Add lifecycleAdd,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _queryBar = queryBar ?? throw new ArgumentNullException(nameof(queryBar));
            _barSetOutOfStock = barSetOutOfStock ?? throw new ArgumentNullException(nameof(barSetOutOfStock));
            _pieceSetOutOfStock = pieceSetOutOfStock ?? throw new ArgumentNullException(nameof(pieceSetOutOfStock));
            _seriesFlag = seriesFlag ?? throw new ArgumentNullException(nameof(seriesFlag));
            _lifecycleAdd = lifecycleAdd ?? throw new ArgumentNullException(nameof(lifecycleAdd));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Positionne l'état de rupture de stock de la barre de production désignée, le répercute
        /// sur ses découpes, recalcule l'indicateur de sa série et, pour une mise en attente,
        /// l'inscrit au journal métier, en une transaction unique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Séquence, intégralement contenue dans le délégué de la stratégie d'exécution et dans
        /// une transaction en isolation <c>ReadCommitted</c> : contrôle de l'identifiant de barre ;
        /// lecture non suivie de la barre, dont l'absence est rejetée ; positionnement de l'état de
        /// la barre ; répercussion sur ses découpes ; recalcul de l'indicateur de la série lue sur
        /// la barre ; pour une mise en attente uniquement, lecture du contexte applicatif et
        /// inscription au journal métier ; enregistrement unique et validation de la transaction.
        /// </para>
        /// <para>
        /// Chaque exception typée est annulée puis confiée à <c>IU_LogAndNotify</c> (clés
        /// <c>No_EC_01</c>, <c>No_EC_02</c>, <c>No_EC_03</c>), le journal d'erreurs écrivant dans un
        /// contexte de persistance dédié. L'annulation coopérative est propagée sans
        /// journalisation ; la transaction est alors annulée par la libération du bloc
        /// <c>await using</c>. Aucune exception non typée n'est captée : une défaillance de
        /// persistance non transitoire, ou l'épuisement des réexécutions, remonte au consommateur,
        /// où elle est captée par le filet de sécurité <c>VM_Generic.ExecuteSafeAsync</c>. Les
        /// entités modifiées restant suivies après une annulation ne sont pas nettoyées : la portée
        /// d'injection est propre à chaque invocation.
        /// </para>
        /// <para>
        /// Comportement au rejeu : une défaillance transitoire survenant lors de l'enregistrement
        /// n'est pas typée ; elle sort du délégué et la stratégie d'exécution le rejoue en entier,
        /// sous une nouvelle transaction. Le contexte partagé conserve toutefois les modifications
        /// suivies de la tentative annulée : le Service de marquage y retrouverait la barre déjà
        /// positionnée dans l'état demandé et rejetterait l'opération à tort, alors que la base a
        /// été rétablie. Chaque tentative postérieure à la première commence donc par vider le suivi des
        /// changements ; elle repart ainsi d'un état équivalent à celui de la première tentative,
        /// l'état de la base ayant été rétabli par l'annulation de la transaction.
        /// </para>
        /// <para>
        /// Limite : si la validation de la transaction aboutit côté serveur alors que son
        /// acquittement est perdu, le rejeu repart de l'état persisté ; la barre étant déjà dans
        /// l'état demandé, le Service de marquage rejette l'opération et le retour vaut
        /// <see langword="false"/>, bien que les écritures aient été validées.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionBar">Identifiant de la barre de production visée. Doit être strictement positif ; contrôlé dans le bloc transactionnel.</param>
        /// <param name="isOutOfStock">Valeur d'état à positionner : <see langword="true"/> pour une mise en attente, <see langword="false"/> pour une libération.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, vaut <see langword="true"/> après
        /// validation de la transaction, et <see langword="false"/> sur échec applicatif typé, après
        /// annulation de la transaction et délégation à <c>IU_LogAndNotify</c>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/> (§4.6).
        /// </exception>
        public async Task<bool> ExecuteAsync(
            string caller,
            int idProductionBar,
            bool isOutOfStock,
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
                // rejeu, les Services retrouveraient les instances déjà modifiées par la
                // tentative précédente, alors que la base a été rétablie. La première
                // tentative n'est pas concernée.
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

                    // Étape 2 — Lecture NON SUIVIE (R-4.10.10) : l'instance n'est jamais
                    // modifiée ; seul son identifiant de série est exploité.
                    ProductionBar? bar = await _queryBar.HandleGetByIdAsNoTrackingAsync(
                        callChain, idProductionBar, ct);

                    if (bar is null)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_03,
                            $"La barre de production désignée est introuvable ; identifiant introuvable : {idProductionBar}.");

                    // Étape 3 — Positionnement de l'état de la barre ; les contrôles d'état
                    // sont portés par le Service.
                    await _barSetOutOfStock.ExecuteAsync(callChain, idProductionBar, isOutOfStock, ct);

                    // Étape 4 — Répercussion sur les découpes rattachées.
                    await _pieceSetOutOfStock.ExecuteAsync(callChain, idProductionBar, isOutOfStock, ct);

                    // Étape 5 — Recalcul de l'indicateur de la série.
                    // AUCUN SaveChangesAsync intermédiaire, et c'est voulu : le Service filtre
                    // les barres sur IdProductionSeries, colonne non modifiée par la séquence,
                    // puis évalue IsOutOfStock en mémoire. La résolution d'identité lui
                    // restitue l'instance suivie modifiée à l'étape 3 (lecture suivie par
                    // FindAsync dans CR_Generic).
                    await _seriesFlag.ExecuteAsync(callChain, bar.IdProductionSeries, ct);

                    // Étape 6 — Journal métier, sur mise en attente uniquement ; le contexte
                    // applicatif est lu au point d'usage.
                    if (isOutOfStock)
                    {
                        DTO_AppContext appContext = _appContext.GetAppContext();

                        await _lifecycleAdd.ExecuteAsync(
                            callChain,
                            appContext,
                            En_LifecycleActionSource.ProductionBar,
                            En_LifecycleActionType.BarOutOfStock,
                            idProductionBar,
                            null,
                            ct);
                    }

                    // Étape 7 — Enregistrement unique, puis validation de la transaction
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

        // Aucune méthode privée.

        #endregion
    }
}