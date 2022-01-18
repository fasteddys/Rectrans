using Prism.Ioc;
using Rectrans.Extensions;
using Rectrans.Services.Implement;
using Rectrans.ViewModels;
using Rectrans.ViewModels.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Rectrans.Views.Windows;

public partial class InputWindow
{
    public Notifier Notifier { get; }

    public InputWindow(IContainerProvider containerProvider)
    {
        InitializeComponent();
        DataContext = containerProvider.Resolve<InputViewModel>((typeof(InputWindow), this));
        Notifier = this.Notifier(Corner.BottomCenter, 0, 50);

        WindowManager.Default.Register(this);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        var mainWindow = WindowManager.Default.Resolve<MainWindow>()!;

        // input window closed, but main window still in visual.
        if (!mainWindow.IsClosed())
        {
            mainWindow.Notifier.ShowError("您已关闭“翻译框”窗口，请重启程序进行恢复！", new MessageOptions
            {
                CloseClickAction = _ =>
                {
                    mainWindow.Close();
                }
            });
        }

        Notifier.Dispose();
    }
}