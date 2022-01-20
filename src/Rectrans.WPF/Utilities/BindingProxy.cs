using System.Windows;

namespace Rectrans.WPF.Utilities
{
    internal class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {

            return new BindingProxy();
        }

        public object DataSource
        {
            get => GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }

        public static readonly DependencyProperty DataSourceProperty
            = DependencyProperty.Register(nameof(DataSource),
                typeof(object),
                typeof(BindingProxy),
                new PropertyMetadata(null));
    }
}
