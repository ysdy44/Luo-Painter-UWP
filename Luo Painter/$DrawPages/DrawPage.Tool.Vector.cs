using Luo_Painter.Elements;
using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {
        private void SetView()
        {
            float radian = this.Transformer.Radian;
            this.RadianSlider.Value = radian * 180 / System.Math.PI;

            float scale = this.Transformer.Scale;
            this.ScaleSlider.Value = this.ScaleRange.InverseProportion.ConvertYToX(scale);
        }

        private void ConstructVector()
        {
            this.RadianStoryboard.Completed += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.RadianClearButton.Tapped += (s, e) => this.RadianStoryboard.Begin(); // Storyboard
            this.RadianSlider.ValueChanged += (s, e) =>
            {
                double radian = e.NewValue / 180 * System.Math.PI;
                this.Transformer.Radian = (float)radian;
                this.Transformer.ReloadMatrix();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.ScaleStoryboard.Completed += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.ScaleClearButton.Tapped += (s, e) => this.ScaleStoryboard.Begin(); // Storyboard
            this.ScaleSlider.ValueChanged += (s, e) =>
            {
                double scale = this.ScaleRange.InverseProportion.ConvertXToY(e.NewValue);
                this.Transformer.Scale = (float)scale;
                this.Transformer.ReloadMatrix();
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.RemoteControl.Moved += (s, vector) =>
            {
                this.Transformer.Position += vector * 20;
                this.Transformer.ReloadMatrix();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.RemoteControl.ValueChangeStarted += (s, value) => this.Transformer.CacheMove(Vector2.Zero);
            this.RemoteControl.ValueChangeDelta += (s, value) =>
            {
                this.Transformer.Move(value);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.RemoteControl.ValueChangeCompleted += (s, value) =>
            {
                this.Transformer.Move(value);
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void View_Start(Vector2 point)
        {
            this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void View_Delta(Vector2 point)
        {
            this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void View_Complete(Vector2 point)
        {
            this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
            this.CanvasControl.Invalidate(); // Invalidate
        }

    }
}