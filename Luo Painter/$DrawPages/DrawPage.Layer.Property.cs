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
            this.PropertyFlyout.Closed += (s, e) =>
            {
                switch (this.OpacityCount)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.LayerListView.SelectedItem is ILayer layer)
                        {
                            float redo = (float)(this.OpacitySlider.Value / 100);

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
                            int index = this.BlendModeComboBox.SelectedIndex;
                            BlendEffectMode? redo = (index is 0) ? default : this.BlendCollection[index];

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
                            string redo = (string)this.NameTextBox.Text;

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

            this.PropertyFlyout.Opened += (s, e) =>
            {
                this.IsPropertyEnabled = false;
                if (this.LayerListView.SelectedItem is ILayer layer)
                {
                    this.NameTextBox.IsEnabled = true;
                    this.NameTextBox.Text = layer.Name ?? string.Empty;
                    this.OpacitySlider.IsEnabled = true;
                    this.OpacitySlider.Value = layer.Opacity * 100;
                    this.BlendModeComboBox.IsEnabled = true;
                    this.BlendModeComboBox.SelectedIndex = layer.BlendMode.HasValue ? this.BlendCollection.IndexOf(layer.BlendMode.Value) : 0;
                    this.TagTypeSegmented.IsEnabled = true;
                }
                else
                {
                    this.NameTextBox.IsEnabled = false;
                    this.NameTextBox.Text = string.Empty;
                    this.OpacitySlider.Value = 100;
                    this.OpacitySlider.IsEnabled = false;
                    this.BlendModeComboBox.IsEnabled = false;
                    this.BlendModeComboBox.SelectedIndex = -1;
                    this.TagTypeSegmented.IsEnabled = false;
                }
                this.IsPropertyEnabled = true;


                this.OpacityCount = 0;

                this.BlendModeCount = 0;

                this.NameCount = 0;
            };
        }

        private void ConstructProperty()
        {
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                float opacity = (float)(e.NewValue / 100);

                if (this.OpacityCount is 0)
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        this.OpacityCount++;
                        item.CacheOpacity();
                        item.Opacity = opacity;
                    }
                }
                else
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        item.Opacity = opacity;
                    }
                }

                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.BlendModeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                int index = this.BlendModeComboBox.SelectedIndex;
                BlendEffectMode? mode = (index is 0) ? default : this.BlendCollection[index];

                if (this.BlendModeCount is 0)
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        this.BlendModeCount++;
                        item.CacheBlendMode();
                        item.BlendMode = mode;
                    }
                }
                else
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        item.BlendMode = mode;
                    }
                }

                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.NameTextBox.SelectionChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                string name = this.NameTextBox.Text;

                if (this.NameCount is 0)
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        this.NameCount++;
                        item.CacheName();
                        item.Name = name;
                    }
                }
                else
                {
                    foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
                    {
                        item.Name = name;
                    }
                }
            };
        }

        private IEnumerable<IHistory> GetOpacityHistory()
        {
            float redo = (float)(this.OpacitySlider.Value / 100);

            foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
            {
                if (item.StartingOpacity == redo) continue;

                yield return new PropertyHistory<float>(HistoryType.Opacity, item.Id, item.StartingOpacity, redo);
            }
        }
        private IEnumerable<IHistory> GetBlendModeHistory()
        {
            int index = this.BlendModeComboBox.SelectedIndex;
            BlendEffectMode? redo = (index is 0) ? default : this.BlendCollection[index];

            foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
            {
                if (item.StartingBlendMode == redo) continue;

                yield return new PropertyHistory<BlendEffectMode?>(HistoryType.BlendMode, item.Id, item.StartingBlendMode, redo);
            }
        }
        private IEnumerable<IHistory> GetNameHistory()
        {
            string redo = this.NameTextBox.Text;

            foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
            {
                if (item.StartingName == redo) continue;

                yield return new PropertyHistory<string>(HistoryType.Name, item.Id, item.StartingName, redo);
            }
        }

    }
}