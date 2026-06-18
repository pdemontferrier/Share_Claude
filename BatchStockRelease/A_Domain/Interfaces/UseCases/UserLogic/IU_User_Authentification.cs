namespace BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic
{
    /// <summary>
    /// Interface définissant le contrat du UseCase d’authentification utilisateur.
    /// </summary>
    public interface IU_User_Authentification
    {
        /// <summary>
        /// Exécute le processus complet d’authentification utilisateur.
        /// </summary>
        /// <param name="caller">Chaîne d’appel d’origine pour la traçabilité.</param>
        Task ExecuteAsync(string caller);
    }
}