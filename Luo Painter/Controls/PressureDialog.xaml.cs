using Luo_Painter.Brushes;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    public sealed partial class PressurePanel : UserControl
    {
        readonly IDictionary<BrushEasePressure, PointCollection> Points = new Dictionary<BrushEasePressure, PointCollection>
        {
            [BrushEasePressure.None] = new PointCollection { new Point(0, 120), new Point(0, 0), new Point(120, 0), new Point(120, 120) },
            [BrushEasePressure.Linear] = new PointCollection { new Point(0, 120), new Point(120, 0), new Point(120, 120) },
            [BrushEasePressure.Quadratic] = new PointCollection { new Point(0, 120), new Point(0, 120), new Point(7.5, 119.53125), new Point(15, 118.125), new Point(22.5, 115.78125), new Point(30, 112.5), new Point(37.5, 108.28125), new Point(45, 103.125), new Point(52.5, 97.03125), new Point(60, 90), new Point(67.5, 82.03125), new Point(75, 73.125), new Point(82.5, 63.28125), new Point(90, 52.5), new Point(97.5, 40.78125), new Point(105, 28.125), new Point(112.5, 14.53125), new Point(120, 0), new Point(120, 120) },
            [BrushEasePressure.QuadraticFlipReverse] = new PointCollection { new Point(0, 120), new Point(0, 120), new Point(7.5, 105.46875), new Point(15, 91.875), new Point(22.5, 79.21875), new Point(30, 67.5), new Point(37.5, 56.71875), new Point(45, 46.875), new Point(52.5, 37.96875), new Point(60, 30), new Point(67.5, 22.96875), new Point(75, 16.875), new Point(82.5, 11.71875), new Point(90, 7.5), new Point(97.5, 4.21875), new Point(105, 1.875), new Point(112.5, 0.46875), new Point(120, 0), new Point(120, 120) },
            [BrushEasePressure.Symmetry] = new PointCollection { new Point(0, 120), new Point(0, 120), new Point(7.5, 119.0625), new Point(15, 116.25), new Point(22.5, 111.5625), new Point(30, 105), new Point(37.5, 96.5625), new Point(45, 86.25), new Point(52.5, 74.0625), new Point(60, 60), new Point(67.5, 45.9375), new Point(75, 33.75), new Point(82.5, 23.4375), new Point(90, 15), new Point(97.5, 8.4375), new Point(105, 3.75), new Point(112.5, 0.9375), new Point(120, 0), new Point(120, 120) },
        };
        readonly IDictionary<BrushEasePressure, Vector3> Lines = new Dictionary<BrushEasePressure, Vector3>
        {
            [BrushEasePressure.None] = new Vector3(0, 0, 0),
            [BrushEasePressure.Linear] = new Vector3(90, 60, 30),
            [BrushEasePressure.Quadratic] = new Vector3(112.5f, 90, 52.5f),
            [BrushEasePressure.QuadraticFlipReverse] = new Vector3(67.5f, 30, 7.5f),
            [BrushEasePressure.Symmetry] = new Vector3(105, 60, 15),
        };
        public PressurePanel()
        {
            this.InitializeComponent();
        }
        public void Update(BrushEasePressure pressure)
        {
            Line25.Y1 = this.Lines[pressure].X;
            Line50.Y1 = this.Lines[pressure].Y;
            Line75.Y1 = this.Lines[pressure].Z;
            Polyline.Points = this.Points[pressure];
        }
    }
}