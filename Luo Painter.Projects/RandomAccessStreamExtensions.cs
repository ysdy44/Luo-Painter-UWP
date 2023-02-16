using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Luo_Painter.Models
{
    public static class RandomAccessStreamExtensions
    {
        public static async Task<SoftwareBitmap> ToSoftwareBitmap(this IRandomAccessStream stream)
        {
            // Create the decoder from the stream
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

            // Get the SoftwareBitmap representation of the file
            return await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        }
        public static async Task<IRandomAccessStream> OpenAsync(this StorageFolder folder, string fileName)
        {
            StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            return await file.OpenAsync(FileAccessMode.ReadWrite);
        }
    }
}