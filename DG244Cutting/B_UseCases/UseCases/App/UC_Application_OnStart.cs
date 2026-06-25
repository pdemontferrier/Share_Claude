using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.Infrastructure;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Services.User;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// UseCase orchestrateur de la séquence de démarrage applicatif de DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Implémentation du contrat <see cref="IU_Application_OnStart"/>
    /// résidant en <c>B_UseCases/UseCases/App/</c> conformément à la 1ère obligation contractuelle
    /// de §4.14.2 amendée et à la note [b] du tableau de §2.8 du 0230. Invoqué par
    /// <c>App.xaml.cs</c> au Jalon 3 de la séquence de démarrage applicatif (§3.10 du 0230),
    /// après Jalon 1 (construction du conteneur DI via <c>SR_ConteneurDI.ConfigureServices</c>)
    /// et Jalon 2 (traitement des arguments de ligne de commande).</para>
    /// <para>Objectif : Orchestrer linéairement les neuf étapes du Jalon 3 dans l'ordre
    /// normatif posé en §3.10 du 0230, et signaler à <c>App.xaml.cs</c> par valeur booléenne
    /// le verdict de démarrage (Jalon 4a si <see langword="true"/>, Jalon 4b si
    /// <see langword="false"/>, §3.10.6).</para>
    /// <para>Nature transactionnelle (item UC14) : UseCase non transactionnel par
    /// construction. Aucune mutation EF Core n'est portée — les seules écritures sont sur
    /// les Settings <see cref="ISE_App"/> et <see cref="ISE_User"/> (hors <c>DbContext</c>
    /// partagé), et la lecture de connectivité s'opère via la factory du Pattern 3 du câblage
    /// triple (§4.8.5 du 0230). Conformité par construction à I-4.10.3 (indépendance
    /// transactionnelle) ; aucun <c>BeginTransactionAsync</c>, <c>SaveChangesAsync</c>,
    /// <c>CommitAsync</c> ni <c>RollbackAsync</c>.</para>
    /// <para>Dérogations doctrinales assumées (à tracer nominativement) :</para>
    /// <list type="number">
    ///   <item><description>Dérogation initiale levée par le fil d'Extension
    ///   <c>UC_Application_OnStart_Extension</c>. Le UseCase consomme désormais
    ///   <see cref="IU_UserAppSession_Open"/> en sous-séquence à l'étape 6
    ///   (<see cref="IdentifyDeviceUserAndOpenSessionAsync"/>) conformément à R-4.14.21 ; les
    ///   trois conditions doctrinales conjointes (retour signalable <c>Task&lt;bool&gt;</c>
    ///   exploité par valeur, indépendance transactionnelle conforme à I-4.10.3, traitement
    ///   terminal propre via <see cref="IU_LogAndNotify"/> dans les catch typés de
    ///   <see cref="ExecuteAsync"/>) sont vérifiées par construction. Item UC22 ✅ après
    ///   extension.</description></item>
    ///   <item><description>Dérogation à §3.10.8 du 0230 (résolution du <c>cultureCode</c>).
    ///   La logique de priorité à 3 niveaux décrite en §3.10.8 (préférence utilisateur en base
    ///   puis préférence poste puis culture système) n'est pas implémentée. Le <c>cultureCode</c>
    ///   est transmis directement par <c>App.xaml.cs</c> sur la base de
    ///   <c>CultureInfo.CurrentCulture.Name</c>, sans lecture en base de la préférence
    ///   utilisateur ou poste. Cette simplification legacy est conservée à dessein.</description></item>
    ///   <item><description>Conformité explicite à R-3.10.3 (aucune propagation d'exception
    ///   applicative typée). Aucune <see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>
    ///   ni <see cref="Ex_Unclassified"/> n'est propagée par la méthode publique
    ///   <see cref="ExecuteAsync"/>. Chaque catch typé applicatif délègue à <see cref="IU_LogAndNotify"/>
    ///   et retourne <see langword="false"/>. Seule <see cref="OperationCanceledException"/> est
    ///   propagée sans requalification (R-4.6.13). La conversion du <see langword="false"/>
    ///   retourné par <see cref="IU_UserAppSession_Open"/> en
    ///   <see cref="Ex_Business"/> code <see cref="Ex_Business.ErrorCodes.BU_ER_04"/> à l'étape 6
    ///   (cf. <see cref="IdentifyDeviceUserAndOpenSessionAsync"/>) entre dans le périmètre de
    ///   cette conformité : l'<c>Ex_Business</c> levée est captée terminalement par le bloc
    ///   <c>catch (Ex_Business)</c> de <see cref="ExecuteAsync"/> avec clé dictionnaire
    ///   <c>"No_EC_01"</c>, homogène avec les refus applicatifs portés par les étapes 3, 4 et 9.</description></item>
    ///   <item><description>Élargissement de la chaîne UC → UC normalisée à une seconde
    ///   consommation en sous-séquence, et divergence de patron de propagation du retour
    ///   <see langword="false"/>, levés par le présent fil d'Extension
    ///   <c>UC_Application_OnStart_Extension</c> (deuxième Extension successive du même
    ///   couple). Le UseCase consomme désormais <see cref="IU_UserAppPageRight_Apply"/> en
    ///   sous-séquence à l'étape 8 (<see cref="ApplyUserPageRightAsync"/>) conformément à
    ///   R-4.14.21 ; les trois conditions doctrinales conjointes (retour signalable
    ///   <c>Task&lt;bool&gt;</c> exploité par valeur, indépendance transactionnelle conforme
    ///   à I-4.10.3 — les deux UseCases sont non transactionnels par construction,
    ///   traitement terminal propre via <see cref="IU_LogAndNotify"/> côté pipeline interne
    ///   du UseCase consommé) sont vérifiées par construction. La signalisation d'échec
    ///   applicatif de cette étape PROPAGE directement le retour <see langword="false"/>
    ///   jusqu'au <c>return false</c> de <see cref="ExecuteAsync"/> sans conversion en
    ///   <see cref="Ex_Business"/>, divergeant du patron homogène des étapes 3, 4, 6 et 9
    ///   qui convertissent toutes leur refus applicatif en <c>Ex_Business</c> code
    ///   <see cref="Ex_Business.ErrorCodes.BU_ER_04"/> captée par le bloc
    ///   <c>catch (Ex_Business)</c> terminal (clé <c>"No_EC_01"</c>). Cette divergence est
    ///   doctrinalement assumée au regard de l'item UC22 du 0232-UC qui prescrit
    ///   l'exploitation par valeur du retour signalable d'un UseCase consommé en
    ///   sous-séquence (cf. §4.3.3 du 0232-UC) ; elle évite par ailleurs une double
    ///   notification utilisateur, la notification d'échec étant déjà portée par le pipeline
    ///   interne de <c>UC_UserAppPageRight_Apply</c> via son propre
    ///   <see cref="IU_LogAndNotify"/>. Item UC22 demeure ✅ après extension ; nombre de
    ///   UseCases consommés en sous-séquence porté de un à deux.</description></item>
    ///   <item><description>Élargissement de la chaîne UC → UC normalisée à une troisième
    ///   consommation en sous-séquence, levée par le présent fil de Refactoring
    ///   <c>UC_ApplicationOnStart_Refactoring</c> par substitution de la dépendance
    ///   <c>IS_Language</c> par <see cref="IU_Language_Apply"/> à l'étape 1
    ///   (<see cref="ApplyCultureAsync"/>) conformément à R-4.14.21. La substitution
    ///   parachève la transposition de la responsabilité d'orchestration du changement
    ///   de langue vers la couche <c>B_UseCases</c>, consécutivement à la mise en place
    ///   du couple <see cref="IU_Language_Apply"/> / <c>UC_Language_Apply</c> en
    ///   remplacement du Service de présentation <c>IS_Language</c> / <c>SR_Language</c>.
    ///   Les trois conditions doctrinales conjointes (retour signalable
    ///   <c>Task&lt;bool&gt;</c> exploité par valeur, indépendance transactionnelle
    ///   conforme à I-4.10.3 — les deux UseCases sont non transactionnels par
    ///   construction, traitement terminal propre via <see cref="IU_LogAndNotify"/> côté
    ///   pipeline interne du UseCase consommé via les clés <c>"La_EC_01"</c>,
    ///   <c>"La_EC_02"</c>, <c>"La_EC_03"</c>) sont vérifiées par construction. La
    ///   signalisation d'échec applicatif de cette étape suit le même Patron A de
    ///   propagation directe du retour <see langword="false"/> que celui retenu à
    ///   l'étape 8 (cf. 4ème dérogation ci-dessus), évitant par analogie stricte la
    ///   double notification utilisateur — la notification d'échec étant déjà portée par
    ///   le pipeline interne de <c>UC_Language_Apply</c> via son propre
    ///   <see cref="IU_LogAndNotify"/>. Patron retenu pour la consommation UC → UC :
    ///   injection directe de <see cref="IU_Language_Apply"/> au constructeur
    ///   (Singleton → Scoped : licite, R-4.10.14 et P4-bis §4.10.10 du 0230 ; pas de
    ///   captive dependency dans ce sens) ; pas de passage par <c>IS_UseCaseInvoker</c>
    ///   (EA-11) — la mention <c>EA-11</c> du contrat <see cref="IU_Language_Apply"/>
    ///   ayant été préalablement requalifiée par le fil prédécesseur
    ///   <c>UC_Language_Apply_Refactoring</c> pour cantonner cette voie aux
    ///   consommateurs ViewModels. Item UC22 demeure ✅ après refactoring ; nombre de
    ///   UseCases consommés en sous-séquence porté de deux à trois.</description></item>
    /// </list>
    /// <para>Particularité du retour <c>Task&lt;bool&gt;</c> au regard de R-4.14.21
    /// (item UC21) : Le retour signalable <c>Task&lt;bool&gt;</c> n'est pas la marque d'une
    /// consommation en sous-séquence par un orchestrateur UseCase amont — le consommateur amont
    /// effectif est <c>App.xaml.cs</c>, point d'amorçage WPF situé hors de la famille UseCases.
    /// Le retour booléen porte une signalisation succès/échec applicatif captée terminalement
    /// par <c>App.xaml.cs</c> à l'issue du Jalon 3 pour arbitrer entre Jalon 4a (ouverture de
    /// <c>MainWindow</c>) et Jalon 4b (<c>Current.Shutdown()</c>), conformément à §3.10.6 du
    /// 0230. Item UC21 marqué ➖ en clôture (état pérenne, inchangé par l'extension).</para>
    /// <para>Découpage interne : La méthode publique <see cref="ExecuteAsync"/> orchestre
    /// linéairement neuf méthodes privées correspondant aux neuf étapes du Jalon 3
    /// (cf. région <c>=== Méthodes privées ===</c>). Pour les chemins de refus applicatif des
    /// étapes 3, 4, 6 et 9, une <see cref="Ex_Business"/> est levée intentionnellement avec
    /// code <see cref="Ex_Business.ErrorCodes.BU_ER_04"/> (état applicatif incompatible),
    /// captée par le bloc <c>catch (Ex_Business)</c> terminal qui délègue à
    /// <c>IU_LogAndNotify</c> avec clé <c>"No_EC_01"</c> et retourne <see langword="false"/> —
    /// patron strictement conforme à §4.7.2 du 0230 et R-4.7.3 du 0231. À l'étape 6, le refus
    /// est porté par conversion explicite du <see langword="false"/> retourné par
    /// <see cref="IU_UserAppSession_Open"/> en <see cref="Ex_Business"/> ; aux étapes 3, 4 et
    /// 9, le refus est levé directement par les méthodes privées correspondantes en cas
    /// d'état applicatif incompatible (connectivité base, disponibilité applicative, conflit
    /// de session). Les étapes 1 (<see cref="ApplyCultureAsync"/>) et 8
    /// (<see cref="ApplyUserPageRightAsync"/>) cohabitent avec ce patron homogène en portant
    /// un patron divergent commun (Patron A — propagation par valeur, R-4.14.21, item UC22)
    /// : le retour <see langword="false"/> de <see cref="IU_Language_Apply.ExecuteAsync"/>
    /// (étape 1) et de <see cref="IU_UserAppPageRight_Apply.ExecuteAsync"/> (étape 8) est
    /// propagé directement au <c>return false</c> de <see cref="ExecuteAsync"/> sans
    /// conversion en <see cref="Ex_Business"/>, pour éviter la double notification
    /// utilisateur (cf. 4ème et 5ème dérogations tracées ci-dessus).</para>
    /// </remarks>
    /// <seealso cref="IU_Application_OnStart"/>
    /// <seealso cref="IU_UserAppSession_Open"/>
    /// <seealso cref="IU_UserAppPageRight_Apply"/>
    public sealed class UC_Application_OnStart : IU_Application_OnStart
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du composant courant, résolu dynamiquement par <c>GetType().Name</c> pour la
        /// construction du segment local de la CallChain (§4.5 du 0230 ; R-4.5.5).
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IU_LogAndNotify _logAndNotify;
        private readonly IS_Dictionary _dictionary;
        private readonly ISE_App _settingsApp;
        private readonly ISE_User _settingsUser;
        private readonly IS_UserDeviceContext _userDeviceContext;
        private readonly IS_DigitTryDb_TestConnection _databaseConnectivity;
        private readonly IQ_UserApp _userAppQuery;
        private readonly IQ_UserAppSession _userAppSessionQuery;
        private readonly IQ_AppList _appListQuery;
        private readonly IU_UserAppSession_Open _userAppSessionOpen;
        private readonly IU_UserAppPageRight_Apply _userAppPageRightApply;
        private readonly IU_Language_Apply _languageApply;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du UseCase orchestrateur de la séquence de démarrage
        /// applicatif.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur appelé par le conteneur d'injection de dépendances
        /// (<c>SR_ConteneurDI</c>) lors de la résolution du contrat <see cref="IU_Application_OnStart"/>
        /// par <c>App.xaml.cs</c> au Jalon 1 de la séquence de démarrage applicatif (§3.10 du 0230).</para>
        /// <para>Objectif : Câbler les douze dépendances nécessaires à l'orchestration des
        /// neuf étapes du Jalon 3.</para>
        /// </remarks>
        /// <param name="logAndNotify">Pipeline terminal d'erreurs (catch typés applicatifs).</param>
        /// <param name="dictionary">Service de résolution des libellés multilingues.</param>
        /// <param name="settingsApp">Setting Singleton d'état applicatif partagé.</param>
        /// <param name="settingsUser">Setting Singleton de contexte utilisateur partagé.</param>
        /// <param name="userDeviceContext">Service d'alimentation du contexte poste.</param>
        /// <param name="databaseConnectivity">Service de diagnostic de connectivité base de données.</param>
        /// <param name="userAppQuery">Query Handler de l'entité <c>UserApp</c>.</param>
        /// <param name="userAppSessionQuery">Query Handler de l'entité <c>UserAppSession</c>.</param>
        /// <param name="appListQuery">Query Handler de l'entité <c>AppList</c>.</param>
        /// <param name="userAppSessionOpen">UseCase orchestré en sous-séquence pour l'ouverture de la session applicative utilisateur (R-4.14.21).</param>
        /// <param name="userAppPageRightApply">UseCase orchestré en sous-séquence pour l'initialisation systématique des droits de pages au moindre privilège et l'application conditionnelle des droits utilisateur (R-4.14.21).</param>
        /// <param name="languageApply">UseCase orchestré en sous-séquence pour l'application d'une langue à l'application — chargement du dictionnaire XAML, persistance du code culture, synchronisation du drapeau et synchronisation des quatre cibles CultureInfo .NET (R-4.14.21).</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_Application_OnStart(
            IU_LogAndNotify logAndNotify,
            IS_Dictionary dictionary,
            ISE_App settingsApp,
            ISE_User settingsUser,
            IS_UserDeviceContext userDeviceContext,
            IS_DigitTryDb_TestConnection databaseConnectivity,
            IQ_UserApp userAppQuery,
            IQ_UserAppSession userAppSessionQuery,
            IQ_AppList appListQuery,
            IU_UserAppSession_Open userAppSessionOpen,
            IU_UserAppPageRight_Apply userAppPageRightApply,
            IU_Language_Apply languageApply)
        {
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            _settingsApp = settingsApp ?? throw new ArgumentNullException(nameof(settingsApp));
            _settingsUser = settingsUser ?? throw new ArgumentNullException(nameof(settingsUser));
            _userDeviceContext = userDeviceContext ?? throw new ArgumentNullException(nameof(userDeviceContext));
            _databaseConnectivity = databaseConnectivity ?? throw new ArgumentNullException(nameof(databaseConnectivity));
            _userAppQuery = userAppQuery ?? throw new ArgumentNullException(nameof(userAppQuery));
            _userAppSessionQuery = userAppSessionQuery ?? throw new ArgumentNullException(nameof(userAppSessionQuery));
            _appListQuery = appListQuery ?? throw new ArgumentNullException(nameof(appListQuery));
            _userAppSessionOpen = userAppSessionOpen ?? throw new ArgumentNullException(nameof(userAppSessionOpen));
            _userAppPageRightApply = userAppPageRightApply ?? throw new ArgumentNullException(nameof(userAppPageRightApply));
            _languageApply = languageApply ?? throw new ArgumentNullException(nameof(languageApply));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <inheritdoc/>
        public async Task<bool> ExecuteAsync(string caller, string cultureCode, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Préconditions structurelles (validation -> ct, R-4.7.25).
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le paramètre 'caller' est null, vide ou composé uniquement d'espaces blancs au démarrage applicatif.");

                if (string.IsNullOrWhiteSpace(cultureCode))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le paramètre 'cultureCode' est null, vide ou composé uniquement d'espaces blancs au démarrage applicatif.");

                ct.ThrowIfCancellationRequested();

                // Étape 1 - Initialisation langue + dictionnaire + drapeau (§3.10, R-3.10.5).
                // Patron de propagation par valeur du retour signalable (R-4.14.21, item UC22 ;
                // cf. 5ème dérogation du <remarks> de classe).
                if (!await ApplyCultureAsync(callChain, cultureCode, ct))
                    return false;

                // Étape 2 - Mise à jour du titre applicatif (clé dictionnaire "App_Ti_00").
                LoadApplicationTitle(callChain, ct);

                // Étape 3 - Test de connectivité base de données + notification d'état.
                await CheckDatabaseConnectivityAsync(callChain, ct);

                // Étape 4 - Vérification de la disponibilité applicative (verrou administrateur).
                await CheckAppAvailabilityAsync(callChain, ct);

                // Étape 5 - Alimentation du contexte poste (§3.10.6, R-3.10.8).
                await LoadDeviceContextAsync(callChain, ct);

                // Étape 6 - Identification du DeviceUser et ouverture de session.
                await IdentifyDeviceUserAndOpenSessionAsync(callChain, ct);

                // Étape 7 - Chargement conditionnel du nom complet utilisateur (si AppUserId > 0).
                await LoadUserFullNameAsync(callChain, ct);

                // Étape 8 - Application des droits de pages (initialisation systématique au
                // moindre privilège + application conditionnelle des droits utilisateur).
                // Patron de propagation par valeur du retour signalable (R-4.14.21, item UC22 ;
                // cf. 4ème dérogation du <remarks> de classe).
                if (!await ApplyUserPageRightAsync(callChain, ct))
                    return false;

                // Étape 9 - Vérification de l'intégrité de session (absence de conflit sur autre poste).
                await CheckSessionIntegrityAsync(callChain, ct);

                return true;
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); return false; }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);  return false; }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);  return false; }
            catch (OperationCanceledException) { throw; }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Étape 1 — Applique la langue applicative correspondant au code culture fourni en
        /// consommant <see cref="IU_Language_Apply"/> en sous-séquence.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Première étape du Jalon 3, déléguée à
        /// <see cref="IU_Language_Apply"/> qui orchestre le chargement du dictionnaire XAML
        /// (<c>ISE_Language</c>), la persistance du code culture
        /// (<c>ISE_App.AppCultureCode</c>) déclenchant la cascade INPC vers le rechargement
        /// des libellés côté Presentation, la mise à jour du drapeau (<c>ISE_Flag</c>), et la
        /// synchronisation des quatre cibles standard de
        /// <see cref="System.Globalization.CultureInfo"/> .NET
        /// (<c>DefaultThreadCurrentCulture</c>, <c>DefaultThreadCurrentUICulture</c>,
        /// <c>Thread.CurrentThread.CurrentCulture</c>,
        /// <c>Thread.CurrentThread.CurrentUICulture</c>).</para>
        /// <para>Objectif : Initialiser le contexte multilingue avant tout libellé
        /// dictionnaire consommé en aval (notamment à l'étape 2).</para>
        /// </remarks>
        private Task<bool> ApplyCultureAsync(string callChain, string cultureCode, CancellationToken ct)
        {
            return _languageApply.ExecuteAsync(callChain, cultureCode, ct);
        }

        /// <summary>
        /// Étape 5 — Alimente le contexte poste (<see cref="ISE_User"/>) de l'utilisateur courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Cinquième étape du Jalon 3, déléguée à
        /// <see cref="IS_UserDeviceContext"/> qui résout machine, IPv4 et compte Windows puis
        /// écrit via <c>SetDeviceContext</c>.</para>
        /// <para>Objectif : Garantir que les étapes ultérieures disposent du contexte
        /// poste pour vérifier l'intégrité de session (étape 9) notamment.</para>
        /// </remarks>
        private Task LoadDeviceContextAsync(string callChain, CancellationToken ct)
        {
            return _userDeviceContext.ExecuteAsync(callChain, ct);
        }

        /// <summary>
        /// Étape 2 — Met à jour le titre applicatif via résolution dictionnaire de la clé
        /// <c>"App_Ti_00"</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Deuxième étape du Jalon 3. <see cref="IS_Dictionary.GetText"/>
        /// est une opération synchrone du Service Presentation ; la méthode locale est donc
        /// déclarée synchrone (pas de <c>async</c>) pour éviter le warning CS1998. La
        /// vérification d'annulation coopérative est effectuée explicitement en entrée
        /// (R-4.6.13).</para>
        /// <para>Objectif : Renseigner <c>ISE_App.ApplicationTitle</c> avec le libellé
        /// résolu, consommé en binding par la fenêtre principale.</para>
        /// </remarks>
        private void LoadApplicationTitle(string callChain, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            _settingsApp.ApplicationTitle = _dictionary.GetText(callChain, "App_Ti_00", ct);
        }

        /// <summary>
        /// Étape 6 — Identifie l'utilisateur applicatif associé au login Windows du poste courant
        /// et orchestre l'ouverture de la session applicative en consommant
        /// <see cref="IU_UserAppSession_Open"/> en sous-séquence.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Sixième étape du Jalon 3, exécutée après alimentation du
        /// contexte poste à l'étape 5 (<see cref="LoadDeviceContextAsync"/>). Lit le login Windows
        /// du poste courant depuis <c>ISE_User.AppDeviceUser</c>, interroge le Query Handler
        /// <see cref="IQ_UserApp.HandleGetByWindowsLoginAsync"/> pour résoudre l'utilisateur
        /// applicatif associé, met à jour <c>ISE_User.AppUserId</c> en cas de succès, puis
        /// orchestre l'ouverture de la session applicative en consommant
        /// <see cref="IU_UserAppSession_Open"/> en sous-séquence conformément à R-4.14.21.</para>
        /// <para>Objectif : Établir l'identité applicative effective de l'utilisateur du
        /// poste courant et garantir qu'une session applicative valide est ouverte avant que les
        /// étapes ultérieures (7 — nom complet utilisateur, 8 — application des droits de pages,
        /// 9 — intégrité de session) ne s'exécutent. En cas d'utilisateur Windows non inscrit en
        /// base ou logiquement supprimé, l'étape sort silencieusement sans erreur ;
        /// <c>AppUserId</c> conserve sa valeur courante (typiquement 0 si aucun identifiant n'a
        /// été transmis en ligne de commande au Jalon 2) et les étapes 7 et 9 sont sautées en
        /// interne par leurs gardes <c>AppUserId &lt;= 0</c> existantes ; l'étape 8
        /// (<see cref="ApplyUserPageRightAsync"/>) procède néanmoins à l'initialisation
        /// systématique au moindre privilège des droits de pages dans le contexte utilisateur
        /// partagé indépendamment de l'identification utilisateur (cf. contrat
        /// <see cref="IU_UserAppPageRight_Apply"/>), comportement assumé conforme à la sémantique
        /// de poursuite tolérée du démarrage applicatif. Arbitrage explicite du développeur tracé
        /// nominativement dans les fils d'extension successifs
        /// <c>UC_Application_OnStart_Extension</c>.</para>
        /// <para>Sortie silencieuse — cas de figure : Lorsque
        /// <see cref="IQ_UserApp.HandleGetByWindowsLoginAsync"/> retourne <see langword="null"/>
        /// (utilisateur Windows non inscrit en base) ou retourne une entité dont l'<c>Id</c> est
        /// inférieur ou égal à zéro (cas limite défensif), la méthode termine sans lever
        /// d'exception ni ouvrir de session ; la séquence orchestrée par
        /// <see cref="ExecuteAsync"/> se poursuit jusqu'à <see langword="true"/> en clôture, sauf
        /// refus ultérieur sur les étapes 8 ou 9 (l'étape 8 procède inconditionnellement à
        /// l'initialisation par défaut et ne refuse que sur référentiel <c>UserAppPage</c> vide ;
        /// l'étape 9 est sautée par sa garde <c>AppUserId &gt; 0</c>).</para>
        /// <para>Conversion du retour <see langword="false"/> de
        /// <see cref="IU_UserAppSession_Open"/> en <see cref="Ex_Business"/> : Lorsque le
        /// UseCase consommé en sous-séquence retourne <see langword="false"/> (échec d'ouverture
        /// de session signalé terminalement par <c>IU_UserAppSession_Open</c> via son propre
        /// <c>IU_LogAndNotify</c>), la méthode lève explicitement
        /// <see cref="Ex_Business"/> avec code <see cref="Ex_Business.ErrorCodes.BU_ER_04"/>.
        /// Cette exception est captée terminalement par le bloc <c>catch (Ex_Business)</c>
        /// existant de <see cref="ExecuteAsync"/> (clé dictionnaire <c>"No_EC_01"</c>),
        /// conformément à R-3.10.3 et au patron des refus applicatifs déjà en place pour les
        /// étapes 3 (<see cref="CheckDatabaseConnectivityAsync"/>), 4
        /// (<see cref="CheckAppAvailabilityAsync"/>) et 9
        /// (<see cref="CheckSessionIntegrityAsync"/>). L'étape 8
        /// (<see cref="ApplyUserPageRightAsync"/>) suit un patron divergent, cf. 4ème dérogation
        /// du <c>&lt;remarks&gt;</c> de classe.</para>
        /// <para>Signature <see cref="Task"/> simple au regard de R-4.14.21 : La méthode
        /// privée n'expose pas de retour signalable <c>Task&lt;bool&gt;</c> bien qu'elle
        /// consomme un UseCase à retour signalable en sous-séquence. La signalisation d'échec
        /// applicatif transite intégralement par levée explicite d'exception applicative typée
        /// (<see cref="Ex_Business"/>) captée terminalement par <see cref="ExecuteAsync"/>,
        /// conformément à la doctrine §4.7 du 0230 et au patron des étapes 3, 4 et 9 en place.
        /// Le retour <see cref="Task"/> simple est interne au UseCase et ne participe pas à la
        /// chaîne UC → UC normalisée — l'invocation de <see cref="IU_UserAppSession_Open"/> à
        /// l'intérieur de cette méthode privée constitue, elle, la participation effective à
        /// R-4.14.21.</para>
        /// </remarks>
        private async Task IdentifyDeviceUserAndOpenSessionAsync(string callChain, CancellationToken ct)
        {
            string windowsLogin = _settingsUser.AppDeviceUser;

            UserApp? userApp = await _userAppQuery.HandleGetByWindowsLoginAsync(callChain, windowsLogin, ct);

            if (userApp is null || userApp.Id <= 0)
                return;

            _settingsUser.AppUserId = userApp.Id;

            bool sessionOpened = await _userAppSessionOpen.ExecuteAsync(callChain, ct);

            if (!sessionOpened)
            {
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_04,
                    $"Échec d'ouverture de session applicative signalé par IU_UserAppSession_Open " +
                    $"(AppUserId={_settingsUser.AppUserId}, AppId={_settingsApp.AppId}) ; ouverture refusée.");
            }
        }

        /// <summary>
        /// Étape 7 — Charge conditionnellement le nom complet de l'utilisateur applicatif courant
        /// si <c>ISE_User.AppUserId &gt; 0</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Septième étape du Jalon 3. L'<c>AppUserId</c> a été
        /// éventuellement initialisé au Jalon 2 par <c>App.xaml.cs.ProcessStartupArguments</c>
        /// (argument <c>"iduser=N"</c> en ligne de commande), ou à l'étape 6 par
        /// <see cref="IdentifyDeviceUserAndOpenSessionAsync"/> en cas d'identification effective
        /// de l'utilisateur applicatif associé au login Windows du poste courant. Si aucun
        /// identifiant utilisateur n'a été transmis et que l'étape 6 n'a pas identifié
        /// d'utilisateur applicatif (sortie silencieuse), <c>AppUserId</c> vaut <c>0</c> et la
        /// présente étape est sautée sans erreur ; l'étape 8
        /// (<see cref="ApplyUserPageRightAsync"/>) procède néanmoins à l'initialisation
        /// systématique au moindre privilège des droits de pages indépendamment de
        /// l'identification utilisateur (cf. contrat <see cref="IU_UserAppPageRight_Apply"/>) ;
        /// l'étape 9 (<see cref="CheckSessionIntegrityAsync"/>) est ensuite sautée par sa propre
        /// garde <c>AppUserId &gt; 0</c>, et la séquence ExecuteAsync se poursuit jusqu'à
        /// <see langword="true"/> en clôture - comportement assumé conforme à la sémantique du
        /// démarrage applicatif (poursuite tolérée en l'absence d'utilisateur identifié).</para>
        /// <para>Objectif : Renseigner <c>ISE_User.AppUserFullName</c> à partir de la
        /// projection retournée par le Query Handler. La projection est garantie non nulle et
        /// non vide par contrat de <see cref="IQ_UserApp.HandleGetFullNameByIdAsync"/> ; la
        /// garde défensive sur <c>IsNullOrWhiteSpace</c> traduit la consigne explicite du prompt
        /// d'ouverture (« écriture si la chaîne retournée est non vide »).</para>
        /// </remarks>
        private async Task LoadUserFullNameAsync(string callChain, CancellationToken ct)
        {
            if (_settingsUser.AppUserId <= 0)
                return;

            string fullName = await _userAppQuery.HandleGetFullNameByIdAsync(callChain, _settingsUser.AppUserId, ct);

            if (!string.IsNullOrWhiteSpace(fullName))
                _settingsUser.AppUserFullName = fullName;
        }

        /// <summary>
        /// Étape 8 — Applique les droits de pages dans le contexte utilisateur partagé en
        /// consommant <see cref="IU_UserAppPageRight_Apply"/> en sous-séquence, selon une
        /// sémantique en deux temps : initialisation systématique au moindre privilège puis
        /// application conditionnelle des droits utilisateur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Huitième étape du Jalon 3, exécutée après le chargement
        /// conditionnel du nom complet utilisateur à l'étape 7
        /// (<see cref="LoadUserFullNameAsync"/>) et avant la vérification de l'intégrité de
        /// session à l'étape 9 (<see cref="CheckSessionIntegrityAsync"/>). Délègue
        /// l'orchestration au UseCase <see cref="IU_UserAppPageRight_Apply"/> consommé en
        /// sous-séquence conformément à R-4.14.21.</para>
        /// <para>Objectif : Constituer l'état des droits de pages dans le contexte
        /// utilisateur partagé (<c>ISE_User</c>) avant l'ouverture de <c>MainWindow</c>. Le
        /// UseCase consommé procède en deux temps - (1) initialisation systématique des droits
        /// de pages au moindre privilège indépendamment de la présence d'un utilisateur
        /// identifié dans le contexte applicatif, puis (2) chargement et application
        /// conditionnels des droits spécifiques de l'utilisateur courant lorsque
        /// <c>AppUserId &gt; 0</c>. Cette sémantique est constitutive du contrat
        /// <see cref="IU_UserAppPageRight_Apply"/> et garantit que les fonctionnalités de
        /// présentation consommant les droits de pages disposent d'un état cohérent au
        /// démarrage, y compris dans le cas de poursuite tolérée en l'absence d'utilisateur
        /// identifié.</para>
        /// <para>Divergence de patron de propagation du retour <see langword="false"/> au
        /// regard du patron des étapes 3, 4, 6 et 9 : Contrairement aux étapes 3
        /// (<see cref="CheckDatabaseConnectivityAsync"/>), 4
        /// (<see cref="CheckAppAvailabilityAsync"/>), 6
        /// (<see cref="IdentifyDeviceUserAndOpenSessionAsync"/>) et 9
        /// (<see cref="CheckSessionIntegrityAsync"/>) qui convertissent toutes leur refus
        /// applicatif en <see cref="Ex_Business"/> code
        /// <see cref="Ex_Business.ErrorCodes.BU_ER_04"/> captée terminalement par le bloc
        /// <c>catch (Ex_Business)</c> de <see cref="ExecuteAsync"/> (clé dictionnaire
        /// <c>"No_EC_01"</c>, notification <c>IU_LogAndNotify</c>), la présente étape PROPAGE
        /// directement la valeur booléenne <see langword="false"/> retournée par
        /// <see cref="IU_UserAppPageRight_Apply.ExecuteAsync"/> jusqu'au <c>return false</c>
        /// de <see cref="ExecuteAsync"/> sans levée d'exception applicative typée. Cette
        /// divergence est doctrinalement assumée au regard de l'item UC22 du 0232-UC qui
        /// prescrit l'exploitation par valeur du retour signalable d'un UseCase consommé en
        /// sous-séquence (cf. §4.3.3 du 0232-UC) ; elle évite par ailleurs une double
        /// notification utilisateur, la notification d'échec étant déjà portée par le pipeline
        /// interne de <c>UC_UserAppPageRight_Apply</c> via son propre
        /// <see cref="IU_LogAndNotify"/>. Le retour <see langword="false"/> couvre quatre cas
        /// d'échec applicatif capté terminalement par le pipeline interne du UseCase consommé
        /// - précondition structurelle <c>AppId &lt;= 0</c> (<c>Ex_Business BU_ER_02</c>),
        /// absence de page applicative non supprimée dans le référentiel <c>UserAppPage</c>
        /// empêchant l'initialisation par défaut (<c>Ex_Business BU_ER_04</c>), défaillance
        /// d'infrastructure (<c>Ex_Infrastructure</c>), défaillance applicative non
        /// classifiée (<c>Ex_Unclassified</c>). Trace nominative dans la 4ème dérogation du
        /// <c>&lt;remarks&gt;</c> de classe.</para>
        /// <para>Signature <see cref="Task{TResult}"/> de type <see langword="bool"/> au
        /// regard de R-4.14.21 : La méthode privée expose un retour signalable
        /// <c>Task&lt;bool&gt;</c> aligné sur la signature de
        /// <see cref="IU_UserAppPageRight_Apply.ExecuteAsync"/>, propagé par valeur à
        /// <see cref="ExecuteAsync"/> sans transformation. La signalisation succès/échec
        /// transite intégralement par cette valeur booléenne, conformément à la doctrine de
        /// chaîne UC → UC normalisée §4.14.21 et à l'invocation par valeur prescrite par
        /// l'item UC22 (cf. §4.3.3 du 0232-UC). L'<see cref="OperationCanceledException"/>
        /// remontée par le UseCase consommé n'est pas captée et est propagée naturellement au
        /// bloc <c>catch (OperationCanceledException)</c> de <see cref="ExecuteAsync"/>
        /// conformément à R-4.6.13.</para>
        /// </remarks>
        private Task<bool> ApplyUserPageRightAsync(string callChain, CancellationToken ct)
        {
            return _userAppPageRightApply.ExecuteAsync(callChain, ct);
        }

        /// <summary>
        /// Étape 3 — Teste la connectivité à la base de données et notifie l'état correspondant
        /// sur <see cref="ISE_App"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Troisième étape du Jalon 3, déléguée à
        /// <see cref="IS_DigitTryDb_TestConnection"/> qui réalise un diagnostic binaire via la
        /// primitive EF Core <c>Database.CanConnectAsync</c> sur un <c>DbContext</c> de courte
        /// durée (Pattern 3 du câblage triple, §4.8.5 du 0230).</para>
        /// <para>Objectif : Mettre à jour <c>ISE_App.IsConnected</c> (via
        /// <c>NotifyConnectionRestored</c> ou <c>NotifyConnectionLost</c>) puis, en cas
        /// d'indisponibilité, refuser le démarrage applicatif en levant une
        /// <see cref="Ex_Business"/> de code <see cref="Ex_Business.ErrorCodes.BU_ER_04"/>
        /// captée par le bloc <c>catch (Ex_Business)</c> terminal de
        /// <see cref="ExecuteAsync"/>.</para>
        /// </remarks>
        private async Task CheckDatabaseConnectivityAsync(string callChain, CancellationToken ct)
        {
            bool isConnected = await _databaseConnectivity.ExecuteAsync(callChain, ct);

            if (isConnected)
            {
                _settingsApp.NotifyConnectionRestored();
                return;
            }

            _settingsApp.NotifyConnectionLost();
            throw new Ex_Business(
                callChain,
                Ex_Business.ErrorCodes.BU_ER_04,
                "Connexion à la base de données indisponible au démarrage applicatif ; ouverture refusée.");
        }

        /// <summary>
        /// Étape 4 — Vérifie la disponibilité applicative (verrou administrateur).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Quatrième étape du Jalon 3, déléguée à
        /// <see cref="IQ_AppList.HandleAppAccessibilityAsync"/> qui interroge la table
        /// <c>AppList</c> pour vérifier que l'application identifiée par <c>ISE_App.AppId</c>
        /// est accessible (<c>Accessible == true</c>) et non supprimée logiquement.</para>
        /// <para>Objectif : Refuser le démarrage applicatif si l'application est marquée
        /// inaccessible (typiquement maintenance administrateur), en levant une
        /// <see cref="Ex_Business"/> de code <see cref="Ex_Business.ErrorCodes.BU_ER_04"/>.</para>
        /// </remarks>
        private async Task CheckAppAvailabilityAsync(string callChain, CancellationToken ct)
        {
            bool isAccessible = await _appListQuery.HandleAppAccessibilityAsync(callChain, _settingsApp.AppId, ct);

            if (isAccessible)
                return;

            throw new Ex_Business(
                callChain,
                Ex_Business.ErrorCodes.BU_ER_04,
                $"Application (AppId={_settingsApp.AppId}) indisponible ou non accessible au démarrage applicatif ; ouverture refusée.");
        }

        /// <summary>
        /// Étape 9 — Vérifie l'absence de conflit de session active sur un autre poste pour le
        /// couple (utilisateur, application) courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Neuvième étape du Jalon 3. Consomme
        /// <see cref="IQ_UserAppSession.HandleGetByUserIdAppIdAsync"/> pour récupérer la liste
        /// des sessions du couple (utilisateur, application), puis applique la réduction
        /// LINQ-to-Objects portée par <see cref="HasActiveSessionOnAnotherDevice"/>. En cas de
        /// conflit, le démarrage est refusé par levée d'une <see cref="Ex_Business"/> de code
        /// <see cref="Ex_Business.ErrorCodes.BU_ER_04"/>.</para>
        /// <para>Objectif : Garantir l'unicité de la session active de l'utilisateur sur
        /// l'application, conformément à la doctrine de cycle de session applicative.</para>
        /// </remarks>
        private async Task CheckSessionIntegrityAsync(string callChain, CancellationToken ct)
        {
            if (_settingsUser.AppUserId <= 0)
                return;

            List<UserAppSession> sessions = await _userAppSessionQuery.HandleGetByUserIdAppIdAsync(
                callChain,
                _settingsUser.AppUserId,
                _settingsApp.AppId,
                ct);

            if (!HasActiveSessionOnAnotherDevice(sessions))
                return;

            throw new Ex_Business(
                callChain,
                Ex_Business.ErrorCodes.BU_ER_04,
                $"Session active détectée sur un autre poste pour l'utilisateur (AppUserId={_settingsUser.AppUserId}, AppId={_settingsApp.AppId}) ; ouverture refusée.");
        }

        /// <summary>
        /// Indique si la liste de sessions fournie comporte au moins une session active sur un
        /// poste autre que celui de l'utilisateur courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode privée appelée par <see cref="CheckSessionIntegrityAsync"/>
        /// après réception du jeu de sessions retourné par
        /// <see cref="IQ_UserAppSession.HandleGetByUserIdAppIdAsync"/>.</para>
        /// <para>Objectif : Appliquer une réduction LINQ-to-Objects sur la collection en
        /// mémoire pour qualifier la présence d'un conflit de session.</para>
        /// <para>Caractère défensif-redondant du filtre par <c>IdApplication</c> et
        /// <c>IdUser</c> : Le prédicat du <c>Any(...)</c> redouble intentionnellement les
        /// critères <c>IdApplication == _settingsApp.AppId</c> et <c>IdUser == _settingsUser.AppUserId</c>
        /// déjà appliqués en amont par le Query Handler
        /// <see cref="IQ_UserAppSession.HandleGetByUserIdAppIdAsync"/>. Ce redoublement est une
        /// défense en profondeur : la collection reçue est par contrat déjà filtrée par le
        /// couple (utilisateur, application), mais le filtre local protège contre une éventuelle
        /// régression de contrat amont et explicite la condition métier complète (couple cible
        /// + poste distinct connecté) au lecteur du UseCase. Les seuls critères porteurs d'une
        /// décision métier propre à la présente méthode sont <c>DeviceId != _settingsUser.AppDeviceId</c>
        /// (poste différent du poste courant) et <c>IsConnected</c> (session effectivement
        /// active) ; les deux critères redondants n'ajoutent aucune décision métier.</para>
        /// <para>Caractère structurel de la réduction (non métier) : La transformation
        /// LINQ-to-Objects portée par cette méthode est une opération de filtrage et
        /// d'agrégation appliquée à un jeu de données déjà projeté en mémoire par le Query
        /// Handler. Elle ne porte aucune décision métier — la décision applicative (« refuser
        /// l'ouverture si conflit ») relève de <see cref="CheckSessionIntegrityAsync"/>. Cette
        /// réduction structurelle en couche B_UseCases est admise par §4.14.6 du 0230 (« la
        /// réduction LINQ-to-Objects d'un jeu de données déjà projeté demeure une opération
        /// de lecture et non une décision métier »), conformément à la consigne du prompt
        /// d'ouverture.</para>
        /// </remarks>
        /// <param name="sessions">
        /// Liste de sessions du couple (utilisateur, application) hors sessions logiquement
        /// supprimées, telle que retournée par
        /// <see cref="IQ_UserAppSession.HandleGetByUserIdAppIdAsync"/>. Les critères
        /// <c>IdApplication</c> et <c>IdUser</c> sont déjà appliqués en amont par le Query
        /// Handler ; la méthode applique un filtre défensif redondant sur ces deux critères en
        /// sus du filtre métier (<c>DeviceId</c> distinct du poste courant, <c>IsConnected</c>).
        /// </param>
        /// <returns><see langword="true"/> si au moins une session est active (<c>IsConnected == true</c>) sur un poste autre que <c>ISE_User.AppDeviceId</c> ; <see langword="false"/> sinon.</returns>
        private bool HasActiveSessionOnAnotherDevice(IEnumerable<UserAppSession> sessions)
        {
            return sessions.Any(s =>
                s.IdApplication == _settingsApp.AppId &&
                s.IdUser == _settingsUser.AppUserId &&
                s.DeviceId != _settingsUser.AppDeviceId &&
                s.IsConnected);
        }

        #endregion
    }
}