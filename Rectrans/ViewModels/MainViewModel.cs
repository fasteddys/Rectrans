using Rectrans.Views;
using Rectrans.Models;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using Rectrans.EventHandlers;
using Rectrans.Infrastructure;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace Rectrans.ViewModels;

public class MainViewModel : MessageViewModel
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
        if (parameter?.GroupName == null) return;

        foreach (var item in ((ObservableCollection<MenuItem>) MenuItemsCollection.Source).FindItems(x =>
                     x.GroupName == parameter.GroupName)!)
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

    private ICommand? _menuCommand;

    // ReSharper disable once MemberCanBePrivate.Global
    public ICommand MenuCommand => _menuCommand ??= new DelegateCommand<MenuItem>(MenuSelected);

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

    private void Translate()
    {
        if (Width == 0 || Height == 0)
        {
            // future: add message box
            return;
        }

        System.Windows.Application.Current.Dispatcher.BeginInvoke(async () =>
        {
            var source = (string) ((ObservableCollection<MenuItem>) MenuItemsCollection.Source)
                .FindItem(x => x.GroupName == "源语言" && x.IsChecked)!.Extra!;

            var target = (string) ((ObservableCollection<MenuItem>) MenuItemsCollection.Source)
                .FindItem(x => x.GroupName == "目标语言" && x.IsChecked)!.Extra!;

            (SourceText, TargetText) = await ImageTranslate.TranslateAsync(X, Y, Height, Width, source, target);

            TextCount = SourceText.Length;
        });
    }

    private System.Timers.Timer? _timer;

    private void Confirm()
    {
        var item = ((ObservableCollection<MenuItem>) MenuItemsCollection.Source)
            .FindItem(x => x.GroupName == "自动翻译" && x.IsChecked);

        if (item != null)
        {
            _timer ??= new System.Timers.Timer();

            _timer.Interval = Convert.ToInt16(item.Extra!);
            _timer.Elapsed += (_, _) => { Translate(); };

            _timer.Start();
        }

        Translate();
    }

    private ICommand? _confirmCommand;

    public ICommand ConfirmCommand => _confirmCommand ??= new DelegateCommand(Confirm);

    // public void OnInputWindowAbnormalClosed() =>
    //     Messenger.Default.Send<Message>(new()
    //     {
    //         MessageType = MessageType.Warning,
    //         BorderText = @"您已关闭“翻译框”窗口，请点击""重置""按钮进行恢复！",
    //         Hyperlink = new()
    //         {
    //             Text = "重置",
    //             Command = new RelayCommand(OnMessageBorderHyperlinkClick)
    //         }
    //     });
    
    

    // private void OnMessageBorderHyperlinkClick(object? parameter)
    // {
    //     Messenger.Default.Send<Message>(new() {MessageType = MessageType.Close});
    //     var inputWindow = new InputWindow();
    //
    //     OnInputWindowCreated(inputWindow);
    //
    //     inputWindow.Show();
    // }

    public event InputWindowCreatedEventHandler? InputWindowCreated;

    private void OnInputWindowCreated(InputWindow inputWindow)
    {
        InputWindowCreated?.Invoke(this, new InputWindowCreatedEventArgs(inputWindow));
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}