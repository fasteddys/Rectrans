using Prism.Ioc;
using Rectrans.ViewModels;

namespace Rectrans.Views;

public partial class OriginalView
{
    public OriginalView(IContainerProvider containerProvider)
    {
        InitializeComponent();
        DataContext = containerProvider.Resolve<OriginalViewModel>((typeof(OriginalView), this));
    }
}