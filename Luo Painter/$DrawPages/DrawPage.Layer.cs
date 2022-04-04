using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        private void ConstructLayers()
        {
            this.LayerListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is ILayer item)
                {
                    foreach (ILayer item2 in this.ObservableCollection)
                    {
                        if (item.Id != item2.Id)
                        {
                            if (item2.IsSelected) item2.IsSelected = false;
                        }
                    }

                    if (item.IsSelected == false) item.IsSelected = true;
                    this.BitmapLayer = item as BitmapLayer;
                }
            };

            this.SelectCommand.Click += (s, layer) =>
            {
                if (layer.IsSelected == false) layer.IsSelected = true;

                int index = this.ObservableCollection.IndexOf(layer);
                this.LayerListView.SelectRange(new ItemIndexRange(index, 1));
            };
            this.VisualCommand.Click += (s, layer) =>
            {
                switch (layer.Visibility)
                {
                    case Visibility.Visible:
                        layer.Visibility = Visibility.Collapsed;
                        break;
                    case Visibility.Collapsed:
                        layer.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

        private void ConstructLayer()
        {
            this.ClearButton.Click += (s, e) =>
            {
                int index = this.LayerListView.SelectedIndex;
                if (index < 0) return;
                if (index >= this.ObservableCollection.Count) return;

                if (this.ObservableCollection[index] is BitmapLayer bitmapLayer)
                {
                    // History
                    int removes2 = this.History.Push(bitmapLayer.GetBitmapClearHistory());
                    bitmapLayer.Clear(Colors.Transparent);
                    bitmapLayer.ClearThumbnail(Colors.Transparent);
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };

            this.RemoveButton.Click += (s, e) =>
            {
                int startingIndex = this.LayerListView.SelectedIndex;

                IEnumerable<string> ids =
                from layer
                in this.ObservableCollection
                where layer.IsSelected
                select layer.Id;

                foreach (string id in ids.ToArray())
                {
                    if (this.ObservableCollection.FirstOrDefault(c => c.Id == id) is ILayer layer)
                    {
                        this.ObservableCollection.Remove(layer);
                    }
                }

                int index = Math.Min(startingIndex, this.ObservableCollection.Count - 1);
                this.LayerListView.SelectedIndex = index;

                if (index >= 0)
                {
                    ILayer layer2 = this.ObservableCollection[index];
                    if (layer2.IsSelected == false) layer2.IsSelected = true;
                    this.BitmapLayer = layer2 as BitmapLayer;
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.AddButton.Click += (s, e) =>
            {
                int index = this.LayerListView.SelectedIndex;
                ICanvasResourceCreator sender = this.CanvasControl;

                foreach (ILayer item2 in this.ObservableCollection)
                {
                    if (item2.IsSelected) item2.IsSelected = false;
                }

                BitmapLayer bitmapLayer = new BitmapLayer(sender, this.Transformer.Width, this.Transformer.Height)
                {
                    IsSelected = true
                };
                this.BitmapLayer = bitmapLayer;

                if (index >= 0)
                {
                    this.ObservableCollection.Insert(index, bitmapLayer);
                    this.LayerListView.SelectedIndex = index;
                }
                else
                {
                    this.ObservableCollection.Add(bitmapLayer);
                    this.LayerListView.SelectedIndex = 0;
                }
            };
        }

    }
}