using System.Windows;
using Prism.Events;

// ReSharper disable ClassNeverInstantiated.Global

// ReSharper disable InconsistentNaming

namespace Rectrans.ViewModels.Windows;

public class MainViewModel : ModernWindow
{
    #region Construcotr

    public MainViewModel(Window window, IEventAggregator aggregator)
        : base(window)
    {
    }

    #endregion
}