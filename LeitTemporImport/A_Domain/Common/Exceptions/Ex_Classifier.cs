using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace LeitTemporImport.A_Domain.Common.Exceptions
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
                ArgumentNullException => new Ex_Business(callChain, "BU_ER_01", "Un paramètre obligatoire est manquant. L'opération ne peut pas se poursuivre.", ex),
                ArgumentOutOfRangeException => new Ex_Business(callChain, "BU_ER_02", "Une valeur fournie dépasse les limites autorisées. Veuillez corriger la saisie.", ex),
                ArgumentException => new Ex_Business(callChain, "BU_ER_03", "Une valeur fournie à un paramètre est invalide. Veuillez vérifier les données saisies.", ex),
                InvalidOperationException => new Ex_Business(callChain, "BU_ER_04", "L'état de l'application ne permet pas d'exécuter cette opération.", ex),
                FormatException => new Ex_Business(callChain, "BU_ER_05", "Le format d'une valeur saisie est incorrect.", ex),
                IndexOutOfRangeException => new Ex_Business(callChain, "BU_ER_06", "Une tentative d'accès à un élément inexistant a été détectée. L'indice est hors limites.", ex),
                DivideByZeroException => new Ex_Business(callChain, "BU_ER_07", "Une opération de division par zéro a été détectée. Vérifiez les données de calcul.", ex),
                NullReferenceException => new Ex_Business(callChain, "BU_ER_08", "Un élément requis est manquant ou non initialisé. Impossible de poursuivre l’opération.", ex),

                TimeoutException => new Ex_Infrastructure(callChain, "IN_ER_01", "Le délai de réponse d'un service ou de la base de données a été dépassé. Veuillez réessayer.", ex),
                FileNotFoundException => new Ex_Infrastructure(callChain, "IN_ER_02", "Le fichier demandé est introuvable. Veuillez vérifier le chemin ou le nom du fichier.", ex),
                DirectoryNotFoundException => new Ex_Infrastructure(callChain, "IN_ER_03", "Le dossier spécifié est introuvable. L'opération ne peut pas se poursuivre.", ex),
                IOException => new Ex_Infrastructure(callChain, "IN_ER_04", "Une erreur est survenue lors d'un accès aux fichiers ou périphériques de stockage.", ex),
                UnauthorizedAccessException => new Ex_Infrastructure(callChain, "IN_ER_05", "L'application n'a pas les autorisations nécessaires pour accéder à la ressource.", ex),
                DbUpdateException => new Ex_Infrastructure(callChain, "IN_ER_06", "Une erreur est survenue lors de l'enregistrement des données.", ex),
                SocketException => new Ex_Infrastructure(callChain, "IN_ER_07", "Une erreur réseau est survenue : Impossible d'établir la connexion à la base de données.", ex),

                // Cas générique : erreur non classifiée
                _ => new Ex_Infrastructure(callChain, "DI_ER_00", "L'éxécution de l'opération à échouée.", ex)
            };
        }
    }
}