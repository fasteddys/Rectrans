using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using ToastNotifications.Messages;

namespace Rectrans.ViewModels;

public class InputViewModel : BindableBase
{
    public InputViewModel()
    {
    }

    private double? _opacity;

    public double Opacity
    {
        get => _opacity ??= 1;
        set
        {
            _opacity = value;
            RaisePropertyChanged();
        }
    }

    public ICommand TranslateCommand => new DelegateCommand(() => { });

}