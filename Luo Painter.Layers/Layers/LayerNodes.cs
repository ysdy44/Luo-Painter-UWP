using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public sealed class LayerNodes : List<ILayer>, ILayerRender
    {
        public Layerage[] Convert() => LayerNodes.Convert(this);
        private static Layerage[] Convert(IList<ILayer> layers) => (layers.Count is 0) ? null : layers.Select(LayerNodes.Convert).ToArray();
        private static Layerage Convert(ILayer layer) => new Layerage
        {
            Id = layer.Id,
            Children = LayerNodes.Convert(layer.Children)
        };

        public ICanvasImage Render(ICanvasImage background)
        {
            for (int i = base.Count - 1; i >= 0; i--)
            {
                ILayer item = base[i];
                switch (item.Visibility)
                {
                    case Visibility.Visible:
                        background = item.Render(background);
                        break;
                }
            }
            return background;
        }
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode)
        {
            for (int i = base.Count - 1; i >= 0; i--)
            {
                ILayer item = base[i];
                switch (item.Visibility)
                {
                    case Visibility.Visible:
                        background = item.Render(background, matrix, interpolationMode);
                        break;
                }
            }
            return background;
        }
        public ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine)
        {
            for (int i = base.Count - 1; i >= 0; i--)
            {
                ILayer item = base[i];
                switch (item.Visibility)
                {
                    case Visibility.Visible:
                        background = item.Render(background, matrix, interpolationMode, id, mezzanine);
                        break;
                }
            }
            return background;
        }

        public ICanvasImage Render(ICanvasImage previousImage, ICanvasImage currentImage) => new CompositeEffect
        {
            Sources =
            {
                previousImage,
                currentImage
            }
        };
    }
}