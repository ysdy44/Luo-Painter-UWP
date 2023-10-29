using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    /// <summary>
    /// https://github.com/microsoft/Windows-universal-samples/tree/main/Samples/D2DCustomEffects
    /// </summary>
    public sealed partial class RippleEffectPage : Page
    {

        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;
        byte[] RippleEffectShaderCodeBytes;
        byte[] DifferenceShaderCodeBytes;

        Rippler Rippler = Rippler.Zero;
        Vector2 Center;

        readonly CanvasTextFormat TextFormat = new CanvasTextFormat
        {
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center,
            FontWeight = FontWeights.Bold
        };

        public RippleEffectPage()
        {
            this.InitializeComponent();
            this.ConstructRippleEffect();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructRippleEffect()
        {
            this.FrequencyRun.Text = this.Rippler.Frequency.ToString();
            this.PhaseRun.Text = this.Rippler.Phase.ToString();
            this.AmplitudeRun.Text = this.Rippler.Amplitude.ToString();
            this.SpreadRun.Text = this.Rippler.Spread.ToString();
            this.Slider.ValueChanged += (s, e) =>
            {
                this.Rippler = new Rippler((float)(e.NewValue / 100));

                this.FrequencyRun.Text = this.Rippler.Frequency.ToString();
                this.PhaseRun.Text = this.Rippler.Phase.ToString();
                this.AmplitudeRun.Text = this.Rippler.Amplitude.ToString();
                this.SpreadRun.Text = this.Rippler.Spread.ToString();

                this.Update();
            };
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return; 
                if (result is false) return;

                this.Update();
            };

            this.FrequencySlider.Value = this.Rippler.Frequency;
            this.FrequencySlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Frequency = (float)(e.NewValue);
                this.FrequencyRun.Text = this.Rippler.Frequency.ToString();
                this.Update();
            };
            this.PhaseSlider.Value = this.Rippler.Phase;
            this.PhaseSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Phase = (float)(e.NewValue);
                this.PhaseRun.Text = this.Rippler.Phase.ToString();
                this.Update();
            };
            this.AmplitudeSlider.Value = this.Rippler.Amplitude;
            this.AmplitudeSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Amplitude = (float)(e.NewValue);
                this.AmplitudeRun.Text = this.Rippler.Amplitude.ToString();
                this.Update();
            };
            this.SpreadSlider.Value = this.Rippler.Spread * 100;
            this.SpreadSlider.ValueChanged += (s, e) =>
            {
                this.Rippler.Spread = (float)(e.NewValue / 100);
                this.SpreadRun.Text = this.Rippler.Spread.ToString();
                this.Update();
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
                if (this.BitmapLayer is null) return;

                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                args.DrawingSession.DrawImage(this.BitmapLayer[BitmapType.Source]);
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                if (this.BitmapLayer is null) return;

                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                args.DrawingSession.DrawImage(new PixelShaderEffect(this.DifferenceShaderCodeBytes)
                {
                    Source1 = this.BitmapLayer[BitmapType.Origin],
                    Source2 = this.BitmapLayer[BitmapType.Source],
                });

                this.BitmapLayer.DrawHits(args.DrawingSession, Colors.Red, this.TextFormat, (i) => i.ToString());

                args.DrawingSession.Units = CanvasUnits.Dips; /// <see cref="DPIExtensions">
                Vector2 center = sender.Dpi.ConvertPixelsToDips(this.Center);
                args.DrawingSession.DrawCircle(center, 12, Windows.UI.Colors.Gray);
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.RippleEffectShaderCodeBytes = await new ShaderUri(ShaderType.RippleEffect).LoadAsync();
            this.DifferenceShaderCodeBytes = await new ShaderUri(ShaderType.Difference).LoadAsync();
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.Center = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Update();
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                this.Center = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Update();
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.Center = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Update();
            };
        }

        private void Update()
        {
            if (this.BitmapLayer is null) return;

            this.BitmapLayer.DrawCopy(new PixelShaderEffect(this.RippleEffectShaderCodeBytes)
            {
                Source1 = this.BitmapLayer[BitmapType.Origin],
                Properties =
                {
                    ["frequency"] = this.Rippler.Frequency,
                    ["phase"] = this.Rippler.Phase,
                    ["amplitude"] = this.Rippler.Amplitude,
                    ["spread"] = this.Rippler.Spread,
                    ["center"] = this.Center,
                    ["dpi"] = 96f, // Default value 96f,
                },
            });

            this.BitmapLayer.Hit(this.BitmapLayer.GetInterpolationColors(new PixelShaderEffect(this.DifferenceShaderCodeBytes)
            {
                Source1 = this.BitmapLayer[BitmapType.Source],
                Source2 = this.BitmapLayer[BitmapType.Origin]
            }));

            this.CanvasControl.Invalidate(); // Invalidate
            this.OriginCanvasControl.Invalidate(); // Invalidate
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
                using (CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream))
                {
                    this.BitmapLayer = new BitmapLayer(this.CanvasControl, bitmap);
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