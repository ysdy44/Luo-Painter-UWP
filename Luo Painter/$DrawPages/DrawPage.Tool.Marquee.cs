using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        MarqueeToolType MarqueeToolType;
        readonly MarqueeTool MarqueeTool = new MarqueeTool();

        public MarqueeCompositeMode MarqueeCompositeMode
        {
            get
            {
                switch (this.MarqueeComboBox.SelectedIndex)
                {
                    case 0: return MarqueeCompositeMode.New;
                    case 1: return MarqueeCompositeMode.Add;
                    case 2: return MarqueeCompositeMode.Subtract;
                    case 3: return MarqueeCompositeMode.Intersect;
                    default: return MarqueeCompositeMode.New;
                }
            }
        }

        private void ConstructMarquee()
        {
        }

        private void Marquee_Start(Vector2 point)
        {
            this.MarqueeToolType = this.GetMarqueeToolType(this.ToolType);
            this.StartingPosition = this.Position = this.ToPosition(point);
            this.MarqueeTool.Start(this.StartingPosition, this.MarqueeToolType, this.IsCtrl, this.IsShift);

            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void Marquee_Delta(Vector2 point)
        {
            this.Position = this.ToPosition(point);
            this.MarqueeTool.Delta(this.StartingPosition, this.Position, this.MarqueeToolType, this.IsCtrl, this.IsShift);

            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void Marquee_Complete(Vector2 point)
        {
            this.Position = this.ToPosition(point);
            bool redraw = this.MarqueeTool.Complete(this.StartingPosition, this.Position, this.MarqueeToolType, this.IsCtrl, this.IsShift);
            if (redraw is false) return;

            using (CanvasDrawingSession ds = this.Marquee.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.FillMarqueeMaskl(this.CanvasAnimatedControl, this.MarqueeToolType, this.MarqueeTool, new Rect(0, 0, this.Transformer.Width, this.Transformer.Height), this.MarqueeCompositeMode);
            }

            // History
            int removes = this.History.Push(this.Marquee.GetBitmapResetHistory());
            this.Marquee.Flush();
            this.Marquee.RenderThumbnail();

            this.CanvasControl.Invalidate(); // Invalidate
            this.MarqueeToolType = MarqueeToolType.None;
        }

        private void SelectionFlood(Vector2 point, BitmapLayer bitmapLayer)
        {
            this.Position = this.ToPosition(point);
            bool result = bitmapLayer.FloodSelect(this.Position, Windows.UI.Colors.DodgerBlue);
            if (result)
            {
                this.Marquee.CopyPixels(bitmapLayer, BitmapType.Temp);

                // History
                int removes = this.History.Push(this.Marquee.GetBitmapResetHistory());
                this.Marquee.Flush();
                this.Marquee.RenderThumbnail();

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            }
            else
            {
                this.Tip("No Pixel", "The current Pixel is Transparent.");
            }
        }

        private MarqueeToolType GetMarqueeToolType(ToolType type)
        {
            switch (type)
            {
                case ToolType.MarqueeRectangular: return MarqueeToolType.Rectangular;
                case ToolType.MarqueeElliptical: return MarqueeToolType.Elliptical;
                case ToolType.MarqueePolygon: return MarqueeToolType.Polygonal;
                case ToolType.MarqueeFreeHand: return MarqueeToolType.FreeHand;
                default: return MarqueeToolType.None;
            }
        }

    }
}