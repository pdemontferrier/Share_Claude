using LeitTemporImport.A_Domain.Interfaces.Settings.Infrastructure;

namespace LeitTemporImport.C_Infrastructure.Settings
{
    /// <summary>
    /// Centralise les paramètres de connexion à la base de données SQL.
    /// Utilise un compte technique dédié, commun à tous les utilisateurs finaux.
    /// </summary>
    public class SE_Database : ISE_Database
    {
        public string Host { get; } = "localhost";
        public int? Port { get; } = 1433; // null en Prod, 1433 en développement
        public string DatabaseName { get; } = "DIGIT_TRY_PROD";

        public string User { get; } = "USERDIGIT";
        public string Password { get; } = "ZX10-66046wX:-MHnUHB";

        public string BuildConnectionString()
        {
            var server = Port.HasValue
                ? $"{Host},{Port.Value}"
                : Host;

            return
                $"Server={server};" +
                $"Database={DatabaseName};" +
                $"User Id={User};" +
                $"Password={Password};" +
                $"TrustServerCertificate=True;";
        }
    }
}

