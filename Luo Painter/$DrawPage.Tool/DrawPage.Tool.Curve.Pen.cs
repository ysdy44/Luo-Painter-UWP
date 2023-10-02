using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        bool PenIsSmooth => this.PenComboBox.SelectedIndex is 0;

        private void ConstructPen()
        {
            this.PenCloseButton.Click += (s, e) =>
            {
                if (this.LayerSelectedItem is null)
                {
                    this.Tip(TipType.NoLayer);
                    return;
                }

                if (this.LayerSelectedItem is CurveLayer curveLayer)
                {
                    AnchorCollection anchors = this.CurveLayer.SelectedItem;
                    if (anchors is null) return;

                    if (anchors.IsClosed) return;
                    anchors.IsClosed = true;

                    anchors.Segment(this.CanvasControl);
                    anchors.Invalidate();
                    curveLayer.Invalidate();

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                }
                else
                {
                    this.Tip(TipType.NotCurveLayer);
                }
            };
        }

        private void Pen_Start()
        {
            if (this.LayerSelectedItem is null)
            {
                this.Tip(TipType.NoLayer);
                return;
            }

            this.CurveLayer = this.LayerSelectedItem as CurveLayer;
            if (this.CurveLayer is null)
            {
                this.Tip(TipType.NotCurveLayer);
                return;
            }

            AnchorCollection anchors = this.CurveLayer.SelectedItem;
            if (anchors is null is false && anchors.IsClosed is false)
            {
                anchors.Add(new Anchor
                {
                    Point = anchors.ClosePoint,
                    LeftControlPoint = anchors.ClosePoint,
                    RightControlPoint = anchors.ClosePoint,
                    IsSmooth = this.PenIsSmooth
                });

                anchors.ClosePoint = this.StartingPosition;
                anchors.CloseIsSmooth = this.PenIsSmooth;
                anchors.Segment(this.CanvasControl, false);

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                return;
            }

            AnchorCollection add = new AnchorCollection(this.CanvasControl, this.Transformer.Width, this.Transformer.Height)
            {
                new Anchor
                {
                    Point = this.Position,
                    LeftControlPoint = this.Position,
                    RightControlPoint = this.Position,
                    IsSmooth = this.PenIsSmooth
                }
            };

            add.Color = this.Color;
            add.StrokeWidth = this.InkPresenter.Size;

            add.ClosePoint = this.Position;
            add.CloseIsSmooth = this.PenIsSmooth;
            add.Segment(this.CanvasControl, false);

            int count = this.CurveLayer.Anchorss.Count;
            this.CurveLayer.Anchorss.Add(add);
            this.CurveLayer.Index = count;

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }
        private void Pen_Delta()
        {
            if (this.CurveLayer is null) return;

            AnchorCollection anchors = this.CurveLayer.SelectedItem;
            if (anchors is null) return;

            anchors.ClosePoint = this.Position;
            anchors.CloseIsSmooth = this.PenIsSmooth;
            anchors.Segment(this.CanvasControl, false);
            anchors.Invalidate();
            this.CurveLayer.Invalidate();

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }
        private void Pen_Complete()
        {
        }

    }
}