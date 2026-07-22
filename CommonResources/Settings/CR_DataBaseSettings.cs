namespace CommonResources.Settings
{
    public static class CR_DataBaseSettings
    {
        // Informations de connexion à la base de données (privées pour ne pas être accessibles en dehors de cette classe)
        // Base de développement
        private static readonly string server_dev = "localhost";
        private static readonly string database_dev = "geststock";
        private static readonly string uid_dev = "root";
        private static readonly string password_dev = "root";
        private static readonly string port_dev = "3306";

        // Base de déploiement
        private static readonly string server_prod = "atrya233";
        private static readonly string database_prod = "geststock";
        private static readonly string uid_prod = "sroyer";
        private static readonly string password_prod = "leverandier";
        private static readonly string port_prod = "3306";

        // Chaîne de connexion publique et statique (seul élément accessible en dehors de la classe)
        // Développement
        public static readonly string CredentialsGeststock_dev = $"Server={server_dev};Port={port_dev};Database={database_dev};Uid={uid_dev};Pwd={password_dev};";
        // Déploiement
        public static readonly string CredentialsGeststock_prod = $"Server={server_prod};Port={port_prod};Database={database_prod};Uid={uid_prod};Pwd={password_prod};";
    }
}
