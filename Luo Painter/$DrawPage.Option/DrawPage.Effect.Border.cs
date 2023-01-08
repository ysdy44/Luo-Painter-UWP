using FanKit.Transformers;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        CanvasEdgeBehavior ExtendX => this.ToExtend(this.BorderXComboBox.SelectedIndex);
        CanvasEdgeBehavior ExtendY => this.ToExtend(this.BorderYComboBox.SelectedIndex);

        CanvasEdgeBehavior ToExtend(int index)
        {
            switch (index)
            {
                case 0: return CanvasEdgeBehavior.Clamp;
                case 1: return CanvasEdgeBehavior.Wrap;
                case 2: return CanvasEdgeBehavior.Mirror;
                default: return default;
            }
        }
        Rect ToRect(Transformer transformer)
        {
            float minX = transformer.MinX;
            float minY = transformer.MinY;
            float maxX = transformer.MaxX;
            float maxY = transformer.MaxY;
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private void ResetBorder(int w, int h)
        {
            this.BorderTransform.Transformer = new Transformer(0, 0, w, h);

            this.BorderCrop = new Rect(0, 0, w, h);
            this.StartingBorderCrop = this.BorderCrop;
        }

        private void DrawBorder(CanvasControl sender, CanvasDrawingSession ds)
        {
            ds.DrawCrop(this.BorderTransform.Transformer, sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
        }


        /// <summary> <see cref="CropCanvas_Start()"/> </summary>
        private void Border_Start()
        {
            this.BorderTransform.Mode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.BorderTransform.Transformer, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()), true);
            this.BorderTransform.IsMove = this.BorderTransform.Mode == TransformerMode.None;
            this.BorderTransform.StartingTransformer = this.BorderTransform.Transformer;

            this.BorderCrop = this.ToRect(this.BorderTransform.Transformer);

            this.CanvasControl.Invalidate(); // Invalidate
        }

        /// <summary> <see cref="CropCanvas_Delta()"/> </summary>
        private void Border_Delta()
        {
            if (this.BorderTransform.IsMove)
                this.BorderTransform.Transformer = this.BorderTransform.StartingTransformer + (this.Position - this.StartingPosition);
            else
                this.BorderTransform.Transformer = FanKit.Transformers.Transformer.Controller(this.BorderTransform.Mode, this.StartingPosition, this.Position, this.BorderTransform.StartingTransformer);

            this.BorderCrop = this.ToRect(this.BorderTransform.Transformer);

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

        /// <summary> <see cref="CropCanvas_Complete()"/> </summary>
        private void Border_Complete()
        {
            this.CanvasControl.Invalidate(); // Invalidate
        }

    }
}