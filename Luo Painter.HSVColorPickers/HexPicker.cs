using System;
using System.Globalization;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.HSVColorPickers
{
    public sealed class HexPicker : TextBox, IColorPickerBase
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;

        public ColorType Type => ColorType.Hex;
        public string Hex { get; private set; } = "000000";

        //@Construct
        public HexPicker()
        {
            base.LostFocus += (s, e) => this.Ok();
            base.GotFocus += (s, e) =>
            {
                base.Text = this.Hex;
                base.SelectAll();
            };
            base.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                        base.RemoveFocusEngagement();
                        break;
                    default:
                        break;
                }
            };
        }

        public void Recolor(Color color) => base.Text = this.Hex = color.ToHex();

        public void Cancel() => base.Text = this.Hex;
        public void Ok()
        {
            string text = base.Text.ToUpper();
            if (this.Hex == text) return;

            if (HSVExtensions.ToColor(text) is Color color)
            {
                this.Hex = text;
                this.ColorChanged?.Invoke(this, color); // Delegate
            }

            base.Text = this.Hex;
        }
    }
}