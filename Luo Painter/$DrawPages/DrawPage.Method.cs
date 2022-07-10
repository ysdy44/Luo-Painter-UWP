using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        private ICanvasImage Transparent => new CanvasCommandList(this.CanvasDevice);
        private ICanvasImage White => new ColorSourceEffect
        {
            Color = Colors.White
        };

        /// <summary>
        /// Export to ...
        /// </summary>
        private async Task<bool?> Export()
        {
            bool isClearWhite = this.ExportMenu.FileFormat == CanvasBitmapFileFormat.Jpeg;

            // Export
            return await FileUtil.SaveCanvasImageFile
            (
                resourceCreator: this.CanvasDevice,
                image: this.Nodes.Render(isClearWhite ? this.White : this.Transparent),

                width: this.Transformer.Width,
                height: this.Transformer.Height,
                dpi: this.ExportMenu.DPI,

                fileChoices: this.ExportMenu.FileChoices,
                suggestedFileName: this.ApplicationView.Title,

                fileFormat: this.ExportMenu.FileFormat,
                quality: 1
            );
        }


        private int FillContainsPoint(Vector2 point)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];
                if (item.FillContainsPoint(point)) return i;
            }

            return -1;
        }

        private bool TryFillContainsPoint(Vector2 point, out ILayer layer)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];
                if (item.FillContainsPoint(point))
                {
                    layer = item;
                    return true;
                }
            }

            layer = null;
            return false;
        }


        public async void AddAsync() => this.AddAsync(await FileUtil.PickMultipleImageFilesAsync(PickerLocationId.Desktop));
     
        public async void AddAsync(IEnumerable<IStorageFile> items)
        {
            if (items is null) return;
            if (items.Count() == 0) return;

            Layerage[] undo = this.Nodes.Convert();

            int count = 0;
            int index = this.LayerListView.SelectedIndex;
            if (index > 0 && this.LayerListView.SelectedItem is ILayer neighbor)
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
                        this.Layers.Push(add);

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
                        this.Layers.Push(add);

                        parent.Children.Insert(indexChild, add);
                        this.ObservableCollection.InsertChild(index, add);
                        count++;
                    }
                }

                this.LayerListView.SelectedIndex = index;
            }
            else
            {
                foreach (IRandomAccessStreamReference item in items)
                {
                    CanvasBitmap bitmap = await this.CreateBitmap(item);
                    if (bitmap is null) continue;

                    BitmapLayer add = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);
                    this.Layers.Push(add);

                    this.Nodes.Insert(0, add);
                    this.ObservableCollection.InsertChild(0, add);
                    count++;
                }

                this.LayerListView.SelectedIndex = 0;
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