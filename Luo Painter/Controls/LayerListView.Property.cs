using Luo_Painter.Blends;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.Models.Historys;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerListView : XamlListView
    {

        //string NameValue => this.NameTextBox.Text;
        float OpacityValue => (float)(this.OpacitySlider.Value / 100);
        BlendEffectMode BlendModeValue => this.BlendModes[this.BlendModeComboBox.SelectedIndex];

        bool IsPropertyEnabled;

        int OpacityCount = 0;
        int BlendModeCount = 0;

        readonly BlendEffectMode[] BlendModes = new BlendEffectMode[]
        {
            BlendExtensions.None,
            BlendEffectMode.Dissolve,
            // Darken
            BlendEffectMode.Darken,
            BlendEffectMode.Multiply,
            BlendEffectMode.ColorBurn,
            BlendEffectMode.LinearBurn,
            BlendEffectMode.DarkerColor,
            // Lighten
            BlendEffectMode.Lighten,
            BlendEffectMode.Screen,
            BlendEffectMode.ColorDodge,
            BlendEffectMode.LinearDodge,
            BlendEffectMode.LighterColor,
            // Contrast
            BlendEffectMode.Overlay,
            BlendEffectMode.SoftLight,
            BlendEffectMode.HardLight,
            BlendEffectMode.VividLight,
            BlendEffectMode.LinearLight,
            BlendEffectMode.PinLight,
            BlendEffectMode.HardMix,
            // Difference
            BlendEffectMode.Difference,
            BlendEffectMode.Exclusion,
            BlendEffectMode.Subtract,
            BlendEffectMode.Division,
            // Color
            BlendEffectMode.Hue,
            BlendEffectMode.Saturation,
            BlendEffectMode.Color,
            BlendEffectMode.Luminosity,
         };

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
            };

            this.Flyout.Opened += (s, e) =>
            {
                this.IsPropertyEnabled = false;
                if (base.SelectedItem is ILayer layer)
                {
                    this.OpacitySlider.IsEnabled = true;
                    this.OpacitySlider.Value = layer.Opacity * 100;
                    this.BlendModeComboBox.IsEnabled = true;
                    for (int i = 0; i < this.BlendModes.Length; i++)
                    {
                        BlendEffectMode item = this.BlendModes[i];
                        if (layer.BlendMode == item)
                        {
                            this.BlendModeComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                    this.TagTypeSegmented.IsEnabled = true;
                    this.TagTypeSegmented.Type = layer.TagType;
                }
                else
                {
                    this.OpacitySlider.Value = 100;
                    this.OpacitySlider.IsEnabled = false;
                    this.BlendModeComboBox.IsEnabled = false;
                    this.BlendModeComboBox.SelectedIndex = -1;
                    this.TagTypeSegmented.IsEnabled = false;
                    this.TagTypeSegmented.Type = TagType.None;
                }
                this.IsPropertyEnabled = true;

                this.OpacityCount = 0;
                this.BlendModeCount = 0;
            };
        }

        private void ConstructProperty()
        {
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                float redo = (float)e.NewValue / 100;

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

                BlendEffectMode redo = this.BlendModeValue;

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
            this.TagTypeSegmented.TypeChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                {
                    item.TagType = e;
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

    }
}