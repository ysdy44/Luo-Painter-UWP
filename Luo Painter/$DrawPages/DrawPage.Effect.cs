using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private ICanvasImage GetPreview(OptionType type, ICanvasImage image)
        {
            switch (type)
            {
                case OptionType.None:
                    return image;

                case OptionType.Transform:
                    return this.GetTransformPreview(image);
                case OptionType.DisplacementLiquefaction:
                    return this.GetDisplacementLiquefactionPreview(image);
                case OptionType.GradientMapping:
                    return this.GetGradientMappingPreview(image);
                case OptionType.RippleEffect:
                    return this.GetRippleEffectPreview(image);

                case OptionType.MarqueeTransform:
                    return this.GetTransformPreview(image);

                default:
                    return this.AppBar.GetPreview(type, image);
            }
        }

    }
}