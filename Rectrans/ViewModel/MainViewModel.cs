using System;
using System.IO;
using System.Linq;
using Rectrans.OCR;
using System.Timers;
using System.Drawing;
using Rectrans.Model;
using Rectrans.Common;
using System.Diagnostics;
using Rectrans.Extensions;
using System.Windows.Input;
using Rectrans.Interpreter;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rectrans.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MenuItem>? _source;

        public ObservableCollection<MenuItem> Source
        {
            get
            {
                if (_source == null)
                {
                    _source = BindingCommand(MainModel.Fetch());
                }

                return _source;
            }
            set
            {
                _source = value;
                OnPropertyChanged("Source");
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

        // Implement interface member for INotifyPropertyChanged.
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private bool _topMost;

        public bool TopMost
        {
            get => _topMost;
            set
            {
                _topMost = value;
                OnPropertyChanged("TopMost");
            }
        }

        private void MenuSelected(object? parameter)
        {
            var current = parameter as System.Windows.Controls.MenuItem;
            var parent = current!.GetTemplatedParent<System.Windows.Controls.MenuItem>();
            if (parent == null)
            {
                if ((string) current!.Header == "窗口置顶")
                {
                    TopMost = !TopMost;
                }

                return;
            }

            foreach (System.Windows.Controls.MenuItem item in parent.Items)
            {
                if (item != current)
                {
                    item.IsChecked = false;
                }
            }

            if (current!.Name != "停止" || _timer == null) return;
            _timer.Dispose();
            _timer = null;
        }

        private ICommand? _menuCommand;

        // ReSharper disable once MemberCanBePrivate.Global
        public ICommand MenuCommand
        {
            get
            {
                if (_menuCommand == null)
                {
                    _menuCommand = new RelayCommand(MenuSelected);
                }

                return _menuCommand;
            }
        }

        private string? _sourceText;

        public string SourceText
        {
            get => _sourceText ??= string.Empty;
            set
            {
                _sourceText = value;
                OnPropertyChanged("SourceText");
            }
        }

        private string? _targetText;

        public string TargetText
        {
            get => _targetText ??= string.Empty;
            set
            {
                _targetText = value;
                OnPropertyChanged("TargetText");
            }
        }

        private int _textCount;

        private int TextCount
        {
            get => _textCount;
            set
            {
                _textCount += value;
                OnPropertyChanged("StatusBarText");
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

            System.Windows.Application.Current.Dispatcher.BeginInvoke(async () =>
            {
                var bitmap = new Bitmap(Width, Height);
                using var graphics = Graphics.FromImage(bitmap);
                // ReSharper disable once UseAwaitUsing
                using var stream = new MemoryStream();
                graphics.CopyFromScreen(X, Y, 0, 0, new Size(Width, Height));
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                var source = Source.FindItem(x => x.Parent?.Name == "Source" && x.IsChecked)?.Name;
                var target = Source.FindItem(x => x.Parent?.Name == "Target" && x.IsChecked)?.Name;

                if (target == null || source == null) Debugger.Break();

                SourceText = Identify.FromMemory(stream.ToArray(), Settings.TrainedData(source));
                TargetText = await Interpret.WithGoogleAsync(SourceText, Settings.ISO_639_1(target),
                    Settings.ISO_639_1(source));

                TextCount = SourceText.Length;
            });
        }

        private Timer? _timer;

        private void Confirm()
        {
            var menuItem = Source.FindItem(x => (string) x.Parent?.Header! == "自动翻译" && x.IsChecked);
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

        public ICommand ConfirmCommand
        {
            get { return _confirmCommand ??= new RelayCommand(_ => Confirm()); }
        }

        public static int X { get; set; }
        public static int Y { get; set; }
        public static int Width { get; set; }
        public static int Height { get; set; }
    }
}