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

        private void ConstructOptions()
        {
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
                if (layer.Type == LayerType.Bitmap)
                {
                    if (layer is BitmapLayer bitmapLayer)
                    {
                        SelectionType state = bitmapLayer.GetSelection(this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);

                        switch (state)
                        {
                            case SelectionType.None:
                                this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                                return false;
                            default:
                                switch (type)
                                {
                                    case OptionType.Gray:
                                        this.Primary(bitmapLayer, new GrayscaleEffect
                                        {
                                            Source = bitmapLayer.Origin
                                        });
                                        return true;
                                    case OptionType.Invert:
                                        this.Primary(bitmapLayer, new InvertEffect
                                        {
                                            Source = bitmapLayer.Origin
                                        });
                                        return true;

                                    case OptionType.Fog:
                                        return true;
                                    case OptionType.Sepia:
                                        this.Primary(bitmapLayer, new SepiaEffect
                                        {
                                            Source = bitmapLayer.Origin
                                        });
                                        return true;
                                    case OptionType.Posterize:
                                        this.Primary(bitmapLayer, new PosterizeEffect
                                        {
                                            Source = bitmapLayer.Origin
                                        });
                                        return true;

                                    default:
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
                                        this.FootType = this.SetFootType(this.EditType, type, this.ToolType);
                                        this.OptionType = type;
                                        this.SetCanvasState(true);
                                        return true;
                                }
                        }
                    }
                }

                this.Tip("Not Bitmap Layer", "Can only operate on Bitmap Layer.");
                return false;
            }
            else
            {
                this.Tip("No Layer", "Create a new Layer?");
                return false;
            }
        }

    }
}