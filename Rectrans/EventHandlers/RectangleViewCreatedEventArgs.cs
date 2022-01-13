using Rectrans.Views;

namespace Rectrans.EventHandlers;

public class InputWindowCreatedEventArgs : EventArgs
{
    public InputWindowCreatedEventArgs(InputWindow inputWindow)
    {
        InputWindow = inputWindow;
    }

    public InputWindow InputWindow { get; }
}