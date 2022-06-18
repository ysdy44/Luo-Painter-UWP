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
                return new CompositeEffect
                {
                    Sources = { image, wet }
                };
            else if (type.HasFlag(InkType.WetBlur))
                return new CompositeEffect
                {
                    Sources = { image, this.GetBlur(image, wet) }
                };
            else if (type.HasFlag(InkType.WetMosaic))
                return new CompositeEffect
                {
                    Sources = { image, this.GetMosaic(image, wet) }
                };
            else if (type.HasFlag(InkType.WetComposite))
            {
                if (type.HasFlag(InkType.WetCompositeBlend))
                    return new BlendEffect
                    {
                        Mode = this.BlendMode,
                        Background = image,
                        Foreground = wet
                    };
                else if (type.HasFlag(InkType.WetCompositeEraseOpacity))
                    return this.GetErase(this.Opacity, image, wet);
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
                case InkType.BrushWetPattern:
                case InkType.BrushWetPatternMix:
                case InkType.MaskBrushWetPattern:
                case InkType.MaskBrushWetPatternMix:
                case InkType.CircleWetPattern:
                case InkType.CircleWetPatternMix:
                case InkType.LineWetPattern:
                case InkType.LineWetPatternMix:
                case InkType.BrushWetPatternBlend:
                case InkType.MaskBrushWetPatternBlend:
                case InkType.CircleWetPatternBlend:
                case InkType.LineWetPatternBlend:
                case InkType.BrushWetPatternBlur:
                case InkType.MaskBrushWetPatternBlur:
                case InkType.CircleWetPatternBlur:
                case InkType.LineWetPatternBlur:
                case InkType.EraseWetPatternOpacity:
                    return this.GetPattern(image);

                case InkType.BrushWetOpacity:
                case InkType.MaskBrushWetOpacity:
                case InkType.CircleWetOpacity:
                case InkType.LineWetOpacity:
                    return new OpacityEffect
                    {
                        Opacity = this.Opacity,
                        Source = image
                    };

                case InkType.BrushWetPatternOpacity:
                case InkType.MaskBrushWetPatternOpacity:
                case InkType.CircleWetPatternOpacity:
                case InkType.LineWetPatternOpacity:
                case InkType.BrushWetPatternOpacityBlend:
                case InkType.MaskBrushWetPatternOpacityBlend:
                case InkType.CircleWetPatternOpacityBlend:
                case InkType.LineWetPatternOpacityBlend:
                    return new OpacityEffect
                    {
                        Opacity = this.Opacity,
                        Source = this.GetPattern(image)
                    };

                case InkType.BrushWetBlend:
                case InkType.MaskBrushWetBlend:
                case InkType.CircleWetBlend:
                case InkType.LineWetBlend:
                case InkType.BrushWetBlur:
                case InkType.MaskBrushWetBlur:
                case InkType.CircleWetBlur:
                case InkType.LineWetBlur:
                case InkType.BrushWetMosaic:
                case InkType.MaskBrushWetMosaic:
                case InkType.CircleWetMosaic:
                case InkType.LineWetMosaic:
                case InkType.EraseWetOpacity:
                    return image;

                case InkType.BrushWetOpacityBlend:
                case InkType.MaskBrushWetOpacityBlend:
                case InkType.CircleWetOpacityBlend:
                case InkType.LineWetOpacityBlend:
                    return new OpacityEffect
                    {
                        Opacity = this.Opacity,
                        Source = image
                    };

                default:
                    return null;
            }
        }

        public ICanvasImage GetErase(float opacity, IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => new ArithmeticCompositeEffect
        {
            MultiplyAmount = 0,
            Source1Amount = 1,
            Source2Amount = -opacity,
            Offset = 0,
            Source1 = image,
            Source2 = alphaMask,
        };

        public ICanvasImage GetBlur(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => new AlphaMaskEffect
        {
            AlphaMask = alphaMask,
            Source = new GaussianBlurEffect
            {
                BorderMode = EffectBorderMode.Hard,
                BlurAmount = System.Math.Clamp(this.Size * this.Opacity, 1, 100),
                Source = image
            }
        };

        public ICanvasImage GetMosaic(IGraphicsEffectSource image, IGraphicsEffectSource alphaMask) => new AlphaMaskEffect
        {
            AlphaMask = alphaMask,
            Source = new ScaleEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                Scale = new Vector2(this.Size / 4),
                Source = new ScaleEffect
                {
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Scale = new Vector2(4 / this.Size),
                    Source = image
                }
            }
        };

        private ICanvasImage GetPattern(IGraphicsEffectSource image)
        {
            return new AlphaMaskEffect
            {
                AlphaMask = new BorderEffect
                {
                    ExtendX = CanvasEdgeBehavior.Wrap,
                    ExtendY = CanvasEdgeBehavior.Wrap,
                    Source = new ScaleEffect
                    {
                        BorderMode = EffectBorderMode.Hard,
                        Source = this.Pattern,
                        Scale = new Vector2
                        {
                            X = this.Step / (float)this.Pattern.SizeInPixels.Width,
                            Y = this.Step / (float)this.Pattern.SizeInPixels.Height
                        }
                    }
                },
                Source = image
            };
        }

    }
}