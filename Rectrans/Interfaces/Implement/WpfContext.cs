using System;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Rectrans.Interfaces.Implement;

[Export(typeof(IContext))]
public class WpfContext : IContext
{
    private readonly Dispatcher _dispatcher;

    public WpfContext() : this(Dispatcher.CurrentDispatcher)
    {
    }

    public WpfContext(Dispatcher dispatcher)
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