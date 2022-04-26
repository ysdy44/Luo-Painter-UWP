using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class FloodSelectPage : Page
    {

        readonly CanvasDevice Device = new CanvasDevice();
        BitmapLayer BitmapLayer;

        public FloodSelectPage()
        {
            this.InitializeComponent();
            this.ConstructFloodSelect();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructFloodSelect()
        {
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file == null) return;

                bool? result = await this.AddAsync(file);
                if (result != true) return;

                this.CanvasControl.Invalidate(); // Invalidate
                this.OriginCanvasControl.Invalidate(); // Invalidate
            };
            this.ClearButton.Click += (s, e) =>
            {
                this.BitmapLayer.ClearTemp();
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

                args.DrawingSession.DrawImage(this.BitmapLayer.Source);
            };


            this.OriginCanvasControl.UseSharedDevice = true;
            this.OriginCanvasControl.CustomDevice = this.Device;
            this.OriginCanvasControl.Draw += (sender, args) =>
            {
                if (this.BitmapLayer is null) return;

                args.DrawingSession.DrawImage(this.BitmapLayer.Temp);
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                if (this.BitmapLayer is null) return;
                this.BitmapLayer.FloodSelect(point, Windows.UI.Colors.DodgerBlue);
                this.OriginCanvasControl.Invalidate(); // Invalidate
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