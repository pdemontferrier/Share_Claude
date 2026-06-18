using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.B_UseCases.Services.App;

namespace DG244Cutting.A_Domain.Common.Exceptions
{
    /// <summary>
    /// Description :
    /// <para>
    /// Exception métier levée lorsqu'une règle fonctionnelle de l'application est violée.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette exception est instanciée exclusivement dans le code applicatif contrôlé :
    /// Services (<c>SR_*</c>) et UseCases (<c>UC_*</c>). Elle ne doit jamais être levée
    /// depuis la couche Infrastructure ni depuis un ViewModel.
    /// Elle est propagée vers le UseCase appelant, qui délègue son traitement à
    /// <see cref="IU_LogAndNotify"/>.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Représenter de manière explicite et contextualisée une erreur liée au domaine métier,
    /// en transportant la chaîne d'appels complète, un identifiant d'erreur normalisé
    /// et un message technique destiné à la journalisation.
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
    /// <item><description>Fournir l'<c>innerException</c> d'origine lorsqu'elle est disponible.</description></item>
    /// </list>
    ///
    /// Exemple d'utilisation :
    /// <code>
    /// throw new Ex_Business(
    ///     callChain,
    ///     Ex_Business.ErrorCodes.BU_ER_01,
    ///     "L'identifiant de la chute est obligatoire.",
    ///     originalException);
    /// </code>
    /// </summary>
    public class Ex_Business : Exception
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
        /// Identifiant normalisé de l'erreur métier.
        /// <para>
        /// Pour les erreurs produites par <see cref="SR_ExClassifier"/>, utiliser les constantes
        /// de <see cref="ErrorCodes"/>. Pour les erreurs fonctionnelles spécifiques à un module,
        /// respecter la nomenclature <c>XX_NN</c> définie dans les spécifications du projet.
        /// Utilisé pour le filtrage et la catégorisation dans les outils de supervision.
        /// </para>
        /// </summary>
        public string? ErrorId { get; }

        /// <summary>
        /// Message technique décrivant la cause de l'erreur métier.
        /// <para>
        /// Rédigé en français, destiné exclusivement aux développeurs et à l'équipe support.
        /// Ne doit jamais être affiché à l'opérateur. Alimenté directement en texte libre
        /// au moment du <c>throw</c>, sans référence au dictionnaire multilingue.
        /// </para>
        /// </summary>
        public string? ErrorException { get; }

        /// <summary>
        /// Description :
        /// <para>
        /// Initialise une nouvelle instance de <see cref="Ex_Business"/> avec le contexte
        /// complet nécessaire à la journalisation et au diagnostic.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Ce constructeur est le seul point d'entrée autorisé pour créer une exception métier
        /// dans le périmètre applicatif contrôlé. Il garantit que toute instance transportera
        /// les quatre informations requises par le pipeline de gestion des erreurs.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Produire une exception métier contextualisée, exploitable par
        /// <c>SR_ErrorLogger</c> via <c>NormalizeException</c>, sans perte d'information
        /// ni ambiguïté sur le format des données transportées.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Transmettre <paramref name="errorException"/> à la classe de base comme message principal.</description></item>
        /// <item><description>Enchaîner <paramref name="innerException"/> pour préserver la trace d'origine.</description></item>
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
        /// Message technique en français décrivant la cause de l'erreur.
        /// Destiné aux logs et à l'équipe support, jamais à l'opérateur.
        /// </param>
        /// <param name="innerException">
        /// Exception d'origine éventuelle. Doit être fournie dès qu'une exception tierce
        /// ou .NET est à l'origine de l'erreur métier, afin de conserver la trace technique complète.
        /// </param>
        public Ex_Business(
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
        /// pour les exceptions métier.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Ces codes sont assignés automatiquement par <see cref="SR_ExClassifier.Execute"/>
        /// lors de la reclassification d'exceptions .NET standard interceptées dans les Services.
        /// Ils ne couvrent pas les codes fonctionnels spécifiques aux modules métier,
        /// qui sont définis dans les classes de constantes propres à chaque module.
        /// </para>
        ///
        /// Utilisation :
        /// <code>
        /// throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "...");
        /// </code>
        /// </summary>
        public static class ErrorCodes
        {
            /// <summary>
            /// BU_ER_01 — Un paramètre obligatoire est manquant.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="ArgumentNullException"/>.</para>
            /// <para>Le paramètre concerné est extrait automatiquement et ajouté au message.</para>
            /// </summary>
            public const string BU_ER_01 = "BU_ER_01";

            /// <summary>
            /// BU_ER_02 — Une valeur fournie dépasse les limites autorisées.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="ArgumentOutOfRangeException"/>.</para>
            /// <para>Le paramètre concerné est extrait automatiquement et ajouté au message.</para>
            /// </summary>
            public const string BU_ER_02 = "BU_ER_02";

            /// <summary>
            /// BU_ER_03 — Une valeur fournie à un paramètre est invalide.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="ArgumentException"/>.</para>
            /// <para>Le paramètre concerné est extrait automatiquement et ajouté au message.</para>
            /// </summary>
            public const string BU_ER_03 = "BU_ER_03";

            /// <summary>
            /// BU_ER_04 — L'état de l'application ne permet pas d'exécuter cette opération.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="InvalidOperationException"/>.</para>
            /// </summary>
            public const string BU_ER_04 = "BU_ER_04";

            /// <summary>
            /// BU_ER_05 — Le format d'une valeur saisie est incorrect.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="FormatException"/>.</para>
            /// </summary>
            public const string BU_ER_05 = "BU_ER_05";

            /// <summary>
            /// BU_ER_06 — Une opération de division par zéro a été détectée.
            /// <para>Produit par <see cref="SR_ExClassifier"/> à partir d'une <see cref="DivideByZeroException"/>.</para>
            /// </summary>
            public const string BU_ER_06 = "BU_ER_06";
        }
    }
}