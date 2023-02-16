using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Layers
{
    public partial interface ILayer : ILayerRender, INotifyPropertyChanged, ICommand, IDisposable
    {
        string Id { get; }
        LayerNodes Children { get; }

        LayerType Type { get; }

        ICanvasImage this[BitmapType type] { get; }
        ImageSource Thumbnail { get; }

        XElement Save();

        bool History(HistoryType type, object parameter);

        bool FillContainsPoint(Vector2 point);
    }
}