using System.Numerics;
using Windows.Foundation;
using Windows.UI.Text.Core;

namespace Luo_Painter.Elements
{
    public static class TextRangeExtensions
    {
        public static int First(this CoreTextRange range) => System.Math.Min(range.StartCaretPosition, range.EndCaretPosition);
        public static int Last(this CoreTextRange range) => System.Math.Max(range.StartCaretPosition, range.EndCaretPosition);
        public static int Last(this CoreTextRange range, int maxLength) => System.Math.Min(maxLength, System.Math.Max(range.StartCaretPosition, range.EndCaretPosition));
        public static int Length(this CoreTextRange range) => System.Math.Abs(range.EndCaretPosition - range.StartCaretPosition);
        public static int Length(this CoreTextRange range, int maxLength) => System.Math.Abs(System.Math.Min(maxLength, range.EndCaretPosition) - range.StartCaretPosition);
        public static bool HasSelection(this CoreTextRange range) => range.StartCaretPosition != range.EndCaretPosition;
        public static bool HitTest(this Rect rect, Vector2 position) => position.X > rect.X + rect.Width / 2;
    }
}