using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        bool FloodIsContiguous => this.SelectionContiguousButton.IsChecked is true;
        float FloodTolerance => (float)(this.SelectionFloodToleranceSlider.Value / 100);
        bool FloodFeather => this.SelectionFeatherButton.IsChecked is true;

        bool SelectionIsSubtract => this.SelectionComboBox.SelectedIndex is 0 is false;
        float SelectionSize = 32;

        private void ConstructSelection()
        {
            // 1.Minimum
            this.SelectionBrushSizeSlider.Minimum = this.SizeRange.XRange.Minimum;

            // 2.Value
            this.SelectionBrushSizeSlider.Value = this.SizeRange.ConvertYToX(this.SelectionSize);

            // 3.Maximum
            this.SelectionBrushSizeSlider.Maximum = this.SizeRange.XRange.Maximum;

            this.SelectionBrushSizeSlider.ValueChanged += (s, e) =>
            {
                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.SelectionSize = (float)size;
            };
        }

        private void SelectionBrush_Start()
        {
            this.Marquee.Marquee(this.StartingPosition, this.StartingPosition, this.SelectionSize, this.SelectionIsSubtract);
            this.Marquee.Hit(RectExtensions.GetRect(this.StartingPosition, this.SelectionSize));

            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void SelectionBrush_Delta()
        {
            this.Marquee.Marquee(this.StartingPosition, this.Position, this.SelectionSize, this.SelectionIsSubtract);
            this.Marquee.Hit(RectExtensions.GetRect(this.StartingPosition, this.Position, this.SelectionSize));

            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasAnimatedControl.Invalidate(); // Invalidate

            this.StartingPosition = this.Position;
        }
        private void SelectionBrush_Complete()
        {
            // History
            IHistory history = this.Marquee.GetBitmapHistory();
            int removes = this.History.Push(history);

            this.Marquee.Flush();
            this.Marquee.RenderThumbnail();
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

        private void SelectionFlood_Complete()
        {
            if (this.LayerSelectedItem is BitmapLayer bitmapLayer)
            {
                this.SelectionFlood(this.Position, bitmapLayer);
            }
            else
            {
                this.Tip(TipType.NoLayer);
            }
        }

        private bool SelectionFlood(Vector2 position, BitmapLayer bitmapLayer)
        {
            bool result = bitmapLayer.FloodSelect(position, Colors.DodgerBlue, this.FloodIsContiguous, this.FloodTolerance, this.FloodFeather);

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
                    this.Click(OptionType.All);
                    return true;
                default:
                    // History
                    IHistory history = this.Marquee.New(bitmapLayer, interpolationColors, BitmapType.Temp);
                    int removes = this.History.Push(history);

                    this.Marquee.Flush();
                    this.Marquee.RenderThumbnail();

                    this.RaiseHistoryCanExecuteChanged();
                    return true;
            }
        }

    }
}