using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Layers
{
    public interface ILayer : INotifyPropertyChanged
    {
        string Id { get; }

        LayerType Type { get; }
        string Name { get; set; }
        BlendEffectMode? BlendMode { get; set; }

        float Opacity { get; set; }
        float StartingOpacity { get; }
        void CacheOpacity();

        Visibility Visibility { get; set; }

        ICanvasImage Origin { get; }
        ICanvasImage Source { get; }
        ICanvasImage Temp { get; }
        ImageSource Thumbnail { get; }

        IHistory GetBlendModeHistory(BlendEffectMode? mode);
        IHistory GetOpacityHistory();
        IHistory GetVisibilityHistory();
        bool Undo(IHistory history);
        bool Redo(IHistory history);

    }
}