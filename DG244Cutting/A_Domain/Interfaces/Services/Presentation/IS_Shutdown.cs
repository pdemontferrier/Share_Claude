using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    /// <summary>
    /// Contrat du service technique transverse de présentation portant le geste WPF
    /// terminal de fermeture de la fenêtre principale de l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : contrat résidant en
    /// <c>A_Domain/Interfaces/Services/Presentation</c> conformément à la première
    /// obligation contractuelle de §4.14.3 amendée. L'implémentation correspondante
    /// <c>SR_Shutdown</c> réside en <c>D_Presentation/Services</c>, conformément à la
    /// deuxième obligation contractuelle de §4.14.3 amendée (couche cohérente avec le
    /// sous-dossier [Domaine] de l'interface).</para>
    /// <para>Objectif : isoler en D_Presentation le pilotage WPF effectif de la
    /// fermeture de <c>Application.Current.MainWindow</c> conformément à R-2.5.6 et à la
    /// frontière d'isolation des dépendances WPF (I-4.14.16). Le couple est mobilisé par
    /// le UseCase orchestrateur de fermeture (à venir : <c>UC_CloseApplication</c>) en
    /// délégation terminale, à l'issue de la séquence de fermeture pilotée par
    /// l'orchestrateur.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer une opération unique de fermeture effective de la
    ///   fenêtre principale WPF, pilotée par <c>Application.Current.Dispatcher.BeginInvoke</c>
    ///   (appel non bloquant, symétrique au pilotage Dispatcher pratiqué par
    ///   <c>SR_Notification</c>).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne met à jour aucun Setting utilisateur ; en particulier, la
    ///   responsabilité de positionnement du flag <c>ISE_User.ForceClose</c> appartient
    ///   exclusivement aux consommateurs amont (UseCase orchestrateur).</description></item>
    ///   <item><description>N'émet aucune notification utilisateur préalable à la
    ///   fermeture (Warning, DialogWindow) : les notifications éventuelles sont pilotées
    ///   par les consommateurs amont via <c>IS_Notification</c>.</description></item>
    ///   <item><description>N'introduit aucun séquencement temporel
    ///   (<c>Task.Delay</c> notamment) : le séquencement appartient également aux
    ///   consommateurs amont.</description></item>
    ///   <item><description>N'accède à aucune ressource externe, n'ouvre ni ne valide
    ///   aucune transaction (R-4.10.1, I-4.10.1).</description></item>
    /// </list>
    /// </remarks>
    public interface IS_Shutdown
    {
        /// <summary>
        /// Déclenche la fermeture effective de la fenêtre principale WPF de l'application
        /// par pilotage non bloquant du <c>Dispatcher</c>.
        /// </summary>
        /// <remarks>
        /// <para>Pilotage : la fermeture est planifiée via
        /// <c>Application.Current?.Dispatcher.BeginInvoke</c> et ferme
        /// <c>Application.Current.MainWindow</c>. Le geste est terminal et n'engage aucune
        /// interaction utilisateur, aucune mutation persistante et aucun séquencement
        /// temporel.</para>
        /// <para>Annulation coopérative : le jeton <paramref name="ct"/> est
        /// consulté avant la planification de la fermeture, conformément à R-4.6.13.</para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant pour la propagation de
        /// la CallChain au format normatif §4.5. Ne doit pas être
        /// <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.
        /// Par défaut <see langword="default"/>.</param>
        /// <returns>Une <see cref="Task"/> sans valeur signalant la planification de la
        /// fermeture (l'effet effectif sur le sous-système WPF est asynchrone du fait du
        /// pilotage Dispatcher).</returns>
        /// <exception cref="OperationCanceledException">
        /// Levée lorsque l'annulation coopérative est demandée avant la planification de
        /// la fermeture, conformément à R-4.6.13.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée par requalification terminale via <c>IS_ExClassifier</c> si une exception
        /// non prévue est issue du sous-système WPF lors de la planification de la
        /// fermeture (§4.7, R-4.7.25).
        /// </exception>
        Task ExecuteAsync(string caller, CancellationToken ct = default);
    }
}