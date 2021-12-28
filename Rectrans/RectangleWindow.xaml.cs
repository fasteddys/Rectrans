using System.Windows;

namespace Rectrans
{
    /// <summary>
    /// RectangleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RectangleWindow : Window
    {
        public RectangleWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
        }
    }
}
