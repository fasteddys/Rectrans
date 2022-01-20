namespace Rectrans.Infrastructure;

public static class ScreenTranslator
{
    /// <summary>
    /// Store original text
    /// </summary>
    private static string? _original;

    /// <summary>
    /// Store translation text
    /// </summary>
    private static string? _translation;

    /// <summary>
    /// Store target language
    /// </summary>
    private static string? _tl;

    /// <summary>
    /// Frame a picture from the screen and translate the text in it 
    /// </summary>
    /// <param name="func">(scaling, (x, y, height, width))</param>
    /// <param name="sl">source language</param>
    /// <param name="tl">target language</param>
    /// <param name="outputPng"></param>
    /// <returns></returns>
    public static async Task<(string, string, int)> TranslateAsync(Func<double,(double, double, double, double)> func, string sl, string tl, bool outputPng = false)
    {
        var count = 0;
        var sourceText = ScreenReader.ReadFormScreen(func, sl, outputPng);

        if (_original == null
            || _translation == null
            || _tl == null
            || _original != sourceText
            || _tl != tl)
        {
            _original = sourceText;
            _translation = await GoogleTranslation.TranslateAsync(sourceText, sl, tl);
            _tl = tl;
            count = _original.Length;
        }

        return (_original, _translation, count);
    }
}