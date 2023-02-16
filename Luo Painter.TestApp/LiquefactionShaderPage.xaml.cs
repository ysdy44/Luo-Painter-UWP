using Luo_Painter.Elements;
using Luo_Painter.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class LiquefactionShaderPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;

        Vector2 Position;

        float RangeSize = 50;
        float Pressure = 0.5f;
        byte[] ShaderCodeBytes;

        Historian<IHistory> History { get; } = new Historian<IHistory>(20);

        public LiquefactionShaderPage()
        {
            this.InitializeComponent();
            this.ConstructLiquefactionShader();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructLiquefactionShader()
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
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.PressureSlider.ValueChanged += (s, e) => this.Pressure = (float)(e.NewValue / 100f);
            this.RangeSizeSlider.ValueChanged += (s, e) => this.RangeSize = (float)e.NewValue;
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(this.Device, 512, 512);
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, 512, 512, Colors.White);

                args.DrawingSession.DrawImage(new CropEffect
                {
                    SourceRectangle = new Rect(0, 0, 512, 512),
                    Source = this.BitmapLayer[BitmapType.Source]
                });
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer[BitmapType.Origin],
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };

            this.SourceCanvasControl.UseSharedDevice = true;
            this.SourceCanvasControl.CustomDevice = this.Device;
            this.SourceCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer[BitmapType.Source],
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };

            this.TempCanvasControl.UseSharedDevice = true;
            this.TempCanvasControl.CustomDevice = this.Device;
            this.TempCanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.Clear(Colors.White);
                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Source = this.BitmapLayer[BitmapType.Temp],
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.ShaderCodeBytes = await ShaderType.Liquefaction.LoadAsync();
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Render(this.Position, position);
                this.Position = position;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                // History
                this.History.Push(this.BitmapLayer.GetBitmapHistory());
                this.BitmapLayer.Flush();
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
                this.BitmapLayer.RenderThumbnail();

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
        }


        private void Render(Vector2 position, Vector2 targetPosition)
        {
            if (this.BitmapLayer is null) return;
            if (this.ShaderCodeBytes is null) return;

            // 1. Region
            Rect rect = RectExtensions.GetRect(position, targetPosition, this.RangeSize);

            // 2. Shader
            PixelShaderEffect shader = new PixelShaderEffect(this.ShaderCodeBytes)
            {
                Source1BorderMode = EffectBorderMode.Hard,
                Source1 = this.BitmapLayer[BitmapType.Source],
                Properties =
                {
                    ["radius"] = this.BitmapLayer.ConvertValueToOne(this.RangeSize),
                    ["position"] = this.BitmapLayer.ConvertValueToOne(position),
                    ["targetPosition"] = this.BitmapLayer.ConvertValueToOne(targetPosition),
                    ["pressure"] = this.Pressure,
                }
            };

            // 3. Render
            this.BitmapLayer.Hit(rect);
            this.BitmapLayer.Shade(shader, rect);
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
                    this.BitmapLayer = new BitmapLayer(this.CanvasControl, bitmap, 512, 512);
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