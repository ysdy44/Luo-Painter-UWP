using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        readonly ObservableCollection<CurveLayer> ObservableCollection = new ObservableCollection<CurveLayer>();
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
            this.ListView.SelectionChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.AddAnchorsButton.Click += (s, e) =>
            {
                int count = this.ObservableCollection.Count;

                this.ObservableCollection.Add(new CurveLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height));
                this.ListView.SelectedIndex = count;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.RemoveAnchorsButton.Click += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    this.ObservableCollection.RemoveAt(index);
                    this.ListView.SelectedIndex = index - 1;

                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };

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
                this.ObservableCollection.Add(new CurveLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height));
                this.Border = new Transformer(this.Transformer.Width, this.Transformer.Height, Vector2.Zero);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                foreach (CurveLayer items in this.ObservableCollection)
                {
                    if (items is null) continue;
                    args.DrawingSession.DrawImage(new Transform2DEffect
                    {
                        Source = items[BitmapType.Source],
                        TransformMatrix = this.Transformer.GetMatrix(),
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    });
                }


                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                args.DrawingSession.DrawBound(this.Border, matrix);

                int index = this.ListView.SelectedIndex;
                if (index < 0) return;

                CurveLayer layer = this.ObservableCollection[index];
                if (layer is null) return;

                args.DrawingSession.DrawAnchorCollection(layer.Anchors, matrix);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.LastPoint = this.Position = this.ToPosition(point);

                CurveLayer layer = new CurveLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height);
                layer.Anchors.Add(new Anchor
                {
                    Point = this.LastPoint,
                    LeftControlPoint = this.LastPoint,
                    RightControlPoint = this.LastPoint,
                    IsSmooth = true,
                    IsChecked = true,
                });

                layer.Anchors.ClosePoint = this.Position;
                layer.Anchors.CloseIsSmooth = true;
                layer.Anchors.Segment(this.CanvasControl, false);

                layer.Anchors.Color = Colors.DodgerBlue;
                layer.Anchors.Invalidate();

                int count = this.ObservableCollection.Count;
                this.ObservableCollection.Add(layer);
                this.ListView.SelectedIndex = count;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.ToPosition(point);

                CurveLayer layer = this.ObservableCollection.Last();
                if (Vector2.DistanceSquared(this.LastPoint, position) > this.Stabilizer)
                {
                    this.LastPoint = position;
                    layer.Anchors.Add(new Anchor
                    {
                        Point = this.LastPoint,
                        LeftControlPoint = this.LastPoint,
                        RightControlPoint = this.LastPoint,
                        IsSmooth = true,
                        IsChecked = true,
                    });
                }

                layer.Anchors.ClosePoint = position;
                layer.Anchors.CloseIsSmooth = true;
                layer.Anchors.Segment(this.CanvasControl);
                layer.Anchors.Invalidate();

                this.Position = position;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                CurveLayer layer = this.ObservableCollection.Last();
                layer.Anchors.Add(new Anchor
                {
                    Point = this.Position,
                    LeftControlPoint = this.Position,
                    RightControlPoint = this.Position,
                    IsSmooth = true,
                    IsChecked = true,
                });

                layer.Anchors.ClosePoint = this.Position;
                layer.Anchors.CloseIsSmooth = false;
                layer.Anchors.Segment(this.CanvasControl, false);
                layer.Anchors.Invalidate();

                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point) =>
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