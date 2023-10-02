using Luo_Painter.HSVColorPickers;
using Luo_Painter.UI;
using Windows.UI;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton
    {

        private void ConstructColorHarmony()
        {
            this.EllipseItem.Click += (s, e) => this.OnColorChanged(this.EllipseBrush.Color, ColorChangedMode.All);
            this.Ellipse1Item.Click += (s, e) => this.OnColorChanged(this.Ellipse1Brush.Color, ColorChangedMode.All);
            this.Ellipse2Item.Click += (s, e) => this.OnColorChanged(this.Ellipse2Brush.Color, ColorChangedMode.All);
            this.Ellipse3Item.Click += (s, e) => this.OnColorChanged(this.Ellipse3Brush.Color, ColorChangedMode.All);

            this.HarmonyPicker.ColorChanged += (s, e) => this.EllipseBrush.Color = e;
            this.HarmonyPicker.Color1Changed += (s, e) => this.Ellipse1Brush.Color = e;
            this.HarmonyPicker.Color2Changed += (s, e) => this.Ellipse2Brush.Color = e;
            this.HarmonyPicker.Color3Changed += (s, e) => this.Ellipse3Brush.Color = e;

            this.EllipseBrush.Color = Colors.DodgerBlue;
            this.HarmonyPicker.Recolor(Colors.DodgerBlue);
            this.HarmonyPicker.Remode(HarmonyMode.Triadic);

            this.ModeListView.SelectionChanged += (s, e) =>
            {
                HarmonyMode mode = this.ModeConverter(this.ModeListView.SelectedIndex);
                this.HarmonyPicker.Remode(mode);

                this.Ellipse1Item.Visibility = mode.HasFlag(HarmonyMode.HasPoint1) ? Visibility.Visible : Visibility.Collapsed;
                this.Ellipse2Item.Visibility = mode.HasFlag(HarmonyMode.HasPoint2) ? Visibility.Visible : Visibility.Collapsed;
                this.Ellipse3Item.Visibility = mode.HasFlag(HarmonyMode.HasPoint3) ? Visibility.Visible : Visibility.Collapsed;
            };
        }

    }
}