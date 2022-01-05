using System.Windows;
using Rectrans.EventHandlers;
using Rectrans.View;
using Rectrans.ViewModel;

namespace Rectrans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // ReSharper disable once InconsistentNaming
        private readonly MainViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
            var rectangleView = new RectangleView();
            ViewModel = new MainViewModel();

            OnRectangleViewCreated(rectangleView);
            ViewModel.RectangleViewCreated += delegate(object? _, RectangleViewCreatedEventArgs args)
            {
                OnRectangleViewCreated(args.RectangleView);
            };
            DataContext = ViewModel;

            rectangleView.Show();
        }

        private void OnRectangleViewCreated(RectangleView rectangleView)
        {
            Closed += delegate { rectangleView.Close(); };
            StateChanged += delegate
            {
                if (WindowState != WindowState.Maximized)
                {
                    rectangleView.WindowState = WindowState;
                }
            };

            rectangleView.Closed += delegate
            {
                if (IsLoaded)
                {
                    ViewModel.OnRectangleViewAbnormalClosed();
                }
            };

            rectangleView.LayoutUpdated += delegate
            {
                ViewModel.X = (int) rectangleView.Left;
                ViewModel.Y = (int) rectangleView.Top;
                ViewModel.Height = (int) rectangleView.Height;
                ViewModel.Width = (int) rectangleView.Width;
            };
        }
    }
}