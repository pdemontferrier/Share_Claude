using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using System.IO;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé de générer et copier les fichiers .elu de découpe pour un lot donné.
    /// </summary>
    public class SR_BatchDoc_AddEluFiles : IS_BatchDoc_AddEluFiles
    {
        private readonly string _callee;
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;

        public SR_BatchDoc_AddEluFiles(IQ_DecoupeDetail qhDecoupeDetail)
        {
            _qhDecoupeDetail = qhDecoupeDetail;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            int decoupeLotId, 
            string batchDirectoryPath,
            string batchFileName,
            string directoryPathServer2,
            string directoryPathElumatec,
            string directoryPathElumatec1,
            string directoryPathElumatec2,
            string directoryPathElumatec3,
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Utiliser le répertoire du projet spécifié dans AppSettings
                if (string.IsNullOrEmpty(batchDirectoryPath) || !Directory.Exists(batchDirectoryPath))
                {
                    throw new Ex_Business(callChain, "DOC_03", "No_Er_Bu_50");
                }

                // Récupérer toutes les valeurs distinctes de Categorie4 pour le projet sélectionné
                var categorie4Values = await _qhDecoupeDetail.HandleGetCuttingMachineDGListToBeSuppliedAsync(decoupeLotId);

                // Pour chaque valeur de Categorie4, créer un fichier texte
                foreach (var categorie4 in categorie4Values)
                {
                    if (categorie4 != null)
                    {
                        // Récupérer les messages Elumatec pour chaque valeur de Categorie4
                        var messages = await _qhDecoupeDetail.HandleGetBatchDecoupeDetailMessageElumatecAsync(decoupeLotId, categorie4);

                        // Définir le nom et le chemin complet du fichier .elu
                        string fileName = $"{batchFileName}_{categorie4}.elu";
                        string filePath = Path.Combine(batchDirectoryPath, fileName);

                        // Écrire les messages dans le fichier .elu
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            writer.WriteLine("\u0002" + "1000000000000" + "                " + "\u0003");
                            foreach (var message in messages)
                                writer.WriteLine(message);
                        }

                        // Copier le fichier .elu dans le dossier machine correspondant
                        string destinationDirectory = categorie4 switch
                        {
                            "DG244_01" => Path.Combine(directoryPathServer2, directoryPathElumatec, directoryPathElumatec1),
                            "DG244_02" => Path.Combine(directoryPathServer2, directoryPathElumatec, directoryPathElumatec2),
                            "DG244_03" => Path.Combine(directoryPathServer2, directoryPathElumatec, directoryPathElumatec3),
                            _ => string.Empty
                        };

                        if (string.IsNullOrEmpty(destinationDirectory))
                        {
                            throw new Ex_Business(callChain, $"DOC_04 - Machine id : {categorie4}", "No_Er_Bu_51");
                        }
                        if (!Directory.Exists(destinationDirectory))
                        {
                            throw new Ex_Business(callChain, $"DOC_05 - Machine id : {categorie4}", "No_Er_Bu_52");
                        }

                        if (!string.IsNullOrEmpty(destinationDirectory) && Directory.Exists(destinationDirectory))
                        {
                            // Chemin complet du fichier de destination
                            string destinationFilePath = Path.Combine(destinationDirectory, fileName);

                            // Copier le fichier
                            File.Copy(filePath, destinationFilePath, overwrite: true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}
