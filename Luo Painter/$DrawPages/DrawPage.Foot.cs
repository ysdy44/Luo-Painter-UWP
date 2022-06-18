using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        double StartingFootX;
        double StartingFootY;


        private void SetFootType(OptionType optionType)
        {
            if (optionType.AllowDrag())
            {
                this.FootThumb.Visibility = Visibility.Visible;
            }
            else
            {
                this.FootThumb.Visibility = Visibility.Collapsed;
            }

            OptionType type = this.GetFootType(optionType);
            this.FootSwitchPresenter.Value = type;
            this.FootSwitchPresenter2.Value = type;
            this.FootSwitchPresenter3.Value = type;

            if (optionType.HasPreview())
            {
                this.FootHead.Visibility = Visibility.Visible;
                this.HeadLeftScrollViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.FootHead.Visibility = Visibility.Collapsed;
                this.HeadLeftScrollViewer.Visibility = Visibility.Visible;
            }

            this.FootTransform2.X = this.FootTransform.X = 0;
            this.FootTransform2.Y = this.FootTransform.Y = 0;
        }
        private OptionType GetFootType(OptionType type)
        {
            if (type == OptionType.MarqueeTransform) return OptionType.Transform;
            else if (type.HasFlag(OptionType.Marquee)) return OptionType.Marquee;
            else if (type.HasFlag(OptionType.Selection)) return OptionType.Selection;
            else return type;
        }


        private void ConstructFoots()
        {
            this.FootPanel.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.FootBorder.Width = e.NewSize.Width;
                this.FootBorder.Height = e.NewSize.Height;

                this.FootTransform2.X = this.FootTransform.X = 0;
                this.FootTransform2.Y = this.FootTransform.Y = 0;
            };

            this.FootThumb.DragStarted += (s, e) =>
            {
                this.StartingFootX = this.FootTransform.X;
                this.StartingFootY = this.FootTransform.Y;
            };
            this.FootThumb.DragDelta += (s, e) =>
            {
                if (this.FootPanel.Margin.Bottom == 0)
                {
                    this.StartingFootY += e.VerticalChange;

                    this.FootTransform2.Y = this.FootTransform.Y = System.Math.Clamp(this.StartingFootY, 0, this.FootBorder.Height - 50);
                }
                else
                {
                    this.StartingFootX += e.HorizontalChange;
                    this.StartingFootY += e.VerticalChange;

                    this.FootTransform2.X = this.FootTransform.X = this.StartingFootX;
                    this.FootTransform2.Y = this.FootTransform.Y = this.StartingFootY;
                }
            };
            this.FootThumb.DragCompleted += (s, e) =>
            {
            };
        }


        private void ConstructFoot()
        {
            this.FeatherSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.GrowSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.ShrinkSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate

            this.ExposureSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.BrightnessSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.SaturationSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.HueRotationSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.ContrastSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.TemperatureSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.TintSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.ShadowsSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.HighlightsSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.ClaritySlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.BlurSlider.ValueChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.LuminanceToAlphaComboBox.SelectionChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.TransformComboBox.SelectionChanged += (s, e) =>
            {
                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };


            this.FootSecondaryButton.Click += (s, e) =>
            {
                this.BitmapLayer = null;
                this.OptionType = this.ToolListView.SelectedItem;
                this.SetFootType(this.ToolListView.SelectedItem);
                this.SetCanvasState(false);
            };

            this.FootPrimaryButton.Click += (s, e) =>
            {
                if (this.OptionType.HasPreview())
                {
                    if (this.OptionType.IsEdit())
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
                this.SetFootType(this.ToolListView.SelectedItem);
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


                case OptionType.Feather:
                    return new GaussianBlurEffect
                    {
                        BlurAmount = (float)this.FeatherSlider.Value,
                        Source = image
                    };
                case OptionType.MarqueeTransform:
                    return this.GetTransformPreview(image);
                case OptionType.Grow:
                    int grow = (int)this.GrowSlider.Value;
                    return new MorphologyEffect
                    {
                        Mode = MorphologyEffectMode.Dilate,
                        Height = grow,
                        Width = grow,
                        Source = image
                    };
                case OptionType.Shrink:
                    int shrink = (int)this.ShrinkSlider.Value;
                    return new MorphologyEffect
                    {
                        Mode = MorphologyEffectMode.Erode,
                        Height = shrink,
                        Width = shrink,
                        Source = image
                    };


                case OptionType.Gray:
                    return new GrayscaleEffect
                    {
                        Source = image
                    };
                case OptionType.Invert:
                    return new InvertEffect
                    {
                        Source = image
                    };
                case OptionType.Exposure:
                    return new ExposureEffect
                    {
                        Exposure = (float)this.ExposureSlider.Value / 100,
                        Source = image
                    };
                case OptionType.Brightness:
                    float brightness = (float)this.BrightnessSlider.Value / 100;
                    return new BrightnessEffect
                    {
                        WhitePoint = new Vector2(System.Math.Clamp(2 - brightness, 0, 1), 1),
                        BlackPoint = new Vector2(System.Math.Clamp(1 - brightness, 0, 1), 0),
                        Source = image
                    };
                case OptionType.Saturation:
                    return new SaturationEffect
                    {
                        Saturation = (float)this.SaturationSlider.Value / 100,
                        Source = image
                    };
                case OptionType.HueRotation:
                    return new HueRotationEffect
                    {
                        Angle = (float)this.HueRotationSlider.Value / 180 * FanKit.Math.Pi,
                        Source = image
                    };
                case OptionType.Contrast:
                    return new ContrastEffect
                    {
                        Contrast = (float)this.ContrastSlider.Value / 100,
                        Source = image
                    };
                case OptionType.Temperature:
                    return new TemperatureAndTintEffect
                    {
                        Temperature = (float)this.TemperatureSlider.Value / 100,
                        Tint = (float)this.TintSlider.Value / 100,
                        Source = image
                    };
                case OptionType.HighlightsAndShadows:
                    return new HighlightsAndShadowsEffect
                    {
                        Shadows = (float)this.ShadowsSlider.Value / 100,
                        Highlights = (float)this.HighlightsSlider.Value / 100,
                        Clarity = (float)this.ClaritySlider.Value / 100,
                        MaskBlurAmount = (float)this.BlurSlider.Value / 100,
                        Source = image
                    };

                case OptionType.LuminanceToAlpha:
                    switch (this.LuminanceToAlphaComboBox.SelectedIndex)
                    {
                        case 0:
                            return new LuminanceToAlphaEffect
                            {
                                Source = image
                            };
                        case 1:
                            return new LuminanceToAlphaEffect
                            {
                                Source = new InvertEffect
                                {
                                    Source = image
                                }
                            };
                        case 2:
                            return new InvertEffect
                            {
                                Source = new LuminanceToAlphaEffect
                                {
                                    Source = new InvertEffect
                                    {
                                        Source = image
                                    }
                                }
                            };
                        default: return image;
                    }
                //case OptionType.Fog:
                case OptionType.Sepia:
                    return new SepiaEffect
                    {
                        Source = image
                    };
                case OptionType.Posterize:
                    return new PosterizeEffect
                    {
                        Source = image
                    };

                default: return image;
            }
        }

    }
}