namespace Rectrans.Models;

public static class DataAccess
{
    public static List<ComboBoxItem> GetLanguageItems()
    {
        return new List<ComboBoxItem>
        {
            new() {Name = "英语", Value = "en"},
            new() {Name = "中文", Value = "zh"},
            new() {Name = "韩语", Value = "ko"},
            new() {Name = "日语", Value = "ja"},
        };
    }

    public static List<ComboBoxItem> GetIntervalItems()
    {
        return new List<ComboBoxItem>
        {
            new() {Name = "手动", Value = "0"},
            new() {Name = "2秒", Value = "2000"},
            new() {Name = "4秒", Value = "4000"},
            new() {Name = "8秒", Value = "8000"},
            new() {Name = "12秒", Value = "12000"},
        };
    }
}