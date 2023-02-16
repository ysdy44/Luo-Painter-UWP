using FanKit.Transformers;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Luo_Painter.Blends
{
    public sealed partial class Anchor : Node, IDisposable
    {
        /// <summary> Pressure. </summary>
        public float Pressure { get; set; } = 1;
        /// <summary> The cache of <see cref="Anchor.Pressure"/>. </summary>
        public float StartingPressure { get; private set; } = 1;

        /// <summary> Geometric Shapes. </summary>
        public CanvasGeometry Geometry { get; private set; }
        internal float ComputePathLength { get; private set; }
        internal IList<Vector3> Strokes { get; } = new List<Vector3>();

        /// <summary>
        /// Get own copy.
        /// </summary>
        /// <returns> The cloned anchor. </returns>
        public new Anchor Clone()
        {
            return new Anchor
            {
                Pressure = this.Pressure,

                Point = base.Point,
                LeftControlPoint = base.LeftControlPoint,
                RightControlPoint = base.RightControlPoint,

                IsChecked = base.IsChecked,
                IsSmooth = base.IsSmooth,
            };
        }
        /// <summary>
        /// Get own copy.
        /// </summary>
        /// <param name="vector"> The add value use to summed. </param>
        /// <returns> The cloned anchor. </returns>
        public Anchor Clone(Vector2 vector)
        {
            return new Anchor
            {
                Pressure = this.Pressure,

                Point = base.Point + vector,
                LeftControlPoint = base.LeftControlPoint + vector,
                RightControlPoint = base.RightControlPoint + vector,

                IsChecked = base.IsChecked,
                IsSmooth = base.IsSmooth,
            };
        }
        /// <summary>
        /// Get own copy.
        /// </summary>
        /// <param name="matrix"> The resulting matrix. </param>
        /// <returns> The cloned anchor. </returns>
        public Anchor Clone(Matrix3x2 matrix)
        {
            return new Anchor
            {
                Pressure = this.Pressure,
                StartingPressure = this.StartingPressure,

                Point = Vector2.Transform(base.Point, matrix),
                LeftControlPoint = Vector2.Transform(base.LeftControlPoint, matrix),
                RightControlPoint = Vector2.Transform(base.RightControlPoint, matrix),

                IsChecked = base.IsChecked,
                IsSmooth = base.IsSmooth,
            };
        }

        public void Dispose()
        {
            this.Geometry?.Dispose();
            this.Geometry = null;
            this.ComputePathLength = 0;

            this.Strokes.Clear();
        }
    }
}