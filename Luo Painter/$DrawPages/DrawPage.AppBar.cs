using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Luo_Painter.UI;
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
                this.TitleTextBlock.Text = type.GetString();
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
                case OptionType.AddImageTransform:
                case OptionType.Transform:
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
                switch (this.OptionTarget)
                {
                    case OptionTarget.Marquee:
                        {
                            Color[] InterpolationColors = this.Marquee.GetInterpolationColorsBySource();
                            PixelBoundsMode mode = this.Marquee.GetInterpolationBoundsMode(InterpolationColors);

                            this.Primary(this.OptionType, mode, InterpolationColors, this.Marquee);
                        }
                        break;
                    case OptionTarget.Image:
                        this.PrimaryAddImageTransform(this.AddImage);
                        break;
                    default:
                        if (this.OptionType.HasPreview() || this.OptionType == OptionType.AddImageLayer)
                        {
                            switch (this.OptionType)
                            {
                                case OptionType.CropCanvas:
                                    this.PrimaryCropCanvas();
                                    break;
                                default:
                                    if (this.OptionType.IsGeometry())
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

                                        this.Primary(this.OptionType, mode, InterpolationColors, this.BitmapLayer);
                                    }
                                    break;
                            }
                        }
                        break;
                }
            };
        }

        private void Secondary()
        {
            this.AddImageId = null;
            this.AddImage?.Dispose();
            this.AddImage = null;
            this.ImageLayer = null;
            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void PrimaryGrayOrInvert(OptionType type, BitmapLayer bitmapLayer, ICanvasImage source)
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
                        history.Title = type.GetString();
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
                        history.Title = type.GetString();
                        int removes = this.History.Push(history);

                        bitmapLayer.Flush();
                        bitmapLayer.RenderThumbnail();
                    }
                    break;
            }

            this.AddImageId = null;
            this.AddImage?.Dispose();
            this.AddImage = null;
            this.ImageLayer = null;
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
                history.Title = type.GetString();
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
                            history.Title = type.GetString();
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
                            history.Title = type.GetString();
                            int removes = this.History.Push(history);

                            bitmapLayer.Flush();
                            bitmapLayer.RenderThumbnail();
                        }
                        break;
                }
            }

            this.AddImageId = null;
            this.AddImage?.Dispose();
            this.AddImage = null;
            this.BitmapLayer = null;
            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasAnimatedControl.Invalidate(false); // Invalidate
            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

        private void PrimaryAddImageTransform(CanvasBitmap bitmap)
        {
            Layerage[] undo = this.Nodes.Convert();

            int count = 0;
            int index = this.LayerSelectedIndex;
            if (index > 0 && this.LayerSelectedItem is ILayer neighbor)
            {
                ILayer parent = this.ObservableCollection.GetParent(neighbor);
                if (parent is null)
                {
                    int indexChild = this.Nodes.IndexOf(neighbor);

                    BitmapLayer add = new BitmapLayer(this.CanvasDevice,
                        this.GetPreview(OptionType.AddImageTransform, bitmap),
                        this.Transformer.Width, this.Transformer.Height);

                    this.Nodes.Insert(indexChild, add);
                    this.ObservableCollection.InsertChild(index, add);
                    count++;
                }
                else
                {
                    int indexChild = parent.Children.IndexOf(neighbor);

                    BitmapLayer add = new BitmapLayer(this.CanvasDevice,
                        this.GetPreview(OptionType.AddImageTransform, bitmap),
                        this.Transformer.Width, this.Transformer.Height);

                    parent.Children.Insert(indexChild, add);
                    this.ObservableCollection.InsertChild(index, add);
                    count++;
                }

                this.LayerSelectedIndex = index;
            }
            else
            {
                {
                    BitmapLayer add = new BitmapLayer(this.CanvasDevice,
                        this.GetPreview(OptionType.AddImageTransform, bitmap),
                        this.Transformer.Width, this.Transformer.Height);

                    this.Nodes.Insert(0, add);
                    this.ObservableCollection.InsertChild(0, add);
                    count++;
                }

                this.LayerSelectedIndex = 0;
            }

            this.AddImageId = null;
            this.AddImage?.Dispose();
            this.AddImage = null;
            this.BitmapLayer = null;

            // History
            Layerage[] redo = this.Nodes.Convert();
            IHistory history = new ArrangeHistory(undo, redo);
            history.Title = OptionType.AddImageLayer.GetString();
            int removes = this.History.Push(history);

            this.OptionType = this.ToolListView.SelectedType;
            this.ConstructAppBar(this.OptionType);

            this.CanvasVirtualControl.Invalidate(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            this.RaiseHistoryCanExecuteChanged();
        }

    }
}