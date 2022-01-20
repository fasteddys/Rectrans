using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Rectrans.Utilities;

// ReSharper disable InconsistentNaming

namespace Rectrans.ViewModels.Base;

public abstract class ModernWindow : BindableBase
{
    #region Private Member

    /// <summary>
    /// The window this view model controls
    /// </summary>
    private readonly Window window;

    /// <summary>
    /// The margin around the window to allow for a drop shadow.
    /// </summary>
    private int outerMarginSize = 10;

    /// <summary>
    /// The radius of the edges of the window.
    /// </summary>
    private int windowRadius = 10;

    /// <summary>
    /// The last known dock position.
    /// </summary>
    private WindowDockPosition DockPosition = WindowDockPosition.Undocked;

    #endregion

    #region Public Properties

    /// <summary>
    /// The smallest width the window can go to.
    /// </summary>
    public double WindowMinimumWidth { get; set; } = 400;

    /// <summary>
    /// The smallest height the window can go to.
    /// </summary>
    public double WindowMinimumHeight { get; set; } = 400;

    /// <summary>
    /// True if the window should be borderless because it is docked or maximized
    /// </summary>
    public bool Borderless =>
        window.WindowState == WindowState.Maximized || DockPosition != WindowDockPosition.Undocked;

    /// <summary>
    /// The size of the resize border around the window.
    /// </summary>
    public int ResizeBorder { get; set; } = 6;

    /// <summary>
    /// The size of the resize border around the window, taking into account the outer margin.
    /// </summary>
    public Thickness ResizeBorderThickness => new(ResizeBorder + OuterMarginSize);

    /// <summary>
    /// The padding of the inner content of the main window.
    /// </summary>
    public Thickness InnerContentPadding => new(ResizeBorder);

    /// <summary>
    /// The margin around the window to allow for a drop shadow.
    /// </summary>
    public int OuterMarginSize
    {
        // If it is maximized or docked, no border
        get => Borderless ? 0 : outerMarginSize;
        set => outerMarginSize = value;
    }

    /// <summary>
    /// The margin around the window to allow for a drop shadow.
    /// </summary>
    public Thickness OuterMarginSizeThickness => new(OuterMarginSize);

    /// <summary>
    /// The radius of the edges of the window.
    /// </summary>
    public int WindowRadius
    {
        // If it is maximized or docked, no border
        get => Borderless ? 0 : windowRadius;
        set => windowRadius = value;
    }

    /// <summary>
    /// The radius of the edges of the window.
    /// </summary>
    public CornerRadius WindowCornerRadius => new(WindowRadius);

    /// <summary>
    /// The height of the title bar / caption of the window.
    /// </summary>
    public int TitleHeight { get; set; } = 42;

    /// <summary>
    /// The height of the title bar / caption of the window.
    /// </summary>
    public GridLength TitleHeightGridLength => new(TitleHeight + ResizeBorder);

    #endregion

    #region Commands

    /// <summary>
    /// The command of minimize the window.
    /// </summary>
    public ICommand MinimizeCommand { get; set; }

    /// <summary>
    /// The command of maximize the window.
    /// </summary>
    public ICommand MaximizeCommand { get; set; }

    /// <summary>
    /// The command of close the window.
    /// </summary>
    public ICommand CloseCommand { get; set; }

    /// <summary>
    /// The command of show the system menu for the window.
    /// </summary>
    public ICommand IconCommand { get; set; }

    /// <summary>
    /// The command of set top most for the window.
    /// </summary>
    public ICommand TopMostCommand { get; set; }

    #endregion

    #region Construcotr

    protected ModernWindow(Window window)
    {
        this.window = window;

        // Listen out for the window resizing.
        this.window.StateChanged += (_, _) =>
        {
            // Fire off events for all properties that are affected by a resize
            WindowResized();
        };

        // Create commands
        MinimizeCommand = new DelegateCommand(() => this.window.WindowState = WindowState.Minimized);
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        MaximizeCommand = new DelegateCommand(() => this.window.WindowState ^= WindowState.Maximized);
        CloseCommand = new DelegateCommand(() => this.window.Close());
        IconCommand = new DelegateCommand(() => SystemCommands.ShowSystemMenu(this.window, GetMousePosition()));
        TopMostCommand = new DelegateCommand(() => this.window.Topmost = !this.window.Topmost);

        // Fix window resize issue.
        var resizer = new WindowResizer(this.window);

        // Listen out for dock changes
        resizer.WindowDockChanged += dock =>
        {
            // Store last position
            DockPosition = dock;

            // Fire off resize events
            WindowResized();
        };
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Gets the current mouse position on the screen
    /// </summary>
    /// <returns></returns>
    private Point GetMousePosition()
    {
        // Position of the mouse relative to the window
        var position = Mouse.GetPosition(window);

        // Add the window position so its a "ToScreen"
        return new Point(position.X + window.Left, position.Y + window.Top);
    }


    /// <summary>
    /// If the window resizes to a special position (docked or maximized)
    /// this will update all required property change events to set the borders and radius values
    /// </summary>
    private void WindowResized()
    {
        // Fire off events for all properties that are affected by a resize
        RaisePropertyChanged(nameof(Borderless));
        RaisePropertyChanged(nameof(ResizeBorderThickness));
        RaisePropertyChanged(nameof(OuterMarginSize));
        RaisePropertyChanged(nameof(OuterMarginSizeThickness));
        RaisePropertyChanged(nameof(WindowRadius));
        RaisePropertyChanged(nameof(WindowCornerRadius));
    }

    #endregion
}