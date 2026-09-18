namespace LeitTemporImport.A_Domain.Interfaces.Settings.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat des paramètres et du contexte métier global liés au traitement d’import MDB → SQL Server.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases et Services du projet afin de localiser les fichiers à traiter
    /// et de partager certaines informations d’exécution (ex : SerialNumberId).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les règles de sélection des fichiers MDB et maintenir l’identifiant
    /// de série courant pendant l’exécution du processus d’import.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases et Services applicatifs du projet.</para>
    /// </summary>
    public interface ISE_Business
    {
        #region === Propriétés publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Chemin absolu du répertoire contenant les fichiers MDB à importer ou à supprimer.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Ce répertoire est scanné par le service de détection de fichiers afin d’identifier
        /// les fichiers conformes aux règles métier (préfixe + extension).
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Définir l’emplacement unique de stockage des fichiers temporaires issus du système source.
        /// </para>
        /// </summary>
        string DataDirectoryPath { get; }


        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Chemin absolu du répertoire de destination des fichiers MDB dont l’importation
        /// a échoué ou dont le numéro de série n’a pas pu être identifié.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Lorsqu’un fichier détecté dans <c>DataDirectoryPath</c> ne peut pas être traité
        /// (ex : SerieNr absent, invalide ou série inexistante en base), il ne doit pas
        /// rester dans le répertoire principal afin d’éviter un retraitement en boucle.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Isoler les fichiers en échec d’import dans un répertoire dédié afin de :
        /// </para>
        /// <list type="bullet">
        /// <item><description>Prévenir les tentatives répétées d’importation.</description></item>
        /// <item><description>Faciliter l’analyse manuelle des anomalies.</description></item>
        /// <item><description>Maintenir la propreté opérationnelle du répertoire principal.</description></item>
        /// </list>
        /// </summary>
        string ImportFailedDirectoryPath { get; }

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Préfixe obligatoire des fichiers MDB à traiter (ex : "Leit").
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Seuls les fichiers dont le nom commence par ce préfixe sont considérés
        /// comme valides pour l’import ou la suppression.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Sécuriser le traitement en excluant tout fichier ne respectant pas la convention
        /// de nommage métier définie par le système source.
        /// </para>
        /// </summary>
        string MdbFilePrefix { get; }

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Extension obligatoire des fichiers à traiter (ex : ".mdb").
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Seuls les fichiers correspondant à cette extension sont considérés
        /// comme éligibles au traitement d’import.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Garantir que seuls les fichiers de type Microsoft Access MDB
        /// sont pris en compte dans le pipeline d’import.
        /// </para>
        /// </summary>
        string MdbFileExtension { get; }

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Identifiant métier du numéro de série extrait du fichier MDB en cours de traitement.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Cette valeur est déterminée à partir du champ <c>SerieNr</c> de la table <c>Tempor</c>
        /// et persistée dans le contexte global de l’application pendant l’exécution.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Permettre la réutilisation de l’identifiant de série dans les UseCases suivants
        /// (contrôle d’état d’import, procédures stockées, journalisation).
        /// </para>
        /// </summary>
        int SerialNumberId { get; set; }

        #endregion
    }
}