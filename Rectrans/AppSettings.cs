using System.IO;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace Rectrans;

public static class AppSettings
{
    internal static Brush GetMessageBorderBackground(string key)
    {
        if (key is null) throw KeyNullException(key);
        var str = GetJObject(MessageBorderJToken, key)?["Background"]?.Value<string>()!;
        return (Brush)new BrushConverter().ConvertFromString(str)!;
    }

    private static JToken MessageBorderJToken =>
        JToken["MessageBorder"]?["Types"] ?? throw KeyNotFoundException("MessageBorder.Types");

    private static JObject? GetJObject(JToken source, string key)
    {
        switch (source)
        {
            case JObject jo when jo["Key"]?.Value<string>() == key:
                return jo;
            default:
                foreach (var item in source)
                {
                    var jo = GetJObject(item, key);
                    if (jo != null) return jo;
                }

                break;
        }

        return null;
    }

    // ReSharper disable once StringLiteralTypo
    private static JToken JToken =>
        JToken.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json")));

    private static Exception KeyNullException(string? key) =>
        new ArgumentNullException(key, "无法查找传入Key为`NULL`的Json对象.");

    internal static Exception KeyNotFoundException(string key) =>
        new NullReferenceException("`Warning` Background Not Found");
}