using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace Luo_Painter.Layers
{
    public enum RenderMode
    {
        None,
        NoneWithOpacity,
        NoneWithBlendMode,
        NoneWithOpacityAndBlendMode,

        Collapsed,
        Transparent,
    }


    public abstract class LayerBase
    {

        public string Id { get; } = Guid.NewGuid().ToString();


        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.OnPropertyChanged(nameof(Name)); // Notify 
            }
        }
        private string name;
        public string StartingName { get; private set; }
        public void CacheName() => this.StartingName = this.Name;


        public float Opacity
        {
            get => this.opacity;
            set
            {
                this.opacity = value;
                this.RenderMode = this.GetRenderMode();
                this.OnPropertyChanged(nameof(Opacity)); // Notify 
            }
        }
        private float opacity = 1;
        public float StartingOpacity { get; private set; }
        public void CacheOpacity() => this.StartingOpacity = this.Opacity;


        public BlendEffectMode? BlendMode
        {
            get => this.blendMode;
            set
            {
                this.blendMode = value;
                this.RenderMode = this.GetRenderMode();
                this.OnPropertyChanged(nameof(BlendMode)); // Notify 
            }
        }
        private BlendEffectMode? blendMode;
        public BlendEffectMode? StartingBlendMode { get; private set; }
        public void CacheBlendMode() => this.StartingBlendMode = this.BlendMode;


        public Visibility Visibility
        {
            get => this.visibility;
            set
            {
                this.visibility = value;
                this.RenderMode = this.GetRenderMode();
                this.OnPropertyChanged(nameof(Visibility)); // Notify 
            }
        }
        private Visibility visibility;


        public RenderMode RenderMode { get; private set; }
        private RenderMode GetRenderMode()
        {
            if (this.Visibility == Visibility.Collapsed)
                return RenderMode.Collapsed;
            else if (this.Opacity == 0)
                return RenderMode.Transparent;
            else if (this.Opacity == 1)
                return this.BlendMode.HasValue ? RenderMode.NoneWithBlendMode : RenderMode.None;
            else
                return this.BlendMode.HasValue ? RenderMode.NoneWithOpacityAndBlendMode : RenderMode.NoneWithOpacity;
        }
        public ICanvasImage Render(ICanvasImage previousImage, ICanvasImage currentImage)
        {
            switch (this.RenderMode)
            {
                case RenderMode.None:
                    return new CompositeEffect
                    {
                        Sources =
                        {
                            previousImage,
                            currentImage
                        }
                    };
                case RenderMode.NoneWithOpacity:
                    return new CompositeEffect
                    {
                        Sources =
                        {
                            previousImage,
                            new OpacityEffect
                            {
                                Opacity = this.Opacity,
                                Source = currentImage
                            }
                        }
                    };
                case RenderMode.NoneWithBlendMode:
                    return new BlendEffect
                    {
                        Background = currentImage,
                        Foreground = previousImage,
                        Mode = this.BlendMode.Value
                    };
                case RenderMode.NoneWithOpacityAndBlendMode:
                    return new BlendEffect
                    {
                        Background = new OpacityEffect
                        {
                            Opacity = this.Opacity,
                            Source = currentImage
                        },
                        Foreground = previousImage,
                        Mode = this.BlendMode.Value
                    };
                default:
                    return previousImage;
            }
        }


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
                        this.BlendMode = null;
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


        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}