using Rectrans.Infrastructure;

namespace Rectrans.OCR
{
    public static class StringExtension
    {
        public static string Parse(this string str, Language lang)
        {
            return lang switch
            {
                Language.English => English.Parse(str),
                Language.ChineseSimplified => ChineseSimplified.Parse(str),
                Language.Japanese => Japanese.Parse(str),
                Language.Korean => Korean.Parse(str),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
