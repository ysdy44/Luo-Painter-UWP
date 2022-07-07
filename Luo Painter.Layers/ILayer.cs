using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Layers
{
    public interface ILayer : IRender, IDrag, ISetup, INotifyPropertyChanged, ICommand
    {
        string Id { get; }
        IList<ILayer> Children { get; }

        LayerType Type { get; }

        ICanvasImage GetRender(ICanvasImage background);
        ICanvasImage Render(ICanvasImage previousImage, ICanvasImage currentImage);

        ICanvasImage this[BitmapType type] { get; }
        ImageSource Thumbnail { get; }

        bool History(HistoryType type, object parameter);

        bool FillContainsPoint(Vector2 point);
    }
}