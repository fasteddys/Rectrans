using Prism.Ioc;
using Rectrans.ViewModels;

namespace Rectrans.Views;

public partial class SettingView
{
    public SettingView(IContainerProvider containerProvider)
    {
        InitializeComponent();
        DataContext = containerProvider.Resolve<SettingViewModel>((typeof(SettingView), this));
    }
}