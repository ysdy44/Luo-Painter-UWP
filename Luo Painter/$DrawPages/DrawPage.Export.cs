using Luo_Painter.Brushes;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
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
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private XElement Save(ILayer layer) => layer.Save(layer.Type);
        [MainPageToDrawPage(NavigationMode.Back)]
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
            foreach (ILayer layer in this.ObservableCollection)
            {
                switch (layer.Type)
                {
                    case LayerType.Bitmap:
                        if (layer is BitmapLayer bitmapLayer)
                        {
                            // Write Buffer
                            StorageFile file = await item.CreateFileAsync(layer.Id, CreationCollisionOption.ReplaceExisting);
                            IBuffer bytes = bitmapLayer.GetPixelBytes();
                            await FileIO.WriteBufferAsync(file, bytes);
                        }
                        break;
                    default:
                        break;
                }
            }

            // 3. Save Layers.xml 
            using (IRandomAccessStream stream = await item.CreateStreamAsync("Layers.xml"))
            {
                docLayers.Save(stream.AsStream());
            }

            // 4. Save Project.xml
            using (IRandomAccessStream stream = await item.CreateStreamAsync("Project.xml"))
            {
                docProject.Save(stream.AsStream());
            }

            // 5. Save Thumbnail.png
            float scaleX = 256f / this.Transformer.Width;
            float scaleY = 256f / this.Transformer.Height;
            float scale = System.Math.Min(scaleX, scaleY);

            using (IRandomAccessStream stream = await item.CreateStreamAsync("Thumbnail.png"))
            using (ScaleEffect image = new ScaleEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                Scale = new Vector2(scale),
                Source = this.Nodes.Render(this.Mesh[BitmapType.Source])
            })
            {
                await CanvasImage.SaveAsync(image, new Rect
                {
                    Width = scale * this.Transformer.Width,
                    Height = scale * this.Transformer.Height,
                }, 96, this.CanvasDevice, stream, CanvasBitmapFileFormat.Png);
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


        public async void AddAsync(IEnumerable<IStorageFile> items)
        {
            if (items is null) return;
            if (items.Count() is 0) return;

            Layerage[] undo = this.Nodes.Convert();

            int count = 0;
            int index = this.LayerSelectedIndex;
            if (index > 0 && this.LayerSelectedItem is ILayer neighbor)
            {
                ILayer parent = this.ObservableCollection.GetParent(neighbor);
                if (parent is null)
                {
                    int indexChild = this.Nodes.IndexOf(neighbor);

                    foreach (IRandomAccessStreamReference item in items)
                    {
                        CanvasBitmap bitmap = await this.CreateBitmap(item);
                        if (bitmap is null) continue;

                        BitmapLayer add = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);

                        this.Nodes.Insert(indexChild, add);
                        this.ObservableCollection.InsertChild(index, add);
                        count++;
                    }
                }
                else
                {
                    int indexChild = parent.Children.IndexOf(neighbor);

                    foreach (IRandomAccessStreamReference item in items)
                    {
                        CanvasBitmap bitmap = await this.CreateBitmap(item);
                        if (bitmap is null) continue;

                        BitmapLayer add = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);

                        parent.Children.Insert(indexChild, add);
                        this.ObservableCollection.InsertChild(index, add);
                        count++;
                    }
                }

                this.LayerSelectedIndex = index;
            }
            else
            {
                foreach (IRandomAccessStreamReference item in items)
                {
                    CanvasBitmap bitmap = await this.CreateBitmap(item);
                    if (bitmap is null) continue;

                    BitmapLayer add = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);

                    this.Nodes.Insert(0, add);
                    this.ObservableCollection.InsertChild(0, add);
                    count++;
                }

                this.LayerSelectedIndex = 0;
            }

            /// History
            Layerage[] redo = this.Nodes.Convert();
            int removes = this.History.Push(new ArrangeHistory(undo, redo));

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

        private async Task<CanvasBitmap> CreateBitmap(IRandomAccessStreamReference item)
        {
            if (item is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await item.OpenReadAsync())
                {
                    return await CanvasBitmap.LoadAsync(this.CanvasDevice, stream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}