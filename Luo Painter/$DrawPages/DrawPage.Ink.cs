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

        bool IsInkEnabled;

        private async void ConstructBrush(PaintBrush brush = null)
        {
            if (brush is null) return;

            if (string.IsNullOrEmpty(brush.Shape)) this.InkPresenter.ClearShape();
            else this.InkPresenter.ConstructShape(brush.Shape, await CanvasBitmap.LoadAsync(this.CanvasDevice, brush.Shape.GetTextureSource()));

            if (string.IsNullOrEmpty(brush.Grain)) this.InkPresenter.ClearGrain();
            else this.InkPresenter.ConstructGrain(brush.Grain, await CanvasBitmap.LoadAsync(this.CanvasDevice, brush.Grain.GetTextureSource()));

            this.InkPresenter.Construct(brush);
            this.InkType = this.InkPresenter.GetType();

            this.IsInkEnabled = false;
            this.ConstructInk(this.InkPresenter, true);
            this.IsInkEnabled = true;

            this.PaintScrollViewer.ConstructInk(this.InkPresenter);
            this.PaintScrollViewer.TryInk();
        }

        private void ConstructSize(float size)
        {
            this.InkPresenter.Size = size;

            this.IsInkEnabled = false;
            this.ConstructInk(this.InkPresenter, true);
            this.IsInkEnabled = true;

            this.PaintScrollViewer.ConstructInk(this.InkPresenter);
            this.PaintScrollViewer.TryInk();
        }

        public void ConstructInk(InkPresenter presenter, bool onlyValue)
        {
            // 1.Minimum
            if (onlyValue is false)
            {
                this.InkSizeSlider.Minimum = this.SizeRange.XRange.Minimum;
                this.InkOpacitySlider.Minimum = 0d;
            }

            // 2.Value
            this.InkSizeSlider.Value = this.SizeRange.ConvertYToX(presenter.Size);
            this.InkOpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);

            // 3.Maximum
            if (onlyValue is false)
            {
                this.InkSizeSlider.Maximum = this.SizeRange.XRange.Maximum;
                this.InkOpacitySlider.Maximum = 100d;
            }
        }


        public void ConstructInk()
        {
            this.ConstructInk(this.InkPresenter, false);
            this.IsInkEnabled = true;

            this.InkSizeSlider.ValueChanged += (s, e) =>
            {
                if (this.IsInkEnabled is false) return;

                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkPresenter.Size = (float)size;
            };
            this.InkOpacitySlider.ValueChanged += (s, e) =>
            {
                if (this.IsInkEnabled is false) return;

                double opacity = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Opacity = (float)opacity;
                this.InkType = this.InkPresenter.GetType();
            };
        }

    }
}