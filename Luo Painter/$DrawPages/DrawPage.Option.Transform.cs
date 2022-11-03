using FanKit.Transformers;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        int TransformMode => this.TransformComboBox.SelectedIndex;

        Vector2 Move;
        Vector2 StartingMove;

        TransformerBorder Bounds;

        Transformer BoundsFreeTransformer;
        Matrix3x2 BoundsFreeMatrix;
        Vector2 BoundsFreeDistance;

        public void ConstructTransform()
        {
            this.TransformComboBox.SelectionChanged += (s, e) =>
            {
                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void SetTransform(PixelBounds bounds)
        {
            this.Move = Vector2.Zero;

            this.Bounds = bounds.ToBorder();

            this.BoundsTransformer = this.Bounds.ToTransformer();
            this.BoundsMatrix = Matrix3x2.Identity;

            this.BoundsFreeTransformer = this.BoundsTransformer;
            this.BoundsFreeMatrix = Matrix3x2.Identity;
            this.BoundsFreeDistance = Vector2.Zero;
        }

        private void DrawTransform(CanvasControl sender, CanvasDrawingSession ds, Matrix3x2 matrix)
        {
            switch (this.TransformMode)
            {
                case 0:
                    break;
                case 1:
                    ds.DrawBoundNodes(this.BoundsTransformer, matrix);
                    break;
                case 2:
                    ds.DrawBound(this.BoundsFreeTransformer, matrix);
                    ds.DrawNode2(Vector2.Transform(this.BoundsFreeTransformer.LeftTop, matrix));
                    ds.DrawNode2(Vector2.Transform(this.BoundsFreeTransformer.RightTop, matrix));
                    ds.DrawNode2(Vector2.Transform(this.BoundsFreeTransformer.RightBottom, matrix));
                    ds.DrawNode2(Vector2.Transform(this.BoundsFreeTransformer.LeftBottom, matrix));
                    break;
                default:
                    break;
            }
        }


        private void Transform_Start()
        {
            switch (this.TransformMode)
            {
                case 0:
                    this.StartingMove = this.Move;
                    break;
                case 1:
                    this.StartingMove = this.Move;
                    this.BoundsMode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.BoundsTransformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                    this.IsBoundsMove = this.BoundsMode is TransformerMode.None && this.BoundsTransformer.FillContainsPoint(this.StartingPosition);
                    this.StartingBoundsTransformer = this.BoundsTransformer;
                    break;
                case 2:
                    this.BoundsMode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.BoundsFreeTransformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                    break;
                default:
                    break;
            }
        }

        private void Transform_Delta()
        {
            switch (this.TransformMode)
            {
                case 0:
                    this.Move = this.Position - this.StartingPosition + this.StartingMove;

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate
                    break;
                case 1:
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
                        this.BoundsTransformer = FanKit.Transformers.Transformer.Controller(this.BoundsMode, this.StartingPosition, this.Position, this.StartingBoundsTransformer, this.IsShift, this.IsCtrl);
                        this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        this.CanvasControl.Invalidate(); // Invalidate
                    }
                    break;
                case 2:
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
                    break;
                default:
                    break;
            }
        }

    }
}