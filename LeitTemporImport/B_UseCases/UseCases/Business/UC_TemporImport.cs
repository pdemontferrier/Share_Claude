using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.Services.User;
using LeitTemporImport.A_Domain.Interfaces.Settings.App;
using LeitTemporImport.A_Domain.Interfaces.Settings.Business;
using LeitTemporImport.A_Domain.Interfaces.Settings.User;
using LeitTemporImport.A_Domain.Interfaces.UseCases.Business;

namespace LeitTemporImport.B_UseCases.UseCases.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// UseCase orchestrateur chargé de parcourir un répertoire de travail contenant des fichiers MDB
    /// (ex: Leitxxxx.mdb) et de déclencher le traitement métier pour chaque fichier via
    /// <c>IU_TemporImport_ProcessFile</c>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Exécuté en mode batch (console) à intervalles réguliers (ex : toutes les 15 minutes) afin de
    /// traiter les nouveaux fichiers présents dans le répertoire configuré.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Garantir un traitement robuste : un fichier en erreur ne doit pas empêcher le traitement des suivants.
    /// Les erreurs sont journalisées via <c>IS_ErrorLogger</c>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Traitements batch / console d’import MDB → SQL Server 2019 (projet 104).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Scanner le répertoire pour trouver les fichiers MDB correspondants.</description></item>
    /// <item><description>Ordonner la liste pour un traitement déterministe.</description></item>
    /// <item><description>Traiter chaque fichier via <c>UC_TemporImport_ProcessFile</c>.</description></item>
    /// <item><description>Logger les erreurs et poursuivre sur le fichier suivant.</description></item>
    /// </list>
    /// </summary>
    public class UC_TemporImport : IU_TemporImport
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du UseCase pour la traçabilité.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_Business _business;
        private readonly IS_FileScanner _fileScanner;
        private readonly IU_TemporImport_ProcessFile _processFileUseCase;
        private readonly IS_ErrorLogger _errorLog;
        private readonly IS_UserSession_Open _userSessionOpen;
        private readonly IS_UserSession_Close _userSessionClose;
        private readonly ISE_User _settingsUser;
        private readonly ISE_App _settingsApp;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le UseCase d’orchestration du traitement des fichiers MDB.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires au scan des fichiers et au traitement unitaire.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider les dépendances.</description></item>
        /// </list>
        /// <param name="business">Paramètres métier (répertoire, filtre nommage…).</param>
        /// <param name="fileScanner">Service infrastructure de scan de fichiers MDB.</param>
        /// <param name="processFileUseCase">UseCase de traitement unitaire d’un fichier MDB.</param>
        /// <param name="errorLog">Service de journalisation des erreurs.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est nulle.</exception>
        /// </summary>
        public UC_TemporImport(
            ISE_Business business,
            IS_FileScanner fileScanner,
            IU_TemporImport_ProcessFile processFileUseCase,
            IS_ErrorLogger errorLog,
            IS_UserSession_Open userSessionOpen,
            IS_UserSession_Close userSessionClose,
            ISE_User settingsUser,
            ISE_App settingsApp)
        {
            _callee = GetType().Name;

            _business = business ?? throw new ArgumentNullException(nameof(business));
            _fileScanner = fileScanner ?? throw new ArgumentNullException(nameof(fileScanner));
            _processFileUseCase = processFileUseCase ?? throw new ArgumentNullException(nameof(processFileUseCase));
            _errorLog = errorLog ?? throw new ArgumentNullException(nameof(errorLog));
            _userSessionOpen = userSessionOpen ?? throw new ArgumentNullException(nameof(userSessionOpen));
            _userSessionClose = userSessionClose ?? throw new ArgumentNullException(nameof(userSessionClose));
            _settingsUser = settingsUser ?? throw new ArgumentNullException(nameof(settingsUser));
            _settingsApp = settingsApp ?? throw new ArgumentNullException(nameof(settingsApp));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute l’orchestration batch de traitement de tous les fichiers MDB détectés.</para>
        /// <para>Contexte</para>
        /// <para>Déclenché périodiquement par l’application console.</para>
        /// <para>Objectif</para>
        /// <para>Traiter chaque fichier de manière indépendante : erreur sur un fichier ⇒ log ⇒ continuer.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Scanner le répertoire de travail.</description></item>
        /// <item><description>Traiter les fichiers un par un via <c>IU_TemporImport_ProcessFile</c>.</description></item>
        /// <item><description>Logger les exceptions sans interrompre la boucle.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        public async Task ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            int appId = _settingsApp.GetAppId();
            int userId = _settingsUser.GetAppUserId();

            try
            {
                ct.ThrowIfCancellationRequested();

                // =========================
                // 1) Ouvrir la session
                // =========================
                await _userSessionOpen.ExecuteAsync(callChain, userId, appId);

                // =========================
                // 2) Traitement des fichiers
                // =========================
                var files = _fileScanner.GetMdbFiles(
                    _business.DataDirectoryPath,
                    _business.MdbFilePrefix,
                    _business.MdbFileExtension);

                // Traitement déterministe : ordre alpha complet (FullName)
                var orderedFiles = files
                    .OrderBy(f => f.FullName, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var file in orderedFiles)
                {
                    ct.ThrowIfCancellationRequested();

                    string filePath = file.FullName;
                    string failedDir = _business.ImportFailedDirectoryPath;

                    try
                    {
                        // Traitement unitaire (toute la logique métier est dedans)
                        await _processFileUseCase.ExecuteAsync(callChain, filePath, failedDir, ct);
                    }
                    catch (Exception ex)
                    {
                        await _errorLog.ExecuteAsync(callChain, ex, ct);
                        // Continue sur le fichier suivant
                    }
                }
            }
            catch (Exception ex)
            {
                // Erreur au niveau orchestration globale (scan répertoire, etc.)
                await _errorLog.ExecuteAsync(callChain, ex, ct);
                throw;
            }
            finally
            {
                // =========================
                // 3) Fermer la session
                // =========================
                try
                {
                    int sessionId = _settingsUser.GetAppSessionId();

                    // On ne tente la fermeture que si on a une session valide.
                    if (userId > 0 && sessionId > 0)
                        await _userSessionClose.ExecuteAsync(callChain, userId, sessionId);
                }
                catch (Exception exClose)
                {
                    // On log l’erreur de fermeture, sans masquer une éventuelle exception déjà levée.
                    await _errorLog.ExecuteAsync(callChain, exClose, ct);
                }
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}
