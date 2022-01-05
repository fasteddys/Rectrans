using System.Windows.Input;

namespace Rectrans.Interface;

public interface IMessageBorder
{
    public string MessageBorderText { get; set; }

    public string MessageBorderVisibility { get; set; }

    public string MessageBorderBackground { get; set; }

    public string MessageBorderCloseButtonVisibility { get; set; }

    public string MessageBorderHyperlinkVisibility { get; set; }

    public string MessageBorderHyperlinkText { get; set; }

    ICommand MessageBorderHyperlinkCommand { get; set; }
    
    ICommand MessageBorderCloseButtonCommand { get; set; }
    
    void OnMessageBorderHyperlinkClick(object? parameter);
    
    void OnPropertyChanged(string propName);
}