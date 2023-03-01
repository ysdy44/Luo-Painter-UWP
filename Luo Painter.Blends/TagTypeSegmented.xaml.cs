using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Blends
{
    /// <summary>
    /// Segmented of <see cref="TagType"/>
    /// </summary>
    public sealed class TagTypeSegmented : MenuFlyoutSeparator
    {

        //@Delegate
        public event EventHandler<TagType> TypeChanged;

        readonly Button[] Controls = new Button[7];
        int Index;

        /// <summary> Gets or sets the tag type. </summary>
        public TagType Type
        {
            get => (TagType)this.Index;
            set
            {
                this.Index = (int)value;
                foreach (Button item in this.Controls)
                {
                    if (item is null) continue;
                    item.IsEnabled = item.TabIndex != this.Index;
                }
            }
        }

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

            foreach (TagType item in System.Enum.GetValues(typeof(TagType)))
            {
                int i = (int)item;

                if (this.Controls[i] != null)
                {
                    this.Controls[i].TabIndex = 0;
                    this.Controls[i].Click -= this.Click;
                    this.Controls[i].Width = double.NaN;
                    this.Controls[i].IsEnabled = true;
                }
                this.Controls[i] = base.GetTemplateChild(item.ToString()) as Button;
                if (this.Controls[i] != null)
                {
                    this.Controls[i].TabIndex = i;
                    this.Controls[i].Click += this.Click;
                    this.Controls[i].Width = base.Width / 7;
                    this.Controls[i].IsEnabled = this.Index != i;
                }
            }
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button item)
            {
                this.Type = (TagType)item.TabIndex;
                this.TypeChanged?.Invoke(this, this.Type); // Delegate
            }
        }
    }
}