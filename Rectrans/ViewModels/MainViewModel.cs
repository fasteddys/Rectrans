using Prism.Mvvm;
using Rectrans.Views;
using Prism.Commands;
using Rectrans.Models;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using Rectrans.Infrastructure;
using Rectrans.Services.Implement;
using System.Collections.ObjectModel;

namespace Rectrans.ViewModels;

public class MainViewModel : BindableBase
{
    // ReSharper disable once InconsistentNaming
    private readonly CollectionViewSource MenuItemsCollection;
    public ICollectionView SourceCollection => MenuItemsCollection.View;

    public MainViewModel()
    {
        MenuItemsCollection = new() {Source = BindingCommand(MenuItem.DefaultCollection)};
    }

    private ObservableCollection<MenuItem> BindingCommand(IEnumerable<MenuItem> source)
    {
        var items = source as MenuItem[] ?? source.ToArray();
        foreach (var item in items)
        {
            if (item.HasItems)
            {
                BindingCommand(item.ItemsSource!);
            }

            item.Command = MenuCommand;
            item.CommandParameter = item;
        }

        return new ObservableCollection<MenuItem>(items);
    }

    private void MenuSelected(MenuItem? parameter)
    {
        if (parameter?.Group == null) return;

        foreach (var item in ((ObservableCollection<MenuItem>) MenuItemsCollection.Source).FindItems(x =>
                     x.Group == parameter.Group)!)
        {
            if (item != parameter)
            {
                item.IsChecked = false;
            }
        }

        MenuItemsCollection.View.Refresh();

        if (parameter.Key != "Stop" || _timer == null) return;
        _timer.Dispose();
        _timer = null;
    }

    private void Translate()
    {
        var inputWindow = WindowManager.Default.Resolve<InputWindow>()!;
        if (inputWindow.Width == 0 || inputWindow.Height == 0)
        {
            // future: add message box
            return;
        }

        System.Windows.Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            var source = (string) ((ObservableCollection<MenuItem>) MenuItemsCollection.Source)
                .FindItem(x => x.Group == Group.SourceLan && x.IsChecked)!.Extra!;

            var target = (string) ((ObservableCollection<MenuItem>) MenuItemsCollection.Source)
                .FindItem(x => x.Group == Group.TargetLan && x.IsChecked)!.Extra!;

            (SourceText, TargetText) = await ImageTranslate.TranslateAsync(inputWindow.Left,
                inputWindow.Top, inputWindow.Height, inputWindow.Width, source, target);

            TextCount = SourceText.Length;
        });
    }

    private static System.Timers.Timer? _timer;

    private void Confirm()
    {
        var item = ((ObservableCollection<MenuItem>) MenuItemsCollection.Source)
            .FindItem(x => x.Group == Group.AntoTranslate && x.IsChecked);

        if (item != null)
        {
            _timer ??= new System.Timers.Timer();

            _timer.Interval = Convert.ToInt16(item.Extra!);
            _timer.Elapsed += (_, _) => { Translate(); };

            _timer.Start();
        }

        Translate();
    }

    #region UIBinding

    private string? _sourceText;

    public string SourceText
    {
        get => _sourceText ??= string.Empty;
        set
        {
            _sourceText = value;
            RaisePropertyChanged();
        }
    }

    private string? _targetText;

    public string TargetText
    {
        get => _targetText ??= string.Empty;
        set
        {
            _targetText = value;
            RaisePropertyChanged();
        }
    }

    private int _textCount;

    public int TextCount
    {
        get => _textCount;
        set
        {
            _textCount += value;
            RaisePropertyChanged();
        }
    }

    private ICommand? _confirmCommand;

    public ICommand ConfirmCommand => _confirmCommand ??= new DelegateCommand(Confirm);

    private ICommand? _menuCommand;

    // ReSharper disable once MemberCanBePrivate.Global
    public ICommand MenuCommand => _menuCommand ??= new DelegateCommand<MenuItem>(MenuSelected);

    #endregion
}