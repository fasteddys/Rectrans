using System.IO;
using Tesseract;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Rectrans.Infrastructure;

internal static class ScreenReader
{
    /// <summary>
    /// Frame a picture from screen and recognize the text in it 
    /// </summary>
    /// <param name="func">(scaling, (x, y, height, width))</param>
    /// <param name="tessdata">tessdata name</param>
    /// <param name="outputPng"></param>
    /// <returns></returns>
    // ReSharper disable once IdentifierTypo
    public static string ReadFormScreen(Func<double,(double,double,double,double)> func, string tessdata, bool outputPng = false)
    {
        using var stream = new MemoryStream();
        var bitmap = Dpi.CreateScalingBitmap(func);

        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

        if (outputPng)
        {
            bitmap.Save("debug.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        return ReadFormFromMemory(stream.ToArray(), tessdata);
    }

    /// <summary>
    /// Frame a picture from file and recognize the text in it 
    /// </summary>
    /// <param name="filename">filename</param>
    /// <param name="tessdata">tessdata name</param>
    /// <returns></returns>
    // ReSharper disable once IdentifierTypo
    // ReSharper disable once UnusedMember.Local
    public static string ReadFormFromFile(string filename, string tessdata)
    {
        try
        {
            // ReSharper disable once StringLiteralTypo
            using var engine = new TesseractEngine(@".\tessdata", tessdata, EngineMode.Default);
            using var pix = Pix.LoadFromFile(filename);
            using var page = engine.Process(pix);
            return PostProcess(page.GetText());
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    /// <summary>
    /// Frame a picture from memory and recognize the text in it 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="tessdata"></param>
    /// <returns></returns>
    // ReSharper disable once IdentifierTypo
    public static string ReadFormFromMemory(byte[] bytes, string tessdata)
    {
        try
        {
            // ReSharper disable once StringLiteralTypo
            using var engine = new TesseractEngine(@".\tessdata", tessdata, EngineMode.Default);
            using var pix = Pix.LoadFromMemory(bytes);
            using var page = engine.Process(pix);
            return PostProcess(page.GetText());
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private static string PostProcess(string text)
        => new Regex(@"(^\s*)|(\s*$)", RegexOptions.Compiled).Replace(text, "").Replace('|', 'I');
}