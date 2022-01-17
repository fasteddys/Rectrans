using ToastNotifications;
using Rectrans.ViewModels;
using Rectrans.Extensions;
using ToastNotifications.Core;
using Rectrans.Services.Implement;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace Rectrans.Views;

public partial class InputWindow
{
    public Notifier Notifier { get; }

    public InputWindow()
    {
        InitializeComponent();
        DataContext = new InputViewModel(this);
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
            mainWindow.Notifier.ShowError("ÄúÒÑ¹Ø±Õ¡°·­Òë¿ò¡±´°¿Ú£¬ÇëÖØÆô³ÌÐò½øÐÐ»Ö¸´£¡", new MessageOptions
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