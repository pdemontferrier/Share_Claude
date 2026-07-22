using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
using DG244Cutting.D_Presentation.Settings;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;

namespace DG244Cutting.D_Presentation.ViewModels.Components.Banner
{
    /// <summary>
    /// ViewModel singulier du composant visuel de bannière principale
    /// <c>Banner</c> de l'application DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant hors familles canoniques (UC, SR, CR, CH,
    /// QH, SE, RS, DTO, VM-Page, MH, Page), instancié en portée Singleton par le
    /// conteneur d'injection de dépendances et consommé par <c>Banner</c>
    /// via affectation directe à son <see cref="System.Windows.FrameworkElement.DataContext"/>.
    /// À l'instar de <see cref="VM_MainWindow"/> et de <c>App</c>, son statut
    /// singulier est défendu en l'absence de 0232 d'autorité documentaire ; les
    /// conventions documentaires et structurelles appliquées sont calquées
    /// nominativement sur celles de <see cref="VM_MainWindow"/>, étalon
    /// documentaire de référence pour la structuration du fichier, la séquence
    /// d'initialisation du constructeur, le helper INPC <see cref="SetField{T}"/>,
    /// le mécanisme d'abonnement INPC aux Settings observables et la garde
    /// <see cref="ArgumentNullException"/> sur les dépendances.</para>
    /// <para>Objectif : Exposer à la View <c>Banner</c> les propriétés
    /// observables et les commandes nécessaires au binding de la bannière
    /// principale, en relayant en INPC trois Settings observables (<see cref="ISE_App"/>,
    /// <see cref="ISE_User"/>, <see cref="ISE_Flag"/>) et en orchestrant les
    /// invocations des UseCases de navigation (<see cref="IU_Navigation"/>) et de
    /// fermeture applicative (<see cref="IU_CloseApplication"/>) déclenchées par les
    /// cinq commandes WPF de la bannière (boutons Langue, Utilisateur, Informations,
    /// Messages, Fermer).</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer <see cref="UserFullName"/>, <see cref="IsBannerVisible"/>,
    ///   <see cref="HasUnreadMessages"/>, <see cref="LanguageFlagUri"/> et
    ///   <see cref="IsConnected"/> bindables en lecture, miroir INPC de
    ///   <see cref="ISE_User.AppUserFullName"/>, <see cref="ISE_User.AppUserId"/>
    ///   &gt; 0, <see cref="ISE_App.HasUnreadMessages"/>,
    ///   <see cref="ISE_Flag.AppFlagUri"/> et <see cref="ISE_App.IsConnected"/>.</item>
    ///   <item>Exposer <see cref="MessageIconUri"/>, <see cref="MessageUnreadIconUri"/>,
    ///   <see cref="AppCloseConnectedIconUri"/> et <see cref="AppCloseDisconnectedIconUri"/>
    ///   bindables en lecture, immuables, lues au constructeur depuis le référentiel
    ///   statique intra-couche <see cref="RS_Icons"/>.</item>
    ///   <item>Exposer <see cref="NavigateLanguageCommand"/>,
    ///   <see cref="NavigateUserCommand"/>, <see cref="NavigateAppInfoCommand"/>,
    ///   <see cref="NavigateMessagesCommand"/> et <see cref="CloseAppCommand"/>,
    ///   câblées sur <see cref="UT_RelayCommandArg0Async"/> et déléguant chacune à
    ///   une méthode privée <c>async Task</c> dédiée du ViewModel.</item>
    ///   <item>Implémenter <see cref="INotifyPropertyChanged"/> et propager à la
    ///   View les changements des Settings sources par abonnement à leur
    ///   <see cref="INotifyPropertyChanged.PropertyChanged"/> respectif.</item>
    ///   <item>Initialiser les backing fields observables à la valeur courante des
    ///   Settings dans le constructeur, préalablement au branchement des
    ///   abonnements, afin de garantir un état cohérent dès l'instanciation
    ///   indépendamment de l'ordre d'exécution entre le constructeur du ViewModel
    ///   et toute mutation antérieure des Settings.</item>
    ///   <item>Héberger trois boucles de polling asynchrones encapsulant les
    ///   invocations périodiques de <see cref="IU_UserAppMessage_CheckUnread"/>
    ///   (cadence <see cref="ISE_App.MessageCheckDelay"/>, gating sur
    ///   <see cref="ISE_App.IsConnected"/> et <see cref="IsBannerVisible"/>), de
    ///   <see cref="IU_DigitTryDb_TestConnection"/> (cadence
    ///   <see cref="ISE_App.DatabaseCheckInterval"/>, sans gating) et de
    ///   <see cref="IU_CloseCommand_Check"/> (cadence
    ///   <see cref="ISE_App.CloseCommandDelay"/>, gating sur
    ///   <see cref="ISE_App.IsConnected"/> seul). Chaque boucle
    ///   est amorcée par un dispositif d'idempotence à drapeau et résolue via
    ///   <see cref="IS_UseCaseInvoker"/> au sein d'un scope DI éphémère par
    ///   invocation, conformément à EA-11 (§4.10.10 du 0230).</item>
    ///   <item>S'abonner à l'événement applicatif système
    ///   <see cref="ISE_App.ConnectionLost"/> et y répondre par l'invocation de
    ///   <see cref="IU_DigitTryDb_RecoverConnection"/> via
    ///   <see cref="IS_UseCaseInvoker"/>, sous gating de réentrance par drapeau
    ///   et sous filet de sécurité ultime spécifique au handler d'événement
    ///   applicatif système.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Aucune logique métier ni règle décisionnelle.</item>
    ///   <item>Aucun accès aux données (repository, base, service externe).</item>
    ///   <item>Aucune décision de navigation interne — toutes les navigations sont
    ///   déléguées à <see cref="IU_Navigation"/>.</item>
    ///   <item>Aucune logique de fermeture applicative interne — déléguée à
    ///   <see cref="IU_CloseApplication"/>.</item>
    ///   <item>Aucun désabonnement explicite des trois handlers
    ///   <see cref="INotifyPropertyChanged.PropertyChanged"/> des Settings ni du
    ///   handler <see cref="OnConnectionLost"/> abonné à
    ///   <see cref="ISE_App.ConnectionLost"/> — le ViewModel est Singleton et sa
    ///   durée de vie est alignée sur celle du processus, qui assure naturellement
    ///   la libération. Posture uniforme aux quatre abonnements, étendue par le
    ///   présent fil au quatrième abonnement (événement applicatif système).</item>
    ///   <item>Aucune libération explicite de <see cref="CancellationTokenSource"/>
    ///   — même justification (statut Singleton, vie alignée sur le processus).</item>
    ///   <item>Aucune logique de notification opérateur au bord de
    ///   <see cref="OnConnectionLost"/> — la sortie utilisateur de la procédure
    ///   de récupération est portée par le pipeline interne de
    ///   <see cref="IU_DigitTryDb_RecoverConnection"/> ; le filet de sécurité
    ///   ultime du handler journalise sans notifier (cf.
    ///   <c>EA-NN-FilSecuriteOnConnectionLost</c>).</item>
    ///   <item>Aucun abonnement à <see cref="ISE_App.ConnectionRestored"/> — la
    ///   mise à jour du miroir <see cref="IsConnected"/> au retour de connexion
    ///   procède de la mécanique INPC <c>NotifyConnectionRestored</c> →
    ///   <c>SetField</c> → <see cref="INotifyPropertyChanged.PropertyChanged"/> →
    ///   <see cref="OnSeAppPropertyChanged"/> → relayage du miroir, suffisante
    ///   pour basculer le binding XAML de l'icône Fermer.</item>
    /// </list>
    /// <para>Note sur le statut hors familles canoniques : Composant hors
    /// familles canoniques (UC, SR, CR, CH, QH, SE, RS, DTO, VM-Page, MH, Page),
    /// ne disposant d'aucun 0232 d'autorité documentaire. Les conventions
    /// documentaires et structurelles appliquées sont calquées sur celles de
    /// <see cref="VM_MainWindow"/> (composant singulier hors familles déjà produit,
    /// désigné comme étalon documentaire par le présent fil). Le helper
    /// <see cref="SetField{T}"/> est aligné sur l'étalon documentaire
    /// <c>SE_App.SetField</c> (signature et corps identiques). Le présent
    /// composant servira ultérieurement, conjointement avec <c>Banner</c>,
    /// d'exemple canonique pour la rédaction de la section 5.11 du 0230 et du
    /// 0232-VI-VM à venir.</para>
    /// <para>Exception architecturale propre 1 — EA-NN-NavigationDirecteBanniere.
    /// Le présent ViewModel consomme directement <see cref="IU_Navigation"/> pour les
    /// quatre commandes de navigation (<see cref="NavigateLanguageCommand"/>,
    /// <see cref="NavigateUserCommand"/>, <see cref="NavigateAppInfoCommand"/>,
    /// <see cref="NavigateMessagesCommand"/>), en dérogation à R-4.12.2 et I-4.12.2
    /// (interdiction nominative d'appel à <see cref="IU_Navigation"/> depuis un
    /// ViewModel). Justification : parallèle exact à la dérogation déjà admise
    /// pour <c>VM_MH_Generic</c> au titre des commandes transverses du menu
    /// horizontal — les navigations portées par la bannière sont triviales (pas de
    /// logique métier amont), de même nature que celles du menu horizontal.
    /// Périmètre strict : la présente exception se matérialise exclusivement dans
    /// les quatre méthodes privées <see cref="NavigateLanguageAsync"/>,
    /// <see cref="NavigateUserAsync"/>, <see cref="NavigateAppInfoAsync"/>,
    /// <see cref="NavigateMessagesAsync"/>. Hors ces quatre sites, R-4.12.2 reste
    /// pleinement applicable. La numérotation <c>NN</c> sera attribuée au moment
    /// de l'intégration de la présente exception dans le 0230 / 0231 (section
    /// 5.11 du 0230 et 0232-VI-VM à venir).</para>
    /// <para>Exception architecturale propre 2 — EA-NN-FilSecuriteVmBanniere.
    /// Le présent ViewModel consomme directement <see cref="IU_LogAndNotify"/> au
    /// titre d'un filet de sécurité de dernier recours au bord de chaque
    /// commande WPF, en dérogation à R-4.7.14 qui réserve habituellement cette
    /// consommation aux UseCases. Justification : parallèle exact à
    /// <c>EA-NN-FilSecuriteShell</c> déjà documentée pour <c>MainWindow</c>, au
    /// titre de §4.7.4 amendée. Le présent ViewModel étant hors familles
    /// canoniques, il ne dispose pas du filet <c>ExecuteSafeAsync</c> de
    /// <c>VM_Page_Generic</c> (EA-01) ; le filet est donc reproduit localement,
    /// au patron canonique parallèle à <c>MainWindow.OnLoadedHandler</c>. La clé
    /// dictionnaire utilisée est <c>"No_EC_03"</c> (exception non classifiée
    /// localement) ; le paramètre <c>ct</c> passé à
    /// <see cref="IU_LogAndNotify.ExecuteAsync"/> est systématiquement
    /// <see cref="CancellationToken.None"/>, posture parallèle à celle de
    /// <c>MainWindow</c>. Périmètre strict : la présente exception se matérialise
    /// exclusivement dans les cinq méthodes privées <c>async Task</c> câblées aux
    /// commandes WPF (<see cref="NavigateLanguageAsync"/>,
    /// <see cref="NavigateUserAsync"/>, <see cref="NavigateAppInfoAsync"/>,
    /// <see cref="NavigateMessagesAsync"/>, <see cref="CloseAppAsync"/>). La
    /// numérotation <c>NN</c> sera attribuée au moment de l'intégration de la
    /// présente exception dans le 0230 / 0231.</para>
    /// <para>Exception architecturale propre 3 — EA-NN-PollingVmBanniere.
    /// Le présent ViewModel héberge trois boucles de polling asynchrones
    /// (<see cref="UnreadMessagesPollingLoopAsync"/>,
    /// <see cref="DatabaseConnectivityPollingLoopAsync"/> et
    /// <see cref="CloseCommandPollingLoopAsync"/>), amorcées en
    /// fire-and-forget par leurs trois méthodes d'amorçage idempotent dédiées
    /// (<see cref="TryStartUnreadMessagesPollingLoop"/>,
    /// <see cref="TryStartDatabaseConnectivityPollingLoop"/> et
    /// <see cref="TryStartCloseCommandPollingLoop"/>). Cette posture
    /// constitue une dérogation à la convention implicite d'absence de boucles
    /// d'exécution périodique au sein d'un ViewModel — aucun précédent dans les
    /// ViewModels de page du projet, dont la durée de vie Scoped à la page rend
    /// le pattern inapplicable. Justification : la portée Singleton du présent
    /// ViewModel, alignée sur la durée de vie du processus, en fait le siège
    /// naturel des trois boucles, dont la cadence est portée par les Settings
    /// applicatifs (<see cref="ISE_App.MessageCheckDelay"/>,
    /// <see cref="ISE_App.DatabaseCheckInterval"/>,
    /// <see cref="ISE_App.CloseCommandDelay"/>). L'annulation coopérative
    /// est portée par le CTS local du ViewModel (<c>_cts.Token</c>) et propagée
    /// à chaque invocation interne via <see cref="IS_UseCaseInvoker"/>.
    /// Périmètre strict : la présente exception se matérialise exclusivement
    /// dans les trois méthodes asynchrones de boucle et dans leurs trois amorces
    /// synchrones idempotentes. La numérotation <c>NN</c> sera attribuée au
    /// moment de l'intégration de la présente exception dans le 0230 / 0231 (et
    /// du 0232-VI-VM à venir).</para>
    /// <para>Exception architecturale propre 4 — EA-NN-AbonnementEventVmBanniere.
    /// Le présent ViewModel s'abonne directement à l'événement applicatif système
    /// <see cref="ISE_App.ConnectionLost"/> dans son constructeur et réagit dans
    /// son handler <see cref="OnConnectionLost"/> par l'invocation d'un UseCase
    /// Scoped (<see cref="IU_DigitTryDb_RecoverConnection"/>) médiée par
    /// <see cref="IS_UseCaseInvoker"/>. L'invocation Singleton → Scoped via
    /// <see cref="IS_UseCaseInvoker"/> est elle-même documentée à EA-11 (§4.10.10
    /// du 0230), mobilisée par ailleurs dans <see cref="CloseAppAsync"/> et dans
    /// les deux boucles de polling au titre de <c>EA-NN-PollingVmBanniere</c>.
    /// L'inédit doctrinal porté par la présente exception est le chaînage
    /// abonnement à un événement applicatif système → invocation Scoped depuis
    /// un VM Singleton ; aucun précédent dans les ViewModels du projet.
    /// Justification : la portée Singleton du présent ViewModel en fait le siège
    /// naturel d'un abonnement d'horizon applicatif, et le chaînage avec
    /// l'invoker permet de respecter strictement la séparation Singleton /
    /// Scoped sans captive dependency. Périmètre strict : la présente exception
    /// se matérialise exclusivement par l'abonnement
    /// <c>_seApp.ConnectionLost += OnConnectionLost</c> au constructeur et par
    /// le corps du handler <see cref="OnConnectionLost"/>. La numérotation
    /// <c>NN</c> sera attribuée au moment de l'intégration de la présente
    /// exception dans le 0230 / 0231 (et du 0232-VI-VM à venir).</para>
    /// <para>Exception architecturale propre 5 — EA-NN-GatingReentranceVmBanniere.
    /// Le présent ViewModel porte un drapeau privé <c>_recoveryInProgress</c>
    /// dédié au gating de réentrance applicative du handler
    /// <see cref="OnConnectionLost"/>. Ce drapeau prévient la concurrence de
    /// deux procédures de récupération simultanées, qui résulterait d'une double
    /// émission rapprochée de <see cref="ISE_App.ConnectionLost"/>. Cette posture
    /// constitue une dérogation à la convention implicite d'absence de
    /// coordination applicative explicite au sein d'un ViewModel — pattern inédit
    /// dans les ViewModels du projet. Justification : la portée Singleton du
    /// présent ViewModel, conjuguée à la nature applicative système de
    /// l'événement source, motive l'introduction d'un mécanisme de coordination
    /// local. Périmètre strict : la présente exception se matérialise
    /// exclusivement par la déclaration du champ <c>_recoveryInProgress</c> et
    /// son usage dans le handler <see cref="OnConnectionLost"/> (lecture en
    /// garde, écriture à <see langword="true"/> avant l'invocation, remise à
    /// <see langword="false"/> en <c>finally</c>). La numérotation <c>NN</c>
    /// sera attribuée au moment de l'intégration de la présente exception dans
    /// le 0230 / 0231 (et du 0232-VI-VM à venir).</para>
    /// <para>Exception architecturale propre 6 — EA-NN-FilSecuriteOnConnectionLost.
    /// Le présent ViewModel reproduit le pattern de filet de sécurité ultime de
    /// <c>EA-NN-FilSecuriteVmBanniere</c> au bord du handler d'événement
    /// applicatif système <see cref="OnConnectionLost"/>. Le pattern est
    /// strictement parallèle (try / catch <see cref="OperationCanceledException"/>
    /// silencieux / catch <see cref="Exception"/> →
    /// <see cref="IU_LogAndNotify"/>, clé <c>"No_EC_03"</c>,
    /// <c>ct: CancellationToken.None</c>), à la seule différence
    /// <c>notify: false</c> (vs <c>notify: true</c> sur
    /// <c>EA-NN-FilSecuriteVmBanniere</c>). Justification de l'asymétrie : la
    /// sortie utilisateur de la procédure de récupération est portée
    /// nativement par le pipeline interne de
    /// <see cref="IU_DigitTryDb_RecoverConnection"/> ; une notification
    /// opérateur supplémentaire au site VM serait redondante et confusionnante.
    /// La journalisation reste, en revanche, indispensable pour préserver la
    /// trace d'incident résiduel. Périmètre strict : la présente exception se
    /// matérialise exclusivement au bord du handler
    /// <see cref="OnConnectionLost"/>. La numérotation <c>NN</c> sera attribuée
    /// au moment de l'intégration de la présente exception dans le 0230 / 0231
    /// (et du 0232-VI-VM à venir).</para>
    /// </remarks>
    public class VM_Banner : INotifyPropertyChanged
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom dynamique de la classe courante, premier maillon de la
        /// <c>callChain</c> construite par chaque méthode privée (§4.5 du 0230).
        /// </summary>
        private readonly string _callee;

        /// <summary>
        /// Source de jeton d'annulation coopérative locale au ViewModel,
        /// conformément à la doctrine §4.6.5 du 0230 (chaque point d'entrée porte
        /// son propre CTS).
        /// </summary>
        /// <remarks>
        /// <para>Non libéré explicitement : le ViewModel est Singleton et sa durée
        /// de vie est alignée sur celle du processus, qui assure naturellement la
        /// libération. Posture parallèle à celle adoptée pour les Settings
        /// Singleton de l'écosystème.</para>
        /// </remarks>
        private readonly CancellationTokenSource _cts;

        // --- Backing fields INPC ---

        private string _userFullName = string.Empty;
        private bool _isBannerVisible;
        private bool _hasUnreadMessages;
        private Uri _languageFlagUri;
        private bool _isConnected;

        // --- Drapeaux applicatifs internes ---

        /// <summary>
        /// Drapeau d'idempotence d'amorçage de la boucle de polling des messages
        /// non lus.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Levé une seule fois lors du premier amorçage effectif
        /// de <see cref="UnreadMessagesPollingLoopAsync"/>, garantissant qu'aucune
        /// seconde boucle ne soit lancée ultérieurement même en cas d'invocations
        /// répétées de <see cref="TryStartUnreadMessagesPollingLoop"/>. L'amorçage
        /// est tenté à deux sites : (a) fin du constructeur si
        /// <c>_isBannerVisible</c> est déjà vrai à l'instanciation ; (b) dans
        /// <see cref="OnSeUserPropertyChanged"/> sur toute notification
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> de
        /// <see cref="IsBannerVisible"/>. Posture d'idempotence par drapeau,
        /// sans logique de comparaison d'état précédente.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// </remarks>
        private bool _unreadLoopStarted;

        /// <summary>
        /// Drapeau d'idempotence d'amorçage de la boucle de polling de la
        /// connectivité base de données.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Levé une seule fois lors de l'amorçage effectif de
        /// <see cref="DatabaseConnectivityPollingLoopAsync"/>, garantissant
        /// qu'aucune seconde boucle ne soit lancée ultérieurement. L'amorçage
        /// est tenté à un site unique : fin du constructeur, sans condition.
        /// Le drapeau est défensif vis-à-vis de toute évolution future du
        /// nombre de sites d'appel.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// </remarks>
        private bool _connectivityLoopStarted;

        /// <summary>
        /// Drapeau de gating de réentrance applicative du handler
        /// <see cref="OnConnectionLost"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Levé à <see langword="true"/> en entrée du handler
        /// (après les gardes de race condition) et remis à <see langword="false"/>
        /// en bloc <c>finally</c> à la sortie du handler. Une seconde émission
        /// rapprochée de <see cref="ISE_App.ConnectionLost"/> trouve le drapeau
        /// levé et sort silencieusement, évitant le chaînage concurrent de deux
        /// procédures de récupération.</para>
        /// <para>Périmètre EA-NN-GatingReentranceVmBanniere : Voir les remarques
        /// de classe.</para>
        /// </remarks>
        private bool _recoveryInProgress;

        /// <summary>
        /// Drapeau d'idempotence d'amorçage de la boucle de polling des ordres
        /// de fermeture applicative.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Levé une seule fois lors de l'amorçage effectif de
        /// <see cref="CloseCommandPollingLoopAsync"/>, garantissant qu'aucune
        /// seconde boucle ne soit lancée ultérieurement. L'amorçage est tenté à
        /// un site unique : fin du constructeur, sans condition. Le drapeau est
        /// défensif vis-à-vis de toute évolution future du nombre de sites
        /// d'appel.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// </remarks>
        private bool _closeCommandLoopStarted;

        #endregion

        #region === Dépendances privées ===

        /// <summary>Setting Singleton centralisant l'état applicatif global ; source
        /// de <see cref="HasUnreadMessages"/> et émetteur de la notification INPC
        /// correspondante.</summary>
        private readonly ISE_App _seApp;

        /// <summary>Setting Singleton centralisant le contexte utilisateur courant ;
        /// source de <see cref="UserFullName"/> et de <see cref="IsBannerVisible"/>
        /// (dérivée de <see cref="ISE_User.AppUserId"/>), et émetteur des
        /// notifications INPC correspondantes.</summary>
        private readonly ISE_User _seUser;

        /// <summary>Setting Singleton centralisant le drapeau de langue actif ;
        /// source de <see cref="LanguageFlagUri"/> et émetteur de la notification
        /// INPC correspondante.</summary>
        private readonly ISE_Flag _seFlag;

        /// <summary>UseCase orchestrateur de navigation WPF. Consommé par les
        /// quatre commandes de navigation de la bannière au titre de l'exception
        /// architecturale propre <c>EA-NN-NavigationDirecteBanniere</c>.</summary>
        private readonly IU_Navigation _uNavigation;

        /// <summary>Service créateur de scope d'injection de dépendances par
        /// invocation (EA-11, §4.10.10 du 0230). Singleton stateless, consommé par
        /// <see cref="CloseAppAsync"/> pour résoudre et invoquer
        /// <see cref="IU_CloseApplication"/> au sein d'un scope DI éphémère, en
        /// contournement de la captive dependency Singleton VM → Scoped UC qui
        /// résulterait d'une injection directe du UseCase Scoped dans le présent
        /// consommateur Singleton.</summary>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        /// <summary>Pipeline terminal de gestion d'erreurs applicatives. Consommé
        /// au bord de chacune des cinq méthodes privées <c>async Task</c> câblées
        /// aux commandes WPF, au titre de l'exception architecturale propre
        /// <c>EA-NN-FilSecuriteVmBanniere</c>.</summary>
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Propriétés publiques ===

        // --- Propriétés observables (4) ---

        /// <summary>
        /// Obtient le nom complet de l'utilisateur applicatif courant, destiné au
        /// binding du libellé du bouton Utilisateur de la bannière.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable miroir INPC de
        /// <see cref="ISE_User.AppUserFullName"/>. L'accesseur en écriture est
        /// privé : la valeur ne peut être modifiée qu'à travers le handler
        /// <see cref="OnSeUserPropertyChanged"/> qui passe par
        /// <see cref="SetField{T}"/>.</para>
        /// </remarks>
        public string UserFullName
        {
            get => _userFullName;
            private set => SetField(ref _userFullName, value);
        }

        /// <summary>
        /// Obtient une valeur indiquant si la bannière doit être visible — vrai
        /// lorsqu'un utilisateur est connecté (<see cref="ISE_User.AppUserId"/>
        /// strictement supérieur à zéro).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable dérivée INPC de
        /// <see cref="ISE_User.AppUserId"/>. Consommée par <c>Banner</c>
        /// pour conditionner la visibilité du composant visuel par binding sur sa
        /// propriété <c>Visibility</c> via un convertisseur WPF natif
        /// <c>BooleanToVisibilityConverter</c> ou équivalent. L'accesseur en
        /// écriture est privé : la valeur ne peut être modifiée qu'à travers le
        /// handler <see cref="OnSeUserPropertyChanged"/>.</para>
        /// </remarks>
        public bool IsBannerVisible
        {
            get => _isBannerVisible;
            private set => SetField(ref _isBannerVisible, value);
        }

        /// <summary>
        /// Obtient un indicateur signalant la présence de messages non lus pour
        /// l'utilisateur courant, destiné au binding conditionnel de l'icône du
        /// bouton Messages de la bannière.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable miroir INPC de
        /// <see cref="ISE_App.HasUnreadMessages"/>. Consommée par
        /// <c>Banner</c> pour basculer entre <see cref="MessageIconUri"/>
        /// (état standard) et <see cref="MessageUnreadIconUri"/> (état messages
        /// non lus). L'accesseur en écriture est privé.</para>
        /// </remarks>
        public bool HasUnreadMessages
        {
            get => _hasUnreadMessages;
            private set => SetField(ref _hasUnreadMessages, value);
        }

        /// <summary>
        /// Obtient l'URI du drapeau de langue actif, destinée au binding de
        /// l'icône du bouton Langue de la bannière.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable miroir INPC de
        /// <see cref="ISE_Flag.AppFlagUri"/>. Consommée par <c>Banner</c>
        /// par binding sur la propriété <c>Source</c> d'un contrôle
        /// <see cref="System.Windows.Controls.Image"/> via le convertisseur WPF
        /// natif <see cref="Uri"/> → <see cref="System.Windows.Media.ImageSource"/>.
        /// Aucun convertisseur applicatif n'est nécessaire. L'accesseur en
        /// écriture est privé.</para>
        /// </remarks>
        public Uri LanguageFlagUri
        {
            get => _languageFlagUri;
            private set => SetField(ref _languageFlagUri, value);
        }

        /// <summary>
        /// Obtient une valeur indiquant l'état courant de la connectivité base
        /// de données — vrai lorsque la base est accessible, faux lorsqu'elle ne
        /// l'est plus.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable miroir INPC de
        /// <see cref="ISE_App.IsConnected"/>. Consommée par <c>Banner</c> pour
        /// basculer entre <see cref="AppCloseConnectedIconUri"/> (état connecté)
        /// et <see cref="AppCloseDisconnectedIconUri"/> (état déconnecté) au sein
        /// du bloc visuel du bouton Fermer. La valeur initiale est lue dans le
        /// constructeur depuis <see cref="ISE_App.IsConnected"/> ; les mises à
        /// jour ultérieures sont relayées par <see cref="OnSeAppPropertyChanged"/>
        /// — branche <c>nameof(ISE_App.IsConnected)</c>. L'accesseur en écriture
        /// est privé.</para>
        /// <para>Note sur l'ordre canonique côté <see cref="SE_App"/> : la
        /// mécanique <c>NotifyConnectionLost</c> de <see cref="SE_App"/> émet
        /// d'abord la notification INPC sur <see cref="ISE_App.IsConnected"/>
        /// puis lève l'événement applicatif système
        /// <see cref="ISE_App.ConnectionLost"/>. Le miroir local est donc à jour
        /// avant l'invocation du handler <see cref="OnConnectionLost"/> sur la
        /// même pile d'exécution — le binding XAML basculant l'icône Fermer est
        /// déjà à jour au moment où la procédure de récupération démarre.</para>
        /// </remarks>
        public bool IsConnected
        {
            get => _isConnected;
            private set => SetField(ref _isConnected, value);
        }

        // --- Propriétés immuables (3) ---

        /// <summary>
        /// Obtient l'URI de l'icône standard du bouton Messages de la bannière,
        /// affichée en l'absence de messages non lus.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable immuable initialisée au
        /// constructeur à partir de <see cref="RS_Icons.IconEmail_Source"/>.
        /// Lecture directe du référentiel statique intra-couche conformément à
        /// R-2.5.7 (un RS_ reste strictement intra-couche ; <see cref="RS_Icons"/>
        /// réside lui-même en <c>D_Presentation</c>).</para>
        /// </remarks>
        public Uri MessageIconUri { get; }

        /// <summary>
        /// Obtient l'URI de l'icône du bouton Messages de la bannière, affichée
        /// en présence de messages non lus.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable immuable initialisée au
        /// constructeur à partir de <see cref="RS_Icons.IconEmailNotRead_Source"/>.
        /// Le binding XAML bascule entre <see cref="MessageIconUri"/> et la
        /// présente propriété en fonction de la valeur de
        /// <see cref="HasUnreadMessages"/>.</para>
        /// </remarks>
        public Uri MessageUnreadIconUri { get; }

        /// <summary>
        /// Obtient l'URI de l'icône du bouton Fermer de la bannière, affichée
        /// lorsque la connectivité base de données est nominale.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable immuable initialisée au
        /// constructeur à partir de <see cref="RS_Icons.AppCloseBlue_Source"/>.
        /// Le binding XAML bascule entre la présente propriété et
        /// <see cref="AppCloseDisconnectedIconUri"/> en fonction de la valeur
        /// de <see cref="IsConnected"/>.</para>
        /// </remarks>
        public Uri AppCloseConnectedIconUri { get; }

        /// <summary>
        /// Obtient l'URI de l'icône du bouton Fermer de la bannière, affichée
        /// lorsque la connectivité base de données est perdue.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété bindable immuable initialisée au
        /// constructeur à partir de <see cref="RS_Icons.AppDisconnected_Source"/>.
        /// Le binding XAML bascule entre <see cref="AppCloseConnectedIconUri"/>
        /// et la présente propriété en fonction de la valeur de
        /// <see cref="IsConnected"/>.</para>
        /// </remarks>
        public Uri AppCloseDisconnectedIconUri { get; }

        // --- Commandes (5) ---

        /// <summary>
        /// Commande WPF déclenchant la navigation vers la page de gestion de la
        /// langue applicative (<c>Page91</c>).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur <see cref="UT_RelayCommandArg0Async"/>,
        /// déléguant à la méthode privée <see cref="NavigateLanguageAsync"/>. Voir
        /// l'exception architecturale propre <c>EA-NN-NavigationDirecteBanniere</c>
        /// documentée en remarques de classe.</para>
        /// </remarks>
        public ICommand NavigateLanguageCommand { get; }

        /// <summary>
        /// Commande WPF déclenchant la navigation vers la page de gestion de
        /// l'utilisateur applicatif (<c>Page90</c>).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur <see cref="UT_RelayCommandArg0Async"/>,
        /// déléguant à la méthode privée <see cref="NavigateUserAsync"/>. Voir
        /// l'exception architecturale propre <c>EA-NN-NavigationDirecteBanniere</c>
        /// documentée en remarques de classe.</para>
        /// </remarks>
        public ICommand NavigateUserCommand { get; }

        /// <summary>
        /// Commande WPF déclenchant la navigation vers la page d'informations
        /// applicatives (<c>Page98</c>).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur <see cref="UT_RelayCommandArg0Async"/>,
        /// déléguant à la méthode privée <see cref="NavigateAppInfoAsync"/>. Voir
        /// l'exception architecturale propre <c>EA-NN-NavigationDirecteBanniere</c>
        /// documentée en remarques de classe.</para>
        /// </remarks>
        public ICommand NavigateAppInfoCommand { get; }

        /// <summary>
        /// Commande WPF déclenchant la navigation vers la page de gestion des
        /// messages utilisateur (<c>Page96</c>).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur <see cref="UT_RelayCommandArg0Async"/>,
        /// déléguant à la méthode privée <see cref="NavigateMessagesAsync"/>. Voir
        /// l'exception architecturale propre <c>EA-NN-NavigationDirecteBanniere</c>
        /// documentée en remarques de classe.</para>
        /// </remarks>
        public ICommand NavigateMessagesCommand { get; }

        /// <summary>
        /// Commande WPF déclenchant la procédure de fermeture applicative en
        /// mode confirmation.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Câblée sur <see cref="UT_RelayCommandArg0Async"/>,
        /// déléguant à la méthode privée <see cref="CloseAppAsync"/> qui invoque
        /// <see cref="IS_UseCaseInvoker.InvokeAsync{TUseCase, TResult}"/>, lequel
        /// résout et invoque <see cref="IU_CloseApplication.ExecuteAsync"/> au sein
        /// d'un scope DI éphémère, avec les paramètres
        /// <c>confirmation: true, warning: false, delaySeconds: 0</c> — mode
        /// confirmation de la matrice à quatre modes documentée par
        /// <see cref="IU_CloseApplication"/>.</para>
        /// </remarks>
        public ICommand CloseAppCommand { get; }

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        /// <summary>
        /// Émis lorsqu'une propriété observable du ViewModel change de valeur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Mécanisme INPC standard exploité par les
        /// bindings WPF de <c>Banner</c>.</para>
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_Banner"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur d'injection
        /// de dépendances lors de la résolution du Singleton. L'instance est
        /// ensuite consommée par <c>Banner</c> qui l'affecte à son
        /// <see cref="System.Windows.FrameworkElement.DataContext"/>.</para>
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item>Résolution et stockage des six dépendances avec gardes
        ///   <see cref="ArgumentNullException"/>, dans l'ordre fonctionnel
        ///   Settings → UseCase d'orchestration directe (<see cref="IU_Navigation"/>)
        ///   → Service d'orchestration indirecte (<see cref="IS_UseCaseInvoker"/>)
        ///   → UseCase de pipeline terminal (<see cref="IU_LogAndNotify"/>).</item>
        ///   <item>Initialisation des champs locaux <c>_callee</c> (à partir du nom
        ///   de type — cohérence avec la doctrine CallChain §4.5 du 0230) et
        ///   <c>_cts</c> (instanciation d'un nouveau
        ///   <see cref="CancellationTokenSource"/> local au ViewModel, §4.6.5).
        ///   Initialisation des quatre drapeaux internes <c>_unreadLoopStarted</c>,
        ///   <c>_connectivityLoopStarted</c>, <c>_recoveryInProgress</c> et
        ///   <c>_closeCommandLoopStarted</c> à <see langword="false"/>.</item>
        ///   <item>Initialisation synchrone des cinq backing fields observables
        ///   à la valeur courante des Settings, préalablement au branchement des
        ///   abonnements, afin de garantir un état cohérent dès l'instanciation
        ///   indépendamment de l'ordre relatif d'exécution entre le constructeur
        ///   du ViewModel et toute mutation antérieure des Settings.</item>
        ///   <item>Initialisation des quatre propriétés immuables <see cref="Uri"/>
        ///   à partir du référentiel statique intra-couche <see cref="RS_Icons"/>.</item>
        ///   <item>Instanciation des cinq commandes <see cref="ICommand"/>
        ///   câblées sur leurs handlers privés <c>async Task</c> respectifs via
        ///   <see cref="UT_RelayCommandArg0Async"/>.</item>
        ///   <item>Branchement des trois abonnements
        ///   <see cref="INotifyPropertyChanged.PropertyChanged"/> sur les Settings
        ///   afin de relayer toute évolution ultérieure vers les propriétés
        ///   bindables observables du ViewModel, et du quatrième abonnement à
        ///   l'événement applicatif système <see cref="ISE_App.ConnectionLost"/>
        ///   du Setting <see cref="ISE_App"/>.</item>
        ///   <item>Amorçage idempotent de la boucle de polling de connectivité
        ///   via <see cref="TryStartDatabaseConnectivityPollingLoop"/> et de la
        ///   boucle de polling des ordres de fermeture applicative via
        ///   <see cref="TryStartCloseCommandPollingLoop"/>, toutes deux
        ///   inconditionnellement, puis amorçage idempotent conditionnel de la
        ///   boucle de polling des messages non lus via
        ///   <see cref="TryStartUnreadMessagesPollingLoop"/> si
        ///   <c>_isBannerVisible</c> est déjà vrai à l'instanciation. Les trois
        ///   boucles sont lancées en fire-and-forget ; les amorces sont
        ///   garanties idempotentes par drapeaux dédiés.</item>
        /// </list>
        /// <para>Filet de sécurité : Aucune invocation asynchrone ni
        /// opération susceptible de lever une exception terminale n'est portée par
        /// le constructeur ; les abonnements à
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> n'engagent pas de
        /// risque exploitable par <c>try/catch</c>. Aucune intervention de
        /// <see cref="IU_LogAndNotify"/> n'est donc requise dans le périmètre du
        /// constructeur — le filet de sécurité ultime
        /// <c>EA-NN-FilSecuriteVmBanniere</c> se matérialise exclusivement au bord
        /// des cinq méthodes privées câblées aux commandes WPF.</para>
        /// </remarks>
        /// <param name="seApp">Setting Singleton centralisant l'état applicatif
        /// global, source de <see cref="HasUnreadMessages"/>. Injecté en Singleton
        /// par le conteneur DI.</param>
        /// <param name="seUser">Setting Singleton centralisant le contexte
        /// utilisateur courant, source de <see cref="UserFullName"/> et de
        /// <see cref="IsBannerVisible"/>. Injecté en Singleton par le conteneur DI.</param>
        /// <param name="seFlag">Setting Singleton centralisant le drapeau de langue
        /// actif, source de <see cref="LanguageFlagUri"/>. Injecté en Singleton
        /// par le conteneur DI.</param>
        /// <param name="uNavigation">UseCase orchestrateur de la navigation WPF.
        /// Consommé par les quatre commandes de navigation de la bannière.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="useCaseInvoker">Service créateur de scope DI par
        /// invocation (EA-11, §4.10.10 du 0230). Consommé par
        /// <see cref="CloseAppAsync"/> pour résoudre et invoquer
        /// <see cref="IU_CloseApplication"/> au sein d'un scope éphémère, en
        /// contournement de la captive dependency Singleton VM → Scoped UC.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="logAndNotify">Pipeline terminal de gestion d'erreurs —
        /// filet de sécurité ultime au bord de chacune des cinq méthodes privées
        /// câblées aux commandes WPF. Injecté en Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si l'une des dépendances
        /// injectées est <see langword="null"/>.</exception>
        public VM_Banner(
            ISE_App seApp,
            ISE_User seUser,
            ISE_Flag seFlag,
            IU_Navigation uNavigation,
            IS_UseCaseInvoker useCaseInvoker,
            IU_LogAndNotify logAndNotify)
        {
            // (1) Résolution des six dépendances avec gardes ArgumentNullException
            //     (ordre fonctionnel : Settings → UseCases d'orchestration → UseCase de pipeline terminal).
            _seApp = seApp ?? throw new ArgumentNullException(nameof(seApp));
            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
            _seFlag = seFlag ?? throw new ArgumentNullException(nameof(seFlag));
            _uNavigation = uNavigation ?? throw new ArgumentNullException(nameof(uNavigation));
            _useCaseInvoker = useCaseInvoker ?? throw new ArgumentNullException(nameof(useCaseInvoker));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));

            // (2) Initialisation des champs locaux _callee et _cts, et des trois drapeaux
            //     internes d'idempotence et de gating de réentrance à false.
            _callee = GetType().Name;
            _cts = new CancellationTokenSource();
            _unreadLoopStarted = false;
            _connectivityLoopStarted = false;
            _recoveryInProgress = false;
            _closeCommandLoopStarted = false;

            // (3) Initialisation synchrone des cinq backing fields observables
            //     depuis les valeurs courantes des Settings.
            _userFullName = _seUser.AppUserFullName;
            _isBannerVisible = _seUser.AppUserId > 0;
            _hasUnreadMessages = _seApp.HasUnreadMessages;
            _languageFlagUri = _seFlag.AppFlagUri;
            _isConnected = _seApp.IsConnected;

            // (4) Initialisation des quatre propriétés immuables Uri depuis RS_Icons.
            MessageIconUri = RS_Icons.IconEmail_Source;
            MessageUnreadIconUri = RS_Icons.IconEmailNotRead_Source;
            AppCloseConnectedIconUri = RS_Icons.AppCloseBlue_Source;
            AppCloseDisconnectedIconUri = RS_Icons.AppDisconnected_Source;

            // (5) Instanciation des cinq commandes ICommand câblées sur leurs
            //     handlers privés async Task respectifs.
            NavigateLanguageCommand = new UT_RelayCommandArg0Async(NavigateLanguageAsync);
            NavigateUserCommand = new UT_RelayCommandArg0Async(NavigateUserAsync);
            NavigateAppInfoCommand = new UT_RelayCommandArg0Async(NavigateAppInfoAsync);
            NavigateMessagesCommand = new UT_RelayCommandArg0Async(NavigateMessagesAsync);
            CloseAppCommand = new UT_RelayCommandArg0Async(CloseAppAsync);

            // (6) Branchement des trois abonnements PropertyChanged sur les Settings
            //     et du quatrième abonnement à l'événement applicatif système ConnectionLost.
            _seApp.PropertyChanged += OnSeAppPropertyChanged;
            _seUser.PropertyChanged += OnSeUserPropertyChanged;
            _seFlag.PropertyChanged += OnSeFlagPropertyChanged;
            _seApp.ConnectionLost += OnConnectionLost;

            // (7) Amorçage idempotent des trois boucles de polling — les boucles de
            //     connectivité et de surveillance des ordres de fermeture sont
            //     amorcées inconditionnellement ; la boucle des messages non lus
            //     n'est amorcée à ce stade que si la bannière est déjà visible à
            //     l'instanciation (cas peu probable mais fonctionnellement légitime ;
            //     les autres cas seront traités par OnSeUserPropertyChanged à toute
            //     notification de IsBannerVisible).
            TryStartDatabaseConnectivityPollingLoop();
            TryStartCloseCommandPollingLoop();
            if (_isBannerVisible)
            {
                TryStartUnreadMessagesPollingLoop();
            }
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes privées ===

        // --- Handlers asynchrones des commandes WPF (patron canonique §9) ---

        /// <summary>
        /// Handler asynchrone câblé à <see cref="NavigateLanguageCommand"/> —
        /// délègue à <see cref="IU_Navigation.NavigateToPageAsync"/> pour la
        /// navigation vers la page <c>Page02</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="UT_RelayCommandArg0Async.ExecuteAsync"/> lorsque
        /// l'utilisateur active le bouton Langue de la bannière.</para>
        /// <para>Objectif : Construire la <c>callChain</c> locale et
        /// déléguer la navigation cible au UseCase orchestrateur en lui propageant
        /// le jeton d'annulation coopérative local <c>_cts.Token</c>.</para>
        /// <para>Périmètre EA-NN-NavigationDirecteBanniere : Cette méthode
        /// est l'un des quatre sites de matérialisation de l'exception
        /// architecturale propre <c>EA-NN-NavigationDirecteBanniere</c>, qui
        /// autorise nominativement la consommation directe de
        /// <see cref="IU_Navigation"/> depuis le présent ViewModel en dérogation
        /// à R-4.12.2.</para>
        /// <para>Périmètre EA-NN-FilSecuriteVmBanniere : Cette méthode est
        /// l'un des cinq sites de matérialisation de l'exception architecturale
        /// propre <c>EA-NN-FilSecuriteVmBanniere</c>, qui autorise nominativement
        /// la consommation directe de <see cref="IU_LogAndNotify"/> depuis le
        /// présent ViewModel en dérogation à R-4.7.14.</para>
        /// <para>Filet de sécurité ultime : Le bloc
        /// <c>try / catch (OperationCanceledException) / catch (Exception)</c>
        /// garantit qu'aucune exception ne remonte au framework WPF via
        /// <see cref="UT_RelayCommandArg0Async.Execute"/>. Le bloc
        /// <c>catch (OperationCanceledException)</c> distinct silencieux précède
        /// le <c>catch (Exception)</c> global : une annulation observée à ce
        /// stade procède d'un signal amont légitime et ne doit pas être
        /// journalisée. Le <c>catch (Exception)</c> global délègue à
        /// <see cref="IU_LogAndNotify"/> avec la clé canonique <c>"No_EC_03"</c>
        /// et <c>notify: true</c> — une défaillance de navigation déclenchée par
        /// une action utilisateur justifie une notification opérateur. Le
        /// paramètre <c>ct</c> passé à
        /// <see cref="IU_LogAndNotify.ExecuteAsync"/> est
        /// <see cref="CancellationToken.None"/>, posture parallèle à
        /// <c>MainWindow.OnLoadedHandler</c>.</para>
        /// </remarks>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// navigation.</returns>
        private async Task NavigateLanguageAsync()
        {
            string callChain = $"{_callee} > {nameof(NavigateLanguageAsync)}";

            try
            {
                await _uNavigation.NavigateToPageAsync(callChain, "Page02", _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Annulation amont légitime — silencieux, pas de journalisation.
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: true,
                    ct: CancellationToken.None);
            }
        }

        /// <summary>
        /// Handler asynchrone câblé à <see cref="NavigateUserCommand"/> — délègue
        /// à <see cref="IU_Navigation.NavigateToPageAsync"/> pour la navigation
        /// vers la page <c>Page01</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="UT_RelayCommandArg0Async.ExecuteAsync"/> lorsque
        /// l'utilisateur active le bouton Utilisateur de la bannière.</para>
        /// <para>Périmètre EA-NN-NavigationDirecteBanniere : Voir la
        /// documentation de <see cref="NavigateLanguageAsync"/>.</para>
        /// <para>Périmètre EA-NN-FilSecuriteVmBanniere : Voir la
        /// documentation de <see cref="NavigateLanguageAsync"/>.</para>
        /// <para>Filet de sécurité ultime : Identique à
        /// <see cref="NavigateLanguageAsync"/>.</para>
        /// </remarks>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// navigation.</returns>
        private async Task NavigateUserAsync()
        {
            string callChain = $"{_callee} > {nameof(NavigateUserAsync)}";

            try
            {
                await _uNavigation.NavigateToPageAsync(callChain, "Page01", _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Annulation amont légitime — silencieux, pas de journalisation.
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: true,
                    ct: CancellationToken.None);
            }
        }

        /// <summary>
        /// Handler asynchrone câblé à <see cref="NavigateAppInfoCommand"/> —
        /// délègue à <see cref="IU_Navigation.NavigateToPageAsync"/> pour la
        /// navigation vers la page <c>Page98</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="UT_RelayCommandArg0Async.ExecuteAsync"/> lorsque
        /// l'utilisateur active le bouton Informations de la bannière.</para>
        /// <para>Périmètre EA-NN-NavigationDirecteBanniere : Voir la
        /// documentation de <see cref="NavigateLanguageAsync"/>.</para>
        /// <para>Périmètre EA-NN-FilSecuriteVmBanniere : Voir la
        /// documentation de <see cref="NavigateLanguageAsync"/>.</para>
        /// <para>Filet de sécurité ultime : Identique à
        /// <see cref="NavigateLanguageAsync"/>.</para>
        /// </remarks>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// navigation.</returns>
        private async Task NavigateAppInfoAsync()
        {
            string callChain = $"{_callee} > {nameof(NavigateAppInfoAsync)}";

            try
            {
                await _uNavigation.NavigateToPageAsync(callChain, "Page98", _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Annulation amont légitime — silencieux, pas de journalisation.
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: true,
                    ct: CancellationToken.None);
            }
        }

        /// <summary>
        /// Handler asynchrone câblé à <see cref="NavigateMessagesCommand"/> —
        /// délègue à <see cref="IU_Navigation.NavigateToPageAsync"/> pour la
        /// navigation vers la page <c>Page03</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="UT_RelayCommandArg0Async.ExecuteAsync"/> lorsque
        /// l'utilisateur active le bouton Messages de la bannière.</para>
        /// <para>Périmètre EA-NN-NavigationDirecteBanniere : Voir la
        /// documentation de <see cref="NavigateLanguageAsync"/>.</para>
        /// <para>Périmètre EA-NN-FilSecuriteVmBanniere : Voir la
        /// documentation de <see cref="NavigateLanguageAsync"/>.</para>
        /// <para>Filet de sécurité ultime : Identique à
        /// <see cref="NavigateLanguageAsync"/>.</para>
        /// </remarks>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// navigation.</returns>
        private async Task NavigateMessagesAsync()
        {
            string callChain = $"{_callee} > {nameof(NavigateMessagesAsync)}";

            try
            {
                await _uNavigation.NavigateToPageAsync(callChain, "Page03", _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Annulation amont légitime — silencieux, pas de journalisation.
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: true,
                    ct: CancellationToken.None);
            }
        }

        /// <summary>
        /// Handler asynchrone câblé à <see cref="CloseAppCommand"/> — délègue à
        /// <see cref="IU_CloseApplication.ExecuteAsync"/> en mode confirmation,
        /// par l'intermédiaire de <see cref="IS_UseCaseInvoker"/> au sein d'un
        /// scope DI éphémère.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="UT_RelayCommandArg0Async.ExecuteAsync"/> lorsque
        /// l'utilisateur active le bouton Fermer de la bannière. L'invocation est
        /// faite en mode confirmation (<c>confirmation: true, warning: false,
        /// delaySeconds: 0</c>) — une demande de confirmation utilisateur précède
        /// toute déconnexion de session et fermeture WPF effective, conformément
        /// à la matrice à quatre modes documentée par
        /// <see cref="IU_CloseApplication"/>.</para>
        /// <para>Objectif : Construire la <c>callChain</c> locale et
        /// déléguer la procédure de fermeture au UseCase orchestrateur en lui
        /// propageant le jeton d'annulation coopérative local <c>_cts.Token</c>.
        /// La valeur de retour <c>En_CloseResult</c> est volontairement non
        /// consommée par le ViewModel — le positionnement éventuel de
        /// <c>CancelEventArgs.Cancel</c> côté présentation est de la
        /// responsabilité du consommateur de l'événement <c>OnClosing</c> de la
        /// fenêtre principale (cf. <c>MainWindow</c>), distinct du présent
        /// site.</para>
        /// <para>Médiation par invoker (EA-11, §4.10.10 du 0230) :
        /// L'invocation de <see cref="IU_CloseApplication.ExecuteAsync"/> est
        /// médiatisée par <see cref="IS_UseCaseInvoker"/> qui matérialise un
        /// <c>IServiceScope</c> distinct par invocation, résout le UseCase Scoped
        /// dans ce scope, exécute le lambda fourni puis dispose le scope. Cette
        /// médiation contourne la captive dependency Singleton VM → Scoped UC
        /// qui résulterait d'une injection directe du UseCase Scoped dans le
        /// présent consommateur Singleton. La construction de la <c>callChain</c>
        /// (§4.5 du 0230) est transparente vis-à-vis de l'invoker : la chaîne
        /// est capturée par closure et transmise inchangée à
        /// <see cref="IU_CloseApplication.ExecuteAsync"/> ; l'invoker n'ajoute
        /// aucun maillon. La sémantique d'annulation coopérative (§4.6 du 0230)
        /// est préservée : <c>_cts.Token</c> est passé à l'invoker et repropagé
        /// au lambda qui le transmet à
        /// <see cref="IU_CloseApplication.ExecuteAsync"/>.</para>
        /// <para>Périmètre EA-NN-FilSecuriteVmBanniere : Voir la
        /// documentation de <see cref="NavigateLanguageAsync"/>. La présente
        /// méthode ne participe pas à <c>EA-NN-NavigationDirecteBanniere</c> (qui
        /// ne couvre que les quatre méthodes de navigation).</para>
        /// <para>Filet de sécurité ultime : Identique à
        /// <see cref="NavigateLanguageAsync"/>. À noter que
        /// <see cref="IU_CloseApplication.ExecuteAsync"/> absorbe en interne les
        /// exceptions applicatives typées via son propre pipeline terminal
        /// (§4.7.4 du 0230) ; <see cref="IS_UseCaseInvoker"/> ne capture aucune
        /// exception et propage transparentement au consommateur. Le filet de
        /// sécurité du présent site ne capture donc en pratique que
        /// <see cref="OperationCanceledException"/> (propagée volontairement par
        /// le UseCase comme seule exception remontée) et les défaillances
        /// résiduelles non absorbées.</para>
        /// </remarks>
        /// <returns>Une tâche représentant l'exécution asynchrone de la procédure
        /// de fermeture.</returns>
        private async Task CloseAppAsync()
        {
            string callChain = $"{_callee} > {nameof(CloseAppAsync)}";

            try
            {
                await _useCaseInvoker.InvokeAsync<IU_CloseApplication, En_CloseResult>(
                    (uc, ct) => uc.ExecuteAsync(
                        callChain,
                        confirmation: true,
                        warning: false,
                        delaySeconds: 0,
                        ct: ct),
                    _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Annulation amont légitime — silencieux, pas de journalisation.
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: true,
                    ct: CancellationToken.None);
            }
        }

        // --- Handlers d'abonnement aux Settings observables ---

        /// <summary>
        /// Gestionnaire d'abonnement à
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> du Setting
        /// <see cref="ISE_App"/>, relayant les changements pertinents vers les
        /// propriétés bindables du ViewModel.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Invoqué automatiquement par <see cref="ISE_App"/>
        /// lors de toute mutation d'une de ses propriétés observables. Filtre les
        /// notifications pour ne propager que celles couvertes par le ViewModel
        /// (<see cref="ISE_App.HasUnreadMessages"/> et
        /// <see cref="ISE_App.IsConnected"/>).</para>
        /// <para>Objectif : Synchroniser <see cref="HasUnreadMessages"/> avec
        /// <see cref="ISE_App.HasUnreadMessages"/> et <see cref="IsConnected"/>
        /// avec <see cref="ISE_App.IsConnected"/>, déclenchant les notifications
        /// INPC correspondantes via <see cref="SetField{T}"/>.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance Singleton de
        /// <see cref="ISE_App"/>).</param>
        /// <param name="e">Arguments de l'événement portant le nom de la propriété
        /// modifiée.</param>
        private void OnSeAppPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISE_App.HasUnreadMessages))
            {
                HasUnreadMessages = _seApp.HasUnreadMessages;
            }
            else if (e.PropertyName == nameof(ISE_App.IsConnected))
            {
                IsConnected = _seApp.IsConnected;
            }
        }

        /// <summary>
        /// Gestionnaire d'abonnement à
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> du Setting
        /// <see cref="ISE_User"/>, relayant les changements pertinents vers les
        /// propriétés bindables du ViewModel.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Invoqué automatiquement par <see cref="ISE_User"/>
        /// lors de toute mutation d'une de ses propriétés observables. Filtre les
        /// notifications pour ne propager que celles couvertes par le ViewModel
        /// (<see cref="ISE_User.AppUserFullName"/> et
        /// <see cref="ISE_User.AppUserId"/>).</para>
        /// <para>Objectif : Synchroniser <see cref="UserFullName"/> avec
        /// <see cref="ISE_User.AppUserFullName"/>, et
        /// <see cref="IsBannerVisible"/> avec le prédicat
        /// <see cref="ISE_User.AppUserId"/> &gt; 0. Sur toute notification
        /// portant sur <see cref="ISE_User.AppUserId"/>, l'amorce idempotente
        /// <see cref="TryStartUnreadMessagesPollingLoop"/> est déclenchée
        /// inconditionnellement — l'idempotence est confiée au drapeau
        /// <c>_unreadLoopStarted</c>, sans logique de comparaison d'état
        /// précédente.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance Singleton de
        /// <see cref="ISE_User"/>).</param>
        /// <param name="e">Arguments de l'événement portant le nom de la propriété
        /// modifiée.</param>
        private void OnSeUserPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISE_User.AppUserFullName))
            {
                UserFullName = _seUser.AppUserFullName;
            }
            else if (e.PropertyName == nameof(ISE_User.AppUserId))
            {
                IsBannerVisible = _seUser.AppUserId > 0;
                TryStartUnreadMessagesPollingLoop();
            }
        }

        /// <summary>
        /// Gestionnaire d'abonnement à
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> du Setting
        /// <see cref="ISE_Flag"/>, relayant les changements pertinents vers les
        /// propriétés bindables du ViewModel.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Invoqué automatiquement par <see cref="ISE_Flag"/>
        /// lors de toute mutation d'une de ses propriétés observables. Filtre les
        /// notifications pour ne propager que celles couvertes par le ViewModel
        /// (<see cref="ISE_Flag.AppFlagUri"/>).</para>
        /// <para>Objectif : Synchroniser le backing field local
        /// <c>_languageFlagUri</c> avec la valeur courante de
        /// <see cref="ISE_Flag.AppFlagUri"/> et déclencher la notification INPC
        /// correspondante via <see cref="SetField{T}"/>.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance Singleton de
        /// <see cref="ISE_Flag"/>).</param>
        /// <param name="e">Arguments de l'événement portant le nom de la propriété
        /// modifiée.</param>
        private void OnSeFlagPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISE_Flag.AppFlagUri))
            {
                LanguageFlagUri = _seFlag.AppFlagUri;
            }
        }

        // --- Boucles de polling et handler d'événement applicatif système ---

        /// <summary>
        /// Tente d'amorcer la boucle de polling des messages non lus si elle ne
        /// l'a pas encore été — opération idempotente garantie par le drapeau
        /// <c>_unreadLoopStarted</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode synchrone, invoquée à deux sites :
        /// (a) à la fin du constructeur si <c>_isBannerVisible</c> est déjà vrai
        /// à l'instanciation ; (b) dans <see cref="OnSeUserPropertyChanged"/>
        /// sur toute notification <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// portant sur <see cref="IsBannerVisible"/>, inconditionnellement —
        /// l'idempotence est confiée au présent dispositif, sans logique de
        /// comparaison d'état précédente.</para>
        /// <para>Objectif : Si la boucle n'a jamais été amorcée
        /// (<c>_unreadLoopStarted</c> à <see langword="false"/>), lever le
        /// drapeau puis lancer <see cref="UnreadMessagesPollingLoopAsync"/> en
        /// fire-and-forget (sans <c>await</c>) — la boucle s'exécute en arrière-
        /// plan sur le scheduler par défaut et résiste à l'annulation
        /// coopérative via le CTS local du ViewModel. Si la boucle est déjà
        /// amorcée, sortie silencieuse.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// <para>Filet de sécurité : Le lancement en fire-and-forget délègue la
        /// gestion d'erreurs à la boucle elle-même
        /// (<see cref="UnreadMessagesPollingLoopAsync"/>) qui absorbe
        /// <see cref="OperationCanceledException"/> silencieusement et propage
        /// toute autre exception au scheduler par défaut. Une défaillance
        /// résiduelle de la boucle est, par construction, tracée dans le log
        /// du scheduler ; le présent site d'amorçage ne porte aucune
        /// responsabilité de capture.</para>
        /// </remarks>
        private void TryStartUnreadMessagesPollingLoop()
        {
            if (_unreadLoopStarted)
            {
                return;
            }

            _unreadLoopStarted = true;
            _ = UnreadMessagesPollingLoopAsync();
        }

        /// <summary>
        /// Boucle de polling asynchrone invoquant périodiquement
        /// <see cref="IU_UserAppMessage_CheckUnread"/> via
        /// <see cref="IS_UseCaseInvoker"/>, à cadence
        /// <see cref="ISE_App.MessageCheckDelay"/> secondes.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Lancée en fire-and-forget par
        /// <see cref="TryStartUnreadMessagesPollingLoop"/>. La boucle court
        /// jusqu'à annulation coopérative du CTS local du ViewModel
        /// (<c>_cts.Token</c>).</para>
        /// <para>Objectif : Maintenir
        /// <see cref="ISE_App.HasUnreadMessages"/> à jour en interrogeant
        /// périodiquement le pipeline applicatif via le UseCase Scoped
        /// <see cref="IU_UserAppMessage_CheckUnread"/>, dans un scope DI
        /// éphémère résolu par <see cref="IS_UseCaseInvoker"/>. Chaque
        /// itération applique un gating sur deux conditions cumulatives :
        /// <see cref="ISE_App.IsConnected"/> doit être vrai (base accessible)
        /// et <see cref="IsBannerVisible"/> doit être vrai (utilisateur
        /// connecté). Si l'une des deux conditions est fausse, l'itération
        /// saute l'invocation et attend la prochaine cadence — la boucle
        /// reprend nativement lorsque les deux conditions sont rétablies, sans
        /// désamorçage ni ré-amorçage.</para>
        /// <para>Médiation par invoker (EA-11, §4.10.10 du 0230) :
        /// L'invocation de <see cref="IU_UserAppMessage_CheckUnread.ExecuteAsync"/>
        /// est médiée par <see cref="IS_UseCaseInvoker"/> qui matérialise un
        /// <c>IServiceScope</c> distinct par invocation, résout le UseCase
        /// Scoped dans ce scope, exécute le lambda fourni puis dispose le
        /// scope. La <c>callChain</c> est capturée par closure et transmise
        /// inchangée ; <c>_cts.Token</c> est propagé.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// <para>Gestion d'erreurs : Le bloc
        /// <c>try / catch (OperationCanceledException)</c> permet une sortie
        /// silencieuse au signal d'annulation coopérative, sans journalisation
        /// (règle absolue §4.7.3 du 0230). Toute autre exception est laissée
        /// remonter au scheduler par défaut, qui se charge de la trace
        /// résiduelle.</para>
        /// </remarks>
        /// <returns>Une tâche représentant l'exécution de la boucle de
        /// polling.</returns>
        private async Task UnreadMessagesPollingLoopAsync()
        {
            string callChain = $"{_callee} > {nameof(UnreadMessagesPollingLoopAsync)}";

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_seApp.IsConnected && IsBannerVisible)
                    {
                        await _useCaseInvoker.InvokeAsync<IU_UserAppMessage_CheckUnread>(
                            (uc, ct) => uc.ExecuteAsync(callChain, ct),
                            _cts.Token);
                    }

                    await Task.Delay(
                        TimeSpan.FromSeconds(_seApp.MessageCheckDelay),
                        _cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Annulation coopérative — sortie silencieuse, pas de journalisation.
            }
        }

        /// <summary>
        /// Tente d'amorcer la boucle de polling de la connectivité base de
        /// données si elle ne l'a pas encore été — opération idempotente
        /// garantie par le drapeau <c>_connectivityLoopStarted</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode synchrone, invoquée à un site unique : la
        /// fin du constructeur, sans condition. Le drapeau est défensif vis-à-
        /// vis de toute évolution future du nombre de sites d'appel.</para>
        /// <para>Objectif : Si la boucle n'a jamais été amorcée
        /// (<c>_connectivityLoopStarted</c> à <see langword="false"/>), lever
        /// le drapeau puis lancer
        /// <see cref="DatabaseConnectivityPollingLoopAsync"/> en
        /// fire-and-forget. Si la boucle est déjà amorcée, sortie
        /// silencieuse.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// <para>Filet de sécurité : Identique à
        /// <see cref="TryStartUnreadMessagesPollingLoop"/>.</para>
        /// </remarks>
        private void TryStartDatabaseConnectivityPollingLoop()
        {
            if (_connectivityLoopStarted)
            {
                return;
            }

            _connectivityLoopStarted = true;
            _ = DatabaseConnectivityPollingLoopAsync();
        }

        /// <summary>
        /// Boucle de polling asynchrone invoquant périodiquement
        /// <see cref="IU_DigitTryDb_TestConnection"/> via
        /// <see cref="IS_UseCaseInvoker"/>, à cadence
        /// <see cref="ISE_App.DatabaseCheckInterval"/> secondes, sans gating.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Lancée en fire-and-forget par
        /// <see cref="TryStartDatabaseConnectivityPollingLoop"/>. La boucle
        /// court jusqu'à annulation coopérative du CTS local du ViewModel.</para>
        /// <para>Objectif : Maintenir la propriété
        /// <see cref="ISE_App.IsConnected"/> à jour en interrogeant
        /// périodiquement le pipeline applicatif via le UseCase Scoped
        /// <see cref="IU_DigitTryDb_TestConnection"/>, dans un scope DI
        /// éphémère résolu par <see cref="IS_UseCaseInvoker"/>. Le UseCase est
        /// responsable de l'émission des notifications
        /// <see cref="ISE_App.ConnectionLost"/> et
        /// <see cref="ISE_App.ConnectionRestored"/> côté
        /// <see cref="ISE_App"/> — le présent site se limite à déclencher
        /// l'interrogation à cadence.</para>
        /// <para>Aucun gating n'est appliqué : la boucle court en permanence
        /// dès l'instanciation du ViewModel, indépendamment de l'état
        /// utilisateur ou de la connectivité courante — cette posture est
        /// nécessaire pour permettre la détection du retour de connexion après
        /// une perte.</para>
        /// <para>Médiation par invoker (EA-11, §4.10.10 du 0230) : Identique
        /// au pattern de <see cref="UnreadMessagesPollingLoopAsync"/>.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// <para>Gestion d'erreurs : Identique à
        /// <see cref="UnreadMessagesPollingLoopAsync"/>.</para>
        /// </remarks>
        /// <returns>Une tâche représentant l'exécution de la boucle de
        /// polling.</returns>
        private async Task DatabaseConnectivityPollingLoopAsync()
        {
            string callChain = $"{_callee} > {nameof(DatabaseConnectivityPollingLoopAsync)}";

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await _useCaseInvoker.InvokeAsync<IU_DigitTryDb_TestConnection>(
                        (uc, ct) => uc.ExecuteAsync(callChain, ct),
                        _cts.Token);

                    await Task.Delay(
                        TimeSpan.FromSeconds(_seApp.DatabaseCheckInterval),
                        _cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Annulation coopérative — sortie silencieuse, pas de journalisation.
            }
        }

        /// <summary>
        /// Tente d'amorcer la boucle de polling des ordres de fermeture
        /// applicative si elle ne l'a pas encore été — opération idempotente
        /// garantie par le drapeau <c>_closeCommandLoopStarted</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode synchrone, invoquée à un site unique : la
        /// fin du constructeur, sans condition. Le drapeau est défensif vis-à-
        /// vis de toute évolution future du nombre de sites d'appel.</para>
        /// <para>Objectif : Si la boucle n'a jamais été amorcée
        /// (<c>_closeCommandLoopStarted</c> à <see langword="false"/>), lever
        /// le drapeau puis lancer
        /// <see cref="CloseCommandPollingLoopAsync"/> en
        /// fire-and-forget. Si la boucle est déjà amorcée, sortie
        /// silencieuse.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// <para>Filet de sécurité : Identique à
        /// <see cref="TryStartUnreadMessagesPollingLoop"/>.</para>
        /// </remarks>
        private void TryStartCloseCommandPollingLoop()
        {
            if (_closeCommandLoopStarted)
            {
                return;
            }

            _closeCommandLoopStarted = true;
            _ = CloseCommandPollingLoopAsync();
        }

        /// <summary>
        /// Boucle de polling asynchrone invoquant périodiquement
        /// <see cref="IU_CloseCommand_Check"/> via
        /// <see cref="IS_UseCaseInvoker"/>, à cadence
        /// <see cref="ISE_App.CloseCommandDelay"/> secondes, sous gating
        /// <see cref="ISE_App.IsConnected"/> seul.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Lancée en fire-and-forget par
        /// <see cref="TryStartCloseCommandPollingLoop"/>. La boucle court
        /// jusqu'à annulation coopérative du CTS local du ViewModel
        /// (<c>_cts.Token</c>).</para>
        /// <para>Objectif : Rendre l'application sensible aux ordres de
        /// fermeture applicative qu'aucune interaction utilisateur ne signale
        /// (demande de déconnexion adressée au couple utilisateur/application
        /// courant ; retrait d'accessibilité de l'application dans le
        /// référentiel AppList), en interrogeant périodiquement le pipeline
        /// applicatif via le UseCase Scoped
        /// <see cref="IU_CloseCommand_Check"/>, dans un scope DI éphémère
        /// résolu par <see cref="IS_UseCaseInvoker"/>. Chaque itération applique
        /// un gating sur la seule condition <see cref="ISE_App.IsConnected"/> :
        /// si la base est accessible, le UseCase est invoqué ; sinon
        /// l'itération saute l'invocation et attend la prochaine cadence. Toute
        /// la logique de détection et de décision de fermeture est portée par le
        /// UseCase consommé — le présent site se limite à amorcer et cadencer
        /// l'invocation.</para>
        /// <para>Médiation par invoker (EA-11, §4.10.10 du 0230) :
        /// L'invocation de <see cref="IU_CloseCommand_Check.ExecuteAsync"/>
        /// est médiée par <see cref="IS_UseCaseInvoker"/> qui matérialise un
        /// <c>IServiceScope</c> distinct par invocation, résout le UseCase
        /// Scoped dans ce scope, exécute le lambda fourni puis dispose le
        /// scope. La <c>callChain</c> est capturée par closure et transmise
        /// inchangée ; <c>_cts.Token</c> est propagé.</para>
        /// <para>Périmètre EA-NN-PollingVmBanniere : Voir les remarques de
        /// classe.</para>
        /// <para>Gestion d'erreurs : Le bloc
        /// <c>try / catch (OperationCanceledException)</c> permet une sortie
        /// silencieuse au signal d'annulation coopérative, sans journalisation
        /// (règle absolue §4.7.3 du 0230). Aucun traitement d'erreur métier
        /// n'incombe à la boucle — le UseCase consommé porte son propre
        /// traitement terminal (<see cref="IU_LogAndNotify"/>) et ne propage
        /// jamais d'exception applicative typée. Toute autre exception est
        /// laissée remonter au scheduler par défaut, qui se charge de la trace
        /// résiduelle.</para>
        /// </remarks>
        /// <returns>Une tâche représentant l'exécution de la boucle de
        /// polling.</returns>
        private async Task CloseCommandPollingLoopAsync()
        {
            string callChain = $"{_callee} > {nameof(CloseCommandPollingLoopAsync)}";

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_seApp.IsConnected)
                    {
                        await _useCaseInvoker.InvokeAsync<IU_CloseCommand_Check>(
                            (uc, ct) => uc.ExecuteAsync(callChain, ct),
                            _cts.Token);
                    }

                    await Task.Delay(
                        TimeSpan.FromSeconds(_seApp.CloseCommandDelay),
                        _cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Annulation coopérative — sortie silencieuse, pas de journalisation.
            }
        }

        /// <summary>
        /// Handler asynchrone abonné à l'événement applicatif système
        /// <see cref="ISE_App.ConnectionLost"/> — déclenche la procédure de
        /// récupération de connexion via
        /// <see cref="IU_DigitTryDb_RecoverConnection"/>, par l'intermédiaire
        /// de <see cref="IS_UseCaseInvoker"/> au sein d'un scope DI éphémère,
        /// sous gating de réentrance.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Invoqué par <see cref="ISE_App"/> lorsque la
        /// mécanique <c>NotifyConnectionLost</c> a effectivement basculé
        /// <see cref="ISE_App.IsConnected"/> à <see langword="false"/> — la
        /// notification INPC sur <see cref="ISE_App.IsConnected"/> est émise
        /// avant la levée de l'événement <see cref="ISE_App.ConnectionLost"/>
        /// sur la même pile d'exécution, garantissant que le miroir local
        /// <see cref="IsConnected"/> est à jour à l'entrée du présent
        /// handler.</para>
        /// <para>Signature <c>async void</c> : Imposée par la signature
        /// <see cref="EventHandler"/> de <see cref="ISE_App.ConnectionLost"/> —
        /// posture parallèle à celle des handlers d'événements WPF qui ne
        /// peuvent pas retourner de <see cref="Task"/>.</para>
        /// <para>Garde race condition : Sortie silencieuse si
        /// <see cref="ISE_App.IsConnected"/> est à <see langword="true"/> à
        /// l'entrée du handler — pose défensive face à toute évolution future
        /// de la mécanique côté <see cref="SE_App"/> ou à toute réémission
        /// opportuniste.</para>
        /// <para>Garde réentrance : Sortie silencieuse si
        /// <c>_recoveryInProgress</c> est déjà à <see langword="true"/> ;
        /// sinon, levée du drapeau avant l'invocation. La remise à
        /// <see langword="false"/> est portée par le bloc <c>finally</c>,
        /// garantissant le déverrouillage même en cas d'exception.</para>
        /// <para>Médiation par invoker (EA-11, §4.10.10 du 0230) : Identique
        /// au pattern de <see cref="CloseAppAsync"/> et des deux boucles de
        /// polling — l'invocation de
        /// <see cref="IU_DigitTryDb_RecoverConnection.ExecuteAsync"/> est
        /// médiée par <see cref="IS_UseCaseInvoker"/> au sein d'un scope DI
        /// éphémère. La <c>callChain</c> est capturée par closure ;
        /// <c>_cts.Token</c> est propagé.</para>
        /// <para>Périmètre EA-NN-AbonnementEventVmBanniere : Voir les remarques
        /// de classe.</para>
        /// <para>Périmètre EA-NN-GatingReentranceVmBanniere : Voir les
        /// remarques de classe.</para>
        /// <para>Périmètre EA-NN-FilSecuriteOnConnectionLost : Le bloc
        /// <c>try / catch (OperationCanceledException) / catch (Exception)</c>
        /// garantit qu'aucune exception ne remonte au scheduler. Le
        /// <c>catch (OperationCanceledException)</c> distinct silencieux
        /// précède le <c>catch (Exception)</c> global. Le filet ultime
        /// délègue à <see cref="IU_LogAndNotify"/> avec la clé canonique
        /// <c>"No_EC_03"</c> et <c>notify: false</c> — la sortie utilisateur
        /// de la procédure de récupération étant déjà portée par le pipeline
        /// interne de <see cref="IU_DigitTryDb_RecoverConnection"/>, une
        /// notification opérateur supplémentaire au site VM serait redondante.
        /// Le paramètre <c>ct</c> passé à
        /// <see cref="IU_LogAndNotify.ExecuteAsync"/> est
        /// <see cref="CancellationToken.None"/>, posture parallèle à
        /// <c>EA-NN-FilSecuriteVmBanniere</c>.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance Singleton de
        /// <see cref="ISE_App"/>).</param>
        /// <param name="e">Arguments d'événement vides
        /// (<see cref="EventArgs.Empty"/>).</param>
        private async void OnConnectionLost(object? sender, EventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnConnectionLost)}";

            // Garde race condition : si la connexion est déjà rétablie à l'entrée
            // du handler, la procédure de récupération est superflue.
            if (_seApp.IsConnected)
            {
                return;
            }

            // Garde réentrance : si une procédure de récupération est déjà en
            // cours, sortie silencieuse pour éviter le chaînage concurrent.
            if (_recoveryInProgress)
            {
                return;
            }

            _recoveryInProgress = true;

            try
            {
                await _useCaseInvoker.InvokeAsync<IU_DigitTryDb_RecoverConnection>(
                    (uc, ct) => uc.ExecuteAsync(callChain, ct),
                    _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Annulation amont légitime — silencieux, pas de journalisation.
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: false,
                    ct: CancellationToken.None);
            }
            finally
            {
                _recoveryInProgress = false;
            }
        }

        // --- Helper INPC ---

        /// <summary>
        /// Met à jour un champ de stockage et déclenche la notification
        /// <see cref="PropertyChanged"/> si la valeur a changé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Helper INPC canonique du ViewModel, utilisé par
        /// les setters privés des propriétés bindables observables et indirectement
        /// par les handlers d'abonnement qui passent par ces setters.</para>
        /// <para>Objectif : Centraliser la triade comparaison de valeur /
        /// écriture du champ support / émission de la notification INPC, et
        /// informer l'appelant du changement effectif via le retour booléen,
        /// exploitable pour des effets de bord conditionnels.</para>
        /// <para>Étalon documentaire : Signature et comportement alignés
        /// sur <c>SE_App.SetField</c> et sur <c>VM_MainWindow.SetField</c>, étalons
        /// documentaires du projet pour le helper INPC canonique.</para>
        /// </remarks>
        /// <typeparam name="T">Type de la valeur stockée.</typeparam>
        /// <param name="field">Référence au champ de stockage privé.</param>
        /// <param name="value">Nouvelle valeur à appliquer.</param>
        /// <param name="propertyName">Nom de la propriété appelante, résolu
        /// automatiquement par <see cref="CallerMemberNameAttribute"/>.</param>
        /// <returns><see langword="true"/> si la valeur a effectivement changé,
        /// <see langword="false"/> sinon.</returns>
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        #endregion
    }
}