using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchCutting_DG.B_UseCases.Services.BusinessLogic
{
    public class SR_CommandeClientAction : IS_CommandeClientAction
    {
        private readonly IC_CommandeClient _chCommandeClient;
        private readonly IQ_CommandeClient _qhCommandeClient;
        private readonly IQ_CommandeClientActionType _qhCommandeClientActionType;
        private readonly IC_CommandeClientModification _chCommandeClientModification;
        private readonly IQ_CommandeClientStatut _qhCommandeClientStatut;
        private readonly IC_CommandeClientAction _chCommandeClientAction;
        private readonly IS_Settings _settings;

        public SR_CommandeClientAction(IC_CommandeClient chCommandeClient, IQ_CommandeClient qhCommandeClient, 
                                        IQ_CommandeClientActionType qhCommandeClientActionType,
                                        IC_CommandeClientModification chCommandeClientModification, IQ_CommandeClientStatut qhCommandeClientStatut,
                                        IC_CommandeClientAction chCommandeClientAction, IS_Settings settings)
        {
            _chCommandeClient = chCommandeClient;
            _qhCommandeClient = qhCommandeClient;
            _chCommandeClientModification = chCommandeClientModification;
            _qhCommandeClientActionType = qhCommandeClientActionType;
            _qhCommandeClientStatut = qhCommandeClientStatut;
            _chCommandeClientAction = chCommandeClientAction;
            _settings = settings;
        }

        public async Task AddNewActionAsync(int idAction)
        {
            // Identifier le statut associé à l'action
            var actionType = await _qhCommandeClientActionType.HandleGetByIdAsync(idAction);
            int statutId = actionType.IdCcStatut;

            // Ajouter une nouvelle action
            await AddNewActionByProjectAsync(idAction, statutId);
        }

        private async Task AddNewActionByProjectAsync(int idAction, int statutId)
        {
            var commandes = await _qhCommandeClient.HandleGetByIdDecoupeLotAsync(_settings.GetDecoupeLotId());
            if (commandes == null || !commandes.Any())
                return;

            foreach (var commande in commandes)
            {
                // Créer un enregistrement CommandeClientAction
                var action = new CommandeClientAction
                {
                    IdCmdClient = commande.Id,
                    IdUser = _settings.GetAppUserID(),
                    IdAction = idAction,
                    DateAction = _settings.GetAppDateTime(),
                    TempsEstime = 0,
                    BonusMalus = 1
                };

                // Ajouter un enregistrement à CommandeClientAction
                await _chCommandeClientAction.HandleAddAsync(action, GetType().Name, nameof(AddNewActionByProjectAsync));

                // Mettre à jour le statut de la commande
                await UpdateProjectStatusAsync(commande, statutId);

                // Ajouter une ligne d'historique dans la table CommandeClientModification
                await UpdateCommandeClientModificationAsync(commande, statutId);

            }
        }

        private async Task UpdateProjectStatusAsync(CommandeClient commande, int statutId)
        {
            commande.Statut = statutId;
            await _chCommandeClient.HandleUpdateAsync(commande, GetType().Name, nameof(UpdateProjectStatusAsync));
        }

        private async Task UpdateCommandeClientModificationAsync(CommandeClient commande, int statutId)
        {
            var statut = await _qhCommandeClientStatut.HandleGetByIdAsync(statutId);
            string statutDesignation = statut.Nom ?? string.Empty;

            // Créer un enregistrement CommandeClientModification
            var modification = new CommandeClientModification
            {
                NumProjet = commande.NumProjet,
                TypeModif = "Changement statut",
                NouvelleValeur = statutDesignation,
                DateModification = _settings.GetAppDateTime(),
                UserId = _settings.GetAppUserID()
            };

            // Ajouter un enregistrement à CommandeClientModification
            await _chCommandeClientModification.HandleAddAsync(modification, GetType().Name, nameof(UpdateCommandeClientModificationAsync));
        }
    }
}