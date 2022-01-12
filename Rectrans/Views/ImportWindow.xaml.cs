namespace Rectrans.Views
{
    /// <summary>
    /// RectangleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImportWindow
    {
        public ImportWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
        }
    }
}
