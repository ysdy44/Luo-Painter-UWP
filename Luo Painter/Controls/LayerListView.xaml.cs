using Luo_Painter.Elements;
using Luo_Painter.Layers;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal class LayerCommand : RelayCommand<ILayer> { }

    public sealed partial class LayerListView : Grid
    {
        //@Delegate
        public event RoutedEventHandler AddClick
        {
            remove => this.AddButton.Click -= value;
            add => this.AddButton.Click += value;
        }
        public event EventHandler<ILayer> VisualClick
        {
            remove => this.VisualCommand.Click -= value;
            add => this.VisualCommand.Click += value;
        }
        public event DragItemsStartingEventHandler DragItemsStarting
        {
            remove => this.ListView.DragItemsStarting -= value;
            add => this.ListView.DragItemsStarting += value;
        }
        public event TypedEventHandler<ListViewBase, DragItemsCompletedEventArgs> DragItemsCompleted
        {
            remove => this.ListView.DragItemsCompleted -= value;
            add => this.ListView.DragItemsCompleted += value;
        }

        double StartingX;
        SplitViewPanePlacement Placement => (this.TranslateTransform.X > 70) ? SplitViewPanePlacement.Left : SplitViewPanePlacement.Right;
        private double GetX(double value) => Math.Max(-70 - 70, Math.Min(0, value));

        public bool isExpanded;
        public bool IsExpanded
        {
            get => this.isExpanded;
            set
            {
                this.isExpanded = value;

                if (this.IsShow)
                {
                    if (value)
                        this.ExpandStoryboard.Begin();
                    else
                        this.NoneStoryboard.Begin();
                }
            }
        }

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
                {
                    if (control.IsExpanded)
                        control.ExpandStoryboard.Begin();
                    else
                        control.NoneStoryboard.Begin();
                }
                else
                    control.HideStoryboard.Begin();
            }
        }));


        #endregion

        public FrameworkElement PlacementTarget => this.AddButton;
        public IList<object> SelectedItems => this.ListView.SelectedItems;
        public object SelectedItem => this.ListView.SelectedItem;
        public int SelectedIndex
        {
            get => this.ListView.SelectedIndex;
            set => this.ListView.SelectedIndex = value;
        }
        public object ItemsSource { set => this.ListView.ItemsSource = value; }

        //@Construct
        public LayerListView()
        {
            this.InitializeComponent();
            this.SplitButton.ManipulationStarted += (s, e) =>
            {
                this.StartingX = this.TranslateTransform.X;
                switch (this.Placement)
                {
                    case SplitViewPanePlacement.Left: this.SplitIcon.Symbol = Symbol.AlignLeft; break;
                    case SplitViewPanePlacement.Right: this.SplitIcon.Symbol = Symbol.AlignRight; break;
                }
                this.SplitButton.IsEnabled = false;
            };
            this.SplitButton.ManipulationDelta += (s, e) =>
            {
                this.TranslateTransform.X = this.GetX(this.StartingX + e.Cumulative.Translation.X);
                switch (this.Placement)
                {
                    case SplitViewPanePlacement.Left: this.SplitIcon.Symbol = Symbol.AlignLeft; break;
                    case SplitViewPanePlacement.Right: this.SplitIcon.Symbol = Symbol.AlignRight; break;
                }
            };
            this.SplitButton.ManipulationCompleted += (s, e) =>
            {
                this.IsExpanded = this.Placement is SplitViewPanePlacement.Left;
                this.SplitIcon.Symbol = Symbol.GlobalNavigationButton;
                this.SplitButton.IsEnabled = true;
            };

            this.SplitButton.Click += (s, e) => this.IsExpanded = this.Placement is SplitViewPanePlacement.Right;
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}