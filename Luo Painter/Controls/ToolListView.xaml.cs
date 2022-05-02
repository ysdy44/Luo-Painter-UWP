using Luo_Painter.Edits;
using Luo_Painter.Elements;
using Luo_Painter.Tools;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal sealed class ToolIcon : ContentControl
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="ToolIcon"/>. </summary>
        public ToolType Type
        {
            get => (ToolType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "ToolIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(ToolType), typeof(ToolIcon), new PropertyMetadata(ToolType.None, (sender, e) =>
        {
            ToolIcon control = (ToolIcon)sender;

            if (e.NewValue is ToolType value)
            {
                control.Content = value;
                control.Template = value.GetTemplate(out ResourceDictionary resource);
                control.Resources = resource;
            }
        }));


        #endregion

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
    }

    internal sealed class ToolGroupingList : GroupingList<ToolGrouping, ToolGroupType, ToolType> { }
    internal sealed class ToolGrouping : Grouping<ToolGroupType, ToolType> { }

    public sealed partial class ToolListView : Grid
    {
        //@Delegate
        public event TypedEventHandler<ToolGroupType, ToolType> ItemClick;

        double StartingX;
        SplitViewPanePlacement Placement => (this.TranslateTransform.X < -70) ? SplitViewPanePlacement.Left : SplitViewPanePlacement.Right;
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

        //@Construct
        public ToolListView()
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
                this.IsExpanded = this.Placement is SplitViewPanePlacement.Right;
                this.SplitIcon.Symbol = Symbol.GlobalNavigationButton;
                this.SplitButton.IsEnabled = true;
            };

            this.SplitButton.Click += (s, e) => this.IsExpanded = this.Placement is SplitViewPanePlacement.Left;

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