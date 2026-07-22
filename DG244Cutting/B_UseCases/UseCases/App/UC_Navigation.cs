using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// UseCase orchestrateur de navigation WPF de l'application DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette classe est une dépendance injectable enregistrée en
    /// Singleton — elle maintient l'état runtime de navigation (page courante, historique)
    /// pour toute la durée de vie de l'application — et consommée via
    /// <see cref="IU_Navigation"/>. Elle réside dans <c>B_UseCases/UseCases/App/</c>,
    /// couche d'orchestration applicative.</para>
    /// <para>Objectif : Constituer le point de jonction normatif entre le référentiel
    /// des pages (<see cref="ISE_Navigation"/>), les droits utilisateur (<see cref="ISE_User"/>)
    /// et l'exécution technique WPF (<see cref="IS_Navigation"/>). Toute décision de
    /// navigation dans l'application transite obligatoirement par ce composant.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Vérifier l'existence de toute page cible dans <see cref="ISE_Navigation"/>
    ///   avant toute navigation.</item>
    ///   <item>Lire et appliquer les droits d'accès de l'utilisateur courant depuis
    ///   <see cref="ISE_User"/> pour chaque page cible.</item>
    ///   <item>Décider des redirections de repli vers
    ///   <see cref="ISE_Navigation.FallbackPageName"/> lorsqu'un accès est refusé, et de la
    ///   redirection vers <see cref="ISE_Navigation.LoginPageName"/> lorsqu'aucun utilisateur
    ///   n'est connecté et que la cible de navigation par défaut est demandée.</item>
    ///   <item>Maintenir la page courante et l'historique de navigation comme état runtime.</item>
    ///   <item>Exposer les opérations de contrôle utilisateur du menu horizontal
    ///   (déploiement, réduction).</item>
    ///   <item>Exposer une vue prédicative des droits granulaires de l'utilisateur courant
    ///   pour chaque page applicative, utilisée par <c>MH_Generic.ApplySecurityRules</c>
    ///   et par les ViewModels conditionnant la visibilité ou l'activation des actions.</item>
    ///   <item>Déléguer l'exécution technique à <see cref="IS_Navigation"/> après décision.</item>
    ///   <item>Propager la <c>CallChain</c> à chaque composant appelé.</item>
    ///   <item>Assurer le traitement terminal des erreurs via <see cref="IU_LogAndNotify"/>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>N'interagit jamais directement avec <c>MainWindow</c>, les frames WPF ou le
    ///   <c>Dispatcher</c> — ce rôle appartient à <see cref="IS_Navigation"/>.</item>
    ///   <item>Ne connaît pas les détails de la couche de persistance — la navigation WPF
    ///   n'est pas un scénario d'écriture transactionnel en base de données.</item>
    ///   <item>Ne modifie pas <see cref="ISE_User"/> ni <see cref="ISE_Navigation"/>.</item>
    ///   <item>Ne porte aucune constante de nom logique de page — l'autorité unique sur
    ///   les noms de pages (page de connexion, page de repli, page par défaut) est
    ///   <see cref="ISE_Navigation"/> et ses propriétés
    ///   <see cref="ISE_Navigation.LoginPageName"/>,
    ///   <see cref="ISE_Navigation.FallbackPageName"/>,
    ///   <see cref="ISE_Navigation.DefaultPageName"/>.</item>
    /// </list>
    /// <para>Note sur la transactionnalité : La navigation WPF ne constitue pas un
    /// scénario d'écriture en base de données. Ce UseCase n'ouvre donc pas de transaction
    /// SQL. Il s'inscrit dans l'architecture UseCase par son rôle d'orchestration et de
    /// gestion terminale des erreurs, conformément aux principes définis dans la partie 3
    /// du référentiel.</para>
    /// <para>Note sur la pluralité de méthodes publiques : Ce UseCase expose
    /// plusieurs méthodes publiques avec des préfixes alternatifs à <c>ExecuteAsync</c>,
    /// conformément à la dérogation admise pour le cas Concept au sens de la convention
    /// de nommage UC_ dual-cas Entité/Concept. La dérogation est tracée par le présent
    /// <c>&lt;remarks&gt;</c> de classe ainsi que dans le <c>&lt;remarks&gt;</c> de chaque
    /// méthode publique individuelle, conformément à §4.3 et §5.3.3 du référentiel.</para>
    /// <para>Note sur la propagation du <c>CancellationToken</c> : Les six méthodes
    /// publiques asynchrones reçoivent un <c>CancellationToken ct = default</c> propagé
    /// au pipeline terminal <see cref="IU_LogAndNotify"/> conformément à §4.6. Les appels
    /// à <see cref="IS_Navigation"/> étant aujourd'hui synchrones, le <c>ct</c> n'est pas
    /// propagé à ce niveau ; il sera utilisé lors de la future évolution éventuelle de
    /// <see cref="IS_Navigation"/> vers des signatures asynchrones.</para>
    /// <seealso cref="IU_Navigation"/>
    /// </remarks>
    public class UC_Navigation : IU_Navigation
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe pour la construction de la <c>CallChain</c>.
        /// Initialisé via <c>GetType().Name</c> dans le constructeur.
        /// </summary>
        private readonly string _callee;

        /// <summary>
        /// Nom logique de la page actuellement affichée.
        /// Chaîne vide à l'état initial, avant la première navigation.
        /// </summary>
        private string _currentPageName = string.Empty;

        /// <summary>
        /// Mapping complet de la page actuellement affichée.
        /// <see langword="null"/> à l'état initial, avant la première navigation.
        /// </summary>
        private PageMapping? _currentPageMapping;

        /// <summary>
        /// Pile LIFO des noms de pages visitées, utilisée pour la navigation arrière.
        /// Chaque appel à <see cref="NavigateToPageAsync"/> ou
        /// <see cref="NavigateToDefaultAsync"/> empile la page courante avant navigation.
        /// </summary>
        private readonly Stack<string> _navigationHistory = new();

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Référentiel central de navigation — source d'autorité pour l'existence,
        /// les URIs des pages et les noms logiques des pages de connexion, de repli
        /// et par défaut.
        /// </summary>
        private readonly ISE_Navigation _seNavigation;

        /// <summary>
        /// Setting utilisateur courant — source des droits d'accès par page et
        /// de l'identifiant d'utilisateur applicatif courant.
        /// </summary>
        private readonly ISE_User _seUser;

        /// <summary>
        /// Service technique WPF de navigation — exécute physiquement les mises à jour
        /// de frames après décision.
        /// </summary>
        private readonly IS_Navigation _srNavigation;

        /// <summary>
        /// Orchestrateur de traitement terminal des erreurs — journalise et notifie
        /// les exceptions applicatives capturées.
        /// </summary>
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le UseCase de navigation avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Ce constructeur est appelé par le conteneur d'injection
        /// lors de la création du Singleton. L'état runtime de navigation est initialisé
        /// à un état neutre : page courante vide, historique vide.</para>
        /// <para>Objectif : Initialiser <c>_callee</c> dynamiquement et stocker les
        /// dépendances après contrôle de non-nullité. Aucune logique de navigation ni aucun
        /// accès aux Settings ne doit figurer dans ce constructeur.</para>
        /// </remarks>
        /// <param name="seNavigation">
        /// Référentiel de navigation, source d'autorité pour l'existence des pages et pour
        /// les noms logiques des pages de connexion, de repli et par défaut.
        /// </param>
        /// <param name="seUser">
        /// Setting utilisateur, source des droits d'accès par page et de l'identifiant
        /// d'utilisateur applicatif courant.
        /// </param>
        /// <param name="srNavigation">
        /// Service technique WPF délégué pour l'exécution physique de la navigation.
        /// </param>
        /// <param name="logAndNotify">
        /// Orchestrateur de traitement terminal des erreurs applicatives.
        /// </param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_Navigation(
            ISE_Navigation seNavigation,
            ISE_User seUser,
            IS_Navigation srNavigation,
            IU_LogAndNotify logAndNotify)
        {
            _seNavigation = seNavigation ?? throw new ArgumentNullException(nameof(seNavigation));
            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
            _srNavigation = srNavigation ?? throw new ArgumentNullException(nameof(srNavigation));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        // --- Groupe 1 : État runtime courant ---

        /// <inheritdoc/>
        public string CurrentPageName => _currentPageName;

        /// <inheritdoc/>
        public bool CanNavigateBack => _navigationHistory.Count > 0;

        // --- Groupe 2 : Opérations de navigation ---

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Séquence interne :</para>
        /// <list type="number">
        ///   <item>Validation du paramètre <paramref name="pageName"/>.</item>
        ///   <item>Court-circuit si <paramref name="pageName"/> est déjà la page courante.</item>
        ///   <item>Empilement de la page courante dans l'historique.</item>
        ///   <item>Résolution de la page cible réelle (droits → redirection éventuelle)
        ///   via <see cref="ResolveTargetPage"/>.</item>
        ///   <item>Mise à jour de l'état runtime.</item>
        ///   <item>Délégation de la séquence WPF à <see cref="ExecuteWpfNavigationSequence"/>.</item>
        /// </list>
        /// <para>Dérogation au préfixe <c>ExecuteAsync</c> : Le nom alternatif
        /// <c>NavigateToPageAsync</c> dérive d'un verbe d'action en anglais à l'impératif,
        /// au titre de la dérogation typologiquement bornée admise pour le cas Concept
        /// démultiplié (§4.2 du 0230, R-4.2.13 du 0231, §5.3.3 du 0232-UC).</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        public async Task NavigateToPageAsync(string caller, string pageName, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(NavigateToPageAsync)}";

            try
            {
                if (string.IsNullOrWhiteSpace(pageName))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le nom de page fourni est null, vide ou composé uniquement d'espaces.");

                // Court-circuit : ne pas naviguer si la page demandée est déjà affichée
                if (pageName.Trim() == _currentPageName)
                    return;

                // Empiler la page courante avant de naviguer (si non vide = pas à l'état initial)
                if (!string.IsNullOrEmpty(_currentPageName))
                    _navigationHistory.Push(_currentPageName);

                ApplyNavigation(callChain, pageName.Trim());
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct); }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct); }
            catch (OperationCanceledException) { throw; }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Séquence interne :</para>
        /// <list type="number">
        ///   <item>Vérification que l'historique n'est pas vide.</item>
        ///   <item>Dépilage du nom de la page précédente.</item>
        ///   <item>Résolution de la page précédente (droits → redirection éventuelle).</item>
        ///   <item>Mise à jour de l'état runtime.</item>
        ///   <item>Délégation WPF — sans empilement supplémentaire dans l'historique.</item>
        /// </list>
        /// <para>Dérogation au préfixe <c>ExecuteAsync</c> : Le nom alternatif
        /// <c>NavigateToPreviousPageAsync</c> dérive d'un verbe d'action en anglais à l'impératif,
        /// au titre de la dérogation typologiquement bornée admise pour le cas Concept
        /// démultiplié (§4.2 du 0230, R-4.2.13 du 0231, §5.3.3 du 0232-UC).</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        public async Task NavigateToPreviousPageAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(NavigateToPreviousPageAsync)}";

            try
            {
                if (_navigationHistory.Count == 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        "L'historique de navigation est vide. La navigation vers la page précédente " +
                        "est impossible depuis cet état.");

                // Dépiler la page précédente — ne pas empiler la courante (navigation arrière)
                string previousPageName = _navigationHistory.Pop();

                ApplyNavigation(callChain, previousPageName);
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct); }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct); }
            catch (OperationCanceledException) { throw; }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Séquence interne :</para>
        /// <list type="number">
        ///   <item>Lecture du nom logique de la page par défaut depuis
        ///   <see cref="ISE_Navigation.DefaultPageName"/>.</item>
        ///   <item>Court-circuit si la page par défaut est déjà la page courante.</item>
        ///   <item>Empilement de la page courante dans l'historique (si non vide).</item>
        ///   <item>Décision de la cible effective selon l'état utilisateur : si
        ///   <see cref="ISE_User.AppUserId"/> est inférieur ou égal à zéro (aucun
        ///   utilisateur connecté), la cible est <see cref="ISE_Navigation.LoginPageName"/> ;
        ///   sinon la cible est la page par défaut lue à l'étape 1.</item>
        ///   <item>Délégation à <see cref="ApplyNavigation"/> avec la cible résolue.</item>
        /// </list>
        /// <para>Cohérence avec le contrat <see cref="IU_Navigation"/> : Cette
        /// décision en amont, qui aiguille vers <see cref="ISE_Navigation.LoginPageName"/>
        /// lorsqu'aucun utilisateur n'est connecté, met en cohérence un comportement qui
        /// résultait précédemment du contrôle des droits dans <see cref="ResolveTargetPage"/>
        /// avec redirection vers <see cref="ISE_Navigation.FallbackPageName"/> faute de droit
        /// <c>CanAccess</c> sur la page par défaut. La cible explicite est désormais la page
        /// de connexion, plus pertinente sémantiquement que la page de repli pour ce cas
        /// d'usage.</para>
        /// <para>Dérogation au préfixe <c>ExecuteAsync</c> : Le nom alternatif
        /// <c>NavigateToDefaultAsync</c> dérive d'un verbe d'action en anglais à l'impératif,
        /// au titre de la dérogation typologiquement bornée admise pour le cas Concept
        /// démultiplié (§4.2 du 0230, R-4.2.13 du 0231, §5.3.3 du 0232-UC).</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        public async Task NavigateToDefaultAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(NavigateToDefaultAsync)}";

            try
            {
                string defaultPageName = _seNavigation.DefaultPageName;
                string loginPageName = _seNavigation.LoginPageName;

                // Court-circuit : déjà sur la page par défaut
                if (defaultPageName == _currentPageName)
                    return;

                if (!string.IsNullOrEmpty(_currentPageName))
                    _navigationHistory.Push(_currentPageName);

                // Décision en amont : si aucun utilisateur n'est connecté (AppUserId <= 0),
                // la cible effective est la page de connexion (LoginPageName), exemptée du
                // contrôle des droits. Sinon, la cible est la page par défaut applicative.
                if (_seUser.AppUserId <= 0)
                    ApplyNavigation(callChain, loginPageName);
                else
                    ApplyNavigation(callChain, defaultPageName);
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct); }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct); }
            catch (OperationCanceledException) { throw; }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Dérogation au préfixe <c>ExecuteAsync</c> : Le nom alternatif
        /// <c>RefreshCurrentPageAsync</c> dérive d'un verbe d'action en anglais à l'impératif,
        /// au titre de la dérogation typologiquement bornée admise pour le cas Concept
        /// démultiplié (§4.2 du 0230, R-4.2.13 du 0231, §5.3.3 du 0232-UC).</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        public async Task RefreshCurrentPageAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(RefreshCurrentPageAsync)}";

            try
            {
                if (string.IsNullOrEmpty(_currentPageName))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        "Aucune page n'est actuellement chargée. Le rafraîchissement est impossible " +
                        "depuis l'état initial de navigation.");

                _srNavigation.RefreshCurrentPage(callChain);
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct); }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct); }
            catch (OperationCanceledException) { throw; }
        }

        // --- Groupe 3 : Gestion de l'historique ---

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Contexte : Cette méthode ne lève pas d'exception applicative et
        /// ne produit pas de notification. Elle est conçue pour être appelée lors de la
        /// déconnexion utilisateur ou d'une réinitialisation de session. Si le vidage du
        /// stack échoue pour une raison imprévue, l'exception remonte naturellement à
        /// l'appelant.</para>
        /// <para>Dérogation au préfixe <c>Execute</c> : Cette méthode synchrone porte
        /// un nom alternatif <c>ClearNavigationHistory</c> dérivé d'un verbe d'action en
        /// anglais à l'impératif, au titre de la dérogation typologiquement bornée admise
        /// pour le cas Concept démultiplié (§4.2 du 0230 « Execute si synchrones », R-4.2.13
        /// du 0231, §5.3.3 du 0232-UC). Aucun <c>CancellationToken</c> n'est reçu en raison
        /// de la nature synchrone et de la portée brève de l'opération.</para>
        /// </remarks>
        public void ClearNavigationHistory(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ClearNavigationHistory)}";

            _navigationHistory.Clear();
        }

        // --- Groupe 4 : Contrôle du menu horizontal ---

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Séquence interne :</para>
        /// <list type="number">
        ///   <item>Vérification qu'une page est actuellement chargée.</item>
        ///   <item>Lecture de l'URI du menu horizontal depuis le mapping courant en état runtime.</item>
        ///   <item>Délégation à <see cref="IS_Navigation.NavigateHorizontalMenu"/>.</item>
        /// </list>
        /// <para>Patron CallChain : Méthode publique — <c>_callee</c> inclus dans la
        /// <c>callChain</c>.</para>
        /// <para>Dérogation au préfixe <c>ExecuteAsync</c> : Le nom alternatif
        /// <c>ExpendHorizontalMenuAsync</c> dérive d'un verbe d'action en anglais à l'impératif,
        /// au titre de la dérogation typologiquement bornée admise pour le cas Concept
        /// démultiplié (§4.2 du 0230, R-4.2.13 du 0231, §5.3.3 du 0232-UC).</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        public async Task ExpendHorizontalMenuAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExpendHorizontalMenuAsync)}";

            try
            {
                if (_currentPageMapping is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_04,
                        "Aucune page n'est actuellement chargée. Le déploiement du menu horizontal " +
                        "est impossible depuis l'état initial de navigation.");

                _srNavigation.NavigateHorizontalMenu(callChain, _currentPageMapping.Value.MHUri);
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct); }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct); }
            catch (OperationCanceledException) { throw; }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Patron CallChain : Méthode publique — <c>_callee</c> inclus dans la
        /// <c>callChain</c>.</para>
        /// <para>Dérogation au préfixe <c>ExecuteAsync</c> : Le nom alternatif
        /// <c>ReduceHorizontalMenuAsync</c> dérive d'un verbe d'action en anglais à l'impératif,
        /// au titre de la dérogation typologiquement bornée admise pour le cas Concept
        /// démultiplié (§4.2 du 0230, R-4.2.13 du 0231, §5.3.3 du 0232-UC).</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché pendant
        /// l'opération, conformément à la doctrine d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        public async Task ReduceHorizontalMenuAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ReduceHorizontalMenuAsync)}";

            try
            {
                _srNavigation.ReduceHorizontalMenu(callChain, _seNavigation.MH_Reduce_Source);
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct); }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct); }
            catch (OperationCanceledException) { throw; }
        }

        // --- Groupe 5 : Lecture des droits utilisateur par page ---

        /// <summary>
        /// Indique si l'utilisateur courant peut accéder à la page par défaut de l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consulté par <c>MH_Generic.ApplyNavigationRules</c>
        /// pour conditionner la visibilité du bouton <c>MH_Home</c> sans codage en dur du nom
        /// logique de la page d'accueil. Ce prédicat encapsule la lecture de
        /// <see cref="ISE_Navigation.DefaultPageName"/> et l'évaluation du droit
        /// <c>CanAccess</c> sur cette page.</para>
        /// <para>Objectif : Permettre aux vues d'évaluer la disponibilité de la page
        /// d'accueil sans avoir à connaître son nom logique ni à injecter elles-mêmes
        /// <see cref="ISE_Navigation"/>. Le couplage entre référentiel des pages et droits
        /// d'accès reste centralisé dans <see cref="UC_Navigation"/>, conformément à son rôle
        /// de point de jonction normatif entre <see cref="ISE_Navigation"/> et
        /// <see cref="ISE_User"/>.</para>
        /// <para>Comportement : Retourne le résultat de <see cref="CanNavigate"/> évalué
        /// sur le nom de la page par défaut. Retourne <see langword="false"/> si la page par
        /// défaut n'est pas accessible à l'utilisateur courant.</para>
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> si l'utilisateur courant a le droit d'accès à la page par
        /// défaut ; <see langword="false"/> sinon.
        /// </returns>
        public bool CanNavigateToDefault() => CanNavigate(_seNavigation.DefaultPageName);

        /// <summary>
        /// Indique si l'utilisateur courant peut accéder à la page spécifiée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consulté par <c>MH_Generic.ApplySecurityRules</c>
        /// pour conditionner la visibilité d'un bouton qui déclencherait une navigation vers
        /// <paramref name="pageName"/>. Encapsule la lecture du droit <c>CanAccess</c> depuis
        /// <see cref="ISE_User"/>.</para>
        /// <para>Comportement : Retourne <see langword="false"/> si <paramref name="pageName"/>
        /// est null, vide ou inconnu, ou si le droit n'est pas explicitement accordé.</para>
        /// </remarks>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page10"</c>).</param>
        public bool CanNavigate(string pageName) => HasRight(pageName, r => r.CanAccess);

        /// <summary>
        /// Indique si l'utilisateur courant peut créer un enregistrement sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        public bool CanCreate(string pageName) => HasRight(pageName, r => r.CanCreate);

        /// <summary>
        /// Indique si l'utilisateur courant peut lire les données de la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        public bool CanRead(string pageName) => HasRight(pageName, r => r.CanRead);

        /// <summary>
        /// Indique si l'utilisateur courant peut modifier un enregistrement sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        public bool CanUpdate(string pageName) => HasRight(pageName, r => r.CanUpdate);

        /// <summary>
        /// Indique si l'utilisateur courant peut supprimer un enregistrement sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        public bool CanDelete(string pageName) => HasRight(pageName, r => r.CanDelete);

        /// <summary>
        /// Indique si l'utilisateur courant peut exercer un contrôle métier sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page40"</c>).</param>
        public bool CanControl(string pageName) => HasRight(pageName, r => r.CanControl);

        /// <summary>
        /// Indique si l'utilisateur courant peut valider une opération sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page40"</c>).</param>
        public bool CanValidate(string pageName) => HasRight(pageName, r => r.CanValidate);

        /// <summary>
        /// Indique si l'utilisateur courant peut superviser un traitement sur la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page50"</c>).</param>
        public bool CanSupervise(string pageName) => HasRight(pageName, r => r.CanSupervise);

        /// <summary>
        /// Indique si l'utilisateur courant peut consulter les données de monitoring de la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page60"</c>).</param>
        public bool CanMonitor(string pageName) => HasRight(pageName, r => r.CanMonitor);

        /// <summary>
        /// Indique si l'utilisateur courant peut administrer les paramètres de la page spécifiée.
        /// </summary>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page90"</c>).</param>
        public bool CanAdmin(string pageName) => HasRight(pageName, r => r.CanAdmin);

        #endregion

        #region === Méthodes privées ===

        // --- Groupe 1 : Orchestration de navigation ---

        /// <summary>
        /// Point d'entrée interne commun à toutes les opérations de navigation.
        /// Résout la page cible, met à jour l'état runtime et déclenche la séquence WPF.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode privée appelée par toutes les méthodes publiques
        /// de navigation après gestion de l'historique. Elle centralise la logique
        /// de résolution et d'application, évitant toute duplication entre les méthodes
        /// publiques.</para>
        /// <para>Patron CallChain : Méthode privée — pas de <c>_callee</c> dans la
        /// construction de la <c>callChain</c>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité construite par la méthode publique appelante.</param>
        /// <param name="pageName">Nom logique de la page cible, déjà validé et normalisé.</param>
        /// <exception cref="Ex_Business">
        /// Propagée depuis <see cref="ResolveTargetPage"/> si la page demandée ou la page de
        /// repli sont absentes du référentiel.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Propagée depuis <see cref="ExecuteWpfNavigationSequence"/> si l'exécution WPF
        /// échoue.
        /// </exception>
        private void ApplyNavigation(string caller, string pageName)
        {
            string callChain = $"{caller} > {nameof(ApplyNavigation)}";

            // 1. Résoudre la page cible réelle (validation existence + droits + repli éventuel)
            ResolveTargetPage(callChain, pageName, out string resolvedPageName, out PageMapping mapping);

            // 2. Mettre à jour l'état runtime avec la page réellement naviguée
            _currentPageName = resolvedPageName;
            _currentPageMapping = mapping;

            // 3. Déléguer la séquence d'exécution WPF
            ExecuteWpfNavigationSequence(callChain, mapping);
        }

        /// <summary>
        /// Résout la page cible effective en vérifiant son existence dans le référentiel
        /// et les droits d'accès de l'utilisateur courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Cette méthode est le point de jonction normatif entre
        /// <see cref="ISE_Navigation"/> (existence de la page) et <see cref="ISE_User"/>
        /// (droits d'accès). Elle encapsule l'ensemble de la logique décisionnelle de
        /// navigation.</para>
        /// <para>Règles appliquées :</para>
        /// <list type="number">
        ///   <item>La page doit exister dans <see cref="ISE_Navigation"/> — sinon
        ///   <see cref="Ex_Business"/> est levée.</item>
        ///   <item>La page de connexion (lue depuis
        ///   <see cref="ISE_Navigation.LoginPageName"/>) est exemptée du contrôle des droits.</item>
        ///   <item>Pour toutes les autres pages, <c>ISE_User.GetPageRights(pageName).CanAccess</c>
        ///   doit être <see langword="true"/>.</item>
        ///   <item>Si l'accès est refusé, la navigation est silencieusement redirigée vers
        ///   la page de repli lue depuis <see cref="ISE_Navigation.FallbackPageName"/>.
        ///   Si cette page de repli est elle-même absente du référentiel,
        ///   <see cref="Ex_Business"/> est levée.</item>
        /// </list>
        /// <para>Les noms logiques de la page de connexion et de la page de repli ne sont
        /// pas portés en constantes locales : ils sont lus directement depuis
        /// <see cref="ISE_Navigation"/> qui est la source d'autorité unique sur ces noms.</para>
        /// <para>Patron CallChain : Méthode privée — pas de <c>_callee</c>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis <see cref="ApplyNavigation"/>.</param>
        /// <param name="pageName">Nom logique de la page demandée.</param>
        /// <param name="resolvedPageName">
        /// En sortie : nom logique de la page effectivement naviguée.
        /// Peut différer de <paramref name="pageName"/> en cas de redirection vers la page de repli.
        /// </param>
        /// <param name="mapping">
        /// En sortie : <see cref="PageMapping"/> de la page effectivement naviguée.
        /// </param>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="pageName"/> est absente du référentiel, ou si la page de
        /// repli est elle-même absente du référentiel en cas de redirection nécessaire.
        /// </exception>
        private void ResolveTargetPage(
            string caller,
            string pageName,
            out string resolvedPageName,
            out PageMapping mapping)
        {
            string callChain = $"{caller} > {nameof(ResolveTargetPage)}";

            // Vérifier l'existence de la page dans le référentiel de navigation
            if (!_seNavigation.TryGetPageMapping(pageName, out mapping))
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_04,
                    $"La page '{pageName}' n'est pas référencée dans le référentiel de navigation " +
                    $"(ISE_Navigation). Vérifier la liste des suffixes déclarés dans SE_Navigation.");

            // La page de connexion est exemptée du contrôle des droits
            if (pageName == _seNavigation.LoginPageName)
            {
                resolvedPageName = pageName;
                return;
            }

            // Vérifier les droits d'accès de l'utilisateur courant pour cette page
            var rights = _seUser.GetPageRights(pageName);

            if (rights is not null && rights.CanAccess)
            {
                resolvedPageName = pageName;
                return;
            }

            // Accès refusé : redirection silencieuse vers la page de repli
            // La page de repli doit exister dans le référentiel — sinon erreur de configuration
            if (!_seNavigation.TryGetPageMapping(_seNavigation.FallbackPageName, out mapping))
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_04,
                    $"L'accès à '{pageName}' a été refusé et la page de repli '{_seNavigation.FallbackPageName}' " +
                    $"est absente du référentiel de navigation. Vérifier la configuration de SE_Navigation.");

            resolvedPageName = _seNavigation.FallbackPageName;
        }

        /// <summary>
        /// Exécute la séquence technique WPF de navigation en deux étapes ordonnées.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Cette méthode est appelée après que la décision de navigation
        /// a été prise et l'état runtime mis à jour. Elle délègue intégralement à
        /// <see cref="IS_Navigation"/> sans contenir aucune logique décisionnelle.</para>
        /// <para>Séquence :</para>
        /// <list type="number">
        ///   <item><see cref="IS_Navigation.NavigateToPage"/> — la frame de page est mise à jour
        ///   vers la vue XAML de la page résolue.</item>
        ///   <item><see cref="IS_Navigation.ReduceHorizontalMenu"/> — le menu horizontal est
        ///   maintenu à l'état réduit après toute navigation. Le déploiement du menu est
        ///   exclusivement déclenché par une action explicite de l'opérateur, via
        ///   <see cref="ExpendHorizontalMenuAsync"/>.</item>
        /// </list>
        /// <para>Patron CallChain : Méthode privée — pas de <c>_callee</c>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis <see cref="ApplyNavigation"/>.</param>
        /// <param name="mapping">Mapping de la page résolue, dont les URIs guident la séquence WPF.</param>
        /// <exception cref="Ex_Infrastructure">
        /// Propagée depuis <see cref="IS_Navigation"/> si l'exécution WPF échoue
        /// (MainWindow indisponible, Dispatcher inaccessible, etc.).
        /// </exception>
        private void ExecuteWpfNavigationSequence(string caller, PageMapping mapping)
        {
            string callChain = $"{caller} > {nameof(ExecuteWpfNavigationSequence)}";

            // Étape 1 — Navigation de la frame de page vers la vue XAML cible
            _srNavigation.NavigateToPage(callChain, mapping);

            // Étape 2 — Le menu horizontal est maintenu à l'état réduit après toute navigation.
            // Le déploiement du menu est exclusivement déclenché par une action explicite de l'opérateur.
            // L'URI du menu réduit est lue ici dans ISE_Navigation et transmise à SR_Navigation par
            // argument, conformément à la règle d'accès aux Settings : seul UC_Navigation lit ISE_Navigation.
            _srNavigation.ReduceHorizontalMenu(callChain, _seNavigation.MH_Reduce_Source);
        }

        // --- Groupe 2 : Lecture des droits utilisateur ---

        /// <summary>
        /// Évalue un prédicat de droit sur la page spécifiée en consultant
        /// <see cref="ISE_User.GetPageRights"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode privée mutualisée par tous les prédicats publics
        /// <c>CanXxx</c>. Elle encapsule la lecture des droits depuis <see cref="ISE_User"/>
        /// et applique une politique de fermeture par défaut : tout droit non explicitement
        /// accordé — y compris pour une page inconnue ou un nom invalide — est refusé.</para>
        /// <para>Politique de robustesse : Cette méthode ne lève pas d'exception
        /// et ne journalise pas. Elle est appelée à haute fréquence depuis le code-behind
        /// des composants de menu horizontal (<c>ApplySecurityRules</c>, exécuté à chaque
        /// chargement de menu) et depuis les ViewModels (évaluations <c>CanExecute</c>) :
        /// elle doit rester légère et déterministe.</para>
        /// <para>Patron CallChain : Aucun — les prédicats de droit ne participent pas
        /// à la <c>CallChain</c> applicative.</para>
        /// </remarks>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page20"</c>).</param>
        /// <param name="predicate">Prédicat appliqué aux droits de la page si ceux-ci existent.</param>
        /// <returns>
        /// <see langword="true"/> si la page est connue de <see cref="ISE_User"/> et si
        /// <paramref name="predicate"/> retourne <see langword="true"/> sur ses droits ;
        /// <see langword="false"/> dans tous les autres cas.
        /// </returns>
        private bool HasRight(string pageName, Func<PageRights, bool> predicate)
        {
            if (string.IsNullOrWhiteSpace(pageName))
                return false;

            var rights = _seUser.GetPageRights(pageName);
            return rights is not null && predicate(rights);
        }

        #endregion
    }
}