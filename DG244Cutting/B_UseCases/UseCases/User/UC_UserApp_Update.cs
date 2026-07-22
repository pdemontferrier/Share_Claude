using System.Data;
using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// UseCase de modification d'un compte utilisateur <see cref="UserApp"/> depuis la page
    /// d'administration (chemin « modification », bouton Enregistrer de Page04).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : maillon 2 de la chaîne (1) d'écriture stricte VM → UC → SR → CH → CR posée en
    /// §4.14.9 et indexée par R-4.14.19. Seul composant de la chaîne autorisé à ouvrir, valider
    /// et annuler la transaction (R-4.10.1) et à appeler <c>SaveChangesAsync</c> (§4.10.4).
    /// Encapsulation du bloc transactionnel dans <c>CreateExecutionStrategy().ExecuteAsync</c>
    /// exigée par <c>EnableRetryOnFailure</c> activé au câblage de <c>DigitTryDbContext</c> dans
    /// <c>SR_ConteneurDI</c> (R-4.10.2 amendée). Les interactions UI du bloc préparatoire
    /// (notification bloquante, confirmations souples) sont placées HORS du délégué de la
    /// stratégie d'exécution, afin de ne pas être rejouées en cas de réexécution.
    /// </para>
    /// <para>
    /// Objectif : orchestrer le scénario unitaire de modification d'un enregistrement
    /// <c>UserApp</c> existant de manière transactionnelle et traçable, en concentrant la
    /// logique métier propre au chemin modification (dérivation des initiales, deux contrôles
    /// souples avec confirmation, contrôle d'immuabilité de la clé métier <c>Login</c>, hachage
    /// conditionnel du mot de passe), en déléguant l'écriture au Service <c>IS_UserApp_Update</c>
    /// et le traitement terminal des erreurs au pipeline <c>IU_LogAndNotify</c>.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire et propager la CallChain en début de scénario, conformément à §4.5.</description></item>
    /// <item><description>Contrôler les préconditions bloquantes non exceptionnelles (identité, logins) via le canal « abandon utilisateur », hors du délégué.</description></item>
    /// <item><description>Dériver les <c>Initials</c> et conduire les deux contrôles souples de composition avec confirmation utilisateur.</description></item>
    /// <item><description>Ouvrir, valider ou annuler la transaction d'écriture, sous stratégie d'exécution (R-4.10.2 amendée).</description></item>
    /// <item><description>Vérifier l'immuabilité de la clé métier <c>Login</c> par rapport à l'état persisté, hacher conditionnellement le mot de passe, puis déléguer l'écriture au Service <c>IS_UserApp_Update</c>.</description></item>
    /// <item><description>Persister la mutation via <c>SaveChangesAsync</c> et déléguer le traitement terminal des erreurs à <c>IU_LogAndNotify</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne construit pas la copie de travail <c>UserApp</c> : rôle du ViewModel <c>VM_Page04</c>.</description></item>
    /// <item><description>N'écrit pas directement en base : l'écriture est déléguée au Service en aval.</description></item>
    /// <item><description>Ne vérifie pas l'unicité du <c>Login</c> : le chemin modification ne recherche aucune collision avec un autre compte ; il contrôle uniquement que le <c>Login</c> soumis n'a pas muté par rapport à l'état persisté.</description></item>
    /// <item><description>Ne re-valide pas les champs obligatoires autres que <c>FirstName</c>, <c>LastName</c>, <c>Login</c> et <c>WindowsLogin</c> : les autres sont portés par le Service (<c>BU_ER_01</c>).</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler ni un Repository (I-4.14.4 amendée, I-4.14.6).</description></item>
    /// <item><description>Ne positionne pas les champs d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) : centralisé dans <c>CH_Generic&lt;T&gt;</c>.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_UserApp_Update"/>
    public class UC_UserApp_Update : IU_UserApp_Update
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly DbContext _dbContext;
        private readonly IS_UserApp_Update _updateService;
        private readonly IS_Hashing _hashing;
        private readonly IS_Notification _notification;
        private readonly IQ_UserApp _queryUserApp;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_UserApp_Update"/> avec ses dépendances.
        /// </summary>
        /// <param name="dbContext">Contexte de persistance partagé, support de la transaction du scénario.</param>
        /// <param name="updateService">Service métier d'écriture d'un compte utilisateur existant.</param>
        /// <param name="hashing">Service de hachage du mot de passe.</param>
        /// <param name="notification">Service de notification (préconditions bloquantes et confirmations souples).</param>
        /// <param name="queryUserApp">Query Handler de lecture de l'état persisté pour le contrôle d'immuabilité du <c>Login</c>.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_UserApp_Update(
            DbContext dbContext,
            IS_UserApp_Update updateService,
            IS_Hashing hashing,
            IS_Notification notification,
            IQ_UserApp queryUserApp,
            IU_LogAndNotify logAndNotify)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            _hashing = hashing ?? throw new ArgumentNullException(nameof(hashing));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
            _queryUserApp = queryUserApp ?? throw new ArgumentNullException(nameof(queryUserApp));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Orchestre la modification d'un enregistrement <c>UserApp</c> existant à partir de la
        /// copie de travail <paramref name="entity"/> et du mot de passe en clair
        /// <paramref name="plainPassword"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Bloc préparatoire (hors du délégué de la stratégie d'exécution) : préconditions
        /// bloquantes sur <c>FirstName</c>/<c>LastName</c> (clé <c>No_St_03</c>) puis sur
        /// <c>Login</c>/<c>WindowsLogin</c> (clé <c>No_St_02</c>) ; dérivation des <c>Initials</c> ;
        /// deux contrôles souples avec confirmation utilisateur (composition du <c>Login</c>,
        /// clé <c>No_AD_02</c> ; égalité <c>WindowsLogin</c> = <c>Login</c>, clé <c>No_AD_03</c>).
        /// Toute entrée invalide ou tout refus emprunte le canal « abandon utilisateur »
        /// (notification suivie d'un return silencieux), sans levée d'exception.
        /// </para>
        /// <para>
        /// Bloc transactionnel (dans le délégué) : contrôle d'immuabilité de la clé métier
        /// <c>Login</c> par lecture <c>NoTracking</c> de l'état persisté via
        /// <see cref="IQ_Generic{T}.HandleGetByIdAsNoTrackingAsync"/> (état introuvable → levée
        /// <see cref="Ex_Business"/> <c>BU_ER_03</c> ; <c>Login</c> muté → levée
        /// <see cref="Ex_Business"/> <c>BU_ER_03</c>) ; hachage conditionnel du mot de passe
        /// (uniquement si un nouveau secret est saisi) ; délégation de l'écriture au Service ;
        /// persistance via <c>SaveChangesAsync</c> ; validation de la transaction. Toute
        /// exception applicative typée est traitée terminalement par <c>IU_LogAndNotify</c> et
        /// n'est pas propagée.
        /// </para>
        /// <para>
        /// Précondition d'entrée sur le mot de passe : lorsque <paramref name="plainPassword"/>
        /// est vide ou blanc, le <c>PasswordHash</c> existant porté par <paramref name="entity"/>
        /// est conservé sans re-hachage ; la copie de travail doit alors déjà porter ce hash,
        /// chargé depuis la base par le ViewModel appelant. À défaut, le Service
        /// <c>IS_UserApp_Update</c> lèvera <see cref="Ex_Business"/> (<c>BU_ER_01</c>).
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="entity">Copie de travail du compte à modifier ; <c>Initials</c> est positionné par le scénario et <c>PasswordHash</c> n'est recalculé que si un nouveau secret est saisi. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="plainPassword">Mot de passe en clair. S'il est renseigné, il est haché avant délégation et jamais persisté en clair ; s'il est vide ou blanc, le mot de passe est réputé inchangé.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Une tâche dont le résultat est l'issue de mutation restituée à la présentation :
        /// <see cref="En_ChangeResult.Changed"/> en clôture du chemin nominal, après
        /// <c>CommitAsync</c> ; <see cref="En_ChangeResult.Unchanged"/> sur tout chemin terminal
        /// non nominal — abandons utilisateur (clés <c>No_St_03</c>, <c>No_St_02</c>,
        /// <c>No_AD_02</c>, <c>No_AD_03</c>) et catch typés applicatifs (clés <c>No_EC_01</c> /
        /// <c>No_EC_02</c> / <c>No_EC_03</c>), après <c>RollbackAsync</c> et délégation à
        /// <c>IU_LogAndNotify</c>. <see cref="OperationCanceledException"/> reste propagée sans
        /// valeur (§4.6).
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6.
        /// </exception>
        public async Task<En_ChangeResult> ExecuteAsync(
            string caller,
            UserApp entity,
            string plainPassword,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // ===================================================================
            // Bloc préparatoire — HORS du délégué de la stratégie d'exécution.
            // Les interactions UI (Stop, ConfirmationReturn) ne doivent pas être
            // rejouées en cas de réexécution par la stratégie. Aucune Ex_Business
            // n'est levée ici : les entrées invalides et les refus utilisateur
            // empruntent le canal « abandon utilisateur » (notification + return
            // silencieux), distinct du canal Ex_Business (réservé au bloc
            // transactionnel).
            // ===================================================================

            // Précondition bloquante #1 — FirstName / LastName obligatoires.
            // Placée AVANT la dérivation des Initials et la composition du Login
            // attendu, qui indexent ces champs : garantit qu'aucune indexation ne
            // peut lever hors du try du délégué.
            if (string.IsNullOrWhiteSpace(entity.FirstName)
                || string.IsNullOrWhiteSpace(entity.LastName))
            {
                _notification.Stop(callChain, "No_St_03", null, ct);
                return En_ChangeResult.Unchanged;
            }

            // Précondition bloquante #2 — Login / WindowsLogin obligatoires.
            // Garantit en outre la non-nullité de WindowsLogin (nullable au schéma)
            // avant la comparaison du contrôle souple #2.
            if (string.IsNullOrWhiteSpace(entity.Login)
                || string.IsNullOrWhiteSpace(entity.WindowsLogin))
            {
                _notification.Stop(callChain, "No_St_02", null, ct);
                return En_ChangeResult.Unchanged;
            }

            // Dérivation des Initials : initiale du prénom + initiale du nom, en
            // majuscules. FirstName et LastName sont garantis non vides (précond. #1).
            entity.Initials =
                $"{entity.FirstName[0]}{entity.LastName[0]}".ToUpperInvariant();

            // Contrôle souple #1 — composition attendue du Login : initiale du prénom
            // suivie des sept premières lettres du nom (moins si le nom est plus court),
            // en minuscules. Si le Login saisi diffère, l'utilisateur confirme sa
            // conservation ou abandonne.
            string expectedLogin =
                $"{entity.FirstName[0]}{entity.LastName.Substring(0, Math.Min(7, entity.LastName.Length))}"
                    .ToLowerInvariant();

            if (!string.Equals(entity.Login, expectedLogin, StringComparison.Ordinal))
            {
                if (!_notification.ConfirmationReturn(callChain, "No_AD_02", null, ct))
                    return En_ChangeResult.Unchanged;
            }

            // Contrôle souple #2 — WindowsLogin = Login. WindowsLogin est garanti non
            // nul (précond. #2).
            if (!string.Equals(entity.WindowsLogin, entity.Login, StringComparison.Ordinal))
            {
                if (!_notification.ConfirmationReturn(callChain, "No_AD_03", null, ct))
                    return En_ChangeResult.Unchanged;
            }

            // ===================================================================
            // Bloc transactionnel — DANS le délégué de la stratégie d'exécution.
            // Encapsulation exigée par EnableRetryOnFailure (R-4.10.2 amendée) : sans
            // elle, BeginTransactionAsync lève InvalidOperationException.
            // ===================================================================
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database
                    .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

                try
                {
                    // Contrôle d'immuabilité de la clé métier Login (propre au chemin
                    // Modification). Lecture NoTracking imposée : l'entité de contrôle ne
                    // doit PAS être suivie par le DbContext, sous peine de conflit d'identité
                    // EF Core avec la copie de travail que le Service attachera sur ce même
                    // DbContext. Placé DANS le délégué, en première position, pour que
                    // l'Ex_Business soit captée par le catch (Ex_Business) ; relecture
                    // idempotente au rejeu (lecture pure NoTracking).
                    UserApp? persisted =
                        await _queryUserApp.HandleGetByIdAsNoTrackingAsync(callChain, entity.Id, ct);

                    // Id introuvable en base : l'enregistrement cible n'existe pas.
                    if (persisted is null)
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_03,
                            "Enregistrement cible introuvable.");

                    // Mutation interdite de la clé métier : le Login d'un compte existant
                    // ne peut être modifié.
                    if (!string.Equals(entity.Login, persisted.Login, StringComparison.Ordinal))
                        throw new Ex_Business(
                            callChain,
                            Ex_Business.ErrorCodes.BU_ER_03,
                            "Le Login ne peut être modifié.");

                    // Hachage conditionnel du mot de passe : seul un nouveau secret saisi
                    // déclenche le re-hachage. Si plainPassword est vide ou blanc, le
                    // PasswordHash existant porté par la copie de travail est conservé
                    // (précondition d'entrée à charge du ViewModel appelant). Hash est un
                    // calcul pur, idempotent au rejeu.
                    if (!string.IsNullOrWhiteSpace(plainPassword))
                        entity.PasswordHash = _hashing.Hash(plainPassword);

                    // Délégation de l'écriture au Service (chaîne (1) maillon 3 :
                    // VM → UC → SR → CH → CR). UpdatedAt est positionné par IC_Generic.
                    await _updateService.ExecuteAsync(callChain, entity, ct);

                    // Persistance unifiée de la mutation et de l'enregistrement Event Store
                    // (R-4.10.1, R-4.10.4 ; SaveChangesAsync exclusif au UseCase).
                    await _dbContext.SaveChangesAsync(ct);

                    // Validation de la transaction (R-4.10.4).
                    await transaction.CommitAsync(ct);

                    // Issue nominale restituée à la présentation (R-4.14.22) : la persistance
                    // a été effectivement modifiée, le consommateur doit rafraîchir son état.
                    return En_ChangeResult.Changed;
                }
                catch (Ex_Business ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
                    return En_ChangeResult.Unchanged;
                }
                catch (Ex_Infrastructure ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
                    return En_ChangeResult.Unchanged;
                }
                catch (Ex_Unclassified ex)
                {
                    await transaction.RollbackAsync(ct);
                    await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
                    return En_ChangeResult.Unchanged;
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

        // Aucune méthode privée requise pour le scénario courant.

        #endregion
    }
}