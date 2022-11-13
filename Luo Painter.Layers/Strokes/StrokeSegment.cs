using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Layers
{
    public partial struct StrokeSegment
    {

        public readonly Vector2 StartingPosition;
        public readonly Vector2 Position;

        public readonly float StartingPressure;
        public readonly float Pressure;


        public readonly float Spacing;

        public readonly float StartingSize;
        public readonly float Size;

        public readonly float StartingDistance;
        public readonly float Distance;


        public readonly Rect Bounds;
        public readonly Vector2 Normalize;
        public readonly bool IsNaN;
        public readonly bool InRadius;


        public StrokeSegment(Vector2 startingPosition, Vector2 position, float startingPressure = 1f, float pressure = 1f, float size = 22f, float spacing = 0.25f)
        {
            this.StartingPosition = startingPosition;
            this.Position = position;

            this.StartingPressure = startingPressure;
            this.Pressure = pressure;


            this.Spacing = spacing;

            this.StartingSize = System.Math.Max(1f, size * startingPressure);
            this.Size = size;

            this.StartingDistance = spacing * this.StartingSize;
            this.Distance = Vector2.Distance(startingPosition, position);


            this.Bounds = new Rect
            {
                X = System.Math.Min(startingPosition.X, position.X) - size,
                Y = System.Math.Min(startingPosition.Y, position.Y) - size,
                Width = System.Math.Abs(startingPosition.X - position.X) + size + size,
                Height = System.Math.Abs(startingPosition.Y - position.Y) + size + size,
            };

            Vector2 vector = position - startingPosition;
            this.Normalize = Vector2.Normalize(vector);

            if (vector == Vector2.Zero) this.IsNaN = true;
            else if (double.IsNaN(vector.X)) this.IsNaN = true;
            else if (double.IsNaN(vector.Y)) this.IsNaN = true;
            else this.IsNaN = false;

            this.InRadius = this.Distance <= this.StartingDistance;
        }

    }
}