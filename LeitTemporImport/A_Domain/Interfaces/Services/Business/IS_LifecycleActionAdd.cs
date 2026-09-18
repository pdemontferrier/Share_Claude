using LeitTemporImport.A_Domain.Common.Exceptions;

namespace LeitTemporImport.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du service métier chargé d’ajouter des enregistrements dans la table <c>LifecycleAction</c>
    /// afin d’historiser des événements structurés du cycle de vie (type, source, identifiant source, commentaire),
    /// enrichis par le contexte applicatif (application, utilisateur, poste, horodatage).
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases batch/console pour tracer les étapes clés du traitement
    /// (ex : une série a été importée depuis un fichier MDB).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser la sémantique métier des écritures dans <c>LifecycleAction</c>
    /// afin d’éviter la duplication de logique dans les UseCases et de conserver des Handlers génériques.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases / Services de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Définir des méthodes publiques dédiées à chaque événement métier à tracer.</description></item>
    /// <item><description>Appliquer les règles de validation et de formatage (ex : longueur commentaire).</description></item>
    /// <item><description>Assurer la traçabilité via la callChain transmise.</description></item>
    /// </list>
    /// </summary>
    public interface IS_LifecycleActionAdd
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Ajoute une action indiquant qu’une série de production a été importée.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Appelé en fin d’import valide pour historiser l’état “imported” de la série.
        /// Le service retrouve l’identifiant technique de la série (<c>ProductionSeries.Id</c>) à partir
        /// du numéro de série métier (<paramref name="serialNumberId"/>).
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Enregistrer une ligne structurée dans <c>LifecycleAction</c> avec :
        /// </para>
        /// <list type="bullet">
        /// <item><description>Type d’action : Imported (valeur référentielle).</description></item>
        /// <item><description>Source : ProductionSeries (valeur référentielle).</description></item>
        /// <item><description>IdSource : <c>ProductionSeries.Id</c> (identifiant technique de la série).</description></item>
        /// <item><description>Commentaire explicite incluant le numéro de série et le nom du fichier MDB.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="serialNumberId">Numéro de série métier (ex : 6 chiffres) utilisé pour retrouver la série et pour le commentaire.</param>
        /// <param name="filePath">Chemin complet du fichier MDB source (utilisé pour extraire le nom de fichier).</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="serialNumberId"/> est invalide.</exception>
        /// <exception cref="ArgumentNullException">Si <paramref name="filePath"/> est null ou vide.</exception>
        /// <exception cref="Ex_Business">Si la série n’existe pas en base pour le numéro fourni.</exception>
        Task ExecuteProductionSeriesImportedAsync(string caller, int serialNumberId, string filePath, CancellationToken ct = default);
    }
}