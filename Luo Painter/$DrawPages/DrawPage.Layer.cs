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
    public sealed partial class DrawPage : Page
    {

        bool HasChangedLayers;

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
            this.LayerListView.DragItemsStarting += (s, e) =>
            {
                if (this.HasChangedLayers) return;
                this.HasChangedLayers = true;

                foreach (ILayer item in this.ObservableCollection)
                {
                    this.LayerTool.ChangedLayers.Add(item);
                }
            };
            this.LayerListView.DragItemsCompleted += (s, e) =>
            {
                switch (e.DropResult)
                {
                    case Windows.ApplicationModel.DataTransfer.DataPackageOperation.None:
                        break;
                    case Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy:
                        break;
                    case Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move:
                        {
                            if (this.HasChangedLayers == false) break;
                            this.HasChangedLayers = false;

                            if (this.LayerTool.ChangedLayers.Count == 0) break;

                            // History
                            string[] undo = this.LayerTool.ChangedLayers.Select(c => c.Id).ToArray();
                            string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                            int removes = this.History.Push(new ArrangeHistory(undo, redo));

                            this.LayerTool.ChangedLayers.Clear();
                            this.CanvasControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                    case Windows.ApplicationModel.DataTransfer.DataPackageOperation.Link:
                        break;
                    default:
                        break;
                }
            };

            this.LayerListView.VisualClick += (s, layer) =>
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
                this.LayerFlyout.ShowAt(this.LayerListView.PlacementTarget);
            };

            this.LayerFlyout.Closed += (s, e) =>
            {
                this.LayerTool.Closed();
            };

            this.LayerFlyout.Opened += (s, e) =>
            {
                this.LayerTool.SetLayer(this.LayerListView.SelectedItem as ILayer);
                this.LayerTool.Opened();
            };
        }

        private void ConstructLayer()
        {
            this.LayerListView.AddClick += (s, e) =>
            {
                string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

                BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasDevice, this.Transformer.Width, this.Transformer.Height);
                this.Layers.Add(bitmapLayer.Id, bitmapLayer);
                this.Add(bitmapLayer);

                // History
                string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                int removes = this.History.Push(new ArrangeHistory(undo, redo));

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };

            //this.SelectAllButton.Click += (s, e) => this.LayerListView.SelectAll();

            this.LayerTool.NameChanged += (hasChanged, name) =>
            {
                if (hasChanged)
                {
                    foreach (ILayer item in this.LayerTool.ChangedLayers)
                    {
                        item.Name = name;
                    }
                }
                else
                {
                    foreach (object item in this.LayerListView.SelectedItems)
                    {
                        if (item is ILayer layer)
                        {
                            this.LayerTool.ChangedLayers.Add(layer);
                            layer.CacheName();
                            layer.Name = name;
                        }
                    }
                }
            };
            this.LayerTool.OpacityChanged += (hasChanged, opacity) =>
            {
                if (hasChanged)
                {
                    foreach (ILayer item in this.LayerTool.ChangedLayers)
                    {
                        item.Opacity = opacity;
                    }
                    this.CanvasControl.Invalidate(); // Invalidate
                }
                else
                {
                    foreach (object item in this.LayerListView.SelectedItems)
                    {
                        if (item is ILayer layer)
                        {
                            this.LayerTool.ChangedLayers.Add(layer);
                            layer.CacheOpacity();
                            layer.Opacity = opacity;
                        }
                    }
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };
            this.LayerTool.BlendModeChanged += (hasChanged, blendMode) =>
            {
                if (hasChanged)
                {
                    foreach (ILayer item in this.LayerTool.ChangedLayers)
                    {
                        item.BlendMode = blendMode;
                    }
                    this.CanvasControl.Invalidate(); // Invalidate
                }
                else
                {
                    foreach (object item in this.LayerListView.SelectedItems)
                    {
                        if (item is ILayer layer)
                        {
                            this.LayerTool.ChangedLayers.Add(layer);
                            layer.CacheBlendMode();
                            layer.BlendMode = blendMode;
                        }
                    }
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };

            this.LayerTool.NameHistory += (undo, redo) =>
            {
                if (this.LayerTool.ChangedLayers.Count > 1)
                {
                    IDictionary<string, string> undoParameters = new Dictionary<string, string>();

                    foreach (ILayer item in this.LayerTool.ChangedLayers)
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
                    foreach (ILayer item in this.LayerTool.ChangedLayers)
                    {
                        // History
                        int removes = this.History.Push(new PropertyHistory<string>(HistoryType.Name, item.Id, item.StartingName, redo));
                    }
                }

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
            this.LayerTool.OpacityHistory += (undo, redo) =>
            {
                if (this.LayerTool.ChangedLayers.Count > 1)
                {
                    IDictionary<string, float> undoParameters = new Dictionary<string, float>();

                    foreach (ILayer item in this.LayerTool.ChangedLayers)
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
                    foreach (ILayer item in this.LayerTool.ChangedLayers)
                    {
                        // History
                        int removes = this.History.Push(new PropertyHistory<float>(HistoryType.Opacity, item.Id, item.StartingOpacity, redo));
                    }
                }

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
            this.LayerTool.BlendModeHistory += (undo, redo) =>
            {
                if (this.LayerTool.ChangedLayers.Count > 1)
                {
                    IDictionary<string, BlendEffectMode?> undoParameters = new Dictionary<string, BlendEffectMode?>();

                    foreach (ILayer item in this.LayerTool.ChangedLayers)
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
                    foreach (ILayer item in this.LayerTool.ChangedLayers)
                    {
                        // History
                        int removes = this.History.Push(new PropertyHistory<BlendEffectMode?>(HistoryType.BlendMode, item.Id, item.StartingBlendMode, redo));
                    }
                }

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
        }

    }
}