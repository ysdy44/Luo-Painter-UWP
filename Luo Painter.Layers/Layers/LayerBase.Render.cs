using Luo_Painter.Blends;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
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

    public abstract partial class LayerBase : IDisposable
    {

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


        public BlendEffectMode BlendMode
        {
            get => this.blendMode;
            set
            {
                this.blendMode = value;
                this.RenderMode = this.GetRenderMode();
                this.OnPropertyChanged(nameof(BlendMode)); // Notify 
            }
        }
        private BlendEffectMode blendMode = BlendExtensions.None;
        public BlendEffectMode StartingBlendMode { get; private set; } = BlendExtensions.None;
        public void CacheBlendMode() => this.StartingBlendMode = this.BlendMode;


        public Visibility Visibility
        {
            get => this.visibility;
            set
            {
                this.visibility = value;
                this.RenderMode = this.GetRenderMode();
                this.OnPropertyChanged(nameof(Visibility)); // Notify 
                this.OnPropertyChanged(nameof(UIVisibility)); // Notify 
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
                return this.BlendMode.IsDefined() ? RenderMode.NoneWithBlendMode : RenderMode.None;
            else
                return this.BlendMode.IsDefined() ? RenderMode.NoneWithOpacityAndBlendMode : RenderMode.NoneWithOpacity;
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
                        Mode = this.BlendMode
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
                        Mode = this.BlendMode
                    };
                default:
                    return previousImage;
            }
        }

        public void CopyWith(LayerBase layerBase)
        {
            this.name = layerBase.Name;
            this.opacity = layerBase.Opacity;
            this.blendMode = layerBase.BlendMode;
            this.visibility = layerBase.Visibility;
            this.RenderMode = this.GetRenderMode();
        }

    }
}