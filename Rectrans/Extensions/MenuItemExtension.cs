using System;
using System.Linq;
using Rectrans.Model;
using System.Collections.Generic;

namespace Rectrans.Extensions;

public static class MenuItemExtension
{
    public static MenuItem? FindItem(this IEnumerable<MenuItem> source,
        Func<MenuItem, bool> predicate)
    {
        var items = source as MenuItem[] ?? source.ToArray();

        foreach (var item in items)
        {
            if (predicate(item))
            {
                return item;
            }
        }

        foreach (var children in items.Select(x => x.ItemsSource))
        {
            var child = children?.FindItem(predicate);
            if (child != null)
            {
                return child;
            }
        }

        return null;
    }
}