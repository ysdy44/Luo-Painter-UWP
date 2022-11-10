using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        float Threshold = 1.5f;
        Vector4 ThresholdColor0 = Vector4.One;
        Vector4 ThresholdColor1 = Vector4.UnitW;
        bool ThresholdIsColor1;

        public void ConstructThreshold()
        {
            this.ThresholdSlider.ValueChanged += (s, e) =>
            {
                this.Threshold = (float)System.Math.Clamp((e.NewValue + 150) / 100, 0, 3);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.ThresholdColor0Button.Click += (s, e) =>
            {
                this.ThresholdIsColor1 = false;
                this.ColorPicker.Color = Color.FromArgb((byte)(this.ThresholdColor0.W * 255f), (byte)(this.ThresholdColor0.X * 255f), (byte)(this.ThresholdColor0.Y * 255f), (byte)(this.ThresholdColor0.Z * 255f));
                base.ContextFlyout.ShowAt(this.ThresholdColor0Button);
            };
            this.ThresholdColor1Button.Click += (s, e) =>
            {
                this.ThresholdIsColor1 = true;
                this.ColorPicker.Color = Color.FromArgb((byte)(this.ThresholdColor1.W * 255f), (byte)(this.ThresholdColor1.X * 255f), (byte)(this.ThresholdColor1.Y * 255f), (byte)(this.ThresholdColor1.Z * 255f));
                base.ContextFlyout.ShowAt(this.ThresholdColor1Button);
            };

            this.ThresholdReverseButton.Click += (s, e) =>
            {
                Color color = this.ThresholdColor0Brush.Color;
                this.ThresholdColor0Brush.Color = this.ThresholdColor1Brush.Color;
                this.ThresholdColor1Brush.Color = color;

                Vector4 thresholdColor = this.ThresholdColor0;
                this.ThresholdColor0 = this.ThresholdColor1;
                this.ThresholdColor1 = thresholdColor;

                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

        private void ThresholdColorChanged(Color color)
        {
            if (this.ThresholdIsColor1)
            {
                this.ThresholdColor1Brush.Color = color;
                this.ThresholdColor1 = new Vector4(color.R, color.G, color.B, color.A) / 255f; // 0~1
            }
            else
            {
                this.ThresholdColor0Brush.Color = color;
                this.ThresholdColor0 = new Vector4(color.R, color.G, color.B, color.A) / 255f; // 0~1
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

    }
}