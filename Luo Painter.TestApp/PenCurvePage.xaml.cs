using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
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
    internal enum PenCurveTool
    {
        Curve,
        Line,
        RectChoose,
        Move,
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
                    default: return default;
                }
            }
        }

        //@Converter
        private Symbol SymbolConverter(bool? value) => value is true ? Symbol.Pin : Symbol.UnPin;
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));
        private CanvasGeometry ToPoint(CanvasGeometry geometry) => geometry.Transform(this.CanvasControl.Dpi.ConvertPixelsToDips((this.Transformer.GetMatrix())));

        CanvasBitmap CanvasBitmap;
        NodeCollection Nodes;
        CanvasGeometry Geometry;
        Transformer Border;

        TransformerRect TransformerRect;
        Vector2 StartingPosition;

        public PenCurvePage()
        {
            this.InitializeComponent();
            this.ConstructPenCurve();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructPenCurve()
        {
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
                this.Nodes = null;
                this.CanvasControl.Invalidate(); // Invalidate
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

                if (this.Nodes is null) return;

                args.DrawingSession.DrawGeometry(ToPoint(this.Geometry), Colors.DodgerBlue, 1);
                foreach (Node item in this.Nodes)
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
                Vector2 position = this.ToPosition(point);
                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                        if (this.Nodes is null)
                        {
                            this.Nodes = new NodeCollection(position, position);
                        }
                        else
                        {
                            this.Nodes.PenAdd(new Node
                            {
                                Point = position,
                                LeftControlPoint = position,
                                RightControlPoint = position,
                                IsSmooth = false,
                            });
                        }
                        this.Geometry = this.Nodes.CreateGeometry(CanvasControl);
                        break;
                    case PenCurveTool.RectChoose:
                        this.TransformerRect = new TransformerRect(position, position);
                        this.Nodes.BoxChoose(this.TransformerRect);
                        break;
                    case PenCurveTool.Move:
                        if (this.Nodes is null) break;

                        this.Nodes.Index = -1;
                        foreach (Node item in this.Nodes)
                        {
                            switch (item.Type)
                            {
                                case NodeType.BeginFigure:
                                case NodeType.Node:
                                    item.CacheTransform();
                                    if (FanKit.Math.InNodeRadius(position, item.Point))
                                    {
                                        this.Nodes.Index = this.Nodes.IndexOf(item);
                                    }
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }

                this.StartingPosition = position;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                if (this.Nodes is null) return;

                Vector2 position = this.ToPosition(point);
                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                        this.PenCurve(position, this.Mode is PenCurveTool.Curve);
                        this.Geometry?.Dispose();
                        this.Geometry = this.Nodes.CreateGeometry(this.CanvasControl);
                        break;
                    case PenCurveTool.RectChoose:
                        this.TransformerRect = new TransformerRect(this.StartingPosition, position);
                        this.Nodes.BoxChoose(this.TransformerRect);
                        break;
                    case PenCurveTool.Move:
                        if (this.Nodes.Index is -1)
                        {
                            foreach (Node item in this.Nodes)
                            {
                                switch (item.Type)
                                {
                                    case NodeType.BeginFigure:
                                    case NodeType.Node:
                                        if (item.IsChecked)
                                        {
                                            item.TransformAdd(position - this.StartingPosition);
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            Node item = this.Nodes.SelectedItem;
                            item.TransformAdd(position - this.StartingPosition);
                        }

                        this.Geometry?.Dispose();
                        this.Geometry = this.Nodes.CreateGeometry(this.CanvasControl);
                        break;
                    default:
                        break;
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                if (this.Nodes is null) return;

                Vector2 position = this.ToPosition(point);
                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                        this.PenCurve(position, this.Mode is PenCurveTool.Curve);
                        this.Geometry?.Dispose();
                        this.Geometry = this.Nodes.CreateGeometry(this.CanvasControl);
                        break;
                    case PenCurveTool.RectChoose:
                        this.TransformerRect = default;
                        break;
                    case PenCurveTool.Move:
                        break;
                    default:
                        break;
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.CanvasControl.PointerMoved += (s, e) =>
            {
                if (this.Nodes is null) return;

                PointerPoint pp = e.GetCurrentPoint(this.CanvasControl);
                switch (pp.PointerDevice.PointerDeviceType)
                {
                    case PointerDeviceType.Touch:
                        return;
                }

                switch (this.Mode)
                {
                    case PenCurveTool.Curve:
                    case PenCurveTool.Line:
                        Vector2 position = this.ToPosition(pp.Position.ToVector2());
                        this.PenCurve(position, this.Mode is PenCurveTool.Curve);
                        this.Geometry?.Dispose();
                        this.Geometry = this.Nodes.CreateGeometry(CanvasControl);
                        this.CanvasControl.Invalidate(); // Invalidate
                        break;
                    default:
                        break;
                }
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

        /// <summary>
        /// Curve all Points (Begin + First + Nodes + Last + End).
        /// </summary>
        /// <param name="position"> The control position. </param>
        private void PenCurve(Vector2 position, bool isSmooth, bool isPos = true)
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