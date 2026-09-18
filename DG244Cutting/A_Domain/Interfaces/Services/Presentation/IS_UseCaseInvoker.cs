
namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    /// <summary>
    /// Contrat du composant créateur de scope d'injection de dépendances par invocation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et constitue le contrat du
    /// composant porteur de la mécanique de séparation des contextes par invocation décrite
    /// en §3.8 du 0230 et dont les règles d'implémentation sont posées en §4.10.10. Son
    /// implémentation concrète <c>SR_UseCaseInvoker</c> réside en <c>D_Presentation/Services/</c>.
    /// </para>
    /// <para>
    /// Rôle : matérialiser un <see cref="System.IServiceProvider"/> de scope distinct
    /// (<c>IServiceScope</c>) à chaque invocation d'un UseCase (chaînes (1), (3), (4) de §4.14.9)
    /// ou d'un Query Handler (chaîne (2) de lecture simple) initiée depuis D_Presentation,
    /// y résoudre le composant cible, exécuter l'opération, puis disposer le scope à la sortie.
    /// Cette séparation par invocation est la condition technique sans laquelle la portée
    /// Scoped enregistrée par le Composition Root dégénère en portée Singleton de fait, et sans
    /// laquelle la doctrine transactionnelle de §3.8 / §4.10 reste inopérante.
    /// </para>
    /// <para>
    /// Substitution structurante : les ViewModels (et accessoirement les MenuHandlers et le
    /// code-behind XAML des Pages) injectent cette interface au constructeur et n'injectent
    /// plus directement les contrats <c>IU_*</c> ou <c>IQ_*</c> qu'ils consommaient antérieurement.
    /// Toute injection directe d'un <c>IU_*</c> ou d'un <c>IQ_*</c> depuis D_Presentation, hors
    /// injection de la présente interface, constitue une non-conformité (§4.10.10, première
    /// interdiction).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Créer un <c>IServiceScope</c> distinct à chaque invocation.</description></item>
    ///   <item><description>Résoudre le composant cible depuis ce scope.</description></item>
    ///   <item><description>Exécuter le délégué fourni en propageant le <see cref="System.Threading.CancellationToken"/>.</description></item>
    ///   <item><description>Disposer le scope à la sortie, en sortie normale comme en propagation d'exception.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>N'orchestre aucun scénario : il ne fait que résoudre et exécuter.</description></item>
    ///   <item><description>Ne journalise pas, ne notifie pas : les exceptions remontent au consommateur (filet <c>ExecuteSafeAsync</c> de <c>VM_Page_Generic</c>, §4.7.5).</description></item>
    ///   <item><description>Ne capture aucune exception applicative typée : elles traversent l'invocation sans interception.</description></item>
    /// </list>
    /// </remarks>
    public interface IS_UseCaseInvoker
    {
        /// <summary>
        /// Invoque un composant à retour signalable au sein d'un scope DI distinct, créé pour
        /// la seule durée de l'invocation puis disposé.
        /// </summary>
        /// <remarks>
        /// Surcharge applicable lorsque le composant invoqué expose une méthode publique à
        /// retour <see cref="System.Threading.Tasks.Task{TResult}"/> (UseCase à retour signalable
        /// au sens de R-4.14.21, UseCase de cas Concept, ou Query Handler en chaîne (2)).
        /// </remarks>
        /// <typeparam name="TUseCase">
        /// Type du contrat à résoudre depuis le scope (typiquement une interface <c>IU_*</c> ou <c>IQ_*</c>).
        /// </typeparam>
        /// <typeparam name="TResult">Type du résultat retourné par l'opération.</typeparam>
        /// <param name="action">
        /// Délégué exprimant l'opération à exécuter sur le composant résolu, recevant le composant
        /// et le jeton d'annulation. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation propagé à l'opération.</param>
        /// <returns>Le résultat produit par l'opération.</returns>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="action"/> est <see langword="null"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/>.</exception>
        Task<TResult> InvokeAsync<TUseCase, TResult>(
            Func<TUseCase, CancellationToken, Task<TResult>> action,
            CancellationToken ct = default)
            where TUseCase : class;

        /// <summary>
        /// Invoque un composant à retour <see cref="System.Threading.Tasks.Task"/> simple au sein
        /// d'un scope DI distinct, créé pour la seule durée de l'invocation puis disposé.
        /// </summary>
        /// <remarks>
        /// Surcharge applicable lorsque le composant invoqué expose une méthode publique à retour
        /// <see cref="System.Threading.Tasks.Task"/> simple (UseCase exclusivement consommé en
        /// chaîne (1) directe par un ViewModel ou un Menu Handler).
        /// </remarks>
        /// <typeparam name="TUseCase">
        /// Type du contrat à résoudre depuis le scope (typiquement une interface <c>IU_*</c>).
        /// </typeparam>
        /// <param name="action">
        /// Délégué exprimant l'opération à exécuter sur le composant résolu, recevant le composant
        /// et le jeton d'annulation. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation propagé à l'opération.</param>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="action"/> est <see langword="null"/>.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/>.</exception>
        Task InvokeAsync<TUseCase>(
            Func<TUseCase, CancellationToken, Task> action,
            CancellationToken ct = default)
            where TUseCase : class;
    }
}