using System.Reflection;

namespace Rectrans.Infrastructure
{
    public enum Language
    {
        [Meta("zh", "简体中文")]
        ChineseSimplified,

        [Meta("en", "英语")]
        English,

        [Meta("ja", "日语")]
        Japanese,

        [Meta("ko", "韩语")]
        Korean,
    }

    public static class LanguageCollection
    {
        public static IEnumerable<MetaAttribute> AbbrAttributes 
            => typeof(Language).GetFields().SelectMany(f => f.GetCustomAttributes<MetaAttribute>());
    }

    public static class LanguageExtension
    {
        public static string? Name(this Language lang)
        {
            return Montage(lang).Item1;
        }

        public static string? Key(this Language lang)
        {
            return Montage(lang).Item2;
        }

        public static string? Name(this Language? lang)
        {
            if (lang is null) return null;
            return Montage(lang).Item1;
        }

        public static string? Key(this Language? lang)
        {
            if (lang is null) return null;
            return Montage(lang).Item2;
        }

        private static (string?, string?) Montage(Language? lang)
        {
            if (lang is null) return (null, null);
            var type = lang.GetType();
            var attr = type.GetField(Enum.GetName(type, lang)!)?.GetCustomAttribute<MetaAttribute>();
            return (attr?.Name, attr?.Key);
        }
    }
}