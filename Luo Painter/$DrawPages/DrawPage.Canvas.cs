using FanKit.Transformers;
using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
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
    public sealed partial class DrawPage : Page, ILayerManager
    {

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

            this.InkCanvasControl.CreateResources += (sender, args) =>
            {
                this.InkRender = new InkRender(sender, 320, 100);
                this.Ink();
            };
            this.InkCanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.DrawImage(this.InkRender.Source);
            };


            this.CanvasControl.Draw += (sender, args) =>
            {
                Matrix3x2 matrix = sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix());
                foreach (ReferenceImage item in this.ReferenceImages)
                {
                    item.Draw(args.DrawingSession, matrix);
                }

                args.DrawingSession.Blend = CanvasBlend.Copy;

                switch (this.OptionType)
                {
                    case OptionType.Feather:
                    case OptionType.MarqueeTransform:
                    case OptionType.Grow:
                    case OptionType.Shrink:
                        //@DPI 
                        args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        args.DrawingSession.Transform = this.Transformer.GetMatrix();

                        args.DrawingSession.DrawImage(new OpacityEffect
                        {
                            Opacity = 0.5f,
                            Source = this.GetPreview(this.OptionType, this.Marquee[BitmapType.Source])
                        });

                        switch (this.OptionType)
                        {
                            case OptionType.MarqueeTransform:
                                args.DrawingSession.Transform = Matrix3x2.Identity;
                                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                                this.DrawTransform(sender, args.DrawingSession, matrix);
                                break;
                        }
                        break;

                    case OptionType.CropCanvas:
                        this.DrawCropCanvas(sender, args.DrawingSession);
                        break;

                    case OptionType.Transform:
                        this.DrawTransform(sender, args.DrawingSession, matrix);
                        break;
                    case OptionType.DisplacementLiquefaction:
                        this.DrawDisplacementLiquefaction(sender, args.DrawingSession);
                        break;
                    case OptionType.GradientMapping:
                        break;
                    case OptionType.RippleEffect:
                        this.DrawRippleEffect(sender, args.DrawingSession);
                        break;

                    default:
                        if (this.OptionType.HasFlag(OptionType.Marquee))
                        {
                            args.DrawingSession.DrawMarqueeTool(this.CanvasDevice, this.MarqueeToolType, this.MarqueeTool, sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                        }
                        if (this.OptionType.HasFlag(OptionType.Geometry))
                        {
                            if (this.BitmapLayer is null) break;

                            args.DrawingSession.DrawBound(this.BoundsTransformer, matrix);
                        }
                        break;
                }
            };


            this.CanvasAnimatedControl.CreateResources += (sender, args) =>
            {
                this.CreateMarqueeResources(this.Transformer.Width, this.Transformer.Height);
                args.TrackAsyncAction(this.CreateDottedLineResourcesAsync().AsAsyncAction());
            };
            this.CanvasAnimatedControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                args.DrawingSession.Blend = CanvasBlend.Copy;

                args.DrawingSession.DrawImage(new PixelShaderEffect(this.DottedLineTransformShaderCodeBytes)
                {
                    Source1 = this.Marquee[BitmapType.Source],
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
                this.GradientMesh = new GradientMesh(this.CanvasDevice);
                this.GrayAndWhiteMesh = CanvasBitmap.CreateFromColors(this.CanvasDevice, new Color[]
                {
                    Colors.LightGray, Colors.White,
                    Colors.White, Colors.LightGray
                }, 2, 2);
                this.CreateResources(this.Transformer.Width, this.Transformer.Height);
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasVirtualControl.RegionsInvalidated += (sender, args) =>
            {
                foreach (Rect region in args.InvalidatedRegions)
                {
                    using (CanvasDrawingSession ds = sender.CreateDrawingSession(region))
                    using (Transform2DEffect mesh = new Transform2DEffect
                    {
                        Source = this.Mesh[BitmapType.Source],
                        TransformMatrix = this.Transformer.GetMatrix(),
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    })
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                        // Mesh
                        // Layer
                        if (this.OptionType.IsEdit() || this.BitmapLayer is null)
                            ds.DrawImage(this.Nodes.Render(mesh, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor));
                        else
                            ds.DrawImage(this.Nodes.Render(mesh, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor, this.BitmapLayer.Id, this.GetMezzanine()));
                    }
                }
            };
        }

        private ICanvasImage GetMezzanine()
        {
            if (this.OptionType.IsEffect() is false)
            {
                switch (this.OptionType)
                {
                    case OptionType.Brush:
                        return this.GetBrushPreview();
                    case OptionType.Transparency:
                        return this.GetTransparencyPreview();
                    default:
                        if (this.OptionType.HasFlag(OptionType.Geometry))
                        {
                            return new CompositeEffect { Sources = { this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp] } };
                        }
                        else
                        {
                            return this.InkPresenter.GetPreview(this.InkType, this.BitmapLayer[BitmapType.Source], this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp]));
                        }
                }
            }


            if (this.SelectionType is SelectionType.MarqueePixelBounds is false)
            {
                return this.GetPreview(this.OptionType, this.BitmapLayer[BitmapType.Source]);
            }


            if (this.OptionType.HasDifference())
            {
                return new CompositeEffect
                {
                    Sources =
                    {
                        new PixelShaderEffect(this.RalphaMaskShaderCodeBytes)
                        {
                            Source1 = this.Marquee[BitmapType.Source],
                            Source2 = this.BitmapLayer[BitmapType.Source],
                        },
                        this.GetPreview(this.OptionType, new AlphaMaskEffect
                        {
                            AlphaMask = this.Marquee[BitmapType.Source],
                            Source = this.BitmapLayer[BitmapType.Source]
                        })
                    }
                };
            }
            else
            {
                return new PixelShaderEffect(this.LalphaMaskShaderCodeBytes)
                {
                    Source1 = this.Marquee[BitmapType.Source],
                    Source2 = this.BitmapLayer[BitmapType.Origin],
                    Source3 = this.GetPreview(this.OptionType, this.BitmapLayer[BitmapType.Origin])
                };
            }
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
                this.SetCanvasState(true);

                this.StartingPosition = this.Position = this.ToPosition(point);
                this.StartingPoint = this.Point = point;
                this.Pressure = properties.Pressure;

                for (int i = 0; i < this.ReferenceImages.Count; i++)
                {
                    ReferenceImage item = this.ReferenceImages[i];
                    if (FanKit.Math.InNodeRadius(this.ToPoint(item.Size + item.Position), point))
                    {
                        this.ReferenceImage = item;
                        this.IsReferenceImageResizing = true;
                        this.CanvasControl.Invalidate();
                        return;
                    }
                    if (item.Contains(this.Position))
                    {
                        item.Cache();
                        this.ReferenceImage = item;
                        this.IsReferenceImageResizing = false;
                        this.CanvasControl.Invalidate();
                        return;
                    }
                }

                this.Tool_Start(this.Position, this.Point, properties.Pressure);
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

                Vector2 position = this.ToPosition(point);

                if (this.ReferenceImage is null)
                {
                    this.Tool_Delta(position, point, properties.Pressure);
                }
                else
                {
                    if (this.IsReferenceImageResizing)
                        this.ReferenceImage.Resizing(position);
                    else
                        this.ReferenceImage.Add(position - this.StartingPosition);

                    this.CanvasControl.Invalidate();
                }
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                if (this.AntiMistouch)
                {
                    this.ToolListView.IsShow = this.StartingToolShow;
                    this.LayerListView.IsShow = this.StartingLayerShow;
                }
                this.SetCanvasState(this.OptionType.IsEdit() || this.OptionType.IsEffect());

                if (this.ReferenceImage is null)
                {
                    this.Tool_Complete(this.ToPosition(point), point, properties.Pressure);
                }
                else
                {
                    this.ReferenceImage = null;
                    this.CanvasControl.Invalidate();
                }
            };


            // Right
            this.Operator.Right_Start += this.View_Start;
            this.Operator.Right_Delta += this.View_Delta;
            this.Operator.Right_Complete += this.View_Complete;


            // Double
            this.Operator.Double_Start += (center, space) =>
            {
                this.Transformer.CachePinch(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(center), this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(space));

                this.SetCanvasState(true);
            };
            this.Operator.Double_Delta += (center, space) =>
            {
                this.Transformer.Pinch(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(center), this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(space));

                this.CanvasVirtualControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Double_Complete += (center, space) =>
            {
                this.SetCanvasState(this.OptionType.IsEdit() || this.OptionType.IsEffect());

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