using Luo_Painter.Brushes;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    internal class BrushEasePressureModel
    {
        public BrushEasePressure Pressure { get; set; }

        public double Line25 => 236 - this.GetPressure(0.25) * 236;
        public double Line50 => 236 - this.GetPressure(0.50) * 236;
        public double Line75 => 236 - this.GetPressure(0.75) * 236;

        public PointCollection Points
        {
            get
            {
                PointCollection points = new PointCollection();

                points.Add(new Point(0, 236));
                for (int i = 0; i <= 32; i++)
                {
                    double scale = i / 32d;
                    points.Add(new Point(scale * 236, 236 - this.GetPressure(scale) * 236));
                }
                points.Add(new Point(236, 236));

                return points;
            }
        }
        
        public double GetPressure(double x)
        {
            switch (this.Pressure)
            {
                case BrushEasePressure.None: return 1;

                case BrushEasePressure.Linear: return this.Linear(x);
                case BrushEasePressure.LinearFlip: return 1 - this.Linear(x);

                case BrushEasePressure.Quadratic: return this.Quadratic(x);
                case BrushEasePressure.QuadraticFlip: return 1 - this.Quadratic(x);
                case BrushEasePressure.QuadraticReverse: return this.Quadratic(1 - x);
                case BrushEasePressure.QuadraticFlipReverse: return 1 - this.Quadratic(1 - x);

                case BrushEasePressure.Mirror: return this.Mirror(x);
                case BrushEasePressure.MirrorFlip: return 1 - this.Mirror(x);

                case BrushEasePressure.Symmetry: return this.Symmetry(x);
                case BrushEasePressure.SymmetryFlip: return 1 - this.Symmetry(x);

                default: return 1;
            }
        }

        public double GetPressure2(double x)
        {
            switch (this.Pressure)
            {
                case BrushEasePressure.None: return 1;

                case BrushEasePressure.Linear: return x;

                case BrushEasePressure.Quadratic: return x * x;
                case BrushEasePressure.QuadraticFlipReverse: return 2 * x - x * x;

                case BrushEasePressure.Symmetry:
                    x *= 2;
                    x -= 1;
                    double y = (x > 0) ? (2 * x - x * x) : (2 * x + x * x);
                    y += 1;
                    y /= 2;
                    return y;

                default: return 1;
            }
        }

        public float GetPressure(int pressure, float x)
        {
            switch (pressure)
            {
                case 0: return 1;

                case 1: return x;

                case 3: return x * x;
                case 6: return 2 * x - x * x;

                case 9:
                    x *= 2;
                    x -= 1;
                    float y = (x > 0) ? (2 * x - x * x) : (2 * x + x * x);
                    y += 1;
                    y /= 2;
                    return y;

                default: return 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Linear(double x) => x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Quadratic(double x) => x * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Mirror(double x)
        {
            x *= 2;
            x -= 1;
            return this.Quadratic(x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Symmetry(double x)
        {
            x *= 2;
            x -= 1;
            double y = (x > 0) ? 1 - this.Quadratic(x - 1) : -1 + this.Quadratic(x + 1);
            y += 1;
            y /= 2;
            return y;
        }
    }

    public sealed partial class BrushEasePressurePage : Page
    {
        public BrushEasePressurePage()
        {
            this.InitializeComponent();
        }
    }
}