using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Services.Business;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Services.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service de lecture (Reader) dédié à <see cref="ProductionSeries"/>.
    /// Encapsule l’accès aux QueryHandlers afin de fournir une API métier simple et traçable.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases d’import MDB → SQL Server pour vérifier l’état d’import d’une série
    /// et piloter les décisions (ex : suppression du fichier si déjà importé).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser la lecture ProductionSeries via CQRS, en garantissant CallChain et reclassification d’exceptions.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Interroger <see cref="IQ_ProductionSeries"/> pour lire une série.</description></item>
    /// <item><description>Interroger <see cref="IQ_ProductionSeries"/> pour lire IsImported.</description></item>
    /// </list>
    /// </summary>
    public class SR_ProductionSeriesReader : IS_ProductionSeriesReader
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_ProductionSeries _qhProductionSeries;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service reader ProductionSeries.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser l’accès au QueryHandler ProductionSeries.</para>
        /// <param name="qhProductionSeries">QueryHandler ProductionSeries.</param>
        /// </summary>
        public SR_ProductionSeriesReader(IQ_ProductionSeries qhProductionSeries)
        {
            _callee = GetType().Name;
            _qhProductionSeries = qhProductionSeries ?? throw new ArgumentNullException(nameof(qhProductionSeries));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne la série de production correspondant à un numéro de série.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour charger l’état complet d’une série lorsque nécessaire.</para>
        /// <para>Objectif</para>
        /// <para>Exposer une lecture métier simple, basée sur CQRS.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="ProductionSeries"/> ou null.</returns>
        /// </summary>
        public async Task<ProductionSeries?> GetByIdSerialNumberAsync(string caller, int idSerialNumber, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByIdSerialNumberAsync)}";

            try
            {
                return await _qhProductionSeries.HandleGetByIdSerialNumberAsync(callChain, idSerialNumber, ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le statut IsImported d’une série identifiée par IdSerialNumber.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé par UC_TemporImport_ProcessFile pour décider si un fichier MDB doit être supprimé.</para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture optimisée du statut d’import, sans charger l’entité complète.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Bool? : true/false si trouvé, null sinon.</returns>
        /// </summary>
        public async Task<bool?> GetIsImportedAsync(string caller, int idSerialNumber, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetIsImportedAsync)}";

            try
            {
                return await _qhProductionSeries.HandleGetIsImportedAsync(callChain, idSerialNumber, ct);
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
