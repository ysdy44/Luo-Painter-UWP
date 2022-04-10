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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    internal class GradientStopSelectorWithUI : GradientStopSelector
    {
        public Button CurrentButton { get; private set; }
        public GradientStop CurrentStop { get; private set; }
        public GradientStop CurrentStopUI { get; private set; }

        double StaringX;
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
                this.SetCurrent(s);
                this.StaringX = Canvas.GetLeft(this.CurrentButton);
            };
            base.ItemManipulationDelta += (s, e) =>
            {
                if (this.CurrentButton == null) return;
                if (this.CurrentStop == null) return;
                if (this.CurrentStopUI == null) return;

                double width = base.ActualWidth;
                double x = this.StaringX + e.Cumulative.Translation.X;
                double offset = Math.Clamp(x / width, 0, 1);

                this.CurrentStop.Offset = offset;
                this.CurrentStopUI.Offset = offset;

                Canvas.SetLeft(this.CurrentButton, base.GetItemLeft(this.CurrentStop));
                Canvas.SetTop(this.CurrentButton, base.GetItemTop(this.CurrentStop));
            };
            base.ItemManipulationCompleted += (s, e) =>
            {
                this.CurrentButton = null;
                this.CurrentStop = null;
                this.CurrentStopUI = null;
            };
        }

        public void Add(Color color, double offset)
        {
            this.Stops.Add(new GradientStop { Color = color, Offset = offset });
            this.StopsUI.Add(new GradientStop { Color = color, Offset = offset });
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

        public void SetCurrent(object sender)
        {
            this.CurrentButton = sender as Button;
            if (this.CurrentButton == null) return;

            this.CurrentStop = this.CurrentButton.Content as GradientStop;
            if (this.CurrentStop == null) return;

            foreach (GradientStop item in this.StopsUI)
            {
                if (item.Offset == this.CurrentStop.Offset && item.Color == this.CurrentStop.Color)
                {
                    this.CurrentStopUI = item;
                }
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

        readonly CanvasDevice Device = new CanvasDevice();
        CanvasBitmap CanvasBitmap;
        CanvasRenderTarget Map;

        byte[] ShaderCodeBytes;

        public GradientMappingPage()
        {
            this.InitializeComponent();
            this.ConstructSelector();
            this.ConstructCanvas();
        }

        private void ConstructSelector()
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
            this.Selector.ItemManipulationDelta += (s, e) => this.OriginCanvasControl.Invalidate(); // Invalidate

            this.ColorPicker.ColorChanged += (s, e) =>
            {
                this.Selector.SetCurrentColor(e.NewColor);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };

            this.AddButton.Click += (s, e) =>
            {
                this.Selector.Add(Colors.DodgerBlue, 0);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.RemoveButton.Click += (s, e) =>
            {
                this.Selector.RemoveCurrent();
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.ImageButton.Click += async (s, e) =>
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