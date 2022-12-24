using Luo_Painter.HSVColorPickers;

namespace Luo_Painter.Controls
{
    internal enum ColorNumberPickerMode : byte
    {
        None,

        Red,
        Green,
        Blue,

        Hue,
        Saturation,
        Value,
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

            this.HSVPicker.HueClick += (s, e) => this.NumberShowAt(this.HSVPicker.HueNumber, ColorNumberPickerMode.Hue);
            this.HSVPicker.SaturationClick += (s, e) => this.NumberShowAt(this.HSVPicker.SaturationNumber, ColorNumberPickerMode.Saturation);
            this.HSVPicker.ValueClick += (s, e) => this.NumberShowAt(this.HSVPicker.ValueNumber, ColorNumberPickerMode.Value);

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

                    case ColorNumberPickerMode.Hue:
                        this.HSVPicker.ResetHue(e);
                        break;
                    case ColorNumberPickerMode.Saturation:
                        this.HSVPicker.ResetSaturation(e / 100f);
                        break;
                    case ColorNumberPickerMode.Value:
                        this.HSVPicker.ResetValue(e / 100f);
                        break;

                    default:
                        break;
                }
            };
        }

    }
}