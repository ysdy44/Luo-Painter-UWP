using Luo_Painter.Blends;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.Tools
{
    public sealed partial class LayerTool : UserControl
    {
        //@Delegate
        public event TypedEventHandler<bool, string> NameChanged;
        public event TypedEventHandler<bool, float> OpacityChanged;
        public event TypedEventHandler<bool, BlendEffectMode?> BlendModeChanged;

        public event TypedEventHandler<string, string> NameHistory;
        public event TypedEventHandler<float, float> OpacityHistory;
        public event TypedEventHandler<BlendEffectMode?, BlendEffectMode?> BlendModeHistory;

        public readonly IList<ILayer> ChangedLayers = new List<ILayer>();
        bool HasChangedLayers;
        bool HasNameChanged;
        bool HasOpacityChanged;
        bool HasBlendModeChanged;

        //@Construct
        public LayerTool()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void SetLayer(ILayer layer)
        {
            if (layer is null)
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

        public void Closed()
        {
            this.LayerTextBox.TextChanged -= this.LayerTextBox_TextChanged;
            this.OpacitySlider.ValueChanged -= this.OpacitySlider_ValueChanged;
            this.BlendModeListView.ItemClick -= this.BlendModeListView_ItemClick;

            if (this.HasChangedLayers == false) return;
            this.HasChangedLayers = false;

            if (this.ChangedLayers.Count == 0) return;

            if (this.HasNameChanged)
            {
                this.HasNameChanged = false;
                if (this.ChangedLayers.Count == 0) return;

                string redo = this.LayerTextBox.Text;
                this.NameHistory?.Invoke(string.Empty, redo); // Delegate
            }

            if (this.HasOpacityChanged)
            {
                this.HasOpacityChanged = false;
                if (this.ChangedLayers.Count == 0) return;

                float redo = (float)(this.OpacitySlider.Value / 100);
                this.OpacityHistory?.Invoke(1, redo); // Delegate
            }

            if (this.HasBlendModeChanged)
            {
                this.HasBlendModeChanged = false;
                if (this.ChangedLayers.Count == 0) return;

                if (this.BlendModeListView.SelectedItem is BlendEffectMode item)
                {
                    if (item.IsNone()) this.BlendModeHistory?.Invoke(null, null); // Delegate
                    else this.BlendModeHistory?.Invoke(null, item); // Delegate
                }
                else this.BlendModeHistory?.Invoke(null, null); // Delegate
            }

            this.ChangedLayers.Clear();
        }

        public void Opened()
        {
            this.LayerTextBox.TextChanged -= this.LayerTextBox_TextChanged;
            this.OpacitySlider.ValueChanged -= this.OpacitySlider_ValueChanged;
            this.BlendModeListView.ItemClick -= this.BlendModeListView_ItemClick;

            this.LayerTextBox.TextChanged += this.LayerTextBox_TextChanged;
            this.OpacitySlider.ValueChanged += this.OpacitySlider_ValueChanged;
            this.BlendModeListView.ItemClick += this.BlendModeListView_ItemClick;
        }

        private void LayerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = this.LayerTextBox.Text;

            if (this.HasChangedLayers)
            {
                this.NameChanged?.Invoke(true, name); // Delegate
            }
            else
            {
                this.HasChangedLayers = true;
                this.HasNameChanged = true;

                this.NameChanged?.Invoke(false, name); // Delegate
            }
        }

        private void OpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            float opacity = (float)(e.NewValue / 100);

            if (this.HasChangedLayers)
            {
                this.OpacityChanged?.Invoke(true, opacity); // Delegate
            }
            else
            {
                this.HasChangedLayers = true;
                this.HasOpacityChanged = true;

                this.OpacityChanged?.Invoke(false, opacity); // Delegate
            }
        }

        private void BlendModeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is BlendEffectMode item2)
            {
                BlendEffectMode? blendMode = item2.IsNone() ? (BlendEffectMode?)null : item2;

                if (this.HasChangedLayers)
                {
                    this.BlendModeChanged?.Invoke(true, blendMode); // Delegate
                }
                else
                {
                    this.HasChangedLayers = true;
                    this.HasBlendModeChanged = true;

                    this.BlendModeChanged?.Invoke(false, blendMode); // Delegate
                }
            }
        }

    }
}