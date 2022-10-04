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
                    this.Tip("No Layer", "Create a new Layer?");
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
                    this.Tip("Not Curve Layer", "Can only operate on Curve Layer.");
                }
            };
        }

        private void Pen_Start(Vector2 position)
        {
            if (this.LayerSelectedItem is null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            this.CurveLayer = this.LayerSelectedItem as CurveLayer;
            if (this.CurveLayer is null)
            {
                this.Tip("Not Curve Layer", "Can only operate on Curve Layer.");
                return;
            }

            {
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

                    anchors.ClosePoint = position;
                    anchors.CloseIsSmooth = this.AppBar.PenIsSmooth;
                    anchors.Segment(this.CanvasControl, false);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    return;
                }
            }

            {
                AnchorCollection anchors = new AnchorCollection(this.CanvasControl, this.Transformer.Width, this.Transformer.Height)
                {
                    new Anchor
                    {
                                    Point = this.Position,
                                    LeftControlPoint = this.Position,
                                    RightControlPoint = this.Position,
                                    IsSmooth = this.AppBar.PenIsSmooth
                    }
                };

                //anchors.Color = this.ColorPicker.Color;
                //if (SizeListView.SelectedItem is BrushSize item)
                //{
                //    anchors.StrokeWidth = (float)item.Size;
                //}

                anchors.ClosePoint = this.Position;
                anchors.CloseIsSmooth = this.AppBar.PenIsSmooth;
                anchors.Segment(this.CanvasControl, false);

                int count = this.CurveLayer.Anchorss.Count;
                this.CurveLayer.Anchorss.Add(anchors);
                this.CurveLayer.Index = count;
            }
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }
        private void Pen_Delta(Vector2 position)
        {
            if (this.CurveLayer is null) return;

            AnchorCollection anchors = this.CurveLayer.SelectedItem;
            if (anchors is null) return;

            anchors.ClosePoint = position;
            anchors.CloseIsSmooth = this.AppBar.PenIsSmooth;
            anchors.Segment(this.CanvasControl, false);
            anchors.Invalidate();
            this.CurveLayer.Invalidate();

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }
        private void Pen_Complete(Vector2 position)
        {
        }

    }
}