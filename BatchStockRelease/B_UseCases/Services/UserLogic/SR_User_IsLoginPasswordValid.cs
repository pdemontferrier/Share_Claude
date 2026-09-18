using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
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
    /// Le service repose sur l’interface <see cref="IQ_User"/> pour effectuer la requête
    /// de vérification via un <i>QueryHandler</i> dédié, garantissant la séparation des responsabilités
    /// entre la logique applicative et l’accès aux données.
    /// </para>
    public class SR_User_IsLoginPasswordValid : IS_User_IsLoginPasswordValid
    {
        #region === Propriétés privées ===
        /// <summary>
        /// Nom interne du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;
        #endregion

        #region === Dépendances privées ===

        private readonly IQ_User _qhUser;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_User_IsLoginPasswordValid"/>.
        /// </summary>
        /// <param name="qhUser">
        /// Instance de <see cref="IQ_User"/> injectée via le conteneur de dépendances,
        /// permettant l'accès aux données utilisateurs.
        /// </param>
        public SR_User_IsLoginPasswordValid(IQ_User qhUser)
        {
            _callee = GetType().Name;

            _qhUser = qhUser;
        }

        #endregion

        #region === Méthodes publiques ===

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
        public async Task<User?> ExecuteAsync(string loginId, string password, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                return await _qhUser.HandleGetSingleAsync(loginId, password);
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }

        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion

    }
}
