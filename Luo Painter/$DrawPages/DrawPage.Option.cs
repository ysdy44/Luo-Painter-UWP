using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal class OptionTypeCommand : RelayCommand<OptionType>
    {
    }

    public sealed partial class DrawPage : Page
    {

        private void ConstructOption()
        {
            this.OptionTypeCommand.Click += (s, type) =>
            {
                this.OptionFlyout.Hide();

                if (this.LayerListView.SelectedItem is ILayer layer)
                {
                    if (layer.Type != LayerType.Bitmap)
                    {
                        this.Tip("Not Bitmap Layer", "Can only operate on Bitmap Layer.");
                    }
                    else if (layer is BitmapLayer bitmapLayer)
                    {
                        Color[] InterpolationColors = bitmapLayer.GetInterpolationColors();
                        PixelBoundsMode mode = bitmapLayer.GetInterpolationBoundsMode(InterpolationColors);

                        switch (mode)
                        {
                            case PixelBoundsMode.Transarent:
                                this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                                break;
                            case PixelBoundsMode.Solid:
                            case PixelBoundsMode.None:
                                switch (type)
                                {
                                    case OptionType.None:
                                        return;
                                    case OptionType.Gray:
                                        bitmapLayer.DrawSource(new GrayscaleEffect
                                        {
                                            Source = bitmapLayer.Origin
                                        });
                                        break;
                                    case OptionType.Invert:
                                        bitmapLayer.DrawSource(new InvertEffect
                                        {
                                            Source = bitmapLayer.Origin
                                        });
                                        break;
                                    case OptionType.LuminanceToAlpha:
                                        bitmapLayer.DrawSource(new AlphaMaskEffect
                                        {
                                            Source = bitmapLayer.Origin,
                                            AlphaMask = new LuminanceToAlphaEffect
                                            {
                                                Source = new InvertEffect
                                                {
                                                    Source = bitmapLayer.Origin
                                                }
                                            }
                                        });
                                        break;
                                    default:
                                        return;
                                }

                                // History
                                switch (mode)
                                {
                                    case PixelBoundsMode.Solid:
                                        int removes2 = this.History.Push(bitmapLayer.GetBitmapResetHistory());
                                        break;
                                    case PixelBoundsMode.None:
                                        bitmapLayer.Hit(InterpolationColors);
                                        int removes3 = this.History.Push(bitmapLayer.GetBitmapHistory());
                                        break;
                                }
                                bitmapLayer.Flush();
                                bitmapLayer.RenderThumbnail();

                                this.CanvasControl.Invalidate(); // Invalidate

                                this.UndoButton.IsEnabled = this.History.CanUndo;
                                this.RedoButton.IsEnabled = this.History.CanRedo;
                                break;
                        }
                    }
                }
                else
                {
                    this.Tip("No Layer", "Create a new Layer?");
                }
            };
        }

    }
}