using System.Collections.ObjectModel;
using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;
using DG244Cutting.A_Domain.Interfaces.ViewModels;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la page d'administration des comptes applicatifs
    /// <c>Page04</c> de l'application DG244Cutting, exposant à la vue la
    /// liste des comptes <see cref="UserApp"/> administrables ainsi qu'une
    /// fiche d'édition à trois modes (Consultation, Modification, Création)
    /// portant les opérations de création et de mise à jour d'un compte,
    /// déléguées aux UseCases dédiés via l'invocateur générique et suivies
    /// d'un rechargement de la page après persistance effective.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille VM_Page de la couche
    /// <c>D_Presentation</c>, ViewModel concret de la page
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page04"/> et
    /// réalisateur du contrat de vue <see cref="IV_Page04"/> défini en
    /// <c>A_Domain</c>. La page présente deux onglets : un Onglet 1 listant
    /// les comptes applicatifs administrables (les comptes techniques
    /// d'identifiant inférieur ou égal à 2 étant exclus du référentiel
    /// exposé) et un Onglet 2 portant la fiche d'un compte. La sortie de la
    /// page s'effectue exclusivement via les boutons transverses du menu
    /// horizontal ; la navigation inter-onglets en cours de saisie est
    /// arbitrée par une garde d'abandon.</para>
    ///
    /// <para>Objectif : Exposer à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page04"/> :</para>
    /// <list type="bullet">
    ///   <item><description>14 propriétés observables
    ///   <c>Label_P04_NN</c> liées aux clés homonymes du dictionnaire
    ///   actif (dont <see cref="Label_P04_01"/> et
    ///   <see cref="Label_P04_02"/> pour les en-têtes des deux onglets),
    ///   alimentées par la mécanique multilingue factorisée par
    ///   <see cref="VM_Generic"/> : premier chargement au constructeur via
    ///   <see cref="VM_Generic.InitializeLabels"/>, rechargement automatique
    ///   à tout changement de langue dynamique par le handler d'abonnement
    ///   INPC de l'ancêtre commun.</description></item>
    ///   <item><description>la collection observable
    ///   <see cref="UserApps"/> des comptes administrables, alimentée à
    ///   l'entrée sur la page par <see cref="LoadAsync"/> au moyen d'un
    ///   Query Handler générique invoqué selon l'EA-11 ;</description></item>
    ///   <item><description>les sept propriétés éditables de la fiche
    ///   (<see cref="LastName"/>, <see cref="FirstName"/>,
    ///   <see cref="Login"/>, <see cref="WindowsLogin"/>,
    ///   <see cref="IsActive"/>, <see cref="Password"/> et
    ///   <see cref="PasswordConfirmation"/>), l'enregistrement sélectionné
    ///   <see cref="SelectedRecord"/>, l'index d'onglet actif
    ///   <see cref="ActiveTabIndex"/> et la garde de changement d'onglet
    ///   <see cref="LeaveTabGuard"/> ;</description></item>
    ///   <item><description>les quatre gardes de commande du contrat
    ///   (<see cref="CanEnterCreate"/>, <see cref="CanEnterEdit"/>,
    ///   <see cref="CanAdd"/>, <see cref="CanSave"/>) et les trois prédicats
    ///   d'état de saisie (<see cref="IsFormEditable"/>,
    ///   <see cref="IsLoginEditable"/>, <see cref="IsTab2Available"/>),
    ///   recalculés à chaque transition de mode ou de sélection.</description></item>
    /// </list>
    ///
    /// <para>Responsabilités : Le présent ViewModel porte le cycle de vie
    /// de la fiche (entrée en Création, entrée en Modification, retour en
    /// Consultation), l'alimentation automatique des identifiants
    /// <see cref="Login"/> et <see cref="WindowsLogin"/> à partir des nom
    /// et prénom saisis tant que l'utilisateur ne les a pas édités
    /// manuellement (mode Création uniquement), la précondition de
    /// concordance des deux saisies de mot de passe avant délégation, la
    /// délégation des écritures aux UseCases
    /// <see cref="IU_UserApp_Create"/> et <see cref="IU_UserApp_Update"/>
    /// via <see cref="IS_UseCaseInvoker"/>, et le déclenchement d'un
    /// rechargement de la page courante par <see cref="IU_Navigation"/>
    /// lorsque l'écriture a effectivement muté l'état
    /// (<see cref="En_ChangeResult.Changed"/>).</para>
    ///
    /// <para>Non-responsabilités : La dérivation des initiales et le
    /// hachage du mot de passe ne relèvent pas de ce ViewModel : ces champs
    /// sont produits par les UseCases invoqués, auxquels le mot de passe est
    /// transmis en clair ; le ViewModel n'assemble pas ces valeurs dérivées.
    /// Les contrôles de forme approfondis sur l'identifiant de connexion
    /// (unicité, normalisation, contrôles souples) ne sont pas portés ici :
    /// seule la concordance des deux saisies de mot de passe est vérifiée en
    /// précondition, la validation métier étant assurée par les UseCases.
    /// Le revert visuel de l'onglet consécutif à un abandon refusé n'est pas
    /// porté par le ViewModel mais par l'utilitaire de garde de changement
    /// d'onglet ; <see cref="EvaluateLeaveTab"/> se borne à restituer la
    /// décision et, en cas d'abandon confirmé, à replacer la fiche en
    /// Consultation.</para>
    ///
    /// <para>Note EA : Au-delà de l'étalon de consultation de la famille,
    /// ce ViewModel consomme <see cref="IS_UseCaseInvoker"/> hors de
    /// <see cref="LoadAsync"/> — dans <see cref="AddAsync"/> et
    /// <see cref="SaveAsync"/> — et constitue à ce titre un initiateur de
    /// chaîne d'écriture au sens de R-4.14.19 du 0231. Il réalise en outre
    /// directement le contrat de vue <see cref="IV_Page04"/>, ce qui autorise
    /// le round-trip menu ↔ page par résolution du seul contrat abstrait sans
    /// référence à la classe concrète.</para>
    ///
    /// <para>Structure : La classe est organisée en sept régions —
    /// Propriétés privées, Dépendances privées, Propriétés publiques,
    /// Constructeur, Méthodes publiques, Méthodes protégées, Méthodes
    /// privées.</para>
    /// </remarks>
    public class VM_Page04 : VM_Page_Generic, IV_Page04
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Modes de la fiche d'édition de l'Onglet 2 : consultation en
        /// lecture seule, modification d'un compte existant, création d'un
        /// nouveau compte.
        /// </summary>
        private enum EditMode
        {
            Consultation,
            Modification,
            Creation
        }

        /// <summary>Mode courant de la fiche.</summary>
        private EditMode _mode = EditMode.Consultation;

        /// <summary>
        /// Indique que l'identifiant de connexion a été édité manuellement
        /// par l'utilisateur, suspendant son alimentation automatique.
        /// </summary>
        private bool _loginUserTouched;

        /// <summary>
        /// Indique que l'identifiant Windows a été édité manuellement par
        /// l'utilisateur, suspendant son alimentation automatique.
        /// </summary>
        private bool _windowsLoginUserTouched;

        /// <summary>
        /// Garde de ré-entrance : lorsqu'elle est active, les mutations de
        /// la fiche sont d'origine programmatique et ne déclenchent ni
        /// l'alimentation automatique ni le marquage d'édition manuelle.
        /// </summary>
        private bool _isProgrammaticEdit;

        private string _labelP04_01 = string.Empty;
        private string _labelP04_02 = string.Empty;
        private string _labelP04_05 = string.Empty;
        private string _labelP04_06 = string.Empty;
        private string _labelP04_07 = string.Empty;
        private string _labelP04_08 = string.Empty;
        private string _labelP04_09 = string.Empty;
        private string _labelP04_11 = string.Empty;
        private string _labelP04_12 = string.Empty;
        private string _labelP04_13 = string.Empty;
        private string _labelP04_14 = string.Empty;
        private string _labelP04_15 = string.Empty;
        private string _labelP04_16 = string.Empty;
        private string _labelP04_17 = string.Empty;

        private string _lastName = string.Empty;
        private string _firstName = string.Empty;
        private string _login = string.Empty;
        private string _windowsLogin = string.Empty;
        private bool _isActive;
        private string _password = string.Empty;
        private string _passwordConfirmation = string.Empty;

        private UserApp? _selectedRecord;
        private int _activeTabIndex;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Invocateur générique de UseCases, résolvant chaque UseCase dans
        /// une portée dédiée et appliquant le filet transactionnel commun.
        /// </summary>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        /// <summary>
        /// Service de notification à l'utilisateur, portant l'arrêt sur
        /// précondition et la demande de confirmation d'abandon.
        /// </summary>
        private readonly IS_Notification _notification;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>Libellé multilingue de la clé <c>P04_01</c>, en-tête de l'onglet de liste.</summary>
        public string Label_P04_01
        {
            get => _labelP04_01;
            private set => SetProperty(ref _labelP04_01, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_02</c>, en-tête de l'onglet de fiche.</summary>
        public string Label_P04_02
        {
            get => _labelP04_02;
            private set => SetProperty(ref _labelP04_02, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_05</c>.</summary>
        public string Label_P04_05
        {
            get => _labelP04_05;
            private set => SetProperty(ref _labelP04_05, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_06</c>.</summary>
        public string Label_P04_06
        {
            get => _labelP04_06;
            private set => SetProperty(ref _labelP04_06, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_07</c>.</summary>
        public string Label_P04_07
        {
            get => _labelP04_07;
            private set => SetProperty(ref _labelP04_07, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_08</c>.</summary>
        public string Label_P04_08
        {
            get => _labelP04_08;
            private set => SetProperty(ref _labelP04_08, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_09</c>.</summary>
        public string Label_P04_09
        {
            get => _labelP04_09;
            private set => SetProperty(ref _labelP04_09, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_11</c>.</summary>
        public string Label_P04_11
        {
            get => _labelP04_11;
            private set => SetProperty(ref _labelP04_11, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_12</c>.</summary>
        public string Label_P04_12
        {
            get => _labelP04_12;
            private set => SetProperty(ref _labelP04_12, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_13</c>.</summary>
        public string Label_P04_13
        {
            get => _labelP04_13;
            private set => SetProperty(ref _labelP04_13, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_14</c>.</summary>
        public string Label_P04_14
        {
            get => _labelP04_14;
            private set => SetProperty(ref _labelP04_14, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_15</c>.</summary>
        public string Label_P04_15
        {
            get => _labelP04_15;
            private set => SetProperty(ref _labelP04_15, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_16</c>.</summary>
        public string Label_P04_16
        {
            get => _labelP04_16;
            private set => SetProperty(ref _labelP04_16, value);
        }

        /// <summary>Libellé multilingue de la clé <c>P04_17</c>.</summary>
        public string Label_P04_17
        {
            get => _labelP04_17;
            private set => SetProperty(ref _labelP04_17, value);
        }

        /// <summary>
        /// Collection observable des comptes applicatifs administrables,
        /// alimentée à l'entrée sur la page par <see cref="LoadAsync"/>.
        /// </summary>
        public ObservableCollection<UserApp> UserApps { get; } = new();

        /// <summary>Nom de famille saisi dans la fiche (liaison TwoWay).</summary>
        public string LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value) && !_isProgrammaticEdit)
                {
                    ApplyAutoPopulation();
                }
            }
        }

        /// <summary>Prénom saisi dans la fiche (liaison TwoWay).</summary>
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value) && !_isProgrammaticEdit)
                {
                    ApplyAutoPopulation();
                }
            }
        }

        /// <summary>
        /// Identifiant de connexion saisi dans la fiche (liaison TwoWay).
        /// Toute édition manuelle suspend son alimentation automatique.
        /// </summary>
        public string Login
        {
            get => _login;
            set
            {
                if (SetProperty(ref _login, value) && !_isProgrammaticEdit)
                {
                    _loginUserTouched = true;
                }
            }
        }

        /// <summary>
        /// Identifiant Windows saisi dans la fiche (liaison TwoWay). Toute
        /// édition manuelle suspend son alimentation automatique.
        /// </summary>
        public string WindowsLogin
        {
            get => _windowsLogin;
            set
            {
                if (SetProperty(ref _windowsLogin, value) && !_isProgrammaticEdit)
                {
                    _windowsLoginUserTouched = true;
                }
            }
        }

        /// <summary>Indicateur d'activité du compte (liaison TwoWay).</summary>
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        /// <summary>Mot de passe saisi dans la fiche (liaison TwoWay).</summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>Confirmation du mot de passe saisi (liaison TwoWay).</summary>
        public string PasswordConfirmation
        {
            get => _passwordConfirmation;
            set => SetProperty(ref _passwordConfirmation, value);
        }

        /// <summary>
        /// Compte sélectionné dans la liste de l'Onglet 1. Sa mutation
        /// bascule la fiche en Consultation sur le compte retenu et ouvre
        /// l'Onglet 2.
        /// </summary>
        public UserApp? SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                if (SetProperty(ref _selectedRecord, value))
                {
                    OnSelectedRecordChanged();
                }
            }
        }

        /// <summary>Index de l'onglet actif (0 : liste, 1 : fiche).</summary>
        public int ActiveTabIndex
        {
            get => _activeTabIndex;
            set => SetProperty(ref _activeTabIndex, value);
        }

        /// <summary>
        /// Garde de changement d'onglet, restituée à l'utilitaire de vue :
        /// reçoit l'index quitté et l'index visé et retourne l'autorisation
        /// de quitter l'onglet courant.
        /// </summary>
        public Func<int, int, bool> LeaveTabGuard { get; }

        /// <summary>
        /// Garde de la commande d'entrée en Création : disponible en
        /// Consultation uniquement.
        /// </summary>
        public bool CanEnterCreate => _mode == EditMode.Consultation;

        /// <summary>
        /// Garde de la commande d'entrée en Modification : disponible en
        /// Consultation, un compte étant sélectionné.
        /// </summary>
        public bool CanEnterEdit =>
            _mode == EditMode.Consultation && SelectedRecord is not null;

        /// <summary>
        /// Garde de la commande d'ajout : disponible en Création uniquement.
        /// </summary>
        public bool CanAdd => _mode == EditMode.Creation;

        /// <summary>
        /// Garde de la commande d'enregistrement : disponible en
        /// Modification uniquement.
        /// </summary>
        public bool CanSave => _mode == EditMode.Modification;

        /// <summary>
        /// Indique que la fiche est éditable (modes Création ou
        /// Modification), pilotant le verrouillage des champs de saisie.
        /// </summary>
        public bool IsFormEditable =>
            _mode is EditMode.Creation or EditMode.Modification;

        /// <summary>
        /// Indique que l'identifiant de connexion est éditable : en Création
        /// uniquement, l'identifiant étant immuable en Modification.
        /// </summary>
        public bool IsLoginEditable => _mode == EditMode.Creation;

        /// <summary>
        /// Indique que l'Onglet 2 est accessible : un compte est sélectionné
        /// ou une création est en cours.
        /// </summary>
        public bool IsTab2Available =>
            SelectedRecord is not null || _mode == EditMode.Creation;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du ViewModel de la page
        /// <c>Page04</c>, résout ses dépendances propres, installe la garde
        /// de changement d'onglet et déclenche le premier chargement des
        /// libellés multilingues.
        /// </summary>
        /// <param name="dictionary">Service de dictionnaire multilingue,
        /// relayé à <see cref="VM_Generic"/> par la chaîne
        /// <c>base(...)</c>.</param>
        /// <param name="logAndNotify">Service transversal de journalisation
        /// et de notification, relayé à <see cref="VM_Generic"/>.</param>
        /// <param name="app">Réglages applicatifs porteurs de la culture
        /// active, relayés à <see cref="VM_Generic"/>.</param>
        /// <param name="useCaseInvoker">Invocateur générique de UseCases,
        /// injecté en Singleton par le conteneur DI.</param>
        /// <param name="notification">Service de notification à
        /// l'utilisateur, injecté par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="useCaseInvoker"/> ou
        /// <paramref name="notification"/> est <see langword="null"/>. Les
        /// gardes sur <paramref name="dictionary"/>,
        /// <paramref name="logAndNotify"/> et <paramref name="app"/> sont
        /// portées par <see cref="VM_Generic"/> via la chaîne
        /// <c>base(...)</c>.</exception>
        public VM_Page04(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IS_UseCaseInvoker useCaseInvoker,
            IS_Notification notification)
            : base(dictionary, logAndNotify, app)
        {
            _useCaseInvoker = useCaseInvoker
                ?? throw new ArgumentNullException(nameof(useCaseInvoker));
            _notification = notification
                ?? throw new ArgumentNullException(nameof(notification));

            LeaveTabGuard = EvaluateLeaveTab;

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge, à l'entrée sur la page, la liste des comptes applicatifs
        /// administrables après remise à zéro de l'état d'édition, rendant
        /// effectif le rechargement consécutif à une persistance sur ce
        /// ViewModel à cycle de vie Singleton.
        /// </summary>
        /// <param name="callChain">Chaîne d'appel de l'invocateur amont.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération de chargement.</returns>
        public override async Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                ResetEditState();

                var list = await _useCaseInvoker
                    .InvokeAsync<IQ_Generic<UserApp>, List<UserApp>>(
                        (handler, innerCt) => handler.HandleGetFilteredAsNoTrackingAsync(
                            innerCallChain,
                            u => u.Id > 2,
                            innerCt),
                        ct);

                UserApps.Clear();
                foreach (var user in list) UserApps.Add(user);
            }, ct);
        }

        /// <summary>
        /// Crée un nouveau compte applicatif à partir de la fiche courante,
        /// après vérification de la concordance des deux saisies de mot de
        /// passe, puis déclenche le rechargement de la page si l'écriture a
        /// effectivement muté l'état.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de l'appelant amont.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération de création.</returns>
        public Task AddAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {nameof(AddAsync)}";

            return ExecuteSafeAsync(callChain, async () =>
            {
                if (Password != PasswordConfirmation)
                {
                    _notification.Stop(callChain, "No_St_04");
                    return;
                }

                var entity = new UserApp
                {
                    LastName = LastName,
                    FirstName = FirstName,
                    Login = Login,
                    WindowsLogin = WindowsLogin,
                    IsActive = IsActive
                };

                var result = await _useCaseInvoker
                    .InvokeAsync<IU_UserApp_Create, En_ChangeResult>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            callChain, entity, Password, innerCt),
                        ct);

                if (result == En_ChangeResult.Changed)
                {
                    await _useCaseInvoker
                        .InvokeAsync<IU_Navigation>(
                            (navigation, innerCt) => navigation.RefreshCurrentPageAsync(
                                callChain, innerCt),
                            ct);
                }
            }, ct);
        }

        /// <summary>
        /// Enregistre les modifications du compte sélectionné à partir de la
        /// fiche courante, après vérification de la concordance des deux
        /// saisies de mot de passe, puis déclenche le rechargement de la
        /// page si l'écriture a effectivement muté l'état. L'identifiant de
        /// connexion et l'identifiant Windows demeurent immuables.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de l'appelant amont.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération de mise à jour.</returns>
        public Task SaveAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {nameof(SaveAsync)}";

            return ExecuteSafeAsync(callChain, async () =>
            {
                if (SelectedRecord is null) return;

                if (Password != PasswordConfirmation)
                {
                    _notification.Stop(callChain, "No_St_04");
                    return;
                }

                var entity = CloneUserApp(SelectedRecord);
                entity.LastName = LastName;
                entity.FirstName = FirstName;
                entity.IsActive = IsActive;

                var result = await _useCaseInvoker
                    .InvokeAsync<IU_UserApp_Update, En_ChangeResult>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            callChain, entity, Password, innerCt),
                        ct);

                if (result == En_ChangeResult.Changed)
                {
                    await _useCaseInvoker
                        .InvokeAsync<IU_Navigation>(
                            (navigation, innerCt) => navigation.RefreshCurrentPageAsync(
                                callChain, innerCt),
                            ct);
                }
            }, ct);
        }

        /// <summary>
        /// Fait entrer la fiche en mode Création : vide les champs de saisie,
        /// active le compte par défaut, réarme l'alimentation automatique des
        /// identifiants et ouvre l'Onglet 2.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de l'appelant amont.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération d'entrée en Création.</returns>
        public Task EnterCreate(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {nameof(EnterCreate)}";

            return ExecuteSafeAsync(callChain, () =>
            {
                _isProgrammaticEdit = true;
                try
                {
                    SetMode(EditMode.Creation);
                    ClearFiche();
                    IsActive = true;
                    _loginUserTouched = false;
                    _windowsLoginUserTouched = false;
                    ActiveTabIndex = 1;
                }
                finally
                {
                    _isProgrammaticEdit = false;
                }

                RaiseComputed();
                return Task.CompletedTask;
            }, ct);
        }

        /// <summary>
        /// Fait entrer la fiche en mode Modification sur le compte
        /// sélectionné : recopie ses valeurs dans les champs de saisie,
        /// réinitialise les saisies de mot de passe et ouvre l'Onglet 2.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de l'appelant amont.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération d'entrée en
        /// Modification.</returns>
        public Task EnterEdit(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {nameof(EnterEdit)}";

            return ExecuteSafeAsync(callChain, () =>
            {
                if (SelectedRecord is null) return Task.CompletedTask;

                _isProgrammaticEdit = true;
                try
                {
                    SetMode(EditMode.Modification);
                    PopulateFicheFrom(SelectedRecord);
                    Password = string.Empty;
                    PasswordConfirmation = string.Empty;
                    _loginUserTouched = false;
                    _windowsLoginUserTouched = false;
                    ActiveTabIndex = 1;
                }
                finally
                {
                    _isProgrammaticEdit = false;
                }

                RaiseComputed();
                return Task.CompletedTask;
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Recharge les quatorze libellés multilingues de la page à partir du
        /// dictionnaire actif. Invoquée au premier chargement et à chaque
        /// changement de langue par la mécanique factorisée de
        /// <see cref="VM_Generic"/>.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de la mécanique amont.</param>
        protected override void LoadLabels(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadLabels)}";

            Label_P04_01 = _dictionary.GetText(callChain, "P04_01");
            Label_P04_02 = _dictionary.GetText(callChain, "P04_02");
            Label_P04_05 = _dictionary.GetText(callChain, "P04_05");
            Label_P04_06 = _dictionary.GetText(callChain, "P04_06");
            Label_P04_07 = _dictionary.GetText(callChain, "P04_07");
            Label_P04_08 = _dictionary.GetText(callChain, "P04_08");
            Label_P04_09 = _dictionary.GetText(callChain, "P04_09");
            Label_P04_11 = _dictionary.GetText(callChain, "P04_11");
            Label_P04_12 = _dictionary.GetText(callChain, "P04_12");
            Label_P04_13 = _dictionary.GetText(callChain, "P04_13");
            Label_P04_14 = _dictionary.GetText(callChain, "P04_14");
            Label_P04_15 = _dictionary.GetText(callChain, "P04_15");
            Label_P04_16 = _dictionary.GetText(callChain, "P04_16");
            Label_P04_17 = _dictionary.GetText(callChain, "P04_17");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Évalue la demande de quitter un onglet. En sortie de l'Onglet 2
        /// alors qu'une saisie est en cours (Création ou Modification),
        /// sollicite une confirmation d'abandon ; si l'abandon est confirmé,
        /// replace la fiche en Consultation. Le revert visuel de l'onglet
        /// est assuré par l'utilitaire de vue et non par cette méthode.
        /// </summary>
        /// <param name="from">Index de l'onglet quitté.</param>
        /// <param name="to">Index de l'onglet visé.</param>
        /// <returns><see langword="true"/> si le changement d'onglet est
        /// autorisé ; sinon <see langword="false"/>.</returns>
        private bool EvaluateLeaveTab(int from, int to)
        {
            if (from == 1
                && (_mode == EditMode.Creation || _mode == EditMode.Modification))
            {
                string callChain = BuildFirstCallChain();
                bool confirmed = _notification.ConfirmationReturn(callChain, "No_AD_04");
                if (confirmed)
                {
                    ShowConsultation();
                }
                return confirmed;
            }

            return true;
        }

        /// <summary>
        /// Réaction au changement de compte sélectionné : bascule la fiche
        /// en Consultation sur le compte retenu et ouvre l'Onglet 2 ; à
        /// défaut de sélection, se borne à recalculer les propriétés
        /// dérivées.
        /// </summary>
        private void OnSelectedRecordChanged()
        {
            if (SelectedRecord is not null)
            {
                ShowConsultation();
                ActiveTabIndex = 1;
            }
            else
            {
                RaiseComputed();
            }
        }

        /// <summary>
        /// Replace la fiche en mode Consultation : restitue les valeurs du
        /// compte sélectionné ou vide la fiche à défaut, réinitialise les
        /// saisies de mot de passe et recalcule les propriétés dérivées.
        /// </summary>
        private void ShowConsultation()
        {
            _isProgrammaticEdit = true;
            try
            {
                SetMode(EditMode.Consultation);
                if (SelectedRecord is not null)
                {
                    PopulateFicheFrom(SelectedRecord);
                }
                else
                {
                    ClearFiche();
                }
                Password = string.Empty;
                PasswordConfirmation = string.Empty;
            }
            finally
            {
                _isProgrammaticEdit = false;
            }

            RaiseComputed();
        }

        /// <summary>
        /// Remet le ViewModel dans un état de consultation vierge : mode
        /// Consultation, sélection nulle, fiche vidée, marqueurs d'édition
        /// réarmés et retour à l'onglet de liste. Invoquée en tête de
        /// <see cref="LoadAsync"/>.
        /// </summary>
        private void ResetEditState()
        {
            _isProgrammaticEdit = true;
            try
            {
                SetMode(EditMode.Consultation);
                _selectedRecord = null;
                ClearFiche();
                _loginUserTouched = false;
                _windowsLoginUserTouched = false;
                ActiveTabIndex = 0;
            }
            finally
            {
                _isProgrammaticEdit = false;
            }

            OnPropertyChanged(nameof(SelectedRecord));
            RaiseComputed();
        }

        /// <summary>
        /// Alimente automatiquement les identifiants <see cref="Login"/> et
        /// <see cref="WindowsLogin"/> à partir des nom et prénom saisis, en
        /// mode Création uniquement et tant que l'utilisateur ne les a pas
        /// édités manuellement. La valeur retenue concatène l'initiale du
        /// prénom et les sept premiers caractères du nom, en minuscules.
        /// </summary>
        private void ApplyAutoPopulation()
        {
            if (_mode != EditMode.Creation) return;
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName)) return;

            string computed =
                $"{FirstName[0]}{LastName.Substring(0, Math.Min(7, LastName.Length))}"
                    .ToLowerInvariant();

            _isProgrammaticEdit = true;
            try
            {
                if (!_loginUserTouched) Login = computed;
                if (!_windowsLoginUserTouched) WindowsLogin = computed;
            }
            finally
            {
                _isProgrammaticEdit = false;
            }
        }

        /// <summary>Affecte le mode courant de la fiche.</summary>
        /// <param name="mode">Nouveau mode.</param>
        private void SetMode(EditMode mode) => _mode = mode;

        /// <summary>Vide les champs de saisie de la fiche.</summary>
        private void ClearFiche()
        {
            LastName = string.Empty;
            FirstName = string.Empty;
            Login = string.Empty;
            WindowsLogin = string.Empty;
            IsActive = false;
            Password = string.Empty;
            PasswordConfirmation = string.Empty;
        }

        /// <summary>
        /// Recopie dans les champs de saisie les valeurs d'identité du compte
        /// fourni. Les saisies de mot de passe ne sont jamais restituées.
        /// </summary>
        /// <param name="source">Compte source.</param>
        private void PopulateFicheFrom(UserApp source)
        {
            LastName = source.LastName;
            FirstName = source.FirstName;
            Login = source.Login;
            WindowsLogin = source.WindowsLogin ?? string.Empty;
            IsActive = source.IsActive;
        }

        /// <summary>
        /// Produit une copie scalaire du compte fourni, laissant les
        /// collections de navigation à leur valeur par défaut. Sert de base
        /// à l'entité transmise au UseCase de mise à jour, patchée des seuls
        /// champs éditables.
        /// </summary>
        /// <param name="source">Compte source.</param>
        /// <returns>Nouvelle instance copiant les champs scalaires.</returns>
        private static UserApp CloneUserApp(UserApp source)
        {
            return new UserApp
            {
                Id = source.Id,
                LastName = source.LastName,
                FirstName = source.FirstName,
                Login = source.Login,
                Initials = source.Initials,
                PasswordHash = source.PasswordHash,
                Birthday = source.Birthday,
                PhonePro = source.PhonePro,
                PhoneProFixed = source.PhoneProFixed,
                PhonePersonal = source.PhonePersonal,
                EmailProfessional = source.EmailProfessional,
                EmailPersonal = source.EmailPersonal,
                ContractType = source.ContractType,
                SectorId = source.SectorId,
                CompanyId = source.CompanyId,
                EntryDate = source.EntryDate,
                Address = source.Address,
                PostalCode = source.PostalCode,
                City = source.City,
                Country = source.Country,
                ProductivityRate = source.ProductivityRate,
                PleiadeNumber = source.PleiadeNumber,
                WindowsLogin = source.WindowsLogin,
                IsActive = source.IsActive,
                IsResetRequired = source.IsResetRequired,
                CreatedAt = source.CreatedAt,
                UpdatedAt = source.UpdatedAt,
                IsDeleted = source.IsDeleted
            };
        }

        /// <summary>
        /// Notifie la vue du recalcul des quatre gardes de commande et des
        /// trois prédicats d'état de saisie.
        /// </summary>
        private void RaiseComputed()
        {
            OnPropertyChanged(nameof(CanEnterCreate));
            OnPropertyChanged(nameof(CanEnterEdit));
            OnPropertyChanged(nameof(CanAdd));
            OnPropertyChanged(nameof(CanSave));
            OnPropertyChanged(nameof(IsFormEditable));
            OnPropertyChanged(nameof(IsLoginEditable));
            OnPropertyChanged(nameof(IsTab2Available));
        }

        #endregion
    }
}