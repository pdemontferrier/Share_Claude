using OfficeOpenXml;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using System.IO;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Implémentation du service d’écriture des lignes de découpe dans un fichier Excel.
    /// </summary>
    public class SR_BatchDoc_AddDecoupeDetailToExcel : IS_BatchDoc_AddDecoupeDetailToExcel
    {
        private readonly string _callee;
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;

        public SR_BatchDoc_AddDecoupeDetailToExcel(
            IQ_DecoupeDetail qhDecoupeDetail)
        {
            _qhDecoupeDetail = qhDecoupeDetail;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            int decoupeLotId,
            string filePath,
            string fileSheet1,
            string decoupeLotDesignation,
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    throw new Ex_Business(callChain, $"DOC_06 - Path : {filePath}", "No_Er_Bu_53");

                // Récupérer les données depuis la base de données
                var data = await _qhDecoupeDetail.HandleGetBatchDecoupeDetailAsync(decoupeLotId);

                // Configurer le contexte de licence pour EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(new FileInfo(filePath));
                var worksheet = package.Workbook.Worksheets[fileSheet1];

                // Débuter l'écriture des données à partir de la deuxième ligne
                int row = 2;

                foreach (var line in data)
                {
                    worksheet.Cells[row, 1].Value = line.Id;
                    worksheet.Cells[row, 2].Value = line.IdCommandeClient;
                    worksheet.Cells[row, 3].Value = line.NumProjet;
                    worksheet.Cells[row, 4].Value = line.NomProjet;
                    worksheet.Cells[row, 5].Value = line.Structure;
                    worksheet.Cells[row, 6].Value = line.Position;
                    worksheet.Cells[row, 7].Value = line.NumLigne;
                    worksheet.Cells[row, 8].Value = line.IndiceDecoupe;
                    worksheet.Cells[row, 9].Value = line.IdArticleInterne;
                    worksheet.Cells[row, 10].Value = line.Reference;
                    worksheet.Cells[row, 11].Value = line.Couleur;
                    worksheet.Cells[row, 12].Value = line.Designation;
                    worksheet.Cells[row, 13].Value = line.LongueurBarre;
                    worksheet.Cells[row, 14].Value = line.LongueurChuteMini;
                    worksheet.Cells[row, 15].Value = line.Quantite;
                    worksheet.Cells[row, 16].Value = line.LongueurOptim;
                    worksheet.Cells[row, 17].Value = line.LongueurDecoupe;
                    worksheet.Cells[row, 18].Value = line.Inclinaison1;
                    worksheet.Cells[row, 19].Value = line.Pivot1;
                    worksheet.Cells[row, 20].Value = line.Pivot2;
                    worksheet.Cells[row, 21].Value = line.Inclinaison2;
                    worksheet.Cells[row, 22].Value = line.ReferenceVue;
                    worksheet.Cells[row, 23].Value = line.RefCommentaires;
                    worksheet.Cells[row, 24].Value = line.Commentaires;
                    worksheet.Cells[row, 25].Value = line.Inactif;
                    worksheet.Cells[row, 26].Value = line.ValidationLigne;
                    worksheet.Cells[row, 27].Value = line.TypeHistorique;
                    worksheet.Cells[row, 28].Value = line.AdvUtilisateurPoste;
                    worksheet.Cells[row, 29].Value = line.AdvUtilisateurErp;
                    worksheet.Cells[row, 30].Value = line.AdvPosteMachineId;
                    worksheet.Cells[row, 31].Value = line.AdvPosteMachineIp;
                    worksheet.Cells[row, 32].Value = line.Source;
                    worksheet.Cells[row, 33].Value = line.MessageElumatec;
                    worksheet.Cells[row, 34].Value = line.Categorie1;
                    worksheet.Cells[row, 35].Value = line.Categorie2;
                    worksheet.Cells[row, 36].Value = line.Categorie3;
                    worksheet.Cells[row, 37].Value = line.Categorie4;
                    worksheet.Cells[row, 38].Value = line.OrdreTri;
                    worksheet.Cells[row, 39].Value = line.IdDecoupeLot;
                    worksheet.Cells[row, 40].Value = decoupeLotDesignation;
                    worksheet.Cells[row, 41].Value = line.IdDecoupeBarre;
                    worksheet.Cells[row, 42].Value = line.DecoupeBarreIndex;
                    worksheet.Cells[row, 43].Value = line.DecoupeLongueurReste;
                    worksheet.Cells[row, 44].Value = string.Empty;
                    worksheet.Cells[row, 45].Value = line.ApproOptimBarreChute;
                    worksheet.Cells[row, 46].Value = line.ApproOptimBarreNeuve;
                    row++;
                }

                package.Save();
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}