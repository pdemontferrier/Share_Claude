using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.B_UseCases.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Handlers.Commands
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// CommandHandler (CH) dédié à l’entité <see cref="Tempor_Import"/>.
    /// Hérite de <see cref="CH_Generic{T}"/> afin de fournir les commandes génériques (CRUD)
    /// via un <see cref="IR_Generic{T}"/> et la journalisation des écritures via <c>Tempor_Import</c>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases/Services lors de l’exécution batch afin d’enregistrer les événements
    /// de cycle de vie (ex : Start/Stop, checkpoints).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Exposer un CH typé pour <see cref="Tempor_Import"/> sans ajouter de logique spécifique
    /// à ce stade, tout en respectant les conventions de traçabilité et de structure.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Couche UseCases : Services et UseCases qui écrivent dans la table Tempor_Import.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Initialiser le handler typé.</description></item>
    /// <item><description>Déléguer les commandes génériques au parent <see cref="CH_Generic{T}"/>.</description></item>
    /// </list>
    /// </summary>
    public class CH_TemporImport : CH_Generic<Tempor_Import>, IC_TemporImport
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        // Héritées via CH_Generic : _repository, _eventStore
        // A compléter si commandes spécifiques

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le CommandHandler <see cref="Tempor_Import"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir un accès typé aux commandes génériques sur <see cref="Tempor_Import"/>
        /// tout en conservant la journalisation standard des écritures.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les dépendances.</description></item>
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Transmettre les dépendances au handler générique parent.</description></item>
        /// </list>
        /// </summary>
        /// <param name="repository">Repository générique pour <see cref="Tempor_Import"/>.</param>
        /// <param name="eventStore">Handler d’écriture dans <c>UserAppEventStore</c>.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est null.</exception>
        public CH_TemporImport(
            IR_Generic<Tempor_Import> repository)
            : base(repository ?? throw new ArgumentNullException(nameof(repository)))
        {
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        // Commandes spécifiques :
        // A compléter (si besoin ultérieur)

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}