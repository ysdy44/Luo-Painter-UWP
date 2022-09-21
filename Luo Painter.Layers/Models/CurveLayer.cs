﻿using Microsoft.Graphics.Canvas;
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
        public ICanvasImage this[BitmapType type] => this.Anchors.Source;

        public readonly AnchorCollection Anchors;


        public CurveLayer(ICanvasResourceCreator resourceCreator, int width, int height) : base(null, resourceCreator, width, height)
        {
            this.Anchors = new AnchorCollection(resourceCreator, width, height);
        }
        private CurveLayer(ICanvasResourceCreator resourceCreator, AnchorCollection anchors, int width, int height) : base(null, resourceCreator, width, height)
        {
            this.Anchors = anchors;
        }


        public ICanvasImage Render(ICanvasImage background) => base.Render(background, this.Anchors.Source);
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = this.Anchors.Source,
        });
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine) => base.Render(background, new Transform2DEffect
        {
            InterpolationMode = interpolationMode,
            TransformMatrix = matrix,
            Source = (base.Id == id) ? mezzanine : this.Anchors.Source,
        });

        public ICanvasImage Merge(ILayerRender previousRender, ICanvasImage previousImage)
        {
            if (base.Opacity == 0.0) return null;
            else if (base.Opacity == 1.0) return previousRender.Render(previousImage, this.Anchors.Source);
            return previousRender.Render(previousImage, new OpacityEffect
            {
                Opacity = base.Opacity,
                Source = this.Anchors.Source
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

            Color target = this.Anchors.GetPixelColors(px, py, 1, 1).Single();
            if (target.A is byte.MinValue) return false;

            return true;
        }


        protected override ILayer CloneSelf(ICanvasResourceCreator resourceCreator) => new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, this.Width, this.Height), this.Width, this.Height);

        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height) => new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, width, height), width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset) => new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, width, height, offset), width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation) => new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, width, height, matrix), width, height);

        protected override ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, width, height, Matrix3x2.CreateScale(width / (float)this.Width, height / (float)this.Height)), width, height);
        protected override ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip)
        {
            switch (flip)
            {
                case BitmapFlip.Horizontal:
                    return new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(-1, 1, this.Center)), this.Width, this.Height);
                case BitmapFlip.Vertical:
                    return new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(1, -1, this.Center)), this.Width, this.Height);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }
        protected override ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            switch (rotation)
            {
                case BitmapRotation.Clockwise90Degrees: // RightTurn
                    return new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, this.Height, this.Width, Matrix3x2.CreateRotation(FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(this.Height, 0)), this.Width, this.Height);
                case BitmapRotation.Clockwise180Degrees: // OverTurn
                    return new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(-1, -1, this.Center)), this.Width, this.Height);
                case BitmapRotation.Clockwise270Degrees: // LeftTurn
                    return new CurveLayer(resourceCreator, this.Anchors.Clone(resourceCreator, this.Height, this.Width, Matrix3x2.CreateRotation(-FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(0, this.Width)), this.Width, this.Height);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }

    }
}