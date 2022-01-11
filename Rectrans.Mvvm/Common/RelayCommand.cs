using System.Windows.Input;

namespace Rectrans.Mvvm.Common;

public class RelayCommand : ICommand
{
    private readonly Action<object?>? _execute;
    private readonly Func<object, bool>? _canExecute;

    public RelayCommand()
    {
    }

    public RelayCommand(Action<object?>? execute)
    {
        _execute = execute;
        _canExecute = null;
    }

    public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute)
        :this(execute)
    {
        _canExecute = canExecute;
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
        _execute?.Invoke(parameter);
    }
}

public class RelayCommandAsync : ICommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Func<object, bool>? _canExecute;

    public RelayCommandAsync(Func<object?, Task> execute)
    {
        _execute = execute;
        _canExecute = null;
    }

    public RelayCommandAsync(Func<object?, Task> execute, Func<object?, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
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

    public async void Execute(object? parameter)
    {
        await _execute(parameter);
    }
}