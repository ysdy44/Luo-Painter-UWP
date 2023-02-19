using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.Elements
{
    public sealed class SelectedImagePresenter : Grid // ContentPresenter
    {

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedImage.xaml.</summary>
        internal GridViewItem Ancestor;

        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "SelectedImagePresenter" />'s Source. </summary>
        public string Source
        {
            get => (string)base.GetValue(SourceProperty);
            set => base.SetValue(SourceProperty, value);
        }
        /// <summary> Identifies the <see cref = "SelectedImagePresenter.Source" /> dependency property. </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(string), typeof(SelectedImagePresenter), new PropertyMetadata(null));


        #endregion

        public SelectedImagePresenter()
        {
            //base.Unloaded += (s, e) =>
            //{
            //    this.Remove();
            //};
            base.Loaded += (s, e) =>
            {
                if (this.FindAncestor<GridViewItem>() is GridViewItem item)
                {
                    this.Add(item);
                }
                else
                {
                    this.Remove();
                }
            };
        }
        private void Remove()
        {
            this.Ancestor = null;
            this.Children.Clear();
        }
        private void Add(GridViewItem item)
        {
            this.Ancestor = item;
            this.Children.Add(new SelectedImage(this));
        }
    }

    internal sealed partial class SelectedImage : UserControl
    {
        //@Converter
        private double BooleanToVisibilityConverter(bool value) => value ? 0.6 : 1;

        readonly SelectedImagePresenter Presenter;

        internal SelectedImage(SelectedImagePresenter presenter)
        {
            this.InitializeComponent();
            this.Presenter = presenter;
        }
    }
}