using LeitTemporImport.A_Domain.DTOs.App;
using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.A_Domain.Interfaces.Services.Business;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Services.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service métier chargé d’ajouter des enregistrements dans <see cref="LifecycleAction"/>
    /// pour historiser des événements structurés du cycle de vie (type/source/idSource/comments),
    /// enrichis par le contexte applicatif (AppId/UserId/Device*/Timestamp).
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases d’import afin de tracer les étapes clés (ex : série importée, fichier traité, etc.).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser la logique de composition des écritures <see cref="LifecycleAction"/> afin que les Handlers
    /// restent génériques et que la sémantique métier soit portée par un service dédié.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases / Services Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Récupérer le <see cref="DTO_AppContext"/>.</description></item>
    /// <item><description>Construire l’entité <see cref="LifecycleAction"/>.</description></item>
    /// <item><description>Déclencher l’insertion via <see cref="IC_Generic{T}"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_LifecycleActionAdd : IS_LifecycleActionAdd
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IC_LifecycleAction _chLifecycleAction;
        private readonly IQ_ProductionSeries _qhProductionSeries;
        private readonly IQ_AppContext _appContext;

        #endregion

        #region === Constructeur ===

        public SR_LifecycleActionAdd(
            IC_LifecycleAction chLifecycleAction,
            IQ_ProductionSeries qhProductionSeries,
            IQ_AppContext appContext)
        {
            _callee = GetType().Name;
            _chLifecycleAction = chLifecycleAction ?? throw new ArgumentNullException(nameof(chLifecycleAction));
            _qhProductionSeries = qhProductionSeries ?? throw new ArgumentNullException(nameof(qhProductionSeries));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Ajoute une action indiquant qu’une série de production a été importée.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Appelé en fin d’import valide pour historiser l’état “imported” de la série dans <see cref="LifecycleAction"/>.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Enregistrer une ligne structurée :
        /// </para>
        /// <list type="bullet">
        /// <item><description>Type = 1 (Imported).</description></item>
        /// <item><description>Source = 1 (ProductionSeries).</description></item>
        /// <item><description>IdSource = ProductionSeries.Id.</description></item>
        /// <item><description>Commentaire explicite (≤ 500).</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="serialNumberId">Numéro de série métier (ex : 6 chiffres) pour le commentaire.</param>
        /// <param name="filePath">Chemin complet du fichier MDB.</param>
        /// <exception cref="ArgumentOutOfRangeException">Si un identifiant est invalide.</exception>
        /// <exception cref="Ex_Business">Si la composition métier ne peut pas être réalisée.</exception>
        /// </summary>
        public async Task ExecuteProductionSeriesImportedAsync( string caller, int serialNumberId, string filePath, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteProductionSeriesImportedAsync)}";

            try
            {
                if (serialNumberId <= 0)
                    throw new ArgumentOutOfRangeException(nameof(serialNumberId), "serialNumberId must be > 0.");

                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentNullException(nameof(filePath));

                // 1) Retrouver ProductionSeries.Id (IdSource)
                var series = await _qhProductionSeries.HandleGetByIdSerialNumberAsync(callChain, serialNumberId, ct);

                if (series == null || series.Id <= 0)
                    throw new Ex_Business($"ProductionSeries not found for IdSerialNumber={serialNumberId}.");
                int productionSeriesId = series.Id;

                // 2) Extraire le nom du fichier à partir du chemin
                string fileName = Path.GetFileName(filePath);

                // 3) Récupérer le contexte applicatif
                DTO_AppContext ctx = _appContext.GetAppContext();

                // 4) Construire l’action (valeurs fixes)
                const short actionTypeImported = 1;           // Imported
                const short sourceProductionSeries = 1;       // ProductionSeries

                string comments = $"SerieNum: {serialNumberId} - Imported from MDB: {fileName} (table Tempor)";

                if (comments.Length > 500)
                    comments = comments[..500];

                var entity = new LifecycleAction
                {
                    IdLifecycleActionType = actionTypeImported,
                    IdLifecycleActionSource = sourceProductionSeries,
                    IdSource = productionSeriesId,
                    Comments = comments,
                    IdApplication = ctx.AppId,
                    IdUser = ctx.AppUserId,
                    DeviceUser = ctx.AppDeviceUser,
                    DeviceId = ctx.AppDeviceId,
                    DeviceIp = ctx.AppDeviceIP,
                    ActionTimestamp = ctx.AppDateTime
                };

                // 5) Insertion via commande générique + event store standard
                await _chLifecycleAction.HandleAddAsync(callChain, entity);
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