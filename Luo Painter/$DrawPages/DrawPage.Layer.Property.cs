using Luo_Painter.Brushes;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        bool IsPropertyEnabled;

        int OpacityCount = 0;
        int BlendModeCount = 0;
        int NameCount = 0;

        private void ConstructPropertys()
        {
            this.LayerButton.Closed += (s, e) =>
            {
                switch (this.OpacityCount)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.LayerListView.SelectedItem is ILayer layer)
                        {
                            float redo = this.LayerButton.OpacityValue;

                            // History
                            int removes = this.History.Push(new PropertyHistory<float>(HistoryType.Opacity, layer.Id, layer.StartingOpacity, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate
                            this.RaiseHistoryCanExecuteChanged();
                        }
                        break;
                    default:
                        // History
                        int removes2 = this.History.Push(new CompositeHistory(this.GetOpacityHistory().ToArray()));
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        this.RaiseHistoryCanExecuteChanged();
                        break;
                }

                switch (this.BlendModeCount)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.LayerListView.SelectedItem is ILayer layer)
                        {
                            BlendEffectMode? redo = this.LayerButton.BlendModeValue;

                            // History
                            int removes = this.History.Push(new PropertyHistory<BlendEffectMode?>(HistoryType.BlendMode, layer.Id, layer.StartingBlendMode, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate
                            this.RaiseHistoryCanExecuteChanged();
                        }
                        break;
                    default:
                        // History
                        int removes2 = this.History.Push(new CompositeHistory(this.GetBlendModeHistory().ToArray()));
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        this.RaiseHistoryCanExecuteChanged();
                        break;
                }

                switch (this.NameCount)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.LayerListView.SelectedItem is ILayer layer)
                        {
                            string redo = this.LayerButton.NameValue;

                            // History
                            int removes = this.History.Push(new PropertyHistory<string>(HistoryType.Name, layer.Id, layer.StartingName, redo));
                            this.CanvasVirtualControl.Invalidate(); // Invalidate
                            this.RaiseHistoryCanExecuteChanged();
                        }
                        break;
                    default:
                        // History
                        int removes2 = this.History.Push(new CompositeHistory(this.GetNameHistory().ToArray()));
                        this.CanvasVirtualControl.Invalidate(); // Invalidate
                        this.RaiseHistoryCanExecuteChanged();
                        break;
                }
            };

            this.LayerButton.Opened += (s, e) =>
            {
                this.IsPropertyEnabled = false;
                if (this.LayerListView.SelectedItem is ILayer layer)
                    this.LayerButton.Construct(layer);
                else
                    this.LayerButton.Clear();
                this.IsPropertyEnabled = true;

                this.OpacityCount = 0;
                this.BlendModeCount = 0;
                this.NameCount = 0;
            };
        }

        private void ConstructProperty()
        {
            this.LayerButton.OpacityChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                float redo = (float)(e.NewValue / 100);

                if (this.OpacityCount is 0)
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        this.OpacityCount++;
                        item.CacheOpacity();
                        item.Opacity = redo;
                    }
                }
                else
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        item.Opacity = redo;
                    }
                }

                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.LayerButton.BlendModeChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                BlendEffectMode? redo = this.LayerButton.BlendModeValue;

                if (this.BlendModeCount is 0)
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        this.BlendModeCount++;
                        item.CacheBlendMode();
                        item.BlendMode = redo;
                    }
                }
                else
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        item.BlendMode = redo;
                    }
                }

                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.LayerButton.NameChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                string redo = this.LayerButton.NameValue;

                if (this.NameCount is 0)
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        this.NameCount++;
                        item.CacheName();
                        item.Name = redo;
                    }
                }
                else
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        item.Name = redo;
                    }
                }
            };
        }

        private IEnumerable<IHistory> GetOpacityHistory()
        {
            float redo = this.LayerButton.OpacityValue;

            foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
            {
                if (item.StartingOpacity == redo) continue;

                yield return new PropertyHistory<float>(HistoryType.Opacity, item.Id, item.StartingOpacity, redo);
            }
        }
        private IEnumerable<IHistory> GetBlendModeHistory()
        {
            BlendEffectMode? redo = this.LayerButton.BlendModeValue;

            foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
            {
                if (item.StartingBlendMode == redo) continue;

                yield return new PropertyHistory<BlendEffectMode?>(HistoryType.BlendMode, item.Id, item.StartingBlendMode, redo);
            }
        }
        private IEnumerable<IHistory> GetNameHistory()
        {
            string redo = this.LayerButton.NameValue;

            foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
            {
                if (item.StartingName == redo) continue;

                yield return new PropertyHistory<string>(HistoryType.Name, item.Id, item.StartingName, redo);
            }
        }

    }
}