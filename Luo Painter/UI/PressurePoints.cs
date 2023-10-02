using Luo_Painter.Brushes;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    public sealed class PressurePoints : Dictionary<BrushEasePressure, PointCollection>
    {
        public PressurePoints()
        {
            this[BrushEasePressure.None] = new PointCollection { new Point(0, 20), new Point(0, 0), new Point(20, 0), new Point(20, 20) };
            this[BrushEasePressure.Linear] = new PointCollection { new Point(0, 20), new Point(20, 0), new Point(20, 20) };
            this[BrushEasePressure.Quadratic] = new PointCollection { new Point(0, 20), new Point(2.618, 19.602), new Point(5.142, 18.633), new Point(7.756, 16.896), new Point(10, 14.94), new Point(12.712, 11.831), new Point(15.089, 8.649), new Point(17.576, 4.553), new Point(20.018, 0), new Point(20.018, 20) };
            this[BrushEasePressure.QuadraticFlipReverse] = new PointCollection { new Point(0, 20), new Point(2.326, 15.452), new Point(5.087, 11.1), new Point(7.427, 8.028), new Point(10.232, 4.846), new Point(12.895, 2.432), new Point(15.272, 1.152), new Point(17.612, 0.311), new Point(20.018, 0), new Point(20.018, 20) };
            this[BrushEasePressure.Symmetry] = new PointCollection { new Point(0, 20), new Point(2.618, 19.31), new Point(5.16, 17.244), new Point(7.628, 14.226), new Point(10.009, 10), new Point(12.529, 5.614), new Point(15.18, 2.341), new Point(17.576, 0.622), new Point(20.018, 0), new Point(20, 20) };
        }
    }
}