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
            this.OptionMenu.ItemClick += (s, type) =>
            {
                this.OptionClick(type);
            };

            this.MoreOptionMenu.ItemClick += (s, type) =>
            {
                this.OptionClick(type);
            };
        }

        private bool OptionClick(OptionType type)
        {
            this.ExpanderLightDismissOverlay.Hide();

            if (this.LayerListView.SelectedItem is ILayer layer)
            {
                if (layer.Type is LayerType.Bitmap)
                {
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
                            this.Primary(bitmapLayer, this.GetPreview(type, bitmapLayer.Origin));
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
                            case OptionType.DisplacementLiquefaction:
                                this.SetDisplacementLiquefaction();
                                break;
                            case OptionType.GradientMapping:
                                this.SetGradientMapping();
                                break;
                        }

                        this.BitmapLayer = bitmapLayer;
                        this.SelectionType = state;
                        this.OptionType = type;
                        this.SetFootType(type);
                        this.SetCanvasState(true);
                        return true;
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