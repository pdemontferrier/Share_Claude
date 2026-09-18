using System.Data;
using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Business;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.Business;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.B_UseCases.UseCases.Business
{
    /// <summary>
    /// UseCase de clôture des découpes d'une série de production : constat transactionnel de
    /// l'absence de découpe restante, puis pose de l'indicateur d'achèvement, levée des
    /// réservations de chutes et inscription au journal métier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : maillon 2 de la chaîne (1) d'écriture stricte VM → UC → SR → CH → CR (§4.14.9,
    /// R-4.14.19), consommé en chaîne directe par <c>VM_Page20</c> lorsque la séquence d'entrée
    /// sur la page de découpe ne trouve plus aucune découpe optimisable. Seul composant de la
    /// chaîne autorisé à ouvrir, valider et annuler la transaction (R-4.10.1) et à appeler
    /// <c>SaveChangesAsync</c>. Le bloc transactionnel est encapsulé dans
    /// <c>CreateExecutionStrategy().ExecuteAsync</c>, exigé par la réexécution sur échec activée
    /// au câblage du contexte de persistance (§4.10.1).
    /// </para>
    /// <para>
    /// Objectif : l'épuisement de la recherche de matière admet deux lectures, que le UseCase
    /// départage. Si la série ne compte plus aucune découpe non coupée et non supprimée, elle est
    /// achevée : le UseCase pose son indicateur d'achèvement, rend au stock les chutes qu'elle
    /// avait réservées sans les consommer et inscrit l'événement au journal métier, chaque
    /// écriture étant déléguée au Service métier qui en porte la responsabilité. Sinon, les
    /// découpes restantes sont nécessairement bloquées par une rupture de stock, la recherche de
    /// matière excluant les découpes bloquées : aucune écriture n'a lieu et le retour maintient
    /// l'opérateur sur la page de découpe.
    /// </para>
    /// <para>
    /// Le test de clôture ignore délibérément l'état de rupture : une découpe bloquée reste une
    /// découpe à faire, ce qui interdit de clôturer une série dont la matière manque encore. Il
    /// s'exécute dans la transaction des écritures qu'il conditionne. La levée des réservations
    /// intervient à la clôture, seul moment où l'on sait avec certitude que la série ne
    /// consommera plus rien ; sans elle, une chute retenue par une barre mise en attente puis
    /// jamais libérée resterait inconnue du stock alors qu'elle est physiquement présente.
    /// </para>
    /// <para>
    /// Un enregistrement unique suffit : aucune des trois écritures ne dépend d'un identifiant
    /// attribué par la base.
    /// </para>
    /// <para>
    /// Écarts au document fonctionnel 0221, §4.2.3 : le test de clôture, que ce document place
    /// dans le ViewModel, est porté par le UseCase, car il constitue une règle métier et doit
    /// s'exécuter dans la transaction des écritures qu'il conditionne. La levée des réservations
    /// de chutes à la clôture, que ce document ne décrit pas alors qu'il en décrit la pose, est
    /// ajoutée aux effets de la clôture.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire et propager la CallChain en début de scénario, conformément à §4.5.</description></item>
    /// <item><description>Ouvrir, valider ou annuler la transaction d'écriture, sous stratégie d'exécution, et rendre chaque tentative de la stratégie indépendante de l'état laissé par une tentative annulée.</description></item>
    /// <item><description>Valider la précondition structurelle du scénario : identifiant de série strictement positif.</description></item>
    /// <item><description>Établir, dans la transaction, l'existence d'au moins une découpe non coupée et non supprimée de la série, sans égard à l'état de rupture.</description></item>
    /// <item><description>Sur une série achevée, déléguer la pose de l'indicateur d'achèvement, la levée des réservations de chutes et l'inscription au journal métier, puis enregistrer et valider l'ensemble.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs typées à <c>IU_LogAndNotify</c> et restituer à la présentation un retour interprétable.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne duplique pas les contrôles d'état de la série (existence, suppression logique, clôture antérieure), portés par le Service de clôture.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository (I-4.14.4 amendée, I-4.14.6) et ne requalifie aucune exception.</description></item>
    /// <item><description>Ne décide d'aucune navigation ni d'aucun état d'interface : l'orientation de l'opérateur relève du consommateur, à partir du retour.</description></item>
    /// <item><description>Ne renseigne pas la date de fin de production de la série.</description></item>
    /// <item><description>Ne libère aucune réservation de chute d'une série non clôturée et ne purge aucune série jamais clôturée.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_ProductionSeries_CompleteCutting"/>
    public class UC_ProductionSeries_CompleteCutting : IU_ProductionSeries_CompleteCutting
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IS_AppContext _appContext;
        private readonly IQ_VwProductionCutPieceFull _queryCutPiece;
        private readonly IS_ProductionSeries_CompleteCutting _seriesComplete;
        private readonly IS_CuttingScrapStock_ReleaseBySeries _scrapRelease;
        private readonly IS_LifecycleAction_Add _lifecycleAction;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_ProductionSeries_CompleteCutting"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la stratégie d'exécution, de la transaction et de l'enregistrement du scénario.</param>
        /// <param name="appContext">Service d'accès au contexte applicatif courant, transmis au journal métier lors d'une clôture.</param>
        /// <param name="queryCutPiece">Query Handler des découpes de série, pour le test d'existence d'une découpe restant à réaliser.</param>
        /// <param name="seriesComplete">Service métier de pose de l'indicateur d'achèvement des découpes de la série.</param>
        /// <param name="scrapRelease">Service métier de levée des réservations de chutes détenues par la série.</param>
        /// <param name="lifecycleAction">Service métier d'inscription d'une entrée au journal métier.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_ProductionSeries_CompleteCutting(
            DbContext dbContext,
            IS_AppContext appContext,
            IQ_VwProductionCutPieceFull queryCutPiece,
            IS_ProductionSeries_CompleteCutting seriesComplete,
            IS_CuttingScrapStock_ReleaseBySeries scrapRelease,
            IS_LifecycleAction_Add lifecycleAction,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _queryCutPiece = queryCutPiece ?? throw new ArgumentNullException(nameof(queryCutPiece));
            _seriesComplete = seriesComplete ?? throw new ArgumentNullException(nameof(seriesComplete));
            _scrapRelease = scrapRelease ?? throw new ArgumentNullException(nameof(scrapRelease));
            _lifecycleAction = lifecycleAction ?? throw new ArgumentNullException(nameof(lifecycleAction));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Statue sur l'achèvement des découpes de la série désignée et, si aucune découpe ne
        /// reste à réaliser, la clôture en une seule transaction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Séquence, intégralement contenue dans le délégué de la stratégie d'exécution et dans
        /// une transaction en isolation <c>ReadCommitted</c> : contrôle de l'identifiant de série ;
        /// lecture du contexte applicatif ; test d'existence d'une découpe de la série non coupée
        /// et non supprimée, rupture de stock comprise, dont le résultat positif clôt le scénario
        /// sans écriture ; pose de l'indicateur d'achèvement ; levée des réservations de chutes ;
        /// inscription au journal métier d'une entrée de source
        /// <see cref="En_LifecycleActionSource.ProductionSeries"/> et de type
        /// <see cref="En_LifecycleActionType.CuttingCompleted"/>, rattachée à l'identifiant de la
        /// série, sans commentaire ; enregistrement unique et validation de la transaction.
        /// </para>
        /// <para>
        /// La sortie sans écriture est clôturée par la validation d'une transaction vide ;
        /// l'annulation explicite est réservée au traitement des exceptions typées, chacune étant
        /// annulée puis confiée à <c>IU_LogAndNotify</c> (clés <c>No_EC_01</c>, <c>No_EC_02</c>,
        /// <c>No_EC_03</c>). L'annulation coopérative est propagée sans journalisation ; la
        /// transaction est alors annulée par la libération du bloc <c>await using</c>. Aucune
        /// exception non typée n'est captée : une défaillance de persistance non transitoire, ou
        /// l'épuisement des réexécutions, remonte au consommateur, où elle est captée par
        /// <c>VM_Generic.ExecuteSafeAsync</c>.
        /// </para>
        /// <para>
        /// Comportement au rejeu : une défaillance transitoire survenant lors de l'enregistrement
        /// n'est pas typée ; elle sort du délégué et la stratégie d'exécution le rejoue en entier,
        /// sous une nouvelle transaction. Le contexte partagé conserve toutefois l'état suivi de la
        /// tentative annulée : série marquée achevée, chutes libérées et entrée de journal en
        /// attente d'insertion. Sans neutralisation, un rejeu réécrirait ces modifications et
        /// insérerait une seconde entrée de journal. Chaque tentative postérieure à la première
        /// commence donc par vider le suivi des changements ; elle repart ainsi d'un état
        /// équivalent à celui de la première tentative, l'état de la base ayant été rétabli par
        /// l'annulation de la transaction. La première tentative n'altère aucun état suivi
        /// antérieur à l'invocation.
        /// </para>
        /// <para>
        /// Limites : seul le chemin de clôture exerce les contrôles d'état de la série ; une série
        /// supprimée logiquement qui conserve des découpes non coupées rend donc
        /// <see langword="false"/> sans erreur. Une série dont toutes les découpes non coupées
        /// sont supprimées passe le test et est clôturée ; ce cas relève de l'anomalie de données.
        /// Si la validation de la transaction aboutit côté serveur alors que son acquittement est
        /// perdu, le rejeu trouve la série déjà clôturée : le Service de clôture la rejette
        /// (<c>BU_ER_04</c>) et le retour vaut <see langword="null"/> avec notification, alors que
        /// la clôture est acquise. Aucune donnée n'est corrompue.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="idProductionSeries">Identifiant de la série de production sélectionnée. Doit être strictement positif ; contrôlé dans le bloc transactionnel.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, vaut :
        /// <see langword="true"/> lorsque la série est clôturée, après validation de la
        /// transaction ; <see langword="false"/> lorsque des découpes non coupées subsistent,
        /// toutes bloquées par une rupture de stock, sans écriture ; <see langword="null"/> sur
        /// échec applicatif typé, après annulation de la transaction et délégation à
        /// <c>IU_LogAndNotify</c>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/> (§4.6).
        /// </exception>
        public async Task<bool?> ExecuteAsync(
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

            return await strategy.ExecuteAsync<bool?>(async () =>
            {
                await using var transaction = await _dbContext.Database
                    .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

                // Neutralisation de l'état suivi laissé par une tentative annulée : sur un
                // rejeu, la série, les chutes et l'entrée de journal de la tentative précédente
                // seraient réécrites par l'enregistrement. La première tentative n'est pas
                // concernée.
                if (attemptCount++ > 0)
                    _dbContext.ChangeTracker.Clear();

                try
                {
                    // E1 — Précondition structurelle sur la série, dans le try afin d'être
                    // captée par le patron canonique.
                    if (idProductionSeries <= 0)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_02,
                            $"Identifiant de série de production invalide ({idProductionSeries}) : une valeur strictement positive est attendue.");

                    // E2 — Contexte applicatif, lu une fois par tentative.
                    DTO_AppContext ctx = _appContext.GetAppContext();

                    // E3 — Test de clôture, dans la transaction des écritures qu'il conditionne.
                    // Le prédicat n'exclut PAS la rupture de stock : une découpe bloquée reste
                    // une découpe à faire. La lecture d'existence est opérante sur la vue sans clé.
                    bool hasRemaining = await _queryCutPiece.HandleAnyByPredicateAsync(
                        callChain,
                        v => v.PSId == idProductionSeries
                            && !v.PCPIsCut
                            && !v.PCPIsDeleted,
                        ct);

                    // E4 — Série non achevée : sortie nominale sans écriture.
                    if (hasRemaining)
                    {
                        await transaction.CommitAsync(ct);
                        return false;
                    }

                    // E5 — Pose de l'indicateur d'achèvement ; les contrôles d'état de la série
                    // sont portés par le Service.
                    await _seriesComplete.ExecuteAsync(callChain, idProductionSeries, ct);

                    // E6 — Levée des réservations de chutes ; l'absence de chute réservée
                    // n'est pas une erreur.
                    await _scrapRelease.ExecuteAsync(callChain, idProductionSeries, ct);

                    // E7 — Action de cycle de vie, sans commentaire.
                    await _lifecycleAction.ExecuteAsync(
                        callChain,
                        ctx,
                        En_LifecycleActionSource.ProductionSeries,
                        En_LifecycleActionType.CuttingCompleted,
                        idProductionSeries,
                        null,
                        ct);

                    // E8 — Enregistrement unique, puis validation de la transaction (§4.10.4).
                    await _dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    // Issue nominale restituée à la présentation (R-4.14.22).
                    return true;
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

        // Aucune méthode privée.

        #endregion
    }
}