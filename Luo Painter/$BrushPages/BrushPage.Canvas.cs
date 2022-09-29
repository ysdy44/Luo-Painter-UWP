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
                this.InkRender = new InkRender
                {
                    //@DPI
                    ScaleForDPI = sender.Dpi.ConvertPixels(),
                    SizeInPixels = new InkRenderSize((int)sender.Dpi.ConvertDipsToPixels(320), (int)sender.Dpi.ConvertDipsToPixels(100)),
                    Size = new InkRenderSize(320, 100),
                };
            };
            this.InkCanvasControl.Draw += (sender, args) =>
            {
                if (this.ShaderCodeByteIsEnabled is false) return;
                if (this.InkRender is null) return;
                this.Ink(args.DrawingSession);
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

                args.DrawingSession.DrawImage(
                    this.InkPresenter.GetPreview(this.InkType, this.BitmapLayer[BitmapType.Source],
                        this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp])));
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Pressure = properties.Pressure;

                this.Paint_Start();
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Paint_Delta(this.CanvasControl.Dpi.ConvertDipsToPixels(point), properties.Pressure);
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.Paint_Complete();
            };
        }

    }
}