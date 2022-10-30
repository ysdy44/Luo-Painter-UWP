using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class LayerCommand : RelayCommand<ILayer> { }

    public sealed partial class LayerListView : XamlListView
    {
        //@Delegate
        public event EventHandler<ILayer> SelectedItemChanged;
        public event EventHandler<ILayer> VisualClick { remove => this.VisualCommand.Click -= value; add => this.VisualCommand.Click += value; }

        //@Converter
        private Visibility DoubleToVisibilityConverter(double value) => value == 0 ? Visibility.Collapsed : Visibility.Visible;
        private double ListViewWidthConverter(double value) => this.ListViewWidthConverter(value, 1);
        private double ListViewWidthConverter(double value, int min) => System.Math.Clamp((int)(value / 70 + 0.5), min, 6) * 70;
        private double ThumbeTopConverter(double value) => value / 2 - 60;
        private double ThumbeBottomConverter(double value) => value / 2 + 60;
        private Visibility SelectedVisibilityConverter(double value) => value > 70 + 70 ? Visibility.Visible : Visibility.Collapsed;

        long SelectedItemToken;

        public ImageSource MarqueeSource { get => this.Image.Source; set => this.Image.Source = value; }

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
            base.Unloaded += (s, e) =>
            {
                // Unregister Listener
                base.UnregisterPropertyChangedCallback(Selector.SelectedItemProperty, this.SelectedItemToken);
            };
            base.Loaded += (s, e) =>
            {
                // Register Listener
                this.SelectedItemToken = base.RegisterPropertyChangedCallback(Selector.SelectedItemProperty, (sender, prop) =>
                {
                    ListView control = (ListView)sender;
                    this.OnSelectedItemChanged(control.GetValue(prop));
                });
            };
        }

        public void OnSelectedItemChanged() => this.OnSelectedItemChanged(this.SelectedItem);
        private void OnSelectedItemChanged(object obj)
        {
            if (obj is ILayer value)
            {
                this.SelectedItemChanged?.Invoke(this, value);//Delegate
            }
            else
            {
                this.SelectedItemChanged?.Invoke(this, null);//Delegate
            }
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}