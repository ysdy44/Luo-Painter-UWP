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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        public void Load()
        {
            // 1. Clear
            this.History.Clear();

            this.Layers.Clear();
            this.Nodes.Clear();
            this.ObservableCollection.Clear();

            // 2. Add
            BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);

            this.Layers.Push(bitmapLayer);
            this.Nodes.Add(bitmapLayer);
            this.ObservableCollection.Add(bitmapLayer);

            // 3. UI
            this.LayerListView.SelectedIndex = 0;
        }

        public async Task LoadAsync(string path)
        {
            // 1. Clear
            this.History.Clear();

            this.Layers.Clear();
            this.Nodes.Clear();
            this.ObservableCollection.Clear();


            StorageFolder zipFolder = await StorageFolder.GetFolderFromPathAsync(path);


            // 2. Load Layers.xml
            StorageFile file2 = await zipFolder.GetFileAsync("Layers.xml");
            using (IRandomAccessStream accessStream2 = await file2.OpenAsync(FileAccessMode.ReadWrite))
            {
                XDocument doc = XDocument.Load(accessStream2.AsStream());

                foreach (XElement item in doc.Root.Elements("Layer"))
                {
                    if (item is null) continue;

                    if (item.Attribute("Id") is XAttribute id2)
                    {
                        string id = id2.Value;
                        if (string.IsNullOrEmpty(id)) continue;

                        if (item.Attribute("Type") is XAttribute type2)
                        {
                            ILayer layer = null;
                            {
                                LayerType type = (LayerType)Enum.Parse(typeof(LayerType), type2.Value);
                                switch (type)
                                {
                                    case LayerType.Bitmap:
                                        StorageFile file = await zipFolder.GetFileAsync(id);
                                        if (file is null) continue;

                                        using (IRandomAccessStream accessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                                        using (CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(this.CanvasDevice, accessStream))
                                        {
                                            layer = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);
                                        }
                                        break;
                                    case LayerType.Group:
                                        layer = new GroupLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (layer is null) continue;

                            if (item.Attribute("Name") is XAttribute name) layer.Name = name.Value;
                            if (item.Attribute("Opacity") is XAttribute opacity) layer.Opacity = (float)opacity;
                            if (item.Attribute("BlendMode") is XAttribute blendMode)
                                if (blendMode.Value is "None" is false)
                                    layer.BlendMode = (BlendEffectMode)Enum.Parse(typeof(BlendEffectMode), blendMode.Value);
                            if (item.Attribute("Visibility") is XAttribute visibility) layer.Visibility = visibility.Value == "Collapsed" ? Visibility.Collapsed : Visibility.Visible;
                            if (item.Attribute("IsExpand") is XAttribute isExpand) layer.IsExpand = (bool)isExpand;

                            this.Layers.Push(id, layer);
                        }
                    }
                }
            }

            // 3. Load Project.xml
            StorageFile file3 = await zipFolder.GetFileAsync("Project.xml");
            using (IRandomAccessStream accessStream3 = await file3.OpenAsync(FileAccessMode.ReadWrite))
            {
                XDocument doc = XDocument.Load(accessStream3.AsStream());
                if (doc.Root.Element("Width") is XElement width) this.Transformer.Width = (int)width;
                if (doc.Root.Element("Height") is XElement height) this.Transformer.Height = (int)height;
                if (doc.Root.Element("Index") is XElement index) this.LayerSelectedIndex = (int)index;

                this.Nodes.Load(this.Layers, doc.Root.Element("Layerages"));
            }

            // 4. UI 
            foreach (ILayer item in this.Nodes)
            {
                item.Arrange(0);
                this.ObservableCollection.AddChild(item);
            }
            this.LayerListView.SelectedIndex = 0;
        }

        public async Task SaveAsync(string path, bool isGoBack = true)
        {
            // 1. Delete
            StorageFolder zipFolder = await StorageFolder.GetFolderFromPathAsync(path);
            IReadOnlyList<StorageFile> files = await zipFolder.GetFilesAsync();
            foreach (StorageFile item in files)
            {
                await item.DeleteAsync();
            }

            // 2. Save Bitmaps 
            foreach (ILayer item in this.ObservableCollection)
            {
                switch (item.Type)
                {
                    case LayerType.Bitmap:
                        if (item is BitmapLayer bitmapLayer)
                        {
                            StorageFile file = await zipFolder.CreateFileAsync(bitmapLayer.Id, CreationCollisionOption.ReplaceExisting);
                            using (IRandomAccessStream accessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                await bitmapLayer.SaveAsync(accessStream);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            // 3. Save Layers.xml 
            StorageFile file2 = await zipFolder.CreateFileAsync("Layers.xml", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream accessStream = await file2.OpenAsync(FileAccessMode.ReadWrite))
            {
                XDocument doc =
                    new XDocument(new XElement("Root",
                    from layer
                    in this.ObservableCollection
                    select new XElement("Layer",
                        new XAttribute("Id", layer.Id),
                        new XAttribute("Type", layer.Type),
                        new XAttribute("Name", layer.Name is null ? String.Empty : layer.Name),
                        new XAttribute("Opacity", layer.Opacity),
                        new XAttribute("BlendMode", layer.BlendMode is null ? "None" : $"{layer.BlendMode}"),
                        new XAttribute("Visibility", layer.Visibility),
                        new XAttribute("IsExpand", layer.IsExpand)
                        )));

                doc.Save(accessStream.AsStream());
            }

            // 4. Save Project.xml
            StorageFile file3 = await zipFolder.CreateFileAsync("Project.xml", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream accessStream = await file3.OpenAsync(FileAccessMode.ReadWrite))
            {
                XDocument doc = new XDocument(new XElement("Root",
                    new XElement("Width", this.Transformer.Width),
                    new XElement("Height", this.Transformer.Height),
                    new XElement("Index", this.LayerSelectedIndex),
                    new XElement("Layerages", this.Nodes.Save())));
                doc.Save(accessStream.AsStream());
            }

            // 5. Save Thumbnail.png
            float scaleX = 256f / this.Transformer.Width;
            float scaleY = 256f / this.Transformer.Height;
            float scale = System.Math.Min(scaleX, scaleY);

            StorageFile file4 = await zipFolder.CreateFileAsync("Thumbnail.png", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream accessStream = await file4.OpenAsync(FileAccessMode.ReadWrite))
            using (CanvasCommandList background = new CanvasCommandList(this.CanvasDevice))
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
            if (isGoBack && base.Frame.CanGoBack)
            {
                this.History.Clear();
                this.Layers.Clear();
                this.Nodes.Clear();
                this.ObservableCollection.Clear();

                base.Frame.GoBack();
            }
        }

    }
}