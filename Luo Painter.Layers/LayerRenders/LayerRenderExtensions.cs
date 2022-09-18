using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public sealed class LayerRenderExtensions
    {

        /// <summary>
        /// Saves the entire bitmap to the specified stream
        /// with the specified file format and quality level.
        /// </summary>
        /// <param name="file"> The storage-file. </param>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="width"> The width of image. </param>
        /// <param name="height"> The height of image. </param>
        /// <param name="dpi"> The file dpi. </param>
        /// <param name="fileFormat"> The file format. </param>
        /// <param name="quality"> The file quality. </param>
        /// <returns> Saved successful? </returns>
        public async Task<bool> Export(ILayerRender render, IStorageFile file, ICanvasResourceCreator resourceCreator, float width, float height, float dpi = 96, CanvasBitmapFileFormat fileFormat = CanvasBitmapFileFormat.Jpeg, float quality = 1.0f)
        {
            using (ICanvasImage background = (fileFormat is CanvasBitmapFileFormat.Jpeg) ? (ICanvasImage)new ColorSourceEffect
            {
                Color = Colors.White
            } : (ICanvasImage)new CanvasCommandList(resourceCreator))
            {
                ICanvasImage image = render.Render(background);

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

        /// <summary>
        /// Saves the all bitmap to the mult-stream
        /// with the specified file format and quality level.
        /// </summary>
        /// <param name="folder"> The storage-folder. </param>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="width"> The width of image. </param>
        /// <param name="height"> The height of image. </param>
        /// <param name="dpi"> The file dpi. </param>
        /// <param name="fileChoices"> The file choices. </param>
        /// <param name="fileFormat"> The file format. </param>
        /// <param name="quality"> The file quality. </param>
        /// <returns> How many layers saved? </returns>
        public async Task<int> ExportAll(IEnumerable<ILayerRender> renders, IStorageFolder folder, ICanvasResourceCreator resourceCreator, float width, float height, float dpi = 96, string fileChoices = ".Jpeg", CanvasBitmapFileFormat fileFormat = CanvasBitmapFileFormat.Jpeg, float quality = 1.0f)
        {
            if (renders.Count() is 0) return 0;

            int index = 0;
            int count = 0;

            using (ICanvasImage background = (fileFormat is CanvasBitmapFileFormat.Jpeg) ? (ICanvasImage)new ColorSourceEffect
            {
                Color = Colors.White
            } : (ICanvasImage)new CanvasCommandList(resourceCreator))
            {
                foreach (ILayerRender item in renders)
                {
                    string name = index.ToString();
                    StorageFile file = await folder.CreateFileAsync($"{name}{fileChoices}", CreationCollisionOption.ReplaceExisting);
                    index++;

                    if (file is null) continue;
                    ICanvasImage image = item.Render(background);

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
                        count++;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return count;
        }

    }
}