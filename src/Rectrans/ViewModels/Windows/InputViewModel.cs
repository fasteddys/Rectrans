using Prism.Events;
using System.Windows;
using Prism.Commands;
using Rectrans.Events;
using System.Windows.Input;
using Rectrans.Views.Windows;
using Rectrans.Infrastructure;
using System.Windows.Threading;
using Rectrans.ViewModels.Base;
using ToastNotifications.Messages;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Rectrans.ViewModels.Windows;

/// <summary>
/// The View Model for the custom flat window
/// </summary>
public class InputViewModel : MiniModernWindow
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
        : base(window)
    {
        inputWindow = window;
        this.aggregator = aggregator;

        // Create commands
        // ReSharper disable once AsyncVoidLambda
        AutoModeCommand = new DelegateCommand(async () => await AutoMode());

        // ReSharper disable once AsyncVoidLambda
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
    /// Auto translate mode
    /// </summary>
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

    /// <summary>
    /// Execution translate
    /// </summary>
    private async Task Translate()
    {
        var textBlock = inputWindow.TranslationAreaBlock;
        
        // This is the actual point in the screen whatever the scaling
        var point = textBlock.PointToScreen(new Point(0, 0));

        // Start another thread to perform this task
        await Task.Run(async () =>
        {
            // Translate text
            var (original, translation, count) = await ScreenTranslator.TranslateAsync(
                scaling => (point.X, point.Y, textBlock.ActualHeight * scaling, textBlock.ActualWidth * scaling),
                SourceLanguage, DestinationLanguage, outputPng);

            // Publish the output event
            aggregator.GetEvent<OutputEvent>().Publish(new OutputEvent
            {
                OriginalText = original,
                TranslationText = translation,
                Count = count,
            });
        });
    }

    private const bool outputPng =
#if DEBUG
        true;
#else
        false;
#endif

    #endregion
}