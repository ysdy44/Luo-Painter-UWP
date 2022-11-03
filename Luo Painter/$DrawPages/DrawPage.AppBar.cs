using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    internal enum ContextAppBarState
    {
        None,
        Main,
        Preview,
        DragPreview,
    }

    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        Point AppBarStartingPosition;

        public void ConstructFoots()
        {
            this.AppBarThumb.DragStarted += (s, e) => this.AppBarStartingPosition = new Point(this.AppBarTransform.X, this.AppBarTransform.Y);
            this.AppBarThumb.DragDelta += (s, e) =>
            {
                if (this.AppBarGrid.Margin.Bottom is 0)
                {
                    this.AppBarStartingPosition.Y += e.VerticalChange;

                    this.AppBarTransform.Y = System.Math.Clamp(this.AppBarStartingPosition.Y, 50, this.SwitchPresenter.ActualHeight);
                }
                else
                {
                    this.AppBarStartingPosition.X += e.HorizontalChange;
                    this.AppBarStartingPosition.Y += e.VerticalChange;

                    this.AppBarTransform.X = this.AppBarStartingPosition.X;
                    this.AppBarTransform.Y = this.AppBarStartingPosition.Y;
                }
            };
            this.AppBarThumb.DragCompleted += (s, e) => { };
        }

        public void ConstructAppBar(OptionType type)
        {
            switch (this.GetState(type))
            {
                case ContextAppBarState.None:
                    this.SwitchPresenter.Value = default;
                    VisualStateManager.GoToState(this, nameof(AppBarNormal), false);
                    break;
                case ContextAppBarState.Main:
                    this.SwitchPresenter.Value = this.GetType(type);
                    VisualStateManager.GoToState(this, nameof(AppBarMain), false);
                    break;
                case ContextAppBarState.Preview:
                    this.SwitchPresenter.Value = this.GetType(type);
                    VisualStateManager.GoToState(this, nameof(AppBarPreview), false);
                    break;
                case ContextAppBarState.DragPreview:
                    this.SwitchPresenter.Value = this.GetType(type);
                    VisualStateManager.GoToState(this, nameof(AppBarDragPreview), false);
                    break;
                default:
                    break;
            }
        }

        private OptionType GetType(OptionType type)
        {
            if (type is OptionType.MarqueeTransform) return OptionType.Transform;
            else if (type.IsMarquee()) return OptionType.Marquee;
            else if (type.IsSelection()) return OptionType.Selection;
            else if (type.IsPaint()) return OptionType.Paint;
            else return type;
        }

        private ContextAppBarState GetState(OptionType type)
        {
            if (type == default) return default;

            if (type.HasPreview() is false)
                return ContextAppBarState.Main;

            return type.AllowDrag() ? ContextAppBarState.DragPreview : ContextAppBarState.Preview;
        }


        public void ConstructFoot()
        {
            this.AppBarSecondaryButton.Click += (s, e) =>
            {
                if (this.OptionType is OptionType.CropCanvas)
                {
                    this.CancelCropCanvas();
                }
                if (this.OptionType.IsGeometry())
                {
                    this.CancelGeometryTransform();
                }

                this.BitmapLayer = null;
                this.OptionType = this.ToolListView.SelectedItem;
                this.ConstructAppBar(this.ToolListView.SelectedItem);
                this.SetCanvasState(false);
            };

            this.AppBarPrimaryButton.Click += (s, e) =>
            {
                if (this.OptionType.HasPreview())
                {
                    if (this.OptionType is OptionType.CropCanvas)
                    {
                        this.PrimaryCropCanvas();
                    }
                    else if (this.OptionType.IsGeometry())
                    {
                        this.PrimaryGeometryTransform();
                    }
                    else if (this.OptionType.IsMarquees())
                    {
                        Color[] InterpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(InterpolationColors);
                        this.Primary(this.OptionType, mode, InterpolationColors, this.Marquee);
                    }
                    else
                    {
                        Color[] InterpolationColors = this.BitmapLayer.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.BitmapLayer.GetInterpolationBoundsMode(InterpolationColors);
                        this.Primary(this.OptionType, mode, InterpolationColors, this.BitmapLayer);
                    }
                }

                this.BitmapLayer = null;
                this.OptionType = this.ToolListView.SelectedItem;
                this.ConstructAppBar(this.ToolListView.SelectedItem);
                this.SetCanvasState(false);
            };
        }

        private void Primary(BitmapLayer bitmapLayer, ICanvasImage source)
        {
            Color[] InterpolationColors = this.Marquee.GetInterpolationColorsBySource();
            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(InterpolationColors);

            // History
            switch (mode)
            {
                case PixelBoundsMode.None:
                    bitmapLayer.Hit(InterpolationColors);

                    switch (this.SelectionType)
                    {
                        case SelectionType.MarqueePixelBounds:
                            bitmapLayer.DrawCopy(new PixelShaderEffect(this.LalphaMaskShaderCodeBytes)
                            {
                                Source1 = this.Marquee[BitmapType.Source],
                                Source2 = bitmapLayer[BitmapType.Origin],
                                Source3 = source
                            });
                            break;
                        default:
                            bitmapLayer.DrawCopy(source);
                            break;
                    }
                    int removes3 = this.History.Push(bitmapLayer.GetBitmapHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    break;
                default:
                    bitmapLayer.DrawCopy(source);
                    int removes2 = this.History.Push(bitmapLayer.GetBitmapResetHistory());
                    bitmapLayer.Flush();
                    bitmapLayer.RenderThumbnail();
                    break;
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

        private void Primary(OptionType type, PixelBoundsMode mode, Color[] InterpolationColors, BitmapLayer bitmapLayer)
        {
            if (type.HasDifference())
            {
                switch (this.SelectionType)
                {
                    case SelectionType.MarqueePixelBounds:
                        bitmapLayer.DrawCopy(new PixelShaderEffect(this.RalphaMaskShaderCodeBytes)
                        {
                            Source1 = this.Marquee[BitmapType.Source],
                            Source2 = bitmapLayer[BitmapType.Origin],
                        }, this.GetPreview(type, new AlphaMaskEffect
                        {
                            AlphaMask = this.Marquee[BitmapType.Source],
                            Source = bitmapLayer[BitmapType.Origin],
                        }));
                        break;
                    default:
                        bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer[BitmapType.Origin]));
                        break;
                }
                bitmapLayer.Hit(bitmapLayer.GetInterpolationColors(new PixelShaderEffect(this.DifferenceShaderCodeBytes)
                {
                    Source1 = bitmapLayer[BitmapType.Source],
                    Source2 = bitmapLayer[BitmapType.Origin]
                }));

                // History
                int removes = this.History.Push(bitmapLayer.GetBitmapHistory());
                bitmapLayer.Flush();
                bitmapLayer.RenderThumbnail();
            }
            else
            {
                // History
                switch (mode)
                {
                    case PixelBoundsMode.Solid:
                    case PixelBoundsMode.Transarent:
                        bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer[BitmapType.Origin]));
                        int removes2 = this.History.Push(bitmapLayer.GetBitmapResetHistory());
                        bitmapLayer.Flush();
                        bitmapLayer.RenderThumbnail();
                        break;
                    case PixelBoundsMode.None:
                        bitmapLayer.Hit(InterpolationColors);

                        switch (this.SelectionType)
                        {
                            case SelectionType.MarqueePixelBounds:
                                bitmapLayer.DrawCopy(new PixelShaderEffect(this.LalphaMaskShaderCodeBytes)
                                {
                                    Source1 = this.Marquee[BitmapType.Source],
                                    Source2 = bitmapLayer[BitmapType.Origin],
                                    Source3 = this.GetPreview(type, bitmapLayer[BitmapType.Origin])
                                });
                                break;
                            default:
                                bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer[BitmapType.Origin]));
                                break;
                        }
                        int removes3 = this.History.Push(bitmapLayer.GetBitmapHistory());
                        bitmapLayer.Flush();
                        bitmapLayer.RenderThumbnail();
                        break;
                }
            }

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

    }
}