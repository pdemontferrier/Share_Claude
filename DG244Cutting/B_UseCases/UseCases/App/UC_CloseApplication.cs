using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.Infrastructure;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// Description :
    /// <para>Implémentation du UseCase d'orchestration de la procédure de fermeture
    /// de l'application DG244Cutting, contrat <see cref="IU_CloseApplication"/>.</para>
    /// </summary>
    /// <remarks>
    /// <para>Contexte : classe résidant en <c>B_UseCases/UseCases/App/</c>
    /// (domaine App, orchestration applicative transverse). Le UseCase est consommé
    /// par la couche de présentation (ViewModel ou code-behind de la fenêtre principale
    /// WPF) à l'occasion de l'événement <c>OnClosing</c>, et plus généralement par tout
    /// consommateur de présentation pilotant la fermeture applicative. Le scénario
    /// porté combine un contexte d'environnement (lu depuis <see cref="ISE_User"/> et
    /// <see cref="ISE_App"/>) et trois paramètres fonctionnels d'entrée
    /// (<c>confirmation</c>, <c>warning</c>, <c>delaySeconds</c>) fournis par le
    /// consommateur de présentation à chaque appel, qui pilotent la matrice à quatre
    /// modes hiérarchisée.</para>
    /// <para>Objectif : orchestrer la procédure de fermeture en distinguant
    /// trois issues fonctionnelles (annulation utilisateur, fermeture confirmée,
    /// fermeture forcée), restituées au consommateur de présentation via
    /// <see cref="En_CloseResult"/>.</para>
    /// <para>Matrice à quatre modes (priorité hiérarchique stricte) :</para>
    /// <list type="number">
    /// <item><description><c>confirmation == true</c> → mode confirmation (prime sur tout).</description></item>
    /// <item><description>sinon <c>delaySeconds &gt; 0</c> → mode delay.</description></item>
    /// <item><description>sinon <c>warning == true</c> → mode warning.</description></item>
    /// <item><description>sinon → mode direct.</description></item>
    /// </list>
    /// <para>Branche <c>ForceClose</c> : le flag
    /// <see cref="ISE_User.ForceClose"/> est lu en début de scénario et, s'il est à
    /// <see langword="true"/>, prime sur la matrice à quatre modes par priorité
    /// absolue. Selon l'état <see cref="ISE_App.IsConnected"/>, la branche
    /// <c>ForceClose</c> effectue soit la déconnexion de la session (cas connecté),
    /// soit une journalisation silencieuse (cas déconnecté), puis converge vers la
    /// fermeture applicative effective via <see cref="IS_Shutdown"/>. Retour
    /// <see cref="En_CloseResult.ForceClosed"/> dans les deux cas.</para>
    /// <para>Convergence PR-B : à l'exception du mode confirmation sur refus
    /// utilisateur (qui retourne <see cref="En_CloseResult.Cancelled"/> sans fermeture
    /// effective), tous les chemins fonctionnels — y compris la branche
    /// <c>ForceClose</c> — convergent vers <see cref="IS_Shutdown.ExecuteAsync"/>,
    /// uniformisant le pilotage WPF côté D_Presentation.</para>
    /// <para>Nature transactionnelle : non transactionnel par construction
    /// (invariant 6 de §2.1 du 0230, R-4.10.1). Aucune mutation EF Core n'est
    /// effectuée directement par ce UseCase. La seule mutation persistante du
    /// scénario (passage de la session à l'état déconnecté) est déléguée à
    /// <see cref="IU_UserAppSession_Close"/> qui ouvre et clôt sa propre transaction,
    /// au titre de l'indépendance transactionnelle I-4.10.3.</para>
    /// <para>Participation à la chaîne UC → UC : ce UseCase consomme
    /// <see cref="IU_UserAppSession_Close"/> en sous-séquence dans chacune des cinq
    /// voies opératoires (branche <c>ForceClose</c> + quatre modes de la matrice),
    /// conditionnellement à l'état <see cref="ISE_App.IsConnected"/> : la délégation
    /// effective à <see cref="IU_UserAppSession_Close"/> n'a lieu que si la connexion
    /// à la base est établie, sinon une journalisation silencieuse via
    /// <see cref="IU_LogAndNotify"/> se substitue à la déconnexion. Ce filet à deux
    /// branches est factorisé dans la méthode privée helper
    /// <c>EnsureSessionClosedAsync</c> mobilisée par les cinq voies. Un second
    /// filet interne de rangement technique, factorisé dans la méthode privée
    /// helper <c>EnsureSingleInstanceReleasedAsync</c>, est également mobilisé
    /// par les cinq voies immédiatement avant la délégation à
    /// <see cref="IS_Shutdown"/> : il libère les primitives noyau d'unicité
    /// d'instance applicative détenues par l'instance courante par délégation
    /// au service d'infrastructure <see cref="IS_SingleInstanceGuard"/> en
    /// mode <see cref="En_SingleInstanceOperation.Release"/>. Ce second filet
    /// est best-effort par construction (idempotence garantie côté implémentation
    /// du guard) et ne conditionne pas la suite de la séquence de fermeture.
    /// Il est
    /// par ailleurs consommé en sous-séquence par
    /// <see cref="IU_DigitTryDb_RecoverConnection"/> en mode delay dans la branche
    /// d'échec de la procédure de récupération de connexion à la base partagée, au
    /// titre de la chaîne UC → UC normalisée (R-4.14.21). La signature signalable
    /// <c>Task&lt;En_CloseResult&gt;</c> du contrat est doublement motivée : par la
    /// nécessité fonctionnelle pour le consommateur de présentation (ViewModel ou
    /// code-behind de la fenêtre principale WPF) de piloter
    /// <c>CancelEventArgs.Cancel</c> selon l'issue retournée, et par la conformité
    /// à R-4.14.21 côté versant amont vis-à-vis de
    /// <see cref="IU_DigitTryDb_RecoverConnection"/>.</para>
    /// <para>Conformité à R-4.14.21 (chaîne UC → UC normalisée) sur le versant
    /// orchestrateur amont : le retour signalable
    /// <c>Task&lt;bool&gt;</c> exposé par <see cref="IU_UserAppSession_Close.ExecuteAsync"/>
    /// est exploité par valeur dans chacun des cinq sites de consommation au moyen
    /// d'un discard explicite (<c>_ = await _userAppSessionClose.ExecuteAsync(...)</c>),
    /// satisfaisant la condition (a) de la règle. Les conditions (b) indépendance
    /// transactionnelle (I-4.10.3) et (c) traitement terminal propre via
    /// <see cref="IU_LogAndNotify"/> dans les catch typés sont satisfaites par
    /// construction (cf. paragraphe « Nature transactionnelle » et pipeline terminal
    /// des trois catch typés ci-après). La conduite à tenir en cas de retour
    /// <see langword="false"/> du UseCase consommé (échec applicatif déjà journalisé
    /// et notifié à l'utilisateur en aval par son propre pipeline terminal) est la
    /// convergence inconditionnelle vers <see cref="IS_Shutdown.ExecuteAsync"/> :
    /// la convergence PR-B documentée prime, l'incident ayant déjà été restitué à
    /// l'utilisateur par le UseCase consommé. Aucun catch applicatif n'est ajouté
    /// autour de l'appel — les exceptions applicatives typées ne sont jamais
    /// propagées entre UseCases orchestrants conformément à la doctrine de chaîne
    /// UC → UC normalisée.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Lire le contexte d'environnement depuis les Settings partagés (<c>AppSessionId</c>, <c>ForceClose</c>, <c>IsConnected</c>).</description></item>
    /// <item><description>Dispatcher l'orchestration selon les cinq voies opératoires : branche <c>ForceClose</c> (priorité absolue) et matrice à quatre modes (confirmation, delay, warning, direct).</description></item>
    /// <item><description>Déléguer la déconnexion de la session à <see cref="IU_UserAppSession_Close"/>.</description></item>
    /// <item><description>Émettre la demande de confirmation utilisateur, l'avertissement ou l'ouverture / fermeture de fenêtre de dialogue via <see cref="IS_Notification"/>.</description></item>
    /// <item><description>Remettre à zéro le flag <see cref="ISE_User.ForceClose"/> juste avant chaque déclenchement de la fermeture WPF effective.</description></item>
    /// <item><description>Déclencher la fermeture WPF effective via <see cref="IS_Shutdown"/>.</description></item>
    /// <item><description>Absorber les exceptions typées du projet dans le pipeline terminal et déléguer la journalisation/notification à <see cref="IU_LogAndNotify"/>.</description></item>
    /// <item><description>Propager <see cref="OperationCanceledException"/> à l'appelant sans absorption.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne pilote pas <c>CancelEventArgs.Cancel</c> : cette responsabilité incombe au consommateur de présentation.</description></item>
    /// <item><description>N'appelle aucun Command Handler (<c>CH_</c>), aucun Repository (<c>CR_</c>) et aucun composant de présentation hors médiation contractuelle.</description></item>
    /// <item><description>Ne porte aucune classification d'exception : cette responsabilité est portée par <see cref="IU_LogAndNotify"/> et le pipeline qu'il orchestre.</description></item>
    /// <item><description>Ne consulte pas les paramètres <c>confirmation</c>, <c>warning</c>, <c>delaySeconds</c> depuis les Settings : ces trois paramètres sont fournis par le consommateur de présentation à chaque appel.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_CloseApplication"/>
    public class UC_CloseApplication : IU_CloseApplication
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Clé dictionnaire de la demande de confirmation utilisateur émise en mode
        /// confirmation, résolue en interne par <see cref="IS_Notification"/> via
        /// <c>IS_Dictionary</c>.
        /// </summary>
        private const string DictKey_Confirmation = "No_AD_01";

        /// <summary>
        /// Clé dictionnaire de l'avertissement utilisateur émis en mode warning,
        /// résolue en interne par <see cref="IS_Notification"/> via <c>IS_Dictionary</c>.
        /// </summary>
        private const string DictKey_Warning = "No_In_10";

        /// <summary>
        /// Clé dictionnaire du titre de la fenêtre de dialogue non bloquante ouverte
        /// en mode delay, résolue en interne par <see cref="IS_Notification"/> via
        /// <c>IS_Dictionary</c>.
        /// </summary>
        private const string DictKey_DialogTitle = "No_Ti_05";

        /// <summary>
        /// Clé dictionnaire du contenu de la fenêtre de dialogue non bloquante ouverte
        /// en mode delay, résolue en interne par <see cref="IS_Notification"/> via
        /// <c>IS_Dictionary</c>.
        /// </summary>
        private const string DictKey_DialogContent = "No_In_11";

        /// <summary>
        /// Nom du UseCase, utilisé dans la composition de la CallChain (§4.5).
        /// Initialisé une seule fois en constructeur par <c>GetType().Name</c>.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_User _settingsUser;
        private readonly ISE_App _settingsApp;
        private readonly IS_Notification _notification;
        private readonly IS_Shutdown _shutdown;
        private readonly IS_SingleInstanceGuard _singleInstanceGuard;
        private readonly IU_UserAppSession_Close _userAppSessionClose;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_CloseApplication"/>.
        /// </summary>
        /// <param name="settingsUser">Settings utilisateur partagé : lecture
        /// <c>AppSessionId</c> et lecture/écriture <c>ForceClose</c>.</param>
        /// <param name="settingsApp">Settings applicatif partagé : lecture
        /// <c>IsConnected</c>.</param>
        /// <param name="notification">Service de présentation utilisé pour les
        /// interactions utilisateur (confirmation, avertissement, fenêtre de
        /// dialogue non bloquante).</param>
        /// <param name="shutdown">Service de présentation pilotant le geste WPF
        /// terminal de fermeture de la fenêtre principale.</param>
        /// <param name="singleInstanceGuard">Service d'infrastructure de garde
        /// d'unicité d'instance applicative sur la session Windows courante,
        /// consommé en fin de séquence de chacune des cinq voies opératoires
        /// (branche <c>ForceClose</c> + quatre modes de la matrice) pour
        /// libération explicite en mode <see cref="En_SingleInstanceOperation.Release"/>
        /// des primitives noyau détenues par l'instance courante, immédiatement
        /// avant la délégation à <see cref="IS_Shutdown"/>.</param>
        /// <param name="userAppSessionClose">UseCase consommé en sous-séquence
        /// pour la déconnexion de la session applicative courante.</param>
        /// <param name="logAndNotify">UseCase terminal de journalisation et
        /// notification (pipeline standard §4.7.4 du 0230).</param>
        /// <exception cref="ArgumentNullException">Levée si l'une quelconque
        /// des dépendances injectées est <see langword="null"/>.</exception>
        public UC_CloseApplication(
            ISE_User settingsUser,
            ISE_App settingsApp,
            IS_Notification notification,
            IS_Shutdown shutdown,
            IS_SingleInstanceGuard singleInstanceGuard,
            IU_UserAppSession_Close userAppSessionClose,
            IU_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;

            _settingsUser = settingsUser ?? throw new ArgumentNullException(nameof(settingsUser));
            _settingsApp = settingsApp ?? throw new ArgumentNullException(nameof(settingsApp));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
            _shutdown = shutdown ?? throw new ArgumentNullException(nameof(shutdown));
            _singleInstanceGuard = singleInstanceGuard ?? throw new ArgumentNullException(nameof(singleInstanceGuard));
            _userAppSessionClose = userAppSessionClose ?? throw new ArgumentNullException(nameof(userAppSessionClose));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Orchestre la procédure de fermeture de l'application selon la matrice
        /// à quatre modes pilotée par les paramètres fonctionnels d'entrée, et
        /// restitue l'issue fonctionnelle au consommateur de présentation.
        /// </summary>
        /// <remarks>
        /// <para>Séquence d'orchestration :</para>
        /// <list type="number">
        /// <item><description>Construction de la CallChain en première instruction effective (§4.5, item UC7).</description></item>
        /// <item><description>Lecture du contexte d'environnement depuis les Settings partagés (<c>AppSessionId</c>, <c>ForceClose</c>, <c>IsConnected</c>).</description></item>
        /// <item><description>Dispatcher à cinq voies : si <c>ForceClose == true</c>, délégation à <see cref="ExecuteWithForceCloseAsync"/> (priorité absolue) ; sinon, dispatch hiérarchique sur la matrice à quatre modes vers la méthode privée correspondante — <see cref="ExecuteWithConfirmationAsync"/>, <see cref="ExecuteWithDelayAsync"/>, <see cref="ExecuteWithWarningAsync"/>, <see cref="ExecuteDirectAsync"/>. L'état <c>IsConnected</c> est propagé à chaque méthode privée de mode pour pilotage du filet <see cref="EnsureSessionClosedAsync"/>.</description></item>
        /// </list>
        /// <para>Matrice à quatre modes (priorité hiérarchique stricte) :
        /// (1) <c>confirmation == true</c> → mode confirmation ;
        /// (2) sinon <c>delaySeconds &gt; 0</c> → mode delay ;
        /// (3) sinon <c>warning == true</c> → mode warning ;
        /// (4) sinon → mode direct.</para>
        /// <para>Convergence PR-B : à l'exception du refus utilisateur en mode
        /// confirmation, tous les chemins (branche <c>ForceClose</c>, confirmation
        /// accepté, delay, warning, direct — soit cinq voies au total) déclenchent
        /// <see cref="IS_Shutdown.ExecuteAsync"/> après remise à zéro de
        /// <see cref="ISE_User.ForceClose"/>.</para>
        /// <para>Pipeline terminal (§4.7.4, items UC10, UC11, UC12, UC13) :
        /// triplet normalisé Ex_Business → <c>No_EC_01</c>, Ex_Infrastructure →
        /// <c>No_EC_02</c>, Ex_Unclassified → <c>No_EC_03</c>, conversion en
        /// <see cref="En_CloseResult.Cancelled"/>. <see cref="OperationCanceledException"/>
        /// est propagée à l'appelant sans appel à <see cref="IU_LogAndNotify"/>.
        /// Aucun <c>catch (Exception)</c> terminal côté UseCase.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="confirmation">Paramètre fonctionnel pilotant le mode
        /// confirmation (priorité absolue dans la matrice à quatre modes).</param>
        /// <param name="warning">Paramètre fonctionnel pilotant le mode warning
        /// (priorité tertiaire dans la matrice à quatre modes).</param>
        /// <param name="delaySeconds">Paramètre fonctionnel pilotant le mode delay
        /// (priorité secondaire dans la matrice à quatre modes) ; mode actif si
        /// strictement supérieur à zéro.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Issue fonctionnelle de la procédure de fermeture
        /// (<see cref="En_CloseResult.Cancelled"/>, <see cref="En_CloseResult.Closed"/>
        /// ou <see cref="En_CloseResult.ForceClosed"/>).</returns>
        /// <exception cref="OperationCanceledException">Seule exception propagée
        /// à l'appelant. Les exceptions typées du projet sont absorbées par le
        /// pipeline terminal et converties en <see cref="En_CloseResult.Cancelled"/>.</exception>
        public async Task<En_CloseResult> ExecuteAsync(
            string caller,
            bool confirmation,
            bool warning,
            int delaySeconds,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Lecture du contexte d'environnement
                int sessionId = _settingsUser.AppSessionId;
                bool forceClose = _settingsUser.ForceClose;
                bool isConnected = _settingsApp.IsConnected;

                // Branche ForceClose — Priorité absolue : fermeture pilotée par
                // ISE_User.ForceClose. Court-circuite la matrice à quatre modes.
                if (forceClose)
                {
                    return await ExecuteWithForceCloseAsync(callChain, sessionId, isConnected, ct);
                }

                // Dispatch hiérarchique sur la matrice à quatre modes
                if (confirmation)
                {
                    return await ExecuteWithConfirmationAsync(callChain, sessionId, isConnected, ct);
                }

                if (delaySeconds > 0)
                {
                    return await ExecuteWithDelayAsync(callChain, sessionId, isConnected, delaySeconds, ct);
                }

                if (warning)
                {
                    return await ExecuteWithWarningAsync(callChain, sessionId, isConnected, ct);
                }

                return await ExecuteDirectAsync(callChain, sessionId, isConnected, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
                return En_CloseResult.Cancelled;
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
                return En_CloseResult.Cancelled;
            }
            catch (Ex_Unclassified ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
                return En_CloseResult.Cancelled;
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Orchestre la branche <c>ForceClose</c> (priorité absolue) : fermeture de
        /// session conditionnelle à <paramref name="isConnected"/> puis fermeture WPF
        /// effective, sans interaction utilisateur.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : délégation au filet <see cref="EnsureSessionClosedAsync"/>
        /// (délégation à <see cref="IU_UserAppSession_Close"/> si
        /// <paramref name="isConnected"/> est <see langword="true"/>, journalisation
        /// silencieuse via <see cref="IU_LogAndNotify"/> sinon), remise à zéro de
        /// <see cref="ISE_User.ForceClose"/>, fermeture WPF via
        /// <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.ForceClosed"/>.</para>
        /// <para>Priorité absolue : cette méthode est appelée directement depuis
        /// <see cref="ExecuteAsync"/> lorsque le flag <see cref="ISE_User.ForceClose"/>
        /// est à <see langword="true"/>, court-circuitant la matrice à quatre modes.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par <see cref="ExecuteAsync"/>.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="isConnected">État de connexion à la base de données,
        /// lu en amont depuis <see cref="ISE_App.IsConnected"/>. Pilote le filet
        /// <see cref="EnsureSessionClosedAsync"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.ForceClosed"/>.</returns>
        private async Task<En_CloseResult> ExecuteWithForceCloseAsync(
            string caller,
            int sessionId,
            bool isConnected,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteWithForceCloseAsync)}";

            await EnsureSessionClosedAsync(callChain, sessionId, isConnected, "forceClose", ct);
            _settingsUser.ForceClose = false;
            await EnsureSingleInstanceReleasedAsync(callChain, ct);
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.ForceClosed;
        }

        /// <summary>
        /// Orchestre le mode confirmation de la matrice à quatre modes : demande de
        /// confirmation utilisateur préalable, puis fermeture de session conditionnelle
        /// à <paramref name="isConnected"/> et fermeture WPF effective sur acceptation,
        /// ou abandon sans effet sur refus.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : demande de confirmation utilisateur via
        /// <see cref="IS_Notification.ConfirmationReturn"/> (clé
        /// <c>DictKey_Confirmation</c>). Sur retour <see langword="false"/> (refus
        /// utilisateur ou fermeture forcée de la boîte) : retour
        /// <see cref="En_CloseResult.Cancelled"/> sans déconnexion ni fermeture
        /// WPF. Sur acceptation (<see langword="true"/>) : délégation au filet
        /// <see cref="EnsureSessionClosedAsync"/> (délégation à
        /// <see cref="IU_UserAppSession_Close"/> si <paramref name="isConnected"/>
        /// est <see langword="true"/>, journalisation silencieuse sinon), remise
        /// à zéro de <see cref="ISE_User.ForceClose"/>, fermeture WPF via
        /// <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.Closed"/>.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par <see cref="ExecuteAsync"/>.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="isConnected">État de connexion à la base de données,
        /// lu en amont depuis <see cref="ISE_App.IsConnected"/>. Pilote le filet
        /// <see cref="EnsureSessionClosedAsync"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.Cancelled"/> sur refus utilisateur,
        /// <see cref="En_CloseResult.Closed"/> sur acceptation.</returns>
        private async Task<En_CloseResult> ExecuteWithConfirmationAsync(
            string caller,
            int sessionId,
            bool isConnected,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteWithConfirmationAsync)}";

            bool result = _notification.ConfirmationReturn(callChain, DictKey_Confirmation, ct: ct);

            if (!result)
            {
                return En_CloseResult.Cancelled;
            }

            await EnsureSessionClosedAsync(callChain, sessionId, isConnected, "confirmation", ct);
            _settingsUser.ForceClose = false;
            await EnsureSingleInstanceReleasedAsync(callChain, ct);
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.Closed;
        }

        /// <summary>
        /// Orchestre le mode delay de la matrice à quatre modes : fermeture de session
        /// conditionnelle à <paramref name="isConnected"/>, ouverture d'une fenêtre
        /// de dialogue non bloquante, attente coopérative, fermeture de la fenêtre
        /// de dialogue puis fermeture WPF effective.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : délégation au filet <see cref="EnsureSessionClosedAsync"/>
        /// (délégation à <see cref="IU_UserAppSession_Close"/> si
        /// <paramref name="isConnected"/> est <see langword="true"/>, journalisation
        /// silencieuse sinon), ouverture d'une fenêtre de dialogue non bloquante via
        /// <see cref="IS_Notification.OpenDialogWindow"/> (clés
        /// <c>DictKey_DialogTitle</c> et <c>DictKey_DialogContent</c>), attente
        /// coopérative de <paramref name="delaySeconds"/> secondes via
        /// <c>Task.Delay</c> avec propagation du jeton d'annulation, fermeture de la
        /// fenêtre de dialogue via <see cref="IS_Notification.CloseDialogWindow"/>,
        /// remise à zéro de <see cref="ISE_User.ForceClose"/>, fermeture WPF via
        /// <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.ForceClosed"/>.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par <see cref="ExecuteAsync"/>.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="isConnected">État de connexion à la base de données,
        /// lu en amont depuis <see cref="ISE_App.IsConnected"/>. Pilote le filet
        /// <see cref="EnsureSessionClosedAsync"/>.</param>
        /// <param name="delaySeconds">Durée de l'attente coopérative en secondes,
        /// reçue depuis le paramètre fonctionnel d'entrée d'<see cref="ExecuteAsync"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.ForceClosed"/>.</returns>
        private async Task<En_CloseResult> ExecuteWithDelayAsync(
            string caller,
            int sessionId,
            bool isConnected,
            int delaySeconds,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteWithDelayAsync)}";

            await EnsureSessionClosedAsync(callChain, sessionId, isConnected, "delay", ct);
            _notification.OpenDialogWindow(callChain, DictKey_DialogTitle, DictKey_DialogContent, ct);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), ct);
            _notification.CloseDialogWindow(callChain, ct);
            _settingsUser.ForceClose = false;
            await EnsureSingleInstanceReleasedAsync(callChain, ct);
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.ForceClosed;
        }

        /// <summary>
        /// Orchestre le mode warning de la matrice à quatre modes : fermeture de
        /// session conditionnelle à <paramref name="isConnected"/>, émission d'un
        /// avertissement utilisateur puis fermeture WPF effective.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : délégation au filet <see cref="EnsureSessionClosedAsync"/>
        /// (délégation à <see cref="IU_UserAppSession_Close"/> si
        /// <paramref name="isConnected"/> est <see langword="true"/>, journalisation
        /// silencieuse sinon), émission d'un avertissement utilisateur via
        /// <see cref="IS_Notification.Warning"/> (clé <c>DictKey_Warning</c>), remise
        /// à zéro de <see cref="ISE_User.ForceClose"/>, fermeture WPF via
        /// <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.ForceClosed"/>.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par <see cref="ExecuteAsync"/>.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="isConnected">État de connexion à la base de données,
        /// lu en amont depuis <see cref="ISE_App.IsConnected"/>. Pilote le filet
        /// <see cref="EnsureSessionClosedAsync"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.ForceClosed"/>.</returns>
        private async Task<En_CloseResult> ExecuteWithWarningAsync(
            string caller,
            int sessionId,
            bool isConnected,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteWithWarningAsync)}";

            await EnsureSessionClosedAsync(callChain, sessionId, isConnected, "warning", ct);
            _notification.Warning(callChain, DictKey_Warning, ct: ct);
            _settingsUser.ForceClose = false;
            await EnsureSingleInstanceReleasedAsync(callChain, ct);
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.ForceClosed;
        }

        /// <summary>
        /// Orchestre le mode direct de la matrice à quatre modes : fermeture de
        /// session conditionnelle à <paramref name="isConnected"/> puis fermeture WPF
        /// effective, sans interaction utilisateur.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : délégation au filet <see cref="EnsureSessionClosedAsync"/>
        /// (délégation à <see cref="IU_UserAppSession_Close"/> si
        /// <paramref name="isConnected"/> est <see langword="true"/>, journalisation
        /// silencieuse sinon), remise à zéro de <see cref="ISE_User.ForceClose"/>,
        /// fermeture WPF via <see cref="IS_Shutdown"/>, retour
        /// <see cref="En_CloseResult.Closed"/>.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par <see cref="ExecuteAsync"/>.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="isConnected">État de connexion à la base de données,
        /// lu en amont depuis <see cref="ISE_App.IsConnected"/>. Pilote le filet
        /// <see cref="EnsureSessionClosedAsync"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.Closed"/>.</returns>
        private async Task<En_CloseResult> ExecuteDirectAsync(
            string caller,
            int sessionId,
            bool isConnected,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteDirectAsync)}";

            await EnsureSessionClosedAsync(callChain, sessionId, isConnected, "direct", ct);
            _settingsUser.ForceClose = false;
            await EnsureSingleInstanceReleasedAsync(callChain, ct);
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.Closed;
        }

        /// <summary>
        /// Filet de fermeture de session à deux branches : délégation à
        /// <see cref="IU_UserAppSession_Close"/> si <paramref name="isConnected"/>
        /// est <see langword="true"/>, journalisation silencieuse via
        /// <see cref="IU_LogAndNotify"/> sinon. Consommé par les cinq voies
        /// opératoires du UseCase.
        /// </summary>
        /// <remarks>
        /// <para>Séquence conditionnelle :</para>
        /// <list type="bullet">
        /// <item><description>Cas <paramref name="isConnected"/> <see langword="true"/> :
        /// délégation à <see cref="IU_UserAppSession_Close"/> pour la déconnexion de
        /// la session applicative courante. Retour signalable <c>Task&lt;bool&gt;</c>
        /// exploité par valeur via discard explicite (R-4.14.21, condition (a)) — la
        /// convergence PR-B est inconditionnelle en aval côté méthode privée de mode,
        /// l'incident éventuel du UseCase consommé ayant déjà été journalisé et
        /// notifié terminalement par son propre pipeline.</description></item>
        /// <item><description>Cas <paramref name="isConnected"/> <see langword="false"/> :
        /// journalisation silencieuse (<c>notify: false</c>) via
        /// <see cref="IU_LogAndNotify"/> avec construction d'un
        /// <see cref="Ex_Infrastructure"/> portant le code
        /// <see cref="Ex_Infrastructure.ErrorCodes.IN_ER_04"/> et un message contextuel
        /// paramétré par <paramref name="modeLabel"/> documentant le mode opératoire
        /// courant. Aucune déconnexion effective de session.</description></item>
        /// </list>
        /// <para>Le helper ne pilote ni la remise à zéro de
        /// <see cref="ISE_User.ForceClose"/>, ni l'appel à
        /// <see cref="IS_Shutdown.ExecuteAsync"/> : ces deux gestes sont portés par
        /// chaque méthode privée de mode appelante (convergence PR-B centralisée
        /// en aval de la consommation du helper).</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par la méthode privée
        /// de mode appelante.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="isConnected">État de connexion à la base de données,
        /// lu en amont depuis <see cref="ISE_App.IsConnected"/>. Pilote la branche
        /// du filet.</param>
        /// <param name="modeLabel">Libellé textuel du mode opératoire courant
        /// (<c>"forceClose"</c>, <c>"confirmation"</c>, <c>"delay"</c>,
        /// <c>"warning"</c> ou <c>"direct"</c>), injecté dans le message contextuel du
        /// <see cref="Ex_Infrastructure"/> construit dans la branche non connectée
        /// pour traçabilité en journalisation.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        private async Task EnsureSessionClosedAsync(
            string caller,
            int sessionId,
            bool isConnected,
            string modeLabel,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(EnsureSessionClosedAsync)}";

            if (_settingsUser.AppUserId > 0)
            {
                if (isConnected)
                {
                    _ = await _userAppSessionClose.ExecuteAsync(callChain, sessionId, ct);
                }
                else
                {
                    await _logAndNotify.ExecuteAsync(
                        callChain,
                        "No_EC_02",
                        new Ex_Infrastructure(
                            callChain: callChain,
                            errorId: Ex_Infrastructure.ErrorCodes.IN_ER_04,
                            errorException: $"Fermeture de l'application en mode '{modeLabel}' sans accès à la base."),
                        notify: false,
                        ct: ct);
                }
            }
        }

        /// <summary>
        /// Filet de libération des primitives noyau d'unicité d'instance
        /// applicative détenues par l'instance courante : délégation au
        /// service d'infrastructure <see cref="IS_SingleInstanceGuard"/>
        /// en mode <see cref="En_SingleInstanceOperation.Release"/>.
        /// Consommé par les cinq voies opératoires du UseCase immédiatement
        /// avant la délégation à <see cref="IS_Shutdown"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : second filet interne de rangement technique
        /// mobilisé par chacune des cinq méthodes privées de mode
        /// (<see cref="ExecuteWithForceCloseAsync"/>,
        /// <see cref="ExecuteWithConfirmationAsync"/>,
        /// <see cref="ExecuteWithDelayAsync"/>,
        /// <see cref="ExecuteWithWarningAsync"/>,
        /// <see cref="ExecuteDirectAsync"/>), homogène au filet préexistant
        /// <see cref="EnsureSessionClosedAsync"/> pour la fermeture de session.
        /// Positionné en site pré-terminal dans chacune des cinq branches,
        /// immédiatement avant l'appel à
        /// <see cref="IS_Shutdown.ExecuteAsync(string, System.Threading.CancellationToken)"/>,
        /// afin de préserver l'unicité d'instance applicative jusqu'à l'ultime
        /// geste WPF de fermeture — en particulier en mode <c>delay</c> où le
        /// mutex est détenu pendant toute la fenêtre d'attente coopérative.</para>
        /// <para>Objectif : solliciter le service d'infrastructure de garde
        /// d'unicité pour libérer proprement le mutex Win32 nommé et le
        /// <c>EventWaitHandle</c> nommé, ainsi que pour arrêter le thread
        /// d'écoute d'activation programmatique côté instance primaire (piloté
        /// par le <c>CancellationTokenSource</c> interne de l'implémentation
        /// du guard, indépendant du <paramref name="ct"/> public).</para>
        /// <para>Séquence de délégation : construction en tête d'une locale
        /// <c>callChain</c> enrichie du nom du helper au format normatif de
        /// §4.5 du 0230 (patron d'enrichissement homogène à celui de
        /// <see cref="EnsureSessionClosedAsync"/>), puis appel synchrone à
        /// <see cref="IS_SingleInstanceGuard.Execute(string, En_SingleInstanceOperation, System.Threading.CancellationToken)"/>
        /// avec la valeur <see cref="En_SingleInstanceOperation.Release"/>.
        /// Le verdict <see langword="bool"/> nominalement <see langword="true"/>
        /// en contexte <c>Release</c> est ignoré par discard explicite
        /// (<c>_ =</c>) : le filet est best-effort par construction et ne
        /// conditionne pas la suite de la séquence de fermeture. L'instruction
        /// terminale <c>await Task.CompletedTask</c> assure l'homogénéité de
        /// forme avec <see cref="EnsureSessionClosedAsync"/> (helper
        /// réellement asynchrone) ; la décoration <c>async</c> sans <c>await</c>
        /// fonctionnel est retenue au titre de la lisibilité et de la symétrie
        /// visuelle des deux filets, l'overhead de machine d'état async étant
        /// négligeable au regard du gain de cohérence.</para>
        /// <para>Absence de try/catch local : les exceptions applicatives typées
        /// éventuelles remontées par
        /// <see cref="IS_SingleInstanceGuard.Execute(string, En_SingleInstanceOperation, System.Threading.CancellationToken)"/>
        /// (<see cref="Ex_Infrastructure"/> requalifiée par
        /// <c>IS_ExClassifier</c> en cas de défaillance des primitives noyau ;
        /// <see cref="OperationCanceledException"/> propagée par
        /// <c>throw</c> conformément à R-4.6.13 du 0231) sont captées par les
        /// catch typés terminaux applicatifs déjà en place dans
        /// <see cref="ExecuteAsync"/>, patron homogène à toutes les autres
        /// méthodes privées du fichier.</para>
        /// <para>Position dans la région : placée dans la région existante
        /// <c>=== Méthodes privées ===</c>, immédiatement après
        /// <see cref="EnsureSessionClosedAsync"/>, aucune nouvelle sous-région
        /// introduite.</para>
        /// <para>Idempotence garantie côté implémentation du guard : la garde
        /// interne N-3 du service admet sans erreur un appel <c>Release</c>
        /// orphelin (<c>Release</c> sans <c>Acquire</c> préalable réussi). Le
        /// caractère best-effort du filet est ainsi garanti par construction.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par la méthode privée
        /// de mode appelante.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        private async Task EnsureSingleInstanceReleasedAsync(
            string caller,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(EnsureSingleInstanceReleasedAsync)}";

            _ = _singleInstanceGuard.Execute(
                    callChain,
                    En_SingleInstanceOperation.Release,
                    ct);

            await Task.CompletedTask;
        }

        #endregion
    }
}