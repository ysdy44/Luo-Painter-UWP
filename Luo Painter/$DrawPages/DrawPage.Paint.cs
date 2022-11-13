﻿using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using System;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private void PaintCapAsync(StrokeCap cap) => this.PaintCapAsync(cap, this.ToPoint(cap.StartingPosition));
        private void PaintCapAsync(StrokeCap cap, Vector2 startingPoint)
        {
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(cap.Bounds);
                this.PaintCap(cap);
            }

            Rect? region = RectExtensions.TryGetRect(startingPoint, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(cap.Size * this.Transformer.Scale));
            if (region.HasValue)
                this.CanvasVirtualControl.Invalidate(region.Value); // Invalidate
        }

        private async void PaintSegmentAsync()
        {
            while (true)
            {
                switch (this.Tasks.GetBehavior())
                {
                    case PaintTaskBehavior.WaitingWork:
                        continue;
                    case PaintTaskBehavior.Working:
                    case PaintTaskBehavior.WorkingBeforeDead:
                        StrokeSegment segment = this.Tasks.First();
                        this.Tasks.Remove(segment);

                        //@Task
                        lock (this.Locker)
                        {
                            this.BitmapLayer.Hit(segment.Bounds);
                            this.PaintSegment(segment);
                        }

                        Rect? region = RectExtensions.TryGetRect(this.ToPoint(segment.StartingPosition), this.ToPoint(segment.Position), this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(segment.Size * this.Transformer.Scale));
                        if (region.HasValue)
                        {
                            await this.CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                            {
                                this.CanvasVirtualControl.Invalidate(region.Value);
                            });
                        }
                        break;
                    default:
                        //@Paint
                        this.Tasks.State = PaintTaskState.Finished;

                        await this.CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        {
                            //@Task
                            lock (this.Locker)
                            {
                                if (this.InkType is InkType.Liquefy is false)
                                {
                                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                                    {
                                        ds.Clear(Colors.Transparent);
                                        this.InkPresenter.Preview(ds, this.InkType, this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]);
                                    }
                                }
                                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

                                // History
                                int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                                this.BitmapLayer.Flush();
                                this.BitmapLayer.RenderThumbnail();

                                this.BitmapLayer = null;
                                this.CanvasVirtualControl.Invalidate(); // Invalidate

                                this.RaiseHistoryCanExecuteChanged();
                            }
                        });
                        return;
                }
            }
        }


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
                        this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
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
                        this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                    this.BitmapLayer.CapDrawShaderBrushEdgeHardnessWithTexture();
                    break;

                case InkType.ShapeGeneral_Mix:
                case InkType.ShapeGeneral_Grain_Mix:
                case InkType.ShapeGeneral_Opacity_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Mix:
                case InkType.ShapeGeneral_Blend_Mix:
                case InkType.ShapeGeneral_Grain_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Blend_Mix:
                case InkType.ShapeGeneral_Opacity_Grain_Blend_Mix:
                    this.BitmapLayer.CapDrawShaderBrushEdgeHardnessWithTexture(cap, this.InkPresenter.Wet);
                    break;

                case InkType.Tip:
                case InkType.Tip_Grain:
                case InkType.Tip_Opacity:
                case InkType.Tip_Opacity_Grain:
                case InkType.Tip_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Tip_Opacity_Grain_Blend:
                    this.BitmapLayer.CapTip(cap, this.Color,
                        this.InkPresenter.Tip, this.InkPresenter.IsStroke);
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
                        this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
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
                        this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.General_Grain_Mix:
                case InkType.General_Mix:
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardness(segment,
                        this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr,
                        this.InkPresenter.Mix, this.InkPresenter.Wet, this.InkPresenter.Persistence,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Opacity_Grain:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Opacity_Grain_Blend:
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardnessWithTexture(segment,
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr, this.InkPresenter.ShapeSource,
                        this.InkPresenter.Rotate, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.ShapeGeneral_Mix:
                case InkType.ShapeGeneral_Grain_Mix:
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardnessWithTexture(segment,
                        this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr, this.InkPresenter.ShapeSource,
                        this.InkPresenter.Mix, this.InkPresenter.Wet, this.InkPresenter.Persistence,
                        this.InkPresenter.Rotate, (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
                    break;

                case InkType.Tip:
                case InkType.Tip_Grain:
                case InkType.Tip_Opacity:
                case InkType.Tip_Opacity_Grain:
                case InkType.Tip_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Tip_Opacity_Grain_Blend:
                    this.BitmapLayer.SegmentTip(segment, this.Color,
                        this.InkPresenter.Tip, this.InkPresenter.IsStroke);
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
                    this.BitmapLayer.DrawLine(segment, this.Color, this.InkPresenter.IgnoreSizePressure);
                    break;

                case InkType.Erase:
                case InkType.Erase_Opacity:
                    this.BitmapLayer.SegmentDrawShaderBrushEdgeHardness(segment, this.BrushEdgeHardnessShaderCodeBytes, Vector4.One,
                        (int)this.InkPresenter.Hardness, this.InkPresenter.Flow,
                        this.InkPresenter.IgnoreSizePressure, this.InkPresenter.IgnoreFlowPressure);
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