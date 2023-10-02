using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private void PaintLine_Start()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.Tasks.State != default) return;

            if (this.LayerSelectedItem is null)
            {
                this.Tip(TipType.NoLayer);
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip(TipType.NotBitmapLayer);
                return;
            }

            this.CanvasAnimatedControl.Paused = true; // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void PaintLine_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void PaintLine_Complete()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasControl.Invalidate(); // Invalidate

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            if (segment.InRadius) return;

            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(segment.Bounds);
                this.PaintSegment(segment);

                if (this.InkType is InkType.Liquefy is false)
                {
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                    using (ds.CreateLayer(1f, segment.Bounds))
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.Preview(ds, this.InkType, this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]);
                    }
                }
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
            }

            // History
            IHistory history = this.BitmapLayer.GetBitmapHistory();
            history.Title = App.Resource.GetString(this.OptionType.ToString());
            int removes = this.History.Push(history);

            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();

            this.BitmapLayer = null;
            this.CanvasAnimatedControl.Paused = this.OptionType.HasPreview(); // Invalidate
            Rect? region = RectExtensions.TryGetRect(this.StartingPoint, this.Point, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(segment.Size * this.Transformer.Scale));
            if (region.HasValue)
            {
                this.CanvasVirtualControl.Invalidate(region.Value);
            }

            this.RaiseHistoryCanExecuteChanged();
        }

    }
}