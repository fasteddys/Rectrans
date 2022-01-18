using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Rectrans.Helpers;

namespace Rectrans.ViewModels.Windows
{
    public class ModernWindow: BindableBase
    {
        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window mWindow;

        /// <summary>
        /// The margin around the window to allow for a drop shadow.
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The radius of the edges of the window.
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position.
        /// </summary>
        private WindowDockPosition DockPosition = WindowDockPosition.Undocked;

        #endregion

        #region Public Properties

        /// <summary>
        /// The smallest width the window can go to.
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 400;

        /// <summary>
        /// The smallest height the window can go to.
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 400;

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless => mWindow.WindowState == WindowState.Maximized || DockPosition != WindowDockPosition.Undocked;

        /// <summary>
        /// The size of the resize border around the window.
        /// </summary>
        public int ResizeBorder { get; set; } = 6;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin.
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        /// <summary>
        /// The padding of the inner content of the main window.
        /// </summary>
        public Thickness InnerContentPadding { get { return new Thickness(ResizeBorder); } }

        /// <summary>
        /// The margin around the window to allow for a drop shadow.
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mOuterMarginSize;
            }
            set => mOuterMarginSize = value;
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow.
        /// </summary>
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// The radius of the edges of the window.
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mWindowRadius;
            }
            set => mWindowRadius = value;
        }

        /// <summary>
        /// The radius of the edges of the window.
        /// </summary>
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        /// <summary>
        /// The height of the title bar / caption of the window.
        /// </summary>
        public int TitleHeight { get; set; } = 42;

        /// <summary>
        /// The height of the title bar / caption of the window.
        /// </summary>
        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        #endregion

        #region Commands

        /// <summary>
        /// The command of minimize the window.
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command of maximize the window.
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command of close the window.
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command of show the system menu for the window.
        /// </summary>
        public ICommand IconCommand { get; set; }

        /// <summary>
        /// The command of set top most for the window.
        /// </summary>
        public ICommand TopMostCommand { get; set; }

        #endregion

        #region Construcotr

        public ModernWindow(Window window)
        {
            mWindow = window;

            // Listen out for the window resizing.
            mWindow.StateChanged += (_, _) =>
            {
                // Fire off events for all properties that are affected by a resize
                WindowResized();
            };

            // Create commands
            MinimizeCommand = new DelegateCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new DelegateCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new DelegateCommand(() => mWindow.Close());
            IconCommand = new DelegateCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));
            TopMostCommand = new DelegateCommand(() => mWindow.Topmost = !mWindow.Topmost);

            // Fix window resize issue.
            var resizer = new WindowResizer(mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                DockPosition = dock;

                // Fire off resize events
                WindowResized();
            };
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the current mouse position on the screen
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(mWindow);

            // Add the window position so its a "ToScreen"
            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
        }


        /// <summary>
        /// If the window resizes to a special position (docked or maximized)
        /// this will update all required property change events to set the borders and radius values
        /// </summary>
        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            RaisePropertyChanged(nameof(Borderless));
            RaisePropertyChanged(nameof(ResizeBorderThickness));
            RaisePropertyChanged(nameof(OuterMarginSize));
            RaisePropertyChanged(nameof(OuterMarginSizeThickness));
            RaisePropertyChanged(nameof(WindowRadius));
            RaisePropertyChanged(nameof(WindowCornerRadius));
        }

        #endregion
    }
}
