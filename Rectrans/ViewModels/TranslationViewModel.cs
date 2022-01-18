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
    private TranslationView translationView;

    /// <summary>
    /// The text of translated
    /// </summary>
    private string translationText = "";

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

    #endregion

    #region Constructor

    public TranslationViewModel(TranslationView view, IEventAggregator aggregator)
    {
        translationView = view;

        // Subscribe output event
        aggregator.GetEvent<OutputEvent>().Subscribe(arg => { TranslationText = arg.TranslationText; });
    }

    #endregion
}