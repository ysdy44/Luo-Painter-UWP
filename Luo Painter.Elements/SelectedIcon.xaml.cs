using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Elements
{
    public sealed class SelectedIconPresenter : Grid // ContentPresenter
    {

        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "SelectedIconPresenter" />'s IsItemClickEnabled. </summary>
        public bool IsItemClickEnabled
        {
            get => (bool)base.GetValue(IsItemClickEnabledProperty);
            set => base.SetValue(IsItemClickEnabledProperty, value);
        }
        /// <summary> Identifies the <see cref = "SelectedIconPresenter.IsItemClickEnabled" /> dependency property. </summary>
        public static readonly DependencyProperty IsItemClickEnabledProperty = DependencyProperty.Register(nameof(IsItemClickEnabled), typeof(bool), typeof(SelectedIconPresenter), new PropertyMetadata(true));


        #endregion

        public SelectedIconPresenter()
        {
            // base.Unloaded += (s, e) => this.Children.Clear();
            base.Loaded += (s, e) => this.Children.Add(new SelectedIcon(this.FindAncestor<ListViewItem>(), this));
        }
    }

    internal sealed partial class SelectedIcon : UserControl
    {

        //@Converter
        private Brush BooleanToBrushConverter(bool value) => value ? base.BorderBrush : base.Background;
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedIcon.xaml.</summary>
        readonly ListViewItem Ancestor;
        readonly SelectedIconPresenter Presenter;

        internal SelectedIcon(ListViewItem ancestor, SelectedIconPresenter presenter)
        {
            this.InitializeComponent();
            this.Ancestor = ancestor;
            this.Presenter = presenter;
        }
    }
}