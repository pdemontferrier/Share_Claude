using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Settings.App;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase orchestrateur du test unitaire de connectivité à la base
    /// de données partagée et de la propagation de l'état observé sur l'indicateur
    /// applicatif de disponibilité de la base.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : interface définie dans <c>A_Domain</c> conformément à la
    /// première obligation contractuelle de §4.14.2 amendée. Elle est consommée
    /// par injection de dépendances par un ViewModel de bandeau opérant en boucle
    /// de surveillance (typiquement <c>VM_BA_MainWindow</c>) en chaîne (1) directe
    /// au sens de §4.14.9, sans sous-séquence amont prévue. Son implémentation
    /// concrète <see cref="DG244Cutting.B_UseCases.UseCases.App.UC_DigitTryDb_TestConnection"/>
    /// réside en <c>B_UseCases/UseCases/App/</c>. Le contrat appartient au domaine
    /// <c>App</c> (mécanique transverse de surveillance applicative).</para>
    ///
    /// <para>Objectif : exposer, sous une forme abstraite et stable, l'opération
    /// orchestrée de diagnostic de connectivité à la base SQL, suivie de la
    /// propagation de l'état observé sur l'indicateur applicatif
    /// <see cref="ISE_App.IsConnected"/> via les opérations atomiques
    /// <see cref="ISE_App.NotifyConnectionLost"/> et
    /// <see cref="ISE_App.NotifyConnectionRestored"/>. Le résultat du diagnostic
    /// ne transite pas par valeur de retour : il transite exclusivement par
    /// propagation sur le Singleton applicatif observable, qui déclenche les
    /// effets visuels et fonctionnels prévus côté Présentation (iconographie de
    /// bandeau, modale de déconnexion, suspension / reprise des tâches temps
    /// réel) par l'émission des événements <see cref="ISE_App.ConnectionLost"/>
    /// et <see cref="ISE_App.ConnectionRestored"/>. Toute défaillance applicative
    /// est traitée terminalement par <c>IU_LogAndNotify</c> conformément à §4.7
    /// et n'est jamais propagée à l'appelant.</para>
    ///
    /// <para>Note sur la convention de méthode publique : Ce contrat expose une
    /// méthode publique unique <see cref="ExecuteAsync"/>, conformément à la
    /// configuration nominale du cas Concept au sens de la convention de nommage
    /// UC_ dual-cas Entité / Concept (R-4.14.7 amendée). Le segment
    /// <c>DigitTryDb</c> est un nom propre de base de données et non une entité
    /// du modèle de domaine (aucune entité EF Core homonyme dans
    /// <c>A_Domain/Entities</c>) ; le segment <c>TestConnection</c> est une
    /// action facultative qualifiant le concept porté, en miroir nominal du
    /// Service Infrastructure consommé en aval
    /// <c>IS_DigitTryDb_TestConnection</c>. Le préfixe canonique
    /// <c>ExecuteAsync</c> est conservé sans dérogation : le cas Concept à
    /// méthode publique unique relève de la configuration nominale, la double
    /// dérogation typologiquement bornée de R-4.2.13 ne s'appliquant qu'au
    /// sous-cas Concept à pluralité de méthodes publiques. Aucune trace de
    /// dérogation n'est par conséquent à porter dans le <c>&lt;remarks&gt;</c>
    /// de la méthode publique.</para>
    ///
    /// <para>Note sur la transactionnalité : Le scénario orchestré est non
    /// transactionnel par construction (invariant 6 de §2.1 du 0230, R-4.10.1).
    /// Il se limite à la délégation d'un test de connectivité au Service
    /// Infrastructure <c>IS_DigitTryDb_TestConnection</c> (test non mutant porté
    /// par la primitive EF Core <c>Database.CanConnectAsync</c> sur un DbContext
    /// de courte durée disposé par le Service lui-même au titre du Pattern 3 du
    /// câblage triple, §4.8.5 du 0230) suivie de la propagation d'un état
    /// observable sur le Singleton applicatif (mutation en mémoire). Aucune
    /// sémantique transactionnelle (commit / rollback) n'est exposée par le
    /// contrat ; le retour est de type <see cref="Task"/> simple, le UseCase
    /// n'étant pas consommé en sous-séquence par un orchestrateur amont
    /// (invariant 13 de §2.1 et R-4.14.21 non applicables, dimension iv-1 de
    /// la pré-qualification du fil de création = NON).</para>
    ///
    /// <para>Note sur la CallChain : Le paramètre <c>caller</c> reçu en première
    /// position obligatoire conformément à la convention de signature canonique
    /// (R-4.5.7) est enrichi à l'entrée de la méthode publique selon le patron
    /// normatif <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c>
    /// (R-4.5.1) et propagé en aval vers <c>IS_DigitTryDb_TestConnection</c>
    /// dans le <c>try</c> et vers <c>IU_LogAndNotify</c> dans chacun des trois
    /// catch typés.</para>
    ///
    /// <para>Note sur la cadence d'invocation : Le contrat n'impose aucune
    /// cadence. La périodicité du test, ses conditions de démarrage et d'arrêt,
    /// la suspension et la reprise de la boucle de surveillance relèvent
    /// strictement du consommateur amont (typiquement <c>VM_BA_MainWindow</c>
    /// par observation des événements <see cref="ISE_App.ConnectionLost"/> et
    /// <see cref="ISE_App.ConnectionRestored"/>).</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Déclarer le point d'entrée du scénario de diagnostic
    ///   de connectivité à la base partagée.</description></item>
    ///   <item><description>Imposer la propagation de la <c>CallChain</c> via le
    ///   paramètre <c>caller</c> contractuel.</description></item>
    ///   <item><description>Imposer le support de l'annulation coopérative via
    ///   un <c>CancellationToken</c>.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte pas la primitive de test de connectivité :
    ///   celle-ci est déléguée au Service Infrastructure
    ///   <c>IS_DigitTryDb_TestConnection</c>.</description></item>
    ///   <item><description>Ne porte pas la cadence d'invocation : celle-ci
    ///   relève du consommateur amont (typiquement
    ///   <c>VM_BA_MainWindow</c>).</description></item>
    ///   <item><description>N'expose aucune sémantique transactionnelle : le
    ///   scénario ne comporte aucune mutation EF Core.</description></item>
    ///   <item><description>N'expose aucun type technique de persistance
    ///   (EF Core, <c>DbContext</c>, <c>IQueryable</c>), conformément à la
    ///   pureté contractuelle de <c>A_Domain</c>.</description></item>
    ///   <item><description>Ne retourne pas le résultat du diagnostic par
    ///   valeur : la propagation se fait exclusivement par les opérations
    ///   atomiques <see cref="ISE_App.NotifyConnectionLost"/> et
    ///   <see cref="ISE_App.NotifyConnectionRestored"/> sur le Singleton
    ///   applicatif.</description></item>
    /// </list>
    /// <seealso cref="DG244Cutting.B_UseCases.UseCases.App.UC_DigitTryDb_TestConnection"/>
    /// </remarks>
    public interface IU_DigitTryDb_TestConnection
    {
        /// <summary>
        /// Exécute un test unitaire de connectivité à la base de données
        /// partagée et propage l'état observé sur l'indicateur applicatif
        /// <see cref="ISE_App.IsConnected"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : appelée depuis un ViewModel de bandeau opérant en
        /// boucle de surveillance (typiquement <c>VM_BA_MainWindow</c>), à
        /// chaque tick de la boucle. La cadence d'invocation, les conditions
        /// de démarrage et d'arrêt de la surveillance sont hors périmètre du
        /// présent contrat : elles relèvent strictement du consommateur amont.
        /// À chaque invocation, le UseCase délègue le test au Service
        /// Infrastructure <c>IS_DigitTryDb_TestConnection</c>, puis propage
        /// l'état observé sur le Singleton applicatif <see cref="ISE_App"/>
        /// via les opérations atomiques
        /// <see cref="ISE_App.NotifyConnectionLost"/> ou
        /// <see cref="ISE_App.NotifyConnectionRestored"/>. La propagation est
        /// inconditionnelle côté UseCase : le setter de
        /// <see cref="ISE_App.IsConnected"/> protège l'émission INPC et celle
        /// des événements <see cref="ISE_App.ConnectionLost"/> et
        /// <see cref="ISE_App.ConnectionRestored"/> par un test d'égalité
        /// interne, rendant inutile toute comparaison préalable côté UseCase
        /// et garantissant l'idempotence observable côté consommateur (les
        /// appels successifs avec le même état n'émettent ni cascade INPC ni
        /// événement supplémentaire).</para>
        ///
        /// <para>Comportement en cas d'erreur : Les trois familles d'exceptions
        /// applicatives (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) sont
        /// captées terminalement par le UseCase lui-même via le patron de
        /// trois catch typés et déléguées au pipeline terminal
        /// <c>IU_LogAndNotify</c> avec une clé dictionnaire dédiée
        /// (<c>No_EC_01</c>, <c>No_EC_02</c>, <c>No_EC_03</c>). Aucune de ces
        /// trois exceptions n'est propagée à l'appelant. Seul
        /// <see cref="OperationCanceledException"/> est propagé conformément à
        /// la doctrine d'annulation coopérative §4.6.</para>
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
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) ne
        /// sont jamais propagées : elles sont captées dans les trois blocs
        /// <c>catch</c> typés du UseCase et traitées terminalement par
        /// <c>IU_LogAndNotify</c>.
        /// </exception>
        Task ExecuteAsync(string caller, CancellationToken ct = default);
    }
}