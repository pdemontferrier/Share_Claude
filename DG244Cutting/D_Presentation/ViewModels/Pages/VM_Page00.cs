using System.Windows.Input;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la page de connexion <c>Page00</c> de l'application
    /// DG244Cutting, exposant à la vue le formulaire d'identification
    /// manuelle (identifiant et mot de passe), ses libellés multilingues
    /// et la commande de validation qui pilote les issues de présentation
    /// selon le verdict d'authentification et le nombre de tentatives.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille VM_Page de la couche
    /// <c>D_Presentation</c>, ViewModel concret de la page de connexion
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page00"/>. La
    /// page est la page d'accueil de repli présentée lorsque
    /// l'identification automatique par contexte device (login Windows)
    /// n'a pas abouti au démarrage. Elle offre à l'utilisateur un
    /// formulaire de connexion manuelle et, à la validation, exploite le
    /// verdict booléen du UseCase d'authentification pour ouvrir l'accès
    /// à l'application, avertir en cas d'échec non terminal, ou fermer
    /// l'application au troisième échec.</para>
    ///
    /// <para>Objectif : Exposer à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page00"/> :</para>
    /// <list type="bullet">
    ///   <item><description>Les deux propriétés observables en
    ///   lecture/écriture <see cref="Login"/> et <see cref="Password"/>,
    ///   liées en <c>TwoWay</c> aux champs de saisie, qui transportent
    ///   les valeurs saisies par l'utilisateur.</description></item>
    ///   <item><description>Les quatre propriétés observables
    ///   <see cref="Label_P00_00"/> à <see cref="Label_P00_03"/> liées
    ///   aux clés homonymes <c>P00_00</c> à <c>P00_03</c> du dictionnaire
    ///   actif, alimentées par la mécanique multilingue factorisée par
    ///   <see cref="VM_Generic"/> (premier chargement au constructeur via
    ///   <see cref="VM_Generic.InitializeLabels"/>, rechargement
    ///   automatique à tout changement de langue dynamique par le handler
    ///   interne d'abonnement INPC à
    ///   <see cref="ISE_App.AppCultureCode"/> de l'ancêtre commun,
    ///   conformément à §4.11.5 du 0230 et à R-4.11.9 du 0231).</description></item>
    ///   <item><description>La commande <see cref="LoginCommand"/>
    ///   (<see cref="UT_RelayCommandArg0Async"/>) de validation du couple
    ///   saisi, dont la garde d'activation exige les deux champs non
    ///   vides et non blancs, et dont l'exécution pilote les issues de
    ///   présentation.</description></item>
    /// </list>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Transporter les valeurs saisies via
    ///   <see cref="Login"/> et <see cref="Password"/>, exposées en accès
    ///   public en lecture et écriture via le helper hérité
    ///   <c>SetProperty&lt;T&gt;</c> pour la liaison bidirectionnelle avec
    ///   les champs de saisie.</description></item>
    ///   <item><description>Redéfinir <see cref="VM_Generic.LoadLabels"/>
    ///   pour résoudre les quatre clés <c>P00_00</c> à <c>P00_03</c> via
    ///   <see cref="VM_Generic._dictionary"/> hérité et affecter les
    ///   valeurs résolues aux quatre propriétés <c>Label_P00_NN</c>,
    ///   conformément à R-4.11.8 du 0231.</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Page_Generic.LoadAsync"/> pour réinitialiser le
    ///   cycle de tentatives (<see cref="ISE_User.UserAttempt"/> remis à
    ///   zéro par le setter) à chaque entrée sur la page, sous protection
    ///   du filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>. Le
    ///   hook est invoqué depuis le code-behind de <c>Page00</c> au point
    ///   d'extension <c>OnLoadedAsync</c> exposé par <c>Page_Generic</c>
    ///   (§4.15.7 du 0230).</description></item>
    ///   <item><description>Piloter, à la validation, les trois issues de
    ///   présentation selon le verdict d'authentification et le rang de
    ///   la tentative : accès à l'application par navigation vers
    ///   <c>Page10</c> (succès) ; avertissement utilisateur (échec des
    ///   rangs 1 et 2) ; trace de l'événement puis fermeture applicative
    ///   directe (échec terminal du rang 3). Le champ mot de passe est
    ///   vidé à l'issue de chaque tentative, le champ identifiant
    ///   conservé.</description></item>
    ///   <item><description>Invoquer les trois UseCases mobilisés
    ///   (<see cref="IU_UserAuthenticate"/>, <see cref="IU_Navigation"/>,
    ///   <see cref="IU_CloseApplication"/>) exclusivement via
    ///   <see cref="IS_UseCaseInvoker"/> (EA-11), conformément au mode
    ///   d'invocation depuis <c>D_Presentation</c> posé en §4.10.10 du
    ///   0230 et à I-4.10.10 du 0231.</description></item>
    ///   <item><description>Déléguer à <see cref="VM_Generic"/> la
    ///   cérémonie multilingue complète par l'unique appel à
    ///   <see cref="VM_Generic.InitializeLabels"/> en dernière instruction
    ///   du constructeur, conformément à I-4.11.11 et R-4.11.8 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne valide pas le couple identifiant/mot de
    ///   passe, ne hache pas le mot de passe, n'ouvre pas de session et
    ///   n'applique pas les droits : ces responsabilités appartiennent au
    ///   UseCase <see cref="IU_UserAuthenticate"/> invoqué. Le présent
    ///   ViewModel transporte les valeurs saisies, exploite le verdict
    ///   booléen retourné et pilote la présentation.</description></item>
    ///   <item><description>Ne décide pas de la navigation : la décision
    ///   appartient au UseCase, conformément à R-4.12.2 du 0231. Le
    ///   ViewModel n'injecte ni <c>IU_Navigation</c> ni
    ///   <c>IS_Navigation</c> ; il demande la navigation vers
    ///   <c>Page10</c> par invocation d'<see cref="IU_Navigation"/> via
    ///   <see cref="IS_UseCaseInvoker"/>, conformément à I-4.10.10 du
    ///   0231.</description></item>
    ///   <item><description>Ne porte aucune logique d'accès aux données
    ///   ni aucune règle métier propre : la logique de vérification, de
    ///   comptage terminal et de fermeture est portée par les UseCases
    ///   mobilisés.</description></item>
    ///   <item><description>Ne porte aucune cérémonie multilingue locale
    ///   ni aucun accès direct à <see cref="ISE_App"/> : l'abonnement
    ///   INPC est branché par <see cref="VM_Generic.InitializeLabels"/>
    ///   et porté par le handler interne de l'ancêtre commun (I-4.11.11
    ///   du 0231) ; aucun désabonnement n'est requis du dérivé
    ///   (Singleton, P4-bis §4.10.10 du 0230).</description></item>
    ///   <item><description>Ne pose aucune logique locale de repli en cas
    ///   de clé absente ni try/catch local dans
    ///   <see cref="LoadLabels"/> : la logique de repli est portée
    ///   exclusivement par <c>SR_Dictionary</c> conformément à R-4.11.6
    ///   et R-4.11.10 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales : Aucune exception
    /// architecturale propre n'est portée par <see cref="VM_Page00"/>.
    /// L'injection de <see cref="ISE_App"/> au constructeur de la base
    /// relève exclusivement de la mécanique multilingue factorisée par
    /// <see cref="VM_Generic"/> (§4.15.5 du 0230, R-4.11.9 du 0231).
    /// L'injection directe de <see cref="IU_LogAndNotify"/> par le
    /// ViewModel relève de l'exception architecturale propre du socle
    /// <see cref="VM_Generic"/> (EA-01, §4.15.5 du 0230), héritée et non
    /// re-déclarée à ce niveau ; elle est mobilisée par la trace de
    /// l'échec terminal. L'injection de <see cref="IS_UseCaseInvoker"/>
    /// par le constructeur est nominale au titre du mode d'invocation
    /// depuis <c>D_Presentation</c> posé en §4.10.10 du 0230 (EA-11) :
    /// les ViewModels invoquent les contrats <c>IU_</c> via
    /// <see cref="IS_UseCaseInvoker"/> qui matérialise un
    /// <c>IServiceScope</c> distinct à chaque invocation. Le présent
    /// ViewModel est consommateur d'EA-11 et non porteur : EA-11 est
    /// portée exclusivement par <c>SR_UseCaseInvoker</c>.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à sept régions,
    /// conformément à §4.4.2 (cinq régions standard) et à §4.4.3 (deux
    /// extensions présentes : <c>Propriétés publiques</c> et
    /// <c>Méthodes protégées</c>) :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> : champs
    ///   support de <see cref="Login"/>, <see cref="Password"/> et des
    ///   quatre libellés <c>_label_p00_NN</c>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> : champs
    ///   <see cref="_useCaseInvoker"/>, <see cref="_userSettings"/> et
    ///   <see cref="_notification"/>.</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c> :
    ///   <see cref="Login"/>, <see cref="Password"/>, les quatre
    ///   <c>Label_P00_NN</c> et <see cref="LoginCommand"/>.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur à
    ///   six paramètres, trois délégués à <see cref="VM_Generic"/> et
    ///   trois propres.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> : override
    ///   de <see cref="LoadAsync"/>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> : override
    ///   de <see cref="LoadLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   <see cref="ExecuteLoginAsync"/> et
    ///   <see cref="CanExecuteLogin"/>.</description></item>
    /// </list>
    ///
    /// <para>L'extension <c>=== Événements / Délégués / Indexeurs ===</c>
    /// n'est pas présente : l'événement <c>PropertyChanged</c> est porté
    /// par <see cref="VM_Generic"/> au titre d'INPC et n'est pas
    /// redéclaré ici.</para>
    /// </remarks>
    public class VM_Page00 : VM_Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Champ support de la propriété observable <see cref="Login"/>.
        /// Initialisé à <see cref="string.Empty"/> pour garantir un état
        /// défini avant le premier binding WPF.
        /// </summary>
        private string _login = string.Empty;

        /// <summary>
        /// Champ support de la propriété observable
        /// <see cref="Password"/>. Initialisé à <see cref="string.Empty"/>
        /// et remis à <see cref="string.Empty"/> à l'issue de chaque
        /// tentative de validation.
        /// </summary>
        private string _password = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P00_00"/> (clé <c>P00_00</c>).
        /// Initialisé à <see cref="string.Empty"/> ; écrasé au constructeur
        /// par le premier appel à <see cref="LoadLabels"/> orchestré par
        /// <see cref="VM_Generic.InitializeLabels"/>. Cette posture vaut
        /// pour les quatre champs <c>_label_p00_NN</c> et n'est documentée
        /// nominativement que sur le premier pour fixer la convention.
        /// </summary>
        private string _label_p00_00 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P00_01"/> (clé <c>P00_01</c>).</summary>
        private string _label_p00_01 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P00_02"/> (clé <c>P00_02</c>).</summary>
        private string _label_p00_02 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P00_03"/> (clé <c>P00_03</c>).</summary>
        private string _label_p00_03 = string.Empty;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Composant Singleton porteur de l'exception architecturale EA-11
        /// (§4.10.10 et §4.15.10 du 0230, §17.4 du 0231), unique voie
        /// d'invocation des UseCases (<c>IU_</c>) depuis un composant de
        /// <c>D_Presentation</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI au
        /// constructeur, conformément au mode d'invocation depuis
        /// <c>D_Presentation</c> posé en §4.10.10 du 0230. À chaque
        /// invocation, <see cref="IS_UseCaseInvoker"/> matérialise un
        /// <c>IServiceScope</c> distinct, y résout le composant cible et
        /// l'exécute via le délégué fourni, puis dispose le scope. Le
        /// présent ViewModel est consommateur de
        /// <see cref="IS_UseCaseInvoker"/> et non porteur d'EA-11.</para>
        /// <para>Mode d'invocation strict : Le passage par
        /// <see cref="IS_UseCaseInvoker"/> est imposé par la lecture
        /// stricte du §4.10.10 du 0230 qui pose l'interdiction structurelle
        /// de l'injection directe d'un contrat <c>IU_</c> dans un composant
        /// de <c>D_Presentation</c>. Les trois UseCases mobilisés
        /// (<see cref="IU_UserAuthenticate"/>, <see cref="IU_Navigation"/>,
        /// <see cref="IU_CloseApplication"/>) transitent par ce composant.
        /// Conformité I-4.10.10 du 0231.</para>
        /// </remarks>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        /// <summary>
        /// Setting Singleton de l'état utilisateur, source du compteur de
        /// tentatives d'authentification manuelle
        /// (<see cref="ISE_User.UserAttempt"/>).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI au
        /// constructeur. Consommé par <see cref="LoadAsync"/> pour la
        /// réinitialisation du cycle de tentatives à chaque entrée sur la
        /// page (remise à zéro par le setter) et par
        /// <see cref="ExecuteLoginAsync"/> pour l'incrément et la lecture
        /// du rang courant.</para>
        /// </remarks>
        private readonly ISE_User _userSettings;

        /// <summary>
        /// Service de présentation d'avertissements à l'opérateur,
        /// consommé pour la notification des échecs d'authentification non
        /// terminaux (rangs 1 et 2).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI au
        /// constructeur. Consommé par <see cref="ExecuteLoginAsync"/> pour
        /// l'avertissement intermédiaire via
        /// <see cref="IS_Notification.Warning"/>.</para>
        /// </remarks>
        private readonly IS_Notification _notification;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient ou définit l'identifiant saisi par l'utilisateur dans
        /// le formulaire de connexion.
        /// </summary>
        /// <value>Chaîne saisie liée en <c>TwoWay</c> au champ
        /// identifiant. Conservée d'une tentative à l'autre.</value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable en lecture/écriture publique
        /// consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page00"/>.
        /// Sa valeur est transmise telle quelle au UseCase
        /// <see cref="IU_UserAuthenticate"/> à la validation. Toute
        /// modification participe à la réévaluation de la garde
        /// d'activation de <see cref="LoginCommand"/> par le mécanisme
        /// <c>CommandManager.RequerySuggested</c> de
        /// <see cref="UT_RelayCommandArg0Async"/>.</para>
        /// </remarks>
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        /// <summary>
        /// Obtient ou définit le mot de passe saisi par l'utilisateur dans
        /// le formulaire de connexion.
        /// </summary>
        /// <value>Chaîne saisie liée en <c>TwoWay</c> au champ mot de passe
        /// (via les propriétés attachées de <c>UT_BindingPasswordBox</c>).
        /// Vidée à l'issue de chaque tentative de validation.</value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable en lecture/écriture publique
        /// consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page00"/>.
        /// Sa valeur est transmise au UseCase
        /// <see cref="IU_UserAuthenticate"/> à la validation, puis remise à
        /// <see cref="string.Empty"/> par <see cref="ExecuteLoginAsync"/>
        /// après le filet de sécurité, garantissant le vidage sur toutes
        /// les issues. Toute modification participe à la réévaluation de la
        /// garde d'activation de <see cref="LoginCommand"/>.</para>
        /// </remarks>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue associé à la clé <c>P00_00</c>
        /// du dictionnaire de langue actif (« Veuillez vous identifier. »
        /// en français).
        /// </summary>
        /// <value>Chaîne localisée résolue à partir du dictionnaire actif.
        /// En cas de clé absente, <c>SR_Dictionary</c> retourne la valeur
        /// de repli conformément à R-4.11.10 du 0231.</value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable en lecture publique, écriture
        /// privée. La valeur n'est modifiée qu'à travers l'override
        /// <see cref="LoadLabels"/>, invoqué initialement par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur puis
        /// par le handler interne d'abonnement INPC de
        /// <see cref="VM_Generic"/> à chaque changement de langue dynamique.
        /// Le présent <c>summary</c> fixe la convention de documentation
        /// pour les quatre propriétés <c>Label_P00_NN</c>.</para>
        /// </remarks>
        public string Label_P00_00
        {
            get => _label_p00_00;
            private set => SetProperty(ref _label_p00_00, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P00_01</c>.</summary>
        public string Label_P00_01
        {
            get => _label_p00_01;
            private set => SetProperty(ref _label_p00_01, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P00_02</c>.</summary>
        public string Label_P00_02
        {
            get => _label_p00_02;
            private set => SetProperty(ref _label_p00_02, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P00_03</c>.</summary>
        public string Label_P00_03
        {
            get => _label_p00_03;
            private set => SetProperty(ref _label_p00_03, value);
        }

        /// <summary>
        /// Obtient la commande de validation du couple identifiant/mot de
        /// passe saisi, câblée sur <see cref="ExecuteLoginAsync"/>.
        /// </summary>
        /// <value>Instance d'<see cref="UT_RelayCommandArg0Async"/>
        /// construite au constructeur, exposée en lecture seule.</value>
        /// <remarks>
        /// <para>Contexte : Commande bindable consommée par le bouton de
        /// validation de la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page00"/>.
        /// Sa garde d'activation <see cref="CanExecuteLogin"/> exige
        /// <see cref="Login"/> et <see cref="Password"/> tous deux non
        /// vides et non blancs ; l'activation est réévaluée par WPF via
        /// <c>CommandManager.RequerySuggested</c>, relayé par
        /// <see cref="UT_RelayCommandArg0Async"/>.</para>
        /// <para>Effets : Selon le verdict d'authentification et le rang de
        /// la tentative, l'exécution déclenche la navigation vers
        /// <c>Page10</c> (succès), un avertissement (échec des rangs 1 et
        /// 2) ou la trace puis la fermeture applicative (échec terminal du
        /// rang 3). Le mot de passe est vidé dans tous les cas.</para>
        /// </remarks>
        public ICommand LoginCommand { get; }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_Page00"/>.
        /// </summary>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue. Injecté en Singleton, transmis à
        /// <see cref="VM_Generic"/>.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement terminal
        /// des erreurs. Injecté en Singleton au titre de l'EA-01, transmis
        /// à <see cref="VM_Generic"/>.</param>
        /// <param name="app">Setting Singleton de l'état applicatif global,
        /// source des notifications INPC de langue. Injecté en Singleton,
        /// transmis à <see cref="VM_Generic"/>.</param>
        /// <param name="useCaseInvoker">Composant de médiation des UseCases
        /// (EA-11). Injecté en Singleton.</param>
        /// <param name="userSettings">Setting Singleton de l'état
        /// utilisateur, porteur du compteur de tentatives. Injecté en
        /// Singleton.</param>
        /// <param name="notification">Service de présentation
        /// d'avertissements à l'opérateur. Injecté en Singleton.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des
        /// paramètres <paramref name="useCaseInvoker"/>,
        /// <paramref name="userSettings"/> ou <paramref name="notification"/>
        /// est <see langword="null"/>. Les gardes des trois dépendances de
        /// base sont portées par le constructeur de
        /// <see cref="VM_Generic"/>.</exception>
        /// <remarks>
        /// <para>Séquence d'initialisation (R-4.4.7) : délégation des trois
        /// dépendances de base à <see cref="VM_Generic"/> via
        /// <c>base(...)</c> ; gardes nullité et affectation des trois
        /// dépendances propres ; instanciation de <see cref="LoginCommand"/>
        /// (garde <see cref="CanExecuteLogin"/> câblée) ; appel à
        /// <see cref="VM_Generic.InitializeLabels"/> en dernière
        /// instruction, conformément à R-4.11.8 et I-4.11.11 du 0231.</para>
        /// </remarks>
        public VM_Page00(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IS_UseCaseInvoker useCaseInvoker,
            ISE_User userSettings,
            IS_Notification notification)
            : base(dictionary, logAndNotify, app)
        {
            _useCaseInvoker = useCaseInvoker
                ?? throw new ArgumentNullException(nameof(useCaseInvoker));
            _userSettings = userSettings
                ?? throw new ArgumentNullException(nameof(userSettings));
            _notification = notification
                ?? throw new ArgumentNullException(nameof(notification));

            LoginCommand = new UT_RelayCommandArg0Async(ExecuteLoginAsync, CanExecuteLogin);

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Redéfinit le hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> pour réinitialiser le
        /// cycle de tentatives d'authentification à chaque entrée sur la
        /// page.
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// l'orchestrateur appelant côté <c>Page_Generic</c> et propagée
        /// par le code-behind via <c>_viewModel.LoadAsync(callChain, ct)</c>.
        /// Le paramètre est reçu par contrat du hook au socle
        /// <see cref="VM_Page_Generic"/> mais n'est pas consommé par le
        /// corps : une CallChain interne distincte est construite via
        /// <see cref="VM_Generic.BuildFirstCallChain"/>, conformément au
        /// patron de surcharge normatif §4.15.6 du 0230.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé par le
        /// code-behind appelant, retransmis à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>. Valeur par défaut :
        /// <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// réinitialisation.</returns>
        /// <remarks>
        /// <para>Contexte : Override du hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> déclaré
        /// <c>public virtual</c> au socle conformément à §4.15.6 du 0230.
        /// Invoquée depuis le code-behind de <c>Page00</c> au point
        /// d'extension <c>OnLoadedAsync</c> exposé par <c>Page_Generic</c>
        /// (§4.15.7 du 0230), à chaque événement <c>Loaded</c> de la
        /// page.</para>
        /// <para>Objectif : Rouvrir le cycle de trois tentatives à chaque
        /// entrée sur la page en remettant <see cref="ISE_User.UserAttempt"/>
        /// à zéro par le setter — réinitialisation chirurgicale du seul
        /// compteur, à l'exclusion de <c>ISE_User.Reset()</c> qui viderait
        /// l'intégralité du Setting utilisateur. Le ViewModel étant
        /// Singleton, ce hook est le seul point rejoué à chaque montage de
        /// la page.</para>
        /// <para>Patron de surcharge normatif (§4.15.6 du 0230) : L'override
        /// construit une CallChain interne via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> hérité et encapsule
        /// l'opération dans le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> (§4.7.3 du 0230),
        /// bien que la réinitialisation ne porte aucun échec métier
        /// propre. Le jeton <paramref name="ct"/> est propagé au filet.</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">Propagée
        /// silencieusement à l'appelant sur signal d'annulation coopérative
        /// par le filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>,
        /// conformément à §4.7.3 du 0230.</exception>
        public override async Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, () =>
            {
                _userSettings.UserAttempt = 0;
                return Task.CompletedTask;
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> pour charger les quatre
        /// libellés multilingues affichés par la page <c>Page00</c> à
        /// partir des clés <c>P00_00</c> à <c>P00_03</c> du dictionnaire de
        /// langue actif et les affecter aux quatre propriétés observables
        /// <see cref="Label_P00_00"/> à <see cref="Label_P00_03"/>.
        /// </summary>
        /// <param name="caller">CallChain construite par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// (premier chargement) ou par le handler interne d'abonnement INPC
        /// de <see cref="VM_Generic"/> au changement de langue dynamique
        /// (rechargement), transmise au service de dictionnaire pour
        /// traçabilité.</param>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinissant le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> conformément à R-4.11.8 du
        /// 0231.</para>
        /// <para>Patron strict : Une affectation par ligne, dans l'ordre
        /// numérique croissant des clés (<c>P00_00</c> à <c>P00_03</c>),
        /// sans regroupement et sans condition, pour une revue de code
        /// aisée et un repérage statique des clés consommées.</para>
        /// <para>Absence d'appel à <c>base.LoadLabels(caller)</c> :
        /// l'implémentation par défaut de
        /// <see cref="VM_Generic.LoadLabels"/> ne porte aucun traitement ;
        /// l'appel n'apporterait qu'un bruit inutile et est délibérément
        /// omis, aligné sur le patron des étalons.</para>
        /// <para>Filet de sécurité : Aucun try/catch local. Le filet est
        /// porté exclusivement par <c>SR_Dictionary</c> conformément à
        /// R-4.11.6 et R-4.11.10 du 0231.</para>
        /// </remarks>
        protected override void LoadLabels(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadLabels)}";

            Label_P00_00 = _dictionary.GetText(callChain, "P00_00");
            Label_P00_01 = _dictionary.GetText(callChain, "P00_01");
            Label_P00_02 = _dictionary.GetText(callChain, "P00_02");
            Label_P00_03 = _dictionary.GetText(callChain, "P00_03");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Orchestre la validation du couple identifiant/mot de passe saisi
        /// et pilote les trois issues de présentation selon le verdict
        /// d'authentification et le rang de la tentative. Méthode câblée à
        /// <see cref="LoginCommand"/>.
        /// </summary>
        /// <returns>Une tâche représentant l'exécution asynchrone de la
        /// validation et du pilotage des issues.</returns>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par
        /// <see cref="UT_RelayCommandArg0Async.ExecuteAsync"/> lorsque
        /// l'utilisateur active le bouton de validation. Le contrat
        /// <see cref="UT_RelayCommandArg0Async"/> portant un
        /// <see cref="System.Func{TResult}"/> sans jeton, l'aval est invoqué
        /// avec <see cref="CancellationToken.None"/> (aucune annulation
        /// coopérative amorcée depuis ce chemin).</para>
        /// <para>Structure : construction d'une CallChain via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> ; corps encapsulé
        /// par le filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// (§4.7.3 du 0230) ; remise du mot de passe à
        /// <see cref="string.Empty"/> APRÈS le filet, garantissant le
        /// vidage sur toutes les issues, y compris exception absorbée.</para>
        /// <para>Trois trajets, sans <c>throw</c> :</para>
        /// <list type="number">
        ///   <item><description>Nominal (verdict <see langword="true"/>) :
        ///   navigation vers <c>Page10</c> par invocation
        ///   d'<see cref="IU_Navigation"/> via
        ///   <see cref="IS_UseCaseInvoker"/>.</description></item>
        ///   <item><description>Échec non terminal (verdict
        ///   <see langword="false"/>, rang &lt; 3) : incrément du compteur
        ///   puis avertissement via
        ///   <see cref="IS_Notification.Warning"/>.</description></item>
        ///   <item><description>Échec terminal (verdict
        ///   <see langword="false"/>, rang &gt;= 3) : construction d'un
        ///   <see cref="Ex_Business"/> comme véhicule de trace (jamais
        ///   levé), trace via <see cref="IU_LogAndNotify"/>, puis fermeture
        ///   applicative directe par invocation d'
        ///   <see cref="IU_CloseApplication"/> (résultat non
        ///   consommé).</description></item>
        /// </list>
        /// <para>Filet de sécurité : Les exceptions applicatives typées
        /// éventuellement levées via l'invoker sont absorbées par les cinq
        /// captures d'<see cref="VM_Generic.ExecuteSafeAsync"/> et
        /// déléguées à <see cref="IU_LogAndNotify"/> (codes
        /// <c>No_EC_xx</c>). Aucun try/catch local n'est posé.</para>
        /// </remarks>
        private async Task ExecuteLoginAsync()
        {
            string callChain = BuildFirstCallChain();

            await ExecuteSafeAsync(callChain, async () =>
            {
                bool ok = await _useCaseInvoker.InvokeAsync<IU_UserAuthenticate, bool>(
                    (useCase, innerCt) => useCase.ExecuteAsync(callChain, Login, Password, innerCt),
                    CancellationToken.None);

                if (ok)
                {
                    await _useCaseInvoker.InvokeAsync<IU_Navigation>(
                        (navigation, innerCt) => navigation.NavigateToDefaultAsync(callChain, innerCt),
                        CancellationToken.None);
                    return;
                }

                _userSettings.IncrementUserAttempt();

                if (_userSettings.UserAttempt < 3)
                {
                    _notification.Warning(
                        callChain,
                        "No_Wa_05",
                        $"{_userSettings.UserAttempt} {_dictionary.GetText(callChain, "No_Wa_06")}");
                    return;
                }

                Ex_Business terminalTrace = new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_02,
                    "Échec d'authentification manuelle : nombre maximal de tentatives atteint ; fermeture applicative déclenchée.");

                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", terminalTrace);

                await _useCaseInvoker.InvokeAsync<IU_CloseApplication>(
                    (useCase, innerCt) => useCase.ExecuteAsync(
                        callChain,
                        confirmation: false,
                        warning: false,
                        delaySeconds: 0,
                        innerCt),
                    CancellationToken.None);
            }, CancellationToken.None);

            Password = string.Empty;
        }

        /// <summary>
        /// Garde d'activation de <see cref="LoginCommand"/> : la commande
        /// n'est activable que si <see cref="Login"/> et
        /// <see cref="Password"/> sont tous deux renseignés (non vides et
        /// non blancs).
        /// </summary>
        /// <returns><see langword="true"/> si les deux champs sont
        /// renseignés ; <see langword="false"/> sinon.</returns>
        /// <remarks>
        /// <para>Contexte : Prédicat câblé à
        /// <see cref="UT_RelayCommandArg0Async"/> au constructeur. Sa
        /// vérité rend structurellement inatteignable la branche « champs
        /// vides » : la validation n'est déclenchable que sur un couple
        /// renseigné. Réévalué par WPF via
        /// <c>CommandManager.RequerySuggested</c>.</para>
        /// </remarks>
        private bool CanExecuteLogin()
            => !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);

        #endregion
    }
}