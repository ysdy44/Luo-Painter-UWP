using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Models
{
    partial struct StrokeCap
    {
        public StrokeCap(StrokeCap mirror, Vector2 center, Orientation orientation)
        {
            // Paint
            switch (orientation)
            {
                case Orientation.Vertical:
                    this.StartingPosition.X = mirror.StartingPosition.X;
                    this.StartingPosition.Y = center.Y + center.Y - mirror.StartingPosition.Y;
                    break;
                default:
                    this.StartingPosition.X = center.X + center.X - mirror.StartingPosition.X;
                    this.StartingPosition.Y = mirror.StartingPosition.Y;
                    break;
            }

            this.StartingPressure = mirror.StartingPressure;

            // Ink
            this.StartingSize = mirror.StartingSize;

            // Stroke
            this.Bounds = RectExtensions.GetRect(this.StartingPosition, this.StartingSize);
        }

        public StrokeCap(StrokeCap symmetry, Vector2 center)
        {
            // Paint
            this.StartingPosition = center + center - symmetry.StartingPosition;

            this.StartingPressure = symmetry.StartingPressure;

            // Ink
            this.StartingSize = symmetry.StartingSize;

            // Stroke
            this.Bounds = RectExtensions.GetRect(this.StartingPosition, this.StartingSize);
        }

        public StrokeCap(StrokeCap symmetry, Matrix3x2 matrix)
        {
            // Paint
            this.StartingPosition = Vector2.Transform(symmetry.StartingPosition, matrix);

            this.StartingPressure = symmetry.StartingPressure;

            // Ink
            this.StartingSize = symmetry.StartingSize;

            // Stroke
            this.Bounds = RectExtensions.GetRect(this.StartingPosition, this.StartingSize);
        }
    }
}