﻿using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

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
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            float redo = (float)this.OpacitySlider.Value;

                            // History
                            IHistory history = new PropertyHistory(HistoryPropertyMode.Opacity, layer.Id, layer.StartingOpacity, redo);
                            history.Title = UIType.Layer_Opacity.GetString();
                            int removes = this.History.Push(history);
                        }
                        break;
                    default:
                        {
                            // History
                            IHistory history = new CompositeHistory(this.GetOpacityHistory().ToArray());
                            history.Title = UIType.Layer_Opacity.GetString();
                            int removes = this.History.Push(history);
                        }
                        break;
                }

                switch (this.BlendModeCount)
                {
                    case 0:
                        break;
                    case 1:
                        if (this.LayerSelectedItem is ILayer layer)
                        {
                            BlendEffectMode redo = this.BlendModes[this.BlendModeComboBox.SelectedIndex];

                            // History
                            IHistory history = new PropertyHistory(HistoryPropertyMode.BlendMode, layer.Id, layer.StartingBlendMode, redo);
                            history.Title = UIType.Layer_BlendMode.GetString();
                            int removes = this.History.Push(history);
                        }
                        break;
                    default:
                        {
                            // History
                            IHistory history = new CompositeHistory(this.GetBlendModeHistory().ToArray());
                            history.Title = UIType.Layer_BlendMode.GetString();
                            int removes = this.History.Push(history);
                        }
                        break;
                }
            };

            this.Flyout.Opened += (s, e) =>
            {
                this.IsPropertyEnabled = false;
                if (this.LayerSelectedItem is ILayer layer)
                {
                    this.RenameButton.IsEnabled = true;
                    this.RemoveButton.IsEnabled = true;
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
                }
                else
                {
                    this.RenameButton.IsEnabled = false;
                    this.RemoveButton.IsEnabled = false;
                    this.OpacitySlider.Value = 100;
                    this.OpacitySlider.IsEnabled = false;
                    this.BlendModeComboBox.IsEnabled = false;
                    this.BlendModeComboBox.SelectedIndex = -1;
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
            this.BlendModeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.IsPropertyEnabled is false) return;

                BlendEffectMode redo = this.BlendModes[this.BlendModeComboBox.SelectedIndex];

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
        }

        private IEnumerable<IHistory> GetOpacityHistory()
        {
            float redo = (float)this.OpacitySlider.Value;

            foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
            {
                if (item.StartingOpacity == redo) continue;

                yield return new PropertyHistory(HistoryPropertyMode.Opacity, item.Id, item.StartingOpacity, redo);
            }
        }
        private IEnumerable<IHistory> GetBlendModeHistory()
        {
            BlendEffectMode redo = this.BlendModes[this.BlendModeComboBox.SelectedIndex];

            foreach (ILayer item in this.LayerSelectedItems.Cast<ILayer>())
            {
                if (item.StartingBlendMode == redo) continue;

                yield return new PropertyHistory(HistoryPropertyMode.BlendMode, item.Id, item.StartingBlendMode, redo);
            }
        }

    }
}