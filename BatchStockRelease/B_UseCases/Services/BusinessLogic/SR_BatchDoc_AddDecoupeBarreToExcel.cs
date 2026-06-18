using System.IO;
using OfficeOpenXml;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Implémentation du service d’écriture des données de découpe des barres dans un fichier Excel.
    /// </summary>
    public class SR_BatchDoc_AddDecoupeBarreToExcel : IS_BatchDoc_AddDecoupeBarreToExcel
    {
        private readonly string _callee;
        private readonly IQ_DecoupeBarreDetails _qhDecoupeBarreDetails;

        public SR_BatchDoc_AddDecoupeBarreToExcel(
            IQ_DecoupeBarreDetails qhDecoupeBarreDetails)
        {
            _qhDecoupeBarreDetails = qhDecoupeBarreDetails;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            int decoupeLotId,
            string filePath,
            string fileSheet2,
            string decoupeLotDesignation,
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    throw new Ex_Business(callChain, $"DOC_07 - Path : {filePath}", "No_Er_Bu_53");

                // Récupérer les données depuis la base de données
                var data = await _qhDecoupeBarreDetails.HandleGetAsync(decoupeLotId);

                // Configurer le contexte de licence pour EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(new FileInfo(filePath));
                var worksheet = package.Workbook.Worksheets[fileSheet2];

                // Débuter l'écriture des données à partir de la deuxième ligne
                int row = 2;

                foreach (var line in data)
                {
                    worksheet.Cells[row, 1].Value = line.Id;
                    worksheet.Cells[row, 2].Value = line.IdArticleInterne;
                    worksheet.Cells[row, 3].Value = line.Reference;
                    worksheet.Cells[row, 4].Value = line.Couleur;
                    worksheet.Cells[row, 5].Value = line.Designation;
                    worksheet.Cells[row, 6].Value = line.IdDecoupeLot;
                    worksheet.Cells[row, 7].Value = decoupeLotDesignation;
                    worksheet.Cells[row, 8].Value = line.ApproOrigine;
                    worksheet.Cells[row, 9].Value = line.LongueurBarre;
                    worksheet.Cells[row, 10].Value = line.LongueurChuteMini;
                    worksheet.Cells[row, 11].Value = line.Categorie1;
                    worksheet.Cells[row, 12].Value = line.Categorie2;
                    worksheet.Cells[row, 13].Value = line.Categorie3;
                    worksheet.Cells[row, 14].Value = line.Categorie4;
                    worksheet.Cells[row, 15].Value = line.OrdreTri;
                    worksheet.Cells[row, 16].Value = line.DecoupeNombre;
                    worksheet.Cells[row, 17].Value = line.DecoupeLongueurReste;
                    worksheet.Cells[row, 18].Value = line.DecoupeTypeReste;
                    worksheet.Cells[row, 19].Value = line.ApproInactif;
                    row++;
                }

                // Sauvegarder les modifications
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