using System.Windows;
using Prism.Mvvm;
using Rectrans.Utilities;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Rectrans.ViewModels.Base;

public abstract class MiniModernWindow : BindableBase
{
    #region Private Member

    /// <summary>
    /// The window this view model controls
    /// </summary>
    private readonly Window window;

    /// <summary>
    /// The margin around the window to allow for a drop shadow
    /// </summary>
    private int outerMarginSize = 10;

    /// <summary>
    /// The margin around the window content.
    /// </summary>
    private int contentBorder = 2;

    /// <summary>
    /// The radius of the edges of the window
    /// </summary>
    private int windowRadius = 6;

    /// <summary>
    /// The last known dock position
    /// </summary>
    private WindowDockPosition dockPosition = WindowDockPosition.Undocked;

    #endregion

    #region Public Properties

    /// <summary>
    /// The smallest width the window can go to
    /// </summary>
    public virtual double WindowMinimumWidth { get; set; } = 200;

    /// <summary>
    /// The smallest height the window can go to
    /// </summary>
    public virtual double WindowMinimumHeight { get; set; } = 100;

    /// <summary>
    /// True if the window should be borderless because it is docked or maximized
    /// </summary>
    public virtual bool Borderless =>
        window.WindowState == WindowState.Maximized || dockPosition != WindowDockPosition.Undocked;

    /// <summary>
    /// The size of the resize border around the window
    /// </summary>
    public virtual int ResizeBorder { get; set; } = 6;

    /// <summary>
    /// The size of the resize border around the window, taking into account the outer margin
    /// </summary>
    public virtual Thickness ResizeBorderThickness => new(ResizeBorder + OuterMarginSize);

    /// <summary>
    /// The margin around the window to allow for a drop shadow
    /// </summary>
    public virtual int OuterMarginSize
    {
        // If it is maximized or docked, no border
        get => Borderless ? 0 : outerMarginSize;
        set => outerMarginSize = value;
    }

    /// <summary>
    /// The margin around the window to allow for a drop shadow
    /// </summary>
    public virtual Thickness OuterMarginSizeThickness => new(OuterMarginSize);

    /// <summary>
    /// The margin around the window content
    /// </summary>
    public virtual int ContentBorder
    {
        // If it is maximized or docked, no border
        get => Borderless ? 0 : contentBorder;
        set => contentBorder = value;
    }

    /// <summary>
    /// The margin around the window content
    /// </summary>
    public virtual Thickness ContentBorderThickness => new(ContentBorder);

    /// <summary>
    /// The radius of the edges of the window
    /// </summary>
    public virtual int WindowRadius
    {
        // If it is maximized or docked, no border
        get => Borderless ? 0 : windowRadius;
        set => windowRadius = value;
    }

    /// <summary>
    /// The radius of the edges of the window top.
    /// </summary>
    public virtual CornerRadius WindowTopCornerRadius => new(WindowRadius, WindowRadius, 0, 0);

    /// <summary>
    /// The radius of the edges of the window bottom.
    /// </summary>
    public virtual CornerRadius WindowBottomCornerRadius => new(0);

    /// <summary>
    /// The height of the title bar / caption of the window
    /// </summary>
    public virtual int TitleHeight { get; set; } = 32;

    /// <summary>
    /// The height of the title bar / caption of the window
    /// </summary>
    public virtual GridLength TitleHeightGridLength => new(TitleHeight + ResizeBorder);
    

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor
    /// </summary>
    protected MiniModernWindow(Window window)
    {
        this.window = window;

        // Listen out for the window resizing
        this.window.StateChanged += (_, _) =>
        {
            // Fire off events for all properties that are affected by a resize
            WindowResized();
        };

        // Fix window resize issue
        var resizer = new WindowResizer(this.window);

        // Listen out for dock changes
        resizer.WindowDockChanged += dock =>
        {
            // Store last position
            dockPosition = dock;

            // Fire off resize events
            WindowResized();
        };
    }

    #endregion

    #region Private Helpers

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
        RaisePropertyChanged(nameof(WindowTopCornerRadius));
    }

    #endregion
}