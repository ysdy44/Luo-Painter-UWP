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

        public void ConstructFreeTransform()
        {
            this.LTXButton.Click += (s, e) => this.NumberShowAt(this.LTXButton, NumberPickerMode.Case0);
            this.LTYButton.Click += (s, e) => this.NumberShowAt(this.LTYButton, NumberPickerMode.Case1);

            this.RTXButton.Click += (s, e) => this.NumberShowAt(this.RTXButton, NumberPickerMode.Case2);
            this.RTYButton.Click += (s, e) => this.NumberShowAt(this.RTYButton, NumberPickerMode.Case3);

            this.RBXButton.Click += (s, e) => this.NumberShowAt(this.RBXButton, NumberPickerMode.Case4);
            this.RBYButton.Click += (s, e) => this.NumberShowAt(this.RBYButton, NumberPickerMode.Case5);

            this.LBXButton.Click += (s, e) => this.NumberShowAt(this.LBXButton, NumberPickerMode.Case6);
            this.LBYButton.Click += (s, e) => this.NumberShowAt(this.LBYButton, NumberPickerMode.Case7);
        }

        private void ResetFreeTransform(PixelBounds bounds)
        {
            this.Bounds = bounds.ToBorder();

            this.BoundsFreeTransformer = this.Bounds.ToTransformer();
            this.BoundsFreeMatrix = Matrix3x2.Identity;
            this.BoundsFreeDistance = Vector2.Zero;

            this.LTXButton.Number = (int)this.BoundsFreeTransformer.LeftTop.X;
            this.LTYButton.Number = (int)this.BoundsFreeTransformer.LeftTop.Y;

            this.RTXButton.Number = (int)this.BoundsFreeTransformer.RightTop.X;
            this.RTYButton.Number = (int)this.BoundsFreeTransformer.RightTop.Y;

            this.RBXButton.Number = (int)this.BoundsFreeTransformer.RightBottom.X;
            this.RBYButton.Number = (int)this.BoundsFreeTransformer.RightBottom.Y;

            this.LBXButton.Number = (int)this.BoundsFreeTransformer.LeftBottom.X;
            this.LBYButton.Number = (int)this.BoundsFreeTransformer.LeftBottom.Y;
        }

        private void SetFreeTransform(NumberPickerMode mode, int e)
        {
            switch (mode)
            {
                case NumberPickerMode.Case0:
                    this.BoundsFreeTransformer.LeftTop = new Vector2(e, this.BoundsFreeTransformer.LeftTop.Y);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LTXButton.Number = e;
                    break;
                case NumberPickerMode.Case1:
                    this.BoundsFreeTransformer.LeftTop = new Vector2(this.BoundsFreeTransformer.LeftTop.X, e);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LTYButton.Number = e;
                    break;

                case NumberPickerMode.Case2:
                    this.BoundsFreeTransformer.RightTop = new Vector2(e, this.BoundsFreeTransformer.RightTop.Y);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RTXButton.Number = e;
                    break;
                case NumberPickerMode.Case3:
                    this.BoundsFreeTransformer.RightTop = new Vector2(this.BoundsFreeTransformer.RightTop.X, e);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RTYButton.Number = e;
                    break;

                case NumberPickerMode.Case4:
                    this.BoundsFreeTransformer.RightBottom = new Vector2(e, this.BoundsFreeTransformer.RightBottom.Y);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RBXButton.Number = e;
                    break;
                case NumberPickerMode.Case5:
                    this.BoundsFreeTransformer.RightBottom = new Vector2(this.BoundsFreeTransformer.RightBottom.X, e);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RBYButton.Number = e;
                    break;

                case NumberPickerMode.Case6:
                    this.BoundsFreeTransformer.LeftBottom = new Vector2(e, this.BoundsFreeTransformer.LeftBottom.Y);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LBXButton.Number = e;
                    break;
                case NumberPickerMode.Case7:
                    this.BoundsFreeTransformer.LeftBottom = new Vector2(this.BoundsFreeTransformer.LeftBottom.X, e);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LBYButton.Number = e;
                    break;

                default:
                    break;
            }
        }

        private void FreeTransform_Start()
        {
            this.BoundsMode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.BoundsFreeTransformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void FreeTransform_Delta()
        {
            switch (this.BoundsMode)
            {
                case TransformerMode.ScaleLeftTop:
                    this.BoundsFreeTransformer.LeftTop = this.Position;
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate
                    break;
                case TransformerMode.ScaleRightTop:
                    this.BoundsFreeTransformer.RightTop = this.Position;
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate
                    break;
                case TransformerMode.ScaleRightBottom:
                    this.BoundsFreeTransformer.RightBottom = this.Position;
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate
                    break;
                case TransformerMode.ScaleLeftBottom:
                    this.BoundsFreeTransformer.LeftBottom = this.Position;
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate
                    break;
                default:
                    break;
            }
        }

        private void FreeTransform_Complete()
        {
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            switch (this.BoundsMode)
            {
                case TransformerMode.ScaleLeftTop:
                    this.LTXButton.Number = (int)this.BoundsFreeTransformer.LeftTop.X;
                    this.LTYButton.Number = (int)this.BoundsFreeTransformer.LeftTop.Y;
                    break;
                case TransformerMode.ScaleRightTop:
                    this.RTXButton.Number = (int)this.BoundsFreeTransformer.RightTop.X;
                    this.RTYButton.Number = (int)this.BoundsFreeTransformer.RightTop.Y;
                    break;
                case TransformerMode.ScaleRightBottom:
                    this.RBXButton.Number = (int)this.BoundsFreeTransformer.RightBottom.X;
                    this.RBYButton.Number = (int)this.BoundsFreeTransformer.RightBottom.Y;
                    break;
                case TransformerMode.ScaleLeftBottom:
                    this.LBXButton.Number = (int)this.BoundsFreeTransformer.LeftBottom.X;
                    this.LBYButton.Number = (int)this.BoundsFreeTransformer.LeftBottom.Y;
                    break;
                default:
                    break;
            }
        }

    }
}