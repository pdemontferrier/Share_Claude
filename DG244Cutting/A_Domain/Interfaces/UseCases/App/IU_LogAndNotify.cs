using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase responsable de la gestion centralisée de la journalisation
    /// et de la notification des erreurs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Ce UseCase est appelé exclusivement depuis les UseCases (<c>UC_*</c>) dans
    /// leurs blocs <c>catch</c>, après réception d'une exception typée du projet
    /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
    /// Il peut également être appelé depuis <c>VM_Page_Generic.ExecuteSafeAsync</c> qui agit comme
    /// filet de sécurité de dernier recours au niveau des ViewModels.
    /// </para>
    /// <para>
    /// Objectif : Fournir un point d'entrée unique et robuste pour le traitement terminal
    /// des erreurs, garantissant que journal et notification reçoivent des données cohérentes
    /// construites dans une séquence déterministe et résistante aux défaillances internes.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Construire le détail technique à partir des propriétés de l'exception typée.</description></item>
    ///   <item><description>Traduire la clé dictionnaire en message destiné à l'opérateur.</description></item>
    ///   <item><description>Déléguer la journalisation technique à <see cref="IS_ErrorLogger"/>.</description></item>
    ///   <item><description>Déléguer la notification opérateur à <see cref="IS_Notification"/>.</description></item>
    ///   <item><description>Garantir le principe best-effort : ne jamais interrompre le flux appelant.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne classifie pas les exceptions : ce rôle appartient à <see cref="IS_ExClassifier"/>.</description></item>
    ///   <item><description>Ne construit pas la CallChain : cette responsabilité appartient à chaque composant.</description></item>
    ///   <item><description>Ne persiste pas directement les logs : cette responsabilité est déléguée à <see cref="IS_ErrorLogger"/>.</description></item>
    /// </list>
    /// <para>Comportement vis-à-vis du CancellationToken :</para>
    /// <para>
    /// Le <see cref="CancellationToken"/> reçu par <see cref="ExecuteAsync"/> est ignoré
    /// activement dans toutes les opérations internes. Une erreur qui atteint ce UseCase
    /// mérite d'être journalisée et signalée, quel que soit l'état d'annulation en cours.
    /// Tous les appels internes utilisent <see cref="CancellationToken.None"/> pour garantir
    /// que la journalisation et la notification ne soient jamais interrompues par un signal
    /// d'annulation concomitant à la défaillance.
    /// </para>
    /// </remarks>
    public interface IU_LogAndNotify
    {
        /// <summary>
        /// Orchestre le traitement terminal d'une erreur applicative : construction du détail
        /// technique, traduction du message opérateur, journalisation et notification.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Cette méthode est appelée depuis les blocs <c>catch</c> des UseCases,
        /// après réception d'une exception typée du projet, ou depuis le filet de sécurité
        /// <c>VM_Page_Generic.ExecuteSafeAsync</c>.
        /// </para>
        /// <para>
        /// Séquence d'exécution (4 étapes isolées, principe best-effort) :
        /// </para>
        /// <list type="number">
        ///   <item><description>Construction de <c>errorDetails</c> au format <c>ErrorId — message</c>.</description></item>
        ///   <item><description>Traduction de <paramref name="dictionaryKey"/> via <see cref="IS_Dictionary"/>.</description></item>
        ///   <item><description>Journalisation via <see cref="IS_ErrorLogger.ExecuteAsync"/>.</description></item>
        ///   <item><description>Notification opérateur via <see cref="IS_Notification.Error"/> si <paramref name="notify"/> vaut <see langword="true"/>.</description></item>
        /// </list>
        /// <para>
        /// Robustesse : Chaque étape est isolée dans son propre bloc <c>try/catch</c>.
        /// La défaillance d'une étape n'empêche pas l'exécution des suivantes. Les erreurs
        /// internes sont tracées via <see cref="Debug.WriteLine(string)"/> en environnement
        /// de développement, sans impact en production.
        /// </para>
        /// <para>
        /// CancellationToken : Le <paramref name="ct"/> reçu est conservé dans la signature
        /// pour conformité avec la convention normative (section 4.6.5) mais il est
        /// ignoré activement dans toutes les opérations internes. Les appels internes
        /// utilisent <see cref="CancellationToken.None"/> afin de garantir que la journalisation
        /// et la notification d'une erreur ne soient jamais interrompues par un signal
        /// d'annulation concomitant.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain amont transmise par le composant appelant.</param>
        /// <param name="dictionaryKey">
        /// Clé dictionnaire du message opérateur. Valeurs standards : <c>"No_EC_01"</c> (Ex_Business),
        /// <c>"No_EC_02"</c> (Ex_Infrastructure), <c>"No_EC_03"</c> (Ex_Unclassified).
        /// </param>
        /// <param name="ex">
        /// Exception typée à traiter (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>
        /// ou <see cref="Ex_Unclassified"/>). Si <see langword="null"/>, la méthode retourne
        /// immédiatement sans traitement.
        /// </param>
        /// <param name="notify">
        /// Indique si une notification opérateur doit être produite. Défaut : <see langword="true"/>.
        /// </param>
        /// <param name="ct">
        /// Token d'annulation. Conservé par convention mais volontairement ignoré dans
        /// les opérations internes (voir remarques).
        /// </param>
        /// <returns>Une tâche représentant l'exécution asynchrone du traitement terminal.</returns>
        Task ExecuteAsync(
            string caller,
            string dictionaryKey,
            Exception ex,
            bool notify = true,
            CancellationToken ct = default);
    }
}