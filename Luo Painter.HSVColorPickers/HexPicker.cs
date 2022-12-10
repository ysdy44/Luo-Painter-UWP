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

        public void Recolor(Color color)
        {
            string r = color.R.ToString("x2");
            string g = color.G.ToString("x2");
            string b = color.B.ToString("x2");

            this.Hex = $"{r}{g}{b}".ToUpper();
            base.Text = this.Hex;
        }

        public void Cancel() => base.Text = this.Hex;
        public void Ok()
        {
            string text = base.Text.ToUpper();
            base.Text = text;

            if (this.Hex == text) return;
            this.Hex = text;

            if (this.Color(text)) return;
            base.Text = this.Hex;
        }

        private bool Color(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            int length = text.Length;
            if (length < 6) return false;
            else if (length > 6) text = text.Substring(length - 6, 6);

            int hexNumber;
            try
            {
                hexNumber = int.Parse(text, NumberStyles.HexNumber);
            }
            catch (Exception)
            {
                return false;
            }

            int r = (hexNumber >> 16) & 255;
            int g = (hexNumber >> 8) & 255;
            int b = (hexNumber >> 0) & 255;

            Color color = Windows.UI.Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
            this.ColorChanged?.Invoke(this, color); // Delegate
            return true;
        }
    }
}