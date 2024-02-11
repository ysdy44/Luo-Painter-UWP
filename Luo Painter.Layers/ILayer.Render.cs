using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    partial interface ILayer
    {
        Visibility Visibility { get; set; }

        string Name { get; set; }
        string StartingName { get; }
        void CacheName();

        float Opacity { get; set; }
        float StartingOpacity { get; }
        void CacheOpacity();

        BlendEffectMode BlendMode { get; set; }
        BlendEffectMode StartingBlendMode { get; }
        void CacheBlendMode();

        TagType TagType { get; set; }

        void CopyWith(LayerBase layerBase);
    }
}