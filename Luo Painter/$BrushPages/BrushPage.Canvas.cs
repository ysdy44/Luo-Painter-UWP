using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter
{
    public sealed partial class BrushPage
    {

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                float width = sender.Dpi.ConvertDipsToPixels((float)sender.ActualWidth);
                float height = sender.Dpi.ConvertDipsToPixels((float)sender.ActualHeight);
                this.CreateResources((int)width, (int)height);
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                this.InkPresenter.Preview(args.DrawingSession, this.InkType, this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]);
            };
        }

    }
}