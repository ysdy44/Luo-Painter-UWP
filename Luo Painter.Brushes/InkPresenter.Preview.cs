using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Graphics.Effects;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
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
                case InkType.Line:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    ds.DrawImage(wet);
                    break;

                case InkType.General_Grain:
                case InkType.General_Grain_Mix:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.Tip_Grain:
                case InkType.Line_Grain:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (AlphaMaskEffect grain = this.GetGrain(wet))
                    {
                        ds.DrawImage(grain);
                    }
                    break;

                case InkType.General_Opacity:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Opacity_Mix:
                case InkType.Tip_Opacity:
                case InkType.Line_Opacity:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (OpacityEffect opacity = this.GetOpacity(wet))
                    {
                        ds.DrawImage(opacity);
                    }
                    break;

                case InkType.General_Opacity_Grain:
                case InkType.General_Opacity_Grain_Mix:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Opacity_Grain_Mix:
                case InkType.Tip_Opacity_Grain:
                case InkType.Line_Opacity_Grain:
                    ds.DrawImage(image);
                    ds.Blend = CanvasBlend.SourceOver;
                    using (AlphaMaskEffect grain = this.GetGrain(wet))
                    using (OpacityEffect opacity = this.GetOpacity(grain))
                    {
                        ds.DrawImage(opacity);
                    }
                    break;

                case InkType.General_Opacity_Grain_Blend:
                case InkType.General_Opacity_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend_Mix:
                case InkType.Tip_Opacity_Grain_Blend:
                case InkType.Line_Opacity_Grain_Blend:
                    using (AlphaMaskEffect grain = this.GetGrain(wet))
                    using (OpacityEffect opacity = this.GetOpacity(grain))
                    using (BlendEffect blend = this.GetBlend(image, opacity))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Blend:
                case InkType.General_Blend_Mix:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Blend_Mix:
                case InkType.Tip_Blend:
                case InkType.Line_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Grain_Blend:
                case InkType.General_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Grain_Blend_Mix:
                case InkType.Tip_Grain_Blend:
                case InkType.Line_Grain_Blend:
                    using (BlendEffect blend = this.GetBlend(image, wet))
                    {
                        ds.DrawImage(blend);
                    }
                    break;

                case InkType.General_Opacity_Blend:
                case InkType.General_Opacity_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Blend_Mix:
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
                case InkType.Line:
                    return InkPresenter.GetComposite(image, wet);

                case InkType.General_Grain:
                case InkType.General_Grain_Mix:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.Tip_Grain:
                case InkType.Line_Grain:
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

                case InkType.General_Opacity_Grain:
                case InkType.General_Opacity_Grain_Mix:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Opacity_Grain_Mix:
                case InkType.Tip_Opacity_Grain:
                case InkType.Line_Opacity_Grain:
                    {
                        AlphaMaskEffect grain = this.GetGrain(wet);
                        OpacityEffect opacity = this.GetOpacity(grain);
                        return InkPresenter.GetComposite(image, opacity);
                    }

                case InkType.General_Opacity_Grain_Blend:
                case InkType.General_Opacity_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend_Mix:
                case InkType.Tip_Opacity_Grain_Blend:
                case InkType.Line_Opacity_Grain_Blend:
                    {
                        AlphaMaskEffect grain = this.GetGrain(wet);
                        OpacityEffect opacity = this.GetOpacity(grain);
                        BlendEffect blend = this.GetBlend(image, opacity);
                        return blend;
                    }

                case InkType.General_Blend:
                case InkType.General_Blend_Mix:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Blend_Mix:
                case InkType.Tip_Blend:
                case InkType.Line_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.General_Grain_Blend:
                case InkType.General_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Grain_Blend_Mix:
                case InkType.Tip_Grain_Blend:
                case InkType.Line_Grain_Blend:
                    {
                        BlendEffect blend = this.GetBlend(image, wet);
                        return blend;
                    }

                case InkType.General_Opacity_Blend:
                case InkType.General_Opacity_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Blend_Mix:
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
            AlphaMask = InkPresenter.GetGrain(this.GrainSource, new Vector2
            {
                X = base.Step / (float)this.GrainSource.SizeInPixels.Width,
                Y = base.Step / (float)this.GrainSource.SizeInPixels.Height
            })
        } : new AlphaMaskEffect
        {
            AlphaMask = image,
            Source = InkPresenter.GetGrain(this.GrainSource, new Vector2
            {
                X = base.Step / (float)this.GrainSource.SizeInPixels.Width,
                Y = base.Step / (float)this.GrainSource.SizeInPixels.Height
            })
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BorderEffect GetGrain(IGraphicsEffectSource grain, Vector2 scale) => new BorderEffect
        {
            ExtendX = CanvasEdgeBehavior.Wrap,
            ExtendY = CanvasEdgeBehavior.Wrap,
            Source = new ScaleEffect
            {
                BorderMode = EffectBorderMode.Hard,
                Source = grain,
                Scale = scale
            }
        };

    }
}