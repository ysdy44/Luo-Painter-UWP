using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class HistorianPage : Page
    {

        Vector2 StartingPosition;
        Vector2 Position;

        BitmapLayer BitmapLayer;
        Historian<IHistory> History { get; } = new Historian<IHistory>(20);

        public HistorianPage()
        {
            this.InitializeComponent();
            this.ConstructHistory();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructHistory()
        {
            this.UndoButton.Click += (s, e) =>
            {
                if (this.History.CanUndo is false) return;

                // History
                bool result = this.History.Undo(this.Undo);
                if (result is false) return;

                this.CanvasControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
                this.ListView.SelectedIndex = this.History.Index;
            };
            this.RedoButton.Click += (s, e) =>
            {
                if (this.History.CanRedo is false) return;

                // History
                bool result = this.History.Redo(this.Redo);
                if (result is false) return;

                this.CanvasControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
                this.ListView.SelectedIndex = this.History.Index;
            };

            this.ItemsControl.ItemsSource = Enumerable.Range(-1, 1 + 20);
            this.ListView.ItemsSource = this.History.Source;
            this.ListView.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.ItemsControl.Height = e.NewSize.Height;
            };

            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is IHistory item)
                {
                    int index = this.History.IndexOf(item);
                    this.History.SetIndex(index, this.Undo, this.Redo);

                    this.CanvasControl.Invalidate(); // Invalidate

                    this.UndoButton.IsEnabled = this.History.CanUndo;
                    this.RedoButton.IsEnabled = this.History.CanRedo;
                    this.ListView.SelectedIndex = this.History.Index;
                }
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.History.SetIndex(-1, this.Undo, this.Redo);

                this.CanvasControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
                this.ListView.SelectedIndex = this.History.Index;
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(sender, 512, 512);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawRectangle(0, 0, this.BitmapLayer.Width, this.BitmapLayer.Height, Colors.Red);

                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Source]);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.StartingPosition = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, 1, 1, 12, 0.25f);
                if (segment.InRadius) return;

                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                {
                    ds.DrawLine(segment.StartingPosition, segment.Position, Colors.White);
                }

                Rect rect = this.Position.GetRect(12 * properties.Pressure);
                this.BitmapLayer.Hit(rect);

                this.CanvasControl.Invalidate(); // Invalidate

                this.StartingPosition = this.Position;
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                this.BitmapLayer.Flush();
                this.BitmapLayer.RenderThumbnail();
                this.CanvasControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
                this.ListView.SelectedIndex = this.History.Index;
            };
        }

        public bool Undo(IHistory history)
        {
            switch (history.Mode)
            {
                case HistoryMode.Property:
                    if (history is IPropertyHistory propertyHistory)
                    {
                        return this.BitmapLayer.History(propertyHistory.Type, propertyHistory.UndoParameter);
                    }
                    else return false;
                default:
                    return false;
            }
        }
        public bool Redo(IHistory history)
        {
            switch (history.Mode)
            {
                case HistoryMode.Property:
                    if (history is IPropertyHistory propertyHistory)
                    {
                        return this.BitmapLayer.History(propertyHistory.Type, propertyHistory.RedoParameter);
                    }
                    else return false;
                default:
                    return false;
            }
        }

    }
}