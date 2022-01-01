using System.IO;
using System.Linq;
using Rectrans.OCR;
using System.Timers;
using System.Drawing;
using Rectrans.Model;
using Rectrans.Extensions;
using System.Windows.Input;
using Rectrans.Interpreter;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Rectrans.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MenuItem>? _sourceCollection;

        public ObservableCollection<MenuItem> SourceCollection
        {
            get
            {
                if (_sourceCollection == null)
                {
                    _sourceCollection = BindingCommand(MainModel.Fetch());
                }

                return _sourceCollection;
            }
            set
            {
                _sourceCollection = value;
                OnPropertyChanged("SourceCollection");
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

        public string StatusBarText
        {
            get
            {
                _textCount += SourceText.Length;
                return "字数统计(字节): " + _textCount;
            }
        }

        private async Task TranslateAsync()
        {
            if (Width == 0 || Height == 0)
            {
                // future: add message box
                return;
            }

            var bitmap = new Bitmap(Width, Height);
            using var graphics = Graphics.FromImage(bitmap);
            using var stream = new MemoryStream();

            graphics.CopyFromScreen(X, Y, 0, 0, new Size(Width, Height));
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            var source = SourceCollection.Recursion(x => x.Parent?.Name == "SourceLanguage" && x.IsChecked)?.Name;
            var target = SourceCollection.Recursion(x => x.Parent?.Name == "TargetLanguage" && x.IsChecked)?.Name;

            SourceText = Identify.FromMemory(stream.ToArray(), Language.FindTesseract(source!));
            TargetText = await Interpret.WithGoogleAsync(SourceText, Language.FindAbrr(target!));
        }

        private Timer? _timer;

        private async Task ConfirmAsync()
        {
            var menuItem = SourceCollection.Recursion(x => (string) x.Parent?.Header! == "自动翻译" && x.IsChecked);
            if (menuItem != null)
            {
                _timer ??= new Timer();

                _timer.Interval = menuItem.Interval!.Value;
                _timer.Elapsed += async (_, _) => { await TranslateAsync(); };

                _timer.Start();
            }

            await TranslateAsync();
        }

        private ICommand? _confirmCommand;

        public ICommand ConfirmCommand
        {
            get { return _confirmCommand ??= new RelayCommand(async _ => await ConfirmAsync()); }
        }

        public static int X { get; set; }
        public static int Y { get; set; }
        public static int Width { get; set; }
        public static int Height { get; set; }
    }
}