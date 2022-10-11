using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
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
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private void Tool_Start(Vector2 position, Vector2 point, float pressure = 0.5f)
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
                    this.Marquee_Start(position);
                    break;

                case OptionType.SelectionBrush:
                    this.SelectionBrush_Start(position);
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Start();
                    break;

                case OptionType.View:
                    this.View_Start(point);
                    break;
                case OptionType.Brush:
                    this.Brush_Start(position);
                    break;
                case OptionType.Transparency:
                    this.Transparency_Start(position, point);
                    break;

                case OptionType.Pen:
                    this.Pen_Start(position);
                    break;

                default:
                    if (this.OptionType.IsGeometry())
                    {
                        this.Geometry_Start(position, point);
                    }
                    break;
            }
        }

        private void Tool_Delta(Vector2 position, Vector2 point, float pressure = 0.5f)
        {
            switch (this.OptionType)
            {
                case OptionType.Feather:
                    break;
                case OptionType.MarqueeTransform:
                    this.Transform_Delta(position, point);
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;

                case OptionType.CropCanvas:
                    this.CropCanvas_Delta(point);
                    break;

                case OptionType.Transform:
                    this.Transform_Delta(position, point);
                    break;
                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Delta(position, point);
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Delta(position, point);
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.Marquee_Delta(position);
                    break;

                case OptionType.SelectionBrush:
                    this.SelectionBrush_Delta(position);
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Delta(position, point, pressure);
                    break;

                case OptionType.View:
                    this.View_Delta(point);
                    break;
                case OptionType.Brush:
                    this.Brush_Delta(position, point);
                    break;
                case OptionType.Transparency:
                    this.Transparency_Delta(position, point);
                    break;

                case OptionType.Pen:
                    this.Pen_Delta(position);
                    break;

                default:
                    if (this.OptionType.IsGeometry())
                    {
                        this.GeometryTransform_Delta(position, point);
                    }
                    break;
            }
        }

        private void Tool_Complete(Vector2 position, Vector2 point, float pressure = 0.5f)
        {
            switch (this.OptionType)
            {
                case OptionType.Feather:
                    break;
                case OptionType.MarqueeTransform:
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;

                case OptionType.CropCanvas:
                    this.CropCanvas_Complete(point);
                    break;

                case OptionType.Transform:
                    break;
                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Complete(point);
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.Marquee_Complete(position);
                    break;

                case OptionType.SelectionFlood:
                    this.SelectionFlood_Complete(position, point);
                    break;
                case OptionType.SelectionBrush:
                    this.SelectionBrush_Complete(position);
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Complete();
                    break;

                case OptionType.View:
                    this.View_Complete(point);
                    break;
                case OptionType.Brush:
                    this.Brush_Complete(position);
                    break;
                case OptionType.Transparency:
                    this.Transparency_Complete(position, point);
                    break;

                case OptionType.Pen:
                    this.Pen_Complete(position);
                    break;

                default:
                    if (this.OptionType.IsGeometry())
                    {
                        this.GeometryTransform_Delta(position, point);
                    }
                    break;
            }
        }

    }
}