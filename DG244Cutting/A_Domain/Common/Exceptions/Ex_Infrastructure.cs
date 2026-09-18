using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.B_UseCases.Services.App;

namespace DG244Cutting.A_Domain.Common.Exceptions
{
    /// <summary>
    /// Description :
    /// <para>
    /// Exception technique levée lorsqu'une défaillance infrastructure survient
    /// dans un périmètre connu et prévu par le code applicatif.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette exception est instanciée exclusivement dans le code applicatif contrôlé :
    /// Services (<c>SR_*</c>) et UseCases (<c>UC_*</c>), lorsqu'une erreur technique
    /// prévisible est détectée (accès base de données, fichier, réseau, système externe).
    /// Elle couvre également les exceptions .NET de frontière reclassifiées par
    /// <see cref="SR_ExClassifier"/> depuis des bibliothèques tierces
    /// (EF Core, IO, sockets, COM).
    /// Elle est propagée vers le UseCase appelant, qui délègue son traitement à
    /// <see cref="IU_LogAndNotify"/>.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Représenter de manière explicite et contextualisée une erreur liée à la couche
    /// technique ou aux systèmes externes, en transportant la chaîne d'appels complète,
    /// un identifiant d'erreur normalisé et un message technique destiné à la journalisation.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// Développeurs et équipe support, via les logs CSV et la table <c>UserAppErrorLog</c>.
    /// Ce message n'est jamais affiché directement à l'opérateur.
    /// </para>
    ///
    /// Règles d'utilisation :
    /// <list type="bullet">
    /// <item><description>Utiliser exclusivement le constructeur enrichi à quatre paramètres.</description></item>
    /// <item><description>Renseigner un <c>errorId</c> issu de <see cref="ErrorCodes"/> ou conforme à la nomenclature fonctionnelle du module.</description></item>
    /// <item><description>Rédiger <c>errorException</c> en français, en texte libre et explicite.</description></item>
    /// <item><description>Fournir systématiquement l'<c>innerException</c> d'origine afin de préserver
    /// le détail technique complet (message SQL, code socket, entrée EF Core, etc.).</description></item>
    /// </list>
    ///
    /// Exemple d'utilisation :
    /// <code>
    /// throw new Ex_Infrastructure(
    ///     callChain,
    ///     Ex_Infrastructure.ErrorCodes.IN_ER_06,
    ///     "Échec de la persistance de l'archive en base de données.",
    ///     dbUpdateException);
    /// </code>
    /// </summary>
    public class Ex_Infrastructure : Exception
    {
        /// <summary>
        /// Chaîne d'appels complète au moment où l'exception a été levée.
        /// <para>
        /// Construite par concaténation des noms de classe et de méthode à chaque niveau
        /// (ViewModel → UseCase → Service → Méthode).
        /// Permet de retracer l'origine exacte de l'erreur dans les logs.
        /// </para>
        /// </summary>
        public string? CallChain { get; }

        /// <summary>
        /// Identifiant normalisé de l'erreur infrastructure.
        /// <para>
        /// Pour les erreurs produites par <see cref="SR_ExClassifier"/>, utiliser les constantes
        /// de <see cref="ErrorCodes"/>. Pour les erreurs fonctionnelles spécifiques à un module,
        /// respecter la nomenclature <c>XX_NN</c> définie dans les spécifications du projet.
        /// Utilisé pour le filtrage et la catégorisation dans les outils de supervision.
        /// </para>
        /// </summary>
        public string? ErrorId { get; }

        /// <summary>
        /// Message technique décrivant la cause de la défaillance infrastructure.
        /// <para>
        /// Rédigé en français, destiné exclusivement aux développeurs et à l'équipe support.
        /// Ne doit jamais être affiché à l'opérateur. Alimenté directement en texte libre
        /// au moment du <c>throw</c>, sans référence au dictionnaire multilingue.
        /// Le détail technique complet de la cause sous-jacente est accessible via
        /// <see cref="Exception.InnerException"/>.
        /// </para>
        /// </summary>
        public string? ErrorException { get; }

        /// <summary>
        /// Description :
        /// <para>
        /// Initialise une nouvelle instance de <see cref="Ex_Infrastructure"/> avec le contexte
        /// complet nécessaire à la journalisation et au diagnostic technique.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Ce constructeur est le seul point d'entrée autorisé pour créer une exception
        /// infrastructure dans le périmètre applicatif contrôlé. Il garantit que toute instance
        /// transportera les quatre informations requises par le pipeline de gestion des erreurs.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Produire une exception technique contextualisée, exploitable par
        /// <c>SR_ErrorLogger</c> via <c>NormalizeException</c>, en préservant
        /// impérativement la trace de l'exception d'origine pour un diagnostic complet
        /// en exploitation.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Transmettre <paramref name="errorException"/> à la classe de base comme message principal.</description></item>
        /// <item><description>Enchaîner <paramref name="innerException"/> pour préserver la trace technique complète.</description></item>
        /// <item><description>Initialiser les propriétés <see cref="CallChain"/>, <see cref="ErrorId"/> et <see cref="ErrorException"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="callChain">
        /// Chaîne d'appels complète au moment du <c>throw</c>.
        /// Construite par le Service ou le UseCase appelant via le mécanisme <c>CallChain</c> du projet.
        /// </param>
        /// <param name="errorId">
        /// Identifiant normalisé de l'erreur. Utiliser <see cref="ErrorCodes"/> pour les codes
        /// génériques, ou la nomenclature fonctionnelle du module pour les codes spécifiques.
        /// </param>
        /// <param name="errorException">
        /// Message technique en français décrivant la nature de la défaillance.
        /// Destiné aux logs et à l'équipe support, jamais à l'opérateur.
        /// </param>
        /// <param name="innerException">
        /// Exception d'origine. Doit être fournie systématiquement afin de conserver
        /// le détail technique complet : message SQL, code réseau, entrées EF Core, etc.
        /// Son absence appauvrit significativement la capacité de diagnostic en exploitation.
        /// </param>
        public Ex_Infrastructure(
            string callChain,
            string errorId,
            string errorException,
            Exception? innerException = null)
            : base(errorException, innerException)
        {
            CallChain = callChain;
            ErrorId = errorId;
            ErrorException = errorException;
        }

        /// <summary>
        /// Description :
        /// <para>
        /// Référentiel des identifiants d'erreur génériques produits par <see cref="SR_ExClassifier"/>
        /// pour les exceptions infrastructure.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Ces codes sont assignés automatiquement par <see cref="SR_ExClassifier.Execute"/>
        /// lors de la reclassification d'exceptions .NET ou tierces interceptées dans les Services.
        /// Ils couvrent les défaillances techniques prévisibles : base de données, fichiers,
        /// réseau et autorisations.
        /// Ils ne couvrent pas les codes fonctionnels spécifiques aux modules métier,
        /// qui sont définis dans les classes de constantes propres à chaque module.
        /// </para>
        ///
        /// Utilisation :
        /// <code>
        /// throw new Ex_Infrastructure(callChain, Ex_Infrastructure.ErrorCodes.IN_ER_06, "...", innerEx);
        /// </code>
        /// </summary>
        public static class ErrorCodes
        {
            /// <summary>
            /// IN_ER_01 — Le délai de réponse d'un service ou de la base de données a été dépassé.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="TimeoutException"/>.</para>
            /// </summary>
            public const string IN_ER_01 = "IN_ER_01";

            /// <summary>
            /// IN_ER_02 — Le fichier demandé est introuvable.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="System.IO.FileNotFoundException"/>.</para>
            /// <para>Le chemin du fichier manquant est extrait automatiquement et ajouté au message.</para>
            /// </summary>
            public const string IN_ER_02 = "IN_ER_02";

            /// <summary>
            /// IN_ER_03 — Le dossier spécifié est introuvable.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="System.IO.DirectoryNotFoundException"/>.</para>
            /// </summary>
            public const string IN_ER_03 = "IN_ER_03";

            /// <summary>
            /// IN_ER_04 — Une erreur est survenue lors d'un accès aux fichiers ou périphériques de stockage.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="System.IO.IOException"/>.</para>
            /// </summary>
            public const string IN_ER_04 = "IN_ER_04";

            /// <summary>
            /// IN_ER_05 — L'application ne dispose pas des autorisations nécessaires pour accéder à la ressource demandée.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="UnauthorizedAccessException"/>.</para>
            /// </summary>
            public const string IN_ER_05 = "IN_ER_05";

            /// <summary>
            /// IN_ER_06 — Échec de la persistance en base de données.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="Microsoft.EntityFrameworkCore.DbUpdateException"/>.</para>
            /// <para>Le message SQL sous-jacent et les entités EF Core concernées sont extraits automatiquement et ajoutés au message.</para>
            /// </summary>
            public const string IN_ER_06 = "IN_ER_06";

            /// <summary>
            /// IN_ER_07 — Erreur réseau lors de la tentative de connexion.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="System.Net.Sockets.SocketException"/>.</para>
            /// <para>Le code d'erreur réseau (<c>SocketErrorCode</c>) est extrait automatiquement et ajouté au message.</para>
            /// </summary>
            public const string IN_ER_07 = "IN_ER_07";
        }
    }
}