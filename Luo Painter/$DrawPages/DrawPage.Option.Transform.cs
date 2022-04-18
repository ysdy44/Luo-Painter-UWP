using FanKit.Transformers;
using Luo_Painter.Elements;
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
    public sealed partial class DrawPage : Page
    {

        Vector2 Move;
        Vector2 StartingMove;

        TransformerBorder Bounds;

        Transformer BoundsTransformer;
        Matrix3x2 BoundsMatrix;

        Transformer BoundsFreeTransformer;
        Matrix4x4 BoundsFreeMatrix;
        Vector2 BoundsFreeDistance;

        private void SetTransform(BitmapLayer bitmapLayer, Color[] InterpolationColors)
        {
            this.Move = Vector2.Zero;

            PixelBounds interpolationBounds = bitmapLayer.CreateInterpolationBounds(InterpolationColors);
            PixelBounds bounds = bitmapLayer.CreatePixelBounds(interpolationBounds);

            this.Bounds = bounds.ToBorder();

            this.BoundsTransformer = this.Bounds.ToTransformer();
            this.BoundsMatrix = Matrix3x2.Identity;

            this.BoundsFreeTransformer = this.BoundsTransformer;
            this.BoundsFreeMatrix = Matrix4x4.Identity;
            this.BoundsFreeDistance = Vector2.Zero;
        }

        private void ConstructTransform()
        {
            this.TransformButton.Click += (s, e) =>
            {
                this.OptionTypeCommand.Execute(OptionType.Transform);
            };

            this.TransformComboBox.SelectionChanged += (s, e) => this.CanvasControl.Invalidate(); // Invalidate
        }

        private void DrawTransform(CanvasVirtualControl sender, CanvasDrawingSession ds)
        {
            switch (this.TransformComboBox.SelectedIndex)
            {
                case 0:
                    ds.DrawBound(this.BoundsTransformer, Matrix3x2.CreateTranslation(this.Move) * sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
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
                        TransformMatrix = Matrix3x2.CreateTranslation(this.Move),
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
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
                    return new PixelShaderEffect(this.FreeTranformShaderCodeBytes)
                    {
                        Source1 = image,
                        Properties =
                        {
                            ["matrix4"] = this.BoundsFreeMatrix,
                            ["distce"] = this.BoundsFreeDistance,
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


        private void Transform_Start(Vector2 point, PointerPointProperties properties)
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

        private void Transform_Delta(Vector2 point, PointerPointProperties properties)
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

        private void Transform_Complete(Vector2 point, PointerPointProperties properties)
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