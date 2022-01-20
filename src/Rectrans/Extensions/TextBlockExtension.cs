using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rectrans.Extensions
{
    public static class TextBlockExtension
    {
        /// <summary>
        /// Calculate for font size.
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="text"></param>
        /// <param name="fontFamily"></param>
        /// <param name="cultureName"> like: en-us</param>
        /// <returns></returns>
        public static double CalculateFontSize(this TextBlock textBlock, string text, string fontFamily, string cultureName)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;

            int maxFontSize = 1;

            var ft = new FormattedText(text, CultureInfo.GetCultureInfo(cultureName), FlowDirection.LeftToRight,
                new Typeface(fontFamily), maxFontSize, Brushes.Black, VisualTreeHelper.GetDpi(textBlock).PixelsPerDip);

            while (true)
            {
                var height = (ft.Width / textBlock.ActualWidth + 1) * ft.Height;
                if (textBlock.ActualHeight > height)
                {
                    ft.SetFontSize(maxFontSize++);
                }
                else
                {
                    break;
                }
            }

            return --maxFontSize;
        }
    }
}
