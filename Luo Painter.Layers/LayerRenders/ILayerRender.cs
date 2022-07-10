using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Luo_Painter.Layers
{
    public interface ILayerRender
    {
        ICanvasImage Render(ICanvasImage background);
        ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode);
        ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine);

        ICanvasImage Render(ICanvasImage previousImage, ICanvasImage currentImage);
    }
}