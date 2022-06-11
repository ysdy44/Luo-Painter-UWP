using Luo_Painter.Blends;
using Luo_Painter.Brushes;
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
    public sealed partial class DrawPage : Page
    {

        private void SetInkToolType(InkType type)
        {
            if (this.InkToolType == type) return;
            this.InkToolType = type;

            if (this.InkRender is null) return;
            this.Ink();
            this.InkCanvasControl.Invalidate();
        }
        private InkType GetInkToolType(OptionType type, bool isMix, bool allowMask)
        {
            switch (type)
            {
                case OptionType.PaintBrush:
                    return isMix ?
                        (allowMask ? InkType.MaskMix : InkType.Mix) :
                        (allowMask ? InkType.MaskBrushDry : InkType.BrushDry);
                case OptionType.PaintWatercolorPen: return InkType.CircleDry;
                case OptionType.PaintPencil: return InkType.LineDry;
                case OptionType.PaintEraseBrush: return InkType.EraseDry;
                case OptionType.PaintLiquefaction: return InkType.Liquefy;
                default: return InkType.None;
            }
        }


        private void ConstructBrush()
        {
            this.InkCanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(this.InkRender.Source);
            };


            this.PaintBrushTool.ItemClick += async (s, brush) =>
            {
                if (brush.Mask is PaintTexture mask) this.InkPresenter.SetMask(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, mask.Source));
                else this.InkPresenter.SetMask(false);

                if (brush.Pattern is PaintTexture pattern) this.InkPresenter.SetPattern(true, await CanvasBitmap.LoadAsync(this.CanvasDevice, pattern.Source));
                else this.InkPresenter.SetPattern(false);

                this.InkPresenter.Construct(brush);
                this.PaintMenu.Construct(brush);

                this.InkToolType = this.GetInkToolType(this.OptionType, this.InkPresenter.IsMix, this.InkPresenter.AllowMask);
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


            this.PaintMenu.InkMixChanged += (s, isMix) =>
            {
                this.InkPresenter.IsMix = isMix;

                this.InkToolType = this.GetInkToolType(this.OptionType, isMix, this.InkPresenter.AllowMask);
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };


            this.PaintMenu.InkBlendModeChanged += (s, e) =>
            {
                this.InkPresenter.SetBlendMode(e.IsDefined(), e);
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };


            this.PaintMenu.SelectMask += async (s, e) =>
            {
                bool result = await this.SelectMask();
                if (result)
                {
                    this.Ink();
                    this.InkCanvasControl.Invalidate();
                }
            };
            this.PaintMenu.SelectPattern += async (s, e) => await this.SelectPattern();

            this.PaintMenu.MaskClosed += (s, e) =>
            {
                this.InkPresenter.SetMask(false);

                this.InkToolType = this.GetInkToolType(this.OptionType, this.InkPresenter.IsMix, false);
                this.Ink();
                this.InkCanvasControl.Invalidate();
            };
            this.PaintMenu.PatternClosed += (s, e) => this.InkPresenter.SetPattern(false);

            this.PaintMenu.MaskOpened += async (s, e) =>
            {
                if (this.InkPresenter.Mask is null)
                {
                    bool result = await this.SelectMask();
                    if (result)
                    {
                        this.InkToolType = this.GetInkToolType(this.OptionType, this.InkPresenter.IsMix, true);
                        this.Ink();
                        this.InkCanvasControl.Invalidate();
                    }
                    else
                    {
                        this.PaintMenu.CloseMask();
                    }
                }
                else
                {
                    this.InkPresenter.SetMask(true);

                    this.InkToolType = this.GetInkToolType(this.OptionType, this.InkPresenter.IsMix, true);
                    this.Ink();
                    this.InkCanvasControl.Invalidate();
                }
            };
            this.PaintMenu.PatternOpened += async (s, e) =>
            {
                if (this.InkPresenter.Pattern is null)
                {
                    bool result = await this.SelectPattern();
                    if (result is false) this.PaintMenu.ClosePattern();
                }
                else this.InkPresenter.SetPattern(true);
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
            double size = this.InkPresenter.Size / 24 + 1;
            switch (this.InkToolType)
            {
                case InkType.BrushDry:
                    this.InkRender.IsometricDrawShaderBrushEdgeHardness(
                        this.BrushEdgeHardnessShaderCodeBytes,
                        base.ActualTheme is ElementTheme.Light ? Vector4.Zero : Vector4.One,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness);
                    break;
                case InkType.MaskBrushDry:
                    this.InkRender.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        base.ActualTheme is ElementTheme.Light ? Vector4.Zero : Vector4.One,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness);
                    break;
                case InkType.LineDry:
                    this.InkRender.DrawLine((float)size, base.ActualTheme is ElementTheme.Light ? Colors.Black : Colors.White);
                    break;
                case InkType.Mix:
                    this.InkRender.IsometricDrawShaderBrushEdgeHardness(
                        this.BrushEdgeHardnessShaderCodeBytes,
                        base.ActualTheme is ElementTheme.Light ? Vector4.Zero : Vector4.One,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness);
                    break;
                case InkType.MaskMix:
                    this.InkRender.IsometricDrawShaderBrushEdgeHardnessWithTexture(
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes,
                        base.ActualTheme is ElementTheme.Light ? Vector4.Zero : Vector4.One,
                        this.InkPresenter.Mask,
                        this.InkPresenter.Rotate,
                        this.InkPresenter.Size,
                        this.InkPresenter.Spacing,
                        (int)this.InkPresenter.Hardness);
                    break;
                default:
                    this.InkRender.IsometricFillCircle(base.ActualTheme is ElementTheme.Light ? Colors.Black : Colors.White, (float)size, this.InkPresenter.Spacing);
                    break;
            }
        }

    }
}