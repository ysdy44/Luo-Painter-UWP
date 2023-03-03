using FanKit.Transformers;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        MarqueeCompositeMode MarqueeCompositeMode
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

        MarqueeToolType MarqueeToolType;
        readonly MarqueeTool MarqueeTool = new MarqueeTool();

        private void Marquee_Start()
        {
            this.MarqueeToolType = this.GetMarqueeToolType(this.OptionType);
            this.MarqueeTool.Start(this.StartingPosition, this.MarqueeToolType, this.IsCtrl, this.IsShift);

            this.CanvasAnimatedControl.Paused = true; // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void Marquee_Delta()
        {
            this.MarqueeTool.Delta(this.StartingPosition, this.Position, this.MarqueeToolType, this.IsCtrl, this.IsShift);

            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void Marquee_Complete()
        {
            bool redraw = this.MarqueeTool.Complete(this.StartingPosition, this.Position, this.MarqueeToolType, this.IsCtrl, this.IsShift);
            if (redraw is false) return;

            using (CanvasDrawingSession ds = this.Marquee.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.FillMarqueeMask(this.CanvasAnimatedControl, this.MarqueeToolType, this.MarqueeTool, new Rect(0, 0, this.Transformer.Width, this.Transformer.Height), this.MarqueeCompositeMode);
            }

            // History
            int removes = this.History.Push(this.Marquee.GetBitmapResetHistory());
            this.Marquee.Flush();
            this.Marquee.RenderThumbnail();

            this.CanvasAnimatedControl.Paused = this.OptionType.HasPreview(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.MarqueeToolType = MarqueeToolType.None;
        }

        private MarqueeToolType GetMarqueeToolType(OptionType type)
        {
            switch (type)
            {
                case OptionType.MarqueeRectangular: return MarqueeToolType.Rectangular;
                case OptionType.MarqueeElliptical: return MarqueeToolType.Elliptical;
                case OptionType.MarqueePolygon: return MarqueeToolType.Polygonal;
                case OptionType.MarqueeFreeHand: return MarqueeToolType.FreeHand;
                default: return MarqueeToolType.None;
            }
        }

    }
}