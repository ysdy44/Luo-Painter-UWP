using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        private void SetInkToolType(OptionType type)
        {
            InkType toolType;
            switch (type)
            {
                case OptionType.PaintBrush: toolType = InkType.Brush; break;
                case OptionType.PaintWatercolorPen: toolType = InkType.Circle; break;
                case OptionType.PaintPencil: toolType = InkType.Line; break;
                case OptionType.PaintEraseBrush: toolType = InkType.Erase; break;
                case OptionType.PaintLiquefaction: toolType = InkType.Liquefy; break;
                default: toolType = default; break;
            }

            if (this.InkPresenter.ToolType == toolType) return;
            this.InkPresenter.ToolType = toolType;
            this.PaintMenu.Type = toolType;

            this.InkType = this.InkPresenter.GetType();

            if (this.InkRender is null) return;
            this.Ink();
            this.InkCanvasControl.Invalidate();
        }


        private void ConstructBrush()
        {
            this.BrushMenu.ItemClick += async (s, brush) =>
            {
                if (brush.Mask is PaintTexture mask) this.InkPresenter.SetMask(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, mask.Source));
                else this.InkPresenter.SetMask(false);

                if (brush.Pattern is PaintTexture pattern) this.InkPresenter.SetPattern(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, pattern.Source));
                else this.InkPresenter.SetPattern(false);

                this.InkPresenter.Construct(brush);
                this.PaintMenu.Construct(brush);

                this.InkType = this.InkPresenter.GetType();
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };
        }

        private void ConstructInk()
        {
            this.PaintMenu.InkSizeChanged += (s, e) =>
            {
                this.InkPresenter.Size = e;
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };
            this.PaintMenu.InkOpacityChanged += (s, e) =>
            {
                this.InkPresenter.Opacity = e;
                this.InkCanvasControl.Opacity = e;
                this.InkType = this.InkPresenter.GetType();
            };
            this.PaintMenu.InkSpacingChanged += (s, e) =>
            {
                this.InkPresenter.Spacing = e;
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };
            this.PaintMenu.InkHardnessChanged += (s, e) =>
            {
                this.InkPresenter.Hardness = e;
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };


            this.PaintMenu.InkModeChanged += (s, mode) =>
            {
                this.InkPresenter.Mode = mode;
                this.InkType = this.InkPresenter.GetType();
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };


            this.PaintMenu.InkBlendModeChanged += (s, blendMode) =>
            {
                if (blendMode.IsDefined())
                {
                    this.InkPresenter.Mode = InkType.Blend;
                    this.InkPresenter.BlendMode = blendMode;
                }
                else
                {
                    this.InkPresenter.Mode = InkType.None;
                }

                this.InkType = this.InkPresenter.GetType();
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };


            this.PaintMenu.SelectMask += async (s, e) =>
            {
                bool result = await this.SelectMask();
                if (result)
                {
                    this.InkType = this.InkPresenter.GetType();
                    this.Ink();
                    this.InkCanvasControl.Invalidate();
                }
            };
            this.PaintMenu.SelectPattern += async (s, e) => await this.SelectPattern();

            this.PaintMenu.MaskClosed += (s, e) =>
            {
                this.InkPresenter.SetMask(false);

                this.InkType = this.InkPresenter.GetType();
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };
            this.PaintMenu.PatternClosed += (s, e) =>
            {
                this.InkPresenter.SetPattern(false);
                this.InkType = this.InkPresenter.GetType();
            };

            this.PaintMenu.MaskOpened += async (s, e) =>
            {
                if (this.InkPresenter.Mask is null)
                {
                    bool result = await this.SelectMask();
                    if (result is false)
                    {
                        this.PaintMenu.CloseMask();
                        return;
                    }
                }
                else this.InkPresenter.SetMask(true);

                this.InkType = this.InkPresenter.GetType();
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };
            this.PaintMenu.PatternOpened += async (s, e) =>
            {
                if (this.InkPresenter.Pattern is null)
                {
                    bool result = await this.SelectPattern();
                    if (result is false)
                    {
                        this.PaintMenu.ClosePattern();
                        return;
                    }
                }
                else this.InkPresenter.SetPattern(true);

                this.InkType = this.InkPresenter.GetType();
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };


            this.PaintMenu.InkRotateChanged += (s, e) =>
            {
                this.InkPresenter.Rotate = e;
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };
            this.PaintMenu.InkStepChanged += (s, e) =>
            {
                this.InkPresenter.Step = e;
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };
        }


        private async Task<bool> SelectMask()
        {
            this.TextureDialog.Construct(this.PaintMenu.MaskTexture);
            ContentDialogResult result = await this.TextureDialog.ShowAsync(ContentDialogPlacement.Popup);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    if (this.TextureDialog.SelectedItem is PaintTexture item)
                    {
                        this.InkPresenter.SetMask(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
                        this.PaintMenu.SetMaskTexture(item.Texture);
                        return true;
                    }
                    else return false;
                default: return false;
            }
        }

        private async Task<bool> SelectPattern()
        {
            this.TextureDialog.Construct(this.PaintMenu.PatternTexture);
            ContentDialogResult result = await this.TextureDialog.ShowAsync(ContentDialogPlacement.Popup);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    if (this.TextureDialog.SelectedItem is PaintTexture item)
                    {
                        this.InkPresenter.SetPattern(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, item.Source));
                        this.PaintMenu.SetPatternTexture(item.Texture);
                        this.PaintMenu.SetStep(item.Step);
                        return true;
                    }
                    else return false;
                default: return false;
            }
        }


        private void Ink()
        {
            if (this.InkRender is null) return;
            double size = this.InkPresenter.Size / 24 + 1;
            switch (this.InkType)
            {
                case InkType.BrushDry:
                case InkType.BrushWetPattern:
                case InkType.BrushWetOpacity:
                case InkType.BrushWetPatternOpacity:
                case InkType.BrushWetBlend:
                case InkType.BrushWetPatternBlend:
                case InkType.BrushWetOpacityBlend:
                case InkType.BrushWetPatternOpacityBlend:
                case InkType.BrushDryMix:
                case InkType.BrushWetPatternMix:
                case InkType.BrushWetBlur:
                case InkType.BrushWetPatternBlur:
                    this.InkRender.IsometricDrawShaderBrushEdgeHardness(this.BrushEdgeHardnessShaderCodeBytes, base.ActualTheme is ElementTheme.Light ? Vector4.Zero : Vector4.One, this.InkPresenter.Size, this.InkPresenter.Spacing, (int)this.InkPresenter.Hardness);
                    break;
                case InkType.MaskBrushDry:
                case InkType.MaskBrushWetPattern:
                case InkType.MaskBrushWetOpacity:
                case InkType.MaskBrushWetPatternOpacity:
                case InkType.MaskBrushWetBlend:
                case InkType.MaskBrushWetPatternBlend:
                case InkType.MaskBrushWetOpacityBlend:
                case InkType.MaskBrushWetPatternOpacityBlend:
                case InkType.MaskBrushDryMix:
                case InkType.MaskBrushWetPatternMix:
                case InkType.MaskBrushWetBlur:
                case InkType.MaskBrushWetPatternBlur:
                    this.InkRender.IsometricDrawShaderBrushEdgeHardnessWithTexture(this.BrushEdgeHardnessWithTextureShaderCodeBytes, base.ActualTheme is ElementTheme.Light ? Vector4.Zero : Vector4.One, this.InkPresenter.Mask, this.InkPresenter.Rotate, this.InkPresenter.Size, this.InkPresenter.Spacing, (int)this.InkPresenter.Hardness);
                    break;
                case InkType.LineDry:
                case InkType.LineWetPattern:
                case InkType.LineWetOpacity:
                case InkType.LineWetPatternOpacity:
                case InkType.LineWetBlend:
                case InkType.LineWetPatternBlend:
                case InkType.LineWetOpacityBlend:
                case InkType.LineWetPatternOpacityBlend:
                case InkType.LineDryMix:
                case InkType.LineWetPatternMix:
                case InkType.LineWetBlur:
                case InkType.LineWetPatternBlur:
                    this.InkRender.DrawLine((float)size, base.ActualTheme is ElementTheme.Light ? Colors.Black : Colors.White);
                    break;
                default:
                    this.InkRender.IsometricFillCircle(base.ActualTheme is ElementTheme.Light ? Colors.Black : Colors.White, (float)size, this.InkPresenter.Spacing);
                    break;
            }
        }

    }
}