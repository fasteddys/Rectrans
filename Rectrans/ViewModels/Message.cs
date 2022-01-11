using System.Windows.Input;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Rectrans.ViewModels
{
    internal class Message
    {
        public MessageType MessageType { get; set; }

        public string? BorderText { get; set; }

        public bool IsShowCloseButton { get; set; }

        public Hyperlink? Hyperlink { get; set; }

        public DelayAction? DelayAction { get; set; }
    }

    internal class Hyperlink
    {
        public string Text { get; set; } = null!;

        public ICommand Command { get; set; } = null!;
    }

    internal class DelayAction
    {
        public int MillisecondsDelay { get; set; }

        public Action Action { get; set; } = null!;
    }

    internal enum MessageType
    {
        Message,
        Warning,
        Error,
        Close
    }
}
