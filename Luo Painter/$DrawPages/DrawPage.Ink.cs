using Luo_Painter.Brushes;
using Luo_Painter.Controls;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        bool InkIsEnabled;

        private async void ConstructBrush(PaintBrush brush = null)
        {
            if (brush is null) return;

            if (string.IsNullOrEmpty(brush.Shape)) this.InkPresenter.ClearShape();
            else this.InkPresenter.ConstructShape(brush.Shape, await CanvasBitmap.LoadAsync(this.CanvasDevice, brush.Shape.GetTextureSource()));

            if (string.IsNullOrEmpty(brush.Grain)) this.InkPresenter.ClearGrain();
            else this.InkPresenter.ConstructGrain(brush.Grain, await CanvasBitmap.LoadAsync(this.CanvasDevice, brush.Grain.GetTextureSource()));

            this.InkPresenter.Construct(brush);
            this.InkType = this.InkPresenter.GetType();

            this.InkIsEnabled = false;
            this.AppBar.ConstructInk(this.InkPresenter, true);
            this.InkIsEnabled = true;

            this.PaintScrollViewer.ConstructInk(this.InkPresenter);
            this.PaintScrollViewer.TryInk();
        }

        private void ConstructSize(float size)
        {
            this.InkPresenter.Size = size;

            this.InkIsEnabled = false;
            this.AppBar.ConstructInk(this.InkPresenter, true);
            this.InkIsEnabled = true;

            this.PaintScrollViewer.ConstructInk(this.InkPresenter);
            this.PaintScrollViewer.TryInk();
        }

        private void ConstructInk()
        {
            this.AppBar.ConstructInk(this.InkPresenter, false);
            this.InkIsEnabled = true;

            this.AppBar.SizeValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkPresenter.Size = (float)size;
            };
            this.AppBar.OpacityValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                double opacity = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Opacity = (float)opacity;
                this.InkType = this.InkPresenter.GetType();
            };
        }

    }
}