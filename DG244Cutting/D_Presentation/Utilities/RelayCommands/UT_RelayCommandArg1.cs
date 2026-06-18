using System.Windows.Input;

namespace DG244Cutting.D_Presentation.Utilities.RelayCommands
{
    /// <summary>
    /// Implémentation d'<see cref="ICommand"/> à un paramètre typé, pilotée par un délégué synchrone.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Utilitaire WPF générique destiné à être instancié dans le
    /// constructeur d'un ViewModel pour exposer une commande synchrone recevant un argument
    /// fortement typé depuis la vue (typiquement via <c>CommandParameter</c> dans le XAML).</para>
    /// <para>Objectif : Fournir une implémentation minimale, immuable et réutilisable du
    /// contrat <see cref="ICommand"/> avec contrôle de type sur l'argument <typeparamref name="T"/>,
    /// en filtrant silencieusement les invocations dont le paramètre n'est pas du type attendu.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer le délégué d'exécution <c>_execute</c> via la méthode <see cref="Execute"/>,
    ///   après vérification que le paramètre est du type <typeparamref name="T"/>.</item>
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
    ///   <item>Ne capture aucune exception levée par le délégué <c>_execute</c> — la responsabilité
    ///   du filet de sécurité incombe au ViewModel via <c>VM_Page_Generic.ExecuteSafeAsync</c>
    ///   (cf. §4.6.5 du référentiel 023).</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">Type de l'argument transmis par WPF lors de l'invocation de la commande.</typeparam>
    public class UT_RelayCommandArg1<T> : ICommand
    {
        #region === Propriétés privées ===

        /// <summary>Délégué synchrone exécuté par <see cref="Execute"/>.</summary>
        private readonly Action<T> _execute;

        /// <summary>
        /// Prédicat optionnel évalué par <see cref="CanExecute"/>.
        /// <see langword="null"/> si la commande est toujours activable lorsqu'un argument valide est fourni.
        /// </summary>
        private readonly Func<T, bool>? _canExecute;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UT_RelayCommandArg1{T}"/>.
        /// </summary>
        /// <param name="execute">
        /// Délégué d'exécution de la commande, recevant un argument de type <typeparamref name="T"/>.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="canExecute">
        /// Prédicat optionnel d'évaluation de l'état activable de la commande pour un argument donné.
        /// Lorsqu'il vaut <see langword="null"/>, la commande est toujours activable lorsqu'un
        /// argument du bon type est fourni.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="execute"/> est <see langword="null"/>.
        /// </exception>
        public UT_RelayCommandArg1(Action<T> execute, Func<T, bool>? canExecute = null)
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
        /// Exécute le délégué synchrone associé à la commande, après vérification du type
        /// du paramètre.
        /// </summary>
        /// <remarks>
        /// <para>Comportement : Si <paramref name="parameter"/> n'est pas du type
        /// <typeparamref name="T"/>, l'invocation est silencieusement ignorée — aucune exception
        /// n'est levée. Ce comportement est conforme au contrat <see cref="ICommand"/>, dont
        /// les implémentations ne sont pas censées lever pour un paramètre absent ou inadapté.</para>
        /// </remarks>
        /// <param name="parameter">
        /// Paramètre transmis par WPF, attendu de type <typeparamref name="T"/>.
        /// </param>
        public void Execute(object? parameter)
        {
            if (parameter is T t)
            {
                _execute(t);
            }
        }

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