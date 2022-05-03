using FanKit.Transformers;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Effects;
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

        Mesh Mesh;

        private void ConstructCanvas()
        {
            this.CanvasAnimatedControl.CreateResources += (sender, args) =>
            {
                this.Marquee = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
                args.TrackAsyncAction(this.CreateDottedLineResourcesAsync().AsAsyncAction());
            };
            this.CanvasAnimatedControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(new PixelShaderEffect(this.DottedLineTransformShaderCodeBytes)
                {
                    Source1 = this.Marquee.Source,
                    Properties =
                    {
                        ["time"] = (float)args.Timing.UpdateCount,
                        ["lineWidth"] = sender.Dpi.ConvertDipsToPixels(2),
                        ["left"] = 0f,
                        ["top"] = 0f,
                        ["right"] = (float)this.Transformer.Width,
                        ["bottom"] = (float)this.Transformer.Height,
                        ["matrix3x2"] = this.Transformer.GetInverseMatrix(),
                    },
                });

                if (this.MarqueeToolType == MarqueeToolType.None) return;

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">
                args.DrawingSession.Blend = CanvasBlend.Copy;
                args.DrawingSession.DrawMarqueeTool(this.CanvasDevice, this.MarqueeToolType, this.MarqueeTool, sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
            };
            //this.CanvasAnimatedControl.Update += (sender, args) =>
            //{
            //};

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
                this.ViewTool.Construct(this.Transformer);

                this.Mesh = new Mesh(this.CanvasDevice, sender.Dpi.ConvertDipsToPixels(25), this.Transformer.Width, this.Transformer.Height);
                this.GradientMesh = new GradientMesh(this.CanvasDevice);
                this.Clipboard = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

                // Layer
                BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
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
                                ds.DrawImage(this.Render(this.Mesh.Image, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor, this.BitmapLayer.Id, this.PaintTool.GetInk(this.BitmapLayer)));
                            else
                                ds.DrawImage(this.Render(this.Mesh.Image, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor, this.BitmapLayer.Id, this.GetPreview(this.OptionType, this.BitmapLayer.Source)));
                        }

                        switch (this.OptionType)
                        {
                            case OptionType.Transform:
                                ds.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">
                                this.DrawTransform(sender, ds);
                                break;
                            case OptionType.RippleEffect:
                                ds.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">
                                this.DrawRippleEffect(sender, ds);
                                break;
                            default:
                                break;
                        }
                    }
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                switch (this.OptionType)
                {
                    case OptionType.None:
                        this.Tool_Start(point, properties);
                        break;
                    case OptionType.Transform:
                        this.Transform_Start(point, properties);
                        break;
                    case OptionType.RippleEffect:
                        this.RippleEffect_Start(point, properties);
                        break;
                    default:
                        break;
                }
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                switch (this.OptionType)
                {
                    case OptionType.None:
                        this.Tool_Delta(point, properties);
                        break;
                    case OptionType.Transform:
                        this.Transform_Delta(point, properties);
                        break;
                    case OptionType.RippleEffect:
                        this.RippleEffect_Delta(point, properties);
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
                        this.Tool_Delta(point, properties);
                        this.Tool_Complete(point, properties);
                        break;
                    case OptionType.Transform:
                        this.Transform_Delta(point, properties);
                        this.Transform_Complete(point, properties);
                        break;
                    case OptionType.RippleEffect:
                        this.RippleEffect_Delta(point, properties);
                        this.RippleEffect_Complete(point, properties);
                        break;
                    default:
                        break;
                }
            };


            // Right
            this.Operator.Right_Start += this.View_Start;
            this.Operator.Right_Delta += this.View_Delta;
            this.Operator.Right_Complete += this.View_Complete;


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

                this.ViewTool.Construct(this.Transformer);
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

                this.ViewTool.Construct(this.Transformer);
            };
        }

    }
}