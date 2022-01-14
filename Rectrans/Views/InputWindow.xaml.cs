namespace Rectrans.Views;

public partial class InputWindow
{

    public InputWindow()
    {
        InitializeComponent();
        MouseLeftButtonDown += delegate { DragMove(); };
    }
}