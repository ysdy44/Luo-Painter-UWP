using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Elements
{
    public sealed class SelectedIconPresenter : Grid // ContentPresenter
    {

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedIcon.xaml.</summary>
        internal ListViewItem Ancestor;

        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "SelectedIconPresenter" />'s IsItemClickEnabled. </summary>
        public bool IsItemClickEnabled
        {
            get => (bool)base.GetValue(IsItemClickEnabledProperty);
            set => base.SetValue(IsItemClickEnabledProperty, value);
        }
        /// <summary> Identifies the <see cref = "SelectedIconPresenter.IsItemClickEnabled" /> dependency property. </summary>
        public static readonly DependencyProperty IsItemClickEnabledProperty = DependencyProperty.Register(nameof(IsItemClickEnabled), typeof(bool), typeof(SelectedIconPresenter), new PropertyMetadata(true));

        public bool IsEnabled
        {
            get
            {
                if (this.Ancestor is null) return false;
                return this.Ancestor.IsEnabled;
            }
            set
            {
                if (this.Ancestor is null) return;
                this.Ancestor.IsEnabled = value;
            }
        }

        #endregion

        public SelectedIconPresenter()
        {
            //base.Unloaded += (s, e) =>
            //{
            //    this.Ancestor = null;
            //    this.Children.Clear();
            //};
            base.Loaded += (s, e) =>
            {
                this.Ancestor = this.FindAncestor<ListViewItem>();
                this.Children.Add(new SelectedIcon(this));
            };
        }
    }

    internal sealed partial class SelectedIcon : UserControl
    {

        //@Converter
        private Brush BooleanToBrushConverter(bool value) => value ? base.BorderBrush : base.Background;
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        readonly SelectedIconPresenter Presenter;

        internal SelectedIcon(SelectedIconPresenter presenter)
        {
            this.InitializeComponent();
            this.Presenter = presenter;
        }
    }
}