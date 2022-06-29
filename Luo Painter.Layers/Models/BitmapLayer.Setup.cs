using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public ILayer Clone(ICanvasResourceCreator resourceCreator)
        {
            BitmapLayer layer = new BitmapLayer(resourceCreator, this);
            layer.CopyWith(this);
            return layer;
        }
        public ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset)
        {
            BitmapLayer layer = new BitmapLayer(resourceCreator, this.SourceRenderTarget, width, height, offset);
            layer.CopyWith(this);
            return layer;
        }
        public ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation)
        {
            BitmapLayer layer = new BitmapLayer(resourceCreator, new Transform2DEffect
            {
                Source = this.SourceRenderTarget,
                TransformMatrix = matrix,
                InterpolationMode = interpolation
            }, width, height);
            layer.CopyWith(this);
            return layer;
        }
        public ILayer Skretch(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation) => this.Crop(resourceCreator, width, height, Matrix3x2.CreateScale(width / (float)this.Width, height / (float)this.Height), interpolation);
        public ILayer FlipHorizontal(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(-1, 1, this.Center), CanvasImageInterpolation.NearestNeighbor);
        public ILayer FlipVertical(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);
        public ILayer LeftTurn(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Height, this.Width, Matrix3x2.CreateRotation(-FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(0, this.Width), CanvasImageInterpolation.NearestNeighbor);
        public ILayer RightTurn(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Height, this.Width, Matrix3x2.CreateRotation(FanKit.Math.PiOver2) * Matrix3x2.CreateTranslation(this.Height, 0), CanvasImageInterpolation.NearestNeighbor);
        public ILayer OverTurn(ICanvasResourceCreator resourceCreator) => this.Crop(resourceCreator, this.Width, this.Height, Matrix3x2.CreateScale(-1, -1, this.Center), CanvasImageInterpolation.NearestNeighbor);

    }
}