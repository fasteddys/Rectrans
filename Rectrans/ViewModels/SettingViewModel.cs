using System.Collections.ObjectModel;
using Prism.Events;
using Prism.Mvvm;
using Rectrans.Events;
using Rectrans.Models;
using Rectrans.Views;

// ReSharper disable ClassNeverInstantiated.Global

// ReSharper disable InconsistentNaming

namespace Rectrans.ViewModels;

public class SettingViewModel : BindableBase
{
    #region Private Members

    /// <summary>
    /// The view this view model controls
    /// </summary>
    private SettingView settingView;

    /// <summary>
    /// The ioc event aggregator
    /// </summary>
    private readonly IEventAggregator aggregator;

    /// <summary>
    /// The comboBox selected value
    /// </summary>
    private ComboBoxItem selectedSourceLanguageValue = new() {Name = "英语", Value = "en"};

    /// <summary>
    /// The comboBox selected value
    /// </summary>
    private ComboBoxItem selectedDestinationLanguageValue = new() {Name = "中文", Value = "zh"};

    /// <summary>
    /// The comboBox selected value
    /// </summary>
    private ComboBoxItem selectedAutomaticTranslationInterval = new() {Name = "手动", Value = "0"};

    #endregion

    #region Public Properties

    public ObservableCollection<ComboBoxItem> LanguageItems { get; set; }

    public ObservableCollection<ComboBoxItem> IntervalItems { get; set; }

    /// <summary>
    /// The comboBox selected value
    /// </summary>
    public ComboBoxItem SelectedSourceLanguageValue
    {
        get => selectedSourceLanguageValue;
        set
        {
            selectedSourceLanguageValue = value;
            RaisePropertyChanged();
            PublishSettingEvent();
        }
    }

    /// <summary>
    /// The comboBox selected value
    /// </summary>
    public ComboBoxItem SelectedDestinationLanguageValue
    {
        get => selectedDestinationLanguageValue;
        set
        {
            selectedDestinationLanguageValue = value;
            RaisePropertyChanged();
            PublishSettingEvent();
        }
    }

    /// <summary>
    /// The comboBox selected value
    /// </summary>
    public ComboBoxItem SelectedAutomaticTranslationInterval
    {
        get => selectedAutomaticTranslationInterval;
        set
        {
            selectedAutomaticTranslationInterval = value;
            RaisePropertyChanged();
            PublishSettingEvent();
        }
    }

    #endregion

    #region Constructor

    public SettingViewModel(SettingView view, IEventAggregator aggregator)
    {
        settingView = view;
        this.aggregator = aggregator;

        LanguageItems = new ObservableCollection<ComboBoxItem>(DataAccess.GetLanguageItems());
        IntervalItems = new ObservableCollection<ComboBoxItem>(DataAccess.GetIntervalItems());
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Publish setting event
    /// </summary>
    private void PublishSettingEvent()
    {
        aggregator.GetEvent<SettingEvent>().Publish(new SettingEvent
        {
            SourceLanguage = SelectedSourceLanguageValue.Value,
            DestinationLanguage = SelectedDestinationLanguageValue.Value,
            AutomaticTranslationInterval = int.Parse(SelectedAutomaticTranslationInterval.Value)
        });
    }

    #endregion
}