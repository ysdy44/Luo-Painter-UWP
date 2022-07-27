using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Graphics.Imaging;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        protected override ILayer CloneSelf(ICanvasResourceCreator resourceCreator) => new BitmapLayer(resourceCreator, this);

        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height ) => new BitmapLayer(resourceCreator, this.SourceRenderTarget, width, height);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset) => new BitmapLayer(resourceCreator, this.SourceRenderTarget, width, height, offset);
        protected override ILayer CropSelf(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation) => new BitmapLayer(resourceCreator, new Transform2DEffect
        {
            Source = this.SourceRenderTarget,
            TransformMatrix = matrix,
            InterpolationMode = interpolation
        }, width, height);

        protected override ILayer SkretchSelf(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => this.Crop(resourceCreator, width, height, Matrix3x2.CreateScale(width / (float)this.Width, height / (float)this.Height), interpolation);
        protected override ILayer FlipSelf(ICanvasResourceCreator resourceCreator, BitmapFlip flip)
        {
            switch (flip)
            {
                case BitmapFlip.Horizontal:
                    return this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(-1, 1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                case BitmapFlip.Vertical:
                    return this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }
        protected override ILayer RotationSelf(ICanvasResourceCreator resourceCreator, BitmapRotation rotation)
        {
            switch (rotation)
            {
                case BitmapRotation.Clockwise90Degrees: // RightTurn
                    return this.Crop(resourceCreator, this.Height, this.Width, Matrix3x2.CreateRotation(FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(this.Height, 0), CanvasImageInterpolation.NearestNeighbor);
                case BitmapRotation.Clockwise180Degrees: // OverTurn
                    return this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(-1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);
                case BitmapRotation.Clockwise270Degrees: // LeftTurn
                    return this.Crop(resourceCreator, this.Height, this.Width, Matrix3x2.CreateRotation(-FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(0, this.Width), CanvasImageInterpolation.NearestNeighbor);
                default:
                    return this.CloneSelf(resourceCreator);
            }
        }

        protected override ILayer FlipHorizontalSelf(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(-1, 1, this.Center), CanvasImageInterpolation.NearestNeighbor);
        protected override ILayer FlipVerticalSelf(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);
        protected override ILayer LeftTurnSelf(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Height, this.Width, Matrix3x2.CreateRotation(-FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(0, this.Width), CanvasImageInterpolation.NearestNeighbor);
        protected override ILayer RightTurnSelf(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Height, this.Width, Matrix3x2.CreateRotation(FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(this.Height, 0), CanvasImageInterpolation.NearestNeighbor);
        protected override ILayer OverTurnSelf(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(-1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);

    }
}