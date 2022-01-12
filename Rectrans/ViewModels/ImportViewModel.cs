using Rectrans.Mvvm.Common;

namespace Rectrans.ViewModels;

public class ImportViewModel : ViewModelBase
{
    public ImportViewModel()
    {
        MessageBorder();
    }

    private void MessageBorder()
    {
        // MessageBorderMonitor.OnWarningWithTimeout(@"提示: 将此窗口拖动至需要翻译的区域! 本提示将在 8 秒后自动消失。", 8000, () => Opacity = 0.2);
    }

    private double? _opacity;

    public double Opacity
    {
        get => _opacity ??= 1;
        set
        {
            _opacity = value;
            OnPropertyChanged(nameof(Opacity));
        }
    }
}