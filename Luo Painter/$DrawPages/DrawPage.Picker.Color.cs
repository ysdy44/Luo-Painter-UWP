using Luo_Painter.Options;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal enum ColorPickerMode : byte
    {
        None,

        Case0,
        Case1,
        Case2,
        Case3,
        Case4,
        Case5,
        Case6,
        Case7,
    }

    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        ColorPickerMode ColorPickerMode;

        private void ColorShowAt(IColorBase color, ColorPickerMode mode = default)
        {
            this.ColorPickerMode = mode;
            this.ColorPicker.Color = color.Color;
            this.ColorFlyout.ShowAt(color.PlacementTarget);
        }

    }
}