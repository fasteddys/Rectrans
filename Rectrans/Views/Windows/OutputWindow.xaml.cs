using Prism.Ioc;
using Rectrans.ViewModels.Windows;

namespace Rectrans.Views.Windows
{
    /// <summary>
    /// OutputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OutputWindow
    {
        public OutputWindow(IContainerProvider containerProvider)
        {
            InitializeComponent();
            DataContext = containerProvider.Resolve<OutputViewModel>((typeof(OutputWindow), this));
        }
    }
}
