using System.Windows;
using Rectrans.Extensions;
using Rectrans.Views;
using Prism.Commands;
using Rectrans.Models;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using Rectrans.Infrastructure;
using Rectrans.Services.Implement;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Rectrans.ViewModels;

public class MainViewModel : ModernWindow
{
    #region Private Member

    // ReSharper disable once InconsistentNaming
    private readonly CollectionViewSource MenuItemsCollection;

    /// <summary>
    /// The timer for auto translate.
    /// </summary>
    private DispatcherTimer timer = new();

    /// <summary>
    /// The text for source text.
    /// </summary>
    private string? sourceText;

    /// <summary>
    /// The text for target text.
    /// </summary>
    private string? targetText;

    /// <summary>
    /// The count of source text.
    /// </summary>
    private int textCount;

    #endregion

    #region Public Properties

    public ICollectionView SourceCollection => MenuItemsCollection.View;

    /// <summary>
    /// The text for source text.
    /// </summary>
    public string SourceText
    {
        get => sourceText ??= string.Empty;
        set
        {
            sourceText = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// The text for target text.
    /// </summary>
    public string TargetText
    {
        get => targetText ??= string.Empty;
        set
        {
            targetText = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// The total count of source text.
    /// </summary>
    public int TextCount
    {
        get => textCount;
        set
        {
            textCount += value;
            RaisePropertyChanged();
        }
    }

    #endregion

    #region Commands

    /// <summary>
    /// The command of Translate.
    /// </summary>
    public ICommand TranslateCommand { get; set; }

    /// <summary>
    /// The command of menu item.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public ICommand MenuItemCommand { get; set; }

    #endregion

    #region Construcotr

    public MainViewModel(Window window)
        : base(window)
    {
        // Create commands
        TranslateCommand = new DelegateCommand(Translate);
        MenuItemCommand = new DelegateCommand<MenuItem>(MenuItemSelected);

        // Create menu items
        MenuItemsCollection = new() { Source = BindingCommand(MenuItem.DefaultCollection) };
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Binding command for menu items.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private ObservableCollection<MenuItem> BindingCommand(IEnumerable<MenuItem> source)
    {
        var items = source as MenuItem[] ?? source.ToArray();
        foreach (var item in items)
        {
            if (item.HasItems)
            {
                BindingCommand(item.ItemsSource!);
            }

            item.Command = MenuItemCommand;
            item.CommandParameter = item;
        }

        return new ObservableCollection<MenuItem>(items);
    }

    /// <summary>
    /// The command of menu item.
    /// </summary>
    /// <param name="parameter"></param>
    private void MenuItemSelected(MenuItem? parameter)
    {
        if (parameter?.Group == null) return;

        foreach (var item in ((ObservableCollection<MenuItem>)MenuItemsCollection.Source).FindItems(x =>
                    x.Group == parameter.Group)!)
        {
            if (item != parameter)
            {
                item.IsChecked = false;
            }
        }

        MenuItemsCollection.View.Refresh();

        if (parameter.Header != "停止") return;
        timer.Stop();
    }

    /// <summary>
    /// Execute translate.
    /// </summary>
    private async void Translate()
    {
        var inputWindow = WindowManager.Default.Resolve<InputWindow>()!;
        if (inputWindow.Width == 0 || inputWindow.Height == 0)
        {
            // future: add message box
            return;
        }

        var source = (string)((ObservableCollection<MenuItem>)MenuItemsCollection.Source)
            .FindItem(x => x.Group == Group.SourceLan && x.IsChecked)!.Extra!;

        var target = (string)((ObservableCollection<MenuItem>)MenuItemsCollection.Source)
            .FindItem(x => x.Group == Group.TargetLan && x.IsChecked)!.Extra!;

        var item = ((ObservableCollection<MenuItem>)MenuItemsCollection.Source)
            .FindItem(x => x.Group == Group.AntoTranslate && x.IsChecked);

        if (item != null)
        {
            timer.Tick += new EventHandler(async (_, _) =>
            {
                (SourceText, TargetText, TextCount) = await ImageTranslate.ExecuteAsync(inputWindow.Left,
                inputWindow.Top, inputWindow.Height, inputWindow.Width, source, target);
            });

            timer.Interval = TimeSpan.FromMilliseconds(Convert.ToDouble(item.Extra!));
            timer.Start();
        }

        (SourceText, TargetText, TextCount) = await ImageTranslate.ExecuteAsync(inputWindow.Left,
                inputWindow.Top, inputWindow.Height, inputWindow.Width, source, target);
    }

    #endregion
}