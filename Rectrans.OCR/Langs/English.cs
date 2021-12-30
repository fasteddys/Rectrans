using Tesseract;

namespace Rectrans.OCR
{
    public static class English
    {
        public static string Parse(string path)
        {
            try
            {
                using var engine = new TesseractEngine(@".\tessdata", "eng", EngineMode.Default);
                using var pix = Pix.LoadFromFile(path);
                using var page = engine.Process(pix);
                return page.GetText();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Parse(byte[] bytes)
        {
            try
            {
                using var engine = new TesseractEngine(@".\tessdata", "eng", EngineMode.Default);
                using var pix = Pix.LoadFromMemory(bytes);
                using var page = engine.Process(pix);
                return page.GetText();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}