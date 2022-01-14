namespace Rectrans.Models;

internal static class MenuItemExtension
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

    public static IEnumerable<MenuItem>? FindItems(this IEnumerable<MenuItem> source,
        Func<MenuItem, bool> predicate)
    {
        var items = source as MenuItem[] ?? source.ToArray();

        var result = items.Where(predicate).ToArray();

        foreach (var item in items.Select(x => x.ItemsSource))
        {
            var childResult = item?.FindItems(predicate);
            if (childResult != null)
            {
                result = result?.Concat(childResult).ToArray();
            }
        }

        return result;
    }
}

