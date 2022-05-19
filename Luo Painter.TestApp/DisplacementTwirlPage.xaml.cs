using Luo_Painter.Elements;
using Luo_Painter.Historys;
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
    public sealed partial class DisplacementTwirlPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;
        CanvasBitmap CanvasBitmap;

        float Amount = 512;
        float RangeSize = 50;
        bool IsExpand;
        byte[] ShaderCodeBytesTwirlLeft;
        byte[] ShaderCodeBytesTwirlRight;

        Historian<IHistory> History { get; } = new Historian<IHistory>(20);

        public DisplacementTwirlPage()
        {
            this.InitializeComponent();
            this.ConstructDisplacementTwirl();
            this.ConstructCanvas();
            this.ConstructOperator();
        }


        private void ConstructDisplacementTwirl()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Origin);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Source);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };

            this.Toggle(this.ToggleButton.IsChecked is true);
            this.ToggleButton.Unchecked += (s, e) => this.Toggle(false);
            this.ToggleButton.Checked += (s, e) => this.Toggle(true);

            this.AmountSlider.ValueChanged += (s, e) =>
            {
                this.Amount = (float)e.NewValue;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.RangeSizeSlider.ValueChanged += (s, e) => this.RangeSize = (float)e.NewValue;
            this.ClearButton.Click += (s, e) =>
            {
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Origin);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Source);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void Toggle(bool value)
        {
            this.IsExpand = value;
            this.ScaleTransform.ScaleX = value ? 1 : -1;
            this.ToggleButton.Label = value ? "TwirlLeft" : "TwirlRight";
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(this.Device, 512, 512);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Origin);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Source);
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);
                args.TrackAsyncAction(this.CreateResourcesAsync().AsAsyncAction());
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, 512, 512, Colors.White);

                if (this.CanvasBitmap is null) return;

                args.DrawingSession.DrawImage(new CropEffect
                {
                    SourceRectangle = this.BitmapLayer.Bounds.ToRect(),
                    Source = new DisplacementMapEffect
                    {
                        Amount = this.Amount,
                        XChannelSelect = EffectChannelSelect.Red,
                        YChannelSelect = EffectChannelSelect.Green,
                        Source = this.CanvasBitmap,
                        Displacement = new GaussianBlurEffect
                        {
                            BorderMode = EffectBorderMode.Hard,
                            BlurAmount = 16,
                            Source = this.BitmapLayer.Source
                        },
                    }
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
                    Source = this.BitmapLayer.Origin,
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
                    Source = this.BitmapLayer.Source,
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
                    Source = this.BitmapLayer.Temp,
                    Scale = new Vector2(this.CanvasControl.Dpi.ConvertDipsToPixels(100) / 512)
                });
            };
        }

        private async Task CreateResourcesAsync()
        {
            this.ShaderCodeBytesTwirlLeft = await ShaderType.DisplacementTwirlLeft.LoadAsync();
            this.ShaderCodeBytesTwirlRight = await ShaderType.DisplacementTwirlRight.LoadAsync();
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                this.Render(position, this.IsExpand);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                // History
                this.History.Push(this.BitmapLayer.GetBitmapHistory());
                this.BitmapLayer.Flush();
                this.BitmapLayer.Clear(DisplacementExtension.ClearColor, BitmapType.Temp);
                this.BitmapLayer.RenderThumbnail();

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
        }


        private void Render(Vector2 targetPosition, bool isExpand)
        {
            if (this.BitmapLayer is null) return;
            if (this.CanvasBitmap is null) return;
            if (this.ShaderCodeBytesTwirlLeft is null) return;
            if (this.ShaderCodeBytesTwirlRight is null) return;

            // 1. Region
            Rect rect = targetPosition.GetRect(this.RangeSize);

            // 2. Shader
            PixelShaderEffect shader = new PixelShaderEffect(isExpand ? this.ShaderCodeBytesTwirlRight : this.ShaderCodeBytesTwirlLeft)
            {
                Source1BorderMode = EffectBorderMode.Hard,
                Source1 = this.BitmapLayer.Source,
                Properties =
                {
                    ["radius"] = this.RangeSize,
                    ["targetPosition"] = targetPosition,
                }
            };

            // 3. Render
            this.BitmapLayer.Hit(rect);
            this.BitmapLayer.Shade(shader, rect);
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
                {
                    this.CanvasBitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}