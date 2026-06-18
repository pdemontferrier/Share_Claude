using DG244Cutting.A_Domain.Common.Enums;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase d'orchestration de la procédure de fermeture
    /// de l'application DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à
    /// l'obligation de placement des contrats (§4.14.1 du 0230). Elle est consommée
    /// par injection de dépendances par la couche de présentation (ViewModel ou
    /// code-behind de la fenêtre principale WPF) à l'occasion de l'événement
    /// <c>OnClosing</c>, et plus généralement par tout consommateur de présentation
    /// pilotant la fermeture applicative (commande de menu, raccourci global,
    /// mécanisme de surveillance de connexion). Le contrat appartient au domaine
    /// <c>App</c> (orchestration applicative transverse, cycle de vie applicatif).
    /// Son implémentation concrète
    /// <see cref="DG244Cutting.B_UseCases.UseCases.App.UC_CloseApplication"/>
    /// réside en <c>B_UseCases/UseCases/App/</c>.
    /// </para>
    /// <para>
    /// Objectif : orchestrer la procédure de fermeture de l'application en
    /// distinguant trois issues fonctionnelles, signalées au consommateur de
    /// présentation par la valeur de retour : annulation utilisateur, fermeture
    /// confirmée après déconnexion de session, fermeture forcée sans confirmation.
    /// </para>
    /// <para>
    /// Matrice à quatre modes (priorité hiérarchique stricte) : le scénario
    /// porté par la méthode publique unique <see cref="ExecuteAsync"/> dispatche
    /// l'orchestration interne selon les trois paramètres fonctionnels
    /// <c>confirmation</c>, <c>delaySeconds</c>, <c>warning</c> fournis par le
    /// consommateur de présentation à chaque appel, dans cet ordre de priorité
    /// strict :
    /// </para>
    /// <list type="number">
    /// <item><description>Mode confirmation (<c>confirmation = true</c>) :
    /// demande de confirmation utilisateur préalable ; sur refus, la fermeture
    /// est abandonnée et la méthode retourne <see cref="En_CloseResult.Cancelled"/>.
    /// Prime sur tout autre paramètre.</description></item>
    /// <item><description>Mode delay (sinon <c>delaySeconds &gt; 0</c>) :
    /// déconnexion de la session, ouverture d'une fenêtre de dialogue non
    /// bloquante, attente cooperative de <c>delaySeconds</c> secondes, fermeture
    /// de la fenêtre de dialogue puis fermeture applicative effective. Retour
    /// <see cref="En_CloseResult.ForceClosed"/>.</description></item>
    /// <item><description>Mode warning (sinon <c>warning = true</c>) :
    /// déconnexion de la session, émission d'un avertissement utilisateur puis
    /// fermeture applicative effective. Retour
    /// <see cref="En_CloseResult.ForceClosed"/>.</description></item>
    /// <item><description>Mode direct (sinon) : déconnexion de la session
    /// puis fermeture applicative effective sans interaction utilisateur. Retour
    /// <see cref="En_CloseResult.Closed"/>.</description></item>
    /// </list>
    /// <para>
    /// Branche legacy <c>ForceClose</c> (priorité absolue) : le flag
    /// <c>ISE_User.ForceClose</c> est lu en début de scénario et, s'il est à
    /// <see langword="true"/>, prime sur la matrice à quatre modes. Selon l'état
    /// <c>ISE_App.IsConnected</c>, la branche legacy effectue soit la déconnexion
    /// de la session (cas connecté), soit une journalisation silencieuse (cas
    /// déconnecté), puis converge vers la fermeture applicative effective. Retour
    /// <see cref="En_CloseResult.ForceClosed"/> dans les deux cas.
    /// </para>
    /// <para>
    /// Convergence PR-B : à l'exception du mode confirmation sur refus
    /// utilisateur (qui retourne <see cref="En_CloseResult.Cancelled"/> sans
    /// fermeture effective), tous les chemins fonctionnels — y compris la branche
    /// legacy <c>ForceClose</c> — convergent vers le geste WPF terminal de
    /// fermeture de la fenêtre principale, isolé en D_Presentation derrière le
    /// contrat <c>IS_Shutdown</c>. Cette convergence garantit l'uniformité du
    /// pilotage WPF côté présentation pour l'ensemble des modes.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée unique du scénario de fermeture de l'application.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel (R-4.5.5).</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c> en queue (R-4.6.13).</description></item>
    /// <item><description>Exposer le pilotage de la matrice à quatre modes via les trois paramètres fonctionnels d'entrée <c>confirmation</c>, <c>warning</c>, <c>delaySeconds</c>.</description></item>
    /// <item><description>Restituer au consommateur de présentation l'issue fonctionnelle de la procédure via <see cref="En_CloseResult"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique de mutation EF Core : la déconnexion de la session est déléguée au UseCase consommé en sous-séquence <c>IU_UserAppSession_Close</c>.</description></item>
    /// <item><description>N'expose aucun type technique de présentation (WPF, <c>CancelEventArgs</c>) afin de garantir la pureté contractuelle de A_Domain (première obligation contractuelle de §4.14.2 amendée).</description></item>
    /// <item><description>Ne pilote pas le positionnement de <c>CancelEventArgs.Cancel</c> : cette responsabilité incombe au consommateur de présentation qui interprète la valeur de retour.</description></item>
    /// <item><description>Ne consulte pas les paramètres <c>confirmation</c>, <c>warning</c>, <c>delaySeconds</c> depuis les Settings : ces trois paramètres sont fournis par le consommateur de présentation à chaque appel.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_CloseApplication
    {
        // --- Groupe 1 : Fermeture de l'application ---

        /// <summary>
        /// Orchestre la procédure de fermeture de l'application selon la matrice
        /// à quatre modes pilotée par les paramètres fonctionnels d'entrée, et
        /// restitue l'issue fonctionnelle au consommateur de présentation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécutée depuis l'événement <c>OnClosing</c> de la
        /// fenêtre principale WPF ou depuis tout autre consommateur de présentation
        /// pilotant la fermeture applicative. Les éléments de contexte d'environnement
        /// (<c>AppSessionId</c>, <c>ForceClose</c>, <c>IsConnected</c>) sont lus
        /// respectivement depuis <c>ISE_User</c> et <c>ISE_App</c> injectés au
        /// constructeur de l'implémentation. Les trois paramètres fonctionnels
        /// d'entrée (<paramref name="confirmation"/>, <paramref name="warning"/>,
        /// <paramref name="delaySeconds"/>) pilotent la matrice à quatre modes.
        /// </para>
        /// <para>Matrice à quatre modes (priorité hiérarchique stricte) :</para>
        /// <list type="number">
        /// <item><description><c>confirmation == true</c> → mode confirmation (prime sur tout).</description></item>
        /// <item><description>sinon <c>delaySeconds &gt; 0</c> → mode delay.</description></item>
        /// <item><description>sinon <c>warning == true</c> → mode warning.</description></item>
        /// <item><description>sinon → mode direct.</description></item>
        /// </list>
        /// <para>Logique d'orchestration :</para>
        /// <list type="bullet">
        /// <item><description>Branche legacy <c>ForceClose</c> (priorité absolue) : si <c>ISE_User.ForceClose == true</c>, la matrice à quatre modes est court-circuitée. Cas <c>IsConnected == true</c> : délégation à <c>IU_UserAppSession_Close</c> pour la déconnexion de la session courante. Cas <c>IsConnected == false</c> : journalisation silencieuse (<c>notify: false</c>) via <c>IU_LogAndNotify</c> avec construction d'un <c>Ex_Infrastructure</c> documentant la situation. Dans les deux cas : remise à zéro du flag <c>ForceClose</c>, fermeture applicative effective via <c>IS_Shutdown</c>. Retour <see cref="En_CloseResult.ForceClosed"/>.</description></item>
        /// <item><description>Mode confirmation : demande de confirmation utilisateur via <c>IS_Notification.ConfirmationReturn</c>. Refus : retour <see cref="En_CloseResult.Cancelled"/> sans déconnexion ni fermeture WPF. Acceptation : délégation à <c>IU_UserAppSession_Close</c>, remise à zéro de <c>ForceClose</c>, fermeture applicative effective via <c>IS_Shutdown</c>. Retour <see cref="En_CloseResult.Closed"/>.</description></item>
        /// <item><description>Mode delay : délégation à <c>IU_UserAppSession_Close</c>, ouverture d'une fenêtre de dialogue non bloquante via <c>IS_Notification.OpenDialogWindow</c>, attente coopérative de <paramref name="delaySeconds"/> secondes (<c>Task.Delay</c> avec propagation du jeton d'annulation), fermeture de la fenêtre de dialogue via <c>IS_Notification.CloseDialogWindow</c>, remise à zéro de <c>ForceClose</c>, fermeture applicative effective via <c>IS_Shutdown</c>. Retour <see cref="En_CloseResult.ForceClosed"/>.</description></item>
        /// <item><description>Mode warning : délégation à <c>IU_UserAppSession_Close</c>, émission d'un avertissement utilisateur via <c>IS_Notification.Warning</c>, remise à zéro de <c>ForceClose</c>, fermeture applicative effective via <c>IS_Shutdown</c>. Retour <see cref="En_CloseResult.ForceClosed"/>.</description></item>
        /// <item><description>Mode direct : délégation à <c>IU_UserAppSession_Close</c>, remise à zéro de <c>ForceClose</c>, fermeture applicative effective via <c>IS_Shutdown</c>. Retour <see cref="En_CloseResult.Closed"/>.</description></item>
        /// </list>
        /// <para>
        /// Convergence PR-B : à l'exception du mode confirmation sur refus
        /// utilisateur, tous les chemins fonctionnels — y compris la branche legacy
        /// <c>ForceClose</c> — convergent vers <c>IS_Shutdown.ExecuteAsync</c>,
        /// uniformisant le pilotage WPF côté présentation.
        /// </para>
        /// <para>Pipeline terminal : toute exception applicative typée
        /// (<c>Ex_Business</c>, <c>Ex_Infrastructure</c>, <c>Ex_Unclassified</c>)
        /// est absorbée par le pipeline terminal (§4.7.4 du 0230), journalisée
        /// et notifiée via <c>IU_LogAndNotify</c> avec la clé <c>No_EC_XX</c>
        /// appropriée, puis convertie en <see cref="En_CloseResult.Cancelled"/>.
        /// <c>OperationCanceledException</c> est la seule exception propagée à
        /// l'appelant : elle ne donne lieu à aucun appel à <c>IU_LogAndNotify</c>,
        /// l'annulation n'étant pas une erreur.</para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le
        /// format normatif de §4.5 du 0230. Ne doit pas être <see langword="null"/>,
        /// vide ou blanc.
        /// </param>
        /// <param name="confirmation">
        /// Paramètre fonctionnel pilotant le mode confirmation de la matrice à
        /// quatre modes. Lorsqu'il vaut <see langword="true"/>, une demande de
        /// confirmation utilisateur est émise préalablement à toute déconnexion
        /// ou fermeture WPF. Sur refus, la fermeture est abandonnée et la
        /// méthode retourne <see cref="En_CloseResult.Cancelled"/>. Ce paramètre
        /// prime sur <paramref name="delaySeconds"/> et sur
        /// <paramref name="warning"/>.
        /// </param>
        /// <param name="warning">
        /// Paramètre fonctionnel pilotant le mode warning de la matrice à quatre
        /// modes. Lorsqu'il vaut <see langword="true"/> et que ni
        /// <paramref name="confirmation"/> ni <paramref name="delaySeconds"/>
        /// n'ont déjà sélectionné un mode prioritaire, un avertissement utilisateur
        /// est émis après déconnexion de la session, préalablement à la fermeture
        /// WPF effective.
        /// </param>
        /// <param name="delaySeconds">
        /// Paramètre fonctionnel pilotant le mode delay de la matrice à quatre
        /// modes. Lorsqu'il est strictement supérieur à zéro et que
        /// <paramref name="confirmation"/> n'a pas déjà sélectionné le mode
        /// prioritaire, une fenêtre de dialogue non bloquante est ouverte après
        /// déconnexion de la session, suivie d'une attente coopérative de la
        /// durée indiquée (en secondes), puis de la fermeture de la fenêtre de
        /// dialogue préalablement à la fermeture WPF effective.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative. Par défaut <see langword="default"/>.
        /// </param>
        /// <returns>
        /// <para>Issue fonctionnelle de la procédure de fermeture :</para>
        /// <list type="bullet">
        /// <item><description><see cref="En_CloseResult.Cancelled"/> : fermeture annulée — soit par refus utilisateur lors de la confirmation, soit par absorption d'une exception applicative typée dans le pipeline terminal. Le consommateur de présentation positionne <c>CancelEventArgs.Cancel</c> à <see langword="true"/>.</description></item>
        /// <item><description><see cref="En_CloseResult.Closed"/> : fermeture confirmée par l'utilisateur (mode confirmation accepté) ou fermeture directe (mode direct), menée à terme avec déconnexion de la session applicative courante et fermeture WPF effective. Le consommateur de présentation positionne <c>CancelEventArgs.Cancel</c> à <see langword="false"/>.</description></item>
        /// <item><description><see cref="En_CloseResult.ForceClosed"/> : fermeture forcée — soit pilotée par <c>ISE_User.ForceClose</c> (branche legacy, priorité absolue), soit issue du mode warning, soit issue du mode delay. Le consommateur de présentation positionne <c>CancelEventArgs.Cancel</c> à <see langword="false"/>.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation coopérative est signalée par <paramref name="ct"/>.
        /// Seule exception propagée à l'appelant : les exceptions typées du
        /// projet (<c>Ex_Business</c>, <c>Ex_Infrastructure</c>, <c>Ex_Unclassified</c>)
        /// sont absorbées par le pipeline terminal côté UseCase et converties
        /// en <see cref="En_CloseResult.Cancelled"/>.
        /// </exception>
        Task<En_CloseResult> ExecuteAsync(
            string caller,
            bool confirmation,
            bool warning,
            int delaySeconds,
            CancellationToken ct = default);
    }
}