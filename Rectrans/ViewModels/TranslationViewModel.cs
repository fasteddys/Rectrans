using Prism.Events;
using Prism.Mvvm;
using Rectrans.Events;
using Rectrans.Views;

// ReSharper disable InconsistentNaming

namespace Rectrans.ViewModels;

public class TranslationViewModel : BindableBase
{
    #region Private Members

    /// <summary>
    /// The view this view model controls
    /// </summary>
    private readonly TranslationView translationView;

    /// <summary>
    /// The output text block width
    /// </summary>
    private int textBlockWidth = 550;

    /// <summary>
    /// The text of translated
    /// </summary>
    private string translationText = "";

    /// <summary>
    /// The font size of out put text block
    /// </summary>
    private double fontSize = 20;

    #endregion

    #region Public Properties

    /// <summary>
    /// The text of translated
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

    public TranslationViewModel(TranslationView view, IEventAggregator aggregator)
    {
        translationView = view;

        // Subscribe output event
        aggregator.GetEvent<OutputEvent>().Subscribe(arg => TranslationText = arg.TranslationText);
    }

    #endregion
}