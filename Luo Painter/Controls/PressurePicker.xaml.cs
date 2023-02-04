using Luo_Painter.Brushes;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal sealed class PressureItem
    {
        public BrushEasePressure Pressure { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }

        public double Line25 { get; set; }
        public double Line50 { get; set; }
        public double Line75 { get; set; }

        public PointCollection Points { get; set; }
    }

    internal sealed class PressureItemList : List<PressureItem> { }

    internal sealed class PressurePoints : Dictionary<BrushEasePressure, PointCollection>
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

    public sealed partial class PressureDialog : ContentDialog
    {
        //@Content
        public BrushEasePressure SelectedItem
        {
            get
            {
                int value = this.ListBox.SelectedIndex;
                foreach (PressureItem item in this.Collection)
                {
                    if (item.Index == value)
                    {
                        return item.Pressure;
                    }
                }
                return default;
            }
        }

        readonly PressurePoints Points = new PressurePoints();
        PointCollection Points0 => this.Points[BrushEasePressure.None];
        PointCollection Points1 => this.Points[BrushEasePressure.Linear];
        PointCollection Points2 => this.Points[BrushEasePressure.Quadratic];
        PointCollection Points3 => this.Points[BrushEasePressure.QuadraticFlipReverse];
        PointCollection Points4 => this.Points[BrushEasePressure.Symmetry];

        //@Construct
        public PressureDialog()
        {
            this.InitializeComponent();
            this.ListBox.SelectionChanged += (s, e) =>
            {
                int value = this.ListBox.SelectedIndex;
                foreach (PressureItem item in this.Collection)
                {
                    if (item.Index == value)
                    {
                        this.Line25.Y2 = item.Line25;
                        this.Line50.Y2 = item.Line50;
                        this.Line75.Y2 = item.Line75;
                        this.Polyline.Points = item.Points;
                        this.TitleTextBlock.Text = item.Title;
                        break;
                    }
                }
            };
        }
        public void Construct(BrushEasePressure pressure)
        {
            foreach (PressureItem item in this.Collection)
            {
                if (item.Pressure == pressure)
                {
                    this.ListBox.SelectedIndex = item.Index;
                    break;
                }
            }
        }
    }
}