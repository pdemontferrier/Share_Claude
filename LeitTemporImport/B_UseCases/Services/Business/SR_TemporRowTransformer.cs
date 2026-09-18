using System.Globalization;
using LeitTemporImport.A_Domain.Interfaces.Services.Business;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Services.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service métier chargé de transformer une ligne issue de la table MDB <c>Tempor</c>
    /// (représentée sous forme clé/valeur) en une entité <see cref="Tempor_Import"/> destinée
    /// à être persistée en base SQL.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé lors de l’import MDB → SQL Server 2019 (projet 104). La colonne <c>Feld_10</c>
    /// contient une chaîne segmentée en zones séparées par <c>|</c> et doit être découpée
    /// en 603 champs <c>Feld_10_001..Feld_10_603</c>, avec des règles spécifiques
    /// (zones ignorées, zone spéciale, normalisation décimale).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Produire une entité <see cref="Tempor_Import"/> fidèle aux règles métier existantes,
    /// sans générer d’exception en cas de données non convertibles (conversion silencieuse).
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Services d’import applicatifs (ex : import d’un fichier MDB).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Copier les champs natifs (hors <c>Feld_10</c>).</description></item>
    /// <item><description>Découper <c>Feld_10</c> en 603 zones.</description></item>
    /// <item><description>Appliquer les règles : zones ignorées, zone spéciale 262, normalisation décimale.</description></item>
    /// <item><description>Renseigner les propriétés <c>Feld_10_XXX</c> via reflection si elles existent.</description></item>
    /// </list>
    /// </summary>
    public class SR_TemporRowTransformer : IS_TemporRowTransformer
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        // A compléter (aucune dépendance pour le moment)

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service de transformation de ligne Tempor.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser la traçabilité.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// </list>
        /// </summary>
        public SR_TemporRowTransformer()
        {
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        // =========================================================
        // === CONSTANTES FELD_10 ==================================
        // =========================================================

        private const int TotalZones = 603;
        private const int SpecialEmptyZoneIndex = 261; // Feld_10_262

        private static readonly HashSet<int> IgnoredZoneIndexes = new()
        {
            24,   // Feld_10_025
            504,  // Feld_10_505
            505   // Feld_10_506
        };

        // Zones décimales (index 0-based)
        private static readonly HashSet<int> DecimalZoneIndexes = new()
        {
            9, 36, 37, 38, 50, 53, 54, 55, 67, 74, 75, 76, 77, 78, 79, 84,
            102, 103, 104, 105, 106, 107, 110, 111, 126, 127, 155, 156,
            165, 228, 229, 230, 231, 244, 245, 266, 359, 360, 388, 390,
            391, 392, 403, 479, 480, 481, 482, 483, 498, 499, 500, 538, 564
        };

        /// <summary>
        /// <para>Description</para>
        /// <para>Transforme une ligne Tempor (clé/valeur) en entité <see cref="Tempor_Import"/>.</para>
        /// <para>Contexte</para>
        /// <para>Appelée lors de l’import de la table MDB Tempor.</para>
        /// <para>Objectif</para>
        /// <para>Construire une entité persistable, avec découpage Feld_10 et conversions silencieuses.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Copier les champs natifs (hors Feld_10).</description></item>
        /// <item><description>Découper Feld_10 en zones et renseigner Feld_10_XXX.</description></item>
        /// </list>
        /// <param name="temporRow">Ligne Tempor sous forme clé/valeur.</param>
        /// <returns>Entité <see cref="Tempor_Import"/> remplie.</returns>
        /// </summary>
        public Tempor_Import Transform(IDictionary<string, string> temporRow)
        {
            // NOTE : service pur, pas de throw ici. callChain préparée pour cohérence projet.
            _ = $"{_callee} > {nameof(Transform)}";

            var entity = new Tempor_Import();

            // =====================================================
            // 1️⃣ Champs natifs de la table Tempor
            // =====================================================

            foreach (var kv in temporRow)
            {
                if (kv.Key.Equals("Feld_10", StringComparison.OrdinalIgnoreCase))
                    continue;

                SetPropertyIfExists(entity, kv.Key, kv.Value);
            }

            // =====================================================
            // 2️⃣ Découpage Feld_10
            // =====================================================

            temporRow.TryGetValue("Feld_10", out string? feld10Raw);
            var zones = (feld10Raw ?? string.Empty).Split('|');

            for (int i = 0; i < TotalZones; i++)
            {
                if (IgnoredZoneIndexes.Contains(i))
                    continue;

                string value = i < zones.Length
                    ? zones[i] ?? string.Empty
                    : string.Empty;

                // Règle spéciale zone 262
                if (i == SpecialEmptyZoneIndex && value.EndsWith(";"))
                    value = value.TrimEnd(';');

                // Normalisation décimale
                value = NormalizeDecimalIfNeeded(i, value);

                string propertyName = $"Feld_10_{(i + 1):D3}";
                SetPropertyIfExists(entity, propertyName, value);
            }

            return entity;
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Normalise une valeur décimale si la zone fait partie des zones décimales connues.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Les valeurs décimales sont saisies au format FR (virgule) dans certaines zones Feld_10.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Convertir au format invariant (point) pour stockage SQL cohérent.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Vérifier si la zone est décimale.</description></item>
        /// <item><description>TryParse en culture fr-FR puis ToString invariant.</description></item>
        /// </list>
        /// </summary>
        private static string NormalizeDecimalIfNeeded(int zoneIndex, string value)
        {
            if (!DecimalZoneIndexes.Contains(zoneIndex))
                return value;

            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (decimal.TryParse(
                value,
                NumberStyles.Number,
                CultureInfo.GetCultureInfo("fr-FR"),
                out decimal d))
            {
                return d.ToString(CultureInfo.InvariantCulture);
            }

            return value;
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Affecte une propriété sur <see cref="Tempor_Import"/> si elle existe, avec tentative de conversion typée.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Les colonnes MDB et les zones Feld_10 sont injectées via reflection selon le nom de propriété cible.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Garantir une transformation robuste : si la conversion échoue, la valeur est ignorée (null) sans exception.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Résoudre la propriété par nom.</description></item>
        /// <item><description>Gérer les valeurs vides en null.</description></item>
        /// <item><description>Tenter la conversion selon le type.</description></item>
        /// <item><description>En cas d’échec, affecter null silencieusement.</description></item>
        /// </list>
        /// </summary>
        private static void SetPropertyIfExists(Tempor_Import entity, string propertyName, string value)
        {
            var prop = typeof(Tempor_Import).GetProperty(propertyName);
            if (prop == null || !prop.CanWrite)
                return;

            // Gestion des valeurs vides
            if (string.IsNullOrWhiteSpace(value))
            {
                prop.SetValue(entity, null);
                return;
            }

            Type targetType = Nullable.GetUnderlyingType(prop.PropertyType)
                              ?? prop.PropertyType;

            try
            {
                object convertedValue;

                if (targetType == typeof(string))
                {
                    convertedValue = value;
                }
                else if (targetType == typeof(short))
                {
                    convertedValue = short.Parse(value, CultureInfo.InvariantCulture);
                }
                else if (targetType == typeof(int))
                {
                    convertedValue = int.Parse(value, CultureInfo.InvariantCulture);
                }
                else if (targetType == typeof(long))
                {
                    convertedValue = long.Parse(value, CultureInfo.InvariantCulture);
                }
                else if (targetType == typeof(decimal))
                {
                    convertedValue = decimal.Parse(value, CultureInfo.InvariantCulture);
                }
                else if (targetType == typeof(double))
                {
                    convertedValue = double.Parse(value, CultureInfo.InvariantCulture);
                }
                else if (targetType == typeof(DateTime))
                {
                    convertedValue = DateTime.Parse(
                        value,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind);
                }
                else if (targetType == typeof(bool))
                {
                    convertedValue = value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    // Fallback générique
                    convertedValue = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                }

                prop.SetValue(entity, convertedValue);
            }
            catch
            {
                // IMPORTANT :
                // On ignore silencieusement la conversion si elle échoue
                // (le UseCase décidera plus tard s’il faut journaliser)
                prop.SetValue(entity, null);
            }
        }

        #endregion
    }
}
