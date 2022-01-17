namespace Rectrans.Infrastructure;

public static class ImageTranslate
{
    private static string? _storeSourceText;
    private static string? _storeDestinationText;

    public static async Task<(string, string, int)> ExecuteAsync(double x, double y, double height, double width,
        string sl, string tl)
    {
        var translatedTextCount = 0;
        var sourceText = Identify.FromScreen(x, y, height, width, sl);

        if (_storeSourceText == null
            || _storeDestinationText == null
            || _storeSourceText != sourceText)
        {
            _storeSourceText = sourceText;
            _storeDestinationText = await Google.ProcessAsync(sourceText, sl, tl);
            translatedTextCount = _storeSourceText.Length;
        }

        return (_storeSourceText, _storeDestinationText, translatedTextCount);
    }
}