﻿using Luo_Painter.Elements;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class InverseProportionPage : Page
    {

        readonly Point[] OnePoints = new Point[1000];

        readonly Range XRange;
        readonly Range YRange = new Range { Default = 1, Minimum = 0.1, Maximum = 10 };
        readonly InverseProportion Prop = new InverseProportion { A = -1, B = 10, C = 1 };

        public InverseProportionPage()
        {
            this.InitializeComponent();
            this.XRange = this.Prop.ConvertYToX(this.YRange);
            for (int i = 0; i < this.OnePoints.Length; i++)
            {
                double xOne = (double)i / this.OnePoints.Length;
                double x = this.XRange.ConvertOneToValue(xOne);
                double y = this.Prop.ConvertXToY(x);
                double yOne = this.YRange.ConvertValueToOne(y);
                Point one = new Point(xOne, yOne);

                this.OnePoints[i] = one;
                this.Polyline.Points.Add(one);
            }

            this.Canvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.XAxix.X2 = e.NewSize.Width;
                this.YAxix.Y2 = e.NewSize.Height;
                for (int i = 0; i < this.OnePoints.Length; i++)
                {
                    Point one = this.OnePoints[i];
                    this.Polyline.Points[i] = new Point
                    {
                        X = one.X * e.NewSize.Width,
                        Y = one.Y * e.NewSize.Height
                    };
                }
            };

            this.XSlider.ValueChanged += (s, e) =>
            {
                double x = e.NewValue;
                double y = this.Prop.ConvertXToY(x);
                this.XRun.Text = $"{x:0.00}";
                this.YRun.Text = $"{y:0.00}";

                double xOne = this.XRange.ConvertValueToOne(x);
                double yOne = this.YRange.ConvertValueToOne(y);
                Canvas.SetLeft(this.Ellipse, xOne * this.Canvas.ActualWidth - 10);
                Canvas.SetTop(this.Ellipse, yOne * this.Canvas.ActualHeight - 10);
            };

            this.YSlider.ValueChanged += (s, e) =>
            {
                double y = e.NewValue;
                double x = this.Prop.ConvertYToX(y);
                this.XRun.Text = $"{x:0.00}";
                this.YRun.Text = $"{y:0.00}";

                double xOne = this.XRange.ConvertValueToOne(x);
                double yOne = this.YRange.ConvertValueToOne(y);
                Canvas.SetLeft(this.Ellipse, xOne * this.Canvas.ActualWidth - 10);
                Canvas.SetTop(this.Ellipse, yOne * this.Canvas.ActualHeight - 10);
            };
        }

    }
}