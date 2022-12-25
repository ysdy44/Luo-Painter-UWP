using Luo_Painter.HSVColorPickers;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.TestApp
{
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
        CanvasRenderTarget GradientMesh;
        byte[] ShaderCodeBytes;

        public GradientMappingPage()
        {
            this.InitializeComponent();
            this.ConstructGradientMapping();
            this.ConstructSelectorItems();
            this.ConstructSelector();
            this.ConstructCanvas();
            base.Loaded += (s, e) => this.Selector.Reset(this.Stops);
        }

        private void ConstructGradientMapping()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return;
                if (result is false) return;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };

            this.ResetButton.Click += (s, e) =>
            {
                this.Stops.Random();

                this.Selector.Reset(this.Stops);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.ReverseButton.Click += (s, e) =>
            {
                bool result = this.Stops.Reverse(this.Selector.Source);
                if (result is false) return;

                this.Selector.Reset(this.Stops);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.SpaceButton.Click += (s, e) =>
            {
                bool result = this.Stops.Space(this.Selector.Source);
                if (result is false) return;

                this.Selector.Reset(this.Stops);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };

            this.ColorPicker.ColorChanged += (s, e) =>
            {
                if (this.Stops.Enabled) return;

                this.Selector.SetCurrentColor(e.NewColor);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructSelectorItems()
        {
            this.Selector.ItemClick += (s, e) =>
            {
                if (this.Selector.IsItemClickEnabled)
                {
                    if (this.Stops.Enabled) return;
                    this.Stops.Start();

                    this.Selector.SetCurrent(s);
                    this.ColorPicker.Color = this.Selector.Color;
                    this.ColorFlyout.ShowAt(this.Selector.PlacementTarget);
                }
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
                this.Stops.Start();
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
                if (result is false) return;

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
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.GradientMesh = new CanvasRenderTarget(sender, 256, 1, 96);

                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap is null) return;

                args.DrawingSession.DrawImage(this.CanvasBitmap);
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap is null) return;

                this.GradientMapping(sender);

                args.DrawingSession.DrawImage(new PixelShaderEffect(this.ShaderCodeBytes)
                {
                    Source2BorderMode = EffectBorderMode.Hard,
                    Source1 = this.CanvasBitmap,
                    Source2 = this.GradientMesh
                });
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.ShaderCodeBytes = await ShaderType.GradientMapping.LoadAsync();
        }

        private void GradientMapping(ICanvasResourceCreator resourceCreator)
        {
            using (CanvasLinearGradientBrush brush = new CanvasLinearGradientBrush(resourceCreator, this.Selector.Data)
            {
                StartPoint = Vector2.Zero,
                EndPoint = new Vector2(256, 0),
            })
            using (CanvasDrawingSession ds = this.GradientMesh.CreateDrawingSession())
            {
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.FillRectangle(0, 0, 256, 1, brush);
            }
        }

        public async Task<StorageFile> PickSingleImageFileAsync(PickerLocationId location)
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