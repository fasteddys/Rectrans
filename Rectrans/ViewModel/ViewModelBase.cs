using System;
using System.Diagnostics;
using System.Composition;
using Rectrans.Interface;
using System.Windows.Input;
using System.ComponentModel;
using System.Composition.Hosting;

namespace Rectrans.ViewModel;

public abstract class ViewModelBase : IMessageBorder, INotifyPropertyChanged
{
    [Import]
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    protected IContext Context { get; set; } = null!;

    // ReSharper disable once InconsistentNaming
    protected readonly MessageBorderMonitor MessageBorderMonitor;

    protected ViewModelBase()
    {
        ContainerConfiguration();
        MessageBorderMonitor = new MessageBorderMonitor(this);
    }

    private void ContainerConfiguration()
    {
        var container = new ContainerConfiguration()
            .WithAssembly(typeof(IContext).Assembly)
            .CreateContainer();
        container.SatisfyImports(this);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string propName)
    {
        VerifyPropertyName(propName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    private void VerifyPropertyName(string propName)
    {
        if (TypeDescriptor.GetProperties(this)[propName] != null) return;
        var msg = "Invalid property name: " + propName;
        if (ThrowOnInvalidPropertyName)
            // ReSharper disable once HeuristicUnreachableCode
#pragma warning disable CS0162
            throw new Exception(msg);
#pragma warning restore CS0162
        // ReSharper disable once RedundantIfElseBlock
        else
            Debug.Fail(msg);
    }

    private const bool ThrowOnInvalidPropertyName = false;
    public string MessageBorderText { get; set; } = null!;
    public string MessageBorderVisibility { get; set; } = null!;
    public string MessageBorderBackground { get; set; } = null!;
    public string MessageBorderCloseButtonVisibility { get; set; } = null!;
    public string MessageBorderHyperlinkVisibility { get; set; } = null!;
    public string MessageBorderHyperlinkText { get; set; } = null!;
    public ICommand MessageBorderHyperlinkCommand { get; set; } = null!;
    public ICommand MessageBorderCloseButtonCommand { get; set; } = null!;

    public virtual void OnMessageBorderHyperlinkClick(object? parameter)
        => MessageBorderMonitor.CloseMessageBorder();
}