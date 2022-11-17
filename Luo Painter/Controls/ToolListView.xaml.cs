using Luo_Painter.Options;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Luo_Painter.Controls
{
    public sealed partial class ToolListView : XamlListView
    {

        //@Converter
        private string S46Converter(int value) => value is 46 ? "✓" : string.Empty;
        private string S56Converter(int value) => value is 56 ? "✓" : string.Empty;
        private string S70Converter(int value) => value is 70 ? "✓" : string.Empty;
        private string S93Converter(int value) => value is 93 ? "✓" : string.Empty;
        private string S140Converter(int value) => value is 140 ? "✓" : string.Empty;

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
            this.S46Item.Click += (s, e) => this.ItemSize = 46;
            this.S56Item.Click += (s, e) => this.ItemSize = 56;
            this.S70Item.Click += (s, e) => this.ItemSize = 70;
            this.S93Item.Click += (s, e) => this.ItemSize = 93;
            this.S140Item.Click += (s, e) => this.ItemSize = 140;
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}