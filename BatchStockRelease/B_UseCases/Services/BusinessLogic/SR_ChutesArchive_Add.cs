using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé d’ajouter une ligne dans la table <c>chutes_archive</c>.
    /// </summary>
    public class SR_ChutesArchive_Add : IS_ChutesArchive_Add
    {
        private readonly string _callee;
        private readonly IC_ChutesArchive _chChutesArchive;
        private readonly IQ_ChutesMagasin _qhChutesMagasin;

        public SR_ChutesArchive_Add(
            IC_ChutesArchive chChutesArchive,
            IQ_ChutesMagasin qhChutesMagasin)
        {
            _callee = GetType().Name;

            _chChutesArchive = chChutesArchive;
            _qhChutesMagasin = qhChutesMagasin;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            int chuteMagasinId,
            DateTime appDateTime,
            int appUserId,
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                var chuteMagasin = await _qhChutesMagasin.HandleGetByIdAsync(chuteMagasinId);

                if (chuteMagasin == null || chuteMagasin.Id <= 0)
                    throw new Ex_Business(callChain,"CA_10", "No_Er_Bu_31");

                // Crer une ChutesArchive pour traçabilité
                var archive = new ChutesArchive
                {
                    IdArticleInterne = chuteMagasin.IdArticleInterne,
                    IdType = chuteMagasin.IdType,
                    Longueur = chuteMagasin.Longueur,
                    Largeur = chuteMagasin.Largeur,
                    ScanEntree = chuteMagasin.Scan,
                    ScanSortie = 1,
                    IdOperateurEntree = chuteMagasin.IdOperateur,
                    IdOperateurSortie = appUserId,
                    DateEntree = chuteMagasin.Enregistrement,
                    DateSortie = appDateTime,
                    Reserve = chuteMagasin.Reserve,
                    CodeBarre = chuteMagasin.CodeBarre,
                    Prix = chuteMagasin.Prix
                };

                // Ajouter un nouvel enregistrement dans ChuteArchive
                await _chChutesArchive.HandleAddAsync(archive, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}