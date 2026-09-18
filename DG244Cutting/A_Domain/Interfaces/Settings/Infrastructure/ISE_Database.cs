namespace DG244Cutting.A_Domain.Interfaces.Settings.Infrastructure
{
    /// <summary>
    /// Contrat exposant les paramètres de connexion à la base de données.
    /// Utilisé par l’infrastructure et les UseCases sans dépendance concrète.
    /// </summary>
    public interface ISE_Database
    {
        string Host { get; }
        int? Port { get; }
        string DatabaseName { get; }

        string User { get; }
        string Password { get; }

        /// <summary>
        /// Construit la chaîne de connexion SQL Server associée.
        /// </summary>
        string BuildConnectionString();
    }
}

