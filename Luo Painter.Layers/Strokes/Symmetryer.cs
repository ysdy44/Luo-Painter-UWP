using System.Collections.Generic;
using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Layers
{
    public enum SymmetryMode : byte
    {
        None = 0,

        Horizontal,
        Vertical,
        Symmetry,
        Mirror
    }

    public enum SymmetryType : byte
    {
        None = 0,

        Horizontal = 1,
        Vertical = 2,
        Symmetry = 4,
        
        Radial = 8,
        RadialHorizontal = Radial | Horizontal,

        HorizontalVerticalSymmetry = Symmetry | Horizontal | Vertical,
    }

    public sealed class Symmetryer : List<Matrix3x2>
    {

        public void Construct(int count, Vector2 center)
        {
            base.Clear();

            if (count > 2)
            {
                // Only 1~count, Because 0 is self
                for (int i = 1; i < count; i++)
                {
                    base.Add(Matrix3x2.CreateRotation(FanKit.Math.PiTwice / count * i, center));
                }
            }
        }

        public SymmetryType GetType(SymmetryMode mode, int count)
        {
            switch (mode)
            {
                case SymmetryMode.None: return SymmetryType.None;
                case SymmetryMode.Horizontal: return SymmetryType.Horizontal;
                case SymmetryMode.Vertical: return SymmetryType.Vertical;
                case SymmetryMode.Symmetry: return (count is 2) ? SymmetryType.Symmetry : (count > 2) ? SymmetryType.Radial : SymmetryType.None;
                case SymmetryMode.Mirror: return (count is 2) ? SymmetryType.HorizontalVerticalSymmetry : (count > 2) ? SymmetryType.RadialHorizontal : SymmetryType.None;
                default: return SymmetryType.None;
            }
        }

        public IEnumerable<StrokeCap> GetCaps(SymmetryType type, StrokeCap cap, Vector2 center)
        {
            yield return cap;
            if (type.HasFlag(SymmetryType.Horizontal)) yield return new StrokeCap(cap, center, Orientation.Horizontal);
            if (type.HasFlag(SymmetryType.Vertical)) yield return new StrokeCap(cap, center, Orientation.Vertical);
            if (type.HasFlag(SymmetryType.Symmetry)) yield return new StrokeCap(cap, center);

            if (type.HasFlag(SymmetryType.Radial))
            {
                foreach (Matrix3x2 item in this)
                {
                    cap = new StrokeCap(cap, item);

                    yield return cap;
                    if (type.HasFlag(SymmetryType.Horizontal)) yield return new StrokeCap(cap, center, Orientation.Horizontal);
                    if (type.HasFlag(SymmetryType.Vertical)) yield return new StrokeCap(cap, center, Orientation.Vertical);
                    if (type.HasFlag(SymmetryType.Symmetry)) yield return new StrokeCap(cap, center);
                }
            }
        }

        public IEnumerable<StrokeSegment> GetSegments(SymmetryType type, StrokeSegment segment, Vector2 center)
        {
            yield return segment;
            if (type.HasFlag(SymmetryType.Horizontal)) yield return new StrokeSegment(segment, center, Orientation.Horizontal);
            if (type.HasFlag(SymmetryType.Vertical)) yield return new StrokeSegment(segment, center, Orientation.Vertical);
            if (type.HasFlag(SymmetryType.Symmetry)) yield return new StrokeSegment(segment, center);

            if (type.HasFlag(SymmetryType.Radial))
            {
                foreach (Matrix3x2 item in this)
                {
                    segment = new StrokeSegment(segment, item);

                    yield return segment;
                    if (type.HasFlag(SymmetryType.Horizontal)) yield return new StrokeSegment(segment, center, Orientation.Horizontal);
                    if (type.HasFlag(SymmetryType.Vertical)) yield return new StrokeSegment(segment, center, Orientation.Vertical);
                    if (type.HasFlag(SymmetryType.Symmetry)) yield return new StrokeSegment(segment, center);
                }
            }
        }

    }
}