using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton : Button, IInkParameter
    {

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                float width = sender.Dpi.ConvertDipsToPixels(320);
                float height = sender.Dpi.ConvertDipsToPixels(358);
                this.CreateResources((int)width, (int)height);
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Source]);
                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Temp]);
            };
        }

    }
}