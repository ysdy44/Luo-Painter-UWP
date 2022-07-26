using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public sealed partial class LayerNodes : List<ILayer>, ILayerRender
    {
        public Layerage[] Convert() => (base.Count is 0) ? null : this.Select(this.Convert).ToArray();
        private Layerage Convert(ILayer layer) => new Layerage
        {
            Id = layer.Id,
            Children = layer.Children.Convert()
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
        public ICanvasImage Merge(ILayerRender previousRender, ICanvasImage previousImage) => this.Render(previousImage);

    }
}