using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase de création d'un compte utilisateur <see cref="UserApp"/> depuis la
    /// page d'administration (chemin « création », bouton Ajouter de Page04).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.2 amendée, première obligation contractuelle). Elle est
    /// consommée par injection de dépendances par le ViewModel <c>VM_Page04</c> orchestrant le
    /// formulaire d'ajout d'un utilisateur (chaîne (1) directe VM → UC), qui en délègue
    /// l'exécution à l'implémentation concrète
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserApp_Create"/> résidant en
    /// <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine <c>User</c> (gestion
    /// du cycle de vie des comptes utilisateurs).
    /// </para>
    /// <para>
    /// Objectif : orchestrer le scénario unitaire de création d'un enregistrement
    /// <c>UserApp</c>. Le UseCase concentre la logique métier propre au chemin création qui ne
    /// relève ni du ViewModel (présentation) ni du Service (écriture pure) : dérivation des
    /// initiales, deux contrôles souples avec confirmation utilisateur, contrôle dur d'unicité
    /// du <c>Login</c> et hachage du mot de passe. Il ouvre la transaction sous stratégie
    /// d'exécution, délègue l'écriture au Service <c>IS_UserApp_Create</c>, persiste
    /// solidairement via <c>SaveChangesAsync</c>, valide la transaction, puis confie le
    /// traitement terminal des erreurs au pipeline <c>IU_LogAndNotify</c>. Il s'inscrit dans le
    /// patron d'écriture transactionnel de <c>UC_UserAppMessage_Add</c>.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario de création d'un compte utilisateur.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// <item><description>Exposer un retour signalable <see cref="En_ChangeResult"/> restituant à la présentation l'issue de mutation, au titre de R-4.14.22 (chaîne (1) directe).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne construit pas la copie de travail <c>UserApp</c> : ce rôle relève du ViewModel <c>VM_Page04</c>.</description></item>
    /// <item><description>N'écrit pas directement en base : l'écriture est déléguée au Service <c>IS_UserApp_Create</c> en aval.</description></item>
    /// <item><description>Ne re-valide pas les champs obligatoires autres que <c>FirstName</c>, <c>LastName</c>, <c>Login</c> et <c>WindowsLogin</c> : ils sont portés par le Service (<see cref="Ex_Business"/>, code <c>BU_ER_01</c>).</description></item>
    /// <item><description>Ne positionne pas les champs d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) : centralisés dans <c>CH_Generic&lt;T&gt;</c>.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserApp_Create
    {
        // --- Groupe 1 : Création d'un compte utilisateur ---

        /// <summary>
        /// Orchestre la création d'un nouvel enregistrement <c>UserApp</c> à partir de la copie
        /// de travail <paramref name="entity"/> et du mot de passe en clair
        /// <paramref name="plainPassword"/> fournis par le ViewModel de la page
        /// d'administration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté typiquement par <c>VM_Page04</c> à la validation du bouton
        /// Ajouter. Le scénario se déroule en deux temps.
        /// </para>
        /// <para>
        /// Bloc préparatoire (hors du délégué de la stratégie d'exécution, afin que les
        /// interactions UI ne soient pas rejouées en cas de réexécution) : contrôle bloquant de
        /// non-nullité et de non-vacuité de <c>FirstName</c> et de <c>LastName</c>, puis de
        /// <c>Login</c> et de <c>WindowsLogin</c> ; dérivation des <c>Initials</c> (initiale du
        /// prénom suivie de l'initiale du nom, en majuscules) ; deux contrôles souples avec
        /// confirmation utilisateur (composition attendue du <c>Login</c>, puis égalité
        /// <c>WindowsLogin</c> = <c>Login</c>).
        /// </para>
        /// <para>
        /// Bloc transactionnel (dans le délégué de la stratégie d'exécution) : contrôle dur
        /// d'unicité du <c>Login</c> en base, hachage du mot de passe, délégation de l'écriture
        /// au Service <c>IS_UserApp_Create</c>, persistance via <c>SaveChangesAsync</c> et
        /// validation de la transaction. Toute exception applicative typée est traitée
        /// terminalement par <c>IU_LogAndNotify</c> et n'est pas propagée à l'appelant.
        /// </para>
        /// <para>
        /// Canal « abandon utilisateur », distinct du canal d'exception métier
        /// <see cref="Ex_Business"/> : le scénario se termine par un retour anticipé
        /// (aucune écriture, aucune exception levée) dans quatre situations relevant d'une
        /// décision de l'utilisateur — <c>FirstName</c> ou <c>LastName</c> nul, vide ou composé
        /// uniquement d'espaces (notification bloquante <c>Stop</c>, clé <c>No_St_03</c>) ;
        /// <c>Login</c> ou <c>WindowsLogin</c> nul, vide ou composé uniquement d'espaces
        /// (notification bloquante <c>Stop</c>, clé <c>No_St_02</c>) ; refus du premier contrôle
        /// souple sur la composition du <c>Login</c> (clé <c>No_AD_02</c>) ; refus du second
        /// contrôle souple sur l'égalité <c>WindowsLogin</c> = <c>Login</c> (clé <c>No_AD_03</c>).
        /// Ces sorties ne lèvent aucune exception, ne sont ni journalisées ni traitées par le
        /// pipeline terminal, et restituent l'issue <see cref="En_ChangeResult.Unchanged"/>.
        /// </para>
        /// <para>
        /// Dérogation de re-validation assumée : la non-nullité et la non-vacuité de
        /// <c>FirstName</c> et <c>LastName</c>, puis de <c>Login</c> et de <c>WindowsLogin</c>,
        /// sont contrôlées ici, en amont, bien que le Service <c>IS_UserApp_Create</c> porte déjà
        /// une validation <see cref="Ex_Business"/> (<c>BU_ER_01</c>) sur ses champs obligatoires.
        /// Cette duplication partielle est délibérée : le contrôle de <c>FirstName</c> et
        /// <c>LastName</c> sécurise la dérivation des <c>Initials</c> et la composition du
        /// <c>Login</c> attendu, qui indexent ces champs ; le contrôle de <c>Login</c> et
        /// <c>WindowsLogin</c> sécurise les deux contrôles souples qui les manipulent avant que
        /// le Service ne soit atteint, <c>WindowsLogin</c> étant nullable au schéma.
        /// </para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif de
        /// la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="entity">
        /// Copie de travail du compte utilisateur à créer, construite par le ViewModel appelant.
        /// Ses champs d'identité (<c>FirstName</c>, <c>LastName</c>, <c>Login</c>,
        /// <c>WindowsLogin</c>…) sont renseignés en entrée ; les <c>Initials</c> et le
        /// <c>PasswordHash</c> sont dérivés puis positionnés par le scénario. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        /// <param name="plainPassword">
        /// Mot de passe en clair saisi par l'administrateur. Il est haché par le scénario via
        /// <c>IS_Hashing</c> avant délégation au Service et n'est jamais persisté en clair.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Une tâche dont le résultat est l'issue de mutation restituée à la présentation :
        /// <see cref="En_ChangeResult.Changed"/> en clôture du chemin nominal, après validation
        /// de la transaction (<c>CommitAsync</c>), le compte ayant été effectivement créé ;
        /// <see cref="En_ChangeResult.Unchanged"/> sur tout autre chemin terminal — abandons
        /// utilisateur (clés <c>No_St_03</c>, <c>No_St_02</c>, <c>No_AD_02</c>, <c>No_AD_03</c>)
        /// et échecs applicatifs captés terminalement (<see cref="Ex_Business"/> <c>BU_ER_04</c>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>), après
        /// <c>RollbackAsync</c> et délégation à <c>IU_LogAndNotify</c>. Le consommateur de
        /// présentation lit cette issue pour décider de rafraîchir ou de conserver son état
        /// d'interface (R-4.14.22).
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée en interne (code <c>BU_ER_04</c>, « Login déjà existant ») lorsque le contrôle
        /// dur d'unicité constate qu'un compte porte déjà le <c>Login</c> demandé, quel que soit
        /// son statut (actif, inactif ou logiquement supprimé). Cette exception, au même titre
        /// que <see cref="Ex_Infrastructure"/> et <see cref="Ex_Unclassified"/> remontées et
        /// requalifiées par le Service en aval, n'est jamais propagée à l'appelant : elle est
        /// captée terminalement par <c>IU_LogAndNotify</c> (clés <c>No_EC_01</c> /
        /// <c>No_EC_02</c> / <c>No_EC_03</c> respectivement), le scénario restituant alors
        /// <see cref="En_ChangeResult.Unchanged"/>.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Seule exception propagée à l'appelant, lorsque l'annulation coopérative est demandée,
        /// conformément à §4.6.
        /// </exception>
        Task<En_ChangeResult> ExecuteAsync(
            string caller,
            UserApp entity,
            string plainPassword,
            CancellationToken ct = default);
    }
}