using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        private void GeometryRectangle_Start(Vector2 position, Vector2 point)
        {
            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void GeometryRectangle_Delta(Vector2 position, Vector2 point)
        {
            if (this.BitmapLayer is null) return;

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                ds.FillRectangle(System.Math.Min(this.StartingPosition.X, position.X), System.Math.Min(this.StartingPosition.Y, position.Y), System.Math.Abs(this.StartingPosition.X - position.X), System.Math.Abs(this.StartingPosition.Y - position.Y), this.ColorMenu.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void GeometryRectangle_Complete(Vector2 position, Vector2 point)
        {
            if (this.BitmapLayer is null) return;

            Rect rect = new Rect(this.StartingPosition.ToPoint(), position.ToPoint());
            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
            {
                ds.FillRectangle(rect, this.ColorMenu.Color);
            }
            this.BitmapLayer.Hit(rect);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();

            this.BitmapLayer = null;
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

    }
}