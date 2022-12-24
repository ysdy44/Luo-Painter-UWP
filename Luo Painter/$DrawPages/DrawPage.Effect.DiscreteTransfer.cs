using System;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public float[] AlphaTable = new float[] { 0, 0.5f, 1 };

        public float[] RedTable = new float[] { 0, 0.5f, 1 };

        public float[] GreenTable = new float[] { 0, 0.5f, 1 };

        public float[] BlueTable = new float[] { 0, 0.5f, 1 };

        public void ConstructDiscreteTransfer()
        {
            this.DTASlider.ValueChanged += (s, e) =>
            {
                this.AlphaTable[1] = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.DTASlider.Click += (s, e) => this.NumberShowAt(this.DTASlider, NumberPickerMode.Case0);

            this.DTRSlider.ValueChanged += (s, e) =>
            {
                this.RedTable[1] = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.DTRSlider.Click += (s, e) => this.NumberShowAt(this.DTRSlider, NumberPickerMode.Case1);

            this.DTGSlider.ValueChanged += (s, e) =>
            {
                this.GreenTable[1] = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.DTGSlider.Click += (s, e) => this.NumberShowAt(this.DTGSlider, NumberPickerMode.Case2);

            this.DTBSlider.ValueChanged += (s, e) =>
            {
                this.BlueTable[1] = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.DTBSlider.Click += (s, e) => this.NumberShowAt(this.DTBSlider, NumberPickerMode.Case3);
        }

        private void SetDiscreteTransfer(NumberPickerMode mode, int e)
        {
            switch (mode)
            {
                case NumberPickerMode.Case0:
                    this.DTASlider.Value = e;
                    break;

                case NumberPickerMode.Case1:
                    this.DTRSlider.Value = e;
                    break;

                case NumberPickerMode.Case2:
                    this.DTGSlider.Value = e;
                    break;

                case NumberPickerMode.Case3:
                    this.DTBSlider.Value = e;
                    break;

                default:
                    break;
            }
        }
    }
}