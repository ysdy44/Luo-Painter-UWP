using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal enum ColorNumberPickerMode : byte
    {
        None,

        Red,
        Green,
        Blue,
        Alpha,

        Hue,
        Saturation,
        Value,
        Opacity,
    }

    public sealed partial class ColorButton
    {

        ColorNumberPickerMode NumberPickerMode;

        private void NumberShowAt(INumberBase number, ColorNumberPickerMode mode = default)
        {
            this.NumberPickerMode = mode;
            this.NumberFlyout.ShowAt(number.PlacementTarget);
            this.NumberPicker.Construct(number);
        }

        public void ConstructPicker()
        {
            this.RGBPicker.RedClick += (s, e) => this.NumberShowAt(this.RGBPicker.RedNumber, ColorNumberPickerMode.Red);
            this.RGBPicker.GreenClick += (s, e) => this.NumberShowAt(this.RGBPicker.GreenNumber, ColorNumberPickerMode.Green);
            this.RGBPicker.BlueClick += (s, e) => this.NumberShowAt(this.RGBPicker.BlueNumber, ColorNumberPickerMode.Blue);
            this.RGBPicker.AlphaClick += (s, e) => this.NumberShowAt(this.RGBPicker.AlphaNumber, ColorNumberPickerMode.Alpha);

            this.HSVPicker.HueClick += (s, e) => this.NumberShowAt(this.HSVPicker.HueNumber, ColorNumberPickerMode.Hue);
            this.HSVPicker.SaturationClick += (s, e) => this.NumberShowAt(this.HSVPicker.SaturationNumber, ColorNumberPickerMode.Saturation);
            this.HSVPicker.ValueClick += (s, e) => this.NumberShowAt(this.HSVPicker.ValueNumber, ColorNumberPickerMode.Value);
            this.HSVPicker.OpacityClick += (s, e) => this.NumberShowAt(this.HSVPicker.OpacityNumber, ColorNumberPickerMode.Opacity);

            this.NumberFlyout.Closed += (s, e) => this.NumberPicker.Close();
            this.NumberFlyout.Opened += (s, e) => this.NumberPicker.Open();

            this.NumberPicker.SecondaryButtonClick += (s, e) => this.NumberFlyout.Hide();
            this.NumberPicker.PrimaryButtonClick += (s, e) =>
            {
                if (this.NumberFlyout.IsOpen is false) return;
                this.NumberFlyout.Hide();

                switch (this.NumberPickerMode)
                {
                    case ColorNumberPickerMode.None:
                        break;

                    case ColorNumberPickerMode.Red:
                        this.RGBPicker.ResetRed((byte)e);
                        break;
                    case ColorNumberPickerMode.Green:
                        this.RGBPicker.ResetGreen((byte)e);
                        break;
                    case ColorNumberPickerMode.Blue:
                        this.RGBPicker.ResetBlue((byte)e);
                        break;
                    case ColorNumberPickerMode.Alpha:
                        this.RGBPicker.ResetAlpha((byte)e);
                        break;

                    case ColorNumberPickerMode.Hue:
                        this.HSVPicker.ResetHue(e);
                        break;
                    case ColorNumberPickerMode.Saturation:
                        this.HSVPicker.ResetSaturation(e / 100f);
                        break;
                    case ColorNumberPickerMode.Value:
                        this.HSVPicker.ResetValue(e / 100f);
                        break;
                    case ColorNumberPickerMode.Opacity:
                        this.HSVPicker.ResetOpacity(e / 100f);
                        break;

                    default:
                        break;
                }
            };
        }

    }
}