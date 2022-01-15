using Prism.Ioc;
using System.Windows;
using Rectrans.Views;
using ToastNotifications;
using Rectrans.ViewModels;
using Rectrans.Extensions;
using Rectrans.Services.Implement;
using ToastNotifications.Messages;

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

            DataContext = new MainViewModel();
            Notifier = this.Notifier();
            WindowManager.Default.Register(this);

            InputWindow = containerProvider.Resolve<InputWindow>();
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
            Notifier.Dispose();
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