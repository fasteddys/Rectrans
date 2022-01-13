using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Rectrans.Themes.Controls;

public class MessageBorder : ContentControl
{
    public MessageBorder()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageBorder),
            new FrameworkPropertyMetadata(typeof(MessageBorder)));
    }

    #region Visibity

    public static readonly DependencyProperty ShowMessageProperty
        = DependencyProperty.Register(nameof(ShowMessage),
            typeof(bool),
            typeof(MessageBorder));

    public bool ShowMessage
    {
        get => (bool) GetValue(ShowMessageProperty);
        set => SetValue(ShowMessageProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ShowHyperlinkProperty
        = DependencyProperty.Register(nameof(ShowHyperlink),
            typeof(bool),
            typeof(MessageBorder));

    public bool ShowHyperlink
    {
        get => (bool) GetValue(ShowHyperlinkProperty);
        set => SetValue(ShowHyperlinkProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty ShowCloseButtonProperty
        = DependencyProperty.Register(nameof(ShowCloseButton),
            typeof(bool),
            typeof(MessageBorder));

    public bool ShowCloseButton
    {
        get => (bool) GetValue(ShowCloseButtonProperty);
        set => SetValue(ShowCloseButtonProperty, BooleanBoxes.Box(value));
    }

    #endregion

    #region Texts

    public static readonly DependencyProperty MessageTextProperty
        = DependencyProperty.Register(nameof(MessageText),
            typeof(string),
            typeof(MessageBorder));

    public string MessageText
    {
        get => (string) GetValue(MessageTextProperty);
        set => SetValue(MessageTextProperty, value);
    }

    public static readonly DependencyProperty HyperlinkTextProperty
        = DependencyProperty.Register(nameof(HyperlinkText),
            typeof(string),
            typeof(MessageBorder));

    public string HyperlinkText
    {
        get => (string) GetValue(HyperlinkTextProperty);
        set => SetValue(HyperlinkTextProperty, value);
    }

    #endregion

    #region Colors

    public static readonly DependencyProperty MessageTextColorProperty
        = DependencyProperty.Register(nameof(MessageTextColor),
            typeof(Brush),
            typeof(MessageBorder),
            new PropertyMetadata(Brushes.Black));

    public Brush MessageTextColor
    {
        get => (Brush) GetValue(MessageTextColorProperty);
        set => SetValue(MessageTextColorProperty, value);
    }

    public static readonly DependencyProperty HyperlinkColorProperty
        = DependencyProperty.Register(nameof(HyperlinkColor),
            typeof(Brush),
            typeof(MessageBorder),
            new PropertyMetadata(Brushes.Black));

    public Brush HyperlinkColor
    {
        get => (Brush) GetValue(HyperlinkColorProperty);
        set => SetValue(HyperlinkColorProperty, value);
    }

    public static readonly DependencyProperty CloseButtonColorProperty
        = DependencyProperty.Register(nameof(CloseButtonColor),
            typeof(Brush),
            typeof(MessageBorder),
            new PropertyMetadata(Brushes.Black));

    public Brush CloseButtonColor
    {
        get => (SolidColorBrush) GetValue(CloseButtonColorProperty);
        set => SetValue(CloseButtonColorProperty, value);
    }

    #endregion

    #region Commands

    public static readonly DependencyProperty HyperlinkCommandProperty
        = DependencyProperty.Register(nameof(HyperlinkCommand),
            typeof(ICommand),
            typeof(MessageBorder));

    public ICommand HyperlinkCommand
    {
        get => (ICommand) GetValue(HyperlinkCommandProperty);
        set => SetValue(HyperlinkCommandProperty, value);
    }

    public static readonly DependencyProperty CloseButtonCommandProperty
        = DependencyProperty.Register(nameof(CloseButtonCommand),
            typeof(ICommand),
            typeof(MessageBorder));

    public ICommand CloseButtonCommand
    {
        get => (ICommand) GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }

    #endregion
}