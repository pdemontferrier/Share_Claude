using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;

namespace LeitTemporImport.B_UseCases.Handlers.Generic
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// QueryHandler générique (QH) fournissant les opérations de lecture communes pour une entité
    /// <typeparamref name="T"/> via un repository générique.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les QueryHandlers typés (ex : QH_UserSession) qui héritent de cette classe afin de
    /// réutiliser les lectures génériques, tout en conservant une callChain homogène dans l’application.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les lectures génériques et appliquer systématiquement :
    /// </para>
    /// <list type="bullet">
    /// <item><description>La construction de la callChain à partir de <paramref name="caller"/>.</description></item>
    /// <item><description>La reclassification d’exceptions via <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// <typeparam name="T">Type d’entité EF Core.</typeparam>
    /// </summary>
    public class QH_Generic<T> : IQ_Generic<T> where T : class
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        protected readonly IR_Generic<T> _repository;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le QueryHandler générique <see cref="QH_Generic{T}"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser le repository générique et le nom interne <c>_callee</c>.</para>
        /// <param name="repository">Repository générique associé à <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="repository"/> est null.</exception>
        /// </summary>
        public QH_Generic(IR_Generic<T> repository)
        {
            _callee = GetType().Name;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion

        #region === Méthodes publiques ===

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
        public async Task<T?> HandleGetByIdAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByIdAsync)}";

            try
            {
                if (id <= 0)
                    throw new ArgumentOutOfRangeException(nameof(id), "id must be > 0.");

                // Repository actuel ne supporte pas ct : on respecte la signature mais on ne le propage pas.
                return await _repository.GetByIdAsync(callChain, id);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le premier enregistrement disponible.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>Premier enregistrement ou null.</returns>
        /// </summary>
        public async Task<T?> HandleGetFirstOrDefaultAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetFirstOrDefaultAsync)}";

            try
            {
                return await _repository.GetFirstOrDefaultAsync(callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Vérifie si un enregistrement existe pour un identifiant donné.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="id">Identifiant technique.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>true si l’entité existe ; sinon false.</returns>
        /// </summary>
        public async Task<bool> HandleGetAnyAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetAnyAsync)}";

            try
            {
                if (id <= 0)
                    throw new ArgumentOutOfRangeException(nameof(id), "id must be > 0.");

                return await _repository.GetAnyAsync(callChain, id);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne l’ensemble des enregistrements (tracking activé).</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>Liste complète des entités.</returns>
        /// </summary>
        public async Task<List<T>> HandleGetAllAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetAllAsync)}";

            try
            {
                return await _repository.GetAllAsync(callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne l’ensemble des enregistrements sans tracking EF Core.</para>
        /// <para>Objectif</para>
        /// <para>Optimiser les lectures lorsque la modification des entités n’est pas requise.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation optionnel.</param>
        /// <returns>Liste complète des entités en lecture seule.</returns>
        /// </summary>
        public async Task<List<T>> HandleGetAllAsNoTrackingAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetAllAsNoTrackingAsync)}";

            try
            {
                return await _repository.GetAllAsNoTrackingAsync(callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}