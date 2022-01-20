

// ReSharper disable ClassNeverInstantiated.Global

// ReSharper disable InconsistentNaming

using Rectrans.ViewModels.Base;

namespace Rectrans.ViewModels.Windows;

public class MainViewModel : ModernWindow
{
    #region Private Members

    /// <summary>
    /// The window this view model controls
    /// </summary>
    private readonly MainWindow mWindow;

    /// <summary>
    /// The tab control height
    /// </summary>
    private int tabControlHeight = 450;

    /// <summary>
    /// The tab control width
    /// </summary>
    private int tabControlWidth = 600;

    #endregion

    #region Public Properties

    /// <summary>
    /// The tab control height
    /// </summary>
    public int TabControlHeight
    {
        get => tabControlHeight;
        set
        {
            tabControlHeight = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// The tab control width
    /// </summary>
    public int TabControlWidth
    {
        get => tabControlWidth;
        set
        {
            tabControlWidth = value;
            RaisePropertyChanged();
        }
    }

    #endregion

    #region Construcotr

    public MainViewModel(MainWindow window)
        : base(window)
    {
        mWindow = window;
    }

    #endregion
}