using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Graphics.Imaging;

namespace Luo_Painter.Layers.Models
{
    partial class BitmapLayer 
    {

        protected override ILayer CloneSelf(ICanvasResourceCreator resourceCreator) => new BitmapLayer(resourceCreator, this);

        protected override ILayer OffsetSelf(ICanvasResourceCreator resourceCreator, Vector2 offset) => new BitmapLayer(resourceCreator, this.SourceRenderTarget, offset);

        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height) => new BitmapLayer(resourceCreator, this.SourceRenderTarget, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset) => new BitmapLayer(resourceCreator, this.SourceRenderTarget, width, height, offset);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation) => new BitmapLayer(resourceCreator, new Transform2DEffect
        {
            Source = this.SourceRenderTarget,
            TransformMatrix = matrix,
            InterpolationMode = interpolation
        }, width, height);

        protected override ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => this.CropSelf(resourceCreator, width, height, Matrix3x2.CreateScale(width / (float)base.Width, height / (float)base.Height), interpolation);
        protected override ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip)
        {
            switch (flip)
            {
                case BitmapFlip.Horizontal:
                    return this.CropSelf(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(-1, 1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                case BitmapFlip.Vertical:
                    return this.CropSelf(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }
        protected override ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            switch (rotation)
            {
                case BitmapRotation.Clockwise90Degrees: // RightTurn
                    return this.CropSelf(resourceCreator, base.Height, base.Width, Matrix3x2.CreateRotation(FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(base.Height, 0), CanvasImageInterpolation.NearestNeighbor);
                case BitmapRotation.Clockwise180Degrees: // OverTurn
                    return this.CropSelf(resourceCreator, base.Width, base.Height, Matrix3x2.CreateScale(-1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                case BitmapRotation.Clockwise270Degrees: // LeftTurn
                    return this.CropSelf(resourceCreator, base.Height, base.Width, Matrix3x2.CreateRotation(-FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(0, base.Width), CanvasImageInterpolation.NearestNeighbor);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }

    }
}