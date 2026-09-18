using LeitTemporImport.A_Domain.Common.Exceptions;
using System.Linq.Expressions;

namespace LeitTemporImport.A_Domain.Interfaces.Repositories.Generic
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Repository Infrastructure générique fournissant les opérations CRUD standard
    /// pour toute entité <typeparamref name="T"/> via Entity Framework Core.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les CommandHandlers et QueryHandlers génériques/spécifiques afin de centraliser
    /// l’accès aux tables MS SQL Server (projet 104). Chaque méthode instancie son propre DbContext
    /// via <see cref="IDbContextFactory{TContext}"/>, ce qui rend les appels isolés et adaptés
    /// aux traitements console/batch.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Garantir un accès robuste, thread-safe et sans état global, en encapsulant :
    /// </para>
    /// <list type="bullet">
    /// <item><description>La création contrôlée des DbContext.</description></item>
    /// <item><description>Les opérations CRUD standard.</description></item>
    /// <item><description>La traçabilité via <c>callChain</c> et la reclassification via <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// <para>Utilisateurs cibles</para>
    /// <para>Handlers / Services de la couche UseCases et Infrastructure.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Créer un DbContext à la demande.</description></item>
    /// <item><description>Exécuter l’opération EF Core correspondante.</description></item>
    /// <item><description>Persister via <c>SaveChangesAsync</c> pour les écritures.</description></item>
    /// <item><description>Reclassifier toute exception via <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// <typeparam name="T">Type d’entité EF Core (table) manipulé par le repository.</typeparam>
    /// </summary>
    public interface IR_Generic<T>
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Ajoute une entité <typeparamref name="T"/> en base et persiste immédiatement.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé par les CommandHandlers lors des insertions.</para>
        /// <para>Objectif</para>
        /// <para>Insérer la ligne correspondante et exécuter <c>SaveChangesAsync</c>.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Créer un DbContext.</description></item>
        /// <item><description>Ajouter l’entité via <c>DbSet.AddAsync</c>.</description></item>
        /// <item><description>Persister via <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="entity">Entité à ajouter.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// </summary>
        Task AddAsync(string caller, T entity, bool logEventStore = true, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne une entité <typeparamref name="T"/> par son identifiant technique.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé par les handlers pour charger une entité avant suppression / modification.</para>
        /// <para>Objectif</para>
        /// <para>Effectuer un <c>FindAsync</c> sur la clé primaire.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="id">Identifiant technique.</param>
        /// <returns>Entité trouvée ou null.</returns>
        /// </summary>
        Task<T?> GetByIdAsync(string caller, int id, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le premier enregistrement <typeparamref name="T"/> ou null.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <returns>Première entité ou null.</returns>
        /// </summary>
        Task<T?> GetFirstOrDefaultAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Indique si un enregistrement <typeparamref name="T"/> existe pour un identifiant donné.</para>
        /// <param name="id">Identifiant technique.</param>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <returns><c>true</c> si l’entité existe, sinon <c>false</c>.</returns>
        /// </summary>
        Task<bool> GetAnyAsync(string caller, int id, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne tous les enregistrements de <typeparamref name="T"/>.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <returns>Liste complète.</returns>
        /// </summary>
        Task<List<T>> GetAllAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne tous les enregistrements de <typeparamref name="T"/> en lecture seule (<c>AsNoTracking</c>).</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <returns>Liste complète sans tracking EF.</returns>
        /// </summary>
        Task<List<T>> GetAllAsNoTrackingAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne les enregistrements de <typeparamref name="T"/> correspondant à un prédicat.</para>
        /// <param name="predicate">Expression de filtrage.</param>
        /// <returns>Liste filtrée.</returns>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="predicate"/> est null.</exception>
        /// </summary>
        Task<List<T>> GetFilteredAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour une entité <typeparamref name="T"/> et persiste immédiatement.</para>
        /// <param name="entity">Entité à mettre à jour.</param>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// </summary>
        Task UpdateAsync(string caller, T entity, bool logEventStore = true, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour un ensemble d’entités <typeparamref name="T"/> et persiste immédiatement.</para>
        /// <param name="entities">Ensemble d’entités à mettre à jour.</param>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entities"/> est null.</exception>
        /// </summary>
        Task UpdateRangeAsync(string caller, IEnumerable<T> entities, bool logEventStore = true, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Supprime physiquement une entité <typeparamref name="T"/> par identifiant.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="id">Identifiant technique.</param>
        /// </summary>
        Task DeleteAsync(string caller, int id, bool logEventStore = true, CancellationToken ct = default);
    }
}