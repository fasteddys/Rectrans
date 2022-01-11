using System.Windows;
using System.Windows.Media;

namespace Rectrans.Mvvm.Extensions
{
    public static class FrameworkElementExtension
    {
        public static T? GetTemplatedParent<T>(this FrameworkElement o)
            where T : DependencyObject
        {
            DependencyObject? child = o, parent = null;

            while (child != null && (parent = LogicalTreeHelper.GetParent(child)) == null)
            {
                child = VisualTreeHelper.GetParent(child);
            }

            return parent is FrameworkElement frameworkParent ? frameworkParent.TemplatedParent as T : null;
        }
    }
}
