using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Services.Business
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
    public interface IS_TemporRowTransformer
    {
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
        Tempor_Import Transform(IDictionary<string, string> temporRow);
    }
}
