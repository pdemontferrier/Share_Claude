using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using Microsoft.Extensions.DependencyInjection;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Composant créateur de scope d'injection de dépendances par invocation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside en <c>D_Presentation/Services/</c> et implémente
    /// <see cref="IS_UseCaseInvoker"/>. Elle porte la mécanique de séparation des contextes
    /// par invocation décrite en §3.8 du 0230, dont les règles d'implémentation figurent en
    /// §4.10.10. Elle matérialise, à chaque invocation initiée depuis D_Presentation, un
    /// <c>IServiceScope</c> distinct au sein duquel le DbContext partagé, le UseCase et les
    /// composants de la chaîne sont résolus comme instances propres à l'invocation, puis disposés
    /// à la sortie.
    /// </para>
    /// <para>
    /// Exception architecturale EA-11 (§4.10.10, §17.4 du 0231) : la classe reçoit
    /// <see cref="IServiceProvider"/> par injection paramétrée au constructeur — dérogation au
    /// modèle général d'injection paramétrée typée, nécessaire parce que seul un composant ayant
    /// accès au conteneur peut appeler <c>CreateAsyncScope()</c> pour matérialiser un nouveau
    /// scope. EA-11 se distingue de la convention de plateforme <c>App.ServiceProvider</c>
    /// (R-4.15.19 à R-4.15.24 du 0231) : ici, injection paramétrée standard, et seule légitimité
    /// d'appel à <c>CreateScope()</c> / <c>CreateAsyncScope()</c> dans la solution.
    /// </para>
    /// <para>
    /// Portée DI : <c>Singleton</c>. Le composant est stateless ; sa seule dépendance,
    /// <see cref="IServiceProvider"/>, est le conteneur racine, unique pour toute la durée de
    /// l'application. Le scope créé à chaque invocation est local à la méthode et disposé
    /// immédiatement : aucune dépendance scoped n'est capturée par champ d'instance.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Créer un <c>IServiceScope</c> distinct à chaque invocation via <c>CreateAsyncScope()</c>.</description></item>
    ///   <item><description>Résoudre le composant cible depuis le scope par <c>GetRequiredService&lt;T&gt;()</c>.</description></item>
    ///   <item><description>Exécuter le délégué fourni en propageant le <see cref="CancellationToken"/>.</description></item>
    ///   <item><description>Disposer le scope à la sortie (sortie normale ou propagation d'exception) via <c>await using</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>N'orchestre aucun scénario, ne journalise pas, ne notifie pas.</description></item>
    ///   <item><description>Ne capture aucune exception applicative : elles remontent au consommateur (filet <c>ExecuteSafeAsync</c>, §4.7.5).</description></item>
    /// </list>
    /// </remarks>
    public sealed class SR_UseCaseInvoker : IS_UseCaseInvoker
    {
        #region === Propriétés privées ===

        // Aucune propriété privée spécifique.

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Conteneur d'injection de dépendances racine, source des scopes créés à chaque invocation.
        /// </summary>
        private readonly IServiceProvider _rootProvider;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="SR_UseCaseInvoker"/> avec le conteneur racine.
        /// </summary>
        /// <param name="rootProvider">
        /// Conteneur d'injection de dépendances racine, résolu par le conteneur lui-même au titre
        /// d'EA-11. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="rootProvider"/> est <see langword="null"/>.</exception>
        public SR_UseCaseInvoker(IServiceProvider rootProvider)
        {
            _rootProvider = rootProvider ?? throw new ArgumentNullException(nameof(rootProvider));
        }

        #endregion

        #region === Méthodes publiques ===

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
        public async Task<TResult> InvokeAsync<TUseCase, TResult>(
            Func<TUseCase, CancellationToken, Task<TResult>> action,
            CancellationToken ct = default)
            where TUseCase : class
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            await using var scope = _rootProvider.CreateAsyncScope();
            var target = scope.ServiceProvider.GetRequiredService<TUseCase>();
            return await action(target, ct);
        }

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
        public async Task InvokeAsync<TUseCase>(
            Func<TUseCase, CancellationToken, Task> action,
            CancellationToken ct = default)
            where TUseCase : class
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            await using var scope = _rootProvider.CreateAsyncScope();
            var target = scope.ServiceProvider.GetRequiredService<TUseCase>();
            await action(target, ct);
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}