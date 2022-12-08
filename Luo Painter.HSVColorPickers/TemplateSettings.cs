using System;
using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.HSVColorPickers
{
    public readonly struct CircleTemplateSettings
    {
        public readonly double Radius; // = 100;
        public readonly double Diameter; // = 200;

        public readonly Point Center; // = new Point(100, 100);

        public CircleTemplateSettings(double size)
        {
            size = Math.Max(80, size);
            this.Radius = size / 2;
            this.Diameter = size;

            this.Center = new Point(this.Radius, this.Radius);
        }

        public Point Offset(Point position) => new Point(position.X - this.Radius, position.Y - this.Radius);

        public Point XY(double radians)
        {
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);
            return new Point(cos * this.Radius + this.Radius, sin * this.Radius + this.Radius);
        }
        public Point XY(double radians, double scale)
        {
            double cos = Math.Cos(radians) * scale;
            double sin = Math.Sin(radians) * scale;
            return new Point(cos * this.Radius + this.Radius, sin * this.Radius + this.Radius);
        }
    }

    public readonly struct CircleTemplateSettingsF
    {
        public readonly float Radius; // = 100;
        public readonly float Diameter; // = 200;

        public readonly Vector2 Center; // = new Vector2(100, 100);

        public CircleTemplateSettingsF(float size)
        {
            size = Math.Max(80, size);
            this.Radius = size / 2;
            this.Diameter = size;

            this.Center = new Vector2(this.Radius, this.Radius);
        }

        public Vector2 Offset(Vector2 position) => new Vector2(position.X - this.Radius, position.Y - this.Radius);

        public Vector2 XY(float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            return new Vector2(cos * this.Radius + this.Radius, sin * this.Radius + this.Radius);
        }
        public Vector2 XY(float radians, float scale)
        {
            float cos = MathF.Cos(radians) * scale;
            float sin = MathF.Sin(radians) * scale;
            return new Vector2(cos * this.Radius + this.Radius, sin * this.Radius + this.Radius);
        }
    }


    public readonly struct BoxTemplateSettings
    {
        public readonly double Slider; // = 20;

        public readonly double Width; // = 320;
        public readonly double Height; // = 280;
        public readonly double Other; // = 40;

        public readonly double Y1; // = 310;
        public readonly double Y2; // = 290;

        public BoxTemplateSettings(double size) : this(size, size) { }
        public BoxTemplateSettings(double width, double height)
        {
            width = Math.Max(40, width);
            height = Math.Max(40, height);
            this.Slider = 20;

            this.Other = 10 + this.Slider + 10;
            this.Height = height - this.Other;
            this.Width = width;

            this.Y2 = this.Height + 10;
            this.Y1 = this.Y2 + this.Slider;
        }
    }

    public readonly struct RingTemplateSettings
    {
        public readonly BoxTemplateSettings BoxSize;
        public readonly CircleTemplateSettings CircleSize;

        public readonly double Left; // = 20;
        public readonly double Top; // = 0;

        public RingTemplateSettings(BoxTemplateSettings size)
        {
            this.BoxSize = size;
            this.CircleSize = new CircleTemplateSettings(Math.Min(this.BoxSize.Width, this.BoxSize.Height));

            this.Left = this.BoxSize.Width / 2 - this.CircleSize.Radius;
            this.Top = this.BoxSize.Height / 2 - this.CircleSize.Radius;
        }
    }


    public readonly struct WheelTemplateSettings
    {
        //@Static
        public static double Atan2(Point vector) => Math.Atan2(vector.Y, vector.X);

        public readonly CircleTemplateSettings CircleSize;
        public readonly CircleTemplateSettings HoleCircleSize;

        public WheelTemplateSettings(CircleTemplateSettings size)
        {
            this.CircleSize = size;

            double hole = 1d - 20 / size.Radius;
            this.HoleCircleSize = new CircleTemplateSettings(hole * size.Diameter);
        }

        public Point XY(double radians, double scale)
        {
            double cos = Math.Cos(radians) * scale;
            double sin = Math.Sin(radians) * scale;
            return new Point(cos * this.HoleCircleSize.Radius + this.CircleSize.Radius, sin * this.HoleCircleSize.Radius + this.CircleSize.Radius);
        }
    }

    public readonly struct TriangleTemplateSettings
    {
        public readonly WheelTemplateSettings WheelSize;

        public readonly double AngleLeft; // = 30.7;
        public readonly double AngleRight; // = 169.3;
        public readonly double AngleWidth; // = 169.3 - 30.7;

        public readonly double AngleTop; // = 20;
        public readonly double AngleBottom; // = 140;
        public readonly double AngleHeight; // = 140 - 20;

        public Point Angle0 => new Point(this.WheelSize.CircleSize.Radius, this.AngleTop); // = new Point(100, 20);
        public Point Angle1 => new Point(this.AngleRight, this.AngleBottom); // = new Point(169.3, 140);
        public Point Angle2 => new Point(this.AngleLeft, this.AngleBottom); // = new Point(30.7, 140);

        public TriangleTemplateSettings(WheelTemplateSettings size)
        {
            this.WheelSize = size;

            // a2 + b2 = c2
            double a = size.HoleCircleSize.Radius / 2;
            double c = size.HoleCircleSize.Radius;
            double b = Math.Sqrt(c * c - a * a);

            this.AngleLeft = size.CircleSize.Radius - b;
            this.AngleRight = size.CircleSize.Radius + b;
            this.AngleWidth = b + b;

            this.AngleTop = size.CircleSize.Radius - c;
            this.AngleBottom = size.CircleSize.Radius + a;
            this.AngleHeight = a + c;
        }

        public double X(double s) => s * this.AngleWidth + this.AngleLeft;
        public double Y(double v) => v * this.AngleHeight + this.AngleTop;

        public Point Offset(Point position) => new Point(position.X - this.AngleLeft, position.Y - this.AngleTop);
        public Point Offset(Vector4 hsv) => new Point
        {
            X = hsv.Y == 0d ? this.WheelSize.CircleSize.Radius : Math.Clamp(this.AngleLeft + this.AngleWidth * hsv.X, this.AngleLeft, this.AngleRight),
            Y = this.AngleTop + this.AngleHeight * hsv.Y
        };

        public double Value(Point triangle) => Math.Clamp(triangle.Y / this.AngleHeight, 0d, 1d);
        public double Saturation(Point triangle, double v)
        {
            if (triangle.Y == 0d) return 0.5d;

            double vh = v / 2d;
            return Math.Clamp(triangle.X / this.AngleWidth, 0.5d - vh, 0.5d + vh);
        }
    }

    public readonly struct RectangleTemplateSettings
    {
        public readonly WheelTemplateSettings WheelSize;

        public readonly double RectangleDiameter; // = 280 / Math.Sqrt(2);

        public readonly double RectangleTop; // = C - R;
        public readonly double RectangleBottom; // = C + R;

        public RectangleTemplateSettings(WheelTemplateSettings size)
        {
            this.WheelSize = size;

            double rectangleRadius = size.HoleCircleSize.Radius / Math.Sqrt(2);
            this.RectangleDiameter = rectangleRadius * 2;

            this.RectangleTop = size.CircleSize.Radius - rectangleRadius;
            this.RectangleBottom = size.CircleSize.Radius + rectangleRadius;
        }

        public double X(double s) => s * this.RectangleDiameter + this.RectangleTop;
        public double Y(double v) => (1d - v) * this.RectangleDiameter + this.RectangleTop;

        public Point Offset(Vector4 hsv) => new Point
        {
            X = this.RectangleDiameter * hsv.X,
            Y = this.RectangleDiameter * (1d - hsv.Y)
        };

        public double Value(Point rectangle) => Math.Clamp(1d - rectangle.Y / this.RectangleDiameter, 0d, 1d);
        public double Saturation(Point rectangle) => Math.Clamp(rectangle.X / this.RectangleDiameter, 0d, 1d);
    }
}