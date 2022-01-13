using System.Windows;
using System.Windows.Media;
using Prism.Ioc;
using Rectrans.ViewModels;
using Rectrans.Views;

namespace Rectrans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(IContainerProvider containerProvider)
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };

            InputWindow = containerProvider.Resolve<InputWindow>();
            RegisterInputWindowAbnormalClosed();

            DataContext = new MainViewModel();
            InputWindow.Show();
        }

        // ReSharper disable once InconsistentNaming
        private readonly InputWindow InputWindow;

        private void RegisterInputWindowAbnormalClosed()
        {
            InputWindow.Closed += (_, _) =>
            {
                if (IsLoaded)
                {
                    // Messenger.Default.Send<Message>(new()
                    // {
                    //     MessageType = MessageType.Warning,
                    //     BorderText = @"您已关闭“翻译框”窗口，请点击""重置""按钮进行恢复！",
                    //     Hyperlink = new()
                    //     {
                    //         Text = "重置",
                    //         Command = new RelayCommand(OnMessageBorderHyperlinkClick)
                    //     }
                    // });

                    var viewModel = (MainViewModel) DataContext;

                    viewModel.MessageBackground = AppSettings.GetMessageBorderBackground("Warning");
                    viewModel.MessageText = @"您已关闭“翻译框”窗口，请点击""重置""按钮进行恢复！";
                    // viewModel.
                    
                }
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            InputWindow.Close();
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