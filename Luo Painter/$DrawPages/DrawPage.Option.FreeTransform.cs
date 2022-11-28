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
            this.LTPicker.XClick += (s, e) => this.NumberShowAt(this.LTPicker.XNumber, NumberPickerMode.Case0);
            this.LTPicker.YClick += (s, e) => this.NumberShowAt(this.LTPicker.YNumber, NumberPickerMode.Case1);

            this.RTPicker.XClick += (s, e) => this.NumberShowAt(this.RTPicker.XNumber, NumberPickerMode.Case2);
            this.RTPicker.YClick += (s, e) => this.NumberShowAt(this.RTPicker.YNumber, NumberPickerMode.Case3);

            this.RBPicker.XClick += (s, e) => this.NumberShowAt(this.RBPicker.XNumber, NumberPickerMode.Case4);
            this.RBPicker.YClick += (s, e) => this.NumberShowAt(this.RBPicker.YNumber, NumberPickerMode.Case5);

            this.LBPicker.XClick += (s, e) => this.NumberShowAt(this.LBPicker.XNumber, NumberPickerMode.Case6);
            this.LBPicker.YClick += (s, e) => this.NumberShowAt(this.LBPicker.YNumber, NumberPickerMode.Case7);
        }

        private void ResetFreeTransform(PixelBounds bounds)
        {
            this.Bounds = bounds.ToBorder();

            this.BoundsFreeTransformer = this.Bounds.ToTransformer();
            this.BoundsFreeMatrix = Matrix3x2.Identity;
            this.BoundsFreeDistance = Vector2.Zero;

            this.LTPicker.Value = this.BoundsFreeTransformer.LeftTop;
            this.RTPicker.Value = this.BoundsFreeTransformer.RightTop;
            this.RBPicker.Value = this.BoundsFreeTransformer.RightBottom;
            this.LBPicker.Value = this.BoundsFreeTransformer.LeftBottom;
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

                    this.LTPicker.X = e;
                    break;
                case NumberPickerMode.Case1:
                    this.BoundsFreeTransformer.LeftTop = new Vector2(this.BoundsFreeTransformer.LeftTop.X, e);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LTPicker.Y = e;
                    break;

                case NumberPickerMode.Case2:
                    this.BoundsFreeTransformer.RightTop = new Vector2(e, this.BoundsFreeTransformer.RightTop.Y);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RTPicker.X = e;
                    break;
                case NumberPickerMode.Case3:
                    this.BoundsFreeTransformer.RightTop = new Vector2(this.BoundsFreeTransformer.RightTop.X, e);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RTPicker.Y = e;
                    break;

                case NumberPickerMode.Case4:
                    this.BoundsFreeTransformer.RightBottom = new Vector2(e, this.BoundsFreeTransformer.RightBottom.Y);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RBPicker.X = e;
                    break;
                case NumberPickerMode.Case5:
                    this.BoundsFreeTransformer.RightBottom = new Vector2(this.BoundsFreeTransformer.RightBottom.X, e);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RBPicker.Y = e;
                    break;

                case NumberPickerMode.Case6:
                    this.BoundsFreeTransformer.LeftBottom = new Vector2(e, this.BoundsFreeTransformer.LeftBottom.Y);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LBPicker.X = e;
                    break;
                case NumberPickerMode.Case7:
                    this.BoundsFreeTransformer.LeftBottom = new Vector2(this.BoundsFreeTransformer.LeftBottom.X, e);
                    this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LBPicker.Y = e;
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
                    this.LTPicker.Value = this.BoundsFreeTransformer.LeftTop;
                    break;
                case TransformerMode.ScaleRightTop:
                    this.RTPicker.Value = this.BoundsFreeTransformer.RightTop;
                    break;
                case TransformerMode.ScaleRightBottom:
                    this.RBPicker.Value = this.BoundsFreeTransformer.RightBottom;
                    break;
                case TransformerMode.ScaleLeftBottom:
                    this.LBPicker.Value = this.BoundsFreeTransformer.LeftBottom;
                    break;
                default:
                    break;
            }
        }

    }
}