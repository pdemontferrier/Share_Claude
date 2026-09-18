using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeMachine : QH_Generic<DecoupeMachine>, IQ_DecoupeMachine
    {
        private readonly IR_DecoupeMachine _repositorySpecifique;

        public QH_DecoupeMachine(IR_DecoupeMachine repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Obtenir un enregistrement par l'adresse IP du PC
        public async Task<DecoupeMachine?> HandleGetByDeviceIpAddressAsync(string ipDeviceAddress)
        {
            return await _repositorySpecifique.GetByDeviceIpAddressAsync(ipDeviceAddress);
        }
    }
}