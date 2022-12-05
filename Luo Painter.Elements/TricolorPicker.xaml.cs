using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Elements
{
    public sealed partial class TricolorPicker : UserControl, IColorPicker
    {
        //@Delegate
        public event EventHandler<Color> ColorChanged;

        //@Converter
        public double VectorToH(Point vector) => ((Math.Atan2(vector.Y, vector.X) * 180d / Math.PI) + 360d) % 360d;

        readonly TriangleSize T = new TriangleSize(new WheelSize(320, 0.87));
        WheelSize W => this.T.Size;

        Point Triangle;
        Point Wheel;
        Vector4 HSV;

        //@Construct
        public TricolorPicker()
        {
            this.InitializeComponent();
            this.Recolor(Colors.Black);

            this.TrianglePath.ManipulationMode = ManipulationModes.All;
            this.TrianglePath.ManipulationStarted += (_, e) =>
            {
                this.Triangle = this.T.Triangle(e.Position);
                this.Move();

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
                //this.TextBlock.Visibility = Visibility.Visible;
            };
            this.TrianglePath.ManipulationDelta += (_, e) =>
            {
                this.Triangle.X += e.Delta.Translation.X;
                this.Triangle.Y += e.Delta.Translation.Y;
                this.Move();

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
            };
            this.TrianglePath.ManipulationCompleted += (_, e) =>
            {
                Color color = this.HSV.ToColor();
                this.Color(color);

                //this.TextBlock.Text = ColorHelper.ToDisplayName(this.HSV.ToColor());
                //this.TextBlock.Visibility = Visibility.Collapsed;
            };

            this.WheelPath.ManipulationMode = ManipulationModes.All;
            this.WheelPath.ManipulationStarted += (_, e) =>
            {
                this.Wheel = this.W.Wheel(e.Position);
                this.Zoom();

                this.TextBlock.Text = $"{(int)this.HSV.Z} °";
                this.TextBlock.Visibility = Visibility.Visible;
            };
            this.WheelPath.ManipulationDelta += (_, e) =>
            {
                this.Wheel.X += e.Delta.Translation.X;
                this.Wheel.Y += e.Delta.Translation.Y;
                this.Zoom();

                this.TextBlock.Text = $"{(int)this.HSV.Z} °";
            };
            this.WheelPath.ManipulationCompleted += (_, e) =>
            {
                Color color = HSVExtensions.ToColor(this.HSV.Z);
                this.Stop(color);
                this.Color(this.HSV.ToColor());

                this.TextBlock.Text = $"{(int)this.HSV.Z} °";
                this.TextBlock.Visibility = Visibility.Collapsed;
            };
        }
    }

    public sealed partial class TricolorPicker
    {
        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();

            this.Triangle.Y = this.T.AT + this.T.AH * this.HSV.Y;
            if (this.HSV.Y == 0d) this.Triangle.X = this.W.R;
            else this.Triangle.X = Math.Clamp(this.T.AL + this.T.AW * this.HSV.X, this.T.AL, this.T.AR);

            this.Line(Math.PI * this.HSV.Z / 180f);
            this.Ellipse(this.HSV.X == 0f ? 0.5 : this.HSV.X, this.HSV.Y);
        }


        private void Move()
        {
            double v = this.T.V(this.Triangle);
            double s = this.T.S(this.Triangle, v);
            this.HSV.X = (float)s;
            this.HSV.Y = (float)v;

            this.Ellipse(s, v);

            Color color = this.HSV.ToColor();
            this.Color(color);
        }

        private void Zoom()
        {
            double h = this.VectorToH(this.Wheel);
            h += 360d;
            h %= 360d;
            this.HSV.Z = (float)h;

            Vector2 v = Vector2.Normalize(this.Wheel.ToVector2());
            this.Line(v.X, v.Y);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }


        public void Left()
        {
            this.Triangle.X -= 1;
            this.Move();
        }
        public void Right()
        {
            this.Triangle.X += 1;
            this.Move();
        }

        public void Down()
        {
            this.Triangle.Y += 1;
            this.Move();
        }
        public void Up()
        {
            this.Triangle.Y -= 1;
            this.Move();
        }

        public void ZoomOut()
        {
            this.HSV.Z -= 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;

            this.Line(this.HSV.Z * MathF.PI / 180d);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }
        public void ZoomIn()
        {
            this.HSV.Z += 1;
            this.HSV.Z += 360f;
            this.HSV.Z %= 360f;

            this.Line(this.HSV.Z * MathF.PI / 180d);

            Color color = HSVExtensions.ToColor(this.HSV.Z);
            this.Stop(color);
            this.Color(this.HSV.ToColor());
        }
    }

    public sealed partial class TricolorPicker
    {
        private void Color(Color color)
        {
            this.ColorChanged?.Invoke(this, color); // Delegate

            this.EllipseSolidColorBrush.Color = color;
        }

        private void Stop(Color color)
        {
            this.LineSolidColorBrush.Color = color;

            this.EndStop.Color = color;
            color.A = 0;
            this.StartStop.Color = color;
        }

        private void Line(double h) => this.Line(Math.Cos(h), Math.Sin(h));
        private void Line(double cos, double sin)
        {
            this.BlackLine.X1 = this.WhiteLine.X1 = this.W.XY1(cos);
            this.BlackLine.Y1 = this.WhiteLine.Y1 = this.W.XY1(sin);

            this.BlackLine.X2 = this.WhiteLine.X2 = this.W.XY2(cos);
            this.BlackLine.Y2 = this.WhiteLine.Y2 = this.W.XY2(sin);
        }

        private void Ellipse(double s, double v)
        {
            double x = this.T.X(s);
            double y = this.T.Y(v);

            Canvas.SetLeft(this.BlackEllipse, x - 14);
            Canvas.SetTop(this.BlackEllipse, y - 14);
            Canvas.SetLeft(this.WhiteEllipse, x - 13);
            Canvas.SetTop(this.WhiteEllipse, y - 13);
        }
    }
}