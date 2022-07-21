using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        private async Task<IRandomAccessStream> CreateStreamAsync(StorageFolder storageFolder, string desiredName) => await (await storageFolder.CreateFileAsync(desiredName, CreationCollisionOption.ReplaceExisting)).OpenAsync(FileAccessMode.ReadWrite);


        private void Load(BitmapLayer bitmapLayer)
        {
            this.Transformer.Width = bitmapLayer.Width;
            this.Transformer.Height = bitmapLayer.Height;

            this.Layers.Push(bitmapLayer);
            this.Nodes.Add(bitmapLayer);
            this.ObservableCollection.Add(bitmapLayer);

            this.LayerSelectedIndex = 0;
        }

        public void Load(BitmapSize size) => this.Load(new BitmapLayer(this.CanvasDevice, (int)size.Width, (int)size.Height));

        public async Task LoadImageAsync(string path)
        {
            StorageFolder item = await StorageFolder.GetFolderFromPathAsync(path);
            foreach (StorageFile item2 in await item.GetFilesAsync())
            {
                using (IRandomAccessStream accessStream = await item2.OpenAsync(FileAccessMode.ReadWrite))
                using (CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasDevice, accessStream))
                {
                    this.Load(new BitmapLayer(this.CanvasDevice, bitmap));
                    return;
                }
            }
        }

        public async Task LoadAsync(string path)
        {
            StorageFolder item = await StorageFolder.GetFolderFromPathAsync(path);
            if (item is null) return;


            // 1. Load All Files
            // XDocument
            // CanvasBitmap
            XDocument docProject = null;
            XDocument docLayers = null;
            IDictionary<string, IBuffer> bitmaps = new Dictionary<string, IBuffer>();
            foreach (StorageFile item2 in await item.GetFilesAsync())
            {
                string id = item2.Name;
                if (string.IsNullOrEmpty(id)) continue;

                switch (id)
                {
                    case "Thumbnail.png":
                        break;
                    case "Project.xml":
                        using (IRandomAccessStream accessStream = await item2.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            docProject = XDocument.Load(accessStream.AsStream());
                        }
                        break;
                    case "Layers.xml":
                        using (IRandomAccessStream accessStream = await item2.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            docLayers = XDocument.Load(accessStream.AsStream());
                        }
                        break;
                    default:
                        bitmaps.Add(id, await FileIO.ReadBufferAsync(item2));
                        break;
                }
            }

            if (docProject is null is false && docLayers is null is false)
            {
                // 1. Load Project.xml
                // Width
                // Height
                if (docProject.Root.Element("Width") is XElement width) this.Transformer.Width = (int)width;
                if (docProject.Root.Element("Height") is XElement height) this.Transformer.Height = (int)height;


                // 2. Load Layers.xml
                // Layers
                foreach (XElement item2 in docLayers.Root.Elements("Layer"))
                {
                    if (item2.Attribute("Id") is XAttribute id2 && item2.Attribute("Type") is XAttribute type2)
                    {
                        string id = id2.Value;
                        if (string.IsNullOrEmpty(id)) continue;

                        string type = type2.Value;
                        if (string.IsNullOrEmpty(id)) continue;

                        switch (type)
                        {
                            case "Bitmap":
                                BitmapLayer bitmapLayer =
                                    bitmaps.ContainsKey(id) ?
                                    new BitmapLayer(this.CanvasDevice, bitmaps[id], this.Transformer.Width, this.Transformer.Height) :
                                    new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
                                bitmapLayer.Load(item2);
                                this.Layers.Push(id, bitmapLayer);
                                break;
                            case "Group":
                                GroupLayer groupLayer = new GroupLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
                                groupLayer.Load(item2);
                                this.Layers.Push(id, groupLayer);
                                break;
                            default:
                                break;
                        }
                    }
                }


                // 3. Nodes 
                if (docProject.Root.Element("Layerages") is XElement layerages)
                {
                    this.Nodes.Load(this.Layers, layerages);
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
            }

            bitmaps.Clear();
        }


        private XElement Save(ILayer layer) => layer.Save(layer.Type);
        public async Task SaveAsync(string path, bool isGoBack)
        {
            XDocument docLayers = new XDocument(new XElement("Root", this.ObservableCollection.Select(this.Save)));
            XDocument docProject = new XDocument(new XElement("Root",
                new XElement("Width", this.Transformer.Width),
                new XElement("Height", this.Transformer.Height),
                new XElement("Index", this.LayerSelectedIndex),
                new XElement("Layerages", this.Nodes.Save())));


            StorageFolder item = await StorageFolder.GetFolderFromPathAsync(path);

            // 1. Delete
            foreach (StorageFile item2 in await item.GetFilesAsync())
            {
                await item2.DeleteAsync();
            }

            // 2. Save Bitmaps 
            foreach (ILayer item2 in this.ObservableCollection)
            {
                switch (item2.Type)
                {
                    case LayerType.Bitmap:
                        if (item2 is BitmapLayer bitmapLayer)
                        {
                            StorageFile file = await item.CreateFileAsync(item2.Id, CreationCollisionOption.ReplaceExisting);
                            await FileIO.WriteBufferAsync(file, bitmapLayer.GetPixelBytes());
                        }
                        break;
                    default:
                        break;
                }
            }

            // 3. Save Layers.xml 
            using (IRandomAccessStream accessStream = await this.CreateStreamAsync(item, "Layers.xml"))
            {
                docLayers.Save(accessStream.AsStream());
            }

            // 4. Save Project.xml
            using (IRandomAccessStream accessStream = await this.CreateStreamAsync(item, "Project.xml"))
            {
                docProject.Save(accessStream.AsStream());
            }

            // 5. Save Thumbnail.png
            float scaleX = 256f / this.Transformer.Width;
            float scaleY = 256f / this.Transformer.Height;
            float scale = System.Math.Min(scaleX, scaleY);

            using (IRandomAccessStream accessStream = await this.CreateStreamAsync(item, "Thumbnail.png"))
            using (ScaleEffect image = new ScaleEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                Scale = new Vector2(scale),
                Source = this.Nodes.Render(this.Mesh.Image)
            })
            {
                await CanvasImage.SaveAsync(image, new Rect
                {
                    Width = scale * this.Transformer.Width,
                    Height = scale * this.Transformer.Height,
                }, 96, this.CanvasDevice, accessStream, CanvasBitmapFileFormat.Png);
            }


            // 6. Clear
            if (isGoBack)
            {
                this.History.Clear();
                this.Layers.Clear();
                this.Nodes.Clear();
                this.ObservableCollection.Clear();

                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            }
        }

    }
}