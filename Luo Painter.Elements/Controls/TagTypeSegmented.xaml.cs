using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Segmented of Tag Type
    /// </summary>
    public sealed class TagTypeSegmented : MenuFlyoutSeparator
    {

        //@Delegate
        public event EventHandler<int> TypeChanged;

        StackPanel StackPanel;
        readonly Button[] Controls = new Button[7];

        /// <summary> Gets or sets the tag type. </summary>
        public int Type
        {
            get => this.type;
            set
            {
                this.type = value;
                foreach (Button item in this.Controls)
                {
                    if (item is null) continue;
                    item.IsEnabled = item.TabIndex != this.type;
                }
            }
        }
        private int type;

        //@Construct
        /// <summary>
        /// Initializes a TagTypeControl. 
        /// </summary>
        public TagTypeSegmented()
        {
            this.DefaultStyleKey = typeof(TagTypeSegmented);
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                foreach (Button item in this.Controls)
                {
                    if (item is null) continue;
                    item.Width = e.NewSize.Width / 7;
                }
            };
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.StackPanel is null is false)
            {
                foreach (Button item in this.StackPanel.Children)
                {
                    if (item == null) continue;
                    item.Click -= this.Click;
                    item.Width = double.NaN;
                    item.IsEnabled = true;
                }
            }
            this.StackPanel = base.GetTemplateChild(nameof(StackPanel)) as StackPanel;
            if (this.StackPanel is null is false)
            {
                foreach (Button item in this.StackPanel.Children)
                {
                    if (item == null) continue;
                    item.Click += this.Click;
                    item.Width = base.Width / 7;
                    item.IsEnabled = this.type != item.TabIndex;
                }
            }
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button item)
            {
                this.Type = item.TabIndex;
                this.TypeChanged?.Invoke(this, this.Type); // Delegate
            }
        }
    }
}