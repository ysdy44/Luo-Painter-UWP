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
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
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
            bool isClearWhite = this.ExportButton.FileFormat == CanvasBitmapFileFormat.Jpeg;

            // Export
            return await FileUtil.SaveCanvasImageFile
            (
                resourceCreator: this.CanvasDevice,
                image: this.Render(isClearWhite ? this.White : this.Transparent),

                width: this.Transformer.Width,
                height: this.Transformer.Height,
                dpi: this.ExportButton.DPI,

                fileChoices: this.ExportButton.FileChoices,
                suggestedFileName: this.ApplicationView.Title,

                fileFormat: this.ExportButton.FileFormat,
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


        private ICanvasImage Render(ICanvasImage background)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                background = item.Render(background, item.Source);
            }
            return background;
        }

        private ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                background = item.Render(background, new Transform2DEffect
                {
                    InterpolationMode = interpolationMode,
                    TransformMatrix = matrix,
                    Source = item.Source,
                });
            }
            return background;
        }

        private ICanvasImage Render(ICanvasImage background, Matrix3x2 matrix, CanvasImageInterpolation interpolationMode, string id, ICanvasImage mezzanine)
        {
            for (int i = this.ObservableCollection.Count - 1; i >= 0; i--)
            {
                ILayer item = this.ObservableCollection[i];

                background = item.Render(background, new Transform2DEffect
                {
                    InterpolationMode = interpolationMode,
                    TransformMatrix = matrix,
                    Source = (item.Id == id) ? mezzanine : item.Source,
                });
            }
            return background;
        }


        public void Remove()
        {
            int index = this.LayerListView.SelectedIndex;
            if (index < 0) return;
            if (index + 1 > this.ObservableCollection.Count) return;

            foreach (string id in this.Ids().ToArray())
            {
                if (this.Layers.ContainsKey(id))
                {
                    ILayer layer = this.Layers[id];
                    this.ObservableCollection.Remove(layer);
                }
            }

            int index2 = System.Math.Min(index, this.ObservableCollection.Count - 1);
            this.LayerListView.SelectedIndex = index2;

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        public async void AddAsync(IEnumerable<IStorageFile> items)
        {
            if (items is null) return;
            if (items.Count() == 0) return;

            int count = 0;
            int index = this.LayerListView.SelectedIndex;
            string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

            foreach (IRandomAccessStreamReference item in items)
            {
                CanvasBitmap bitmap = null;
                try
                {
                    using (IRandomAccessStreamWithContentType stream = await item.OpenReadAsync())
                    {
                        bitmap = await CanvasBitmap.LoadAsync(this.CanvasDevice, stream);
                    }
                }
                catch (Exception)
                {
                }
                if (bitmap is null) continue;

                BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasDevice, bitmap, this.Transformer.Width, this.Transformer.Height);
                this.Layers.Add(bitmapLayer.Id, bitmapLayer);
                if (index >= 0)
                    this.ObservableCollection.Insert(index + count, bitmapLayer);
                else
                    this.ObservableCollection.Add(bitmapLayer);

                count++;
            }

            if (count == 0) return;
            if (count > 1) this.Tip("Add Images", $"{count}"); // Tip

            // History
            string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
            int removes = this.History.Push(new ArrangeHistory(undo, redo));

            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
            this.LayerListView.SelectedIndex = System.Math.Max(0, index);
        }


        public async Task<CanvasBitmap> AddAsync(IRandomAccessStreamReference reference)
        {
            if (reference is null) return null;

            try
            {
                using (IRandomAccessStreamWithContentType stream = await reference.OpenReadAsync())
                {
                    return await CanvasBitmap.LoadAsync(this.CanvasDevice, stream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Add(ILayer layer)
        {
            int index = this.LayerListView.SelectedIndex;

            if (index >= 0)
            {
                this.ObservableCollection.Insert(index, layer);
                this.LayerListView.SelectedIndex = index;
            }
            else
            {
                this.ObservableCollection.Add(layer);
                this.LayerListView.SelectedIndex = 0;
            }
        }

    }
}