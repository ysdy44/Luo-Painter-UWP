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

namespace Luo_Painter
{
    internal class OptionTypeCommand : RelayCommand<OptionType>
    {
    }

    public sealed partial class DrawPage : Page
    {

        private void SetOptionType(OptionType type)
        {
            this.TransformComboBox.Visibility = type == OptionType.Transform ? Visibility.Visible : Visibility.Collapsed;
            this.GradientMappingTextBlock.Visibility = this.GradientMappingSelector.Visibility = type == OptionType.GradientMapping ? Visibility.Visible : Visibility.Collapsed;

            this.ExposureTextBlock.Visibility = this.ExposureSlider.Visibility = type == OptionType.Exposure ? Visibility.Visible : Visibility.Collapsed;
            this.BrightnessTextBlock.Visibility = this.BrightnessSlider.Visibility = type == OptionType.Brightness ? Visibility.Visible : Visibility.Collapsed;
            this.SaturationTextBlock.Visibility = this.SaturationSlider.Visibility = type == OptionType.Saturation ? Visibility.Visible : Visibility.Collapsed;
            this.HueRotationTextBlock.Visibility = this.HueRotationSlider.Visibility = type == OptionType.HueRotation ? Visibility.Visible : Visibility.Collapsed;
            this.ContrastTextBlock.Visibility = this.ContrastSlider.Visibility = type == OptionType.Contrast ? Visibility.Visible : Visibility.Collapsed;
            this.TemperatureTextBlock.Visibility = this.TemperatureSlider.Visibility = this.TintSlider.Visibility = type == OptionType.Temperature ? Visibility.Visible : Visibility.Collapsed;
            this.HighlightsAndShadowsTextBlock.Visibility = this.HighlightsAndShadowsPanel.Visibility = type == OptionType.HighlightsAndShadows ? Visibility.Visible : Visibility.Collapsed;

            this.LuminanceToAlphaComboBox.Visibility = type == OptionType.LuminanceToAlpha ? Visibility.Visible : Visibility.Collapsed;

            switch (type)
            {
                case OptionType.GradientMapping:
                    this.FootGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                    break;
                default:
                    this.FootGrid.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
            }
        }

        private void ConstructOptions()
        {

            this.OptionSecondaryButton.Click += (s, e) =>
            {
                this.OptionType = OptionType.None;
                this.SetOptionType(OptionType.None);
                this.SetFullScreenState(FullScreenState.UnFullScreen);

                this.BitmapLayer = null;
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.OptionPrimaryButton.Click += (s, e) =>
            {
                OptionType type = this.OptionType;
                BitmapLayer bitmapLayer = this.BitmapLayer;

                Color[] InterpolationColors = bitmapLayer.GetInterpolationColorsBySource();
                PixelBoundsMode mode = bitmapLayer.GetInterpolationBoundsMode(InterpolationColors);
                this.Option(type, mode, InterpolationColors, bitmapLayer);

                this.OptionType = OptionType.None;
                this.SetOptionType(OptionType.None);
                this.SetFullScreenState(FullScreenState.UnFullScreen);

                this.BitmapLayer = null;
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.OptionTypeCommand.Click += (s, type) =>
            {
                this.OptionFlyout.Hide();
                this.OptionClick(type);
            };

            this.MoreOptionTypeCommand.Click += (s, type) =>
            {
                this.MoreOptionFlyout.Hide();
                this.OptionClick(type);
            };
        }

        private void OptionClick(OptionType type)
        {
            if (this.LayerListView.SelectedItem is ILayer layer)
            {
                if (layer.Type != LayerType.Bitmap)
                {
                    this.Tip("Not Bitmap Layer", "Can only operate on Bitmap Layer.");
                }
                else if (layer is BitmapLayer bitmapLayer)
                {
                    Color[] InterpolationColors = bitmapLayer.GetInterpolationColorsBySource();
                    PixelBoundsMode mode = bitmapLayer.GetInterpolationBoundsMode(InterpolationColors);

                    switch (mode)
                    {
                        case PixelBoundsMode.Transarent:
                            this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                            break;
                        case PixelBoundsMode.Solid:
                        case PixelBoundsMode.None:
                            if (type.HasPreview())
                            {
                                switch (type)
                                {
                                    case OptionType.Transform:
                                        this.SetTransform(bitmapLayer, InterpolationColors);
                                        break;
                                    case OptionType.GradientMapping:
                                        this.SetGradientMapping();
                                        break;
                                }

                                this.OptionType = type;
                                this.SetOptionType(type);
                                this.SetFullScreenState(FullScreenState.Option);

                                this.BitmapLayer = bitmapLayer;
                                this.CanvasControl.Invalidate(); // Invalidate
                            }
                            else
                            {
                                this.Option(type, mode, InterpolationColors, bitmapLayer);
                            }
                            break;
                    }
                }
            }
            else
            {
                this.Tip("No Layer", "Create a new Layer?");
            }
        }

        private void Option(OptionType type, PixelBoundsMode mode, Color[] InterpolationColors, BitmapLayer bitmapLayer)
        {
            // History
            switch (mode)
            {
                case PixelBoundsMode.Solid:
                    bitmapLayer.DrawSource(this.GetPreview(type, bitmapLayer.Origin));
                    int removes2 = this.History.Push(bitmapLayer.GetBitmapResetHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    break;
                case PixelBoundsMode.None:
                    bitmapLayer.Hit(InterpolationColors);

                    bitmapLayer.DrawSource(this.GetPreview(type, bitmapLayer.Origin));
                    int removes3 = this.History.Push(bitmapLayer.GetBitmapHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    break;
            }

            this.CanvasControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

        private void ConstructOption()
        {
            this.LuminanceToAlphaComboBox.SelectionChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.ExposureSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.BrightnessSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.SaturationSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.HueRotationSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.ContrastSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.TemperatureSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.TintSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.ShadowsSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.HighlightsSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.ClaritySlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.BlurSlider.ValueChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
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
                    float brightness = (float)this.ExposureSlider.Value / 100;
                    return new BrightnessEffect
                    {
                        WhitePoint = new Vector2(System.Math.Min(2 - brightness, 1), 1f),
                        BlackPoint = new Vector2(System.Math.Max(1 - brightness, 0), 0f),
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