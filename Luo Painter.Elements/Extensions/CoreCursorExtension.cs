using System;
using System.Numerics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Provides constant and static member 
    /// for <see cref="Window.Current.CoreWindow.PointerCursor"/>.
    /// </summary>
    public static class CoreCursorExtension
    {

        public static CoreCursorType ToCursorType(this Vector2 vector, int offset = 0)
        {
            // 0~360
            double angle = Math.Atan2(vector.X, vector.Y) * 180 / Math.PI;
            double angle2 = (angle + 360) % 360;

            // 0~8
            double index = angle2 * 8 / 360;
            int index2 = offset + (int)Math.Round(index, MidpointRounding.ToEven);

            // 0~4
            int i = 7 + index2 % 4;

            // 7~11
            return (CoreCursorType)i;
        }

        public static CoreCursorType ToCursorType(this Vector2 vector, Orientation orientation)
        {
            // 0~360
            double angle = Math.Atan2(vector.X, vector.Y) * 180 / Math.PI;
            double angle2 = (angle + 360) % 360;

            // 0~8
            double index = angle2 * 8 / 360;
            int index2 = (int)Math.Round(index, MidpointRounding.ToEven);

            // 0~4
            int i = index2 % 4;

            switch (orientation)
            {
                //      ↑
                // ←     →
                //      ↓ 
                case Orientation.Vertical:
                    switch (i)
                    {
                        case 0: return CoreCursorType.SizeNorthSouth; // ↓
                        case 1: return CoreCursorType.SizeNorthwestSoutheast; // ↖
                        case 2: return CoreCursorType.SizeWestEast; // →
                        case 3: return CoreCursorType.SizeNortheastSouthwest; // ↗
                        default: return default;
                    }
                // ↗ → ↘
                // ↑         ↓
                // ↖ ← ↙
                case Orientation.Horizontal:
                    switch (i)
                    {
                        case 0: return CoreCursorType.SizeWestEast; // →
                        case 1: return CoreCursorType.SizeNortheastSouthwest; // ↗
                        case 2: return CoreCursorType.SizeNorthSouth; // ↓
                        case 3: return CoreCursorType.SizeNorthwestSoutheast; // ↖
                        default: return default;
                    }
                default:
                    return default;
            }
        }

    }
}