using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Rectrans.Infrastructure.Extensions;

public static class VisualExtension
{
    /// <summary>
    /// Transforms device independent units (1/96 of an inch)
    /// to pixels
    /// </summary>
    /// <param name="visual">a visual object</param>
    /// <param name="unitX">a device independent unit value X</param>
    /// <param name="unitY">a device independent unit value Y</param>
    /// <param name="pixelX">returns the X value in pixels</param>
    /// <param name="pixelY">returns the Y value in pixels</param>
    public static void TransformToPixels(this Visual visual, double unitX, double unitY, out int pixelX, out int pixelY)
    {
        Matrix matrix;
        var source = PresentationSource.FromVisual(visual);
        if (source != null)
        {
            matrix = source.CompositionTarget!.TransformToDevice;
        }
        else
        {
            using var src = new HwndSource(new HwndSourceParameters());
            matrix = src.CompositionTarget!.TransformToDevice;
        }

        pixelX = (int) (matrix.M11 * unitX);
        pixelY = (int) (matrix.M22 * unitY);
    }
    
    /// <summary>
    /// Transforms device independent units (1/96 of an inch)
    /// to pixels
    /// </summary>
    /// <param name="visual">a visual object</param>
    /// <param name="point">a device independent unit value</param>
    public static Point TransformToPixels(this Visual visual, Point point)
    {
        Matrix matrix;
        var source = PresentationSource.FromVisual(visual);
        if (source != null)
        {
            matrix = source.CompositionTarget!.TransformToDevice;
        }
        else
        {
            using var src = new HwndSource(new HwndSourceParameters());
            matrix = src.CompositionTarget!.TransformToDevice;
        }

        return matrix.Transform(point);
    }
}