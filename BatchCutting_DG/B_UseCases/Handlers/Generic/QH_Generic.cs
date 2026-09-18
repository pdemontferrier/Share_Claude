using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Generic
{
    public class QH_Generic<T> : IQ_Generic<T> where T : class
    {
        protected readonly IR_Generic<T> _repository;

        public QH_Generic(IR_Generic<T> repository)
        {
            _repository = repository;
        }


        // Requête générique : Obtenir un enregistrement par ID
        public async Task<T?> HandleGetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // Requête générique : Obtenir le premier enregistrement
        public async Task<T?> HandleGetFirstOrDefaultAsync()
        {
            return await _repository.GetFirstOrDefaultAsync();
        }

        // Requête générique : Vérifier si un enregistrement existe par ID
        public async Task<bool> HandleGetAnyAsync(int id)
        {
            return await _repository.GetAnyAsync(id);
        }

        // Requête générique : Obtenir tous les enregistrements
        public async Task<List<T>> HandleGetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Requête générique : Obtenir tous les enregistrements sans traking
        public async Task<List<T>> HandleGetAllAsNoTrackingAsync()
        {
            return await _repository.GetAllAsNoTrackingAsync();
        }
    }
}