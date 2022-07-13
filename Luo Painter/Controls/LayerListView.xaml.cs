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

    public sealed partial class LayerListView : Spliter
    {
        //@Delegate
        public event RoutedEventHandler Add { remove => this.AddButton.Click -= value; add => this.AddButton.Click += value; }
        public event RoutedEventHandler Remove { remove => this.RemoveButton.Click -= value; add => this.RemoveButton.Click += value; }
        public event RoutedEventHandler Opening { remove => this.Button.Click -= value; add => this.Button.Click += value; }
        public event EventHandler<ILayer> SelectedItemChanged;
        public event EventHandler<ILayer> VisualClick { remove => this.VisualCommand.Click -= value; add => this.VisualCommand.Click += value; }
        public event DragItemsStartingEventHandler DragItemsStarting { remove => this.ListView.DragItemsStarting -= value; add => this.ListView.DragItemsStarting += value; }
        public event TypedEventHandler<ListViewBase, DragItemsCompletedEventArgs> DragItemsCompleted { remove => this.ListView.DragItemsCompleted -= value; add => this.ListView.DragItemsCompleted += value; }

        //@Converter
        private Visibility DoubleToVisibilityConverter(double value) => value == 0 ? Visibility.Collapsed : Visibility.Visible;
        private double ListViewWidthConverter(double value) => this.ListViewWidthConverter(value, 1);
        private double ListViewWidthConverter(double value, int min) => System.Math.Clamp((int)(value / 70 + 0.5), min, 6) * 70;
        private double ThumbeTopConverter(double value) => value / 2 - 60;
        private double ThumbeBottomConverter(double value) => value / 2 + 60;
        private Visibility SelectedVisibilityConverter(double value) => value > 70 + 70 ? Visibility.Visible : Visibility.Collapsed;

        long SelectedItemToken;

        public FrameworkElement PlacementTarget => this.Grid;
        public int SelectedIndex { get => this.ListView.SelectedIndex; set => this.ListView.SelectedIndex = value; }
        public object SelectedItem { get => this.ListView.SelectedItem; set => this.ListView.SelectedItem = value; }
        public IList<object> SelectedItems => this.ListView.SelectedItems;
        public object ItemsSource { set => this.ListView.ItemsSource = value; }
        public ImageSource MarqueeSource { set => this.Image.Source = value; }

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
            base.Unloaded += (s, e) =>
            {
                // Unregister Listener
                this.ListView.UnregisterPropertyChangedCallback(Selector.SelectedItemProperty, this.SelectedItemToken);
            };
            base.Loaded += (s, e) =>
            {
                // Register Listener
                this.SelectedItemToken = this.ListView.RegisterPropertyChangedCallback(Selector.SelectedItemProperty, (sender, prop) =>
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
                this.RemoveButton.IsEnabled = true;
                this.SelectedItemChanged?.Invoke(this, value);//Delegate
            }
            else
            {
                this.RemoveButton.IsEnabled = false;
                this.SelectedItemChanged?.Invoke(this, null);//Delegate
            }
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}