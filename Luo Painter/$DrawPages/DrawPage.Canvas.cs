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
using Windows.UI.Xaml;
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
        bool StartingToolShow;
        bool StartingLayerShow;
        private bool AntiMistouch => true;

        private void ConstructCanvas()
        {
            this.Canvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                Vector2 size = this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;

                this.AlignmentGrid.RebuildWithInterpolation(e.NewSize);

                this.CanvasControl.Width =
                this.CanvasAnimatedControl.Width =
                this.CanvasVirtualControl.Width = e.NewSize.Width;

                this.CanvasControl.Height =
                this.CanvasAnimatedControl.Height =
                this.CanvasVirtualControl.Height = e.NewSize.Height;
            };

            this.CanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.Blend = CanvasBlend.Copy;

                switch (this.OptionType)
                {
                    case OptionType.None:
                        //@DPI 
                        if (this.MarqueeToolType == MarqueeToolType.None) break;

                        args.DrawingSession.DrawMarqueeTool(this.CanvasDevice, this.MarqueeToolType, this.MarqueeTool, sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                        break;
                    case OptionType.Transform:
                        this.DrawTransform(sender, args.DrawingSession);
                        break;
                    case OptionType.RippleEffect:
                        this.DrawRippleEffect(sender, args.DrawingSession);
                        break;
                    default:
                        break;
                }
            };

            this.CanvasAnimatedControl.CreateResources += (sender, args) =>
            {
                this.Marquee = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
                this.Marquee.RenderThumbnail();
                this.LayerListView.MarqueeSource = this.Marquee.Thumbnail;
                args.TrackAsyncAction(this.CreateDottedLineResourcesAsync().AsAsyncAction());
            };
            this.CanvasAnimatedControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                args.DrawingSession.Blend = CanvasBlend.Copy;

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
            };

            this.CanvasVirtualControl.CreateResources += (sender, args) =>
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
            this.CanvasVirtualControl.RegionsInvalidated += (sender, args) =>
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
                    }
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                if (this.AntiMistouch)
                {
                    this.StartingToolShow = this.ToolListView.IsShow;//&& this.ToolListView.Width > 70;
                    this.StartingLayerShow = this.LayerListView.IsShow;//&& this.LayerListView.Width > 70;
                }
                this.Tool_Start(point, properties);
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                if (this.AntiMistouch)
                {
                    if (this.StartingToolShow && this.ToolListView.IsShow && point.X > 0 && point.X < this.ToolListView.Width)
                        this.ToolListView.IsShow = false;
                    if (this.StartingLayerShow && this.LayerListView.IsShow && point.X < base.ActualWidth && point.X > base.ActualWidth - this.LayerListView.Width)
                        this.LayerListView.IsShow = false;
                }
                this.Tool_Delta(point, properties);
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                if (this.AntiMistouch)
                {
                    this.ToolListView.IsShow = this.StartingToolShow;
                    this.LayerListView.IsShow = this.StartingLayerShow;
                }
                this.Tool_Complete(point, properties);
            };


            // Right
            this.Operator.Right_Start += this.View_Start;
            this.Operator.Right_Delta += this.View_Delta;
            this.Operator.Right_Complete += this.View_Complete;


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(center), this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate

                this.CanvasAnimatedControl.Paused = true;
                this.CanvasAnimatedControl.Visibility = Visibility.Collapsed;
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(center), this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
                this.CanvasAnimatedControl.Invalidate(); // Invalidate

                this.CanvasAnimatedControl.Paused = this.OptionType != default;
                this.CanvasAnimatedControl.Visibility = this.CanvasAnimatedControl.Paused ? Visibility.Collapsed : Visibility.Visible;

                this.ViewTool.Construct(this.Transformer);
            };

            // Wheel
            this.Operator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.Transformer.ZoomIn(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), 1.05f);
                else
                    this.Transformer.ZoomOut(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), 1.05f);

                this.Tip("Zoom", $"{this.Transformer.Scale * 100:0.00}%"); // Tip

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate

                this.ViewTool.Construct(this.Transformer);
            };
        }

    }
}