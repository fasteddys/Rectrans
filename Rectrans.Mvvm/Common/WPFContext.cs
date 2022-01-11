using System.Composition;
using System.Diagnostics;
using System.Windows.Threading;

namespace Rectrans.Mvvm.Common;

[Export(typeof(IContext))]
// ReSharper disable once InconsistentNaming
public class WPFContext : IContext
{
    private readonly Dispatcher _dispatcher;

    public WPFContext() : this(Dispatcher.CurrentDispatcher)
    {
    }

    public WPFContext(Dispatcher dispatcher)
    {
        Debug.Assert(dispatcher != null);
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public bool IsSynchronized => _dispatcher.Thread == Thread.CurrentThread;

    public void Invoke(Action callback)
    {
        Debug.Assert(callback != null);
        _dispatcher.Invoke(callback);
    }

    public void BeginInvoke(Delegate method, params object[] args)
    {
        Debug.Assert(method != null);
        _dispatcher.BeginInvoke(method, args);
    }
}