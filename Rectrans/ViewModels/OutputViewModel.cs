using Prism.Commands;
using Prism.Mvvm;
using Rectrans.Extensions;
using Rectrans.Helpers;
using Rectrans.Views;
using System.Windows;
using System.Windows.Input;

namespace Rectrans.ViewModels
{
    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    public class OutputViewModel : BindableBase
    {
        #region Private Member

        /// <summary>
        /// The window this view model controls
        /// </summary>
        private OutputWindow mWindow;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int mOuterMarginSize = 10;

        /// <summary>
        /// The margin around the window content.
        /// </summary>
        private int mInnerMarginSize = 2;

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        private int mWindowRadius = 10;

        /// <summary>
        /// The last known dock position
        /// </summary>
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        /// <summary>
        /// The text of output.
        /// </summary>
        private string? outputText;

        #endregion

        #region Public Properties

        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 400;

        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 200;

        /// <summary>
        /// True if the window should be borderless because it is docked or maximized
        /// </summary>
        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder { get; set; } = 6;

        /// <summary>
        /// The size of the resize border around the window, taking into account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        /// <summary>
        /// The margin around the window content
        /// </summary>
        public int InnerMarginSize
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mInnerMarginSize;
            }
            set
            {
                mInnerMarginSize = value;
            }
        }

        /// <summary>
        /// The margin around the window content
        /// </summary>
        public Thickness InnerMarginSizeThickness { get { return new Thickness(InnerMarginSize); } }

        /// <summary>
        /// The radius of the edges of the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }

        /// <summary>
        /// The radius of the edges of the window top.
        /// </summary>
        public CornerRadius WindowTopCornerRadius { get { return new CornerRadius(WindowRadius, WindowRadius, 0, 0); } }

        /// <summary>
        /// The radius of the edges of the window bottom.
        /// </summary>
        public CornerRadius WindowBottomCornerRadius { get { return new CornerRadius(0, 0, WindowRadius, WindowRadius); } }

        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public int TitleHeight { get; set; } = 32;
        /// <summary>
        /// The height of the title bar / caption of the window
        /// </summary>
        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        /// <summary>
        /// The padding of the inner content of the main window
        /// </summary>
        public Thickness InnerContentPadding { get { return new Thickness(ResizeBorder); } }

        /// <summary>
        /// The text of output.
        /// </summary>
        public string OutputText
        {
            get => outputText ??= "采用设置一个矩形区域的做法进行文字识别，将识别出的文字进行翻译。不需要hook，基本适用于所有类型的游戏文本翻译。";
            set
            {
                outputText = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The font size of out put textblock.
        /// </summary>
        public double FontSize { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public OutputViewModel(OutputWindow window)
        {
            mWindow = window;

            // Listen out for the window resizing
            mWindow.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                WindowResized();
            };

            // Listen out for the window size changed
            mWindow.SizeChanged += (sender, e) =>
            {
                FontSize = mWindow.OutputTextBlock.CalculateFontSize(OutputText, "Lato Thin", "zh-cn");
                RaisePropertyChanged(nameof(FontSize));
            };

            // Create commands
            MinimizeCommand = new DelegateCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new DelegateCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new DelegateCommand(() => mWindow.Close());

            // Fix window resize issue
            var resizer = new WindowResizer(mWindow);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                mDockPosition = dock;

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
            RaisePropertyChanged(nameof(WindowTopCornerRadius));
        }


        #endregion
    }
}
