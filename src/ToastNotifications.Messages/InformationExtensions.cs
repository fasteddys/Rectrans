using ToastNotifications.Core;
using ToastNotifications.Messages.Information;

namespace ToastNotifications.Messages
{
    public static class InformationExtensions
    {
        public static void ShowInformation(this Notifier notifier, string message)
        {
            notifier.Notify(() => new InformationMessage(message));
        }

        public static void ShowInformation(this Notifier notifier, string message, MessageOptions displayOptions)
        {
            notifier.Notify(() => new InformationMessage(message, displayOptions));
        }
    }
}
