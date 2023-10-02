using Luo_Painter.Brushes;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.UI
{
    public sealed class PressureItem
    {
        public BrushEasePressure Pressure { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }

        public double Line25 { get; set; }
        public double Line50 { get; set; }
        public double Line75 { get; set; }

        public PointCollection Points { get; set; }
    }
}