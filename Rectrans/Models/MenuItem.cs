using Newtonsoft.Json;
using System.Collections;
using System.Windows.Input;
using System.Collections.ObjectModel;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Rectrans.Models;


public class MenuItem
{
    public string Key { get; set; } = null!;

    public string Header { get; set; } = null!;

    public bool IsCheckable { get; set; }

    public bool IsChecked { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? ISO_639_1 { get; set; }

    public string? TrainedData { get; set; }

    public string? Description { get; set; }

    public MenuItem? Parent { get; set; }

    public ICommand? Command { get; set; }

    public object? CommandParameter { get; set; }

    public object? Extra { get; set; }

    public bool HasItems => ItemsSource != null && ItemsSource.Any();

    private ObservableCollection<MenuItem>? _itemsSource;

    public ObservableCollection<MenuItem>? ItemsSource
    {
        get => _itemsSource;

        set
        {
            _itemsSource = value;
            if (ItemsSource == null) return;
            foreach (var item in ItemsSource)
            {
                item.Parent = this;
            }
        }
    }
}

public class MenuItemConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(IEnumerable);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.Path.Contains("ItemsSource"))
        {
            return serializer.Deserialize<IEnumerable<System.Windows.Controls.MenuItem>>(reader);
        }

        return serializer.Deserialize(reader);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) =>
        serializer.Serialize(writer, value);
}

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