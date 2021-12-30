using Rectrans.Infrastructure;

namespace Rectrans.OCR
{
    public static class BytesExtension
    {
        public static string Parse(this byte[] bytes, Language lang)
        {
            return lang switch
            {
                Language.English => English.Parse(bytes),
                Language.ChineseSimplified => ChineseSimplified.Parse(bytes),
                Language.Japanese => Japanese.Parse(bytes),
                Language.Korean => Korean.Parse(bytes),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
