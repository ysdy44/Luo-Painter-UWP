using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private void PaintCap(StrokeCap cap)
        {
            switch (this.InkType)
            {
                case InkType.General:
                case InkType.General_Grain:
                case InkType.General_Opacity:
                case InkType.General_Opacity_Grain:
                case InkType.General_Blend:
                case InkType.General_Grain_Blend:
                case InkType.General_Opacity_Blend:
                case InkType.General_Opacity_Grain_Blend:
                case InkType.Blur:
                    this.BitmapLayer.CapDrawShaderBrushEdgeHardness(cap,
                        this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.General_Mix:
                case InkType.General_Grain_Mix:
                case InkType.General_Opacity_Mix:
                case InkType.General_Opacity_Grain_Mix:
                case InkType.General_Blend_Mix:
                case InkType.General_Grain_Blend_Mix:
                case InkType.General_Opacity_Blend_Mix:
                case InkType.General_Opacity_Grain_Blend_Mix:
                    this.BitmapLayer.CapDrawShaderBrushEdgeHardness(cap,
                        this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr,
                        this.InkPresenter.Mix, this.InkPresenter.Wet, this.InkPresenter.Persistence,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                    this.BitmapLayer.CapDrawShaderBrushEdgeHardnessWithTexture(cap,
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr,
                        this.InkPresenter.ShapeSource, this.InkPresenter.Rotate,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.ShapeGeneral_Mix:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.ShapeGeneral_Opacity_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Mix:
                case InkType.ShapeGeneral_Blend_Mix:
                case InkType.ShapeGeneral_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Blend_Mix:
                    this.BitmapLayer.CapDrawShaderBrushEdgeHardnessWithTexture(cap,
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr,
                        this.InkPresenter.ShapeSource, this.InkPresenter.Rotate,
                        this.InkPresenter.Mix, this.InkPresenter.Wet, this.InkPresenter.Persistence,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.Tip:
                case InkType.Tip_Grain:
                case InkType.Tip_Opacity:
                case InkType.Tip_Opacity_Grain:
                case InkType.Tip_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Tip_Opacity_Grain_Blend:
                    this.BitmapLayer.CapTip(cap,
                        this.Color, this.InkPresenter.Tip, this.InkPresenter.IsStroke,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize);
                    break;

                case InkType.Line:
                case InkType.Line_Grain:
                case InkType.Line_Opacity:
                case InkType.Line_Opacity_Grain:
                case InkType.Line_Blend:
                case InkType.Line_Grain_Blend:
                case InkType.Line_Opacity_Blend:
                case InkType.Line_Opacity_Grain_Blend:
                case InkType.Mosaic:
                    break;

                case InkType.Erase:
                case InkType.Erase_Opacity:
                    this.BitmapLayer.CapDrawShaderBrushEdgeHardness(cap,
                        this.BrushEdgeHardnessShaderCodeBytes, Vector4.One,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.Liquefy:
                    break;

                default:
                    break;
            }
        }

        private void PaintSegment(StrokeSegment segment)
        {
            switch (this.InkType)
            {
                case InkType.General:
                case InkType.General_Grain:
                case InkType.General_Opacity:
                case InkType.General_Opacity_Grain:
                case InkType.General_Blend:
                case InkType.General_Grain_Blend:
                case InkType.General_Opacity_Blend:
                case InkType.General_Opacity_Grain_Blend:
                case InkType.Blur:
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardness(segment,
                        this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.General_Grain_Mix:
                case InkType.General_Mix:
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardness(segment,
                        this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr,
                        this.InkPresenter.Mix, this.InkPresenter.Wet, this.InkPresenter.Persistence,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                    if (segment.IsNaN) return; // Shape without NaN
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardnessWithTexture(segment,
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr,
                        this.InkPresenter.ShapeSource, this.InkPresenter.Rotate,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.ShapeGeneral_Mix:
                case InkType.ShapeGeneral_Grain_Mix:
                    if (segment.IsNaN) return; // Shape without NaN
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardnessWithTexture(segment,
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr,
                        this.InkPresenter.ShapeSource, this.InkPresenter.Rotate,
                        this.InkPresenter.Mix, this.InkPresenter.Wet, this.InkPresenter.Persistence,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.Tip:
                case InkType.Tip_Grain:
                case InkType.Tip_Opacity:
                case InkType.Tip_Opacity_Grain:
                case InkType.Tip_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Tip_Opacity_Grain_Blend:
                    this.BitmapLayer.SegmentTip(segment,
                        this.Color, this.InkPresenter.Tip, this.InkPresenter.IsStroke,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize);
                    break;

                case InkType.Line:
                case InkType.Line_Grain:
                case InkType.Line_Opacity:
                case InkType.Line_Opacity_Grain:
                case InkType.Line_Blend:
                case InkType.Line_Grain_Blend:
                case InkType.Line_Opacity_Blend:
                case InkType.Line_Opacity_Grain_Blend:
                case InkType.Mosaic:
                    this.BitmapLayer.DrawLine(segment, this.Color, (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize);
                    break;

                case InkType.Erase:
                case InkType.Erase_Opacity:
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardness(segment,
                        this.BrushEdgeHardnessShaderCodeBytes, Vector4.One,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        (int)this.InkPresenter.SizePressure, this.InkPresenter.MinSize,
                        (int)this.InkPresenter.FlowPressure, this.InkPresenter.MinFlow);
                    break;

                case InkType.Liquefy:
                    if (segment.IsNaN) return; // Liquefy without NaN
                    this.BitmapLayer.Shade(new PixelShaderEffect(this.LiquefactionShaderCodeBytes)
                    {
                        Source1BorderMode = EffectBorderMode.Hard,
                        Source1 = this.BitmapLayer[BitmapType.Source],
                        Properties =
                        {
                            ["radius"] = this.BitmapLayer.ConvertValueToOne(this.InkPresenter.Size),
                            ["position"] = this.BitmapLayer .ConvertValueToOne(segment.StartingPosition),
                            ["targetPosition"] = this.BitmapLayer.ConvertValueToOne(segment.Position),
                            ["pressure"] = segment.Pressure,
                        }
                    }, segment.Bounds);
                    break;

                default:
                    break;
            }
        }

    }
}