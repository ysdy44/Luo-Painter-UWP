using Luo_Painter.UI;
using Windows.System;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    partial class ColorButton
    {

        private void ConstructColorValue()
        {
            this.RGBPicker.ColorChanged += (s, color) =>
            {
                this.OnColorChanged(color, ColorChangedMode.All);
                this.HSVPicker.Recolor(color);
                this.HexPicker.Recolor(color);
            };
            this.HSVPicker.ColorChanged += (s, color) =>
            {
                this.OnColorChanged(color, ColorChangedMode.All);
                this.RGBPicker.Recolor(color);
                this.HexPicker.Recolor(color);
            };

            this.HexPicker.ColorChanged += (s, color) =>
            {
                this.OnColorChanged(color, ColorChangedMode.All);
                this.RGBPicker.Recolor(color);
                this.HSVPicker.Recolor(color);
            };
            this.HexPicker.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case VirtualKey.Enter:
                        this.HexPicker.Ok();
                        break;
                    case VirtualKey.Execute:
                        this.HexPicker.Cancel();
                        break;
                    default:
                        break;
                }

                switch (e.Key)
                {
                    case VirtualKey.Enter:
                    case VirtualKey.Execute:
                        this.ComboBox.Focus(FocusState.Keyboard);
                        break;
                    default:
                        break;
                }
            };
        }

    }
}