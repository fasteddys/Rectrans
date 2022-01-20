using System.Drawing;
using System.Windows;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Rectrans.Infrastructure;

public static class Dpi
{
    /// <summary>
    /// Create bitmap based on scaling
    /// </summary>
    /// <param name="func">(scaling, (x, y, height, width))</param>
    /// <returns></returns>
    public static Bitmap CreateScalingBitmap(Func<double, (double, double, double, double)> func)
    {
        var scale = GetScreenScalingFactor();

        var (x, y, height, width) = func.Invoke(scale);

        var ix = (int)x;
        var iy = (int)y;
        var ih = (int)height;
        var iw = (int)width;

        var bitmap = new Bitmap(iw, ih);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(ix, iy, 0, 0, new System.Drawing.Size(iw, ih));
        return bitmap;
    }

    #region Private Methods

    /// <summary>
    /// Get screen scaling factor 
    /// </summary>
    /// <returns></returns>
    private static double GetScreenScalingFactor()
    {
        var desktop = GetDC(IntPtr.Zero);
        var physicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

        return physicalScreenHeight / SystemParameters.PrimaryScreenHeight;
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

    #endregion
}