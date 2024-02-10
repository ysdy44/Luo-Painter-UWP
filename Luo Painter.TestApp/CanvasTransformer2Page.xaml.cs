using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class CanvasTransformer2Page : Page
    {

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        //@Key
        private bool IsCenter => false;
        private bool IsRatio => false;


        CanvasBitmap Bitmap;

        Vector2 StartingPosition;
        Vector2 Position;

        Vector2 StartingPoint;
        Vector2 Point;

        TransformMatrix Transform;

        public CanvasTransformer2Page()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                Vector2 size = this.CanvasControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;
            };
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                args.TrackAsyncAction(this.CreateResourcesAsync(sender).AsAsyncAction());
                this.Transformer.Fit();
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawCard(new ColorSourceEffect
                {
                    Color = Colors.White
                }, this.Transformer, Colors.Black);

                args.DrawingSession.DrawImage(new Transform2DEffect
                {
                    Source = this.Bitmap,
                    TransformMatrix = this.Transform.Matrix * this.Transformer.GetMatrix()
                });

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());

                args.DrawingSession.DrawBoundNodes(this.Transform.Transformer, matrix);
            };
        }

        private async Task CreateResourcesAsync(ICanvasResourceCreator sender)
        {
            this.Bitmap = await CanvasBitmap.LoadAsync(sender, "Assets\\Square150x150Logo.scale-200.png");
            this.Transform = new TransformMatrix
            {
                Matrix = Matrix3x2.Identity,
                Border = new TransformerBorder(512, 512),
                Transformer = new Transformer(this.Bitmap.SizeInPixels.Width, this.Bitmap.SizeInPixels.Height, Vector2.Zero)
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPoint = this.Point = point;


                this.Transform.Mode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.Transform.Transformer, this.CanvasControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));

                this.Transform.IsMove = this.Transform.Mode is TransformerMode.None && this.Transform.Transformer.FillContainsPoint(this.StartingPosition);
                this.Transform.StartingTransformer = this.Transform.Transformer;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Point = point;


                if (this.Transform.IsMove)
                {
                    this.Transform.Transformer = this.Transform.StartingTransformer + (this.Position - this.StartingPosition);
                    this.Transform.UpdateMatrix();

                    this.CanvasControl.Invalidate(); // Invalidate
                }
                else if (this.Transform.Mode != default)
                {
                    this.Transform.Transformer = FanKit.Transformers.Transformer.Controller(this.Transform.Mode, this.StartingPosition, this.Position, this.Transform.StartingTransformer, this.IsRatio, this.IsCenter);
                    this.Transform.UpdateMatrix();

                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point, isHolding) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point, isHolding) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));

                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasControl.Dpi.ConvertDipsToPixels(center), this.CanvasControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}