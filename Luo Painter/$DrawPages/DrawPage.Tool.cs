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

        private void Tool_Start()
        {
            switch (this.OptionType)
            {
                case OptionType.Feather:
                    break;
                case OptionType.MarqueeTransform:
                    this.Transform_Start();
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;

                case OptionType.CropCanvas:
                    this.CropCanvas_Start();
                    break;

                case OptionType.Transform:
                    this.Transform_Start();
                    break;
                case OptionType.DisplacementLiquefaction:
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Start();
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.Marquee_Start();
                    break;

                case OptionType.SelectionBrush:
                    this.SelectionBrush_Start();
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Start();
                    break;

                case OptionType.View:
                    this.View_Start();
                    break;
                case OptionType.Brush:
                    this.Brush_Start();
                    break;
                case OptionType.Transparency:
                    this.Transparency_Start();
                    break;

                case OptionType.Pen:
                    this.Pen_Start();
                    break;

                default:
                    if (this.OptionType.IsGeometry())
                    {
                        this.Geometry_Start();
                    }
                    break;
            }
        }

        private void Tool_Delta()
        {
            switch (this.OptionType)
            {
                case OptionType.Feather:
                    break;
                case OptionType.MarqueeTransform:
                    this.Transform_Delta();
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;

                case OptionType.CropCanvas:
                    this.CropCanvas_Delta();
                    break;

                case OptionType.Transform:
                    this.Transform_Delta();
                    break;
                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Delta();
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Delta();
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.Marquee_Delta();
                    break;

                case OptionType.SelectionBrush:
                    this.SelectionBrush_Delta();
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Delta();
                    break;

                case OptionType.View:
                    this.View_Delta();
                    break;
                case OptionType.Brush:
                    this.Brush_Delta();
                    break;
                case OptionType.Transparency:
                    this.Transparency_Delta();
                    break;

                case OptionType.Pen:
                    this.Pen_Delta();
                    break;

                default:
                    if (this.OptionType.IsGeometry())
                    {
                        this.GeometryTransform_Delta();
                    }
                    break;
            }
        }

        private void Tool_Complete()
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
                    this.CropCanvas_Complete();
                    break;

                case OptionType.Transform:
                    break;
                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Complete();
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    break;

                case OptionType.MarqueeRectangular:
                case OptionType.MarqueeElliptical:
                case OptionType.MarqueePolygon:
                case OptionType.MarqueeFreeHand:
                    this.Marquee_Complete();
                    break;

                case OptionType.SelectionFlood:
                    this.SelectionFlood_Complete();
                    break;
                case OptionType.SelectionBrush:
                    this.SelectionBrush_Complete();
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintWatercolorPen:
                case OptionType.PaintPencil:
                case OptionType.PaintEraseBrush:
                case OptionType.PaintLiquefaction:
                    this.Paint_Complete();
                    break;

                case OptionType.View:
                    this.View_Complete();
                    break;
                case OptionType.Brush:
                    this.Brush_Complete();
                    break;
                case OptionType.Transparency:
                    this.Transparency_Complete();
                    break;

                case OptionType.Pen:
                    this.Pen_Complete();
                    break;

                default:
                    if (this.OptionType.IsGeometry())
                    {
                        this.GeometryTransform_Delta();
                    }
                    break;
            }
        }

    }
}