using Rectrans.Utilities;
using Rectrans.Mvvm.Common;
using System.Windows.Input;
using System.Windows.Media;
using Rectrans.Mvvm.Messaging;

namespace Rectrans.ViewModels;

public class MessageBorderViewModel : ViewModelBase
{
    public MessageBorderViewModel()
    {
        Messenger.Default.Register<Message>(this, Execute);
    }

    private async void Execute(Message message)
    {
        if(message.MessageType == MessageType.Close)
        {
            CloseMessageBorder();
            return;
        }

        var type = message.MessageType.ToString();
        var background = AppSettings.GetMessageBorderBackground(type)
            ?? throw AppSettings.KeyNotFoundException(type);

        BorderBackground = (Brush)new BrushConverter()
            .ConvertFromString(background)!;

        BorderText = message.BorderText!;

        if (message.Hyperlink != null)
        {
            HyperlinkText = message.Hyperlink.Text;
            HyperlinkCommand = message.Hyperlink.Command;
        }

        ShowCloseButton = message.IsShowCloseButton;

        var delay = message.DelayAction;
        if (delay != null)
        {
            await Task.Delay(delay.MillisecondsDelay);
            delay.Action();
        }
    }

    private Brush? _borderBackground;

    public Brush BorderBackground
    {
        get => _borderBackground ??= Brushes.Transparent;
        set
        {
            _borderBackground = value;
            OnPropertyChanged();
        }
    }

    private string? _borderText;

    public string BorderText
    {
        get => _borderText ??= string.Empty;
        set
        {
            _borderText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowBorder));
        }
    }

    private bool _showCloseButton;

    public bool ShowCloseButton
    {
        get => _showCloseButton;
        set
        {
            _showCloseButton = value;
            OnPropertyChanged();
        }
    }

    private ICommand? _hyperlinkCommand;

    public ICommand HyperlinkCommand
    {
        get => _hyperlinkCommand ??= new RelayCommand();
        set
        {
            _hyperlinkCommand = value;
            OnPropertyChanged();
        }
    }

    private string? _hyperlinkText;

    public string HyperlinkText
    {
        get => _hyperlinkText ??= string.Empty;
        set
        {
            _hyperlinkText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowHyperlink));
        }
    }

    private ICommand? _closeButtonCommand;

    public ICommand CloseButtonCommand =>
        _closeButtonCommand ??= new RelayCommand(_ => CloseMessageBorder());

    public bool ShowBorder => !string.IsNullOrEmpty(BorderText);

    public bool ShowHyperlink => !string.IsNullOrEmpty(_hyperlinkText);

    private void CloseMessageBorder()
    {
        ShowCloseButton = false;
        BorderText = HyperlinkText = string.Empty;
    }
}