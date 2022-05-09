﻿using Luo_Painter.Edits;
using Luo_Painter.Elements;
using Luo_Painter.Tools;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal sealed class ToolIcon : TIcon<ToolType>
    {
        public ToolIcon()
        {
            base.Loaded += (s, e) =>
            {
                ListViewItem parent = SelectedButtonPresenter.FindAncestor(this);
                if (parent is null) return;
                ToolTipService.SetToolTip(parent, new ToolTip
                {
                    Content = this.Type,
                    Style = App.Current.Resources["AppToolTipStyle"] as Style
                });
            };
        }
        protected override void OnTypeChanged(ToolType value)
        {
            base.Content = value;
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }

    internal sealed class ToolGroupingList : GroupingList<ToolGrouping, ToolGroupType, ToolType> { }
    internal sealed class ToolGrouping : Grouping<ToolGroupType, ToolType> { }

    public sealed partial class ToolListView : Canvas
    {
        //@Delegate
        public event TypedEventHandler<ToolGroupType, ToolType> ItemClick;

        //@Converter
        private Visibility DoubleToVisibilityConverter(double value) => value == 0 ? Visibility.Collapsed : Visibility.Visible;
        private double ReverseDoubleConverter(double value) => -value;
        private double ListViewLeftConverter(double value) => value - System.Math.Clamp((int)(value / 70 + 0.5), 1, 6) * 70;
        private double ListViewWidthConverter(double value) => this.ListViewWidthConverter(value, 1);
        private double ListViewWidthConverter(double value, int min) => System.Math.Clamp((int)(value / 70 + 0.5), min, 6) * 70;
        private double ThumbeTopConverter(double value) => value / 2 - 60;

        double StartingX;

        #region DependencyProperty


        /// <summary> Gets or set the state for <see cref="ToolListView"/>. </summary>
        public bool IsShow
        {
            get => (bool)base.GetValue(IsShowProperty);
            set => base.SetValue(IsShowProperty, value);
        }
        /// <summary> Identifies the <see cref = "ToolListView.IsShow" /> dependency property. </summary>
        public static readonly DependencyProperty IsShowProperty = DependencyProperty.Register(nameof(IsShow), typeof(bool), typeof(ToolListView), new PropertyMetadata(true, (sender, e) =>
        {
            ToolListView control = (ToolListView)sender;

            if (e.NewValue is bool value)
            {
                if (value)
                    control.ShowStoryboard.Begin();
                else
                    control.HideStoryboard.Begin();
            }
        }));


        #endregion

        //@Construct
        public ToolListView()
        {
            this.InitializeComponent();
            this.Thumb.DragStarted += (s, e) => this.StartingX = base.Width;
            this.Thumb.DragDelta += (s, e) => base.Width = System.Math.Clamp(this.StartingX += e.HorizontalChange, 0, 6 * 70);
            this.Thumb.DragCompleted += (s, e) => base.Width = this.ListViewWidthConverter(this.StartingX, 0);
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;
                if (e.NewSize.Height == e.PreviousSize.Height) return;

                this.ListView.Height = e.NewSize.Height;
            };
            this.ListView.ItemClick += (s, e) =>
            {
                if (this.ItemClick is null) return;

                if (e.ClickedItem is ToolType item)
                {
                    this.ItemClick(this.ToolCollection[item], item); // Delegate
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void Construct(ToolType type)
        {
            this.ListView.SelectedIndex = this.ToolCollection.IndexOf(type);

            this.ItemClick?.Invoke(this.ToolCollection[type], type); // Delegate
        }

    }
}