using System;
using Rectrans.View;

namespace Rectrans.EventHandlers;

public class RectangleViewCreatedEventArgs : EventArgs
{
    public RectangleViewCreatedEventArgs(RectangleView rectangleView)
    {
        RectangleView = rectangleView;
    }

    public RectangleView RectangleView { get; }
}