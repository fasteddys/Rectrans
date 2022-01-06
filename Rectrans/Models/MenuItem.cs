using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Rectrans.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class MenuItem : System.Windows.Controls.MenuItem
{
    public string Key { get; set; } = null!;

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

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public object? Extra { get; set; }
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