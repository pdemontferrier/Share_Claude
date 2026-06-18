using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Common.Exceptions;
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
    /// <para>Branche legacy <c>ForceClose</c> (priorité absolue) : le flag
    /// <see cref="ISE_User.ForceClose"/> est lu en début de scénario et, s'il est à
    /// <see langword="true"/>, prime sur la matrice à quatre modes. Selon l'état
    /// <see cref="ISE_App.IsConnected"/>, la branche legacy effectue soit la
    /// déconnexion de la session (cas connecté), soit une journalisation silencieuse
    /// (cas déconnecté), puis converge vers la fermeture applicative effective via
    /// <see cref="IS_Shutdown"/>. Retour <see cref="En_CloseResult.ForceClosed"/>
    /// dans les deux cas.</para>
    /// <para>Convergence PR-B : à l'exception du mode confirmation sur refus
    /// utilisateur (qui retourne <see cref="En_CloseResult.Cancelled"/> sans fermeture
    /// effective), tous les chemins fonctionnels — y compris la branche legacy
    /// <c>ForceClose</c> — convergent vers <see cref="IS_Shutdown.ExecuteAsync"/>,
    /// uniformisant le pilotage WPF côté D_Presentation.</para>
    /// <para>Nature transactionnelle : non transactionnel par construction
    /// (invariant 6 de §2.1 du 0230, R-4.10.1). Aucune mutation EF Core n'est
    /// effectuée directement par ce UseCase. La seule mutation persistante du
    /// scénario (passage de la session à l'état déconnecté) est déléguée à
    /// <see cref="IU_UserAppSession_Close"/> qui ouvre et clôt sa propre transaction,
    /// au titre de l'indépendance transactionnelle I-4.10.3.</para>
    /// <para>Participation à la chaîne UC → UC : ce UseCase consomme
    /// <see cref="IU_UserAppSession_Close"/> en sous-séquence dans cinq des six sites
    /// d'orchestration (branche legacy connectée, mode confirmation accepté, mode
    /// delay, mode warning, mode direct ; la branche legacy déconnectée court-circuite
    /// cette consommation au profit d'une journalisation silencieuse). Il n'est
    /// lui-même pas consommé en sous-séquence par un UseCase amont ; son consommateur
    /// est la couche de présentation. La signature signalable
    /// <c>Task&lt;En_CloseResult&gt;</c> du contrat est motivée par la nécessité
    /// fonctionnelle pour ce consommateur de présentation de piloter
    /// <c>CancelEventArgs.Cancel</c> selon l'issue retournée.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Lire le contexte d'environnement depuis les Settings partagés (<c>AppSessionId</c>, <c>ForceClose</c>, <c>IsConnected</c>).</description></item>
    /// <item><description>Dispatcher l'orchestration selon la matrice à quatre modes et la branche legacy <c>ForceClose</c> (priorité absolue).</description></item>
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

        #region === Dépendances injectées ===

        private readonly ISE_User _settingsUser;
        private readonly ISE_App _settingsApp;
        private readonly IU_UserAppSession_Close _userAppSessionClose;
        private readonly IS_Notification _notification;
        private readonly IU_LogAndNotify _logAndNotify;
        private readonly IS_Shutdown _shutdown;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_CloseApplication"/>.
        /// </summary>
        /// <param name="settingsUser">Settings utilisateur partagé : lecture
        /// <c>AppSessionId</c> et lecture/écriture <c>ForceClose</c>.</param>
        /// <param name="settingsApp">Settings applicatif partagé : lecture
        /// <c>IsConnected</c>.</param>
        /// <param name="userAppSessionClose">UseCase consommé en sous-séquence
        /// pour la déconnexion de la session applicative courante.</param>
        /// <param name="notification">Service de présentation utilisé pour les
        /// interactions utilisateur (confirmation, avertissement, fenêtre de
        /// dialogue non bloquante).</param>
        /// <param name="logAndNotify">UseCase terminal de journalisation et
        /// notification (pipeline standard §4.7.4 du 0230).</param>
        /// <param name="shutdown">Service de présentation pilotant le geste WPF
        /// terminal de fermeture de la fenêtre principale.</param>
        /// <exception cref="ArgumentNullException">Levée si l'une quelconque
        /// des dépendances injectées est <see langword="null"/>.</exception>
        public UC_CloseApplication(
            ISE_User settingsUser,
            ISE_App settingsApp,
            IU_UserAppSession_Close userAppSessionClose,
            IS_Notification notification,
            IU_LogAndNotify logAndNotify,
            IS_Shutdown shutdown)
        {
            _callee = GetType().Name;

            _settingsUser = settingsUser ?? throw new ArgumentNullException(nameof(settingsUser));
            _settingsApp = settingsApp ?? throw new ArgumentNullException(nameof(settingsApp));
            _userAppSessionClose = userAppSessionClose ?? throw new ArgumentNullException(nameof(userAppSessionClose));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _shutdown = shutdown ?? throw new ArgumentNullException(nameof(shutdown));
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
        /// <item><description>Lecture du contexte d'environnement depuis les Settings partagés.</description></item>
        /// <item><description>Branchement sur <c>ForceClose</c> (priorité absolue) :
        /// <list type="bullet">
        /// <item><description>Legacy + connecté : délégation à <see cref="IU_UserAppSession_Close"/>, remise à zéro de <c>ForceClose</c>, fermeture WPF via <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.ForceClosed"/>.</description></item>
        /// <item><description>Legacy + non connecté : journalisation silencieuse (<c>notify: false</c>) via <see cref="IU_LogAndNotify"/> avec construction d'un <see cref="Ex_Infrastructure"/> documentant la situation, remise à zéro de <c>ForceClose</c>, fermeture WPF via <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.ForceClosed"/>.</description></item>
        /// </list>
        /// </description></item>
        /// <item><description>Sinon, dispatch hiérarchique sur la matrice à quatre modes vers la méthode privée correspondante : <see cref="ExecuteWithConfirmationAsync"/>, <see cref="ExecuteWithDelayAsync"/>, <see cref="ExecuteWithWarningAsync"/>, <see cref="ExecuteDirectAsync"/>.</description></item>
        /// </list>
        /// <para>Matrice à quatre modes (priorité hiérarchique stricte) :
        /// (1) <c>confirmation == true</c> → mode confirmation ;
        /// (2) sinon <c>delaySeconds &gt; 0</c> → mode delay ;
        /// (3) sinon <c>warning == true</c> → mode warning ;
        /// (4) sinon → mode direct.</para>
        /// <para>Convergence PR-B : à l'exception du refus utilisateur en mode
        /// confirmation, tous les chemins (legacy ×2, confirmation accepté, delay,
        /// warning, direct — soit six sites au total) déclenchent
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

                // Branche legacy — Priorité absolue : fermeture forcée pilotée par
                // ISE_User.ForceClose (arbitrage R2-B de l'atelier amont, point (6)
                // du prompt d'ouverture). Convergence PR-B vers _shutdown.ExecuteAsync
                // dans les deux sous-branches.
                if (forceClose)
                {
                    if (isConnected)
                    {
                        await _userAppSessionClose.ExecuteAsync(callChain, sessionId, ct);
                    }
                    else
                    {
                        await _logAndNotify.ExecuteAsync(
                            callChain,
                            "No_EC_02",
                            new Ex_Infrastructure(
                                callChain: callChain,
                                errorId: Ex_Infrastructure.ErrorCodes.IN_ER_04,
                                errorException: "Fermeture forcée de l'application sans accès à la base."),
                            notify: false,
                            ct: ct);
                    }

                    _settingsUser.ForceClose = false;
                    await _shutdown.ExecuteAsync(callChain, ct);
                    return En_CloseResult.ForceClosed;
                }

                // Branche normale — Dispatch hiérarchique sur la matrice à quatre modes
                if (confirmation)
                {
                    return await ExecuteWithConfirmationAsync(callChain, sessionId, ct);
                }

                if (delaySeconds > 0)
                {
                    return await ExecuteWithDelayAsync(callChain, sessionId, delaySeconds, ct);
                }

                if (warning)
                {
                    return await ExecuteWithWarningAsync(callChain, sessionId, ct);
                }

                return await ExecuteDirectAsync(callChain, sessionId, ct);
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
        /// Orchestre le mode confirmation de la matrice à quatre modes : demande de
        /// confirmation utilisateur préalable, puis déconnexion de session et fermeture
        /// WPF effective sur acceptation, ou abandon sans effet sur refus.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : demande de confirmation utilisateur via
        /// <see cref="IS_Notification.ConfirmationReturn"/> (clé
        /// <c>DictKey_Confirmation</c>). Sur retour <see langword="false"/> (refus
        /// utilisateur ou fermeture forcée de la boîte) : retour
        /// <see cref="En_CloseResult.Cancelled"/> sans déconnexion ni fermeture
        /// WPF. Sur acceptation (<see langword="true"/>) : délégation à
        /// <see cref="IU_UserAppSession_Close"/>, remise à zéro de
        /// <see cref="ISE_User.ForceClose"/>, fermeture WPF via
        /// <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.Closed"/>.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par <see cref="ExecuteAsync"/>.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.Cancelled"/> sur refus utilisateur,
        /// <see cref="En_CloseResult.Closed"/> sur acceptation.</returns>
        private async Task<En_CloseResult> ExecuteWithConfirmationAsync(
            string caller,
            int sessionId,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteWithConfirmationAsync)}";

            bool result = _notification.ConfirmationReturn(callChain, DictKey_Confirmation, ct: ct);

            if (!result)
            {
                return En_CloseResult.Cancelled;
            }

            await _userAppSessionClose.ExecuteAsync(callChain, sessionId, ct);
            _settingsUser.ForceClose = false;
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.Closed;
        }

        /// <summary>
        /// Orchestre le mode delay de la matrice à quatre modes : déconnexion de
        /// session, ouverture d'une fenêtre de dialogue non bloquante, attente
        /// coopérative, fermeture de la fenêtre de dialogue puis fermeture WPF
        /// effective.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : délégation à <see cref="IU_UserAppSession_Close"/>,
        /// ouverture d'une fenêtre de dialogue non bloquante via
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
        /// <param name="delaySeconds">Durée de l'attente coopérative en secondes,
        /// reçue depuis le paramètre fonctionnel d'entrée d'<see cref="ExecuteAsync"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.ForceClosed"/>.</returns>
        private async Task<En_CloseResult> ExecuteWithDelayAsync(
            string caller,
            int sessionId,
            int delaySeconds,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteWithDelayAsync)}";

            await _userAppSessionClose.ExecuteAsync(callChain, sessionId, ct);
            _notification.OpenDialogWindow(callChain, DictKey_DialogTitle, DictKey_DialogContent, ct);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), ct);
            _notification.CloseDialogWindow(callChain, ct);
            _settingsUser.ForceClose = false;
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.ForceClosed;
        }

        /// <summary>
        /// Orchestre le mode warning de la matrice à quatre modes : déconnexion de
        /// session, émission d'un avertissement utilisateur puis fermeture WPF
        /// effective.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : délégation à <see cref="IU_UserAppSession_Close"/>,
        /// émission d'un avertissement utilisateur via
        /// <see cref="IS_Notification.Warning"/> (clé <c>DictKey_Warning</c>), remise
        /// à zéro de <see cref="ISE_User.ForceClose"/>, fermeture WPF via
        /// <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.ForceClosed"/>.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par <see cref="ExecuteAsync"/>.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.ForceClosed"/>.</returns>
        private async Task<En_CloseResult> ExecuteWithWarningAsync(
            string caller,
            int sessionId,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteWithWarningAsync)}";

            await _userAppSessionClose.ExecuteAsync(callChain, sessionId, ct);
            _notification.Warning(callChain, DictKey_Warning, ct: ct);
            _settingsUser.ForceClose = false;
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.ForceClosed;
        }

        /// <summary>
        /// Orchestre le mode direct de la matrice à quatre modes : déconnexion de
        /// session puis fermeture WPF effective, sans interaction utilisateur.
        /// </summary>
        /// <remarks>
        /// <para>Séquence : délégation à <see cref="IU_UserAppSession_Close"/>,
        /// remise à zéro de <see cref="ISE_User.ForceClose"/>, fermeture WPF via
        /// <see cref="IS_Shutdown"/>, retour <see cref="En_CloseResult.Closed"/>.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par <see cref="ExecuteAsync"/>.</param>
        /// <param name="sessionId">Identifiant de la session applicative courante,
        /// lu en amont depuis <see cref="ISE_User.AppSessionId"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns><see cref="En_CloseResult.Closed"/>.</returns>
        private async Task<En_CloseResult> ExecuteDirectAsync(
            string caller,
            int sessionId,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ExecuteDirectAsync)}";

            await _userAppSessionClose.ExecuteAsync(callChain, sessionId, ct);
            _settingsUser.ForceClose = false;
            await _shutdown.ExecuteAsync(callChain, ct);
            return En_CloseResult.Closed;
        }

        #endregion
    }
}