using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchCutting_DG.B_UseCases.Services.BusinessLogic
{
    public class SR_Decoupe : IS_Decoupe
    {
        private readonly string ServiceName;
        private readonly IS_Settings _settings;
        private readonly IS_Navigation _navigation;
        private readonly IS_SerialSender _serialSender;
        private readonly IS_LabelPrinter _labelPrinter;
        private readonly IS_DataBase _dataBase;
        private readonly IS_CommandeClientAction _commandeClientAction;
        private readonly IS_Messages _messages;
        private readonly IS_Dictionary _dictionary;
        private readonly IS_BarDropOptim _barDropOptim;
        private readonly IS_BarNewOptim _barNewOptim;
        private readonly IC_DecoupeLot _chDecoupeLot;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IC_DecoupeBarre _chDecoupeBarre;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IC_DecoupeDetail _chDecoupeDetail;
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly IC_ChutesMagasin _chChutesMagasin;
        private readonly DateTime DefaultDate = new DateTime(1970, 1, 1, 0, 0, 0);

        public SR_Decoupe(IS_Settings settings, IS_Navigation navigation,
                                IS_SerialSender serialSender, IS_LabelPrinter labelPrinter,
                                IS_DataBase dataBase, IS_CommandeClientAction commandeClientAction,
                                IS_Messages messages, IS_Dictionary dictionary,
                                IS_BarDropOptim barDropOptim, IS_BarNewOptim barNewOptim,
                                IC_DecoupeLot chDecoupeLot, IQ_DecoupeLot qhDecoupeLot,
                                IC_DecoupeBarre chDecoupeBarre, IQ_DecoupeBarre qhDecoupeBarre,
                                IC_DecoupeDetail chDecoupeDetail, IQ_DecoupeDetail qhDecoupeDetail,
                                IC_ChutesMagasin chChutesMagasin)
        {
            ServiceName = GetType().Name;
            _settings = settings;
            _navigation = navigation;
            _serialSender = serialSender;
            _labelPrinter = labelPrinter;
            _dataBase = dataBase;
            _commandeClientAction = commandeClientAction;
            _messages = messages;
            _dictionary = dictionary;
            _barDropOptim = barDropOptim;
            _barNewOptim = barNewOptim;
            _chDecoupeLot = chDecoupeLot;
            _qhDecoupeLot = qhDecoupeLot;
            _chDecoupeBarre = chDecoupeBarre;
            _qhDecoupeBarre = qhDecoupeBarre;
            _chDecoupeDetail = chDecoupeDetail;
            _qhDecoupeDetail = qhDecoupeDetail;
            _chChutesMagasin = chChutesMagasin;
        }

        public async Task LoadDecoupeDetailAsync(int decoupeDetailId, string decoupeDetailMessageElumatec)
        {
            // Mettre à jour l'enregistrement
            await UpdateDecoupeDetailAtCutStartAsync(decoupeDetailId);

            // Envoyer les données vers la Machine DG
            await _serialSender.SendMessageAsync(decoupeDetailMessageElumatec);
        }

        private async Task BarWasteProcessingAsync()
        {
            // Imprimer l'étiquette de Déchet
            _labelPrinter.PrintBarWasteLabel();

            // Tester si il reste des enregistrements à traiter et afficher la Page20
            await CheckIfCutToDoAsync("Page20");
        }

        private async Task CheckIfCutToDoAsync(string nextPage)
        {
            // Récupérer le premier détail de découpe à traiter
            var decoupeDetail = await _qhDecoupeDetail.HandleGetFirstToCutAsync();
            if (decoupeDetail == null)
            {
                // Terminer les découpe du lot
                await DecoupeLotCompletedAsync();
            }
            else
            {
                _navigation.NavigateToNewPage(nextPage);
            }
        }

        public async Task DecoupeLotStartAsync()
        {
            // Mettre à jour DecoupeLot
            var entity = await _qhDecoupeLot.HandleGetByIdAsync(_settings.GetDecoupeLotId());

            if (entity.DecoupeDgDateDebut == DefaultDate) entity.DecoupeDgDateDebut = _settings.GetAppDateTime();

            await _chDecoupeLot.HandleUpdateAsync(entity, ServiceName, nameof(DecoupeLotStartAsync));
        }

        private async Task DecoupeLotEndAsync()
        {
            // Mettre à jour DecoupeLot
            var entity = await _qhDecoupeLot.HandleGetByIdAsync(_settings.GetDecoupeLotId());

            entity.DecoupeDg = true;
            entity.DecoupeDgDateFin = _settings.GetAppDateTime();

            await _chDecoupeLot.HandleUpdateAsync(entity, ServiceName, nameof(DecoupeLotEndAsync));
        }

        private async Task NewBatchCuttingDGActionAsync()
        {
            // Déduction de l'action à appliquer selon la machine
            var machineDG = _settings.GetCuttingMachine();
            int idAction = machineDG switch
            {
                "DG244_01" => _settings.GetActionBarCutDG244_01(),
                "DG244_02" => _settings.GetActionBarCutDG244_02(),
                "DG244_03" => _settings.GetActionBarCutDG244_03(),
                _ => 0 // Ne devrait jamais arriver
            };

            await _commandeClientAction.AddNewActionAsync(idAction);
        }

        public async Task DecoupeLotCompletedAsync()
        {
            // Mettre à jour DecoupeDarre
            await UpdateDecoupeBarreAtCutValidationAsync();

            // Lancer la procédure stockée SprCommandeClientProductionUpdateAsync()
            await _dataBase.SprCommandeClientProductionUpdateAsync();

            // Mettre à jour CommandeClientAction
            await NewBatchCuttingDGActionAsync();

            // Mettre à jour DecoupeLot
            await DecoupeLotEndAsync();

            // Afficher la Page10
            _navigation.NavigateToNewPage("Page10");
        }

        public async Task BarValidationAsync()
        {
            var entity = await _qhDecoupeBarre.HandleGetByIdAsync(_settings.GetDecoupeBarreId());
            if (entity == null) return;

            if (entity.DecoupeDateDebut == DefaultDate)
            {
                entity.DecoupeDateDebut = _settings.GetAppDateTime();
                entity.DecoupeUtilisateurPoste = _settings.GetAppDeviceUser();
                entity.DecoupeUtilisateurErp = _settings.GetAppUserID();
                entity.DecoupePosteMachineId = _settings.GetAppDeviceID();
                entity.DecoupePosteMachineIp = _settings.GetAppDeviceIP();

                await _chDecoupeBarre.HandleUpdateAsync(entity, ServiceName, nameof(BarValidationAsync));
            }

            // Lancer la procédure stockée SprCommandeClientProductionUpdateAsync()
            await _dataBase.SprCommandeClientProductionUpdateAsync();

            // Tester si il reste des enregistrements à traiter et si oui afficher la Page30
            await CheckIfCutToDoAsync("Page30");
        }

        public async Task UpdateDecoupeBarreAtCutValidationAsync()
        {
            var entity = await _qhDecoupeBarre.HandleGetByIdAsync(_settings.GetDecoupeBarreId());
            if (entity == null) return;
            entity.DecoupeFaite = true;
            entity.DecoupeDateFin = _settings.GetAppDateTime();
            entity.DecoupeUtilisateurPoste = _settings.GetAppDeviceUser();
            entity.DecoupeUtilisateurErp = _settings.GetAppUserID();
            entity.DecoupePosteMachineId = _settings.GetAppDeviceID();
            entity.DecoupePosteMachineIp = _settings.GetAppDeviceIP();

            await _chDecoupeBarre.HandleUpdateAsync(entity, ServiceName, nameof(UpdateDecoupeBarreAtCutValidationAsync));
        }

        private async Task UpdateDecoupeBarreLongueurChuteFinaleAsync(int longueurFinale)
        {
            var decoupeBarre = await _qhDecoupeBarre.HandleGetByIdAsync(_settings.GetDecoupeBarreId());
            decoupeBarre.DecoupeLongueurChuteFinale = longueurFinale;
            await _chDecoupeBarre.HandleUpdateAsync(decoupeBarre, ServiceName, nameof(BarDropValidationAsync));
        }


        private async Task UpdateDecoupeBarreAtRefusalAsync()
        {
            var entity = await _qhDecoupeBarre.HandleGetByIdAsync(_settings.GetDecoupeBarreId());

            if (entity == null) return;

            if (string.IsNullOrEmpty(entity.DecoupeCommentaires))
            {
                entity.DecoupeCommentaires = _settings.GetDecoupeBarreDecoupeCommentaires();
            }
            else
            {
                entity.DecoupeCommentaires += " - " + _settings.GetDecoupeBarreDecoupeCommentaires();
            }

            entity.DecoupeNombre = 0;
            entity.DecoupeLongueurReste = 0;
            entity.DecoupeTypeReste = "nd";
            entity.DecoupeDateDebut = _settings.GetAppDateTime();
            entity.DecoupeDateFin = _settings.GetAppDateTime();

            await _chDecoupeBarre.HandleUpdateAsync(entity, ServiceName, nameof(UpdateDecoupeBarreAtRefusalAsync));
        }

        private async Task UpdateDecoupeDetailAtRefusalAsync(int decoupeDetailId, string? commentaires)
        {
            var entity = await _qhDecoupeDetail.HandleGetByIdAsync(decoupeDetailId);
            if (entity == null) return;

            entity.OrdreTri = 999;
            entity.ApproOptimBarreChute = false;
            entity.ApproOptimBarreNeuve = false;
            entity.IdDecoupeBarre = 0;
            entity.DecoupeBarreIndex = 0;
            entity.DecoupeLongueurReste = null;
            entity.DecoupeDateDebut = DefaultDate;

            if (string.IsNullOrEmpty(entity.DecoupeCommentaires))
            {
                entity.DecoupeCommentaires = commentaires;
            }
            else
            {
                entity.DecoupeCommentaires += " - " + commentaires;
            }

            await _chDecoupeDetail.HandleUpdateAsync(entity, ServiceName, nameof(UpdateDecoupeDetailAtRefusalAsync));
        }

        private async Task UpdateDecoupeDetailAtCutStartAsync(int decoupeDetailId)
        {
            var entity = await _qhDecoupeDetail.HandleGetByIdAsync(decoupeDetailId);
            if (entity == null) return;

            entity.DecoupeDateDebut = _settings.GetAppDateTime();
            entity.DecoupeUtilisateurPoste = _settings.GetAppDeviceUser();
            entity.DecoupeUtilisateurErp = _settings.GetAppUserID();
            entity.DecoupePosteMachineId = _settings.GetAppDeviceID();
            entity.DecoupePosteMachineIp = _settings.GetAppDeviceIP();

            await _chDecoupeDetail.HandleUpdateAsync(entity, ServiceName, nameof(UpdateDecoupeDetailAtCutStartAsync));
        }

        private async Task UpdateDecoupeDetailAtCutValidationAsync(int decoupeDetailId)
        {
            var entity = await _qhDecoupeDetail.HandleGetByIdAsync(decoupeDetailId);
            if (entity == null) return;

            entity.DecoupeFaite = true;
            entity.DecoupeDateFin = _settings.GetAppDateTime();
            entity.DecoupeUtilisateurPoste = _settings.GetAppDeviceUser();
            entity.DecoupeUtilisateurErp = _settings.GetAppUserID();
            entity.DecoupePosteMachineId = _settings.GetAppDeviceID();
            entity.DecoupePosteMachineIp = _settings.GetAppDeviceIP();

            await _chDecoupeDetail.HandleUpdateAsync(entity, ServiceName, nameof(UpdateDecoupeDetailAtCutValidationAsync));
        }


        public async Task CutValidationAsync()
        {
            // Charger DecoupeDetailWithCut
            var decoupeDetailWithCut = _settings.GetDecoupeDetailWithCut();

            // Vérifier si un enregistrement de decoupeDetailWithCut existe
            if (decoupeDetailWithCut == null) return;

            // Mettre à jour l'enregistrement DecoupeDetail
            await UpdateDecoupeDetailAtCutValidationAsync(decoupeDetailWithCut.Id);

            // Imprimer une étiquette
            _labelPrinter.PrintBarCutLabel(decoupeDetailWithCut);

            // Tester l'existence d'un indice de coupe = 2
            var decoupeDetailNext = await _qhDecoupeDetail.HandleGetFirstToCutAsync();
            if (decoupeDetailNext != null && decoupeDetailNext.NumLigne == (decoupeDetailWithCut.NumLigne + 1))
            {
                _navigation.RefreshCurrentPage();
            }
            // Tester si la découpe actuelle n'est pas la dernière découpe à réaliser de la barre
            else if (decoupeDetailWithCut.DecoupeBarreDecoupeNombre != decoupeDetailWithCut.DecoupeBarreIndex)
            {
                _navigation.RefreshCurrentPage();
            }
            // Tester si la découpe actuelle est la dernière découpe à réaliser de la barre
            else if (decoupeDetailWithCut.DecoupeBarreDecoupeNombre == decoupeDetailWithCut.DecoupeBarreIndex)
            {
                // Mettre à jour DecoupeDarre
                await UpdateDecoupeBarreAtCutValidationAsync();

                if (decoupeDetailWithCut.DecoupeBarreGestionChutes == true && decoupeDetailWithCut.DecoupeBarreTypeReste == "chute")
                {
                    _navigation.NavigateToNewPage("Page22");
                }
                else
                {
                    await BarWasteProcessingAsync();
                }
            }
            // Tester si il reste une découpe à réaliser
            else if (decoupeDetailNext == null)
            {
                // Terminer le lot
                await DecoupeLotCompletedAsync();
            }
            else
            {
                _navigation.RefreshCurrentPage();
            }
        }


        public async Task BarDropValidationAsync()
        {
            // Charger DecoupeBarreWithCut et Length
            var decoupeBarreWithCut = _settings.GetDecoupeBarreWithCut();
            decoupeBarreWithCut.LongueurChuteFinale = _settings.GetDecoupeBarreLongueurChuteFinale();

            // Vérifier si un enregistrement de decoupeBarreWithCut existe
            if (decoupeBarreWithCut == null) return;

            // Tester la longueur de la chute pour identifier si chute ou déchet
            if (decoupeBarreWithCut.LongueurChuteFinale >= decoupeBarreWithCut.LongueurChuteMini)
            {
                // Créer une nouvelle chute
                var chute = new ChutesMagasin
                {
                    IdArticleInterne = decoupeBarreWithCut.IdArticleInterne,
                    IdType = 2,
                    Longueur = decoupeBarreWithCut.LongueurChuteFinale,
                    Largeur = 0,
                    Scan = 0,
                    IdOperateur = _settings.GetAppUserID(),
                    Enregistrement = _settings.GetAppDateTime(),
                    Reserve = "0",
                    Emplacement = decoupeBarreWithCut.IdEmpSc,
                    CodeBarre = decoupeBarreWithCut.CodeBarreChute,
                    Prix = decoupeBarreWithCut.DernierPrixMm * decoupeBarreWithCut.LongueurChuteFinale,
                    AttenteIntegration = true
                };

                // Ajouter un nouvel enregistrement à la table ChutesMagasin
                await _chChutesMagasin.HandleAddAsync(chute, ServiceName, nameof(BarDropValidationAsync));

                // Mettre à jour LongueurChuteFinale dans DecoupeBarre
                await UpdateDecoupeBarreLongueurChuteFinaleAsync(decoupeBarreWithCut.LongueurChuteFinale);

                // Mettre à jour les emplacement en fonction de la longueur finale
                await _dataBase.SprDecoupeBarreUpdateEmpSCAsync(_settings.GetDecoupeLotId());

                // Mettre à jour le nouvel emplacement
                var decoupeBarre = await _qhDecoupeBarre.HandleGetByIdAsync(_settings.GetDecoupeBarreId());
                decoupeBarreWithCut.EmpSc = decoupeBarre.DecoupeEmpSc;
                decoupeBarreWithCut.IdEmpSc = decoupeBarre.DecoupeIdEmpSc;

                // Imprimer l'étiquette
                _labelPrinter.PrintBarDropLabel(decoupeBarreWithCut);

                // Tester si il reste des enregistrements à traiter et si oui afficher la Page20
                await CheckIfCutToDoAsync("Page20");
            }
            else
            {
                // Convertir la chute en déchet
                await BarWasteProcessingAsync();
            }
        }

        public async Task DecoupeBarreRefuseAsync()
        {
            // Mettre à jour Découpe Barre
            await UpdateDecoupeBarreAtRefusalAsync();

            // Envoyer un message à l'application d'approvisionnement
            string? commentaires = _settings.GetDecoupeBarreDecoupeCommentaires();
            string? reference = _settings.GetDecoupeBarreWithCut().ReferenceArticle;
            string? couleur = _settings.GetDecoupeBarreWithCut().CouleurArticle;
            string? designation = _settings.GetDecoupeBarreWithCut().DesignationArticle;
            await SendDecoupeRefusalNotificationAsync(commentaires, reference, couleur, designation);

            // Mettre à jour toutes les enregistrement de DecoupeDetail
            var decoupeDetails = await _qhDecoupeDetail.HandleGetAllForDecoupeBarreIdAsync();
            if (decoupeDetails == null || decoupeDetails.Count == 0) return;

            foreach (var decoupeDetail in decoupeDetails)
            {
                await UpdateDecoupeDetailAtRefusalAsync(decoupeDetail.Id, _settings.GetDecoupeBarreDecoupeCommentaires());
            }

            // Lancer le besoin d'approvisionnement en chute
            await _barDropOptim.ExecuteAsync(_settings.GetDecoupeLotId());

            // Lancer le besoin d'approvisionnement en barre neuve
            await _barNewOptim.ExecuteAsync(_settings.GetDecoupeLotId());

            // Afficher la Page20
            _navigation.NavigateToNewPage("Page20");
        }

        public async Task DecoupeDetailRefusalAsync()
        {
            // Charger DecoupeDetailWithCut
            var decoupeDetailWithCut = _settings.GetDecoupeDetailWithCut();

            // Vérifier si un enregistrement de decoupeDetailWithCut existe
            if (decoupeDetailWithCut == null) return;

            // Mettre à jour l'enregistrement de DecoupeDetail
            await UpdateDecoupeDetailAtRefusalAsync(_settings.GetDecoupeDetailId(), _settings.GetDecoupeDetailDecoupeCommentaires());

            // Tester l'existence d'un indice de coupe = 2
            var decoupeDetailNext = await _qhDecoupeDetail.HandleGetFirstToCutAsync();
            if (decoupeDetailNext != null && decoupeDetailNext.NumLigne == (decoupeDetailWithCut.NumLigne + 1))
            {
                // Mettre à jour l'enregistrement de DecoupeDetail indice = 2
                await UpdateDecoupeDetailAtRefusalAsync(decoupeDetailNext.Id, _settings.GetDecoupeDetailDecoupeCommentaires());
            }

            // Envoyer un message à l'application d'approvisionnement
            string? commentaires = _settings.GetDecoupeDetailDecoupeCommentaires();
            string? reference = _settings.GetDecoupeDetailWithCut().Reference;
            string? couleur = _settings.GetDecoupeDetailWithCut().Couleur;
            string? designation = _settings.GetDecoupeDetailWithCut().Designation;
            await SendDecoupeRefusalNotificationAsync(commentaires, reference, couleur, designation);

            // Lancer le besoin d'approvisionnement en chute
            await _barDropOptim.ExecuteAsync(_settings.GetDecoupeLotId());

            // Lancer le besoin d'approvisionnement en barre neuve
            await _barNewOptim.ExecuteAsync(_settings.GetDecoupeLotId());

            // Mettre à jour decoupeDetailNext
            decoupeDetailNext = await _qhDecoupeDetail.HandleGetFirstToCutAsync();

            // Tester si la découpe actuelle n'est pas la dernière découpe à réaliser de la barre
            if (decoupeDetailWithCut.DecoupeBarreDecoupeNombre != decoupeDetailWithCut.DecoupeBarreIndex)
            {
                _navigation.NavigateToNewPage("Page30");
            }
            // Tester si la découpe actuelle est la dernière découpe à réaliser de la barre
            else if (decoupeDetailWithCut.DecoupeBarreDecoupeNombre == decoupeDetailWithCut.DecoupeBarreIndex)
            {
                // Mettre à jour DecoupeDarre
                await UpdateDecoupeBarreAtCutValidationAsync();

                // Lancer la procédure stockée SprCommandeClientProductionUpdateAsync()
                await _dataBase.SprCommandeClientProductionUpdateAsync();

                if (decoupeDetailWithCut.DecoupeBarreGestionChutes == true && decoupeDetailWithCut.DecoupeBarreTypeReste == "chute")
                {
                    _navigation.NavigateToNewPage("Page22");
                }
                else
                {
                    await BarWasteProcessingAsync();
                }
            }
            // Tester si il reste une découpe à réaliser
            else if (decoupeDetailNext == null)
            {
                // Terminer le lot
                await DecoupeLotCompletedAsync();
            }
            else
            {
                _navigation.NavigateToNewPage("Page30");
            }
        }

        private async Task SendDecoupeRefusalNotificationAsync(string? commentaires, string? reference, string? couleur, string? designation)
        {
            // Définir le sujet
            string subject = $"{_dictionary.GetText("Me_Re_00")} {_settings.GetDecoupeLotId()}";

            // Contenu détaillé du message
            string content = $"{_dictionary.GetText("Me_Re_01")} {_settings.GetCuttingMachine()}\n" +
                             $"{_dictionary.GetText("Me_Re_02")} {_settings.GetAppUserFullName()}\n" +
                             $"{_dictionary.GetText("Me_Re_03")} {commentaires}\n" +
                             $"{_dictionary.GetText("Me_Re_04")} {reference} - {couleur} - {designation}";

            // Envoyer un message à l'application d'approvisionnement AppID = 41
            await _messages.AddNewMessageAsync(41, subject, content);
        }
    }
}