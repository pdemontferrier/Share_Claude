namespace LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat définissant les opérations de lecture d’un fichier MDB (Microsoft Access)
    /// dans le cadre des traitements d’import du projet 104.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases d’import pour extraire des données depuis la table Tempor
    /// ou toute autre table présente dans le fichier MDB.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir une API d’accès aux données MDB indépendante de la technologie
    /// (OleDb, provider, etc.), tout en garantissant la traçabilité via la CallChain.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases et Services de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Lire le champ SerieNr depuis la table Tempor.</description></item>
    /// <item><description>Lire la liste des colonnes d’une table MDB.</description></item>
    /// <item><description>Lire toutes les lignes d’une table MDB sous forme clé/valeur.</description></item>
    /// </list>
    /// </summary>
    public interface IS_MdbReader
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Lit la valeur du champ <c>SerieNr</c> depuis la table Tempor
        /// sur le premier enregistrement du fichier MDB.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé par le UseCase UC_TemporImport_ProcessFile pour déterminer
        /// l’identifiant de série associé au fichier.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Extraire la valeur brute de SerieNr afin de permettre sa conversion
        /// et son traitement métier (int attendu).
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Ouvrir une connexion MDB.</description></item>
        /// <item><description>Exécuter une requête SELECT TOP 1.</description></item>
        /// <item><description>Retourner la valeur sous forme de chaîne trimée.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="filePath">Chemin complet du fichier MDB.</param>
        /// <returns>Valeur brute de SerieNr sous forme de chaîne (trim).</returns>
        /// </summary>
        Task<string> ReadSerieNrAsync(string caller, string filePath, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne la liste des colonnes d’une table MDB.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Peut être utilisé pour des traitements dynamiques
        /// ou des validations structurelles.
        /// </para>
        /// <para>Objectif</para>
        /// <para>Permettre l’analyse structurelle d’une table MDB.</para>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="filePath">Chemin complet du fichier MDB.</param>
        /// <param name="tableName">Nom de la table à analyser.</param>
        /// <returns>Liste des noms de colonnes.</returns>
        IReadOnlyList<string> ReadTableColumns(string caller, string filePath, string tableName);

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Lit toutes les lignes d’une table MDB sous forme de dictionnaires
        /// (clé = nom colonne, valeur = chaîne normalisée InvariantCulture).
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé lors de l’import complet de la table Tempor
        /// avant transformation métier.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir une représentation générique et indépendante du modèle
        /// pour traitement ultérieur par un transformer métier.
        /// </para>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="filePath">Chemin complet du fichier MDB.</param>
        /// <param name="tableName">Nom de la table à lire.</param>
        /// <returns>
        /// Liste en mémoire des lignes sous forme clé/valeur.
        /// </returns>
        IEnumerable<IDictionary<string, string>> StreamTableRows(string caller, string filePath, string tableName);

        #endregion
    }
}
