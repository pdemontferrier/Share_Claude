namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Generic
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat générique de QueryHandler (QH) conforme au modèle CQRS.
    /// Fournit des opérations de lecture génériques pour une entité <typeparamref name="T"/>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases et services applicatifs pour interroger la base via des repositories,
    /// en conservant une traçabilité (callChain) cohérente dans l’application.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Standardiser les lectures génériques (GetById, GetAll, etc.) en imposant le paramètre
    /// <paramref name="caller"/> comme point d’entrée de la traçabilité.
    /// </para>
    /// <typeparam name="T">Type d’entité EF Core.</typeparam>
    /// </summary>
    public interface IQ_Generic<T>
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne un enregistrement par son identifiant technique.</para>
        /// <para>Objectif</para>
        /// <para>Permettre la récupération ciblée d’une entité unique.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="id">Identifiant technique de l’entité.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>L’entité correspondante ou null.</returns>
        /// </summary>
        Task<T?> HandleGetByIdAsync(string caller, int id, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le premier enregistrement disponible.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>Premier enregistrement ou null.</returns>
        /// </summary>
        Task<T?> HandleGetFirstOrDefaultAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Vérifie si un enregistrement existe pour un identifiant donné.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="id">Identifiant technique.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>true si l’entité existe ; sinon false.</returns>
        /// </summary>
        Task<bool> HandleGetAnyAsync(string caller, int id, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne l’ensemble des enregistrements (tracking activé).</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>Liste complète des entités.</returns>
        /// </summary>
        Task<List<T>> HandleGetAllAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne l’ensemble des enregistrements sans tracking EF Core.</para>
        /// <para>Objectif</para>
        /// <para>Optimiser les lectures lorsque la modification des entités n’est pas requise.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>Liste complète des entités en lecture seule.</returns>
        /// </summary>
        Task<List<T>> HandleGetAllAsNoTrackingAsync(string caller, CancellationToken ct = default);
    }
}
