﻿using FanKit.Transformers;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Linq;
using System.Numerics;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        TransformerMode CropMode;
        bool IsCropMove;

        Transformer CropTransformer;
        Transformer StartingCropTransformer;

        private void ConstructSetup()
        {
            this.AppBar.CropCanvasValueChanged += (s, e) =>
            {
                double radian = e.NewValue / 180 * System.Math.PI;
                this.Transformer.Radian = (float)radian;
                this.Transformer.ReloadMatrix();

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private Vector2 ToPositionWithoutRadian(Vector2 point) => Vector2.Transform(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point),
            Matrix3x2.CreateTranslation(-this.Transformer.Position) *
            Matrix3x2.CreateScale(1 / this.Transformer.Scale) *
            Matrix3x2.CreateTranslation(this.Transformer.Width / 2, this.Transformer.Height / 2));
        private Matrix3x2 GetMatrixWithoutRadian(float dpi) => dpi.ConvertPixelsToDips(
                Matrix3x2.CreateTranslation(-this.Transformer.Width / 2, -this.Transformer.Height / 2) *
                Matrix3x2.CreateScale(this.Transformer.Scale) *
                Matrix3x2.CreateTranslation(this.Transformer.Position));

        private Matrix3x2 GetMatrix(Vector2 leftTop) =>
            Matrix3x2.CreateRotation(this.Transformer.Radian,
                new Vector2(this.Transformer.Width / 2, this.Transformer.Height / 2)) *
            Matrix3x2.CreateTranslation(-leftTop);

        private void SetCropCanvas(int w, int h)
        {
            this.CropTransformer = new Transformer(0, 0, w, h);

            this.AppBar.CropCanvasValue = 0d;

            if (this.Transformer.Radian == 0f) return;
            this.Transformer.Radian = 0f;
            this.Transformer.ReloadMatrix();
        }

        private void DrawCropCanvas(CanvasControl sender, CanvasDrawingSession ds)
        {
            Transformer crop = FanKit.Transformers.Transformer.Multiplies(this.CropTransformer, this.GetMatrixWithoutRadian(sender.Dpi));

            ds.Clear(FanKit.Transformers.CanvasDrawingSessionExtensions.ShadowColor);
            ds.FillRectangle(crop.MinX, crop.MinY, crop.MaxX - crop.MinX, crop.MaxY - crop.MinY, Colors.Transparent);

            ds.DrawCrop(crop);
        }


        /// <summary> <see cref="Transform_Start(Vector2)"/> </summary>
        private void CropCanvas_Start(Vector2 point)
        {
            this.StartingPosition = this.ToPositionWithoutRadian(point);

            this.CropMode = FanKit.Transformers.Transformer.ContainsNodeMode(point, this.CropTransformer, this.GetMatrixWithoutRadian(this.CanvasVirtualControl.Dpi), true);
            this.IsCropMove = this.CropMode == TransformerMode.None;
            this.StartingCropTransformer = this.CropTransformer;
        }

        /// <summary> <see cref="Transform_Delta(Vector2)"/> </summary>
        private void CropCanvas_Delta(Vector2 point)
        {
            Vector2 position = this.ToPositionWithoutRadian(point);

            this.CropTransformer =
                this.IsCropMove ?
                this.StartingCropTransformer + (position - this.StartingPosition) :
                FanKit.Transformers.Transformer.Controller(this.CropMode, this.StartingPosition, position, this.StartingCropTransformer);

            this.CanvasControl.Invalidate(); // Invalidate
        }

        /// <summary> <see cref="Transform_Complete(Vector2)"/> </summary>
        private void CropCanvas_Complete(Vector2 point)
        {
        }


        private void CancelCropCanvas()
        {
            if (this.AppBar.CropCanvasValue is 0d) return;

            this.Transformer.Radian = 0f;
            this.Transformer.ReloadMatrix();
            this.AppBar.CropCanvasValue = 0;
        }
        private void PrimaryCropCanvas()
        {
            int width2 = this.Transformer.Width;
            int height2 = this.Transformer.Height;

            uint width = (uint)width2;
            uint height = (uint)height2;

            float w3 = this.CropTransformer.MaxX - this.CropTransformer.MinX;
            float h3 = this.CropTransformer.MaxY - this.CropTransformer.MinY;

            int w = (int)w3;
            int h = (int)h3;

            uint w2 = (uint)w;
            uint h2 = (uint)h;

            Vector2 offset = new Vector2
            {
                X = -this.CropTransformer.MinX,
                Y = -this.CropTransformer.MinY,
            };

            if (this.AppBar.CropCanvasValue is 0d)
            {
                this.Transformer.Width = w;
                this.Transformer.Height = h;
                this.Transformer.Fit();

                this.CreateResources(w, h);
                this.CreateMarqueeResources(w, h);

                int removes = this.History.Push(this.LayerManager.Setup(this, this.Nodes.Select(c => c.Crop(this.CanvasDevice, w, h, offset)).ToArray(), new SetupSizes
                {
                    UndoParameter = new BitmapSize { Width = width, Height = height },
                    RedoParameter = new BitmapSize { Width = w2, Height = h2 }
                }));

                this.CanvasVirtualControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            }
            else
            {
                Matrix3x2 matrix = this.GetMatrix(this.CropTransformer.LeftTop);
                {
                    this.Transformer.Width = w;
                    this.Transformer.Height = h;
                    this.Transformer.Fit();

                    this.CreateResources(w, h);
                    this.CreateMarqueeResources(w, h);
                }
                int removes = this.History.Push(this.LayerManager.Setup(this, this.Nodes.Select(c => c.Crop(this.CanvasDevice, w, h, matrix, CanvasImageInterpolation.NearestNeighbor)).ToArray(), new SetupSizes
                {
                    UndoParameter = new BitmapSize
                    {
                        Width = width,
                        Height = height
                    },
                    RedoParameter = new BitmapSize
                    {
                        Width = w2,
                        Height = h2
                    }
                }));

                this.CanvasVirtualControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;


                this.Transformer.Radian = 0f;
                this.Transformer.ReloadMatrix();
                this.AppBar.CropCanvasValue = 0;
            }
        }

    }
}