using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Rectrans.Events;
using Rectrans.Helpers;
using Rectrans.Infrastructure;
using Rectrans.Views.Windows;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Rectrans.ViewModels.Windows;

/// <summary>
/// The View Model for the custom flat window
/// </summary>
public class InputViewModel : BindableBase
{
    #region Private Member

    /// <summary>
    /// The window this view model controls
    /// </summary>
    private readonly InputWindow inputWindow;

    /// <summary>
    /// The ioc event aggregator
    /// </summary>
    private readonly IEventAggregator aggregator;

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
    /// The timer for auto mode
    /// </summary>
    private DispatcherTimer? autoModeTimer;

    #endregion

    #region Public Properties

    /// <summary>
    /// The smallest width the window can go to
    /// </summary>
    public double WindowMinimumWidth { get; set; } = 400;

    /// <summary>
    /// The smallest height the window can go to
    /// </summary>
    public double WindowMinimumHeight { get; set; } = 200;

    /// <summary>
    /// True if the window should be borderless because it is docked or maximized
    /// </summary>
    public bool Borderless => inputWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked;

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
    public int InnerMarginSize
    {
        
        // If it is maximized or docked, no border
        get => Borderless ? 0 : mInnerMarginSize;
        set => mInnerMarginSize = value;
    }

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
    /// The selected auto translate item, for others set
    /// </summary>
    public int AutomaticTranslationInterval { get; set; }

    /// <summary>
    /// The source of translate language
    /// </summary>
    public string SourceLanguage { get; set; } = "en";

    /// <summary>
    /// The destination of translate language
    /// </summary>
    public string DestinationLanguage { get; set; } = "zh";

    #endregion

    #region Commands

    /// <summary>
    /// The command to auto translate
    /// </summary>
    public ICommand AutoModeCommand { get; set; }

    /// <summary>
    /// The command of translate
    /// </summary>
    public ICommand TranslateCommand { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor
    /// </summary>
    public InputViewModel(InputWindow window, IEventAggregator aggregator)
    {
        inputWindow = window;
        this.aggregator = aggregator;

        // Listen out for the window resizing
        inputWindow.StateChanged += (_, _) =>
        {
            // Fire off events for all properties that are affected by a resize
            WindowResized();
        };

        // Create commands
        AutoModeCommand = new DelegateCommand(async()=> await AutoMode());
        TranslateCommand = new DelegateCommand(async()=> await Translate());

        // Fix window resize issue
        var resizer = new WindowResizer(inputWindow);

        // Listen out for dock changes
        resizer.WindowDockChanged += (dock) =>
        {
            // Store last position
            mDockPosition = dock;

            // Fire off resize events
            WindowResized();
        };
        
        // Subscribe setting event
        aggregator.GetEvent<SettingEvent>().Subscribe(arg =>
        {
            SourceLanguage = arg.SourceLanguage;
            DestinationLanguage = arg.DestinationLanguage;
            AutomaticTranslationInterval = arg.AutomaticTranslationInterval;
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

    private async Task AutoMode()
    {
        // dosometing
        if (AutomaticTranslationInterval == 0) return;

        autoModeTimer ??= new DispatcherTimer();

        // The second execution is for stop timer
        if (autoModeTimer.IsEnabled)
        {
            autoModeTimer.Stop();
            return;
        }

        // Config the timer
        autoModeTimer.Interval = TimeSpan.FromMilliseconds(AutomaticTranslationInterval);
        autoModeTimer.Tick += async (_, _) => await Translate();

        // Execution immediately
        await Translate();

        // Start the timer
        autoModeTimer.Start();
    }

    private async Task Translate()
    {
        var textBlock = (TextBlock)inputWindow.FindName("TranslationAreaBlock")!;
        var point = textBlock.PointToScreen(new Point(0, 0));
        
        // Translate text
        var (original, translation, count) = await ImageTranslate.ExecuteAsync(point.X,
            point.Y, textBlock.ActualHeight, textBlock.ActualWidth, SourceLanguage, DestinationLanguage);

        // Publish the output event
        aggregator.GetEvent<OutputEvent>().Publish(new OutputEvent
        {
            OriginalText = original,
            TranslationText = translation,
            Count = count,
        });
    }

    #endregion
}