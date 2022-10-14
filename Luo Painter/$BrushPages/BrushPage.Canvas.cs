using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
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

                if (this.ShaderCodeByteIsEnabled is false) return;
                Vector2 size = this.CanvasControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.CreateResources((int)size.X, (int)size.Y);
            };

            this.InkCanvasControl.CreateResources += (sender, args) =>
            {
                this.InkRender = new CanvasRenderTarget(sender, InkPresenter.Width, InkPresenter.Height);
            };
            this.InkCanvasControl.Draw += (sender, args) =>
            {
                switch (this.InkType)
                {
                    case InkType.Blur:
                        args.DrawingSession.DrawImage(InkPresenter.GetBlur(this.InkRender, this.InkPresenter.Flow * 4));
                        break;
                    case InkType.Mosaic:
                        args.DrawingSession.DrawImage(InkPresenter.GetMosaic(this.InkRender, this.InkPresenter.Size / 10));
                        break;
                    default:
                        args.DrawingSession.DrawImage(this.InkRender);
                        break;
                }
            };


            this.CanvasControl.CreateResources += (sender, args) =>
            {
                float sizeX = sender.Dpi.ConvertDipsToPixels((float)sender.ActualWidth);
                float sizeY = sender.Dpi.ConvertDipsToPixels((float)sender.ActualHeight);
                this.CreateResources((int)sizeX, (int)sizeY);
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