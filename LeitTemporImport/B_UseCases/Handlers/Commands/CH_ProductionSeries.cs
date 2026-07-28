using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using LeitTemporImport.B_UseCases.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Handlers.Commands
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// CommandHandler dédié à <see cref="ProductionSeries"/> pour exécuter des commandes de mise à jour
    /// tout en respectant la mécanique générique : update via <see cref="CH_Generic{T}"/> puis log snapshot
    /// complet dans <c>UserAppEventStore</c>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases d’import console pour marquer une série comme importée à partir de
    /// son identifiant métier <c>IdSerialNumber</c>.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Charger l’entité cible, appliquer la mise à jour en mémoire, puis appeler l’update générique
    /// afin de déclencher automatiquement la journalisation snapshot.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Valider <c>idSerialNumber</c>.</description></item>
    /// <item><description>Charger la série active via le repository spécifique.</description></item>
    /// <item><description>Mettre à jour <c>IsImported</c> et <c>UpdatedAt</c>.</description></item>
    /// <item><description>Appeler <see cref="CH_Generic{T}.HandleUpdateAsync"/> pour persister et logger.</description></item>
    /// <item><description>Reclassifier les exceptions via <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class CH_ProductionSeries : CH_Generic<ProductionSeries>, IC_ProductionSeries
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IR_ProductionSeries _repositorySpecifique;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le CommandHandler <see cref="CH_ProductionSeries"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Initialiser le repository spécifique nécessaire au chargement par <c>IdSerialNumber</c>,
        /// tout en conservant la mécanique générique (update + eventstore).
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider <paramref name="repository"/>.</description></item>
        /// </list>
        /// <param name="repository">Repository spécifique de <see cref="ProductionSeries"/>.</param>
        /// <param name="eventStore">Handler EventStore.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="repository"/> ou <paramref name="eventStore"/> est null.</exception>
        /// </summary>
        public CH_ProductionSeries(
            IR_ProductionSeries repository)
            : base(repository ?? throw new ArgumentNullException(nameof(repository)))
        {
            _callee = GetType().Name;
            _repositorySpecifique = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Marque une série comme importée en positionnant <c>IsImported = true</c>.</para>
        /// <para>Contexte</para>
        /// <para>Appelé à la fin d’un import valide pour tracer la série traitée.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Respecter la mécanique générique (update + snapshot eventstore) en chargeant
        /// l’entité puis en appelant <see cref="CH_Generic{T}.HandleUpdateAsync"/>.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Construire la callChain.</description></item>
        /// <item><description>Charger la série active via <see cref="IR_ProductionSeries.GetByIdSerialNumberAsync"/>.</description></item>
        /// <item><description>Mettre à jour <c>IsImported</c> et <c>UpdatedAt</c>.</description></item>
        /// <item><description>Persister et logger via <see cref="CH_Generic{T}.HandleUpdateAsync"/>.</description></item>
        /// </list>
        /// <param name="caller">Chaîne d’appel amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="idSerialNumber"/> est invalide.</exception>
        /// <exception cref="Ex_Business">Si aucune série active n’existe.</exception>
        /// <exception cref="Exception">Toute exception est reclassifiée via <see cref="Ex_Classifier"/>.</exception>
        /// </summary>
        public async Task HandleSetIsImportedTrueAsync(string caller, int idSerialNumber, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleSetIsImportedTrueAsync)}";

            try
            {
                if (idSerialNumber <= 0)
                    throw new ArgumentOutOfRangeException(nameof(idSerialNumber), "idSerialNumber must be > 0.");

                ProductionSeries? series = await _repositorySpecifique.GetByIdSerialNumberAsync(callChain, idSerialNumber, ct);

                if (series is null)
                    throw new Ex_Business($"No active ProductionSeries found for IdSerialNumber={idSerialNumber}.");

                // Idempotence : si déjà importée, on ne refait pas d'update (et donc pas de log).
                if (series.IsImported)
                    return;

                series.IsImported = true;

                await HandleUpdateAsync(callChain, series);
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