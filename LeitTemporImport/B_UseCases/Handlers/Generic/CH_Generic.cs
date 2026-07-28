using System.Reflection;
using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Handlers.Generic
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// CommandHandler générique fournissant les commandes CRUD standard pour toute entité <typeparamref name="T"/>,
    /// avec historisation automatique dans <see cref="UserAppEventStore"/> sous forme de snapshot JSON.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé comme classe de base des CommandHandlers spécialisés afin d’éviter la duplication
    /// des opérations standard (add/update/update-range/delete) et de la logique de journalisation.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser l’exécution des écritures via <see cref="IR_Generic{T}"/> et déclencher une journalisation
    /// systématique via <see cref="IC_UserAppEventStore"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>CommandHandlers de la couche UseCases.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Exécuter les commandes CRUD.</description></item>
    /// <item><description>Mettre à jour automatiquement <c>UpdatedAt</c> si la propriété existe.</description></item>
    /// <item><description>Proposer une commande de soft delete via <c>IsDeleted</c> si la propriété existe.</description></item>
    /// <item><description>Journaliser chaque écriture dans l’Event Store (snapshot JSON).</description></item>
    /// </list>
    /// </summary>
    public class CH_Generic<T> : IC_Generic<T> where T : class
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IR_Generic<T> _repositorySpecifique;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le CommandHandler générique <see cref="CH_Generic{T}"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI, généralement en tant que classe de base d’un CommandHandler spécialisé.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires à l’exécution des commandes et à la journalisation.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider les dépendances injectées.</description></item>
        /// </list>
        /// </summary>
        /// <param name="repository">Repository générique pour <typeparamref name="T"/>.</param>
        /// <param name="eventStore">CommandHandler d’écriture dans l’Event Store.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est null.</exception>
        public CH_Generic(IR_Generic<T> repository)
        {
            _callee = GetType().Name;
            _repositorySpecifique = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Ajoute un enregistrement <typeparamref name="T"/> et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>Commande générique utilisée par les handlers spécialisés.</para>
        /// <para>Objectif</para>
        /// <para>Persister l’entité puis écrire un snapshot dans l’Event Store.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Construire la callChain.</description></item>
        /// <item><description>Ajouter l’entité via repository.</description></item>
        /// <item><description>Journaliser le snapshot via Event Store.</description></item>
        /// </list>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="entity"> Entité <typeparamref name="T"/> à persister en base de données.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de l’écriture en base ou dans l’Event Store.</exception>
        /// </summary>
        public async Task HandleAddAsync(string caller, T entity, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleAddAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                await _repositorySpecifique.AddAsync(callChain, entity, logEventStore, ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour un enregistrement <typeparamref name="T"/> et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>Commande générique utilisée par les handlers spécialisés.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Mettre à jour l’entité, positionner <c>UpdatedAt</c> si disponible,
        /// puis écrire un snapshot dans l’Event Store.
        /// </para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="entity"> Entité <typeparamref name="T"/> à persister en base de données.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de l’écriture en base ou dans l’Event Store.</exception>
        /// </summary>
        public async Task HandleUpdateAsync(string caller, T entity, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleUpdateAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                SetUpdatedAtUtcNowIfExists(entity);
                await _repositorySpecifique.UpdateAsync(callChain, entity, logEventStore, ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour un ensemble d’enregistrements <typeparamref name="T"/> et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>Commande générique utilisée par les handlers spécialisés.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Positionner <c>UpdatedAt</c> si disponible, effectuer l’update range,
        /// puis écrire un snapshot pour chaque entité.
        /// </para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="entities"> Collection d’entités <typeparamref name="T"/> à persister en base de données.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entities"/> est null.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de l’écriture en base ou dans l’Event Store.</exception>
        /// </summary>
        public async Task HandleUpdateRangeAsync(string caller, IEnumerable<T> entities, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleUpdateRangeAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                foreach (var entity in entities)
                    SetUpdatedAtUtcNowIfExists(entity);
                await _repositorySpecifique.UpdateRangeAsync(callChain, entities, logEventStore, ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Supprime physiquement un enregistrement (hard delete) et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé uniquement si le modèle autorise la suppression physique.</para>
        /// <para>Objectif</para>
        /// <para>Récupérer l’entité, la supprimer, puis journaliser le snapshot supprimé.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="id">Identifiant technique de l’entité à supprimer.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de la suppression  en base ou dans l’Event Store.</exception>
        /// </summary>
        public async Task HandleHardDeleteAsync(string caller, int id, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleHardDeleteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                var entity = await _repositorySpecifique.GetByIdAsync(callChain,id, ct);
                if (entity != null)
                {
                    await _repositorySpecifique.DeleteAsync(callChain, id, logEventStore, ct);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Réalise un soft delete (si supporté) et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé lorsque l’entité supporte la suppression logique via la propriété <c>IsDeleted</c>.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Charger l’entité, positionner <c>IsDeleted = true</c> (et <c>UpdatedAt</c> si disponible),
        /// puis persister et journaliser un snapshot.
        /// </para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="id">Identifiant technique de l’entité à supprimer.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de l’écriture en base ou dans l’Event Store.</exception>
        /// </summary>
        public async Task HandleSoftDeleteAsync(string caller, int id, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleSoftDeleteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                var entity = await _repositorySpecifique.GetByIdAsync(callChain, id, ct);
                if (entity is null)
                    throw new Ex_Business($"Entity not found for Id={id}.");

                if (!SetIsDeletedTrueIfExists(entity))
                    throw new Ex_Business($"Entity type '{typeof(T).Name}' does not support soft delete (IsDeleted property missing).");

                SetUpdatedAtUtcNowIfExists(entity);

                await _repositorySpecifique.UpdateAsync(callChain, entity, logEventStore, ct);
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
        /// <para>Positionne dynamiquement la propriété <c>UpdatedAt</c> à la date UTC courante
        /// si celle-ci existe sur le type <typeparamref name="T"/>.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée lors des opérations de mise à jour ou de suppression logique afin
        /// de garantir la cohérence des métadonnées temporelles.</para>
        /// <para>Objectif</para>
        /// <para>Mettre à jour automatiquement l’horodatage sans imposer une contrainte
        /// d’héritage ou d’interface spécifique aux entités.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Rechercher la propriété publique <c>UpdatedAt</c>.</description></item>
        /// <item><description>Vérifier qu’elle est modifiable.</description></item>
        /// <item><description>Positionner la valeur à <c>DateTime.UtcNow</c>.</description></item>
        /// </list>
        /// <param name="entity">Instance de l’entité à modifier. </param>
        /// </summary>
        private static void SetUpdatedAtUtcNowIfExists(T entity)
        {
            var prop = typeof(T).GetProperty("UpdatedAt", BindingFlags.Public | BindingFlags.Instance);
            if (prop is null || !prop.CanWrite) return;

            if (prop.PropertyType == typeof(DateTime))
                prop.SetValue(entity, DateTime.UtcNow);
            else if (prop.PropertyType == typeof(DateTime?))
                prop.SetValue(entity, (DateTime?)DateTime.UtcNow);
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Positionne dynamiquement la propriété <c>IsDeleted</c> à <c>true</c>
        /// si celle-ci existe sur le type <typeparamref name="T"/>.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée dans le cadre d’une suppression logique (soft delete)
        /// lorsque l’entité supporte ce mécanisme.</para>
        /// <para>Objectif</para>
        /// <para>Permettre une suppression logique générique sans imposer
        /// d’interface métier spécifique.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Rechercher la propriété publique <c>IsDeleted</c>.</description></item>
        /// <item><description>Vérifier qu’elle est modifiable.</description></item>
        /// <item><description>Positionner la valeur à <c>true</c>.</description></item>
        /// </list>
        /// <param name="entity">Instance de l’entité à modifier.</param>
        /// <returns>
        /// <c>true</c> si la propriété existe et a été positionnée;
        /// <c>false</c> sinon.
        /// </returns>
        /// </summary>
        private static bool SetIsDeletedTrueIfExists(T entity)
        {
            var prop = typeof(T).GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance);
            if (prop is null || !prop.CanWrite) return false;

            if (prop.PropertyType == typeof(bool))
            {
                prop.SetValue(entity, true);
                return true;
            }

            if (prop.PropertyType == typeof(bool?))
            {
                prop.SetValue(entity, (bool?)true);
                return true;
            }

            return false;
        }

        #endregion
    }
}