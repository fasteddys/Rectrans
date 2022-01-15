using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace Rectrans.Extensions
{
    internal static class WindowExtension
    {
        public static bool IsClosed(this Window window)
        {
            return PresentationSource.FromVisual(window) == null;
        }

        public static Notifier Notifier(this Window window,
            Corner? corner = null, double? offsetX = null, double? offsetY = null)
        {
            return new(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: window,
                    corner: corner ?? Corner.TopRight,
                    offsetX: offsetX ?? 10,
                    offsetY: offsetY ?? 40);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = window.Dispatcher;
            });
        }
    }
}
