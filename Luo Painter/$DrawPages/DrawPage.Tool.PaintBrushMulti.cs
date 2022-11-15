using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        int MultiMode => this.MultiComboBox.SelectedIndex;
        int MultiMirrorMode => this.MultiMirrorComboBox.SelectedIndex;
        int MultiCount => (int)this.MultiSlider.Value;

        bool IsMultiSymmetry;
        readonly IList<Matrix3x2> MultiMatrixs = new List<Matrix3x2>();

        private async void PaintBrushMulti_Start()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.Tasks.State != default) return;

            if (this.LayerSelectedItem is null)
            {
                this.Tip(TipType.NoLayer);
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip(TipType.NotBitmapLayer);
                return;
            }

            //@Paint
            StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);
            switch (this.MultiMode)
            {
                case 0:
                    switch (this.MultiMirrorMode)
                    {
                        case 0:
                            this.PaintCapAsync(cap);
                            this.PaintCapAsync(new StrokeCap(cap, this.BitmapLayer.Center, Orientation.Horizontal));
                            break;
                        case 1:
                            this.PaintCapAsync(cap);
                            this.PaintCapAsync(new StrokeCap(cap, this.BitmapLayer.Center, Orientation.Vertical));
                            break;
                        case 2:
                            this.PaintCapAsync(cap);
                            this.PaintCapAsync(new StrokeCap(cap, this.BitmapLayer.Center, Orientation.Horizontal));
                            this.PaintCapAsync(new StrokeCap(cap, this.BitmapLayer.Center, Orientation.Vertical));
                            this.PaintCapAsync(new StrokeCap(cap, this.BitmapLayer.Center));
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    {
                        int count = this.MultiCount;

                        this.IsMultiSymmetry = (count <= 2);
                        if (this.IsMultiSymmetry) break;

                        this.MultiMatrixs.Clear();
                        for (int i = 1; i < count; i++)
                        {
                            this.MultiMatrixs.Add(Matrix3x2.CreateRotation(FanKit.Math.PiTwice / count * i, this.BitmapLayer.Center));
                        }

                        this.PaintCapAsync(cap);
                        foreach (Matrix3x2 item in this.MultiMatrixs)
                        {
                            this.PaintCapAsync(new StrokeCap(cap, item));
                        }
                    }
                    break;
                case 2:
                    {
                        int count = this.MultiCount;

                        this.MultiMatrixs.Clear();
                        for (int i = 0; i < count; i++)
                        {
                            this.MultiMatrixs.Add(Matrix3x2.CreateRotation(FanKit.Math.PiTwice / count * i, this.BitmapLayer.Center));
                        }

                        foreach (Matrix3x2 item in this.MultiMatrixs)
                        {
                            StrokeCap cap2 = new StrokeCap(cap, item);
                            this.PaintCapAsync(cap2);
                            this.PaintCapAsync(new StrokeCap(cap2, this.BitmapLayer.Center, Orientation.Horizontal));
                        }
                    }
                    break;
                default:
                    break;
            }

            //@Paint
            this.Tasks.State = PaintTaskState.Painting;
            await Task.Run(this.PaintSegmentAsync);

            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void PaintBrushMulti_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            //@Paint
            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            if (segment.InRadius) return;
            this.Tasks.Add(segment);

            switch (this.MultiMode)
            {
                case 0:
                    switch (this.MultiMirrorMode)
                    {
                        case 0:
                            this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center, Orientation.Horizontal));
                            break;
                        case 1:
                            this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center, Orientation.Vertical));
                            break;
                        case 2:
                            this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center, Orientation.Horizontal));
                            this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center, Orientation.Vertical));
                            this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center));
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    if (this.IsMultiSymmetry)
                        this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center));
                    else
                        foreach (Matrix3x2 item in this.MultiMatrixs)
                        {
                            this.Tasks.Add(new StrokeSegment(segment, item));
                        }
                    break;
                case 2:
                    foreach (Matrix3x2 item in this.MultiMatrixs)
                    {
                        StrokeSegment segment2 = new StrokeSegment(segment, item);
                        this.Tasks.Add(segment2);
                        this.Tasks.Add(new StrokeSegment(segment2, this.BitmapLayer.Center, Orientation.Horizontal));
                    }
                    break;
                default:
                    break;
            }

            this.StartingPosition = this.Position;
            this.StartingPoint = this.Point;
            this.StartingPressure = this.Pressure;

            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void PaintBrushMulti_Complete()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasControl.Invalidate(); // Invalidate

            //@Paint
            this.Tasks.State = PaintTaskState.Painted;
        }

    }
}