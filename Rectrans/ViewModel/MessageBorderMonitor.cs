using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Rectrans.Common;
using Rectrans.Interface;

namespace Rectrans.ViewModel;

public class MessageBorderMonitor
{
    // ReSharper disable once InconsistentNaming
    private readonly IMessageBorder MessageBorder;

    public MessageBorderMonitor(IMessageBorder messageBorder)
    {
        MessageBorder = messageBorder;
        InitializeMessageBorder();
    }

    #region OnWarning

    public async void OnWarningWithTimeout(string messageBorderText, int millisecondsDelay, Action? callback = null)
    {
        OnWarning(messageBorderText);
        OnPropertyChanged();

        await Task.Delay(millisecondsDelay);
        callback?.Invoke();
        CloseMessageBorder();
    }

    public void OnWarningWithHyperlinkText(string messageBorderText, string hyperlinkText)
    {
        OnWarning(messageBorderText);
        MessageBorder.MessageBorderHyperlinkText = hyperlinkText;
        MessageBorder.MessageBorderHyperlinkVisibility = Visible;

        OnPropertyChanged();
    }

    public void OnWarningWithCloseButton(string messageBorderText)
    {
        OnWarning(messageBorderText);
        MessageBorder.MessageBorderCloseButtonVisibility = Visible;

        OnPropertyChanged();
    }

    #endregion

    #region OnMessage

    public async void OnMessageWithTimeout(string messageBorderText, int millisecondsDelay, Action? callback = null)
    {
        OnMessage(messageBorderText);
        OnPropertyChanged();

        await Task.Delay(millisecondsDelay);
        callback?.Invoke();
        CloseMessageBorder();
    }

    public void OnMessageWithHyperlinkText(string messageBorderText, string hyperlinkText)
    {
        OnMessage(messageBorderText);
        MessageBorder.MessageBorderHyperlinkText = hyperlinkText;
        MessageBorder.MessageBorderHyperlinkVisibility = Visible;

        OnPropertyChanged();
    }

    public void OnMessageWithCloseButton(string messageBorderText)
    {
        OnMessage(messageBorderText);
        MessageBorder.MessageBorderCloseButtonVisibility = Visible;

        OnPropertyChanged();
    }

    #endregion

    #region OnError

    public async void OnErrorWithTimeout(string messageBorderText, int millisecondsDelay, Action? callback = null)
    {
        OnError(messageBorderText);
        OnPropertyChanged();

        await Task.Delay(millisecondsDelay);
        callback?.Invoke();
        CloseMessageBorder();
    }

    public void OnErrorWithHyperlinkText(string messageBorderText, string hyperlinkText)
    {
        OnError(messageBorderText);
        MessageBorder.MessageBorderHyperlinkText = hyperlinkText;
        MessageBorder.MessageBorderHyperlinkVisibility = Visible;

        OnPropertyChanged();
    }

    public void OnErrorWithCloseButton(string messageBorderText)
    {
        OnError(messageBorderText);
        MessageBorder.MessageBorderCloseButtonVisibility = Visible;

        OnPropertyChanged();
    }

    #endregion

    #region PrivateMethods

    private void OnWarning(string messageBorderText)
    {
        var background = AppSettings.GetMessageBorderBackground("Warning") ??
                         throw AppSettings.KeyNotFoundException("Warning");

        MessageBorder.MessageBorderVisibility = Visible;
        MessageBorder.MessageBorderBackground = background;
        MessageBorder.MessageBorderText = messageBorderText;
    }

    private void OnMessage(string messageBorderText)
    {
        var background = AppSettings.GetMessageBorderBackground("Message") ??
                         throw AppSettings.KeyNotFoundException("Message");

        MessageBorder.MessageBorderVisibility = Visible;
        MessageBorder.MessageBorderBackground = background;
        MessageBorder.MessageBorderText = messageBorderText;
    }

    private void OnError(string messageBorderText)
    {
        var background = AppSettings.GetMessageBorderBackground("Error") ??
                         throw AppSettings.KeyNotFoundException("Error");

        MessageBorder.MessageBorderVisibility = Visible;
        MessageBorder.MessageBorderBackground = background;
        MessageBorder.MessageBorderText = messageBorderText;
    }

    #endregion

    private ICommand? _messageBorderHyperlinkCommand;

    private ICommand? _messageBorderCloseButtonCommand;

    private void MessageBorderCloseButtonClick(object? parameter) => CloseMessageBorder();

    public void CloseMessageBorder()
    {
        DefaultValue();
        OnPropertyChanged();
    }

    private void InitializeMessageBorder()
    {
        MessageBorder.MessageBorderHyperlinkCommand = _messageBorderHyperlinkCommand ??=
            new RelayCommand(MessageBorder.OnMessageBorderHyperlinkClick);
        MessageBorder.MessageBorderCloseButtonCommand = _messageBorderCloseButtonCommand ??=
            new RelayCommand(MessageBorderCloseButtonClick);
        DefaultValue();
    }

    private void DefaultValue()
    {
        MessageBorder.MessageBorderVisibility = MessageBorder.MessageBorderHyperlinkVisibility =
            MessageBorder.MessageBorderCloseButtonVisibility = Hidden;
        MessageBorder.MessageBorderText = MessageBorder.MessageBorderHyperlinkText = Empty;
    }

    private void OnPropertyChanged()
    {
        MessageBorder.OnPropertyChanged("MessageBorderBackground");
        MessageBorder.OnPropertyChanged("MessageBorderVisibility");
        MessageBorder.OnPropertyChanged("MessageBorderText");
        MessageBorder.OnPropertyChanged("MessageBorderHyperlinkText");
        MessageBorder.OnPropertyChanged("MessageBorderHyperlinkVisibility");
        MessageBorder.OnPropertyChanged("MessageBorderCloseButtonVisibility");
    }

    private const string Hidden = "Hidden";
    private const string Visible = "Visible";
    private const string Empty = "";
}