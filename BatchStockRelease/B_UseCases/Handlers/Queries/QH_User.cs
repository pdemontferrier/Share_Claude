using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_User : QH_Generic<User>, IQ_User
    {
        private readonly IR_User _repositorySpecifique;
        private readonly IS_Utilities _utilities;

        public QH_User(IR_User repository, IS_Utilities utilities)
            : base(repository)
        {
            _repositorySpecifique = repository;
            _utilities = utilities;
        }


        // Requête spécifique : Obtenir un enregistrement par nom d'utilisateur et mot de passe
        public async Task<User?> HandleGetSingleAsync(string loginId, string password)
        {
            string encryptedPassword = _utilities.GetCrypte_md5(password);
            return await _repositorySpecifique.GetByLoginAndPasswordAsync(loginId, encryptedPassword);
        }


        // Requête spécifique : Vérifier si le login Windows existe
        public async Task<User?> HandleGetByLoginWindowsAsync(string loginWindows)
        {
            return await _repositorySpecifique.GetByLoginWindowsAsync(loginWindows);
        }


        // Requête spécifique : Obtenir le nom complet d'un utilisateur
        public async Task<string> HandleGetUserFullNameAsync(int userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null)
                return "User not identified";

            return $"{user.Prenom} {user.Nom}";
        }
    }
}