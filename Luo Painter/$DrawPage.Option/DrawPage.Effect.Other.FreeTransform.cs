using FanKit.Transformers;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using System.Numerics;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public void ConstructFreeTransform()
        {
            this.LTPicker.XClick += (s, e) => this.NumberShowAt(this.LTPicker.XNumber, (NumberPickerMode)0);
            this.LTPicker.YClick += (s, e) => this.NumberShowAt(this.LTPicker.YNumber, (NumberPickerMode)1);

            this.RTPicker.XClick += (s, e) => this.NumberShowAt(this.RTPicker.XNumber, (NumberPickerMode)2);
            this.RTPicker.YClick += (s, e) => this.NumberShowAt(this.RTPicker.YNumber, (NumberPickerMode)3);

            this.RBPicker.XClick += (s, e) => this.NumberShowAt(this.RBPicker.XNumber, (NumberPickerMode)4);
            this.RBPicker.YClick += (s, e) => this.NumberShowAt(this.RBPicker.YNumber, (NumberPickerMode)5);

            this.LBPicker.XClick += (s, e) => this.NumberShowAt(this.LBPicker.XNumber, (NumberPickerMode)6);
            this.LBPicker.YClick += (s, e) => this.NumberShowAt(this.LBPicker.YNumber, (NumberPickerMode)7);
        }

        private void ResetFreeTransform(PixelBounds bounds)
        {
            this.FreeTransform.Distance = Vector2.Zero;
            this.FreeTransform.Matrix = Matrix3x2.Identity;
            this.FreeTransform.Border = bounds.ToBorder();
            this.FreeTransform.Transformer = this.FreeTransform.Border.ToTransformer();

            this.LTPicker.Value = this.FreeTransform.Transformer.LeftTop;
            this.RTPicker.Value = this.FreeTransform.Transformer.RightTop;
            this.RBPicker.Value = this.FreeTransform.Transformer.RightBottom;
            this.LBPicker.Value = this.FreeTransform.Transformer.LeftBottom;
        }

        private void SetFreeTransform(NumberPickerMode mode, float e)
        {
            switch ((int)mode)
            {
                case 0:
                    this.FreeTransform.Transformer.LeftTop = new Vector2(e, this.FreeTransform.Transformer.LeftTop.Y);
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LTPicker.X = e;
                    break;
                case 1:
                    this.FreeTransform.Transformer.LeftTop = new Vector2(this.FreeTransform.Transformer.LeftTop.X, e);
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LTPicker.Y = e;
                    break;

                case 2:
                    this.FreeTransform.Transformer.RightTop = new Vector2(e, this.FreeTransform.Transformer.RightTop.Y);
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RTPicker.X = e;
                    break;
                case 3:
                    this.FreeTransform.Transformer.RightTop = new Vector2(this.FreeTransform.Transformer.RightTop.X, e);
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RTPicker.Y = e;
                    break;

                case 4:
                    this.FreeTransform.Transformer.RightBottom = new Vector2(e, this.FreeTransform.Transformer.RightBottom.Y);
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RBPicker.X = e;
                    break;
                case 5:
                    this.FreeTransform.Transformer.RightBottom = new Vector2(this.FreeTransform.Transformer.RightBottom.X, e);
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RBPicker.Y = e;
                    break;

                case 6:
                    this.FreeTransform.Transformer.LeftBottom = new Vector2(e, this.FreeTransform.Transformer.LeftBottom.Y);
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LBPicker.X = e;
                    break;
                case 7:
                    this.FreeTransform.Transformer.LeftBottom = new Vector2(this.FreeTransform.Transformer.LeftBottom.X, e);
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

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
            this.FreeTransform.Mode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.FreeTransform.Transformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void FreeTransform_Delta()
        {
            switch (this.FreeTransform.Mode)
            {
                case TransformerMode.ScaleLeftTop:
                    this.FreeTransform.Transformer.LeftTop = this.Position;
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LTPicker.Value = this.Position;
                    break;
                case TransformerMode.ScaleRightTop:
                    this.FreeTransform.Transformer.RightTop = this.Position;
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RTPicker.Value = this.Position;
                    break;
                case TransformerMode.ScaleRightBottom:
                    this.FreeTransform.Transformer.RightBottom = this.Position;
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.RBPicker.Value = this.Position;
                    break;
                case TransformerMode.ScaleLeftBottom:
                    this.FreeTransform.Transformer.LeftBottom = this.Position;
                    this.FreeTransform.Matrix = FanKit.Transformers.Transformer.FindHomography(this.FreeTransform.Transformer, this.FreeTransform.Border, out this.FreeTransform.Distance);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate

                    this.LBPicker.Value = this.Position;
                    break;
                default:
                    break;
            }
        }

        private void FreeTransform_Complete()
        {
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            switch (this.FreeTransform.Mode)
            {
                case TransformerMode.ScaleLeftTop:
                    this.LTPicker.Value = this.FreeTransform.Transformer.LeftTop;
                    break;
                case TransformerMode.ScaleRightTop:
                    this.RTPicker.Value = this.FreeTransform.Transformer.RightTop;
                    break;
                case TransformerMode.ScaleRightBottom:
                    this.RBPicker.Value = this.FreeTransform.Transformer.RightBottom;
                    break;
                case TransformerMode.ScaleLeftBottom:
                    this.LBPicker.Value = this.FreeTransform.Transformer.LeftBottom;
                    break;
                default:
                    break;
            }
        }

    }
}