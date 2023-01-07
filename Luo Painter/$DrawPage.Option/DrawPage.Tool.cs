using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage
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

                case OptionType.Move:
                    this.Move_Start();
                    break;
                case OptionType.Transform:
                    this.Transform_Start();
                    break;
                case OptionType.FreeTransform:
                    this.FreeTransform_Start();
                    break;

                case OptionType.DisplacementLiquefaction:
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Start();
                    break;

                case OptionType.Border:
                    this.Border_Start();
                    break;

                case OptionType.Lighting:
                    this.Lighting_Start();
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
                    this.PaintBrush_Start();
                    break;
                case OptionType.PaintLine:
                    this.PaintLine_Start();
                    break;
                case OptionType.PaintBrushForce:
                    this.PaintBrushForce_Start();
                    break;
                case OptionType.PaintBrushMulti:
                    this.PaintBrushMulti_Start();
                    break;

                case OptionType.View:
                    this.View_Start();
                    break;
                case OptionType.Straw:
                    this.Straw_Start();
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

                case OptionType.Move:
                    this.Move_Delta();
                    break;
                case OptionType.Transform:
                    this.Transform_Delta();
                    break;
                case OptionType.FreeTransform:
                    this.FreeTransform_Delta();
                    break;

                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Delta();
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    this.RippleEffect_Delta();
                    break;

                case OptionType.Border:
                    this.Border_Delta();
                    break;

                case OptionType.Lighting:
                    this.Lighting_Delta();
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
                    this.PaintBrush_Delta();
                    break;
                case OptionType.PaintLine:
                    this.PaintLine_Delta();
                    break;
                case OptionType.PaintBrushForce:
                    this.PaintBrushForce_Delta();
                    break;
                case OptionType.PaintBrushMulti:
                    this.PaintBrushMulti_Delta();
                    break;

                case OptionType.View:
                    this.View_Delta();
                    break;
                case OptionType.Straw:
                    this.Straw_Delta();
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
                    this.Transform_Complete();
                    break;
                case OptionType.Grow:
                    break;
                case OptionType.Shrink:
                    break;

                case OptionType.CropCanvas:
                    this.CropCanvas_Complete();
                    break;

                case OptionType.Move:
                    this.Move_Complete();
                    break;
                case OptionType.Transform:
                    this.Transform_Complete();
                    break;
                case OptionType.FreeTransform:
                    this.FreeTransform_Complete();
                    break;

                case OptionType.DisplacementLiquefaction:
                    this.DisplacementLiquefaction_Complete();
                    break;
                case OptionType.GradientMapping:
                    break;
                case OptionType.RippleEffect:
                    break;

                case OptionType.Border:
                    this.Border_Complete();
                    break;

                case OptionType.Lighting:
                    this.Lighting_Complete();
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
                    this.PaintBrush_Complete();
                    break;
                case OptionType.PaintLine:
                    this.PaintLine_Complete();
                    break;
                case OptionType.PaintBrushForce:
                    this.PaintBrushForce_Complete();
                    break;
                case OptionType.PaintBrushMulti:
                    this.PaintBrushMulti_Complete();
                    break;

                case OptionType.View:
                    this.View_Complete();
                    break;
                case OptionType.Straw:
                    this.Straw_Complete();
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