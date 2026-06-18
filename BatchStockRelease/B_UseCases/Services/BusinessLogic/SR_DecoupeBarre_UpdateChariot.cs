using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using System.Windows;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service de validation du chariot sélectionné pour l’approvisionnement.
    /// Met à jour la table DecoupeBarre avec les informations du chariot choisi.
    /// </summary>
    public class SR_DecoupeBarre_UpdateChariot : IS_DecoupeBarre_UpdateChariot
    {
        private readonly string _callee;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IC_DecoupeBarre _chDecoupeBarre;


        public SR_DecoupeBarre_UpdateChariot(
            IQ_DecoupeBarre qhDecoupeBarre,
            IC_DecoupeBarre chDecoupeBarre)
        {
            _callee = GetType().Name;
            _qhDecoupeBarre = qhDecoupeBarre;
            _chDecoupeBarre = chDecoupeBarre;
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
                    throw new Ex_Business(callChain,"DBU_09", "No_Er_Bu_55");

                // Mettre à jour les barres non encore allouées
                var decoupeBarres = await _qhDecoupeBarre.HandleGetAllocatedAsync(decoupeLotId);

                if (decoupeBarres == null)
                    throw new Ex_Business(callChain,"DBU_10", "No_Er_Bu_56");

                foreach (var decoupeBarre in decoupeBarres)
                {
                    // Mettre à jour les valeurs
                    decoupeBarre.ApproIdChariot = chariotId;
                    decoupeBarre.ApproChariotDesignation = chariotDesignation;

                    // Mettre à jour la base de données
                    await _chDecoupeBarre.HandleUpdateAsync(decoupeBarre, callChain);
                }
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}