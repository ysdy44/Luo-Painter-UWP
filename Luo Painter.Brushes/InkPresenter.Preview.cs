using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Graphics.Effects;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public ICanvasImage GetPreview(InkType type, ICanvasImage image, ICanvasImage wet)
        {
            if (type.HasFlag(InkType.Dry))
                return image;
            else if (type.HasFlag(InkType.Wet))
                return InkPresenter.GetComposite(image, wet);
            else if (type.HasFlag(InkType.WetBlur))
                return InkPresenter.GetComposite(image, this.GetBlur(image, wet));
            else if (type.HasFlag(InkType.WetMosaic))
                return InkPresenter.GetComposite(image, this.GetMosaic(image, wet));
            else if (type.HasFlag(InkType.WetComposite))
            {
                if (type.HasFlag(InkType.WetComposite_Blend))
                    return new BlendEffect
                    {
                        Mode = this.BlendMode,
                        Background = image,
                        Foreground = wet
                    };
                else if (type.HasFlag(InkType.WetComposite_Erase_Opacity))
                    return this.GetErase(image, wet);
                else
                    return null;
            }
            else
                return null;
        }

        public ICanvasImage GetWet(InkType type, ICanvasImage image)
        {
            switch (type)
            {
                case InkType.Brush_Wet_Pattern:
                case InkType.Brush_Wet_Pattern_Mix:
                case InkType.MaskBrush_Wet_Pattern:
                case InkType.MaskBrush_Wet_Pattern_Mix:
                case InkType.Circle_Wet_Pattern:
                case InkType.Circle_Wet_Pattern_Mix:
                case InkType.Line_Wet_Pattern:
                case InkType.Line_Wet_Pattern_Mix:
                case InkType.Brush_WetComposite_Pattern_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Blend:
                case InkType.Circle_WetComposite_Pattern_Blend:
                case InkType.Line_WetComposite_Pattern_Blend:
                case InkType.Brush_WetBlur_Pattern_Blur:
                case InkType.MaskBrush_WetBlur_Pattern_Blur:
                case InkType.Circle_WetBlur_Pattern_Blur:
                case InkType.Line_WetBlur_Pattern_Blur:
                case InkType.Erase_WetComposite_Pattern_Opacity:
                    return this.GetPattern(image);

                case InkType.Brush_Wet_Opacity:
                case InkType.MaskBrush_Wet_Opacity:
                case InkType.Circle_Wet_Opacity:
                case InkType.Line_Wet_Opacity:
                    return new OpacityEffect
                    {
                        Opacity = this.Opacity,
                        Source = image
                    };

                case InkType.Brush_Wet_Pattern_Opacity:
                case InkType.MaskBrush_Wet_Pattern_Opacity:
                case InkType.Circle_Wet_Pattern_Opacity:
                case InkType.Line_Wet_Pattern_Opacity:
                case InkType.Brush_WetComposite_Pattern_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend:
                case InkType.Circle_WetComposite_Pattern_Opacity_Blend:
                case InkType.Line_WetComposite_Pattern_Opacity_Blend:
                    return new OpacityEffect
                    {
                        Opacity = this.Opacity,
                        Source = this.GetPattern(image)
                    };

                case InkType.Brush_WetComposite_Blend:
                case InkType.MaskBrush_WetComposite_Blend:
                case InkType.Circle_WetComposite_Blend:
                case InkType.Line_WetComposite_Blend:
                case InkType.Brush_WetBlur_Blur:
                case InkType.MaskBrush_WetBlur_Blur:
                case InkType.Circle_WetBlur_Blur:
                case InkType.Line_WetBlur_Blur:
                case InkType.Brush_WetMosaic_Mosaic:
                case InkType.MaskBrush_WetMosaic_Mosaic:
                case InkType.Circle_WetMosaic_Mosaic:
                case InkType.Line_WetMosaic_Mosaic:
                case InkType.Erase_WetComposite_Opacity:
                    return image;

                case InkType.Brush_WetComposite_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Opacity_Blend:
                case InkType.Circle_WetComposite_Opacity_Blend:
                case InkType.Line_WetComposite_Opacity_Blend:
                    return new OpacityEffect
                    {
                        Opacity = this.Opacity,
                        Source = image
                    };

                default:
                    return null;
            }
        }


        public ICanvasImage GetErase(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetErase(image, alphaMask, this.Opacity);
        public ICanvasImage GetBlur(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetBlur(image, alphaMask, this.Size * this.Opacity);
        public ICanvasImage GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetMosaic(image, alphaMask, this.Size);
        public ICanvasImage GetPattern(IGraphicsEffectSource image) => InkPresenter.GetPattern(image, this.Pattern, new Vector2
        {
            X = this.Step / (float)this.Pattern.SizeInPixels.Width,
            Y = this.Step / (float)this.Pattern.SizeInPixels.Height
        });


        //@Static  
        public static ICanvasImage GetDraw(IGraphicsEffectSource image1, IGraphicsEffectSource image2) => new CompositeEffect
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
        public static ICanvasImage GetComposite(IGraphicsEffectSource image1, IGraphicsEffectSource image2) => new CompositeEffect
        {
            Sources =
            {
                image1,
                image2
            }
        };
        public static ICanvasImage GetDrawComposite(IGraphicsEffectSource image1, IGraphicsEffectSource image2, IGraphicsEffectSource alphaMask) => new CompositeEffect
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

        public static ICanvasImage GetErase(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask, float opacity) => new ArithmeticCompositeEffect
        {
            MultiplyAmount = 0,
            Source1Amount = 1,
            Source2Amount = -opacity,
            Offset = 0,
            Source1 = image,
            Source2 = alphaMask,
        };
        public static ICanvasImage GetBlur(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask, float blurAmount) => new AlphaMaskEffect
        {
            AlphaMask = alphaMask,
            Source = new GaussianBlurEffect
            {
                BorderMode = EffectBorderMode.Hard,
                BlurAmount = blurAmount,
                Source = image
            }
        };
        public static ICanvasImage GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask, float size) => new AlphaMaskEffect
        {
            AlphaMask = alphaMask,
            Source = new ScaleEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                Scale = new Vector2(size / 4),
                Source = new ScaleEffect
                {
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Scale = new Vector2(4 / size),
                    Source = image
                }
            }
        };
        public static ICanvasImage GetPattern(IGraphicsEffectSource image, IGraphicsEffectSource pattern, Vector2 scale) => new AlphaMaskEffect
        {
            AlphaMask = new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = new ScaleEffect
                {
                    BorderMode = EffectBorderMode.Hard,
                    Source = pattern,
                    Scale = scale
                }
            },
            Source = image
        };

    }
}