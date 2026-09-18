using System.Diagnostics;
using System.Windows.Input;

namespace DG244Cutting.D_Presentation.Utilities.RelayCommands
{
    /// <summary>
    /// Implémentation d'<see cref="ICommand"/> sans paramètre, pilotée par un délégué asynchrone.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Utilitaire WPF sans état destiné à être instancié dans le
    /// constructeur d'un ViewModel pour exposer une commande asynchrone (sans argument transmis
    /// par la vue) liée à une méthode <c>private async Task</c> du ViewModel — typiquement une
    /// méthode encapsulée dans le filet de sécurité <c>VM_Page_Generic.ExecuteSafeAsync</c>.</para>
    /// <para>Objectif : Fournir une implémentation minimale et réutilisable du contrat
    /// <see cref="ICommand"/> capable de relayer un délégué <see cref="Func{Task}"/>, tout en
    /// posant un filet de sécurité ultime autour de l'invocation <c>async void</c> exigée par
    /// le contrat WPF <see cref="ICommand.Execute"/>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Exposer le délégué asynchrone <c>_execute</c> via la méthode publique
    ///   <see cref="ExecuteAsync"/> (qui retourne un <see cref="Task"/> observable par les
    ///   appelants directs).</item>
    ///   <item>Exposer la méthode <see cref="Execute"/> requise par <see cref="ICommand"/>,
    ///   sous forme <c>async void</c>, avec un filet de sécurité ultime <c>try/catch</c>.</item>
    ///   <item>Relayer l'événement <see cref="CanExecuteChanged"/> sur
    ///   <see cref="CommandManager.RequerySuggested"/>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne porte aucune logique métier, ni accès aux services applicatifs, ni accès aux Settings.</item>
    ///   <item>Ne participe pas à la <c>CallChain</c> applicative — utilitaire WPF sans état.</item>
    ///   <item>Ne journalise pas via <c>IS_ErrorLogger</c> et ne notifie pas via <c>IS_Notification</c> :
    ///   le filet de sécurité ultime trace uniquement via <see cref="Debug.WriteLine"/>, en
    ///   environnement de développement, conformément au comportement du filet de sécurité de
    ///   <c>Page_Generic</c> (§4.14.6).</item>
    /// </list>
    /// <para>Filet de sécurité ultime. La méthode <see cref="Execute"/> encapsule l'appel
    /// à <see cref="ExecuteAsync"/> dans un bloc <c>try/catch (Exception ex)</c> qui trace
    /// l'exception capturée selon le format normatif de §4.14.6 :
    /// <c>[NomClasse.NomMéthode] Exception non gérée capturée par le filet de sécurité ultime : Type — Message</c>.
    /// Ce rempart est de second niveau et n'intervient que si l'exception a échappé au filet
    /// principal porté par <c>VM_Page_Generic.ExecuteSafeAsync</c> côté ViewModel.</para>
    /// <para>Exception architecturale propre — <c>async void</c>. La méthode
    /// <see cref="Execute"/> adopte la signature <c>async void</c>, habituellement à proscrire,
    /// pour les trois raisons cumulatives suivantes (parallèles à l'Exception 2 de
    /// <c>Page_Generic</c> documentée en §4.14.6) :</para>
    /// <list type="bullet">
    ///   <item>Elle est imposée par le contrat <see cref="ICommand.Execute"/>, qui retourne <c>void</c>.</item>
    ///   <item>Elle est isolée par un <c>try/catch (Exception ex)</c> ultime qui capture toute
    ///   exception y compris celle qui se serait échappée d'un <c>await</c> interne.</item>
    ///   <item>Aucun appelant n'a besoin d'observer la complétion — WPF ne consomme pas le
    ///   <see cref="Task"/> qui aurait pu être retourné. Les tests et appelants directs disposent
    ///   pour leur part de la méthode <see cref="ExecuteAsync"/> qui, elle, retourne un <see cref="Task"/>.</item>
    /// </list>
    /// </remarks>
    public class UT_RelayCommandArg0Async : ICommand
    {
        #region === Propriétés privées ===

        /// <summary>Délégué asynchrone exécuté par <see cref="ExecuteAsync"/>.</summary>
        private readonly Func<Task> _execute;

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
        /// Initialise une nouvelle instance de <see cref="UT_RelayCommandArg0Async"/>.
        /// </summary>
        /// <param name="execute">
        /// Délégué asynchrone d'exécution de la commande. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="canExecute">
        /// Prédicat optionnel d'évaluation de l'état activable de la commande.
        /// Lorsqu'il vaut <see langword="null"/>, la commande est toujours activable.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="execute"/> est <see langword="null"/>.
        /// </exception>
        public UT_RelayCommandArg0Async(Func<Task> execute, Func<bool>? canExecute = null)
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
        /// Exécute le délégué asynchrone associé à la commande, sous filet de sécurité ultime.
        /// </summary>
        /// <remarks>
        /// <para>Comportement : Cette méthode est l'entrée appelée par WPF via
        /// <see cref="ICommand.Execute"/>. Elle délègue à <see cref="ExecuteAsync"/> et capture
        /// toute exception non gérée parvenue jusqu'à ce niveau, en la traçant via
        /// <see cref="Debug.WriteLine"/>. Aucune exception n'est ainsi propagée vers le thread
        /// UI WPF, conformément au principe du filet de sécurité ultime défini en §4.14.6.</para>
        /// <para>Justification de la signature <c>async void</c> : voir le paragraphe
        /// « Exception architecturale propre — <c>async void</c> » de la documentation de classe.</para>
        /// </remarks>
        /// <param name="parameter">
        /// Paramètre transmis par WPF — non utilisé dans cette variante sans argument.
        /// </param>
        public async void Execute(object? parameter)
        {
            try
            {
                await ExecuteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[{nameof(UT_RelayCommandArg0Async)}.{nameof(Execute)}] " +
                    $"Exception non gérée capturée par le filet de sécurité ultime : " +
                    $"{ex.GetType().Name} — {ex.Message}");
            }
        }

        /// <summary>
        /// Exécute le délégué asynchrone associé à la commande et retourne le <see cref="Task"/>
        /// correspondant pour permettre à un appelant direct (test unitaire, code de support)
        /// d'observer la complétion ou de capturer les exceptions.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : WPF n'appelle jamais cette méthode — il appelle uniquement
        /// <see cref="Execute"/>. <see cref="ExecuteAsync"/> existe pour exposer un point d'entrée
        /// asynchrone observable hors du contexte WPF (tests, code de support, harnais).</para>
        /// <para>Pas de filet de sécurité : contrairement à <see cref="Execute"/>, cette
        /// méthode ne capture aucune exception. L'appelant direct est libre — et tenu — d'observer
        /// le <see cref="Task"/> retourné et de gérer ses exceptions selon ses propres conventions.</para>
        /// </remarks>
        /// <returns>Un <see cref="Task"/> représentant l'opération asynchrone en cours.</returns>
        public async Task ExecuteAsync() => await _execute();

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