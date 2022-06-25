using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        private void ConstructTools()
        {
            this.ToolListView.ItemClick += (s, type) =>
            {
                this.ToolMenu.Title = type.ToString();

                this.ToolResource.Source = new Uri(type.GetResource());
                this.ToolIcon.Template = type.GetTemplate(this.ToolResource);

                this.ToolSwitchPresenter.Value = type;

                this.SetInkToolType(type);

                this.OptionType = type;
                this.SetFootType(type);
                this.SetCanvasState(default);
            };
            this.ToolListView.Construct(this.OptionType);
        }


        private void Tool_Start(Vector2 point, float pressure = 0.5f)
        {
            switch (this.OptionType)
            {
                case OptionType.Feather:
                    break;
                case OptionType.MarqueeTransform:
                    this.Transform_Start(point);
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;

                case OptionType.CropCanvas:
                    this.CropCanvas_Start(point);
                    break;

                case OptionType.Transform:
                    this.Transform_Start(point);
                    break;
                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Start(point);
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Start(point);
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.Marquee_Start(point);
                    break;

                case OptionType.SelectionBrush:
                    this.Position = this.ToPosition(point);
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Start(point, pressure);
                    break;

                case OptionType.View:
                    this.View_Start(point);
                    break;
                case OptionType.Brush:
                    this.Brush_Start(point);
                    break;
                case OptionType.Transparency:
                    this.Transparency_Start(point);
                    break;
            }
        }

        private void Tool_Delta(Vector2 point, float pressure = 0.5f)
        {
            switch (this.OptionType)
            {
                case OptionType.Feather:
                    break;
                case OptionType.MarqueeTransform:
                    this.Transform_Delta(point);
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;

                case OptionType.CropCanvas:
                    this.CropCanvas_Delta(point);
                    break;

                case OptionType.Transform:
                    this.Transform_Delta(point);
                    break;
                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Delta(point);
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Delta(point);
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.Marquee_Delta(point);
                    break;

                case OptionType.SelectionBrush:
                    Vector2 position = this.ToPosition(point);
                    bool isSubtract = this.SelectionComboBox.SelectedIndex != 0;
                    this.Marquee.Marquee(this.Position, position, 32, isSubtract);
                    this.Marquee.Hit(RectExtensions.GetRect(this.Position, position, 32));
                    this.Position = position;
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Delta(point, pressure);
                    break;

                case OptionType.View:
                    this.View_Delta(point);
                    break;
                case OptionType.Brush:
                    this.Brush_Delta(point);
                    break;
                case OptionType.Transparency:
                    this.Transparency_Delta(point);
                    break;
            }
        }

        private void Tool_Complete(Vector2 point, float pressure = 0.5f)
        {
            switch (this.OptionType)
            {
                case OptionType.Feather:
                    break;
                case OptionType.MarqueeTransform:
                    this.Transform_Complete(point);
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;

                case OptionType.CropCanvas:
                    this.CropCanvas_Complete(point);
                    break;

                case OptionType.Transform:
                    this.Transform_Complete(point);
                    break;
                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Complete(point);
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Complete(point);
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.Marquee_Complete(point);
                    break;

                case OptionType.SelectionFlood:
                    if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
                    {
                        bool isSubtract = this.SelectionComboBox.SelectedIndex != 0;
                        this.SelectionFlood(point, bitmapLayer, isSubtract);
                    }
                    else
                    {
                        this.Tip("No Layer", "Create a new Layer?");
                    }
                    break;
                case OptionType.SelectionBrush:
                    {
                        // History
                        int removes = this.History.Push(this.Marquee.GetBitmapHistory());
                        this.Marquee.Flush();
                        this.Marquee.RenderThumbnail();

                        this.UndoButton.IsEnabled = this.History.CanUndo;
                        this.RedoButton.IsEnabled = this.History.CanRedo;
                    }
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Complete(point, pressure);
                    break;

                case OptionType.View:
                    this.View_Complete(point);
                    break;
                case OptionType.Brush:
                    this.Brush_Complete(point);
                    break;
                case OptionType.Transparency:
                    this.Transparency_Complete(point);
                    break;
            }
        }

    }
}