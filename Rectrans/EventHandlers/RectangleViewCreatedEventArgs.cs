using Rectrans.Views;

namespace Rectrans.EventHandlers;

public class ImportWindowCreatedEventArgs : EventArgs
{
    public ImportWindowCreatedEventArgs(ImportWindow importWindow)
    {
        ImportWindow = importWindow;
    }

    public ImportWindow ImportWindow { get; }
}