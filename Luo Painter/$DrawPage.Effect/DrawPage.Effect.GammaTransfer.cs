using Luo_Painter.UI;
using System;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        float GTAA = 1;
        float GTEA = 1;
        float GTOA = 0;

        float GTAR = 1;
        float GTER = 1;
        float GTOR = 0;

        float GTAG = 1;
        float GTEG = 1;
        float GTOG = 0;

        float GTAB = 1;
        float GTEB = 1;
        float GTOB = 0;

        public void ConstructGammaTransfer()
        {
            this.GTAASlider.ValueChanged += (s, e) =>
            {
                this.GTAA = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTAASlider.Click += (s, e) => this.NumberShowAt(this.GTAASlider, NumberPickerMode.GTAASlider);

            this.GTEASlider.ValueChanged += (s, e) =>
            {
                this.GTEA = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTEASlider.Click += (s, e) => this.NumberShowAt(this.GTEASlider, NumberPickerMode.GTEASlider);

            this.GTOASlider.ValueChanged += (s, e) =>
            {
                this.GTOA = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTOASlider.Click += (s, e) => this.NumberShowAt(this.GTOASlider, NumberPickerMode.GTOASlider);



            this.GTARSlider.ValueChanged += (s, e) =>
            {
                this.GTAR = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTARSlider.Click += (s, e) => this.NumberShowAt(this.GTARSlider, NumberPickerMode.GTARSlider);

            this.GTERSlider.ValueChanged += (s, e) =>
            {
                this.GTER = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTERSlider.Click += (s, e) => this.NumberShowAt(this.GTERSlider, NumberPickerMode.GTERSlider);

            this.GTORSlider.ValueChanged += (s, e) =>
            {
                this.GTOR = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTORSlider.Click += (s, e) => this.NumberShowAt(this.GTORSlider, NumberPickerMode.GTORSlider);



            this.GTAGSlider.ValueChanged += (s, e) =>
            {
                this.GTAG = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTAGSlider.Click += (s, e) => this.NumberShowAt(this.GTAGSlider, NumberPickerMode.GTAGSlider);

            this.GTEGSlider.ValueChanged += (s, e) =>
            {
                this.GTEG = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTEGSlider.Click += (s, e) => this.NumberShowAt(this.GTEGSlider, NumberPickerMode.GTEGSlider);

            this.GTOGSlider.ValueChanged += (s, e) =>
            {
                this.GTOG = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTOGSlider.Click += (s, e) => this.NumberShowAt(this.GTOGSlider, NumberPickerMode.GTOGSlider);



            this.GTABSlider.ValueChanged += (s, e) =>
            {
                this.GTAB = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTABSlider.Click += (s, e) => this.NumberShowAt(this.GTABSlider, NumberPickerMode.GTABSlider);

            this.GTEBSlider.ValueChanged += (s, e) =>
            {
                this.GTEB = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTEBSlider.Click += (s, e) => this.NumberShowAt(this.GTEBSlider, NumberPickerMode.GTEBSlider);

            this.GTOBSlider.ValueChanged += (s, e) =>
            {
                this.GTOB = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTOBSlider.Click += (s, e) => this.NumberShowAt(this.GTOBSlider, NumberPickerMode.GTOBSlider);
        }

        private void SetGammaTransfer(NumberPickerMode mode, float e)
        {
            switch (mode)
            {
                case NumberPickerMode.GTAASlider:
                    this.GTAASlider.Value = e;
                    break;
                case NumberPickerMode.GTEASlider:
                    this.GTEASlider.Value = e;
                    break;
                case NumberPickerMode.GTOASlider:
                    this.GTOASlider.Value = e;
                    break;

                case NumberPickerMode.GTARSlider:
                    this.GTARSlider.Value = e;
                    break;
                case NumberPickerMode.GTERSlider:
                    this.GTERSlider.Value = e;
                    break;
                case NumberPickerMode.GTORSlider:
                    this.GTORSlider.Value = e;
                    break;

                case NumberPickerMode.GTAGSlider:
                    this.GTAGSlider.Value = e;
                    break;
                case NumberPickerMode.GTEGSlider:
                    this.GTEGSlider.Value = e;
                    break;
                case NumberPickerMode.GTOGSlider:
                    this.GTOGSlider.Value = e;
                    break;

                case NumberPickerMode.GTABSlider:
                    this.GTABSlider.Value = e;
                    break;
                case NumberPickerMode.GTEBSlider:
                    this.GTEBSlider.Value = e;
                    break;
                case NumberPickerMode.GTOBSlider:
                    this.GTOBSlider.Value = e;
                    break;

                default:
                    break;
            }
        }
    }
}