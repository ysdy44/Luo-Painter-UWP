using Luo_Painter.Historys;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.ComponentModel;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Layers
{
    public interface ILayer : INotifyPropertyChanged
    {
        string Id { get; }

        LayerType Type { get; }

        string Name { get; set; }
        string StartingName { get; }
        void CacheName();

        float Opacity { get; set; }
        float StartingOpacity { get; }
        void CacheOpacity();

        BlendEffectMode? BlendMode { get; set; }
        BlendEffectMode? StartingBlendMode { get; }
        void CacheBlendMode();

        Visibility Visibility { get; set; }

        RenderMode RenderMode { get; }
        ICanvasImage Render(ICanvasImage previousImage, ICanvasImage currentImage);

        ICanvasImage this[BitmapType type] { get; }
        ImageSource Thumbnail { get; }

        bool History(HistoryType type, object parameter);

        bool FillContainsPoint(Vector2 point);

        ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset);
        ILayer Crop(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix, CanvasImageInterpolation interpolation);
        ILayer Skretch(ICanvasResourceCreator resourceCreator, int width, int height, CanvasImageInterpolation interpolation);
        ILayer FlipHorizontal(ICanvasResourceCreator resourceCreator);
        ILayer FlipVertical(ICanvasResourceCreator resourceCreator);
        ILayer LeftTurn(ICanvasResourceCreator resourceCreator);
        ILayer RightTurn(ICanvasResourceCreator resourceCreator);
        ILayer OverTurn(ICanvasResourceCreator resourceCreator);

    }
}