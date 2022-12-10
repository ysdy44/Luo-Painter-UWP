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

    public interface IColorPicker : IColorPickerBase
    {
        //@Delegate
        event EventHandler<Color> ColorChangedCompleted;

        void Left();
        void Right();

        void Down();
        void Up();

        void ZoomOut();
        void ZoomIn();
    }
}