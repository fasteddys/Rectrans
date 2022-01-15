using System.Windows;

namespace Rectrans.Services
{
    public interface IWindowManager
    {
        void Register(Window window);

        TWindow? Resolve<TWindow>() where TWindow : Window;
    }
}
