using System.IO;
using Tesseract;
using System.Text.RegularExpressions;

namespace Rectrans.OCR
{
    public static class Identify
    {
        // ReSharper disable once IdentifierTypo
        public static string FromScreen(double x, double y, double height, double width, string? tessdata)
        {
            if (tessdata == null)
            {
                throw new ArgumentNullException(tessdata, "传入训练用的数据文件名为 `NULL`.");
            }

            using var stream = new MemoryStream();
            var bitmap = Dpi.CreateBitmapWithActualScreen(x, y, height, width);
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            // bitmap.Save("debug.png", System.Drawing.Imaging.ImageFormat.Png);

            return FromMemory(stream.ToArray(), tessdata);
        }

        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Local
        private static string FromFile(string filename, string tessdata)
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

        // ReSharper disable once IdentifierTypo
        private static string FromMemory(byte[] bytes, string tessdata)
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
}