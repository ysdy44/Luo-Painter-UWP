using System.Collections.Generic;
using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Models
{
    public enum SymmetryMode
    {
        /// <summary> Nornal. </summary>
        None = 0,

        /// <summary> Mirror X. </summary>
        SymmetryX,
        /// <summary> Mirror Y. </summary>
        SymmetryY,

        /// <summary> Symmetry Radial. </summary>
        SymmetryRadial,

        /// <summary> Mirror Radial. </summary>
        MirrorRadial
    }

    public enum SymmetryType
    {
        None = 0,

        X = 1, // Axis-X
        Y = 2, // Axis-Y

        D = 4, // Diagonal
        DXY = D | X | Y,

        R = 8, // Radial
        RX = R | X,
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
                    base.Add(Matrix3x2.CreateRotation(System.MathF.PI * 2 / count * i, center));
                }
            }
        }

        public SymmetryType GetType(SymmetryMode mode, int count)
        {
            switch (mode)
            {
                case SymmetryMode.None: return SymmetryType.None;
                case SymmetryMode.SymmetryX: return SymmetryType.X;
                case SymmetryMode.SymmetryY: return SymmetryType.Y;
                case SymmetryMode.SymmetryRadial: return (count is 2) ? SymmetryType.D : (count > 2) ? SymmetryType.R : SymmetryType.None;
                case SymmetryMode.MirrorRadial: return (count is 2) ? SymmetryType.DXY : (count > 2) ? SymmetryType.RX : SymmetryType.None;
                default: return SymmetryType.None;
            }
        }

        public IEnumerable<StrokeCap> GetCaps(SymmetryType type, StrokeCap cap, Vector2 center)
        {
            yield return cap;
            if (type.HasFlag(SymmetryType.X)) yield return new StrokeCap(cap, center, Orientation.Horizontal);
            if (type.HasFlag(SymmetryType.Y)) yield return new StrokeCap(cap, center, Orientation.Vertical);
            if (type.HasFlag(SymmetryType.D)) yield return new StrokeCap(cap, center);

            if (type.HasFlag(SymmetryType.R))
            {
                foreach (Matrix3x2 item in this)
                {
                    cap = new StrokeCap(cap, item);

                    yield return cap;
                    if (type.HasFlag(SymmetryType.X)) yield return new StrokeCap(cap, center, Orientation.Horizontal);
                    if (type.HasFlag(SymmetryType.Y)) yield return new StrokeCap(cap, center, Orientation.Vertical);
                    if (type.HasFlag(SymmetryType.D)) yield return new StrokeCap(cap, center);
                }
            }
        }

        public IEnumerable<StrokeSegment> GetSegments(SymmetryType type, StrokeSegment segment, Vector2 center)
        {
            yield return segment;
            if (type.HasFlag(SymmetryType.X)) yield return new StrokeSegment(segment, center, Orientation.Horizontal);
            if (type.HasFlag(SymmetryType.Y)) yield return new StrokeSegment(segment, center, Orientation.Vertical);
            if (type.HasFlag(SymmetryType.D)) yield return new StrokeSegment(segment, center);

            if (type.HasFlag(SymmetryType.R))
            {
                foreach (Matrix3x2 item in this)
                {
                    segment = new StrokeSegment(segment, item);

                    yield return segment;
                    if (type.HasFlag(SymmetryType.X)) yield return new StrokeSegment(segment, center, Orientation.Horizontal);
                    if (type.HasFlag(SymmetryType.Y)) yield return new StrokeSegment(segment, center, Orientation.Vertical);
                    if (type.HasFlag(SymmetryType.D)) yield return new StrokeSegment(segment, center);
                }
            }
        }

    }
}