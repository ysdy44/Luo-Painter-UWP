using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Brushes
{
    public partial struct StrokeSegment
    {

        public readonly Vector2 StartingPosition;
        public readonly Vector2 Position;

        public readonly float StartingPressure;
        public readonly float Pressure;

        public readonly float Spacing;

        public readonly float Size;
        public readonly float StartingSize;

        public readonly float Distance;
        public readonly float StartingDistance;

        public StrokeSegment(Vector2 startingPosition, Vector2 position, float startingPressure = 1f, float pressure = 1f, float size = 22f, float spacing = 0.25f)
        {
            this.StartingPosition = startingPosition;
            this.Position = position;

            this.StartingPressure = startingPressure;
            this.Pressure = pressure;

            this.Spacing = spacing;

            this.StartingSize = System.Math.Max(1f, size * startingPressure);
            this.Size = size;

            this.StartingDistance = this.Spacing * this.StartingSize;
            this.Distance = Vector2.Distance(this.StartingPosition, this.Position);
        }

        public Vector2 Normalize()
        {
            Vector2 vector = this.Position - this.StartingPosition;

            return Vector2.Normalize(vector);
        }

        public bool IsNaN()
        {
            Vector2 vector = this.Position - this.StartingPosition;

            if (vector == Vector2.Zero) return true;
            else if (double.IsNaN(vector.X)) return true;
            else if (double.IsNaN(vector.Y)) return true;
            else return false;
        }

        public Rect GetRect(double radius) => new Rect
        {
            X = System.Math.Min(this.StartingPosition.X, this.Position.X) - radius,
            Y = System.Math.Min(this.StartingPosition.Y, this.Position.Y) - radius,
            Width = System.Math.Abs(this.StartingPosition.X - this.Position.X) + radius + radius,
            Height = System.Math.Abs(this.StartingPosition.Y - this.Position.Y) + radius + radius,
        };

        public bool InRadius() => this.Distance <= this.StartingDistance;

    }
}