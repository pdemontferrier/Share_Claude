using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    /// <summary>
    /// Handler dédié à la gestion des enregistrements dans la table <c>UserAppErrorLog</c>.
    /// Contrairement aux handlers génériques, ce handler ne génère pas d’entrée dans <c>UserAppEventStore</c>,
    /// afin d’éviter les boucles d’audit et de préserver la stabilité du système de journalisation.
    /// </summary>
    public class CH_UserAppErrorLog : IC_UserAppErrorLog
    {
        private readonly IR_Generic<UserAppErrorLog> _repository;

        /// <summary>
        /// Initialise une nouvelle instance du handler <see cref="CH_UserAppErrorLog"/>.
        /// </summary>
        /// <param name="repository">Repository générique pour la table <c>UserAppErrorLog</c>.</param>
        public CH_UserAppErrorLog(IR_Generic<UserAppErrorLog> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ajoute un enregistrement d’erreur dans la table <c>UserAppErrorLog</c>.
        /// Cette opération n’est pas accompagnée d’un enregistrement dans <c>UserAppEventStore</c>.
        /// </summary>
        /// <param name="entity">Entité d’erreur à enregistrer.</param>
        public async Task HandleAddAsync(UserAppErrorLog entity)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }
    }
}