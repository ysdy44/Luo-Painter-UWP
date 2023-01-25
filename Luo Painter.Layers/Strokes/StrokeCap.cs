using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Layers
{
    public partial struct StrokeCap
    {
        // Paint
        public readonly Vector2 StartingPosition;

        public readonly float StartingPressure;

        // Ink
        public readonly float StartingSize;
        public readonly float Size;

        // Stroke
        public readonly Rect Bounds;

        public StrokeCap(Vector2 startingPosition, float startingPressure = 1f, float size = 22f, bool ignoreSizePressure = false)
        {
            // Paint
            this.StartingPosition = startingPosition;

            this.StartingPressure = ignoreSizePressure ? 1 : startingPressure;

            // Ink
            this.StartingSize = ignoreSizePressure ? size : System.Math.Max(1f, size * startingPressure);
            this.Size = size;

            // Stroke
            this.Bounds = this.StartingPosition.GetRect(this.Size);
        }
    }
}