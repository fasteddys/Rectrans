using System.Windows;

namespace Rectrans.View
{
    /// <summary>
    /// RectangleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RectangleView
    {
        public RectangleView()
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
        }
    }
}
