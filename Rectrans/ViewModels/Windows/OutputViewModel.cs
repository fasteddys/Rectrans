using Prism.Events;
using System.Windows;
using Prism.Commands;
using Rectrans.Events;
using Rectrans.Extensions;
using System.Windows.Input;
using System.Windows.Media;
using Rectrans.Views.Windows;

// ReSharper disable FieldCanBeMadeReadOnly.Local

// ReSharper disable InconsistentNaming

namespace Rectrans.ViewModels.Windows;

/// <summary>
/// The View Model for the custom flat window
/// </summary>
public class OutputViewModel : MiniModernWindow
{
    #region Private Member

    /// <summary>
    /// The window this view model controls
    /// </summary>
    private OutputWindow outputWindow;

    /// <summary>
    /// The text of output.
    /// </summary>
    private string translationText = "此窗口为翻译后文字的输出窗口，请拖动此窗口至方便查看的位置。";

    /// <summary>
    /// The font size of out put text block
    /// </summary>
    private double fontSize = 18;

    /// <summary>
    /// the foreground of out put text block
    /// </summary>
    private Brush foreground = Brushes.Chocolate;

    #endregion

    #region Public Properties

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
    public double FontSize
    {
        get => fontSize;
        set
        {
            fontSize = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// the foreground of out put text block
    /// </summary>
    public Brush Foreground
    {
        get => foreground;
        set
        {
            foreground = value;
            RaisePropertyChanged();
        }
    }

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
        : base(window)
    {
        outputWindow = window;

        // Listen out for the window size changed
        outputWindow.SizeChanged += (_, _) => CalculateFontSize();

        // Create commands
        MinimizeCommand = new DelegateCommand(() => outputWindow.WindowState = WindowState.Minimized);
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        MaximizeCommand = new DelegateCommand(() => outputWindow.WindowState ^= WindowState.Maximized);
        CloseCommand = new DelegateCommand(() => outputWindow.Close());

        // Subscribe output event
        aggregator.GetEvent<OutputEvent>().Subscribe(arg =>
        {
            TranslationText = arg.TranslationText;
            CalculateFontSize();
        });
    }

    #endregion

    #region Private Helpers

    private void CalculateFontSize()
    {
        FontSize = outputWindow.OutputTextBlock.CalculateFontSize(TranslationText, "Lato Thin", "zh-cn");
        RaisePropertyChanged(nameof(FontSize));
    }

    #endregion
}