using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// AlignmentGrid with Spot.
    /// </summary>
    public sealed class AlignmentGridWithSpot : AlignmentGrid
    {
        protected override IEnumerable<Line> Lines(int width, int height)
        {
            for (int y = 1; y < base.Row; y++)
            {
                int top = y * AlignmentGrid.Step;
                yield return new Line
                {
                    X1 = 0,
                    Y1 = top,
                    X2 = width,
                    Y2 = top,
                    Stroke = this.LineBrush,
                    StrokeStartLineCap = PenLineCap.Flat,
                    StrokeEndLineCap = PenLineCap.Flat,
                    StrokeDashCap = PenLineCap.Flat,
                    StrokeThickness = 3,
                    StrokeDashArray =
                    {
                        1,
                        AlignmentGrid.Step / 3 - 1,
                    }
                };
            }
        }
    }
}