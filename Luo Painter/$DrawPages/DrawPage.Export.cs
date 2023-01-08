using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Projects;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        public async Task SaveAsync(string path = null)
        {
            // 1. Save Thumbnail.png
            float scaleX = 256f / this.Transformer.Width;
            float scaleY = 256f / this.Transformer.Height;
            float scale = System.Math.Min(scaleX, scaleY);

            using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Thumbnail.png"))
            using (ScaleEffect image = new ScaleEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                Scale = new Vector2(scale),
                Source = this.Nodes.Render(this.Mesh)
            })
            {
                await CanvasImage.SaveAsync(image, new Rect
                {
                    Width = scale * this.Transformer.Width,
                    Height = scale * this.Transformer.Height,
                }, 96, this.CanvasDevice, stream, CanvasBitmapFileFormat.Png);
            }

            // 2. Save Project.xml
            using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Project.xml"))
            {
                new XDocument(new XElement("Root",
                    new XElement("Width", this.Transformer.Width),
                    new XElement("Height", this.Transformer.Height),
                    new XElement("Index", this.LayerSelectedIndex),
                    new XElement("Layerages", this.Nodes.Save()))).Save(stream.AsStream());
            }

            // 3. Save Layers.xml 
            bool hasLayers = this.ObservableCollection.Count > 0;
            if (hasLayers)
                using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Layers.xml"))
                {
                    new XDocument(new XElement("Root",
                        from l
                        in this.ObservableCollection
                        select l.Save())).Save(stream.AsStream());
                }

            // 4. Save Bitmaps
            IEnumerable<BitmapLayer> bitmapLayers = from layer in this.ObservableCollection where layer.Type is LayerType.Bitmap select layer as BitmapLayer;
            foreach (BitmapLayer bitmapLayer in bitmapLayers)
            {
                // Write Buffer
                StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(bitmapLayer.Id, CreationCollisionOption.ReplaceExisting);
                IBuffer bytes = bitmapLayer.GetPixelBytes();
                await FileIO.WriteBufferAsync(file, bytes);
            }

            // 5. Save Bitmaps.xml 
            IEnumerable<string> bitmaps = bitmapLayers.Select(b => b.Id);
            bool hasBitmaps = bitmaps.Count() > 0;
            if (hasBitmaps)
                using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Bitmaps.xml"))
                {
                    new XDocument(new XElement("Root",
                        from b
                        in bitmaps
                        select new XElement("Bitmap", b))).Save(stream.AsStream());
                }

            // 6. Save Images
            IEnumerable<ImageLayer> imageLayers = from layer in this.ObservableCollection where layer.Type is LayerType.Image select layer as ImageLayer;
            // Do what?

            // 7. Save Photos.xml 
            IEnumerable<string> photos = imageLayers.Select(p => p.Photo);
            bool hasPhotos = photos.Count() > 0;
            if (hasPhotos)
                using (IRandomAccessStream stream = await ApplicationData.Current.TemporaryFolder.OpenAsync("Photos.xml"))
                {

                    new XDocument(new XElement("Root",
                        from p
                        in photos
                        select new XElement("Photo", p))).Save(stream.AsStream());
                }


            if (string.IsNullOrEmpty(path)) return;
            StorageFolder item = await StorageFolder.GetFolderFromPathAsync(path);

            // Delete All
            foreach (StorageFile item2 in await item.GetFilesAsync())
            {
                await item2.DeleteAsync();
            }

            // Move Valuable Files
            foreach (StorageFile item2 in await ApplicationData.Current.TemporaryFolder.GetFilesAsync())
            {
                string id = item2.Name;
                bool valuable = false;

                if (string.IsNullOrEmpty(id))
                    valuable = false;
                else
                {
                    switch (id)
                    {
                        case "Thumbnail.png":
                        case "Project.xml":
                            valuable = true;
                            break;

                        case "Layers.xml":
                            valuable = hasLayers;
                            break;

                        case "Bitmaps.xml":
                            valuable = hasBitmaps;
                            break;
                        case "Photos.xml":
                            valuable = hasPhotos;
                            break;
                        default:
                            valuable =
                                (hasBitmaps && bitmaps.Contains(id)) ||
                                (hasPhotos && photos.Contains(id));
                            break;
                    }

                    if (valuable)
                        await item2.MoveAsync(item, item2.Name, NameCollisionOption.ReplaceExisting);
                    else
                        await item2.DeleteAsync();
                }
            }
        }

        public void Clear()
        {
            this.History.Clear();
            ImageLayer.Instance.Clear();
            LayerDictionary.Instance.Clear();
            this.Nodes.Clear();
            this.ObservableCollection.Clear();
        }

        public void Load(ProjectParameter item)
        {
            switch (item.Type)
            {
                case ProjectParameterType.None:
                    BitmapLayer bitmapLayer1 = new BitmapLayer(this.CanvasDevice, item.Width, item.Height);
                    this.Nodes.Add(bitmapLayer1);
                    this.ObservableCollection.Add(bitmapLayer1);

                    this.LayerSelectedIndex = 0;
                    break;
                case ProjectParameterType.Image:
                    BitmapLayer bitmapLayer2 = new BitmapLayer(this.CanvasDevice, item.Bitmap, item.Width, item.Height);
                    this.Nodes.Add(bitmapLayer2);
                    this.ObservableCollection.Add(bitmapLayer2);

                    this.LayerSelectedIndex = 0;
                    break;
                case ProjectParameterType.File:
                    if (string.IsNullOrEmpty(item.DocProject)) break;

                    // 1. Load Photos.xml
                    // Photos
                    if (item.Photos is null is false)
                    {
                        foreach (var item2 in item.Photos)
                        {
                            SoftwareBitmap softwareBitmap = item2.Value;
                            if (softwareBitmap is null) continue;

                            using (softwareBitmap)
                            {
                                string id = item2.Key;
                                if (string.IsNullOrEmpty(id)) continue;

                                CanvasBitmap bitmap = CanvasBitmap.CreateFromSoftwareBitmap(this.CanvasDevice, softwareBitmap);
                                if (bitmap is null) continue;

                                ImageLayer.Instance.Add(id, bitmap);
                            }
                        }
                    }

                    // 2. Load Layers.xml
                    // Layers
                    if (string.IsNullOrEmpty(item.DocLayers) is false)
                    {
                        XDocument docLayers = XDocument.Load(item.DocLayers);
                        foreach (XElement layer in docLayers.Root.Elements("Layer"))
                        {
                            if (layer.Attribute("Id") is XAttribute id2 && layer.Attribute("Type") is XAttribute type2)
                            {
                                string id = id2.Value;
                                if (string.IsNullOrEmpty(id)) continue;

                                string type = type2.Value;
                                if (string.IsNullOrEmpty(id)) continue;

                                _ = this.LoadLayer(type, id, layer, item);
                            }
                        }
                    }

                    // 2. Load Project.xml
                    // Nodes 
                    XDocument docProject = XDocument.Load(item.DocProject);
                    if (docProject.Root.Element("Layerages") is XElement layerages)
                    {
                        /// Gets <see cref="LayerDictionary"/>
                        this.Nodes.Load(layerages);
                    }

                    // 4. UI
                    foreach (ILayer item2 in this.Nodes)
                    {
                        item2.Arrange(0);
                        this.ObservableCollection.AddChild(item2);
                    }

                    if (docProject.Root.Element("Index") is XElement index)
                        this.LayerSelectedIndex = (int)index;
                    else if (this.ObservableCollection.Count > 0)
                        this.LayerSelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }

        private ILayer LoadLayer(string type, string id, XElement layer, ProjectParameter item)
        {
            switch (type)
            {
                case "Bitmap":
                    if (item.Bitmaps is null) break;
                    if (item.Bitmaps.ContainsKey(id))
                    {
                        /// Sets<see cref="ProjectParameter.Bitmap"/>
                        return new BitmapLayer(id, layer, this.CanvasDevice, item.Bitmaps[id], item.Width, item.Height); /// Sets <see cref="LayerDictionary"/>
                    }
                    break;
                case "Image":
                    if (layer.Element("Photo") is XElement photo2)
                    {
                        string photo = photo2.Value;
                        if (string.IsNullOrEmpty(photo)) break;

                        if (ImageLayer.Instance.ContainsKey(photo))
                        {
                            /// Sets<see cref="ProjectParameter.Photos"/>
                            return _ = new ImageLayer(id, layer, this.CanvasDevice, photo, item.Width, item.Height); /// Sets <see cref="LayerDictionary"/>
                        }
                    }
                    break;
                case "Group":
                    return new GroupLayer(id, layer, this.CanvasDevice, item.Width, item.Height); /// Sets <see cref="LayerDictionary"/>
                case "Curve":
                    return new CurveLayer(id, layer, this.CanvasDevice, item.Width, item.Height); /// Sets <see cref="LayerDictionary"/>
                case "Fill":
                    return new FillLayer(id, layer, this.CanvasDevice, item.Width, item.Height); /// Sets <see cref="LayerDictionary"/>
                default:
                    break;
            }

            return new BitmapLayer(id, layer, this.CanvasDevice, item.Width, item.Height); /// Sets <see cref="LayerDictionary"/>
        }

    }
}