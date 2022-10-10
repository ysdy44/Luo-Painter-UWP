using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public interface IRender
    {
        Visibility Visibility { get; set; }

        string Name { get; set; }
        string StartingName { get; }
        void CacheName();

        float Opacity { get; set; }
        float StartingOpacity { get; }
        void CacheOpacity();

        BlendEffectMode? BlendMode { get; set; }
        BlendEffectMode? StartingBlendMode { get; }
        void CacheBlendMode();

        void CopyWith(LayerBase layerBase);
    }
}