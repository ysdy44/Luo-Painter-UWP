using System.Collections.Generic;
using Windows.UI.Xaml.Shapes;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// AlignmentGrid with Lines.
    /// </summary>
    public sealed class AlignmentGridWithLines : AlignmentGrid
    {
        protected override IEnumerable<Line> Lines(int width, int height)
        {
            for (int x = 0; x < base.Column; x++)
            {
                int left = x * AlignmentGrid.Step;
                yield return new Line
                {
                    X1 = left,
                    X2 = left,
                    Y1 = 0,
                    Y2 = height,
                    StrokeThickness = 1,
                    Stroke = base.LineBrush
                };
            }

            for (int y = 0; y < base.Row; y++)
            {
                int top = y * AlignmentGrid.Step;
                yield return new Line
                {
                    X1 = 0,
                    X2 = width,
                    Y1 = top,
                    Y2 = top,
                    StrokeThickness = 1,
                    Stroke = this.LineBrush
                };
            }
        }
    }
}