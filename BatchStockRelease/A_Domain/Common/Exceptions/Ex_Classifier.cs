using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Sockets;

namespace BatchStockRelease.A_Domain.Common.Exceptions
{
    /// <summary>
    /// Classe utilitaire permettant de classifier et contextualiser une exception technique ou .NET
    /// en une exception métier (<see cref="Ex_Business"/>) ou infrastructure (<see cref="Ex_Infrastructure"/>).
    /// <para>
    /// Elle garantit la propagation cohérente des erreurs entre les couches
    /// (Services, UseCases, Infrastructure) tout en préservant la trace d’origine.
    /// </para>
    /// </summary>
    public static class Ex_Classifier
    {
        /// <summary>
        /// Classifie une exception selon sa nature (métier ou infrastructure)
        /// et l’enveloppe dans le type d’exception correspondant.
        /// </summary>
        /// <param name="callChain">Chaîne complète des appels (UseCase > Service > Méthode).</param>
        /// <param name="ex">Exception d’origine capturée.</param>
        /// <returns>
        /// Une instance contextualisée de <see cref="Ex_Business"/> ou <see cref="Ex_Infrastructure"/>.
        /// </returns>
        public static Exception Execute(string callChain, Exception ex)
        {
            // Ne pas reclassifier une exception déjà contextualisée
            if (ex is Ex_Business or Ex_Infrastructure)
                return ex;

            // Classification par type
            return ex switch
            {
                ArgumentNullException => new Ex_Business(callChain, "BU_ER_01", "No_Er_Bu_01", ex),
                ArgumentOutOfRangeException => new Ex_Business(callChain, "BU_ER_02", "No_Er_Bu_02", ex),
                ArgumentException => new Ex_Business(callChain, "BU_ER_03", "No_Er_Bu_03", ex),
                InvalidOperationException => new Ex_Business(callChain, "BU_ER_04", "No_Er_Bu_04", ex),
                FormatException => new Ex_Business(callChain, "BU_ER_05", "No_Er_Bu_05", ex),
                IndexOutOfRangeException => new Ex_Business(callChain, "BU_ER_06", "No_Er_Bu_06", ex),
                DivideByZeroException => new Ex_Business(callChain, "BU_ER_07", "No_Er_Bu_07", ex),
                NullReferenceException => new Ex_Business(callChain, "BU_ER_08", "No_Er_Bu_08", ex),

                TimeoutException => new Ex_Infrastructure(callChain, "IN_ER_01", "No_Er_In_01", ex),
                FileNotFoundException => new Ex_Infrastructure(callChain, "IN_ER_02", "No_Er_In_02", ex),
                DirectoryNotFoundException => new Ex_Infrastructure(callChain, "IN_ER_03", "No_Er_In_03", ex),
                IOException => new Ex_Infrastructure(callChain, "IN_ER_04", "No_Er_In_04", ex),
                UnauthorizedAccessException => new Ex_Infrastructure(callChain, "IN_ER_05", "No_Er_In_05", ex),
                DbUpdateException => new Ex_Infrastructure(callChain, "IN_ER_06", "No_Er_In_06", ex),
                SocketException => new Ex_Infrastructure(callChain, "IN_ER_07", "No_Er_In_07", ex),

                // Cas générique : erreur non classifiée
                _ => new Ex_Infrastructure(callChain, "DI_ER_00", "No_Er_Di_00", ex)
            };
        }
    }
}