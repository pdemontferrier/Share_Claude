using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    /// <summary>
    /// Service métier <c>SR_User_IsLoginPasswordValid</c>.
    /// </summary>
    /// <para>
    /// Ce service permet de vérifier la validité des informations d'identification d'un utilisateur,
    /// en comparant l'identifiant de connexion et le mot de passe saisi avec les enregistrements
    /// existants dans la base de données.
    /// </para>
    /// <para>
    /// Il est principalement appelé par le ViewModel <c>VM_Page00</c> lors du processus de connexion
    /// afin de déterminer si un utilisateur est autorisé à accéder à l'application.
    /// </para>
    /// <para>
    /// Le service repose sur l’interface <see cref="BatchStockRelease.A_Domain.Interfaces.Handlers.Queries.IQ_User"/> pour effectuer la requête
    /// de vérification via un <i>QueryHandler</i> dédié, garantissant la séparation des responsabilités
    /// entre la logique applicative et l’accès aux données.
    /// </para>
    public interface IS_User_IsLoginPasswordValid
    {
        /// <summary>
        /// Vérifie si les informations d'identification (login et mot de passe)
        /// correspondent à un utilisateur enregistré dans la base de données.
        /// </summary>
        /// <param name="loginId">Identifiant de connexion saisi par l'utilisateur.</param>
        /// <param name="password">Mot de passe associé à l'identifiant.</param>
        /// <param name="caller">Chaîne de contexte de l'appelant pour la traçabilité.</param>
        /// <returns>
        /// Une instance de <see cref="User"/> si l'authentification est réussie,
        /// sinon <see langword="null"/>.
        /// </returns>
        Task<User?> ExecuteAsync(string loginId, string password, string caller);
    }
}