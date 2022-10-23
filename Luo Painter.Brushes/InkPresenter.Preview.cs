using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Devices.Power;
using Windows.Graphics.Effects;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter : InkAttributes
    {

        public void Preview(CanvasDrawingSession ds, InkType type, ICanvasImage image, ICanvasImage wet)
        {
            ds.Blend = CanvasBlend.Copy;
            switch (type)
            {
                case InkType.General:
                case InkType.General_Mix:
                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Mix:
                case InkType.Tip:
                case InkType.Tip_Mix:
                case InkType.Line:
                case InkType.Line_Mix:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    ds.DrawImage(wet);
                    break;

                case InkType.General_Grain:
                case InkType.General_Grain_Mix:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.Tip_Grain:
                case InkType.Tip_Grain_Mix:
                case InkType.Line_Grain:
                case InkType.Line_Grain_Mix:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (AlphaMaskEffect grain = this.GetGrain(wet))
                    {
                        ds.DrawImage(grain);
                    }
                    break;

                case InkType.General_Opacity:
                case InkType.ShapeGeneral_Opacity:
                case InkType.Tip_Opacity:
                case InkType.Line_Opacity:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    {
                        ds.DrawImage(opacity);
                    }
                    break;

                case InkType.General_Grain_Opacity:
                case InkType.ShapeGeneral_Grain_Opacity:
                case InkType.Tip_Grain_Opacity:
                case InkType.Line_Grain_Opacity:
                case InkType.General_Grain_Opacity_Blend:
                case InkType.ShapeGeneral_Grain_Opacity_Blend:
                case InkType.Tip_Grain_Opacity_Blend:
                case InkType.Line_Grain_Opacity_Blend:
                    using (AlphaMaskEffect grain = this.GetGrain(wet))
                    using (OpacityEffect opacity = this.GetOpacity(grain))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Blend:
                case InkType.ShapeGeneral_Blend:
                case InkType.Tip_Blend:
                case InkType.Line_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Grain_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Line_Grain_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Line_Opacity_Blend:
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.Blur:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (AlphaMaskEffect blur = this.GetBlur(image, wet))
                    {
                        ds.DrawImage(blur);
                    }
                    break;

                case InkType.Mosaic:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (ScaleEffect mosaic = this.GetMosaic(image, wet))
                    {
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
                case InkType.General:
                case InkType.General_Mix:
                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Mix:
                case InkType.Tip:
                case InkType.Tip_Mix:
                case InkType.Line:
                case InkType.Line_Mix:
                    return InkPresenter.GetComposite(image, wet);

                case InkType.General_Grain:
                case InkType.General_Grain_Mix:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.Tip_Grain:
                case InkType.Tip_Grain_Mix:
                case InkType.Line_Grain:
                case InkType.Line_Grain_Mix:
                    {
                        AlphaMaskEffect grain = this.GetGrain(wet);
                        return InkPresenter.GetComposite(image, grain);
                    }

                case InkType.General_Opacity:
                case InkType.ShapeGeneral_Opacity:
                case InkType.Tip_Opacity:
                case InkType.Line_Opacity:
                    {
                        OpacityEffect opacity = this.GetOpacity(wet);
                        return InkPresenter.GetComposite(image, opacity);
                    }

                case InkType.General_Grain_Opacity:
                case InkType.ShapeGeneral_Grain_Opacity:
                case InkType.Tip_Grain_Opacity:
                case InkType.Line_Grain_Opacity:
                case InkType.General_Grain_Opacity_Blend:
                case InkType.ShapeGeneral_Grain_Opacity_Blend:
                case InkType.Tip_Grain_Opacity_Blend:
                case InkType.Line_Grain_Opacity_Blend:
                    {
                        AlphaMaskEffect grain = this.GetGrain(wet);
                        OpacityEffect opacity = this.GetOpacity(grain);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.General_Blend:
                case InkType.ShapeGeneral_Blend:
                case InkType.Tip_Blend:
                case InkType.Line_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.General_Grain_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Line_Grain_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.General_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Line_Opacity_Blend:
                    {
                        OpacityEffect opacity = this.GetOpacity(wet);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.Blur:
                    {
                        AlphaMaskEffect blur = this.GetBlur(image, wet);
                        return InkPresenter.GetComposite(image, blur);
                    }

                case InkType.Mosaic:
                    {
                        ScaleEffect mosaic = this.GetMosaic(image, wet);
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
        public AlphaMaskEffect GetBlur(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetBlur(image, alphaMask, 30 * this.Flow);
        public ScaleEffect GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => InkPresenter.GetMosaic(image, alphaMask, this.Size);
        public AlphaMaskEffect GetGrain(IGraphicsEffectSource image) => InkPresenter.GetGrain(image, this.GrainSource, new Vector2
        {
            X = this.Step / (float)this.GrainSource.SizeInPixels.Width,
            Y = this.Step / (float)this.GrainSource.SizeInPixels.Height
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
        public static GaussianBlurEffect GetBlur(IGraphicsEffectSource image, float blurAmount) => new GaussianBlurEffect
        {
            BorderMode = EffectBorderMode.Hard,
            BlurAmount = blurAmount,
            Source = image
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
        public static AlphaMaskEffect GetGrain(IGraphicsEffectSource image, IGraphicsEffectSource Grain, Vector2 scale) => new AlphaMaskEffect
        {
            AlphaMask = new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = new ScaleEffect
                {
                    BorderMode = EffectBorderMode.Hard,
                    Source = Grain,
                    Scale = scale
                }
            },
            Source = image
        };

    }
}