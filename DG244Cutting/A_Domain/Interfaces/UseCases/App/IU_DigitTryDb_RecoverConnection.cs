using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase orchestrateur de la procédure de récupération de
    /// connexion à la base de données partagée à la suite d'une perte de
    /// connexion observée par le mécanisme de surveillance applicative.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : interface définie dans <c>A_Domain</c> conformément à la
    /// première obligation contractuelle de §4.14.2 amendée. Elle est consommée
    /// par injection de dépendances par un ViewModel de bandeau (typiquement
    /// <c>VM_BA_MainWindow</c>) en chaîne (1) directe au sens de §4.14.9, sur
    /// réception de l'événement applicatif <see cref="ISE_App.ConnectionLost"/>
    /// déclenché par <see cref="ISE_App.NotifyConnectionLost"/>, sans
    /// sous-séquence amont prévue. Son implémentation concrète
    /// <see cref="DG244Cutting.B_UseCases.UseCases.App.UC_DigitTryDb_RecoverConnection"/>
    /// réside en <c>B_UseCases/UseCases/App/</c>. Le contrat appartient au
    /// domaine <c>App</c> (mécanique transverse de récupération de connectivité
    /// applicative).</para>
    ///
    /// <para>Objectif : exposer, sous une forme abstraite et stable, l'opération
    /// orchestrée de récupération de connexion à la base partagée, articulée
    /// en trois phases : (i) ouverture d'une fenêtre de dialogue d'attente
    /// indiquant à l'opérateur que la tentative de reconnexion est en cours
    /// et empêchant toute interaction avec la fenêtre principale pour la
    /// durée de la procédure ; (ii) exécution séquentielle de deux cycles
    /// d'observation de <see cref="ISE_App.IsConnected"/> aux cadences
    /// applicatives — cycle 1 rapide
    /// (<see cref="ISE_App.DatabaseCheckFirstLoop"/>) puis cycle 2 lent
    /// (<see cref="ISE_App.DatabaseCheckSecondLoop"/>) — chacun conditionné
    /// par une garde de stabilité exigeant des observations consécutives
    /// confirmant la disponibilité de la connexion ; (iii) en cas de
    /// récupération réussie pendant l'un des deux cycles, fermeture de la
    /// fenêtre de dialogue et restitution de la main à l'opérateur ; en cas
    /// d'échec des deux cycles, délégation à
    /// <see cref="IU_CloseApplication"/> en mode delay pour informer
    /// l'opérateur de l'imminence de la fermeture applicative et procéder à
    /// celle-ci, la fenêtre de dialogue d'attente restant ouverte au moment
    /// de la délégation et ses libellés basculant en place par idempotence
    /// de <see cref="IS_Notification.OpenDialogWindow"/> à la seconde
    /// ouverture portée par <see cref="IU_CloseApplication"/> — la
    /// fermeture WPF unique et effective de la fenêtre est alors portée par
    /// le <see cref="IS_Notification.CloseDialogWindow"/> terminal de
    /// <see cref="IU_CloseApplication"/>.</para>
    ///
    /// <para>Note sur la convention de méthode publique : Ce contrat expose
    /// une méthode publique unique <see cref="ExecuteAsync"/>, conformément
    /// à la configuration nominale du cas Concept au sens de la convention
    /// de nommage UC_ dual-cas Entité / Concept (R-4.14.7 amendée). Le
    /// segment <c>DigitTryDb</c> est un nom propre de concept applicatif (la
    /// base de données partagée) non rattaché à une entité du modèle de
    /// domaine (aucune entité EF Core homonyme dans
    /// <c>A_Domain/Entities</c>) ; le segment <c>RecoverConnection</c> est
    /// une action facultative qualifiant le concept porté. Le préfixe
    /// canonique <c>ExecuteAsync</c> est conservé sans dérogation : le cas
    /// Concept à méthode publique unique relève de la configuration
    /// nominale, la dérogation typologiquement bornée de R-4.2.13 ne
    /// s'appliquant qu'au sous-cas Concept à pluralité de méthodes
    /// publiques. Aucune trace de dérogation n'est par conséquent à porter
    /// dans le <c>&lt;remarks&gt;</c> de la méthode publique.</para>
    ///
    /// <para>Note sur la transactionnalité : Le scénario orchestré est non
    /// transactionnel par construction (invariant 6 de §2.1 du 0230,
    /// R-4.10.1). Il se limite à l'observation d'un Singleton applicatif
    /// (<see cref="ISE_App.IsConnected"/> lu via le contrat), au pilotage
    /// d'un Service de présentation
    /// (<see cref="IS_Notification.OpenDialogWindow"/> et
    /// <see cref="IS_Notification.CloseDialogWindow"/>), à des attentes
    /// coopératives sur jeton d'annulation, et à la délégation en
    /// sous-séquence à <see cref="IU_CloseApplication"/>. Aucune sémantique
    /// transactionnelle (commit / rollback) n'est exposée par le contrat ;
    /// le retour est de type <see cref="Task"/> simple, le UseCase n'étant
    /// pas consommé en sous-séquence par un orchestrateur amont (invariant
    /// 13 de §2.1 et R-4.14.21 non applicables sur la dimension iv-1 de la
    /// pré-qualification du fil de création = NON).</para>
    ///
    /// <para>Note sur la consommation d'un UseCase en sous-séquence (chaîne
    /// UC → UC normalisée, R-4.14.21) : Le UseCase consomme en sous-séquence
    /// <see cref="IU_CloseApplication"/> en mode delay
    /// (<c>confirmation = false</c>, <c>warning = false</c>,
    /// <c>delaySeconds &gt; 0</c>) dans la branche d'échec des deux cycles
    /// d'observation. Les trois conditions doctrinales conjointes de
    /// R-4.14.21 sont satisfaites : (C1) <see cref="IU_CloseApplication"/>
    /// expose un retour signalable <c>Task&lt;En_CloseResult&gt;</c>
    /// exploité par valeur sans interception applicative typée ; (C2)
    /// indépendance transactionnelle (I-4.10.3) — les deux UseCases sont
    /// non transactionnels par construction, aucune imbrication possible ;
    /// (C3) traitement terminal propre — <see cref="IU_CloseApplication"/>
    /// absorbe ses propres exceptions applicatives typées dans son pipeline
    /// et les convertit en <see cref="En_CloseResult.Cancelled"/>.
    /// L'annulation coopérative (<see cref="OperationCanceledException"/>)
    /// reste propagée selon §4.6 du référentiel, distincte de la
    /// signalisation d'échec applicatif.</para>
    ///
    /// <para>Note sur la CallChain : Le paramètre <c>caller</c> reçu en
    /// première position obligatoire conformément à la convention de
    /// signature canonique (R-4.5.7) est enrichi à l'entrée de la méthode
    /// publique selon le patron normatif
    /// <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c> (R-4.5.1)
    /// et propagé en aval vers <see cref="IS_Notification"/> dans le
    /// <c>try</c>, vers <see cref="IU_CloseApplication"/> dans la branche
    /// d'échec, et vers <c>IU_LogAndNotify</c> dans chacun des trois
    /// <c>catch</c> typés.</para>
    ///
    /// <para>Note sur la garde d'idempotence : Le présent contrat n'impose
    /// pas de garde de non-réentrance. La protection contre le lancement
    /// concurrent du UseCase à la suite d'événements
    /// <see cref="ISE_App.ConnectionLost"/> rapprochés (par exemple lors
    /// d'une instabilité réseau intermittente) relève strictement du
    /// consommateur amont (typiquement <c>VM_BA_MainWindow</c>) qui porte
    /// la responsabilité de la sérialisation des invocations selon les
    /// modalités qui lui sont propres.</para>
    ///
    /// <para>Note sur le pilotage de la fenêtre de dialogue : Le terme
    /// « non bloquante » qualifiant la fenêtre de dialogue ouverte via
    /// <see cref="IS_Notification.OpenDialogWindow"/> qualifie le thread
    /// (la fenêtre n'engage pas un <c>ShowDialog</c> bloquant du Dispatcher
    /// WPF), non l'expérience utilisateur : pendant l'affichage, la
    /// fenêtre principale est désactivée (<c>MainWindow.IsEnabled = false</c>),
    /// empêchant l'opérateur d'interagir avec elle pour la durée de la
    /// procédure de récupération, conformément à l'objectif fonctionnel
    /// d'inhibition d'interaction pendant la tentative.</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Déclarer le point d'entrée du scénario de
    ///   récupération de connexion à la base partagée.</description></item>
    ///   <item><description>Imposer la propagation de la <c>CallChain</c>
    ///   via le paramètre <c>caller</c> contractuel.</description></item>
    ///   <item><description>Imposer le support de l'annulation coopérative
    ///   via un <see cref="CancellationToken"/>.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte pas la primitive de test de
    ///   connectivité : celle-ci est portée par
    ///   <c>UC_DigitTryDb_TestConnection</c> en boucle de surveillance
    ///   indépendante. Le présent UseCase observe l'indicateur applicatif
    ///   <see cref="ISE_App.IsConnected"/> qui en reflète
    ///   l'état.</description></item>
    ///   <item><description>Ne porte pas la cadence ni les délais de
    ///   surveillance : ceux-ci sont lus depuis <see cref="ISE_App"/>
    ///   (<see cref="ISE_App.DatabaseCheckInterval"/>,
    ///   <see cref="ISE_App.DatabaseCheckFirstLoop"/>,
    ///   <see cref="ISE_App.DatabaseCheckSecondLoop"/>).</description></item>
    ///   <item><description>Ne porte pas la garde d'idempotence empêchant
    ///   le lancement concurrent du UseCase : celle-ci relève strictement
    ///   du consommateur amont.</description></item>
    ///   <item><description>Ne porte pas le geste WPF terminal de fermeture
    ///   applicative : celui-ci est délégué à
    ///   <see cref="IU_CloseApplication"/> en mode delay, qui orchestre
    ///   lui-même l'ouverture, l'attente et la fermeture de sa propre
    ///   fenêtre de dialogue, la déconnexion de session et la fermeture
    ///   WPF effective via <c>IS_Shutdown</c>.</description></item>
    ///   <item><description>N'expose aucune sémantique transactionnelle :
    ///   le scénario ne comporte aucune mutation EF Core.</description></item>
    ///   <item><description>N'expose aucun type technique de persistance
    ///   (EF Core, <c>DbContext</c>, <c>IQueryable</c>) ni de présentation
    ///   (WPF), conformément à la pureté contractuelle de
    ///   <c>A_Domain</c>.</description></item>
    /// </list>
    /// <seealso cref="DG244Cutting.B_UseCases.UseCases.App.UC_DigitTryDb_RecoverConnection"/>
    /// <seealso cref="IU_CloseApplication"/>
    /// <seealso cref="ISE_App"/>
    /// <seealso cref="IS_Notification"/>
    /// </remarks>
    public interface IU_DigitTryDb_RecoverConnection
    {
        /// <summary>
        /// Exécute la procédure de récupération de connexion à la base de
        /// données partagée à la suite d'une perte de connexion observée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : appelée depuis un ViewModel de bandeau
        /// (typiquement <c>VM_BA_MainWindow</c>) sur réception de
        /// l'événement applicatif <see cref="ISE_App.ConnectionLost"/>. La
        /// protection contre le lancement concurrent à la suite d'événements
        /// rapprochés relève du consommateur amont (cf. <c>&lt;remarks&gt;</c>
        /// de classe « Note sur la garde d'idempotence »). À chaque
        /// invocation, le UseCase ouvre une fenêtre de dialogue d'attente,
        /// exécute deux cycles d'observation successifs de
        /// <see cref="ISE_App.IsConnected"/>, et oriente la suite selon
        /// l'issue : récupération réussie pendant l'un des cycles —
        /// fermeture de la fenêtre de dialogue et retour à l'opérateur ;
        /// échec des deux cycles — délégation à
        /// <see cref="IU_CloseApplication"/> en mode delay
        /// (<c>confirmation = false</c>, <c>warning = false</c>,
        /// <c>delaySeconds &gt; 0</c>) pour fermeture applicative informée,
        /// la fenêtre de dialogue d'attente restant ouverte au moment de la
        /// délégation et ses libellés basculant en place par idempotence de
        /// <see cref="IS_Notification.OpenDialogWindow"/> à la seconde
        /// ouverture portée par <see cref="IU_CloseApplication"/> ; la
        /// fermeture WPF de la fenêtre est portée par le
        /// <see cref="IS_Notification.CloseDialogWindow"/> terminal de
        /// <see cref="IU_CloseApplication"/>.</para>
        ///
        /// <para>Cadences d'observation : les délais sont lus depuis le
        /// Singleton applicatif. Le cycle 1 dispose de
        /// <see cref="ISE_App.DatabaseCheckFirstLoop"/> secondes pour
        /// observer une récupération stable ; le cycle 2 dispose de
        /// <see cref="ISE_App.DatabaseCheckSecondLoop"/> secondes. À
        /// l'intérieur de chaque cycle, l'observation est rythmée par
        /// <see cref="ISE_App.DatabaseCheckInterval"/> secondes entre
        /// chaque relevé de <see cref="ISE_App.IsConnected"/>. La conclusion
        /// de réussite d'un cycle exige des observations consécutives
        /// confirmant la disponibilité de la connexion (garde de stabilité),
        /// afin d'éviter qu'une remontée fugace ne soit interprétée comme
        /// une récupération effective.</para>
        ///
        /// <para>Comportement en cas d'erreur : Les trois familles
        /// d'exceptions applicatives (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>)
        /// sont captées terminalement par le UseCase lui-même via le patron
        /// de trois catch typés et déléguées au pipeline terminal
        /// <c>IU_LogAndNotify</c> avec une clé dictionnaire dédiée
        /// (<c>No_EC_01</c>, <c>No_EC_02</c>, <c>No_EC_03</c>). Aucune de
        /// ces trois exceptions n'est propagée à l'appelant. Seul
        /// <see cref="OperationCanceledException"/> est propagé conformément
        /// à la doctrine d'annulation coopérative §4.6 du référentiel.</para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le
        /// patron normatif
        /// <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c>
        /// (R-4.5.1). Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut
        /// <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est
        /// déclenché pendant l'opération, conformément à la doctrine
        /// d'annulation coopérative §4.6 du référentiel. Les exceptions
        /// applicatives typées (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>)
        /// ne sont jamais propagées : elles sont captées dans les trois
        /// blocs <c>catch</c> typés du UseCase et traitées terminalement
        /// par <c>IU_LogAndNotify</c>.
        /// </exception>
        Task ExecuteAsync(string caller, CancellationToken ct = default);
    }
}