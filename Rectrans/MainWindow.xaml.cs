using Prism.Ioc;
using Prism.Events;
using Rectrans.Events;
using ToastNotifications;
using Rectrans.Extensions;
using Rectrans.Views.Windows;
using ToastNotifications.Core;
using ToastNotifications.Messages;
using Rectrans.ViewModels.Windows;

// ReSharper disable InconsistentNaming

// ReSharper disable MemberCanBePrivate.Global

namespace Rectrans;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    #region Private Members

    /// <summary>
    /// The input window
    /// </summary>
    private readonly InputWindow inputWindow;

    /// <summary>
    /// The output window
    /// </summary>
    private readonly OutputWindow outputWindow;

    #endregion

    #region Public Properties

    /// <summary>
    /// The message notifier
    /// </summary>
    public Notifier Notifier { get; }

    #endregion

    #region Construction

    public MainWindow(IContainerProvider containerProvider, IEventAggregator aggregator)
    {
        InitializeComponent();

        DataContext = containerProvider.Resolve<MainViewModel>((typeof(MainWindow), this));
        Notifier = this.Notifier();

        inputWindow = containerProvider.Resolve<InputWindow>();
        outputWindow = containerProvider.Resolve<OutputWindow>();

        outputWindow.Show();
        inputWindow.Show();

        // must show the window first
        inputWindow.Notifier.ShowInformation("提示: 将此窗口拖动至需要翻译的区域!");

        // Subscribe window closed event
        aggregator.GetEvent<WindowClosedEvent>().Subscribe(() =>
        {
            // input window closed, but main window still in visual.
            if (!this.IsClosed())
            {
                Notifier.ShowError("您已关闭“翻译框”窗口，请重启程序进行恢复！", new MessageOptions
                {
                    CloseClickAction = _ => Close()
                });
            }
        });
    }

    #endregion

    #region Override Methods

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if (!inputWindow.IsClosed())
        {
            inputWindow.Close();
        }
        
        if (!outputWindow.IsClosed())
        {
            outputWindow.Close();
        }

        Notifier.Dispose();
    }

    #endregion
}