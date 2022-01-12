namespace Rectrans.Mvvm.Common;

public interface IContext
{
    bool IsSynchronized { get; }
    void Invoke(Action callback);
    void BeginInvoke(Delegate method, params object[] args);
}