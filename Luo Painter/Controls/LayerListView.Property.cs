using Luo_Painter.Brushes;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerListView : XamlListView
    {

        string NameValue => this.NameTextBox.Text;
        float OpacityValue => (float)(this.OpacitySlider.Value / 100);
        BlendEffectMode? BlendModeValue
        {
            get
            {
                int index = this.BlendModeComboBox.SelectedIndex;
                if (index is 0) return null;
                else return this.BlendCollection[index];
            }
        }

        bool IsPropertyEnabled;

        int OpacityCount = 0;
        int BlendModeCount = 0;
        int NameCount = 0;

        private void ConstructPropertys()
        {
            this.Flyout.Closed += (s, e) =>
            {
                switch (this.OpacityCount)
                {
                    case 0:
                        break;
                    case 1:
                        if (base.SelectedItem is ILayer layer)
                        {
                            float redo = this.OpacityValue;

                            // History
                            this.History(this, new PropertyHistory<float>(HistoryType.Opacity, layer.Id, layer.StartingOpacity, redo));
                        }
                        break;
                    default:
                        // History
                        this.History(this, new CompositeHistory(this.GetOpacityHistory().ToArray()));
                        break;
                }

                switch (this.BlendModeCount)
                {
                    case 0:
                        break;
                    case 1:
                        if (base.SelectedItem is ILayer layer)
                        {
                            BlendEffectMode? redo = this.BlendModeValue;

                            // History
                            this.History(this, new PropertyHistory<BlendEffectMode?>(HistoryType.BlendMode, layer.Id, layer.StartingBlendMode, redo));
                        }
                        break;
                    default:
                        // History
                        this.History(this, new CompositeHistory(this.GetBlendModeHistory().ToArray()));
                        break;
                }

                switch (this.NameCount)
                {
                    case 0:
                        break;
                    case 1:
                        if (base.SelectedItem is ILayer layer)
                        {
                            string redo = this.NameValue;

                            // History
                            this.History(this, new PropertyHistory<string>(HistoryType.Name, layer.Id, layer.StartingName, redo));
                        }
                        break;
                    default:
                        // History
                        this.History(this, new CompositeHistory(this.GetNameHistory().ToArray()));
                        break;
                }
            };

            this.Flyout.Opened += (s, e) =>
            {
                this.IsPropertyEnabled = false;
                if (base.SelectedItem is ILayer layer)
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

                float redo = (float)(e.NewValue / 100);

                if (this.OpacityCount is 0)
                {
                    foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                    {
                        this.OpacityCount++;
                        item.CacheOpacity();
                        item.Opacity = redo;
                    }
                }
                else
                {
                    foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                    {
                        item.Opacity = redo;
                    }
                }

                this.Invalidate(s, e); // Invalidate
            };
            this.BlendModeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                BlendEffectMode? redo = this.BlendModeValue;

                if (this.BlendModeCount is 0)
                {
                    foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                    {
                        this.BlendModeCount++;
                        item.CacheBlendMode();
                        item.BlendMode = redo;
                    }
                }
                else
                {
                    foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                    {
                        item.BlendMode = redo;
                    }
                }

                this.Invalidate(s, e); // Invalidate
            };
            this.NameTextBox.TextChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                string redo = this.NameValue;

                if (this.NameCount is 0)
                {
                    foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                    {
                        this.NameCount++;
                        item.CacheName();
                        item.Name = redo;
                    }
                }
                else
                {
                    foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                    {
                        item.Name = redo;
                    }
                }
            };
        }

        private IEnumerable<IHistory> GetOpacityHistory()
        {
            float redo = this.OpacityValue;

            foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
            {
                if (item.StartingOpacity == redo) continue;

                yield return new PropertyHistory<float>(HistoryType.Opacity, item.Id, item.StartingOpacity, redo);
            }
        }
        private IEnumerable<IHistory> GetBlendModeHistory()
        {
            BlendEffectMode? redo = this.BlendModeValue;

            foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
            {
                if (item.StartingBlendMode == redo) continue;

                yield return new PropertyHistory<BlendEffectMode?>(HistoryType.BlendMode, item.Id, item.StartingBlendMode, redo);
            }
        }
        private IEnumerable<IHistory> GetNameHistory()
        {
            string redo = this.NameValue;

            foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
            {
                if (item.StartingName == redo) continue;

                yield return new PropertyHistory<string>(HistoryType.Name, item.Id, item.StartingName, redo);
            }
        }

    }
}