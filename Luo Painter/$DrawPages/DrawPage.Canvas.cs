﻿using Luo_Painter.Elements;
using Luo_Painter.Options;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal sealed class Mesh
    {
        // Mesh
        public readonly CanvasBitmap Bitmap;
        public readonly ICanvasImage Image;
        public readonly CanvasGeometry Geometry;
        public Mesh(ICanvasResourceCreator resourceCreator, float scale, int width, int height)
        {
            this.Bitmap = CanvasBitmap.CreateFromColors(resourceCreator, new Color[]
            {
                Colors.LightGray, Colors.White,
                Colors.White, Colors.LightGray
            }, 2, 2);

            this.Image = new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = new ScaleEffect
                {
                    Scale = new Vector2(scale),
                    BorderMode = EffectBorderMode.Hard,
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Source = new BorderEffect
                    {
                        ExtendX = CanvasEdgeBehavior.Wrap,
                        ExtendY = CanvasEdgeBehavior.Wrap,
                        Source = this.Bitmap
                    }
                }
            };

            this.Geometry = CanvasGeometry.CreateRectangle(resourceCreator, 0, 0, width, height);
        }
    }

    public sealed partial class DrawPage : Page
    {

        Vector2 Position;
        float Pressure;

        Mesh Mesh;
        byte[] LiquefactionShaderCodeBytes;
        byte[] FreeTranformShaderCodeBytes;

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
                this.Transformer.Fit();

                this.Mesh = new Mesh(sender, sender.Dpi.ConvertDipsToPixels(25), this.Transformer.Width, this.Transformer.Height);

                // Layer
                BitmapLayer bitmapLayer = new BitmapLayer(sender, this.Transformer.Width, this.Transformer.Height);
                this.Layers.Add(bitmapLayer.Id, bitmapLayer);
                this.ObservableCollection.Add(bitmapLayer);
                this.LayerListView.SelectedIndex = 0;

                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.RegionsInvalidated += (sender, args) =>
            {
                foreach (Rect region in args.InvalidatedRegions)
                {
                    using (CanvasDrawingSession ds = sender.CreateDrawingSession(region))
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                        // Mesh
                        using (ds.CreateLayer(1, this.Mesh.Geometry, this.Transformer.GetMatrix()))
                        {
                            // Layer
                            if (this.BitmapLayer is null)
                                ds.DrawImage(this.Render(this.Mesh.Image, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor));
                            else if (this.OptionType == OptionType.None)
                                ds.DrawImage(this.Render(this.Mesh.Image, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor, this.BitmapLayer.Id, this.GetInk(this.BitmapLayer)));
                            else
                                ds.DrawImage(this.Render(this.Mesh.Image, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor, this.BitmapLayer.Id, this.GetPreview(this.OptionType, this.BitmapLayer.Source)));
                        }

                        switch (this.OptionType)
                        {
                            case OptionType.Transform:
                                ds.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">
                                this.DrawTransform(sender, ds);
                                break;
                            default:
                                break;
                        }
                    }
                }
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.LiquefactionShaderCodeBytes = await ShaderType.Liquefaction.LoadAsync();
            this.FreeTranformShaderCodeBytes = await ShaderType.FreeTranform.LoadAsync();
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.ToPosition(point);
                switch (this.OptionType)
                {
                    case OptionType.None:
                        this.Pressure = properties.Pressure;
                        this.Tool_Start(this.Position);
                        break;
                    case OptionType.Transform:
                        this.Transform_Start(this.Position);
                        break;
                    default:
                        break;
                }
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.ToPosition(point);
                switch (this.OptionType)
                {
                    case OptionType.None:
                        this.Tool_Delta(this.Position, position, this.Pressure, properties.Pressure);
                        this.Position = position;
                        this.Pressure = properties.Pressure;
                        break;
                    case OptionType.Transform:
                        this.Transform_Delta(this.Position, position);
                        break;
                    default:
                        break;
                }
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                switch (this.OptionType)
                {
                    case OptionType.None:
                        Vector2 position = this.ToPosition(point);
                        this.Tool_Delta(this.Position, position, this.Pressure, properties.Pressure);
                        this.Tool_Complete(position);
                        break;
                    case OptionType.Transform:
                        this.Transform_Complete(this.Position);
                        break;
                    default:
                        break;
                }
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


                this.Tip("Zoom", $"{this.Transformer.Scale * 100:0.00}%"); // Tip

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}