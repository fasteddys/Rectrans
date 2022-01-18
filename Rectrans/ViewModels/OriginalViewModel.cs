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
    /// The text of untranslated
    /// </summary>
    private string originalText = "";

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

    #endregion

    #region Constructor

    public OriginalViewModel(OriginalView view, IEventAggregator aggregator)
    {
        originalView = view;

        // Subscribe output event
        aggregator.GetEvent<OutputEvent>().Subscribe(arg => { OriginalText = arg.OriginalText; });
    }

    #endregion
}