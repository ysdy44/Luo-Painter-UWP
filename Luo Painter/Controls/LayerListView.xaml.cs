﻿using Luo_Painter.Elements;
using Luo_Painter.HSVColorPickers;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class LayerCommand : RelayCommand<ILayer> { }

    public sealed partial class LayerListView : XamlListView
    {
        //@Delegate
        public event EventHandler<ILayer> VisualClick { remove => this.VisualCommand.Click -= value; add => this.VisualCommand.Click += value; }
        public event EventHandler<IHistory> History;
        public event EventHandler<object> Invalidate;
        public event RoutedEventHandler OpacitySliderClick { remove => this.OpacitySlider.Click -= value; add => this.OpacitySlider.Click += value; }

        //@Command
        public ICommand Command { get; set; }

        //@Content
        public bool PasteLayerIsEnabled { get; set; }
        public bool IsOpen => this.RenamePopup.IsOpen;
        public ImageSource Source { get; set; }

        readonly Popup RenamePopup = new Popup();
        readonly TextBox RenameTextBox = new TextBox();
        
        bool IsPropertyEnabled;

        int OpacityCount = 0;
        int BlendModeCount = 0;

        public INumberBase OpacityNumber => this.OpacitySlider;
        public double OpacitySliderValue { set => this.OpacitySlider.Value = value; }

        //string NameValue => this.NameTextBox.Text;
        float OpacityValue => (float)(this.OpacitySlider.Value / 100);
        BlendEffectMode BlendModeValue => this.BlendModes[this.BlendModeComboBox.SelectedIndex];

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

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
            this.RenamePopup.Child = this.RenameTextBox;

            this.ConstructPropertys();
            this.ConstructProperty();

            this.ConstructRenames();
            this.ConstructRename();

            base.RightTapped += (s, e) =>
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.DataContext is ILayer item)
                    {
                        if (base.SelectedItem is null)
                            base.SelectedItem = item;
                        else if (base.SelectedItems.All(c => c != item))
                            base.SelectedItem = item;
                        this.MenuFlyout.ShowAt(this, e.GetPosition(this));
                    }
                }
            };

            this.MenuFlyout.Opened += (s, e) =>
            {
                bool isEnabled = base.SelectedItem is null is false;

                this.CutLayerItem.GoToState(isEnabled);
                this.CopyLayerItem.GoToState(isEnabled);
                this.PasteLayerItem.GoToState(this.PasteLayerIsEnabled);
                this.RemoveItem.GoToState(isEnabled);
                this.DuplicateItem.GoToState(isEnabled);
                this.ExtractItem.GoToState(isEnabled);
                this.MergeItem.GoToState(isEnabled);
                this.FlattenItem.GoToState(isEnabled);
                this.GroupItem.GoToState(isEnabled);
                this.UngroupItem.GoToState(isEnabled);
                this.ReleaseItem.GoToState(isEnabled);

                TagType type = isEnabled && base.SelectedItem is ILayer layer ? layer.TagType : TagType.None;
                this.TagTypeSegmented.Type = (int)type;
                this.TagTypeSegmented.IsEnabled = isEnabled;
            };
            this.TagTypeSegmented.TypeChanged += (s, e) =>
            {
                TagType type = (TagType)e;
                foreach (ILayer item in base.SelectedItems.Cast<ILayer>())
                {
                    item.TagType = type;
                }
            };
        }

    }
}