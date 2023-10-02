using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        int BrushMode => this.BrushComboBox.SelectedIndex;

        bool IsBrushOpaque => this.BrushOpaqueButton.IsChecked is true;
        bool IsBrushReverse => this.BrushReverseButton.IsChecked is true;

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


        private void Brush_Start()
        {
            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                return;
            }

            this.SelectionType = this.BitmapLayer.GetDrawSelection(this.IsBrushOpaque, this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
            switch (this.SelectionType)
            {
                case SelectionType.None:
                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                    return;
                case SelectionType.All:
                    break;
                default:
                    this.BitmapLayer.Hit(InterpolationColors);
                    this.BrushBounds = this.Marquee.CreateInterpolationBoundsScaled(InterpolationColors).ToRect();
                    break;
            }

            Color startColor = this.Color;
            Color endColor = this.Color;
            switch (this.IsBrushReverse)
            {
                case true: startColor.A = byte.MinValue; break;
                case false: endColor.A = byte.MinValue; break;
                default: break;
            }

            switch (this.BrushMode)
            {
                case 0:
                    this.LinearGradientBrush = new CanvasLinearGradientBrush(this.CanvasDevice, startColor, endColor)
                    {
                        StartPoint = this.StartingPosition,
                        EndPoint = this.Position,
                    };
                    return;
                case 1:
                case 2:
                    this.RadialGradientBrush = new CanvasRadialGradientBrush(this.CanvasDevice, startColor, endColor)
                    {
                        Center = this.StartingPosition,
                        RadiusX = 10,
                        RadiusY = 10,
                    };
                    break;
                default:
                    break;
            }

            this.CanvasAnimatedControl.Paused = true; // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Brush_Delta()
        {
            if (this.BitmapLayer is null) return;
            if (this.SelectionType is SelectionType.None) return;
            if (Vector2.DistanceSquared(this.StartingPoint, this.Point) < 100) return;

            switch (this.BrushMode)
            {
                case 0:
                    this.LinearGradientBrush.EndPoint = this.Position;
                    this.BrushClear(this.LinearGradientBrush);
                    break;
                case 1:
                    this.RadialGradientBrush.RadiusX =
                    this.RadialGradientBrush.RadiusY =
                    Vector2.Distance(this.StartingPosition, this.Position);
                    this.BrushClear(this.RadialGradientBrush);
                    break;
                case 2:
                    this.RadialGradientBrush.RadiusX = System.Math.Abs(this.StartingPosition.X - this.Position.X);
                    this.RadialGradientBrush.RadiusY = System.Math.Abs(this.StartingPosition.Y - this.Position.Y);
                    this.BrushClear(this.RadialGradientBrush);
                    break;
                default:
                    break;
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Brush_Complete()
        {
            if (this.BitmapLayer is null) return;
            if (this.SelectionType is SelectionType.None) return;

            switch (this.BrushMode)
            {
                case 0:
                    {
                        Color startColor = this.Color;
                        Color endColor = this.Color;
                        switch (this.IsBrushReverse)
                        {
                            case true: startColor.A = byte.MinValue; break;
                            case false: endColor.A = byte.MinValue; break;
                            default: break;
                        }

                        this.LinearGradientBrush?.Dispose();
                        this.Brush(new CanvasLinearGradientBrush(this.CanvasDevice, startColor, endColor)
                        {
                            StartPoint = this.StartingPosition,
                            EndPoint = this.Position
                        });
                    }
                    break;
                case 1:
                    {
                        Color startColor = this.Color;
                        Color endColor = this.Color;

                        switch (this.IsBrushReverse)
                        {
                            case true: startColor.A = byte.MinValue; break;
                            case false: endColor.A = byte.MinValue; break;
                            default: break;
                        }

                        float radius = Vector2.Distance(this.Position, this.Position);
                        this.RadialGradientBrush?.Dispose();
                        this.Brush(new CanvasRadialGradientBrush(this.CanvasDevice, startColor, endColor)
                        {
                            Center = this.StartingPosition,
                            RadiusX = radius,
                            RadiusY = radius
                        });
                    }
                    break;
                case 2:
                    {
                        Color startColor = this.Color;
                        Color endColor = this.Color;

                        switch (this.IsBrushReverse)
                        {
                            case true: startColor.A = byte.MinValue; break;
                            case false: endColor.A = byte.MinValue; break;
                            default: break;
                        }

                        this.RadialGradientBrush?.Dispose();
                        this.Brush(new CanvasRadialGradientBrush(this.CanvasDevice, startColor, endColor)
                        {
                            Center = this.StartingPosition,
                            RadiusX = System.Math.Abs(this.StartingPosition.X - this.Position.X),
                            RadiusY = System.Math.Abs(this.StartingPosition.Y - this.Position.Y)
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

            this.CanvasAnimatedControl.Paused = this.OptionType.HasPreview(); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }


        private void Fill_Complete()
        {
            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                return;
            }

            this.SelectionType = this.BitmapLayer.GetDrawSelection(this.IsBrushOpaque, this.Marquee, out Color[] InterpolationColors, out PixelBoundsMode mode);
            switch (this.SelectionType)
            {
                case SelectionType.None:
                    this.ToastTip.Tip(TipType.NoPixelForBitmapLayer.GetString(), TipType.NoPixelForBitmapLayer.GetString(true));
                    return;
                case SelectionType.All:
                    break;
                default:
                    this.BitmapLayer.Hit(InterpolationColors);
                    this.BrushBounds = this.Marquee.CreateInterpolationBoundsScaled(InterpolationColors).ToRect();
                    break;
            }

            this.Brush(this.Color);

            this.SelectionType = default;

            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();
            this.BitmapLayer = null;

            this.CanvasAnimatedControl.Paused = this.OptionType.HasPreview(); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
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
                    {
                        this.BitmapLayer.Fill(brush, BitmapType.Source);

                        // History
                        IHistory history = this.BitmapLayer.GetBitmapResetHistory();
                        history.Title = this.OptionType.GetString();
                        int removes = this.History.Push(history);
                    }
                    break;
                case SelectionType.PixelBounds:
                    {
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
                        IHistory history = this.BitmapLayer.GetBitmapHistory();
                        history.Title = this.OptionType.GetString();
                        int removes = this.History.Push(history);
                    }
                    break;
                case SelectionType.MarqueePixelBounds:
                    {
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
                        IHistory history = this.BitmapLayer.GetBitmapHistory();
                        history.Title = this.OptionType.GetString();
                        int removes = this.History.Push(history);
                    }
                    break;
                default:
                    break;
            }
        }

        private void Brush(Color color)
        {
            switch (this.SelectionType)
            {
                case SelectionType.None:
                    break;
                case SelectionType.All:
                    {
                        this.BitmapLayer.Fill(color, BitmapType.Source);

                        // History
                        IHistory history = this.BitmapLayer.GetBitmapResetHistory();
                        history.Title = this.OptionType.GetString();
                        int removes = this.History.Push(history);
                    }
                    break;
                case SelectionType.PixelBounds:
                    {
                        CanvasCommandList commandList1 = new CanvasCommandList(this.CanvasDevice);
                        using (CanvasDrawingSession ds = commandList1.CreateDrawingSession())
                        {
                            ds.FillRectangle(this.BrushBounds, color);
                        }
                        this.BitmapLayer.Draw(new AlphaMaskEffect
                        {
                            AlphaMask = this.BitmapLayer[BitmapType.Origin],
                            Source = commandList1
                        }, BitmapType.Source);

                        // History
                        IHistory history = this.BitmapLayer.GetBitmapHistory();
                        history.Title = this.OptionType.GetString();
                        int removes = this.History.Push(history);
                    }
                    break;
                case SelectionType.MarqueePixelBounds:
                    {
                        CanvasCommandList commandList2 = new CanvasCommandList(this.CanvasDevice);
                        using (CanvasDrawingSession ds = commandList2.CreateDrawingSession())
                        {
                            ds.FillRectangle(this.BrushBounds, color);
                        }
                        this.BitmapLayer.Draw(new AlphaMaskEffect
                        {
                            AlphaMask = this.Marquee[BitmapType.Source],
                            Source = commandList2
                        }, BitmapType.Source);

                        // History
                        IHistory history = this.BitmapLayer.GetBitmapHistory();
                        history.Title = this.OptionType.GetString();
                        int removes = this.History.Push(history);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}