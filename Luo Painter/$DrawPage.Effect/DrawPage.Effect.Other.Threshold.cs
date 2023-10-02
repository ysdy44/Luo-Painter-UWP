using Luo_Painter.UI;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        float Threshold = 1.5f;
        Vector4 ThresholdColor0 => this.ThresholdColor0Button.ColorHdr;
        Vector4 ThresholdColor1 => this.ThresholdColor1Button.ColorHdr;

        public void ConstructThreshold()
        {
            this.ThresholdSlider.Click += (s, e) => this.NumberShowAt(this.ThresholdSlider);
            this.ThresholdSlider.ValueChanged += (s, e) =>
            {
                this.Threshold = (float)System.Math.Clamp((e.NewValue + 150) / 100, 0, 3);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.ThresholdColor0Button.SetColor(Colors.White);
            this.ThresholdColor0Button.SetColorHdr(Vector4.One);
            this.ThresholdColor0Button.Click += (s, e) => this.ColorShowAt(this.ThresholdColor0Button, ColorPickerMode.Case0);

            this.ThresholdColor1Button.SetColor(Colors.Black);
            this.ThresholdColor1Button.SetColorHdr(Vector4.UnitW);
            this.ThresholdColor1Button.Click += (s, e) => this.ColorShowAt(this.ThresholdColor1Button, ColorPickerMode.Case1);

            this.ThresholdReverseButton.Click += (s, e) =>
            {
                this.ThresholdColor0Button.Reverse(this.ThresholdColor1Button);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

    }
}