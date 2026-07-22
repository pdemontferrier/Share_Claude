using System.Linq.Expressions;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// UseCase orchestrateur de la détermination, à chaque invocation, de la nécessité
    /// pour l'application courante de se fermer d'elle-même, et du déclenchement le cas
    /// échéant de sa fermeture temporisée par délégation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/UseCases/App</c>. Il est résolu par injection de dépendances et
    /// constitue le maillon 2 d'une chaîne (4) de lecture au sens de R-4.14.19 :
    /// UC → QH → CR → DbContext (et non d'une chaîne (1) d'écriture stricte). Il est
    /// consommé en chaîne (1) directe par un ViewModel (typiquement <c>VM_Banner</c>
    /// orchestrant une boucle de polling) via <c>IS_UseCaseInvoker</c>, sans sous-séquence
    /// amont prévue (signature <see cref="Task"/> simple, sans retour signalable - la
    /// dimension iv-1 de la pré-qualification = NON, invariant 13 de §2.1 et R-4.14.21 non
    /// applicables au titre du retour signalable).
    /// </para>
    /// <para>
    /// Configuration typologique : ce UseCase relève du cas Concept à méthode publique
    /// unique - le segment <c>CloseCommand</c> désigne un concept fonctionnel (ordre de
    /// fermeture) et non une entité du domaine ; l'entité effectivement lue est
    /// <see cref="UserAppSessionCommand"/>, absente du nom du couple. Parallèle doctrinal :
    /// <c>UC_Navigation</c>, <c>UC_LogAndNotify</c>. Le UseCase est non transactionnel par
    /// construction : le scénario orchestré se limite à une lecture pure déléguée à deux
    /// Query Handlers, suivie d'une délégation à un autre UseCase - aucune mutation EF Core.
    /// Conséquences directes sur l'implémentation : aucune injection de <c>DbContext</c> ;
    /// aucune encapsulation dans <c>CreateExecutionStrategy().ExecuteAsync</c> ; aucun
    /// <c>BeginTransactionAsync</c>, <c>SaveChangesAsync</c>, <c>CommitAsync</c> ni
    /// <c>RollbackAsync</c>. L'item UC14 de la checklist §4.3 est sans objet sur le présent
    /// composant.
    /// </para>
    /// <para>
    /// Chaîne UC → UC normalisée (R-4.14.21) : ce UseCase consomme
    /// <see cref="IU_CloseApplication"/> en sous-séquence (dimension iv-2 = OUI). La chaîne
    /// est conforme : indépendance transactionnelle (I-4.10.3, UseCase non transactionnel)
    /// et traitement terminal propre via <see cref="IU_LogAndNotify"/> sont satisfaits ; le
    /// retour signalable <c>Task&lt;En_CloseResult&gt;</c> du UseCase consommé est
    /// exploitable mais volontairement non exploité par l'appelant terminal - la fermeture
    /// est la conséquence immédiate et unique de la détection, aucune issue n'est remontée
    /// à <c>VM_Banner</c>.
    /// </para>
    /// <para>
    /// Objectif : garantir qu'à chaque invocation, la nécessité de fermeture de
    /// l'application courante soit déterminée par une lecture CQRS traçable évaluant deux
    /// conditions distinctes - l'existence d'une demande de déconnexion adressée au couple
    /// utilisateur courant / application courante (ligne active dans
    /// <see cref="UserAppSessionCommand"/>) et la perte d'accessibilité de l'application
    /// courante dans le référentiel <c>AppList</c> - et que, si au moins l'une est vérifiée,
    /// la fermeture temporisée soit déléguée à <see cref="IU_CloseApplication"/> en mode
    /// delay, toute défaillance applicative étant traitée terminalement sans propagation à
    /// l'appelant. La cadence d'invocation est hors périmètre : elle relève d'un consommateur
    /// amont (boucle de polling de <c>VM_Banner</c>).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire la CallChain au format normatif et la propager aux deux Query Handlers consommés en aval et au UseCase délégué (R-4.5.5, R-4.5.7).</description></item>
    /// <item><description>Lire le contexte applicatif et utilisateur courant (<see cref="DG244Cutting.A_Domain.DTOs.App.DTO_AppContext.AppId"/>, <see cref="DG244Cutting.A_Domain.DTOs.App.DTO_AppContext.AppUserId"/>) via <see cref="IS_AppContext.GetAppContext"/>.</description></item>
    /// <item><description>Évaluer en court-circuit les deux conditions de fermeture (commande de session active, non-accessibilité applicative).</description></item>
    /// <item><description>Déléguer la fermeture temporisée à <see cref="IU_CloseApplication"/> en mode delay lorsqu'au moins une condition est vérifiée.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/> dans les trois <c>catch</c> typés (R-4.7.14, I-4.7.6).</description></item>
    /// <item><description>Propager <see cref="OperationCanceledException"/> à l'appelant sans journalisation ni notification (§4.6).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne valide aucune précondition structurelle : la précondition <c>id &gt; 0</c> est portée par les Query Handlers en aval (code <c>BU_ER_02</c>). L'item UC9 de la checklist §4.3 est sans objet sur le présent composant.</description></item>
    /// <item><description>N'ouvre aucune transaction et ne réalise aucune mutation EF Core : scénario non transactionnel par construction (I-4.10.1 trivialement respectée).</description></item>
    /// <item><description>N'exploite pas l'issue de la délégation à <see cref="IU_CloseApplication"/> : la fermeture est la conséquence immédiate et unique de la détection.</description></item>
    /// <item><description>Ne réalise aucun accès EF Core métier ni aucune requalification d'exception : ces rôles relèvent des Query Handlers et des Handlers en aval (I-4.14.4 amendée).</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler (I-4.14.4 amendée) ni un Repository.</description></item>
    /// <item><description>Ne pilote pas la cadence d'invocation : celle-ci relève du consommateur amont (boucle de polling de <c>VM_Banner</c>).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_CloseCommand_Check"/>
    public class UC_CloseCommand_Check : IU_CloseCommand_Check
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        /// <summary>
        /// Code d'erreur spécifique du module métier signant la trace informationnelle
        /// de fermeture forcée de l'application émise dans le flux nominal de
        /// <see cref="ExecuteAsync"/> lorsqu'au moins une condition de fermeture est
        /// vérifiée.
        /// </summary>
        /// <remarks>
        /// <para>Code spécifique au format <c>xxxx_ER_NN</c> conforme R-4.7.9
        /// (interdiction du littéral pour <c>errorId</c>, obligation de la constante
        /// privée). Porté par le paramètre <see cref="Ex_Business.ErrorId"/> d'une
        /// <see cref="Ex_Business"/> construite comme simple conteneur de message et
        /// jamais levée, transmise à <see cref="IU_LogAndNotify"/> avec la clé
        /// dictionnaire <c>No_EC_01</c> et <c>notify = false</c> (journalisation seule).
        /// À la différence des trois catch typés qui traitent une exception effectivement
        /// levée, cet usage relève d'une trace informationnelle émise hors catch dans le
        /// flux nominal (dérogation <c>EA-NN-LogInfoViaPipelineErreur</c> tracée dans le
        /// <c>remarks</c> de <see cref="ExecuteAsync"/>).</para>
        /// <para>Inventaire centralisé en
        /// <c>E_Miscellaneous/Documentation/Exceptions/SpecificErrorCodes.md</c>
        /// conformément à R-4.7.10 (origine : Module métier ; composant
        /// générateur : <c>UC_CloseCommand_Check</c>).</para>
        /// </remarks>
        private const string CLOS_ER_01 = "CLOS_ER_01";

        #endregion

        #region === Dépendances privées ===

        private readonly IS_AppContext _appContext;
        private readonly IQ_Generic<UserAppSessionCommand> _qhSessionCommand;
        private readonly IQ_AppList _qhAppList;
        private readonly IU_CloseApplication _closeApplication;
        private readonly ISE_App _settingsApp;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_CloseCommand_Check"/>
        /// avec ses dépendances.
        /// </summary>
        /// <param name="appContext">
        /// Service de fourniture du contexte applicatif courant, source des identifiants
        /// d'application et d'utilisateur courants (<c>AppId</c>, <c>AppUserId</c>) via
        /// <see cref="IS_AppContext.GetAppContext"/>.
        /// </param>
        /// <param name="qhSessionCommand">
        /// Query Handler générique de lecture sur l'entité <see cref="UserAppSessionCommand"/>,
        /// support de la vérification d'existence d'une commande de fermeture active pour le
        /// couple utilisateur courant / application courante via
        /// <see cref="IQ_Generic{T}.HandleAnyByPredicateAsync"/>.
        /// </param>
        /// <param name="qhAppList">
        /// Query Handler de lecture sur le référentiel <c>AppList</c>, support de la
        /// vérification d'accessibilité de l'application courante via
        /// <see cref="IQ_AppList.HandleAppAccessibilityAsync"/>.
        /// </param>
        /// <param name="closeApplication">
        /// UseCase de fermeture applicative, cible de la délégation de la fermeture
        /// temporisée en mode delay (chaîne UC → UC normalisée conforme R-4.14.21).
        /// </param>
        /// <param name="settingsApp">
        /// Singleton applicatif fournissant le délai de temporisation
        /// (<see cref="ISE_App.NotificationDelay"/>) transmis à
        /// <see cref="IU_CloseApplication"/> comme paramètre <c>delaySeconds</c>.
        /// </param>
        /// <param name="logAndNotify">
        /// Pipeline terminal de journalisation et de notification des erreurs.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'un des paramètres est <see langword="null"/>.
        /// </exception>
        public UC_CloseCommand_Check(
            IS_AppContext appContext,
            IQ_Generic<UserAppSessionCommand> qhSessionCommand,
            IQ_AppList qhAppList,
            IU_CloseApplication closeApplication,
            ISE_App settingsApp,
            IU_LogAndNotify logAndNotify)
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _qhSessionCommand = qhSessionCommand ?? throw new ArgumentNullException(nameof(qhSessionCommand));
            _qhAppList = qhAppList ?? throw new ArgumentNullException(nameof(qhAppList));
            _closeApplication = closeApplication ?? throw new ArgumentNullException(nameof(closeApplication));
            _settingsApp = settingsApp ?? throw new ArgumentNullException(nameof(settingsApp));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Détermine, à chaque invocation, si l'application courante doit se fermer
        /// d'elle-même et, le cas échéant, déclenche sa fermeture temporisée par
        /// délégation à <see cref="IU_CloseApplication"/> en mode delay, sans retour vers
        /// l'appelant.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : maillon 2 d'une chaîne (4) de lecture (UC → QH → CR → DbContext) au
        /// sens de R-4.14.19. Le scénario se limite à la lecture du contexte courant, à
        /// l'évaluation court-circuitée de deux conditions de fermeture déléguées à deux
        /// Query Handlers, puis à une éventuelle délégation de fermeture ; aucune
        /// transaction EF Core n'est ouverte, aucune mutation persistante n'est réalisée.
        /// En conséquence, le présent <c>ExecuteAsync</c> n'encapsule aucun bloc
        /// transactionnel dans <c>CreateExecutionStrategy().ExecuteAsync</c> et n'invoque
        /// ni <c>BeginTransactionAsync</c>, ni <c>SaveChangesAsync</c>, ni
        /// <c>CommitAsync</c>, ni <c>RollbackAsync</c>. Le patron terminal des catch typés
        /// reste néanmoins entièrement appliqué : trois catch typés
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) déléguant à <see cref="IU_LogAndNotify"/> avec les
        /// clés <c>No_EC_01</c> / <c>No_EC_02</c> / <c>No_EC_03</c>, et un catch distinct sur
        /// <see cref="OperationCanceledException"/> propageant à l'appelant conformément à
        /// §4.6.
        /// </para>
        /// <para>
        /// Aucune précondition structurelle n'est validée à l'intérieur du <c>try</c> :
        /// les identifiants <c>AppId</c> et <c>AppUserId</c> proviennent d'un Singleton de
        /// contexte considéré comme toujours valide, et la précondition <c>id &gt; 0</c> est
        /// portée par les Query Handlers en aval qui lèveront <see cref="Ex_Business"/>
        /// (code <c>BU_ER_02</c>) le cas échéant - exception captée par le catch typé du
        /// présent UseCase. L'évaluation des deux conditions est court-circuitée : la
        /// commande de session est évaluée d'abord et, si elle est vérifiée,
        /// l'accessibilité applicative n'est pas interrogée (l'ordre est sans incidence
        /// fonctionnelle, il optimise le coût de requête). La fermeture n'est jamais
        /// déclenchée sans qu'au moins une des deux conditions soit vérifiée ; une itération
        /// sans condition remplie est sans effet observable (idempotence de détection).
        /// </para>
        /// <para>
        /// Dérogation <c>EA-NN-LogInfoViaPipelineErreur</c> : lorsqu'au moins une condition
        /// de fermeture est vérifiée, une trace informationnelle de fermeture forcée est
        /// émise dans le flux nominal, avant la délégation de fermeture temporisée, par appel
        /// à <see cref="IU_LogAndNotify"/>. Cet usage déroge à la vocation nominale du
        /// pipeline : <see cref="IU_LogAndNotify"/> est un pipeline terminal d'erreur, appelé
        /// exclusivement depuis les blocs <c>catch</c> après réception d'une exception typée ;
        /// il est ici mobilisé hors <c>catch</c> pour une trace informationnelle. L'
        /// <see cref="Ex_Business"/> transmise est construite comme simple conteneur de message
        /// (jamais levée) ; le code spécifique <c>CLOS_ER_01</c> porteur du sens métier est
        /// transporté par son <see cref="Ex_Business.ErrorId"/>, la clé dictionnaire
        /// <c>No_EC_01</c> étant celle du canal <see cref="Ex_Business"/> standard. Le paramètre
        /// <c>notify:false</c> garantit la journalisation seule (aucune notification opérateur).
        /// Le non-blocage du scénario de détection/fermeture est garanti par le best-effort
        /// documenté du pipeline, qui n'interrompt jamais le flux appelant. Les catch typés
        /// ci-après demeurent inchangés dans leur rôle terminal de traitement des exceptions
        /// effectivement levées.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire la CallChain au format normatif et la propager aux deux Query Handlers et au UseCase délégué (R-4.5.5, R-4.5.7).</description></item>
        /// <item><description>Lire le contexte applicatif et utilisateur courant via <see cref="IS_AppContext.GetAppContext"/>.</description></item>
        /// <item><description>Évaluer en court-circuit les deux conditions de fermeture.</description></item>
        /// <item><description>Déléguer la fermeture temporisée à <see cref="IU_CloseApplication"/> en mode delay lorsqu'au moins une condition est vérifiée.</description></item>
        /// <item><description>Déléguer à <see cref="IU_LogAndNotify"/> en cas d'exception applicative typée.</description></item>
        /// <item><description>Propager <see cref="OperationCanceledException"/> à l'appelant sans journalisation ni notification (§4.6).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative. Par défaut <see langword="default"/>.
        /// </param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée,
        /// conformément à §4.6. Les exceptions applicatives typées
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles sont captées
        /// terminalement par les trois <c>catch</c> typés et traitées par
        /// <see cref="IU_LogAndNotify"/>.
        /// </exception>
        public async Task ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Lecture du contexte applicatif et utilisateur courant via le service
                // dédié. IS_AppContext agrège les Settings courants ; AppId et AppUserId
                // sont considérés comme toujours valides. La précondition id > 0 est
                // portée par les Query Handlers en aval (code BU_ER_02), captée le cas
                // échéant par le catch typé Ex_Business du présent UseCase.
                DTO_AppContext context = _appContext.GetAppContext();

                // Évaluation court-circuitée des deux conditions de fermeture.
                // Condition (a) évaluée d'abord (commande de session active pour le couple
                // courant) ; si elle est vérifiée, l'opérateur || court-circuite l'appel à
                // HandleAppAccessibilityAsync (condition (b)). L'ordre est sans incidence
                // fonctionnelle : il optimise le coût de requête. Chaînes (4) de lecture
                // R-4.14.19 (UC → QH → CR → DbContext) - aucune transaction côté UC.
                Expression<Func<UserAppSessionCommand, bool>> hasActiveCommand =
                    c => c.IdUserTarget == context.AppUserId
                         && c.IdApplicationTarget == context.AppId
                         && !c.IsDeleted;

                bool fermetureJustifiee =
                    await _qhSessionCommand.HandleAnyByPredicateAsync(callChain, hasActiveCommand, ct)
                    || !(await _qhAppList.HandleAppAccessibilityAsync(callChain, context.AppId, ct));

                if (fermetureJustifiee)
                {
                    // Trace informationnelle de fermeture forcée émise en fire-and-forget
                    // AVANT la délégation de fermeture. Dérogation EA-NN-LogInfoViaPipelineErreur :
                    // IU_LogAndNotify est doctrinalement un pipeline terminal d'erreur, appelé
                    // exclusivement depuis les catch après réception d'une exception typée ; il
                    // est ici détourné vers une trace informationnelle émise dans le flux nominal
                    // (hors catch). L'Ex_Business est construite comme simple conteneur de message
                    // et n'est JAMAIS levée ; le code spécifique CLOS_ER_01 porteur du sens métier
                    // est transporté par son paramètre errorId. La clé "No_EC_01" est celle du
                    // canal Ex_Business standard. notify:false garantit l'absence de notification
                    // opérateur (journalisation seule). L'appel est non bloquant par la garantie
                    // best-effort documentée du pipeline (IU_LogAndNotify n'interrompt jamais le
                    // flux appelant) : le scénario de détection/fermeture n'est jamais interrompu
                    // par cette trace. Le ct courant et la callChain déjà construite sont propagés.
                    await _logAndNotify.ExecuteAsync(
                        callChain,
                        "No_EC_01",
                        new Ex_Business(
                            callChain: callChain,
                            errorId: CLOS_ER_01,
                            errorException: "Fermeture forcée de l'application par l'administrateur."),
                        notify: false,
                        ct: ct);

                    // Délégation de la fermeture temporisée en mode delay (confirmation
                    // absente, préavis temporisé). L'issue En_CloseResult retournée est
                    // volontairement non exploitée : chaîne UC → UC normalisée conforme
                    // R-4.14.21, le présent UseCase est appelant terminal. Le paramètre
                    // delaySeconds est alimenté par ISE_App.NotificationDelay.
                    await _closeApplication.ExecuteAsync(
                        callChain,
                        confirmation: false,
                        warning: false,
                        delaySeconds: _settingsApp.NotificationDelay,
                        ct);
                }

                // Sinon : fin sans effet observable (idempotence de détection).
            }
            catch (Ex_Business ex)
            {
                // Pas de RollbackAsync : aucune transaction n'a été ouverte
                // (non transactionnel par construction).
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
            }
            catch (Ex_Unclassified ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
            }
            catch (OperationCanceledException)
            {
                // Annulation coopérative : propagation directe à l'appelant
                // conformément à §4.6.
                // Pas de RollbackAsync ici - aucune transaction n'a été ouverte.
                // Pas d'appel à IU_LogAndNotify - l'annulation n'est pas une erreur.
                throw;
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}