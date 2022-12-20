using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Luo_Painter.Projects.Models
{
    public sealed partial class ProjectFile
    {

        public async Task<ProjectParameter> SaveAsync(System.Drawing.Size size)
        {
            // Save Project.xml
            using (IRandomAccessStream stream = await (await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Project.xml", CreationCollisionOption.ReplaceExisting)).OpenAsync(FileAccessMode.ReadWrite))
            {
                new XDocument(new XElement("Root",
                    new XElement("Width", size.Width),
                    new XElement("Height", size.Height)
                    )).Save(stream.AsStream());
            }

            return new ProjectParameter
            {
                Type = ProjectParameterType.None,

                Path = this.Path,
                Name = this.Name,
                DisplayName = this.DisplayName,

                Width = size.Width,
                Height = size.Height,
            };
        }

        public async Task<ProjectParameter> SaveAsync(BitmapSize size, IBuffer bytes, StorageItemThumbnail thumbnail)
        {
            // 1. ?
            string id = "0";

            // 2. Save Bitmaps 
            string type = "Bitmap";

            // Write Buffer
            StorageFile file2 = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(id, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(file2, bytes);

            // 3. Save Layers.xml 
            using (IRandomAccessStream stream = await (await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Layers.xml", CreationCollisionOption.ReplaceExisting)).OpenAsync(FileAccessMode.ReadWrite))
            {
                XDocument docLayers = new XDocument(new XElement("Root", new XElement[]
                {
                    new XElement("Layer", new XAttribute("Id", id), new XAttribute("Type", type))
                }));
                docLayers.Save(stream.AsStream());
            }

            // 4. Save Project.xml
            using (IRandomAccessStream stream = await (await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Project.xml", CreationCollisionOption.ReplaceExisting)).OpenAsync(FileAccessMode.ReadWrite))
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
            using (IRandomAccessStream stream = await (await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Thumbnail.png", CreationCollisionOption.ReplaceExisting)).OpenAsync(FileAccessMode.ReadWrite))
            {
                await RandomAccessStream.CopyAsync(thumbnail, stream);
            }

            return new ProjectParameter
            {
                Type = ProjectParameterType.Image,

                Path = this.Path,
                Name = this.Name,
                DisplayName = this.DisplayName,

                Width = (int)size.Width,
                Height = (int)size.Height,

                Bitmap = bytes
            };
        }

        public async Task<ProjectParameter> LoadAsync()
        {
            StorageFolder item;
            try { item = await StorageFolder.GetFolderFromPathAsync(this.Path); }
            catch (Exception) { return null; }

            // Copy
            foreach (StorageFile item2 in await item.GetFilesAsync())
            {
                await item2.CopyAsync(ApplicationData.Current.TemporaryFolder, item2.Name, NameCollisionOption.ReplaceExisting);
            }
          
            string docProject = null;
            string docLayers = null;

            IReadOnlyList<StorageFile> files = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
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

                            return new ProjectParameter
                            {
                                Type = ProjectParameterType.File,

                                Path = this.Path,
                                Name = this.Name,
                                DisplayName = this.DisplayName,

                                Width = width,
                                Height = height,

                                DocProject = docProject,
                                DocLayers = docLayers,
                                Bitmaps = bitmaps,
                            };
                        }
                    }
                }
            }

            return null;
        }

    }
}