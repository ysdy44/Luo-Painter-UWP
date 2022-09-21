using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class PenCurve2Page : Page
    {

        //@Key
        private bool IsSmooth => this.SmoothButton.IsChecked is true;

        //@Converter
        private Symbol SymbolConverter(bool? value) => value is true ? Symbol.Pin : Symbol.UnPin;
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));
        private CanvasGeometry ToPoint(CanvasGeometry geometry) => geometry.Transform(this.CanvasControl.Dpi.ConvertPixelsToDips((this.Transformer.GetMatrix())));

        CanvasBitmap CanvasBitmap;
        NodeCollection Nodes;
        Transformer Border;

        public PenCurve2Page()
        {
            this.InitializeComponent();
            this.ConstructPenCurve();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructPenCurve()
        {
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

                CanvasGeometry geometry = this.ToPoint(this.Nodes.CreateGeometry(sender));
                args.DrawingSession.DrawGeometry(geometry, Colors.DodgerBlue, (3));

                foreach (Node item in Nodes)
                {
                    args.DrawingSession.FillCircle(this.ToPoint(item.Point), (2), Colors.Red);
                }
                args.DrawingSession.DrawNodeCollection(Nodes, matrix);
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
                        IsChecked = true,
                        IsSmooth = false,
                    });
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                if (this.Nodes is null) return;

                Vector2 position = this.ToPosition(point);
                this.PenCurve(position);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                if (this.Nodes is null) return;

                Vector2 position = this.ToPosition(point);
                this.PenCurve(position);

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.CanvasControl.PointerMoved += (s, e) =>
            {
                if (this.Nodes is null) return;

                Vector2 position = this.ToPosition(e.GetCurrentPoint(this.CanvasControl).Position.ToVector2());
                this.PenCurve(position);

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

        /// <summary>
        /// Curve all Points (Begin + First + Nodes + Last + End).
        /// </summary>
        /// <param name="position"> The control position. </param>
        private void PenCurve(Vector2 position)
        {
            if (this.Nodes.Count <= 2) return;

            // Begin
            //{
            //    int i = 0;
            //
            //    this.Nodes[i].IsChecked = true;
            //    this.Nodes[i].IsSmooth = false;
            //}

            // End
            {
                int i = this.Nodes.Count - 1;

                this.Nodes[i].IsSmooth = false;

                this.Nodes[i].Point = position;
            }

            // Last
            {
                int i = this.Nodes.Count - 2;

                this.Nodes[i].IsSmooth = this.IsSmooth;

                this.Nodes[i].Point = position;
                this.Nodes[i].LeftControlPoint = position;
                this.Nodes[i].RightControlPoint = position;
            }

            if (this.Nodes.Count <= 3) return;

            // First
            if (this.Nodes[1].IsSmooth)
            {
                Vector2 point = this.Nodes[1].Point;
                Vector2 vector = this.Nodes[2].LeftControlPoint - (point + this.Nodes[0].RightControlPoint) / 2;

                float left = (point - (point + this.Nodes[0].Point) / 2).LengthSquared();
                float right = (point - this.Nodes[2].Point).LengthSquared();
                float length = left + right;

                this.Nodes[1].LeftControlPoint = point - left / length * vector;
                this.Nodes[1].RightControlPoint = point + right / length / 2 * vector;
            }

            if (this.Nodes.Count <= 4) return;

            // Nodes
            for (int i = 2; i < this.Nodes.Count - 2; i++)
            {
                if (this.Nodes[i].IsSmooth)
                {
                    Vector2 point = this.Nodes[i].Point;
                    Vector2 vector = this.Nodes[i + 1].LeftControlPoint - this.Nodes[i - 1].RightControlPoint;

                    float left = (point - this.Nodes[i - 1].Point).LengthSquared();
                    float right = (point - this.Nodes[i + 1].Point).LengthSquared();
                    float length = left + right;

                    this.Nodes[i].LeftControlPoint = point - left / length / 2 * vector;
                    this.Nodes[i].RightControlPoint = point + right / length / 2 * vector;
                }
            }
        }

    }
}