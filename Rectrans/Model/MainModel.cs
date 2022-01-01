using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Rectrans.Model
{
    public static class MainModel
    {
        private static readonly JsonSerializerSettings MenuItemSerializerSettings = new()
        {
            //Converters = new[] { new MenuItemConverter() }
        };

        public static IEnumerable<MenuItem> Fetch()
            => JsonConvert.DeserializeObject<IEnumerable<MenuItem>>
                (File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Model/Json/menu_items.json")), MenuItemSerializerSettings)!;
    }

    public class MenuItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IEnumerable);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Path.Contains("ItemsSource"))
            {
                return serializer.Deserialize<IEnumerable<System.Windows.Controls.MenuItem>>(reader);

            }
            return serializer.Deserialize(reader);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => serializer.Serialize(writer, value);
    }
}
