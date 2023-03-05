using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Elements
{
    [ContentProperty(Name = nameof(Child))]
    //[ContentProperty(Name = nameof(Source))]
    public sealed class SelectedImagePresenter : Grid // ContentPresenter
    {

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedImage.xaml.</summary>
        internal GridViewItem Ancestor;

        #region DependencyProperty 


        /// <summary> Gets or sets <see cref = "SelectedImagePresenter" />'s child. </summary>
        public UIElement Child
        {
            get => (UIElement)base.GetValue(ChildProperty);
            set => base.SetValue(ChildProperty, value);
        }
        /// <summary> Identifies the <see cref = "SelectedImagePresenter.Child" /> dependency property. </summary>
        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register(nameof(Child), typeof(UIElement), typeof(SelectedImagePresenter), new PropertyMetadata(null));
        

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