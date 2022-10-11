using FanKit.Transformers;
using Luo_Painter.Options;
using Luo_Painter.Layers.Models;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Luo_Painter.Layers;
using Luo_Painter.Brushes;
using Luo_Painter.Blends;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        MarqueeToolType MarqueeToolType;
        readonly MarqueeTool MarqueeTool = new MarqueeTool();

        private void Marquee_Start(Vector2 position)
        {
            this.MarqueeToolType = this.GetMarqueeToolType(this.OptionType);
            this.MarqueeTool.Start(position, this.MarqueeToolType, this.IsCtrl, this.IsShift);

            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void Marquee_Delta(Vector2 position)
        {
            this.MarqueeTool.Delta(this.StartingPosition, position, this.MarqueeToolType, this.IsCtrl, this.IsShift);

            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void Marquee_Complete(Vector2 position)
        {
            bool redraw = this.MarqueeTool.Complete(this.StartingPosition, position, this.MarqueeToolType, this.IsCtrl, this.IsShift);
            if (redraw is false) return;

            using (CanvasDrawingSession ds = this.Marquee.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.FillMarqueeMaskl(this.CanvasAnimatedControl, this.MarqueeToolType, this.MarqueeTool, new Rect(0, 0, this.Transformer.Width, this.Transformer.Height), this.AppBar.MarqueeCompositeMode);
            }

            // History
            int removes = this.History.Push(this.Marquee.GetBitmapResetHistory());
            this.Marquee.Flush();
            this.Marquee.RenderThumbnail();

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