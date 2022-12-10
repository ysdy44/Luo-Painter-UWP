using Windows.System;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton
    {

        private void ConstructColorValue()
        {
            this.RGBPicker.ColorChanged += (s, color) =>
            {
                this.Color3(color);
                this.HexPicker.Recolor(color);
            };
            this.HSVPicker.ColorChanged += (s, color) =>
            {
                this.Color3(color);
                this.HexPicker.Recolor(color);
            };

            this.HexPicker.ColorChanged += (s, color) =>
            {
                this.Color3(color);
                if (this.RGBPicker.Visibility == default) this.RGBPicker.Recolor(color);
                if (this.HSVPicker.Visibility == default) this.HSVPicker.Recolor(color);
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
                        if (this.RGBPicker.Visibility == default) this.RGBPicker.Focus(FocusState.Keyboard);
                        if (this.HSVPicker.Visibility == default) this.HSVPicker.Focus(FocusState.Keyboard);
                        break;
                    default:
                        break;
                }
            };
        }

    }
}