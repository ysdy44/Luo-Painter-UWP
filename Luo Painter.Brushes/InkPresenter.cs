using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace Luo_Painter.Brushes
{
    public sealed class InkPresenter
    {
        public float Size { get; set; } = 22f;
        public float Opacity { get; set; } = 1f;
        public float Spacing { get; set; } = 0.25f;
        public BrushEdgeHardness Hardness { get; set; }
        public bool Rotate { get; set; }
        public int Step { get; set; } = 1024;

        public bool IsNone { get; private set; } = true;
        public BlendEffectMode BlendMode { get; private set; }

        public bool AllowMask { get; private set; }
        public string MaskTexture { get; private set; }
        public CanvasBitmap Mask { get; private set; }

        public bool AllowPattern { get; private set; }
        public string PatternTexture { get; private set; }
        public CanvasBitmap Pattern { get; private set; }


        public void SetBrush(PaintBrush brush)
        {
            this.Size = (float)brush.Size;
            this.Opacity = (float)brush.Opacity;
            this.Spacing = (float)brush.Spacing;
            this.Hardness = brush.Hardness;
        }

        public void SetMask(bool allowMask, CanvasBitmap mask = null)
        {
            this.AllowMask = allowMask;

            if (mask is null) return;

            if (this.Mask is null)
                this.Mask = mask;
            else
            {
                this.Mask.Dispose();
                this.Mask = mask;
            }
        }

        public void SetBlendMode(bool isNone, BlendEffectMode blendMode)
        {
            if (isNone)
                this.IsNone = true;
            else
            {
                this.BlendMode = blendMode;
                this.IsNone = false;
            }
        }

        public void SetPattern(bool allowPattern, CanvasBitmap pattern = null)
        {
            this.AllowPattern = allowPattern;

            if (pattern is null) return;

            if (this.Pattern is null)
                this.Pattern = pattern;
            else
            {
                this.Pattern.Dispose();
                this.Pattern = pattern;
            }
        }


        public InkType GetType(InkType toolType)
        {
            switch (toolType)
            {
                case InkType.None: return InkType.None;
                case InkType.Liquefy: return InkType.Liquefy;
                case InkType.EraseDry:
                    if (this.Opacity == 0f) return InkType.None;
                    else if (this.Opacity != 1f) toolType |= InkType.Opacity;
                    if (this.AllowPattern) toolType |= InkType.Pattern;
                    return toolType;
                default:
                    if (this.Opacity == 0f) return InkType.None;
                    else if (this.Opacity != 1f) toolType |= InkType.Opacity;
                    if (this.AllowPattern) toolType |= InkType.Pattern;
                    if (this.IsNone is false) toolType |= InkType.BlendMode;
                    return toolType;
            }
        }

        public ICanvasImage GetWetPreview(InkType type, ICanvasImage image, ICanvasImage background) => new BlendEffect
        {
            Mode = this.BlendMode,
            Background = background,
            Foreground = this.GetWetPreview(type, image)
        };
        public ICanvasImage GetWetPreview(InkType type, ICanvasImage image)
        {
            if (type.HasFlag(InkType.Pattern)) image = new AlphaMaskEffect
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

            if (type.HasFlag(InkType.Opacity)) image = new OpacityEffect
            {
                Source = image,
                Opacity = this.Opacity
            };

            return image;
        }

    }
}