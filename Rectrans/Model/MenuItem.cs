using System;
using System.Linq;
using System.Collections.Generic;

namespace Rectrans.Model
{
    public class MenuItem : System.Windows.Controls.MenuItem
    {
        public new MenuItem? Parent { get; set; }

        public new IEnumerable<MenuItem>? ItemsSource
        {
            get => (IEnumerable<MenuItem>) base.ItemsSource;
            set
            {
                base.ItemsSource = value;
                if (value == null) return;
                foreach (var item in value)
                {
                    item.Parent = this;
                }
            }
        }

        public new IEnumerable<MenuItem>? Items => ItemsSource;

        public int? Interval { get; set; }
    }

    public static class MenuItemExtension
    {
        public static MenuItem? Recursion(this IEnumerable<MenuItem> source,
            Func<MenuItem, bool> predicate)
        {
            var items = source as MenuItem[] ?? source.ToArray();
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                return item;
            }

            foreach (var children in items.Select(x => x.ItemsSource))
            {
                var child = children?.Recursion(predicate);
                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }
    }
}