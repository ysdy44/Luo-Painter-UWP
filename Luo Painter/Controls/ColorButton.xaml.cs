using Luo_Painter.Brushes;
using Luo_Painter.HSVColorPickers;
using System;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    [Flags]
    public enum ColorChangedMode
    {
        WithPrimaryBrush = 1,
        WithSecondaryBrush = 2,
        WithColor = 4,
        All = WithPrimaryBrush | WithSecondaryBrush | WithColor
    }

    public sealed partial class ColorButton : EyedropperButton, IColorHdrBase, IColorBase
    {
        //@Converter
        private HarmonyMode ModeConverter(int value)
        {
            switch (value)
            {
                case 0: return HarmonyMode.Complementary;
                case 1: return HarmonyMode.SplitComplementary;
                case 2: return HarmonyMode.Analogous;
                case 3: return HarmonyMode.Triadic;
                case 4: return HarmonyMode.Tetradic;
                default: return default;
            }
        }
        private int IndexConverter(int value)
        {
            switch (value)
            {
                case 0: case 1: return 0;
                case 2: return 1;
                case 3: return 2;
                case 4: return 3;
                case 5: return 4;
                default: return default;
            }
        }

        //@Delegate
        public event EventHandler<Color> ColorChanged;

        //@Content
        public FrameworkElement PlacementTarget => this;
        public ICommand OpenCommand => this;

        public ObservableCollection<Color> ObservableCollection { get; } = new ObservableCollection<Color>();

        public Color Color { get; private set; } = Colors.Black;
        public Vector4 ColorHdr { get; private set; } = Vector4.UnitW;

        public void Construct(IInkParameter item)
        {
        }

        //@Construct
        public ColorButton()
        {
            this.InitializeComponent();
            this.ConstructPicker();

            this.ConstructColor();
            this.ConstructColorHarmony();
            this.ConstructColorValue();

            this.ComboBox.SelectionChanged += (s, e) => this.Recolor(this.Color);
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void SetColor(Color color) => this.Color = color;
        public void SetColorHdr(Vector4 colorHdr) => this.ColorHdr = colorHdr;

        public void SetColor(Vector4 colorHdr) => this.Color = Color.FromArgb((byte)(colorHdr.W * 255f), (byte)(colorHdr.X * 255f), (byte)(colorHdr.Y * 255f), (byte)(colorHdr.Z * 255f));
        public void SetColorHdr(Color color) => this.ColorHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f; // 0~1

        //@Override
        public override void Recolor(Color color)
        {
            if (this.TricolorPicker.Visibility == default) this.TricolorPicker.Recolor(color);
            if (this.HuePicker.Visibility == default) this.HuePicker.Recolor(color);
            if (this.ValuePicker.Visibility == default)
            {
                this.RGBPicker.Recolor(color);
                this.HSVPicker.Recolor(color);
                this.HexPicker.Recolor(color);
            }
        }
        public override void OnColorChanged(Color color) => this.OnColorChanged(color, ColorChangedMode.All);
        public void OnColorChanged(Color color, ColorChangedMode mode)
        {
            if (mode.HasFlag(ColorChangedMode.WithPrimaryBrush))
            {
                this.PrimarySolidColorBrush.Color = color;
                this.SolidColorBrush.Color = color;
            }
            if (mode.HasFlag(ColorChangedMode.WithSecondaryBrush))
            {
                this.SecondarySolidColorBrush.Color = color;
            }
            if (mode.HasFlag(ColorChangedMode.WithColor))
            {
                this.SetColor(color);
                this.SetColorHdr(color);
                this.ColorChanged?.Invoke(this, color); // Delegate
            }
        }

    }
}