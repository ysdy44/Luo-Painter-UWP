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
using Windows.Graphics.DirectX;
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
                        StorageFile file = await item.CreateFileAsync(item2.Id, CreationCollisionOption.ReplaceExisting);
                        IBuffer buffer = ((BitmapLayer)item2).GetPixelBytes();
                        await FileIO.WriteBufferAsync(file, buffer);
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
                LayerDictionary.Instance.Clear();
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