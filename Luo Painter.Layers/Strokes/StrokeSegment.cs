using Luo_Painter.Layers;
using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Models
{
    public partial struct StrokeSegment
    {
        // Paint
        public readonly Vector2 StartingPosition;
        public readonly Vector2 Position;

        public readonly float StartingPressure;
        public readonly float Pressure;

        // Ink
        public readonly float Spacing;

        public readonly float Size;

        // Radius
        public readonly float Radius;
        public readonly float Distance;

        // Stroke
        public readonly Rect Bounds;
        public readonly Vector2 Normalize;
        public readonly bool IsNaN;
        public readonly bool InRadius;

        public StrokeSegment(Vector2 startingPosition, Vector2 position, float startingPressure = 1f, float pressure = 1f, float size = 22f, float spacing = 0.25f)
        {
            // Paint
            this.StartingPosition = startingPosition;
            this.Position = position;

            this.StartingPressure = startingPressure;
            this.Pressure = pressure;

            // Ink
            this.Spacing = spacing;

            this.Size = size;

            this.Radius = spacing * size * startingPressure;
            this.Distance = Vector2.Distance(startingPosition, position);

            // Stroke
            this.Bounds = RectExtensions.GetRect(this.StartingPosition, this.Position, this.Size);

            Vector2 vector = position - startingPosition;
            this.Normalize = Vector2.Normalize(vector);

            if (vector == Vector2.Zero) this.IsNaN = true;
            else if (double.IsNaN(vector.X)) this.IsNaN = true;
            else if (double.IsNaN(vector.Y)) this.IsNaN = true;
            else this.IsNaN = false;

            this.InRadius = this.Distance <= this.Radius;
        }
    }
}