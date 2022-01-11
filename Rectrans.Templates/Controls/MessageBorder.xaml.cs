using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rectrans.Templates.ValueBoxes;

namespace Rectrans.Templates.Controls;

public partial class MessageBorder : UserControl
{
    public MessageBorder()
    {
        InitializeComponent();
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageBorder),
            new FrameworkPropertyMetadata(typeof(MessageBorder)));
    }

    public static readonly DependencyProperty MessageBorderTextProperty
        = DependencyProperty.Register(nameof(MessageBorderText),
            typeof(string),
            typeof(MessageBorder),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnMessageBorderTextPropertyChangedCallback),
                new CoerceValueCallback(CoerceText)));

    private static void OnMessageBorderTextPropertyChangedCallback(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var border = (MessageBorder) d;
        if (e.OldValue != e.NewValue)
        {
            border.SetValue(MessageBorderTextProperty, e.NewValue);
        }
    }

    private static object CoerceText(DependencyObject d, object value)
    {
        return value;
    }

    [Bindable(true)]
    public string MessageBorderText
    {
        get => (string) GetValue(MessageBorderTextProperty);
        set => SetValue(MessageBorderTextProperty, value);
    }

    public static readonly DependencyProperty ShowMessageBorderProperty
        = DependencyProperty.Register(nameof(ShowMessageBorder),
            typeof(bool),
            typeof(MessageBorder));

    private static void OnShowMessageBorderPropertyChangedCallback(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var border = (MessageBorder) d;
        if (e.OldValue != e.NewValue)
        {
            border.Visibility = (bool) e.NewValue switch
            {
                true => Visibility.Visible,
                false => Visibility.Hidden
            };
        }
    }

    [Bindable(true)]
    public bool ShowMessageBorder
    {
        get => (bool) GetValue(ShowMessageBorderProperty);
        set => SetValue(ShowMessageBorderProperty, BooleanBoxes.Box(value));
    }


    public static readonly DependencyProperty HyperlinkTextProperty
        = DependencyProperty.Register(nameof(HyperlinkText),
            typeof(string),
            typeof(MessageBorder),
            new PropertyMetadata(default(string), OnHyperlinkTextPropertyChangedCallback));

    private static void OnHyperlinkTextPropertyChangedCallback(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var border = (MessageBorder) d;
        if (e.OldValue != e.NewValue)
        {
            border.HyperlinkText = (string) e.NewValue;
        }
    }

    [Bindable(true)]
    public string HyperlinkText
    {
        get => (string) GetValue(HyperlinkTextProperty);
        set => SetValue(HyperlinkTextProperty, value);
    }


    public static readonly DependencyProperty ShowHyperlinkProperty
        = DependencyProperty.Register(nameof(ShowHyperlink),
            typeof(bool),
            typeof(MessageBorder));

    private static void OnShowHyperlinkPropertyChangedCallback(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var border = (MessageBorder) d;
        if (e.OldValue != e.NewValue)
        {
            border.ShowHyperlink = (bool) e.NewValue;
        }
    }

    [Bindable(true)]
    public bool ShowHyperlink
    {
        get => (bool) GetValue(ShowHyperlinkProperty);
        set => SetValue(ShowHyperlinkProperty, BooleanBoxes.Box(value));
    }


    public static readonly DependencyProperty HyperlinkCommandProperty
        = DependencyProperty.Register(nameof(HyperlinkCommand),
            typeof(ICommand),
            typeof(MessageBorder));

    private static void OnHyperlinkCommandPropertyChangedCallback(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var border = (MessageBorder) d;
        if (e.OldValue != e.NewValue)
        {
            border.HyperlinkCommand = (ICommand) e.NewValue;
        }
    }

    [Bindable(true)]
    public ICommand HyperlinkCommand
    {
        get => (ICommand) GetValue(HyperlinkCommandProperty);
        set => SetValue(HyperlinkCommandProperty, value);
    }


    public static readonly DependencyProperty ShowCloseButtonProperty
        = DependencyProperty.Register(nameof(ShowCloseButton),
            typeof(bool),
            typeof(MessageBorder));

    private static void OnShowCloseButtonPropertyChangedCallback(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var border = (MessageBorder) d;
        if (e.OldValue != e.NewValue)
        {
            border.ShowCloseButton = (bool) e.NewValue;
        }
    }

    [Bindable(true)]
    public bool ShowCloseButton
    {
        get => (bool) GetValue(ShowCloseButtonProperty);
        set => SetValue(ShowCloseButtonProperty, BooleanBoxes.Box(value));
    }


    public static readonly DependencyProperty CloseButtonCommandProperty
        = DependencyProperty.Register(nameof(CloseButtonCommand),
            typeof(ICommand),
            typeof(MessageBorder));

    private static void OnCloseButtonCommandPropertyChangedCallback(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var border = (MessageBorder) d;
        if (e.OldValue != e.NewValue)
        {
            border.CloseButtonCommand = (ICommand) e.NewValue;
        }
    }

    [Bindable(true)]
    public ICommand CloseButtonCommand
    {
        get => (ICommand) GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }
}