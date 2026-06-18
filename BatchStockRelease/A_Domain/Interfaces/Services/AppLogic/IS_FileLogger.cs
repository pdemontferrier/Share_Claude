namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    /// <summary>
    /// Service de journalisation des erreurs dans un fichier CSV structuré et si possible dans la base de données.
    /// Le fichier est situé dans le dossier parent de l'application, sous "99_Errors_logs".
    /// </summary>
    public interface IS_FileLogger
    {
        /// <summary>
        /// Enregistre une erreur dans un fichier CSV et si possible dans la base de données.
        /// </summary>
        /// <param name="callChain">Chaîne d'appel</param>
        /// <param name="errorId">Code d’erreur métier ou technique</param>
        /// <param name="errorMessage">Message descriptif de l’erreur</param>
        /// <param name="errorDetails">Details du message d'exception</param>
        Task LogErrorAsync(string callChain, string errorId, string errorMessage, string errorDetails);
    }
}
