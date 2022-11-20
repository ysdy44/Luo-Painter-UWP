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

        int SymmetryCount => (int)this.SymmetrySlider.Value;
        SymmetryMode SymmetryMode
        {
            get
            {
                switch (this.SymmetryComboBox.SelectedIndex)
                {
                    case 0: return SymmetryMode.Horizontal;
                    case 1: return SymmetryMode.Vertical;
                    case 2: return SymmetryMode.Symmetry;
                    case 3: return SymmetryMode.Mirror;
                    default: return SymmetryMode.None;
                }
            }
        }

        private async void PaintBrushMulti_Start()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.Tasks.State != default) return;

            //@Paint
            int count = this.SymmetryCount;
            this.SymmetryType = this.Symmetryer.GetType(this.SymmetryMode, count);
            if (this.SymmetryMode == default) return;

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
            this.Symmetryer.Construct(count, this.BitmapLayer.Center);
            foreach (StrokeCap cap in this.Symmetryer.GetCaps(this.SymmetryType, new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size), this.BitmapLayer.Center))
            {
                this.PaintCapAsync(cap);
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
            if (this.SymmetryMode == default) return;
            if (this.BitmapLayer is null) return;

            //@Paint
            foreach (StrokeSegment segment in this.Symmetryer.GetSegments(this.SymmetryType, new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing), this.BitmapLayer.Center))
            {
                this.Tasks.Add(segment);
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
            if (this.SymmetryMode == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasControl.Invalidate(); // Invalidate

            //@Paint
            this.Tasks.State = PaintTaskState.Painted;
        }

    }
}