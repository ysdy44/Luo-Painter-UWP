using Luo_Painter.Elements;
using Luo_Painter.Historys;
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
                if (this.History.CanUndo == false) return;

                // History
                bool result = this.History.Undo(this.BitmapLayer.Undo);
                if (result == false) return;

                this.CanvasControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
                this.ListView.SelectedIndex = this.History.Index;
            };
            this.RedoButton.Click += (s, e) =>
            {
                if (this.History.CanRedo == false) return;

                // History
                bool result = this.History.Redo(this.BitmapLayer.Redo);
                if (result == false) return;

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
                    this.History.SetIndex(index, this.BitmapLayer.Undo, this.BitmapLayer.Redo);

                    this.CanvasControl.Invalidate(); // Invalidate

                    this.UndoButton.IsEnabled = this.History.CanUndo;
                    this.RedoButton.IsEnabled = this.History.CanRedo;
                    this.ListView.SelectedIndex = this.History.Index;
                }
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.History.SetIndex(-1, this.BitmapLayer.Undo, this.BitmapLayer.Redo);

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

                args.DrawingSession.DrawImage(this.BitmapLayer.Source);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                Rect rect = position.GetRect(12 * properties.Pressure);
                this.BitmapLayer.Hit(rect);
                this.BitmapLayer.FillCircleDry(this.Position, position, 1, 1, 12, Colors.White);
                this.Position = position;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
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

    }
}