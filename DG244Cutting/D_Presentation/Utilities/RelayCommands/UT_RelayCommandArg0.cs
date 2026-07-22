using System.Windows.Input;

namespace DG244Cutting.D_Presentation.Utilities.RelayCommands
{
    /// <summary>
    /// Implémentation d'<see cref="ICommand"/> sans paramètre, pilotée par un délégué synchrone.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Utilitaire WPF sans état destiné à être instancié dans le
    /// constructeur d'un ViewModel pour exposer une commande synchrone (sans argument transmis
    /// par la vue) liée à une méthode privée du ViewModel.</para>
    /// <para>Objectif : Fournir une implémentation minimale, immuable et réutilisable du
    /// contrat <see cref="ICommand"/> compatible avec le DataBinding WPF, en s'appuyant sur
    /// <see cref="CommandManager.RequerySuggested"/> pour la propagation passive du changement
    /// d'état d'activation.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer le délégué d'exécution <c>_execute</c> via la méthode <see cref="Execute"/>.</item>
    ///   <item>Exposer le prédicat optionnel d'activation <c>_canExecute</c> via la méthode
    ///   <see cref="CanExecute"/>.</item>
    ///   <item>Relayer l'événement <see cref="CanExecuteChanged"/> sur
    ///   <see cref="CommandManager.RequerySuggested"/>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne porte aucune logique métier, ni accès aux services applicatifs, ni accès aux Settings.</item>
    ///   <item>Ne participe pas à la <c>CallChain</c> applicative — utilitaire WPF sans état.</item>
    ///   <item>Ne capture aucune exception levée par le délégué <c>_execute</c> — la responsabilité
    ///   du filet de sécurité incombe au ViewModel via <c>VM_Page_Generic.ExecuteSafeAsync</c>
    ///   (cf. §4.6.5 du référentiel 023).</item>
    /// </list>
    /// </remarks>
    public class UT_RelayCommandArg0 : ICommand
    {
        #region === Propriétés privées ===

        /// <summary>Délégué synchrone exécuté par <see cref="Execute"/>.</summary>
        private readonly Action _execute;

        /// <summary>
        /// Prédicat optionnel évalué par <see cref="CanExecute"/>.
        /// <see langword="null"/> si la commande est toujours activable.
        /// </summary>
        private readonly Func<bool>? _canExecute;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UT_RelayCommandArg0"/>.
        /// </summary>
        /// <param name="execute">
        /// Délégué d'exécution de la commande. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="canExecute">
        /// Prédicat optionnel d'évaluation de l'état activable de la commande.
        /// Lorsqu'il vaut <see langword="null"/>, la commande est toujours activable.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="execute"/> est <see langword="null"/>.
        /// </exception>
        public UT_RelayCommandArg0(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Évalue si la commande est activable dans l'état courant.
        /// </summary>
        /// <param name="parameter">
        /// Paramètre transmis par WPF — non utilisé dans cette variante sans argument.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si <c>_canExecute</c> est <see langword="null"/> ou si son
        /// évaluation retourne <see langword="true"/> ; <see langword="false"/> sinon.
        /// </returns>
        public bool CanExecute(object? parameter) => _canExecute is null || _canExecute();

        /// <summary>
        /// Exécute le délégué synchrone associé à la commande.
        /// </summary>
        /// <param name="parameter">
        /// Paramètre transmis par WPF — non utilisé dans cette variante sans argument.
        /// </param>
        public void Execute(object? parameter) => _execute();

        /// <summary>
        /// Événement déclenché lorsque l'état d'activation de la commande a potentiellement changé.
        /// </summary>
        /// <remarks>
        /// <para>Implémentation : Relais transparent vers
        /// <see cref="CommandManager.RequerySuggested"/>, déclenché par WPF sur les événements
        /// UI standard (changements de focus, clics, etc.). Pour forcer une réévaluation
        /// explicite à un moment précis, l'appelant peut invoquer
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
