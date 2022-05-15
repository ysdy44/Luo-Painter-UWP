using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System.Numerics;
using Windows.Foundation;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        double StartingFootTitleX;
        double StartingFootTitleY;

        private void SetOptionType(OptionType type)
        {
            if (type.HasIcon())
            {
                this.FootTransform2.X = this.FootTransform.X = 0;
                this.FootTransform2.Y = this.FootTransform.Y = 0;

                this.OptionIcon.Type = type;
                this.OptionTextBlock.Text = type.ToString();
                this.FootTitle.Visibility = Visibility.Visible;
            }
            else
            {
                this.FootTitle.Visibility = Visibility.Collapsed;
            }
        }

        private void ConstructOptions()
        {
            this.FootPanel.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.FootBorder.Width = e.NewSize.Width;
                if (e.NewSize.Height == e.PreviousSize.Height) return;

                this.FootBorder.Height = e.NewSize.Height;
                this.FootTransform2.X = this.FootTransform.X = 0;
                this.FootTransform2.Y = this.FootTransform.Y = 0;
            };

            this.FootTitle.ManipulationStarted += (s, e) =>
            {
                this.StartingFootTitleX = this.FootTransform.X;
                this.StartingFootTitleY = this.FootTransform.Y;
            };
            this.FootTitle.ManipulationDelta += (s, e) =>
            {
                if (this.FootPanel.Margin.Bottom == 0)
                {
                    this.FootTransform2.Y = this.FootTransform.Y = System.Math.Clamp(this.StartingFootTitleY + e.Cumulative.Translation.Y, 0, this.FootBorder.Height - 50);
                }
                else
                {
                    this.FootTransform2.X = this.FootTransform.X = this.StartingFootTitleX + e.Cumulative.Translation.X;
                    this.FootTransform2.Y = this.FootTransform.Y = this.StartingFootTitleY + e.Cumulative.Translation.Y;
                }
            };
            this.FootTitle.ManipulationCompleted += (s, e) =>
            {
            };

            this.FootSecondaryButton.Click += (s, e) =>
            {
                this.BitmapLayer = null;
                this.EditType = default;
                this.OptionType = default;
                this.SetOptionType(default);
                this.SetCanvasState(false);
            };

            this.FootPrimaryButton.Click += (s, e) =>
            {
                if (this.EditType != default)
                {
                    Color[] InterpolationColors = this.Marquee.GetInterpolationColorsBySource();
                    PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(InterpolationColors);
                    this.Option(this.OptionType, mode, InterpolationColors, this.Marquee);
                }
                else if (this.OptionType != default)
                {
                    Color[] InterpolationColors = this.BitmapLayer.GetInterpolationColorsBySource();
                    PixelBoundsMode mode = this.BitmapLayer.GetInterpolationBoundsMode(InterpolationColors);
                    this.Option(this.OptionType, mode, InterpolationColors, this.BitmapLayer);
                }

                this.BitmapLayer = null;
                this.EditType = default;
                this.OptionType = default;
                this.SetOptionType(default);
                this.SetCanvasState(false);
            };

            this.OptionButton.ItemClick += (s, type) =>
            {
                this.OptionClick(type);
            };

            this.MoreOptionButton.ItemClick += (s, type) =>
            {
                this.OptionClick(type);
            };
        }

        private bool OptionClick(OptionType type)
        {
            this.ExpanderLightDismissOverlay.Hide();

            if (this.LayerListView.SelectedItem is ILayer layer)
            {
                if (layer.Type != LayerType.Bitmap)
                {
                    this.Tip("Not Bitmap Layer", "Can only operate on Bitmap Layer.");
                    return false;
                }

                if (layer is BitmapLayer bitmapLayer)
                {
                    SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);

                    if (state is SelectionType.None)
                    {
                        this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                        return false;
                    }

                    if (type.HasPreview() is false)
                    {
                        this.Option(type, mode, InterpolationColors, bitmapLayer);
                        return true;
                    }

                    switch (type)
                    {
                        case OptionType.Transform:
                            switch (state)
                            {
                                case SelectionType.PixelBounds:
                                    {
                                        PixelBounds interpolationBounds = bitmapLayer.CreateInterpolationBounds(InterpolationColors);
                                        PixelBounds bounds = bitmapLayer.CreatePixelBounds(interpolationBounds);
                                        this.SetTransform(bounds);
                                    }
                                    break;
                                case SelectionType.MarqueePixelBounds:
                                    {
                                        PixelBounds interpolationBounds = this.Marquee.CreateInterpolationBounds(InterpolationColors);
                                        PixelBounds bounds = this.Marquee.CreatePixelBounds(interpolationBounds);
                                        this.SetTransform(bounds);
                                    }
                                    break;
                                default:
                                    this.SetTransform(bitmapLayer.Bounds);
                                    break;
                            }
                            break;
                        case OptionType.GradientMapping:
                            this.SetGradientMapping();
                            break;
                        case OptionType.RippleEffect:
                            this.SetRippleEffect(bitmapLayer);
                            break;
                    }

                    this.BitmapLayer = bitmapLayer;
                    this.SelectionType = state;
                    this.OptionType = type;
                    this.SetOptionType(type);
                    this.SetCanvasState(true);
                    return true;
                }
            }

            this.Tip("No Layer", "Create a new Layer?");
            return false;
        }

        private void Option(OptionType type, PixelBoundsMode mode, Color[] InterpolationColors, BitmapLayer bitmapLayer)
        {
            if (type.HasDifference())
            {
                switch (this.SelectionType)
                {
                    case SelectionType.MarqueePixelBounds:
                        bitmapLayer.DrawCopy(new PixelShaderEffect(this.RalphaMaskShaderCodeBytes)
                        {
                            Source1 = this.Marquee.Source,
                            Source2 = bitmapLayer.Origin,
                        }, this.GetPreview(type, new AlphaMaskEffect
                        {
                            AlphaMask = this.Marquee.Source,
                            Source = bitmapLayer.Origin,
                        }));
                        break;
                    default:
                        bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer.Origin));
                        break;
                }
                bitmapLayer.Hit(bitmapLayer.GetInterpolationColors(new PixelShaderEffect(this.DifferenceShaderCodeBytes)
                {
                    Source1 = bitmapLayer.Source,
                    Source2 = bitmapLayer.Origin
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
                        bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer.Origin));
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
                                    Source1 = this.Marquee.Source,
                                    Source2 = bitmapLayer.Origin,
                                    Source3 = this.GetPreview(type, bitmapLayer.Origin)
                                });
                                break;
                            default:
                                bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer.Origin));
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

        private void ConstructOption()
        {
            this.LuminanceToAlphaComboBox.SelectionChanged += (s, e) => this.CanvasVirtualControl.Invalidate(); // Invalidate
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
        }

        private ICanvasImage GetPreview(OptionType type, ICanvasImage image)
        {
            switch (type)
            {
                case OptionType.None:
                    return image;
                case OptionType.Transform:
                    return this.GetTransformPreview(image);
                case OptionType.GradientMapping:
                    return this.GetGradientMappingPreview(image);
                case OptionType.RippleEffect:
                    return this.GetRippleEffectPreview(image);
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