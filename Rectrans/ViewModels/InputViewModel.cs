using Prism.Mvvm;

namespace Rectrans.ViewModels;

public class InputViewModel : MessageViewModel
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

}