using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Rectrans.Events;
using Rectrans.Extensions;
using Rectrans.Helpers;
using Rectrans.Views.Windows;

// ReSharper disable FieldCanBeMadeReadOnly.Local

// ReSharper disable InconsistentNaming

namespace Rectrans.ViewModels.Windows;

/// <summary>
/// The View Model for the custom flat window
/// </summary>
public class OutputViewModel : BindableBase
{
    #region Private Member

    /// <summary>
    /// The window this view model controls
    /// </summary>
    private OutputWindow outputWindow;

    /// <summary>
    /// The margin around the window to allow for a drop shadow
    /// </summary>
    private int mOuterMarginSize = 10;

    /// <summary>
    /// The margin around the window content.
    /// </summary>
    private int mInnerMarginSize = 2;

    /// <summary>
    /// The radius of the edges of the window
    /// </summary>
    private int mWindowRadius = 10;

    /// <summary>
    /// The last known dock position
    /// </summary>
    private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

    /// <summary>
    /// The text of output.
    /// </summary>
    private string translationText = "此窗口为翻译后文字的输出窗口，请拖动此窗口至方便查看的位置！";

    #endregion

    #region Public Properties

    /// <summary>
    /// The smallest width the window can go to
    /// </summary>
    public double WindowMinimumWidth { get; set; } = 200;

    /// <summary>
    /// The smallest height the window can go to
    /// </summary>
    public double WindowMinimumHeight { get; set; } = 100;

    /// <summary>
    /// True if the window should be borderless because it is docked or maximized
    /// </summary>
    public bool Borderless =>
        (outputWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked);

    /// <summary>
    /// The size of the resize border around the window
    /// </summary>
    public int ResizeBorder { get; set; } = 6;

    /// <summary>
    /// The size of the resize border around the window, taking into account the outer margin
    /// </summary>
    public Thickness ResizeBorderThickness => new(ResizeBorder + OuterMarginSize);

    /// <summary>
    /// The margin around the window to allow for a drop shadow
    /// </summary>
    public int OuterMarginSize
    {
        // If it is maximized or docked, no border
        get => Borderless ? 0 : mOuterMarginSize;
        set => mOuterMarginSize = value;
    }

    /// <summary>
    /// The margin around the window to allow for a drop shadow
    /// </summary>
    public Thickness OuterMarginSizeThickness => new(OuterMarginSize);

    /// <summary>
    /// The margin around the window content
    /// </summary>
    public int InnerMarginSize =>
        // If it is maximized or docked, no border
        Borderless ? 0 : mInnerMarginSize;

    /// <summary>
    /// The margin around the window content
    /// </summary>
    public Thickness InnerMarginSizeThickness => new(InnerMarginSize);

    /// <summary>
    /// The radius of the edges of the window
    /// </summary>
    public int WindowRadius
    {
        // If it is maximized or docked, no border
        get => Borderless ? 0 : mWindowRadius;
        set => mWindowRadius = value;
    }

    /// <summary>
    /// The radius of the edges of the window top.
    /// </summary>
    public CornerRadius WindowTopCornerRadius => new(WindowRadius, WindowRadius, 0, 0);

    /// <summary>
    /// The radius of the edges of the window bottom.
    /// </summary>
    public CornerRadius WindowBottomCornerRadius => new(0, 0, WindowRadius, WindowRadius);

    /// <summary>
    /// The height of the title bar / caption of the window
    /// </summary>
    public int TitleHeight { get; set; } = 32;

    /// <summary>
    /// The height of the title bar / caption of the window
    /// </summary>
    public GridLength TitleHeightGridLength => new(TitleHeight + ResizeBorder);

    /// <summary>
    /// The padding of the inner content of the main window
    /// </summary>
    public Thickness InnerContentPadding => new(ResizeBorder);

    /// <summary>
    /// The text of output.
    /// </summary>
    public string TranslationText
    {
        get => translationText;
        set
        {
            translationText = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// The font size of out put text block.
    /// </summary>
    public double FontSize { get; set; }

    #endregion

    #region Commands

    /// <summary>
    /// The command to minimize the window
    /// </summary>
    public ICommand MinimizeCommand { get; set; }

    /// <summary>
    /// The command to maximize the window
    /// </summary>
    public ICommand MaximizeCommand { get; set; }

    /// <summary>
    /// The command to close the window
    /// </summary>
    public ICommand CloseCommand { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor
    /// </summary>
    public OutputViewModel(OutputWindow window, IEventAggregator aggregator)
    {
        outputWindow = window;

        // Listen out for the window resizing
        outputWindow.StateChanged += (_, _) =>
        {
            // Fire off events for all properties that are affected by a resize
            WindowResized();
        };

        // Listen out for the window size changed
        outputWindow.SizeChanged += (_, _) => CalculateFontSize();

        // Create commands
        MinimizeCommand = new DelegateCommand(() => outputWindow.WindowState = WindowState.Minimized);
        MaximizeCommand = new DelegateCommand(() => outputWindow.WindowState ^= WindowState.Maximized);
        CloseCommand = new DelegateCommand(() => outputWindow.Close());

        // Fix window resize issue
        var resizer = new WindowResizer(outputWindow);

        // Listen out for dock changes
        resizer.WindowDockChanged += dock =>
        {
            // Store last position
            mDockPosition = dock;

            // Fire off resize events
            WindowResized();
        };

        // Subscribe output event
        aggregator.GetEvent<OutputEvent>().Subscribe(arg =>
        {
            TranslationText = arg.TranslationText;
            CalculateFontSize();
        });
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

    private void CalculateFontSize()
    {
        FontSize = outputWindow.OutputTextBlock.CalculateFontSize(TranslationText, "Lato Thin", "zh-cn");
        RaisePropertyChanged(nameof(FontSize));
    }

    #endregion
}