using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Models
{
    public partial struct StrokeCap
    {
        // Paint
        public readonly Vector2 StartingPosition;

        public readonly float StartingPressure;

        // Ink
        public readonly float StartingSize;

        // Stroke
        public readonly Rect Bounds;

        public StrokeCap(Vector2 startingPosition, float startingPressure = 1f, float startingSize = 22f)
        {
            // Paint
            this.StartingPosition = startingPosition;

            this.StartingPressure = startingPressure;

            // Ink
            this.StartingSize = startingSize;

            // Stroke
            this.Bounds = this.StartingPosition.GetRect(this.StartingSize);
        }
    }
}