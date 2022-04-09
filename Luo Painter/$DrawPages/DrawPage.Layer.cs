using Luo_Painter.Blends;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
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

        private void SetLayer(ILayer layer)
        {
            if (layer == null)
            {
                this.LayerImage.Source = null;
                this.LayerTextBox.Text = string.Empty;
                this.OpacitySlider.Value = 100;
                this.OpacitySlider.IsEnabled = false;
                this.BlendModeListView.IsEnabled = false;
                this.BlendModeListView.SelectedIndex = -1;
            }
            else
            {
                this.LayerImage.Source = layer.Thumbnail;
                this.LayerTextBox.Text = layer.Name ?? string.Empty;
                this.OpacitySlider.IsEnabled = true;
                this.OpacitySlider.Value = layer.Opacity * 100;
                this.BlendModeListView.IsEnabled = true;
                this.BlendModeListView.SelectedIndex = layer.BlendMode.HasValue ? this.BlendCollection.IndexOf(layer.BlendMode.Value) : 0;
            }
        }

        private void ConstructLayers()
        {
            this.VisualCommand.Click += (s, layer) =>
            {
                // History
                int removes2 = this.History.Push(layer.GetVisibilityHistory());
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.LayerButton.Click += (s, e) =>
            {
                this.SetLayer(this.LayerListView.SelectedItem as ILayer);
                this.LayerFlyout.ShowAt(this.AddButton);
            };
        }

        private void ConstructLayer()
        {
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                float opacity = (float)(e.NewValue / 100);
                foreach (object item in this.LayerListView.SelectedItems)
                {
                    if (item is ILayer layer)
                    {
                        layer.Opacity = opacity;
                    }
                }

                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item)
                {
                    bool isNone = item.IsNone();

                    foreach (object item2 in this.LayerListView.SelectedItems)
                    {
                        if (item2 is ILayer layer)
                        {
                            if (isNone) layer.BlendMode = null;
                            else layer.BlendMode = item;
                        }
                    }

                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };

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
                this.SetLayer(this.LayerListView.SelectedItem as ILayer);

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

            //this.SelectAllButton.Click += (s, e) => this.LayerListView.SelectAll();
        }

    }
}