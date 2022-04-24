using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
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
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter
{
    internal class LayerCommand : RelayCommand<ILayer>
    {
    }

    public sealed partial class DrawPage : Page
    {

        IList<ILayer> ChangedLayers = new List<ILayer>();
        bool HasChangedLayers;
        bool NameChanged;
        bool OpacityChanged;
        bool BlendModeChanged;

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
                Visibility undo = layer.Visibility;
                Visibility redo = layer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                string[] ids = this.Ids().ToArray();
                if (ids.Contains(layer.Id) && ids.Length > 1)
                {
                    IDictionary<string, Visibility> undoParameters = new Dictionary<string, Visibility>();

                    foreach (string id in ids)
                    {
                        if (this.Layers.ContainsKey(id))
                        {
                            ILayer layer2 = this.Layers[id];
                            if (layer2.Visibility != redo)
                            {
                                undoParameters.Add(id, layer2.Visibility);
                                layer2.Visibility = redo;
                            }
                        }
                    }

                    // History
                    int removes = this.History.Push(new PropertysHistory<Visibility>(HistoryType.Visibilitys, HistoryType.Visibility, undoParameters, undo, redo));
                }
                else
                {
                    // History
                    layer.Visibility = redo;
                    int removes = this.History.Push(new PropertyHistory<Visibility>(HistoryType.Visibility, layer.Id, undo, redo));
                }

                this.CanvasControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };

            this.LayerButton.Click += (s, e) =>
            {
                this.LayerFlyout.ShowAt(this.AddButton);
            };

            this.LayerFlyout.Closed += (s, e) =>
            {
                this.LayerTextBox.TextChanged -= this.LayerTextBox_TextChanged;
                this.OpacitySlider.ValueChanged -= this.OpacitySlider_ValueChanged;
                this.BlendModeListView.ItemClick -= this.BlendModeListView_ItemClick;

                if (this.HasChangedLayers == false) return;
                this.HasChangedLayers = false;

                if (this.ChangedLayers.Count == 0) return;

                if (this.NameChanged)
                {
                    this.NameChanged = false;
                    string redo = this.LayerTextBox.Text;
                    this.NameHistory(string.Empty, redo);
                }

                if (this.OpacityChanged)
                {
                    this.OpacityChanged = false;
                    float redo = (float)(this.OpacitySlider.Value / 100);
                    this.OpacityHistory(1, redo);
                }

                if (this.BlendModeChanged)
                {
                    this.BlendModeChanged = false;
                    if (this.BlendModeListView.SelectedItem is BlendEffectMode item)
                    {
                        if (item.IsNone()) this.BlendModeHistory(null, null);
                        else this.BlendModeHistory(null, item);
                    }
                    else this.BlendModeHistory(null, null);
                }

                this.ChangedLayers.Clear();
            };

            this.LayerFlyout.Opened += (s, e) =>
            {
                this.SetLayer(this.LayerListView.SelectedItem as ILayer);

                this.LayerTextBox.TextChanged -= this.LayerTextBox_TextChanged;
                this.OpacitySlider.ValueChanged -= this.OpacitySlider_ValueChanged;
                this.BlendModeListView.ItemClick -= this.BlendModeListView_ItemClick;

                this.LayerTextBox.TextChanged += this.LayerTextBox_TextChanged;
                this.OpacitySlider.ValueChanged += this.OpacitySlider_ValueChanged;
                this.BlendModeListView.ItemClick += this.BlendModeListView_ItemClick;
            };
        }

        private void ConstructLayer()
        {
            this.ClearButton.Click += (s, e) =>
            {
                if (this.LayerListView.SelectedItem is BitmapLayer bitmapLayer)
                {
                    // History
                    int removes = this.History.Push(bitmapLayer.GetBitmapClearHistory(Colors.Transparent));
                    bitmapLayer.Clear(Colors.Transparent);
                    bitmapLayer.ClearThumbnail(Colors.Transparent);

                    this.CanvasControl.Invalidate(); // Invalidate

                    this.UndoButton.IsEnabled = this.History.CanUndo;
                    this.RedoButton.IsEnabled = this.History.CanRedo;
                }
            };

            this.RemoveButton.Click += (s, e) =>
            {
                int startingIndex = this.LayerListView.SelectedIndex;

                foreach (string id in this.Ids().ToArray())
                {
                    if (this.Layers.ContainsKey(id))
                    {
                        ILayer layer = this.Layers[id];
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
                string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

                BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasControl, this.Transformer.Width, this.Transformer.Height);
                this.Layers.Add(bitmapLayer.Id, bitmapLayer);
                this.Add(bitmapLayer);

                // History
                string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                int removes = this.History.Push(new ArrangeHistory(undo, redo));

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };

            //this.SelectAllButton.Click += (s, e) => this.LayerListView.SelectAll();
        }


        private void LayerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = this.LayerTextBox.Text;

            if (this.HasChangedLayers == false)
            {
                this.HasChangedLayers = true;
                this.NameChanged = true;

                foreach (object item in this.LayerListView.SelectedItems)
                {
                    if (item is ILayer layer)
                    {
                        this.ChangedLayers.Add(layer);
                        layer.CacheName();
                        layer.Name = name;
                    }
                }
            }
            else
            {
                foreach (ILayer item in this.ChangedLayers)
                {
                    item.Name = name;
                }
            }
        }

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            float opacity = (float)(e.NewValue / 100);

            if (this.HasChangedLayers == false)
            {
                this.HasChangedLayers = true;
                this.OpacityChanged = true;

                foreach (object item in this.LayerListView.SelectedItems)
                {
                    if (item is ILayer layer)
                    {
                        this.ChangedLayers.Add(layer);
                        layer.CacheOpacity();
                        layer.Opacity = opacity;
                    }
                }
                this.CanvasControl.Invalidate(); // Invalidate
            }
            else
            {
                foreach (ILayer item in this.ChangedLayers)
                {
                    item.Opacity = opacity;
                }
                this.CanvasControl.Invalidate(); // Invalidate
            }
        }

        private void BlendModeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is BlendEffectMode item2)
            {
                BlendEffectMode? blendMode = item2.IsNone() ? (BlendEffectMode?)null : item2;

                if (this.HasChangedLayers == false)
                {
                    this.HasChangedLayers = true;
                    this.BlendModeChanged = true;

                    foreach (object item in this.LayerListView.SelectedItems)
                    {
                        if (item is ILayer layer)
                        {
                            this.ChangedLayers.Add(layer);
                            layer.CacheBlendMode();
                            layer.BlendMode = blendMode;
                        }
                    }
                    this.CanvasControl.Invalidate(); // Invalidate
                }
                else
                {
                    foreach (ILayer item in this.ChangedLayers)
                    {
                        item.BlendMode = blendMode;
                    }
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            }
        }


        private void NameHistory(string undo, string redo)
        {
            if (this.ChangedLayers.Count > 1)
            {
                IDictionary<string, string> undoParameters = new Dictionary<string, string>();

                foreach (ILayer item in this.ChangedLayers)
                {
                    if (item.Name != redo)
                    {
                        undoParameters.Add(item.Id, item.StartingName);
                    }
                }

                // History
                int removes = this.History.Push(new PropertysHistory<string>(HistoryType.Names, HistoryType.Name, undoParameters, undo, redo));
            }
            else
            {
                foreach (ILayer item in this.ChangedLayers)
                {
                    // History
                    int removes = this.History.Push(new PropertyHistory<string>(HistoryType.Name, item.Id, item.StartingName, redo));
                }
            }

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

        private void OpacityHistory(float undo, float redo)
        {
            if (this.ChangedLayers.Count > 1)
            {
                IDictionary<string, float> undoParameters = new Dictionary<string, float>();

                foreach (ILayer item in this.ChangedLayers)
                {
                    if (item.Opacity != redo)
                    {
                        undoParameters.Add(item.Id, item.StartingOpacity);
                    }
                }

                // History
                int removes = this.History.Push(new PropertysHistory<float>(HistoryType.Opacitys, HistoryType.Opacity, undoParameters, undo, redo));
            }
            else
            {
                foreach (ILayer item in this.ChangedLayers)
                {
                    // History
                    int removes = this.History.Push(new PropertyHistory<float>(HistoryType.Opacity, item.Id, item.StartingOpacity, redo));
                }
            }

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

        private void BlendModeHistory(BlendEffectMode? undo, BlendEffectMode? redo)
        {
            if (this.ChangedLayers.Count > 1)
            {
                IDictionary<string, BlendEffectMode?> undoParameters = new Dictionary<string, BlendEffectMode?>();

                foreach (ILayer item in this.ChangedLayers)
                {
                    if (item.BlendMode != redo)
                    {
                        undoParameters.Add(item.Id, item.StartingBlendMode);
                    }
                }

                // History
                int removes = this.History.Push(new PropertysHistory<BlendEffectMode?>(HistoryType.BlendModes, HistoryType.BlendMode, undoParameters, undo, redo));
            }
            else
            {
                foreach (ILayer item in this.ChangedLayers)
                {
                    // History
                    int removes = this.History.Push(new PropertyHistory<BlendEffectMode?>(HistoryType.BlendMode, item.Id, item.StartingBlendMode, redo));
                }
            }

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
        }

    }
}