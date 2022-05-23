using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

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

    public enum ThumbnailType
    {
        None,
        Origin,
        Oversize,
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


        public readonly Vector2 Center; // (125, 125)
        public readonly int Width; // 250
        public readonly int Height; // 250

        public LayerBase(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            this.Center = new Vector2((float)width / 2, (float)height / 2);
            this.Width = width;
            this.Height = height;


            this.ThumbnailWriteableBitmap = new WriteableBitmap(50, 50);
            this.ThumbnailRenderTarget = new CanvasRenderTarget(resourceCreator, 50, 50, 96);

            int wh = System.Math.Max(width, height);
            if (wh <= 50) this.ThumbnailType = ThumbnailType.Origin;
            else if (wh >= 5000) this.ThumbnailType = ThumbnailType.Oversize;
            else
            {
                this.ThumbnailScale = new Vector2(50f / wh);
                this.ThumbnailType = ThumbnailType.None;
            }
        }


        public ImageSource Thumbnail => this.ThumbnailWriteableBitmap;

        readonly CanvasRenderTarget ThumbnailRenderTarget;
        readonly WriteableBitmap ThumbnailWriteableBitmap;
        readonly ThumbnailType ThumbnailType;
        readonly Vector2 ThumbnailScale;

        public void RenderThumbnail(ICanvasImage image)
        {
            using (CanvasDrawingSession ds = this.ThumbnailRenderTarget.CreateDrawingSession())
            {
                ds.Blend = CanvasBlend.Copy;
                switch (this.ThumbnailType)
                {
                    case ThumbnailType.Origin:
                        ds.DrawImage(image);
                        break;
                    case ThumbnailType.Oversize:
                        ds.DrawImage(new ScaleEffect
                        {
                            Scale = new Vector2(0.01f),
                            BorderMode = EffectBorderMode.Hard,
                            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                            Source = image,
                        });
                        break;
                    default:
                        ds.DrawImage(new ScaleEffect
                        {
                            Scale = this.ThumbnailScale,
                            BorderMode = EffectBorderMode.Hard,
                            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                            Source = image,
                        });
                        break;
                }
            }

            byte[] bytes = this.ThumbnailRenderTarget.GetPixelBytes();
            using (Stream stream = this.ThumbnailWriteableBitmap.PixelBuffer.AsStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            this.ThumbnailWriteableBitmap.Invalidate();
        }

        public void ClearThumbnail(Color color)
        {
            using (CanvasDrawingSession ds = this.ThumbnailRenderTarget.CreateDrawingSession())
            {
                ds.Clear(color);
            }

            byte[] bytes = this.ThumbnailRenderTarget.GetPixelBytes();
            using (Stream stream = this.ThumbnailWriteableBitmap.PixelBuffer.AsStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            this.ThumbnailWriteableBitmap.Invalidate();
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