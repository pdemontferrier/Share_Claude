using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Settings.App;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase de vérification de la présence de messages applicatifs
    /// non lus pour l'application courante.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.2 amendée, 1ère obligation contractuelle). Elle est
    /// consommée par injection de dépendances par un ViewModel en chaîne (1) directe -
    /// typiquement un VM_BA_MainWindow orchestrant la boucle de polling de présence de
    /// messages non lus - qui en délègue l'exécution au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserAppMessage_CheckUnread"/>
    /// résidant en <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (messagerie applicative interne, entité <c>UserAppMessage</c>).
    /// </para>
    /// <para>
    /// Objectif : orchestrer la détermination de l'existence d'au moins un message
    /// applicatif non lu adressé à l'application courante (identifiée par
    /// <see cref="ISE_App.AppId"/>), suivie de la propagation inconditionnelle du
    /// résultat sur la propriété <see cref="ISE_App.HasUnreadMessages"/> du Singleton
    /// applicatif, sous un traitement terminal des erreurs conforme à §4.7.
    /// </para>
    /// <para>
    /// Configuration typologique atypique : ce UseCase relève du cas Entité
    /// (segment [Entité] = <c>UserAppMessage</c> + segment [Action] = <c>CheckUnread</c>
    /// obligatoire, R-4.14.7 amendée) mais est non transactionnel par construction -
    /// le scénario orchestré se limite à une lecture CQRS via le Query Handler
    /// (chaîne (4) de R-4.14.19 : UC → QH → CR → DbContext) suivie de l'écriture
    /// d'une propriété en mémoire sur un Singleton applicatif, sans aucune mutation
    /// EF Core. Combinaison inédite parmi les étalons disponibles, n'enfreignant
    /// cependant aucune règle normative explicite : la sous-classe est dictée par le
    /// nommage (R-4.14.7 amendée), la nature transactionnelle est dictée par la
    /// présence ou non d'une mutation EF Core (R-4.10.1) ; les deux axes sont
    /// orthogonaux. Conséquences contractuelles : aucune sémantique transactionnelle
    /// (commit/rollback) n'est exposée par le contrat ; le retour est de type
    /// <see cref="Task"/> simple (le UseCase n'est pas consommé en sous-séquence par
    /// un orchestrateur amont, invariant 13 de §2.1 et R-4.14.21 non applicables).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario de vérification de présence de messages non lus.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte pas la lecture en base : celle-ci est déléguée au Query Handler <c>IQ_UserAppMessage</c> via la méthode <c>HandleGetAnyMessageNotReadAsync</c>.</description></item>
    /// <item><description>Ne porte pas la résolution de l'identifiant d'application : celui-ci est lu en interne via <see cref="ISE_App.AppId"/>.</description></item>
    /// <item><description>N'expose aucune sémantique transactionnelle : le scénario ne comporte aucune mutation EF Core.</description></item>
    /// <item><description>N'expose aucun type technique de persistance (EF Core, DbContext, IQueryable), conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserAppMessage_CheckUnread
    {
        // --- Groupe 1 : Vérification de présence de messages non lus ---

        /// <summary>
        /// Orchestre la vérification de la présence d'au moins un message applicatif
        /// non lu adressé à l'application courante, puis propage le résultat sur la
        /// propriété <see cref="ISE_App.HasUnreadMessages"/> du Singleton applicatif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelé depuis un ViewModel en chaîne (1) directe, typiquement
        /// dans le cadre d'une boucle de polling orchestrée par VM_BA_MainWindow. La
        /// cadence d'invocation est hors périmètre du présent contrat : elle relève
        /// d'un consommateur amont. À chaque invocation, le UseCase délègue la lecture
        /// à <c>IQ_UserAppMessage.HandleGetAnyMessageNotReadAsync(caller, appId, ct)</c>
        /// pour l'application courante identifiée par <see cref="ISE_App.AppId"/>, puis
        /// écrit inconditionnellement le résultat booléen sur
        /// <see cref="ISE_App.HasUnreadMessages"/>. L'écriture est inconditionnelle :
        /// le setter du Singleton protège l'émission INPC par un test d'égalité interne,
        /// rendant inutile toute comparaison préalable dans le UseCase. Toute exception
        /// applicative typée remontée par le Query Handler est traitée terminalement
        /// par <c>IU_LogAndNotify</c> et n'est jamais propagée à l'appelant.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire la CallChain et la propager au Query Handler consommé en aval.</description></item>
        /// <item><description>Lire l'identifiant d'application courante via <see cref="ISE_App.AppId"/>.</description></item>
        /// <item><description>Déléguer la vérification d'existence au Query Handler <c>IQ_UserAppMessage</c>.</description></item>
        /// <item><description>Écrire inconditionnellement le résultat sur <see cref="ISE_App.HasUnreadMessages"/>.</description></item>
        /// <item><description>Déléguer le traitement terminal des erreurs à <c>IU_LogAndNotify</c>.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne valide pas la précondition structurelle sur <c>appId</c> : cette validation est portée par le Query Handler en aval (code <c>BU_ER_02</c>).</description></item>
        /// <item><description>N'ouvre aucune transaction et ne réalise aucune mutation EF Core : scénario non transactionnel par construction.</description></item>
        /// <item><description>Ne pilote pas la cadence d'invocation : celle-ci relève du consommateur amont (typiquement la boucle de polling de VM_BA_MainWindow).</description></item>
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