using Luo_Painter.Brushes;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    public sealed partial class PressurePicker : UserControl
    {
        //@Delegate
        public event System.EventHandler<BrushEasePressure> PressureChanged;

        bool IsSetEnabled = true;
        readonly PressurePoints Points = new PressurePoints();
        PointCollection Points0 => this.Points[BrushEasePressure.None];
        PointCollection Points1 => this.Points[BrushEasePressure.Linear];
        PointCollection Points2 => this.Points[BrushEasePressure.Quadratic];
        PointCollection Points3 => this.Points[BrushEasePressure.QuadraticFlipReverse];
        PointCollection Points4 => this.Points[BrushEasePressure.Symmetry];

        //@Construct
        public PressurePicker()
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

                if (this.IsSetEnabled is false) return;

                foreach (PressureItem item in this.Collection)
                {
                    if (item.Index == value)
                    {
                        this.PressureChanged?.Invoke(this, item.Pressure); // Delegate
                        break;
                    }
                }
            };
        }
        public void Construct(BrushEasePressure pressure)
        {
            this.IsSetEnabled = false;
            foreach (PressureItem item in this.Collection)
            {
                if (item.Pressure == pressure)
                {
                    this.ListBox.SelectedIndex = item.Index;
                    break;
                }
            }
            this.IsSetEnabled = true;
        }
    }
}