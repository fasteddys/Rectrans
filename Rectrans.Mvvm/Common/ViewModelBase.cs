using System.Composition;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rectrans.Mvvm.Common
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        [Import]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        protected IContext Context { get; set; } = null!;

        protected ViewModelBase()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            if (propName == null) throw new ArgumentNullException(nameof(propName));

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
    }
}
