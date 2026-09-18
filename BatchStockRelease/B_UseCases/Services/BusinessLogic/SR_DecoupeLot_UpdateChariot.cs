using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using System.Windows;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service de validation du chariot sélectionné pour l’approvisionnement.
    /// Met à jour la table DecoupeLot avec les informations du chariot choisi.
    /// </summary>
    public class SR_DecoupeLot_UpdateChariot : IS_DecoupeLot_UpdateChariot
    {
        private readonly string _callee;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IC_DecoupeLot _chDecoupeLot;


        public SR_DecoupeLot_UpdateChariot(
            IQ_DecoupeLot qhDecoupeLot,
            IC_DecoupeLot chDecoupeLot)
        {
            _callee = GetType().Name;
            _qhDecoupeLot = qhDecoupeLot;
            _chDecoupeLot = chDecoupeLot;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, int chariotId, string chariotDesignation, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Vérification des données
                if (chariotId <= 0 || string.IsNullOrWhiteSpace(chariotDesignation))
                    throw new Ex_Business(callChain,"DLU_03", "No_Er_Bu_55");

                // Mettre à jour la table DecoupeLot
                var decoupeLot = await _qhDecoupeLot.HandleGetByIdAsync(decoupeLotId);

                if (decoupeLot == null)
                    throw new Ex_Business(callChain,"DLU_04", "No_Er_Bu_30");

                decoupeLot.ApproIdChariot = chariotId;
                decoupeLot.ApproChariotDesignation = chariotDesignation;
                await _chDecoupeLot.HandleUpdateAsync(decoupeLot, callChain);
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}