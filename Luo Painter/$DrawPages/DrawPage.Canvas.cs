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

    internal sealed class DottedLine
    {
        public readonly CanvasLinearGradientBrush DottedLineBrush;
        public Matrix3x2 DottedLineTransform = Matrix3x2.Identity;

        public DottedLine(ICanvasResourceCreator resourceCreator)
        {
            this.DottedLineBrush = new CanvasLinearGradientBrush(resourceCreator, new CanvasGradientStop[]
            {
                new CanvasGradientStop
                {
                    Color = Windows.UI.Colors.White,
                    Position = 0
                },
                new CanvasGradientStop
                {
                    Color = Windows.UI.Colors.Black,
                    Position = 1
                }
            }, CanvasEdgeBehavior.Mirror, CanvasAlphaMode.Premultiplied)
            {
                StartPoint = Vector2.Zero,
                EndPoint = new Vector2(12)
            };
        }
        public void Draw(CanvasDrawingSession ds, CanvasTransformer transformer, IGraphicsEffectSource source)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
            ds.DrawImage(new InvertEffect
            {
                Source = new LuminanceToAlphaEffect//Alpha
                {
                    Source = new EdgeDetectionEffect//Edge
                    {
                        Amount = 1,
                        Source = new Transform2DEffect
                        {
                            BorderMode = EffectBorderMode.Hard,
                            InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                            TransformMatrix = transformer.GetMatrix(),
                            Source = source
                        }
                    }
                }
            });

            ds.Blend = CanvasBlend.Min;
            ds.FillRectangle(0, 0, transformer.ControlWidth, transformer.ControlHeight, this.DottedLineBrush);
        }
        public void Update()
        {
            this.DottedLineTransform.M31--;
            this.DottedLineTransform.M32--;
            this.DottedLineBrush.Transform = this.DottedLineTransform;
        }
    }

    public sealed partial class DrawPage : Page
    {

        Mesh Mesh;
        DottedLine DottedLine;

        private void ConstructCanvas()
        {
            this.CanvasAnimatedControl.CreateResources += (sender, args) =>
            {
                this.DottedLine = new DottedLine(sender);
                this.Marquee = new BitmapLayer(sender, this.Transformer.Width, this.Transformer.Height);
            };
            this.CanvasAnimatedControl.Draw += (sender, args) =>
            {
                this.DottedLine.Draw(args.DrawingSession, this.Transformer, this.Marquee.Source);

                if (this.MarqueeToolType == MarqueeToolType.None) return;

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">
                args.DrawingSession.Blend = CanvasBlend.Copy;
                args.DrawingSession.DrawMarqueeTool(sender, this.MarqueeToolType, this.MarqueeTool, sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
            };
            this.CanvasAnimatedControl.Update += (sender, args) => this.DottedLine.Update();

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
                this.GradientMesh = new GradientMesh(sender);
                this.Clipboard = new BitmapLayer(sender, this.Transformer.Width, this.Transformer.Height);

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