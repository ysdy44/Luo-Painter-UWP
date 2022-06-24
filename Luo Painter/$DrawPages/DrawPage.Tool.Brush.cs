﻿using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        bool IsBrushOpaque => this.BrushOpaqueCheckBox.IsChecked is true;

        CanvasLinearGradientBrush LinearGradientBrush;
        CanvasRadialGradientBrush RadialGradientBrush;
        Rect BrushBounds;


        private ICanvasImage GetBrushPreview()
        {
            switch (this.SelectionType)
            {
                case SelectionType.None:
                    return this.BitmapLayer[BitmapType.Source];
                case SelectionType.All:
                    return InkPresenter.GetComposite(this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]);
                case SelectionType.PixelBounds:
                    return InkPresenter.GetDraw(this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp]);
                case SelectionType.MarqueePixelBounds:
                    return InkPresenter.GetDrawComposite(this.BitmapLayer[BitmapType.Source], this.BitmapLayer[BitmapType.Temp], this.Marquee[BitmapType.Source]);
                default:
                    return this.BitmapLayer[BitmapType.Source];
            }
        }


        private void Brush_Start(Vector2 point)
        {
            this.BitmapLayer = this.LayerListView.SelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip("No Layer", "Create a new Layer?");
                return;
            }

            this.SelectionType = this.BitmapLayer.GetDrawSelection(this.IsBrushOpaque, this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
            switch (this.SelectionType)
            {
                case SelectionType.None:
                    this.Tip("No Pixel", "The current Bitmap Layer is Transparent.");
                    return;
                case SelectionType.All:
                    break;
                default:
                    this.BitmapLayer.Hit(InterpolationColors);
                    this.BrushBounds = this.Marquee.CreateInterpolationBoundsScaled(InterpolationColors).ToRect();
                    break;
            }

            Color startColor = this.ColorMenu.Color;
            Color endColor = this.ColorMenu.Color;
            switch (this.BrushReverseCheckBox.IsChecked)
            {
                case true: startColor.A = byte.MinValue; break;
                case false: endColor.A = byte.MinValue; break;
                default: break;
            }

            this.Point = point;
            this.Position = this.ToPosition(point);

            switch (this.BrushComboBox.SelectedIndex)
            {
                case 0:
                    return;
                case 1:
                    this.LinearGradientBrush = new CanvasLinearGradientBrush(this.CanvasDevice, startColor, endColor)
                    {
                        StartPoint = this.Position,
                        EndPoint = this.Position,
                    };
                    break;
                case 2:
                case 3:
                    this.RadialGradientBrush = new CanvasRadialGradientBrush(this.CanvasDevice, startColor, endColor)
                    {
                        Center = this.Position,
                        RadiusX = 10,
                        RadiusY = 10,
                    };
                    break;
                default:
                    break;
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Brush_Delta(Vector2 point)
        {
            if (this.BitmapLayer is null) return;
            if (this.SelectionType is SelectionType.None) return;
            if (Vector2.DistanceSquared(this.Point, point) < 100) return;

            switch (this.BrushComboBox.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    {
                        Vector2 position = this.ToPosition(point);

                        this.LinearGradientBrush.EndPoint = position;
                        this.BrushClear(this.LinearGradientBrush);
                    }
                    break;
                case 2:
                    {
                        Vector2 position = this.ToPosition(point);

                        this.RadialGradientBrush.RadiusX =
                        this.RadialGradientBrush.RadiusY =
                        Vector2.Distance(this.Position, position);
                        this.BrushClear(this.RadialGradientBrush);
                    }
                    break;
                case 3:
                    {
                        Vector2 position = this.ToPosition(point);

                        this.RadialGradientBrush.RadiusX = System.Math.Abs(this.Position.X - position.X);
                        this.RadialGradientBrush.RadiusY = System.Math.Abs(this.Position.Y - position.Y);
                        this.BrushClear(this.RadialGradientBrush);
                    }
                    break;
                default:
                    break;
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Brush_Complete(Vector2 point)
        {
            if (this.BitmapLayer is null) return;
            if (this.SelectionType is SelectionType.None) return;

            switch (this.BrushComboBox.SelectedIndex)
            {
                case 0:
                    this.Brush(new CanvasSolidColorBrush(this.CanvasDevice, this.ColorMenu.Color));
                    break;
                case 1:
                    {
                        Vector2 position = this.ToPosition(point);

                        Color startColor = this.ColorMenu.Color;
                        Color endColor = this.ColorMenu.Color;
                        switch (this.BrushReverseCheckBox.IsChecked)
                        {
                            case true: startColor.A = byte.MinValue; break;
                            case false: endColor.A = byte.MinValue; break;
                            default: break;
                        }

                        this.LinearGradientBrush?.Dispose();
                        this.Brush(new CanvasLinearGradientBrush(this.CanvasDevice, startColor, endColor)
                        {
                            StartPoint = this.Position,
                            EndPoint = position
                        });
                    }
                    break;
                case 2:
                    {
                        Vector2 position = this.ToPosition(point);

                        Color startColor = this.ColorMenu.Color;
                        Color endColor = this.ColorMenu.Color;

                        switch (this.BrushReverseCheckBox.IsChecked)
                        {
                            case true: startColor.A = byte.MinValue; break;
                            case false: endColor.A = byte.MinValue; break;
                            default: break;
                        }

                        float radius = Vector2.Distance(this.Position, position);
                        this.RadialGradientBrush?.Dispose();
                        this.Brush(new CanvasRadialGradientBrush(this.CanvasDevice, startColor, endColor)
                        {
                            Center = this.Position,
                            RadiusX = radius,
                            RadiusY = radius
                        });
                    }
                    break;
                case 3:
                    {
                        Vector2 position = this.ToPosition(point);

                        Color startColor = this.ColorMenu.Color;
                        Color endColor = this.ColorMenu.Color;

                        switch (this.BrushReverseCheckBox.IsChecked)
                        {
                            case true: startColor.A = byte.MinValue; break;
                            case false: endColor.A = byte.MinValue; break;
                            default: break;
                        }

                        this.RadialGradientBrush?.Dispose();
                        this.Brush(new CanvasRadialGradientBrush(this.CanvasDevice, startColor, endColor)
                        {
                            Center = this.Position,
                            RadiusX = System.Math.Abs(this.Position.X - position.X),
                            RadiusY = System.Math.Abs(this.Position.Y - position.Y)
                        });
                    }
                    break;
                default:
                    break;
            }

            this.SelectionType = default;

            this.BitmapLayer.Flush();
            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
            this.BitmapLayer.RenderThumbnail();
            this.BitmapLayer = null;

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }


        private void BrushClear(ICanvasBrush brush)
        {
            switch (this.SelectionType)
            {
                case SelectionType.None:
                    return;
                case SelectionType.All:
                    this.BitmapLayer.Clear(brush, BitmapType.Temp);
                    break;
                case SelectionType.PixelBounds:
                    this.BitmapLayer.Clear(brush, this.BrushBounds, BitmapType.Temp);
                    break;
                case SelectionType.MarqueePixelBounds:
                    this.BitmapLayer.Clear(brush, this.BrushBounds, BitmapType.Temp);
                    break;
                default:
                    break;
            }
        }

        private void Brush(ICanvasBrush brush)
        {
            switch (this.SelectionType)
            {
                case SelectionType.None:
                    break;
                case SelectionType.All:
                    this.BitmapLayer.Fill(brush, BitmapType.Source);
                    // History
                    int removes1 = this.History.Push(this.BitmapLayer.GetBitmapResetHistory());
                    break;
                case SelectionType.PixelBounds:
                    CanvasCommandList commandList1 = new CanvasCommandList(this.CanvasDevice);
                    using (CanvasDrawingSession ds = commandList1.CreateDrawingSession())
                    {
                        ds.FillRectangle(this.BrushBounds, brush);
                    }
                    this.BitmapLayer.Draw(new AlphaMaskEffect
                    {
                        AlphaMask = this.BitmapLayer[BitmapType.Origin],
                        Source = commandList1
                    }, BitmapType.Source);
                    // History
                    int removes2 = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                    break;
                case SelectionType.MarqueePixelBounds:
                    CanvasCommandList commandList2 = new CanvasCommandList(this.CanvasDevice);
                    using (CanvasDrawingSession ds = commandList2.CreateDrawingSession())
                    {
                        ds.FillRectangle(this.BrushBounds, brush);
                    }
                    this.BitmapLayer.Draw(new AlphaMaskEffect
                    {
                        AlphaMask = this.Marquee[BitmapType.Source],
                        Source = commandList2
                    }, BitmapType.Source);
                    // History
                    int removes3 = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                    break;
                default:
                    break;
            }
        }

    }
}