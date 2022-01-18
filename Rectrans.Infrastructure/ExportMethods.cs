namespace Rectrans.Infrastructure;

public static class ImageTranslate
{
    private static string? _original;
    private static string? _translationg;

    public static async Task<(string, string, int)> ExecuteAsync(double x, double y, double height,
        double width, string sl, string tl)
    {
        var count = 0;
        var sourceText = Identify.FromScreen(x, y, height, width, sl);

        if (_original == null
            || _translationg == null
            || _original != sourceText)
        {
            _original = sourceText;
            _translationg = await Google.ProcessAsync(sourceText, sl, tl);
            count = _original.Length;
        }

        return (_original, _translationg, count);
    }
}