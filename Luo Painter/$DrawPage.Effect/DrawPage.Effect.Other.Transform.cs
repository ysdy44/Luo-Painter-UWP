using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.UI;
using System.Numerics;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public void ConstructTransform()
        {
            this.TXButton.Click += (s, e) => this.NumberShowAt(this.TXButton, NumberPickerMode.TransformX);
            this.TYButton.Click += (s, e) => this.NumberShowAt(this.TYButton, NumberPickerMode.TransformY);

            this.TWButton.Click += (s, e) => this.NumberShowAt(this.TWButton, NumberPickerMode.TransformWidth);
            this.THButton.Click += (s, e) => this.NumberShowAt(this.THButton, NumberPickerMode.TransformHeight);

            this.TSButton.Click += (s, e) => this.NumberShowAt(this.TSButton, NumberPickerMode.TransformSkew);
            this.TRButton.Click += (s, e) => this.NumberShowAt(this.TRButton, NumberPickerMode.TransformRotate);
        }

        private void ResetTransform(PixelBounds bounds)
        {
            this.Transform = new TransformMatrix
            {
                Matrix = Matrix3x2.Identity,
                X = bounds.Left,
                Y = bounds.Top,
                Width = bounds.Width(),
                Height = bounds.Height(),
                Transformer = bounds.ToBorder().ToTransformer()
            };

            this.SetTransformer(this.Transform.Transformer);
        }

        private void SetTransform(NumberPickerMode mode, float e)
        {
            switch (mode)
            {
                case NumberPickerMode.TransformX:
                    this.Transform.Transformer += this.Transform.Transformer.TransformX(e, IndicatorMode.LeftTop);
                    this.Transform.UpdateMatrix();

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TXButton.Value = e;
                    break;
                case NumberPickerMode.TransformY:
                    this.Transform.Transformer += this.Transform.Transformer.TransformY(e, IndicatorMode.LeftTop);
                    this.Transform.UpdateMatrix();

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TYButton.Value = e;
                    break;

                case NumberPickerMode.TransformWidth:
                    this.Transform.Transformer *= this.Transform.Transformer.TransformWidth(e, IndicatorMode.LeftTop, this.IsRatio);
                    this.Transform.UpdateMatrix();

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TWButton.Value = e;
                    break;
                case NumberPickerMode.TransformHeight:
                    this.Transform.Transformer *= this.Transform.Transformer.TransformHeight(e, IndicatorMode.LeftTop, this.IsRatio);
                    this.Transform.UpdateMatrix();

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.THButton.Value = e;
                    break;

                case NumberPickerMode.TransformSkew:
                    this.Transform.Transformer *= this.Transform.Transformer.TransformSkew(e, IndicatorMode.Center);
                    this.Transform.UpdateMatrix();

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TSButton.Value = e;
                    break;
                case NumberPickerMode.TransformRotate:
                    this.Transform.Transformer *= this.Transform.Transformer.TransformRotate(e, IndicatorMode.Center);
                    this.Transform.UpdateMatrix();

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TRButton.Value = e;
                    break;

                default:
                    break;
            }
        }


        private void Transform_Start()
        {
            this.Transform.Mode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.Transform.Transformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
            this.Transform.IsMove = this.Transform.Mode is TransformerMode.None && this.Transform.Transformer.FillContainsPoint(this.StartingPosition);
            this.Transform.StartingTransformer = this.Transform.Transformer;

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void Transform_Delta()
        {
            if (this.Transform.IsMove)
            {
                this.Transform.Transformer = this.Transform.StartingTransformer + (this.Position - this.StartingPosition);
                this.Transform.UpdateMatrix();

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            }
            else if (this.Transform.Mode != default)
            {
                this.Transform.Transformer = FanKit.Transformers.Transformer.Controller(this.Transform.Mode, this.StartingPosition, this.Position, this.Transform.StartingTransformer, this.IsRatio, this.IsCenter);
                this.Transform.UpdateMatrix();

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            }
        }

        private void Transform_Complete()
        {
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.SetTransformer(this.Transform.Transformer);
        }

        private void SetTransformer(Transformer transformer)
        {
            // X Y
            Vector2 vector = transformer.GetIndicatorVector(default);
            this.TXButton.Value = vector.X;
            this.TYButton.Value = vector.Y;

            // Value
            Vector2 horizontal = transformer.Horizontal;
            Vector2 vertical = transformer.Vertical;

            //@Release: case Debug
            // Width Height
            //float width = horizontal.Length();
            //float height = vertical.Length();

            //@Release: case Release
            double width = System.Math.Sqrt(horizontal.X * horizontal.X + horizontal.Y * horizontal.Y);
            double height = System.Math.Sqrt(vertical.X * vertical.X + vertical.Y * vertical.Y);
            this.TWButton.Value = width;
            this.THButton.Value = height;

            // Radians
            float angle = FanKit.Transformers.Transformer.GetRadians(horizontal);
            this.TRButton.Value = angle;

            // Skew
            this.TSButton.Value = FanKit.Transformers.Transformer.GetSkew(vertical, angle);
        }

    }
}