using System;
using Windows.UI;

namespace Luo_Painter.HSVColorPickers
{

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