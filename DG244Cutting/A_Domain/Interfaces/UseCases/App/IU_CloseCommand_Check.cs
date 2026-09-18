using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase de détermination, à chaque invocation, de la nécessité
    /// pour l'application courante de se fermer d'elle-même, et de déclenchement
    /// le cas échéant de sa fermeture temporisée par délégation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.2 amendée, 1ère obligation contractuelle). Elle est
    /// consommée par injection de dépendances par un ViewModel en chaîne (1) directe -
    /// en pratique <c>VM_Banner</c> orchestrant une boucle de polling via
    /// <c>IS_UseCaseInvoker</c> - qui en délègue l'exécution au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.App.UC_CloseCommand_Check"/>
    /// résidant en <c>B_UseCases/UseCases/App/</c>. Le contrat appartient au domaine
    /// <c>App</c> (orchestration applicative transverse, cycle de vie applicatif).
    /// </para>
    /// <para>
    /// Configuration typologique : ce UseCase relève du cas Concept à méthode publique
    /// unique - le segment <c>CloseCommand</c> désigne un concept fonctionnel (ordre de
    /// fermeture) et non une entité du domaine ; l'entité effectivement lue est
    /// <c>UserAppSessionCommand</c>, absente du nom du couple. La pluralité interne
    /// attendue est unique (une seule méthode publique <see cref="ExecuteAsync"/>).
    /// Parallèle doctrinal : <c>UC_Navigation</c>, <c>UC_LogAndNotify</c>. Le UseCase
    /// est non transactionnel par construction : le scénario orchestré se limite à une
    /// lecture pure déléguée (chaîne (4) de R-4.14.19 : UC → QH → CR → DbContext) suivie
    /// d'une délégation à un autre UseCase, sans aucune mutation EF Core ; en conséquence
    /// le contrat n'expose aucune sémantique transactionnelle (commit/rollback) et le
    /// retour est de type <see cref="Task"/> simple.
    /// </para>
    /// <para>
    /// Rôle dans le cycle de vie applicatif : ce UseCase est un jumeau structurel de la
    /// détection périodique déclenchée par la bannière ; il en diffère en ce qu'il ne met
    /// pas à jour un Setting mais déclenche une action applicative. À chaque invocation, il
    /// évalue deux conditions distinctes justifiant la fermeture : (a) l'existence d'une
    /// demande de déconnexion adressée au couple utilisateur courant / application courante,
    /// matérialisée par une ligne active dans la table <c>UserAppSessionCommand</c> ; (b) la
    /// perte d'accessibilité de l'application courante dans le référentiel <c>AppList</c>. Si
    /// au moins l'une des deux conditions est vérifiée, la fermeture temporisée est déléguée
    /// à <see cref="IU_CloseApplication"/> en mode delay ; aucune issue n'est remontée à
    /// l'appelant. La cadence d'invocation est hors périmètre du présent contrat : elle relève
    /// du consommateur amont (boucle de polling de <c>VM_Banner</c>).
    /// </para>
    /// <para>
    /// Chaîne UC → UC normalisée (R-4.14.21) : ce UseCase consomme
    /// <see cref="IU_CloseApplication"/> en sous-séquence. La chaîne est conforme :
    /// indépendance transactionnelle (I-4.10.3, UseCase non transactionnel) et traitement
    /// terminal propre via <c>IU_LogAndNotify</c> sont satisfaits ; le retour signalable
    /// <c>Task&lt;En_CloseResult&gt;</c> du UseCase consommé est exploitable mais
    /// volontairement non exploité par l'appelant terminal - la fermeture est la conséquence
    /// immédiate et unique de la détection, aucune issue n'est remontée. Le présent UseCase
    /// n'est pour sa part pas consommé en sous-séquence par un orchestrateur amont
    /// (invariant 13 de §2.1 et R-4.14.21 non applicables au titre du retour signalable).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario de détection de nécessité de fermeture applicative.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel (R-4.5.5).</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c> en queue (R-4.6.13).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la lecture en base : celle-ci est déléguée aux Query Handlers consommés en aval (existence d'une commande de fermeture, accessibilité applicative).</description></item>
    /// <item><description>Ne porte pas la procédure de fermeture : celle-ci est déléguée à <see cref="IU_CloseApplication"/> en mode delay.</description></item>
    /// <item><description>N'expose aucune sémantique transactionnelle : le scénario ne comporte aucune mutation EF Core.</description></item>
    /// <item><description>N'expose aucun type technique de persistance (EF Core, DbContext, IQueryable) ni de présentation, conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// <item><description>Ne pilote pas la cadence d'invocation : celle-ci relève du consommateur amont (boucle de polling de <c>VM_Banner</c>).</description></item>
    /// </list>
    /// </remarks>
    public interface IU_CloseCommand_Check
    {
        // --- Groupe 1 : Détection de nécessité de fermeture applicative ---

        /// <summary>
        /// Détermine, à chaque invocation, si l'application courante doit se fermer
        /// d'elle-même et, le cas échéant, déclenche sa fermeture temporisée par
        /// délégation, sans retour vers l'appelant.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelé depuis un ViewModel en chaîne (1) directe, typiquement dans
        /// le cadre d'une boucle de polling orchestrée par <c>VM_Banner</c> via
        /// <c>IS_UseCaseInvoker</c>. La cadence d'invocation est hors périmètre du présent
        /// contrat : elle relève du consommateur amont. À chaque invocation, le UseCase lit
        /// le contexte courant (<c>AppId</c>, <c>AppUserId</c>) puis évalue en court-circuit
        /// deux conditions de fermeture : l'existence d'une ligne active dans
        /// <c>UserAppSessionCommand</c> pour le couple courant, et la non-accessibilité de
        /// l'application dans <c>AppList</c>. Si au moins l'une est vérifiée, la fermeture
        /// temporisée est déléguée à <see cref="IU_CloseApplication"/> en mode delay ; sinon
        /// l'invocation se termine sans effet observable. Toute exception applicative typée
        /// remontée par les lectures est traitée terminalement par <c>IU_LogAndNotify</c> et
        /// n'est jamais propagée à l'appelant, afin que la boucle de polling survive à une
        /// itération en échec.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire la CallChain et la propager aux Query Handlers consommés en aval et au UseCase délégué.</description></item>
        /// <item><description>Lire le contexte applicatif et utilisateur courant (<c>AppId</c>, <c>AppUserId</c>).</description></item>
        /// <item><description>Évaluer en court-circuit les deux conditions de fermeture (commande de session active, non-accessibilité applicative).</description></item>
        /// <item><description>Déléguer la fermeture temporisée à <see cref="IU_CloseApplication"/> en mode delay lorsqu'au moins une condition est vérifiée.</description></item>
        /// <item><description>Déléguer le traitement terminal des erreurs à <c>IU_LogAndNotify</c>.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne valide aucune précondition structurelle : la précondition <c>id &gt; 0</c> est portée par les Query Handlers en aval (code <c>BU_ER_02</c>).</description></item>
        /// <item><description>N'ouvre aucune transaction et ne réalise aucune mutation EF Core : scénario non transactionnel par construction.</description></item>
        /// <item><description>N'exploite pas l'issue de la délégation : la fermeture est la conséquence immédiate et unique de la détection.</description></item>
        /// <item><description>Ne pilote pas la cadence d'invocation : celle-ci relève du consommateur amont.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format
        /// normatif de §4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée,
        /// conformément à §4.6. Les exceptions applicatives typées
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles sont captées
        /// dans les trois blocs <c>catch</c> typés du UseCase et traitées terminalement
        /// par <c>IU_LogAndNotify</c>.
        /// </exception>
        Task ExecuteAsync(string caller, CancellationToken ct = default);
    }
}