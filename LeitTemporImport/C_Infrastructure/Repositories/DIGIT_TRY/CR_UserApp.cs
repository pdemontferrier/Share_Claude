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
    /// Repository concret (CR) dédié à l’entité <see cref="UserApp"/> pour la base DIGIT_TRY.
    /// Implémente les requêtes de lecture spécifiques, notamment la recherche par login Windows.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les QueryHandlers (QH) et Services Infrastructure nécessitant des accès optimisés à UserApp.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les accès EF Core spécifiques à UserApp avec traçabilité (CallChain)
    /// et reclassification des exceptions via <see cref="Ex_Classifier"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Couche Infrastructure : QueryHandlers / Services / UseCases via interfaces.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher un utilisateur par WindowsLogin en lecture (AsNoTracking).</description></item>
    /// </list>
    /// </summary>
    public class CR_UserApp : CR_Generic<UserApp>, IR_UserApp
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
        /// <para>Construit le repository concret pour <see cref="UserApp"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche Infrastructure.</para>
        /// <para>Objectif</para>
        /// <para>Permettre l’accès EF Core à UserApp via un DbContextFactory.</para>
        /// <param name="contextFactory">Factory EF Core du contexte DIGIT_TRY.</param>
        /// </summary>
        public CR_UserApp(
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
        /// <para>Retourne un utilisateur applicatif à partir de son login Windows.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé lors d’une identification de l’utilisateur courant sur un poste (device user).</para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture optimisée (AsNoTracking) via le champ WindowsLogin.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Créer un DbContext.</description></item>
        /// <item><description>Interroger UserApps par WindowsLogin.</description></item>
        /// <item><description>Retourner null si aucun utilisateur ne correspond.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="windowsLogin">Login Windows à rechercher.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="UserApp"/> ou null.</returns>
        /// </summary>
        public async Task<UserApp?> GetByWindowsLoginAsync(string caller, string windowsLogin, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByWindowsLoginAsync)}";

            try
            {
                if (string.IsNullOrWhiteSpace(windowsLogin))
                    throw new ArgumentException("windowsLogin is required.", nameof(windowsLogin));


                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                // Si UserApp possède IsDeleted, tu peux ajouter le filtre ici.
                return await context.UserApps
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.WindowsLogin == windowsLogin && u.IsDeleted == false, ct);
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