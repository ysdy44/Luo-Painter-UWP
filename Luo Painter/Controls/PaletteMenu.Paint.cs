using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaletteMenu : Expander, IInkParameter
    {

        private void Paint(StrokeSegment segment)
        {
            switch (this.InkType)
            {
                case InkType.General:
                case InkType.General_Grain:
                case InkType.General_Opacity:
                case InkType.General_Grain_Opacity:
                case InkType.General_Blend:
                case InkType.General_Grain_Blend:
                case InkType.General_Opacity_Blend:
                case InkType.General_Grain_Opacity_Blend:
                case InkType.Blur:
                    this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(segment, this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.General_Grain_Mix:
                case InkType.General_Mix:
                    this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(segment, this.BrushEdgeHardnessShaderCodeBytes, this.InkMixer.ColorHdr, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Grain_Opacity:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Grain_Opacity_Blend:
                    this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(segment, this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr, this.InkPresenter.ShapeSource, this.InkPresenter.Rotate, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.ShapeGeneral_Mix:
                case InkType.ShapeGeneral_Grain_Mix:
                    this.BitmapLayer.IsometricDrawShaderBrushEdgeHardnessWithTexture(segment, this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.InkMixer.ColorHdr, this.InkPresenter.ShapeSource, this.InkPresenter.Rotate, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.Tip:
                case InkType.Tip_Grain:
                case InkType.Tip_Opacity:
                case InkType.Tip_Grain_Opacity:
                case InkType.Tip_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Tip_Grain_Opacity_Blend:
                    this.BitmapLayer.IsometricTip(segment, this.Color, this.InkPresenter.Tip, this.InkPresenter.IsStroke);
                    break;

                case InkType.Tip_Mix:
                case InkType.Tip_Grain_Mix:
                    this.BitmapLayer.IsometricTip(segment, this.InkMixer.Color, this.InkPresenter.Tip, this.InkPresenter.IsStroke);
                    break;

                case InkType.Line:
                case InkType.Line_Grain:
                case InkType.Line_Opacity:
                case InkType.Line_Grain_Opacity:
                case InkType.Line_Blend:
                case InkType.Line_Grain_Blend:
                case InkType.Line_Opacity_Blend:
                case InkType.Line_Grain_Opacity_Blend:
                case InkType.Mosaic:
                    this.BitmapLayer.DrawLine(segment, this.Color, this.InkPresenter.IgnoreSizePressure);
                    break;

                case InkType.Line_Mix:
                case InkType.Line_Grain_Mix:
                    this.BitmapLayer.DrawLine(segment, this.InkMixer.Color, this.InkPresenter.IgnoreSizePressure);
                    break;

                case InkType.Erase:
                case InkType.Erase_Opacity:
                    this.BitmapLayer.IsometricDrawShaderBrushEdgeHardness(segment, this.BrushEdgeHardnessShaderCodeBytes, Vector4.One, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow, this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.Liquefy:
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