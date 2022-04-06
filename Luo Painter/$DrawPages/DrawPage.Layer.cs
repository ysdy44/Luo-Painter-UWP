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
        private IEnumerable<string> Ids()
        {
            foreach (object item in this.LayerListView.SelectedItems)
            {
                if (item is ILayer layer)
                {
                    yield return layer.Id;
                }
            }
        }

        private void ConstructLayers()
        {
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

                foreach (string id in this.Ids().ToArray())
                {
                    if (this.ObservableCollection.FirstOrDefault(c => c.Id == id) is ILayer layer)
                    {
                        this.ObservableCollection.Remove(layer);
                    }
                }

                int index = Math.Min(startingIndex, this.ObservableCollection.Count - 1);
                this.LayerListView.SelectedIndex = index;

                this.CanvasControl.Invalidate(); // Invalidate
            };

            this.AddButton.Click += (s, e) =>
            {
                int index = this.LayerListView.SelectedIndex;

                ICanvasResourceCreator sender = this.CanvasControl;
                BitmapLayer bitmapLayer = new BitmapLayer(sender, this.Transformer.Width, this.Transformer.Height);

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

            this.SelectAllButton.Click += (s, e) => this.LayerListView.SelectAll();
        }

    }
}