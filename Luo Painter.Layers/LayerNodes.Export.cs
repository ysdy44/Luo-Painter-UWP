using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public sealed partial class LayerNodes : List<ILayer>, ILayerRender
    {

        public IEnumerable<XElement> Save() =>
            from layer
            in this
            select
            layer.Children.Count is 0 ?
            new XElement("Layerage", new XAttribute("Id", layer.Id)) :
            new XElement("Layerage", new XAttribute("Id", layer.Id), new XElement("Children", layer.Children.Save()));

        public void Load(XElement element)
        {
            if (element is null) return;

            foreach (XElement item in element.Elements("Layerage"))
            {
                if (item.Attribute("Id") is XAttribute id2)
                {
                    string id = id2.Value;
                    if (string.IsNullOrEmpty(id)) continue;

                    ILayer layer = LayerDictionary.Instance[id];
                    base.Add(layer);

                    XElement children = item.Element("Children");
                    if (children is null) continue;

                    layer.Children.Load(children);
                }
            }
        }


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
        public async Task<bool> Export(IStorageFile file, ICanvasResourceCreator resourceCreator, float width, float height, float dpi = 96, CanvasBitmapFileFormat fileFormat = CanvasBitmapFileFormat.Jpeg, float quality = 1.0f)
        {
            using (ICanvasImage background = (fileFormat is CanvasBitmapFileFormat.Jpeg) ? (ICanvasImage)new ColorSourceEffect
            {
                Color = Colors.White
            } : (ICanvasImage)new CanvasCommandList(resourceCreator))
            {
                ICanvasImage image = this.Render(background);

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
        public async Task<int> ExportAllthis(IStorageFolder folder, ICanvasResourceCreator resourceCreator, float width, float height, float dpi = 96, string fileChoices = ".Jpeg", CanvasBitmapFileFormat fileFormat = CanvasBitmapFileFormat.Jpeg, float quality = 1.0f)
        {
            if (this.Count is 0) return 0;

            int index = 0;
            int count = 0;

            using (ICanvasImage background = (fileFormat is CanvasBitmapFileFormat.Jpeg) ? (ICanvasImage)new ColorSourceEffect
            {
                Color = Colors.White
            } : (ICanvasImage)new CanvasCommandList(resourceCreator))
            {
                foreach (ILayer item in this)
                {
                    StorageFile file = await folder.CreateFileAsync($"{index}{fileChoices}");
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