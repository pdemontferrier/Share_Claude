using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure;
using System.Data.OleDb;
using System.Globalization;

namespace LeitTemporImport.C_Infrastructure.Services
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service Infrastructure de lecture des fichiers MDB via OleDb.
    /// Fournit des opérations de lecture ciblées (ex : lecture du SerieNr depuis Tempor),
    /// avec traçabilité (CallChain) et reclassification d’exceptions (Ex_Classifier).
    /// </para>
    /// <para>Contexte</para>
    /// <para>Utilisé par les UseCases d’import pour extraire des informations de fichiers Access.</para>
    /// <para>Objectif</para>
    /// <para>Lire de manière robuste et reproductible des données MDB indépendamment des UseCases.</para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Services et UseCases de la couche Application.</para>
    /// </summary>
    public class SR_MdbReader : IS_MdbReader
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_ErrorLogger _errorLog;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service de lecture MDB.</para>
        /// </summary>
        public SR_MdbReader(IS_ErrorLogger errorLog)
        {
            _callee = GetType().Name;
            _errorLog = errorLog ?? throw new ArgumentNullException(nameof(errorLog));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Lit le champ SerieNr depuis la table Tempor sur le premier enregistrement.</para>
        /// <para>Objectif</para>
        /// <para>Permettre au UseCase de récupérer l’identifiant de série associé au fichier MDB.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="filePath">Chemin complet du fichier MDB.</param>
        /// <returns>Valeur brute de SerieNr sous forme de chaîne (trim).</returns>
        /// </summary>
        public async Task<string> ReadSerieNrAsync(string caller, string filePath,  CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ReadSerieNrAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                using var cnx = OpenConnection(callChain, filePath);
                using var cmd = new OleDbCommand("SELECT TOP 1 [SerieNr] FROM [Tempor];", cnx);

                object? scalar = cmd.ExecuteScalar();

                if (scalar is null || scalar == DBNull.Value)
                    return await ReturnZeroAndLogAndMoveAsync(callChain, $"SerieNr not found in MDB. File='{filePath}'.",
                        ct);

                string raw = (scalar.ToString() ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(raw))
                    return await ReturnZeroAndLogAndMoveAsync(callChain, $"SerieNr is empty. File='{filePath}'.",
                        ct);

                if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int serieNr))
                    return await ReturnZeroAndLogAndMoveAsync(callChain, $"SerieNr is not a valid integer ('{raw}'). File='{filePath}'.",
                        ct);

                if (serieNr <= 0)
                    return await ReturnZeroAndLogAndMoveAsync(callChain, $"SerieNr must be > 0 (value='{raw}'). File='{filePath}'.",
                        ct);

                return raw;
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);

            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne la liste des colonnes d’une table MDB.</para>
        /// </summary>
        public IReadOnlyList<string> ReadTableColumns(string caller, string mdbPath, string tableName)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ReadTableColumns)}";

            try
            {
                using var cnx = OpenConnection(callChain, mdbPath);
                using var cmd = new OleDbCommand($"SELECT * FROM [{tableName}]", cnx);
                using var reader = cmd.ExecuteReader();

                var columns = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                    columns.Add(reader.GetName(i));

                return columns;
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Lit toutes les lignes d’une table MDB sous forme de dictionnaires (colonne → valeur).</para>
        /// </summary>
        public IEnumerable<IDictionary<string, string>> StreamTableRows(string caller, string mdbPath, string tableName)
        {
            string callChain = $"{caller} > {_callee} > {nameof(StreamTableRows)}";

            OleDbConnection? cnx = null;
            OleDbCommand? cmd = null;
            OleDbDataReader? reader = null;

            try
            {
                cnx = OpenConnection(callChain, mdbPath);
                cmd = new OleDbCommand($"SELECT * FROM [{tableName}]", cnx);
                reader = cmd.ExecuteReader();

                if (reader == null)
                    yield break;

                while (reader.Read())
                {
                    var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    for (int i = 0; i < reader.FieldCount; i++)
                        row[reader.GetName(i)] = ToInvariantString(reader.GetValue(i));

                    yield return row;
                }
            }
            finally
            {
                reader?.Dispose();
                cmd?.Dispose();
                cnx?.Dispose();
            }
        }

        #endregion

        #region === Méthodes privées ===

        private OleDbConnection OpenConnection(string caller, string mdbPath)
        {
            string callChain = $"{caller} > {nameof(OpenConnection)}";

            try
            {
                if (string.IsNullOrWhiteSpace(mdbPath))
                    throw new ArgumentException("filePath is required.", nameof(mdbPath));

                var cnx = new OleDbConnection(
                    $@"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={mdbPath};Persist Security Info=False;");

                cnx.Open();
                return cnx;
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        private static string ToInvariantString(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;

            return value switch
            {
                DateTime d => d.ToString("o", CultureInfo.InvariantCulture),
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString() ?? string.Empty
            };
        }

        private async Task<string> ReturnZeroAndLogAndMoveAsync(string caller, string message, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ReturnZeroAndLogAndMoveAsync)}";

            // 1) Logger l’anomalie
            Exception iex = new Ex_Infrastructure(message);
            await _errorLog.ExecuteAsync(callChain, iex, ct);

            // 2) Politique : retour "0" pour permettre au batch de continuer
            return "0";
        }

        #endregion
    }
}
