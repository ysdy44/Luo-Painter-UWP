using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
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
    public static class DisplacementColorExtension
    {
        public static readonly Color ClearColor = Color.FromArgb(255, 127, 127, 255);
        public static readonly Color Left = Color.FromArgb(255, 255, 127, 255);
        public static readonly Color Right = Color.FromArgb(255, 0, 127, 255);
        public static readonly Color Up = Color.FromArgb(255, 127, 255, 255);
        public static readonly Color Down = Color.FromArgb(255, 127, 0, 255);
        public static Color ToColor(this Vector2 vectorUnit)
        {
            return new Color
            {
                A = 255,
                R = (byte)(127 - 127 * vectorUnit.X),
                G = (byte)(127 - 127 * vectorUnit.Y),
                B = 255,
            };
        }
    }

    public sealed partial class DisplacementMapPage : Page
    {
        readonly CanvasDevice Device = new CanvasDevice();
        CanvasBitmap CanvasBitmap;
        BitmapLayer BitmapLayer;

        Vector2 Position;

        float InkOpacity = 0.4f;

        public DisplacementMapPage()
        {
            this.InitializeComponent();
            this.ConstructDisplacementMap();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructDisplacementMap()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                this.BitmapLayer.DrawCopy(this.CanvasBitmap);
                this.BitmapLayer.Flush();

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                this.InkOpacity = (float)e.NewValue;
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Source);
                this.BitmapLayer.DrawCopy(this.CanvasBitmap);
                this.BitmapLayer.Flush();

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(this.Device, 512, 512);
                this.BitmapLayer.Clear(DisplacementColorExtension.ClearColor, BitmapType.Temp);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.FillRectangle(0, 0, this.BitmapLayer.Width, this.BitmapLayer.Height, Colors.White);

                args.DrawingSession.DrawImage(new CropEffect
                {
                    SourceRectangle = this.BitmapLayer.Bounds.ToRect(),
                    Source = new DisplacementMapEffect
                    {
                        Source = this.BitmapLayer.Source,
                        Displacement = this.BitmapLayer.Temp,
                        XChannelSelect = EffectChannelSelect.Red,
                        YChannelSelect = EffectChannelSelect.Green,
                        Amount = 100,
                    }
                });

                args.DrawingSession.DrawCircle(this.Position, 122, Colors.Gray, sender.Dpi.ConvertDipsToPixels(1));
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

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.Position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);
                float pressure = properties.Pressure;

                Vector2 vector = position - this.Position;
                float length = vector.Length();
                if (length < 2) return;

                Vector2 unit = vector / length;
                Color color = unit.ToColor();

                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession(BitmapType.Temp))
                {
                    ds.FillCircle(position, 80, new CanvasRadialGradientBrush(this.CanvasControl, color, Colors.Transparent)
                    {
                        Center = position,
                        RadiusX = 80,
                        RadiusY = 80,
                        Opacity = this.InkOpacity,
                    });
                }

                this.Position = position;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
                this.SourceCanvasControl.Invalidate(); // Invalidate
                this.TempCanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                this.BitmapLayer.DrawCopy(new DisplacementMapEffect
                {
                    Source = this.BitmapLayer.Origin,
                    Displacement = this.BitmapLayer.Temp,
                    XChannelSelect = EffectChannelSelect.Red,
                    YChannelSelect = EffectChannelSelect.Green,
                    Amount = 100,
                });
                this.BitmapLayer.Flush();
                this.BitmapLayer.Clear(DisplacementColorExtension.ClearColor, BitmapType.Temp);

                this.BitmapLayer.RenderThumbnail();
                this.CanvasControl.Invalidate(); // Invalidate
            };
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

    }
}