using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public sealed partial class AnchorCollection : List<Anchor>, ICacheTransform, IDisposable
    {
        internal readonly CanvasRenderTarget SourceRenderTarget;
        public ICanvasImage Source => this.SourceRenderTarget;
        public Color[] GetPixelColors(int left, int top, int width, int height) => this.SourceRenderTarget.GetPixelColors(left, top, width, height);

        public Color Color { get; set; } = Colors.Black;
        public float StrokeWidth { get; set; } = 4;

        public bool IsClosed { get; set; }
        public Vector2 ClosePoint { get; set; }
        public bool CloseIsSmooth { get; set; }

        public int Index = -1;
        public Anchor SelectedItem => (this.Index is -1) ? null : base[this.Index];

        public AnchorCollection(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            //@DPI
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
        }

        public AnchorCollection Clone(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            AnchorCollection anchors = this.CloneCore(resourceCreator, width, height);
            foreach (Anchor item in this)
            {
                anchors.Add(item.Clone());
            }
            anchors.Segment(resourceCreator);
            anchors.Invalidate();
            return anchors;
        }
        public AnchorCollection Clone(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset)
        {
            AnchorCollection anchors = this.CloneCore(resourceCreator, width, height);
            foreach (Anchor item in this)
            {
                anchors.Add(item.Clone(offset));
            }
            anchors.Segment(resourceCreator);
            anchors.Invalidate();
            return anchors;
        }
        public AnchorCollection Clone(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix)
        {
            AnchorCollection anchors = this.CloneCore(resourceCreator, width, height);
            foreach (Anchor item in this)
            {
                anchors.Add(item.Clone(matrix));
            }
            anchors.Segment(resourceCreator);
            anchors.Invalidate();
            return anchors;
        }
        private AnchorCollection CloneCore(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            return new AnchorCollection(resourceCreator, width, height)
            {
                Color = this.Color,
                StrokeWidth = this.StrokeWidth,
                IsClosed = this.IsClosed,
                ClosePoint = this.ClosePoint,
                CloseIsSmooth = this.CloseIsSmooth,
                Index = this.Index
            };
        }

        public void Dispose()
        {
            this.Source.Dispose();
            foreach (Anchor item in this)
            {
                item.Dispose();
            }
        }
    }
}