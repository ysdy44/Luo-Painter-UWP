using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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

    /// <summary>
    /// AlignmentGrid is used to display a grid to help aligning controls
    /// </summary>
    public abstract class AlignmentGrid : Canvas
    {
        //@Abstract
        protected abstract IEnumerable<Line> Lines(int width, int height);

        /// <summary>
        /// Step for <see cref="AlignmentGrid"/>'s lines, Default 20.
        /// </summary>
        public const int Step = 20;

        protected readonly SolidColorBrush LineBrush = new SolidColorBrush(Color.FromArgb(38, 127, 127, 127));

        /// <summary>
        /// Column for <see cref="AlignmentGrid"/>'s lines, Default 0.
        /// </summary>
        public int Column { get; private set; }
        /// <summary>
        /// Row for <see cref="AlignmentGrid"/>'s lines, Default 0.
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// Rebuild all lines by interpolated size.
        /// </summary>
        /// <param name="size"> The source size. </param>
        /// <param name="lazy"> The lazy space. </param>
        /// <returns><c>true</c> if the change is greater than lazy space; otherwise, <c>false</c>.</returns>
        public bool RebuildWithInterpolation(Size size, int lazy = 5)
        {
            // 1. Interpolation
            int column = lazy + (int)(size.Width / AlignmentGrid.Step);
            int row = lazy + (int)(size.Height / AlignmentGrid.Step);

            // 2. Column & Row
            if (System.Math.Abs(this.Column - column) + System.Math.Abs(this.Row - row) < lazy) return false;
            this.Rebuild(column, row);
            return true;
        }

        /// <summary>
        /// Rebuild all lines.
        /// </summary>
        /// <param name="column"> The source column. </param>
        /// <param name="row"> The source row. </param>
        public void Rebuild(int column, int row)
        {
            this.Column = column;
            this.Row = row;

            // 3. Width & Height
            int width = this.Column * AlignmentGrid.Step;
            int height = this.Row * AlignmentGrid.Step;
            base.Width = width;
            base.Height = height;

            // 4. Lines
            base.Children.Clear();
            foreach (Line item in this.Lines(width, height))
            {
                base.Children.Add(item);
            }
        }

    }
}