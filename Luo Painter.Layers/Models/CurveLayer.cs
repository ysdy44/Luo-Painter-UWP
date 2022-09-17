using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Linq;
using System.Numerics;
using Windows.Graphics.Imaging;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class CurveLayer : LayerBase, ILayer
    {

        public LayerType Type => LayerType.Curve;
        public ICanvasImage this[BitmapType type] => this.Source;

        readonly CanvasRenderTarget Source;
        public readonly AnchorCollection Anchors;

        public Color Color { get; set; } = Colors.Black;
        public float StrokeWidth { get; set; } = 1;

        public int Index { get => this.Anchors.Index; set => this.Anchors.Index = value; }
        public Anchor SelectedItem => this.Anchors.SelectedItem;

        public CurveLayer(ICanvasResourceCreator resourceCreator, int width, int height) : base(null, resourceCreator, width, height)
        {
            //@DPI
            this.Source = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.Anchors = new AnchorCollection();
        }
        public CurveLayer(ICanvasResourceCreator resourceCreator, int width, int height, AnchorCollection anchors) : base(null, resourceCreator, width, height)
        {
            //@DPI
            this.Source = new CanvasRenderTarget(resourceCreator, width, height, 96);
            this.Anchors = anchors;
        }

        private CurveLayer(ICanvasResourceCreator resourceCreator, CurveLayer curveLayer) : base(null, resourceCreator, curveLayer.Width, curveLayer.Height)
        {
            //@DPI
            this.Source = new CanvasRenderTarget(resourceCreator, curveLayer.Width, curveLayer.Height, 96);

            this.Anchors = new AnchorCollection();
            foreach (Anchor item in curveLayer.Anchors)
            {
                this.Anchors.Add(item.Clone());
            }

            this.Color = curveLayer.Color;
            this.StrokeWidth = curveLayer.StrokeWidth;

            this.BuildGeometry(resourceCreator);
        }
        private CurveLayer(ICanvasResourceCreator resourceCreator, CurveLayer curveLayer, int width, int height) : base(null, resourceCreator, width, height)
        {
            //@DPI
            this.Source = new CanvasRenderTarget(resourceCreator, width, height, 96);

            this.Anchors = new AnchorCollection();
            foreach (Anchor item in curveLayer.Anchors)
            {
                this.Anchors.Add(item.Clone());
            }

            this.Color = curveLayer.Color;
            this.StrokeWidth = curveLayer.StrokeWidth;

            this.BuildGeometry(resourceCreator);
        }
        private CurveLayer(ICanvasResourceCreator resourceCreator, CurveLayer curveLayer, int width, int height, Vector2 offset) : base(null, resourceCreator, width, height)
        {
            //@DPI
            this.Source = new CanvasRenderTarget(resourceCreator, width, height, 96);

            this.Anchors = new AnchorCollection();
            foreach (Anchor item in curveLayer.Anchors)
            {
                this.Anchors.Add(item.Clone(offset));
            }

            this.Color = curveLayer.Color;
            this.StrokeWidth = curveLayer.StrokeWidth;

            this.BuildGeometry(resourceCreator);
        }
        private CurveLayer(ICanvasResourceCreator resourceCreator, CurveLayer curveLayer, int width, int height, Matrix3x2 matrix) : base(null, resourceCreator, width, height)
        {
            //@DPI
            this.Source = new CanvasRenderTarget(resourceCreator, width, height, 96);

            this.Anchors = new AnchorCollection();
            foreach (Anchor item in curveLayer.Anchors)
            {
                this.Anchors.Add(item.Clone(matrix));
            }

            this.Color = curveLayer.Color;
            this.StrokeWidth = curveLayer.StrokeWidth;

            this.BuildGeometry(resourceCreator);
        }


        public void BuildGeometry(ICanvasResourceCreator resourceCreator)
        {
            if (this.Anchors.BuildGeometry(resourceCreator))
            {
                using (CanvasDrawingSession ds = this.Source.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.Clear(Colors.Transparent);
                    ds.FillAnchorCollection(this.Anchors, this.Color, this.StrokeWidth);
                }
            }
        }
        public void BuildGeometry(ICanvasResourceCreator resourceCreator, Vector2 position, bool isSmooth)
        {
            if (this.Anchors.BuildGeometry(resourceCreator, position, isSmooth))
            {
                using (CanvasDrawingSession ds = this.Source.CreateDrawingSession())
                {
                    //@DPI 
                    ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                    ds.Clear(Colors.Transparent);
                    ds.FillAnchorCollection(this.Anchors, position, this.Color, this.StrokeWidth);
                }
            }
        }


        public void RectChoose(float left, float top, float right, float bottom) => this.Anchors.RectChoose(left, top, right, bottom);
        public void BoxChoose(TransformerRect boxRect) => this.Anchors.BoxChoose(boxRect);


        public ICanvasImage Render(ICanvasImage background) => base.Render(background, this.Source);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = this.Source,
        });
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = (base.Id == id) ? mezzanine : this.Source,
        });

        public ICanvasImage Merge(ILayerRender previousRender, ICanvasImage previousImage)
        {
            if (base.Opacity == 0.0) return null;
            else if (base.Opacity == 1.0) return previousRender.Render(previousImage, this.Source);
            return previousRender.Render(previousImage, new OpacityEffect
            {
                Opacity = base.Opacity,
                Source = this.Source
            });
        }


        public bool FillContainsPoint(Vector2 point)
        {
            int px = (int)point.X;
            int py = (int)point.Y;

            if (px < 0) return false;
            if (py < 0) return false;

            if (px >= this.Width) return false;
            if (py >= this.Height) return false;

            return true;

            Color target = this.Source.GetPixelColors(px, py, 1, 1).Single();
            if (target.A is byte.MinValue) return false;

            return true;
        }


        protected override ILayer CloneSelf(ICanvasResourceCreator resourceCreator) => new CurveLayer(resourceCreator, this);

        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height) => new CurveLayer(resourceCreator, this, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset) => new CurveLayer(resourceCreator, this, width, height, offset);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation) => new CurveLayer(resourceCreator, this, width, height, matrix);

        protected override ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => new CurveLayer(resourceCreator, this, width, height, Matrix3x2.CreateScale(width / (float)this.Width, height / (float)this.Height));
        protected override ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip)
        {
            switch (flip)
            {
                case BitmapFlip.Horizontal:
                    return new CurveLayer(resourceCreator, this, this.Width, this.Height, Matrix3x2.CreateScale(-1, 1, this.Center));
                case BitmapFlip.Vertical:
                    return new CurveLayer(resourceCreator, this, this.Width, this.Height, Matrix3x2.CreateScale(1, -1, this.Center));
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }
        protected override ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            switch (rotation)
            {
                case BitmapRotation.Clockwise90Degrees: // RightTurn
                    return new CurveLayer(resourceCreator, this, this.Height, this.Width, Matrix3x2.CreateRotation(FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(this.Height, 0));
                case BitmapRotation.Clockwise180Degrees: // OverTurn
                    return new CurveLayer(resourceCreator, this, this.Width, this.Height, Matrix3x2.CreateScale(-1, -1, this.Center));
                case BitmapRotation.Clockwise270Degrees: // LeftTurn
                    return new CurveLayer(resourceCreator, this, this.Height, this.Width, Matrix3x2.CreateRotation(-FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(0, this.Width));
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }

    }
}