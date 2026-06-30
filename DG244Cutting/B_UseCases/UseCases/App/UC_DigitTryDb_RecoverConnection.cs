using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// UseCase orchestrateur de la procédure de récupération de connexion à la
    /// base de données partagée à la suite d'une perte de connexion observée
    /// par le mécanisme de surveillance applicative.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et
    /// réside en <c>B_UseCases/UseCases/App/</c>. Il est résolu par injection
    /// de dépendances et constitue le maillon orchestrant la procédure de
    /// récupération de connectivité à la base SQL partagée, consommé en chaîne
    /// (1) directe par un ViewModel de bandeau (typiquement
    /// <c>VM_BA_MainWindow</c>) sur réception de l'événement applicatif
    /// <see cref="ISE_App.ConnectionLost"/>, sans sous-séquence amont prévue
    /// (signature <see cref="Task"/> simple, sans retour signalable — dimension
    /// iv-1 de la pré-qualification = NON, invariant 13 de §2.1 et R-4.14.21
    /// non applicables sur cette dimension).
    /// </para>
    /// <para>
    /// Configuration typologique : cas Concept à méthode publique unique
    /// (segment <c>Database</c> nom propre de concept applicatif non rattaché
    /// à une entité du modèle de domaine, segment <c>RecoverConnection</c>
    /// action facultative qualifiant le concept porté ; R-4.14.7 amendée),
    /// domaine <c>App</c> (mécanique transverse de récupération de
    /// connectivité applicative), non transactionnel par construction (le
    /// scénario se limite à l'observation d'un Singleton applicatif, au
    /// pilotage d'un Service de présentation, à des attentes coopératives et
    /// à la délégation en sous-séquence à un autre UseCase, sans aucune
    /// mutation EF Core ; R-4.10.1 ; item UC14 de la checklist §4.3 sans
    /// objet). Combinaison structurelle alignée sur les étalons
    /// <c>UC_DigitTryDb_TestConnection</c>, <c>UC_LogAndNotify</c> et
    /// <c>UC_Navigation</c> du domaine App sur les trois premières dimensions
    /// de pré-qualification, et mobilisant la doctrine R-4.14.21 sur la
    /// dimension iv-2 (consommation d'<c>IU_CloseApplication</c> en
    /// sous-séquence).
    /// </para>
    /// <para>
    /// Objectif : garantir qu'à chaque invocation déclenchée par un événement
    /// <see cref="ISE_App.ConnectionLost"/>, la procédure de récupération de
    /// connexion soit conduite à terme : (i) ouverture d'une fenêtre de
    /// dialogue d'attente informant l'opérateur et empêchant toute interaction
    /// avec la fenêtre principale pour la durée de la procédure ; (ii)
    /// exécution séquentielle de deux cycles d'observation de
    /// <see cref="ISE_App.IsConnected"/> aux cadences applicatives, conditionnés
    /// par une garde de stabilité ; (iii) en cas de récupération réussie,
    /// fermeture de la fenêtre de dialogue et restitution de la main à
    /// l'opérateur ; (iv) en cas d'échec des deux cycles, fermeture de la
    /// fenêtre de dialogue puis délégation à <see cref="IU_CloseApplication"/>
    /// en mode delay (<c>confirmation = false</c>, <c>warning = false</c>,
    /// <c>delaySeconds = <see cref="ShutdownDelaySeconds"/></c>) pour
    /// fermeture applicative informée. Toute défaillance applicative est
    /// traitée terminalement sans propagation à l'appelant.
    /// </para>
    /// <para>
    /// Doctrine R-4.14.21 (chaîne UC → UC normalisée) : Le UseCase consomme en
    /// sous-séquence <see cref="IU_CloseApplication"/> dans la branche d'échec
    /// des deux cycles d'observation. Les trois conditions doctrinales
    /// conjointes sont satisfaites : (C1) retour signalable
    /// <c>Task&lt;En_CloseResult&gt;</c> exploité par valeur sans interception
    /// applicative typée — l'exploitation prend la forme nominale d'un
    /// <c>discard</c> (<c>_ = await</c>), arbitrage Q5.4 du fil de création
    /// option A, le pipeline terminal de <c>UC_CloseApplication</c> assurant
    /// déjà la journalisation des cas <see cref="En_CloseResult.Cancelled"/>
    /// par son propre <c>IU_LogAndNotify</c> ; (C2) indépendance
    /// transactionnelle (I-4.10.3) — les deux UseCases sont non transactionnels
    /// par construction, aucune imbrication possible ; (C3) traitement
    /// terminal propre — <c>UC_CloseApplication</c> absorbe ses propres
    /// exceptions applicatives typées dans son pipeline et les convertit en
    /// <see cref="En_CloseResult.Cancelled"/>. L'annulation coopérative
    /// (<see cref="OperationCanceledException"/>) reste propagée selon §4.6
    /// du référentiel, distincte de la signalisation d'échec applicatif.
    /// </para>
    /// <para>
    /// Structure du cycle d'observation (arbitrage Q5.3 du fil de création
    /// option B) : Pattern simplifié sans phase passive — observation active
    /// immédiate sur toute la durée du cycle, ticks rythmés par
    /// <see cref="ISE_App.DatabaseCheckInterval"/> secondes, garde de
    /// stabilité de <see cref="RequiredStableChecks"/> observations
    /// consécutives confirmant la disponibilité de la connexion (arbitrage
    /// Q5.2 option A). Durée totale du cycle = <see cref="ISE_App.DatabaseCheckFirstLoop"/>
    /// secondes pour le cycle 1, <see cref="ISE_App.DatabaseCheckSecondLoop"/>
    /// secondes pour le cycle 2. Cette structure simplifiée s'écarte du
    /// pattern legacy à deux phases (<c>HandleDisconnectionAsync</c> de
    /// <c>BatchStockRelease/UC_DB_Monitoring</c>) qui combinait phase passive
    /// + phase active de même durée ; la simplification est documentée et
    /// assumée comme évolution doctrinale dans le présent fil.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire la CallChain au format normatif et la propager aux composants consommés en aval (R-4.5.5, R-4.5.7).</description></item>
    /// <item><description>Piloter l'ouverture et la fermeture de la fenêtre de dialogue d'attente via <see cref="IS_Notification.OpenDialogWindow"/> et <see cref="IS_Notification.CloseDialogWindow"/>.</description></item>
    /// <item><description>Orchestrer deux cycles d'observation séquentiels de <see cref="ISE_App.IsConnected"/> avec garde de stabilité.</description></item>
    /// <item><description>En cas d'échec des deux cycles, déléguer la fermeture applicative à <see cref="IU_CloseApplication"/> en mode delay avec exploitation par valeur du retour signalable (R-4.14.21).</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/> dans les trois <c>catch</c> typés (R-4.7.14, I-4.7.6).</description></item>
    /// <item><description>Propager <see cref="OperationCanceledException"/> à l'appelant sans journalisation ni notification (§4.6, R-4.6.13).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune précondition structurelle validée par levée d'<see cref="Ex_Business"/> interne : la méthode publique ne reçoit que la CallChain amont et le jeton d'annulation ; l'item UC9 de la checklist §4.3 est sans objet sur le présent composant.</description></item>
    /// <item><description>N'ouvre aucune transaction et ne réalise aucune mutation EF Core : scénario non transactionnel par construction (I-4.10.1 trivialement respectée) ; l'item UC14 de la checklist §4.3 est sans objet sur le présent composant.</description></item>
    /// <item><description>Ne porte pas la primitive de test de connectivité : celle-ci est portée par <c>UC_DigitTryDb_TestConnection</c> en boucle de surveillance indépendante. Le présent UseCase observe l'indicateur applicatif <see cref="ISE_App.IsConnected"/> qui en reflète l'état.</description></item>
    /// <item><description>Ne porte pas la garde d'idempotence empêchant le lancement concurrent du UseCase : celle-ci relève strictement du consommateur amont (typiquement <c>VM_BA_MainWindow</c>).</description></item>
    /// <item><description>Ne porte pas le geste WPF terminal de fermeture applicative : celui-ci est délégué à <see cref="IU_CloseApplication"/> en mode delay.</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler (I-4.14.4 amendée), un Query Handler ni un Repository.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_DigitTryDb_RecoverConnection"/>
    /// <seealso cref="IU_CloseApplication"/>
    public class UC_DigitTryDb_RecoverConnection : IU_DigitTryDb_RecoverConnection
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        /// <summary>
        /// Nombre d'observations consécutives confirmant la disponibilité de
        /// la connexion requis pour conclure à la réussite d'un cycle (garde
        /// de stabilité, arbitrage Q5.2 du fil de création option A).
        /// </summary>
        /// <remarks>
        /// <para>Cette garde évite qu'une remontée fugace de
        /// <see cref="ISE_App.IsConnected"/> (par exemple une oscillation
        /// transitoire pendant une coupure réseau intermittente) ne soit
        /// interprétée comme une récupération effective de la
        /// connexion.</para>
        /// </remarks>
        private const int RequiredStableChecks = 2;

        /// <summary>
        /// Délai (en secondes) passé à <see cref="IU_CloseApplication"/> en
        /// mode delay dans la branche d'échec des deux cycles d'observation
        /// (arbitrage Q5.1 du fil de création option A).
        /// </summary>
        /// <remarks>
        /// <para>Cette durée correspond au délai pendant lequel la fenêtre de
        /// dialogue d'<see cref="IU_CloseApplication"/> mode delay informe
        /// l'opérateur de l'imminence de la fermeture applicative, avant
        /// fermeture effective. La valeur est suffisante pour permettre à
        /// l'opérateur de prendre connaissance de la situation et de
        /// sauvegarder le cas échéant des éléments contextuels externes à
        /// l'application avant fermeture.</para>
        /// <para>Une remarque écosystème est consignée en clôture du fil de
        /// création pour signaler la pertinence d'une éventuelle introduction
        /// future d'une propriété dédiée à <see cref="ISE_App"/>
        /// (typiquement <c>DatabaseRecoverShutdownDelay</c>), dans un fil
        /// distinct portant sur la famille SE/RS.</para>
        /// </remarks>
        private const int ShutdownDelaySeconds = 300;

        /// <summary>
        /// Clé dictionnaire du titre de la fenêtre de dialogue d'attente
        /// affichée pendant la tentative de récupération de connexion.
        /// </summary>
        /// <remarks>
        /// <para>Clé reprise du legacy <c>BatchStockRelease/UC_DB_Monitoring</c>
        /// et confirmée présente au dictionnaire actif de DG244Cutting
        /// (RE_Language.fr.xaml et autres). Sémantique : « Avertissement ! ».</para>
        /// </remarks>
        private const string DictKey_DialogTitle = "No_Ti_05";

        /// <summary>
        /// Clé dictionnaire du contenu de la fenêtre de dialogue d'attente
        /// affichée pendant la tentative de récupération de connexion.
        /// </summary>
        /// <remarks>
        /// <para>Clé reprise du legacy <c>BatchStockRelease/UC_DB_Monitoring</c>
        /// et confirmée présente au dictionnaire actif de DG244Cutting. La
        /// traduction française porte le message attendu (« L'application n'a
        /// pas accès à la base de données ! Veuillez patienter pendant la
        /// tentative de reconnexion. ») ; les traductions en/de/it/es portent
        /// des placeholders à corriger — discordance signalée en clôture du
        /// fil de création en rubrique « Remarques sur l'écosystème
        /// documentaire » pour traitement dans un fil de maintenance
        /// dédié au dictionnaire.</para>
        /// </remarks>
        private const string DictKey_DialogContent = "No_Wa_15";

        #endregion

        #region === Dépendances privées ===

        private readonly IS_Notification _notification;
        private readonly ISE_App _settingsApp;
        private readonly IU_CloseApplication _closeApplication;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de
        /// <see cref="UC_DigitTryDb_RecoverConnection"/> avec ses dépendances.
        /// </summary>
        /// <param name="notification">
        /// Service technique transverse de présentation, support du pilotage
        /// de la fenêtre de dialogue d'attente via les opérations
        /// <see cref="IS_Notification.OpenDialogWindow"/> et
        /// <see cref="IS_Notification.CloseDialogWindow"/>.
        /// </param>
        /// <param name="settingsApp">
        /// Singleton applicatif fournissant l'indicateur observable
        /// <see cref="ISE_App.IsConnected"/> et les délais de surveillance
        /// <see cref="ISE_App.DatabaseCheckInterval"/>,
        /// <see cref="ISE_App.DatabaseCheckFirstLoop"/>,
        /// <see cref="ISE_App.DatabaseCheckSecondLoop"/>.
        /// </param>
        /// <param name="closeApplication">
        /// UseCase consommé en sous-séquence dans la branche d'échec des deux
        /// cycles d'observation, invoqué en mode delay (chaîne UC → UC
        /// normalisée, R-4.14.21).
        /// </param>
        /// <param name="logAndNotify">
        /// Pipeline terminal de journalisation et de notification des
        /// erreurs.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'un des paramètres est <see langword="null"/>.
        /// </exception>
        public UC_DigitTryDb_RecoverConnection(
            IS_Notification notification,
            ISE_App settingsApp,
            IU_CloseApplication closeApplication,
            IU_LogAndNotify logAndNotify)
        {
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
            _settingsApp = settingsApp ?? throw new ArgumentNullException(nameof(settingsApp));
            _closeApplication = closeApplication ?? throw new ArgumentNullException(nameof(closeApplication));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Exécute la procédure de récupération de connexion à la base de
        /// données partagée à la suite d'une perte de connexion observée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : maillon orchestrateur d'une chaîne (1) directe consommée
        /// par un ViewModel de bandeau sur événement
        /// <see cref="ISE_App.ConnectionLost"/>. Le scénario se limite à
        /// l'observation d'un Singleton applicatif, au pilotage d'un Service
        /// de présentation, à des attentes coopératives et à la délégation
        /// en sous-séquence à <see cref="IU_CloseApplication"/> ; aucune
        /// transaction EF Core n'est ouverte, aucune mutation persistante
        /// n'est réalisée. En conséquence, le présent <c>ExecuteAsync</c>
        /// n'encapsule aucun bloc transactionnel dans
        /// <c>CreateExecutionStrategy().ExecuteAsync</c> et n'invoque ni
        /// <c>BeginTransactionAsync</c>, ni <c>SaveChangesAsync</c>, ni
        /// <c>CommitAsync</c>, ni <c>RollbackAsync</c>. Le patron terminal
        /// des catch typés reste néanmoins entièrement appliqué : trois
        /// catch typés (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>)
        /// déléguant à <see cref="IU_LogAndNotify"/> avec les clés
        /// <c>No_EC_01</c> / <c>No_EC_02</c> / <c>No_EC_03</c>, et un catch
        /// distinct sur <see cref="OperationCanceledException"/> propageant
        /// à l'appelant conformément à §4.6.
        /// </para>
        /// <para>
        /// Chemin nominal du <c>try</c> : (i) ouverture de la fenêtre de
        /// dialogue d'attente via
        /// <see cref="IS_Notification.OpenDialogWindow"/> avec les clés
        /// <see cref="DictKey_DialogTitle"/> et
        /// <see cref="DictKey_DialogContent"/> ; (ii) exécution du cycle 1
        /// rapide de durée <see cref="ISE_App.DatabaseCheckFirstLoop"/>
        /// secondes ; si réussi (<see cref="RequiredStableChecks"/>
        /// observations consécutives confirmant
        /// <see cref="ISE_App.IsConnected"/>), fermeture de la fenêtre de
        /// dialogue et retour à l'appelant ; (iii) sinon exécution du cycle 2
        /// lent de durée <see cref="ISE_App.DatabaseCheckSecondLoop"/>
        /// secondes ; si réussi, fermeture de la fenêtre de dialogue et
        /// retour à l'appelant ; (iv) sinon fermeture de la fenêtre de
        /// dialogue puis délégation à
        /// <see cref="IU_CloseApplication.ExecuteAsync"/> en mode delay
        /// (<c>confirmation = false</c>, <c>warning = false</c>,
        /// <c>delaySeconds = <see cref="ShutdownDelaySeconds"/></c>) avec
        /// exploitation par valeur du retour signalable
        /// <c>Task&lt;En_CloseResult&gt;</c> sous forme de discard
        /// (arbitrage Q5.4 option A — le pipeline terminal de
        /// <c>UC_CloseApplication</c> assure déjà la journalisation des cas
        /// <see cref="En_CloseResult.Cancelled"/> par son propre
        /// <see cref="IU_LogAndNotify"/>).
        /// </para>
        /// <para>
        /// Aucune précondition structurelle n'est validée à l'intérieur du
        /// <c>try</c> : la méthode publique ne reçoit que la CallChain amont
        /// (enrichie en première instruction effective) et le jeton
        /// d'annulation coopérative ; aucun input métier susceptible de
        /// violer un invariant structurel n'est consommé. En conséquence, le
        /// présent <c>ExecuteAsync</c> ne lève aucune <see cref="Ex_Business"/>
        /// interne ; le bloc <c>catch (Ex_Business)</c> est conservé par
        /// uniformité doctrinale (R-4.7.14) au titre de la défense en
        /// profondeur sans source identifiable en amont.
        /// </para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative. Par défaut <see langword="default"/>.
        /// </param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est
        /// demandée, conformément à §4.6. Les exceptions applicatives typées
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles
        /// sont captées terminalement par les trois <c>catch</c> typés et
        /// traitées par <see cref="IU_LogAndNotify"/>.
        /// </exception>
        public async Task ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // (i) Ouverture de la fenêtre de dialogue d'attente.
                // IS_Notification.OpenDialogWindow est synchrone ; la mention
                // « non bloquante » du contrat qualifie le thread (pas de
                // ShowDialog bloquant du Dispatcher WPF), non l'UX : pendant
                // l'affichage, la fenêtre principale est désactivée
                // (MainWindow.IsEnabled = false), empêchant toute interaction
                // de l'opérateur pour la durée de la procédure.
                _notification.OpenDialogWindow(callChain, DictKey_DialogTitle, DictKey_DialogContent, ct);

                // (ii) Cycle 1 rapide - durée DatabaseCheckFirstLoop secondes.
                bool cycle1Ok = await RunObservationCycleAsync(
                    callChain,
                    _settingsApp.DatabaseCheckFirstLoop,
                    ct);

                if (cycle1Ok)
                {
                    // (iii) Récupération réussie pendant le cycle 1 -
                    // fermeture de la fenêtre de dialogue et retour à
                    // l'opérateur.
                    _notification.CloseDialogWindow(callChain, ct);
                    return;
                }

                // (ii) Cycle 2 lent - durée DatabaseCheckSecondLoop secondes.
                bool cycle2Ok = await RunObservationCycleAsync(
                    callChain,
                    _settingsApp.DatabaseCheckSecondLoop,
                    ct);

                if (cycle2Ok)
                {
                    // (iii) Récupération réussie pendant le cycle 2 -
                    // fermeture de la fenêtre de dialogue et retour à
                    // l'opérateur.
                    _notification.CloseDialogWindow(callChain, ct);
                    return;
                }

                // (iv) Échec des deux cycles - fermeture de la fenêtre
                // d'attente avant délégation à IU_CloseApplication mode delay
                // (qui ouvrira sa propre fenêtre de dialogue d'imminence de
                // fermeture).
                _notification.CloseDialogWindow(callChain, ct);

                // Délégation à IU_CloseApplication en mode delay
                // (confirmation: false, warning: false, delaySeconds > 0).
                // R-4.14.21 (chaîne UC → UC normalisée) : retour signalable
                // Task<En_CloseResult> exploité par valeur sans interception
                // applicative typée. Discard nominal (arbitrage Q5.4 option A
                // du fil de création) : le pipeline terminal de
                // UC_CloseApplication absorbe ses propres Ex_Business /
                // Ex_Infrastructure / Ex_Unclassified et les convertit en
                // En_CloseResult.Cancelled, qu'il a déjà journalisé via son
                // propre IU_LogAndNotify ; aucune journalisation
                // supplémentaire n'est requise au niveau présent.
                // OperationCanceledException reste propagée selon §4.6,
                // distincte de la signalisation d'échec applicatif.
                _ = await _closeApplication.ExecuteAsync(
                    callChain,
                    confirmation: false,
                    warning: false,
                    delaySeconds: ShutdownDelaySeconds,
                    ct: ct);
            }
            catch (Ex_Business ex)
            {
                // Branche défensive : aucune précondition structurelle interne
                // au présent UseCase ne lève Ex_Business ; les contrats
                // consommés en aval (IS_Notification.OpenDialogWindow /
                // CloseDialogWindow) documentent BU_ER_01 sur précondition
                // structurelle violée (caller, titleKey, contentKey null /
                // empty / whitespace) - défense en profondeur conservée par
                // uniformité doctrinale R-4.7.14.
                // Pas de RollbackAsync : aucune transaction n'a été ouverte
                // (non transactionnel par construction).
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
            }
            catch (Ex_Infrastructure ex)
            {
                // Pas de RollbackAsync : aucune transaction n'a été ouverte
                // (non transactionnel par construction).
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
            }
            catch (Ex_Unclassified ex)
            {
                // Pas de RollbackAsync : aucune transaction n'a été ouverte
                // (non transactionnel par construction).
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
            }
            catch (OperationCanceledException)
            {
                // Annulation coopérative : propagation directe à l'appelant
                // conformément à §4.6 et R-4.6.13.
                // Pas de RollbackAsync ici - aucune transaction n'a été
                // ouverte.
                // Pas d'appel à IU_LogAndNotify - l'annulation n'est pas une
                // erreur.
                throw;
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Exécute un cycle d'observation de
        /// <see cref="ISE_App.IsConnected"/> sur la durée indiquée, avec
        /// garde de stabilité de <see cref="RequiredStableChecks"/>
        /// observations consécutives confirmant la disponibilité de la
        /// connexion.
        /// </summary>
        /// <remarks>
        /// <para>Pattern simplifié sans phase passive (arbitrage Q5.3 option
        /// B du fil de création) : observation active immédiate sur toute la
        /// durée du cycle, ticks rythmés par
        /// <see cref="ISE_App.DatabaseCheckInterval"/> secondes. La méthode
        /// retourne <see langword="true"/> dès que la garde de stabilité est
        /// satisfaite (<see cref="RequiredStableChecks"/> observations
        /// consécutives à <see langword="true"/>), <see langword="false"/> à
        /// l'expiration de la durée du cycle sans stabilité confirmée.</para>
        /// <para>Le compteur de stabilité est remis à zéro à toute
        /// observation à <see langword="false"/>, garantissant que seules
        /// les observations strictement consécutives sont
        /// comptabilisées.</para>
        /// <para><see cref="DateTime.UtcNow"/> est utilisé pour la mesure de
        /// durée du cycle, insensible aux ajustements d'horloge système
        /// (heure d'été) ; la précision est largement suffisante à l'échelle
        /// des secondes manipulées ici.</para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de la méthode publique, enrichie localement.
        /// </param>
        /// <param name="cycleDurationSeconds">
        /// Durée totale du cycle d'observation en secondes
        /// (<see cref="ISE_App.DatabaseCheckFirstLoop"/> pour le cycle 1,
        /// <see cref="ISE_App.DatabaseCheckSecondLoop"/> pour le cycle 2).
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>
        /// <see langword="true"/> si la garde de stabilité a été satisfaite
        /// pendant la durée du cycle, <see langword="false"/> sinon.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton d'annulation est déclenché
        /// pendant l'attente ou au tour de boucle suivant, conformément à
        /// §4.6.
        /// </exception>
        private async Task<bool> RunObservationCycleAsync(
            string caller,
            int cycleDurationSeconds,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {_callee} > {nameof(RunObservationCycleAsync)}";

            DateTime start = DateTime.UtcNow;
            int stableCount = 0;

            while ((DateTime.UtcNow - start).TotalSeconds < cycleDurationSeconds)
            {
                ct.ThrowIfCancellationRequested();

                if (_settingsApp.IsConnected)
                {
                    stableCount++;
                    if (stableCount >= RequiredStableChecks)
                    {
                        return true;
                    }
                }
                else
                {
                    stableCount = 0;
                }

                await Task.Delay(TimeSpan.FromSeconds(_settingsApp.DatabaseCheckInterval), ct);
            }

            return false;
        }

        #endregion
    }
}