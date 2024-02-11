using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Graphics.Imaging;

namespace Luo_Painter.Layers
{
    partial interface ILayer
    {
        ILayer Clone(ICanvasResourceCreator resourceCreator);

        ILayer Offset(ICanvasResourceCreator resourceCreator, Vector2 offset);

        ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height);
        ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset);
        ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation);

        ILayer Skretch(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation);
        ILayer Flip(ICanvasResourceCreator resourceCreator, BitmapFlip flip);
        ILayer Rotation(ICanvasResourceCreator resourceCreator, BitmapRotation rotation);
    }
}