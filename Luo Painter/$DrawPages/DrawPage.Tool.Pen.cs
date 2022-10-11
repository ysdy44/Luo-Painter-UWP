using FanKit.Transformers;
using Luo_Painter.Options;
using Luo_Painter.Layers.Models;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Luo_Painter.Layers;
using Luo_Painter.Brushes;
using System.Drawing;
using Luo_Painter.Blends;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private void ConstructPen()
        {
            this.AppBar.PenCloseButtonClick += (s, e) =>
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
                    IsSmooth = this.AppBar.PenIsSmooth
                });

                anchors.ClosePoint = this.StartingPosition;
                anchors.CloseIsSmooth = this.AppBar.PenIsSmooth;
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
                    IsSmooth = this.AppBar.PenIsSmooth
                }
            };

            add.Color = this.ColorMenu.Color;
            add.StrokeWidth = this.InkPresenter.Size;

            add.ClosePoint = this.Position;
            add.CloseIsSmooth = this.AppBar.PenIsSmooth;
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
            anchors.CloseIsSmooth = this.AppBar.PenIsSmooth;
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