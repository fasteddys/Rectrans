using Newtonsoft.Json;
using Rectrans.Common;
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
            => JsonConvert.DeserializeObject<IEnumerable<MenuItem>>(Settings.GetJsonStr("Settings/menu_items_data.json"),
                MenuItemSerializerSettings)!;
    }
}