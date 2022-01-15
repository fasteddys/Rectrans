using System.Windows;

namespace Rectrans.Services.Implement
{
    public class WindowManager : IWindowManager
    {
        private static readonly object CreationLock = new();
        private static IWindowManager? _defaultInstance;
        private static readonly Dictionary<Type, Window> _storeNameAndWindows = new();

        public static IWindowManager Default
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (CreationLock)
                    {
                        if (_defaultInstance == null)
                        {
                            _defaultInstance = new WindowManager();
                        }
                    }
                }
                return _defaultInstance;
            }
        }

        private WindowManager()
        {
        }

        public void Register(Window window)
        {
            var type = window.GetType();
            lock (_storeNameAndWindows)
            {
                if (_storeNameAndWindows.ContainsKey(type))
                {
                    _storeNameAndWindows.Remove(type);
                }

                _storeNameAndWindows.Add(type, window);
            }
        }

        public TWindow? Resolve<TWindow>() where TWindow : Window
        {
            if (_storeNameAndWindows.Count == 0
                || !_storeNameAndWindows.TryGetValue(typeof(TWindow), out Window? value))
                return null;

            return (TWindow)value;
        }
    }
}
