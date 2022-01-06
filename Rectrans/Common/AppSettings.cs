using System;
using System.IO;
using Rectrans.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Rectrans.Common;

public static class AppSettings
{
    internal static IEnumerable<MenuItem> MenuItems => MenuItemsJToken.ToObject<IEnumerable<MenuItem>>()!;

    internal static string? ISO_639_1(string? key)
    {
        if (key is null) throw KeyNullException(key);
        return GetJObject(MenuItemsJToken, key)?["ISO_639_1"]?.Value<string>();
    }

    internal static string? TrainedData(string? key)
    {
        if (key is null) throw KeyNullException(key);
        return GetJObject(MenuItemsJToken, key)?["TrainedData"]?.Value<string>();
    }

    internal static string? GetMessageBorderBackground(string key)
    {
        if (key is null) throw KeyNullException(key);
        return GetJObject(MessageBorderJToken, key)?["Background"]?.Value<string>();
    }

    private static JToken MenuItemsJToken => JToken["MenuItems"] ?? throw KeyNotFoundException("MenuItems");

    private static JToken MessageBorderJToken =>
        JToken["MessageBorder"]?["Types"] ?? throw KeyNotFoundException("MessageBorder.Types");

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

    private static JToken JToken =>
        JToken.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json")));

    private static Exception KeyNullException(string? key) =>
        new ArgumentNullException(key, "无法查找传入Key为`NULL`的Json对象.");

    internal static Exception KeyNotFoundException(string key) =>
        new NullReferenceException("`Warning` Background Not Found");
}