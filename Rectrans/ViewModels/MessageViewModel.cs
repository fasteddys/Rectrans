using Prism.Mvvm;
using System.Windows;
using Prism.Commands;
using System.Windows.Input;
using System.Windows.Media;

namespace Rectrans.ViewModels;

public abstract class MessageViewModel : BindableBase
{
    private void Reset()
    {
        _messageFontWeight = null;

        _messageText = null;
        _messageHyperlinkText = null;

        _messageHyperlinkCommand = null;
        _messageCloseButtonCommand = null;

        _messageBackground = null;
        _messageHyperlinkColor = null;
        _messageCloseButtonColor = null;

        _showMessage = false;
        _showMessageCloseButton = false;
    }

    private FontWeight? _messageFontWeight;

    public FontWeight MessageFontWeight
    {
        get => _messageFontWeight ??= FontWeights.Normal;
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
            if (!value) Reset();

            _showMessage = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(ShowMessageHyperlink));
        }
    }

    public bool ShowMessageHyperlink => _messageHyperlinkText != null;

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

    public string MessageText
    {
        get => _messageText ??= string.Empty;
        set
        {
            _messageText = value;
            RaisePropertyChanged();
        }
    }

    private string? _messageHyperlinkText;

    public string? MessageHyperlinkText
    {
        get => _messageHyperlinkText ??= string.Empty;
        set
        {
            _messageHyperlinkText = value;
            RaisePropertyChanged();
        }
    }

    #endregion

    #region Colors

    private Brush? _messageBackground;

    public Brush MessageBackground
    {
        get => _messageBackground ??= Brushes.Black;
        set
        {
            _messageBackground = value;
            RaisePropertyChanged();
        }
    }

    private Brush? _messageHyperlinkColor;

    public Brush MessageHyperlinkColor
    {
        get => _messageHyperlinkColor ??= Brushes.Black;
        set
        {
            _messageHyperlinkColor = value;
            RaisePropertyChanged();
        }
    }

    private Brush? _messageCloseButtonColor;

    public Brush MessageCloseButtonColor
    {
        get => _messageCloseButtonColor ??= Brushes.Black;
        set
        {
            _messageCloseButtonColor = value;
            RaisePropertyChanged();
        }
    }

    #endregion

    #region Commands

    private ICommand? _messageHyperlinkCommand;

    public virtual ICommand? MessageHyperlinkCommand
    {
        get => _messageHyperlinkCommand ??= new DelegateCommand(() => { });
        set
        {
            _messageHyperlinkCommand = value;
            RaisePropertyChanged();
        }
    }

    private ICommand? _messageCloseButtonCommand;

    public virtual ICommand MessageCloseButtonCommand
    {
        get => _messageCloseButtonCommand ??= new DelegateCommand(() => ShowMessage = false);
        set
        {
            _messageCloseButtonCommand = value;
            RaisePropertyChanged();
        }
    }

    #endregion
}