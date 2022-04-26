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
            switch (this.ToolType)
            {
                case ToolType.MarqueeRectangular:
                case ToolType.MarqueeElliptical:
                case ToolType.MarqueePolygon:
                case ToolType.MarqueeFreeHand:
                    this.MarqueeToolType = this.GetMarqueeToolType(this.ToolType);
                    this.StartingPosition = this.Position = this.ToPosition(point);
                    this.MarqueeTool.Start(this.StartingPosition, this.MarqueeToolType, false, false);
                    break;
                case ToolType.MarqueeSelectionBrush:
                    this.Position = this.ToPosition(point);
                    break;
                default:
                    break;
            }
        }
        private void Marquee_Delta(Vector2 point)
        {
            switch (this.ToolType)
            {
                case ToolType.MarqueeRectangular:
                case ToolType.MarqueeElliptical:
                case ToolType.MarqueePolygon:
                case ToolType.MarqueeFreeHand:
                    this.Position = this.ToPosition(point);
                    this.MarqueeTool.Delta(this.StartingPosition, this.Position, this.MarqueeToolType, false, false);
                    break;
                case ToolType.MarqueeSelectionBrush:
                    Vector2 position = this.ToPosition(point);
                    this.Marquee.Marquee(this.Position, position, 32, false);
                    this.Marquee.Hit(RectExtensions.GetRect(this.Position, position, 32));
                    this.Position = position;
                    break;
                default:
                    break;
            }
        }

        private void Marquee_Complete(Vector2 point)
        {
            switch (this.ToolType)
            {
                case ToolType.MarqueeRectangular:
                case ToolType.MarqueeElliptical:
                case ToolType.MarqueePolygon:
                case ToolType.MarqueeFreeHand:
                    this.Position = this.ToPosition(point);
                    bool redraw = this.MarqueeTool.Complete(this.StartingPosition, this.Position, this.MarqueeToolType, false, false);
                    if (redraw is false) break;

                    using (CanvasDrawingSession ds = this.Marquee.CreateSourceDrawingSession())
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                        ds.FillMarqueeMaskl(this.CanvasAnimatedControl, this.MarqueeToolType, this.MarqueeTool, new Rect(0, 0, this.Transformer.Width, this.Transformer.Height), this.MarqueeCompositeMode);
                    }
                    this.MarqueeToolType = MarqueeToolType.None;
                    break;
                case ToolType.MarqueeSelectionBrush:
                    {
                        // History
                        int removes = this.History.Push(this.Marquee.GetBitmapHistory());
                        this.Marquee.Flush();

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
                    }
                    break;
                case ToolType.MarqueeFloodSelect:
                    if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
                    {
                        bool result = bitmapLayer.FloodSelect(this.ToPosition(point), Windows.UI.Colors.DodgerBlue);
                        if (result)
                        {
                            this.Marquee.CopyPixels(bitmapLayer);

                            // History
                            int removes = this.History.Push(this.Marquee.GetBitmapResetHistory());
                            this.Marquee.Flush();

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        else
                        {
                            this.Tip("No Pixel", "The current Pixel is Transparent.");
                        }
                    }
                    break;
                default:
                    break;
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