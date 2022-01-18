using Prism.Ioc;
using System.Windows;
using ToastNotifications;
using Rectrans.ViewModels;
using Rectrans.Extensions;
using ToastNotifications.Messages;
using Rectrans.Services.Implement;
using Rectrans.ViewModels.Windows;
using Rectrans.Views.Windows;
using OutputWindow = Rectrans.Views.Windows.OutputWindow;

namespace Rectrans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public Notifier Notifier { get; }
        // ReSharper disable once InconsistentNaming
        private readonly InputWindow InputWindow = null!;

        public MainWindow(IContainerProvider containerProvider)
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };

            DataContext = containerProvider.Resolve<MainViewModel>((typeof(MainWindow), this));
            Notifier = this.Notifier();
            WindowManager.Default.Register(this);

            InputWindow = containerProvider.Resolve<InputWindow>();
            var outputWindow = containerProvider.Resolve<OutputWindow>();

            outputWindow.Show();
            InputWindow.Show();

            // must show the window first
            InputWindow.Notifier.ShowInformation("提示: 将此窗口拖动至需要翻译的区域!");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!InputWindow.IsClosed())
            {
                InputWindow.Close();
            }
            //Notifier.Dispose();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState != WindowState.Maximized)
            {
                InputWindow.WindowState = WindowState;
            }
        }
    }
}