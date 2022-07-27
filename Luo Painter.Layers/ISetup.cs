using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Graphics.Imaging;

namespace Luo_Painter.Layers
{
    public interface ISetup
    {
        ILayer Clone(ICanvasResourceCreator resourceCreator);

        ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height);
        ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset);
        ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation);

        ILayer Skretch(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation);
        ILayer Flip(ICanvasResourceCreator resourceCreator, BitmapFlip flip);
        ILayer Rotation(ICanvasResourceCreator resourceCreator, BitmapRotation rotation);
       
        ILayer FlipHorizontal(ICanvasResourceCreator resourceCreator);
        ILayer FlipVertical(ICanvasResourceCreator resourceCreator);
        ILayer LeftTurn(ICanvasResourceCreator resourceCreator);
        ILayer RightTurn(ICanvasResourceCreator resourceCreator);
        ILayer OverTurn(ICanvasResourceCreator resourceCreator);
    }
}