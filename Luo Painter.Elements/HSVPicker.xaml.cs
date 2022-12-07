﻿using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public sealed partial class HSVPicker : UserControl, IColorPickerBase
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;
        public event RoutedEventHandler HueClick { remove => this.HueSlider.Click -= value; add => this.HueSlider.Click += value; }
        public event RoutedEventHandler SaturationClick { remove => this.SaturationSlider.Click -= value; add => this.SaturationSlider.Click += value; }
        public event RoutedEventHandler ValueClick { remove => this.ValueSlider.Click -= value; add => this.ValueSlider.Click += value; }

        //@Content
        public INumberBase HueNumber => this.HueSlider;
        public INumberBase SaturationNumber => this.SaturationSlider;
        public INumberBase ValueNumber => this.ValueSlider;
        public FrameworkElement HuePlacementTarget => this.HueSlider.PlacementTarget;
        public FrameworkElement SaturationPlacementTarget => this.SaturationSlider.PlacementTarget;
        public FrameworkElement ValuePlacementTarget => this.ValueSlider.PlacementTarget;

        bool IsSetEnabled = true;

        Vector4 HSV;

        //@Construct
        public HSVPicker()
        {
            this.InitializeComponent();
            this.Recolor(Colors.Black);

            this.HueSlider.ValueChanged += (s, e) =>
            {
                if (this.IsSetEnabled is false) return;

                this.HSV.Z = (float)e.NewValue;

                this.Stop(this.HSV.Z);
                this.Color(this.HSV.ToColor());
            };
            this.SaturationSlider.ValueChanged += (s, e) =>
            {
                if (this.IsSetEnabled is false) return;

                this.HSV.X = (float)(e.NewValue / 100d);

                this.Stop(this.HSV.Z);
                this.Color(this.HSV.ToColor());
            };
            this.ValueSlider.ValueChanged += (s, e) =>
            {
                if (this.IsSetEnabled is false) return;

                this.HSV.Y = (float)(e.NewValue / 100d);

                this.Stop(this.HSV.Z);
                this.Color(this.HSV.ToColor());
            };
        }
    }

    public sealed partial class HSVPicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Stop(this.HSV.Z);

            this.Reset(this.HSV);
        }

        public void ResetHue(float h)
        {
            this.HSV.Z = h;

            this.Stop(this.HSV.Z);
            this.Color(this.HSV.ToColor());

            this.IsSetEnabled = false;
            this.HueSlider.Value = h;
            this.IsSetEnabled = true;
        }
        public void ResetSaturation(float s)
        {
            this.HSV.X = s;

            this.Color(this.HSV.ToColor());

            this.IsSetEnabled = false;
            this.SaturationSlider.Value = s * 100d;
            this.IsSetEnabled = true;
        }
        public void ResetValue(float v)
        {
            this.HSV.Y = v;

            this.Color(this.HSV.ToColor());

            this.IsSetEnabled = false;
            this.ValueSlider.Value = v * 100d;
            this.IsSetEnabled = true;
        }

        private void Reset(Vector4 hsv)
        {
            this.IsSetEnabled = false;
            this.HueSlider.Value = hsv.Z;
            this.SaturationSlider.Value = hsv.X * 100d;
            this.ValueSlider.Value = hsv.Y * 100d;
            this.IsSetEnabled = true;
        }
    }

    public sealed partial class HSVPicker
    {
        private void Color(Color color)
        {
            this.ColorChanged?.Invoke(this, color); // Delegate
        }

        private void Stop(float h)
        {
            this.SaturationEndStop.Color = HSVExtensions.ToColor(h);
        }
    }
}