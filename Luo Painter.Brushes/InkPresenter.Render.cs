using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Graphics.Effects;

namespace Luo_Painter.Brushes
{
    partial class InkPresenter
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OpacityEffect GetOpacity(IGraphicsEffectSource image) => new OpacityEffect
        {
            Opacity = base.Opacity,
            Source = image
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlendEffect GetBlend(IGraphicsEffectSource image, IGraphicsEffectSource wet) => new BlendEffect
        {
            Mode = base.BlendMode,
            Background = image,
            Foreground = wet
        };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArithmeticCompositeEffect GetErase(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetErase(image, alphaMask, base.Opacity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AlphaMaskEffect GetBlur(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetBlur(image, alphaMask, 30 * base.Flow);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScaleEffect GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetMosaic(image, alphaMask, base.Size);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AlphaMaskEffect GetGrain(IGraphicsEffectSource image) => base.RecolorGrain ? new AlphaMaskEffect
        {
            Source = image,
            AlphaMask = new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = new ScaleEffect
                {
                    BorderMode = EffectBorderMode.Hard,
                    Source = this.GrainSource,
                    Scale = new Vector2(this.GrainScale)
                }
            }
        } : new AlphaMaskEffect
        {
            AlphaMask = image,
            Source = new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = new ScaleEffect
                {
                    BorderMode = EffectBorderMode.Hard,
                    Source = this.GrainSource,
                    Scale = new Vector2(this.GrainScale)
                }
            }
        };


        //@Static  
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CompositeEffect GetDraw(IGraphicsEffectSource image1, IGraphicsEffectSource image2) => new CompositeEffect
        {
            Sources =
            {
                image1,
                new AlphaMaskEffect
                {
                    AlphaMask = image1,
                    Source = image2
                }
            }
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CompositeEffect GetComposite(IGraphicsEffectSource image1, IGraphicsEffectSource image2) => new CompositeEffect
        {
            Sources =
            {
                image1,
                image2
            }
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CompositeEffect GetDrawComposite(IGraphicsEffectSource image1, IGraphicsEffectSource image2, IGraphicsEffectSource alphaMask) => new CompositeEffect
        {
            Sources =
            {
                image1,
                new AlphaMaskEffect
                {
                    AlphaMask = alphaMask,
                    Source = image2
                }
            }
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArithmeticCompositeEffect GetErase(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask, float opacity) => new ArithmeticCompositeEffect
        {
            MultiplyAmount = 0,
            Source1Amount = 1,
            Source2Amount = -opacity,
            Offset = 0,
            Source1 = image,
            Source2 = alphaMask,
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GaussianBlurEffect GetBlur(IGraphicsEffectSource image, float blurAmount) => new GaussianBlurEffect
        {
            BorderMode = EffectBorderMode.Hard,
            BlurAmount = blurAmount,
            Source = image
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AlphaMaskEffect GetBlur(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask, float blurAmount) => new AlphaMaskEffect
        {
            AlphaMask = alphaMask,
            Source = new GaussianBlurEffect
            {
                BorderMode = EffectBorderMode.Hard,
                BlurAmount = blurAmount,
                Source = image
            }
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScaleEffect GetMosaic(IGraphicsEffectSource image, float size) => new ScaleEffect
        {
            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
            Scale = new Vector2(size / 4),
            Source = new ScaleEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                Scale = new Vector2(4 / size),
                Source = image
            }
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScaleEffect GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask, float size) => new ScaleEffect
        {
            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
            Scale = new Vector2(size / 4),
            Source = new ScaleEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                Scale = new Vector2(4 / size),
                Source = new AlphaMaskEffect
                {
                    AlphaMask = alphaMask,
                    Source = image
                }
            }
        };

    }
}