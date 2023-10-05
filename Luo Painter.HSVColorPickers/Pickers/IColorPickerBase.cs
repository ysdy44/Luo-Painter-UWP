using System;
using Windows.UI;

namespace Luo_Painter.HSVColorPickers
{
    public interface IColorPickerBase
    {
        //@Delegate
        event EventHandler<Color> ColorChanged;

        ColorType Type { get; }

        void Recolor(Color color);
    }
}