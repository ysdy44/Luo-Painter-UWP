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

    public sealed partial class LayerListView : Canvas
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }
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

        double StartingX;

        long SelectedItemToken;

        #region DependencyProperty


        /// <summary> Gets or set the state for <see cref="LayerListView"/>. </summary>
        public bool IsShow
        {
            get => (bool)base.GetValue(IsShowProperty);
            set => base.SetValue(IsShowProperty, value);
        }
        /// <summary> Identifies the <see cref = "LayerListView.IsShow" /> dependency property. </summary>
        public static readonly DependencyProperty IsShowProperty = DependencyProperty.Register(nameof(IsShow), typeof(bool), typeof(LayerListView), new PropertyMetadata(true, (sender, e) =>
        {
            LayerListView control = (LayerListView)sender;

            if (e.NewValue is bool value)
            {
                if (value)
                    control.ShowStoryboard.Begin();
                else
                    control.HideStoryboard.Begin();
            }
        }));

        /// <summary> Gets or set the state for <see cref="LayerListView"/>. </summary>
        public bool IsSelected
        {
            get => (bool)base.GetValue(IsSelectedProperty);
            set => base.SetValue(IsSelectedProperty, value);
        }
        /// <summary> Identifies the <see cref = "LayerListView.IsSelected" /> dependency property. </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(LayerListView), new PropertyMetadata(false));


        #endregion

        public IList<object> SelectedItems => this.ListView.SelectedItems;
        public object SelectedItem => this.ListView.SelectedItem;
        public int SelectedIndex
        {
            get => this.ListView.SelectedIndex;
            set => this.ListView.SelectedIndex = value;
        }
        public object ItemsSource { set => this.ListView.ItemsSource = value; }
        public ImageSource MarqueeSource { set => this.Image.Source = value; }

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
            this.Thumb.DragStarted += (s, e) => this.StartingX = base.Width;
            this.Thumb.DragDelta += (s, e) => base.Width = System.Math.Clamp(this.StartingX -= e.HorizontalChange, 0, 6 * 70);
            this.Thumb.DragCompleted += (s, e) => base.Width = this.ListViewWidthConverter(this.StartingX, 0);
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;
                if (e.NewSize.Height == e.PreviousSize.Height) return;

                this.ListView.Height = e.NewSize.Height;
            };
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
            this.CommandBar.Loaded += (s, e) =>
            {
                if (s is DependencyObject reference)
                {
                    DependencyObject layoutRoot = VisualTreeHelper.GetChild(reference, 0); // Grid LayoutRoot
                    DependencyObject contentRoot = VisualTreeHelper.GetChild(layoutRoot, 0); // Grid ContentRoot

                    DependencyObject moreButton = VisualTreeHelper.GetChild(contentRoot, 1); // Button MoreButton Width Auto
                    if (moreButton is Button moreButton1)
                    {
                        moreButton1.Width = 70;
                        moreButton1.HorizontalContentAlignment = HorizontalAlignment.Center;
                    }

                    DependencyObject grid = VisualTreeHelper.GetChild(contentRoot, 0); // Grid
                    DependencyObject primaryItemsControl = VisualTreeHelper.GetChild(grid, 1); // ItemsControl PrimaryItemsControl HorizontalAlignment Right Grid.Column 1
                    if (primaryItemsControl is ItemsControl primaryItemsControl1)
                    {
                        primaryItemsControl1.HorizontalAlignment = HorizontalAlignment.Left;
                        Grid.SetColumn(primaryItemsControl1, 0);
                    }
                }
            };
        }

        public void OnSelectedItemChanged() => this.OnSelectedItemChanged(this.SelectedItem);
        private void OnSelectedItemChanged(object obj)
        {
            if (obj is ILayer value)
            {
                this.IsSelected = true;
                this.SelectedItemChanged?.Invoke(this, value);//Delegate
            }
            else
            {
                this.IsSelected = false;
                this.SelectedItemChanged?.Invoke(this, null);//Delegate
            }
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}