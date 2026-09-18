using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommonResources.Utilities
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?>? _execute;
        private readonly Func<object?, bool>? _canExecute;
        private readonly Func<Task>? _executeAsync;
        private readonly Func<bool>? _canExecuteAsync;

        public event EventHandler? CanExecuteChanged;

        // Constructeur pour les commandes synchrones
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Constructeur pour les commandes asynchrones
        public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecuteAsync = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecuteAsync = canExecuteAsync;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute(parameter);
            }
            if (_canExecuteAsync != null)
            {
                return _canExecuteAsync();
            }
            return true;
        }

        public async void Execute(object? parameter)
        {
            if (_execute != null)
            {
                _execute(parameter);
            }
            else if (_executeAsync != null)
            {
                await _executeAsync();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
