using Luo_Painter.Elements;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal sealed class ScaleRange
    {
        public readonly Range XRange;
        public readonly Range YRange = new Range
        {
            Default = 1,
            IP = new InverseProportion
            {
                Minimum = 0.1,
                Maximum = 10,
            }
        };

        public readonly InverseProportion XIP;
        public readonly InverseProportion YIP = new InverseProportion
        {
            Minimum = 0.3333333333333333333333333333333333333333333333333333333333333,
            Maximum = 1,
        };

        public ScaleRange()
        {
            if (this.YRange.Minimum >= this.YRange.Maximum || this.YIP.Minimum >= this.YIP.Maximum)
                throw new System.IndexOutOfRangeException("The minimum must be less than the maximum.");

            this.XIP = this.YIP.Convert();
            this.XRange = this.YRange.Convert(this.YIP, this.YRange.IP, 100);
        }

        public double ConvertXToY(double x) => InverseProportion.Convert(x, this.XIP, this.XRange.IP, this.YIP, this.YRange.IP, RangeRounding.Maximum, RangeRounding.Minimum);
        public double ConvertYToX(double y) => InverseProportion.Convert(y, this.YIP, this.YRange.IP, this.XIP, this.XRange.IP, RangeRounding.Minimum, RangeRounding.Maximum);
    }

    public sealed partial class InverseProportionPage : Page
    {

        const double Unit = 4;
        readonly Point[] Points = new Point[100];

        public InverseProportionPage()
        {
            this.InitializeComponent();
            {
                Point one = new Point(Unit, 1 / Unit);
                this.Points[0] = one;
                this.Polyline.Points.Add(one);
            }
            for (int i = 1; i < this.Points.Length; i++)
            {
                double xOne = 1 - (double)i / this.Points.Length; // 0~1

                double xip = Unit * xOne; // 0 ~ Unit
                double yip = 1 / xip; // ∞ ~ 1/Unit

                Point one = new Point(xip, yip);
                this.Points[i] = one;
                this.Polyline.Points.Add(one);
            }

            this.Canvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.XAxix.X2 = e.NewSize.Width;
                this.YAxix.Y2 = e.NewSize.Height;
                for (int i = 0; i < this.Points.Length; i++)
                {
                    Point ip = this.Points[i];
                    this.Polyline.Points[i] = new Point
                    {
                        X = ip.X / Unit * e.NewSize.Width,
                        Y = ip.Y / Unit * e.NewSize.Height
                    };
                }

                // Left
                this.LineLeft.X1 = this.LineLeft.X2 = this.LineTop.X1 = this.LineBottom.X2 =
                this.ScaleRange.XIP.Minimum / Unit * e.NewSize.Width;
                // Top
                this.LineTop.Y1 = this.LineTop.Y2 = this.LineLeft.Y1 = this.LineRight.Y2 =
                this.ScaleRange.YIP.Minimum / Unit * e.NewSize.Height;
                // Right
                this.LineRight.X1 = this.LineRight.X2 = this.LineBottom.X1 = this.LineTop.X2 =
                this.ScaleRange.XIP.Maximum / Unit * e.NewSize.Width;
                // Bottom
                this.LineBottom.Y1 = this.LineBottom.Y2 = this.LineRight.Y1 = this.LineLeft.Y2 =
                this.ScaleRange.YIP.Maximum / Unit * e.NewSize.Height;
            };

            this.XSlider.ValueChanged += (s, e) =>
            {
                /// <see cref="ScaleRange.ConvertXToY(double)"/>
                double x = e.NewValue;
                double xOne = this.ScaleRange.XRange.IP.ConvertValueToOne(x, RangeRounding.Maximum);
                double xip = this.ScaleRange.XIP.ConvertOneToValue(xOne);

                double yip = InverseProportion.Convert(xip);
                double yOne = this.ScaleRange.YIP.ConvertValueToOne(yip);
                double y = this.ScaleRange.YRange.IP.ConvertOneToValue(yOne);

                this.XRun.Text = $"{x:0.00}";
                this.YRun.Text = $"{y:0.00}";

                Canvas.SetLeft(this.Ellipse, xip / Unit * this.Canvas.ActualWidth - 10);
                Canvas.SetTop(this.Ellipse, yip / Unit * this.Canvas.ActualHeight - 10);
            };
            this.YSlider.ValueChanged += (s, e) =>
            {
                /// <see cref="ScaleRange.ConvertYToX(double)(double)"/>
                double y = e.NewValue;
                double yOne = this.ScaleRange.YRange.IP.ConvertValueToOne(y);
                double yip = this.ScaleRange.YIP.ConvertOneToValue(yOne);

                double xip = InverseProportion.Convert(yip);
                double xOne = this.ScaleRange.XIP.ConvertValueToOne(xip);
                double x = this.ScaleRange.XRange.IP.ConvertOneToValue(xOne);

                this.XRun.Text = $"{x:0.00}";
                this.YRun.Text = $"{y:0.00}";

                Canvas.SetLeft(this.Ellipse, xip / Unit * this.Canvas.ActualWidth - 10);
                Canvas.SetTop(this.Ellipse, yip / Unit * this.Canvas.ActualHeight - 10);
            };
        }
    }
}