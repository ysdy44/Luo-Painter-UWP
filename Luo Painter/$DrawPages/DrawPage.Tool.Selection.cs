using FanKit.Transformers;
using Luo_Painter.Options;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Luo_Painter.Layers;
using Luo_Painter.Brushes;
using Luo_Painter.Blends;
using Luo_Painter.Elements;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        bool SelectionIsSubtract => this.SelectionComboBox.SelectedIndex is 0 is false;

        private void SelectionBrush_Start()
        {
        }
        private void SelectionBrush_Delta()
        {
            this.Marquee.Marquee(this.StartingPosition, this.Position, 32, this.SelectionIsSubtract);
            this.Marquee.Hit(RectExtensions.GetRect(this.StartingPosition, this.Position, 32));
            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void SelectionBrush_Complete()
        {
            // History
            int removes = this.History.Push(this.Marquee.GetBitmapHistory());
            this.Marquee.Flush();
            this.Marquee.RenderThumbnail();
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

        private void SelectionFlood_Complete()
        {
            if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
            {
                this.SelectionFlood(this.Position, bitmapLayer, this.SelectionIsSubtract);
            }
            else
            {
                this.Tip(TipType.NoLayer);
            }
        }

        private bool SelectionFlood(Vector2 position, BitmapLayer bitmapLayer, bool isSubtract)
        {
            bool result = bitmapLayer.FloodSelect(position, Windows.UI.Colors.DodgerBlue);

            if (result is false)
            {
                this.Tip(TipType.NoPixel);
                return false;
            }

            ICanvasImage floodSelect = bitmapLayer[BitmapType.Temp];
            Color[] interpolationColors = this.Marquee.GetInterpolationColors(floodSelect);
            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(interpolationColors);

            switch (mode)
            {
                case PixelBoundsMode.Transarent:
                    this.Tip(TipType.NoPixelForMarquee);
                    return false;
                case PixelBoundsMode.Solid:
                    this.EditMenu.Execute(isSubtract ? OptionType.Deselect : OptionType.All);
                    return true;
                default:
                    // History
                    int removes = this.History.Push
                    (
                        isSubtract ?
                        this.Marquee.Clear(bitmapLayer, interpolationColors, BitmapType.Temp) :
                        this.Marquee.Add(bitmapLayer, interpolationColors, BitmapType.Temp)
                    );

                    this.Marquee.Flush();
                    this.Marquee.RenderThumbnail();

                    this.RaiseHistoryCanExecuteChanged();
                    return true;
            }
        }

    }
}