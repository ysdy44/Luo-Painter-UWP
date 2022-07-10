using FanKit.Transformers;
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
    public sealed partial class DrawPage : Page, ILayerManager
    {
        Vector2 StartingPosition;

        Vector2 Move;
        Vector2 StartingMove;

        TransformerBorder Bounds;

        TransformerMode BoundsMode;
        bool IsBoundsMove;

        Transformer BoundsTransformer;
        Transformer StartingBoundsTransformer;
        Matrix3x2 BoundsMatrix;

        Transformer BoundsFreeTransformer;
        Matrix3x2 BoundsFreeMatrix;
        Vector2 BoundsFreeDistance;

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

        private void DrawTransform(CanvasControl sender, CanvasDrawingSession ds)
        {
            switch (this.TransformComboBox.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    ds.DrawBoundNodes(this.BoundsTransformer, sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                    break;
                case 2:
                    ds.DrawBound(this.BoundsFreeTransformer, sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                    ds.DrawNode2(sender.Dpi.ConvertPixelsToDips(Vector2.Transform(this.BoundsFreeTransformer.LeftTop, this.Transformer.GetMatrix())));
                    ds.DrawNode2(sender.Dpi.ConvertPixelsToDips(Vector2.Transform(this.BoundsFreeTransformer.RightTop, this.Transformer.GetMatrix())));
                    ds.DrawNode2(sender.Dpi.ConvertPixelsToDips(Vector2.Transform(this.BoundsFreeTransformer.RightBottom, this.Transformer.GetMatrix())));
                    ds.DrawNode2(sender.Dpi.ConvertPixelsToDips(Vector2.Transform(this.BoundsFreeTransformer.LeftBottom, this.Transformer.GetMatrix())));
                    break;
                default:
                    break;
            }
        }

        private ICanvasImage GetTransformPreview(ICanvasImage image)
        {
            switch (this.TransformComboBox.SelectedIndex)
            {
                case 0:
                    return new Transform2DEffect
                    {
                        BorderMode = EffectBorderMode.Hard,
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                        TransformMatrix = Matrix3x2.CreateTranslation(this.Move),
                        Source = image
                    };
                case 1:
                    return new Transform2DEffect
                    {
                        BorderMode = EffectBorderMode.Hard,
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                        TransformMatrix = this.BoundsMatrix,
                        Source = image
                    };
                case 2:
                    return new PixelShaderEffect(this.FreeTransformShaderCodeBytes)
                    {
                        Source1 = image,
                        Properties =
                        {
                            ["matrix3x2"] = this.BoundsFreeMatrix,
                            ["zdistance"] = this.BoundsFreeDistance,
                            ["left"] = this.Bounds.Left,
                            ["top"] = this.Bounds.Top,
                            ["right"] = this.Bounds.Right,
                            ["bottom"] = this.Bounds.Bottom,
                        },
                    };
                default:
                    return image;
            }
        }


        private void Transform_Start(Vector2 point)
        {
            this.StartingPosition = this.ToPosition(point);
            switch (this.TransformComboBox.SelectedIndex)
            {
                case 0:
                    this.StartingMove = this.Move;
                    break;
                case 1:
                    this.StartingMove = this.Move;
                    this.BoundsMode = FanKit.Transformers.Transformer.ContainsNodeMode(point, this.BoundsTransformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                    this.IsBoundsMove = this.BoundsMode == TransformerMode.None && this.BoundsTransformer.FillContainsPoint(this.StartingPosition);
                    this.StartingBoundsTransformer = this.BoundsTransformer;
                    break;
                case 2:
                    this.BoundsMode = FanKit.Transformers.Transformer.ContainsNodeMode(point, this.BoundsFreeTransformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                    break;
                default:
                    break;
            }
        }

        private void Transform_Delta(Vector2 point)
        {
            Vector2 position = this.ToPosition(point);
            switch (this.TransformComboBox.SelectedIndex)
            {
                case 0:
                    this.Move = position - this.StartingPosition + this.StartingMove;

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate
                    break;
                case 1:
                    this.BoundsTransformer =
                        this.IsBoundsMove ?
                        this.StartingBoundsTransformer + (position - this.StartingPosition) :
                        FanKit.Transformers.Transformer.Controller(this.BoundsMode, this.StartingPosition, position, this.StartingBoundsTransformer);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasVirtualControl.Invalidate(); // Invalidate
                    this.CanvasControl.Invalidate(); // Invalidate
                    break;
                case 2:
                    switch (this.BoundsMode)
                    {
                        case TransformerMode.ScaleLeftTop:
                            this.BoundsFreeTransformer.LeftTop = position;
                            this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                            this.CanvasVirtualControl.Invalidate(); // Invalidate
                            this.CanvasControl.Invalidate(); // Invalidate
                            break;
                        case TransformerMode.ScaleRightTop:
                            this.BoundsFreeTransformer.RightTop = position;
                            this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                            this.CanvasVirtualControl.Invalidate(); // Invalidate
                            this.CanvasControl.Invalidate(); // Invalidate
                            break;
                        case TransformerMode.ScaleRightBottom:
                            this.BoundsFreeTransformer.RightBottom = position;
                            this.BoundsFreeMatrix = FanKit.Transformers.Transformer.FindHomography(this.BoundsFreeTransformer, this.Bounds, out this.BoundsFreeDistance);

                            this.CanvasVirtualControl.Invalidate(); // Invalidate
                            this.CanvasControl.Invalidate(); // Invalidate
                            break;
                        case TransformerMode.ScaleLeftBottom:
                            this.BoundsFreeTransformer.LeftBottom = position;
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

        private void Transform_Complete(Vector2 point)
        {
            switch (this.TransformComboBox.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

    }
}