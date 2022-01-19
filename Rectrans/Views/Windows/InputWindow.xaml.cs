using Prism.Events;
using Prism.Ioc;
using Rectrans.Events;
using Rectrans.Extensions;
using Rectrans.ViewModels.Windows;
using ToastNotifications;
using ToastNotifications.Position;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Rectrans.Views.Windows;

public partial class InputWindow
{
    #region Private Members

    /// <summary>
    /// The ioc event aggregator
    /// </summary>
    private readonly IEventAggregator aggregator;

    #endregion

    #region Public Properties

    /// <summary>
    /// The message notifier
    /// </summary>
    public Notifier Notifier { get; }

    #endregion

    #region Construction

    public InputWindow(IContainerProvider containerProvider, IEventAggregator aggregator)
    {
        this.aggregator = aggregator;
        InitializeComponent();
        MouseLeftButtonDown += (_, _) => DragMove();

        // Set data context and message notifier
        DataContext = containerProvider.Resolve<InputViewModel>((typeof(InputWindow), this));
        Notifier = this.Notifier(Corner.BottomCenter, 0, 50);
    }

    #endregion

    #region Override Methods

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Publish window closed event
        aggregator.GetEvent<WindowClosedEvent>().Publish();

        // dispose notifier
        Notifier.Dispose();
    }

    #endregion
}