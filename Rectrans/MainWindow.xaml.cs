using Prism.Ioc;
using System.Windows;
using Rectrans.Models;
using Rectrans.Views;
using Rectrans.ViewModels;
using ToastNotifications.Messages;
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using System.Windows.Threading;

namespace Rectrans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Notifier MainNotifier;

        private Notifier? InputNotifier;

        public MainWindow(IContainerProvider containerProvider)
        {
            ContainerProvider = containerProvider;
            DataContext = new MainViewModel();

            InitializeComponent();
            InitializeInputWindow();

            MouseLeftButtonDown += delegate { DragMove(); };

            MainNotifier = InitializeNotifier(this);
        }

        // ReSharper disable once InconsistentNaming
        private InputWindow InputWindow = null!;

        // ReSharper disable once InconsistentNaming
        private readonly IContainerProvider ContainerProvider;

        private void InitializeInputWindow()
        {
            InputWindow = ContainerProvider.Resolve<InputWindow>();
            InputNotifier = InitializeNotifier(InputWindow);

            var viewModel = (MainViewModel)DataContext;

            // register close event when inputWindow abnormal closed.
            InputWindow.Closed += (_, _) =>
            {
                // Waring: when main close IsLoaded is true
                if (IsLoaded)
                {
                    MainNotifier.ShowError("您已关闭“翻译框”窗口，请重启程序进行恢复！", new ToastNotifications.Core.MessageOptions
                    {
                        CloseClickAction = _ =>
                        {
                            Close();
                        }
                    });
                }
                // when input window closed, dispose notifier.
                InputNotifier.Dispose();
            };

            // register event when inputWindow size changed.
            InputWindow.SizeChanged += (_, args) =>
            {
                viewModel.InputWindowSize = new WindowSize
                {
                    Height = args.NewSize.Height,
                    Width = args.NewSize.Width,
                };
            };

            // register event when inputWindow location changed.
            InputWindow.LocationChanged += (_, _) =>
            {
                viewModel.InputWindowLocation = new WindowLocation
                {
                    Top = InputWindow.Top,
                    Left = InputWindow.Left,
                };
            };

            InputWindow.Show();
            InputNotifier.ShowInformation("提示: 将此窗口拖动至需要翻译的区域!");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            InputWindow.Close();

            // when main window closed, dispose notifier.
            MainNotifier.Dispose();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState != WindowState.Maximized)
            {
                InputWindow.WindowState = WindowState;
            }
        }

        private Notifier InitializeNotifier(Window parentWindow)
        {
            return new(cfg =>
           {
               cfg.PositionProvider = new WindowPositionProvider(
                   parentWindow: parentWindow,
                   corner: Corner.TopRight,
                   offsetX: 10,
                   offsetY: 50);

               cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                   notificationLifetime: TimeSpan.FromSeconds(3),
                   maximumNotificationCount: MaximumNotificationCount.FromCount(5));

               cfg.Dispatcher = parentWindow.Dispatcher;
           });
        }
    }
}