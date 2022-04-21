using Luo_Painter.Elements;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    /// <summary>
    /// https://github.com/microsoft/Windows-universal-samples/tree/main/Samples/D2DCustomEffects
    /// </summary>
    public sealed partial class RippleEffectPage : Page
    {

        CanvasBitmap CanvasBitmap;
        byte[] ShaderCodeBytes;

        float Frequency = 50;
        float Phase = 50;
        float Amplitude = 180;
        float Spread = 0.5f;
        Vector2 Center;

        public RippleEffectPage()
        {
            this.InitializeComponent();
            this.ConstructRippleEffect();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructRippleEffect()
        {
            this.Slider.ValueChanged += (s, e) => this.Update((float)(e.NewValue / 100));
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                this.CanvasControl.Invalidate(); // Invalidate
                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.FrequencySlider.Value = this.Frequency;
            this.FrequencySlider.ValueChanged += (s, e) =>
            {
                this.Frequency = (float)(e.NewValue);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.PhaseSlider.Value = this.Phase;
            this.PhaseSlider.ValueChanged += (s, e) =>
            {
                this.Phase = (float)(e.NewValue);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.AmplitudeSlider.Value = this.Amplitude;
            this.AmplitudeSlider.ValueChanged += (s, e) =>
            {
                this.Amplitude = (float)(e.NewValue);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.SpreadSlider.Value = this.Spread;
            this.SpreadSlider.ValueChanged += (s, e) =>
            {
                this.Spread = (float)(e.NewValue / 100);
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (s, e) =>
            {
                e.TrackAsyncAction(CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.CanvasBitmap is null) return;

                args.DrawingSession.DrawImage(new PixelShaderEffect(this.ShaderCodeBytes)
                {
                    Source1 = this.CanvasBitmap,
                    Properties =
                    {
                        ["frequency"] = this.Frequency,
                        ["phase"] = this.Phase,
                        ["amplitude"] = this.Amplitude,
                        ["spread"] = this.Spread,
                        ["center"] = this.Center,
                        ["dpi"] = sender.Dpi, // Default value 96f,
                    },
                });

                Vector2 center = sender.Dpi.ConvertPixelsToDips(this.Center);
                args.DrawingSession.DrawCircle(center, 12, Windows.UI.Colors.Gray);

                float spread = this.Spread * Math.Max(this.CanvasBitmap.SizeInPixels.Width, this.CanvasBitmap.SizeInPixels.Height);
                float radius = sender.Dpi.ConvertPixelsToDips(spread);
                args.DrawingSession.DrawCircle(center, radius, Windows.UI.Colors.Gray);
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.ShaderCodeBytes = await ShaderType.RippleEffect.LoadAsync();
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Center = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                this.Center = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.Center = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        /// <summary>
        /// https://github.com/microsoft/Windows-universal-samples/blob/main/Samples/D2DCustomEffects/cpp/PixelShader/CustomPixelShaderRenderer.cpp
        /// </summary>
        private void Update(float timer)
        {
            float delta = timer;

            // Stop animating after four seconds.
            if (delta >= 4) return;

            // Increase the spread over time to make the visible area of the waves spread out.
            this.Spread = 0.01f + delta / 10.0f;

            // Reduce the amplitude over time to make the waves decay in intensity.
            this.Amplitude = 60.0f - delta * 15.0f;

            // Reduce the frequency over time to make each individual wave spread out.
            this.Frequency = 140.0f - delta * 30.0f;

            // Change the phase over time to make each individual wave travel away from the center.
            this.Phase = -delta * 20.0f;

            this.CanvasControl.Invalidate(); // Invalidate
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