using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;
using LeitTemporImport.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LeitTemporImport.C_Infrastructure.Repositories.DIGIT_TRY
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Repository concret (CR) spécialisé pour l’entité <see cref="UserAppSession"/>.
    /// Hérite de <see cref="CR_Generic{T}"/> afin de fournir les opérations CRUD génériques,
    /// et expose des requêtes spécifiques liées aux sessions applicatives.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases/Handlers d’ouverture et fermeture du programme afin de retrouver
    /// les sessions existantes d’un utilisateur pour une application donnée.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les accès EF Core spécifiques à <see cref="UserAppSession"/> en appliquant :
    /// la traçabilité (callChain), des lectures optimisées (AsNoTracking) et la reclassification
    /// des exceptions via <see cref="Ex_Classifier"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Couche Infrastructure : QueryHandlers / Services / UseCases via interfaces.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Fournir un repository concret pour <see cref="UserAppSession"/>.</description></item>
    /// <item><description>Rechercher les sessions par <c>IdUser</c> et <c>IdApplication</c>.</description></item>
    /// <item><description>Appliquer les filtres standard (soft delete) et les optimisations de lecture.</description></item>
    /// </list>
    /// </summary>
    public class CR_UserSession : CR_Generic<UserAppSession>, IR_UserSession
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        // Héritées via CR_Generic : _contextFactory

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le repository concret <see cref="CR_UserSession"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche Infrastructure.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Initialiser l’accès au contexte EF Core via <see cref="IDbContextFactory{TContext}"/>
        /// afin de garantir une création contrôlée des DbContext pour les traitements console/batch.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la dépendance <paramref name="contextFactory"/>.</description></item>
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="contextFactory">Factory EF Core de création de <see cref="DigitTryDbContext"/>.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="contextFactory"/> est null.</exception>
        public CR_UserSession(
            IDbContextFactory<DigitTryDbContext> contextFactory,
            IQ_AppContext appContext)
            : base(
                  contextFactory ?? throw new ArgumentNullException(nameof(contextFactory)),
                  appContext ?? throw new ArgumentNullException(nameof(appContext)))
        {
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne la liste des sessions <see cref="UserAppSession"/> pour un utilisateur et une application.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé lors de l’ouverture/fermeture de l’application pour retrouver la (ou les) sessions existantes
        /// d’un utilisateur sur une application donnée.
        /// </para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture optimisée (AsNoTracking) en filtrant les enregistrements soft-deleted.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Construire la callChain.</description></item>
        /// <item><description>Créer un DbContext via la factory.</description></item>
        /// <item><description>Interroger <see cref="UserAppSession"/> filtré par <c>IdUser</c> et <c>IdApplication</c>.</description></item>
        /// <item><description>Appliquer le filtre standard <c>IsDeleted == false</c>.</description></item>
        /// <item><description>Retourner la liste (éventuellement vide).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        /// <param name="appId">Identifiant de l’application.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Liste de sessions correspondant aux critères.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="userId"/> ou <paramref name="appId"/> est invalide.</exception>
        /// <exception cref="Exception">Toute exception est reclassifiée via <see cref="Ex_Classifier"/>.</exception>
        public async Task<List<UserAppSession>> GetByUserIdAppIdAsync(string caller, int userId, int appId, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByUserIdAppIdAsync)}";

            try
            {
                if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "userId must be > 0.");
                if (appId <= 0) throw new ArgumentOutOfRangeException(nameof(appId), "appId must be > 0.");

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                return await context.UserAppSessions
                    .AsNoTracking()
                    .Where(us =>
                        us.IdUser == userId &&
                        us.IdApplication == appId &&
                        us.IsDeleted == false)
                    .ToListAsync(ct);
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