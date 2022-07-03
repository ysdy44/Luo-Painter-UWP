using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.TestApp
{
    public sealed partial class WriteableBitmapSourcePage : Page
    {
        public ImageSource Thumbnail => this.ThumbnailWriteableBitmap;

        readonly CanvasDevice CanvasControl = new CanvasDevice();
        readonly CanvasRenderTarget ThumbnailRenderTarget;
        readonly WriteableBitmap ThumbnailWriteableBitmap;

        CanvasBitmap CanvasBitmap;

        public WriteableBitmapSourcePage()
        {
            this.InitializeComponent();
            this.ThumbnailRenderTarget = new CanvasRenderTarget(this.CanvasControl, 50, 50, 96);
            this.ThumbnailWriteableBitmap = new WriteableBitmap(50, 50);
            this.AddButton.Click += async (s, e) =>
            {
                StorageFile file = await this.PickSingleImageFileAsync(PickerLocationId.Desktop);
                if (file is null) return;

                bool? result = await this.AddAsync(file);
                if (result is null) return;
                if (result is false) return;

                this.RenderThumbnail();
            };
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

        public void RenderThumbnail()
        {
            using (CanvasDrawingSession ds = this.ThumbnailRenderTarget.CreateDrawingSession())
            {
                ds.Blend = CanvasBlend.Copy;
                ds.DrawImage(new ScaleEffect
                {
                    Scale = new Vector2(50f / Math.Max(this.CanvasBitmap.SizeInPixels.Width, this.CanvasBitmap.SizeInPixels.Height)),
                    BorderMode = EffectBorderMode.Hard,
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                    Source = this.CanvasBitmap,
                });
            }

            byte[] bytes = this.ThumbnailRenderTarget.GetPixelBytes();
            using (Stream stream = this.ThumbnailWriteableBitmap.PixelBuffer.AsStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            this.ThumbnailWriteableBitmap.Invalidate();
        }

    }
}