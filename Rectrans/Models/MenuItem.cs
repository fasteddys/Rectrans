using Newtonsoft.Json;
using System.Collections;
using System.Windows.Input;
using System.Collections.ObjectModel;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Rectrans.Models;

public class MenuItem
{
    public static readonly ObservableCollection<MenuItem> DefaultCollection = new()
    {
        new()
        {
            Key = "Language", Header = "语言", ItemsSource = new()
            {
                new()
                {
                    Key = "Source", Header = "源语言", GroupName = "语言", ItemsSource = new()
                    {
                        new()
                        {
                            Key = "English", Header = "英语", GroupName = "源语言", IsCheckable = true, IsChecked = true,
                            Extra = "en"
                        },
                        new() {Key = "Japanese", Header = "日语", GroupName = "源语言", IsCheckable = true, Extra = "ja"},
                        new() {Key = "Korean", Header = "韩语", GroupName = "源语言", IsCheckable = true, Extra = "ko"},
                        new()
                        {
                            Key = "ChineseSimplified", Header = "简体中文", GroupName = "源语言", IsCheckable = true,
                            Extra = "zh"
                        },
                    }
                },
                new()
                {
                    Key = "Target", Header = "目标语言", GroupName = "语言", ItemsSource = new()
                    {
                        new()
                        {
                            Key = "ChineseSimplified", Header = "简体中文", GroupName = "目标语言", IsCheckable = true,
                            IsChecked = true, Extra = "zh"
                        },
                        new() {Key = "English", Header = "英语", GroupName = "目标语言", IsCheckable = true, Extra = "en"},
                        new() {Key = "Japanese", Header = "日语", GroupName = "目标语言", IsCheckable = true, Extra = "ja"},
                        new() {Key = "Korean", Header = "韩语", GroupName = "目标语言", IsCheckable = true, Extra = "ko"},
                    }
                },
            }
        },
        new()
        {
            Key = "AutomaticTranslation", Header = "自动翻译", ItemsSource = new()
            {
                new() {Header = "停止", GroupName = "自动翻译"},
                new() {Header = "2s", GroupName = "自动翻译", IsCheckable = true, Extra = 2000},
                new() {Header = "4s", GroupName = "自动翻译", IsCheckable = true, Extra = 4000},
                new() {Header = "8s", GroupName = "自动翻译", IsCheckable = true, Extra = 8000},
                new() {Header = "12s", GroupName = "自动翻译", IsCheckable = true, Extra = 12000},
            }
        }
    };

    public string Key { get; set; } = null!;

    public string Header { get; set; } = null!;

    public bool IsCheckable { get; set; }

    public bool IsChecked { get; set; }

    public string? GroupName { get; set; }

    public ICommand? Command { get; set; }

    public object? CommandParameter { get; set; }

    public bool HasItems => ItemsSource != null && ItemsSource.Any();

    public ObservableCollection<MenuItem>? ItemsSource { get; set; }

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