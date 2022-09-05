using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal class PenStrokes : List<Vector2>
    {
        public PenStrokes(CanvasGeometry geometry, float strokeWidth = 12) : base
        (
            from i
            in Enumerable.Range(0, (int)(geometry.ComputePathLength() / strokeWidth))
            select geometry.ComputePointOnPath(i * strokeWidth)
        )
        { }

        public int[] GetStrokes(IEnumerable<Node> nodes) => nodes.Select(this.GetStroke).ToArray();
        public int GetNode(int[] nodesStroke, Vector2 position)
        {
            int stroke = this.GetStroke(position);

            int strokeLeft = this.GetStrokeLeft(nodesStroke, stroke);
            if (strokeLeft is int.MinValue) return -1;

            int strokeRight = this.GetStrokeRight(nodesStroke, stroke);
            if (strokeRight is int.MaxValue) return -1;

            int index = System.Math.Max(strokeLeft, strokeRight);
            for (int i = 0; i < nodesStroke.Length; i++)
            {
                if (nodesStroke[i] == index)
                    return i;
            }

            return -1;
        }

        private int GetStroke(Node node) => this.GetStroke(node.Point);
        private int GetStroke(Vector2 position)
        {
            int index = -1;
            float distance = float.MaxValue;

            for (int i = 0; i < base.Count; i++)
            {
                float d = Vector2.Distance(position, base[i]);
                if (distance > d)
                {
                    distance = d;
                    index = i;
                }
            }
            return index;
        }

        private int GetStrokeLeft(IEnumerable<int> nodesStroke, int stroke)
        {
            int index = int.MinValue;

            foreach (int item in nodesStroke)
            {
                if (stroke > item)
                    if (index < item)
                        index = item;
            }
            return index;
        }
        private int GetStrokeRight(IEnumerable<int> nodesStroke, int stroke)
        {
            int index = int.MaxValue;

            foreach (int item in nodesStroke)
            {
                if (stroke < item)
                    if (index > item)
                        index = item;
            }
            return index;
        }

    }

    internal enum PenCurveTool
    {
        Curve,
        Line,
        RectChoose,
        Move,
        Hitter,
        Cutter,
    }

    public sealed partial class PenCurvePage : Page
    {

        //@Key
        private PenCurveTool Mode
        {
            get
            {
                switch (this.ListBox.SelectedIndex)
                {
                    case 0: return PenCurveTool.Curve;
                    case 1: return PenCurveTool.Line;
                    case 2: return PenCurveTool.RectChoose;
                    case 3: return PenCurveTool.Move;
                    case 4: return PenCurveTool.Hitter;
                    case 5: return PenCurveTool.Cutter;
                    default: return default;
                }
            }
        }

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));
        private CanvasGeometry ToPoint(CanvasGeometry geometry) => geometry.Transform(this.CanvasControl.Dpi.ConvertPixelsToDips((this.Transformer.GetMatrix())));

        readonly ObservableCollection<PenCurve> ObservableCollection = new ObservableCollection<PenCurve>
        {
            new PenCurve()
        };

        CanvasBitmap CanvasBitmap;
        Transformer Border;

        TransformerRect TransformerRect;
        Vector2 StartingPosition;
        Vector2 Position;

        PenHitter Hitter;
        PenCutter Cutter;

        public PenCurvePage()
        {
            this.InitializeComponent();
            this.ConstructPenCurve();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructPenCurve()
        {
            this.ListView.SelectionChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
            this.AddNodesButton.Click += (s, e) =>
            {
                this.ObservableCollection.Add(new PenCurve());
                this.ListView.SelectedIndex = this.ObservableCollection.Count - 1;
            };

            this.ListBox.ItemsSource = System.Enum.GetValues(typeof(PenCurveTool));
            this.ListBox.SelectedIndex = 0;

            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return;
                if (result is false) return;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.ClearButton.Click += (s, e) =>
            {
                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    this.ObservableCollection[index].Nodes = null;
                    this.CanvasControl.Invalidate(); // Invalidate
                }
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
                this.Border = new Transformer(this.Transformer.Width, this.Transformer.Height, Vector2.Zero);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                args.DrawingSession.DrawBound(this.Border, matrix);

                if (this.CanvasBitmap is null is false) args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.CanvasBitmap,
                    TransformMatrix = matrix
                });

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    PenCurve curve = this.ObservableCollection[index];

                    foreach (PenCurve item in this.ObservableCollection)
                    {
                        if (item is null) continue;
                        if (item.Nodes is null) continue;

                        Color color = curve == item ? Colors.DodgerBlue : Colors.Gray;
                        args.DrawingSession.DrawGeometry(this.ToPoint(item.Geometry), color, 1);
                    }

                    if (curve.Nodes is null) return;

                    foreach (Node item in curve.Nodes)
                    {
                        switch (item.Type)
                        {
                            case NodeType.BeginFigure:
                            case NodeType.Node:
                                Vector2 p = this.ToPoint(item.Point);
                                if (item.IsSmooth)
                                {
                                    if (item.IsChecked) args.DrawingSession.FillCircle(p, 4, Colors.White);
                                    args.DrawingSession.FillCircle(p, 3, Colors.DodgerBlue);
                                }
                                else
                                {
                                    if (item.IsChecked) args.DrawingSession.FillRectangle(p.X - 3, p.Y - 3, 6, 6, Colors.White);
                                    args.DrawingSession.FillRectangle(p.X - 2, p.Y - 2, 4, 4, Colors.DodgerBlue);
                                }
                                break;
                        }
                    }
                }

                switch (this.Mode)
                {
                    case PenCurveTool.RectChoose:
                        if (this.TransformerRect.Width == default) break;
                        if (this.TransformerRect.Height == default) break;
                        {
                            CanvasGeometry canvasGeometry = this.TransformerRect.ToRectangle(this.CanvasControl);
                            CanvasGeometry canvasGeometryTransform = canvasGeometry.Transform(matrix);
                            args.DrawingSession.DrawGeometryDodgerBlue(canvasGeometryTransform);
                        }
                        break;
                    case PenCurveTool.Hitter:
                        if (this.Hitter.IsContains)
                        {
                            Vector2 p = this.ToPoint(this.Hitter.Target);
                            args.DrawingSession.DrawLine(p, this.ToPoint(this.Position), this.Hitter.IsHit ? Colors.Orange : Colors.DodgerBlue, 2);

                            args.DrawingSession.FillCircle(p, 4, Colors.White);
                            args.DrawingSession.FillCircle(p, 3, Colors.DodgerBlue);
                        }
                        break;
                    case PenCurveTool.Cutter:
                        Vector2 sp = this.ToPoint(this.StartingPosition);
                        Vector2 pp = this.ToPoint(this.Position);
                        args.DrawingSession.DrawLine(sp, pp, Colors.Orange, 2);

                        if (this.Cutter.IsHit)
                        {
                            Vector2 p = this.ToPoint(this.Cutter.Target);
                            args.DrawingSession.FillCircle(p, 4, Colors.White);
                            args.DrawingSession.FillCircle(p, 3, Colors.Orange);
                        }
                        break;
                    default:
                        break;
                }

                //args.DrawingSession.DrawNodeCollection(this.Nodes, matrix);
                //
                //for (int i = 0; i < this.Nodes.Count; i++)
                //{
                //    args.DrawingSession.DrawText(i.ToString(), Vector2.Transform(this.Nodes[i].Point, matrix) - new Vector2(40, 40), Colors.Red);
                //}
                //
                //args.DrawingSession.DrawText(this.Nodes.Count.ToString(), Vector2.Zero, Colors.Red);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    PenCurve curve = this.ObservableCollection[index];

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            if (curve.Nodes is null)
                            {
                                curve.Nodes = new NodeCollection(this.Position, this.Position);
                            }
                            else
                            {
                                curve.Nodes.PenAdd(new Node
                                {
                                    Point = this.Position,
                                    LeftControlPoint = this.Position,
                                    RightControlPoint = this.Position,
                                    IsSmooth = false,
                                });
                            }
                            curve.Geometry = curve.Nodes.CreateGeometry(this.CanvasControl);
                            break;
                        case PenCurveTool.RectChoose:
                            this.TransformerRect = new TransformerRect(this.Position, this.Position);
                            curve.Nodes.BoxChoose(this.TransformerRect);
                            break;
                        case PenCurveTool.Move:
                            if (curve.Nodes is null) break;

                            curve.Nodes.Index = -1;
                            foreach (Node item in curve.Nodes)
                            {
                                switch (item.Type)
                                {
                                    case NodeType.BeginFigure:
                                    case NodeType.Node:
                                        item.CacheTransform();
                                        if (FanKit.Math.InNodeRadius(this.Position, item.Point))
                                        {
                                            curve.Nodes.Index = curve.Nodes.IndexOf(item);
                                        }
                                        break;
                                }
                            }
                            break;
                        case PenCurveTool.Hitter:
                            this.Hitter.Hit(curve.Geometry, this.Position, 32 / this.Transformer.Scale);
                            break;
                        case PenCurveTool.Cutter:
                            this.Cutter.Hit(curve.Geometry, this.StartingPosition, this.Position, 32 / this.Transformer.Scale);
                            break;
                        default:
                            break;
                    }
                }

                this.StartingPosition = this.Position;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    PenCurve curve = this.ObservableCollection[index];

                    if (curve.Nodes is null) return;

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            curve.Curve(this.Position, this.Mode is PenCurveTool.Curve);
                            curve.Geometry?.Dispose();
                            curve.Geometry = curve.Nodes.CreateGeometry(this.CanvasControl);
                            break;
                        case PenCurveTool.RectChoose:
                            this.TransformerRect = new TransformerRect(this.StartingPosition, this.Position);
                            curve.Nodes.BoxChoose(this.TransformerRect);
                            break;
                        case PenCurveTool.Move:
                            if (curve.Nodes.Index is -1)
                            {
                                foreach (Node item in curve.Nodes)
                                {
                                    switch (item.Type)
                                    {
                                        case NodeType.BeginFigure:
                                        case NodeType.Node:
                                            if (item.IsChecked)
                                            {
                                                item.TransformAdd(this.Position - this.StartingPosition);
                                            }
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                Node item = curve.Nodes.SelectedItem;
                                item.TransformAdd(this.Position - this.StartingPosition);
                            }

                            curve.Curve(default, default, default);

                            curve.Geometry?.Dispose();
                            curve.Geometry = curve.Nodes.CreateGeometry(this.CanvasControl);
                            break;
                        case PenCurveTool.Hitter:
                            this.Hitter.Hit(curve.Geometry, this.Position, 32 / this.Transformer.Scale);
                            break;
                        case PenCurveTool.Cutter:
                            this.Cutter.Hit(curve.Geometry, this.StartingPosition, this.Position, 32 / this.Transformer.Scale);
                            break;
                        default:
                            break;
                    }
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.Position = this.ToPosition(point);

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    PenCurve curve = this.ObservableCollection[index];

                    if (curve.Nodes is null) return;

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            curve.Curve(this.Position, this.Mode is PenCurveTool.Curve);
                            curve.Geometry?.Dispose();
                            curve.Geometry = curve.Nodes.CreateGeometry(this.CanvasControl);
                            break;
                        case PenCurveTool.RectChoose:
                            this.TransformerRect = default;
                            break;
                        case PenCurveTool.Move:
                            break;
                        case PenCurveTool.Hitter:
                        case PenCurveTool.Cutter:
                            Vector2 target = Vector2.Zero;

                            if (this.StartingPosition == this.Position)
                            {
                                this.Hitter.Hit(curve.Geometry, this.Position, 32 / this.Transformer.Scale);
                                if (this.Hitter.IsHit) target = this.Hitter.Target;
                                else break;
                            }
                            else
                            {
                                this.Cutter.Hit(curve.Geometry, this.StartingPosition, this.Position, 8 / this.Transformer.Scale);
                                if (this.Cutter.IsHit) target = this.Cutter.Target;
                                else break;
                            }

                            PenStrokes strokes = new PenStrokes(curve.Geometry);
                            int[] nodesStroke = strokes.GetStrokes(curve.Nodes);

                            int index2 = strokes.GetNode(nodesStroke, target);
                            if (index2 is -1) break;

                            curve.Nodes.Insert(index2, new Node
                            {
                                IsChecked = true,
                                IsSmooth = true,
                                Point = target,
                                LeftControlPoint = target,
                                RightControlPoint = target,
                            });

                            curve.Curve(default, default, default);

                            curve.Geometry?.Dispose();
                            curve.Geometry = curve.Nodes.CreateGeometry(this.CanvasControl);
                            break;
                        default:
                            break;
                    }
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.CanvasControl.PointerMoved += (s, e) =>
            {
                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                    case PenCurveTool.Hitter:
                        break;
                    default:
                        return;
                }

                PointerPoint pp = e.GetCurrentPoint(this.CanvasControl);
                switch (pp.PointerDevice.PointerDeviceType)
                {
                    case PointerDeviceType.Touch:
                        return;
                }

                this.Position = this.ToPosition(pp.Position.ToVector2());

                int index = this.ListView.SelectedIndex;
                if (index >= 0)
                {
                    PenCurve curve = this.ObservableCollection[index];

                    if (curve.Nodes is null) return;

                    switch (this.Mode)
                    {
                        case PenCurveTool.Curve:
                        case PenCurveTool.Line:
                            curve.Curve(this.Position, this.Mode is PenCurveTool.Curve);

                            curve.Geometry?.Dispose();
                            curve.Geometry = curve.Nodes.CreateGeometry(this.CanvasControl);
                            break;
                        case PenCurveTool.Hitter:
                            this.Hitter.Hit(curve.Geometry, this.Position, 32 / this.Transformer.Scale);
                            break;
                        case PenCurveTool.Cutter:
                            break;
                        default:
                            break;
                    }
                }

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

        public async Task<StorageFile> PickSingleImageFileAsync(PickerLocationId location)
        {
            // Picker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = location,
                FileTypeFilter =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".bmp"
                }
            };

            // File
            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
        }

        public async Task<bool?> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                    this.CanvasBitmap = bitmap;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public sealed class PenCurve
    {
        public NodeCollection Nodes { get; set; }
        public CanvasGeometry Geometry { get; set; }

        public override string ToString() => "Pen Curve";

        /// <summary>
        /// Curve all Points (Begin + First + Nodes + Last + End).
        /// </summary>
        /// <param name="position"> The control position. </param>
        public void Curve(Vector2 position, bool isSmooth, bool isPos = true)
        {
            if (this.Nodes.Count <= 2) return;

            // 0. Begin
            //{
            //    int i = 0;
            //
            //    this.Nodes[i].IsChecked = true;
            //    this.Nodes[i].IsSmooth = false;
            //}

            // 1. First
            if (this.Nodes.Count > 3)
            {
                if (this.Nodes[1].IsSmooth)
                {
                    Vector2 point = this.Nodes[1].Point;
                    Vector2 vector = this.Nodes[2].LeftControlPoint - (point + this.Nodes[0].RightControlPoint) / 2;

                    float left = (point - (point + this.Nodes[0].Point) / 2).Length();
                    float right = (point - this.Nodes[2].Point).Length();
                    float length = left + right;

                    this.Nodes[1].LeftControlPoint = point - left / length * vector;
                    this.Nodes[1].RightControlPoint = point + right / length / 2 * vector;
                }
            }

            // 2. Nodes
            if (this.Nodes.Count > 4)
            {
                for (int i = 2; i < this.Nodes.Count - 2; i++)
                {
                    if (this.Nodes[i].IsSmooth)
                    {
                        Vector2 point = this.Nodes[i].Point;
                        Vector2 vector = this.Nodes[i + 1].LeftControlPoint - this.Nodes[i - 1].RightControlPoint;

                        float left = (point - this.Nodes[i - 1].Point).Length();
                        float right = (point - this.Nodes[i + 1].Point).Length();
                        float length = left + right;

                        this.Nodes[i].LeftControlPoint = point - left / length / 2 * vector;
                        this.Nodes[i].RightControlPoint = point + right / length / 2 * vector;
                    }
                }
            }

            // 3. Last
            if (isPos)
            {
                int i = this.Nodes.Count - 2;

                this.Nodes[i].IsSmooth = isSmooth;

                this.Nodes[i].Point = position;
                this.Nodes[i].LeftControlPoint = position;
                this.Nodes[i].RightControlPoint = position;
            }

            // 4. End
            if (isPos)
            {
                int i = this.Nodes.Count - 1;

                this.Nodes[i].IsSmooth = false;

                this.Nodes[i].Point = position;
                this.Nodes[i].LeftControlPoint = position;
                this.Nodes[i].RightControlPoint = position;
            }
        }

    }
}