using System.Diagnostics;
using System.IO;
using System.Text;
using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// Service de journalisation des erreurs dans un fichier CSV structuré et si possible dans la base de données.
    /// Le fichier est situé dans le dossier parent de l'application, sous "99_Errors_logs".
    /// </summary>
    public class SR_FileLogger : IS_FileLogger
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        /// <summary>
        /// En-tête CSV du fichier de log.
        /// </summary>
        private const string CsvHeader = "Timestamp;AppId;AppCallChain;AppErrorId;AppErrorMessage;AppErrorDetails;AppUserId;DeviceUser;DeviceId;DeviceIp";

        #endregion

        #region === Dépendances privées ===

        private readonly IC_UserAppErrorLog _chUserAppErrorLog;
        private readonly IQ_AppContext _appContextProvider;
        private readonly IS_Settings_App _settings;
        private readonly string _logPath;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service de journalisation des erreurs.
        /// </summary>
        /// <param name="chUserAppErrorLog">Handler pour l’écriture en base.</param>
        /// <param name="appContextProvider">Handler pour la lecture en base.</param>
        /// <param name="settings">Service de configuration de l’application.</param>
        public SR_FileLogger(
            IC_UserAppErrorLog chUserAppErrorLog, 
            IQ_AppContext appContextProvider, 
            IS_Settings_App settings)
        {
            _callee = GetType().Name;
            _chUserAppErrorLog = chUserAppErrorLog;
            _appContextProvider = appContextProvider;
            _settings = settings;

            // Initialiser le chemin complet du fichier de log
            _logPath = BuildLogFilePath();

            // Vérifie l’existence du fichier et le crée si nécessaire
            EnsureLogFileExists();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Journalise une erreur dans un fichier CSV et tente un enregistrement en base.
        /// </summary>
        /// <param name="callChain">Chaîne d’appel complète (UseCase &gt; Service &gt; Méthode).</param>
        /// <param name="errorId">Identifiant unique de l’erreur (code fonctionnel ou technique).</param>
        /// <param name="errorMessage">Message principal décrivant le contexte de l’erreur.</param>
        /// <param name="errorDetails">Détails techniques ou exceptionnels associés.</param>
        public async Task LogErrorAsync(string callChain, string errorId, string errorMessage, string errorDetails)
        {

            DTO_AppContext context = _appContextProvider.GetAppContext();

            var csvLog = string.Join(";", new[]
            {
                EscapeCsvValue(context.AppDateTime.ToString("yyyy-MM-dd HH:mm:ss")),
                context.AppId.ToString(),
                EscapeCsvValue(callChain),
                EscapeCsvValue(errorId),
                EscapeCsvValue(errorMessage),
                EscapeCsvValue(errorDetails),
                context.AppUserId.ToString(),
                EscapeCsvValue(context.AppDeviceUser ?? string.Empty),
                EscapeCsvValue(context.AppDeviceId ?? string.Empty),
                EscapeCsvValue(context.AppDeviceIP ?? string.Empty)
            });

            var entityLog = new UserAppErrorLog
            {
                Timestamp = context.AppDateTime,
                AppId = context.AppId,
                AppCallChain = callChain,
                AppErrorId = errorId,
                AppErrorMessage = errorMessage,
                AppErrorException = errorDetails,
                AppUserId = context.AppUserId,
                DeviceUser = context.AppDeviceUser,
                DeviceId = context.AppDeviceId,
                DeviceIp = context.AppDeviceIP
            };

            await WriteToFileAsync(csvLog);
            await WriteToDatabaseAsync(entityLog);
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Construit le chemin complet du fichier de log à partir des paramètres d’application.
        /// </summary>
        private string BuildLogFilePath()
        {
            var baseDir = AppContext.BaseDirectory;
            var releaseFolder = Directory.GetParent(baseDir)?.FullName ?? baseDir;
            var logDir = Directory.GetParent(releaseFolder)?.FullName ?? releaseFolder;

            return Path.Combine(logDir, _settings.GetErrorLogFileName());
        }

        /// <summary>
        /// Vérifie l’existence du dossier et du fichier de log, et les crée si nécessaire.
        /// </summary>
        private void EnsureLogFileExists()
        {
            var logDir = Path.GetDirectoryName(_logPath)!;

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            if (!File.Exists(_logPath))
                File.WriteAllText(_logPath, CsvHeader + Environment.NewLine, new UTF8Encoding(true));
        }

        /// <summary>
        /// Échappe les caractères interdits pour le format CSV.
        /// </summary>
        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            return $"\"{value.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", " ").Trim()}\"";
        }

        /// <summary>
        /// Écrit la ligne de log dans le fichier CSV.
        /// </summary>
        private async Task WriteToFileAsync(string csvLog)
        {
            try
            {
                await File.AppendAllTextAsync(_logPath, csvLog + Environment.NewLine, new UTF8Encoding(true));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{_callee}] Erreur écriture fichier : {ex.Message}");
            }
        }

        /// <summary>
        /// Écrit le log dans la base de données si possible.
        /// </summary>
        private async Task WriteToDatabaseAsync(UserAppErrorLog entityLog)
        {
            try
            {
                await _chUserAppErrorLog.HandleAddAsync(entityLog);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{_callee}] Erreur écriture en base : {ex.Message}");
            }
        }

        #endregion

    }
}