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
using Windows.Graphics.Effects;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    internal sealed class GradientMesh
    {
        readonly int Length;
        readonly CanvasRenderTarget Map;
        public IGraphicsEffectSource Source => this.Map;

        public GradientMesh(ICanvasResourceCreator resourceCreator, int length = 256)
        {
            this.Length = length;
            this.Map = new CanvasRenderTarget(resourceCreator, length, 1, 96);
        }

        public void Render(ICanvasResourceCreator resourceCreator, IEnumerable<GradientStop> stops)
        {
            IEnumerable<CanvasGradientStop> array =
                from item
                in stops
                select new CanvasGradientStop
                {
                    Position = (float)item.Offset,
                    Color = item.Color,
                };

            CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(resourceCreator, array.ToArray())
            {
                StartPoint = Vector2.Zero,
                EndPoint = new Vector2(this.Length, 0),
            };

            using (CanvasDrawingSession ds = this.Map.CreateDrawingSession())
            {
                ds.Units = CanvasUnits.Pixels;

                ds.FillRectangle(0, 0, this.Length, 1, brush);
            }
        }
    }

    internal sealed class GradientStops : Dictionary<double, Color>
    {
        readonly Random Ran = new Random();
        public void Random()
        {
            int count = this.Ran.Next(3, 10);
            float space = 1f / (count - 1);

            base.Clear();
            for (int i = 0; i < count; i++)
            {
                base.Add(i * space, new Color
                {
                    A = 255,
                    R = (byte)this.Ran.Next(0, 256),
                    G = (byte)this.Ran.Next(0, 256),
                    B = (byte)this.Ran.Next(0, 256),
                });
            }
        }
    }

    public sealed partial class GradientMappingPage : Page
    {

        //@Converter
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        readonly CanvasDevice Device = new CanvasDevice();
        readonly GradientStops Stops = new GradientStops
        {
            [0] = Colors.LightBlue,
            [0.3333] = Colors.LightSteelBlue,
            [0.6666] = Colors.LightGoldenrodYellow,
            [1] = Colors.PaleVioletRed,
        };

        CanvasBitmap CanvasBitmap;
        GradientMesh GradientMesh;
        byte[] ShaderCodeBytes;

        public GradientMappingPage()
        {
            this.InitializeComponent();
            this.ConstructSelectorItems();
            this.ConstructSelector();
            this.ConstructCanvas();
            base.Loaded += (s, e) => this.Selector.Reset(this.Stops);
        }

        private void ConstructSelectorItems()
        {
            this.Selector.ItemClick += (s, e) =>
            {
                this.Selector.SetCurrent(s);
                if (this.Selector.CurrentStop == null) return;

                this.ColorPicker.Color = this.Selector.CurrentStop.Color;
                this.ColorFlyout.ShowAt(this.Selector.CurrentButton);
            };

            this.Selector.ItemManipulationStarted += (s, e) =>
            {
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.Selector.ItemManipulationDelta += (s, e) =>
            {
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.Selector.ItemManipulationCompleted += (s, e) =>
            {
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.Selector.ItemPreviewKeyDown += (s, e) =>
            {
                if (e.Handled)
                {
                    this.OriginCanvasControl.Invalidate(); // Invalidate
                }
            };
        }

        private void ConstructSelector()
        {
            this.Selector.ManipulationMode = ManipulationModes.TranslateX;
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
            this.ResetButton.Click += (s, e) =>
            {
                this.Stops.Random();
                this.Selector.Reset(this.Stops);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.GradientMesh = new GradientMesh(sender);

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

                this.GradientMesh.Render(sender, this.Selector.Source);

                args.DrawingSession.DrawImage(new PixelShaderEffect(this.ShaderCodeBytes)
                {
                    Source2BorderMode = EffectBorderMode.Hard,
                    Source1 = this.CanvasBitmap,
                    Source2 = this.GradientMesh.Source
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
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                using (CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream))
                {
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