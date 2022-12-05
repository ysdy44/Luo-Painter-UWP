using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Elements
{
    public readonly struct WheelSize
    {
        public readonly double R; // = 100;
        public readonly double D; // = 200;

        public readonly double HR; // = 80;
        public readonly double HD; // = 160;

        public Point C => new Point(this.R, this.R); // = new Point(100, 100);

        public WheelSize(double size, double hole = 0.8f)
        {
            this.R = size / 2;
            this.D = size;

            this.HR = hole * this.R;
            this.HD = hole * this.D;
        }

        public double XY1(double cossin) => cossin * this.R + this.R;
        public double XY2(double cossin) => cossin * this.HR + this.R;

        public Point Wheel(Point position) => new Point(position.X - this.R, position.Y - this.R);
    }

    public readonly struct WheelSizeF
    {
        public readonly float R; // = 100;
        public readonly float D; // = 200;

        public readonly float HR; // = 80;
        public readonly float HD; // = 160;

        public Vector2 C => new Vector2(this.R, this.R); // = new Point(100, 100);

        public WheelSizeF(float size, float hole = 0.8f)
        {
            this.R = size / 2;
            this.D = size;

            this.HR = hole * this.R;
            this.HD = hole * this.D;
        }

        public float XY1(float cossin) => cossin * this.R + this.R;
        public float XY2(float cossin) => cossin * this.HR + this.R;

        public Vector2 Wheel(Vector2 position) => new Vector2(position.X - this.R, position.Y - this.R);
    }

    public readonly struct TriangleSize
    {
        public readonly WheelSize Size;

        public readonly double AL; // = 30.7;
        public readonly double AR; // = 169.3;
        public readonly double AW; // = 169.3 - 30.7;

        public readonly double AT; // = 20;
        public readonly double AB; // = 140;
        public readonly double AH; // = 140 - 20;

        public Point Angle0 => new Point(this.Size.R, this.AT); // = new Point(100, 20);
        public Point Angle1 => new Point(this.AR, this.AB); // = new Point(169.3, 140);
        public Point Angle2 => new Point(this.AL, this.AB); // = new Point(30.7, 140);

        public TriangleSize(WheelSize size)
        {
            Size = size;

            // a2 + b2 = c2
            double a = Size.HR / 2;
            double c = Size.HR;
            double b = Math.Sqrt(c * c - a * a);

            this.AL = Size.R - b; // AngleLeft
            this.AR = Size.R + b; // AngleRight
            this.AW = b + b; // AngleWidth

            this.AT = Size.R - c; // AngleTop
            this.AB = Size.R + a; // AngleBottom
            this.AH = a + c; // AngleHeight
        }

        public double X(double s) => s * this.AW + this.AL;
        public double Y(double v) => v * this.AH + this.AT;

        public Point Triangle(Point position) => new Point(position.X - this.AL, position.Y - this.AT);

        public double V(Point triangle) => Math.Clamp(triangle.Y / this.AH, 0d, 1d);
        public double S(Point triangle, double v)
        {
            if (triangle.Y == 0d) return 0.5d;

            double vh = v / 2d;
            return Math.Clamp(triangle.X / this.AW, 0.5d - vh, 0.5d + vh);
        }
    }

    public sealed partial class TricolorPicker : UserControl
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


        public void Recolor(Color color)
        {
            this.HSV = color.ToHSV();
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