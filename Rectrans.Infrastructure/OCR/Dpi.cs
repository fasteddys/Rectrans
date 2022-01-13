using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Rectrans.Infrastructure;

internal static class Dpi
{
    public static Bitmap CreateBitmapWithActualScreen(double x, double y, double height, double width)
    {
        var factor = GetScreenScalingFactor();
        var ix = (int) (x * factor);
        var iy = (int) (y * factor);
        var iw = (int) (width * factor);
        var ih = (int) (height * factor);

        var bitmap = new Bitmap(iw, ih);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(ix, iy, 0, 0, new Size(iw, ih));
        return bitmap;
    }

    public static int FontSize(double x, double y, int textCount) => (int) Math.Ceiling(x * y);

    private static double GetScreenScalingFactor()
    {
        var desktop = GetDC(IntPtr.Zero);
        var physicalScreenHeight = GetDeviceCaps(desktop, (int) DeviceCap.DESKTOPVERTRES);

        return physicalScreenHeight / System.Windows.SystemParameters.PrimaryScreenHeight;
    }

    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr ptr);

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    private enum DeviceCap
    {
        VERTRES = 10,
        PHYSICALWIDTH = 110,
        SCALINGFACTORX = 114,
        DESKTOPVERTRES = 117,
    }
}