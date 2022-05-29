using Luo_Painter.Brushes;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.TestApp
{
    public sealed partial class BrushEdgeHardnessPage : Page
    {
        public BrushEdgeHardnessPage()
        {
            this.InitializeComponent();
            this.ListView.ItemsSource = Enum.GetValues(typeof(BrushEdgeHardness));
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BrushEdgeHardness item)
                {
                    foreach (Line child in this.Canvas.Children)
                    {
                        child.Y2 = this.Formula(item, Math.Abs(child.X2 - 200) / 200) * 300;
                    }
                }
            };
        }

        private double Cos(double value) => Math.Cos(value * Math.PI) / 2 + 0.5;
        private double Formula(BrushEdgeHardness cap, double x) // 1 ~ 0 ~ 1
        {
            switch (cap)
            {
                case BrushEdgeHardness.None: return 1;
                case BrushEdgeHardness.Cosine: return this.Cos(x);
                case BrushEdgeHardness.Quadratic: return this.Cos(x * x);
                case BrushEdgeHardness.Cube: return this.Cos(x * x * x);
                case BrushEdgeHardness.Quartic: return this.Cos(x * x * x * x);
                default: return 1;
            }
        }

    }
}