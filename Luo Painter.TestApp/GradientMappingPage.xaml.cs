using Luo_Painter.Elements;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    internal class GradientStopSelectorWithUI : GradientStopSelector
    {
        public Button CurrentButton { get; private set; }
        public GradientStop CurrentStop { get; private set; }
        public GradientStop CurrentStopUI { get; private set; }

        readonly ObservableCollection<GradientStop> Stops = new ObservableCollection<GradientStop>();
        readonly GradientStopCollection StopsUI = new GradientStopCollection();

        public GradientStopSelectorWithUI()
        {
            base.ItemSource = this.Stops;
            base.Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops = this.StopsUI
            };
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                foreach (var item in base.Items)
                {
                    Canvas.SetLeft(item.Value, base.GetItemLeft(item.Key));
                    Canvas.SetTop(item.Value, base.GetItemTop(item.Key));
                }
            };


            base.ItemManipulationStarted += (s, e) =>
            {
                UIElement button = e.Container;

                button.RenderTransformOrigin = new Point
                {
                    X = Canvas.GetLeft(button) + e.Cumulative.Translation.X + base.Margin.Left,
                    Y = Canvas.GetTop(button) + e.Cumulative.Translation.Y + base.Margin.Top,
                };

                int index = -1;
                foreach (var item in base.Items)
                {
                    index++;
                    if (item.Value == button)
                    {
                        Canvas.SetZIndex(button, index);
                        break;
                    }
                }
            };
            base.ItemManipulationDelta += (s, e) =>
            {
                UIElement button = e.Container;

                // Remove
                if (base.Count > 2)
                {
                    double staringY = button.RenderTransformOrigin.Y;
                    double y = staringY + e.Cumulative.Translation.Y;
                    bool isRemove = y < -50;

                    if (isRemove)
                    {
                        Canvas.SetTop(button, y - base.Margin.Top);
                        base.IsHitTestVisible = false;
                        return;
                    }
                    else
                    {
                        Canvas.SetTop(button, -base.Margin.Top);
                        base.IsHitTestVisible = true;
                    }
                }

                double staringX = button.RenderTransformOrigin.X;
                double x = staringX + e.Cumulative.Translation.X;
                double width = base.ActualWidth;
                double offsetX = Math.Clamp(x / width, 0, 1);
                Canvas.SetLeft(button, offsetX * width - base.Margin.Left);

                int index = Canvas.GetZIndex(button);
                this.Stops[index].Offset = offsetX;
                this.StopsUI[index].Offset = offsetX;
            };
            base.ItemManipulationCompleted += (s, e) =>
            {
                UIElement button = e.Container;

                // Remove
                base.IsHitTestVisible = true;
                if (base.Count > 2)
                {
                    double staringY = button.RenderTransformOrigin.Y;
                    double y = staringY + e.Cumulative.Translation.Y;
                    double height = base.ActualHeight;
                    bool isRemove = y < -height;

                    if (isRemove)
                    {
                        this.SetCurrent(button);
                        this.RemoveCurrent();
                    }
                }

                button.RenderTransformOrigin = new Point();
                Canvas.SetZIndex(button, 0);
            };
        }

        public void Add(Color color, double offset)
        {
            this.Stops.Add(new GradientStop { Color = color, Offset = offset });
            this.StopsUI.Add(new GradientStop { Color = color, Offset = offset });
        }
        public bool Interpolation(Point point)
        {
            double x = point.X;
            double width = base.ActualWidth;
            double offsetX = Math.Clamp(x / width, 0, 1); // 0.5

            //double y = point.Y;
            //double height = base.ActualHeight;
            //double offsetY = Math.Clamp(y / height, 0, 1); // 0.5

            GradientStop left = null; // 0
            GradientStop right = null; // 1
            foreach (GradientStop item in this.Stops)
            {
                double offset = item.Offset;
                if (offset < offsetX)
                {
                    if (left is null) left = item;
                    else if (item.Offset < left.Offset) continue;
                    else left = item;
                }
                else if (offset > offsetX)
                {
                    if (right is null) right = item;
                    else if (item.Offset > left.Offset) continue;
                    else right = item;
                }
                else return false;
            }

            if (left is null) return false;
            if (right is null) return false;

            double length = Math.Abs(right.Offset - left.Offset);
            double min = Math.Abs(left.Offset - offsetX) / length;
            double max = Math.Abs(right.Offset - offsetX) / length;
            Color color = new Color
            {
                A = 255,
                R = (byte)(left.Color.R * min + right.Color.R * max),
                G = (byte)(left.Color.G * min + right.Color.G * max),
                B = (byte)(left.Color.B * min + right.Color.B * max),
            };

            this.Stops.Add(new GradientStop { Color = color, Offset = offsetX });
            this.StopsUI.Add(new GradientStop { Color = color, Offset = offsetX });
            return true;
        }
        public void Clear()
        {
            this.Stops.Clear();
            this.StopsUI.Clear();
        }

        public void RemoveCurrent()
        {
            this.CurrentButton = null;

            if (this.CurrentStop != null)
            {
                this.Stops.Remove(this.CurrentStop);
                this.CurrentStop = null;
            }

            if (this.CurrentStopUI != null)
            {
                this.StopsUI.Remove(this.CurrentStopUI);
                this.CurrentStopUI = null;
            }
        }

        public void SetCurrentColor(Color color)
        {
            if (this.CurrentStop == null) return;
            this.CurrentStop.Color = color;

            if (this.CurrentStopUI == null) return;
            this.CurrentStopUI.Color = color;
        }
        public void SetCurrentOffset(Point point)
        {
            double x = point.X;
            double width = base.ActualWidth;
            double offsetX = Math.Clamp(x / width, 0, 1); // 0.5

            if (this.CurrentButton == null) return;
            Canvas.SetLeft(this.CurrentButton, offsetX * width - base.Margin.Left);

            if (this.CurrentStop == null) return;
            this.CurrentStop.Offset = offsetX;

            if (this.CurrentStopUI == null) return;
            this.CurrentStopUI.Offset = offsetX;
        }

        public void SetCurrent(Point point, double distance = 20)
        {
            double width = base.ActualWidth;

            this.CurrentButton = null;
            this.CurrentStop = null;
            foreach (var item in base.Items)
            {
                double offset = item.Key.Offset;
                double x = offset * width;

                if (Math.Abs(x - point.X) < distance)
                {
                    this.CurrentButton = item.Value;
                    this.CurrentStop = item.Key;
                }
            }
            if (this.CurrentStop == null) return;

            foreach (GradientStop item in this.StopsUI)
            {
                if (item.Offset == this.CurrentStop.Offset)
                    this.CurrentStopUI = item;
            }
        }
        public void SetCurrent(object sender)
        {
            this.CurrentButton = sender as Button;
            if (this.CurrentButton == null) return;

            this.CurrentStop = this.CurrentButton.Content as GradientStop;
            if (this.CurrentStop == null) return;

            foreach (GradientStop item in this.StopsUI)
            {
                if (item.Offset == this.CurrentStop.Offset)
                    this.CurrentStopUI = item;
            }
        }

        public IEnumerable<CanvasGradientStop> GetStops()
        {
            foreach (GradientStop item in this.Stops)
            {
                yield return new CanvasGradientStop
                {
                    Position = (float)item.Offset,
                    Color = item.Color,
                };
            }
        }

    }

    public sealed partial class GradientMappingPage : Page
    {

        //@Converter
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        readonly CanvasDevice Device = new CanvasDevice();
        CanvasBitmap CanvasBitmap;
        CanvasRenderTarget Map;

        byte[] ShaderCodeBytes;

        public GradientMappingPage()
        {
            this.InitializeComponent();
            this.ConstructSelectorItems();
            this.ConstructSelector();
            this.ConstructCanvas();
        }

        private void ConstructSelectorItems()
        {
            base.Loaded += (s, e) =>
            {
                this.Selector.Add(Colors.LightBlue, 0);
                this.Selector.Add(Colors.LightSteelBlue, 0.3333);
                this.Selector.Add(Colors.LightGoldenrodYellow, 0.6666);
                this.Selector.Add(Colors.PaleVioletRed, 1);
            };

            this.Selector.ItemClick += (s, e) =>
            {
                this.Selector.SetCurrent(s);
                if (this.Selector.CurrentStop == null) return;

                this.ColorPicker.Color = this.Selector.CurrentStop.Color;
                this.ColorFlyout.ShowAt(this.Selector.CurrentButton);
            };

            this.Selector.ItemManipulationStarted += (s, e) =>
            {
                e.Handled = true; // ManipulationStarted
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.Selector.ItemManipulationDelta += (s, e) =>
            {
                e.Handled = true; // ManipulationDelta
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.Selector.ItemManipulationCompleted += (s, e) =>
            {
                e.Handled = true; // ManipulationCompleted
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructSelector()
        {
            this.Selector.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX;
            this.Selector.ManipulationStarted += (s, e) =>
            {
                Point point = e.Position;
                bool result = this.Selector.Interpolation(point);
                if (result == false) return;

                this.Selector.SetCurrent(point);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.Selector.ManipulationDelta += (s, e) =>
            {
                this.Selector.SetCurrentOffset(e.Position);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.Selector.ManipulationCompleted += (s, e) =>
            {
                this.Selector.SetCurrentOffset(e.Position);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };

            this.ColorPicker.ColorChanged += (s, e) =>
            {
                this.Selector.SetCurrentColor(e.NewColor);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };

            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Map = new CanvasRenderTarget(sender, 256, 1, 96);

                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap == null) return;

                args.DrawingSession.DrawImage(this.CanvasBitmap);
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap == null) return;

                var array = this.Selector.GetStops();
                CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(this.CanvasControl, array.ToArray())
                {
                    StartPoint = new Vector2(0, 0),
                    EndPoint = new Vector2(256, 0),
                };

                using (CanvasDrawingSession ds = this.Map.CreateDrawingSession())
                {
                    ds.FillRectangle(0, 0, 256, 1, brush);
                }

                args.DrawingSession.DrawImage(new PixelShaderEffect(this.ShaderCodeBytes)
                {
                    Source2BorderMode = EffectBorderMode.Hard,
                    Source1 = this.CanvasBitmap,
                    Source2 = this.Map
                });
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.ShaderCodeBytes = await ShaderType.GradientMapping.LoadAsync();
        }

        public async static Task<StorageFile> PickSingleImageFileAsync(PickerLocationId location)
        {
            // Picker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = location,
                FileTypeFilter =
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".bmp"
                }
            };

            // File
            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
        }

        public async Task<bool?> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference == null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                    this.CanvasBitmap = bitmap;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}