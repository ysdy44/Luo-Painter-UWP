using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
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

        Visibility Visibility { get; set; }
        bool IsSelected { get; set; }

        ICanvasImage Origin { get; }
        ICanvasImage Source { get; }
        ICanvasImage Temp { get; }
        ImageSource Thumbnail { get; }
        
        IHistory GetVisibilityHistory();
        bool Undo(IHistory history);
        bool Redo(IHistory history);

    }
}