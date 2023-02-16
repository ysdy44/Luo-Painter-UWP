using Luo_Painter.Blends;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public abstract partial class LayerBase
    {

        public virtual bool History(HistoryType type, object parameter)
        {
            switch (type)
            {
                case HistoryType.Name:
                    if (parameter is string name)
                    {
                        this.Name = name;
                        return true;
                    }
                    else return false;
                case HistoryType.Opacity:
                    if (parameter is float opacity)
                    {
                        this.Opacity = opacity;
                        return true;
                    }
                    else return false;
                case HistoryType.BlendMode:
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
                case HistoryType.Visibility:
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