using Rectrans.OCR;
using Rectrans.Mvvm;
using Rectrans.Views;
using Rectrans.Models;
using Rectrans.Utilities;
using System.Diagnostics;
using Rectrans.Interpreter;
using Rectrans.Mvvm.Common;
using System.Windows.Input;
using Rectrans.EventHandlers;
using Rectrans.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace Rectrans.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ObservableCollection<MenuItem>? _source;

    public ObservableCollection<MenuItem> Source
    {
        get
        {
            if (_source == null)
            {
                _source = BindingCommand(AppSettings.MenuItems);
            }

            return _source;
        }
        set
        {
            _source = value;
            OnPropertyChanged(nameof(Source));
        }
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

    private void MenuSelected(object? parameter)
    {
        var current = parameter as MenuItem;
        var parent = current!.Parent;
        if (parent == null) return;

        foreach (var item in parent.ItemsSource!)
        {
            if (item != current)
            {
                item.IsChecked = false;
            }
        }

        if (current.Key != "Stop" || _timer == null) return;
        _timer.Dispose();
        _timer = null;
    }

    private ICommand? _menuCommand;

    // ReSharper disable once MemberCanBePrivate.Global
    public ICommand MenuCommand => _menuCommand ??= new RelayCommand(MenuSelected);

    private string? _sourceText;

    public string SourceText
    {
        get => _sourceText ??= string.Empty;
        set
        {
            _sourceText = value;
            OnPropertyChanged(nameof(SourceText));
        }
    }

    private string? _targetText;

    public string TargetText
    {
        get => _targetText ??= string.Empty;
        set
        {
            _targetText = value;
            OnPropertyChanged(nameof(TargetText));
        }
    }

    private int _textCount;

    private int TextCount
    {
        get => _textCount;
        set
        {
            _textCount += value;
            OnPropertyChanged(nameof(StatusBarText));
        }
    }

    public string StatusBarText => "字数统计(字节): " + TextCount;

    private void Translate()
    {
        if (Width == 0 || Height == 0)
        {
            // future: add message box
            return;
        }

        Context.BeginInvoke(async () =>
        {
            var source = Source.FindItem(x => x.Parent?.Key == "Source" && x.IsChecked)?.Key;
            var target = Source.FindItem(x => x.Parent?.Key == "Target" && x.IsChecked)?.Key;

            if (target == null || source == null) Debugger.Break();

            var sourceText = Identify.FromScreen(X, Y, Height, Width, AppSettings.TrainedData(source));

            // if the sourceText not change, return
            if (sourceText == SourceText) return;

            SourceText = sourceText;
            TargetText = await Interpret.WithGoogleAsync(sourceText, AppSettings.ISO_639_1(target),
                AppSettings.ISO_639_1(source));

            TextCount = sourceText.Length;
        });
    }

    private System.Timers.Timer? _timer;

    private void Confirm()
    {
        var menuItem = Source.FindItem(x => x.Parent?.Key == "AutomaticTranslation" && x.IsChecked);
        if (menuItem != null)
        {
            _timer ??= new System.Timers.Timer();

            _timer.Interval = Convert.ToInt16(menuItem.Extra!);
            _timer.Elapsed += (_, _) => { Translate(); };

            _timer.Start();
        }

        Translate();
    }

    private ICommand? _confirmCommand;

    public ICommand ConfirmCommand => _confirmCommand ??= new RelayCommand(_ => Confirm());

    public void OnRectangleViewAbnormalClosed() =>
        Messenger.Default.Send<Message>(new()
        {
            MessageType = MessageType.Warning,
            BorderText = @"您已关闭“翻译框”窗口，请点击""重置""按钮进行恢复！",
            Hyperlink = new()
            {
                Text = "重置",
                Command = new RelayCommand(OnMessageBorderHyperlinkClick)
            }
        });

    private void OnMessageBorderHyperlinkClick(object? parameter)
    {
        Messenger.Default.Send<Message>(new() {MessageType = MessageType.Close});
        var rectangleView = new RectangleView();

        OnRectangleViewCreated(rectangleView);

        rectangleView.Show();
    }

    public event RectangleViewCreatedEventHandler? RectangleViewCreated;

    private void OnRectangleViewCreated(RectangleView rectangleView)
    {
        RectangleViewCreated?.Invoke(this, new RectangleViewCreatedEventArgs(rectangleView));
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}