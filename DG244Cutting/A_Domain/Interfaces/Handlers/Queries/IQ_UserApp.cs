using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du QueryHandler (IQ) dédié à l'entité <see cref="UserApp"/> dans le
    /// cadre du modèle CQRS. Hérite du socle de lecture <see cref="IQ_Generic{T}"/>
    /// (treize lectures de base, non redéclarées ici) et ajoute trois lectures
    /// spécialisées par clés fonctionnelles.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les ViewModels (chaîne (2)) et les DataProviders
    /// (chaîne (3)), sans accès direct au repository ni au DbContext.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir un point d'entrée de requête traçable via CallChain, homogène avec
    /// les conventions projet, sans redéclarer les opérations du socle générique.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>ViewModels et DataProviders de lecture.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher un utilisateur par login Windows.</description></item>
    /// <item><description>Rechercher un utilisateur par login de connexion (repli d'authentification login/mot de passe), sous réserve de compte actif et non supprimé.</description></item>
    /// <item><description>Projeter l'identité d'un utilisateur (FullName) à partir de son identifiant primaire.</description></item>
    /// </list>
    /// </summary>
    public interface IQ_UserApp : IQ_Generic<UserApp>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne un utilisateur applicatif à partir de son login Windows.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour identifier l'utilisateur courant sur un poste (device user).</para>
        /// <para>Objectif</para>
        /// <para>Permettre une requête CQRS dédiée, traçable et cohérente avec la CallChain.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre <paramref name="windowsLogin"/>.</description></item>
        /// <item><description>Interroger le socle pour le premier enregistrement satisfaisant le prédicat, sans tracking.</description></item>
        /// <item><description>Retourner null si aucun utilisateur ne correspond.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="windowsLogin">Login Windows à rechercher.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Entité <see cref="UserApp"/> ou null.</returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code BU_ER_01) si <paramref name="windowsLogin"/> est null ou vide.
        /// </exception>
        Task<UserApp?> HandleGetByWindowsLoginAsync(string caller, string windowsLogin, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Retourne le nom complet (FullName) d'un utilisateur applicatif à partir de son
        /// identifiant primaire, sous la forme <c>"Prénom Nom"</c>.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Lecture spécialisée à clé fonctionnelle simple (<see cref="UserApp.Id"/>) avec
        /// projection vers une chaîne unique. Consommée par les ViewModels (chaîne (2))
        /// et les DataProviders (chaîne (3)) ayant besoin d'afficher l'identité usuelle
        /// d'un utilisateur sans manipuler l'entité <see cref="UserApp"/>.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir une projection traçable et robuste de l'identité d'un utilisateur,
        /// avec garantie de non-nullité et de non-vacuité du résultat. Une valeur de
        /// repli défensive est retournée en cas d'état dégradé non attendu sur
        /// <see cref="UserApp.FirstName"/> ou <see cref="UserApp.LastName"/>, les deux
        /// propriétés étant non-nullables côté schéma.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="userId"/>.</description></item>
        /// <item><description>Charger l'utilisateur par sa clé primaire via le socle hérité, sans tracking.</description></item>
        /// <item><description>Rejeter le cas d'utilisateur inexistant ou logiquement supprimé.</description></item>
        /// <item><description>Projeter l'entité vers la concaténation prénom + nom (garde-fou défensif).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant primaire de l'utilisateur. Doit être strictement positif.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Le nom complet de l'utilisateur au format <c>"Prénom Nom"</c>. Chaîne garantie
        /// non nulle et non vide ; en cas d'état dégradé non attendu où
        /// <see cref="UserApp.FirstName"/> ou <see cref="UserApp.LastName"/> serait blanc,
        /// la valeur de repli <c>"Utilisateur non identifié : {userId}"</c> est retournée.
        /// </returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code BU_ER_02) si <paramref name="userId"/> est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code BU_ER_03) si aucun utilisateur applicatif utilisable ne correspond
        /// à <paramref name="userId"/> (identifiant inexistant ou entité logiquement supprimée).
        /// </exception>
        Task<string> HandleGetFullNameByIdAsync(string caller, int userId, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne un utilisateur applicatif à partir de son login de connexion.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Lecture spécialisée mobilisée en repli d'authentification login/mot de passe
        /// (Page00), lorsque l'identification automatique par contexte device échoue. La
        /// recherche filtre sur trois critères conjoints : correspondance exacte du login,
        /// compte actif (<see cref="UserApp.IsActive"/>) et compte non logiquement supprimé
        /// (<see cref="UserApp.IsDeleted"/>). Un compte inactif ou supprimé est volontairement
        /// indistinguable d'un login inconnu du point de vue du consommateur (posture
        /// d'opacité) : les trois situations retournent null.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Permettre une requête CQRS dédiée, traçable et cohérente avec la CallChain. La
        /// lecture est pure : aucune comparaison de mot de passe n'est effectuée. L'entité
        /// <see cref="UserApp"/> complète (<see cref="UserApp.PasswordHash"/> inclus) est
        /// retournée, à charge du UseCase d'authentification consommateur d'en exploiter les
        /// champs.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre <paramref name="login"/>.</description></item>
        /// <item><description>Interroger le socle pour le premier enregistrement satisfaisant le prédicat conjoint (login exact, actif, non supprimé), sans tracking.</description></item>
        /// <item><description>Retourner null si aucun compte utilisable ne correspond.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="login">Login de connexion à rechercher.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Entité <see cref="UserApp"/> complète si un compte actif, non supprimé, au login
        /// exact existe ; null sinon (login inconnu, compte inactif ou compte supprimé,
        /// indistinguables).
        /// </returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code BU_ER_01) si <paramref name="login"/> est null, vide ou composé
        /// uniquement d'espaces.
        /// </exception>
        Task<UserApp?> HandleGetByLoginAsync(string caller, string login, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Indique si au moins un compte applicatif porte le login de connexion fourni,
        /// quel que soit le statut de ce compte.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Lecture d'existence pure au service du contrôle d'unicité de la clé fonctionnelle
        /// <see cref="UserApp.Login"/> en base. Le résultat reflète l'unicité en base
        /// indépendamment de l'état du compte : un compte inactif (<see cref="UserApp.IsActive"/>
        /// à <see langword="false"/>) ou logiquement supprimé (<see cref="UserApp.IsDeleted"/>
        /// à <see langword="true"/>) est pris en compte au même titre qu'un compte actif. Le
        /// prédicat ne filtre donc ni le statut d'activité ni la suppression logique : QH-17
        /// n'impose aucun filtrage IsDeleted par défaut côté socle, et un résultat
        /// <see langword="true"/> peut provenir d'un compte inactif ou logiquement supprimé.
        /// Cette portée constitue une divergence explicite et assumée vis-à-vis des trois
        /// lectures spécialisées existantes, qui filtrent le statut.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir une requête CQRS d'existence dédiée, traçable et cohérente avec la CallChain.
        /// Cette lecture n'est pas un chemin d'authentification : elle ne matérialise aucune
        /// entité, n'effectue aucune comparaison de mot de passe, et ne restitue ni
        /// <see cref="UserApp.PasswordHash"/> ni aucune donnée personnelle. Le rôle
        /// d'authentification (avec posture d'opacité et filtrage du statut) est tenu par
        /// <see cref="HandleGetByLoginAsync"/>, dont la sémantique diffère explicitement.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre <paramref name="login"/>.</description></item>
        /// <item><description>Interroger le socle pour l'existence d'un enregistrement satisfaisant le prédicat de login exact, sans matérialisation d'entité ni filtrage de statut.</description></item>
        /// <item><description>Retourner true si un tel compte existe quel que soit son statut ; false sinon.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="login">Login de connexion dont l'existence en base est vérifiée.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// <see langword="true"/> si au moins un compte applicatif porte exactement le login
        /// fourni, quel que soit son statut (actif ou inactif, supprimé logiquement ou non) ;
        /// <see langword="false"/> sinon.
        /// </returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code BU_ER_01) si <paramref name="login"/> est null, vide ou composé
        /// uniquement d'espaces.
        /// </exception>
        Task<bool> HandleAnyByLoginAsync(string caller, string login, CancellationToken ct = default);

        #endregion
    }
}