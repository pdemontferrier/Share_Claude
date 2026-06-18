using DG244Cutting.B_UseCases.Services.App;

namespace DG244Cutting.A_Domain.Common.Exceptions
{
    /// <summary>
    /// Description :
    /// <para>
    /// Exception de dernier recours représentant une erreur dont la nature n'a pas pu être
    /// déterminée par le service <see cref="SR_ExClassifier"/>.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette exception est produite exclusivement par
    /// <see cref="SR_ExClassifier"/>
    /// lorsqu'aucun cas de classification connu ne correspond à l'exception reçue.
    /// Elle ne doit jamais être instanciée directement dans le code applicatif :
    /// Services (<c>SR_*</c>), UseCases (<c>UC_*</c>), ViewModels (<c>VM_*</c>) ou
    /// Infrastructure (<c>C_Infrastructure</c>).
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Nommer honnêtement une situation non prévue par l'architecture, en la distinguant
    /// clairement des erreurs métier (<see cref="Ex_Business"/>) et des défaillances
    /// infrastructure connues (<see cref="Ex_Infrastructure"/>).
    /// Garantir qu'aucune exception brute .NET ne traverse le pipeline de gestion
    /// des erreurs sans être contextualisée.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// Développeurs et équipe support, via les logs CSV et la table <c>UserAppErrorLog</c>.
    /// La présence d'une <see cref="Ex_Unclassified"/> dans les logs constitue un signal
    /// d'amélioration : elle indique qu'un cas non prévu doit être analysé et intégré
    /// dans <see cref="SR_ExClassifier"/> ou traité
    /// explicitement dans le code.
    /// </para>
    ///
    /// Règles d'utilisation :
    /// <list type="bullet">
    /// <item><description>Ne jamais instancier cette classe directement dans le code applicatif.</description></item>
    /// <item><description>Toute instance provient exclusivement de <see cref="SR_ExClassifier"/>.</description></item>
    /// <item><description>Sa présence dans les logs est un indicateur de dette de classification à traiter.</description></item>
    /// <item><description>Dans les UseCases, intercepter via <c>catch (Ex_Unclassified ex)</c> distinct
    /// de <c>catch (Ex_Business ex)</c> et <c>catch (Ex_Infrastructure ex)</c>.</description></item>
    /// </list>
    ///
    /// Exemple de catch attendu dans un UseCase :
    /// <code>
    /// catch (Ex_Business ex)       { await _ucLogAndNotify.ExecuteAsync(callChain, "No_EC_18", ex); }
    /// catch (Ex_Infrastructure ex) { await _ucLogAndNotify.ExecuteAsync(callChain, "No_EC_19", ex); }
    /// catch (Ex_Unclassified ex)   { await _ucLogAndNotify.ExecuteAsync(callChain, "No_EC_20", ex); }
    /// </code>
    /// </summary>
    public class Ex_Unclassified : Exception
    {
        /// <summary>
        /// Chaîne d'appels complète au moment où l'exception d'origine a été interceptée
        /// par <see cref="SR_ExClassifier"/>.
        /// <para>
        /// Construite par concaténation des noms de classe et de méthode à chaque niveau.
        /// Permet de retracer le point d'entrée dans le classificateur.
        /// </para>
        /// </summary>
        public string? CallChain { get; }

        /// <summary>
        /// Identifiant normalisé assigné par
        /// <see cref="SR_ExClassifier"/> au cas non classifié.
        /// <para>
        /// Respecte la nomenclature <c>UN_ER_NN</c> définie dans les spécifications du projet.
        /// Permet de repérer et de comptabiliser les occurrences non classifiées
        /// dans les outils de supervision.
        /// </para>
        /// </summary>
        public string? ErrorId { get; }

        /// <summary>
        /// Message technique généré par
        /// <see cref="SR_ExClassifier"/>
        /// décrivant l'exception d'origine non reconnue.
        /// <para>
        /// Inclut systématiquement le type .NET et le message de l'exception source
        /// afin de maximiser l'information disponible pour le diagnostic.
        /// Le détail complet de la trace d'origine est accessible via
        /// <see cref="Exception.InnerException"/>.
        /// </para>
        /// </summary>
        public string? ErrorException { get; }

        /// <summary>
        /// Description :
        /// <para>
        /// Initialise une nouvelle instance de <see cref="Ex_Unclassified"/>.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Ce constructeur est <c>public</c> pour permettre à
        /// <see cref="SR_ExClassifier"/>,
        /// situé dans un assembly distinct de <c>A_Domain</c>, d'instancier cette exception.
        /// Par convention architecturale, il ne doit être appelé que depuis
        /// <see cref="SR_ExClassifier"/>.
        /// Tout autre usage constitue une violation des spécifications du projet.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Produire une instance contextualisée transportant la trace complète
        /// de l'exception d'origine non reconnue, exploitable par <c>SR_ErrorLogger</c>
        /// via <c>NormalizeException</c>.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Transmettre <paramref name="errorException"/> à la classe de base comme message principal.</description></item>
        /// <item><description>Enchaîner <paramref name="innerException"/> pour conserver la trace technique intégrale.</description></item>
        /// <item><description>Initialiser les propriétés <see cref="CallChain"/>, <see cref="ErrorId"/> et <see cref="ErrorException"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="callChain">
        /// Chaîne d'appels transmise par le Service appelant au moment de l'interception
        /// par <see cref="SR_ExClassifier"/>.
        /// </param>
        /// <param name="errorId">
        /// Identifiant du cas non classifié. Voir <see cref="ErrorCodes"/> pour le code assigné.
        /// </param>
        /// <param name="errorException">
        /// Message technique décrivant l'exception d'origine non reconnue.
        /// Généré par <see cref="SR_ExClassifier"/>
        /// en incluant le type .NET et le message source.
        /// </param>
        /// <param name="innerException">
        /// Exception .NET d'origine, systématiquement fournie par
        /// <see cref="SR_ExClassifier"/>
        /// afin de préserver la stack trace et le contexte technique complet.
        /// </param>
        public Ex_Unclassified(
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
        /// Référentiel des identifiants d'erreur produits par
        /// <see cref="SR_ExClassifier"/>
        /// pour les exceptions non classifiées.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Ces codes sont assignés par <see cref="SR_ExClassifier"/>
        /// lorsqu'aucun type .NET connu ne correspond à l'exception interceptée.
        /// Chaque occurrence dans les logs constitue un signal de dette de classification
        /// à analyser et à intégrer dans
        /// <see cref="SR_ExClassifier"/>
        /// lors d'une prochaine itération de maintenance.
        /// </para>
        /// </summary>
        public static class ErrorCodes
        {
            /// <summary>
            /// UN_ER_00 — Exception non classifiée : type .NET non reconnu par
            /// <see cref="SR_ExClassifier"/>.
            /// <para>
            /// Le type .NET et le message de l'exception d'origine sont inclus automatiquement
            /// dans le message pour faciliter le diagnostic.
            /// </para>
            /// <para>
            /// Toute occurrence de ce code dans les logs doit déclencher une analyse visant
            /// à intégrer le type concerné dans
            /// <see cref="SR_ExClassifier"/> ou à le traiter
            /// explicitement dans le Service d'origine.
            /// </para>
            /// </summary>
            public const string UN_ER_00 = "UN_ER_00";
        }
    }
}