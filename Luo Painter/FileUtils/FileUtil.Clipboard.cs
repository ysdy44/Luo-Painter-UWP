using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;

namespace Luo_Painter
{
    public static partial class FileUtil
    {
        /// <summary>
        /// This is the maximum Bitmap render size for Win2D
        /// <see cref="CanvasDevice.MaximumBitmapSizeInPixels"/>
        /// </summary>
        public const int MaxImageSize = 16384;

        public static Size GetCurrentViewBounds()
        {
            try
            {
                ApplicationView applicationView = ApplicationView.GetForCurrentView();
                Rect bounds = applicationView.VisibleBounds;

                DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
                double scale = displayInformation.RawPixelsPerViewPixel;

                return new Size(bounds.Width * scale, bounds.Height * scale);
            }
            catch (Exception)
            {
                return Size.Empty;
            }
        }

        public static async Task<RandomAccessStreamReference> ToStream(this CanvasBitmap bitmap)
        {
            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            await bitmap.SaveAsync(stream, CanvasBitmapFileFormat.Bmp);
            return RandomAccessStreamReference.CreateFromStream(stream);
        }
        public static async Task<IRandomAccessStreamReference> ToThumbnail(this CanvasBitmap bitmap, StorageFile file)
        {
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await bitmap.SaveAsync(fileStream, CanvasBitmapFileFormat.Png);
            }

            StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem);
            return RandomAccessStreamReference.CreateFromStream(thumbnail);
        }

        public static async void SetBitmap(CanvasBitmap bitmap)
        {
            DataPackage dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy,
            };

            dataPackage.SetBitmap(await bitmap.ToStream());

            Clipboard.SetContent(dataPackage);
        }

        public static async void SetBitmapWithFile(CanvasBitmap bitmap)
        {
            StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("File.png", CreationCollisionOption.GenerateUniqueName);
            DataPackage dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };

            dataPackage.SetBitmap(await bitmap.ToStream());
            dataPackage.SetStorageItems(new[] { file });
            dataPackage.Properties.Thumbnail = await bitmap.ToThumbnail(file);

            Clipboard.SetContent(dataPackage);
        }
    }
}