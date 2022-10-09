using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Graphics.Effects;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public void Preview(CanvasDrawingSession ds, InkType type, ICanvasImage image, ICanvasImage wet)
        {
            switch (type)
            {
                case InkType.Brush_Dry:
                case InkType.Brush_Dry_Mix:
                case InkType.MaskBrush_Dry:
                case InkType.MaskBrush_Dry_Mix:
                case InkType.Circle_Dry:
                case InkType.Circle_Dry_Mix:
                case InkType.Line_Dry:
                case InkType.Line_Dry_Mix:
                    ds.DrawImage(image);
                    ds.DrawImage(wet);
                    break;

                case InkType.Brush_Wet_Pattern:
                case InkType.Brush_Wet_Pattern_Mix:
                case InkType.MaskBrush_Wet_Pattern:
                case InkType.MaskBrush_Wet_Pattern_Mix:
                case InkType.Circle_Wet_Pattern:
                case InkType.Circle_Wet_Pattern_Mix:
                case InkType.Line_Wet_Pattern:
                case InkType.Line_Wet_Pattern_Mix:
                    using (AlphaMaskEffect pattern = this.GetPattern(wet))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(pattern);
                    }
                    break;

                case InkType.Brush_Wet_Opacity:
                case InkType.MaskBrush_Wet_Opacity:
                case InkType.Circle_Wet_Opacity:
                case InkType.Line_Wet_Opacity:
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(opacity);
                    }
                    break;

                case InkType.Brush_Wet_Pattern_Opacity:
                case InkType.MaskBrush_Wet_Pattern_Opacity:
                case InkType.Circle_Wet_Pattern_Opacity:
                case InkType.Line_Wet_Pattern_Opacity:
                case InkType.Brush_WetComposite_Pattern_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend:
                case InkType.Circle_WetComposite_Pattern_Opacity_Blend:
                case InkType.Line_WetComposite_Pattern_Opacity_Blend:
                    using (AlphaMaskEffect pattern = this.GetPattern(wet))
                    using (OpacityEffect opacity = this.GetOpacity(pattern))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Brush_WetComposite_Blend:
                case InkType.MaskBrush_WetComposite_Blend:
                case InkType.Circle_WetComposite_Blend:
                case InkType.Line_WetComposite_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Brush_WetComposite_Pattern_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Blend:
                case InkType.Circle_WetComposite_Pattern_Blend:
                case InkType.Line_WetComposite_Pattern_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Brush_WetComposite_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Opacity_Blend:
                case InkType.Circle_WetComposite_Opacity_Blend:
                case InkType.Line_WetComposite_Opacity_Blend:
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Brush_WetBlur_Blur:
                case InkType.MaskBrush_WetBlur_Blur:
                case InkType.Circle_WetBlur_Blur:
                case InkType.Line_WetBlur_Blur:
                    using (AlphaMaskEffect blur = this.GetBlur(image, wet))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(blur);
                    }
                    break;

                case InkType.Brush_WetBlur_Pattern_Blur:
                case InkType.MaskBrush_WetBlur_Pattern_Blur:
                case InkType.Circle_WetBlur_Pattern_Blur:
                case InkType.Line_WetBlur_Pattern_Blur:
                    using (AlphaMaskEffect pattern = this.GetPattern(wet))
                    using (AlphaMaskEffect blur = this.GetBlur(image, pattern))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(blur);
                    }
                    break;

                case InkType.Brush_WetMosaic_Mosaic:
                case InkType.MaskBrush_WetMosaic_Mosaic:
                case InkType.Circle_WetMosaic_Mosaic:
                case InkType.Line_WetMosaic_Mosaic:
                    using (AlphaMaskEffect mosaic = this.GetMosaic(image, wet))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(mosaic);
                    }
                    break;

                case InkType.Brush_WetMosaic_Pattern_Mosaic:
                case InkType.MaskBrush_WetMosaic_Pattern_Mosaic:
                case InkType.Circle_WetMosaic_Pattern_Mosaic:
                case InkType.Line_WetMosaic_Pattern_Mosaic:
                    using (AlphaMaskEffect pattern = this.GetPattern(wet))
                    using (AlphaMaskEffect mosaic = this.GetMosaic(image, pattern))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(mosaic);
                    }
                    break;

                case InkType.Erase_Dry:
                case InkType.Erase_WetComposite_Opacity:
                    using (ArithmeticCompositeEffect erase = this.GetErase(image, wet))
                    {
                        ds.DrawImage(erase);
                    }
                    break;

                case InkType.Liquefy:
                    ds.DrawImage(image);
                    break;

                default:
                    break;
            }
        }

        public ICanvasImage GetPreview(InkType type, ICanvasImage image, ICanvasImage wet)
        {
            switch (type)
            {
                case InkType.Brush_Dry:
                case InkType.Brush_Dry_Mix:
                case InkType.MaskBrush_Dry:
                case InkType.MaskBrush_Dry_Mix:
                case InkType.Circle_Dry:
                case InkType.Circle_Dry_Mix:
                case InkType.Line_Dry:
                case InkType.Line_Dry_Mix:
                    return InkPresenter.GetComposite(image, wet);

                case InkType.Brush_Wet_Pattern:
                case InkType.Brush_Wet_Pattern_Mix:
                case InkType.MaskBrush_Wet_Pattern:
                case InkType.MaskBrush_Wet_Pattern_Mix:
                case InkType.Circle_Wet_Pattern:
                case InkType.Circle_Wet_Pattern_Mix:
                case InkType.Line_Wet_Pattern:
                case InkType.Line_Wet_Pattern_Mix:
                    {
                        AlphaMaskEffect pattern = this.GetPattern(wet);
                        return InkPresenter.GetComposite(image, pattern);
                    }

                case InkType.Brush_Wet_Opacity:
                case InkType.MaskBrush_Wet_Opacity:
                case InkType.Circle_Wet_Opacity:
                case InkType.Line_Wet_Opacity:
                    {
                        OpacityEffect opacity = this.GetOpacity(wet);
                        return InkPresenter.GetComposite(image, opacity);
                    }

                case InkType.Brush_Wet_Pattern_Opacity:
                case InkType.MaskBrush_Wet_Pattern_Opacity:
                case InkType.Circle_Wet_Pattern_Opacity:
                case InkType.Line_Wet_Pattern_Opacity:
                case InkType.Brush_WetComposite_Pattern_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend:
                case InkType.Circle_WetComposite_Pattern_Opacity_Blend:
                case InkType.Line_WetComposite_Pattern_Opacity_Blend:
                    {
                        AlphaMaskEffect pattern = this.GetPattern(wet);
                        OpacityEffect opacity = this.GetOpacity(pattern);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.Brush_WetComposite_Blend:
                case InkType.MaskBrush_WetComposite_Blend:
                case InkType.Circle_WetComposite_Blend:
                case InkType.Line_WetComposite_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.Brush_WetComposite_Pattern_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Blend:
                case InkType.Circle_WetComposite_Pattern_Blend:
                case InkType.Line_WetComposite_Pattern_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.Brush_WetComposite_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Opacity_Blend:
                case InkType.Circle_WetComposite_Opacity_Blend:
                case InkType.Line_WetComposite_Opacity_Blend:
                    {
                        OpacityEffect opacity = this.GetOpacity(wet);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.Brush_WetBlur_Blur:
                case InkType.MaskBrush_WetBlur_Blur:
                case InkType.Circle_WetBlur_Blur:
                case InkType.Line_WetBlur_Blur:
                    {
                        AlphaMaskEffect blur = this.GetBlur(image, wet);
                        return InkPresenter.GetComposite(image, blur);
                    }

                case InkType.Brush_WetBlur_Pattern_Blur:
                case InkType.MaskBrush_WetBlur_Pattern_Blur:
                case InkType.Circle_WetBlur_Pattern_Blur:
                case InkType.Line_WetBlur_Pattern_Blur:
                    {
                        AlphaMaskEffect pattern = this.GetPattern(wet);
                        AlphaMaskEffect blur = this.GetBlur(image, pattern);
                        return InkPresenter.GetComposite(image, blur);
                    }

                case InkType.Brush_WetMosaic_Mosaic:
                case InkType.MaskBrush_WetMosaic_Mosaic:
                case InkType.Circle_WetMosaic_Mosaic:
                case InkType.Line_WetMosaic_Mosaic:
                    {
                        AlphaMaskEffect mosaic = this.GetMosaic(image, wet);
                        return InkPresenter.GetComposite(image, mosaic);
                    }

                case InkType.Brush_WetMosaic_Pattern_Mosaic:
                case InkType.MaskBrush_WetMosaic_Pattern_Mosaic:
                case InkType.Circle_WetMosaic_Pattern_Mosaic:
                case InkType.Line_WetMosaic_Pattern_Mosaic:
                    {
                        AlphaMaskEffect pattern = this.GetPattern(wet);
                        AlphaMaskEffect mosaic = this.GetMosaic(image, pattern);
                        return InkPresenter.GetComposite(image, mosaic);
                    }

                case InkType.Erase:
                case InkType.Erase_WetComposite_Opacity:
                    {
                        ArithmeticCompositeEffect erase = this.GetErase(image, wet);
                        return erase;
                    }

                case InkType.Liquefy:
                    return image;

                default:
                    return image;
            }
        }


        public OpacityEffect GetOpacity(IGraphicsEffectSource image) => new OpacityEffect
        {
            Opacity = this.Opacity,
            Source = image
        };
        public BlendEffect GetBlend(IGraphicsEffectSource image, IGraphicsEffectSource wet) => new BlendEffect
        {
            Mode = this.BlendMode,
            Background = image,
            Foreground = wet
        };


        public ArithmeticCompositeEffect GetErase(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetErase(image, alphaMask, this.Opacity);
        public AlphaMaskEffect GetBlur(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetBlur(image, alphaMask, this.Size * this.Opacity);
        public AlphaMaskEffect GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetMosaic(image, alphaMask, this.Size);
        public AlphaMaskEffect GetPattern(IGraphicsEffectSource image) => InkPresenter.GetPattern(image, this.Pattern, new Vector2
        {
            X = this.Step / (float)this.Pattern.SizeInPixels.Width,
            Y = this.Step / (float)this.Pattern.SizeInPixels.Height
        });


        //@Static  
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
        public static CompositeEffect GetComposite(IGraphicsEffectSource image1, IGraphicsEffectSource image2) => new CompositeEffect
        {
            Sources =
            {
                image1,
                image2
            }
        };
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

        public static ArithmeticCompositeEffect GetErase(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask, float opacity) => new ArithmeticCompositeEffect
        {
            MultiplyAmount = 0,
            Source1Amount = 1,
            Source2Amount = -opacity,
            Offset = 0,
            Source1 = image,
            Source2 = alphaMask,
        };
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
        public static AlphaMaskEffect GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask, float size) => new AlphaMaskEffect
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
        public static AlphaMaskEffect GetPattern(IGraphicsEffectSource image, IGraphicsEffectSource pattern, Vector2 scale) => new AlphaMaskEffect
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