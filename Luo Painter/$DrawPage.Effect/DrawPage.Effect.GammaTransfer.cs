﻿using Luo_Painter.UI;
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
            this.GTAASlider.Click += (s, e) => this.NumberShowAt(this.GTAASlider, (NumberPickerMode)0);

            this.GTEASlider.ValueChanged += (s, e) =>
            {
                this.GTEA = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTEASlider.Click += (s, e) => this.NumberShowAt(this.GTEASlider, (NumberPickerMode)1);

            this.GTOASlider.ValueChanged += (s, e) =>
            {
                this.GTOA = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTOASlider.Click += (s, e) => this.NumberShowAt(this.GTOASlider, (NumberPickerMode)2);



            this.GTARSlider.ValueChanged += (s, e) =>
            {
                this.GTAR = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTARSlider.Click += (s, e) => this.NumberShowAt(this.GTARSlider, (NumberPickerMode)3);

            this.GTERSlider.ValueChanged += (s, e) =>
            {
                this.GTER = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTERSlider.Click += (s, e) => this.NumberShowAt(this.GTERSlider, (NumberPickerMode)4);

            this.GTORSlider.ValueChanged += (s, e) =>
            {
                this.GTOR = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTORSlider.Click += (s, e) => this.NumberShowAt(this.GTORSlider, (NumberPickerMode)5);



            this.GTAGSlider.ValueChanged += (s, e) =>
            {
                this.GTAG = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTAGSlider.Click += (s, e) => this.NumberShowAt(this.GTAGSlider, (NumberPickerMode)6);

            this.GTEGSlider.ValueChanged += (s, e) =>
            {
                this.GTEG = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTEGSlider.Click += (s, e) => this.NumberShowAt(this.GTEGSlider, (NumberPickerMode)7);

            this.GTOGSlider.ValueChanged += (s, e) =>
            {
                this.GTOG = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTOGSlider.Click += (s, e) => this.NumberShowAt(this.GTOGSlider, (NumberPickerMode)8);



            this.GTABSlider.ValueChanged += (s, e) =>
            {
                this.GTAB = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTABSlider.Click += (s, e) => this.NumberShowAt(this.GTABSlider, (NumberPickerMode)9);

            this.GTEBSlider.ValueChanged += (s, e) =>
            {
                this.GTEB = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTEBSlider.Click += (s, e) => this.NumberShowAt(this.GTEBSlider, (NumberPickerMode)10);

            this.GTOBSlider.ValueChanged += (s, e) =>
            {
                this.GTOB = (float)Math.Clamp(e.NewValue / 100, 0, 1);
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.GTOBSlider.Click += (s, e) => this.NumberShowAt(this.GTOBSlider, (NumberPickerMode)11);
        }

        private void SetGammaTransfer(NumberPickerMode mode, float e)
        {
            switch ((int)mode)
            {
                case 0:
                    this.GTAASlider.Value = e;
                    break;
                case 1:
                    this.GTEASlider.Value = e;
                    break;
                case 2:
                    this.GTOASlider.Value = e;
                    break;

                case 3:
                    this.GTARSlider.Value = e;
                    break;
                case 4:
                    this.GTERSlider.Value = e;
                    break;
                case 5:
                    this.GTORSlider.Value = e;
                    break;

                case 6:
                    this.GTAGSlider.Value = e;
                    break;
                case 7:
                    this.GTEGSlider.Value = e;
                    break;
                case 8:
                    this.GTOGSlider.Value = e;
                    break;

                case 9:
                    this.GTABSlider.Value = e;
                    break;
                case 10:
                    this.GTEBSlider.Value = e;
                    break;
                case 11:
                    this.GTOBSlider.Value = e;
                    break;

                default:
                    break;
            }
        }
    }
}