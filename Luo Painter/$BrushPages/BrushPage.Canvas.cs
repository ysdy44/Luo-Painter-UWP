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
            this.Canvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;
                if (this.AlignmentGrid.RebuildWithInterpolation(e.NewSize) is false) return;

                this.CanvasControl.Width = e.NewSize.Width;
                this.CanvasControl.Height = e.NewSize.Height;

                Vector2 size = this.CanvasControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.CreateResources((int)size.X, (int)size.Y);
            };


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