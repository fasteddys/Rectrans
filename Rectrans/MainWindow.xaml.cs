using System.Windows;
using Rectrans.View;

namespace Rectrans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var child = new RectangleView();
            child.Show();
            RegisterEvent();

            void RegisterEvent()
            {
                Closed += delegate { child.Close(); };
                MouseLeftButtonDown += delegate { DragMove(); };
                StateChanged += delegate
                {
                    if (WindowState != WindowState.Maximized)
                    {
                        child.WindowState = WindowState;
                    }
                };
            }
        }
    }
}
