using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure;
using System.Text;

namespace LeitTemporImport.C_Infrastructure.Services
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service Infrastructure chargé de mettre en quarantaine un fichier MDB non traité dans un répertoire dédié
    /// (ImportFailed). La mise en quarantaine s’effectue selon une stratégie robuste :
    /// copie du fichier vers le répertoire cible, contrôle d’intégrité (existence + taille),
    /// puis suppression du fichier source.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les workflows d’import (batch/console) lorsque le traitement d’un fichier doit être interrompu
    /// (ex : SerieNr absent / invalide, série inexistante, import partiel avec erreurs, etc.).
    /// Le fichier ne doit pas rester dans le répertoire de scan afin d’éviter un retraitement en boucle
    /// à chaque exécution du batch.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Isoler les fichiers en échec dans un répertoire de quarantaine, avec un nom unique et traçable,
    /// tout en maximisant la robustesse face aux contraintes I/O (fichier verrouillé, antivirus, latence disque, etc.).
    /// En cas d’échec, produire un diagnostic enrichi (type d’exception, HResult, informations fichier/répertoire).
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Services et UseCases d’import du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Valider les entrées (<c>filePath</c>, <c>failedDir</c>, <c>reason</c>).</description></item>
    /// <item><description>Vérifier que <c>failedDir</c> référence bien un répertoire (et non un fichier).</description></item>
    /// <item><description>Créer le répertoire cible si nécessaire.</description></item>
    /// <item><description>Générer un chemin de destination unique (nom + reason + timestamp [+ GUID]).</description></item>
    /// <item><description>Copier le fichier (<c>File.Copy</c>) vers la destination.</description></item>
    /// <item><description>Contrôler la copie (existence + taille identique).</description></item>
    /// <item><description>Supprimer ensuite la source (<c>File.Delete</c>).</description></item>
    /// <item><description>En cas d’erreur I/O, enrichir le diagnostic et reclassifier en <see cref="Ex_Infrastructure"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_FileMoveToFailed : IS_FileMoveToFailed
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        // Aucune dépendance

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service de mise en quarantaine ImportFailed.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser la traçabilité (<c>_callee</c>).</para>
        /// </summary>
        public SR_FileMoveToFailed()
        {
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Met en quarantaine un fichier MDB dans le répertoire ImportFailed.
        /// La stratégie est volontairement robuste : Copy → contrôle d’intégrité → Delete,
        /// afin de réduire les échecs liés à un déplacement direct (fichier momentanément verrouillé, antivirus, etc.).
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Appelé lorsqu’un fichier ne doit pas rester dans le répertoire de scan, pour éviter le retraitement en boucle
        /// au prochain lancement du batch.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Garantir que le fichier est sécurisé dans la quarantaine avant toute tentative de suppression de la source.
        /// En cas d’échec I/O, fournir un diagnostic détaillé permettant d’identifier précisément la cause
        /// (HResult, attributs, existence, longueur, etc.).
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider <c>filePath</c> et <c>failedDir</c>.</description></item>
        /// <item><description>Normaliser/compléter <c>reason</c>.</description></item>
        /// <item><description>Créer le répertoire <c>failedDir</c>.</description></item>
        /// <item><description>Construire <c>destPath</c> (chemin de fichier unique dans <c>failedDir</c>).</description></item>
        /// <item><description>Copier le fichier source vers <c>destPath</c>.</description></item>
        /// <item><description>Vérifier la copie (existence + taille).</description></item>
        /// <item><description>Supprimer le fichier source uniquement après succès.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="filePath">Chemin complet du fichier source (fichier).</param>
        /// <param name="failedDir">Chemin complet du répertoire de quarantaine (répertoire).</param>
        /// <param name="reason">Jeton de raison (fonctionnel/technique) utilisé pour le nommage et la traçabilité.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <exception cref="Ex_Infrastructure">
        /// Levée en cas d’erreur I/O lors de la copie/suppression, avec diagnostic enrichi.
        /// </exception>
        /// </summary>
        public async Task ExecuteAsync(string caller, string filePath, string failedDir, string reason, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                // -------- Validations --------
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("filePath is required.", nameof(filePath));

                if (string.IsNullOrWhiteSpace(failedDir))
                    throw new ArgumentException("failedDir is required.", nameof(failedDir));

                if (string.IsNullOrWhiteSpace(reason))
                    reason = "UnknownReason";

                // Si le fichier n’existe plus : on ne bloque pas le batch.
                if (!File.Exists(filePath))
                    return;

                // S’assurer que failedDir est bien un répertoire, pas un fichier
                // (cas rare : failedDir pointe vers un path existant qui est un fichier)
                if (File.Exists(failedDir))
                    throw new IOException($"failedDir points to a file, not a directory. failedDir='{failedDir}'.");

                Directory.CreateDirectory(failedDir);

                // Construire un vrai chemin de destination de FICHIER
                string destPath = BuildUniqueDestinationPath(failedDir, filePath, reason);


                // Double sécurité : destPath doit être un fichier (pas juste le répertoire)
                // => au minimum, il doit contenir un nom de fichier.
                if (string.Equals(Path.GetFullPath(destPath).TrimEnd(Path.DirectorySeparatorChar),
                                  Path.GetFullPath(failedDir).TrimEnd(Path.DirectorySeparatorChar),
                                  StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException($"Destination path resolves to directory. destPath='{destPath}', failedDir='{failedDir}'.");
                }

                // -------- COPY FIRST --------
                File.Copy(filePath, destPath, overwrite: false);

                // Vérification de cohérence (sécurité supplémentaire)
                if (!File.Exists(destPath))
                    throw new IOException($"Copy failed: destination file not found. destPath='{destPath}'.");

                var srcInfo = new FileInfo(filePath);
                var destInfo = new FileInfo(destPath);

                if (srcInfo.Length != destInfo.Length)
                    throw new IOException(
                        $"Copy integrity check failed. SourceLength={srcInfo.Length}, DestLength={destInfo.Length}.");

                // -------- DELETE SOURCE --------
                File.Delete(filePath);

                // Double vérification
                if (File.Exists(filePath))
                    throw new IOException($"Source file still exists after delete attempt. filePath='{filePath}'.");

                await Task.CompletedTask;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is PathTooLongException || ex is NotSupportedException)
            {
                // Enrichir les erreurs IO/FS
                string details = BuildFileMoveDiagnostics(filePath, failedDir, reason, ex);
                throw Ex_Classifier.Execute(callChain, new Ex_Infrastructure(details, ex));
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit un chemin de destination unique (fichier) dans le répertoire ImportFailed.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour éviter les collisions lorsque plusieurs fichiers portent le même nom.</para>
        /// <para>Objectif</para>
        /// <para>Garantir un nom stable et traçable : <c>Nom__Reason__Timestamp[__Guid].ext</c>.</para>
        /// </summary>
        private static string BuildUniqueDestinationPath(string failedDir, string sourceFilePath, string reason)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string ext = Path.GetExtension(sourceFilePath);

            string safeReason = SanitizeToken(reason);
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            string candidate = Path.Combine(failedDir, $"{fileName}__{safeReason}__{stamp}{ext}");

            // Ultra-sécurité : si collision improbable, on ajoute un guid.
            if (File.Exists(candidate))
                candidate = Path.Combine(failedDir, $"{fileName}__{safeReason}__{stamp}__{Guid.NewGuid():N}{ext}");

            return candidate;
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Nettoie un jeton pour permettre son utilisation dans un nom de fichier.</para>
        /// <para>Objectif</para>
        /// <para>Garantir un nom compatible système en remplaçant les caractères non autorisés.</para>
        /// </summary>
        private static string SanitizeToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Unknown";

            var sb = new StringBuilder(value.Length);
            foreach (char c in value.Trim())
            {
                if (char.IsLetterOrDigit(c) || c is '-' or '_')
                    sb.Append(c);
                else
                    sb.Append('_');
            }
            return sb.ToString();
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit un diagnostic détaillé en cas d’échec de la mise en quarantaine.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour enrichir les erreurs I/O et faciliter l’analyse opérationnelle.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Produire un message exploitable comprenant : type d’exception, HResult, inner exception,
        /// état du fichier source (existence, taille, attributs), état du répertoire cible.
        /// </para>
        /// </summary>
        private static string BuildFileMoveDiagnostics(string filePath, string failedDir, string reason, Exception ex)
        {
            var sb = new StringBuilder();

            sb.AppendLine("File move to failed directory failed.");
            sb.AppendLine($"ReasonToken='{reason}'");
            sb.AppendLine($"SourceFile='{filePath}'");
            sb.AppendLine($"FailedDir='{failedDir}'");
            sb.AppendLine();

            // Exception info
            sb.AppendLine("Exception:");
            sb.AppendLine($"- Type={ex.GetType().FullName}");
            sb.AppendLine($"- HResult=0x{ex.HResult:X8}");
            sb.AppendLine($"- Message={ex.Message}");

            var inner = ex.InnerException;
            if (inner != null)
            {
                sb.AppendLine("InnerException:");
                sb.AppendLine($"- Type={inner.GetType().FullName}");
                sb.AppendLine($"- HResult=0x{inner.HResult:X8}");
                sb.AppendLine($"- Message={inner.Message}");
            }

            sb.AppendLine();
            sb.AppendLine("Environment checks:");

            // Source checks
            try
            {
                sb.AppendLine($"- SourceExists={File.Exists(filePath)}");
                if (File.Exists(filePath))
                {
                    var fi = new FileInfo(filePath);
                    sb.AppendLine($"- SourceLength={fi.Length}");
                    sb.AppendLine($"- SourceLastWriteUtc={fi.LastWriteTimeUtc:O}");
                    sb.AppendLine($"- SourceAttributes={fi.Attributes}");
                    sb.AppendLine($"- SourceDirectory='{fi.DirectoryName}'");
                }
            }
            catch (Exception e)
            {
                sb.AppendLine($"- SourceCheckFailed: {e.GetType().Name} / {e.Message}");
            }

            // Dest dir checks
            try
            {
                sb.AppendLine($"- FailedDirExists={Directory.Exists(failedDir)}");
                sb.AppendLine($"- FailedDirIsFile={File.Exists(failedDir)}");
                if (Directory.Exists(failedDir))
                {
                    var di = new DirectoryInfo(failedDir);
                    sb.AppendLine($"- FailedDirAttributes={di.Attributes}");
                    sb.AppendLine($"- FailedDirFullName='{di.FullName}'");
                }
            }
            catch (Exception e)
            {
                sb.AppendLine($"- FailedDirCheckFailed: {e.GetType().Name} / {e.Message}");
            }

            return sb.ToString();
        }

        #endregion
    }
}
