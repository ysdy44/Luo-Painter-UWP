﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Devices.Power;
using Windows.Graphics.Effects;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public void Preview(CanvasDrawingSession ds, InkType type, ICanvasImage image, ICanvasImage wet)
        {
            switch (type)
            {
                case InkType.Brush:
                case InkType.Brush_Mix:
                case InkType.MaskBrush:
                case InkType.MaskBrush_Mix:
                case InkType.Circle:
                case InkType.Circle_Mix:
                case InkType.Line:
                case InkType.Line_Mix:
                    ds.DrawImage(image);
                    ds.DrawImage(wet);
                    break;

                case InkType.Brush_Pattern:
                case InkType.Brush_Pattern_Mix:
                case InkType.MaskBrush_Pattern:
                case InkType.MaskBrush_Pattern_Mix:
                case InkType.Circle_Pattern:
                case InkType.Circle_Pattern_Mix:
                case InkType.Line_Pattern:
                case InkType.Line_Pattern_Mix:
                    using (AlphaMaskEffect pattern = this.GetPattern(wet))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(pattern);
                    }
                    break;

                case InkType.Brush_Opacity:
                case InkType.MaskBrush_Opacity:
                case InkType.Circle_Opacity:
                case InkType.Line_Opacity:
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(opacity);
                    }
                    break;

                case InkType.Brush_Pattern_Opacity:
                case InkType.MaskBrush_Pattern_Opacity:
                case InkType.Circle_Pattern_Opacity:
                case InkType.Line_Pattern_Opacity:
                case InkType.Brush_Pattern_Opacity_Blend:
                case InkType.MaskBrush_Pattern_Opacity_Blend:
                case InkType.Circle_Pattern_Opacity_Blend:
                case InkType.Line_Pattern_Opacity_Blend:
                    using (AlphaMaskEffect pattern = this.GetPattern(wet))
                    using (OpacityEffect opacity = this.GetOpacity(pattern))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Brush_Blend:
                case InkType.MaskBrush_Blend:
                case InkType.Circle_Blend:
                case InkType.Line_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Brush_Pattern_Blend:
                case InkType.MaskBrush_Pattern_Blend:
                case InkType.Circle_Pattern_Blend:
                case InkType.Line_Pattern_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Brush_Opacity_Blend:
                case InkType.MaskBrush_Opacity_Blend:
                case InkType.Circle_Opacity_Blend:
                case InkType.Line_Opacity_Blend:
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Brush_Blur:
                case InkType.MaskBrush_Blur:
                case InkType.Circle_Blur:
                case InkType.Line_Blur:
                    using (AlphaMaskEffect blur = this.GetBlur(image, wet))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(blur);
                    }
                    break;

                case InkType.Brush_Pattern_Blur:
                case InkType.MaskBrush_Pattern_Blur:
                case InkType.Circle_Pattern_Blur:
                case InkType.Line_Pattern_Blur:
                    using (AlphaMaskEffect pattern = this.GetPattern(wet))
                    using (AlphaMaskEffect blur = this.GetBlur(image, pattern))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(blur);
                    }
                    break;

                case InkType.Brush_Mosaic:
                case InkType.MaskBrush_Mosaic:
                case InkType.Circle_Mosaic:
                case InkType.Line_Mosaic:
                    using (ScaleEffect mosaic = this.GetMosaic(image, wet))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(mosaic);
                    }
                    break;

                case InkType.Brush_Pattern_Mosaic:
                case InkType.MaskBrush_Pattern_Mosaic:
                case InkType.Circle_Pattern_Mosaic:
                case InkType.Line_Pattern_Mosaic:
                    using (AlphaMaskEffect pattern = this.GetPattern(wet))
                    using (ScaleEffect mosaic = this.GetMosaic(image, pattern))
                    {
                        ds.DrawImage(image);
                        ds.DrawImage(mosaic);
                    }
                    break;

                case InkType.Erase:
                case InkType.Erase_Opacity:
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
                case InkType.Brush:
                case InkType.Brush_Mix:
                case InkType.MaskBrush:
                case InkType.MaskBrush_Mix:
                case InkType.Circle:
                case InkType.Circle_Mix:
                case InkType.Line:
                case InkType.Line_Mix:
                    return InkPresenter.GetComposite(image, wet);

                case InkType.Brush_Pattern:
                case InkType.Brush_Pattern_Mix:
                case InkType.MaskBrush_Pattern:
                case InkType.MaskBrush_Pattern_Mix:
                case InkType.Circle_Pattern:
                case InkType.Circle_Pattern_Mix:
                case InkType.Line_Pattern:
                case InkType.Line_Pattern_Mix:
                    {
                        AlphaMaskEffect pattern = this.GetPattern(wet);
                        return InkPresenter.GetComposite(image, pattern);
                    }

                case InkType.Brush_Opacity:
                case InkType.MaskBrush_Opacity:
                case InkType.Circle_Opacity:
                case InkType.Line_Opacity:
                    {
                        OpacityEffect opacity = this.GetOpacity(wet);
                        return InkPresenter.GetComposite(image, opacity);
                    }

                case InkType.Brush_Pattern_Opacity:
                case InkType.MaskBrush_Pattern_Opacity:
                case InkType.Circle_Pattern_Opacity:
                case InkType.Line_Pattern_Opacity:
                case InkType.Brush_Pattern_Opacity_Blend:
                case InkType.MaskBrush_Pattern_Opacity_Blend:
                case InkType.Circle_Pattern_Opacity_Blend:
                case InkType.Line_Pattern_Opacity_Blend:
                    {
                        AlphaMaskEffect pattern = this.GetPattern(wet);
                        OpacityEffect opacity = this.GetOpacity(pattern);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.Brush_Blend:
                case InkType.MaskBrush_Blend:
                case InkType.Circle_Blend:
                case InkType.Line_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.Brush_Pattern_Blend:
                case InkType.MaskBrush_Pattern_Blend:
                case InkType.Circle_Pattern_Blend:
                case InkType.Line_Pattern_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.Brush_Opacity_Blend:
                case InkType.MaskBrush_Opacity_Blend:
                case InkType.Circle_Opacity_Blend:
                case InkType.Line_Opacity_Blend:
                    {
                        OpacityEffect opacity = this.GetOpacity(wet);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.Brush_Blur:
                case InkType.MaskBrush_Blur:
                case InkType.Circle_Blur:
                case InkType.Line_Blur:
                    {
                        AlphaMaskEffect blur = this.GetBlur(image, wet);
                        return InkPresenter.GetComposite(image, blur);
                    }

                case InkType.Brush_Pattern_Blur:
                case InkType.MaskBrush_Pattern_Blur:
                case InkType.Circle_Pattern_Blur:
                case InkType.Line_Pattern_Blur:
                    {
                        AlphaMaskEffect pattern = this.GetPattern(wet);
                        AlphaMaskEffect blur = this.GetBlur(image, pattern);
                        return InkPresenter.GetComposite(image, blur);
                    }

                case InkType.Brush_Mosaic:
                case InkType.MaskBrush_Mosaic:
                case InkType.Circle_Mosaic:
                case InkType.Line_Mosaic:
                    {
                        ScaleEffect mosaic = this.GetMosaic(image, wet);
                        return InkPresenter.GetComposite(image, mosaic);
                    }

                case InkType.Brush_Pattern_Mosaic:
                case InkType.MaskBrush_Pattern_Mosaic:
                case InkType.Circle_Pattern_Mosaic:
                case InkType.Line_Pattern_Mosaic:
                    {
                        AlphaMaskEffect pattern = this.GetPattern(wet);
                        ScaleEffect mosaic = this.GetMosaic(image, pattern);
                        return InkPresenter.GetComposite(image, mosaic);
                    }

                case InkType.Erase:
                case InkType.Erase_Opacity:
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
        public ScaleEffect GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetMosaic(image, alphaMask, this.Size);
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