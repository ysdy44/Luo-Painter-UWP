using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public sealed partial class RGBPicker : UserControl, IColorPickerBase
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;
        public event RoutedEventHandler RedClick { remove => this.RedSlider.Click -= value; add => this.RedSlider.Click += value; }
        public event RoutedEventHandler GreenClick { remove => this.GreenSlider.Click -= value; add => this.GreenSlider.Click += value; }
        public event RoutedEventHandler BlueClick { remove => this.BlueSlider.Click -= value; add => this.BlueSlider.Click += value; }
        public event RoutedEventHandler AlphaClick { remove => this.AlphaSlider.Click -= value; add => this.AlphaSlider.Click += value; }

        //@Content
        public INumberBase RedNumber => this.RedSlider;
        public INumberBase GreenNumber => this.GreenSlider;
        public INumberBase BlueNumber => this.BlueSlider;
        public INumberBase AlphaNumber => this.AlphaSlider;
        public object RedHeader { get => this.RedSlider.Header; set => this.RedSlider.Header = value; }
        public object GreenHeader { get => this.GreenSlider.Header; set => this.GreenSlider.Header = value; }
        public object BlueHeader { get => this.BlueSlider.Header; set => this.BlueSlider.Header = value; }
        public object AlphaHeader { get => this.AlphaSlider.Header; set => this.AlphaSlider.Header = value; }

        bool IsSetEnabled = true;

        Color RGB = Colors.Black;

        //@Construct
        public RGBPicker()
        {
            this.InitializeComponent();
            this.Recolor(Colors.Black);

            this.RedSlider.ValueChanged += (s, e) =>
            {
                if (this.IsSetEnabled is false) return;

                this.RGB.R = (byte)e.NewValue;

                this.Stop(this.RGB);
                this.Color(this.RGB);
            };
            this.GreenSlider.ValueChanged += (s, e) =>
            {
                if (this.IsSetEnabled is false) return;

                this.RGB.G = (byte)e.NewValue;

                this.Stop(this.RGB);
                this.Color(this.RGB);
            };
            this.BlueSlider.ValueChanged += (s, e) =>
            {
                if (this.IsSetEnabled is false) return;

                this.RGB.B = (byte)e.NewValue;

                this.Stop(this.RGB);
                this.Color(this.RGB);
            };
            this.AlphaSlider.ValueChanged += (s, e) =>
            {
                if (this.IsSetEnabled is false) return;

                this.RGB.A = (byte)e.NewValue;

                this.Color(this.RGB);
            };
        }
    }

    public sealed partial class RGBPicker
    {
        public void Recolor(Color color)
        {
            this.RGB = color;

            this.Stop(this.RGB);

            this.ResetRGB(this.RGB);
        }

        public void ResetRed(byte r)
        {
            this.RGB.R = r;

            this.Stop(this.RGB);
            this.Color(this.RGB);

            this.IsSetEnabled = false;
            this.RedSlider.Value = r;
            this.IsSetEnabled = true;
        }
        public void ResetGreen(byte g)
        {
            this.RGB.G = g;

            this.Stop(this.RGB);
            this.Color(this.RGB);

            this.IsSetEnabled = false;
            this.GreenSlider.Value = g;
            this.IsSetEnabled = true;
        }
        public void ResetBlue(byte b)
        {
            this.RGB.B = b;

            this.Stop(this.RGB);
            this.Color(this.RGB);

            this.IsSetEnabled = false;
            this.BlueSlider.Value = b;
            this.IsSetEnabled = true;
        }
        public void ResetAlpha(byte a)
        {
            this.RGB.A = a;

            this.Color(this.RGB);

            this.IsSetEnabled = false;
            this.AlphaSlider.Value = a;
            this.IsSetEnabled = true;
        }

        private void ResetRGB(Color rgb)
        {
            this.IsSetEnabled = false;
            this.RedSlider.Value = rgb.R;
            this.GreenSlider.Value = rgb.G;
            this.BlueSlider.Value = rgb.B;
            this.AlphaSlider.Value = rgb.A;
            this.IsSetEnabled = true;
        }
    }

    public sealed partial class RGBPicker
    {
        private void Color(Color color)
        {
            this.ColorChanged?.Invoke(this, color); // Delegate
        }

        private void Stop(Color color)
        {
            this.RedStartStop.Color = Windows.UI.Color.FromArgb(color.A, byte.MinValue, color.G, color.B);
            this.RedEndStop.Color = Windows.UI.Color.FromArgb(color.A, byte.MaxValue, color.G, color.B);

            this.GreenStartStop.Color = Windows.UI.Color.FromArgb(color.A, color.R, byte.MinValue, color.B);
            this.GreenEndStop.Color = Windows.UI.Color.FromArgb(color.A, color.R, byte.MaxValue, color.B);

            this.BlueStartStop.Color = Windows.UI.Color.FromArgb(color.A, color.R, color.G, byte.MinValue);
            this.BlueEndStop.Color = Windows.UI.Color.FromArgb(color.A, color.R, color.G, byte.MaxValue);
        }
    }
}