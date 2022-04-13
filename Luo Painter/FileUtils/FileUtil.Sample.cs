using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Luo_Painter
{
    public static partial class FileUtil
    {

        /// <summary>
        /// Saves the entire bitmap to the specified stream
        /// with the specified file format and quality level.
        /// </summary>
        /// <param name="bitmap"> The bitmap.</param>
        /// <param name="fileChoices"> The file choices. </param>
        /// <param name="suggestedFileName"> The suggested name of file. </param>
        /// <param name="fileFormat"> The file format. </param>
        /// <param name="quality"> The file quality. </param>
        /// <returns> Saved successful? </returns>
        public static async Task<bool?> SaveCanvasBitmapFile(CanvasBitmap bitmap, string fileChoices = ".Jpeg", string suggestedFileName = "Untitled", CanvasBitmapFileFormat fileFormat = CanvasBitmapFileFormat.Jpeg, float quality = 1.0f)
        {
            // FileSavePicker
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                SuggestedFileName = suggestedFileName,
                FileTypeChoices =
                {
                    {"DB", new[] { fileChoices } }
                }
            };


            // PickSaveFileAsync
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file == null) return null;

            try
            {
                using (IRandomAccessStream accessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await bitmap.SaveAsync(accessStream, fileFormat, quality);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Saves the entire image to the specified stream
        /// with the specified file format and quality level.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="image"> The image. </param>
        /// <param name="width"> The width of image. </param>
        /// <param name="height"> The height of image. </param>
        /// <param name="dpi"> The file dpi. </param>
        /// <param name="fileChoices"> The file choices. </param>
        /// <param name="suggestedFileName"> The suggested name of file. </param>
        /// <param name="fileFormat"> The file format. </param>
        /// <param name="quality"> The file quality. </param>
        /// <returns> Saved successful? </returns>
        public static async Task<bool?> SaveCanvasImageFile(ICanvasResourceCreator resourceCreator, ICanvasImage image, float width, float height, float dpi = 96, string fileChoices = ".Jpeg", string suggestedFileName = "Untitled", CanvasBitmapFileFormat fileFormat = CanvasBitmapFileFormat.Jpeg, float quality = 1.0f)
        {
            // FileSavePicker
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop,
                SuggestedFileName = suggestedFileName,
                FileTypeChoices =
                {
                    {"DB", new[] { fileChoices } }
                }
            };


            // PickSaveFileAsync
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file == null) return null;

            try
            {
                using (IRandomAccessStream accessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await CanvasImage.SaveAsync(image, new Rect
                    {
                        Width = 96 / dpi * width,
                        Height = 96 / dpi * height,
                    }, dpi, resourceCreator, accessStream, fileFormat, quality);
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