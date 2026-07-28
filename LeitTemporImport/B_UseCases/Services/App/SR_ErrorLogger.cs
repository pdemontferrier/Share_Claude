using System.Text;
using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.DTOs.App;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.Settings.App;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Services.App
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service de journalisation des erreurs dans un fichier CSV structuré et, si possible, dans la base de données.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases et Services de l’application console. L’application ne dispose pas de mécanisme
    /// de notification utilisateur ; la stratégie retenue consiste uniquement à journaliser (fichier + base).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser la capture et la persistance des erreurs en respectant les standards 104 :
    /// CallChain, normalisation des exceptions, robustesse (le logger ne doit pas faire tomber l’application).
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases et Services applicatifs du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Normaliser l’exception (Business / Infrastructure / Unknown).</description></item>
    /// <item><description>Écrire une ligne CSV dans un fichier de log.</description></item>
    /// <item><description>Tenter un enregistrement en base.</description></item>
    /// <item><description>Ne jamais casser le pipeline (best effort).</description></item>
    /// </list>
    /// </summary>
    public class SR_ErrorLogger : IS_ErrorLogger
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        private const string CsvHeader =
            "Timestamp;AppId;AppCallChain;AppErrorId;AppErrorMessage;AppErrorDetails;AppUserId;DeviceUser;DeviceId;DeviceIp";

        private readonly UTF8Encoding _utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

        #endregion

        #region === Dépendances privées ===

        private readonly IC_UserAppErrorLog _chUserAppErrorLog;
        private readonly IQ_AppContext _appContextProvider;
        private readonly string _logPath;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service de journalisation des erreurs.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances et préparer le chemin du fichier de log.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider les dépendances.</description></item>
        /// <item><description>Construire le chemin du fichier de log.</description></item>
        /// <item><description>Créer le dossier/fichier si nécessaire (best effort).</description></item>
        /// </list>
        /// </summary>
        public SR_ErrorLogger(
            IC_UserAppErrorLog chUserAppErrorLog,
            IQ_AppContext appContextProvider,
            ISE_App settings)
        {
            _callee = GetType().Name;

            _chUserAppErrorLog = chUserAppErrorLog ?? throw new ArgumentNullException(nameof(chUserAppErrorLog));
            _appContextProvider = appContextProvider ?? throw new ArgumentNullException(nameof(appContextProvider));
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            _logPath = BuildLogFilePath(settings);

            // Un logger ne doit pas faire échouer l’app : best effort
            TryEnsureLogFileExists(_logPath);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Journalise une exception (CSV + tentative base) sans interrompre le flux d’exécution.</para>
        /// <para>Contexte</para>
        /// <para>Appelée dans les blocs catch des UseCases et Services.</para>
        /// <para>Objectif</para>
        /// <para>Persister une erreur de manière robuste, traçable (CallChain) et normalisée.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Récupérer le contexte applicatif.</description></item>
        /// <item><description>Normaliser l’exception.</description></item>
        /// <item><description>Écrire dans le fichier CSV (best effort).</description></item>
        /// <item><description>Écrire en base si possible (best effort).</description></item>
        /// </list>
        /// </summary>
        public async Task ExecuteAsync(string caller, Exception ex, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (ex is null) return;

                DTO_AppContext context = _appContextProvider.GetAppContext();

                NormalizedError normalized = NormalizeException(callChain, caller, ex);

                string csvLine = BuildCsvLine(context, normalized);
                UserAppErrorLog entity = BuildEntity(context, normalized);

                await TryWriteToFileAsync(callChain, csvLine, ct);
                await TryWriteToDatabaseAsync(callChain, entity, ct);
            }
            catch
            {
                // Un logger ne doit jamais déclencher un échec du traitement principal.
                // Best effort : on avale toute erreur interne.
            }
        }

        #endregion

        #region === Méthodes privées ===

        private static string BuildLogFilePath(ISE_App settings)
        {
            // BaseDirectory -> ...\bin\Release\net8.0-windows\  => on remonte pour retrouver la racine exécution
            string baseDir = AppContext.BaseDirectory;
            string releaseFolder = Directory.GetParent(baseDir)?.FullName ?? baseDir;
            string rootFolder = Directory.GetParent(releaseFolder)?.FullName ?? releaseFolder;

            return Path.Combine(rootFolder, settings.GetErrorLogFileName());
        }

        private void TryEnsureLogFileExists(string logPath)
        {
            try
            {
                string? logDir = Path.GetDirectoryName(logPath);
                if (string.IsNullOrWhiteSpace(logDir))
                    return;

                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                if (!File.Exists(logPath))
                    File.WriteAllText(logPath, CsvHeader + Environment.NewLine, _utf8Bom);
            }
            catch
            {
                // Best effort : ne pas casser l’application
            }
        }

        private NormalizedError NormalizeException(string caller, string originalCaller, Exception ex)
        {
            string callChain = $"{caller} > {nameof(NormalizeException)}";

            try
            {
                // Valeurs par défaut
                string normalizedCallChain = originalCaller;
                string errorId = "UnknownErrorId";
                string errorMessage = "Une erreur inattendue s'est produite lors de l'exécution :";
                string errorDetails = ex.Message ?? "UnknownException";

                switch (ex)
                {
                    case Ex_Business bex:
                        normalizedCallChain = bex.CallChain ?? originalCaller;
                        errorId = bex.ErrorId ?? "UnknownErrorId";
                        errorMessage = "Une erreur métier s'est produite lors de l'exécution :";
                        errorDetails = bex.ErrorException ?? (bex.Message ?? "UnknownException");
                        break;

                    case Ex_Infrastructure iex:
                        normalizedCallChain = iex.CallChain ?? originalCaller;
                        errorId = iex.ErrorId ?? "UnknownErrorId";
                        errorMessage = "Une erreur d'infrastructure s'est produite lors de l'exécution :";
                        errorDetails = iex.ErrorException ?? (iex.Message ?? "UnknownException");
                        break;
                }

                return new NormalizedError(
                    CallChain: normalizedCallChain,
                    ErrorId: errorId,
                    ErrorMessage: errorMessage,
                    ErrorDetails: errorDetails);
            }
            catch (Exception inner)
            {
                // Normalisation best effort
                return new NormalizedError(
                    CallChain: originalCaller,
                    ErrorId: "NormalizeErrorFailed",
                    ErrorMessage: "Erreur lors de la normalisation d'une exception :",
                    ErrorDetails: inner.Message ?? "UnknownException");
            }
        }

        private static string BuildCsvLine(DTO_AppContext context, NormalizedError normalized)
        {
            return string.Join(";", new[]
            {
                EscapeCsvValue(context.AppDateTime.ToString("yyyy-MM-dd HH:mm:ss")),
                EscapeCsvValue(context.AppId.ToString()),
                EscapeCsvValue(normalized.CallChain),
                EscapeCsvValue(normalized.ErrorId),
                EscapeCsvValue(normalized.ErrorMessage),
                EscapeCsvValue(normalized.ErrorDetails),
                EscapeCsvValue(context.AppUserId.ToString()),
                EscapeCsvValue(context.AppDeviceUser ?? string.Empty),
                EscapeCsvValue(context.AppDeviceId ?? string.Empty),
                EscapeCsvValue(context.AppDeviceIP ?? string.Empty)
            });
        }

        private static UserAppErrorLog BuildEntity(DTO_AppContext context, NormalizedError normalized)
        {
            return new UserAppErrorLog
            {
                ErrorTimestamp = context.AppDateTime,
                IdApplication = context.AppId,
                CallChain = normalized.CallChain,
                ErrorCode = normalized.ErrorId,
                ErrorMessage = normalized.ErrorMessage,
                ErrorException = normalized.ErrorDetails,
                IdUser = context.AppUserId,
                DeviceUser = context.AppDeviceUser,
                DeviceId = context.AppDeviceId,
                DeviceIp = context.AppDeviceIP
            };
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string safe = value
                .Replace("\"", "\"\"")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Trim();

            return $"\"{safe}\"";
        }

        private async Task TryWriteToFileAsync(string caller, string csvLine, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(TryWriteToFileAsync)}";

            try
            {
                // Best effort : si ça échoue, on n’arrête pas le traitement.
                await File.AppendAllTextAsync(_logPath, csvLine + Environment.NewLine, _utf8Bom, ct);
            }
            catch
            {
                // Avaler : un logger ne doit pas déclencher un échec
            }
        }

        private async Task TryWriteToDatabaseAsync(string caller, UserAppErrorLog entity, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(TryWriteToDatabaseAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                // IC_UserAppErrorLog ne prend pas ct actuellement.
                // On garde la signature existante et on laisse le handler décider.
                await _chUserAppErrorLog.HandleAddAsync(entity);
            }
            catch
            {
                // Avaler : un logger ne doit pas déclencher un échec
            }
        }

        #endregion

        #region === Types privés ===

        private sealed record NormalizedError(string CallChain, string ErrorId, string ErrorMessage, string ErrorDetails);

        #endregion
    }
}