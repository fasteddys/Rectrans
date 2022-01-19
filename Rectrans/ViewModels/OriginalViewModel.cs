using Prism.Events;
using Prism.Mvvm;
using Rectrans.Events;
using Rectrans.Views;

// ReSharper disable InconsistentNaming

namespace Rectrans.ViewModels;

public class OriginalViewModel : BindableBase
{
    #region Private Members

    /// <summary>
    /// The view this view model controls
    /// </summary>
    private OriginalView originalView;

    /// <summary>
    /// The output text block width
    /// </summary>
    private int textBlockWidth = 550;

    /// <summary>
    /// The text of untranslated
    /// </summary>
    private string originalText = "";

    /// <summary>
    /// The font size of out put text block
    /// </summary>
    private double fontSize = 20;

    #endregion

    #region Public Properties

    /// <summary>
    /// The text of untranslated
    /// </summary>
    public string OriginalText
    {
        get => originalText;
        set
        {
            originalText = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// The output text block width
    /// </summary>
    public int TextBlockWidth
    {
        get => textBlockWidth;
        set
        {
            textBlockWidth = value;
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

    #endregion

    #region Constructor

    public OriginalViewModel(OriginalView view, IEventAggregator aggregator)
    {
        originalView = view;

        // Subscribe output event
        aggregator.GetEvent<OutputEvent>().Subscribe(arg => OriginalText = arg.OriginalText);
    }

    #endregion
}