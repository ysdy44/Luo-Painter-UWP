using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter.Layers
{
    public partial struct StrokeCap
    {
        public readonly Vector2 StartingPosition;

        public readonly float StartingPressure;


        public readonly float StartingSize;
        public readonly float Size;

        public readonly Rect Bounds;


        public StrokeCap(Vector2 startingPosition, float startingPressure = 1f, float size = 22f)
        {
            this.StartingPosition = startingPosition;

            this.StartingPressure = startingPressure;


            this.StartingSize = System.Math.Max(1f, size * startingPressure);
            this.Size = size;

            this.Bounds = new Rect
            {
                X = startingPosition.X - size,
                Y = startingPosition.Y - size,
                Width = startingPosition.X + size + size,
                Height = startingPosition.Y + size + size,
            };
        }

    }
}