using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// UseCase orchestrateur de la vérification de présence de messages applicatifs
    /// non lus pour l'application courante.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/UseCases/User</c>. Il est résolu par injection de dépendances et
    /// constitue le maillon 2 d'une chaîne (4) de lecture au sens de R-4.14.19 :
    /// UC → QH → CR → DbContext (et non d'une chaîne (1) d'écriture stricte
    /// VM → UC → SR → CH → CR). Il est consommé en chaîne (1) directe par un
    /// ViewModel (typiquement VM_BA_MainWindow orchestrant la boucle de polling de
    /// présence de messages non lus), sans sous-séquence amont prévue (signature
    /// <see cref="Task"/> simple, sans retour signalable - dimension iv-1 et iv-2 de
    /// la pré-qualification = NON, invariant 13 de §2.1 et R-4.14.21 non applicables).
    /// </para>
    /// <para>
    /// Configuration typologique atypique : ce UseCase relève du cas Entité
    /// (segment [Entité] = <c>UserAppMessage</c> + segment [Action] = <c>CheckUnread</c>
    /// obligatoire, R-4.14.7 amendée) mais est non transactionnel par construction.
    /// Le scénario orchestré se limite à une lecture déléguée au Query Handler
    /// <see cref="IQ_UserAppMessage.HandleGetAnyMessageNotReadAsync"/>, suivie de la
    /// propagation inconditionnelle du résultat booléen sur la propriété
    /// <see cref="ISE_App.HasUnreadMessages"/> du Singleton applicatif - mutation en
    /// mémoire sans aucun appel EF Core. Conséquences directes sur l'implémentation :
    /// aucune injection de <c>DbContext</c> ; aucune encapsulation dans
    /// <c>CreateExecutionStrategy().ExecuteAsync</c> ; aucun <c>BeginTransactionAsync</c>,
    /// <c>SaveChangesAsync</c>, <c>CommitAsync</c> ni <c>RollbackAsync</c>. L'item UC14
    /// de la checklist §4.3 est sans objet sur le présent composant. Combinaison
    /// inédite parmi les étalons disponibles (les UCs Entité existants sur
    /// <c>UserAppMessage</c> - <c>UC_UserAppMessage_Add</c> et
    /// <c>UC_UserAppMessage_MarkAsRead</c> - sont transactionnels ; les UCs non
    /// transactionnels disponibles - <c>UC_Navigation</c>, <c>UC_LogAndNotify</c> -
    /// sont cas Concept), n'enfreignant cependant aucune règle normative explicite :
    /// la sous-classe est dictée par le nommage (R-4.14.7 amendée), la nature
    /// transactionnelle est dictée par la présence ou non d'une mutation EF Core
    /// (R-4.10.1) ; les deux axes sont orthogonaux.
    /// </para>
    /// <para>
    /// Objectif : garantir qu'à chaque invocation, l'état de présence de messages non
    /// lus pour l'application courante (<see cref="ISE_App.AppId"/>) soit déterminé
    /// par une lecture CQRS traçable et propagé sur <see cref="ISE_App.HasUnreadMessages"/>,
    /// et que toute défaillance applicative soit traitée terminalement sans propagation
    /// à l'appelant. La cadence d'invocation est hors périmètre : elle relève d'un
    /// consommateur amont (typiquement la boucle de polling de VM_BA_MainWindow).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire la CallChain au format normatif et la propager au Query Handler consommé en aval (R-4.5.5, R-4.5.7).</description></item>
    /// <item><description>Lire l'identifiant d'application courante via <see cref="ISE_App.AppId"/>.</description></item>
    /// <item><description>Déléguer la vérification d'existence à <see cref="IQ_UserAppMessage.HandleGetAnyMessageNotReadAsync"/>.</description></item>
    /// <item><description>Écrire inconditionnellement le résultat booléen sur <see cref="ISE_App.HasUnreadMessages"/>.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/> dans les trois <c>catch</c> typés (R-4.7.14, I-4.7.6).</description></item>
    /// <item><description>Propager <see cref="OperationCanceledException"/> à l'appelant sans journalisation ni notification (§4.6).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne valide pas la précondition structurelle sur <c>appId</c> : cette validation est portée par le Query Handler en aval (code <c>BU_ER_02</c>). L'item UC9 de la checklist §4.3 est sans objet sur le présent composant.</description></item>
    /// <item><description>N'ouvre aucune transaction et ne réalise aucune mutation EF Core : scénario non transactionnel par construction (I-4.10.1 trivialement respectée).</description></item>
    /// <item><description>Ne réalise aucun accès EF Core métier ni aucune requalification d'exception : ces rôles relèvent du Query Handler et des Handlers en aval (I-4.14.4 amendée).</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler (I-4.14.4 amendée) ni un Repository.</description></item>
    /// <item><description>Ne pilote pas la cadence d'invocation : celle-ci relève du consommateur amont (typiquement la boucle de polling de VM_BA_MainWindow).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_UserAppMessage_CheckUnread"/>
    public class UC_UserAppMessage_CheckUnread : IU_UserAppMessage_CheckUnread
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserAppMessage _qhUserAppMessage;
        private readonly ISE_App _settingsApp;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_UserAppMessage_CheckUnread"/>
        /// avec ses dépendances.
        /// </summary>
        /// <param name="qhUserAppMessage">
        /// Query Handler de lecture sur l'entité <c>UserAppMessage</c>, support de la
        /// vérification d'existence d'au moins un message non lu pour l'application
        /// courante via <see cref="IQ_UserAppMessage.HandleGetAnyMessageNotReadAsync"/>.
        /// </param>
        /// <param name="settingsApp">
        /// Singleton applicatif fournissant l'identifiant d'application courante
        /// (<see cref="ISE_App.AppId"/>) et cible de la propagation du résultat sur
        /// <see cref="ISE_App.HasUnreadMessages"/>.
        /// </param>
        /// <param name="logAndNotify">
        /// Pipeline terminal de journalisation et de notification des erreurs.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'un des paramètres est <see langword="null"/>.
        /// </exception>
        public UC_UserAppMessage_CheckUnread(
            IQ_UserAppMessage qhUserAppMessage,
            ISE_App settingsApp,
            IU_LogAndNotify logAndNotify)
        {
            _qhUserAppMessage = qhUserAppMessage ?? throw new ArgumentNullException(nameof(qhUserAppMessage));
            _settingsApp = settingsApp ?? throw new ArgumentNullException(nameof(settingsApp));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Orchestre la vérification de la présence d'au moins un message applicatif
        /// non lu adressé à l'application courante, puis propage le résultat sur la
        /// propriété <see cref="ISE_App.HasUnreadMessages"/> du Singleton applicatif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : maillon 2 d'une chaîne (4) de lecture (UC → QH → CR → DbContext)
        /// au sens de R-4.14.19. Le scénario se limite à une lecture déléguée au Query
        /// Handler suivie d'une écriture en mémoire sur le Singleton applicatif ; aucune
        /// transaction EF Core n'est ouverte, aucune mutation persistante n'est réalisée.
        /// En conséquence, le présent <c>ExecuteAsync</c> n'encapsule aucun bloc
        /// transactionnel dans <c>CreateExecutionStrategy().ExecuteAsync</c> et n'invoque
        /// ni <c>BeginTransactionAsync</c>, ni <c>SaveChangesAsync</c>, ni
        /// <c>CommitAsync</c>, ni <c>RollbackAsync</c>. Le patron terminal des catch
        /// typés reste néanmoins entièrement appliqué : trois catch typés
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) déléguant à <see cref="IU_LogAndNotify"/> avec
        /// les clés <c>No_EC_01</c> / <c>No_EC_02</c> / <c>No_EC_03</c>, et un catch
        /// distinct sur <see cref="OperationCanceledException"/> propageant à l'appelant
        /// conformément à §4.6.
        /// </para>
        /// <para>
        /// Aucune précondition structurelle n'est validée à l'intérieur du <c>try</c> :
        /// l'identifiant d'application provient d'un Singleton de configuration considéré
        /// comme toujours valide, et la précondition <c>appId &gt; 0</c> est portée par
        /// le Query Handler en aval qui lèvera <see cref="Ex_Business"/> (code
        /// <c>BU_ER_02</c>) le cas échéant - exception captée par le catch typé du
        /// présent UseCase. L'écriture sur <see cref="ISE_App.HasUnreadMessages"/> est
        /// inconditionnelle (sans test d'égalité préalable) : le setter du Singleton
        /// protège déjà l'émission INPC par un test d'égalité interne.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire la CallChain au format normatif et la propager au Query Handler consommé en aval (R-4.5.5, R-4.5.7).</description></item>
        /// <item><description>Lire l'identifiant d'application courante via <see cref="ISE_App.AppId"/>.</description></item>
        /// <item><description>Déléguer la vérification d'existence au Query Handler.</description></item>
        /// <item><description>Écrire inconditionnellement le résultat booléen sur <see cref="ISE_App.HasUnreadMessages"/>.</description></item>
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
                // Lecture de l'identifiant d'application courante via le Singleton applicatif.
                // ISE_App étant un Singleton de configuration, AppId est considéré comme
                // toujours valide ; la précondition appId > 0 est portée par le Query
                // Handler en aval (code BU_ER_02), captée le cas échéant par le catch
                // typé Ex_Business du présent UseCase.
                int appId = _settingsApp.AppId;

                // Délégation au Query Handler - chaîne (4) de R-4.14.19
                // (UC → QH → CR → DbContext). Aucune ouverture de transaction côté UC
                // (non transactionnel par construction) : le QH lit en lecture seule
                // via son socle générique.
                bool hasUnread =
                    await _qhUserAppMessage.HandleGetAnyMessageNotReadAsync(callChain, appId, ct);

                // Écriture inconditionnelle sur le Singleton applicatif.
                // Le setter de SE_App protège l'émission INPC par un test d'égalité
                // interne (SetField) ; aucune comparaison préalable n'est requise ici.
                _settingsApp.HasUnreadMessages = hasUnread;
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