using System.Windows;
using Rectrans.Views;
using Rectrans.ViewModels;
using Rectrans.EventHandlers;

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
            var importWindow = new ImportWindow();
            ViewModel = new MainViewModel();

            OnImportWindowCreated(importWindow);
            ViewModel.ImportWindowCreated += delegate(object? _, ImportWindowCreatedEventArgs args)
            {
                OnImportWindowCreated(args.ImportWindow);
            };
            DataContext = ViewModel;

            importWindow.Show();
        }

        private void OnImportWindowCreated(ImportWindow importWindow)
        {
            Closed += delegate { importWindow.Close(); };
            StateChanged += delegate
            {
                if (WindowState != WindowState.Maximized)
                {
                    importWindow.WindowState = WindowState;
                }
            };

            importWindow.Closed += delegate
            {
                if (IsLoaded)
                {
                    ViewModel.OnImportWindowAbnormalClosed();
                }
            };

            importWindow.LayoutUpdated += delegate
            {
                ViewModel.X = (int) importWindow.Left;
                ViewModel.Y = (int) importWindow.Top;
                ViewModel.Height = (int) importWindow.Height;
                ViewModel.Width = (int) importWindow.Width;
            };
        }
    }
}