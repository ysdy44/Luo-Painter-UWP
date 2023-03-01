using Luo_Painter.Brushes;
using Luo_Painter.Controls;
using Microsoft.Graphics.Canvas;
using System;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private async void ConstructBrush(PaintBrush brush)
        {
            if (string.IsNullOrEmpty(brush.Shape)) this.InkPresenter.ClearShape();
            else this.InkPresenter.ConstructShape(brush.Shape, await CanvasBitmap.LoadAsync(this.CanvasDevice, brush.Shape.GetTextureSource()));

            if (string.IsNullOrEmpty(brush.Grain)) this.InkPresenter.ClearGrain();
            else this.InkPresenter.ConstructGrain(brush.Grain, await CanvasBitmap.LoadAsync(this.CanvasDevice, brush.Grain.GetTextureSource()));

            this.InkPresenter.CopyWith(brush);
            this.InkType = this.InkPresenter.GetType();

            this.IsInkEnabled = false;
            this.ConstructInkSliderValue(this.InkPresenter);
            this.IsInkEnabled = true;

            this.PaintScrollViewer.ConstructInk(this.InkPresenter);
            this.TryInk();
        }

        private void ConstructSize(float size)
        {
            this.InkPresenter.Size = size;

            this.IsInkEnabled = false;
            this.ConstructInkSliderValue(this.InkPresenter);
            this.IsInkEnabled = true;

            this.PaintScrollViewer.ConstructInkSliderValue(this.InkPresenter);
            this.TryInk();
        }


        public void ConstructInkSliderValue(InkPresenter presenter)
        {
            this.IsInkEnabled = false;
            {
            // 2.Value
            this.InkSizeSlider.Value = this.SizeRange.ConvertYToX(presenter.Size);
            this.InkOpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);
            }
            this.IsInkEnabled = true;
        }

        public void ConstructInkSlider(InkPresenter presenter)
        {
            this.IsInkEnabled = false;
            {
            // 1.Minimum
            this.InkSizeSlider.Minimum = this.SizeRange.XRange.Minimum;
            this.InkOpacitySlider.Minimum = 0d;

            // 2.Value
            this.InkSizeSlider.Value = this.SizeRange.ConvertYToX(presenter.Size);
            this.InkOpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);

            // 3.Maximum
            this.InkSizeSlider.Maximum = this.SizeRange.XRange.Maximum;
            this.InkOpacitySlider.Maximum = 100d;
            }
            this.IsInkEnabled = true;
        }

        public void ConstructInk(InkPresenter presenter)
        {
        }


        public void ConstructInk()
        {
            this.ConstructInkSlider(this.InkPresenter); 
            
            this.PaintScrollViewer.Closed += (s, e) => this.ConstructInkSliderValue(this.InkPresenter);

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