using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI;
using Windows.UI.Xaml;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public void ConstructAppBar(OptionType type)
        {
            if (type.HasPreview())
            {
                this.TitleTextBlock.Text = App.Resource.GetString(type.ToString());
                VisualStateManager.GoToState(this, nameof(AppBarPreview), false);
            }
            else
            {
                this.TitleTextBlock.Text = string.Empty;
                VisualStateManager.GoToState(this, nameof(AppBarNormal), false);
            }

            switch (type)
            {
                case OptionType.MarqueeTransform:
                    this.SwitchPresenter.Value = OptionType.Transform;
                    break;

                case OptionType.PaintBrush:
                case OptionType.PaintLine:
                case OptionType.PaintBrushForce:
                    this.SwitchPresenter.Value = OptionType.Paint;
                    break;

                default:
                    if (type.IsMarquee()) this.SwitchPresenter.Value = OptionType.Marquee;
                    else if (type.IsGeometry()) this.SwitchPresenter.Value = type.ToGeometryTransform();
                    else if (type.IsPattern()) this.SwitchPresenter.Value = type.ToPatternTransform();
                    else this.SwitchPresenter.Value = type;
                    break;
            }

            this.AppBarGrid.Visibility = this.SwitchPresenter.Content is null ? Visibility.Collapsed : Visibility.Visible;

            switch (this.PaintScrollViewer.Visibility)
            {
                case Visibility.Visible:
                    if (type.IsPaint()) break;
                    this.PaintScrollViewer.Hide();
                    break;
                default:
                    break;
            }
        }


        public void ConstructAppBar()
        {
            this.AppBarBackButton.Click += (s, e) =>
            {
                if (this.OptionType is OptionType.CropCanvas)
                {
                    this.CancelCropCanvas();
                }
                else if (this.OptionType.IsGeometry())
                {
                    this.CancelGeometryTransform();
                }
                else if (this.OptionType.IsPattern())
                {
                    this.CancelPatternTransform();
                }

                this.Secondary();
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
                    else if (this.OptionType.IsPattern())
                    {
                        this.PrimaryPatternTransform();
                    }
                    else
                    {
                        Color[] InterpolationColors = this.Marquee.GetInterpolationColorsBySource();
                        PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(InterpolationColors);

                        if (this.OptionType.IsMarquees())
                        {
                            this.Primary(this.OptionType, mode, InterpolationColors, this.Marquee);
                        }
                        else
                        {
                            this.Primary(this.OptionType, mode, InterpolationColors, this.BitmapLayer);
                        }
                    }
                }
            };
        }

        private void Secondary()
        {
            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void Primary(OptionType type, BitmapLayer bitmapLayer, ICanvasImage source)
        {
            Color[] InterpolationColors = this.Marquee.GetInterpolationColorsBySource();
            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(InterpolationColors);

            // History
            switch (mode)
            {
                case PixelBoundsMode.None:
                    {
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

                        // History
                        IHistory history = bitmapLayer.GetBitmapHistory();
                        history.Title = App.Resource.GetString(type.ToString());
                        int removes = this.History.Push(history);

                        bitmapLayer.Flush();
                        bitmapLayer.RenderThumbnail();
                    }
                    break;
                default:
                    {
                        bitmapLayer.DrawCopy(source);

                        // History
                        IHistory history = bitmapLayer.GetBitmapResetHistory();
                        history.Title = App.Resource.GetString(type.ToString());
                        int removes = this.History.Push(history);

                        bitmapLayer.Flush();
                        bitmapLayer.RenderThumbnail();
                    }
                    break;
            }

            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

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
                IHistory history = bitmapLayer.GetBitmapHistory();
                history.Title = App.Resource.GetString(type.ToString());
                int removes = this.History.Push(history);

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
                        {
                            bitmapLayer.DrawCopy(this.GetPreview(type, bitmapLayer[BitmapType.Origin]));

                            // History
                            IHistory history = bitmapLayer.GetBitmapResetHistory();
                            history.Title = App.Resource.GetString(type.ToString());
                            int removes = this.History.Push(history);

                            bitmapLayer.Flush();
                            bitmapLayer.RenderThumbnail();
                        }
                        break;
                    case PixelBoundsMode.None:
                        {
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

                            // History
                            IHistory history = bitmapLayer.GetBitmapHistory();
                            history.Title = App.Resource.GetString(type.ToString());
                            int removes = this.History.Push(history);

                            bitmapLayer.Flush();
                            bitmapLayer.RenderThumbnail();
                        }
                        break;
                }
            }

            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

    }
}