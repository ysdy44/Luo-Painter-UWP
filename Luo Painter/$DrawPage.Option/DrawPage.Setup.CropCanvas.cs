using FanKit.Transformers;
using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private Vector2 ToPositionWithoutRadian(Vector2 point) => Vector2.Transform(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point),
            Matrix3x2.CreateTranslation(-this.Transformer.Position) *
            Matrix3x2.CreateScale(1 / this.Transformer.Scale) *
            Matrix3x2.CreateTranslation(this.Transformer.Width / 2, this.Transformer.Height / 2));
        private Matrix3x2 GetMatrixWithoutRadian(float dpi) => dpi.ConvertPixelsToDips(
                Matrix3x2.CreateTranslation(-this.Transformer.Width / 2, -this.Transformer.Height / 2) *
                Matrix3x2.CreateScale(this.Transformer.Scale) *
                Matrix3x2.CreateTranslation(this.Transformer.Position));

        private Matrix3x2 GetMatrix(Vector2 leftTop) =>
            Matrix3x2.CreateRotation(this.Transformer.Radian,
                new Vector2(this.Transformer.Width / 2, this.Transformer.Height / 2)) *
            Matrix3x2.CreateTranslation(-leftTop);

        private void ConstructSetup()
        {
            this.CropCanvasSlider.ValueChanged += (s, e) =>
            {
                double radian = e.NewValue / 180 * System.Math.PI;
                this.Transformer.Radian = (float)radian;
                this.Transformer.ReloadMatrix();

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void SetCropCanvas(int w, int h)
        {
            this.CropTransform.Transformer = new Transformer(0, 0, w, h);

            this.CropCanvasSlider.Value = 0d;

            if (this.Transformer.Radian == 0f) return;
            this.Transformer.Radian = 0f;
            this.Transformer.ReloadMatrix();
        }

        private void DrawCropCanvas(CanvasControl sender, CanvasDrawingSession ds)
        {
            Transformer crop = FanKit.Transformers.Transformer.Multiplies(this.CropTransform.Transformer, this.GetMatrixWithoutRadian(sender.Dpi));

            ds.Clear(FanKit.Transformers.CanvasDrawingSessionExtensions.ShadowColor);
            ds.FillRectangle(crop.MinX, crop.MinY, crop.MaxX - crop.MinX, crop.MaxY - crop.MinY, Colors.Transparent);

            ds.DrawCrop(crop);
        }


        /// <summary> <see cref="Transform_Start()"/> </summary>
        private void CropCanvas_Start()
        {
            this.StartingPositionWithoutRadian = this.ToPositionWithoutRadian(this.Point);

            this.CropTransform.Mode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.CropTransform.Transformer, this.GetMatrixWithoutRadian(this.CanvasVirtualControl.Dpi), true);
            this.CropTransform.IsMove = this.CropTransform.Mode == TransformerMode.None;
            this.CropTransform.StartingTransformer = this.CropTransform.Transformer;

            this.CanvasControl.Invalidate(); // Invalidate
        }

        /// <summary> <see cref="Transform_Delta()"/> </summary>
        private void CropCanvas_Delta()
        {
            this.PositionWithoutRadian = this.ToPositionWithoutRadian(this.Point);

            if (this.CropTransform.IsMove)
                this.CropTransform.Transformer = this.CropTransform.StartingTransformer + (this.PositionWithoutRadian - this.StartingPositionWithoutRadian);
            else
                this.CropTransform.Transformer = FanKit.Transformers.Transformer.Controller(this.CropTransform.Mode, this.StartingPositionWithoutRadian, this.PositionWithoutRadian, this.CropTransform.StartingTransformer);

            this.CanvasControl.Invalidate(); // Invalidate
        }

        /// <summary> <see cref="Transform_Complete()"/> </summary>
        private void CropCanvas_Complete()
        {
            this.CanvasControl.Invalidate(); // Invalidate
        }


        private void CancelCropCanvas()
        {
            if (this.CropCanvasSlider.Value is 0d) return;

            this.Transformer.Radian = 0f;
            this.Transformer.ReloadMatrix();
            this.CropCanvasSlider.Value = 0;
        }
        private void PrimaryCropCanvas()
        {
            int width = this.Transformer.Width;
            int height = this.Transformer.Height;

            float w3 = this.CropTransform.Transformer.MaxX - this.CropTransform.Transformer.MinX;
            float h3 = this.CropTransform.Transformer.MaxY - this.CropTransform.Transformer.MinY;

            int w = (int)w3;
            int h = (int)h3;

            Vector2 offset = new Vector2
            {
                X = -this.CropTransform.Transformer.MinX,
                Y = -this.CropTransform.Transformer.MinY,
            };

            if (this.CropCanvasSlider.Value is 0d)
            {
                this.Transformer.Width = w;
                this.Transformer.Height = h;
                this.Transformer.Fit();

                this.CreateResources(w, h);
                this.CreateMarqueeResources(w, h);

                /// History
                int removes = this.History.Push(new CompositeHistory(
                    this.LayerManager.Setup(this, this.Nodes.Select(c => c.Crop(this.CanvasDevice, w, h, offset)).ToArray()),
                    new SetupHistory(width, height, w, h)
                ));
            }
            else
            {
                Matrix3x2 matrix = this.GetMatrix(this.CropTransform.Transformer.LeftTop);
                {
                    this.Transformer.Width = w;
                    this.Transformer.Height = h;
                    this.Transformer.Fit();

                    this.CreateResources(w, h);
                    this.CreateMarqueeResources(w, h);
                }

                /// History
                int removes = this.History.Push(new CompositeHistory(
                    this.LayerManager.Setup(this, this.Nodes.Select(c => c.Crop(this.CanvasDevice, w, h, matrix, CanvasImageInterpolation.NearestNeighbor)).ToArray()),
                    new SetupHistory(width, height, w, h)
                ));

                this.Transformer.Radian = 0f;
                this.Transformer.ReloadMatrix();
                this.CropCanvasSlider.Value = 0;
            }


            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

    }
}