using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using System.IO;
using Rectrans.OCR;
using Rectrans.Infrastructure;
using System.Text.RegularExpressions;

namespace Rectrans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int count = 0;
        private static Language sourceLan = Infrastructure.Language.English;
        private static Language targetLan = Infrastructure.Language.ChineseSimplified;
        private static Timer? timer = null;
        private double x, y, width, height;
        private static RectangleWindow? rectWindow = null;
        private static Regex spaceReg = new(@"(^\s*)|(\s*$)", RegexOptions.Compiled);

        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
        }

        private void BtnsEnable()
        {
            Trans_Btn.IsEnabled = true;
            RectReset_Btn.IsEnabled = true;
        }

        private void BtnsDisable()
        {
            Trans_Btn.IsEnabled = false;
            RectReset_Btn.IsEnabled = false;
        }

        private void TopMost_Click(object sender, RoutedEventArgs e) => Topmost = !Topmost;

        private void EN_Click(object sender, RoutedEventArgs e)
        {
            sourceLan = Infrastructure.Language.English;
            SourceLan_MI.Header = $"源语言({Infrastructure.Language.English.Name()})";
            UnClickedChildren(SourceLan_MI.Items);
            var item = e.Source as MenuItem;
            item!.IsChecked = true;
        }
        private void JA_Click(object sender, RoutedEventArgs e)
        {
            sourceLan = Infrastructure.Language.Japanese;
            SourceLan_MI.Header = $"源语言({Infrastructure.Language.Japanese.Name()})";
            UnClickedChildren(SourceLan_MI.Items);
            var item = e.Source as MenuItem;
            item!.IsChecked = true;
        }
        private void KO_Click(object sender, RoutedEventArgs e)
        {
            sourceLan = Infrastructure.Language.Korean;
            SourceLan_MI.Header = $"源语言({Infrastructure.Language.Korean.Name()})";
            UnClickedChildren(SourceLan_MI.Items);
            var item = e.Source as MenuItem;
            item!.IsChecked = true;
        }

        private void AutoTrans_MI_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is MenuItem mi)
            {
                if (timer is null)
                {
                    timer = new();
                    MessageBox.Show("自动翻译可能会加速翻译API限额的消耗，请谨慎使用。");
                }

                switch (mi.Header)
                {
                    case "2s":
                        timer.Interval = 2000;
                        AutoTrans_MI.Header = "自动翻译(2s)";
                        UnClickedChildren(AutoTrans_MI.Items);
                        AutoTrans_MI.IsChecked = true;
                        mi.IsChecked = true;
                        break;
                    case "4s":
                        timer.Interval = 4000;
                        AutoTrans_MI.Header = "自动翻译(4s)";
                        UnClickedChildren(AutoTrans_MI.Items);
                        AutoTrans_MI.IsChecked = true;
                        mi.IsChecked = true;
                        break;
                    case "6s":
                        timer.Interval = 6000;
                        UnClickedChildren(AutoTrans_MI.Items);
                        AutoTrans_MI.IsChecked = true;
                        mi.IsChecked = true;
                        break;
                    case "8s":
                        timer.Interval = 8000;
                        AutoTrans_MI.Header = "自动翻译(8s)";
                        UnClickedChildren(AutoTrans_MI.Items);
                        AutoTrans_MI.IsChecked = true;
                        mi.IsChecked = true;
                        break;
                    case "10s":
                        timer.Interval = 10000;
                        AutoTrans_MI.Header = "自动翻译(10s)";
                        UnClickedChildren(AutoTrans_MI.Items);
                        AutoTrans_MI.IsChecked = true;
                        mi.IsChecked = true;
                        break;
                    case "停止":
                        timer.Dispose();
                        timer = null;
                        AutoTrans_MI.Header = "自动翻译";
                        AutoTrans_MI.IsChecked = false;
                        UnClickedChildren(AutoTrans_MI.Items);
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }
        }

        private void Rect_Click(object sender, RoutedEventArgs e)
        {
            if (rectWindow is not null) return;

            rectWindow = new RectangleWindow();

            rectWindow.Show();
            BtnsEnable();

            rectWindow.LayoutUpdated += delegate { RectChanged(); };
            rectWindow.Closed += delegate
            {
                rectWindow = null;
                BtnsDisable();
            };
        }

        private void RectReset_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(rectWindow is not null);
#pragma warning disable CS8602 // 解引用可能出现空引用。
            rectWindow.Topmost = true;
            rectWindow.WindowState = WindowState.Normal;
            rectWindow.Left = Left;
            rectWindow.Top = Top;
            rectWindow.Width = 500;
            rectWindow.Height = 200;

        }

        private async void Trans_Click(object sender, RoutedEventArgs e)
        {
            BtnsDisable();

            await Task.Run(async () => await InterpretAsync());

            if (timer is not null)
            {
                await Task.Run(() => AutoInterpret());
            }

            BtnsEnable();
        }

        private void AutoInterpret()
        {
            Debug.Assert(timer is not null);
            timer.Elapsed += new ElapsedEventHandler(async (object? sender, ElapsedEventArgs e) =>
            {
                await InterpretAsync();

            });

            timer.Start();
        }

        private async Task InterpretAsync()
        {
            var bytes = ScreenShot(x, y, width, height);

            var original = toI(bytes.Parse(sourceLan));

#if NET5_0_OR_GREATER
            var translated = await Interpreter.Interpret.WithGoogleAsync(original, targetLan);
#else
            var translated = await Interpreter.Interpret.WithGoogleAsync(original, targetLan, null);
#endif

            _ = Dispatcher.BeginInvoke(() =>
            {
                OriginalTextBlock.Text = original;
                TranslatedTextBlock.Text = translated;
                StatusBar.Text = $"字数统计(字节): {count += original.Length}";
            });
        }

        private static byte[] ScreenShot(double x, double y, double width, double height)
        {
            var ix = Convert.ToInt32(x);
            var iy = Convert.ToInt32(y);
            var iw = Convert.ToInt32(width);
            var ih = Convert.ToInt32(height);

            var bitmap = new Bitmap(iw, ih);
            using var graphics = Graphics.FromImage(bitmap);
            using var stream = new MemoryStream();
            graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih));
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }

        private void RectChanged()
        {
            if (rectWindow is null) return;

            x = rectWindow.Left;
            y = rectWindow.Top;
            width = rectWindow.Width;
            height = rectWindow.Height;
        }

        private static string toI(string text) => spaceReg.Replace(text, "").Replace("|", "I");
        private static void UnClickedChildren(ItemCollection items)
        {
            foreach (var item in items)
            {
                if (item is MenuItem mi)
                {
                    mi.IsChecked = false;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            rectWindow?.Close();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (rectWindow is null) return;
            if (WindowState is not WindowState.Maximized)
                rectWindow.WindowState = WindowState;
        }
    }
}
