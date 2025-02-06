using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class CurveLayer : LayerBase, ILayer
    {
        internal readonly CanvasRenderTarget SourceRenderTarget;
        public ICanvasImage Source => this.SourceRenderTarget;
        public Color[] GetPixelColors(int left, int top, int width, int height) => this.SourceRenderTarget.GetPixelColors(left, top, width, height);

        public LayerType Type => LayerType.Curve;
        public ICanvasImage this[BitmapType type] => this.Source;


        //@Construct
        public CurveLayer(ICanvasResourceCreator resourceCreator, int width, int height) : base(null, null, resourceCreator, width, height)
        {
            //@DPI
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.Anchorss = new List<AnchorCollection>();
        }
        private CurveLayer(ICanvasResourceCreator resourceCreator, IEnumerable<AnchorCollection> anchors, int width, int height) : base(null, null, resourceCreator, width, height)
        {
            //@DPI
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.Anchorss = new List<AnchorCollection>(anchors);
            this.Invalidate();
        }
        public CurveLayer(string id, XElement element, ICanvasResourceCreator resourceCreator, int width, int height) : base(id, element, resourceCreator, width, height)
        {
            //@DPI
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);

            if (element is null is false)
            {
                if (element.Element("Data") is XElement data)
                {
                    this.Anchorss = new List<AnchorCollection>
                    (
                        from item
                        in data.Elements("AnchorCollection")
                        select new AnchorCollection(item, resourceCreator, width, height)
                    );
                    this.Invalidate();
                    return;
                }
            }

            this.Anchorss = new List<AnchorCollection>();
        }

        public XElement Save()
        {
            XElement element = base.Save(this.Type);
            element.Add(new XElement("Data", this.Anchorss.Select(c => c.Save())));
            return element;
        }


        public ICanvasImage Render(ICanvasImage background) => base.Render(background, this.Source);
        public ICanvasImage ReplaceRender(ICanvasImage background, string id, ICanvasImage mezzanine) => base.Render(background,
            (base.Id == id) ? mezzanine : this.Source);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = this.Source
        });
        public ICanvasImage ReplaceRender(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = (base.Id == id) ? mezzanine : this.Source
        });


        public bool FillContainsPoint(Vector2 point)
        {
            int px = (int)point.X;
            int py = (int)point.Y;

            if (px < 0) return false;
            if (py < 0) return false;

            if (px >= base.Width) return false;
            if (py >= base.Height) return false;

            return true;

            Color target = this.GetPixelColors(px, py, 1, 1).Single();
            if (target.A is byte.MinValue) return false;

            return true;
        }


        protected override ILayer CloneSelf(ICanvasResourceCreator resourceCreator) => new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, base.Width, base.Height), base.Width, base.Height);

        protected override ILayer OffsetSelf(ICanvasResourceCreator resourceCreator, Vector2 offset) => new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, base.Width, base.Height, offset), base.Width, base.Height);

        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height) => new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, width, height), width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset) => new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, width, height, offset), width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation) => new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, width, height, matrix), width, height);

        protected override ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, width, height, Matrix3x2.CreateScale(width / (float)base.Width, height / (float)base.Height)), width, height);
        protected override ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip)
        {
            switch (flip)
            {
                case BitmapFlip.Horizontal:
                    return new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(-1, 1, this.Center)), base.Width, base.Height);
                case BitmapFlip.Vertical:
                    return new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(1, -1, this.Center)), base.Width, base.Height);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }
        protected override ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            switch (rotation)
            {
                case BitmapRotation.Clockwise90Degrees: // RightTurn
                    return new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, base.Height, base.Width, Matrix3x2.CreateRotation(FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(base.Height, 0)), base.Width, base.Height);
                case BitmapRotation.Clockwise180Degrees: // OverTurn
                    return new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(-1, -1, this.Center)), base.Width, base.Height);
                case BitmapRotation.Clockwise270Degrees: // LeftTurn
                    return new CurveLayer(resourceCreator, from a in this.Anchorss select a.Clone(resourceCreator, base.Height, base.Width, Matrix3x2.CreateRotation(-FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(0, base.Width)), base.Width, base.Height);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            this.SourceRenderTarget.Dispose();
            foreach (AnchorCollection item in this.Anchorss)
            {
                item.Dispose();
            }
        }
    }
}