using Prism.Mvvm;
using Prism.Events;
using System.Windows;
using Prism.Commands;
using Rectrans.Events;
using Rectrans.Helpers;
using System.Windows.Input;
using Rectrans.Views.Windows;
using Rectrans.Infrastructure;
using System.Windows.Threading;
using ToastNotifications.Messages;

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
    private int outerMarginSize = 10;

    /// <summary>
    /// The margin around the window content.
    /// </summary>
    private int innerMarginSize = 2;

    /// <summary>
    /// The radius of the edges of the window
    /// </summary>
    private int windowRadius = 10;

    /// <summary>
    /// The last known dock position
    /// </summary>
    private WindowDockPosition dockPosition = WindowDockPosition.Undocked;

    /// <summary>
    /// The timer for auto mode
    /// </summary>
    private DispatcherTimer? autoModeTimer;

    /// <summary>
    /// The auto mode system button checked or ont
    /// </summary>
    private bool autoModeButtonIsChecked;

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
        inputWindow.WindowState == WindowState.Maximized || dockPosition != WindowDockPosition.Undocked;

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
        get => Borderless ? 0 : outerMarginSize;
        set => outerMarginSize = value;
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
        get => Borderless ? 0 : innerMarginSize;
        set => innerMarginSize = value;
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
        get => Borderless ? 0 : windowRadius;
        set => windowRadius = value;
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
    /// The auto mode system button checked or ont
    /// </summary>
    public bool AutoModeButtonIsChecked
    {
        get => autoModeButtonIsChecked;
        set
        {
            autoModeButtonIsChecked = value;
            RaisePropertyChanged();
        }
    }

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
        AutoModeCommand = new DelegateCommand(async () => await AutoMode());
        TranslateCommand = new DelegateCommand(async () =>
        {
            // Check the timer enable

            if (autoModeTimer is {IsEnabled: true})
            {
                inputWindow.Notifier.ShowWarning("自动翻译模式已开启，请先关闭自动模式。");
                return;
            }
            
            await Translate();
        });

        // Fix window resize issue
        var resizer = new WindowResizer(inputWindow);

        // Listen out for dock changes
        resizer.WindowDockChanged += dock =>
        {
            // Store last position
            dockPosition = dock;

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
        // The second execution is for stop timer
        if (autoModeTimer is {IsEnabled: true})
        {
            autoModeTimer.Stop();
            return;
        }

        if (AutomaticTranslationInterval == 0)
        {
            inputWindow.Notifier.ShowInformation("未设置自动翻译时间间隔，请在主界面选项栏中进行设置。");
            AutoModeButtonIsChecked = false;
            return;
        }

        // If timer is null, new one
        autoModeTimer ??= new DispatcherTimer();

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
        var textBlock = inputWindow.TranslationAreaBlock;
        var point = textBlock.PointToScreen(new Point(0, 0));

        // Start another thread to perform this task
        await Task.Run(async () =>
        {
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
        });
    }

    #endregion
}