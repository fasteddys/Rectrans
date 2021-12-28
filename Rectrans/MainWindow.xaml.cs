using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;

namespace Rectrans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double x, y, width, height;

        private bool AutoTranslate = false;

        private RectangleWindow? rectWindow = null;

        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
        }
        private void Topmost_Button_Click(object sender, RoutedEventArgs e) => Topmost = !Topmost;

        private void UpdatePosition()
        {
            if (rectWindow is null)
            {
                rectWindow = new RectangleWindow();
            }

            x = rectWindow.Left;
            y = rectWindow.Top;
            width = rectWindow.Width;
            height = rectWindow.Height;
        }

        private void Rect_Button_Click(object sender, RoutedEventArgs e)
        {
            if (rectWindow is null)
            {
                rectWindow = new RectangleWindow();
            }

            rectWindow.LayoutUpdated += delegate { UpdatePosition(); };

            rectWindow.Show();
        }

        private async void Trans_Button_Click(object sender, RoutedEventArgs e)
        {
            if (rectWindow is null)
            {
                MessageBox.Show("请先设置翻译区域");
                return;
            }

            Trans_Button.IsEnabled = false;

            if (AutoTranslate)
            {
                await Task.Run(() => AutoWoker());
            }
            else
            {
                await Task.Run(async () => await WokerAsync());
            }

            Trans_Button.IsEnabled = true;
        }

        private void AutoWoker()
        {
            System.Timers.Timer timer = new();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(async (object? sender, System.Timers.ElapsedEventArgs e) =>
            {
                await WokerAsync();

            });

            timer.Interval = 1000;
            timer.Start();
        }

        private async Task WokerAsync()
        {
            var bitmap = CaptureScreen(x, y, width, height);
            var original = toI(OCR.English.FromImage(bitmap));

            var translated = await Interpreter.Interpret.WithGoogleAsync(original, Interpreter.Language.Chinese);

            _ = Dispatcher.BeginInvoke(() =>
            {
                OriginalTextBlock.Text = original;
                TranslatedTextBlock.Text = translated;
            });
        }

        private static Bitmap CaptureScreen(double x, double y, double width, double height)
        {
            var ix = Convert.ToInt32(x);
            var iy = Convert.ToInt32(y);
            var iw = Convert.ToInt32(width);
            var ih = Convert.ToInt32(height);

            var bitmap = new Bitmap(iw, ih);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih));

                //var dialog = new Microsoft.Win32.SaveFileDialog();
                //dialog.Filter = "Png Files|*.png";
                //if (dialog.ShowDialog() == true)
                //{
                //    bitmap.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                //}
            }

            return bitmap;
        }

        private static string toI(string text) => text.Replace("|", "I");

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (rectWindow is not null)
            {
                rectWindow.Close();
            }
        }
    }
}
