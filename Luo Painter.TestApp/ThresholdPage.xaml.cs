using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class ThresholdPage : Page
    {

        readonly CanvasDevice Device = new CanvasDevice();
        CanvasBitmap CanvasBitmap;
        byte[] ThresholdShaderCodeBytes;

        float Threshold = 1.5f;
        Vector4 Color0 = Vector4.One; // White
        Vector4 Color1 = Vector4.UnitW; // Black

        public ThresholdPage()
        {
            this.InitializeComponent();
            this.ConstructThreshold();
            this.ConstructCanvas();
        }

        private void ConstructThreshold()
        {
            this.Slider.ValueChanged += (s, e) =>
            {
                this.Threshold = (float)(e.NewValue / 100);
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
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

            this.ColorPicker0.ColorChanged += (s, e) =>
            {
                Color c = e.NewColor;
                this.Color0 = new Vector4(c.R, c.G, c.B, c.A) / 255f;
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.ColorPicker1.ColorChanged += (s, e) =>
            {
                Color c = e.NewColor;
                this.Color1 = new Vector4(c.R, c.G, c.B, c.A) / 255f;
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (s, e) =>
            {
                e.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap is null) return;

                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                args.DrawingSession.DrawImage(this.CanvasBitmap);
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap is null) return;

                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                args.DrawingSession.DrawImage(new PixelShaderEffect(this.ThresholdShaderCodeBytes)
                {
                    Source1 = this.CanvasBitmap,
                    Properties =
                    {
                        ["threshold"] = this.Threshold,
                        ["color0"] = this.Color0,
                        ["color1"] = this.Color1,
                    }
                });
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.ThresholdShaderCodeBytes = await ShaderType.Threshold.LoadAsync();
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
                    this.CanvasBitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
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