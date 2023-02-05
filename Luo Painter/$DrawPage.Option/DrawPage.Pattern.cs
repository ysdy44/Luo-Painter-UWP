using FanKit.Transformers;
using Luo_Painter.Blends;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public void ConstructPattern()
        {
        }

        private void PatternInvalidate()
        {
            if (this.BitmapLayer is null) return;

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                this.DrawPattern(ds, this.OptionType, this.CreateTransform.Transformer, this.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void CancelPatternTransform()
        {
            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }
        private void PrimaryPatternTransform()
        {
            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
            {
                this.DrawPattern(ds, this.OptionType, this.CreateTransform.Transformer, this.Color);
            }
            this.BitmapLayer.Hit(this.CreateTransform.Transformer);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();


            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }


        private void PatternTransform_Delta()
        {
            if (this.BitmapLayer is null) return;

            if (this.CreateTransform.IsMove)
                this.CreateTransform.Transformer = this.CreateTransform.StartingTransformer + (this.Position - this.StartingPosition);
            else if (this.CreateTransform.Mode == default)
                this.CreateTransform.Transformer = new Transformer(this.StartingPosition, this.Position, this.IsCtrl, this.IsShift);
            else
                this.CreateTransform.Transformer = FanKit.Transformers.Transformer.Controller(this.CreateTransform.Mode, this.StartingPosition, this.Position, this.CreateTransform.StartingTransformer, this.IsShift, this.IsCtrl);

            using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
            {
                ds.Clear(Colors.Transparent);
                this.DrawPattern(ds, this.OptionType, this.CreateTransform.Transformer, this.Color);
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }


        private void Pattern_Start()
        {
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

            if (this.OptionType.HasPreview())
            {
                // PatternTransform_Start
                this.Transform_Start();
                return;
            }

            this.CreateTransform.IsMove = false;
            this.CreateTransform.Mode = default;
            this.CreateTransform.Transformer = new Transformer(this.StartingPosition, this.Position, this.IsCtrl, this.IsShift);

            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);


            this.OptionType = this.OptionType.ToPatternTransform();
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(true); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        public void DrawPattern(CanvasDrawingSession ds, OptionType type, ITransformerLTRB transformerLTRB, Color color)
        {
            switch (type)
            {
                case OptionType.PatternGrid:
                case OptionType.PatternGridTransform:
                    break;
                case OptionType.PatternDiagonal:
                case OptionType.PatternDiagonalTransform:
                    break;
                case OptionType.PatternSpotted:
                case OptionType.PatternSpottedTransform:
                    break;
                default:
                    break;
            }
        }

    }
}