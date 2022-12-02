using Luo_Painter.Options;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    internal sealed class ItemSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public override string ToString() => $"{this.Width} × {this.Height}";
    }

    public sealed partial class ToolListView : XamlListView
    {

        //@Content
        public OptionType SelectedType
        {
            get => (base.SelectedItem is OptionType item) ? item : OptionType.PaintBrush;
            set => base.SelectedIndex = this.Collection.IndexOf(value);
        }

        #region DependencyProperty

        /// <summary> Gets or set the size for <see cref="ToolListView"/>'s item. </summary>
        public int ItemSize
        {
            get => (int)base.GetValue(ItemSizeProperty);
            set => base.SetValue(ItemSizeProperty, value);
        }
        /// <summary> Identifies the <see cref = "ToolListView.ItemSize" /> dependency property. </summary>
        public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register(nameof(ItemSize), typeof(int), typeof(ToolListView), new PropertyMetadata(70));

        #endregion

        //@Construct
        public ToolListView()
        {
            this.InitializeComponent();
            this.SizeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is ItemSize item)
                {
                    this.ItemSize = item.Width;
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}