﻿using Luo_Painter.HSVColorPickers;
using Luo_Painter.UI;
using Microsoft.Graphics.Canvas.Effects;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {
        float HSBHue = 0;
        float HSBSaturation = 1;
        float HSBBrightness = 0;

        Matrix5x4 HSBMatrix = Matrix5x4Extension.Identity;

        public void ConstructHSB()
        {
            this.HSBHueSlider.Click += (s, e) => this.NumberShowAt(this.HSBHueSlider, NumberPickerMode.HSBHueSlider);
            this.HSBHueSlider.ValueChanged += (s, e) =>
            {
                this.HSBHue = (float)(e.NewValue / 120d);
                this.HSBMatrix = Matrix5x4Extension.HSB(this.HSBHue, this.HSBSaturation, this.HSBBrightness);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.HSBSaturationSlider.Click += (s, e) => this.NumberShowAt(this.HSBSaturationSlider, NumberPickerMode.HSBSaturationSlider);
            this.HSBSaturationSlider.ValueChanged += (s, e) =>
            {
                this.HSBSaturation = (float)(e.NewValue / 100d);
                this.HSBMatrix = Matrix5x4Extension.HSB(this.HSBHue, this.HSBSaturation, this.HSBBrightness);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.HSBBrightnessSlider.Click += (s, e) => this.NumberShowAt(this.HSBBrightnessSlider, NumberPickerMode.HSBBrightnessSlider);
            this.HSBBrightnessSlider.ValueChanged += (s, e) =>
            {
                this.HSBBrightness = (float)(e.NewValue / 100d);
                this.HSBMatrix = Matrix5x4Extension.HSB(this.HSBHue, this.HSBSaturation, this.HSBBrightness);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

    }
}