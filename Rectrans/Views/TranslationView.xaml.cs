using Prism.Ioc;
using Rectrans.ViewModels;

namespace Rectrans.Views;

public partial class TranslationView
{
    public TranslationView(IContainerProvider containerProvider)
    {
        InitializeComponent();
        DataContext = containerProvider.Resolve<TranslationViewModel>((typeof(TranslationView), this));
    }
}