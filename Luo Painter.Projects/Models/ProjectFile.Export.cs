using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Project.xml"))
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
            const string id = "0";

            // 2. Save Bitmaps 
            const string type = "Bitmap";

            // Write Buffer
            StorageFile file2 = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(id, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(file2, bytes);

            // 3. Save Layers.xml 
            using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Layers.xml"))
            {
                new XDocument(new XElement("Root", new XElement[]
                {
                    new XElement("Layer", new XAttribute("Id", id), new XAttribute("Type", type))
                })).Save(stream.AsStream());
            }

            // 4. Save Project.xml
            using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Project.xml"))
            {
                new XDocument(new XElement("Root",
                    new XElement("Width", size.Width),
                    new XElement("Height", size.Height),
                    new XElement("Index", 0),
                    new XElement("Layerages", new XElement[]
                    {
                        new XElement("Layerage", new XAttribute("Id", id))
                    }))).Save(stream.AsStream());
            }

            // 5. Save Thumbnail.png
            using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Thumbnail.xml"))
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

            // 1. Load xml
            string docProject = null;
            string docLayers = null;
            string docBitmaps = null;
            string docPhotos = null;

            IReadOnlyList<StorageFile> files = await item.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                string id = file.Name;
                if (string.IsNullOrEmpty(id)) continue;

                switch (id)
                {
                    case "Project.xml": docProject = file.Path; break;
                    case "Layers.xml": docLayers = file.Path; break;
                    case "Bitmaps.xml": docBitmaps = file.Path; break;
                    case "Photos.xml": docPhotos = file.Path; break;
                }
            }
            if (docProject is null) return null;

            // Copy
            foreach (StorageFile file in files)
            {
                await file.CopyAsync(ApplicationData.Current.TemporaryFolder, file.Name, NameCollisionOption.ReplaceExisting);
            }

            // 2. Load Project.xml
            int width = 0;
            int height = 0;
            bool hasProject = false;

            XDocument docProject2 = XDocument.Load(docProject);
            if (docProject2.Root.Element("Width") is XElement width2)
            {
                if (docProject2.Root.Element("Height") is XElement height2)
                {
                    if (int.TryParse(width2.Value, out width))
                    {
                        if (int.TryParse(height2.Value, out height))
                        {
                            hasProject = true;
                        }
                    }
                }
            }
            if (hasProject is false) return null;
            if (width < 16) return null;
            if (height < 16) return null;

            // 3. Load Bitmaps.xml 
            IDictionary<string, IBuffer> bitmaps = null;
            if (docBitmaps is null is false)
            {
                XDocument docBitmaps2 = XDocument.Load(docBitmaps);
                if (docBitmaps2.Root.Elements("Bitmap") is IEnumerable<XElement> bitmaps2)
                {
                    foreach (StorageFile file in files)
                    {
                        string id = file.Name;
                        if (string.IsNullOrEmpty(id)) continue;

                        foreach (XElement item2 in bitmaps2)
                        {
                            if (id == item2.Value)
                            {
                                // Read Buffer
                                IBuffer bytes = await FileIO.ReadBufferAsync(file);

                                if (bitmaps is null)
                                    bitmaps = new Dictionary<string, IBuffer>
                                    {
                                        [id] = bytes
                                    };
                                else
                                    bitmaps.Add(id, bytes);
                                break;
                            }
                        }
                    }
                }
            }

            // 4. Load Photos.xml 
            IDictionary<string, SoftwareBitmap> photos = null;
            if (docPhotos is null is false)
            {
                XDocument docPhotos2 = XDocument.Load(docPhotos);
                if (docPhotos2.Root.Elements("Photo") is IEnumerable<XElement> photos2)
                {
                    foreach (StorageFile file in files)
                    {
                        string id = file.Name;
                        if (string.IsNullOrEmpty(id)) continue;

                        foreach (XElement item2 in photos2)
                        {
                            if (id == item2.Value)
                            {
                                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                                {
                                    SoftwareBitmap softwareBitmap = await stream.ToSoftwareBitmap();

                                    if (photos is null)
                                        photos = new Dictionary<string, SoftwareBitmap>
                                        {
                                            [id] = softwareBitmap
                                        };
                                    else
                                        photos.Add(id, softwareBitmap);
                                }
                                break;
                            }
                        }
                    }
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
                Photos = photos,
            };
        }

    }
}