using System;
using System.IO;
using System.Linq;
using Rectrans.OCR;
using System.Timers;
using Rectrans.View;
using Rectrans.Model;
using Rectrans.Common;
using System.Diagnostics;
using Rectrans.Extensions;
using System.Windows.Input;
using Rectrans.Interpreter;
using Rectrans.EventHandlers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rectrans.ViewModel;

public class MainViewModel : ViewModelBase
{
    private ObservableCollection<MenuItem>? _source;

    public ObservableCollection<MenuItem> Source
    {
        get => _source ??= BindingCommand(AppSettings.MenuItems);
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

    private bool _topMost;

    public bool TopMost
    {
        get => _topMost;
        set
        {
            _topMost = value;
            OnPropertyChanged(nameof(TopMost));
        }
    }

    private void MenuSelected(object? parameter)
    {
        var current = parameter as MenuItem;
        var parent = current!.GetTemplatedParent<MenuItem>();
        if (parent == null)
        {
            if (current!.Key == "TopMost")
            {
                TopMost = !TopMost;
            }

            return;
        }

        foreach (var item in parent.ItemsSource!)
        {
            if (item != current)
            {
                item.IsChecked = false;
            }
        }

        if (current!.Key != "Stop" || _timer == null) return;
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
            var bitmap = Dpi.Screenshot(X, Y, Height, Width);
            // ReSharper disable once UseAwaitUsing
            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            
            bitmap.Save("debug.png", System.Drawing.Imaging.ImageFormat.Png);

            var source = Source.FindItem(x => x.Parent?.Key == "Source" && x.IsChecked)?.Key;
            var target = Source.FindItem(x => x.Parent?.Key == "Target" && x.IsChecked)?.Key;

            if (target == null || source == null) Debugger.Break();

            SourceText = Identify.FromMemory(stream.ToArray(), AppSettings.TrainedData(source));
            TargetText = await Interpret.WithGoogleAsync(SourceText, AppSettings.ISO_639_1(target),
                AppSettings.ISO_639_1(source));

            TextCount = SourceText.Length;
        });
    }

    private Timer? _timer;

    private void Confirm()
    {
        var menuItem = Source.FindItem(x => x.Parent?.Key == "AutomaticTranslation" && x.IsChecked);
        if (menuItem != null)
        {
            _timer ??= new Timer();

            _timer.Interval = Convert.ToInt16(menuItem.Extra!);
            _timer.Elapsed += (_, _) => { Translate(); };

            _timer.Start();
        }

        Translate();
    }

    private ICommand? _confirmCommand;

    public ICommand ConfirmCommand => _confirmCommand ??= new RelayCommand(_ => Confirm());

    public void OnRectangleViewAbnormalClosed() =>
        MessageBorderMonitor.OnWarningWithHyperlinkText(@"您已关闭“翻译区域”窗口，请点击""重置""按钮进行恢复！", "重置");

    public override void OnMessageBorderHyperlinkClick(object? parameter)
    {
        MessageBorderMonitor.CloseMessageBorder();
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