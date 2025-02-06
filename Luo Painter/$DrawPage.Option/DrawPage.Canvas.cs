﻿using FanKit.Transformers;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.UI;
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
    public sealed partial class DrawPage
    {

        private void ConstructCanvas()
        {
            this.Canvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;
                if (this.AlignmentGrid.RebuildWithInterpolation(e.NewSize) is false) return;

                this.CanvasControl.Width =
                this.CanvasAnimatedControl.Width =
                this.CanvasVirtualControl.Width =
                this.SimulateCanvas.Width =
                e.NewSize.Width;

                this.CanvasControl.Height =
                this.CanvasAnimatedControl.Height =
                this.CanvasVirtualControl.Height =
                this.SimulateCanvas.Height =
                e.NewSize.Height;

                Vector2 size = this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(e.NewSize.ToVector2());
                this.Transformer.ControlWidth = size.X;
                this.Transformer.ControlHeight = size.Y;
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
                    case OptionType.SelectionBrush:
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

                                args.DrawingSession.DrawBoundNodes(this.Transform.Transformer, matrix);
                                break;

                            case OptionType.SelectionBrush:
                                args.DrawingSession.Transform = Matrix3x2.Identity;
                                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">

                                args.DrawingSession.DrawCircle(this.Point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.SelectionSize * this.Transformer.Scale), Colors.Gray);
                                break;
                        }
                        break;

                    case OptionType.CropCanvas:
                        this.DrawCropCanvas(sender, args.DrawingSession);
                        break;

                    case OptionType.AddImageTransform:
                        args.DrawingSession.DrawBoundNodes(this.Transform.Transformer, matrix);
                        break;

                    case OptionType.Move:
                        break;
                    case OptionType.Transform:
                        args.DrawingSession.DrawBoundNodes(this.Transform.Transformer, matrix);
                        break;
                    case OptionType.FreeTransform:
                        args.DrawingSession.DrawBound(this.FreeTransform.Transformer, matrix);
                        args.DrawingSession.DrawNode2(Vector2.Transform(this.FreeTransform.Transformer.LeftTop, matrix));
                        args.DrawingSession.DrawNode2(Vector2.Transform(this.FreeTransform.Transformer.RightTop, matrix));
                        args.DrawingSession.DrawNode2(Vector2.Transform(this.FreeTransform.Transformer.RightBottom, matrix));
                        args.DrawingSession.DrawNode2(Vector2.Transform(this.FreeTransform.Transformer.LeftBottom, matrix));
                        break;

                    case OptionType.DisplacementLiquefaction:
                        if (this.BitmapLayer is null) break;

                        args.DrawingSession.DrawCircle(this.Point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips((float)this.DisplacementLiquefactionSize * this.Transformer.Scale), Colors.Gray);
                        break;
                    case OptionType.GradientMapping:
                        break;
                    case OptionType.RippleEffect:
                        this.DrawRippleEffect(args.DrawingSession);
                        break;

                    case OptionType.Border:
                        this.DrawBorder(sender, args.DrawingSession);
                        break;
                    case OptionType.Lighting:
                        this.DrawLighting(args.DrawingSession);
                        break;

                    case OptionType.PaintBrush:
                    case OptionType.PaintBrushForce:
                        if (this.BitmapLayer is null) break;

                        args.DrawingSession.DrawCircle(this.Point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale), Colors.Gray);
                        break;
                    case OptionType.PaintLine:
                        if (this.BitmapLayer is null) break;

                        args.DrawingSession.DrawLine(this.StartingPoint, this.Point, Colors.Gray);
                        args.DrawingSession.DrawCircle(this.Point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale), Colors.Gray);
                        args.DrawingSession.DrawCircle(this.StartingPoint, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale), Colors.Gray);
                        break;
                    case OptionType.PaintBrushMulti:
                        this.DrawPaintBrushMulti(sender, args.DrawingSession, this.ToPoint(this.Marquee.Center));
                        break;

                    default:
                        if (this.OptionType.IsMarquee())
                        {
                            args.DrawingSession.DrawMarqueeTool(this.CanvasDevice, this.MarqueeToolType, this.MarqueeTool, sender.Dpi.ConvertPixelsToDips(this.Transformer.GetMatrix()));
                            break;
                        }

                        if (this.OptionType.IsGeometry() || this.OptionType.IsPattern())
                        {
                            if (this.BitmapLayer is null) break;

                            args.DrawingSession.DrawBound(this.CreateTransform.Transformer, matrix);
                            break;
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
                this.GradientMesh = new CanvasRenderTarget(this.CanvasDevice, 256, 1, 96);
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
                        Source = this.Mesh,
                        TransformMatrix = this.Transformer.GetMatrix(),
                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    })
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                        // Mesh
                        // Layer
                        switch (this.OptionTarget)
                        {
                            case OptionTarget.BitmapLayer:
                                if (this.BitmapLayer is null)
                                    goto case OptionTarget.Marquee;

                                ds.DrawImage(this.Nodes.ReplaceRender(mesh, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor, BitmapLayer.Id, this.GetMezzanine()));
                                break;
                            case OptionTarget.Marquee:
                                // Normal
                                ds.DrawImage(this.Nodes.Render(mesh, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor));
                                break;
                            case OptionTarget.Image:
                                if (string.IsNullOrEmpty(this.AddImageId))
                                {
                                    // Normal
                                    ds.DrawImage(this.Nodes.Render(mesh, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor));
                                    ds.DrawImage(new Transform2DEffect
                                    {
                                        InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                                        TransformMatrix = this.Transformer.GetMatrix(),
                                        Source = this.GetPreview(OptionType.AddImageTransform, this.AddImage)
                                    });
                                }
                                else
                                {
                                    ds.DrawImage(this.Nodes.AboveRender(mesh, this.Transformer.GetMatrix(), CanvasImageInterpolation.NearestNeighbor, this.AddImageId, this.GetPreview(OptionType.AddImageTransform, this.AddImage)));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            };


            this.HistogramCanvasControl.Draw += (sender, args) =>
            {
                args.DrawingSession.Blend = CanvasBlend.Add;
                if (this.RedHistogram != null) for (int i = 0; i < 240; i++) args.DrawingSession.DrawLine(i, 240 - 240 * 64 * this.RedHistogram[i], i, 240, Colors.Red);
                if (this.GreenHistogram != null) for (int i = 0; i < 240; i++) args.DrawingSession.DrawLine(i, 240 - 240 * 64 * this.GreenHistogram[i], i, 240, Colors.Lime);
                if (this.BlueHistogram != null) for (int i = 0; i < 240; i++) args.DrawingSession.DrawLine(i, 240 - 240 * 64 * this.BlueHistogram[i], i, 240, Colors.Blue);
            };


            this.StrawCanvasControl.CreateResources += (sender, args) =>
            {
                this.StrawViewer.CreateResources(sender);
            };
            this.StrawCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(this.StrawViewer.StrawImage);
            };


            this.InkCanvasControl.CreateResources += (sender, args) =>
            {
                this.InkRender = new CanvasRenderTarget(sender, InkPresenter.Width, InkPresenter.Height);
                this.Ink();
            };
            this.InkCanvasControl.Draw += (sender, args) =>
            {
                switch (this.InkType)
                {
                    case InkType.Blur:
                        args.DrawingSession.DrawImage(InkPresenter.GetBlur(this.InkRender, this.InkPresenter.Flow * 4));
                        break;
                    case InkType.Mosaic:
                        args.DrawingSession.DrawImage(InkPresenter.GetMosaic(this.InkRender, this.InkPresenter.Size / 10));
                        break;
                    default:
                        args.DrawingSession.DrawImage(this.InkRender);
                        break;
                }
            };
        }

        private ICanvasImage GetMezzanine()
        {
            switch (this.OptionType)
            {
                case OptionType.Brush:
                    return this.GetBrushPreview();
                case OptionType.Transparency:
                    return this.GetTransparencyPreview();
                case OptionType.Transform:
                case OptionType.AddImageTransform:
                case OptionType.MarqueeTransform:
                    switch (this.OptionTarget)
                    {
                        case OptionTarget.Marquee:
                            return this.GetPreview(this.OptionType, this.Marquee[BitmapType.Source]);
                        case OptionTarget.Image:
                            return this.GetPreview(this.OptionType, this.AddImage);
                        default:
                            return this.GetPreview(this.OptionType, this.BitmapLayer[BitmapType.Source]);
                    }
                default:
                    // 1. Geometry Tool
                    if (this.OptionType.IsGeometry())
                    {
                        return new CompositeEffect { Sources = { this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp] } };
                    }
                    // 2. Others Tool
                    else if (this.OptionType.IsEffect())
                    {
                        // 3. Marquee Preview
                        switch (this.SelectionType)
                        {
                            case SelectionType.MarqueePixelBounds:
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
                                // 3. Mask Preview
                                else
                                {
                                    return new PixelShaderEffect(this.LalphaMaskShaderCodeBytes)
                                    {
                                        Source1 = this.Marquee[BitmapType.Source],
                                        Source2 = this.BitmapLayer[BitmapType.Origin],
                                        Source3 = this.GetPreview(this.OptionType, this.BitmapLayer[BitmapType.Origin])
                                    };
                                }
                            default:
                                // 3. Difference Preview
                                return this.GetPreview(this.OptionType, this.BitmapLayer[BitmapType.Source]);
                        }
                    }
                    else
                    {
                        return this.InkPresenter.GetPreview(this.InkType, this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]);
                    }
            }
        }

        private void Histogram()
        {
            using (ColorSourceEffect background = new ColorSourceEffect
            {
                Color = Colors.Transparent
            })
            {
                ICanvasImage image = this.Nodes.Render(background);
                Rect bounds = this.Mesh.Bounds;
                this.RedHistogram = CanvasImage.ComputeHistogram(image, bounds, this.CanvasDevice, EffectChannelSelect.Red, 240);
                this.GreenHistogram = CanvasImage.ComputeHistogram(image, bounds, this.CanvasDevice, EffectChannelSelect.Green, 240);
                this.BlueHistogram = CanvasImage.ComputeHistogram(image, bounds, this.CanvasDevice, EffectChannelSelect.Blue, 240);
            }
        }

        private void Straw()
        {
            using (CanvasDrawingSession ds = this.StrawViewer.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                ds.Blend = CanvasBlend.Copy;

                Matrix3x2 matrix =
                    this.Transformer.GetMatrix() *
                    Matrix3x2.CreateTranslation(-this.StrawCanvasControl.Dpi.ConvertDipsToPixels(this.Point)) *
                    Matrix3x2.CreateScale(4) *
                    Matrix3x2.CreateTranslation(this.StrawViewer.StrawCenterX, this.StrawViewer.StrawCenterY);

                using (Transform2DEffect mesh = new Transform2DEffect
                {
                    Source = this.Mesh,
                    TransformMatrix = matrix,
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                })
                {
                    ds.DrawImage(this.Nodes.Render(mesh, matrix, CanvasImageInterpolation.NearestNeighbor));
                }
            }
        }

    }
}