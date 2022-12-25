using Luo_Painter.HSVColorPickers;
using System;
using System.Numerics;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorPicker2 : UserControl, IColorBase
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;
        public event RoutedEventHandler StrawClick { remove => this.StrawButton.Click -= value; add => this.StrawButton.Click += value; }

        //@Content
        public FrameworkElement PlacementTarget => this;

        public Color Color { get; private set; } = Colors.Black;

        //@Construct
        public ColorPicker2()
        {
            this.InitializeComponent();
            this.ComboBox.SelectionChanged += (s, e) => this.Recolor(this.Color);

            this.HuePicker.ColorChanged += (s, color) => this.OnColorChanged(color);

            this.RGBPicker.ColorChanged += (s, color) => this.OnColorChanged(color);
            this.HSVPicker.ColorChanged += (s, color) => this.OnColorChanged(color);

            this.ConstructPicker();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void SetColor(Color color) => this.Color = color;
        public void SetColor(Vector4 colorHdr) => this.Color = Color.FromArgb((byte)(colorHdr.W * 255f), (byte)(colorHdr.X * 255f), (byte)(colorHdr.Y * 255f), (byte)(colorHdr.Z * 255f));

        public void OnColorChanged(Color color)
        {
            this.SetColor(color);
            this.ColorChanged?.Invoke(this, color); // Delegate
        }
        public void Recolor(Color color)
        {
            if (this.HuePicker.Visibility == default) this.HuePicker.Recolor(color);

            if (this.RGBPicker.Visibility == default) this.RGBPicker.Recolor(color);
            if (this.HSVPicker.Visibility == default) this.HSVPicker.Recolor(color);
        }

    }
}