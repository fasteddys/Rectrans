using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Rectrans.Common;

public static class Settings
{
    internal static string? ISO_639_1(string? name)
    {
        if (name is null) throw new ArgumentNullException(name, "无法查找传入Name为`NULL`的Json对象.");

        var jt = GetJToken("Settings/languages.json");
        var jo = GetJObject(jt, name);
        return jo?["ISO_639_1"]?.Value<string>();
    }

    internal static string? TrainedData(string? name)
    {
        if (name is null) throw new ArgumentNullException(name, "无法查找传入Name为`NULL`的Json对象.");

        var jt = GetJToken("Settings/languages.json");
        var jo = GetJObject(jt, name);
        return jo?["TrainedData"]?.Value<string>();
    }

    internal static string GetJsonStr(string filename) =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, filename));

    private static JObject? GetJObject(JToken source, string name)
    {
        foreach (var jt in source)
        {
            switch (jt)
            {
                case JArray:
                    GetJObject(jt, name);
                    break;
                case JObject jo when jo["Name"]?.Value<string>() == name:
                    return jo;
                default:
                    foreach (var child in jt)
                    {
                        if (child is JArray)
                        {
                            GetJObject(jt, name);
                        }
                    }

                    break;
            }
        }

        return null;
    }

    private static JToken GetJToken(string filename) =>
        JToken.Parse(GetJsonStr(filename));
}