using System.Diagnostics;
using System.Windows.Input;

namespace DG244Cutting.D_Presentation.Utilities.RelayCommands
{
    /// <summary>
    /// Implémentation d'<see cref="ICommand"/> à un paramètre typé, pilotée par un délégué asynchrone.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Utilitaire WPF générique destiné à être instancié dans le
    /// constructeur d'un ViewModel pour exposer une commande asynchrone recevant un argument
    /// fortement typé depuis la vue (typiquement via <c>CommandParameter</c> dans le XAML),
    /// liée à une méthode <c>private async Task</c> du ViewModel — typiquement une méthode
    /// encapsulée dans le filet de sécurité <c>VM_Page_Generic.ExecuteSafeAsync</c>.</para>
    /// <para>Objectif : Fournir une implémentation minimale et réutilisable du contrat
    /// <see cref="ICommand"/> capable de relayer un délégué <see cref="Func{T, Task}"/> avec
    /// contrôle de type sur l'argument <typeparamref name="T"/>, tout en posant un filet de
    /// sécurité ultime autour de l'invocation <c>async void</c> exigée par le contrat WPF
    /// <see cref="ICommand.Execute"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer le délégué asynchrone <c>_execute</c> via la méthode publique
    ///   <see cref="ExecuteAsync"/> (qui retourne un <see cref="Task"/> observable par les
    ///   appelants directs et reçoit un argument fortement typé <typeparamref name="T"/>).</item>
    ///   <item>Exposer la méthode <see cref="Execute"/> requise par <see cref="ICommand"/>,
    ///   sous forme <c>async void</c>, après vérification que le paramètre est du type
    ///   <typeparamref name="T"/>, avec un filet de sécurité ultime <c>try/catch</c>.</item>
    ///   <item>Exposer le prédicat optionnel d'activation <c>_canExecute</c> via la méthode
    ///   <see cref="CanExecute"/>, avec la même vérification de type.</item>
    ///   <item>Relayer l'événement <see cref="CanExecuteChanged"/> sur
    ///   <see cref="CommandManager.RequerySuggested"/>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne porte aucune logique métier, ni accès aux services applicatifs, ni accès aux Settings.</item>
    ///   <item>Ne participe pas à la <c>CallChain</c> applicative — utilitaire WPF sans état.</item>
    ///   <item>Ne lève pas d'exception en cas de paramètre incompatible : l'invocation est
    ///   silencieusement ignorée, conformément au contrat <see cref="ICommand"/>.</item>
    ///   <item>Ne journalise pas via <c>IS_ErrorLogger</c> et ne notifie pas via <c>IS_Notification</c> :
    ///   le filet de sécurité ultime trace uniquement via <see cref="Debug.WriteLine"/>, en
    ///   environnement de développement, conformément au comportement du filet de sécurité de
    ///   <c>Page_Generic</c> (§4.14.6).</item>
    /// </list>
    /// <para>Filet de sécurité ultime. La méthode <see cref="Execute"/> encapsule l'appel
    /// à <see cref="ExecuteAsync"/> dans un bloc <c>try/catch (Exception ex)</c> qui trace
    /// l'exception capturée selon le format normatif de §4.14.6 :
    /// <c>[NomClasse.NomMéthode] Exception non gérée capturée par le filet de sécurité ultime : Type — Message</c>.
    /// Le filet n'est posé que dans la branche où le paramètre est effectivement compatible
    /// avec <typeparamref name="T"/> et où le délégué async est donc invoqué — un paramètre
    /// incompatible est silencieusement ignoré sans <c>try/catch</c>, en cohérence avec le
    /// contrat <see cref="ICommand"/> et avec <c>UT_RelayCommandArg1&lt;T&gt;</c> synchrone.</para>
    /// <para>Exception architecturale propre — <c>async void</c>. La méthode
    /// <see cref="Execute"/> adopte la signature <c>async void</c>, habituellement à proscrire,
    /// pour les trois raisons cumulatives suivantes (parallèles à l'Exception 2 de
    /// <c>Page_Generic</c> documentée en §4.14.6, et identiques à celles de
    /// <c>UT_RelayCommandArg0Async</c>) :</para>
    /// <list type="bullet">
    ///   <item>Elle est imposée par le contrat <see cref="ICommand.Execute"/>, qui retourne <c>void</c>.</item>
    ///   <item>Elle est isolée par un <c>try/catch (Exception ex)</c> ultime qui capture toute
    ///   exception y compris celle qui se serait échappée d'un <c>await</c> interne.</item>
    ///   <item>Aucun appelant n'a besoin d'observer la complétion — WPF ne consomme pas le
    ///   <see cref="Task"/> qui aurait pu être retourné. Les tests et appelants directs disposent
    ///   pour leur part de la méthode <see cref="ExecuteAsync"/> qui, elle, retourne un
    ///   <see cref="Task"/> et reçoit l'argument fortement typé.</item>
    /// </list>
    /// <para>Cette exception architecturale est consolidée dans l'inventaire transverse §4.14.9
    /// du référentiel sous la sous-famille B.2 — Méthodes <c>Execute</c> d'<see cref="ICommand"/>
    /// asynchrone.</para>
    /// </remarks>
    /// <typeparam name="T">Type de l'argument transmis par WPF lors de l'invocation de la commande.</typeparam>
    public class UT_RelayCommandArg1Async<T> : ICommand
    {
        #region === Propriétés privées ===

        /// <summary>Délégué asynchrone exécuté par <see cref="ExecuteAsync"/>.</summary>
        private readonly Func<T, Task> _execute;

        /// <summary>
        /// Prédicat optionnel évalué par <see cref="CanExecute"/>.
        /// <see langword="null"/> si la commande est toujours activable lorsqu'un argument
        /// valide est fourni.
        /// </summary>
        private readonly Func<T, bool>? _canExecute;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UT_RelayCommandArg1Async{T}"/>.
        /// </summary>
        /// <param name="execute">
        /// Délégué asynchrone d'exécution de la commande, recevant un argument de type
        /// <typeparamref name="T"/>. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="canExecute">
        /// Prédicat optionnel d'évaluation de l'état activable de la commande pour un argument donné.
        /// Lorsqu'il vaut <see langword="null"/>, la commande est toujours activable lorsqu'un
        /// argument du bon type est fourni.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="execute"/> est <see langword="null"/>.
        /// </exception>
        public UT_RelayCommandArg1Async(Func<T, Task> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Évalue si la commande est activable dans l'état courant pour l'argument fourni.
        /// </summary>
        /// <param name="parameter">
        /// Paramètre transmis par WPF, attendu de type <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si <c>_canExecute</c> est <see langword="null"/> et que
        /// <paramref name="parameter"/> est compatible avec <typeparamref name="T"/>, ou si
        /// <c>_canExecute</c> évalué sur l'argument typé retourne <see langword="true"/> ;
        /// <see langword="false"/> sinon.
        /// </returns>
        public bool CanExecute(object? parameter) =>
            _canExecute is null || (parameter is T t && _canExecute(t));

        /// <summary>
        /// Exécute le délégué asynchrone associé à la commande, après vérification du type
        /// du paramètre, sous filet de sécurité ultime.
        /// </summary>
        /// <remarks>
        /// <para>Comportement : Cette méthode est l'entrée appelée par WPF via
        /// <see cref="ICommand.Execute"/>. Si <paramref name="parameter"/> n'est pas du type
        /// <typeparamref name="T"/>, l'invocation est silencieusement ignorée — aucune exception
        /// n'est levée, aucun filet de sécurité n'est posé. Si le paramètre est compatible,
        /// l'appel est délégué à <see cref="ExecuteAsync"/> sous <c>try/catch (Exception ex)</c>
        /// ultime, qui trace toute exception non gérée via <see cref="Debug.WriteLine"/>.</para>
        /// <para>Justification de la signature <c>async void</c> : voir le paragraphe
        /// « Exception architecturale propre — <c>async void</c> » de la documentation de classe.</para>
        /// </remarks>
        /// <param name="parameter">
        /// Paramètre transmis par WPF, attendu de type <typeparamref name="T"/>.
        /// </param>
        public async void Execute(object? parameter)
        {
            if (parameter is T t)
            {
                try
                {
                    await ExecuteAsync(t);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(
                        $"[{nameof(UT_RelayCommandArg1Async<T>)}.{nameof(Execute)}] " +
                        $"Exception non gérée capturée par le filet de sécurité ultime : " +
                        $"{ex.GetType().Name} — {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Exécute le délégué asynchrone associé à la commande pour l'argument typé fourni
        /// et retourne le <see cref="Task"/> correspondant pour permettre à un appelant direct
        /// (test unitaire, code de support) d'observer la complétion ou de capturer les exceptions.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : WPF n'appelle jamais cette méthode — il appelle uniquement
        /// <see cref="Execute"/>. <see cref="ExecuteAsync"/> existe pour exposer un point d'entrée
        /// asynchrone observable et fortement typé hors du contexte WPF (tests, code de support,
        /// harnais).</para>
        /// <para>Pas de filet de sécurité : contrairement à <see cref="Execute"/>, cette
        /// méthode ne capture aucune exception. L'appelant direct est libre — et tenu — d'observer
        /// le <see cref="Task"/> retourné et de gérer ses exceptions selon ses propres conventions.</para>
        /// </remarks>
        /// <param name="parameter">Argument fortement typé transmis au délégué asynchrone.</param>
        /// <returns>Un <see cref="Task"/> représentant l'opération asynchrone en cours.</returns>
        public async Task ExecuteAsync(T parameter) => await _execute(parameter);

        /// <summary>
        /// Événement déclenché lorsque l'état d'activation de la commande a potentiellement changé.
        /// </summary>
        /// <remarks>
        /// <para>Implémentation : Relais transparent vers
        /// <see cref="CommandManager.RequerySuggested"/>, déclenché par WPF sur les événements
        /// UI standard. Pour forcer une réévaluation explicite, l'appelant peut invoquer
        /// <see cref="CommandManager.InvalidateRequerySuggested"/>.</para>
        /// </remarks>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}