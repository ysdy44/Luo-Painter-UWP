using System.Numerics;

namespace Luo_Painter.Brushes
{
    public struct Stroke
    {
        public Vector2 Position;
        public Vector2 StartingPosition;

        public float Pressure;
        public float StartingPressure;

        public float Distance() => Vector2.Distance(this.StartingPosition, this.Position);

        public Vector2 Normalize()
        {
            Vector2 vector = this.Position - this.StartingPosition;

            return Vector2.Normalize(vector);
        }

        public bool IsNaN()
        {
            Vector2 vector = this.Position - this.StartingPosition;

            if (double.IsNaN(vector.X)) return true;
            else if (double.IsNaN(vector.Y)) return true;
            else return false;
        }
    }
}