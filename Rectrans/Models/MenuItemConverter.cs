using Newtonsoft.Json;
using System.Collections;

namespace Rectrans.Models;

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
