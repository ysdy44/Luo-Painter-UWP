using Luo_Painter.Layers;
using Luo_Painter.Projects;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class MainPage : Page
    {

        private async Task<ProjectParameter> SaveAsync(StorageFolder item, BitmapSize size)
        {
            // Save Project.xml
            using (IRandomAccessStream stream = await item.CreateStreamAsync("Project.xml"))
            {
                new XDocument(new XElement("Root",
                    new XElement("Width", size.Width),
                    new XElement("Height", size.Height)
                    )).Save(stream.AsStream());
            }

            return new ProjectParameter(item.Path, item.Name, size);
        }

        private async Task<ProjectParameter> SaveAsync(StorageFolder item, CanvasBitmap bitmap, StorageItemThumbnail thumbnail)
        {
            // 1. ?

            // 2. Save Bitmaps 
            BitmapSize size = bitmap.SizeInPixels;
            string id = 0.ToString();
            IBuffer bytes = bitmap.GetPixelBytes().AsBuffer();

            // Write Buffer
            StorageFile file2 = await item.CreateFileAsync(id, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(file2, bytes);

            // 3. Save Layers.xml 
            using (IRandomAccessStream stream = await item.CreateStreamAsync("Layers.xml"))
            {
                XDocument docLayers = new XDocument(new XElement("Root", new XElement[]
                {
                    new XElement("Layer", new XAttribute("Id", id), new XAttribute("Type", LayerType.Bitmap))
                }));
                docLayers.Save(stream.AsStream());
            }

            // 4. Save Project.xml
            using (IRandomAccessStream stream = await item.CreateStreamAsync("Project.xml"))
            {
                XDocument docProject = new XDocument(new XElement("Root",
                new XElement("Width", size.Width),
                new XElement("Height", size.Height),
                new XElement("Index", 0),
                new XElement("Layerages", new XElement[]
                {
                    new XElement("Layerage", new XAttribute("Id", id))
                })));
                docProject.Save(stream.AsStream());
            }

            // 5. Save Thumbnail.png
            using (IRandomAccessStream fileStream = await item.CreateStreamAsync("Thumbnail.png"))
            {
                IInputStream source = thumbnail;
                IOutputStream destination = fileStream;
                await RandomAccessStream.CopyAsync(source, destination);
            }

            return new ProjectParameter(item.Path, item.Name, size.Width, size.Height, bytes);
        }

        private async Task<ProjectParameter> SaveAsync(Project project)
        {
            StorageFolder item = await FileUtil.GetFolderFromPathAsync(project.Path);
            if (item is null) return null;

            string docProject = null;
            string docLayers = null;

            IReadOnlyList<StorageFile> files = await item.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                string id = file.Name;
                if (string.IsNullOrEmpty(id)) continue;

                switch (id)
                {
                    case "Project.xml": docProject = file.Path; break;
                    case "Layers.xml": docLayers = file.Path; break;
                }
            }
            if (docProject is null) return null;

            XDocument docProject2 = XDocument.Load(docProject);
            if (docProject2.Root.Element("Width") is XElement width2)
            {
                if (docProject2.Root.Element("Height") is XElement height2)
                {
                    if (int.TryParse(width2.Value, out int width))
                    {
                        if (int.TryParse(height2.Value, out int height))
                        {

                            IDictionary<string, IBuffer> bitmaps = new Dictionary<string, IBuffer>();
                            foreach (StorageFile file in files)
                            {
                                string id = file.Name;
                                if (string.IsNullOrEmpty(id)) continue;

                                switch (id)
                                {
                                    case "Thumbnail.png":
                                    case "Project.xml":
                                    case "Layers.xml":
                                        break;
                                    default:
                                        // Read Buffer
                                        bitmaps.Add(id, await FileIO.ReadBufferAsync(file));
                                        break;
                                }
                            }

                            return new ProjectParameter(project.Path, project.Name, width, height, docProject, docLayers, bitmaps);
                        }
                    }
                }
            }

            return null;
        }

    }
}