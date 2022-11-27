using FanKit.Transformers;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        public void ConstructTransform()
        {
            this.TXButton.Click += (s, e) => this.NumberShowAt(this.TXButton, NumberPickerMode.Case0);
            this.TYButton.Click += (s, e) => this.NumberShowAt(this.TYButton, NumberPickerMode.Case1);

            this.TWButton.Click += (s, e) => this.NumberShowAt(this.TWButton, NumberPickerMode.Case2);
            this.THButton.Click += (s, e) => this.NumberShowAt(this.THButton, NumberPickerMode.Case3);

            this.TSButton.Click += (s, e) => this.NumberShowAt(this.TSButton, NumberPickerMode.Case4);
            this.TRButton.Click += (s, e) => this.NumberShowAt(this.TRButton, NumberPickerMode.Case5);
        }

        private void SetTransform(PixelBounds bounds) => this.ResetTransform(bounds);
        private void ResetTransform(PixelBounds bounds)
        {
            this.Bounds = bounds.ToBorder();

            this.BoundsTransformer = this.Bounds.ToTransformer();
            this.BoundsMatrix = Matrix3x2.Identity;

            this.SetTransformer(this.BoundsTransformer);
        }

        private void SetTransform(NumberPickerMode mode, int e)
        {
            switch (mode)
            {
                case NumberPickerMode.Case0:
                    this.BoundsTransformer += this.BoundsTransformer.TransformX(e, IndicatorMode.LeftTop);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TXButton.Number = e;
                    break;
                case NumberPickerMode.Case1:
                    this.BoundsTransformer += this.BoundsTransformer.TransformY(e, IndicatorMode.LeftTop);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TYButton.Number = e;
                    break;

                case NumberPickerMode.Case2:
                    this.BoundsTransformer *= this.BoundsTransformer.TransformWidth(e, IndicatorMode.LeftTop, this.IsRatio);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TWButton.Number = e;
                    break;
                case NumberPickerMode.Case3:
                    this.BoundsTransformer *= this.BoundsTransformer.TransformHeight(e, IndicatorMode.LeftTop, this.IsRatio);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.THButton.Number = e;
                    break;

                case NumberPickerMode.Case4:
                    this.BoundsTransformer *= this.BoundsTransformer.TransformSkew(e, IndicatorMode.Center);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TSButton.Number = e;
                    break;
                case NumberPickerMode.Case5:
                    this.BoundsTransformer *= this.BoundsTransformer.TransformRotate(e, IndicatorMode.Center);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.TRButton.Number = e;
                    break;

                default:
                    break;
            }
        }

        private void Transform_Start()
        {
            this.StartingMove = this.Move;
            this.BoundsMode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.BoundsTransformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
            this.IsBoundsMove = this.BoundsMode is TransformerMode.None && this.BoundsTransformer.FillContainsPoint(this.StartingPosition);
            this.StartingBoundsTransformer = this.BoundsTransformer;
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Transform_Delta()
        {
            if (this.IsBoundsMove)
            {
                this.BoundsTransformer = this.StartingBoundsTransformer + (this.Position - this.StartingPosition);
                this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            }
            else if (this.BoundsMode == default)
            {
            }
            else
            {
                this.BoundsTransformer = FanKit.Transformers.Transformer.Controller(this.BoundsMode, this.StartingPosition, this.Position, this.StartingBoundsTransformer, this.IsRatio, this.IsCenter);
                this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            }
        }

        private void Transform_Complete()
        {
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.SetTransformer(this.BoundsTransformer);
        }

        private void SetTransformer(Transformer transformer)
        {
            // X Y
            Vector2 vector = transformer.GetIndicatorVector(default);
            this.TXButton.Number = (int)vector.X;
            this.TYButton.Number = (int)vector.Y;

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
            this.TWButton.Number = (int)width;
            this.THButton.Number = (int)height;

            // Radians
            float angle = FanKit.Transformers.Transformer.GetRadians(horizontal);
            this.TRButton.Number = (int)angle;

            // Skew
            this.TSButton.Number = (int)FanKit.Transformers.Transformer.GetSkew(vertical, angle);
        }

    }
}