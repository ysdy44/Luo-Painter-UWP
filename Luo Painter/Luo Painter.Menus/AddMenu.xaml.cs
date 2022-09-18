using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Luo_Painter.Menus
{
    public sealed partial class AddMenu : Expander
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }
        public event TypedEventHandler<string, string> NameHistory;
        public event TypedEventHandler<float, float> OpacityHistory;
        public event TypedEventHandler<BlendEffectMode?, BlendEffectMode?> BlendModeHistory;

        //@Converter
        private string RoundConverter(double value) => $"{value:0}";

        public ThumbSlider OpacitySlider => this.OpacitySliderCore;

        string StartingName;
        BlendEffectMode? StartingBlendMode;

        bool IsEnable;

        //@Construct
        public AddMenu()
        {
            this.InitializeComponent();
            this.NameTextBox.GotFocus += (s, e) =>
            {
                this.StartingName = this.NameTextBox.Text;
            };
            this.NameTextBox.LostFocus += (s, e) =>
            {
                string name = this.NameTextBox.Text;
                if (this.StartingName == name) return;
                this.NameHistory?.Invoke(this.StartingName, name); // Delegate
            };

            this.OpacitySlider.ValueChangedUnfocused += (s, e) =>
            {
                float undo = (float)(e.OldValue / 100);
                float redo = (float)(e.NewValue / 100);

                this.OpacityHistory?.Invoke(undo, redo); // Delegate
            };


            this.BlendModeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;

                if (this.BlendModeComboBox.SelectedItem is BlendEffectMode item)
                {
                    if (item.IsDefined())
                    {
                        if (this.StartingBlendMode == item) return;
                        this.BlendModeHistory?.Invoke(this.StartingBlendMode, item); // Delegate

                        this.StartingBlendMode = item;
                    }
                    else
                    {
                        if (this.StartingBlendMode is null) return;
                        this.BlendModeHistory?.Invoke(this.StartingBlendMode, null); // Delegate

                        this.StartingBlendMode = null;
                    }
                }
            };
        }

        public void SetSelectedItem(ILayer layer)
        {
            this.IsEnable = false;

            if (layer is null)
            {
                this.NameTextBox.IsEnabled = false;
                this.NameTextBox.Text = string.Empty;
                this.OpacitySliderCore.Value = 100;
                this.OpacitySliderCore.IsEnabled = false;
                this.BlendModeComboBox.IsEnabled = false;
                this.BlendModeComboBox.SelectedIndex = -1;
                this.TagTypeSegmented.IsEnabled = false;
            }
            else
            {
                this.StartingBlendMode = layer.BlendMode;

                this.NameTextBox.IsEnabled = true;
                this.NameTextBox.Text = layer.Name ?? string.Empty;
                this.OpacitySliderCore.IsEnabled = true;
                this.OpacitySliderCore.Value = layer.Opacity * 100;
                this.BlendModeComboBox.IsEnabled = true;
                this.BlendModeComboBox.SelectedIndex = layer.BlendMode.HasValue ? this.Collection.IndexOf(layer.BlendMode.Value) : 0;
                this.TagTypeSegmented.IsEnabled = true;
            }

            this.IsEnable = true;
        }

        public void Rename()
        {
            this.NameTextBox.Focus(FocusState.Keyboard);
            this.NameTextBox.SelectAll();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}