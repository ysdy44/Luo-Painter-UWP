using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Elements
{
    public interface IColorPickerBase
    {
        //@Delegate
        event EventHandler<Color> ColorChanged;

        void Recolor(Color color);
    }

    public interface IColorPicker : IColorPickerBase
    {
        void Left();
        void Right();

        void Down();
        void Up();

        void ZoomOut();
        void ZoomIn();
    }

    public readonly struct BoxSize
    {
        public readonly double S; // = 20;

        public readonly double W; // = 320;
        public readonly double H; // = 280;
        public readonly double O; // = 40;

        public double Y1 => (this.H + this.W + this.S) / 2;
        public double Y2 => (this.H + this.W - this.S) / 2;

        public BoxSize(double size, double slider = 20)
        {
            this.W = size; // Width
            this.H = size - 10 - slider - 10; // Height
            this.O = this.W - this.H; // Other

            this.S = slider; // Slider
        }
    }

    public readonly struct WheelSize
    {
        public readonly double R; // = 100;
        public readonly double D; // = 200;

        public readonly double HR; // = 80;
        public readonly double HD; // = 160;

        public Point C => new Point(this.R, this.R); // = new Point(100, 100);

        public WheelSize(double size, double hole = 0.87d)
        {
            this.R = size / 2; // Radius
            this.D = size; // Diameter

            this.HR = hole * this.R; // HoleRadius
            this.HD = hole * this.D; // HoleDiameter
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

        public WheelSizeF(float size, float hole = 0.87f)
        {
            this.R = size / 2; // Radius
            this.D = size; // Diameter

            this.HR = hole * this.R; // HoleRadius
            this.HD = hole * this.D; // HoleDiameter
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
            this.Size = size;

            // a2 + b2 = c2
            double a = size.HR / 2;
            double c = size.HR;
            double b = Math.Sqrt(c * c - a * a);

            this.AL = size.R - b; // AngleLeft
            this.AR = size.R + b; // AngleRight
            this.AW = b + b; // AngleWidth

            this.AT = size.R - c; // AngleTop
            this.AB = size.R + a; // AngleBottom
            this.AH = a + c; // AngleHeight
        }

        public double X(double s) => s * this.AW + this.AL;
        public double Y(double v) => v * this.AH + this.AT;

        public Point Triangle(Point position) => new Point(position.X - this.AL, position.Y - this.AT);
        public Point Triangle(Vector4 hsv) => new Point
        {
            X = hsv.Y == 0d ? this.Size.R : Math.Clamp(this.AL + this.AW * hsv.X, this.AL, this.AR),
            Y = this.AT + this.AH * hsv.Y
        };

        public double V(Point triangle) => Math.Clamp(triangle.Y / this.AH, 0d, 1d);
        public double S(Point triangle, double v)
        {
            if (triangle.Y == 0d) return 0.5d;

            double vh = v / 2d;
            return Math.Clamp(triangle.X / this.AW, 0.5d - vh, 0.5d + vh);
        }
    }

    public readonly struct RectangleSize
    {
        public readonly WheelSize Size;

        public readonly double RD; // = 280 / Math.Sqrt(2);

        public readonly double RT; // = C - R;
        public readonly double RB; // = C + R;

        public RectangleSize(WheelSize size)
        {
            this.Size = size;

            double rr = size.HR / Math.Sqrt(2); // RectangleRadius
            this.RD = rr * 2; // RectangleDiameter

            this.RT = size.R - rr; // RectangleTop
            this.RB = size.R + rr; // RectangleBottom
        }

        public double X(double s) => s * this.RD + this.RT;
        public double Y(double v) => (1d - v) * this.RD + this.RT;

        public Point Rectangle(Vector4 hsv) => new Point
        {
            X = this.RD * hsv.X,
            Y = this.RD * (1d - hsv.Y)
        };

        public double V(Point rectangle) => Math.Clamp(1d - rectangle.Y / this.RD, 0d, 1d);
        public double S(Point rectangle) => Math.Clamp(rectangle.X / this.RD, 0d, 1d);
    }
}