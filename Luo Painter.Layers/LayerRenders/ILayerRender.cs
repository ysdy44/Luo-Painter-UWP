﻿using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Luo_Painter.Layers
{
    public interface ILayerRender
    {
        ICanvasImage Render(ICanvasImage background);
        ICanvasImage ReplaceRender(ICanvasImage background, string id, ICanvasImage mezzanine);
        ICanvasImage AboveRender(ICanvasImage background, string id, ICanvasImage mezzanine);

        ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode);
        ICanvasImage ReplaceRender(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine);
        ICanvasImage AboveRender(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine);

        ICanvasImage Render(ICanvasImage previousImage, ICanvasImage currentImage);
    }
}