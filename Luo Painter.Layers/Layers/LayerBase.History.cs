using Luo_Painter.Models;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    partial class LayerBase
    {

        public virtual bool History(HistoryPropertyMode type, object parameter)
        {
            switch (type)
            {
                case HistoryPropertyMode.Opacity:
                    if (parameter is float opacity)
                    {
                        this.Opacity = opacity;
                        return true;
                    }
                    else return false;
                case HistoryPropertyMode.BlendMode:
                    if (parameter is BlendEffectMode blendMode)
                    {
                        this.BlendMode = blendMode;
                        return true;
                    }
                    else
                    {
                        this.BlendMode = BlendExtensions.None;
                        return true;
                    }
                case HistoryPropertyMode.Visibility:
                    if (parameter is Visibility visibility)
                    {
                        this.Visibility = visibility;
                        return true;
                    }
                    else return false;
                default:
                    return false;
            }
        }

    }
}