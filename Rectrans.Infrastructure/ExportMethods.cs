namespace Rectrans.Infrastructure;

public static class ImageTranslate
{
    private static string? _original;
    private static string? _translation;
    private static string? _tl;

    public static async Task<(string, string, int)> ExecuteAsync(double x, double y, double height,
        double width, string sl, string tl)
    {
        var count = 0;
        var sourceText = Identify.FromScreen(x, y, height, width, sl);

        if (_original == null
            || _translation == null
            || _tl == null
            || _original != sourceText
            || _tl != tl)
        {
            _original = sourceText;
            _translation = await Google.ProcessAsync(sourceText, sl, tl);
            _tl = tl;
            count = _original.Length;
        }

        return (_original, _translation, count);
    }
}