using FanKit.Transformers;
using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private void GeometryInvalidate()
        {
            if (this.BitmapLayer is null) return;

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                ds.FillGeometry(this.AppBar.CreateGeometry(this.CanvasDevice, this.OptionType, this.BoundsTransformer), this.ColorMenu.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void CancelGeometryTransform() { }
        private void PrimaryGeometryTransform()
        {
            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
            {
                ds.FillGeometry(this.AppBar.CreateGeometry(this.CanvasDevice, this.OptionType, this.BoundsTransformer), this.ColorMenu.Color);
            }
            this.BitmapLayer.Hit(this.BoundsTransformer);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }


        private void GeometryTransform_Delta(Vector2 position, Vector2 point)
        {
            if (this.IsBoundsMove)
            {
                this.BoundsTransformer = this.StartingBoundsTransformer + (position - this.StartingPosition);
            }
            else if (this.BoundsMode == default)
            {
                this.BoundsTransformer = new Transformer(this.StartingPosition, position, this.IsCtrl, this.IsShift);
            }
            else
            {
                this.BoundsTransformer = FanKit.Transformers.Transformer.Controller(this.BoundsMode, this.StartingPosition, position, this.StartingBoundsTransformer, this.IsShift, this.IsCtrl);
            }

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                ds.FillGeometry(this.AppBar.CreateGeometry(this.CanvasDevice, this.OptionType, this.BoundsTransformer), this.ColorMenu.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }


        private void Geometry_Start(Vector2 position, Vector2 point)
        {
            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            if (this.OptionType.HasPreview())
            {
                // GeometryTransform_Start
                this.Transform_Start(point);
                return;
            }

            this.IsBoundsMove = false;
            this.BoundsMode = default;
            this.BoundsTransformer = new Transformer(this.StartingPosition, position, this.IsCtrl, this.IsShift);

            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

            this.OptionType = this.OptionType.ToGeometryTransform();
            this.AppBar.Construct(this.OptionType);
            this.SetCanvasState(true);
        }

    }
}