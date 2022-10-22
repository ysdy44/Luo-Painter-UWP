using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {


        private void ConstructFoots()
        {
        }


        private void ConstructFoot()
        {
            this.AppBar.GeometryInvalidate += this.GeometryInvalidate;

            this.AppBar.CanvasControlInvalidate += this.CanvasControl.Invalidate;
            this.AppBar.CanvasVirtualControlInvalidate += this.CanvasVirtualControl.Invalidate;

            this.AppBar.SecondaryButtonClick += (s, e) =>
            {
                if (this.OptionType is OptionType.CropCanvas)
                {
                    this.CancelCropCanvas();
                }
                if (this.OptionType.IsGeometry())
                {
                    this.CancelGeometryTransform();
                }

                this.BitmapLayer = null;
                this.OptionType = this.ToolListView.SelectedItem;
                this.AppBar.Construct(this.ToolListView.SelectedItem);
                this.SetCanvasState(false);
            };

            this.AppBar.PrimaryButtonClick += (s, e) =>
            {
                if (this.OptionType.HasPreview())
                {
                    if (this.OptionType is OptionType.CropCanvas)
                    {
                        this.PrimaryCropCanvas();
                    }
                    else if (this.OptionType.IsGeometry())
                    {
                        this.PrimaryGeometryTransform();
                    }
                    else if (this.OptionType.IsMarquees())
                    {
                        Color[] InterpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(InterpolationColors);
                        this.Primary(this.OptionType, mode, InterpolationColors, this.Marquee);
                    }
                    else
                    {
                        Color[] InterpolationColors = this.BitmapLayer.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.BitmapLayer.GetInterpolationBoundsMode(InterpolationColors);
                        this.Primary(this.OptionType, mode, InterpolationColors, this.BitmapLayer);
                    }
                }

                this.BitmapLayer = null;
                this.OptionType = this.ToolListView.SelectedItem;
                this.AppBar.Construct(this.ToolListView.SelectedItem);
                this.SetCanvasState(false);
            };
        }


        private void Primary(BitmapLayer bitmapLayer, ICanvasImage source)
        {
            Color[] InterpolationColors = this.Marquee.GetInterpolationColorsBySource();
            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(InterpolationColors);

            // History
            switch (mode)
            {
                case PixelBoundsMode.None:
                    bitmapLayer.Hit(InterpolationColors);

                    switch (this.SelectionType)
                    {
                        case SelectionType.MarqueePixelBounds:
                            bitmapLayer.DrawCopy(new PixelShaderEffect(this.LalphaMaskShaderCodeBytes)
                            {
                                Source1 = this.Marquee[BitmapType.Source],
                                Source2 = bitmapLayer[BitmapType.Origin],
                                Source3 = source
                            });
                            break;
                        default:
                            bitmapLayer.DrawCopy(source);
                            break;
                    }
                    int removes3 = this.History.Push(bitmapLayer.GetBitmapHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    break;
                default:
                    bitmapLayer.DrawCopy(source);
                    int removes2 = this.History.Push(bitmapLayer.GetBitmapResetHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    break;
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }


        private void Primary(OptionType type, PixelBoundsMode mode, Color[] InterpolationColors, BitmapLayer bitmapLayer)
        {
            if (type.HasDifference())
            {
                switch (this.SelectionType)
                {
                    case SelectionType.MarqueePixelBounds:
                        bitmapLayer.DrawCopy(new PixelShaderEffect(this.RalphaMaskShaderCodeBytes)
                        {
                            Source1 = this.Marquee[BitmapType.Source],
                            Source2 = bitmapLayer[BitmapType.Origin],
                        }, this.GetPreview(type, new AlphaMaskEffect
                        {
                            AlphaMask = this.Marquee[BitmapType.Source],
                            Source = bitmapLayer[BitmapType.Origin],
                        }));
                        break;
                    default:
                        bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer[BitmapType.Origin]));
                        break;
                }
                bitmapLayer.Hit(bitmapLayer.GetInterpolationColors(new PixelShaderEffect(this.DifferenceShaderCodeBytes)
                {
                    Source1 = bitmapLayer[BitmapType.Source],
                    Source2 = bitmapLayer[BitmapType.Origin]
                }));

                // History
                int removes = this.History.Push(bitmapLayer.GetBitmapHistory());
                bitmapLayer.Flush();
                bitmapLayer.RenderThumbnail();
            }
            else
            {
                // History
                switch (mode)
                {
                    case PixelBoundsMode.Solid:
                    case PixelBoundsMode.Transarent:
                        bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer[BitmapType.Origin]));
                        int removes2 = this.History.Push(bitmapLayer.GetBitmapResetHistory());
                        bitmapLayer.Flush();
                        bitmapLayer.RenderThumbnail();
                        break;
                    case PixelBoundsMode.None:
                        bitmapLayer.Hit(InterpolationColors);

                        switch (this.SelectionType)
                        {
                            case SelectionType.MarqueePixelBounds:
                                bitmapLayer.DrawCopy(new PixelShaderEffect(this.LalphaMaskShaderCodeBytes)
                                {
                                    Source1 = this.Marquee[BitmapType.Source],
                                    Source2 = bitmapLayer[BitmapType.Origin],
                                    Source3 = this.GetPreview(type, bitmapLayer[BitmapType.Origin])
                                });
                                break;
                            default:
                                bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer[BitmapType.Origin]));
                                break;
                        }
                        int removes3 = this.History.Push(bitmapLayer.GetBitmapHistory());
                        bitmapLayer.Flush();
                        bitmapLayer.RenderThumbnail();
                        break;
                }
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }


        private ICanvasImage GetPreview(OptionType type, ICanvasImage image)
        {
            switch (type)
            {
                case OptionType.None:
                    return image;

                case OptionType.Transform:
                    return this.GetTransformPreview(image);
                case OptionType.DisplacementLiquefaction:
                    return this.GetDisplacementLiquefactionPreview(image);
                case OptionType.GradientMapping:
                    return this.GetGradientMappingPreview(image);
                case OptionType.RippleEffect:
                    return this.GetRippleEffectPreview(image);

                case OptionType.MarqueeTransform:
                    return this.GetTransformPreview(image);

                default:
                    return this.AppBar.GetPreview(type, image);
            }
        }

    }
}