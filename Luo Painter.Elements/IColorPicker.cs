using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter.Elements
{
    public interface IColorPicker
    {
        //@Delegate
        event EventHandler<Color> ColorChanged;

        void Recolor(Color color);

        void Left();
        void Right();

        void Down();
        void Up();

        void ZoomOut();
        void ZoomIn();
    }

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

        public double V(Point triangle) => Math.Clamp(triangle.Y / this.AH, 0d, 1d);
        public double S(Point triangle, double v)
        {
            if (triangle.Y == 0d) return 0.5d;

            double vh = v / 2d;
            return Math.Clamp(triangle.X / this.AW, 0.5d - vh, 0.5d + vh);
        }
    }
}