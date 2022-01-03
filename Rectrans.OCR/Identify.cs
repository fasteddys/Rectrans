using System.Text.RegularExpressions;
using Tesseract;

namespace Rectrans.OCR
{
    public static class Identify
    {
        // ReSharper disable once IdentifierTypo
        public static string FromFile(string filename, string tessdata)
        {
            if (tessdata is null) throw new ArgumentNullException(tessdata, "传入训练用的数据文件名为 `NULL`.");

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

        // ReSharper disable once IdentifierTypo
        public static string FromMemory(byte[] bytes, string? tessdata)
        {
            if (tessdata is null) throw new ArgumentNullException(tessdata, "传入训练用的数据文件名为 `NULL`.");

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
}