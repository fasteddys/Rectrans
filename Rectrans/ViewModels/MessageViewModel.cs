using Prism.Mvvm;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;

namespace Rectrans.ViewModels;

public abstract class MessageViewModel : BindableBase
{
    private FontWeight? _messageFontWeight;

    public FontWeight? MessageFontWeight
    {
        get => _messageFontWeight;
        set
        {
            _messageFontWeight = value;
            RaisePropertyChanged();
        }
    }

    #region Visibities

    private bool _showMessage;

    public bool ShowMessage
    {
        get => _showMessage;
        set
        {
            _showMessage = value;
            RaisePropertyChanged();
        }
    }

    private bool _showMessageHyperlink;

    public bool ShowMessageHyperlink
    {
        get => _showMessageHyperlink;
        set
        {
            _showMessageHyperlink = value;
            RaisePropertyChanged();
        }
    }

    private bool _showMessageCloseButton;

    public bool ShowMessageCloseButton
    {
        get => _showMessageCloseButton;
        set
        {
            _showMessageCloseButton = value;
            RaisePropertyChanged();
        }
    }

    #endregion

    #region Texts

    private string? _messageText;

    public string? MessageText
    {
        get => _messageText;
        set
        {
            _messageText = value;
            RaisePropertyChanged();
        }
    }

    private string? _messageHyperlinkText;

    public string? MessageHyperlinkText
    {
        get => _messageHyperlinkText;
        set
        {
            _messageHyperlinkText = value;
            RaisePropertyChanged();
        }
    }

    #endregion

    #region Colors

    private Brush? _messageBackground;

    public Brush? MessageBackground
    {
        get => _messageBackground;
        set
        {
            _messageBackground = value;
            RaisePropertyChanged();
        }
    }

    private Brush? _messageHyperlinkColor;

    public Brush? MessageHyperlinkColor
    {
        get => _messageHyperlinkColor;
        set
        {
            _messageHyperlinkColor = value;
            RaisePropertyChanged();
        }
    }

    private Brush? _messageCloseButtonColor;

    public Brush? MessageCloseButtonColor
    {
        get => _messageCloseButtonColor;
        set
        {
            _messageCloseButtonColor = value;
            RaisePropertyChanged();
        }
    }

    #endregion

    #region Commands

    public virtual ICommand? MessageHyperlinkCommand =>
        MessageHyperlinkText == null ? null : throw new NotImplementedException();


    private ICommand? _messageCloseButtonCommand;

    public virtual ICommand MessageCloseButtonCommand => _messageCloseButtonCommand ??= new DelegateCommand(() =>
    {
        ShowMessage = false;
    });

    #endregion
}