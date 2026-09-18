using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Commands;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using System.IO;
using System.Text;

namespace DG244Cutting.B_UseCases.Services.App
{
    /// <summary>
    /// Service transverse de journalisation des erreurs applicatives, opérant sur deux canaux
    /// indépendants : un fichier CSV structuré sur disque et la table <c>UserAppErrorLog</c>
    /// en base de données.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : service positionné dans B_UseCases/Services/App, implémentant
    /// <see cref="IS_ErrorLogger"/>. Il constitue le canal de persistance technique des
    /// erreurs applicatives, invocable selon deux régimes distincts.
    /// </para>
    /// <para>
    /// Régime primaire — UseCases : ce service est principalement invoqué depuis
    /// l'orchestrateur de traitement terminal des erreurs (typiquement
    /// <c>UC_LogAndNotify</c>), après que l'exception typée a remonté le pipeline complet
    /// et que le message UI traduit a été résolu via le dictionnaire multilingue.
    /// C'est le flux standard et principal de journalisation.
    /// </para>
    /// <para>
    /// Régime secondaire — Services transverses autorisés (exception architecturale
    /// documentée) : certains services transverses spécifiques dont le comportement de type
    /// best effort requiert une journalisation autonome non bloquante peuvent appeler
    /// directement ce service (ex. : <c>SR_Dictionary</c> lors d'une clé de traduction
    /// manquante). Cette exception est délibérée : elle couvre les cas où produire une trace
    /// ne doit pas interrompre le flux principal et où l'escalade vers un UseCase serait
    /// disproportionnée ou techniquement impossible.
    /// </para>
    /// <para>
    /// Exception architecturale de persistance : contrairement aux services métier standard,
    /// <c>SR_ErrorLogger</c> délègue la persistance base à <c>CH_UserAppErrorLog</c>, dont
    /// l'implémentation crée son propre DbContext de courte durée, garantissant la survie
    /// des enregistrements même en cas de rollback de la transaction principale du UseCase.
    /// </para>
    /// <para>
    /// Chemin du fichier de log : calculé à la construction par <c>BuildLogFilePath()</c>
    /// à partir des constantes internes <c>ErrorLogFolder</c> et <c>ErrorLogFileName</c>,
    /// sans dépendance envers <c>ISE_App</c>. Le nom du répertoire et du fichier de log
    /// sont des informations d'implémentation internes, non configurables, ce qui permet à
    /// <c>SR_ErrorLogger</c> d'être instancié sans accès aux interfaces de settings.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Normaliser l'exception reçue en structure interne exploitable.
    ///   </description></item>
    ///   <item><description>
    ///     Reconstruire la chaîne complète des exceptions imbriquées, bornée à
    ///     <see cref="MaxErrorDetailsLength"/> caractères.
    ///   </description></item>
    ///   <item><description>
    ///     Tenter une écriture fichier CSV en mode best effort.
    ///   </description></item>
    ///   <item><description>
    ///     Tenter une écriture en base via <see cref="IC_UserAppErrorLog"/> en mode
    ///     best effort.
    ///   </description></item>
    ///   <item><description>
    ///     Propager la CallChain à travers toutes les étapes de journalisation.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Aucune traduction du message d'erreur : fourni résolu par l'appelant.
    ///   </description></item>
    ///   <item><description>
    ///     Aucune notification à l'opérateur.
    ///   </description></item>
    ///   <item><description>
    ///     Aucune classification des exceptions brutes .NET.
    ///   </description></item>
    ///   <item><description>
    ///     Aucun calcul de chemin externe : le nom et le chemin du fichier de log sont
    ///     entièrement gérés en interne via <c>ErrorLogFileName</c> et
    ///     <c>BuildLogFilePath()</c>.
    ///   </description></item>
    /// </list>
    /// </remarks>
    public class SR_ErrorLogger : IS_ErrorLogger
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        /// <summary>
        /// En-tête CSV fixe correspondant à la structure des colonnes du fichier de log.
        /// </summary>
        private const string CsvHeader =
            "Timestamp;AppId;AppCallChain;AppErrorId;AppErrorMessage;AppErrorDetails;AppUserId;DeviceUser;DeviceId;DeviceIp";

        /// <summary>
        /// Longueur maximale autorisée pour le champ <c>ErrorDetails</c> persisté en base de
        /// données.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Correspond à la taille de la colonne <c>ErrorException</c> dans la table
        /// <c>UserAppErrorLog</c>. Toute chaîne excédant cette limite est tronquée avec
        /// l'indicateur <c>…</c>, distinguant ainsi un détail complet d'un détail tronqué
        /// lors de l'analyse des logs.
        /// </para>
        /// </remarks>
        private const int MaxErrorDetailsLength = 3999;

        /// <summary>
        /// Nom du fichier CSV de journalisation des erreurs applicatives.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Constante technique propre à ce service : le nom du fichier de log est une
        /// information d'implémentation interne, non configurable via les settings
        /// applicatifs. Ce choix garantit que <c>SR_ErrorLogger</c> reste autonome et
        /// ne dépend d'aucune interface de settings (<c>ISE_App</c>), conformément à la
        /// règle d'accès aux settings : seuls les UseCases accèdent directement aux
        /// interfaces de settings ; les services reçoivent les valeurs résolues par
        /// arguments ou les définissent comme constantes internes lorsqu'elles ne sont
        /// pas configurables.
        /// </para>
        /// </remarks>
        private const string ErrorLogFileName = "error_log.csv";

        /// <summary>
        /// Nom du répertoire de journalisation, situé au même niveau que le répertoire de
        /// l'application dans le dossier parent des applications.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Structure cible sur disque :
        /// </para>
        /// <code>
        /// Dossier des applications\
        ///   ├── DG244Cutting\          ← répertoire de l'application
        ///   └── 99_Errorlog\           ← répertoire frère, au même niveau
        ///         └── error_log.csv
        /// </code>
        /// <para>
        /// Ce positionnement garantit que les logs restent accessibles et stables
        /// indépendamment des mises à jour ou redéploiements du répertoire applicatif,
        /// et qu'ils peuvent être partagés entre plusieurs applications du parc si
        /// nécessaire.
        /// </para>
        /// </remarks>
        private const string ErrorLogFolder = "99_Errorlog";

        /// <summary>Encodage UTF-8 avec BOM utilisé pour l'écriture du fichier CSV.</summary>
        private readonly UTF8Encoding _utf8Bom = new(encoderShouldEmitUTF8Identifier: true);

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Command handler spécialisé assurant la persistance immédiate et autonome des
        /// enregistrements de log d'erreurs, dans un DbContext indépendant de toute
        /// transaction UseCase en cours.
        /// </summary>
        private readonly IC_UserAppErrorLog _chUserAppErrorLog;

        /// <summary>
        /// Chemin complet du fichier CSV de journalisation, calculé à la construction
        /// depuis <see cref="ErrorLogFolder"/>, <see cref="ErrorLogFileName"/> et le
        /// répertoire parent des applications.
        /// </summary>
        private readonly string _logPath;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_ErrorLogger"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instanciée via le conteneur DI dans B_UseCases. Le chemin du fichier
        /// de log est calculé automatiquement par <see cref="BuildLogFilePath"/> à partir des
        /// constantes internes <see cref="ErrorLogFolder"/> et <see cref="ErrorLogFileName"/>.
        /// Aucun settings externe n'est requis.
        /// </para>
        /// <para>
        /// Objectif : préparer le service à journaliser les erreurs dans un mode robuste et
        /// non bloquant, en initialisant la CallChain locale, les dépendances injectées et
        /// le support de journalisation fichier.
        /// </para>
        /// </remarks>
        /// <param name="chUserAppErrorLog">
        /// Command handler dédié à la persistance immédiate et autonome des enregistrements
        /// de log d'erreurs. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="appContext">
        /// Provider du contexte applicatif courant, fournissant les informations d'identité
        /// nécessaires à chaque entrée de log. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="chUserAppErrorLog"/> ou <paramref name="appContextProvider"/>
        /// est <see langword="null"/>.
        /// </exception>
        public SR_ErrorLogger(IC_UserAppErrorLog chUserAppErrorLog)
        {
            _callee = GetType().Name;

            _chUserAppErrorLog = chUserAppErrorLog ?? throw new ArgumentNullException(nameof(chUserAppErrorLog));
            _logPath = BuildLogFilePath();
            TryEnsureLogFileExists(_logPath);
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Journalise une erreur de manière robuste dans un fichier CSV structuré et,
        /// si possible, dans la base de données applicative.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée soit depuis l'orchestrateur terminal des erreurs après réception
        /// d'une exception typée remontée du pipeline, soit depuis un service transverse
        /// autorisé nécessitant une journalisation autonome non bloquante. Dans le premier
        /// cas, <paramref name="errorMessage"/> contient le message principal traduit via le
        /// dictionnaire multilingue. Dans le second cas, il s'agit d'un message technique en
        /// français rédigé en dur.
        /// </para>
        /// <para>
        /// Objectif : persister une erreur de manière traçable et non bloquante, en conservant
        /// la CallChain d'origine, l'identifiant d'erreur normalisé et le détail technique
        /// complet reconstruit depuis la chaîne des exceptions imbriquées.
        /// </para>
        /// <para>
        /// Garantie best effort : cette méthode n'est jamais la source d'une interruption
        /// applicative. Toute défaillance interne est absorbée silencieusement après que
        /// <paramref name="ct"/> a été contrôlé en entrée.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour
        /// enrichissement et traçabilité.</param>
        /// <param name="appCtx">Contexte applicatif courant résolu par le composant appelant et utilisé pour
        /// enrichir l'entrée de log : application, utilisateur, poste, adresse IP et
        /// horodatage applicatif. Si <see langword="null"/>, la méthode retourne
        /// immédiatement sans erreur afin de préserver le comportement best effort.</param>
        /// <param name="errorMessage">
        /// Message principal à enregistrer dans le log. Doit être non nul et non vide.
        /// Fourni résolu (traduit ou rédigé en français) par l'appelant.
        /// </param>
        /// <param name="ex">Exception typée à journaliser. Si <see langword="null"/>, la
        /// méthode retourne immédiatement sans erreur.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière
        /// coopérative avant l'entrée dans le mode best effort interne.</param>
        /// <returns>Tâche représentant l'exécution asynchrone de la journalisation.</returns>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant l'entrée dans
        /// le mode best effort. Aucune exception n'est levée après ce point.
        /// </exception>
        public async Task ExecuteAsync(
            string caller,
            DTO_AppContext appCtx,
            string errorMessage,
            Exception ex,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                if (ex is null)
                    return;

                NormalizedError normalized = NormalizeException(callChain, caller, errorMessage, ex);

                string csvLine = BuildCsvLine(appCtx, normalized);
                UserAppErrorLog entity = BuildEntity(appCtx, normalized);

                await TryWriteToFileAsync(callChain, csvLine, ct);
                await TryWriteToDatabaseAsync(callChain, entity, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                // Best effort : un logger ne doit jamais interrompre le flux principal.
            }
        }

        #endregion


        #region === Méthodes privées ===

        /// <summary>
        /// Calcule le chemin complet du fichier CSV de journalisation à partir du répertoire
        /// racine de l'application et des constantes <see cref="ErrorLogFolder"/> et
        /// <see cref="ErrorLogFileName"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée une unique fois depuis le constructeur. Le chemin est calculé
        /// en remontant de deux niveaux depuis <c>AppContext.BaseDirectory</c> afin d'obtenir
        /// le répertoire parent des applications, indépendamment de la configuration de build
        /// (<c>Debug</c> / <c>Release</c>).
        /// </para>
        /// <para>
        /// Structure produite :
        /// </para>
        /// <code>
        /// Dossier des applications\
        ///   ├── DG244Cutting\          ← répertoire de l'application
        ///   └── 99_Errorlog\           ← ErrorLogFolder
        ///         └── error_log.csv    ← ErrorLogFileName
        /// </code>
        /// <para>
        /// Le répertoire <see cref="ErrorLogFolder"/> est créé automatiquement par
        /// <see cref="TryEnsureLogFileExists"/> s'il n'existe pas encore.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Chemin complet du fichier CSV de journalisation.
        /// </returns>
        private static string BuildLogFilePath()
        {
            string baseDir = AppContext.BaseDirectory;
            string releaseFolder = Directory.GetParent(baseDir)?.FullName ?? baseDir;
            string rootFolder = Directory.GetParent(releaseFolder)?.FullName ?? releaseFolder;

            return Path.Combine(rootFolder, ErrorLogFolder, ErrorLogFileName);
        }

        /// <summary>
        /// Crée le répertoire et le fichier de log s'ils n'existent pas encore, en initialisant
        /// l'en-tête CSV.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée une unique fois depuis le constructeur afin de préparer le
        /// support de journalisation fichier dès l'instanciation du service.
        /// </para>
        /// <para>
        /// Objectif : garantir que le fichier CSV est opérationnel dès la première erreur,
        /// sans interrompre le démarrage applicatif si une anomalie de création survient
        /// (disque plein, permissions insuffisantes, chemin invalide).
        /// </para>
        /// </remarks>
        /// <param name="logPath">Chemin complet du fichier de log à créer si absent.</param>
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
                // Best effort : ne pas bloquer le démarrage applicatif.
            }
        }

        /// <summary>
        /// Normalise une exception typée du projet en structure interne exploitable par les
        /// écritures fichier et base de données.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="ExecuteAsync"/> après réception d'une exception
        /// déjà classifiée (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/> ou
        /// <see cref="Ex_Unclassified"/>). Toute exception brute .NET a été requalifiée en
        /// amont par <c>SR_ExClassifier</c> dans les composants producteurs.
        /// </para>
        /// <para>
        /// Objectif : produire un objet <see cref="NormalizedError"/> cohérent contenant la
        /// CallChain d'origine extraite de l'exception (ou <paramref name="callerFallback"/>
        /// si absente), l'identifiant d'erreur normalisé, le message principal déjà résolu
        /// et le détail technique complet reconstruit depuis la chaîne des exceptions
        /// imbriquées, borné à <see cref="MaxErrorDetailsLength"/> caractères.
        /// </para>
        /// <para>
        /// Résilience interne : tout échec survenant dans cette méthode produit une structure
        /// de secours minimale afin de ne pas priver le pipeline de toute trace.
        /// </para>
        /// </remarks>
        /// <param name="callChain">CallChain construite par <see cref="ExecuteAsync"/>.</param>
        /// <param name="callerFallback">
        /// CallChain d'origine (paramètre <c>caller</c> d'<see cref="ExecuteAsync"/>), utilisée
        /// comme valeur de repli lorsque l'exception typée ne porte pas de CallChain propre.
        /// </param>
        /// <param name="originalErrorMessage">
        /// Message principal déjà résolu par l'appelant via le dictionnaire ou rédigé en dur.
        /// </param>
        /// <param name="ex">Exception typée à normaliser.</param>
        /// <returns>Structure normalisée prête à être transmise aux canaux de persistance.</returns>
        private NormalizedError NormalizeException(
            string callChain,
            string callerFallback,
            string originalErrorMessage,
            Exception ex)
        {
            string localCallChain = $"{callChain} > {nameof(NormalizeException)}";

            try
            {
                string errorMessage = string.IsNullOrWhiteSpace(originalErrorMessage)
                    ? "Une erreur inattendue s'est produite."
                    : originalErrorMessage;

                return ex switch
                {
                    Ex_Business bex => new NormalizedError(
                        CallChain: string.IsNullOrWhiteSpace(bex.CallChain) ? callerFallback : bex.CallChain,
                        ErrorId: bex.ErrorId ?? "UnknownBusinessErrorId",
                        ErrorMessage: errorMessage,
                        ErrorDetails: BuildErrorDetails(bex.ErrorException, bex.InnerException)),

                    Ex_Infrastructure iex => new NormalizedError(
                        CallChain: string.IsNullOrWhiteSpace(iex.CallChain) ? callerFallback : iex.CallChain,
                        ErrorId: iex.ErrorId ?? "UnknownInfrastructureErrorId",
                        ErrorMessage: errorMessage,
                        ErrorDetails: BuildErrorDetails(iex.ErrorException, iex.InnerException)),

                    Ex_Unclassified uex => new NormalizedError(
                        CallChain: string.IsNullOrWhiteSpace(uex.CallChain) ? callerFallback : uex.CallChain,
                        ErrorId: uex.ErrorId ?? Ex_Unclassified.ErrorCodes.UN_ER_00,
                        ErrorMessage: errorMessage,
                        ErrorDetails: BuildErrorDetails(uex.ErrorException, uex.InnerException)),

                    _ => new NormalizedError(
                        CallChain: callerFallback,
                        ErrorId: "UnexpectedErrorId",
                        ErrorMessage: errorMessage,
                        ErrorDetails: BuildErrorDetails(ex.Message, ex.InnerException))
                };
            }
            catch (Exception inner)
            {
                return new NormalizedError(
                    CallChain: callerFallback,
                    ErrorId: "NormalizeErrorFailed",
                    ErrorMessage: string.IsNullOrWhiteSpace(originalErrorMessage)
                        ? "Erreur lors de la normalisation d'une exception."
                        : originalErrorMessage,
                    ErrorDetails: inner.Message ?? "UnknownException");
            }
        }

        /// <summary>
        /// Construit le détail technique en combinant le message de l'exception typée et
        /// la chaîne complète des exceptions imbriquées.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : méthode d'assemblage appelée depuis <see cref="NormalizeException"/>
        /// pour chaque type d'exception supporté.
        /// </para>
        /// <para>
        /// Objectif : fournir un point d'entrée nommé et cohérent pour la construction du
        /// détail, en séparant la responsabilité d'assemblage de celle de parcours itératif
        /// de la chaîne des exceptions imbriquées, déléguée à
        /// <see cref="BuildInnerExceptionChain"/>.
        /// </para>
        /// </remarks>
        /// <param name="errorException">
        /// Message technique porté par l'exception typée (<c>ErrorException</c>), utilisé
        /// comme premier niveau du détail.
        /// </param>
        /// <param name="innerException">
        /// Exception d'origine enchaînée dont la chaîne sera parcourue itérativement.
        /// </param>
        /// <returns>
        /// Détail technique complet, borné à <see cref="MaxErrorDetailsLength"/> caractères.
        /// </returns>
        private static string BuildErrorDetails(string? errorException, Exception? innerException)
        {
            return BuildInnerExceptionChain(errorException, innerException);
        }

        /// <summary>
        /// Reconstruit la chaîne complète des exceptions imbriquées jusqu'à la cause racine,
        /// bornée à <see cref="MaxErrorDetailsLength"/> caractères.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="BuildErrorDetails"/> pour chaque exception à
        /// journaliser. Parcourt itérativement la chaîne des <c>InnerException</c> afin de ne
        /// perdre aucune information technique exploitable (message SQL, code de contrainte,
        /// code réseau, etc.).
        /// </para>
        /// <para>
        /// Format produit : <c>message de l'exception typée | Cause : msg1 | Cause : msg2 | …</c>
        /// </para>
        /// <para>
        /// Troncature : la borne de <see cref="MaxErrorDetailsLength"/> caractères correspond
        /// à la taille de la colonne <c>ErrorException</c> dans la table
        /// <c>UserAppErrorLog</c>. Toute troncature est signalée par l'indicateur <c>…</c>,
        /// permettant de distinguer un détail complet d'un détail tronqué lors de l'analyse.
        /// </para>
        /// </remarks>
        /// <param name="baseDetail">
        /// Message technique porté directement par l'exception typée (<c>ErrorException</c>).
        /// Constitue le premier niveau du détail.
        /// </param>
        /// <param name="innerException">
        /// Exception imbriquée d'origine dont la chaîne complète sera parcourue.
        /// Peut être <see langword="null"/>.
        /// </param>
        /// <returns>
        /// Détail technique reconstruit, borné à <see cref="MaxErrorDetailsLength"/> caractères,
        /// ou <c>"UnknownException"</c> si aucune information n'est disponible.
        /// </returns>
        private static string BuildInnerExceptionChain(string? baseDetail, Exception? innerException)
        {
            var sb = new StringBuilder(baseDetail ?? string.Empty);

            Exception? current = innerException;
            while (current is not null)
            {
                if (!string.IsNullOrWhiteSpace(current.Message))
                {
                    if (sb.Length > 0)
                        sb.Append(" | Cause : ");

                    sb.Append(current.Message);
                }
                current = current.InnerException;
            }

            if (sb.Length == 0)
                return "UnknownException";

            if (sb.Length <= MaxErrorDetailsLength)
                return sb.ToString();

            return sb.ToString(0, MaxErrorDetailsLength) + "…";
        }

        /// <summary>
        /// Construit une ligne CSV à partir du contexte applicatif et de l'erreur normalisée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="ExecuteAsync"/> avant l'écriture fichier afin
        /// de produire un enregistrement structuré, portable et analysable.
        /// </para>
        /// <para>
        /// Objectif : assembler une ligne CSV correctement échappée et conforme à l'en-tête
        /// défini par <see cref="CsvHeader"/>, compatible avec les outils de traitement de
        /// données usuels.
        /// </para>
        /// </remarks>
        /// <param name="context">Contexte applicatif courant fourni par <c>IQ_AppContext</c>.</param>
        /// <param name="normalized">Erreur normalisée produite par <see cref="NormalizeException"/>.</param>
        /// <returns>Ligne CSV prête à être ajoutée au fichier de log.</returns>
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
                EscapeCsvValue(context.AppDeviceId   ?? string.Empty),
                EscapeCsvValue(context.AppDeviceIP   ?? string.Empty)
            });
        }

        /// <summary>
        /// Construit l'entité de persistance base de données à partir du contexte applicatif
        /// et de l'erreur normalisée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="ExecuteAsync"/> avant la tentative d'écriture
        /// dans la table <c>UserAppErrorLog</c>.
        /// </para>
        /// <para>
        /// Objectif : produire une entité <see cref="UserAppErrorLog"/> complète et cohérente,
        /// prête à être transmise à <see cref="IC_UserAppErrorLog.HandleAddAndSaveAsync"/>.
        /// </para>
        /// </remarks>
        /// <param name="context">Contexte applicatif courant.</param>
        /// <param name="normalized">Erreur normalisée.</param>
        /// <returns>Entité <see cref="UserAppErrorLog"/> prête à être persistée.</returns>
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

        /// <summary>
        /// Échappe une valeur pour une insertion sûre dans une ligne CSV.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : méthode utilitaire appelée par <see cref="BuildCsvLine"/> pour chaque
        /// champ de la ligne CSV.
        /// </para>
        /// <para>
        /// Objectif : protéger les valeurs contenant des guillemets, des retours à la ligne
        /// ou des caractères de contrôle, afin de garantir un format CSV lisible et stable,
        /// compatible avec les outils de traitement de données usuels.
        /// </para>
        /// </remarks>
        /// <param name="value">Valeur brute à échapper. Peut être <see langword="null"/>.</param>
        /// <returns>
        /// Valeur encadrée de guillemets avec guillemets internes doublés et retours à la
        /// ligne supprimés, ou chaîne vide si <paramref name="value"/> est <see langword="null"/>,
        /// vide ou composée uniquement d'espaces.
        /// </returns>
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

        /// <summary>
        /// Tente d'écrire une ligne dans le fichier CSV de journalisation en mode best effort.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="ExecuteAsync"/> après construction de la ligne
        /// CSV, en tant que premier canal de persistance (canal disque).
        /// </para>
        /// <para>
        /// Objectif : persister l'erreur sur disque de manière non bloquante. Toute
        /// défaillance d'écriture (disque plein, permissions, verrou) est absorbée
        /// silencieusement afin de ne pas interrompre le flux principal ni empêcher la
        /// tentative d'écriture sur le second canal (base de données).
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par <see cref="ExecuteAsync"/>.</param>
        /// <param name="csvLine">Ligne CSV à ajouter au fichier de log.</param>
        /// <param name="ct">Jeton d'annulation.</param>
        /// <returns>Tâche représentant l'exécution asynchrone de l'écriture fichier.</returns>
        private async Task TryWriteToFileAsync(string caller, string csvLine, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(TryWriteToFileAsync)}";

            try
            {
                await File.AppendAllTextAsync(_logPath, csvLine + Environment.NewLine, _utf8Bom, ct);
            }
            catch
            {
                // Best effort : ne pas interrompre le flux principal ni le second canal.
            }
        }

        /// <summary>
        /// Tente d'écrire l'erreur dans la base de données via
        /// <see cref="IC_UserAppErrorLog"/> en mode best effort.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="ExecuteAsync"/> après la tentative d'écriture
        /// fichier, en tant que second canal de persistance (canal base de données).
        /// </para>
        /// <para>
        /// Objectif : persister l'erreur en base de manière non bloquante.
        /// <see cref="IC_UserAppErrorLog.HandleAddAndSaveAsync"/> garantit une persistance
        /// autonome dans un DbContext indépendant de toute transaction UseCase en cours :
        /// l'enregistrement survit à tout rollback de la transaction principale.
        /// </para>
        /// <para>
        /// Toute défaillance de persistance est absorbée silencieusement afin de ne pas
        /// interrompre le flux principal. Le jeton d'annulation est contrôlé en entrée afin
        /// de respecter les demandes d'annulation coopérative légitimes avant l'appel réseau.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par <see cref="ExecuteAsync"/>.</param>
        /// <param name="entity">Entité <see cref="UserAppErrorLog"/> à persister.</param>
        /// <param name="ct">Jeton d'annulation.</param>
        /// <returns>Tâche représentant l'exécution asynchrone de l'écriture base.</returns>
        private async Task TryWriteToDatabaseAsync(string caller, UserAppErrorLog entity, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(TryWriteToDatabaseAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                await _chUserAppErrorLog.HandleAddAndSaveAsync(callChain, entity, ct);
            }
            catch
            {
                // Best effort : ne pas interrompre le flux principal.
            }
        }

        #endregion


        #region === Types privés ===

        /// <summary>
        /// Structure interne représentant une erreur normalisée, prête à être écrite dans le
        /// fichier CSV et dans la base de données.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Produite exclusivement par <see cref="NormalizeException"/> et consommée par
        /// <see cref="BuildCsvLine"/> et <see cref="BuildEntity"/>. Son caractère
        /// <see langword="sealed"/> garantit qu'elle ne peut pas être étendue hors de cette
        /// classe.
        /// </para>
        /// </remarks>
        /// <param name="CallChain">
        /// CallChain d'origine extraite de l'exception typée, ou valeur de repli si absente.
        /// </param>
        /// <param name="ErrorId">Identifiant normalisé de l'erreur.</param>
        /// <param name="ErrorMessage">
        /// Message principal résolu par l'appelant, destiné au log et à la notification.
        /// </param>
        /// <param name="ErrorDetails">
        /// Détail technique complet reconstruit depuis la chaîne des exceptions imbriquées,
        /// borné à <see cref="MaxErrorDetailsLength"/> caractères. La présence de l'indicateur
        /// <c>…</c> en fin de chaîne signale une troncature.
        /// </param>
        private sealed record NormalizedError(
            string CallChain,
            string ErrorId,
            string ErrorMessage,
            string ErrorDetails);

        #endregion
    }
}