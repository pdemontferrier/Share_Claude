using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé d’enregistrer une ligne d’historique de modification sur une commande client.
    /// </summary>
    public class SR_CommandeClientModification_Add : IS_CommandeClientModification_Add
    {
        private readonly IC_CommandeClientModification _chModification;
        private readonly IQ_CommandeClientStatut _qhStatut;

        private readonly string _callee;

        public SR_CommandeClientModification_Add(
            IC_CommandeClientModification chModification,
            IQ_CommandeClientStatut qhStatut)
        {
            _chModification = chModification;
            _qhStatut = qhStatut;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            CommandeClient commande, 
            int statutId,
            DateTime appDateTime,
            int appUserId,
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                var statut = await _qhStatut.HandleGetByIdAsync(statutId);
                if (statut == null)
                    throw new Ex_Business(callChain,"STK_08", "No_Er_Bu_29");

                // Créer un enregistrement CommandeClientModification
                var modification = new CommandeClientModification
                {
                    NumProjet = commande.NumProjet,
                    TypeModif = "Changement statut",
                    NouvelleValeur = statut.Nom ?? string.Empty,
                    DateModification = appDateTime,
                    UserId = appUserId,
                };

                // Ajouter un enregistrement CommandeClientModification
                await _chModification.HandleAddAsync(modification, callChain);
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}