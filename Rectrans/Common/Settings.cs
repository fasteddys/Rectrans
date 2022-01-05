using System;
using System.IO;
using Rectrans.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Rectrans.Common;

public static class Settings
{
    internal static string? ISO_639_1(string? key)
    {
        if (key is null) throw KeyNullException(key);

        var jt = GetJToken("Settings/translation_chart.json");
        var jo = GetJObject(jt, key);
        return jo?["ISO_639_1"]?.Value<string>();
    }

    internal static string? TrainedData(string? key)
    {
        if (key is null) throw KeyNullException(key);

        var jt = GetJToken("Settings/translation_chart.json");
        var jo = GetJObject(jt, key);
        return jo?["TrainedData"]?.Value<string>();
    }

    internal static IEnumerable<MenuItem> FetchMenuItems()
        => JsonConvert.DeserializeObject<IEnumerable<MenuItem>>(Settings.GetJsonStr("Settings/menu_items_data.json"))!;

    internal static string? GetMessageBorderBackground(string key)
    {
        if (key is null) throw KeyNullException(key);

        var jt = GetJToken("Settings/message_border.json");
        var jo = GetJObject(jt, key);
        return jo?["Background"]?.Value<string>();
    }

    internal static string GetJsonStr(string filename) =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, filename));

    private static JObject? GetJObject(JToken source, string key)
    {
        foreach (var jt in source)
        {
            switch (jt)
            {
                case JArray:
                    GetJObject(jt, key);
                    break;
                case JObject jo when jo["Key"]?.Value<string>() == key:
                    return jo;
                default:
                    foreach (var child in jt)
                    {
                        if (child is JArray)
                        {
                            GetJObject(jt, key);
                        }
                    }

                    break;
            }
        }

        return null;
    }

    private static JToken GetJToken(string filename) =>
        JToken.Parse(GetJsonStr(filename));

    private static Exception KeyNullException(string? key) =>
        new ArgumentNullException(key, "无法查找传入Key为`NULL`的Json对象.");

    internal static Exception KeyNotFoundException(string key) =>
        new NullReferenceException("`Warning` Background Not Found");
}