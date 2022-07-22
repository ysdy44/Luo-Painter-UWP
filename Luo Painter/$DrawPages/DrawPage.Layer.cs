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
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        IEnumerable<string> ChangedLayers;
        bool HasChangedLayers;


        readonly IDictionary<string, string> NameUndoParameters = new Dictionary<string, string>();
        private IDictionary<string, string> CloneNameUndoParameters()
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in this.NameUndoParameters)
            {
                dic.Add(item);
            }
            return dic;
        }

        readonly IDictionary<string, BlendEffectMode?> BlendModeUndoParameters = new Dictionary<string, BlendEffectMode?>();
        private IDictionary<string, BlendEffectMode?> CloneBlendModeUndoParameters()
        {
            IDictionary<string, BlendEffectMode?> dic = new Dictionary<string, BlendEffectMode?>();
            foreach (var item in this.BlendModeUndoParameters)
            {
                dic.Add(item);
            }
            return dic;
        }

        float StartingOpacity;
        readonly IDictionary<string, float> OpacityUndoParameters = new Dictionary<string, float>();
        private IDictionary<string, float> CloneOpacityUndoParameters()
        {
            IDictionary<string, float> dic = new Dictionary<string, float>();
            foreach (var item in this.OpacityUndoParameters)
            {
                dic.Add(item);
            }
            return dic;
        }


        private IEnumerable<string> Ids()
        {
            foreach (object item in this.LayerSelectedItems)
            {
                if (item is ILayer layer)
                {
                    yield return layer.Id;
                }
            }
        }


        private void ConstructLayers()
        {
            this.LayerListView.SelectedItemChanged += (s, item) => this.LayerMenu.SetSelectedItem(item);

            this.LayerListView.DragItemsStarting += (s, e) =>
            {
                if (this.HasChangedLayers) return;
                this.HasChangedLayers = true;

                this.ChangedLayers = this.ObservableCollection.Select(c => c.Id);
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

                            if (this.ChangedLayers.Count() == 0) break;

                            // History
                            string[] undo = this.ChangedLayers.ToArray();
                            string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                            int removes = this.History.Push(new ArrangeHistory(undo, redo));

                            this.ChangedLayers = null;
                            this.CanvasVirtualControl.Invalidate(); // Invalidate

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
                        if (LayerDictionary.Instance.ContainsKey(id))
                        {
                            ILayer layer2 = LayerDictionary.Instance[id];
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

                this.CanvasVirtualControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
        }

        private void ConstructLayer()
        {
            this.LayerMenu.NameHistory += (undo, redo) =>
            {
                this.NameUndoParameters.Clear();

                foreach (object item in this.LayerSelectedItems)
                {
                    if (item is ILayer layer)
                    {
                        if (layer.Name != redo)
                        {
                            layer.CacheName();
                            layer.Name = redo;

                            this.NameUndoParameters.Add(layer.Id, layer.StartingName);
                        }
                    }
                }

                switch (this.NameUndoParameters.Count)
                {
                    case 0:
                        break;
                    case 1:
                        foreach (var item in this.NameUndoParameters)
                        {
                            // History
                            int removes = this.History.Push(new PropertyHistory<string>(HistoryType.Name, item.Key, item.Value, redo));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                            break;
                        }
                        break;
                    default:
                        {
                            // History
                            int removes = this.History.Push(new PropertysHistory<string>(HistoryType.Names, HistoryType.Name, this.CloneNameUndoParameters(), undo, redo));

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };

            this.LayerMenu.BlendModeHistory += (undo, redo) =>
            {
                this.BlendModeUndoParameters.Clear();

                foreach (object item in this.LayerSelectedItems)
                {
                    if (item is ILayer layer)
                    {
                        if (layer.BlendMode != redo)
                        {
                            layer.CacheBlendMode();
                            layer.BlendMode = redo;

                            this.BlendModeUndoParameters.Add(layer.Id, layer.StartingBlendMode);
                        }
                    }
                }

                switch (this.BlendModeUndoParameters.Count)
                {
                    case 0:
                        break;
                    case 1:
                        foreach (var item in this.BlendModeUndoParameters)
                        {
                            // History
                            int removes = this.History.Push(new PropertyHistory<BlendEffectMode?>(HistoryType.BlendMode, item.Key, item.Value, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                            break;
                        }
                        break;
                    default:
                        {
                            // History
                            int removes = this.History.Push(new PropertysHistory<BlendEffectMode?>(HistoryType.BlendModes, HistoryType.BlendMode, this.CloneBlendModeUndoParameters(), undo, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };

            this.LayerMenu.OpacitySlider.ValueChangedStarted += (s, e) =>
            {
                this.StartingOpacity = (float)(this.LayerMenu.OpacitySlider.Value / 100);

                this.OpacityUndoParameters.Clear();

                foreach (object item in this.LayerSelectedItems)
                {
                    if (item is ILayer layer)
                    {
                        layer.CacheOpacity();

                        this.OpacityUndoParameters.Add(layer.Id, layer.StartingOpacity);
                    }
                }
            };

            this.LayerMenu.OpacitySlider.ValueChangedDelta += (s, e) =>
            {
                float opacity = (float)(e.NewValue / 100);
                foreach (object item in this.LayerSelectedItems)
                {
                    if (item is ILayer layer)
                    {
                        layer.Opacity = opacity;
                    }
                }
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };

            this.LayerMenu.OpacitySlider.ValueChangedCompleted += (s, e) =>
            {
                float undo = (float)(this.StartingOpacity / 100);
                float redo = (float)(this.LayerMenu.OpacitySlider.Value / 100);

                switch (this.OpacityUndoParameters.Count)
                {
                    case 0:
                        break;
                    case 1:
                        foreach (var item in this.OpacityUndoParameters)
                        {
                            // History
                            int removes = this.History.Push(new PropertyHistory<float>(HistoryType.Opacity, item.Key, item.Value, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                            break;
                        }
                        break;
                    default:
                        {
                            // History
                            int removes = this.History.Push(new PropertysHistory<float>(HistoryType.Opacitys, HistoryType.Opacity, this.CloneOpacityUndoParameters(), undo, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };

            this.LayerMenu.OpacityHistory += (undo, redo) =>
            {
                this.OpacityUndoParameters.Clear();

                foreach (object item in this.LayerSelectedItems)
                {
                    if (item is ILayer layer)
                    {
                        layer.CacheOpacity();
                        layer.Opacity = redo;
                        this.OpacityUndoParameters.Add(layer.Id, layer.StartingOpacity);
                    }
                }

                switch (this.OpacityUndoParameters.Count)
                {
                    case 0:
                        break;
                    case 1:
                        foreach (var item in this.OpacityUndoParameters)
                        {
                            // History
                            int removes = this.History.Push(new PropertyHistory<float>(HistoryType.Opacity, item.Key, item.Value, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                            break;
                        }
                        break;
                    default:
                        {
                            // History
                            int removes = this.History.Push(new PropertysHistory<float>(HistoryType.Opacitys, HistoryType.Opacity, this.CloneOpacityUndoParameters(), undo, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.UndoButton.IsEnabled = this.History.CanUndo;
                            this.RedoButton.IsEnabled = this.History.CanRedo;
                        }
                        break;
                }
            };
        }

    }
}