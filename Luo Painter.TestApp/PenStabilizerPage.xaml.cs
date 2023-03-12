using FanKit.Transformers;
using Luo_Painter.Layers;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class PenStabilizerPage : Page
    {

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        CurveLayer CurveLayer;
        Transformer Border;
        Vector2 Position;
        Vector2 LastPoint;

        int Stabilizer = 10 * 10 * 10 * 10;
        private readonly IList<int> Stabilizers = new List<int>
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
        };

        public PenStabilizerPage()
        {
            this.InitializeComponent();
            this.ConstructPenStabilizer();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructPenStabilizer()
        {
            this.ListBox.SelectionChanged += (s, e) =>
            {
                int index = this.ListBox.SelectedIndex;
                int stabilizer = this.Stabilizers[index];
                this.Stabilizer = stabilizer * stabilizer * stabilizer * stabilizer;
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                Vector2 size = this.CanvasControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Transformer.Fit();
                this.CurveLayer = new CurveLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height);
                this.Border = new Transformer(this.Transformer.Width, this.Transformer.Height, Vector2.Zero);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.CurveLayer.Source,
                    TransformMatrix = this.Transformer.GetMatrix(),
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                });


                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                args.DrawingSession.DrawBound(this.Border, matrix);

                foreach (AnchorCollection anchors in this.CurveLayer.Anchorss)
                {
                    args.DrawingSession.DrawAnchorCollection(anchors, matrix);
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.LastPoint = this.Position = this.ToPosition(point);

                AnchorCollection anchors = new AnchorCollection(this.CanvasControl, this.Transformer.Width, this.Transformer.Height)
                {
                    new Anchor
                    {
                        Point = this.LastPoint,
                        LeftControlPoint = this.LastPoint,
                        RightControlPoint = this.LastPoint,
                        IsSmooth = true,
                        IsChecked = true,
                    }
                };

                anchors.ClosePoint = this.Position;
                anchors.CloseIsSmooth = true;
                anchors.Segment(this.CanvasControl, false);

                anchors.Color = Colors.DodgerBlue;
                anchors.Invalidate();

                int count = this.CurveLayer.Anchorss.Count;
                this.CurveLayer.Anchorss.Add(anchors);
                this.CurveLayer.Index = count;

                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                Vector2 position = this.ToPosition(point);

                AnchorCollection anchors = this.CurveLayer.Anchorss.Last();
                if (Vector2.DistanceSquared(this.LastPoint, position) > this.Stabilizer)
                {
                    this.LastPoint = position;
                    anchors.Add(new Anchor
                    {
                        Point = this.LastPoint,
                        LeftControlPoint = this.LastPoint,
                        RightControlPoint = this.LastPoint,
                        IsSmooth = true,
                        IsChecked = true,
                    });
                }

                anchors.ClosePoint = position;
                anchors.CloseIsSmooth = true;
                anchors.Segment(this.CanvasControl);
                anchors.Invalidate();

                this.Position = position;
                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);

                AnchorCollection anchors = this.CurveLayer.Anchorss.Last();
                anchors.Add(new Anchor
                {
                    Point = this.Position,
                    LeftControlPoint = this.Position,
                    RightControlPoint = this.Position,
                    IsSmooth = true,
                    IsChecked = true,
                });

                anchors.ClosePoint = this.Position;
                anchors.CloseIsSmooth = false;
                anchors.Segment(this.CanvasControl, false);
                anchors.Invalidate();

                this.CurveLayer.Invalidate();
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point, isHolding) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }
    }
}