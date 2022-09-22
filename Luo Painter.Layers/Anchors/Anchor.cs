using FanKit.Transformers;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Luo_Painter.Layers
{
    public sealed partial class Anchor : Node, IDisposable
    {
        /// <summary> Pressure. </summary>
        public float Pressure { get; set; } = 1;
        /// <summary> The cache of <see cref="Anchor.Pressure"/>. </summary>
        public float StartingPressure { get; private set; } = 1;

        public CanvasGeometry Geometry { get; private set; }
        public float ComputePathLength { get; private set; }

        public IList<Vector3> Strokes { get; } = new List<Vector3>();

        /// <summary>
        /// Get own copy.
        /// </summary>
        /// <returns> The cloned anchor. </returns>
        public new Anchor Clone()
        {
            return new Anchor
            {
                Pressure = this.Pressure,

                Point = this.Point,
                LeftControlPoint = this.LeftControlPoint,
                RightControlPoint = this.RightControlPoint,

                IsChecked = this.IsChecked,
                IsSmooth = this.IsSmooth,
            };
        }
        public Anchor Clone(Vector2 offset)
        {
            return new Anchor
            {
                Pressure = this.Pressure,

                Point = this.Point + offset,
                LeftControlPoint = this.LeftControlPoint + offset,
                RightControlPoint = this.RightControlPoint + offset,

                IsChecked = this.IsChecked,
                IsSmooth = this.IsSmooth,
            };
        }
        public Anchor Clone(Matrix3x2 matrix)
        {
            return new Anchor
            {
                Pressure = this.Pressure,
                StartingPressure = this.StartingPressure,

                Point = Vector2.Transform(this.Point, matrix),
                LeftControlPoint = Vector2.Transform(this.LeftControlPoint, matrix),
                RightControlPoint = Vector2.Transform(this.RightControlPoint, matrix),

                IsChecked = this.IsChecked,
                IsSmooth = this.IsSmooth,
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