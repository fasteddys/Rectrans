using Rectrans.ViewModels;
using System.Windows;

namespace Rectrans.Views
{
    /// <summary>
    /// OutputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OutputWindow : Window
    {
        public OutputWindow()
        {
            InitializeComponent();
            DataContext = new OutputViewModel(this);
        }
    }
}
