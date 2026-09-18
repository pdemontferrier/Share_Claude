using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace DG244Cutting.B_UseCases.Services.App
{
    /// <summary>
    /// Description :
    /// <para>
    /// Service responsable de la classification et de la contextualisation des exceptions
    /// .NET et tierces en exceptions typées du projet :
    /// <see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Ce service est injecté exclusivement dans les Services (<c>SR_*</c>) et invoqué
    /// dans leur bloc <c>catch (Exception ex)</c> terminal, afin de garantir qu'aucune
    /// exception brute .NET ou tierce ne traverse le pipeline de gestion des erreurs
    /// sans être contextualisée.
    /// Son implémentation réside dans <c>B_UseCases</c> et non dans <c>A_Domain</c>,
    /// ce qui lui permet de référencer librement des types tiers
    /// (EF Core, sockets, IO) sans violer les principes de la Clean Architecture.
    /// Il ne reclassifie pas les exceptions déjà typées
    /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
    /// <see cref="Ex_Unclassified"/>), qu'il retourne immédiatement sans modification.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Centraliser la logique de classification des exceptions non prévues par le code
    /// applicatif, en enrichissant chaque cas avec les informations techniques disponibles
    /// (paramètre concerné, nom de fichier, message SQL, entités EF Core, code socket)
    /// afin de maximiser la capacité de diagnostic en exploitation.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// Services (<c>SR_*</c>) uniquement, via l'interface <see cref="IS_ExClassifier"/>.
    /// Ce service ne doit pas être appelé depuis les UseCases, ViewModels ou l'Infrastructure.
    /// </para>
    ///
    /// Règles d'utilisation :
    /// <list type="bullet">
    /// <item><description>Injecter via <see cref="IS_ExClassifier"/> dans les Services concernés.</description></item>
    /// <item><description>Appeler <see cref="Execute"/> uniquement depuis un <c>catch (Exception ex)</c> terminal d'un Service.</description></item>
    /// <item><description>Toujours utiliser le résultat dans un <c>throw</c> immédiat.</description></item>
    /// </list>
    ///
    /// Exemple d'utilisation :
    /// <code>
    /// catch (Exception ex)
    /// {
    ///     throw _classifier.Execute(callChain, ex);
    /// }
    /// </code>
    /// </summary>
    public class SR_ExClassifier : IS_ExClassifier
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Description :
        /// <para>
        /// Initialise une nouvelle instance du service <see cref="SR_ExClassifier"/>.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c> pour la traçabilité.</description></item>
        /// </list>
        /// </summary>
        public SR_ExClassifier()
        {
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>
        /// Classifie une exception selon sa nature et l'enveloppe dans le type d'exception
        /// correspondant, enrichi des informations techniques disponibles.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette méthode constitue le point d'entrée unique du service.
        /// Elle est invoquée depuis le bloc <c>catch (Exception ex)</c> terminal des Services,
        /// lorsqu'une exception non contrôlée doit être normalisée avant de remonter
        /// vers le UseCase appelant.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner systématiquement une exception typée et contextualisée,
        /// en préservant l'exception d'origine en <c>innerException</c>
        /// et en enrichissant le message avec les propriétés spécifiques
        /// du type .NET intercepté.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Retourner immédiatement toute exception déjà typée sans reclassification.</description></item>
        /// <item><description>Classifier les exceptions .NET connues en <see cref="Ex_Business"/> ou <see cref="Ex_Infrastructure"/>.</description></item>
        /// <item><description>Enrichir chaque cas avec les propriétés techniques disponibles via les helpers privés.</description></item>
        /// <item><description>Produire une <see cref="Ex_Unclassified"/> pour tout cas non reconnu.</description></item>
        /// </list>
        /// </summary>
        /// <param name="callChain">
        /// Chaîne d'appels complète transmise par le Service appelant au moment de l'interception.
        /// </param>
        /// <param name="ex">
        /// Exception d'origine à classifier. Ne doit pas être nulle.
        /// </param>
        /// <returns>
        /// Une instance contextualisée de <see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>,
        /// avec l'exception d'origine enchaînée en <c>innerException</c>.
        /// </returns>
        public Exception Execute(string callChain, Exception ex)
        {
            if (ex is Ex_Business or Ex_Infrastructure or Ex_Unclassified)
                return ex;

            return ex switch
            {
                ArgumentNullException anex => new Ex_Business(callChain, "BU_ER_01",
                                                        BuildArgumentDetail("Un paramètre obligatoire est manquant.", anex.ParamName), anex),

                ArgumentOutOfRangeException aoex => new Ex_Business(callChain, "BU_ER_02",
                                                        BuildArgumentDetail("Une valeur fournie dépasse les limites autorisées.", aoex.ParamName), aoex),

                ArgumentException aex => new Ex_Business(callChain, "BU_ER_03",
                                                        BuildArgumentDetail("Une valeur fournie à un paramètre est invalide.", aex.ParamName), aex),

                InvalidOperationException => new Ex_Business(callChain, "BU_ER_04",
                                                        "L'état de l'application ne permet pas d'exécuter cette opération.", ex),

                FormatException => new Ex_Business(callChain, "BU_ER_05",
                                                        "Le format d'une valeur saisie est incorrect.", ex),

                DivideByZeroException => new Ex_Business(callChain, "BU_ER_06",
                                                        "Une opération de division par zéro a été détectée. Vérifiez les données de calcul.", ex),

                TimeoutException => new Ex_Infrastructure(callChain, "IN_ER_01",
                                                        "Le délai de réponse d'un service ou de la base de données a été dépassé.", ex),

                FileNotFoundException fnex => new Ex_Infrastructure(callChain, "IN_ER_02",
                                                        BuildFileDetail("Le fichier demandé est introuvable.", fnex.FileName), fnex),

                DirectoryNotFoundException => new Ex_Infrastructure(callChain, "IN_ER_03",
                                                        "Le dossier spécifié est introuvable.", ex),

                IOException => new Ex_Infrastructure(callChain, "IN_ER_04",
                                                        "Une erreur est survenue lors d'un accès aux fichiers ou périphériques de stockage.", ex),

                UnauthorizedAccessException => new Ex_Infrastructure(callChain, "IN_ER_05",
                                                        "L'application ne dispose pas des autorisations nécessaires pour accéder à la ressource demandée.", ex),

                DbUpdateException dbex => new Ex_Infrastructure(callChain, "IN_ER_06",
                                                        BuildDbUpdateDetail(dbex), dbex),

                SocketException sex => new Ex_Infrastructure(callChain, "IN_ER_07",
                                                        BuildSocketDetail(sex), sex),

                _ => new Ex_Unclassified(callChain, Ex_Unclassified.ErrorCodes.UN_ER_00,
                                                        $"Exception non classifiée : {ex.GetType().Name} — {ex.Message}", ex)
            };
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Description :
        /// <para>
        /// Enrichit le message d'une exception <see cref="ArgumentException"/> avec le nom
        /// du paramètre concerné si disponible.
        /// </para>
        /// </summary>
        /// <param name="baseMessage">Message de base décrivant la nature de l'erreur.</param>
        /// <param name="paramName">Nom du paramètre incriminé, extrait de <c>ArgumentException.ParamName</c>.</param>
        /// <returns>Message enrichi ou message de base si <paramref name="paramName"/> est absent.</returns>
        private static string BuildArgumentDetail(string baseMessage, string? paramName)
        {
            if (string.IsNullOrWhiteSpace(paramName))
                return baseMessage;

            return $"{baseMessage} Paramètre concerné : {paramName}.";
        }

        /// <summary>
        /// Description :
        /// <para>
        /// Enrichit le message d'une <see cref="FileNotFoundException"/> avec le chemin
        /// du fichier manquant si disponible.
        /// </para>
        /// </summary>
        /// <param name="baseMessage">Message de base décrivant la nature de l'erreur.</param>
        /// <param name="fileName">Chemin du fichier manquant, extrait de <c>FileNotFoundException.FileName</c>.</param>
        /// <returns>Message enrichi ou message de base si <paramref name="fileName"/> est absent.</returns>
        private static string BuildFileDetail(string baseMessage, string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return baseMessage;

            return $"{baseMessage} Fichier : {fileName}.";
        }

        /// <summary>
        /// Description :
        /// <para>
        /// Construit un message de diagnostic enrichi pour une <see cref="DbUpdateException"/>,
        /// en extrayant le message de l'exception SQL sous-jacente et les noms des entités
        /// EF Core impliquées dans l'échec de persistance.
        /// </para>
        /// </summary>
        /// <param name="ex">Exception EF Core à analyser.</param>
        /// <returns>Message enrichi incluant la cause SQL et les entités concernées.</returns>
        private static string BuildDbUpdateDetail(DbUpdateException ex)
        {
            var sb = new StringBuilder("Échec de la persistance en base de données.");

            if (ex.InnerException is not null)
                sb.Append($" Cause : {ex.InnerException.Message}");

            if (ex.Entries.Count > 0)
            {
                var entityNames = ex.Entries
                    .Select(e => e.Entity.GetType().Name)
                    .Distinct();

                sb.Append($" Entité(s) concernée(s) : {string.Join(", ", entityNames)}.");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Description :
        /// <para>
        /// Construit un message de diagnostic enrichi pour une <see cref="SocketException"/>,
        /// en exposant le code d'erreur réseau normalisé aux côtés du message d'origine.
        /// </para>
        /// </summary>
        /// <param name="ex">Exception socket à analyser.</param>
        /// <returns>Message incluant le <c>SocketErrorCode</c> et le message d'origine.</returns>
        private static string BuildSocketDetail(SocketException ex)
        {
            return $"Erreur réseau (code : {ex.SocketErrorCode}) — {ex.Message}";
        }

        #endregion
    }
}