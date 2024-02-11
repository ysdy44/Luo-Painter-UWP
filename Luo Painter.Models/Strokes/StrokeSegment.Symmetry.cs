using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Models
{
    partial struct StrokeSegment
    {
        public StrokeSegment(StrokeSegment mirror, Vector2 center, Orientation orientation)
        {
            // Paint
            switch (orientation)
            {
                case Orientation.Vertical:
                    this.StartingPosition.X = mirror.StartingPosition.X;
                    this.StartingPosition.Y = center.Y + center.Y - mirror.StartingPosition.Y;
                    this.Position.X = mirror.Position.X;
                    this.Position.Y = center.Y + center.Y - mirror.Position.Y;
                    break;
                default:
                    this.StartingPosition.X = center.X + center.X - mirror.StartingPosition.X;
                    this.StartingPosition.Y = mirror.StartingPosition.Y;
                    this.Position.X = center.X + center.X - mirror.Position.X;
                    this.Position.Y = mirror.Position.Y;
                    break;
            }

            this.StartingPressure = mirror.StartingPressure;
            this.Pressure = mirror.Pressure;

            // Ink
            this.Spacing = mirror.Spacing;

            this.Size = mirror.Size;

            // Radius
            this.Radius = mirror.Radius;
            this.Distance = mirror.Distance;

            // Stroke
            this.Bounds = RectExtensions.GetRect(this.StartingPosition, this.Position, this.Size);
            this.Normalize = mirror.Normalize;
            this.IsNaN = mirror.IsNaN;
            this.InRadius = mirror.InRadius;
        }

        public StrokeSegment(StrokeSegment symmetry, Vector2 center)
        {
            // Paint
            this.StartingPosition = center + center - symmetry.StartingPosition;
            this.Position = center + center - symmetry.Position;

            this.StartingPressure = symmetry.StartingPressure;
            this.Pressure = symmetry.Pressure;

            // Ink
            this.Spacing = symmetry.Spacing;

            this.Size = symmetry.Size;

            // Radius
            this.Radius = symmetry.Radius;
            this.Distance = symmetry.Distance;

            // Stroke
            this.Bounds = RectExtensions.GetRect(this.StartingPosition, this.Position, this.Size);
            this.Normalize = symmetry.Normalize;
            this.IsNaN = symmetry.IsNaN;
            this.InRadius = symmetry.InRadius;
        }

        public StrokeSegment(StrokeSegment symmetry, Matrix3x2 matrix)
        {
            // Paint
            this.StartingPosition = Vector2.Transform(symmetry.StartingPosition, matrix);
            this.Position = Vector2.Transform(symmetry.Position, matrix);

            this.StartingPressure = symmetry.StartingPressure;
            this.Pressure = symmetry.Pressure;

            // Ink
            this.Spacing = symmetry.Spacing;

            this.Size = symmetry.Size;

            // Radius
            this.Radius = symmetry.Radius;
            this.Distance = symmetry.Distance;

            // Stroke
            this.Bounds = RectExtensions.GetRect(this.StartingPosition, this.Position, this.Size);
            this.Normalize = symmetry.Normalize;
            this.IsNaN = symmetry.IsNaN;
            this.InRadius = symmetry.InRadius;
        }
    }
}