using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.B_UseCases.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// QueryHandler (QH) dédié à l’entité <see cref="LifecycleAction"/>.
    /// Hérite de <see cref="QH_Generic{T}"/> afin de fournir les opérations de lecture génériques
    /// basées sur <see cref="IR_Generic{T}"/>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases/Services qui ont besoin de consulter les informations d’actions
    /// de cycle de vie enregistrées en base (diagnostic, contrôles, cohérence d’exécution).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Exposer un QueryHandler typé sans logique spécifique à ce stade, tout en respectant
    /// la structure CQRS et la séparation UseCases / Infrastructure.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Couche UseCases : UseCases / Services consommant les lectures génériques.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Initialiser le QH typé pour <see cref="LifecycleAction"/>.</description></item>
    /// <item><description>Déléguer les lectures génériques à <see cref="QH_Generic{T}"/>.</description></item>
    /// </list>
    /// </summary>
    public class QH_LifecycleAction : QH_Generic<LifecycleAction>, IQ_LifecycleAction
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        // Héritées via QH_Generic : repository générique

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le QueryHandler <see cref="QH_LifecycleAction"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir un accès typé aux lectures génériques de <see cref="LifecycleAction"/>
        /// via le repository générique.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la dépendance <paramref name="repository"/>.</description></item>
        /// <item><description>Transmettre le repository au handler générique parent.</description></item>
        /// </list>
        /// </summary>
        /// <param name="repository">Repository générique pour <see cref="LifecycleAction"/>.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="repository"/> est null.</exception>
        public QH_LifecycleAction(IR_Generic<LifecycleAction> repository)
            : base(repository ?? throw new ArgumentNullException(nameof(repository)))
        {
        }

        #endregion

        #region === Méthodes publiques ===

        // Requêtes spécifiques :
        // A compléter (si besoin ultérieur)

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}