namespace Rectrans.OCR
{
    public static class English
    {
        private static readonly IronOcr.IronTesseract Tesseract = new();

        public static string FromPath(string path)
            => Tesseract.Read(path).Text;
        public static string FromImage(System.Drawing.Image image)
            => Tesseract.Read(image).Text;
        public static async Task<string> FromPathAsync(string path)
            => (await Tesseract.ReadAsync(path)).Text;
        public static async Task<string> FromImageAsync(System.Drawing.Image image)
            => (await Tesseract.ReadAsync(image)).Text;
    }
}