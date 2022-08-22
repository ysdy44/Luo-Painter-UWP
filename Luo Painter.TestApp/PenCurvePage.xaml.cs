using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class PenCurvePage : Page
    {

        CanvasBitmap CanvasBitmap;
        NodeCollection Nodes;

        public PenCurvePage()
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
            this.CanvasControl.CreateResources += (sender, args) =>
            {
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap is null is false) args.DrawingSession.DrawImage(CanvasBitmap);

                if (this.Nodes is null) return;
                Matrix3x2 matrix = this.CanvasControl.Dpi.ConvertPixelsToDips();
                CanvasGeometry geometry = this.Nodes.CreateGeometry(sender).Transform(matrix);

                args.DrawingSession.DrawGeometry(geometry, Colors.DodgerBlue, 3);
                foreach (Node item in Nodes)
                {
                    args.DrawingSession.FillCircle(Vector2.Transform(item.Point, matrix), 2, Colors.Red);
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
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

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

                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.PenCurve(position);

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                if (this.Nodes is null) return;

                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.PenCurve(position);

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.CanvasControl.PointerMoved += (s, e) =>
            {
                if (this.Nodes is null) return;

                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(e.GetCurrentPoint(this.CanvasControl).Position.ToVector2());
                this.PenCurve(position);

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

                this.Nodes[i].IsChecked = true;
                this.Nodes[i].IsSmooth = false;

                this.Nodes[i].Point = position;
            }

            // Last
            {
                int i = this.Nodes.Count - 2;

                this.Nodes[i].IsChecked = true;
                this.Nodes[i].IsSmooth = true;

                this.Nodes[i].Point = position;
                this.Nodes[i].LeftControlPoint = position;
                this.Nodes[i].RightControlPoint = position;
            }

            if (this.Nodes.Count <= 3) return;

            // First
            {
                int i = 1;

                this.Nodes[i].IsChecked = true;
                this.Nodes[i].IsSmooth = true;

                Vector2 point = this.Nodes[i].Point;
                Vector2 left = (point + this.Nodes[i - 1].Point) / 2;
                Vector2 right = this.Nodes[i + 1].Point;

                Vector2 center = (left + right) / 2;
                Vector2 centerP = (center + point) / 2;

                float leftLength = (centerP - left).Length();
                float rightLength = (centerP - right).Length();
                float leftRightLength = leftLength + rightLength;

                Vector2 vector = right - left;
                this.Nodes[i].LeftControlPoint = point - leftLength / leftRightLength * vector;
                this.Nodes[i].RightControlPoint = point + rightLength / leftRightLength / 2 * vector;
            }

            if (this.Nodes.Count <= 4) return;

            // Nodes
            for (int i = 2; i < this.Nodes.Count - 2; i++)
            {
                this.Nodes[i].IsChecked = true;
                this.Nodes[i].IsSmooth = true;

                Vector2 point = this.Nodes[i].Point;
                Vector2 left = this.Nodes[i - 1].Point;
                Vector2 right = this.Nodes[i + 1].Point;

                Vector2 center = (left + right) / 2;
                Vector2 centerP = (center + point) / 2;

                float leftLength = (centerP - left).Length();
                float rightLength = (centerP - right).Length();
                float leftRightLength = leftLength + rightLength;

                Vector2 vector = right - left;
                this.Nodes[i].LeftControlPoint = point - leftLength / leftRightLength / 2 * vector;
                this.Nodes[i].RightControlPoint = point + rightLength / leftRightLength / 2 * vector;
            }
        }

    }
}