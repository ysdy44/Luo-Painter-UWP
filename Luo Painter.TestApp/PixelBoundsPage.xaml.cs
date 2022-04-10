using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using FanKit.Transformers;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;

namespace Luo_Painter.TestApp
{
    public sealed partial class PixelBoundsPage : Page
    {

        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer; // 1024 * 1024
        PixelBounds InterpolationBounds; // 5 * 7
        PixelBounds PixelBounds; // 540 * 620

        public PixelBoundsPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                this.InterpolationBounds = this.BitmapLayer.CreateInterpolationBounds();
                this.PixelBounds = this.BitmapLayer.CreatePixelBounds(this.InterpolationBounds);

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.UseSharedDevice = true;
            this.CanvasControl.CustomDevice = this.Device;
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.BitmapLayer is null) return;

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(this.BitmapLayer.Source);

                float stroke = sender.Dpi.ConvertDipsToPixels(1);
                args.DrawingSession.DrawRectangle(0, 0, this.BitmapLayer.Width, this.BitmapLayer.Height, Colors.YellowGreen, stroke);
                args.DrawingSession.DrawRectangle(this.InterpolationBounds.ToRect(BitmapLayer.Unit), Colors.Red, stroke);
                args.DrawingSession.DrawRectangle(this.PixelBounds.ToRect(), Colors.DodgerBlue, stroke);
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                if (this.BitmapLayer is null) return;

                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawImage(new ScaleEffect
                {
                    Scale = new Vector2(BitmapLayer.Unit),
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Source = this.BitmapLayer.Temp
                });

                float stroke = sender.Dpi.ConvertDipsToPixels(1);
                args.DrawingSession.DrawRectangle(0, 0, this.BitmapLayer.Width, this.BitmapLayer.Height, Colors.YellowGreen, stroke);
                args.DrawingSession.DrawRectangle(this.InterpolationBounds.ToRect(BitmapLayer.Unit), Colors.Red, stroke);
                args.DrawingSession.DrawRectangle(this.PixelBounds.ToRect(), Colors.DodgerBlue, stroke);
            };
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
                    //@DPI
                    CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasControl, stream, 96);
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