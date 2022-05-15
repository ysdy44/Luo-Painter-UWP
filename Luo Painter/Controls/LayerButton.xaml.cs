using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas.Effects;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerButton : StackPanel
    {
        //@Delegate
        public event TypedEventHandler<string, string> NameHistory;
        public event TypedEventHandler<float, float> OpacityHistory;
        public event TypedEventHandler<BlendEffectMode?, BlendEffectMode?> BlendModeHistory;

        public ThumbSlider OpacitySlider => this.OpacitySliderCore;

        string StartingName;
        BlendEffectMode? StartingBlendMode;

        //@Construct
        public LayerButton()
        {
            this.InitializeComponent();
            this.LayerTextBox.GotFocus += (s, e) =>
            {
                this.StartingName = this.LayerTextBox.Text;
            };
            this.LayerTextBox.LostFocus += (s, e) =>
            {
                string name = this.LayerTextBox.Text;
                if (this.StartingName == name) return;
                this.NameHistory?.Invoke(this.StartingName, name); // Delegate
            };

            this.OpacitySlider.ValueChangedUnfocused += (s, e) =>
            {
                float undo = (float)(e.OldValue / 100);
                float redo = (float)(e.NewValue / 100);

                this.OpacityHistory?.Invoke(undo, redo); // Delegate
            };

            this.ListViewListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is int item)
                {
                    float undo = (float)(this.OpacitySlider.Value / 100);
                    float redo = (float)item / 100;

                    this.OpacityHistory?.Invoke(undo, redo); // Delegate

                    this.OpacitySlider.Value = item;
                }
            };

            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item2)
                {
                    BlendEffectMode? blendMode = item2.IsNone() ? (BlendEffectMode?)null : item2;
                    if (this.StartingBlendMode == blendMode) return;
                    this.BlendModeHistory?.Invoke(this.StartingBlendMode, blendMode); // Delegate

                    this.StartingBlendMode = blendMode;
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void SetSelectedItem(ILayer layer)
        {
            if (layer is null)
            {
                this.LayerImageButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.LayerImage.Source = null;
                this.LayerTextBox.Text = string.Empty;
                this.OpacitySliderCore.Value = 100;
                this.OpacitySliderCore.IsEnabled = false;
                this.BlendModeListView.IsEnabled = false;
                this.BlendModeListView.SelectedIndex = -1;
            }
            else
            {
                this.StartingBlendMode = layer.BlendMode;

                this.LayerImage.Source = layer.Thumbnail;
                this.LayerImageButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.LayerTextBox.Text = layer.Name ?? string.Empty;
                this.OpacitySliderCore.IsEnabled = true;
                this.OpacitySliderCore.Value = layer.Opacity * 100;
                this.BlendModeListView.IsEnabled = true;
                this.BlendModeListView.SelectedIndex = layer.BlendMode.HasValue ? this.BlendCollection.IndexOf(layer.BlendMode.Value) : 0;
            }
        }

    }
}