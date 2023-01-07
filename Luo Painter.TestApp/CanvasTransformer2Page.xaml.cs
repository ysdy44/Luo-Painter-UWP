﻿using FanKit.Transformers;
using Luo_Painter.Elements;
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


        Transformer StartingBoundsTransformer;
        Transformer BoundsTransformer;

        // Transform
        TransformerBorder Bounds;
        Matrix3x2 BoundsMatrix = Matrix3x2.Identity;
        TransformerMode BoundsMode;
        bool IsBoundsMove;

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
                    TransformMatrix = this.BoundsMatrix * this.Transformer.GetMatrix()
                });

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());

                args.DrawingSession.DrawBoundNodes(this.BoundsTransformer, matrix);
            };
        }

        private async Task CreateResourcesAsync(ICanvasResourceCreator sender)
        {
            this.Bitmap = await CanvasBitmap.LoadAsync(sender, "Assets\\Square150x150Logo.scale-200.png");
            this.BoundsTransformer = new Transformer(this.Bitmap.SizeInPixels.Width, this.Bitmap.SizeInPixels.Height, Vector2.Zero);
            this.Bounds = new TransformerBorder(this.Bitmap.SizeInPixels.Width, this.Bitmap.SizeInPixels.Height);
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPoint = this.Point = point;

                this.BoundsMode = FanKit.Transformers.Transformer.ContainsNodeMode(this.Point, this.BoundsTransformer, this.CanvasControl.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));

                this.IsBoundsMove = this.BoundsMode is TransformerMode.None && this.BoundsTransformer.FillContainsPoint(this.StartingPosition);
                this.StartingBoundsTransformer = this.BoundsTransformer;

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Position = this.ToPosition(point);
                this.Point = point;

                if (this.IsBoundsMove)
                {
                    this.BoundsTransformer = this.StartingBoundsTransformer + (this.Position - this.StartingPosition);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasControl.Invalidate(); // Invalidate
                }
                else if (this.BoundsMode != default)
                {
                    this.BoundsTransformer = FanKit.Transformers.Transformer.Controller(this.BoundsMode, this.StartingPosition, this.Position, this.StartingBoundsTransformer, this.IsRatio, this.IsCenter);
                    this.BoundsMatrix = FanKit.Transformers.Transformer.FindHomography(this.Bounds, this.BoundsTransformer);

                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };


            // Right
            this.Operator.Right_Start += (point) =>
            {
                this.Transformer.CacheMove(this.CanvasControl.Dpi.ConvertDipsToPixels(point));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Delta += (point) =>
            {
                this.Transformer.Move(this.CanvasControl.Dpi.ConvertDipsToPixels(point));

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Right_Complete += (point) =>
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