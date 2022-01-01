using System;
using System.Windows.Input;

namespace Rectrans
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object, bool>? _canExecute;

        public RelayCommand(Action<object?> execute)
        {
            this._execute = execute;
            _canExecute = null;
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || CanExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
