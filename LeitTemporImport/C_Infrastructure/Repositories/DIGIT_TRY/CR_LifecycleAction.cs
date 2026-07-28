using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;
using LeitTemporImport.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LeitTemporImport.C_Infrastructure.Repositories.DIGIT_TRY
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Repository concret (CR) spécialisé pour l’entité <see cref="LifecycleAction"/>.
    /// Hérite de <see cref="CR_Generic{T}"/> afin de fournir les opérations CRUD génériques,
    /// et peut exposer des requêtes spécifiques liées à l’historisation du cycle de vie métier.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les Handlers/Services/UseCases responsables de journaliser les actions de cycle de vie
    /// des tables principales métiers(ex : série, commande client) dans la table  <see cref="LifecycleAction"/>.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les accès EF Core à <see cref="LifecycleAction"/> en appliquant :
    /// la traçabilité (callChain) lorsque des méthodes spécifiques sont implémentées,
    /// des lectures optimisées (AsNoTracking) lorsque nécessaire,
    /// et la reclassification des exceptions via <see cref="Ex_Classifier"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Couche Infrastructure : CommandHandlers / Services / UseCases via interfaces.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Fournir un repository concret pour <see cref="LifecycleAction"/>.</description></item>
    /// <item><description>Supporter l’écriture des événements de cycle de vie via les CRUD génériques.</description></item>
    /// <item><description>Ajouter, si besoin, des méthodes de lecture/filtrage spécifiques au cycle de vie.</description></item>
    /// <item><description>Garantir une création contrôlée des DbContext via <see cref="IDbContextFactory{TContext}"/>.</description></item>
    /// </list>
    /// </summary>
    public class CR_LifecycleAction : CR_Generic<LifecycleAction>, IR_Generic<LifecycleAction>
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
        /// <para>Construit le repository concret <see cref="CR_LifecycleAction"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche Infrastructure.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Initialiser l’accès au contexte EF Core via <see cref="IDbContextFactory{TContext}"/>
        /// afin de garantir une création contrôlée des DbContext, compatible avec les traitements console/batch.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la dépendance <paramref name="contextFactory"/>.</description></item>
        /// <item><description>Initialiser <c>_callee</c> pour la traçabilité interne.</description></item>
        /// </list>
        /// <param name="contextFactory">Factory EF Core de création de <see cref="DigitTryDbContext"/>.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="contextFactory"/> est null.</exception>
        /// </summary>
        public CR_LifecycleAction(
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

        // A compléter

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}