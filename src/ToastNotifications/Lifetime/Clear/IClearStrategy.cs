using ToastNotifications.Core;

namespace ToastNotifications.Lifetime.Clear
{
    public interface IClearStrategy
    {
        IEnumerable<INotification> GetNotificationsToRemove(NotificationsList notifications);
    }
}