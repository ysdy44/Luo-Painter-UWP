using Luo_Painter.Layers;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerButton : Grid
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick { remove => this.Command.Click -= value; add => this.Command.Click += value; }

        public event EventHandler<object> Closed { remove => this.Flyout.Closed -= value; add => this.Flyout.Closed += value; }
        public event EventHandler<object> Opened { remove => this.Flyout.Opened -= value; add => this.Flyout.Opened += value; }

        public event RangeBaseValueChangedEventHandler OpacityChanged { remove => this.OpacitySlider.ValueChanged -= value; add => this.OpacitySlider.ValueChanged += value; }
        public event SelectionChangedEventHandler BlendModeChanged { remove => this.BlendModeComboBox.SelectionChanged -= value; add => this.BlendModeComboBox.SelectionChanged += value; }
        public event TextChangedEventHandler NameChanged { remove => this.NameTextBox.TextChanged -= value; add => this.NameTextBox.TextChanged += value; }

        //@Content
        public bool PasteLayerIsEnabled { get => this.PasteLayerItem.IsEnabled; set => this.PasteLayerItem.IsEnabled = value; }

        public string NameValue => this.NameTextBox.Text;
        public float OpacityValue => (float)(this.OpacitySlider.Value / 100);
        public BlendEffectMode? BlendModeValue
        {
            get
            {
                int index = this.BlendModeComboBox.SelectedIndex;
                if (index is 0) return null;
                else return this.BlendCollection[index];
            }
        }

        //@Construct
        public LayerButton()
        {
            this.InitializeComponent();
        }

        public void Construct(ILayer layer)
        {
            this.NameTextBox.IsEnabled = true;
            this.NameTextBox.Text = layer.Name ?? string.Empty;
            this.OpacitySlider.IsEnabled = true;
            this.OpacitySlider.Value = layer.Opacity * 100;
            this.BlendModeComboBox.IsEnabled = true;
            this.BlendModeComboBox.SelectedIndex = layer.BlendMode.HasValue ? this.BlendCollection.IndexOf(layer.BlendMode.Value) : 0;
            this.TagTypeSegmented.IsEnabled = true;
        }

        public void Clear()
        {
            this.NameTextBox.IsEnabled = false;
            this.NameTextBox.Text = string.Empty;
            this.OpacitySlider.Value = 100;
            this.OpacitySlider.IsEnabled = false;
            this.BlendModeComboBox.IsEnabled = false;
            this.BlendModeComboBox.SelectedIndex = -1;
            this.TagTypeSegmented.IsEnabled = false;
        }

    }
}